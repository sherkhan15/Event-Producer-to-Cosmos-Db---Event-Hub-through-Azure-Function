using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTEmulator
{
    public class IoTData
    {
      
        [JsonProperty("iotid")]
        public string iotid;

        [JsonProperty("lat")]
        public double lat;

        [JsonProperty("longitude")]
        public double longitude;

        [JsonProperty("carid")]
        public string carid;

        [JsonProperty("temp")]
        public double temp;

        [JsonProperty("timestamp")]
        public long timestamp;

        public static explicit operator IoTData(Microsoft.Azure.Documents.Document doc)
        {
            IoTData _iotData = new IoTData();
            _iotData.iotid = doc.GetPropertyValue<string>("iotid");
            _iotData.temp = doc.GetPropertyValue<double>("temp");
            _iotData.timestamp = doc.GetPropertyValue<long>("timestamp");
            return _iotData;
        }
    }
}
