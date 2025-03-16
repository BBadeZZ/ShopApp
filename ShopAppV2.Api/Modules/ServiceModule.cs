using Autofac;
using ShopApp.Application.Services;
using ShopApp.Infrastructure.Services;

namespace ShopAppV2.Modules;

public class ServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register service interfaces and implementations
        builder.RegisterType<EfProductService>().As<IProductService>().InstancePerLifetimeScope();
        builder.RegisterType<EfCategoryService>().As<ICategoryService>().InstancePerLifetimeScope();
        // builder.RegisterType<ImageUploaderService>().As<IImageUploaderService>().InstancePerLifetimeScope();
        builder.RegisterType<CategoryManagerService>().As<ICategoryManagerService>().InstancePerLifetimeScope();
        builder.RegisterType<JwtTokenService>().As<IJwtTokenService>().InstancePerLifetimeScope();
        builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
        builder.RegisterType<MemoryCacheService>().As<ICacheService>().InstancePerLifetimeScope();
    }
}