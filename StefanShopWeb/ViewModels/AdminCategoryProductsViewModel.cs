using Microsoft.AspNetCore.Mvc.Rendering;
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


        public string SelectedTitleSortingOption { get; set; }
        public string LastSelectedTitleSortingOption { get; set; }


        public string SelectedPriceSortingOption { get; set; }
        public string LastSelectedPriceSortingOption { get; set; }


        public string SelectedSortingOption { get; set; }
        public string LastSelectedDateSortingOption { get; set; }

        public IEnumerable<SelectListItem> SortingOptions
        {
            get
            {
                return new[]
                {
                    new SelectListItem("Filtrera", ""),
                    new SelectListItem("Senaste", "dateDesc"),
                    new SelectListItem("Äldsta", "dateAsc"),
                    new SelectListItem("Alfabetisk stigande", "titleAsc"),
                    new SelectListItem("Alfabetisk fallande", "titleDesc"),
                    new SelectListItem("Pris stigande", "priceAsc"),
                    new SelectListItem("Pris fallande", "priceDesc")
                };
            }
        }
        public IEnumerable<SelectListItem> TitleSortingOptions
        {
            get
            {
                return new[]
                {
                    new SelectListItem("Titel", ""),
                    new SelectListItem("Alfabetisk stigande", "titleAsc"),
                    new SelectListItem("Alfabetisk fallande", "titleDesc")
                };
            }
        }
        public IEnumerable<SelectListItem> PriceSortingOptions
        {
            get
            {
                return new[]
                {
                    new SelectListItem("Pris", ""),
                    new SelectListItem("Pris stigande", "priceAsc"),
                    new SelectListItem("Pris fallande", "priceDesc")
                };
            }
        }
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

            public DateTime FirstSalesDate { get; set; }
        }

        //public class CategoryProductsListViewModel
        //{
            
        //}
    }
}
