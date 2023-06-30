using System.CommandLine;

namespace RimworldAnalyzer.Commands;

public sealed class ExportCommand : Command {
	public ExportCommand() : base("export") {
		Description = "Groups export subcommands.";

		AddCommand(new JsonCommand());
	}
}
