using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Yu.Core.Emails
{
    public class EmailSender
    {
        public readonly SmtpClient _smtpClient;

        public EmailSender(IOptions<EmailOption> emailOptions)
        {
            _smtpClient = new SmtpClient();
            _smtpClient.EnableSsl = bool.Parse(emailOptions.Value.UseSsl);
            _smtpClient.UseDefaultCredentials = bool.Parse(emailOptions.Value.DefaultCredentials);
            _smtpClient.Credentials = new NetworkCredential(emailOptions.Value.Username, emailOptions.Value.Password);
            _smtpClient.Port = int.Parse(emailOptions.Value.ServerPort);
            _smtpClient.Host = emailOptions.Value.ServerName;
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        public async Task<bool> SendEmail(string code, string mailAddress)
        {
            string url = $"https://XXXXXXX?code={code}";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"  请点击<a href=\"{url}\">此处</a>重置您的密码");
            MailMessage message = new MailMessage(from: "xxxx@163.com", to: mailAddress, subject: "重置密码", body: sb.ToString());
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            try
            {
                _smtpClient.Send(message);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
