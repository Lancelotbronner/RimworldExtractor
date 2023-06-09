using System.Runtime.Intrinsics.X86;
using System.Text.Json;

namespace RimworldExplorer.Analysis;

public struct JsonAnalysisReportWriter : IDisposable {

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
		_document.WriteStartObject("modules");

		foreach (AnalysisModule module in _report.OrderedModules) {
			_document.WriteStartObject(module.Identifier);
			_document.WriteEndObject();
		}

		_document.WriteEndObject();
	}

	#endregion

	#region Class Methods

	private void WriteClasses() {
		_document.WriteStartObject("classes");

		foreach (AnalysisClass @class in _report.OrderedClasses) {
			_document.WriteStartObject(@class.Name);
			_document.WriteEndObject();
		}

		_document.WriteEndObject();
	}

	#endregion

	#region Tag Methods

	private void WriteTags() {
		_document.WriteStartObject("tags");

		foreach (AnalysisTag tag in _report.OrderedTags) {
			_document.WriteStartObject(tag.Name);

			if (tag.Children.Count > 0) {
				_document.WriteStartArray("children");
				foreach (AnalysisTag child in tag.OrderedChildren)
					_document.WriteStringValue(child.Name);
				_document.WriteEndArray();
			}

			if (tag.Parents.Count > 0) {
				_document.WriteStartObject("contexts");
				foreach (AnalysisTag parent in tag.OrderedParents)
					WriteTagContext(tag, parent);
				_document.WriteEndObject();
			}

			_document.WriteEndObject();
		}

		_document.WriteEndObject();
	}

	private void WriteTagContext(AnalysisTag tag, AnalysisTag context) {
		PropertyUsage[] uses = tag.OrderedUses
			.Where(use => use.Parent == context)
			.ToArray();

		if (uses.Length is 0)
			return;

		_document.WriteStartObject(context.Name);

		string[] examples = uses
			.Select(use => use.Value)
			.Distinct()
			.ToArray();

		// Write distinct examples
		_document.WriteStartArray("examples");
		foreach (string example in examples)
			_document.WriteStringValue(example);
		_document.WriteEndArray();

		// Write example index per definition
		_document.WriteStartObject("usage");
		foreach (var group in uses.ToLookup(use => use.Definition)) {
			//TODO: Possibly group them by class (ThingDef)
			_document.WritePropertyName(group.Key.TypedIdentifier);

			var enumerator = group.GetEnumerator();
			enumerator.MoveNext();
			PropertyUsage first = enumerator.Current;
			bool many = enumerator.MoveNext();

			if (many)
				_document.WriteStartArray();

			_document.WriteNumberValue(Array.IndexOf(examples, first.Value));

			if (many) {
				do
					_document.WriteNumberValue(Array.IndexOf(examples, enumerator.Current.Value));
				while (enumerator.MoveNext());
				_document.WriteEndArray();
			}
		}
		_document.WriteEndObject();

		_document.WriteEndObject();
	}

	private void WriteTagUse(string value, string[] examples) {
		int i = Array.IndexOf(examples, value);
		_document.WriteNumberValue(i);
	}

	private void WriteTagExamples(IEnumerable<PropertyUsage> examples) {
		_document.WriteStartArray();
		foreach (PropertyUsage example in examples)
			_document.WriteStringValue(example.Value);
		_document.WriteEndArray();
	}

	private void WriteTagUses(IEnumerable<PropertyUsage> uses, PropertyUsage[] examples) {
		_document.WriteStartObject();
		foreach (var definition in uses.ToLookup(use => use.Definition)) {
			PropertyUsage[] group = definition.ToArray();
			_document.WritePropertyName(definition.Key.TypedIdentifier);

			if (group.Length is 1) {
				//_document.WriteNumberValue(Array.IndexOf(examples, group[0].Value));
				_document.WriteStartObject();
				_document.WriteNumber("i", Array.IndexOf(examples, group[0].Value));
				_document.WriteString("v", group[0].Value);
				_document.WriteEndObject();
			} else {
				_document.WriteStartArray();
				foreach (PropertyUsage use in definition) {
					//_document.WriteNumberValue(Array.IndexOf(examples, use.Value));

					_document.WriteStartObject();
					_document.WriteNumber("i", Array.IndexOf(examples, use.Value));
					_document.WriteString("v", use.Value);
					_document.WriteEndObject();
				}
				_document.WriteEndArray();
			}
		}
		_document.WriteEndObject();
	}

	private IEnumerable<IEnumerable<PropertyUsage>> WriteByTagParent(AnalysisTag tag, IEnumerable<PropertyUsage> uses) {
		if (tag.Parents.Count is 1) {
			yield return uses;
			yield break;
		}

		_document.WriteStartObject();
		foreach (AnalysisTag parent in tag.OrderedParents) {
			_document.WritePropertyName(parent.Name);
			yield return uses.Where(use => use.Parent == parent);
		}
		_document.WriteEndObject();
	}

	private IEnumerable<IEnumerable<PropertyUsage>> WriteByTagParent(string property, AnalysisTag tag, IReadOnlyCollection<PropertyUsage> uses) {
		if (uses.Count is 0)
			return Array.Empty<IEnumerable<PropertyUsage>>();
		_document.WritePropertyName(property);
		return WriteByTagParent(tag, uses);
	}

	private IEnumerable<IEnumerable<PropertyUsage>> WriteByTagParent(string property, AnalysisTag tag, IEnumerable<PropertyUsage> uses)
		=> WriteByTagParent(property, tag, uses.ToArray());

	#endregion

	#region Template Methods

	private void WriteTemplates() {
		_document.WriteStartObject("templates");

		ILookup<AnalysisModule, AnalysisDefinition> TemplatesByModule = _report.OrderedDefinitions
			.Where(definition => definition.IsAbstract)
			.ToLookup(definition => definition.Module);

		foreach (AnalysisModule module in _report.OrderedModules) {
			AnalysisDefinition[] templatesWithinModule = TemplatesByModule[module].ToArray();
			if (templatesWithinModule.Length is 0)
				continue;

			ILookup<AnalysisClass, AnalysisDefinition> TemplatesByClass = templatesWithinModule
				.ToLookup(template => template.Class);

			_document.WriteStartObject(module.Identifier);

			foreach (AnalysisClass type in _report.OrderedClasses) {
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

		ILookup<AnalysisModule, AnalysisDefinition> DefinitionsByModule = _report.OrderedDefinitions
			.Where(definition => !definition.IsAbstract)
			.ToLookup(definition => definition.Module);

		foreach (AnalysisModule module in _report.OrderedModules) {
			AnalysisDefinition[] definitionsWithinModule = DefinitionsByModule[module].ToArray();
			if (definitionsWithinModule.Length is 0)
				continue;

			ILookup<AnalysisClass, AnalysisDefinition> DefinitionsByClass = definitionsWithinModule
				.ToLookup(definition => definition.Class);

			_document.WriteStartObject(module.Identifier);

			foreach (AnalysisClass type in _report.OrderedClasses) {
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
