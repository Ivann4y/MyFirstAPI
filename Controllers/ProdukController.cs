using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models.DTOs.Produk;
using MyFirstAPI.Models.Entities;

namespace MyFirstAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdukController : ControllerBase
    {
        private readonly AplikasiDbContext _dbContext;

        public ProdukController(AplikasiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // 1️. CREATE (POST)
        [HttpPost]
        public async Task<IActionResult> CreateProduk([FromBody] ProdukCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Cek kategori dulu
            var kategoriEntity = await _dbContext.Kategori.FindAsync(dto.KategoriId);
            if (kategoriEntity == null)
                return BadRequest(new { Message = $"Kategori ID {dto.KategoriId} tidak ditemukan." });

            // Pemetaan DTO -> Entity
            var produkEntity = new Produk
            {
                Nama = dto.Nama,
                Harga = dto.Harga,
                Stok = dto.Stok,
                KategoriId = dto.KategoriId,
                TanggalDibuat = DateTime.UtcNow
            };

            try
            {
                _dbContext.Produk.Add(produkEntity);
                await _dbContext.SaveChangesAsync();

                // Mapping hasil ke DTO Read
                var produkReadDto = new ProdukReadDto
                {
                    Id = produkEntity.Id,
                    Nama = produkEntity.Nama,
                    Harga = produkEntity.Harga,
                    Stok = produkEntity.Stok,
                    KategoriNama = kategoriEntity.Nama
                };

                return CreatedAtAction(nameof(GetProdukById), new { id = produkReadDto.Id }, produkReadDto);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Gagal menyimpan produk karena kesalahan database.");
            }
        }

        // 2️. READ ALL (GET)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdukReadDto>>> GetAllProduk()
        {
            var produkList = await _dbContext.Produk
                .Include(p => p.Kategori)
                .Select(p => new ProdukReadDto
                {
                    Id = p.Id,
                    Nama = p.Nama,
                    Harga = p.Harga,
                    Stok = p.Stok,
                    KategoriNama = p.Kategori.Nama
                })
                .ToListAsync();

            return Ok(produkList);
        }

        // 3️. READ BY ID (GET {id})
        [HttpGet("{id}")]
        public async Task<ActionResult<ProdukReadDto>> GetProdukById(int id)
        {
            var produkEntity = await _dbContext.Produk
                .Include(p => p.Kategori)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produkEntity == null)
                return NotFound($"Produk dengan ID {id} tidak ditemukan.");

            var produkReadDto = new ProdukReadDto
            {
                Id = produkEntity.Id,
                Nama = produkEntity.Nama,
                Harga = produkEntity.Harga,
                Stok = produkEntity.Stok,
                KategoriNama = produkEntity.Kategori.Nama
            };

            return Ok(produkReadDto);
        }

        // 4️. UPDATE (PUT {id})
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduk(int id, [FromBody] ProdukUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var produkEntity = await _dbContext.Produk.FindAsync(id);
            if (produkEntity == null)
                return NotFound($"Produk dengan ID {id} tidak ditemukan.");

            // Cek kategori baru
            var kategoriExists = await _dbContext.Kategori.AnyAsync(k => k.Id == dto.KategoriId);
            if (!kategoriExists)
                return BadRequest(new { Message = $"Kategori ID {dto.KategoriId} tidak ditemukan." });

            // Update field-field produk
            produkEntity.Nama = dto.Nama;
            produkEntity.Harga = dto.Harga;
            produkEntity.Stok = dto.Stok; 
            produkEntity.KategoriId = dto.KategoriId;

            try
            {
                await _dbContext.SaveChangesAsync();
                return NoContent(); // 204 sukses tanpa isi body
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Gagal memperbarui produk karena kesalahan konkurensi database.");
            }
        }

        // 5️ DELETE (DELETE {id})
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduk(int id)
        {
            var produkEntity = await _dbContext.Produk.FindAsync(id);
            if (produkEntity == null)
                return NotFound($"Produk dengan ID {id} tidak ditemukan.");

            try
            {
                _dbContext.Produk.Remove(produkEntity);
                await _dbContext.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Gagal menghapus produk karena kesalahan database.");
            }
        }
    }
}
