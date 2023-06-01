using DM.WR.Models.Config;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DM.WR.BL.Email
{
    public interface IEmailSender
    {
        Task Send(MailMessage mailMessage);
    }

    public class EmailSender : IEmailSender
    {
        public async Task Send(MailMessage mailMessage)
        {
            var client = new SmtpClient
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(ConfigSettings.Email.SmtpUserName, ConfigSettings.Email.SmtpPassword),
                Host = ConfigSettings.Email.SmtpHost,
                Port = ConfigSettings.Email.SmtpPort,
                DeliveryMethod = SmtpDeliveryMethod.Network
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}
