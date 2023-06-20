namespace RimworldExtractor.Analysis;

public sealed class AnalysisTag {

	public AnalysisTag(string identifier, AnalysisModule? module) {
		_identifier = identifier.Trim();
		_module = module;

		Title = identifier.ToTitleCase();
		Title = RegexLibrary.DetectDefinitionAbbreviation().Replace(Title, "Definition");
		Title = RegexLibrary.DetectTextureAbbreviation().Replace(Title, "Texture");
	}

	private readonly string _identifier;
	private readonly AnalysisModule? _module;

	public string Title { get; }

	public string Name => _identifier;

	#region Relation Methods

	private HashSet<AnalysisTag> _parents = new();
	private HashSet<AnalysisTag> _children = new();

	public IReadOnlyCollection<AnalysisTag> Parents => _parents;
	public IEnumerable<AnalysisTag> OrderedParents => _parents.OrderBy(parent => parent.Name);

	public IReadOnlyCollection<AnalysisTag> Children => _children;
	public IEnumerable<AnalysisTag> OrderedChildren => _children.OrderBy(parent => parent.Name);

	public void AddParent(AnalysisTag parent) {
		_parents.Add(parent);
		parent._children.Add(this);
	}

	public void AddChild(AnalysisTag child)
		=> child.AddParent(this);

	#endregion

	#region Usage Methods

	private readonly HashSet<PropertyUsage> _uses = new();

	public IReadOnlyCollection<PropertyUsage> Uses => _uses;

	public IEnumerable<PropertyUsage> OrderedUses => _uses
		.OrderBy(usage => usage.Value)
		.ThenBy(usage => usage.Definition.TypedIdentifier);

	public IEnumerable<PropertyUsage> ExampleUses => ExamplesOf(_uses);

	public ILookup<string, PropertyUsage> UsageByValue => _uses
		.ToLookup(usage => usage.Value);

	public ILookup<AnalysisTag, PropertyUsage> UsageByParent => _uses
		.ToLookup(usage => usage.Parent);

	public ILookup<AnalysisDefinition, PropertyUsage> UsageByDefinition => _uses
		.ToLookup(usage => usage.Definition);

	public void AddUsage(string value, AnalysisTag parent, AnalysisDefinition definition) {
		_uses.Add(new() {
			Value = value,
			Parent = parent,
			Definition = definition,
		});
	}

	public static IEnumerable<PropertyUsage> ExamplesOf(IEnumerable<PropertyUsage> uses)
		=> uses
		.DistinctBy(usage => (usage.Value, usage.Parent))
		.OrderBy(usage => usage.Value);

	#endregion

	public override string ToString()
		=> _identifier;

}

public record struct PropertyUsage {
	public required string Value { get; init; }
	public required AnalysisTag Parent { get; init; }
	public required AnalysisDefinition Definition { get; init; }
}
