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

            AerospikeClient client = new AerospikeClient("127.0.0.1", 3000);
            AsyncClient clientAsync = new AsyncClient("127.0.0.1", 3000);

            try
            {
                WritePolicy policyWrite = new WritePolicy();
                Policy policy = new Policy();
                policyWrite.SetTimeout(50);
                policy.SetTimeout(0);

                const string ns = "TestNS";
                const string setName = "SinjulMSBH";
                const string indexName = "IndexSinjulMSBH";
                const string keyPrefix = "PrefixSinjulMSBH";
                const string binName = "BinSinjulMSBH";
                const int size = 800;

                client.CreateIndex(policy, ns, setName, indexName, binName, IndexType.NUMERIC).Wait();

                Key keyAsync = new Key(ns, setName, keyPrefix);
                Bin binAsync = new Bin(binName, 0);
                clientAsync.Put(policyWrite, keyAsync, binAsync);

                for (int i = 1; i <= size; i++)
                {
                    Key key = new Key(ns, setName, keyPrefix + i);
                    Bin bin = new Bin(binName, i);
                    client.Put(policyWrite, key, bin);
                }

                Statement statement = new Statement();
                statement.SetNamespace(ns);
                statement.SetSetName(setName);
                statement.SetIndexName(indexName);
                statement.SetBinNames(binName);
                statement.SetFilter(Filter.Range(binName, 13, 62));

                QueryPolicy queryPolicy = new QueryPolicy();
                queryPolicy.SetTimeout(40);

                RecordSet recordSet = client.Query(queryPolicy, statement);
                while (recordSet.Next())
                {
                    Key key = recordSet.Key;
                    Record record = recordSet.Record;
                    object result = record.GetValue(binName);
                    Console.WriteLine("Record found: ns=" + key.ns +
                        " set=" + key.setName +
                        " bin=" + binName +
                        " digest=" + ByteUtil.BytesToHexString(key.digest) +
                        " value=" + result);
                }
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
