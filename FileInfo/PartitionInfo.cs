using Newtonsoft.Json;
using System.Collections.Generic;

namespace AnalyticsStats.FileInfo
{
    public class PartitionInfo
    {
        [JsonProperty(PropertyName = "partitionKeyRangeRid")]
        public string PartitionKeyRangeRid { get; set; }

        [JsonProperty(PropertyName = "uuid")]
        public string UUID { get; set; }

        [JsonProperty(PropertyName = "lastFlushedGLSN")]
        public long LastFlushedGLSN { get; set; }

        [JsonProperty(PropertyName = "isArchived")]
        public bool IsArchived { get; set; }

        [JsonProperty(PropertyName = "dataFilePath")]
        public string DataFilePath { get; set; }

        [JsonProperty(PropertyName = "invalidationFilePath")]
        public string InvalidationFilePath { get; set; }

        [JsonProperty(PropertyName = "segmentFiles")]
        public List<SegmentFileInfo> SegmentFiles { get; set; }

        [JsonProperty(PropertyName = "invalidationFiles")]
        public List<InvalidationFileInfo> InvalidationFiles { get; set; }
    }
}
