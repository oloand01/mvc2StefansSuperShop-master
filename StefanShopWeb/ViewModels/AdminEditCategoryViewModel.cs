using Microsoft.AspNetCore.Http;
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

        [Required(ErrorMessage = "Please enter a description")]
        [MaxLength(100)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please select a file to upload")]
        public IFormFile PictureName { get; set; } //?? string, kanske nåt annat
    }
}
