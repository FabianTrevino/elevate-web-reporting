using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DM.WR.Models.Config;

namespace DM.WR.Web.ActionFilters
{
    /// <summary>
    /// Restrict the access from different request eg.
    /// <code> RequestFilter["abc.com,localhost] </code>
    /// & to accept all referrer request use *
    /// <code>RequestFilter["*"]</code>
    /// </summary>
    public class RequestFilter : AuthorizeAttribute
    {
        public string ReferrerUrl { get; set; }

        public RequestFilter()
        {
            ReferrerUrl = ConfigSettings.EntryAllowedUrls;
        }

        private IList<string> SplitAndAddUrls(string referrerUrls)
        {
            IList<string> list = new List<string>();
            var referrerList = referrerUrls.Split(',');
            foreach (string ip in referrerList)
                list.Add(ip);

            return list;
        }

        private bool CheckAllowed(string referreredUrl)
        {
            if (string.IsNullOrEmpty(referreredUrl))
            {
                return false;
            }

            if (ReferrerUrl.Contains("*")) return true;

            var referrerUrls = SplitAndAddUrls(ReferrerUrl);

#if DEBUG
            referrerUrls.Add("localhost");
#endif 

            return referrerUrls.Any(referrer => referreredUrl.Contains(referrer));
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            string referreredUrl = httpContext.Request.UrlReferrer?.ToString();

            if (string.IsNullOrEmpty(referreredUrl))
            {
                referreredUrl = httpContext.Request.ServerVariables["http_referer"] as string;
            }

            try
            {
                // Check that the referred url is allowed to access
                return CheckAllowed(referreredUrl);
            }
            catch
            {
                // Log the exception, probably something wrong with the configuration
                throw;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpStatusCodeResult(403, "Access Denied");
        }

        public override void OnAuthorization(AuthorizationContext actionContext)
        {
            if (AuthorizeCore((HttpContextBase)actionContext.HttpContext))
                return;
            HandleUnauthorizedRequest(actionContext);
        }
    }
}