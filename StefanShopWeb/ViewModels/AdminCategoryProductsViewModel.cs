using StefanShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StefanShopWeb.ViewModels
{
    public class AdminCategoryProductsViewModel
    {
        public List<Products> prodList { get; set; }
        public Categories cats { get; set; }

        public AdminCategoryProductsViewModel()
        {
            prodList = new List<Products>();
            cats = new Categories();
        }
    }
}
