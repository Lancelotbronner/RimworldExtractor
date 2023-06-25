using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("attributes"), Index(nameof(Identifier), IsUnique = true)]
public sealed class AttributeTable {

	private AttributeTable() {
		Identifier = null!;
	}

	public AttributeTable(string identifier) {
		Identifier = identifier;
		UpdateName();
	}

	[Column("id"), Key]
	public int Id { get; private set; }

	[Column("identifier"), MaxLength(128)]
	public string Identifier { get; set; }

	[Column("name"), MaxLength(128)]
	public string? Name { get; set; }

	[Column("module"), ForeignKey(nameof(Module))]
	public int? ModuleId { get; set; }

	[InverseProperty(nameof(ModuleTable.Attributes))]
	public ModuleTable? Module { get; set; }

	[InverseProperty(nameof(IssueTable.Attribute))]
	public ICollection<IssueTable>? Issues { get; private set; }

	/// <summary>
	/// Updates the attribute name based on its identifier
	/// </summary>
	public void UpdateName() {
		Name = Identifier.ToTitleCase();
	}

}
