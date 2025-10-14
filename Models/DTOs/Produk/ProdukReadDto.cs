namespace MyFirstAPI.Models.DTOs.Produk
{
    public class ProdukReadDto
    {
        public int Id { get; set; }
        public string Nama { get; set; } = string.Empty;
        public decimal Harga { get; set; }
        public int Stok { get; set; }
        public string KategoriNama { get; set; } = string.Empty;
    }
}
