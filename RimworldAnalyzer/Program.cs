using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using RimworldAnalyzer.Commands;

// Construct the root command
Command root = new RimworldExplorerCommand();
CommandLineBuilder cli = new(root);

// Invoke the root command
await cli
	.UseDefaults()
	.Build()
	.InvokeAsync(args);
