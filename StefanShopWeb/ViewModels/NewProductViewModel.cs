using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StefanShopWeb.ViewModels
{
    public class NewProductViewModel
    {
        public NewProduct ProductNew { get; set; }
        public Products Product { get; set; }
        public Suppliers Supplier { get; set; }
        public Categories Category { get; set; }

        public List<Products> ListProducts { get; set; }
        public List<Suppliers> ListSuppliers { get; set; }
        public List<Categories> ListCategories { get; set; }

        public Suppliers SelectedSupplier { get; set; }
        public Categories SelectedCategory { get; set; }

        public NewProductViewModel()
        {
            Product = new Products();
            ListProducts = new List<Products>();
            ListSuppliers = new List<Suppliers>();
            ListCategories = new List<Categories>();
            //CategoryList = new List<SelectListItem>();
            //IngredienseList = new List<SelectListItem>();



        }
    }
}
