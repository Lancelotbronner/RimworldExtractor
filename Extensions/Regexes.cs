using System.Text.RegularExpressions;

namespace RimworldExtractor;

public static partial class RegexLibrary {

	[GeneratedRegex(@"(\p{L})(\p{N})")]
	public static partial Regex NumberTransition();

	[GeneratedRegex(@"(\p{Ll}|\p{N})(\p{Lu})")]
	public static partial Regex CaseTransition();

	[GeneratedRegex(@"(\p{Lu}+)(\p{Lu})")]
	public static partial Regex UppercaseGroups();

	[GeneratedRegex(@"Def\b")]
	public static partial Regex DetectDefinitionAbbreviation();

	[GeneratedRegex(@"Tex\b")]
	public static partial Regex DetectTextureAbbreviation();

	[GeneratedRegex(@"(.?){(.+?)}")]
	public static partial Regex DetectTemplateToken();

}
