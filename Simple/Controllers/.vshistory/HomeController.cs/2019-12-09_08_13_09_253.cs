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
            #region Initialize
            AerospikeClient client = new AerospikeClient("127.0.0.1", 3000);

            // Initialize policy.
            WritePolicy policy = new WritePolicy();
            policy.SetTimeout(50);  // 50 millisecond timeout.
            #endregion

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

            #region Read Records
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
            #endregion

            #region Delete Record
            Key keyDel = new Key("test", "myset", "mykey");
            client.Delete(policy, keyDel);
            #endregion

            #region Batch Reads
            //Multiple records can be read in a single batch call.
            Key[] keysBach = new Key[size];
            for (int i = 0; i < 1000; i++)
            {
                keys[i] = new Key("test", "myset", (i + 1));
            }
            Record[] records = client.Get(policy, keysBach);
            #endregion





            #region Cleaning Up
            //Call Close() when all transactions are finished and the application is ready to shutdown.
            //The AerospikeClient object can no longer be called after calling Close).
            client.Close();
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
