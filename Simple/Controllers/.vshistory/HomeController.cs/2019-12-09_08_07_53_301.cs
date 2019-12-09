using System;
using System.Collections.Generic;
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
            AerospikeClient client = new AerospikeClient("127.0.0.1", 3000);

            // Initialize policy.
            WritePolicy policy = new WritePolicy();
            policy.SetTimeout(50);  // 50 millisecond timeout.


            #region Write Records
            // Write single value.
            Key key = new Key("test", "myset", "mykey");
            Bin bin = new Bin("mybin", "myvalue");
            client.Put(policy, key, bin);

            // Write multiple values.
            Key key2 = new Key("test", "myset", "mykey");
            Bin bin1 = new Bin("name", "John");
            Bin bin2 = new Bin("age", 25);
            client.Put(policy, key2, bin1, bin2);
            #endregion


            //Reading a Single or Multiple  Value
            Record record = client.Get(policy, key, "name");
            if (record != null)
            {
                Console.WriteLine("Got name: " + record.GetValue("name"));
            }
            Record recordMultiple = client.Get(policy, key);
            if (recordMultiple != null)
            {
                foreach (KeyValuePair<string, object> entry in recordMultiple.bins)
                {
                    Console.WriteLine("Name=" + entry.Key + " Value=" + entry.Value);
                }
            }



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
