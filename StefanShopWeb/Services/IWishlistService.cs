using Microsoft.AspNetCore.Identity;
using StefanShopWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StefanShopWeb.Services
{
    public interface IWishlistService
    {
        WishlistViewModel FetchWishlistItems(WishlistViewModel viewModel, int? Page, int? PageSize, IdentityUser user);
    }
}
