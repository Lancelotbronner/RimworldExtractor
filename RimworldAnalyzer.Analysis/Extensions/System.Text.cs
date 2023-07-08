using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace RimworldAnalyzer.Analysis;

public static partial class RimworldAnalysisExtensions {

	public static string ToTitleCase(this string self) {
		string result = self;
		result = result.Replace('_', ' ');
		result = NumberTransition().Replace(result, SpaceCaptures);
		result = CaseTransition().Replace(result, SpaceCaptures);
		result = UppercaseGroups().Replace(result, SpaceCaptures);
		result = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(result);
		return result.Trim();
	}

	private static string SpaceCaptures(Match match)
		=> $"{match.Groups[1]} {match.Groups[2]}";

	internal static void WriteIdentifierColumnValue(this Utf8JsonWriter json, int? value) {
		if (value is int id)
			json.WriteNumberValue(id - 1);
		else
			json.WriteNullValue();
	}

}