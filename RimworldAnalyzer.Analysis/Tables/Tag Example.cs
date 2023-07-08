using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("tag-examples"), Index(nameof(ExampleId), nameof(RelationshipId), IsUnique = true)]
public sealed class TagExampleTable {

	private TagExampleTable() { }

	public TagExampleTable(RelationshipTable relationship, ExampleTable example) {
		RelationshipId = relationship.Id;
		Relationship = relationship;
		ExampleId = example.Id;
		Example = example;
	}

	[Column("id"), Key]
	public int Id { get; private set; }

	[Column("example"), ForeignKey(nameof(Example))]
	public int ExampleId { get; set; }

	public ExampleTable? Example { get; set; }

	[Column("relationship"), ForeignKey(nameof(Relationship))]
	public int RelationshipId { get; set; }

	public RelationshipTable? Relationship { get; set; }

	[InverseProperty(nameof(TagUsageTable.Example))]
	public ICollection<TagUsageTable>? Usage { get; private set; }

}
