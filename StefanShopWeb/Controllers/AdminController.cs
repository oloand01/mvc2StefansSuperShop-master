using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using StefanShopWeb.Data;
using StefanShopWeb.ViewModels;

namespace StefanShopWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IHostingEnvironment _env;
        //test kommentar
        //test 2

        public AdminController(ApplicationDbContext dbContext, IHostingEnvironment env) //TEST3
        {
            this.dbContext = dbContext;
            _env = env;
        }
        List<MenuItem> SetupMenu(string activeAction)
        {
            var list = new List<MenuItem>();
            list.Add( new MenuItem { Text = "Products", Action = "Products", Controller="Admin", IsActive = activeAction == "Products"  } );
            list.Add(new MenuItem { Text = "Categories", Action = "Categories", Controller = "Admin", IsActive = activeAction == "Categories" });
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
                Select(p => new AdminProductListViewModel.Product {
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