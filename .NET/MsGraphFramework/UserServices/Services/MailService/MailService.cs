using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MsGraph.Utils;

namespace UserServices.Services.MailService
{
      public class MailService
    {
        private readonly MailConfig _config;
        private readonly ILogger<MailService> _logger;

        public MailService(MailConfig config, ILogger<MailService> logger)
        {
            _config = config;
            _logger = logger;
        }
        
        public async Task SendEmail(MailMessage message)
        {
            var client = new SmtpClient
            {
                UseDefaultCredentials = false,
                Host = _config.SmtpHost,
                Port = _config.SmtpPort,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(_config.SmtpUser, _config.SmtpPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
            _logger.LogInformation($"Mail sent to {message.To}: '{message.Subject}'");
        }

        public async Task SendEmail(string to, string subject, string textBody = "", string htmlBody = "")
        {
            if (textBody.IsBlank()) throw new Exception("Email must have at least a text body");
            
            var message = new MailMessage(
                _config.From,
                to,
                subject,
                textBody);

            if (!htmlBody.IsBlank())
            {
                var mimeType = new ContentType("text/html");
                var alternate = AlternateView.CreateAlternateViewFromString(htmlBody, mimeType);
                message.AlternateViews.Add(alternate);
            }
            await SendEmail(message);
            _logger.LogInformation($"Mail sent to {to}: '{subject}'");
        }

        public MailMessage ResetPasswordEmailTemplate(string name, string email, string password, string link)
        {
            string MakeBody(bool useHtml)
            {
                var br = useHtml ? "<br>" : "\n";
                var sb = new StringBuilder();
                if (useHtml) sb.AppendLine("<html>");
                sb.Append($"Dear {name}! Your account has been created.{br}{br}");
                sb.Append($"Username: {email}{br}");
                sb.Append($"Temporary password: {password}{br}{br}");
                sb.Append("You may now log in ");
                sb.Append(useHtml ? $"<a href='{link}'>here</a>.{br}" : $"at {link}.{br}");
                sb.AppendLine("You will be asked to choose a new password and accept terms and conditions");
                if (useHtml) sb.AppendLine("</html>");
                return sb.ToString();
            }
            
            var result = new MailMessage(
                _config.From,
                email,
                "Your credentials",
                MakeBody(false));
            result.ReplyToList.Add(_config.ReplyTo);
            result.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(MakeBody(true), new ContentType("text/html")));
            return result;
        }
        
        public MailMessage TestEmailTemplate(string name, string email)
        {
            string MakeBody(bool useHtml)
            {
                var br = useHtml ? "<br>" : "\n";
                var sb = new StringBuilder();
                if (useHtml) sb.AppendLine("<html>");
                sb.Append($"Dear {name}! This is a test message{br}{br}");
                if (useHtml) sb.AppendLine("</html>");
                return sb.ToString();
            }
            
            var result = new MailMessage(
                _config.From,
                email,
                "Test message from backend",
                MakeBody(false));
            result.ReplyToList.Add(_config.ReplyTo);
            
            result.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(MakeBody(true), new ContentType("text/html")));
            return result;
        }
        
        public MailMessage ClientSecretExpirationEmailTemplate(string name, string email, DateTime expirationDate)
        {
            var dateStr = expirationDate.Date.ToString("yyyy.MM.dd");

            string MakeBody(bool useHtml)
            {
                var br = useHtml ? "<br>" : "\n";
                var li = useHtml ? "<li>" : "-";
                var _li = useHtml ? "</li>" : "\n";
                var ul = useHtml ? "<ul>" : "\n";
                var _ul = useHtml ? "</ul>" : "\n";
                var ahref = useHtml ? "<a href='https://portal.azure.com/'>Azure portal</a>" : "https://portal.azure.com/";
                var code = useHtml ? "<code>" : "";
                var _code = useHtml ? "</code>" : "";
                var b = useHtml ? "<b>" : "";
                var _b = useHtml ? "</b>" : "";
                var sb = new StringBuilder();
                if (useHtml) sb.AppendLine("<html>");

                // TODO email body
                
                if (useHtml) sb.AppendLine("</html>");
                return sb.ToString();
            }



            var result = new MailMessage
            (
                _config.From,
                email,
                $"[IMPORTANT] Azure client secret will expire on {dateStr}!",
                MakeBody(false)
            );
            result.ReplyToList.Add(_config.ReplyTo);
            result.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(MakeBody(true), new ContentType("text/html")));

            return result;
        }
    }
}