namespace RimworldExtractor.Analysis;

public readonly struct AnalysisReport {

	public string Title { get; init; }
	public string Version { get; init; }

	public AnalysisModule[] Modules { get; init; }

	public AnalysisClass[] Classes { get; init; }

	public AnalysisTag[] Tags { get; init; }

	public AnalysisDefinition[] Definitions { get; init; }

	public IReadOnlyList<string> Warnings { get; init; }
	public IReadOnlyList<string> Errors { get; init; }

	public int IndexOf(AnalysisClass @class)
		=> Array.IndexOf(Classes, @class);

	public int IndexOf(AnalysisTag tag)
		=> Array.IndexOf(Tags, tag);

}
