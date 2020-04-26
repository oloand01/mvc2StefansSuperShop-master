using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StefanShopWeb.Data
{
    public class DatabaseInitializer
    {
        public void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            context.Database.Migrate();
        }
    }
}
