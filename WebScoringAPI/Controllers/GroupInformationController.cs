using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebScoringApi.Data;
using WebScoringApi.Models;

namespace WebScoringApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupInformationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GroupInformationController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/GroupInformation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupInformation>>> GetAll()
        {
            return Ok(await _context.GroupInformations.ToListAsync());
        }

        // GET: api/GroupInformation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupInformation>> GetById(int id)
        {
            var groupInformation = await _context.GroupInformations.FindAsync(id);

            if (groupInformation == null)
            {
                return NotFound();
            }

            return Ok(groupInformation);
        }

        // POST: api/GroupInformation
        [HttpPost]
        public async Task<ActionResult<GroupInformation>> Create(GroupInformation groupInformation)
        {
            _context.GroupInformations.Add(groupInformation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = groupInformation.Id }, groupInformation);
        }

        // PUT: api/GroupInformation/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, GroupInformation groupInformation)
        {
            if (id != groupInformation.Id)
            {
                return BadRequest();
            }

            _context.Entry(groupInformation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupInformationExists(id))
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

        // DELETE: api/GroupInformation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var groupInformation = await _context.GroupInformations.FindAsync(id);
            if (groupInformation == null)
            {
                return NotFound();
            }

            _context.GroupInformations.Remove(groupInformation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GroupInformationExists(int id)
        {
            return _context.GroupInformations.Any(e => e.Id == id);
        }
    }
}
