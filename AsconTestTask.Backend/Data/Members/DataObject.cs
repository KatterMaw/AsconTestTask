namespace AsconTestTask.Backend.Data.Members;

public class DataObject
{
	public int Id { get; set; }
	public string Type { get; set; } = string.Empty;
	public string Product { get; set; } = string.Empty;
	public List<DataLink> LinksAsChild { get; set; } = new();
	public List<DataLink> LinksAsParent { get; set; } = new();
	public List<DataAttribute> Attributes { get; set; } = new();
}
