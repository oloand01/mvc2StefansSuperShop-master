using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StefanShopWeb.ViewModels
{
    public class AdminMessageViewModel
    {
        [Required]
        [StringLength(60)]
        public string Name { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }

       
    }
}
