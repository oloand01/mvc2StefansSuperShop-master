using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;
using StefanShopWeb.Data;
using StefanShopWeb.Models;
using StefanShopWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StefanShopWeb.Services
{
    public enum Status
    {
        Done,
        Uncompleted
    }
    public class NewsletterServices : INewsletterServices
    {
        private readonly ApplicationDbContext _context;

        public NewsletterServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CreateNews(AdminNewsletterViewModel model)
        {
            var newsletter = new Newsletter()
            {
                Date = DateTime.Now,
                Status = Status.Uncompleted.ToString(),
                Text = model.Text
            };
            _context.Newsletters.Add(newsletter);
            _context.SaveChanges();
        }
        public void EditNews(AdminNewsletterViewModel model)
        {
            var newsletter = _context.Newsletters.SingleOrDefault(n => n.Id == model.Id);
            newsletter.Date = DateTime.Now;
            newsletter.Status = Status.Uncompleted.ToString();
            newsletter.Text = model.Text;
            
            _context.SaveChanges();
        }
        public AdminNewsletterViewModel GetNewsText(int id)
        {
            var newsletter = _context.Newsletters.SingleOrDefault(n => n.Id == id);
            var model = new AdminNewsletterViewModel()
            {
                Date = newsletter.Date,
                Status = newsletter.Status,
                Text = newsletter.Text
            };
            return model;
        }
        public List<AdminNewsletterViewModel> GetNewsLetterList()
        {
            var list = _context.Newsletters.Select(c => new AdminNewsletterViewModel
                {
                    Id= c.Id,
                    Text = c.Text,
                    Date = new DateTime(c.Date.Ticks / 600000000 * 600000000),
                    Status= c.Status
                }).ToList();

            return list;

        }

        public void SendNews(AdminMessageViewModel model)
        {
            var newsletter = _context.Newsletters.SingleOrDefault(n => n.Id == model.Id);
            
            string emailBody = string.Empty;
            var message = new MimeMessage();

            var emailMessage = _context.NewsletterSubscriptions;
            message.To.AddRange(emailMessage.Select(x => new MailboxAddress(x.Email, x.Email)));
            message.From.Add(new MailboxAddress("info", "info@email.com"));
            message.Subject = model.Subject;

            var builder = new BodyBuilder();
            builder.TextBody = model.Message;
            var image = builder.LinkedResources.Add("./wwwroot/img/logo.png");
            image.ContentId = MimeUtils.GenerateMessageId();
            builder.HtmlBody = string.Format($@"<center><img src=""cid:{image.ContentId}""></center>
<h4>{builder.TextBody}</h4> 
<br/><hr/><br/><p>Message was sent by: {model.Name}.</p>");
            message.Body = builder.ToMessageBody();

            using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
            {
                ////VARIANT 1: för att få email måste man installera Papercut SMTP (https://github.com/ChangemakerStudios/Papercut-SMTP)
                 emailClient.Connect("127.0.0.1", 25, false);

                emailClient.Send(message);
                emailClient.Disconnect(true);
            };

            
            newsletter.Date = DateTime.Now;
            newsletter.Status = Status.Done.ToString();

            _context.SaveChanges();
        }

        public void DeleteNewsletter(int id)
        {
            var newsletter = _context.Newsletters.Find(id);
            _context.Newsletters.Remove(newsletter);
            _context.SaveChanges();

        }
    }
}
