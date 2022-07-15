namespace AsconTestTask.Backend.Data.Members;

public class DataObject
{
	public int Id { get; }
	public string Type { get; }
	public string Product { get; }
	public List<DataLink> Links { get; } 
	public List<DataAttribute> Attributes { get; }
}
