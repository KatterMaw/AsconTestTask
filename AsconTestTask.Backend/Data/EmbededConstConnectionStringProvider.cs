namespace AsconTestTask.Backend.Data;

public class EmbeddedConstConnectionStringProvider : IConnectionStringProvider
{
	public string GetConnectionString()
	{
		return "Server=(localdb)\\mssqllocaldb;Database=asconApp;Trusted_Connection=True;";
	}
}
