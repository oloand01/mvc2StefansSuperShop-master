﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using StefanShopWeb.Data;
using StefanShopWeb.Models;
using StefanShopWeb.Services;
using StefanShopWeb.ViewModels;

namespace StefanShopWeb.Controllers
{
    public class ProductController : Controller 
    {

        private readonly ApplicationDbContext dbContext;

        private readonly IWishlistService _wishlistService;
        private readonly UserManager<IdentityUser> _userManager;
        public ProductController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWishlistService wishlistService)
        {
            dbContext = context;
            _userManager = userManager;
            _wishlistService = wishlistService;
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
                product.FirstSalesDate = products.Product.FirstSalesDate;



                dbContext.SaveChanges();

                return RedirectToAction("Index", "Product");
            }

            return RedirectToAction("UpdateProduct", "Product");
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
            product.FirstSalesDate = model.ProductNew.FirstSalesDate;


            dbContext.Products.Add(product);
            dbContext.SaveChanges();

            return RedirectToAction("Index", "Product");
        }


        public IActionResult DeleteProduct(int id)
        {
            var product = dbContext.Products.SingleOrDefault(x => x.ProductId == id);
           

            if (ModelState.IsValid)
            {
                dbContext.Remove(product);
                dbContext.SaveChanges();
            }

            return RedirectToAction("Index", "Product");
        }

        
        public IActionResult CategoryProducts(int id, string sortcolumn, string sortorder)
        {
            var viewModel = new AdminCategoryProductsViewModel();
            viewModel.categoryId = id;
            viewModel = SetProductListProperties(viewModel);

            return View("CategoryProductsParent", viewModel);
        }

        public IActionResult ProductPagingResult(AdminCategoryProductsViewModel viewModel, int? Page, int? PageSize, int lastSelectedTitleSortingOption, int lastSelectedPriceSortingOption)
        {
            if (Page != null) viewModel.pagingViewModel.Page = Page.GetValueOrDefault();
            if (PageSize != null) viewModel.pagingViewModel.PageSize = PageSize.GetValueOrDefault();
            
            viewModel = SetProductListProperties(viewModel);
            return PartialView("PagerAndTablePartial", viewModel);
        }

        public AdminCategoryProductsViewModel SetProductListProperties(AdminCategoryProductsViewModel viewModel)
        {
            var userId = _userManager.GetUserId(HttpContext.User);

            viewModel.cats = dbContext.Categories.SingleOrDefault(c => c.CategoryId == viewModel.categoryId);
            
            var products = dbContext.Products.Where(p => p.CategoryId == viewModel.categoryId && p.Discontinued == false).Select(n => 
            new AdminCategoryProductsViewModel.CategoryProductsViewModel { ProductId = n.ProductId, 
                                                                               ProductName = n.ProductName, 
                                                                               UnitPrice = n.UnitPrice, 
                                                                               UnitsInStock = n.UnitsInStock,  
                                                                               UnitsOnOrder = n.UnitsOnOrder, 
                                                                               FirstSalesDate = n.FirstSalesDate,
                                                                               IsWhished = dbContext.Wishinglist.Where(w => w.ProductId == n.ProductId && w.UserId == userId).Any()}).AsQueryable();



            if (viewModel.SelectedSortingOption == "titleAsc")
            {
                products = products.OrderBy(q => q.ProductName);
            }

            if (viewModel.SelectedSortingOption == "titleDesc")
            {
                products = products.OrderByDescending(q => q.ProductName);
            }

            //Datum
            if (viewModel.SelectedSortingOption ==  "dateAsc")
            {
                products = products.OrderBy(q => q.FirstSalesDate);
            }

            if (viewModel.SelectedSortingOption == "dateDesc")
            {
                products = products.OrderByDescending(q => q.FirstSalesDate);
            }

            // Pris
            if (viewModel.SelectedSortingOption == "priceAsc")
            {
                products = products.OrderBy(q => q.UnitPrice);
            }

            if (viewModel.SelectedSortingOption == "priceDesc")
            {
                products = products.OrderByDescending(q => q.UnitPrice);
            }




            products = viewModel.pagingViewModel.SetPaging(viewModel.pagingViewModel.Page, viewModel.pagingViewModel.PageSize, products).Cast<AdminCategoryProductsViewModel.CategoryProductsViewModel>();

            viewModel.prodList = products.ToList();

            //if (viewModel.SelectedSortingOption == 1)
            //{
            //    products = products.OrderBy(q => q.ProductName);
            //}


            //if (viewModel.SelectedSortingOption == 2)
            //    products = products.OrderByDescending(q => q.ProductName);

            return viewModel;
        }
        [Authorize]
        public async Task<IActionResult> GetWishlist(WishlistViewModel viewModel, int? Page, int? PageSize)
        {
            if (User.Identity.Name == null)
            {
                return View("~/Identity/Account/Login.cshtml");
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            viewModel = _wishlistService.FetchWishlistItems(viewModel, Page, PageSize, user);

            if (viewModel.WishProducts.Count() == 0)
            {
                return RedirectToAction("EmptyWishlist");
            }

            return View(viewModel);
        }

        [Authorize]
        public IActionResult EmptyWishlist()
        {
            return View();
        }

        public async Task<IActionResult> AddToWishlist(int wishlistid, int productid)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (!await dbContext.Wishinglist.AnyAsync(w => w.ProductId == productid && w.UserId == _userManager.GetUserId(HttpContext.User)))
                {
                    var wish = new Wishinglist { ProductId = productid, UserId = user.Id };
                    await dbContext.Wishinglist.AddAsync(wish);
                    dbContext.SaveChanges();
                }
            else
            {
                var wish = dbContext.Wishinglist.FirstOrDefault(w => w.ProductId == productid && w.UserId == user.Id);
                dbContext.Wishinglist.Remove(wish);
                dbContext.SaveChanges();
            }         
            //
            return ViewComponent("WishIcon", new { userId = user.Id });
        }

    }
}