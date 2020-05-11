using System;
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
using StefanShopWeb.ViewModels;

namespace StefanShopWeb.Controllers
{
    public class ProductController : Controller 
    {

        private readonly ApplicationDbContext dbContext;


        private readonly UserManager<IdentityUser> _userManager;
        public ProductController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            dbContext = context;
            _userManager = userManager;
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
            product.FirstSalesDate = model.Product.FirstSalesDate;


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
                                                                               IsWhished = dbContext.Wishinglist.Where(w => w.ProductId == n.ProductId && w.UserId == userId).Any()}).AsQueryable();


            // Titel
            //if (viewModel.SelectedTitleSortingOption == 0 && viewModel.LastSelectedTitleSortingOption == 0)
            //{
            //    products = products.OrderBy(q => q.ProductName);
            //    viewModel.LastSelectedTitleSortingOption = 0;
            //}

            //if (viewModel.SelectedTitleSortingOption == 1 || viewModel.SelectedTitleSortingOption == 1)
            //{
            //    products = products.OrderBy(q => q.ProductName);
            //    viewModel.LastSelectedTitleSortingOption = 1;
            //}

            //if (viewModel.SelectedTitleSortingOption == 2 || viewModel.LastSelectedTitleSortingOption == 2)
            //{
            //    products = products.OrderByDescending(q => q.ProductName);
            //    viewModel.LastSelectedTitleSortingOption = 2;
            //}

            // Pris
            //if (viewModel.SelectedPriceSortingOption == 0 && viewModel.LastSelectedTitleSortingOption == 0)
            //{
            //    products = products.OrderBy(q => q.UnitPrice);
            //    viewModel.LastSelectedPriceSortingOption = 0;
            //}

            if (viewModel.SelectedPriceSortingOption == 1 || viewModel.LastSelectedPriceSortingOption == 1)
            {
                products = products.OrderBy(q => q.UnitPrice);
                viewModel.LastSelectedPriceSortingOption = 1;
            }

            if (viewModel.SelectedPriceSortingOption == 2 || viewModel.LastSelectedPriceSortingOption == 2)
            {
                products = products.OrderByDescending(q => q.UnitPrice);
                viewModel.LastSelectedTitleSortingOption = 2;
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
        public async Task<IActionResult> GetWishlist( )
        {
            var model = new WishlistViewModel();

            if (User.Identity.Name == null)
            {
                return View("~/Identity/Account/Login.cshtml");
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            model.WishProducts = dbContext.Wishinglist
                .Where(u => u.UserId == user.Id)
                .Where(p => p.Product.ProductId==p.ProductId)
                .Select(r => new Wishinglist 
                { 
                    Product=r.Product,
                    ProductId=r.ProductId,
                    UserId = r.UserId
                })
                .ToList();

            if (model.WishProducts.Count() == 0)
            {
                 return RedirectToAction("EmptyWishlist");
            }

            return View(model);
        }
        [Authorize]
        public IActionResult EmptyWishlist()
        {
            return View();
        }
        public async Task<IActionResult> AddToWishlist(int wishlistid, int productid)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (wishlistid == 0)
            {
                var wish = new Wishinglist { ProductId = productid, UserId = user.Id };
                await dbContext.Wishinglist.AddAsync(wish);
                dbContext.SaveChanges();
                return ViewComponent("WishIcon", new { userId = user.Id});
            }
            else
            {
                var wish = dbContext.Wishinglist.FirstOrDefault(w => w.ProductId == productid && w.UserId == user.Id);
                dbContext.Wishinglist.Remove(wish);
                dbContext.SaveChanges();
            }         
            //
            return View("HeartViewComponent");
        }

    }
}