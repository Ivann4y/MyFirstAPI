using System.ComponentModel.DataAnnotations;

namespace MyFirstAPI.Models.DTOs.Transaction
{
    public class TransactionItemDto
    {
        [Required(ErrorMessage = "ProductId wajib diisi.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity wajib diisi.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity harus minimal 1.")]
        public int Quantity { get; set; }

    }
}
