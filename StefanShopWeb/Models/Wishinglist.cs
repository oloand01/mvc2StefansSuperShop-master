using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace StefanShopWeb.Models
{
    public partial class Wishinglist
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column("UserId")]
        public string UserId { get; set; }
        [Column("ProductID")]
        public int ProductID { get; set; }
    }
}
