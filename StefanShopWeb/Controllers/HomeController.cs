﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StefanShopWeb.Data;
using StefanShopWeb.Models;
using StefanShopWeb.Services;
using StefanShopWeb.ViewModels;

namespace StefanShopWeb.Controllers
{
    public class HomeController : Controller
    {
		// Kommentar från Mikael
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext context;
        private readonly INewsletterServices _services;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, INewsletterServices services)
        {
            _logger = logger;
            this.context = context;
            _services = services;
        }

        public IActionResult Index()
        {
            var model = new StartPageModel();
            model.TrendingCategories = context.Categories.Take(3).Select( c=> 
                        new StartPageModel.TrendingCategory { Id = c.CategoryId, Name = c.CategoryName }
                    ).ToList();

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewsletterSubscription(string email)
        {
            if(ModelState.IsValid)
            {
                if(_services.IsExistingNewsletterSubscription(email))
                {
                    var newsletterSubscription = new NewsletterSubscriptions();

                    newsletterSubscription.Email = email;

                    context.NewsletterSubscriptions.Add(newsletterSubscription);

                    context.SaveChanges();
            
                    return View("NewsletterSubscriptionConfirmation");
                }
            }
            return View("NewsletterSubscriptionError");
        }
    }
}
