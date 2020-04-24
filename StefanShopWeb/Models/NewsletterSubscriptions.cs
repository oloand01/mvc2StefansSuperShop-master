using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StefanShopWeb.Models
{
    public class NewsletterSubscriptions
    {

        public NewsletterSubscriptions(string email)
        {
            Email = email;
        }
        public int Id { get; set; }
        public string Email { get; set; }
    }
}
