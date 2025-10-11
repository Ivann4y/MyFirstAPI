using System.ComponentModel.DataAnnotations;

namespace MyFirstAPI.Models.Entities
{
    public class Produk
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama produk harus diisi.")]
        public string Nama { get; set; }

        [Range(0.01, 1000000)]
        public decimal Harga { get; set; }

        [Required(ErrorMessage = "ID Kategori harus diisi.")]
        public int KategoriId { get; set; }
        public Kategori Kategori { get; set; }

        public DateTime TanggalDibuat { get; set; }
    }
}
