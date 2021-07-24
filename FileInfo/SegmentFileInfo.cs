using Newtonsoft.Json;

namespace AnalyticsStats.FileInfo
{
    public class SegmentFileInfo
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        public long LSID { get; set; }

        [JsonProperty(PropertyName = "IsLastFile")]
        public bool IsLastFile { get; set; }

        [JsonProperty(PropertyName = "sasUri")]
        public string SasUri { get; set; }

        [JsonProperty(PropertyName = "isPartialSegment")]
        public bool IsPartialSegment { get; set; }
    }
}
