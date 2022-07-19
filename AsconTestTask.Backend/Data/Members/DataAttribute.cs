using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsconTestTask.Backend.Data.Members;

public class DataAttribute
{
	[Key] [Column("selfid")]  public int Id { get; set; } // using EF unique key is required
	[Column("id")] public DataObject Obj { get; set; }
	[Column("name")] public string Name { get; set; } = string.Empty;
	[Column("Value")] public string Value { get; set; } = string.Empty; // The name of the column according to the technical specification
}