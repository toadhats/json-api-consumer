using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace JSONApiConsumer
{

    class Program
    {
        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                // TODO - Send HTTP requests
                client.BaseAddress = new Uri("http://data.gov.au/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/action/datastore_search?resource_id=5a45d7b2-8579-425b-bb46-53a0e0bfa053&limit=1000");
                if (response.IsSuccessStatusCode)
                {
                    JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
                    IList<JToken> records = json["result"]["records"].Children().ToList();
                    IList< Clink > offices = new List<Clink>();
                    foreach(JToken record in records)
                    {
                        Clink clink = JsonConvert.DeserializeObject<Clink>(record.ToString());
                        Console.WriteLine("{0}\t-\t{1}", clink._id, clink.siteName, clink.postcode);
                        offices.Add(clink);
                    }
                    Console.WriteLine("VICTORIA:\n-------------------------------");
                    var inVic = (from office in offices
                                 where office.state == "VIC"
                                 orderby office.postcode ascending
                                 select new { office.siteName, office.postcode });
                    foreach(var vicOffice in inVic)
                    {
                        Console.WriteLine("{0}\t-\t{1}",vicOffice.postcode, vicOffice.siteName);
                    }
                    Console.WriteLine("{0} offices found in Victoria", inVic.Count());
                        
                }
            }
        }
    }
}
