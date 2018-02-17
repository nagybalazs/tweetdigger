using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TweetFlow.EF;
using TweetFlow.StreamService;

namespace TweetFlow.Portal.Controllers
{
    public class HomeController : Controller
    {
        private StreamFactory streamFactory;

        public HomeController(StreamFactory streamFactory)
        {
            this.streamFactory = streamFactory;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
