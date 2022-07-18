using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AsconTestTask.Backend.Data.Members;

public class DataLink
{
	
	[Key] [Column("id")] public int Id { get; set; }
	[Column("idparent")] public DataObject? Parent { get; set; }
	[Column("idchild")] public DataObject? Child { get; set; }
	[Column("linkname")] public string LinkName { get; set; }
}
