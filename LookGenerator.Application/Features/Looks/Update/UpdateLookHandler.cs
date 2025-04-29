using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LookGenerator.Application.Features.Looks.Update;

public class UpdateLookHandler(IApplicationDbContext applicationDbContext) : ICommandHandler<UpdateLookCommand>
{
    public async Task Handle(UpdateLookCommand request, CancellationToken cancellationToken)
    {
        var look = await applicationDbContext.Looks
            .Include(l => l.LookProductVariations)
            .FirstAsync(l => l.Id == request.Id, cancellationToken);

        look.Name = request.Name;
        look.Description = request.Description;
        look.ColorPalette = request.ColorPalette;
        look.LookStatus = request.Status;

        var existingVariationIds = look.LookProductVariations
            .Select(lpv => lpv.ProductVariationId)
            .ToHashSet();

        var lookProductVariationsToDelete = look.LookProductVariations
            .Where(lpv => !request.ProductVariationIds.Contains(lpv.ProductVariationId))
            .ToList();

        var lookProductVariationsToAdd = request.ProductVariationIds
            .Where(pvId => !existingVariationIds.Contains(pvId))
            .Select(pvId => new LookProductVariation
            {
                LookId = look.Id,
                ProductVariationId = pvId
            })
            .ToList();

        applicationDbContext.LookProductVariations.RemoveRange(lookProductVariationsToDelete);
        await applicationDbContext.LookProductVariations.AddRangeAsync(lookProductVariationsToAdd, cancellationToken);

        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}
