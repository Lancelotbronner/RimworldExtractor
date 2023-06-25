using System.Text.RegularExpressions;

namespace RimworldAnalyzer.Analysis;

static partial class RimworldAnalysisExtensions {

	[GeneratedRegex(@"(\p{L})(\p{N})")]
	internal static partial Regex NumberTransition();

	[GeneratedRegex(@"(\p{Ll}|\p{N})(\p{Lu})")]
	internal static partial Regex CaseTransition();

	[GeneratedRegex(@"(\p{Lu}+)(\p{Lu})")]
	internal static partial Regex UppercaseGroups();

	[GeneratedRegex(@"Def\b")]
	internal static partial Regex DetectDefinitionAbbreviation();

	[GeneratedRegex(@"Tex\b")]
	internal static partial Regex DetectTextureAbbreviation();

	[GeneratedRegex(@"(.?){(.+?)}")]
	internal static partial Regex DetectTemplateToken();

}
