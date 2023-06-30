using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using RimworldAnalyzer.Analyzer;
using RimworldAnalyzer.Commands;
using RimworldAnalyzer.Installation;

namespace RimworldAnalyzer.Parameters;

public readonly struct AnalysisParameters {

	public DirectoryInfo? Rimworld { get; init; }

	public string[] OfficialModules { get; init; }
	public string[] WorkshopModules { get; init; }
	public string[] InstalledModules { get; init; }
	public DirectoryInfo[] Modules { get; init; }

	public AnalyzerOptions Options { get; init; }

	public bool IsAnythingQueued => OfficialModules.Length is not 0 || WorkshopModules.Length is not 0 || InstalledModules.Length is not 0 || Modules.Length is not 0;
	public bool IsRimWorldInstallationRequired => OfficialModules is not null || InstalledModules is not null;
	public bool IsSteamInstallationRequired => WorkshopModules is not null;

}

public sealed class AnalysisParameterBinding : BinderBase<AnalysisParameters> {

	protected override AnalysisParameters GetBoundValue(BindingContext context)
		=> ProcessParameters(context.ParseResult);

	private static AnalysisParameters ProcessParameters(ParseResult parser) => new() {
		Rimworld = parser.GetValueForOption(AnalyzeCommand.RimworldOption),
		Options = ProcessAnalyzerOption(parser),

		OfficialModules = ProcessModules(parser, AnalyzeCommand.IncludeOfficialOption, AnalyzeCommand.IncludeOfficialsOption, Rimworld.GetAvailableOfficialModules),
		WorkshopModules = ProcessModules(parser, AnalyzeCommand.IncludeWorkshopOption, AnalyzeCommand.IncludeWorkshopsOption, Rimworld.GetAvailableWorkshopModules),
		InstalledModules = ProcessModules(parser, AnalyzeCommand.IncludeInstalledOption, AnalyzeCommand.IncludeModsOption, Rimworld.GetAvailableInstalledModules),
		Modules = parser.GetValueForOption(AnalyzeCommand.IncludeOption) ?? Array.Empty<DirectoryInfo>(),

	};

	private static string[] ProcessModules(ParseResult parser, Option<string[]?> list, Option<bool> everything, Func<IEnumerable<string>> fetch) {
		HashSet<string> modules = new();
		bool isEverythingIncluded = parser.GetValueForOption(everything);
		string[]? includes = parser.GetValueForOption(list);

		if (isEverythingIncluded)
			foreach (string include in fetch.Invoke())
				modules.Add(include);

		if (includes is not null)
			foreach (string include in includes)
				if (isEverythingIncluded)
					modules.Remove(include);
				else
					modules.Add(include);

		return modules.ToArray();
	}

	private static AnalyzerOptions ProcessAnalyzerOption(ParseResult parser) {
		AnalyzerOptions options = new();

		if (parser.GetValueForOption(AnalyzeCommand.TagsTraversalDefaultOption) is bool tmp1)
			options.SetDefaultBehaviour(TagBehaviour.Traverse, tmp1);
		if (parser.GetValueForOption(AnalyzeCommand.TagsTraversalEnableOption) is string[] tmp2)
			options.SetBehaviour(TagBehaviour.Traverse, true, tmp2);
		if (parser.GetValueForOption(AnalyzeCommand.TagsTraversalDisableOption) is string[] tmp3)
			options.SetBehaviour(TagBehaviour.Traverse, false, tmp3);

		if (parser.GetValueForOption(AnalyzeCommand.TagsAttributesDefaultOption) is bool tmp4)
			options.SetDefaultBehaviour(TagBehaviour.Attributes, tmp4);
		if (parser.GetValueForOption(AnalyzeCommand.TagsAttributesEnableOption) is string[] tmp5)
			options.SetBehaviour(TagBehaviour.Attributes, true, tmp5);
		if (parser.GetValueForOption(AnalyzeCommand.TagsAttributesDisableOption) is string[] tmp6)
			options.SetBehaviour(TagBehaviour.Attributes, false, tmp6);

		if (parser.GetValueForOption(AnalyzeCommand.TagsExamplesDefaultOption) is bool tmp7)
			options.SetDefaultBehaviour(TagBehaviour.CollectExamples, tmp7);
		if (parser.GetValueForOption(AnalyzeCommand.TagsExamplesEnableOption) is string[] tmp8)
			options.SetBehaviour(TagBehaviour.CollectExamples, true, tmp8);
		if (parser.GetValueForOption(AnalyzeCommand.TagsExamplesDisableOption) is string[] tmp9)
			options.SetBehaviour(TagBehaviour.CollectExamples, false, tmp9);

		if (parser.GetValueForOption(AnalyzeCommand.TagsElementDefaultOption) is bool tmp10)
			options.SetDefaultBehaviour(TagBehaviour.Transient, tmp10);
		if (parser.GetValueForOption(AnalyzeCommand.TagsElementEnableOption) is string[] tmp11)
			options.SetBehaviour(TagBehaviour.Transient, true, tmp11);
		if (parser.GetValueForOption(AnalyzeCommand.TagsElementDisableOption) is string[] tmp12)
			options.SetBehaviour(TagBehaviour.Transient, false, tmp12);

		if (parser.GetValueForOption(AnalyzeCommand.AttributesExamplesDefaultOption) is bool tmp13)
			options.SetDefaultBehaviour(AttributeBehaviour.CollectExamples, tmp13);
		if (parser.GetValueForOption(AnalyzeCommand.AttributesExamplesEnableOption) is string[] tmp14)
			options.SetBehaviour(AttributeBehaviour.CollectExamples, true, tmp14);
		if (parser.GetValueForOption(AnalyzeCommand.AttributesExamplesDisableOption) is string[] tmp15)
			options.SetBehaviour(AttributeBehaviour.CollectExamples, false, tmp15);

		return options;
	}

}
