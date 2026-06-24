using Microsoft.Extensions.Configuration;
using PharmaBridge.Abstraction.IServices.Notification;
using PharmaBridge.Shared.DTOs.Notificaiton;
using PharmaBridge.Shared.EnumHelper.NotificationEnums;
using System.Net;
using System.Net.Mail;

namespace PharmaBridge.Services.ServicesImplementation.Notification.StrategyPattern
{
    public class EmailNotificationStrategy : INotificationStrategy
    {
        private readonly IConfiguration _config;

        public EmailNotificationStrategy(IConfiguration config)
        {
            _config = config;
        }

        public NotificationType Type => NotificationType.Email;

        public async Task DeliverAsync(NotificationContentDto dto)
        {
            var emailSettings = _config.GetSection("EmailSettings");

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["SenderEmail"]!, emailSettings["SenderName"]),
                Subject = dto.Subject ?? "PharmaBridge Notification",
                Body = dto.Body,
                IsBodyHtml = true 
            };

            mailMessage.To.Add(dto.Email!); 

            using var smtpClient = new SmtpClient(emailSettings["SmtpServer"])
            {
                Port = int.Parse(emailSettings["Port"]!),
                Credentials = new NetworkCredential(emailSettings["SenderEmail"], emailSettings["Password"]),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}