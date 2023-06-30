using System.Diagnostics;
using System.Xml;
using RimworldAnalyzer.Analysis;
using RimworldAnalyzer.Installation;

namespace RimworldAnalyzer.Analyzer;

public class RimworldAnalysisWriter {

	public RimworldAnalysisWriter(AnalyzerOptions options, AnalysisDatabase context) {
		_context = context;
		Options = options;
	}

	public AnalyzerOptions Options { get; }

	private readonly AnalysisDatabase _context;
	private DefinitionTable _definition = null!;
	private TagTable? _tagContext;
	private TagTable? _tagParent;

	private Task SaveChangesAsync()
		=> _context.SaveChangesAsync();

	#region Analysis Management

	/// <summary>
	/// Analyzes an installed official Rimworld module.
	/// </summary>
	/// <param name="module">The directory name of the module.</param>
	public Task AnalyzeOfficialModule(string module)
		=> AnalyzeModule(Rimworld.GetOfficialModule(module), true);

	/// <summary>
	/// Analyzes every official modules.
	/// </summary>
	public Task AnalyzeOfficialModules()
		=> Task.WhenAll(Rimworld.GetAvailableOfficialModules().Select(AnalyzeOfficialModule));

	/// <summary>
	/// Analyzes an installed Steam workshop mod.
	/// </summary>
	/// <param name="module">The identifier of the workship item.</param>
	public Task AnalyzeWorkshopModule(string module)
		=> AnalyzeModule(Rimworld.GetWorkshopModule(module), false);

	/// <summary>
	/// Analyzes every installed Steam workshop mod.
	/// </summary>
	public Task AnalyzeWorkshopModules()
		=> Task.WhenAll(Rimworld.GetAvailableWorkshopModules().Select(AnalyzeWorkshopModule));

	/// <summary>
	/// Analyzes an installed mod.
	/// </summary>
	/// <param name="module">The directory path of the mod.</param>
	public Task AnalyzeInstalledModule(string module)
		=> AnalyzeModule(Rimworld.GetInstalledModule(module), false);

	/// <summary>
	/// Analyzes all installed mods.
	/// </summary>
	public Task AnalyzeInstalledModules()
		=> Task.WhenAll(Rimworld.GetAvailableInstalledModules().Select(AnalyzeInstalledModule));

	public Task AnalyzeOtherModule(string path)
		=> AnalyzeModule(path, false);

	private async Task AnalyzeModule(string path, bool official) {
		string pathOfAbout = Path.Join(path, "About", "About.xml");

		if (!File.Exists(pathOfAbout))
			throw new($"Invalid Rimworld module: missing About/About.xml at '{path}'");

		XmlDocument about = new();

		using FileStream file = File.OpenRead(pathOfAbout);
		about.Load(file);

		XmlElement? metadata = about["ModMetaData"];
		string filename = Path.GetFileName(path);
		string? identifier = metadata?["packageId"]?.InnerText;
		string? name = metadata?["name"]?.InnerText;
		string? version = metadata?["modVersion"]?.InnerText;

		ModuleTable module = await _context.GetOrCreateModule(identifier ?? filename);
		module.Name = name ?? filename;
		module.Version = version;
		module.IsOfficial = official;
		await SaveChangesAsync();

		await AnalyzeModule(path, module);
	}

	#endregion

	#region Module Analysis

	private async Task AnalyzeModule(string path, ModuleTable module) {
		Debug.WriteLine($"Module {module.Name} v{module.Version} at '{path}'");
		Debug.Indent();

		// Traverse XML definition files
		string directory = Path.Join(path, "Defs");

		if (Directory.Exists(directory)) {
			string[] resources = Directory
				.EnumerateFiles(directory, "*.xml", SearchOption.AllDirectories)
				.Order()
				.ToArray();

			Debug.WriteLine($"Parsing {resources.Length} definition file(s)");

			foreach (string resource in resources)
				await AnalyzeResourceXML(resource, directory, module);
		}

		//Debug.WriteLine($"Completed analysis of {module.Name} with {_errors.Count} errors and {_warnings.Count} warnings");
		//Terminal.Information($"Analyzed {_tags.Count} tags across {_definitions.Count} definitions of {_classes.Count} types within {_modules.Count} modules");

		await SaveChangesAsync();
		Debug.Unindent();
	}

	#endregion

	#region Resource Analysis

	private async Task AnalyzeResourceXML(string path, string directory, ModuleTable module) {
		string filepath = Path.GetRelativePath(directory, path);
		string location = Path.Join(module.Identifier, filepath);

		XmlDocument document = new();

		try {
			using FileStream file = File.OpenRead(path);
			document.Load(file);
		} catch (Exception e) {
			_context.Error(location, e.Message);
			return;
		}

		// Create the resource
		ResourceTable resource = await _context.GetOrCreateResource(filepath, module);

		// Ignore non-defs files
		if (document.DocumentElement is not XmlElement root || root.Name is not "Defs") {
			_context.Warning(location, "Skipping invalid XML definition file").Resource = resource;
			return;
		}

		// Log the resource
		Debug.WriteLine($"==> {location}");
		Debug.Indent();

		// Traverse the document
		foreach (XmlElement node in root.OfType<XmlElement>())
			await AnalyzeDefinition(node, module, resource);

		await SaveChangesAsync();
		Debug.Unindent();
	}

