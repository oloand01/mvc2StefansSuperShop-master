using StefanShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StefanShopWeb.ViewModels
{
    public class AdminCategoryProductsViewModel
    {
        public IEnumerable<Products> prodList { get; set; }
        public Categories cats { get; set; }
        public PagingViewModel pagingViewModel { get; set; }
        public int categoryId { get; set; }
        public AdminCategoryProductsViewModel()
        {
            prodList = new List<Products>();
            cats = new Categories();
            pagingViewModel = new PagingViewModel();
        }
    }
}
