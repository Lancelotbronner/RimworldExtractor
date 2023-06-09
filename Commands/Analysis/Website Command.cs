using System.CommandLine;
using RimworldExplorer.Analysis;

namespace RimworldExplorer.Commands;

public sealed class WebsiteAnalysisCommand : AnalysisCommand {

	public static readonly Option<DirectoryInfo> Output = CreateOutputDirectoryOption("Website");

	public WebsiteAnalysisCommand() : base("website") {
		// Specify the output directory
		AddOption(Output);
	}

	protected override void ConfigureHandler(AnalysisParameterPackBinding parameters) {
		this.SetHandler(Handle, parameters, Output);
	}

	public static void Handle(AnalysisParameterPack parameters, DirectoryInfo output) {
		Announce();
		Analyze(in parameters, out AnalysisReport report);

		// Prepare the report writer
		using FileStream file = File.OpenWrite(output.FullName);
		WebsiteAnalysisReportWriter writer = new(report);

		// Produce the report
		writer.Produce();
	}

}
