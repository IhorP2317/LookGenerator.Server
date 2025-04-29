using LookGenerator.Domain.Entities;

namespace LookGenerator.Application.Abstractions;

public interface ISizeGuideService
{
    Task<(Dictionary<Guid, Dictionary<string, SizeOption>> topSizeMap,
            Dictionary<Guid, Dictionary<string, SizeOption>> bottomSizeMap,
            Dictionary<Guid, Dictionary<string, SizeOption>> footwearSizeMap)>
        GetSizeMapsAsync(Dictionary<string, double> measurements, string gender, CancellationToken cancellationToken);

    Task<Dictionary<Guid, Dictionary<string, SizeOption>>> FindMatchingBeltMasterSizesAsync(
        double waistContour,
        string gender,
        CancellationToken cancellationToken);
}