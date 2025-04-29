using LookGenerator.Infrastructure.Jobs.Definitions;
using Microsoft.Extensions.Options;
using Quartz;

namespace LookGenerator.Infrastructure.Jobs.Setups;

public class BershkaColorNormalizationJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = new JobKey(nameof(BershkaColorNormalizationJob));
        options.AddJob<BershkaColorNormalizationJob>(jobBuilder => jobBuilder.WithIdentity(jobKey).StoreDurably());
        options.AddTrigger(builder =>
            builder.ForJob(jobKey).WithIdentity($"{nameof(BershkaColorNormalizationJob)}Trigger").StartNow());
    }
}