namespace RimworldExtractor.Analysis;

public sealed class AnalysisDefinition {

	public AnalysisDefinition(string identifier, string path, AnalysisModule module, AnalysisClass type, string? parent, bool isAbstract) {
		_identifier = identifier.Trim();
		_module = module;
		_module.AddDefinition(this);
		_path = module.GetRelativeDefinitionPath(path);
		_class = type;

		_parent = parent;
		_abstract = isAbstract;
	}

	private readonly string _identifier;
	private readonly string _path;
	private readonly AnalysisModule _module;
	private readonly AnalysisClass _class;

	private readonly string? _parent;
	private readonly bool _abstract;

	public AnalysisModule Module => _module;
	public string Filepath => _path;

	public bool IsAbstract => _abstract;
	public AnalysisClass Class => _class;
	public string Name => _identifier;
	public string? Parent => _parent;

	public string TypedIdentifier => $"{_class.Name} {_identifier}";

	public string Declaration {
		get {
			string declaration = TypedIdentifier;
			if (_abstract)
				declaration = "abstract " + declaration;
			if (_parent is not null)
				declaration = declaration + " : " + _parent;
			return declaration;
		}
	}

	public string Location => Path.Combine(_module.Identifier, _path);

	public override string ToString()
		=> $"[{Path.Combine(_module.Identifier, _path)}] {Declaration}";

}
