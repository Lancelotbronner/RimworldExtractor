using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("tag-examples"), Index(nameof(ExampleId), nameof(TagId), nameof(ContextId), IsUnique = true)]
public sealed class TagExampleTable {

	private TagExampleTable() { }

	public TagExampleTable(TagTable tag, TagTable context, ExampleTable example) {
		TagId = tag.Id;
		Tag = tag;
		ContextId = context.Id;
		Context = context;
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

	[Column("context"), ForeignKey(nameof(Context))]
	public int? ContextId { get; set; }

	public TagTable? Context { get; set; }

	[InverseProperty(nameof(TagUsageTable.Example))]
	public ICollection<TagUsageTable>? Usage { get; private set; }

}
