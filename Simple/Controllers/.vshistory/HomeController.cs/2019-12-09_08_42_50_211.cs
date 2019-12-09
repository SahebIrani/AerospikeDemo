using System;
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
            static void LogCallback(Log.Level level, string message) =>
                Console.WriteLine(DateTime.Now.ToString() + ' ' + level + ' ' + message);

            Log.SetLevel(Log.Level.INFO);
            Log.SetCallback(LogCallback);

            AsyncClient clientA = new AsyncClient("127.0.0.1", 3000);
            AerospikeClient client = new AerospikeClient("127.0.0.1", 3000);

            try
            {
                WritePolicy policy = new WritePolicy();
                policy.SetTimeout(50);

                // Write a single value.
                Key key = new Key("test", "myset", "mykey");
                Bin bin = new Bin("mybin", "myvalue");
                clientA.Put(policy, key, bin);


            }
            catch (AerospikeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }

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
