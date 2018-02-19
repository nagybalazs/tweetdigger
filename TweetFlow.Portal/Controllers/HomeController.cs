using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TweetFlow.Model;
using TweetFlow.Model.Hubs;
using TweetFlow.StreamService;

namespace TweetFlow.Portal.Controllers
{
    public class HomeController : Controller
    {
        private string tguid;
        private StreamFactory streamFactory;

        public HomeController(StreamFactory streamFactory)
        {
            this.streamFactory = streamFactory;
        }
        public IActionResult Index()
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
