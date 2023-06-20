using System.CommandLine;
using System.CommandLine.Binding;
using RimworldExtractor.Analysis;

namespace RimworldExtractor.Commands;

public abstract class AnalysisCommand : Command {

	protected AnalysisCommand(string name)
		: base(name) {

		// Specify included definitions
		AddOption(IncludeRimworldOption);
		AddOption(IncludeExpansionOption);
		AddOption(IncludeExpansionsOption);
		AddOption(IncludeOption);

		// Customize analysis
		AddOption(IgnoreTagOption);

		// Provide non-standard configuration
		AddOption(RimworldOption);

		ConfigureHandler(new AnalysisParameterPackBinding());
	}

	protected abstract void ConfigureHandler(AnalysisParameterPackBinding parameters);

	#region Options Management

	public static readonly Option<DirectoryInfo?> RimworldOption = new("--rimworld", "Specify a non-standard installation directory");

	public static readonly Option<bool> IncludeRimworldOption = new("--include-rimworld", "Include the Core definitions in the report");
	public static readonly Option<string[]?> IncludeExpansionOption = new("--include-expansion", "Include an official expansion in the report");
	public static readonly Option<bool> IncludeExpansionsOption = new("--include-expansions", "Includes all available official expansion in the report");
	public static readonly Option<DirectoryInfo[]?> IncludeOption = new("--include", "Include a definition directory in the report");

	public static readonly Option<string[]?> IgnoreTagOption = new("--ignore", "Ignore certain tags, excluding them from the report");

	public static Option<FileInfo> CreateOutputFileOption(string format, string extension) {
		Option<FileInfo> option = new("--output", $"The name of the generated {format} file");
		option.SetDefaultValueFactory(() => new FileInfo($"analysis.{extension}"));
		return option;
	}

	public static Option<DirectoryInfo> CreateOutputDirectoryOption(string format) {
		Option<DirectoryInfo> option = new("--output", $"The name of the generated {format} directory");
		option.SetDefaultValueFactory(() => new DirectoryInfo($"analysis"));
		return option;
	}

	#endregion

	public static void Announce() {
		Console.WriteLine("Created by Lancelotbronner. Based on modification by Epicguru. Based on original by milon.");
		Console.WriteLine();
	}

	public static void Analyze(in AnalysisParameterPack parameters, out AnalysisReport report) {
		HashSet<string> searchPaths = new();
		string version = "unknown";

		// Validate RimWorld installation if needed

		if (parameters.IsRimWorldInstallationRequired) {
			Rimworld.Installation = parameters.Rimworld ?? Rimworld.Installation;

			if (!Rimworld.IsInstalled)
				throw new InvalidOperationException("Specified Rimworld installation directory does not exist");

			Console.WriteLine($"Using Rimworld installation at {Rimworld.Installation}");

			// Document the available official modules
			string[] modules = Rimworld
				.GetAvailableModules()
				.Select(module => Path.GetFileName(module) ?? module)
				.ToArray();
			Console.WriteLine($"Found {modules.Length} available official modules; {string.Join(',', modules)}");

			Console.WriteLine();
		}

		// Insert RimWorld Core module if requested

		if (parameters.ShouldIncludeRimworld) {
			string module = Rimworld.GetModule("Core");

			// Retrieve module metadata
			string versionPath = Path.Combine(Rimworld.Installation.FullName, "Version.txt");
			if (File.Exists(versionPath)) {
				version = File.ReadAllText(versionPath).Trim();
				Console.WriteLine($"Including Core definitions of Rimworld {version}");
			} else
				Console.WriteLine($"Unable to retrieve Rimworld version; could not find {versionPath}");

			//TODO: Instead add a module using Name/Version/Path
			searchPaths.Add(module);
		}

		// Insert expansions if requested

		void IncludeExpansion(string path, string name) {
			if (name is "Core")
				return;

			//TODO: Instead add a module using Name/Version/Path
			searchPaths.Add(path);
			Console.WriteLine($"Including the {name} expansion");
		}

		foreach (string expansion in parameters.Expansions) {
			string path = Rimworld.GetModule(expansion);

			if (!Directory.Exists(path)) {
				Console.WriteLine($"Could not locate expansion at {path}");
				continue;
			}

			IncludeExpansion(path, expansion);
		}

		if (parameters.ShouldIncludeAvailableExpansions)
			foreach (string path in Rimworld.GetAvailableModules())
				IncludeExpansion(path, Path.GetFileName(path) ?? path);

		// Insert includes

		foreach (DirectoryInfo path in parameters.Includes) {
			if (!path.Exists) {
				Console.WriteLine($"Could not locate included path {path}");
				continue;
			}

			searchPaths.Add(path.FullName);
		}

		// Resolve search paths

		string[] paths = searchPaths.ToArray();

		Console.WriteLine($"Using definitions from {paths.Length} directories");
		foreach (ref string path in paths.AsSpan()) {
			path = Path.GetFullPath(path);
			Console.WriteLine($"  {path}");
		}

		Console.WriteLine();

		// Configure the analyzer

		InMemoryAnalyzer analyzer = new("RimWorld", version);
		analyzer.AddSearchPaths(paths);

		analyzer.ArrayElementTags.Add("li");

		foreach (string ignored in parameters.IgnoredTags)
			analyzer.IgnoredTags.Add(ignored);

		// Execute analysis

		report = analyzer.Analyze();
	}

}

public sealed class AnalysisParameterPackBinding : BinderBase<AnalysisParameterPack> {
	protected override AnalysisParameterPack GetBoundValue(BindingContext context) {
		bool includeRimworld = context.ParseResult.GetValueForOption(AnalysisCommand.IncludeRimworldOption);
		string[]? includedExpansions = context.ParseResult.GetValueForOption(AnalysisCommand.IncludeExpansionOption);
		bool includeExpansions = context.ParseResult.GetValueForOption(AnalysisCommand.IncludeExpansionsOption);
		DirectoryInfo[]? includes = context.ParseResult.GetValueForOption(AnalysisCommand.IncludeOption);

		string[]? ignored = context.ParseResult.GetValueForOption(AnalysisCommand.IgnoreTagOption);

		DirectoryInfo? rimworld = context.ParseResult.GetValueForOption(AnalysisCommand.RimworldOption);

		return new() {
			ShouldIncludeRimworld = includeRimworld,
			Expansions = includedExpansions ?? Array.Empty<string>(),
			ShouldIncludeAvailableExpansions = includeExpansions,
			Includes = includes ?? Array.Empty<DirectoryInfo>(),
			IgnoredTags = ignored ?? Array.Empty<string>(),
			Rimworld = rimworld,
		};
	}
}

public struct AnalysisParameterPack {
	public bool ShouldIncludeRimworld { get; init; }
	public IReadOnlyCollection<string> Expansions { get; init; }
	public bool ShouldIncludeAvailableExpansions { get; init; }
	public IReadOnlyCollection<DirectoryInfo> Includes { get; init; }

	public IReadOnlyCollection<string> IgnoredTags { get; init; }

	public DirectoryInfo? Rimworld { get; init; }

	public bool IsRimWorldInstallationRequired => ShouldIncludeRimworld || ShouldIncludeAvailableExpansions || Expansions.Count > 0;
}
