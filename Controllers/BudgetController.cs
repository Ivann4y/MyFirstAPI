using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MyFirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController : ControllerBase
    {
        private static readonly List<Budget> budgets = new();

        // GET: api/Budget
        [HttpGet]
        public IActionResult GetAll() => Ok(budgets);

        // GET: api/Budget/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var budget = budgets.FirstOrDefault(b => b.Id == id);
            if (budget == null) return NotFound(new { message = "Budget tidak ditemukan" });
            return Ok(budget);
        }

        // POST: api/Budget
        [HttpPost]
        public IActionResult Add([FromBody] Budget newBudget)
        {
            budgets.Add(newBudget);
            return Ok(new { message = "Budget ditambahkan!", data = newBudget });
        }

        // PUT: api/Budget/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Budget updateBudget)
        {
            var budget = budgets.FirstOrDefault(b => b.Id == id);
            if (budget == null) return NotFound(new { message = "Budget tidak ditemukan" });

            budget.Name = updateBudget.Name;
            budget.Amount = updateBudget.Amount;

            return Ok(new { message = "Budget diupdate!", data = budget });
        }

        // DELETE: api/Budget/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var budget = budgets.FirstOrDefault(b => b.Id == id);
            if (budget == null) return NotFound(new { message = "Budget tidak ditemukan" });

            budgets.Remove(budget);
            return Ok(new { message = "Budget dihapus!" });
        }
    }

    public class Budget
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal Amount { get; set; }
    }
}
