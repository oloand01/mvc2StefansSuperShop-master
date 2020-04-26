using StefanShopWeb.Data;
using StefanShopWeb.Models;
using StefanShopWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StefanShopWeb.Services
{
    public interface INewsletterServices
    {
        public List<AdminNewsletterViewModel> GetNewsLetterList();
        public void CreateNews(AdminNewsletterViewModel model);
        public AdminNewsletterViewModel GetNewsText(int id);
        public void EditNews(AdminNewsletterViewModel model);
        public void SendNews(AdminMessageViewModel model);
        public void DeleteNewsletter(int id);
    }
}
