using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Features.Looks.Create;

public class CreateLookHandler(IApplicationDbContext applicationDbContext) : ICommandHandler<CreateLookCommand, Guid>
{
    public async Task<Guid> Handle(CreateLookCommand request, CancellationToken cancellationToken)
    {
        var variationIds = request.ProductVariationIds.Distinct().ToList();

        var createdLook = new Look
        {
            Name = request.Name,
            Description = request.Description,
            ColorPalette = request.ColorPalette,
            LookStatus = LookStatus.Private,
            LookProductVariations = variationIds
                .Select(pvId => new LookProductVariation
                {
                    ProductVariationId = pvId
                })
                .ToList()
        };

        await applicationDbContext.Looks.AddAsync(createdLook, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return createdLook.Id;
    }
}
