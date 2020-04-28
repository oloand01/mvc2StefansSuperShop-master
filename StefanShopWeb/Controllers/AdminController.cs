using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StefanShopWeb.Data;
using StefanShopWeb.Models;
using StefanShopWeb.Services;
using StefanShopWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StefanShopWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private INewsletterServices _newsletterServices;
        private readonly IHostingEnvironment _env;

        public AdminController(ApplicationDbContext dbContext, INewsletterServices newsletterServices, IHostingEnvironment env)
        {
            this.dbContext = dbContext;
            _newsletterServices = newsletterServices;
            _env = env;
        }
        List<MenuItem> SetupMenu(string activeAction)
        {
            var list = new List<MenuItem>();
            list.Add(new MenuItem { Text = "Products", Action = "Products", Controller = "Admin", IsActive = activeAction == "Products" });
            list.Add(new MenuItem { Text = "Categories", Action = "Categories", Controller = "Admin", IsActive = activeAction == "Categories" });
            list.Add(new MenuItem { Text = "Newsletters", Action = "NewsletterList", Controller = "Admin", IsActive = activeAction == "NewsletterList" });
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

        public IActionResult EditCategory(int id)
        {
            var model = dbContext.Categories.Where(c => c.CategoryId == id).Select(c => new AdminEditCategoryViewModel { Id = c.CategoryId, CategoryName = c.CategoryName, Description = c.Description }).FirstOrDefault();

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SaveCategory(AdminEditCategoryViewModel model, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {

                string picName = UploadFiles(files);

                Categories editCategory = new Categories
                {
                    CategoryId = model.Id,
                    CategoryName = model.CategoryName,
                    Description = model.Description,
                    PictureName = picName
                };
                dbContext.Update(editCategory);
                await dbContext.SaveChangesAsync();

                ViewBag.Message = "File successfully uploaded.";

                return View("EditCategory", model);

            }

            return View();
        }

        private string UploadFiles(List<IFormFile> files)
        {
            //long size = files.Sum(f => f.Length);

            var filePath = Path.GetTempFileName();
            string picName = null;

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)

                {
                    picName = GetUniqueFileName(formFile.FileName);
                    var uploads = Path.Combine(_env.WebRootPath, "ProductImages");
                    var fullPath = Path.Combine(uploads, picName);
                    formFile.CopyTo(new FileStream(fullPath, FileMode.Create));
                }
            }

            return picName;
        }

        [Authorize(Roles = "Admin")]
        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);

            if (System.IO.File.Exists(fileName))
            {
                System.IO.File.Delete(fileName);
            }

            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

        public IActionResult NewsletterList()
        {
            var list = _newsletterServices.GetNewsLetterList();

            return View(list);
        }

        public IActionResult CreateNews()
        {

            var model = new AdminNewsletterViewModel();
            model.Date = new DateTime(DateTime.UtcNow.Ticks / 600000000 * 600000000);
            model.Status = Status.Uncompleted.ToString();
            return View("CreateNews", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNews(AdminNewsletterViewModel model)
        {
            if (ModelState.IsValid)
            {
                _newsletterServices.CreateNews(model);
                return RedirectToAction("NewsletterList");
            }
            return View("CreateNews", model);
        }

        public IActionResult EditNews(int id)
        {
            var model = _newsletterServices.GetNewsText(id);
            model.Date = new DateTime(DateTime.UtcNow.Ticks / 600000000 * 600000000);
            model.Status = Status.Uncompleted.ToString();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditNews(AdminNewsletterViewModel model)
        {
            if (ModelState.IsValid)
            {
                _newsletterServices.EditNews(model);
                return RedirectToAction("NewsletterList");
            }
            return View(model);
        }
        public IActionResult DeleteNews(int id)
        {
            try
            {
                _newsletterServices.DeleteNewsletter(id);

                return RedirectToAction("NewsletterList");
            }
            catch (Exception ex)
            {
                ViewBag.Exception = $" Oops! Delete failed. Error:  {ex.Message}";

            }

            return View();
        }
        public IActionResult Message(int id)
        {

            var model = new AdminMessageViewModel();
            var letter = _newsletterServices.GetNewsText(id);
            model.Message = letter.Text;

            return View(model);
        }

        [HttpPost]
        public IActionResult Message(AdminMessageViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _newsletterServices.SendNews(model);
                    ModelState.Clear();
                    return RedirectToAction("NewsletterList");
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