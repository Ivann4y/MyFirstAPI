using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFirstAPI.Models.Entities
{
    public class Produk
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama produk harus diisi.")]
        [StringLength(50, ErrorMessage = "Nama produk maksimal 50 karakter.")]
        public string Nama { get; set; }

        [Range(0.01, 1000000, ErrorMessage = "Harga harus di antara 0.01 dan 1.000.000.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Harga { get; set; }

        [Required(ErrorMessage = "ID Kategori harus diisi.")]
        public int KategoriId { get; set; }
        public Kategori Kategori { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stok tidak boleh negatif.")]
        public int Stok { get; set; } = 0;

        public DateTime TanggalDibuat { get; set; } = DateTime.Now;
    }
}
