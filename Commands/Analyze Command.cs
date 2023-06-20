using System.CommandLine;

namespace RimworldExtractor.Commands;

public sealed class AnalyzeCommand : Command {

	public AnalyzeCommand() : base("analyze") {
		Description = "Analyze vanilla definitions";

		// Provide the analysis formats
		AddCommand(new JsonAnalysisCommand());
		AddCommand(new HtmlAnalysisCommand());
		AddCommand(new WebsiteAnalysisCommand());
	}

}
