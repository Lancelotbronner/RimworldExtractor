using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("tag-usage"), PrimaryKey(nameof(ExampleId), nameof(DefinitionId))]
public sealed class TagUsageTable {

	private TagUsageTable() { }

	public TagUsageTable(DefinitionTable definition, TagExampleTable example) {
		DefinitionId = definition.Id;
		Definition = definition;
		ExampleId = example.Id;
		Example = example;
	}

	[Column("example"), ForeignKey(nameof(Example))]
	public int? ExampleId { get; set; }

	public TagExampleTable? Example { get; set; }

	[Column("definition"), ForeignKey(nameof(Definition))]
	public int? DefinitionId { get; set; }

	public DefinitionTable? Definition { get; set; }

}
