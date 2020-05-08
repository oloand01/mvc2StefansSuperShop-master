using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

            var items = dbContext.Products.Where(p => p.ProductId == id).Select(o => new AdminCategoryProductsViewModel.CategoryProductsListViewModel
            {
                ProdName = o.ProductName,
                ProdPrice = o.UnitPrice,
                ProdDate = o.FirstSalesDate,
            }) ;

            if (string.IsNullOrEmpty(sortorder))
                sortorder = "asc";

            items = AddSorting(items, ref sortcolumn, ref sortorder);
            viewModel.Items = items.ToList();
            viewModel.SortColumn = sortcolumn;
            viewModel.SortOrder = sortorder;

            return View("CategoryProductsParent", viewModel);
        }

        private IQueryable<AdminCategoryProductsViewModel.CategoryProductsListViewModel> AddSorting(IQueryable<AdminCategoryProductsViewModel.CategoryProductsListViewModel> items, ref string sortcolumn, ref string sortorder)
        {
            if (string.IsNullOrEmpty(sortcolumn))
                sortcolumn = "id";
            if (string.IsNullOrEmpty(sortorder))
                sortorder = "asc";


            if (sortcolumn == "Name")
            {
                if (sortorder == "asc")
                    items = items.OrderBy(p => p.ProdName);
                else
                    items = items.OrderByDescending(p => p.ProdName);
            }
            else if (sortcolumn == "Date")
            {
                if (sortorder == "asc")
                    items = items.OrderBy(p => p.ProdDate);
                else
                    items = items.OrderByDescending(p => p.ProdDate);

            }
            else
            {
                if (sortorder == "asc")
                    items = items.OrderBy(p => p.ProdPrice);
                else
                    items = items.OrderByDescending(p => p.ProdPrice);

                sortcolumn = "Price";
            }

            return items;

        }

        
        public IActionResult ProductPagingResult(AdminCategoryProductsViewModel viewModel, int? Page, int? PageSize)
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
            var products = dbContext.Products.Where(p => p.CategoryId == viewModel.categoryId).Select(n => 
                new AdminCategoryProductsViewModel.CategoryProductsViewModel { ProductId = n.ProductId, 
                                                                               ProductName = n.ProductName, 
                                                                               UnitPrice = n.UnitPrice, 
                                                                               UnitsInStock = n.UnitsInStock,  
                                                                               UnitsOnOrder = n.UnitsOnOrder, 
                                                                               IsWhished = dbContext.Wishinglist.Where(w => w.ProductId == n.ProductId && w.UserId == userId).Any()}).AsQueryable();

            products = viewModel.pagingViewModel.SetPaging(viewModel.pagingViewModel.Page, viewModel.pagingViewModel.PageSize, products).Cast<AdminCategoryProductsViewModel.CategoryProductsViewModel>();
            products.OrderBy(q => q.ProductName);
            viewModel.prodList = products.ToList();
            //viewModel.prods = products.ToList();
            return viewModel;
        }
        public async Task<IActionResult> AddToWishlist(int wishlistid, int productid)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (wishlistid == 0)
            {
                var wish = new Wishinglist { ProductId = productid, UserId = user.Id };
                await dbContext.Wishinglist.AddAsync(wish);
                
            }
            else
            {
                var wish = dbContext.Wishinglist.FirstOrDefault(w => w.ProductId == productid && w.UserId == user.Id);
                dbContext.Wishinglist.Remove(wish);
                dbContext.SaveChanges();
            }
            //

            return View();
        }

    }
}