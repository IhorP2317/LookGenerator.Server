
using LookGenerator.Application.Features.AttributeOptions.GetAttributeOptions;
using LookGenerator.Domain.Entities;
using LookGenerator.Persistence.Data; // Adjust namespace as needed
using Microsoft.EntityFrameworkCore;


namespace LookGeneration.Application.UnitTests.Features.AttributeOptions;

public class GetAttributeOptionsTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly GetAttributeOptionsHandler _handler;

    public GetAttributeOptionsTests()
    {
           
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"AttributeOptionsTestDb_{Guid.NewGuid()}")
            .Options;

           
        _dbContext = new ApplicationDbContext(options);
            
           
        _handler = new GetAttributeOptionsHandler(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task Handle_ShouldReturnAllAttributeOptions()
    {
        // Arrange
        var attributeOptions = new List<AttributeOption>
        {
            new() { Id = Guid.NewGuid(), Name = "Option 1", AttributeTypeId = Guid.NewGuid() },
            new() { Id = Guid.NewGuid(), Name = "Option 2", AttributeTypeId = Guid.NewGuid() }
        };

        _dbContext.AttributeOptions.AddRange(attributeOptions);
        await _dbContext.SaveChangesAsync();

        var query = new GetAttributeOptionsQuery();

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Name == "Option 1");
        Assert.Contains(result, x => x.Name == "Option 2");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyCollection_WhenNoAttributeOptionsExist()
    {
        // Arrange - no setup needed as DB is empty
        var query = new GetAttributeOptionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_ShouldIncludeAttributeTypes_WhenDataExists()
    {
        // Arrange
        var attributeTypeId = Guid.NewGuid();
        var attributeType = new AttributeType { Id = attributeTypeId, Name = "Color" };
            
        _dbContext.AttributeTypes.Add(attributeType);
        await _dbContext.SaveChangesAsync();
            
        var attributeOption = new AttributeOption 
        { 
            Id = Guid.NewGuid(), 
            Name = "Red", 
            AttributeTypeId = attributeTypeId
        };
            
        _dbContext.AttributeOptions.Add(attributeOption);
        await _dbContext.SaveChangesAsync();

        var query = new GetAttributeOptionsQuery();

        // Act
        var result = (await _handler.Handle(query, CancellationToken.None)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(attributeTypeId, result.First().AttributeTypeId);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyResult_WhenDatabaseIsCleared()
    {
        // Arrange - Add data
        var attributeOption = new AttributeOption 
        { 
            Id = Guid.NewGuid(), 
            Name = "Option", 
            AttributeTypeId = Guid.NewGuid() 
        };
            
        _dbContext.AttributeOptions.Add(attributeOption);
        await _dbContext.SaveChangesAsync();
            
        // Clear the database
        _dbContext.AttributeOptions.RemoveRange(_dbContext.AttributeOptions);
        await _dbContext.SaveChangesAsync();
            
        var query = new GetAttributeOptionsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}