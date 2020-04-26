using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StefanShopWeb.ViewModels
{
    public class AdminNewsletterViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = new DateTime();
        [Required]
        public string Text { get; set; }
        public string Status { get; set; }

       
    }
}
