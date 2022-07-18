using AsconTestTask.Backend.Data.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AsconTestTask.Backend.Data;

public sealed class AppDbContext : DbContext
{
	private const string ConfigFilePath = "appsettings.json";
	public DbSet<DataAttribute> Attributes { get; set; } = null!;
	public DbSet<DataLink> Links { get; set; } = null!;
	public DbSet<DataObject> Objects { get; set; } = null!;

	private readonly string _connectionString;
	
	/// <param name="connectionString">if null, the connection string from the configuration file will be used</param>
	public AppDbContext(string? connectionString = null)
	{
		_connectionString = connectionString ?? GetConnectionStringFromConfig();
		//Database.EnsureDeleted();
		Database.EnsureCreated();
	}
	
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.EnableSensitiveDataLogging();
		if (!optionsBuilder.IsConfigured)
		{
			optionsBuilder.UseSqlServer(_connectionString);
		}
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfiguration(new DataObjectConfiguration());
		modelBuilder.ApplyConfiguration(new DataLinkConfiguration());
	}

	private string GetConnectionStringFromConfig()
	{
		var builder = new ConfigurationBuilder();
		builder.SetBasePath(Directory.GetCurrentDirectory());
		builder.AddJsonFile(ConfigFilePath);
		IConfigurationRoot? config = builder.Build();
		var connectionString = config.GetConnectionString("DefaultConnection");
		if (string.IsNullOrEmpty(connectionString))
			throw new Exception("Invalid DefaultConnection value in appsettings.json");
		return connectionString;
	}
}