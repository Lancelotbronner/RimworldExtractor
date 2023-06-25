using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("classes"), Index(nameof(Identifier), IsUnique = true)]
public sealed class ClassTable {

	private ClassTable() {
		Identifier = null!;
	}

	public ClassTable(string identifier) {
		Identifier = identifier;
		UpdateName();
	}

	[Column("id"), Key]
	public int Id { get; private set; }

	/// <summary>
	/// The identifier of the class in XML and C#
	/// </summary>
	[Column("identifier"), MaxLength(128)]
	public string Identifier { get; set; }

	/// <summary>
	/// The display name of the class
	/// </summary>
	[Column("name"), MaxLength(128)]
	public string? Name { get; set; }

	[Column("module"), ForeignKey(nameof(Module))]
	public int? ModuleId { get; set; }

	/// <summary>
	/// The module in which the class was declared
	/// </summary>
	public ModuleTable? Module { get; set; }

	/// <summary>
	/// The definitions using this class
	/// </summary>
	[InverseProperty(nameof(DefinitionTable.Class))]
	public ICollection<DefinitionTable>? Definitions { get; private set; }

	/// <summary>
	/// The issues pertaining to this class
	/// </summary>
	[InverseProperty(nameof(IssueTable.Class))]
	public ICollection<IssueTable>? Issues { get; private set; }

	/// <summary>
	/// Updates the class name based on its identifier
	/// </summary>
	public void UpdateName() {
		Name = Identifier.ToTitleCase();
		Name = RimworldAnalysisExtensions.DetectDefinitionAbbreviation().Replace(Name, "Definition");
	}

}
