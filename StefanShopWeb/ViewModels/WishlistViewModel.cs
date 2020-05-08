using StefanShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StefanShopWeb.ViewModels
{
    public class WishlistViewModel
    {
        public List<Products> WishProducts { get; set; } = new List<Products>();
    }
}
