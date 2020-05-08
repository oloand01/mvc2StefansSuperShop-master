using StefanShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StefanShopWeb.ViewModels
{
    public class AdminCategoryProductsViewModel
    {
        public IEnumerable<CategoryProductsViewModel> prodList { get; set; }
        public Categories cats { get; set; }
        public PagingViewModel pagingViewModel { get; set; }
        public Products prods { get; set; }
        public int categoryId { get; set; }

        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public List<CategoryProductsListViewModel> Items { get; set; } = new List<CategoryProductsListViewModel>();
        public class CategoryProductsListViewModel
        {
            public int ProdId { get; set; }
            public string ProdName { get; set; }
            public DateTime? ProdDate { get; set; }
            public decimal? ProdPrice { get; set; }
        }

        public AdminCategoryProductsViewModel()
        {
            prodList = new List<CategoryProductsViewModel>();
            prods = new Products();
            cats = new Categories();
            pagingViewModel = new PagingViewModel();
        }

        public class CategoryProductsViewModel
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal? UnitPrice { get; set; }

            public short? UnitsInStock { get; set; }
            public short? UnitsOnOrder { get; set; }
            public bool IsWhished { get; set; }
        }

        //public class CategoryProductsListViewModel
        //{
            
        //}
    }
}
