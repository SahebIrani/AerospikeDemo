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
            static void LogCallback(Log.Level level, string message) =>
                Console.WriteLine(DateTime.Now.ToString() + ' ' + level + ' ' + message);

            Log.SetLevel(Log.Level.INFO);
            Log.SetCallback(LogCallback);

            AsyncClient clientAsync = new AsyncClient("127.0.0.1", 3000);
            AerospikeClient client = new AerospikeClient("127.0.0.1", 3000);

            try
            {
                Policy policy = new Policy();
                WritePolicy policyWrite = new WritePolicy();
                QueryPolicy queryPolicy = new QueryPolicy();

                policy.SetTimeout(0);
                policyWrite.SetTimeout(40);
                queryPolicy.SetTimeout(40);

                const string ns = "test";
                const string setName = "SetSinjul_MSBH";
                const string indexName = "IndexSinjul_MSBH";
                const string keyPrefix = "PrefixSinju_lMSBH";
                const string binName = "BinSinjul_MSBH";
                const int size = 40;

                IndexTask task = client.CreateIndex(policy, ns, setName, indexName, binName, IndexType.NUMERIC);
                task.Wait();

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
                statement.SetFilter(Filter.Range(binName, 13, 35));

                List<object> result = new List<object>();

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


                    var item = new
                    {
                        nsNmae = key.ns,
                        setName = key.setName,
                        binName = binName,
                        digest = ByteUtil.BytesToHexString(key.digest),
                        value = result
                    };
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
