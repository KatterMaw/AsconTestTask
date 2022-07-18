using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsconTestTask.Backend.Data.Members;

public class DataObject
{
	[Column("id")] public int Id { get; set; }
	[Column("type")] public string Type { get; set; } = string.Empty;
	[Column("product")] public string Product { get; set; } = string.Empty;
	public List<DataLink>? LinksAsChild { get; set; } = new();
	public List<DataLink>? LinksAsParent { get; set; } = new();
	public List<DataAttribute> Attributes { get; set; } = new();
}

public class DataObjectConfiguration : IEntityTypeConfiguration<DataObject>
{
	public void Configure(EntityTypeBuilder<DataObject> dataObject)
	{
		dataObject.HasMany(obj => obj.LinksAsParent).WithOne().IsRequired(false).OnDelete(DeleteBehavior.Cascade);
		dataObject.HasMany(obj => obj.LinksAsChild).WithOne().IsRequired(false).OnDelete(DeleteBehavior.Cascade);
	}
}