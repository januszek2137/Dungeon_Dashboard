﻿using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Dungeon_Dashboard.Areas.Identity.Pages.Account.EmailSender {
    public class EmailSender : IEmailSender {
        public Task SendEmailAsync(string email, string subject, string htmlMessage) {
            var smtpClient = new SmtpClient("smtp.gmail.com") {
                Port = 587,
                Credentials = new NetworkCredential("mr.janolex@gmail.com", "zept hugv qgfm ssvr"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage {
                From = new MailAddress("mr.janolex@gmail.com"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            return smtpClient.SendMailAsync(mailMessage);
        }
    }
}
