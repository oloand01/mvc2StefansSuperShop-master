using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StefanShopWeb.Data;
using StefanShopWeb.Models;
using StefanShopWeb.ViewModels;

namespace StefanShopWeb.Controllers
{
    public class ProductController : Controller 
    {

        private readonly ApplicationDbContext dbContext;

       

        public ProductController(ApplicationDbContext context) 
        {
            dbContext = context;
        }

        public IActionResult Index()
        {
            var model = dbContext.Products.ToList();

            return View(model);
        }

        public IActionResult UpdateProduct(int id)
        {
            NewProductViewModel model = new NewProductViewModel();
            model.Product = dbContext.Products.SingleOrDefault(x => x.ProductId == id);

            model.ListCategories = dbContext.Categories.ToList();
            model.ListSuppliers = dbContext.Suppliers.ToList();



            model.SelectedCategory = (from p in dbContext.Categories
                                      join m in dbContext.Products on p.CategoryId equals m.CategoryId
                                      where m.ProductId == id
                                      select p).SingleOrDefault();

            model.SelectedSupplier = (from p in dbContext.Suppliers
                                      join m in dbContext.Products on p.SupplierId equals m.SupplierId
                                      where m.ProductId == id
                                      select p).SingleOrDefault();



            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateProduct(NewProductViewModel products)
        {

            if (ModelState.IsValid)
            {
                var product = dbContext.Products.SingleOrDefault(x => x.ProductId == products.Product.ProductId);


                product.ProductName = products.Product.ProductName;
                product.CategoryId = products.Product.CategoryId;
                product.UnitPrice = products.Product.UnitPrice;
                product.SupplierId = products.Product.SupplierId;
                product.QuantityPerUnit = products.Product.QuantityPerUnit;
                product.UnitsInStock = products.Product.UnitsInStock;
                product.UnitsOnOrder = products.Product.UnitsOnOrder;
                product.ReorderLevel = products.Product.ReorderLevel;
                product.Discontinued = products.Product.Discontinued;



                dbContext.SaveChanges();

                return RedirectToAction("Index", "ProductC");
            }

            return RedirectToAction("UpdateProduct", "ProductC");
        }

        public IActionResult CreateProduct()
        {
            NewProductViewModel model = new NewProductViewModel();
            model.ListCategories = dbContext.Categories.ToList();
            model.ListSuppliers = dbContext.Suppliers.ToList();


            return View(model);
        }

        [HttpPost]
        public IActionResult CreateProduct(NewProductViewModel model)
        {

            Products product = new Products();
            product.ProductName = model.ProductNew.ProductName;
            product.CategoryId = model.ProductNew.CategoryId;
            product.UnitPrice = model.ProductNew.UnitPrice;
            product.SupplierId = model.ProductNew.SupplierId;
            product.QuantityPerUnit = model.ProductNew.QuantityPerUnit;
            product.UnitsInStock = model.ProductNew.UnitsInStock;
            product.UnitsOnOrder = model.ProductNew.UnitsOnOrder;
            product.ReorderLevel = model.ProductNew.ReorderLevel;
            product.Discontinued = model.ProductNew.Discontinued;

            dbContext.Products.Add(product);
            dbContext.SaveChanges();

            return RedirectToAction("Index", "ProductC");
        }


        public IActionResult Category(int id)
        {

            return View();
        }

    }
}