using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("definitions"), Index(nameof(Identifier), IsUnique = true)]
public sealed class DefinitionTable {

	private DefinitionTable() {
		Identifier = null!;
	}

	public DefinitionTable(string identifier) {
		Identifier = identifier;
	}

	[Column("id"), Key]
	public int Id { get; private set; }

	/// <summary>
	/// The unique identifier of the definition
	/// </summary>
	[Column("identifier"), MaxLength(128)]
	public string Identifier { get; set; }

	/// <summary>
	/// Wether this definition can have concrete instances
	/// </summary>
	[Column("abstract")]
	public bool IsAbstract { get; set; }

	[Column("parent"), ForeignKey(nameof(Parent))]
	public int? ParentId { get; set; }

	/// <summary>
	/// The parent from which values are inherited
	/// </summary>
	public DefinitionTable? Parent { get; set; }

	[Column("module"), ForeignKey(nameof(Module))]
	public int? ModuleId { get; set; }

	/// <summary>
	/// The module in which the definition was declared
	/// </summary>
	public ModuleTable? Module { get; set; }

	[Column("class"), ForeignKey(nameof(Class))]
	public int? ClassId { get; set; }

	/// <summary>
	/// The base class of this definition
	/// </summary>
	public ClassTable? Class { get; set; }

	[Column("resource"), ForeignKey(nameof(Resource))]
	public int? ResourceId { get; set; }

	/// <summary>
	/// The resource in which this definition was created
	/// </summary>
	public ResourceTable? Resource { get; set; }

	/// <summary>
	/// The issues pertaining to this definition
	/// </summary>
	[InverseProperty(nameof(IssueTable.Definition))]
	public ICollection<IssueTable>? Issues { get; private set; }

	/// <summary>
	/// The C# class style declaration of this definition.
	/// </summary>
	public string ToDeclarationString() {
		StringBuilder result = new(256);
		if (IsAbstract)
			result.Append("abstract ");
		result.Append(Class?.Identifier ?? $"<fault {ClassId}>");
		result.Append(' ');
		result.Append(Identifier);
		if (ParentId is int parent) {
			result.Append(": ");
			result.Append(Parent?.Identifier ?? $"<fault {parent}>");
		}
		return result.ToString();
	}

	public string ToLocationString() {
		string module = Module?.Identifier ?? $"<fault {ModuleId}>";
		string resource = Resource?.Path ?? $"<fault {ResourceId}>";
		return Path.Join(module, resource);
	}

	public override string ToString()
		=> $"[{ToLocationString()}] {ToDeclarationString()}";

}
