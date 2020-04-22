using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using StefanShopWeb.Data;
using StefanShopWeb.ViewModels;
using System.Net;



namespace StefanShopWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        List<MenuItem> SetupMenu(string activeAction)
        {
            var list = new List<MenuItem>();
            list.Add(new MenuItem { Text = "Products", Action = "Products", Controller = "Admin", IsActive = activeAction == "Products" });
            list.Add(new MenuItem { Text = "Categories", Action = "Categories", Controller = "Admin", IsActive = activeAction == "Categories" });
            list.Add(new MenuItem { Text = "Message", Action = "Message", Controller = "Admin", IsActive = activeAction == "Message" });
            return list;
        }
        public IActionResult Index()
        {
            var model = new AdminViewModel();
            model.MenuItems = SetupMenu("");
            return View(model);
        }
        public IActionResult Products()
        {
            var model = new AdminProductListViewModel();
            model.MenuItems = SetupMenu("Products");
            model.Products = dbContext.Products.Include(p => p.Category).
                Select(p => new AdminProductListViewModel.Product
                {
                    Id = p.ProductId,
                    CategoryName = p.Category.CategoryName,
                    Name = p.ProductName,
                    Price = p.UnitPrice.Value
                }).ToList();
            return View(model);
        }

        public IActionResult EditProduct(int id)
        {
            var model = new AdminEditProductViewModel();
            model.MenuItems = SetupMenu("Products");
            var prod = dbContext.Products.FirstOrDefault(p => p.ProductId == id);
            model.ProductId = prod.ProductId;
            model.ProductName = prod.ProductName;
            model.SupplierId = prod.SupplierId.Value;
            model.UnitPrice = prod.UnitPrice.Value;
            model.CategoryId = prod.CategoryId;
            model.Discontinued = prod.Discontinued;
            model.UnitsInStock = prod.UnitsInStock.Value;

            return View(model);
        }


        public IActionResult Categories()
        {
            var model = new AdminCategoryListViewModel();
            model.MenuItems = SetupMenu("Categories");
            model.Categories = dbContext.Categories.Select(c =>
                new AdminCategoryListViewModel.Category { Id = c.CategoryId, Name = c.CategoryName }).ToList();
            return View(model);
        }


        public IActionResult Message()
        {
            var model = new AdminMessageViewModel();
            
            return View(model);
        }

        [HttpPost]
        public IActionResult Message(AdminMessageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string emailBody = string.Empty;
                    var message = new MimeMessage();
                    
                    
                    var emailMessage = dbContext.Users;
                    message.To.AddRange(emailMessage.Select(x => new MailboxAddress(x.UserName, x.NormalizedEmail)));
                    message.From.Add(new MailboxAddress("info", "info@email.com"));
                    message.Subject = model.Subject;
                    emailBody = model.Message + "\n\t\n\t Message was sent by: " + model.Name;

                    message.Body = new TextPart(TextFormat.Html)
                    {
                        Text = emailBody
                    };

                    using (var emailClient = new MailKit.Net.Smtp.SmtpClient()) {
                        emailClient.Connect("smtp.mailtrap.io", 587, false);
                        emailClient.Authenticate("a83b18c9f0570b", "ae426e3d31c5fb");
                        emailClient.Send(message);
                        emailClient.Disconnect(true);
                    } ;

                    ModelState.Clear();
                    //using (SmtpClient emailClient = new SmtpClient())
                    //{
                    //    emailClient.Host = "smtp.mailtrap.io";
                    //    emailClient.Port = 587;
                    //    emailClient.Credentials = new NetworkCredential("a83b18c9f0570b", "ae426e3d31c5fb");
                    //    emailClient.EnableSsl = true;
                    //    emailClient.Send(message.From.ToString(), message.To.ToString(), message.Subject, message.Body.ToString());
                    //}
                }
                catch (Exception ex)
                {

                    ModelState.Clear();
                    ViewBag.Exception = $" Oops! Message could not be sent. Error:  {ex.Message}";
                }

            }
            return View();
        }
    }
}