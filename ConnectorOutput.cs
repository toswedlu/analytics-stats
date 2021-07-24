using AnalyticsStats.FileInfo;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AnalyticsStats
{
    public class CosmosProperties
    {
        public string Region { get; set; }
        public string Account { get; set; }
        public string Database { get; set; }
        public string Container { get; set; }
        public string Key { get; set; }
    }

    public class ConnectorOutput
    {
        const string _exePath = @"ConnectorOutput\ConnectorOutput.exe";
        const string _filesJsonPath = @"files.json";

        CosmosProperties Properties { get; set; }

        Files _files = null;
        public PartitionInfo[] Partitions 
        { 
            get
            {
                if (_files == null)
                {
                    using (Stream stream = File.OpenRead(_filesJsonPath))
                    {
                        _files = Files.Load(stream);
                    }
                }
                return _files.Partitions;
            }
        }

        public static bool OutputExists { get { return File.Exists(_filesJsonPath); } }

        public ConnectorOutput(CosmosProperties properties)
        {
            Properties = properties;
        }

        public void Run()
        {
            using (Process proc = new Process())
            {
                proc.EnableRaisingEvents = true;
                proc.StartInfo.FileName = _exePath;
                proc.StartInfo.Arguments = BuildArgString();
                proc.Start();
                proc.WaitForExit();
                if (proc.ExitCode != 0)
                {
                    throw new Exception($"The Connector executable exited with error: {proc.ExitCode}");
                }
            }
        }

        private string BuildArgString()
        {
            string args = $"-account {Properties.Account} -database {Properties.Database} -container {Properties.Container} -key {Properties.Key}";
            StringBuilder builder = new StringBuilder(args);
            if (!string.IsNullOrWhiteSpace(Properties.Region))
            {
                builder.Append($" -region {Properties.Region}");
            }
            return builder.ToString();
        }
    }
}
