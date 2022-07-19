using AsconTestTask.Backend.Data.Members;

namespace AsconTestTask.Backend.Data;

public class DbDump
{
	public DbDump(List<DataObject> objects, List<DataLink> links, List<DataAttribute> attributes)
	{
		Objects = objects;
		Links = links;
		Attributes = attributes;
	}

	public List<DataObject> Objects { get; }
	public List<DataLink> Links { get; }
	public List<DataAttribute> Attributes { get; }
}
