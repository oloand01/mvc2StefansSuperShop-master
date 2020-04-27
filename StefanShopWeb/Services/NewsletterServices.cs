using MimeKit;
using MimeKit.Text;
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

            var emailMessage = _context.Users;
            message.To.AddRange(emailMessage.Select(x => new MailboxAddress(x.UserName, x.NormalizedEmail)));
            message.From.Add(new MailboxAddress("info", "info@email.com"));
            message.Subject = model.Subject;
            emailBody = model.Message;

            var body = new TextPart(TextFormat.Plain)
            {
                Text = $"{ emailBody } \n\t ---\n\t Message was sent by: {model.Name}."
            };
            
            var attachment = new MimePart("image", "png")
            {
                Content = new MimeContent(System.IO.File.OpenRead("./wwwroot/img/logo.png"), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName("./wwwroot/img/logo.png")
            };

            var multipart = new Multipart("mixed");
            multipart.Add(body);
            multipart.Add(attachment);
            message.Body = multipart;


            using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
            {
                //VARIANT 1: för att få email måste man installera Papercut SMTP (https://github.com/ChangemakerStudios/Papercut-SMTP)
                emailClient.Connect("127.0.0.1", 25, false);

                ////VARIANT 2: för att få email behöver man gå på https://mailtrap.io/share/664808/e0626a741efbc9a521dcc29b06061ee7 och loga in med sin google- eller gitHubkonto
                //emailClient.Connect("smtp.mailtrap.io", 587, false);
                //emailClient.Authenticate("a83b18c9f0570b", "ae426e3d31c5fb");

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
