using System.Text.Json;

namespace RimworldExtractor.Analysis;

public readonly struct JsonAnalysisReportWriter : IDisposable {

	public JsonAnalysisReportWriter(AnalysisReport report, Stream stream, JsonWriterOptions options = default) {
		_report = report;
		_document = new(stream, options);
	}

	private readonly AnalysisReport _report;
	private readonly Utf8JsonWriter _document;

	public void Produce() {
		_document.WriteStartObject();

		WriteMetadata();

		WriteModules();
		WriteClasses();
		WriteTags();
		WriteTemplates();
		WriteDefinitions();

		_document.WriteEndObject();
	}

	public long Finalize() {
		_document.Flush();
		return _document.BytesCommitted;
	}

	#region Metadata Methods

	private void WriteMetadata() {
		_document.WriteStartObject("metadata");

		_document.WriteNumber("format", 1);
		_document.WriteString("title", _report.Title);
		_document.WriteString("version", _report.Version);

		if (_report.Errors.Count > 0) {
			_document.WriteStartArray("errors");
			foreach (string error in _report.Errors)
				_document.WriteStringValue(error);
			_document.WriteEndArray();
		}

		if (_report.Warnings.Count > 0) {
			_document.WriteStartArray("warnings");
			foreach (string warning in _report.Warnings)
				_document.WriteStringValue(warning);
			_document.WriteEndArray();
		}

		_document.WriteEndObject();
	}

	#endregion

	#region Module Methods

	private void WriteModules() {
		_document.WriteStartArray("modules");

		foreach (AnalysisModule module in _report.Modules) {
			_document.WriteStartArray();

			_document.WriteStringValue(module.Identifier);

			_document.WriteEndArray();
		}

		_document.WriteEndArray();
	}

	#endregion

	#region Class Methods

	private void WriteClasses() {
		_document.WriteStartArray("classes");

		foreach (AnalysisClass @class in _report.Classes) {
			_document.WriteStartArray();

			_document.WriteStringValue(@class.Name);
			_document.WriteStringValue(@class.Title);

			_document.WriteEndArray();
		}

		_document.WriteEndArray();
	}

	#endregion

	#region Tag Methods

	private void WriteTags() {
		_document.WriteStartArray("tags");

		foreach (AnalysisTag tag in _report.Tags) {
			_document.WriteStartArray();

			_document.WriteStringValue(tag.Name);
			_document.WriteStringValue(tag.Title);

				_document.WriteStartArray();
				foreach (AnalysisTag child in tag.OrderedChildren)
					_document.WriteNumberValue(_report.IndexOf(child));
				_document.WriteEndArray();

				_document.WriteStartArray();
				foreach (AnalysisTag parent in tag.OrderedParents)
					WriteTagContext(tag, parent);
				_document.WriteEndArray();

			_document.WriteEndArray();
		}

		_document.WriteEndArray();
	}

	private void WriteTagContext(AnalysisTag tag, AnalysisTag context) {
		ILookup<string, PropertyUsage> examples = tag.OrderedUses
			.Where(use => use.Parent == context)
			.ToLookup(use => use.Value);

		if (examples.Count is 0)
			return;

		_document.WriteStartArray();
		_document.WriteNumberValue(_report.IndexOf(context));

		// Write distinct examples
		_document.WriteStartArray();
		foreach (var example in examples)
			_document.WriteStringValue(example.Key);
		_document.WriteEndArray();

		// Write definitions per example
		_document.WriteStartArray();
		foreach (var example in examples) {
			_document.WriteStartArray();
			foreach (PropertyUsage use in example)
				_document.WriteStringValue(use.Definition.TypedIdentifier);
			_document.WriteEndArray();
		}
		_document.WriteEndArray();

		_document.WriteEndArray();
	}

	#endregion

	#region Template Methods

	private void WriteTemplates() {
		_document.WriteStartObject("templates");

		ILookup<AnalysisModule, AnalysisDefinition> TemplatesByModule = _report.Definitions
			.Where(definition => definition.IsAbstract)
			.ToLookup(definition => definition.Module);

		foreach (AnalysisModule module in _report.Modules) {
			AnalysisDefinition[] templatesWithinModule = TemplatesByModule[module].ToArray();
			if (templatesWithinModule.Length is 0)
				continue;

			ILookup<AnalysisClass, AnalysisDefinition> TemplatesByClass = templatesWithinModule
				.ToLookup(template => template.Class);

			_document.WriteStartObject(module.Identifier);

			foreach (AnalysisClass type in _report.Classes) {
				AnalysisDefinition[] templatesWithinClass = TemplatesByClass[type].ToArray();
				if (templatesWithinClass.Length is 0)
					continue;

				ILookup<string, AnalysisDefinition> TemplatesByFile = templatesWithinClass
					.ToLookup(template => template.Filepath);
				_document.WriteStartObject(type.Name);

				foreach (var file in TemplatesByFile) {
					_document.WriteStartObject(file.Key);
					foreach (AnalysisDefinition template in file)
						_document.WriteString(template.Name, template.Parent);
					_document.WriteEndObject();
				}

				_document.WriteEndObject();
			}

			_document.WriteEndObject();
		}

		_document.WriteEndObject();
	}

	#endregion

	#region Definition Methods

	private void WriteDefinitions() {
		_document.WriteStartObject("definitions");

		ILookup<AnalysisModule, AnalysisDefinition> DefinitionsByModule = _report.Definitions
			.Where(definition => !definition.IsAbstract)
			.ToLookup(definition => definition.Module);

		foreach (AnalysisModule module in _report.Modules) {
			AnalysisDefinition[] definitionsWithinModule = DefinitionsByModule[module].ToArray();
			if (definitionsWithinModule.Length is 0)
				continue;

			ILookup<AnalysisClass, AnalysisDefinition> DefinitionsByClass = definitionsWithinModule
				.ToLookup(definition => definition.Class);

			_document.WriteStartObject(module.Identifier);

			foreach (AnalysisClass type in _report.Classes) {
				AnalysisDefinition[] definitionsWithinClass = DefinitionsByClass[type].ToArray();
				if (definitionsWithinClass.Length is 0)
					continue;

				ILookup<string, AnalysisDefinition> DefinitionsByFile = definitionsWithinClass
					.ToLookup(definition => definition.Filepath);
				_document.WriteStartObject(type.Name);

				foreach (var file in DefinitionsByFile) {
					_document.WriteStartObject(file.Key);
					foreach (AnalysisDefinition definition in file)
						_document.WriteString(definition.Name, definition.Parent);
					_document.WriteEndObject();
				}

				_document.WriteEndObject();
			}

			_document.WriteEndObject();
		}

		_document.WriteEndObject();
	}

	#endregion

	public void Dispose() {
		_document.Dispose();
	}

}
