using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace AnalyticsStats.FileInfo
{
    public class Files
    {
        [JsonProperty(PropertyName = "partitions")]
        public PartitionInfo[] Partitions { get; set; } = new PartitionInfo[0];

        public static Files Load(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize(reader, typeof(Files)) as Files;
            }
        }
    }
}
