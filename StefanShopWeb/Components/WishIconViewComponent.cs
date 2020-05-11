using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StefanShopWeb.Data;
using StefanShopWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StefanShopWeb.Components
{
    public class WishIconViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public WishIconViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            var model = new WishIconViewModel { Quantity = await _context.Wishinglist.Where(u => u.UserId == userId).CountAsync() };

            return View("WishIconViewComponent", model);
        } 
    }
}
