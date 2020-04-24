using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            var offset = 78;
            var imageData = dbContext.Categories.Find(id).Picture;
            var bytes = new byte[imageData.Length - offset];

            Array.Copy(imageData, offset, bytes, 0, bytes.Length);
            return File(bytes, "image/png");
        }

        public IActionResult SetCategoryImage(int id)
        {

            return View();
        }

        [HttpGet]
        public IActionResult UploadFiles()
        {
            return View();
        }

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