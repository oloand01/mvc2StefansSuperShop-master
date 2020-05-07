using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StefanShopWeb.Data;
using StefanShopWeb.Models;
using StefanShopWeb.ViewModels;

namespace StefanShopWeb.Controllers
{
    public class GeneralController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IHostingEnvironment _env;

        public GeneralController(ApplicationDbContext dbContext, IHostingEnvironment env)
        {
            this.dbContext = dbContext;
            _env = env;
        }
        public IActionResult GetCategoryImage(int id)
        {
            var imageData = dbContext.Categories.Find(id).PictureName;
            var uploads = Path.Combine(_env.WebRootPath, "ProductImages", imageData);
            var imageFileStream = System.IO.File.OpenRead(uploads);
            
            return File(imageFileStream, "image/png");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SetCategoryImage(int id)
        {

            return View();
        }

        


    }
}