using WebScoringApi.Models;

namespace WebScoringApi.Controllers
{
    public class ApplicationDto
    {
        public Application Application { get; set; } = default!;
        public Dictionary<int, int> Selections { get; set; } = new();
    }
}
