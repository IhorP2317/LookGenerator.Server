using LookGenerator.Application.Features.Looks.Update;
using LookGenerator.Domain.Entities;
using LookGenerator.Persistence.Data;
using Microsoft.EntityFrameworkCore;


namespace LookGeneration.Application.UnitTests.Features.Looks;

public class UpdateLookTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UpdateLookHandler _handler;

    public UpdateLookTests()
    {
        // Create in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"UpdateLookTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _handler = new UpdateLookHandler(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    private async Task<Look> CreateTestLook(string name = "Initial Look", string description = "Initial Description", 
        string colorPalette = "Initial Palette", LookStatus status = LookStatus.Draft, 
        List<ProductVariation>? productVariations = null)
    {
        productVariations ??= [];
            
        // Ensure the product variations are in the database
        if (productVariations.Any())
        {
            _dbContext.ProductVariations.AddRange(productVariations);
            await _dbContext.SaveChangesAsync();
        }

        var look = new Look
        {
            Name = name,
            Description = description,
            ColorPalette = colorPalette,
            LookStatus = status,
            LookProductVariations = productVariations
                .Select(pv => new LookProductVariation
                {
                    ProductVariationId = pv.Id
                })
                .ToList()
        };

        _dbContext.Looks.Add(look);
        await _dbContext.SaveChangesAsync();
        return look;
    }

    [Fact]
    public async Task Handle_ShouldUpdateBasicProperties()
    {
        // Arrange
        var initialLook = await CreateTestLook();

        var command = new UpdateLookCommand(
            Id: initialLook.Id,
            Name: "Updated Look",
            Description: "Updated Description",
            ColorPalette: "Updated Palette",
            Status: LookStatus.Private,
            ProductVariationIds: new List<Guid>()
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedLook = await _dbContext.Looks
            .FirstAsync(l => l.Id == initialLook.Id);

        Assert.Equal("Updated Look", updatedLook.Name);
        Assert.Equal("Updated Description", updatedLook.Description);
        Assert.Equal("Updated Palette", updatedLook.ColorPalette);
        Assert.Equal(LookStatus.Private, updatedLook.LookStatus);
    }

    [Fact]
    public async Task Handle_ShouldUpdateToNullDescription()
    {
        // Arrange
        var initialLook = await CreateTestLook(description: "Initial Description");

        var command = new UpdateLookCommand(
            Id: initialLook.Id,
            Name: "Test Look",
            Description: null, // Setting description to null
            ColorPalette: "Red",
            Status: LookStatus.Draft,
            ProductVariationIds: new List<Guid>()
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedLook = await _dbContext.Looks
            .FirstAsync(l => l.Id == initialLook.Id);

        Assert.Null(updatedLook.Description);
    }

    [Fact]
    public async Task Handle_ShouldAddNewProductVariations()
    {
        // Arrange
        var initialVariations = new List<ProductVariation>
        {
            new() { Id = Guid.NewGuid(), Size = "S" }
        };

        var initialLook = await CreateTestLook(productVariations: initialVariations);

        var newVariation = new ProductVariation { Id = Guid.NewGuid(), Size = "M" };
        _dbContext.ProductVariations.Add(newVariation);
        await _dbContext.SaveChangesAsync();

        var allVariationIds = new List<Guid>
        {
            initialVariations[0].Id,
            newVariation.Id
        };

        var command = new UpdateLookCommand(
            Id: initialLook.Id,
            Name: initialLook.Name,
            Description: initialLook.Description,
            ColorPalette: initialLook.ColorPalette,
            Status: initialLook.LookStatus,
            ProductVariationIds: allVariationIds
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedLook = await _dbContext.Looks
            .Include(l => l.LookProductVariations)
            .FirstAsync(l => l.Id == initialLook.Id);

        Assert.Equal(2, updatedLook.LookProductVariations.Count);
        Assert.Contains(updatedLook.LookProductVariations, lpv => lpv.ProductVariationId == initialVariations[0].Id);
        Assert.Contains(updatedLook.LookProductVariations, lpv => lpv.ProductVariationId == newVariation.Id);
    }

    [Fact]
    public async Task Handle_ShouldRemoveExistingProductVariations()
    {
        // Arrange
        var initialVariations = new List<ProductVariation>
        {
            new() { Id = Guid.NewGuid(), Size = "S" },
            new() { Id = Guid.NewGuid(), Size = "M" }
        };

        var initialLook = await CreateTestLook(productVariations: initialVariations);

        // Only keep the first variation
        var command = new UpdateLookCommand(
            Id: initialLook.Id,
            Name: initialLook.Name,
            Description: initialLook.Description,
            ColorPalette: initialLook.ColorPalette,
            Status: initialLook.LookStatus,
            ProductVariationIds: new List<Guid> { initialVariations[0].Id }
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedLook = await _dbContext.Looks
            .Include(l => l.LookProductVariations)
            .FirstAsync(l => l.Id == initialLook.Id);

        Assert.Single(updatedLook.LookProductVariations);
        Assert.Contains(updatedLook.LookProductVariations, lpv => lpv.ProductVariationId == initialVariations[0].Id);
        Assert.DoesNotContain(updatedLook.LookProductVariations, lpv => lpv.ProductVariationId == initialVariations[1].Id);
    }

    [Fact]
    public async Task Handle_ShouldReplaceAllProductVariations()
    {
        // Arrange
        var initialVariations = new List<ProductVariation>
        {
            new() { Id = Guid.NewGuid(), Size = "S" }
        };

        var initialLook = await CreateTestLook(productVariations: initialVariations);

        var newVariation = new ProductVariation { Id = Guid.NewGuid(), Size = "L" };
        _dbContext.ProductVariations.Add(newVariation);
        await _dbContext.SaveChangesAsync();

        // Completely replace the variations
        var command = new UpdateLookCommand(
            Id: initialLook.Id,
            Name: initialLook.Name,
            Description: initialLook.Description,
            ColorPalette: initialLook.ColorPalette,
            Status: initialLook.LookStatus,
            ProductVariationIds: new List<Guid> { newVariation.Id }
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedLook = await _dbContext.Looks
            .Include(l => l.LookProductVariations)
            .FirstAsync(l => l.Id == initialLook.Id);

        Assert.Single(updatedLook.LookProductVariations);
        Assert.DoesNotContain(updatedLook.LookProductVariations, lpv => lpv.ProductVariationId == initialVariations[0].Id);
        Assert.Contains(updatedLook.LookProductVariations, lpv => lpv.ProductVariationId == newVariation.Id);
    }

    [Fact]
    public async Task Handle_ShouldUpdateStatus()
    {
        // Arrange
        var initialLook = await CreateTestLook(status: LookStatus.Draft);

        var command = new UpdateLookCommand(
            Id: initialLook.Id,
            Name: initialLook.Name,
            Description: initialLook.Description,
            ColorPalette: initialLook.ColorPalette,
            Status: LookStatus.Private, // Change status
            ProductVariationIds: new List<Guid>()
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedLook = await _dbContext.Looks
            .FirstAsync(l => l.Id == initialLook.Id);

        Assert.Equal(LookStatus.Private, updatedLook.LookStatus);
    }

    [Fact]
    public async Task Handle_ShouldUpdateAllPropertiesAtOnce()
    {
        // Arrange
        var initialVariations = new List<ProductVariation>
        {
            new() { Id = Guid.NewGuid(), Size = "S" }
        };

        var initialLook = await CreateTestLook(
            name: "Initial Name",
            description: "Initial Description",
            colorPalette: "Initial Palette",
            status: LookStatus.Draft,
            productVariations: initialVariations
        );

        var newVariation = new ProductVariation { Id = Guid.NewGuid(), Size = "XL" };
        _dbContext.ProductVariations.Add(newVariation);
        await _dbContext.SaveChangesAsync();

        var command = new UpdateLookCommand(
            Id: initialLook.Id,
            Name: "Completely Updated Name",
            Description: "Completely Updated Description",
            ColorPalette: "Completely Updated Palette",
            Status: LookStatus.Public,
            ProductVariationIds: new List<Guid> { newVariation.Id }
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedLook = await _dbContext.Looks
            .Include(l => l.LookProductVariations)
            .FirstAsync(l => l.Id == initialLook.Id);

        Assert.Equal("Completely Updated Name", updatedLook.Name);
        Assert.Equal("Completely Updated Description", updatedLook.Description);
        Assert.Equal("Completely Updated Palette", updatedLook.ColorPalette);
        Assert.Equal(LookStatus.Public, updatedLook.LookStatus);
        Assert.Single(updatedLook.LookProductVariations);
        Assert.Contains(updatedLook.LookProductVariations, lpv => lpv.ProductVariationId == newVariation.Id);
    }

    [Fact]
    public async Task Handle_ShouldMaintainUnchangedProductVariations()
    {
        // Arrange
        var initialVariations = new List<ProductVariation>
        {
            new() { Id = Guid.NewGuid(), Size = "S" },
            new() { Id = Guid.NewGuid(), Size = "M" }
        };

        var initialLook = await CreateTestLook(productVariations: initialVariations);

        // Pass the same variation IDs
        var command = new UpdateLookCommand(
            Id: initialLook.Id,
            Name: "Updated Name",
            Description: "Updated Description",
            ColorPalette: "Updated Palette",
            Status: LookStatus.Draft,
            ProductVariationIds: initialVariations.Select(v => v.Id).ToList()
        );

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedLook = await _dbContext.Looks
            .Include(l => l.LookProductVariations)
            .FirstAsync(l => l.Id == initialLook.Id);

        Assert.Equal(2, updatedLook.LookProductVariations.Count);
        Assert.Contains(updatedLook.LookProductVariations, lpv => lpv.ProductVariationId == initialVariations[0].Id);
        Assert.Contains(updatedLook.LookProductVariations, lpv => lpv.ProductVariationId == initialVariations[1].Id);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenLookNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        var command = new UpdateLookCommand(
            Id: nonExistentId,
            Name: "Test",
            Description: "Test",
            ColorPalette: "Test",
            Status: LookStatus.Draft,
            ProductVariationIds: new List<Guid>()
        );

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _handler.Handle(command, CancellationToken.None));
    }
}