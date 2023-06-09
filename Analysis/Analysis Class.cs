namespace RimworldExplorer.Analysis;

public sealed class AnalysisClass {

	public AnalysisClass(string identifier, AnalysisModule? module = null) {
		_identifier = identifier;
		_module = module;
	}

	/// <summary>
	/// The XML and C# class name of the type
	/// </summary>
	private readonly string _identifier;

	/// <summary>
	/// The module in which the type was declared
	/// </summary>
	private readonly AnalysisModule? _module;

	public string Name => _identifier;

}
