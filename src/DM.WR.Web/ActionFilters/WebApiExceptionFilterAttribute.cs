using DM.UI.Library.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;

namespace DM.WR.Web.ActionFilters
{
    //TODO:  For phase 2 this has to be moved over to DM UI Library
    public class WebApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is NotImplementedException)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            }

            if (context.Exception.Message.Contains("GraphQL API"))
                context.Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent(typeof(Alert), CreateAlert(AlertType.Error, context.Exception.Message, true), new JsonMediaTypeFormatter())
                };
            else if (context.Exception.Message.Contains("Adaptive: No data"))
                context.Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent(typeof(Alert), CreateAlert(AlertType.Error, AjaxNoDataErrorText, true), new JsonMediaTypeFormatter())
                };
            else
                context.Response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ObjectContent(typeof(Alert), CreateAlert(AlertType.Error, AjaxErrorText, true), new JsonMediaTypeFormatter())
                };
        }

        private Alert CreateAlert(AlertType alertType, string message, bool isDismissable)
        {
            return new Alert
            {
                Message = message,
                IsDismissable = isDismissable,
                AlertType = alertType
            };
        }

        private string AjaxErrorText =>
            "An error occurred while processing your request. Please try again later or contact the <em>DataManager</em> Support Center.<br/>" +
            "1-877-246-8337 | <a href=\"mailto:help@riversidedatamanager.com\">help@riversidedatamanager.com</a><br/>" +
            "<em>DataManager Support Center hours are Monday - Friday from 7:00 AM - 6:00 PM (CST).</em>";

        private string AjaxNoDataErrorText =>
            "<em>DataManager</em> currently could not find any data. This could be because tests have not been completed as of yet. Please try again later or contact the <em>DataManager</em> Support Center.<br/>" +
            "1-877-246-8337 | <a href=\"mailto:help@riversidedatamanager.com\">help@riversidedatamanager.com</a><br/>" +
            "<em>DataManager Support Center hours are Monday - Friday from 7:00 AM - 6:00 PM (CST).</em>";
    }
}