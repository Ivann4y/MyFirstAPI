using System.ComponentModel.DataAnnotations;

namespace MyFirstAPI.Models.DTOs
{
    public class ProdukCreateDto
    {
        [Required(ErrorMessage = "Nama produk harus diisi.")]
        public string Nama { get; set; }

        [Range(0.01, 1000000)]
        public decimal Harga { get; set; }

        [Required(ErrorMessage = "ID Kategori harus diisi.")]
        public int KategoriId { get; set; }
    }
}
