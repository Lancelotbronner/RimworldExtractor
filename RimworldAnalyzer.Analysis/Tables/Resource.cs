using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RimworldAnalyzer.Analysis;

[Table("resources")]
public sealed class ResourceTable {

	[Column("id"), Key]
	public int Id { get; private set; }

	[Column("path"), MaxLength(256)]
	public string Path { get; set; } = null!;

	[Column("module"), ForeignKey(nameof(Module))]
	public int? ModuleId { get; set; }

	/// <summary>
	/// The module in which the resource is located
	/// </summary>
	public ModuleTable? Module { get; set; }

	[InverseProperty(nameof(DefinitionTable.Resource))]
	public ICollection<DefinitionTable>? Definitions { get; private set; }

	[InverseProperty(nameof(IssueTable.Resource))]
	public ICollection<IssueTable>? Issues { get; private set; }

}
