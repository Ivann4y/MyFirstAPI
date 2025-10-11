using System.ComponentModel.DataAnnotations;

namespace MyFirstAPI.Models.Entities
{
    public class Kategori
    {
        public int Id { get; set; }
        public string Nama { get; set; }
        public ICollection<Produk> Produks { get; set; }
    }
}
