using System.Linq;
using AsconTestTask.Backend.Data;
using AsconTestTask.Backend.Data.Members;
using Xunit;

namespace AsconTestTask.Tests.Data;

public class AppDbTests
{
	private const string connectionString = "Server=(localdb)\\mssqllocaldb;Database=testDb;Trusted_Connection=True;";
	
	[Fact]
	public void LinkAddedToDbWillAppearTroughObjects()
	{
		// assign
		var firstObject = new DataObject();
		var secondObject = new DataObject();
		var link = new DataLink {Parent = firstObject, Child = secondObject};
		using var dbContext = new AppDbContext();
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
		using var dbContext = new AppDbContext();
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
		using var dbContext = new AppDbContext();
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
		using var dbContext = new AppDbContext();
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
		using var dbContext = new AppDbContext();
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
}
