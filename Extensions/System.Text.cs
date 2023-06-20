using System.Globalization;
using System.Text.RegularExpressions;

namespace RimworldExtractor;

public static partial class RimworldExtractorExtensions {

	public static string ToTitleCase(this string self) {
		string result = self;
		result = result.Replace('_', ' ');
		result = RegexLibrary.NumberTransition().Replace(result, SpaceCaptures);
		result = RegexLibrary.CaseTransition().Replace(result, SpaceCaptures);
		result = RegexLibrary.UppercaseGroups().Replace(result, SpaceCaptures);
		result = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(result);
		return result.Trim();
	}

	private static string SpaceCaptures(Match match)
		=> $"{match.Groups[1]} {match.Groups[2]}";

}