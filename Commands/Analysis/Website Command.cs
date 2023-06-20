using System.CommandLine;
using RimworldExtractor.Analysis;

namespace RimworldExtractor.Commands;

public sealed class WebsiteAnalysisCommand : AnalysisCommand {

	public static readonly Option<DirectoryInfo> Output = CreateOutputDirectoryOption("Website");

	public WebsiteAnalysisCommand() : base("website") {
		// Specify the output directory
		AddOption(Output);
	}

	protected override void ConfigureHandler(AnalysisParameterPackBinding parameters) {
		this.SetHandler(Handle, parameters, Output);
	}

	public static async Task Handle(AnalysisParameterPack parameters, DirectoryInfo output) {
		Announce();
		Analyze(in parameters, out AnalysisReport report);

		// Prepare the report writer
		if (output.Exists)
			output.Delete(true);
		output.Create();
		WebsiteAnalysisReportWriter writer = new(report);

		// Produce the report
		Directory.SetCurrentDirectory(output.FullName);
		await writer.Produce();
	}

}
