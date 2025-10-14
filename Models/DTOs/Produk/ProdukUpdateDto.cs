using System.ComponentModel.DataAnnotations;

namespace MyFirstAPI.Models.DTOs.Produk
{
    public class ProdukUpdateDto
    {
        [Required(ErrorMessage = "Nama produk harus diisi.")]
        [StringLength(50, ErrorMessage = "Nama produk maksimal 50 karakter.")]
        public string Nama { get; set; } = string.Empty;

        [Range(0.01, 1000000, ErrorMessage = "Harga harus di antara 0.01 dan 1.000.000.")]
        public decimal Harga { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stok tidak boleh negatif.")]
        public int Stok { get; set; }

        [Required(ErrorMessage = "ID Kategori harus diisi.")]
        public int KategoriId { get; set; }
    }
}
