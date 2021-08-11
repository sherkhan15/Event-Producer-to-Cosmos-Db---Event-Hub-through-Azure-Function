
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Azure.EventHubs;

namespace EventHub_DataProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationSettings.AppSettings["EventHubConnection"];
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(connectionString)
            {
                EntityPath = "rafat-eventhub1"
            };
            var client = Microsoft.Azure.EventHubs.EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            int i = 0;

            while (i++ < 1000000) {
                Random rnd = new Random(i);
                int _temp = rnd.Next(100);
                string json = string.Format("{{\"iotid\":\"{0}\",\"temp\":{1}}}", GetRandomlyRepeatedString(3), _temp);
                EventData data = new Microsoft.Azure.EventHubs.EventData(Encoding.UTF8.GetBytes(json));
                client.SendAsync(data);
                System.Threading.Thread.Sleep(100);
                Console.Write(".");
                }

            Console.WriteLine("Done sending 100 events");
            Console.Read();


        }


         public static string GetRandomlyRepeatedString(int length)
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                sb.Append((char)random.Next('A', 'C'));
            }
            return sb.ToString();
        }

        //public class IoTData
        //{

        //    [JsonProperty("iotid")]
        //    public string iotid;

        //    [JsonProperty("temp")]
        //    public double temp;

        //    [JsonProperty("timestamp")]
        //    public long timestamp;

        //    public static explicit operator IoTData(Document doc)
        //    {
        //        IoTData _iotData = new IoTData();
        //        _iotData.id = doc.Id;
        //        _iotData.iotid = doc.GetPropertyValue<string>("iotid");
        //        _iotData.temp = doc.GetPropertyValue<double>("temp");
        //        _iotData.timestamp = doc.GetPropertyValue<long>("timestamp");
        //        return _iotData;
        //    }
        //}

    }
}
