using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core8_nextjs_postgre.Entities
{
    
[Table("products")]
public class Product {

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("category", TypeName ="varchar")]
        public string Category { get; set; }

        [Column("descriptions", TypeName ="varchar")]
        public string Descriptions { get; set; }

        [Column("qty")]
        public int Qty { get; set; }

        [Column("unit", TypeName ="varchar")]
        public string Unit { get; set; }

        [Column("costPrice",TypeName="decimal(10,2)")]
        public decimal CostPrice { get; set; }

        [Column("sellPrice",TypeName="decimal(10,2)")]
        public decimal SellPrice { get; set; }

        [Column("productPicture", TypeName ="varchar")]
        public string ProductPicture { get; set; }

        [Column("salePrice",TypeName="decimal(10,2)")]
        public decimal SalePrice { get; set; }

        [Column("alertStocks")]
        public int AlertStocks { get; set; }

        [Column("criticalStocks")]
        public int CriticalStocks { get; set; }

        [Column("createdAt", TypeName ="timestamp")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt", TypeName ="timestamp")]
        public DateTime UpdatedAt { get; set; }
    }    
}