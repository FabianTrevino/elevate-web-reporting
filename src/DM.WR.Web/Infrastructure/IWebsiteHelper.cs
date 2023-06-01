using System;
using DM.UI.Library.Models;
using System.Collections.Generic;

namespace DM.WR.Web.Infrastructure
{
    public interface IWebsiteHelper
    {
        string GetAppPath();
        MainHeaderViewModel GetHeaderModel();
        List<MagicMenuItem> GetMainMenu();
        MagicFooterViewModel GetFooterModel();
        NotFoundViewModel BuildNotFoundPageModel();
        UiSettings GetUiSettings();
        string GetDataManagerLoginUrl();
        string GetDataManagerAddReportingKeyUrl();
        bool IsDemo();
        void LogToDatabase(Exception exc);
    }
}