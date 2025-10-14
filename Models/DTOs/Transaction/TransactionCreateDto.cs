using System.ComponentModel.DataAnnotations;

namespace MyFirstAPI.Models.DTOs.Transaction
{
    public class TransactionCreateDto
    {
        [Required(ErrorMessage = "Nama pelanggan wajib diisi.")]
        [StringLength(30, ErrorMessage = "Nama pelanggan maksimal 30 karakter.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tanggal transaksi wajib diisi.")]
        public DateTime TransactionDate { get; set; }

        [Required(ErrorMessage = "Minimal harus ada satu item dalam transaksi.")]
        [MinLength(1, ErrorMessage = "Daftar item tidak boleh kosong.")]
        public List<TransactionItemDto> Items { get; set; } = new List<TransactionItemDto>();
    }
}
   