using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("attribute-usage"), PrimaryKey(nameof(ExampleId), nameof(DefinitionId))]
public sealed class AttributeUsageTable {

	private AttributeUsageTable() { }

	public AttributeUsageTable(DefinitionTable definition, AttributeExampleTable example) {
		DefinitionId = definition.Id;
		Definition = definition;
		ExampleId = example.Id;
		Example = example;
	}

	[Column("example"), ForeignKey(nameof(Example))]
	public int? ExampleId { get; set; }

	public AttributeExampleTable? Example { get; set; }

	[Column("definition"), ForeignKey(nameof(Definition))]
	public int? DefinitionId { get; set; }

	public DefinitionTable? Definition { get; set; }

}
