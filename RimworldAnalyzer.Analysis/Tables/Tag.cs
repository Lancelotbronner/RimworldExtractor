using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("tags"), Index(nameof(Identifier), IsUnique = true)]
public sealed class TagTable {

	private TagTable() {
		Identifier = null!;
	}

	public TagTable(string identifier) {
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

	public ModuleTable? Module { get; set; }

	[InverseProperty(nameof(Relationship.Child))]
	public ICollection<Relationship>? Parents { get; private set; }

	[InverseProperty(nameof(Relationship.Parent))]
	public ICollection<Relationship>? Children { get; private set; }

	[InverseProperty(nameof(IssueTable.Tag))]
	public ICollection<IssueTable>? Issues { get; private set; }

	/// <summary>
	/// Updates the tag name based on its identifier
	/// </summary>
	public void UpdateName() {
		Name = Identifier.ToTitleCase();
		Name = RimworldAnalysisExtensions.DetectDefinitionAbbreviation().Replace(Name, "Definition");
		Name = RimworldAnalysisExtensions.DetectTextureAbbreviation().Replace(Name, "Texture");
	}

}
