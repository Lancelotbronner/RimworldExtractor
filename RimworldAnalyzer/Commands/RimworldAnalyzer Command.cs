using System.CommandLine;

namespace RimworldAnalyzer.Commands;

public sealed class RimworldExplorerCommand : RootCommand {

	public RimworldExplorerCommand() : base() {
		Description = """
		Tool to produce and work with Rimworld analysis archives.

		Created by Lancelotbronner. Based on modifications by Epicguru. Based on the original by milon.
		""";

		AddCommand(new AnalyzeCommand());
		AddCommand(new ExportCommand());
	}

}
