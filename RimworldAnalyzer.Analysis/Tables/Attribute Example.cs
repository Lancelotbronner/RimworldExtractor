using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("attribute-examples"), PrimaryKey(nameof(ExampleId), nameof(TagId), nameof(AttributeId), nameof(DefinitionId))]
public sealed class AttributeExampleTable {

	private AttributeExampleTable() { }

	public AttributeExampleTable(AttributeTable attribute, TagTable tag, DefinitionTable definition, ExampleTable example) {
		AttributeId = attribute.Id;
		Attribute = attribute;
		TagId = tag.Id;
		Tag = tag;
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

	[Column("attribute"), ForeignKey(nameof(Attribute))]
	public int? AttributeId { get; set; }

	public AttributeTable? Attribute { get; set; }

	[Column("definition"), ForeignKey(nameof(Definition))]
	public int? DefinitionId { get; set; }

	public DefinitionTable? Definition { get; set; }

}
