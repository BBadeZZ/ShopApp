using Autofac;
using ShopApp.Infrastructure.Data;

namespace ShopAppV2.Modules;

public class DataModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<EfDbContext>().InstancePerLifetimeScope();
        // builder.RegisterType<ADODbContext>().InstancePerLifetimeScope();
    }
}