using System.ComponentModel.DataAnnotations;

namespace MyFirstAPI.Models.DTOs.Transaction
{
    public class TransactionUpdateDto
    {
        [StringLength(30, ErrorMessage = "Nama pelanggan maksimal 30 karakter.")]
        public string? CustomerName { get; set; }

        [StringLength(20, ErrorMessage = "Status maksimal 20 karakter.")]
        public string? Status { get; set; }
    }
}
