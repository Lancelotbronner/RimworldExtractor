using System.CommandLine;

namespace RimworldExtractor.Commands;

public sealed class RimworldExplorerCommand : RootCommand {

	public RimworldExplorerCommand() : base() {
		Description = """
			Explore's the Rimworld definitions to document their usage
		""";

		AddCommand(new AnalyzeCommand());
	}

}
