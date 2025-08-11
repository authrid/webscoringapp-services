using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebScoringApi.Data;
using WebScoringApi.Models;

namespace WebScoringApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RiskCategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RiskCategoryController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/RiskCategory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RiskCategory>>> GetRiskCategories()
        {
            return await _context.RiskCategories.ToListAsync();
        }

        // GET: api/RiskCategory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RiskCategory>> GetRiskCategory(int id)
        {
            var riskCategory = await _context.RiskCategories.FindAsync(id);

            if (riskCategory == null)
            {
                return NotFound();
            }

            return riskCategory;
        }

        // POST: api/RiskCategory
        [HttpPost]
        public async Task<ActionResult<RiskCategory>> CreateRiskCategory(RiskCategory riskCategory)
        {
            _context.RiskCategories.Add(riskCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRiskCategory), new { id = riskCategory.Id }, riskCategory);
        }

        // PUT: api/RiskCategory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRiskCategory(int id, RiskCategory riskCategory)
        {
            if (id != riskCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(riskCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RiskCategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/RiskCategory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRiskCategory(int id)
        {
            var riskCategory = await _context.RiskCategories.FindAsync(id);
            if (riskCategory == null)
            {
                return NotFound();
            }

            _context.RiskCategories.Remove(riskCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RiskCategoryExists(int id)
        {
            return _context.RiskCategories.Any(e => e.Id == id);
        }
    }
}
