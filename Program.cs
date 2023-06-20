using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using RimworldExtractor.Commands;

//TODO: Final cleanup
//TODO: Special handling of li parent to be an element of a list
//		eg. on encounter XmlElement of li, instead use XmlElement.Parent and mark the parent as a list

// Construct the root command
Command root = new RimworldExplorerCommand();
CommandLineBuilder cli = new(root);

// Invoke the root command
await cli
	.UseDefaults()
	.Build()
	.InvokeAsync(args);
