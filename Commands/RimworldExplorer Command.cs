using System.CommandLine;

namespace RimworldExplorer.Commands;

public sealed class RimworldExplorerCommand : RootCommand {

	public RimworldExplorerCommand() : base() {
		Description = """
			Explore's the Rimworld definitions to document their usage
		""";

		AddCommand(new AnalyzeCommand());
	}

}
