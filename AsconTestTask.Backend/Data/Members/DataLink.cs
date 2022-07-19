using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsconTestTask.Backend.Data.Members;

[Serializable]
public class DataLink
{
	[Column("idparent")] public int ParentId { get; set; }
	[XmlIgnore] public DataObject Parent { get; set; }
	[Column("idchild")] public int ChildId { get; set; }
	[XmlIgnore] public DataObject Child { get; set; }
	[Column("linkname")] public string LinkName { get; set; } = string.Empty;
}

public class DataLinkConfiguration : IEntityTypeConfiguration<DataLink>
{
	public void Configure(EntityTypeBuilder<DataLink> dataLink)
	{
		dataLink.HasKey(link => new {link.ParentId, link.ChildId});
		dataLink.HasOne(link => link.Parent).WithMany(obj => obj.LinksAsParent).HasForeignKey(link => link.ParentId).IsRequired().OnDelete(DeleteBehavior.ClientCascade);
		dataLink.HasOne(link => link.Child).WithMany(obj => obj.LinksAsChild).HasForeignKey(link => link.ChildId).IsRequired().OnDelete(DeleteBehavior.ClientCascade);
	}
}