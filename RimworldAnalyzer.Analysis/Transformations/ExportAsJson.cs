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
		var rows = context.Attributes
			.AsNoTracking()
			.OrderBy(row => row.Id);
		await foreach (AttributeTable row in rows.AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteStringValue(row.Identifier);
			json.WriteStringValue(row.Name);
			json.WriteIdentifierColumnValue(row.ModuleId);
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteAttributeExamples() {
		json.WriteStartArray("attribute-examples");
		var rows = context.AttributeExamples
			.AsNoTracking()
			.OrderBy(row => row.Id);
		await foreach (AttributeExampleTable row in rows.AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteIdentifierColumnValue(row.ExampleId);
			json.WriteIdentifierColumnValue(row.TagId);
			json.WriteIdentifierColumnValue(row.AttributeId);
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteAttributeUsage() {
		json.WriteStartArray("attribute-usage");
		await foreach (AttributeUsageTable row in context.AttributeUsage.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteIdentifierColumnValue(row.ExampleId);
			json.WriteIdentifierColumnValue(row.DefinitionId);
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteClasses() {
		json.WriteStartArray("classes");
		var rows = context.Classes
			.AsNoTracking()
			.OrderBy(row => row.Id);
		await foreach (ClassTable row in rows.AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteStringValue(row.Identifier);
			json.WriteStringValue(row.Name);
			json.WriteIdentifierColumnValue(row.ModuleId);
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteDefinitions() {
		json.WriteStartArray("definitions");
		var rows = context.Definitions
			.AsNoTracking()
			.OrderBy(row => row.Id);
		await foreach (DefinitionTable row in rows.AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteStringValue(row.Identifier);
			json.WriteBooleanValue(row.IsAbstract);
			json.WriteIdentifierColumnValue(row.ParentId);
			json.WriteIdentifierColumnValue(row.ModuleId);
			json.WriteIdentifierColumnValue(row.ResourceId);
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteExamples() {
		json.WriteStartArray("examples");
		var rows = context.Examples
			.AsNoTracking()
			.OrderBy(row => row.Id);
		await foreach (ExampleTable row in rows.AsAsyncEnumerable()) {
			json.WriteStringValue(row.Value);
		}
		json.WriteEndArray();
	}

	private async Task WriteIssues() {
		json.WriteStartArray("issues");
		var rows = context.Issues
			.AsNoTracking()
			.OrderBy(row => row.Id);
		await foreach (IssueTable row in rows.AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteNumberValue((byte)row.Severity);
			json.WriteStringValue(row.Message);
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteModules() {
		json.WriteStartArray("modules");
		var rows = context.Modules
			.AsNoTracking()
			.OrderBy(row => row.Id);
		await foreach (ModuleTable row in rows.AsAsyncEnumerable()) {
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
		var rows = context.Relationships
			.AsNoTracking()
			.OrderBy(row => row.Id);
		await foreach (RelationshipTable row in rows.AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteIdentifierColumnValue(row.ParentId);
			json.WriteIdentifierColumnValue(row.ChildId);
			json.WriteIdentifierColumnValue(row.ContextId);
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteResources() {
		json.WriteStartArray("resources");
		var rows = context.Resources
			.AsNoTracking()
			.OrderBy(row => row.Id);
		await foreach (ResourceTable row in rows.AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteStringValue(row.Path);
			json.WriteIdentifierColumnValue(row.ModuleId);
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteTags() {
		json.WriteStartArray("tags");
		var rows = context.Tags
			.AsNoTracking()
			.OrderBy(row => row.Id);
		await foreach (TagTable row in rows.AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteStringValue(row.Identifier);
			json.WriteStringValue(row.Name);
			json.WriteIdentifierColumnValue(row.ModuleId);
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteTagExamples() {
		json.WriteStartArray("tag-examples");
		var rows = context.TagExamples
			.AsNoTracking()
			.OrderBy(row => row.Id);
		await foreach (TagExampleTable row in rows.AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteIdentifierColumnValue(row.ExampleId);
			json.WriteIdentifierColumnValue(row.RelationshipId);
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

	private async Task WriteTagUsage() {
		json.WriteStartArray("tag-usage");
		await foreach (TagUsageTable row in context.TagUsage.AsNoTracking().AsAsyncEnumerable()) {
			json.WriteStartArray();
			json.WriteIdentifierColumnValue(row.ExampleId);
			json.WriteIdentifierColumnValue(row.DefinitionId);
			json.WriteEndArray();
		}
		json.WriteEndArray();
	}

}
