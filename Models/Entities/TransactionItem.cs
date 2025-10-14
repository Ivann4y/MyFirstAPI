using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyFirstAPI.Models.Entities
{
    public class TransactionItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; } = null!;
        [Required]
        public int ProductId { get; set; }
        public Produk Product { get; set; } = null!;
        [Required]
        public int Quantity { get; set; }
        [Precision(18, 2)]
        public decimal SubTotal { get; set; } = 0;
    }
}
