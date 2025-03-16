using Quartz;

namespace ShopApp.Infrastructure.Jobs;

public class CleanUploadsJobConfig : IJobConfig
{
    public void ConfigureJob(IServiceCollectionQuartzConfigurator quartz)
    {
        var jobKey = new JobKey("CleanUploadsJob", "DEFAULT");

        // Define the job
        quartz.AddJob<CleanUploadsJob>(opts => opts.WithIdentity(jobKey));

        // Define the trigger
        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity("CleanUploadsJobTrigger", "DEFAULT")
            .StartNow()
            .WithCronSchedule("0 0 0 * * ?")); // Every day at midnight
    }
}