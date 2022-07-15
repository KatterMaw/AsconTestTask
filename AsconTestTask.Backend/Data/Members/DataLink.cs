using Microsoft.EntityFrameworkCore;

namespace AsconTestTask.Backend.Data.Members;

[Keyless]
public class DataLink
{
	public DataObject Parent { get; }
	public DataObject Child { get; }
	public string LinkName { get; }
}
