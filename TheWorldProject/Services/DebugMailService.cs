using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MimeKit;
using MailKit.Net.Smtp;

namespace TheWorldProject.Services
{
    public class DebugMailService : IMailService
    {
        public async Task ConfirmationByEmailMsgAsync(string email, string subject, string msg)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("The World Project", "mailbot.core@gmail.com"));
            message.To.Add(new MailboxAddress(email));
            message.Subject = subject;
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html) {
                Text = msg
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("mailbot.core@gmail.com", "Passw@rd1234");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

        }

        public void SendMail(string to, string from, string subject, string body)
        {
            Debug.WriteLine($"Sending Mail: To {to}, From: {from} Subject: {subject} Body: {body}");
        }

        public void SendMessage(string name, string email, string callbackUrl)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("User of TheWorldProject", "mailbot.core@gmail.com"));
            message.To.Add(new MailboxAddress("edik.kucherniuk@gmail.com"));
            message.Subject = "Mail service testing";
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = $"From: {name} <br> Contact information: {email} <br> Message: {callbackUrl}"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("mailbot.core@gmail.com", "Passw@rd1234");
                client.Send(message);
                client.Disconnect(true);
            }
        }

    }
}
