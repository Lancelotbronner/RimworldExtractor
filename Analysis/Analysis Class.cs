namespace RimworldExtractor.Analysis;

public sealed class AnalysisClass {

	public AnalysisClass(string identifier, AnalysisModule? module = null) {
		_identifier = identifier;
		_module = module;

		Title = identifier.ToTitleCase();
		Title = RegexLibrary.DetectDefinitionAbbreviation().Replace(Title, "Definition");
	}

	/// <summary>
	/// The XML and C# class name of the type
	/// </summary>
	private readonly string _identifier;

	public string Title { get; }

	/// <summary>
	/// The module in which the type was declared
	/// </summary>
	private readonly AnalysisModule? _module;

	public string Name => _identifier;

}
