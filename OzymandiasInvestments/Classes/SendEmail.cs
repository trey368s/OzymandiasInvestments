﻿using System.Net.Mail;
using System.Net;
using OzymandiasInvestments.Models.AppSettingsModels;

namespace OzymandiasInvestments.Classes
{
    public class SendEmail
    {
        private readonly EmailSettingsModel _emailSettings;

        public SendEmail(EmailSettingsModel emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public void CreateEmail(string to, string subject, string body)
        {
            var smtpServer = _emailSettings.SmtpServer;
            var smtpPort = int.Parse(_emailSettings.SmtpPort);
            var senderName = _emailSettings.SenderName;
            var senderEmail = _emailSettings.SenderEmail;
            var smtpUsername = _emailSettings.smtpUsername;
            var smtpPassword = _emailSettings.smtpPassword;

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                client.Send(mailMessage);
            }
        }
    }
}
