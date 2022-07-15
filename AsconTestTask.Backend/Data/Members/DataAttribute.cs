namespace AsconTestTask.Backend.Data.Members;

public class DataAttribute
{
	public DataAttribute(int id, string name, string value)
	{
		Id = id;
		Name = name;
		Value = value;
	}

	public int Id { get; }
	public string Name { get; }
	public string Value { get; }
}
