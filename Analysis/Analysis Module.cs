namespace RimworldExtractor.Analysis;

public sealed class AnalysisModule {

	public AnalysisModule(string path) {
		_path = path;
	}

	private readonly string _path;

	public string Identifier => Path.GetFileName(_path) ?? _path;

	#region Definition Methods

	public readonly HashSet<AnalysisDefinition> _definitions = new();
	public IReadOnlyCollection<AnalysisDefinition> Definitions => _definitions;

	public void AddDefinition(AnalysisDefinition definition)
		=> _definitions.Add(definition);

	public string GetRelativeDefinitionPath(string path)
		=> Path.GetRelativePath(Path.Combine(_path, "Defs"), path);

	public string GetDefinitionHeader(string filepath)
		=> $"[{Identifier} {GetRelativeDefinitionPath(filepath)}]";

	#endregion

	#region Utility Methods

	public string GetRelativePath(string path)
		=> Path.GetRelativePath(_path, path);

	public string GetHeader(string filepath)
		=> $"[{Identifier} {GetRelativeDefinitionPath(filepath)}]";

	public IEnumerable<string> EnumerateDefinitionFiles()
		=> Directory.EnumerateFiles(_path, "Defs/*.xml", SearchOption.AllDirectories);

	#endregion

}
