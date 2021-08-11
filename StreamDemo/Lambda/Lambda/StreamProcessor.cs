using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Microsoft.Azure.Documents;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Lambda
{
    public static class StreamProcessor
    {
        [FunctionName("StreamProcessor")]
        public static void Run([CosmosDBTrigger(
            databaseName: "demodb",
            collectionName: "conatiner1",
            ConnectionStringSetting = "DBConnection",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input, TraceWriter log)
        {

            string connectionString = ConfigurationSettings.AppSettings["EventHubConnection"];
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(connectionString)
            {
                EntityPath = "demoevent1"
            };
            var client = Microsoft.Azure.EventHubs.EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            foreach (var doc in input)
            {
                string json = string.Format("{{\"iotid\":\"{0}\",\"temp\":{1}}}", doc.GetPropertyValue<string>("iotid"), doc.GetPropertyValue<string>("temp"));
                EventData data = new Microsoft.Azure.EventHubs.EventData(Encoding.UTF8.GetBytes(json));
                client.SendAsync(data);
            }
            
        }
    }
}
