using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StefanShopWeb.ViewModels
{
    public class NewProduct
    {
        public int ProductId { get; set; }


        [Required]
        public string ProductName { get; set; }

        [Required]
        public int? SupplierId { get; set; }

        [Required]
        public int? CategoryId { get; set; }


        public string QuantityPerUnit { get; set; }

        [Required]
        public decimal? UnitPrice { get; set; }


        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        public virtual Categories Category { get; set; }
        public virtual Suppliers Supplier { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
