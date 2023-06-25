using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("modules"), Index(nameof(Identifier), IsUnique = true)]
public sealed class ModuleTable {

	private ModuleTable() {
		Identifier = null!;
	}

	public ModuleTable(string identifier) {
		Identifier = identifier;
	}

	[Column("id"), Key]
	public int Id { get; set; }

	[Column("identifier"), MaxLength(64)]
	public string Identifier { get; init; }

	[Column("name"), MaxLength(64)]
	public string? Name { get; set; }

	[Column("version"), MaxLength(32)]
	public string? Version { get; set; }

	[Column("official")]
	public bool IsOfficial { get; set; }

	[InverseProperty(nameof(AttributeTable.Module))]
	public ICollection<AttributeTable>? Attributes { get; private set; }

	[InverseProperty(nameof(ClassTable.Module))]
	public ICollection<ClassTable>? Classes { get; private set; }

	[InverseProperty(nameof(TagTable.Module))]
	public ICollection<TagTable>? Tags { get; private set; }

	[InverseProperty(nameof(DefinitionTable.Module))]
	public ICollection<DefinitionTable>? Definitions { get; private set; }

	[InverseProperty(nameof(IssueTable.Module))]
	public ICollection<IssueTable>? Issues { get; private set; }

}