	#endregion

	#region Definition Analysis

	private async Task AnalyzeDefinition(XmlElement node, ModuleTable module, ResourceTable resource) {
		// Retrieve information about the class
		ClassTable @class = await _context.GetOrCreateClass(node.Name);

		// Gather subclassing information
		string? _parent = node.HasAttribute("ParentName") ? node.GetAttribute("ParentName") : null;
		DefinitionTable? parent = _parent is null ? null : await _context.GetOrCreateDefinition(_parent);
		bool isAbstract = node.HasAttribute("Abstract") && node.GetAttribute("Abstract") is "True";

		// Retrieve the name of the definition
		string? name = null;
		if (node.HasAttribute("Name"))
			name = node.GetAttribute("Name");
		else if (node.HasChildNodes)
			foreach (XmlNode child in node.ChildNodes)
				if (child is XmlElement element && element.Name is "defName") {
					name = element.InnerText;
					break;
				}

		if (name is null) {
			_context.Error(Path.Join(module.Identifier, node.Name, name), $"Could not find name for {@class.Name} definition").Resource = resource;
			return;
		}

		// Update the definition
		_definition = await _context.GetOrCreateDefinition(name);
		_definition.ModuleId = module.Id;
		_definition.Module = module;
		_definition.ClassId = @class.Id;
		_definition.Class = @class;
		_definition.ResourceId = resource.Id;
		_definition.Resource = resource;
		_definition.IsAbstract = isAbstract;
		_definition.ParentId = parent?.Id;
		_definition.Parent = parent;

		// Log the definition
		Debug.WriteLine(_definition.ToDeclarationString());
		Debug.Indent();

		// Analyze the definition's tag without a parent
		_tagContext = _tagParent = null!;
		await AnalyzeTag(node, module, resource);

		Debug.Unindent();
	}

	#endregion

	#region Tag Analysis

	private async Task AnalyzeTag(XmlElement node, ModuleTable module, ResourceTable resource) {
		// Collection information on the tag
		string? rawValue = node.ChildNodes.Count is 1 && node.FirstChild is XmlText text ? text.InnerText : null;
		TagBehaviour behaviour = Options.BehaviourOfTag(node.Name);

		// Log the tag
		string log = Path.Join(module.Identifier, resource.Path, node.Name);
		if (rawValue is not null)
			log += @$" ""{rawValue}""";
		Debug.WriteLine(log);

		// Update the tag
		TagTable tag = await _context.GetOrCreateTag(node.Name);

		// Configure relationships
		TagTable? tagContext = _tagContext;
		TagTable? parent = _tagParent;
		if (_tagParent is not null)
			await _context.GetOrCreateRelationship(_tagParent, tag, _tagContext);
		_tagContext = _tagParent;
		_tagParent = tag;

		if (behaviour.HasFlag(TagBehaviour.Attributes))
			await Task.WhenAll(node.Attributes.OfType<XmlAttribute>().Select(AnalyzeAttribute));

		//TODO: Tags keep array-ing near-infinitely, investigate that and possibly remove?

		// If the tag is transient, the context is used as the parent
		//if (behaviour.HasFlag(TagBehaviour.Transient) && parent is not null) {
		//	TagTable array = await _context.GetOrCreateTag(parent.Identifier + "[]");
		//	await _context.GetOrCreateRelationship(parent, array, tagContext);
		//	_tagParent = array;
		//}

		if (behaviour.HasFlag(TagBehaviour.CollectExamples) && rawValue is not null) {
			ExampleTable value = await _context.GetOrCreateExample(rawValue);
			TagExampleTable example = await _context.GetOrCreateTagExample(tag, parent!, value);
			await _context.GetOrCreateTagUsage(_definition, example);
		}

		if (behaviour.HasFlag(TagBehaviour.Traverse))
			await Task.WhenAll(node.OfType<XmlElement>().Select(node => AnalyzeTag(node, module, resource)));
	}

	private async Task AnalyzeAttribute(XmlAttribute node) {
		Debug.WriteLine(@$"@{node.Name} ""{node.Value}""");

		AttributeBehaviour behaviour = Options.BehaviourOfAttribute(node.Name);
		AttributeTable attribute = await _context.GetOrCreateAttribute(node.Name);

		if (behaviour.HasFlag(AttributeBehaviour.CollectExamples)) {
			ExampleTable value = await _context.GetOrCreateExample(node.Value);
			AttributeExampleTable example = await _context.GetOrCreateAttributeExample(attribute, _tagParent!, value);
			await _context.GetOrCreateAttributeUsage(_definition, example);
		}
	}

	#endregion

}