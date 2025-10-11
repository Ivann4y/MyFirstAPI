using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models.DTOs;
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

        // 1. CREATE (POST)
        [HttpPost]
        public async Task<IActionResult> CreateProduk([FromBody] ProdukCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Cek Kategori (Logika Bisnis)
            var kategoriEntity = await _dbContext.Kategori.FindAsync(dto.KategoriId);
            if (kategoriEntity == null)
                return BadRequest(new { Message = $"Kategori ID {dto.KategoriId} tidak ditemukan." });

            // Pemetaan DTO ke Entity
            var produkEntity = new Produk
            {
                Nama = dto.Nama,
                Harga = dto.Harga,
                KategoriId = dto.KategoriId,
                TanggalDibuat = DateTime.UtcNow
            };

            try
            {
                _dbContext.Produk.Add(produkEntity);
                await _dbContext.SaveChangesAsync();

                // Pemetaan Entity ke DTO Output
                var produkReadDto = new ProdukReadDto
                {
                    Id = produkEntity.Id,
                    Nama = produkEntity.Nama,
                    Harga = produkEntity.Harga,
                    KategoriNama = kategoriEntity.Nama
                };

                return CreatedAtAction(nameof(GetProdukById), new { id = produkReadDto.Id }, produkReadDto);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Gagal menyimpan produk karena kesalahan database.");
            }
        }

        // 2. READ ALL(GET)
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
                    KategoriNama = p.Kategori.Nama
                })
                .ToListAsync();
            return Ok(produkList);
        }

        // 3. READ SINGLE (GET {id})
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
                KategoriNama = produkEntity.Kategori.Nama
            };
            return Ok(produkReadDto);
        }

        // 4. UPDATE (PUT {id})
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduk(int id, [FromBody] ProdukUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var produkEntity = await _dbContext.Produk.FindAsync(id);
            if (produkEntity == null)
                return NotFound($"Produk dengan ID {id} tidak ditemukan.");

            // Cek kebereadaan Kategori yang baru
            var kategoriExists = await _dbContext.Kategori.AnyAsync(k => k.Id == dto.KategoriId);
            if (!kategoriExists)
                return BadRequest(new { Message = $"Kategori ID {dto.KategoriId} tidak ditemukan." });

            // Update Entity
            produkEntity.Nama = dto.Nama;
            produkEntity.Harga = dto.Harga;
            produkEntity.KategoriId = dto.KategoriId;
            try
            {
                await _dbContext.SaveChangesAsync();
                return NoContent(); // Status 204 No Content
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Gagal memperbarui: Kesalahan konkurensi.");
            }
        }

        // 5. DELETE (DELETE {id})
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
                return NoContent(); // Status 204 No Content
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Gagal menghapus produk karena kesalahan database.");
            }
        }
    }
}
