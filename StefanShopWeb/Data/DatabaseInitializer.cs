using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StefanShopWeb.Models;
using System.Threading.Tasks;

namespace StefanShopWeb.Data
{
    public class DatabaseInitializer
    {
        public void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();
            context.Database.EnsureCreated();
            SeedData(context);
        }

        private void SeedData(ApplicationDbContext context)
        {

            foreach (var item in context.Categories)
            {
                if (item.CategoryId == 1 && item.PictureName == null)
                {
                    item.PictureName = "beverages2_e6d2.png";
                }

                if (item.CategoryId == 2 && item.PictureName == null)
                {
                    item.PictureName = "condiments.jpg";
                }

                if (item.CategoryId == 3 && item.PictureName == null)
                {
                    item.PictureName = "confectionsfinal.jpg";
                }

                if (item.CategoryId == 4 && item.PictureName == null)
                {
                    item.PictureName = "dairyproducts.jpg";
                }

                if (item.CategoryId == 5 && item.PictureName == null)
                {
                    item.PictureName = "grainproducts.jpg";
                }

                if (item.CategoryId == 6 && item.PictureName == null)
                {
                    item.PictureName = "meatproducts.png";
                }

                if (item.CategoryId == 7 && item.PictureName == null)
                {
                    item.PictureName = "produce.jpg";
                }

                if (item.CategoryId == 8 && item.PictureName == null)
                {
                    item.PictureName = "seafoodproducts.jpg";
                }

                
            }


            //if (context.Categories.Any(m => m.PictureName == null))
            //    context.Categories.Update(new Categories
            //    {
            //        CategoryId = 1,
            //        Description = "Savory beverages",
            //        CategoryName = "Beverages",
            //        PictureName = "beverages2_e6d2.png"
            //    });
            //if (context.Categories.Any(m => m.PictureName == null))
            //    context.Categories.Update(new Categories
            //    {
            //        CategoryId = 2,
            //        Description = "Sweet and savory sauces, relishes, spreads, and seasonings",
            //        CategoryName = "Condiments",
            //        PictureName = "condiments.jpg" 
            //    });
            //if (context.Categories.Any(m => m.PictureName == null))
            //    context.Categories.Update(new Categories 
            //    {
            //        CategoryId = 3,
            //        Description = "Desserts, candies, and sweet breads",
            //        CategoryName = "Confections",
            //        PictureName = "confectionsfinal.jpg" 
            //    });
            //if (context.Categories.Any(m => m.PictureName == null))
            //    context.Categories.Update(new Categories 
            //    {
            //        CategoryId = 4,
            //        Description = "Cheeses",
            //        CategoryName = "Dairy Products",
            //        PictureName = "dairyproducts.jpg" 
            //    });
            //if (context.Categories.Any(m => m.PictureName == null))
            //    context.Categories.Update(new Categories 
            //    {
            //        CategoryId = 5,
            //        Description = "Breads, crackers, pasta, and cereal",
            //        CategoryName = "Grains/Cereals",
            //        PictureName = "grainproducts.jpg" 
            //    });
            //if (context.Categories.Any(m => m.PictureName == null))
            //    context.Categories.Update(new Categories 
            //    {
            //        CategoryId = 6,
            //        Description = "Prepared meats",
            //        CategoryName = "Meat/Poultry",
            //        PictureName = "meatproducts.png" 
            //    });
            //if (context.Categories.Any(m => m.PictureName == null))
            //    context.Categories.Update(new Categories 
            //    {
            //        CategoryId = 7,
            //        Description = "Dried fruit and bean curd",
            //        CategoryName = "Produce",
            //        PictureName = "produce.jpg" 
            //    });
            //if (context.Categories.Any(m => m.PictureName == null))
            //    context.Categories.Update(new Categories 
            //    {
            //        CategoryId = 8,
            //        Description = "Seaweed and fish",
            //        CategoryName = "Seafood",
            //        PictureName = "seafoodproducts.jpg" 
            //    });


            context.SaveChanges();
        }
    }
}
