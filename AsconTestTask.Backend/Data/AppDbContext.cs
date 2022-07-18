using AsconTestTask.Backend.Data.Members;
using Microsoft.EntityFrameworkCore;

namespace AsconTestTask.Backend.Data;

public sealed class AppDbContext : DbContext
{
	public DbSet<DataAttribute> Attributes { get; set; } = null!;
	public DbSet<DataLink> Links { get; set; } = null!;
	public DbSet<DataObject> Objects { get; set; } = null!;

	public AppDbContext()
	{ 
		//Database.EnsureDeleted();
		Database.EnsureCreated();
	}
	
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		IConnectionStringProvider connectionStringProvider = new EmbeddedConstConnectionStringProvider();
		optionsBuilder.EnableSensitiveDataLogging();
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseSqlServer(connectionStringProvider.GetConnectionString());
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<DataObject>().HasMany(obj => obj.LinksAsChild).WithOne(link => link.Child)
			.OnDelete(DeleteBehavior.NoAction);
		modelBuilder.Entity<DataObject>().HasMany(obj => obj.LinksAsParent).WithOne(link => link.Parent)
			.OnDelete(DeleteBehavior.NoAction);
	}
}