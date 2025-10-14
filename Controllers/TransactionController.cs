using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models.Entities;
using MyFirstAPI.Models.DTOs.Transaction;

namespace MyFirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly AplikasiDbContext _context;

        public TransactionController(AplikasiDbContext context)
        {
            _context = context;
        }

        // 1. CREATE (POST /api/transactions)
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreateDto dto)
        {
            // 1️ Validasi input body
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 2️ Variabel total
            decimal totalAmount = 0;
            var transactionItems = new List<TransactionItem>();

            // 3️ Loop tiap item transaksi
            foreach (var itemDto in dto.Items)
            {
                // Validasi ProdukID
                var product = await _context.Produk.FindAsync(itemDto.ProductId);
                if (product == null)
                    return NotFound($"Produk dengan ID {itemDto.ProductId} tidak ditemukan.");

                // Validasi stok
                if (itemDto.Quantity > product.Stok)
                    return BadRequest($"Stok produk '{product.Nama}' tidak mencukupi. Stok tersedia: {product.Stok}");

                // Hitung subtotal
                decimal subTotal = itemDto.Quantity * product.Harga;
                totalAmount += subTotal;

                // Kurangi stok produk
                product.Stok -= itemDto.Quantity;

                // Buat object TransactionItem
                var transactionItem = new TransactionItem
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    SubTotal = subTotal
                };

                transactionItems.Add(transactionItem);
            }

            // 4️ Buat object Transaction utama
            var transaction = new Transaction
            {
                CustomerName = dto.CustomerName,
                TransactionDate = dto.TransactionDate,
                TotalAmount = totalAmount,
                Status = "Pending",
                Items = transactionItems
            };

            // 5️ Simpan ke database
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // 6️ Bentuk output hasil transaksi baru
            var result = new TransactionReadDto
            {
                Id = transaction.Id,
                CustomerName = transaction.CustomerName,
                TransactionDate = transaction.TransactionDate,
                TotalAmount = transaction.TotalAmount,
                Status = transaction.Status,
                Items = transaction.Items.Select(i => new TransactionItemDetailDto
                {
                    ProductId = i.ProductId,
                    ProductName = _context.Produk.FirstOrDefault(p => p.Id == i.ProductId)!.Nama,
                    Quantity = i.Quantity,
                    SubTotal = i.SubTotal
                }).ToList()
            };

            // 7️ Kembalikan response 201 Created
            return CreatedAtAction(nameof(CreateTransaction), new { id = transaction.Id }, result);
        }

        // 2. READ ONE (GET /api/transactions/{id})
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            // 1️ Ambil transaksi berdasarkan ID
            var transaction = await _context.Transactions
                .Include(t => t.Items)                // ambil item transaksi
                .ThenInclude(i => i.Product)          // ambil data produk di setiap item
                .FirstOrDefaultAsync(t => t.Id == id);

            // 2️ Jika tidak ditemukan
            if (transaction == null)
                return NotFound($"Transaksi dengan ID {id} tidak ditemukan.");

            // 3️ Bentuk data output DTO
            var result = new TransactionReadDto
            {
                Id = transaction.Id,
                CustomerName = transaction.CustomerName,
                TransactionDate = transaction.TransactionDate,
                TotalAmount = transaction.TotalAmount,
                Status = transaction.Status,
                Items = transaction.Items.Select(i => new TransactionItemDetailDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product.Nama,
                    Quantity = i.Quantity,
                    SubTotal = i.SubTotal
                }).ToList()
            };

            // 4️ Kembalikan hasil
            return Ok(result);
        }

        // 3. UPDATE (PUT /api/transactions/{id})
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] TransactionUpdateDto dto)
        {
            // 1️ Validasi input
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 2️ Cari transaksi berdasarkan ID (termasuk item + produk)
            var transaction = await _context.Transactions
                .Include(t => t.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
                return NotFound($"Transaksi dengan ID {id} tidak ditemukan.");

            // 3️ Cek apakah ada data yang diubah
            bool isUpdated = false;

            // 4️ Update CustomerName (jika diisi)
            if (!string.IsNullOrWhiteSpace(dto.CustomerName) && dto.CustomerName != transaction.CustomerName)
            {
                transaction.CustomerName = dto.CustomerName;
                isUpdated = true;
            }

            // 5️ Logika Pembatalan
            if (!string.IsNullOrWhiteSpace(dto.Status) && dto.Status != transaction.Status)
            {
                // Jika status baru = CANCELLED dan sebelumnya bukan CANCELLED
                if (dto.Status.ToUpper() == "CANCELLED" && transaction.Status.ToUpper() != "CANCELLED")
                {
                    foreach (var item in transaction.Items)
                    {
                        var product = item.Product;
                        product.Stok += item.Quantity; // kembalikan stok
                    }
                }

                transaction.Status = dto.Status.ToUpper(); // update status (PAID / CANCELLED / dsb)
                isUpdated = true;
            }

            if (!isUpdated)
                return BadRequest("Tidak ada perubahan yang dilakukan pada transaksi.");

            // 6️ Simpan ke database
            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Transaksi berhasil diperbarui." });
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Gagal memperbarui transaksi karena kesalahan database.");
            }
        }

        // 4. DELETE (DELETE /api/transactions/{id})
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            // 1️ Ambil transaksi dari database (termasuk item & produk)
            var transaction = await _context.Transactions
                .Include(t => t.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
                return NotFound($"Transaksi dengan ID {id} tidak ditemukan.");

            // 2️ Cek status transaksi
            if (transaction.Status.ToUpper() == "PAID")
                return BadRequest("Transaksi dengan status 'PAID' tidak dapat dihapus.");

            // 3️ Kembalikan stok produk
            foreach (var item in transaction.Items)
            {
                var product = item.Product;
                product.Stok += item.Quantity;
            }

            // 4️ Hapus transaksi (EF otomatis hapus anak-anaknya karena relasi)
            _context.Transactions.Remove(transaction);

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { Message = $"Transaksi dengan ID {id} berhasil dihapus." });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Terjadi kesalahan saat menghapus transaksi.");
            }
        }

    }
}
