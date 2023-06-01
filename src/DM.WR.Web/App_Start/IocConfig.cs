using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using DM.WR.Web.Infrastructure;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;

namespace DM.WR.Web
{
    public class IocConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder = BL.IocConfig.RegisterDependencies(builder);

            //Types
            builder.RegisterType<WebsiteHelper>().As<IWebsiteHelper>().InstancePerRequest();

            //Modules
            //builder.RegisterControllers(typeof(WebApiApplication).Assembly).InstancePerRequest();
            builder.RegisterControllers(Assembly.GetExecutingAssembly()).InstancePerRequest();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).InstancePerRequest();
            //builder.RegisterType<DashboardController>().InstancePerRequest();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver((IContainer)container); //Set the WebApi DependencyResolver
        }
    }
}