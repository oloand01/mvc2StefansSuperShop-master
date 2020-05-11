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
            if (context.Categories.Any(m => m.CategoryId == 1))
                context.Categories.Update(new Categories
                {
                    CategoryId = 1,
                    Description = "Savory beverages",
                    CategoryName = "Beverages",
                    PictureName = "beverages2_e6d2.png"
                });

            if (context.Categories.Any(m => m.CategoryId == 2))
                context.Categories.Update(new Categories
                {
                    CategoryId = 2,
                    Description = "Sweet and savory sauces, relishes, spreads, and seasonings",
                    CategoryName = "Condiments",
                    PictureName = "condiments.jpg" 
                });

            if (context.Categories.Any(m => m.CategoryId == 3))
                context.Categories.Update(new Categories 
                {
                    CategoryId = 3,
                    Description = "Desserts, candies, and sweet breads",
                    CategoryName = "Confections",
                    PictureName = "confectionsfinal.jpg" 
                });

            if (context.Categories.Any(m => m.CategoryId == 4))
                context.Categories.Update(new Categories 
                {
                    CategoryId = 4,
                    Description = "Cheeses",
                    CategoryName = "Dairy Products",
                    PictureName = "dairyproducts.jpg" 
                });

            if (context.Categories.Any(m => m.CategoryId == 5))
                context.Categories.Update(new Categories 
                {
                    CategoryId = 5,
                    Description = "Breads, crackers, pasta, and cereal",
                    CategoryName = "Grains/Cereals",
                    PictureName = "grainproducts.jpg" 
                });

            if (context.Categories.Any(m => m.CategoryId == 6))
                context.Categories.Update(new Categories 
                {
                    CategoryId = 6,
                    Description = "Prepared meats",
                    CategoryName = "Meat/Poultry",
                    PictureName = "meatproducts.png" 
                });

            if (context.Categories.Any(m => m.CategoryId == 7))
                context.Categories.Update(new Categories 
                {
                    CategoryId = 7,
                    Description = "Dried fruit and bean curd",
                    CategoryName = "Produce",
                    PictureName = "produce.jpg" 
                });

            if (context.Categories.Any(m => m.CategoryId == 8))
                context.Categories.Update(new Categories 
                {
                    CategoryId = 8,
                    Description = "Seaweed and fish",
                    CategoryName = "Seafood",
                    PictureName = "seafoodproducts.jpg" 
                });

            context.SaveChanges();
        }
    }
}
