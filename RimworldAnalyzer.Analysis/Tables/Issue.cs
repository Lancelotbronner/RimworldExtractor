using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RimworldAnalyzer.Analysis;

[Table("issues")]
public sealed class IssueTable {

	[Column("Id"), Key]
	public int Id { get; set; }

	[Column("severity")]
	public required IssueSeverity Severity { get; set; }

	[Column("message")]
	public required string Message { get; set; } = null!;

	[Column("attribute"), ForeignKey(nameof(Attribute))]
	public int? AttributeId { get; set; }

	public AttributeTable? Attribute { get; set; }

	[Column("class"), ForeignKey(nameof(Class))]
	public int? ClassId { get; set; }

	public ClassTable? Class { get; set; }

	[Column("definition"), ForeignKey(nameof(Definition))]
	public int? DefinitionId { get; set; }

	public DefinitionTable? Definition { get; set; }

	[Column("module"), ForeignKey(nameof(Module))]
	public int? ModuleId { get; set; }

	public ModuleTable? Module { get; set; }

	[Column("resource"), ForeignKey(nameof(Resource))]
	public int? ResourceId { get; set; }

	public ResourceTable? Resource { get; set; }

	[Column("tag"), ForeignKey(nameof(Tag))]
	public int? TagId { get; set; }

	public TagTable? Tag { get; set; }

}

public enum IssueSeverity : byte {

	Trace = 1,
	Debug = 2,
	Information = 3,
	Warning = 4,
	Error = 5,
	Critical = 6,

}
