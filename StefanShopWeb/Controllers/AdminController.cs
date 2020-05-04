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
using System.Threading;
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


        public IActionResult Categories(string msg="")
        {
            var model = new AdminCategoryListViewModel();
            model.MenuItems = SetupMenu("Categories");
            model.Categories = dbContext.Categories.Select(c =>
                new AdminCategoryListViewModel.Category { Id = c.CategoryId, Name = c.CategoryName }).ToList();

            ViewBag.Msg = msg == "success" ? "File successfully uploaded":"";
            return View(model);
        }

        public IActionResult EditCategory(int id)
        {
            var model = dbContext.Categories.Where(c => c.CategoryId == id).Select(c => new AdminEditCategoryViewModel { Id = c.CategoryId, CategoryName = c.CategoryName, Description = c.Description }).FirstOrDefault();
            ViewBag.Edit = "Edit";

            return View(model);
        }

        public IActionResult NewCategory()
        {
            var model = new AdminEditCategoryViewModel();
            ViewBag.Edit = "New";

            return View("EditCategory", model);
        }
        [HttpGet]
        public IActionResult SaveCategory()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SaveCategory(AdminEditCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                string picName = UploadFiles(model);

                Categories category = new Categories
                {
                    CategoryId = model.Id,
                    CategoryName = model.CategoryName,
                    Description = model.Description,
                    PictureName = picName
                };
                if(category.CategoryId == 0)
                {
                    dbContext.Add(category);
                }
                else
                {
                    dbContext.Update(category);
                }
                await dbContext.SaveChangesAsync();

                return RedirectToAction("Categories", new { msg = "success" });
            }

            return View("EditCategory", model);
        }

        private string UploadFiles(AdminEditCategoryViewModel model)
        {
            string picName = null;

            if (model.PictureName != null)
            {
                var uploads = Path.Combine(_env.WebRootPath, "ProductImages");
                picName = Path.GetFileNameWithoutExtension(model.PictureName.FileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(model.PictureName.FileName);
                var fullPath = Path.Combine(uploads, picName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    model.PictureName.CopyTo(stream);
                }
            }

            return picName;
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
            model.Date = new DateTime(DateTime.Now.Ticks / 600000000 * 600000000);
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
        public IActionResult ViewMessage(int id)
        {

            var model = new AdminNewsletterViewModel();
            var letter = _newsletterServices.GetNewsText(id);
            model.Text = letter.Text;
            model.Date = letter.Date;
            model.Status = letter.Status;
            return View(model);
        }
    }
}