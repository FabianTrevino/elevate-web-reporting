using Autofac;
using DM.WR.Data.Logging;
using DM.WR.Data.Repository;

namespace DM.WR.Data
{
    public class IocConfig
    {
        public static ContainerBuilder RegisterDependencies(ContainerBuilder builder)
        {
            //Types - DM.WR.Data
            builder.RegisterType<DbLogger>().As<IDbLogger>().SingleInstance();
            builder.RegisterType<DataAccessHelpers>().As<IDataAccessHelpers>().InstancePerRequest();
            builder.RegisterType<ReportCriteriaClient>().As<IReportCriteriaClient>().InstancePerRequest();
            builder.RegisterType<DbClient>().As<IDbClient>().InstancePerRequest();

            return builder;
        }
    }
}