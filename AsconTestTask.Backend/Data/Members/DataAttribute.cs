using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsconTestTask.Backend.Data.Members;

public class DataAttribute
{
	[Key] [Column("attributeId")]  public int Id { get; set; } // using EF unique key is required
	[Column("id")] public int ObjectId { get; set; }
	public DataObject Object { get; set; }
	[Column("name")] public string Name { get; set; } = string.Empty;
	[Column("Value")] public string Value { get; set; } = string.Empty; // The name of the column according to the technical specification
}

public class DataAttributeConfiguration : IEntityTypeConfiguration<DataAttribute>
{
	public void Configure(EntityTypeBuilder<DataAttribute> dataAttribute)
	{
		dataAttribute.HasOne(attribute => attribute.Object).WithMany(obj => obj.Attributes).HasForeignKey(attribute => attribute.ObjectId).OnDelete(DeleteBehavior.Cascade);
	}
}