using Autofac;
using DM.WR.BL.Builders;
using DM.WR.BL.Email;
using DM.WR.BL.Managers;
using DM.WR.BL.Providers;

namespace DM.WR.BL
{
    public class IocConfig
    {
        public static ContainerBuilder RegisterDependencies(ContainerBuilder builder)
        {
            Data.IocConfig.RegisterDependencies(builder);
            ServiceClient.IocConfig.RegisterDependencies(builder);
            GraphQlClient.IocConfig.RegisterDependencies(builder);

            //Builders
            builder.RegisterType<IowaFlexFiltersBuilder>().As<IIowaFlexFiltersBuilder>().InstancePerRequest();
            builder.RegisterType<DashboardIowaFlexProviderBuilder>().As<IDashboardIowaFlexProviderBuilder>().InstancePerRequest();
            builder.RegisterType<OptionsBuilder>().As<IOptionsBuilder>().InstancePerRequest();
            builder.RegisterType<CogatFiltersBuilder>().As<ICogatFiltersBuilder>().InstancePerRequest();
            builder.RegisterType<GraphQlQueryStringBuilder>().As<IGraphQlQueryStringBuilder>().InstancePerRequest();
            builder.RegisterType<ActuateQueryStringBuilder>().As<IActuateQueryStringBuilder>().InstancePerRequest();
            builder.RegisterType<GenerateReportRequestBuilder>().As<IGenerateReportRequestBuilder>().InstancePerRequest();
            builder.RegisterType<SMIModelBuilder>().As<ISMIModelBuilder>().InstancePerRequest();
            builder.RegisterType<OptionPageParser>().As<IOptionPageParser>().InstancePerRequest();
            builder.RegisterType<EREngineParamterBuilder>().As<IEREngineParamterBuilder>().InstancePerRequest();
            builder.RegisterType<BackgroundModelBuilder>().As<IBackgroundModelBuilder>().InstancePerRequest();
            //Email
            builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerRequest();
            builder.RegisterType<CogatFeedbackSender>().As<ICogatFeedbackSender>().InstancePerRequest();

            //Managers
            builder.RegisterType<SessionManager>().As<ISessionManager>().InstancePerRequest();
            builder.RegisterType<UserDataManager>().As<IUserDataManager>().InstancePerRequest();
            builder.RegisterType<OptionsManager>().As<IOptionsManager>().InstancePerRequest();
            builder.RegisterType<CriteriaManager>().As<ICriteriaManager>().InstancePerRequest();
            builder.RegisterType<LoginManager>().As<ILoginManager>().InstancePerRequest();
            builder.RegisterType<EncryptionManagerElevate>().As<IEncryptionManagerElevate>().InstancePerRequest();

            //Providers
            builder.RegisterType<OptionsProvider>().As<IOptionsProvider>().InstancePerRequest();
            builder.RegisterType<CriteriaProvider>().As<ICriteriaProvider>().InstancePerRequest();
            builder.RegisterType<LibraryProvider>().As<ILibraryProvider>().InstancePerRequest();
            builder.RegisterType<ReportProvider>().As<IReportProvider>().InstancePerRequest();
            builder.RegisterType<GlobalProvider>().As<IGlobalProvider>().InstancePerRequest();
            builder.RegisterType<IowaFlexCommonProviderFunctions>().As<IIowaFlexCommonProviderFunctions>().InstancePerRequest();
            builder.RegisterType<IowaFlexProvider>().As<IIowaFlexProvider>().InstancePerRequest();
            builder.RegisterType<IowaFlexLongitudinalProvider>().As<IIowaFlexLongitudinalProvider>().InstancePerRequest();
            builder.RegisterType<CogatProvider>().As<ICogatProvider>().InstancePerRequest();

            return builder;
        }
    }
}