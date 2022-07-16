using Microsoft.EntityFrameworkCore;

namespace AsconTestTask.Backend.Data.Members;

public class DataLink
{
	public int ParentId { get; set; }
	public DataObject Parent { get; set; }
	public int ChildId { get; set; }
	public DataObject Child { get; set; }
	public string LinkName { get; set; }
}
