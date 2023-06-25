using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("tag-examples"), PrimaryKey(nameof(ExampleId), nameof(TagId), nameof(ContextId), nameof(DefinitionId))]
public sealed class TagExampleTable {

	private TagExampleTable() { }

	public TagExampleTable(TagTable tag, TagTable context, DefinitionTable definition, ExampleTable example) {
		TagId = tag.Id;
		Tag = tag;
		ContextId = context.Id;
		Context = context;
		DefinitionId = definition.Id;
		Definition = definition;
		ExampleId = example.Id;
		Example = example;
	}

	[Column("example"), ForeignKey(nameof(Example))]
	public int? ExampleId { get; set; }

	public ExampleTable? Example { get; set; }

	[Column("tag"), ForeignKey(nameof(Tag))]
	public int? TagId { get; set; }

	public TagTable? Tag { get; set; }

	[Column("context"), ForeignKey(nameof(Context))]
	public int? ContextId { get; set; }

	public TagTable? Context { get; set; }

	[Column("definition"), ForeignKey(nameof(Definition))]
	public int? DefinitionId { get; set; }

	public DefinitionTable? Definition { get; set; }

}
