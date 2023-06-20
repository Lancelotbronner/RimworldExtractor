using System.Reflection;
using System.Text.RegularExpressions;
using RimworldExtractor.Templates;

namespace RimworldExtractor.Analysis;

public readonly struct WebsiteAnalysisReportWriter {

	public WebsiteAnalysisReportWriter(AnalysisReport report) {
		_report = report;
	}

	private readonly AnalysisReport _report;

	#region Writing Management

	public async Task Produce() {
		List<Task[]> groups = new(8);

		TemplateEvaluator top = new();
		top.Add(EvaluateToken);

		// Write main navigation
		Task index = ProcessTemplateAsync("index.html", top);
		Task classes = ProcessTemplateAsync("classes.html", top);
		Task definitions = ProcessTemplateAsync("definitions.html", top);
		Task modules = ProcessTemplateAsync("modules.html", top);
		Task tags = ProcessTemplateAsync("tags.html", top);

		// Write all classes
		Directory.CreateDirectory("classes");
		groups.Add(_report.Classes.Select(schema => {
			TemplateEvaluator evaluator = new();
			evaluator.Add(schema, EvaluateSchemaToken);
			evaluator.Add(EvaluateToken);
			return ProcessTemplateAsync("class.html", $"classes/{schema.Name}.html", evaluator);
		}).ToArray());

		await Task.WhenAll(index, classes, definitions, modules, tags);
		foreach (Task[] group in groups)
			await Task.WhenAll(group);
	}

	#endregion

	#region Templating Management

	private static async Task ProcessTemplateAsync(string template, string filename, TemplateEvaluator evaluator) {
		string contents = await GetTemplateAsync(template);
		await File.WriteAllTextAsync(filename, evaluator.Evaluate(contents));
	}

	private static Task ProcessTemplateAsync(string filename, TemplateEvaluator evaluator)
		=> ProcessTemplateAsync(filename, filename, evaluator);

	public string? EvaluateToken(ref ReadOnlySpan<char> token) => token switch {
		"Title" => _report.Title,
		"Version" => _report.Version,
		"Classes.Count" => _report.Classes.Length.ToString(),
		"Definitions.Count" => _report.Definitions.Length.ToString(),
		"Modules.Count" => _report.Modules.Length.ToString(),
		"Tags.Count" => _report.Tags.Length.ToString(),
		_ => null,
	};

	public static string? EvaluateSchemaToken(AnalysisClass schema, ref ReadOnlySpan<char> token) => token switch {
		"Identifier" => schema.Name,
		"Title" => schema.Title,
		_ => null,
	};

	private readonly static Assembly CurrentAssembly = Assembly.GetAssembly(typeof(WebsiteAnalysisReportWriter))!;
	const string ResourcePrefix = "RimworldExtractor.Templates.Website.";

	private readonly static string AvailableResources = string.Join(", ", CurrentAssembly
		.GetManifestResourceNames()
		.Where(name => name.StartsWith(ResourcePrefix))
		.Select(name => $"'{name[ResourcePrefix.Length..]}'"));

	private static Task<string> GetTemplateAsync(string filename) {
		Assembly assembly = Assembly.GetExecutingAssembly();
		using Stream? stream = assembly.GetManifestResourceStream($"{ResourcePrefix}{filename}") ?? throw new($"Unknown template '{filename}', available templates are {AvailableResources}");
		using TextReader text = new StreamReader(stream);
		return text.ReadToEndAsync();
	}

	#endregion

}
