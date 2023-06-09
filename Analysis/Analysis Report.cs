namespace RimworldExplorer.Analysis;

public readonly struct AnalysisReport {

	public string Title { get; init; }
	public string Version { get; init; }

	public IReadOnlyCollection<AnalysisModule> Modules { get; init; }
	public IEnumerable<AnalysisModule> OrderedModules => Modules.OrderBy(module => module.Identifier);

	public IReadOnlyCollection<AnalysisClass> Classes { get; init; }
	public IEnumerable<AnalysisClass> OrderedClasses => Classes.OrderBy(type => type.Name);

	public IReadOnlyCollection<AnalysisTag> Tags { get; init; }
	public IEnumerable<AnalysisTag> OrderedTags => Tags.OrderBy(tag => tag.Name);

	public IReadOnlyCollection<AnalysisDefinition> Definitions { get; init; }
	public IEnumerable<AnalysisDefinition> OrderedDefinitions => Definitions.OrderBy(definition => definition.Class.Name).ThenBy(definition => definition.Name);

	public IReadOnlyCollection<string> Warnings { get; init; }
	public IReadOnlyCollection<string> Errors { get; init; }

}

