using System.Xml;
using System.Xml.Linq;

namespace RimworldExplorer.Analysis;

public class InMemoryAnalyzer {

	public InMemoryAnalyzer(string title, string version) {
		Title = title;
		Version = version;
	}

	public string Title { get; init; }
	public string Version { get; init; }

	private readonly HashSet<AnalysisModule> _modules = new();
	private readonly Dictionary<string, AnalysisTag> _tags = new();
	private readonly Dictionary<string, AnalysisDefinition> _definitions = new();
	private readonly Dictionary<string, AnalysisClass> _classes = new();

	private readonly HashSet<string> _warnings = new();
	private readonly HashSet<string> _errors = new();

	public AnalysisReport Report => new() {
		Title = Title,
		Version = Version,

		Modules = _modules
			.OrderBy(module => module.Identifier)
			.ToArray(),
		Tags = _tags.Values
			.OrderBy(tag => tag.Name)
			.ToArray(),
		Definitions = _definitions.Values
			.OrderBy(definition => definition.Name)
			.ToArray(),
		Classes = _classes.Values
			.OrderBy(@class => @class.Name)
			.ToArray(),

		Warnings = _warnings,
		Errors = _errors,
	};

	#region Configuration

	public readonly HashSet<string> IgnoredTags = new();
	public readonly HashSet<string> ArrayElementTags = new();

	public void AddSearchPath(string path) {
		_modules.Add(new(path));
	}

	public void AddSearchPaths(IEnumerable<string> paths) {
		foreach (string path in paths)
			_modules.Add(new(path));
	}

	public void Clear() {
		_errors.Clear();
		_warnings.Clear();
	}

	#endregion

	#region Definition Analysis

	public AnalysisReport Analyze() {
		AnalyzeDefinitions();
		return Report;
	}

	private void AnalyzeDefinitions() {
		Terminal.Milestone($"Finding definition files across {_modules.Count} modules...");

		int documents = 0;
		foreach (AnalysisModule module in _modules)
			documents += module.EnumerateDefinitionFiles().Count();

		Terminal.Start($"Parsing {documents} definitions files across {_modules.Count} modules", documents);

		foreach (AnalysisModule module in _modules)
			foreach (string filepath in module.EnumerateDefinitionFiles()) {
				Terminal.Progress(Path.Combine(module.Identifier, module.GetRelativeDefinitionPath(filepath)));
				ParseDefinitionsFile(filepath, module);
			}

		Terminal.Complete($"Completed analysis with {_errors.Count} errors and {_warnings.Count} warnings");
		Terminal.Information($"Analyzed {_tags.Count} tags across {_definitions.Count} definitions of {_classes.Count} types within {_modules.Count} modules");

		Console.ForegroundColor = ConsoleColor.Red;
		foreach (string error in _errors)
			Console.WriteLine(error.TrimEnd());

		Console.ForegroundColor = ConsoleColor.Yellow;
		foreach (string warning in _warnings)
			Console.WriteLine(warning.TrimEnd());

		Console.ResetColor();
	}

	private void ParseDefinitionsFile(string filepath, AnalysisModule module) {
		XmlDocument document = new();

		try {
			using FileStream file = File.OpenRead(filepath);
			document.Load(file);
		} catch (Exception e) {
			_errors.Add($"{module.GetDefinitionHeader(filepath)} {e.Message}");
			return;
		}

		// Ignore non-defs files
		if (document.DocumentElement is not XmlElement root || root.Name is not "Defs") {
			_warnings.Add($"{module.GetDefinitionHeader(filepath)} Skipping invalid definition file");
			return;
		}

		// Parse each defs in the document
		foreach (XmlNode node in root) {
			if (node is not XmlElement definition)
				continue;

			// Record the type if unknown
			if (!_classes.TryGetValue(definition.Name, out AnalysisClass? type))
				_classes[definition.Name] = type = new(definition.Name);

			// Retrieve the name of the definition
			string? name = null;
			if (definition.HasAttribute("Name"))
				name = definition.GetAttribute("Name");
			else if (definition.HasChildNodes)
				foreach (XmlNode child in definition.ChildNodes)
					if (child is XmlElement element && element.Name is "defName") {
						name = element.InnerText;
						break;
					}

			if (name is null) {
				_warnings.Add($"{module.GetDefinitionHeader(filepath)} Could not retrieve name for {type.Name} definition");
				continue;
			}

			// Store the definition
			string? parent = definition.HasAttribute("ParentName") ? definition.GetAttribute("ParentName") : null;
			bool isAbstract = definition.HasAttribute("Abstract") ? definition.GetAttribute("Abstract") is "True" : false;
			AnalysisDefinition def = _definitions[name] = new(name, filepath, module, type, parent, isAbstract);

			ExploreDefinition(definition, def, null);
		}
	}

	private void ExploreDefinition(XmlElement node, AnalysisDefinition definition, AnalysisTag? parent) {
		if (IgnoredTags.Contains(node.Name))
			return;

		if (ArrayElementTags.Contains(node.Name)) {
			ExploreArrayDefinition(node, definition, parent);
			return;
		}

		AnalysisTag tag = GetOrCreateTag(node.Name);

		if (parent is not null)
			tag.AddParent(parent);

		VisitNode(node, tag, definition, parent);
	}

	private void ExploreArrayDefinition(XmlElement node, AnalysisDefinition definition, AnalysisTag? parent) {
		if (parent is null) {
			_warnings.Add($"[{definition.Location}] Failed to process list item in {definition.Declaration}");
			return;
		}

		// Create the element tag
		AnalysisTag list = GetOrCreateTag(parent.Name + "[]");
		list.AddParent(parent);

		VisitNode(node, list, definition, parent);
	}

	#endregion

	#region Utility Methods

	private AnalysisTag GetOrCreateTag(string name) {
		if (!_tags.TryGetValue(name, out AnalysisTag? list))
			list = _tags[name] = new(name, null);
		return list;
	}

	private void VisitNode(XmlNode node, AnalysisTag tag, AnalysisDefinition definition, AnalysisTag? parent) {
		if (!node.HasChildNodes)
			return;

		// Check if the node contains inner text and nothing else
		if (node.ChildNodes.Count is 1 && node.FirstChild is XmlText value) {
			tag.AddUsage(value.InnerText, parent, definition);
			return;
		}

		// Visit the node's children
		foreach (XmlNode child in node.ChildNodes)
			if (child is XmlElement element)
				ExploreDefinition(element, definition, tag);
	}

	#endregion

}