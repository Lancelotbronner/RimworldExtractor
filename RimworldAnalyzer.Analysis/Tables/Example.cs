using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RimworldAnalyzer.Analysis;

[Table("examples"), Index(nameof(Value), IsUnique = true)]
public sealed class ExampleTable {

	private ExampleTable() {
		Value = null!;
	}

	public ExampleTable(string value) {
		Value = value;
	}

	[Column("id"), Key]
	public int Id { get; private set; }

	[Column("value")]
	public string Value { get; set; }

	[InverseProperty(nameof(TagExampleTable.Example))]
	public ICollection<TagExampleTable>? Tags { get; private set; }

	[InverseProperty(nameof(AttributeExampleTable.Example))]
	public ICollection<AttributeExampleTable>? Attributes { get; private set; }

}
