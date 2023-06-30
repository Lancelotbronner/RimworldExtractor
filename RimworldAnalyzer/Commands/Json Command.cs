using System.CommandLine;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RimworldAnalyzer.Analysis;

namespace RimworldAnalyzer.Commands;

public sealed class JsonCommand : Command {

	public JsonCommand() : base("json") {
		Description = "Produces a JSON document with the contents of the database";

		AddArgument(Input);
		AddOption(Output);

		this.SetHandler(Handle, Input, Output);
	}

	public async Task Handle(FileInfo input, FileInfo? output) {
		if (!input.Exists) {
			Console.WriteLine($"There are no analysis reports at '{input.FullName}'");
			return;
		}

		DbContextOptions<AnalysisDatabase> dboptions = new DbContextOptionsBuilder<AnalysisDatabase>()
			.UseSqlite($"Data Source={input.FullName};Foreign Keys=False")
			.Options;
		AnalysisDatabase context = new(dboptions);

		output ??= new(Path.ChangeExtension(input.FullName, "json"));
		using FileStream file = output.OpenWrite();
		Utf8JsonWriter json = new(file);

		ExportAnalysisAsJson transform = new(context, json);

		await transform.Execute();
	}

	public static readonly Argument<FileInfo> Input = new("input", "The archive to export");
	public static readonly Option<FileInfo?> Output = new("output", "The destination of the exported file");

}
