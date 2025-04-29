using LookGenerator.Infrastructure.Jobs.Definitions;
using Microsoft.Extensions.Options;
using Quartz;

namespace LookGenerator.Infrastructure.Jobs.Setups;

public class BershkaLookParsingJobSetup : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobKey = new JobKey("BershkaLookParsingJob_Page_1");

        options.AddJob<BershkaLookParsingJob>(jobBuilder =>
            jobBuilder.WithIdentity(jobKey)
                .UsingJobData("Page", 1)
                .StoreDurably());

        options.AddTrigger(trigger => trigger
            .ForJob(jobKey)
            .WithIdentity("BershkaLookParsingTrigger_Page_1")
            .StartNow()
        );
    }
}
