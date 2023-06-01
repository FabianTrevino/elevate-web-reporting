using DM.UI.Library.Models;
using DM.WR.Models.Config;
using DM.WR.BL.Providers;
using System;
using System.Web;
using System.Collections.Generic;

namespace DM.WR.Web.Infrastructure
{
    public class WebsiteHelper : IWebsiteHelper
    {
        private readonly IGlobalProvider _provider;

        public readonly string AppPath;

        public WebsiteHelper(IGlobalProvider provider)
        {
            _provider = provider;

            AppPath = HttpContext.Current.Request.ApplicationPath == "/" ? "" : HttpContext.Current.Request.ApplicationPath;
        }

        public string GetAppPath()
        {
            return AppPath;
        }

        public MainHeaderViewModel GetHeaderModel()
        {
            return _provider.BuildHeaderModel(AppPath);
        }

        public List<MagicMenuItem> GetMainMenu()
        {
            return _provider.BuildMainMenu();
        }

        public MagicFooterViewModel GetFooterModel()
        {
            return _provider.BuildFooterModel();
        }

        public NotFoundViewModel BuildNotFoundPageModel()
        {
            return new NotFoundViewModel { DmSignInLink = GetDataManagerLoginUrl() };
        }

        public UiSettings GetUiSettings()
        {
            return new UiSettings
            {
                SessionKeepAliveUrl = "/Utility/KeepAlive",
                SiteRoot = AppPath,
                AlertTimeToSessionEnd = Convert.ToInt32(ConfigSettings.AlertTimeToSessionEnd) * 60,
                SessionTimeOut = Convert.ToInt32(ConfigSettings.SessionTimeOut) * 60,
                LoginUrl = _provider.IsDemo() ? ConfigSettings.RsLink.IowaFlex : ConfigSettings.DmLink.LoginPage
            };
        }

        public string GetDataManagerLoginUrl()
        {
            return ConfigSettings.DmLink.LoginPage;
        }

        public string GetDataManagerAddReportingKeyUrl()
        {
            return ConfigSettings.DmLink.AddReportingKey;
        }

        public bool IsDemo()
        {
            return _provider.IsDemo();
        }

        public void LogToDatabase(Exception exc)
        {
            _provider.LogToDatabase(exc);
        }
    }
}