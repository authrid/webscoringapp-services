using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebScoringApi.Data;
using WebScoringApi.Models;
using WebScoringApi.Services;

namespace WebScoringApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ScoreService _scoreService;

        public ApplicationController(AppDbContext context, ScoreService scoreService)
        {
            _context = context;
            _scoreService = scoreService;
        }

        // GET: api/application
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Application>>> GetApplications()
        {
            var apps = await _context.Applications.ToListAsync();
            return Ok(apps);
        }

        // GET: api/application/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Application>> GetApplication(int id)
        {
            var application = await _context.Applications
                .Include(a => a.ApplicationSelections)
                    .ThenInclude(s => s.ItemOption)
                    .ThenInclude(io => io!.GroupItem)
                    .ThenInclude(gi => gi!.GroupInformation)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            if (application.ApplicationSelections == null ||
                application.ApplicationSelections.Any(s => s.ItemOption?.GroupItem == null))
            {
                return NotFound();
            }

            return Ok(application);
        }

        // POST: api/application
        [HttpPost]
        public async Task<ActionResult<Application>> CreateApplication([FromBody] ApplicationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var application = dto.Application;
            var selections = dto.Selections;

            application.AppNo = GenerateAppNo();
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            var appSelections = new List<ApplicationSelection>();
            foreach (var selection in selections)
            {
                if (selection.Value == 0) continue;
                var option = await _context.ItemOptions
                    .Include(o => o.GroupItem)
                    .FirstOrDefaultAsync(o => o.Id == selection.Value);

                if (option != null && option.GroupItem != null)
                {
                    var bobotItem = option.BobotF * (option.GroupItem.BobotD / 100m);
                    appSelections.Add(new ApplicationSelection
                    {
                        ApplicationId = application.Id,
                        GroupItemId = selection.Key,
                        ItemOptionId = selection.Value,
                        Bobot = bobotItem,
                        HighRisk = option.HighRisk
                    });
                }
            }

            _context.ApplicationSelections.AddRange(appSelections);
            await _context.SaveChangesAsync();

            var (score, risk) = _scoreService.CalculateFinalScore(application.Id);
            application.TotalScore = score;
            application.RiskCategoryName = risk;
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
        }

        // PUT: api/application/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateApplication(int id, [FromBody] ApplicationDto dto)
        {
            if (id != dto.Application.Id)
                return BadRequest();

            var application = dto.Application;
            var selections = dto.Selections;

            _context.Entry(application).State = EntityState.Modified;

            var existingSelections = _context.ApplicationSelections
                .Where(s => s.ApplicationId == id);
            _context.ApplicationSelections.RemoveRange(existingSelections);

            var appSelections = new List<ApplicationSelection>();
            foreach (var selection in selections)
            {
                if (selection.Value == 0) continue;

                var option = await _context.ItemOptions
                    .Include(o => o.GroupItem)
                    .FirstOrDefaultAsync(o => o.Id == selection.Value);

                if (option != null && option.GroupItem != null)
                {
                    var bobotItem = option.BobotF * (option.GroupItem.BobotD / 100m);
                    appSelections.Add(new ApplicationSelection
                    {
                        ApplicationId = application.Id,
                        GroupItemId = selection.Key,
                        ItemOptionId = selection.Value,
                        Bobot = bobotItem,
                        HighRisk = option.HighRisk
                    });
                }
            }

            _context.ApplicationSelections.AddRange(appSelections);
            await _context.SaveChangesAsync();

            var (score, risk) = _scoreService.CalculateFinalScore(application.Id);
            application.TotalScore = score;
            application.RiskCategoryName = risk;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/application/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            var application = await _context.Applications
                .Include(a => a.ApplicationSelections)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            _context.ApplicationSelections.RemoveRange(application.ApplicationSelections);
            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private string GenerateAppNo()
        {
            return DateTime.UtcNow.ToString("yyMMdd") + new Random().Next(1000, 9999);
        }
    }
}
