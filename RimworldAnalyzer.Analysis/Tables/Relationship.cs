using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("relationships"), Index(nameof(ParentId), nameof(ChildId), nameof(ContextId), IsUnique = true)]
public sealed class RelationshipTable {

	private RelationshipTable() { }

	public RelationshipTable(TagTable parent, TagTable child, TagTable? context) {
		ParentId = parent.Id;
		Parent = parent;
		ChildId = child.Id;
		Child = child;
		ContextId = context?.Id;
		Context = context;
	}

	[Column("id"), Key]
	public int Id { get; private set; }

	[Column("parent"), ForeignKey(nameof(Parent))]
	public int? ParentId { get; set; }

	public TagTable? Parent { get; set; }

	[Column("child"), ForeignKey(nameof(Child))]
	public int? ChildId { get; set; }

	public TagTable? Child { get; set; }

	[Column("context"), ForeignKey(nameof(Context))]
	public int? ContextId { get; set; }

	public TagTable? Context { get; set; }

}
