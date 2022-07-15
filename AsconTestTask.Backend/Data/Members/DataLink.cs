using Microsoft.EntityFrameworkCore;

namespace AsconTestTask.Backend.Data.Members;

[Keyless]
public class DataLink
{
	public DataLink(int parentId, int childId, string linkName)
	{
		ParentId = parentId;
		ChildId = childId;
		LinkName = linkName;
	}

	public int ParentId { get; }
	public int ChildId { get; }
	public string LinkName { get; }
}
