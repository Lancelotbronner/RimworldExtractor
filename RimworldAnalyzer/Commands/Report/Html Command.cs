//using System.CommandLine;
//using RimworldExtractor.Analysis;

//namespace RimworldExtractor.Commands;

//public sealed class HtmlAnalysisCommand : AnalysisCommand {

//	public static readonly Option<FileInfo> Output = AnalysisCommand.CreateOutputFileOption("HTML", "html");

//	public HtmlAnalysisCommand() : base("html") {
//		// Specify the output file
//		AddOption(Output);
//	}

//	protected override void ConfigureHandler(AnalysisParameterPackBinding parameters) {
//		this.SetHandler(Handle, parameters, Output);
//	}

//	public static void Handle(AnalysisParameterPack parameters, FileInfo output) {
//		Announce();
//		Analyze(in parameters, out AnalysisReport report);

//		// Prepare the report writer
//		using FileStream file = File.OpenWrite(output.FullName);
//		HtmlAnalysisReportWriter writer = new(report, file);

//		// Produce the report
//		writer.Produce();
//	}

//}
