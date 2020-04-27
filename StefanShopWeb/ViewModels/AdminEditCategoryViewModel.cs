using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StefanShopWeb.ViewModels
{
    public class AdminEditCategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(15)]
        [Display(Name = "Category name")]
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; } //?? string, kanske nåt annat
    }
}
