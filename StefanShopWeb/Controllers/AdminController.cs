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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace StefanShopWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IHostingEnvironment _env;

        public AdminController(ApplicationDbContext dbContext, IHostingEnvironment env)
        {
            this.dbContext = dbContext;
            _env = env;
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
                        emailClient.Connect("127.0.0.1", 25, false);
                        //emailClient.Connect("smtp.mailtrap.io", 587, false);
                        //emailClient.Authenticate("a83b18c9f0570b", "ae426e3d31c5fb");
                        emailClient.Send(message);
                        emailClient.Disconnect(true);
                    };

                    ModelState.Clear();

                }
                catch (Exception ex)
                {

                    ModelState.Clear();
                    ViewBag.Exception = $" Oops! Message could not be sent. Error:  {ex.Message}";
                }

            }
            return View();
        }

        [HttpGet]
        public IActionResult UploadFiles()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            //var filePaths = new List<string>();
            var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)

                {
                    var uploads = Path.Combine(_env.WebRootPath, "ProductImages");
                    var fullPath = Path.Combine(uploads, GetUniqueFileName(formFile.FileName));
                    formFile.CopyTo(new FileStream(fullPath, FileMode.Create));
                    // full path to file in temp location

                    //var filePath = Path.Combine(_env.ContentRootPath, "ProductImages") + $@"\{newFileName}";  //we are using Temp file name just for the example. Add your own file path.
                    //filePaths.Add(filePath);

                    //using (var stream = new FileStream(filePath, FileMode.Create))
                    //{
                    //    await formFile.CopyToAsync(stream);
                    //}
                }

            }

            return Ok(new { count = files.Count, size, filePath });
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

    }
}