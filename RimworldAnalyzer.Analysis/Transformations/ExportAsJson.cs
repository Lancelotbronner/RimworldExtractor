using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

public readonly struct ExportAnalysisAsJson {

	public ExportAnalysisAsJson(AnalysisDatabase context, Utf8JsonWriter json) {
		this.context = context;
		this.json = json;
	}

	private readonly AnalysisDatabase context;
	private readonly Utf8JsonWriter json;

	public async Task Execute() {
		json.WriteStartObject();
		json.WriteNumber("format", 1);
		await WriteCount();
		await WriteAttributes();
		await WriteAttributeExamples();
		await WriteAttributeUsage();
		await WriteClasses();
		await WriteDefinitions();
		await WriteExamples();
		await WriteIssues();
		await WriteModules();
		await WriteRelationships();
		await WriteResources();
		await WriteTags();
		await WriteTagExamples();
		await WriteTagUsage();
		json.WriteEndObject();
		await json.FlushAsync();
	}

	private async Task WriteCount() {
		json.WriteStartObject("size");
		json.WriteNumber("attributes", await context.Attributes.CountAsync());
		json.WriteNumber("attribute-examples", await context.AttributeExamples.CountAsync());
		json.WriteNumber("attribute-usage", await context.AttributeUsage.CountAsync());
		json.WriteNumber("classes", await context.Classes.CountAsync());
		json.WriteNumber("definitions", await context.Definitions.CountAsync());
		json.WriteNumber("examples", await context.Examples.CountAsync());
		json.WriteNumber("issues", await context.Issues.CountAsync());
		json.WriteNumber("modules", await context.Modules.CountAsync());
		json.WriteNumber("relationships", await context.Relationships.CountAsync());
		json.WriteNumber("resources", await context.Resources.CountAsync());
		json.WriteNumber("tags", await context.Tags.CountAsync());
		json.WriteNumber("tag-examples", await context.TagExamples.CountAsync());
		json.WriteNumber("tag-usage", await context.TagUsage.CountAsync());
		json.WriteEndObject();
	}

	private async Task WriteAttributes() {
		json.WriteStartArray("attributes");
		await foreach (AttributeTable row in context.Attributes.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteStringValue(row.Identifier);
			json.WriteStringValue(row.Name);
			if (row.ModuleId is int module)
				json.WriteNumberValue(module);
			else
				json.WriteNullValue();
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteAttributeExamples() {
		json.WriteStartArray("attribute-examples");
		await foreach (AttributeExampleTable row in context.AttributeExamples.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			if (row.ExampleId is int example)
				json.WriteNumberValue(example);
			else
				json.WriteNullValue();
			if (row.TagId is int tag)
				json.WriteNumberValue(tag);
			else
				json.WriteNullValue();
			if (row.AttributeId is int attribute)
				json.WriteNumberValue(attribute);
			else
				json.WriteNullValue();
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteAttributeUsage() {
		json.WriteStartArray("attribute-usage");
		await foreach (AttributeUsageTable row in context.AttributeUsage.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			if (row.ExampleId is int example)
				json.WriteNumberValue(example);
			else
				json.WriteNullValue();
			if (row.DefinitionId is int definition)
				json.WriteNumberValue(definition);
			else
				json.WriteNullValue();
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteClasses() {
		json.WriteStartArray("classes");
		await foreach (ClassTable row in context.Classes.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteStringValue(row.Identifier);
			json.WriteStringValue(row.Name);
			if (row.ModuleId is int module)
				json.WriteNumberValue(module);
			else
				json.WriteNullValue();
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteDefinitions() {
		json.WriteStartArray("definitions");
		await foreach (DefinitionTable row in context.Definitions.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteStringValue(row.Identifier);
			json.WriteBooleanValue(row.IsAbstract);
			if (row.ParentId is int parent)
				json.WriteNumberValue(parent);
			else
				json.WriteNullValue();
			if (row.ModuleId is int module)
				json.WriteNumberValue(module);
			else
				json.WriteNullValue();
			if (row.ResourceId is int resource)
				json.WriteNumberValue(resource);
			else
				json.WriteNullValue();
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteExamples() {
		json.WriteStartArray("examples");
		await foreach (ExampleTable row in context.Examples.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStringValue(row.Value);
		}
		json.WriteEndArray();
	}

	private async Task WriteIssues() {
		json.WriteStartArray("issues");
		await foreach (IssueTable row in context.Issues.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteNumberValue((byte)row.Severity);
			json.WriteStringValue(row.Message);
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteModules() {
		json.WriteStartArray("modules");
		await foreach (ModuleTable row in context.Modules.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteStringValue(row.Identifier);
			json.WriteStringValue(row.Name);
			json.WriteStringValue(row.Version);
			json.WriteBooleanValue(row.IsOfficial);
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteRelationships() {
		json.WriteStartArray("relationships");
		await foreach (RelationshipTable row in context.Relationships.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			if (row.ParentId is int parent)
				json.WriteNumberValue(parent);
			else
				json.WriteNullValue();
			if (row.ChildId is int child)
				json.WriteNumberValue(child);
			else
				json.WriteNullValue();
			if (row.ContextId is int context)
				json.WriteNumberValue(context);
			else
				json.WriteNullValue();
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteResources() {
		json.WriteStartArray("resources");
		await foreach (ResourceTable row in context.Resources.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteStringValue(row.Path);
			if (row.ModuleId is int module)
				json.WriteNumberValue(module);
			else
				json.WriteNullValue();
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteTags() {
		json.WriteStartArray("tags");
		await foreach (TagTable row in context.Tags.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteStringValue(row.Identifier);
			json.WriteStringValue(row.Name);
			if (row.ModuleId is int module)
				json.WriteNumberValue(module);
			else
				json.WriteNullValue();
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteTagExamples() {
		json.WriteStartArray("tag-examples");
		await foreach (TagExampleTable row in context.TagExamples.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			if (row.ExampleId is int example)
				json.WriteNumberValue(example);
			else
				json.WriteNullValue();
			if (row.TagId is int tag)
				json.WriteNumberValue(tag);
			else
				json.WriteNullValue();
			if (row.ContextId is int context)
				json.WriteNumberValue(context);
			else
				json.WriteNullValue();
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteTagUsage() {
		json.WriteStartArray("tag-usage");
		await foreach (TagUsageTable row in context.TagUsage.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			if (row.ExampleId is int example)
				json.WriteNumberValue(example);
			else
				json.WriteNullValue();
			if (row.DefinitionId is int definition)
				json.WriteNumberValue(definition);
			else
				json.WriteNullValue();
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

}
