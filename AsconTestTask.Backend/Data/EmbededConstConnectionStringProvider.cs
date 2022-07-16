namespace AsconTestTask.Backend.Data;

public class EmbeddedConstConnectionStringProvider : IConnectionStringProvider
{
	public string GetConnectionString()
	{
		return "Server=(localdb)\\mssqllocaldb;Database=asconTTAppDb;Trusted_Connection=True;";
	}
}
