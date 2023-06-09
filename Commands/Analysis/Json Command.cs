using System.CommandLine;
using RimworldExplorer.Analysis;

namespace RimworldExplorer.Commands;

public sealed class JsonAnalysisCommand : AnalysisCommand {

	public static readonly Option<FileInfo> Output = AnalysisCommand.CreateOutputFileOption("JSON", "json");

	public JsonAnalysisCommand() : base("json") {
		// Specify the output file
		AddOption(Output);
	}

	protected override void ConfigureHandler(AnalysisParameterPackBinding parameters) {
		this.SetHandler(Handle, parameters, Output);
	}

	public static void Handle(AnalysisParameterPack parameters, FileInfo output) {
		Announce();
		Analyze(in parameters, out AnalysisReport report);

		// Prepare the report writer
		using FileStream file = File.OpenWrite(output.FullName);
		using JsonAnalysisReportWriter writer = new(report, file);

		// Produce the report
		writer.Produce();
		file.SetLength(writer.Finalize());
	}

}
