using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsconTestTask.Backend.Data.Members;

public class DataLink
{
	[Column("idparent")] public int ParentId { get; set; }
	public DataObject Parent { get; set; }
	[Column("idchild")] public int ChildId { get; set; }
	public DataObject Child { get; set; }
	[Column("linkname")] public string LinkName { get; set; }
}

public class DataLinkConfiguration : IEntityTypeConfiguration<DataLink>
{
	public void Configure(EntityTypeBuilder<DataLink> dataLink)
	{
		dataLink.HasOne(link => link.Parent).WithMany(obj => obj.LinksAsParent).HasForeignKey(link => link.ParentId).OnDelete(DeleteBehavior.SetNull);
		dataLink.HasOne(link => link.Child).WithMany(obj => obj.LinksAsChild).HasForeignKey(link => link.ChildId).OnDelete(DeleteBehavior.SetNull);
	}
}