using Quartz;

namespace ShopApp.Infrastructure.Jobs;

public interface IJobConfig
{
    void ConfigureJob(IServiceCollectionQuartzConfigurator quartz);
}