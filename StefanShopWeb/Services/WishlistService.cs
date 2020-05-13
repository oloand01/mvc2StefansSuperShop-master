using Microsoft.AspNetCore.Identity;
using StefanShopWeb.Data;
using StefanShopWeb.Models;
using StefanShopWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StefanShopWeb.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly ApplicationDbContext _context;
        public WishlistService(ApplicationDbContext context)
        {
            _context = context;
        }
        public WishlistViewModel FetchWishlistItems(WishlistViewModel viewModel, int? Page, int? PageSize, IdentityUser user)
        {
            if (Page != null) viewModel.pagingViewModel.Page = Page.GetValueOrDefault();
            if (PageSize != null) viewModel.pagingViewModel.PageSize = PageSize.GetValueOrDefault();

            viewModel.WishProducts = _context.Wishinglist
                .Where(u => u.UserId == user.Id)
                .Select(r => new Wishinglist
                {
                    Product = r.Product,
                    ProductId = r.ProductId,
                    UserId = r.UserId
                })
                .ToList();

            viewModel.WishProducts = viewModel.pagingViewModel
                .SetPaging(viewModel.pagingViewModel.Page, viewModel.pagingViewModel.PageSize, viewModel.WishProducts.AsQueryable())
                .Cast<Wishinglist>()
                .ToList();

            return viewModel;
        }
    }
}
