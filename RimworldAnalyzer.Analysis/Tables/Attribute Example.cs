using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("attribute-examples"), Index(nameof(ExampleId), nameof(TagId), nameof(AttributeId), IsUnique = true)]
public sealed class AttributeExampleTable {

	private AttributeExampleTable() { }

	public AttributeExampleTable(AttributeTable attribute, TagTable tag, ExampleTable example) {
		AttributeId = attribute.Id;
		Attribute = attribute;
		TagId = tag.Id;
		Tag = tag;
		ExampleId = example.Id;
		Example = example;
	}

	[Column("id"), Key]
	public int Id { get; private set; }

	[Column("example"), ForeignKey(nameof(Example))]
	public int? ExampleId { get; set; }

	public ExampleTable? Example { get; set; }

	[Column("tag"), ForeignKey(nameof(Tag))]
	public int? TagId { get; set; }

	public TagTable? Tag { get; set; }

	[Column("attribute"), ForeignKey(nameof(Attribute))]
	public int? AttributeId { get; set; }

	public AttributeTable? Attribute { get; set; }

	[InverseProperty(nameof(AttributeUsageTable.Example))]
	public ICollection<AttributeUsageTable>? Usage { get; private set; }

}
