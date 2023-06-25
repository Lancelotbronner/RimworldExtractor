using System.CommandLine;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using RimworldAnalyzer.Analysis;
using RimworldAnalyzer.Analyzer;
using RimworldAnalyzer.Installation;
using RimworldAnalyzer.Parameters;

namespace RimworldExtractor.Commands;

public sealed class AnalyzeCommand : Command {

	public AnalyzeCommand() : base("analyze") {
		Description = "Analyze the Rimworld installation or individual modules";

		// Specify included modules
		AddOption(IncludeOfficialOption);
		AddOption(IncludeOfficialsOption);
		AddOption(IncludeWorkshopOption);
		AddOption(IncludeWorkshopsOption);
		AddOption(IncludeInstalledOption);
		AddOption(IncludeModsOption);
		AddOption(IncludeOption);

		// Customize tag analysis
		//TODO: Add all the behaviour options

		// Provide non-standard configuration
		AddOption(RimworldOption);
		AddOption(OutputOption);
		AddOption(VerboseOption);

		this.SetHandler(Handle, new AnalysisParameterBinding(), VerboseOption, OutputOption);
	}

	public static async Task Handle(AnalysisParameters parameters, bool verbose, FileInfo output) {
		if (verbose)
			Trace.Listeners.Add(new ConsoleTraceListener());

		if (!parameters.IsAnythingQueued) {
			Console.WriteLine("Nothing was included, there is nothing to do!");
			Console.WriteLine("Use `help analyze` to see available options.");
			return;
		}

		string filename = output.Extension.Length is 0 ? output.Name + ".db" : output.Name;
		DbContextOptions<AnalysisDatabase> dboptions = new DbContextOptionsBuilder<AnalysisDatabase>()
			.UseSqlite($"Data Source={filename};Foreign Keys=False")
			.Options;

		if (parameters.IsRimWorldInstallationRequired) {
			Rimworld.Installation = parameters.Rimworld ?? Rimworld.Installation;

			if (!Rimworld.IsInstalled)
				throw new InvalidOperationException("Could not retrieve Rimworld installation");

			Console.WriteLine($"Using Rimworld installation at {Rimworld.Installation}");
		}

		//TODO: Check Steam installation?

		// Ensure the database is ready for analysis
		AnalysisDatabase context = new(dboptions);
		await context.Database.EnsureCreatedAsync();

		Analyzer analyzer = new(parameters.Options, context);

		Task official = Task.WhenAll(parameters.OfficialModules.Select(analyzer.AnalyzeOfficialModule));
		Task workshop = Task.WhenAll(parameters.WorkshopModules.Select(analyzer.AnalyzeWorkshopModule));
		Task installed = Task.WhenAll(parameters.InstalledModules.Select(analyzer.AnalyzeInstalledModule));
		Task other = Task.WhenAll(parameters.Modules.Select(module => analyzer.AnalyzeOtherModule(module.FullName)));

		await Task.WhenAll(official, workshop, installed, other);
	}

	#region Options Management

	public static readonly Option<bool> VerboseOption = new("--verbose", "Display additional information as the analyzer is going through everything");
	public static readonly Option<FileInfo> OutputOption = CreateOutputFileOption("database", "sqlite");

	public static readonly Option<DirectoryInfo?> RimworldOption = new("--rimworld", "Specify a non-standard installation directory");

	public static readonly Option<string[]?> IncludeOfficialOption = new("--include-official", "Include an official module such as Core or an expansion");
	public static readonly Option<bool> IncludeOfficialsOption = new("--include-officials", "Includes all official modules");
	public static readonly Option<string[]?> IncludeWorkshopOption = new("--include-workshop", "Include a workshop module");
	public static readonly Option<bool> IncludeWorkshopsOption = new("--include-workshops", "Includes all workshop modules");
	public static readonly Option<string[]?> IncludeInstalledOption = new("--include-mod", "Includes an installed module");
	public static readonly Option<bool> IncludeModsOption = new("--include-mods", "Includes all installed modules");
	public static readonly Option<DirectoryInfo[]?> IncludeOption = new("--include", "Include a module");

	public static readonly Option<string[]?> TagsTraversalEnableOption = new("--tag-traversal-enable", "Enables traversal for specific tags");
	public static readonly Option<string[]?> TagsTraversalDisableOption = new("--tag-traversal-disable", "Disables traversal for specific tags");
	public static readonly Option<bool?> TagsTraversalDefaultOption = new("--tag-traversal-default", "Wether or not to traverse tags and visit their children by default");

	public static readonly Option<string[]?> TagsAttributesEnableOption = new("--tag-attributes-enable", "Enables attributes for specific tags");
	public static readonly Option<string[]?> TagsAttributesDisableOption = new("--tag-attributes-disable", "Disables attributes for specific tags");
	public static readonly Option<bool?> TagsAttributesDefaultOption = new("--tag-attributes-default", "Wether or not to analyze attributes on tags by default");

	public static readonly Option<string[]?> TagsExamplesEnableOption = new("--tag-examples-enable", "Enables examples for specific tags");
	public static readonly Option<string[]?> TagsExamplesDisableOption = new("--tag-examples-disable", "Disables examples for specific tags");
	public static readonly Option<bool?> TagsExamplesDefaultOption = new("--tag-examples-default", "Wether or not to collect examples of the tag by default");

	public static readonly Option<string[]?> TagsElementEnableOption = new("--tag-element-enable", "Marks tags as array elements");
	public static readonly Option<string[]?> TagsElementDisableOption = new("--tag-element-disable", "Unmarks tags as array element");
	public static readonly Option<bool?> TagsElementDefaultOption = new("--tag-element-default", "Wether or not tags are array element by default");

	public static readonly Option<string[]?> AttributesExamplesEnableOption = new("--attribute-examples-enable", "Enables examples for specific attributes");
	public static readonly Option<string[]?> AttributesExamplesDisableOption = new("--attribute-examples-disable", "Disables examples for specific attributes");
	public static readonly Option<bool?> AttributesExamplesDefaultOption = new("--attribute-examples-default", "Wether or not to collect examples of the attribute by default");

	public static Option<FileInfo> CreateOutputFileOption(string format, string extension) {
		Option<FileInfo> option = new("--output", $"The name of the generated {format} file");
		option.SetDefaultValueFactory(() => new FileInfo($"analysis.{extension}"));
		return option;
	}

	#endregion

}
