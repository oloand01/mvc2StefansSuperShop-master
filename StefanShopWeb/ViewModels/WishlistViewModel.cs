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
        public List<Wishinglist> WishProducts { get; set; } = new List<Wishinglist>();
        public PagingViewModel pagingViewModel { get; set; } = new PagingViewModel();

    }
}
