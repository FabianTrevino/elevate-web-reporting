using Autofac;

namespace DM.WR.GraphQlClient
{
    public class IocConfig
    {
        public static ContainerBuilder RegisterDependencies(ContainerBuilder builder)
        {
            builder.RegisterType<CacheWrapper>().As<ICacheWrapper>().InstancePerRequest();
            builder.RegisterType<ApiClient>().As<IApiClient>().InstancePerRequest();

            return builder;
        }
    }
}