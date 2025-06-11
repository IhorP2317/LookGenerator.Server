
using FluentValidation.TestHelper;
using LookGenerator.Application.Features.Looks.Create;
using LookGenerator.Domain.Entities;
using LookGenerator.Persistence.Data; 
using Microsoft.EntityFrameworkCore;


namespace LookGeneration.Application.UnitTests.Features.Looks;

public class CreateLookTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly CreateLookHandler _handler;
    private readonly CreateLookValidator _validator;

    public CreateLookTests()
    {
        // Create in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"CreateLookTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _handler = new CreateLookHandler(_dbContext);
        _validator = new CreateLookValidator(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task Handle_ShouldCreateLook_WhenCommandIsValid()
    {
        // Arrange
        // Create product variations to reference
        var productVariations = new List<ProductVariation>
        {
            new() { Id = Guid.NewGuid(), Size = "M"},
            new() { Id = Guid.NewGuid(), Size = "L"}
        };

        _dbContext.ProductVariations.AddRange(productVariations);
        await _dbContext.SaveChangesAsync();

        var command = new CreateLookCommand(
            Name: "Test Look",
            Description: "This is a test look",
            ColorPalette: "Red, Blue, Green",
            ProductVariationIds: productVariations.Select(pv => pv.Id).ToList()
        );

        // Act
        var lookId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var createdLook = await _dbContext.Looks
            .Include(l => l.LookProductVariations)
            .FirstOrDefaultAsync(l => l.Id == lookId);

        Assert.NotNull(createdLook);
        Assert.Equal("Test Look", createdLook.Name);
        Assert.Equal("This is a test look", createdLook.Description);
        Assert.Equal("Red, Blue, Green", createdLook.ColorPalette);
        Assert.Equal(LookStatus.Draft, createdLook.LookStatus);
        Assert.Equal(2, createdLook.LookProductVariations.Count);
        Assert.Contains(createdLook.LookProductVariations, lpv => lpv.ProductVariationId == productVariations[0].Id);
        Assert.Contains(createdLook.LookProductVariations, lpv => lpv.ProductVariationId == productVariations[1].Id);
    }

    [Fact]
    public async Task Handle_ShouldDeduplicateProductVariationIds()
    {
        // Arrange
        var productVariationId = Guid.NewGuid();
        var productVariation = new ProductVariation { Id = productVariationId, Size = "S"};

        _dbContext.ProductVariations.Add(productVariation);
        await _dbContext.SaveChangesAsync();

        var command = new CreateLookCommand(
            Name: "Test Look",
            Description: "Look with duplicate product variations",
            ColorPalette: "Red",
            ProductVariationIds: new List<Guid> { productVariationId, productVariationId } // Duplicate IDs
        );

        // Act
        var lookId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var createdLook = await _dbContext.Looks
            .Include(l => l.LookProductVariations)
            .FirstOrDefaultAsync(l => l.Id == lookId);

        Assert.NotNull(createdLook);
        Assert.Single(createdLook.LookProductVariations); // Should contain only one reference
    }

    [Fact]
    public async Task Validator_ShouldFailValidation_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateLookCommand(
            Name: "",
            Description: "Description",
            ColorPalette: "Red",
            ProductVariationIds: new List<Guid>()
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("Name is required.");
    }

    [Fact]
    public async Task Validator_ShouldFailValidation_WhenColorPaletteIsEmpty()
    {
        // Arrange
        var command = new CreateLookCommand(
            Name: "Test Look",
            Description: "Description",
            ColorPalette: "",
            ProductVariationIds: new List<Guid>()
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ColorPalette)
            .WithErrorMessage("Color palette is required.");
    }

    [Fact]
    public async Task Validator_ShouldFailValidation_WhenProductVariationIdsAreInvalid()
    {
        // Arrange
        var nonExistentProductVariationId = Guid.NewGuid();
        var command = new CreateLookCommand(
            Name: "Test Look",
            Description: "Description",
            ColorPalette: "Red",
            ProductVariationIds: new List<Guid> { nonExistentProductVariationId }
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductVariationIds)
            .WithErrorMessage("Some product variations not found.");
    }

    [Fact]
    public async Task Validator_ShouldPassValidation_WhenCommandIsValid()
    {
        // Arrange
        var productVariation = new ProductVariation { Id = Guid.NewGuid(), Size = "M"};
        _dbContext.ProductVariations.Add(productVariation);
        await _dbContext.SaveChangesAsync();

        var command = new CreateLookCommand(
            Name: "Test Look",
            Description: "Description",
            ColorPalette: "Red",
            ProductVariationIds: new List<Guid> { productVariation.Id }
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Handle_ShouldSetLookStatusToDraft()
    {
        // Arrange
        var productVariation = new ProductVariation { Id = Guid.NewGuid(), Size = "40"};
        _dbContext.ProductVariations.Add(productVariation);
        await _dbContext.SaveChangesAsync();

        var command = new CreateLookCommand(
            Name: "Test Look",
            Description: "Description",
            ColorPalette: "Red",
            ProductVariationIds: new List<Guid> { productVariation.Id }
        );

        // Act
        var lookId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var createdLook = await _dbContext.Looks.FindAsync(lookId);
        Assert.Equal(LookStatus.Draft, createdLook?.LookStatus);
    }

    [Fact]
    public async Task Handle_ShouldCreateLook_WithNullDescription()
    {
        // Arrange
        var productVariation = new ProductVariation { Id = Guid.NewGuid(), Size = "100"};
        _dbContext.ProductVariations.Add(productVariation);
        await _dbContext.SaveChangesAsync();

        var command = new CreateLookCommand(
            Name: "Test Look",
            Description: null, // Null description
            ColorPalette: "Red",
            ProductVariationIds: new List<Guid> { productVariation.Id }
        );

        // Act
        var lookId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        var createdLook = await _dbContext.Looks.FindAsync(lookId);
        Assert.NotNull(createdLook);
        Assert.Null(createdLook.Description);
    }
}