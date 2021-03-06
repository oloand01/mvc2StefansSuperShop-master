﻿using Microsoft.AspNetCore.Mvc;
using StefanShopWeb.Data;
using StefanShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace StefanShopWeb.Components
{
    public class HeartViewComponent : ViewComponent
    {
        private ApplicationDbContext _context;
        public HeartViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(string userId, int productId)
        {
            var wish = await _context.Wishinglist.FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
            if(Object.ReferenceEquals(wish, null))
            {
                wish = new Wishinglist { ProductId = productId };
            }
            return View("HeartViewComponent",wish);
        }
    }
}
