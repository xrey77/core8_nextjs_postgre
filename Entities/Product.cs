using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core8_nextjs_postgre.Entities
{
    
[Table("products")]
public class Product {

        [Key]
        [Column("id")]
        public int Id { get; set; }
        public string Category { get; set; }
        public string Descriptions { get; set; }
        public int Qty { get; set; }
        public string Unit { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public string ProductPicture { get; set; }
        public decimal SalePrice { get; set; }
        public int AlertStocks { get; set; }
        public int CriticalStocks { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }    
}