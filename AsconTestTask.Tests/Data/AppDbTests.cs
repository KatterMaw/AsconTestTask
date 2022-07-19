using System.Linq;
using AsconTestTask.Backend.Data;
using AsconTestTask.Backend.Data.Members;
using Xunit;

namespace AsconTestTask.Tests.Data;

public class AppDbTests
{
	private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=testDb;Trusted_Connection=True;";
	
	[Fact]
	public void LinkAddedToDbWillAppearTroughObjects()
	{
		// assign
		var firstObject = new DataObject();
		var secondObject = new DataObject();
		var link = new DataLink {Parent = firstObject, Child = secondObject};
		using var dbContext = new AppDbContext(ConnectionString);
		dbContext.Database.EnsureDeleted();
		dbContext.Database.EnsureCreated();

		// act
		dbContext.Add(firstObject);
		dbContext.Add(secondObject);
		dbContext.Add(link);
		dbContext.SaveChanges();
		
		// assert
		Assert.Contains(link, firstObject.LinksAsParent);
		Assert.Contains(link, secondObject.LinksAsChild);
		
		// clean-up
		dbContext.Database.EnsureDeleted();
	}

	[Fact]
	public void DeletingLinkDoesNotDeleteObjects()
	{
		// assign
		var firstObject = new DataObject();
		var secondObject = new DataObject();
		var link = new DataLink {Parent = firstObject, Child = secondObject};
		using var dbContext = new AppDbContext(ConnectionString);
		dbContext.Database.EnsureDeleted();
		dbContext.Database.EnsureCreated();
		dbContext.Add(firstObject);
		dbContext.Add(secondObject);
		dbContext.Add(link);
		dbContext.SaveChanges();

		// act
		dbContext.Remove(link);
		
		// assert
		Assert.Contains(firstObject, dbContext.Objects);
		Assert.Contains(secondObject, dbContext.Objects);
		
		// clean-up
		dbContext.Database.EnsureDeleted();
	}

	[Fact]
	public void DeletingLinkRemovesLinkInObjectFromTheNavigationProperty()
	{
		// assign
		var firstObject = new DataObject();
		var secondObject = new DataObject();
		var link = new DataLink {Parent = firstObject, Child = secondObject};
		using var dbContext = new AppDbContext(ConnectionString);
		dbContext.Database.EnsureDeleted();
		dbContext.Database.EnsureCreated();
		dbContext.Add(firstObject);
		dbContext.Add(secondObject);
		dbContext.Add(link);
		dbContext.SaveChanges();

		// act
		dbContext.Links.Remove(link);
		dbContext.SaveChanges();
		
		// assert
		Assert.DoesNotContain(link, firstObject.LinksAsParent);
		Assert.DoesNotContain(link, secondObject.LinksAsChild);
		
		// clean-up
		dbContext.Database.EnsureDeleted();
	}

	[Fact]
	public void DeletingParentWillNotDeleteChild()
	{
		// assign
		var firstObject = new DataObject();
		var secondObject = new DataObject();
		var link = new DataLink {Parent = firstObject, Child = secondObject};
		using var dbContext = new AppDbContext(ConnectionString);
		dbContext.Database.EnsureDeleted();
		dbContext.Database.EnsureCreated();
		dbContext.Add(firstObject);
		dbContext.Add(secondObject);
		dbContext.Add(link);
		dbContext.SaveChanges();

		// act
		dbContext.Objects.Remove(firstObject);
		dbContext.SaveChanges();
		
		// assert
		Assert.Contains(secondObject, dbContext.Objects);

		// clean-up
		dbContext.Database.EnsureDeleted();
	}

	[Fact]
	public void DeletingParentWillDeleteLink()
	{
		// assign
		var firstObject = new DataObject();
		var secondObject = new DataObject();
		var link = new DataLink {Parent = firstObject, Child = secondObject};
		using var dbContext = new AppDbContext(ConnectionString);
		dbContext.Database.EnsureDeleted();
		dbContext.Database.EnsureCreated();
		dbContext.Add(firstObject);
		dbContext.Add(secondObject);
		dbContext.Add(link);
		dbContext.SaveChanges();

		// act
		dbContext.Objects.Remove(firstObject);
		dbContext.SaveChanges();
		
		// assert
		Assert.DoesNotContain(link, dbContext.Links);

		// clean-up
		dbContext.Database.EnsureDeleted();
	}

	[Fact]
	public void CreateDumpMethodResultContainExpectedEntities()
	{
		// assign
		var firstObject = new DataObject();
		var secondObject = new DataObject();
		var link = new DataLink {Parent = firstObject, Child = secondObject};
		var attribute = new DataAttribute {Obj = firstObject};
		using var dbContext = new AppDbContext(ConnectionString);
		dbContext.Database.EnsureDeleted();
		dbContext.Database.EnsureCreated();
		dbContext.Add(firstObject);
		dbContext.Add(secondObject);
		dbContext.Add(link);
		dbContext.Add(attribute);
		dbContext.SaveChanges();

		// act
		DbDump dump = dbContext.CreateDump();
		
		// assert
		Assert.Contains(firstObject, dump.Objects);
		Assert.Contains(secondObject, dump.Objects);
		Assert.Contains(link, dump.Links);
		Assert.Contains(attribute, dump.Attributes);

		// clean-up
		dbContext.Database.EnsureDeleted();
	}
}
