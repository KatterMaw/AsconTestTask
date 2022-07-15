using Microsoft.EntityFrameworkCore;

namespace AsconTestTask.Backend.Data.Members;

[Keyless]
public class DataLink
{
	public DataMember Parent { get; }
	public DataMember Child { get; }
	public string LinkName { get; }
}
