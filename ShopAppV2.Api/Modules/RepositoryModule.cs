using Autofac;
using ShopApp.Infrastructure.Repositories;

namespace ShopAppV2.Modules;

public class RepositoryModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register repository interfaces and implementations
        builder.RegisterType<EfProductRepository>().As<IProductRepository>().InstancePerLifetimeScope();
        builder.RegisterType<EfCategoryRepository>().As<ICategoryRepository>().InstancePerLifetimeScope();

        // Register generic repository
        builder.RegisterGeneric(typeof(EfGenericRepository<>)).As(typeof(IGenericRepository<>))
            .InstancePerLifetimeScope();
    }
}