using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MyFirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private static readonly List<Transaction> transactions = new();

        // GET: api/Transaction
        [HttpGet]
        public IActionResult GetAll() => Ok(transactions);

        // GET: api/Transaction/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var trans = transactions.FirstOrDefault(t => t.Id == id);
            if (trans == null) return NotFound(new { message = "Transaction tidak ditemukan" });
            return Ok(trans);
        }

        // POST: api/Transaction
        [HttpPost]
        public IActionResult Add([FromBody] Transaction newTrans)
        {
            transactions.Add(newTrans);
            return Ok(new { message = "Transaction ditambahkan!", data = newTrans });
        }

        // PUT: api/Transaction/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Transaction updateTrans)
        {
            var trans = transactions.FirstOrDefault(t => t.Id == id);
            if (trans == null) return NotFound(new { message = "Transaction tidak ditemukan" });

            trans.Description = updateTrans.Description;
            trans.Amount = updateTrans.Amount;
            trans.Date = updateTrans.Date;

            return Ok(new { message = "Transaction diupdate!", data = trans });
        }

        // DELETE: api/Transaction/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var trans = transactions.FirstOrDefault(t => t.Id == id);
            if (trans == null) return NotFound(new { message = "Transaction tidak ditemukan" });

            transactions.Remove(trans);
            return Ok(new { message = "Transaction dihapus!" });
        }
    }

    public class Transaction
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public string? Date { get; set; }
    }
}
