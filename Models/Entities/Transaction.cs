using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyFirstAPI.Models.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        [Required]
        [MaxLength(30)]
        public string CustomerName { get; set; } = string.Empty;
        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        // One-to-Many relationship with TransactionItem
        public ICollection<TransactionItem> Items { get; set; } = new List<TransactionItem>();
    }
}
