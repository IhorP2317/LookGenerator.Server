using LookGenerator.Infrastructure.Jobs.Definitions;
using Microsoft.Extensions.Options;
using Quartz;

namespace LookGenerator.Infrastructure.Jobs.Setups;

public class BershkaSizeGuideParsingJobSetup:IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = new JobKey(nameof(BershkaSizeGuideParsingJob));

        options.AddJob<BershkaSizeGuideParsingJob>(opts => opts.WithIdentity(jobKey));
        options.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("BershkaSizeGuideTrigger")
                .StartNow()
                .WithSimpleSchedule(s => s.WithRepeatCount(0))
                // .WithSchedule(CronScheduleBuilder
                //         .CronSchedule("0 0 0 1 JAN,APR,JUL,OCT ? *")
                // )
                );
    }
    
}