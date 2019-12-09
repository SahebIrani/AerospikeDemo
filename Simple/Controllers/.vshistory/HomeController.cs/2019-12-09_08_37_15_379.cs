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
            #region Test

            static void LogCallback(Log.Level level, string message) =>
                Console.WriteLine(DateTime.Now.ToString() + ' ' + level + ' ' + message);

            Log.SetLevel(Log.Level.INFO);
            Log.SetCallback(LogCallback);

            AsyncClient client = new AsyncClient("127.0.0.1", 3000);

            try
            {
                WritePolicy policy = new WritePolicy();
                policy.SetTimeout(50);


            }
            catch (AerospikeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
            #endregion


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
