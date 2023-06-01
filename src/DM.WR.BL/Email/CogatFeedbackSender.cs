using DM.WR.Models.Config;
using DM.WR.Models.Email;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DM.WR.BL.Email
{
    public interface ICogatFeedbackSender
    {
        Task SendFeedback(FeedbackModel feedback);
    }

    public class CogatFeedbackSender : ICogatFeedbackSender
    {
        private readonly IEmailSender _emailSender;

        public CogatFeedbackSender(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendFeedback(FeedbackModel feedback)
        {
            var message = feedback.Message.Replace("\r\n", "<br/>");

            var messageBodyLines = new List<string>
            {
                $"{feedback.FirstName} {feedback.LastName}<br/>",
                $"Email: {feedback.EmailAddress}<br/>",
                $"District: {feedback.District}<br/>",
                $"School: {feedback.School}<br/>",
                $"Role: {feedback.Role}<br/>",
                "<br/>",
                message
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("\"CogAT Dashboard Feedback\" <cogat-dashboard-feedback@dm.riverside-insights.com>"),
                Subject = "CogAT Dashboard Feedback",
                Body = string.Join("", messageBodyLines),
                IsBodyHtml = true,
            };

            var emailList = ConfigSettings.Email.CogatFeedbackMailingList.Split(',');
            foreach (var email in emailList)
                mailMessage.To.Add(new MailAddress(email));

            await _emailSender.Send(mailMessage);
        }
    }
}
