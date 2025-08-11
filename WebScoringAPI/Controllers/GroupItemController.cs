using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebScoringApi.Data;
using WebScoringApi.Models;

namespace WebScoringApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupItemController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/GroupItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupItem>>> GetGroupItems()
        {
            var groupItems = await _context.GroupItems
                .Include(g => g.GroupInformation)
                .ToListAsync();

            return Ok(groupItems);
        }

        // GET: api/GroupItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupItem>> GetGroupItem(int id)
        {
            var groupItem = await _context.GroupItems
                .Include(g => g.GroupInformation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (groupItem == null)
            {
                return NotFound();
            }

            return Ok(groupItem);
        }

        // POST: api/GroupItem
        [HttpPost]
        public async Task<ActionResult<GroupItem>> CreateGroupItem(GroupItem groupItem)
        {
            _context.GroupItems.Add(groupItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGroupItem), new { id = groupItem.Id }, groupItem);
        }

        // PUT: api/GroupItem/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroupItem(int id, GroupItem groupItem)
        {
            if (id != groupItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(groupItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupItemExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/GroupItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupItem(int id)
        {
            var groupItem = await _context.GroupItems.FindAsync(id);
            if (groupItem == null)
            {
                return NotFound();
            }

            _context.GroupItems.Remove(groupItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GroupItemExists(int id)
        {
            return _context.GroupItems.Any(e => e.Id == id);
        }
    }
}
