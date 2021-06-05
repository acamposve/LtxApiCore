using LtxMagayaCore.Core.Domain;
using LtxMagayaCore.Infrastructure.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LtxMagayaCore.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly AppSettings _appSettings;

        public EmailService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public void Send(string to, string subject, string html, string from = null)
        {
            // create message
            var email = new MimeMessage();

            var EmailFrom = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["EmailFrom"];
            var SmtpHost = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["SmtpHost"];
            var SmtpUser = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["SmtpUser"];
            var SmtpPass = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["SmtpPass"];
            email.From.Add(MailboxAddress.Parse(from ?? EmailFrom));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(SmtpHost, _appSettings.SmtpPort, SecureSocketOptions.None);
            smtp.Authenticate(SmtpUser, SmtpPass);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
