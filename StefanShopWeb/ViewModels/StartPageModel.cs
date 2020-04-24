using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StefanShopWeb.ViewModels
{
    public class StartPageModel
    {
        public class TrendingCategory
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        public List<TrendingCategory> TrendingCategories { get; set; }

        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Misspelled email address")]
        public string Email { get; set; }
    }
}
