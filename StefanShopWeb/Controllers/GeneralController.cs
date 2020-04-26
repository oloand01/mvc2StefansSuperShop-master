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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult UploadFiles()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult UploadFiles(List<IFormFile> files)
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

            ViewBag.Message = "File successfully uploaded.";

            //return Ok(new { count = files.Count, size, filePath });
            return View();
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
    }
}