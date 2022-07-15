using AsconTestTask.Backend.Data.Members;
using Microsoft.EntityFrameworkCore;

namespace AsconTestTask.Backend.Data;

public sealed class AppDbContext : DbContext
{
	public DbSet<DataAttribute> Attributes { get; } = null!;
	public DbSet<DataLink> Links { get; } = null!;
	public DbSet<DataObject> Objects { get; } = null!;

	public AppDbContext()
	{
		Database.EnsureCreated();
	}
	
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		IConnectionStringProvider connectionStringProvider = new EmbeddedConstConnectionStringProvider();
		
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseSqlServer(connectionStringProvider.GetConnectionString());
		}
	}
}
