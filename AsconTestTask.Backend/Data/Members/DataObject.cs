namespace AsconTestTask.Backend.Data.Members;

public class DataObject
{
	public DataObject(int id, string type, string product)
	{
		Id = id;
		Type = type;
		Product = product;
	}

	public int Id { get; }
	public string Type { get; }
	public string Product { get; }
}
