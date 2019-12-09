using System.Diagnostics;

using Aerospike.Client;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Simple.Models;

namespace Simple.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            AerospikeClient client = new AerospikeClient(new ClientPolicy(), "127.0.0.1", 3000);

            // Initialize policy.
            WritePolicy policy = new WritePolicy();
            policy.SetTimeout(50);  // 50 millisecond timeout.

            // Write single or multiple values.
            Key key = new Key("test", "myset", "mykey");
            Bin bin1 = new Bin("name", "John");
            Bin bin2 = new Bin("age", 25);
            client.Put(policy, key, bin1, bin2);




            client.Close();


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
