using Newtonsoft.Json;

namespace AnalyticsStats.FileInfo
{
    public class InvalidationFileInfo
    {
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        [JsonProperty(PropertyName = "sasUri")]
        public string sasUri { get; set; }
    }
}
