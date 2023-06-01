using Autofac;
using DM.WR.ServiceClient.BackgroundReport;
using DM.WR.ServiceClient.DmServices;
using DM.WR.ServiceClient.ElevateReportingEngine;

namespace DM.WR.ServiceClient
{
    public class IocConfig
    {
        public static ContainerBuilder RegisterDependencies(ContainerBuilder builder)
        {
            //Actuate
            builder.RegisterType<ActuateServiceClient>().As<IActuateServiceClient>().InstancePerRequest();

            builder.RegisterType<ReportingBackgroundRepository>().As<IReportingBackgroundRepository>().InstancePerRequest();
            builder.RegisterType<ElevateReportingEngineClient>().As<IElevateReportingEngineClient>().InstancePerRequest();
            //DM Services
            builder.RegisterType<WebReportingAshxClient>().As<IWebReportingClient>().InstancePerRequest();
            builder.RegisterType<UserApiClient>().As<IUserApiClient>().InstancePerRequest();

            return builder;
        }
    }
}