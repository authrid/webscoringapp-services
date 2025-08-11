using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebScoringApi.Data;
using WebScoringApi.Models;

namespace WebScoringApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemOptionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemOptionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemOption>>> GetAll()
        {
            var itemOptions = await _context.ItemOptions
                .Include(io => io.GroupItem)
                .ToListAsync();
            return Ok(itemOptions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemOption>> GetById(int id)
        {
            var itemOption = await _context.ItemOptions
                .Include(io => io.GroupItem)
                .FirstOrDefaultAsync(io => io.Id == id);

            if (itemOption == null)
                return NotFound();

            return Ok(itemOption);
        }

        [HttpPost]
        public async Task<ActionResult<ItemOption>> Create(ItemOption itemOption)
        {
            _context.ItemOptions.Add(itemOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = itemOption.Id }, itemOption);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ItemOption itemOption)
        {
            if (id != itemOption.Id)
                return BadRequest();

            _context.Entry(itemOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ItemOptions.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var itemOption = await _context.ItemOptions.FindAsync(id);
            if (itemOption == null)
                return NotFound();

            _context.ItemOptions.Remove(itemOption);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
