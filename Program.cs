using AnalyticsStats.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace AnalyticsStats
{
    class Program
    {
        const string _accountArgName = "account";
        const string _regionArgName = "region";
        const string _databaseArgName = "database";
        const string _containerArgName = "container";
        const string _keyArgName = "key";
        const string _rerunArgName = "rerun-connector";
        const string _archivedArgName = "include-archived";
        const string _helpArgName = "help";
        const string _outputFilename = "stats.json";
        const string _separator = "========================================";

        static void Main(string[] args)
        {
            // Parse the command line arguments.
            CommandLineParser parser = new CommandLineParser();
            try
            {
                parser.AddArgument(new CommandLineArg() { Name = _accountArgName, Description = "The name of the CosmosDB account (Needed to run the connector)." });
                parser.AddArgument(new CommandLineArg() { Name = _regionArgName, Description = "The CosmosDB region to read from (Optionalley needed to run the connector)." });
                parser.AddArgument(new CommandLineArg() { Name = _databaseArgName, Description = "The name of the database (Needed to run the connector)." });
                parser.AddArgument(new CommandLineArg() { Name = _containerArgName, Description = "The name of the container (Needed to run the connector)." });
                parser.AddArgument(new CommandLineArg() { Name = _keyArgName, Description = "The CosmosDB account key (Needed to run the connector)." });
                parser.AddArgument(new CommandLineArg() { Name = _rerunArgName, Description = "Whether or not to re-run the connector if connector output is available.", ExpectsValue = false });
                parser.AddArgument(new CommandLineArg() { Name = _archivedArgName, Description = "Whether or not to include archived partitions in the output.", ExpectsValue = false });
                parser.AddArgument(new CommandLineArg() { Name = _helpArgName, Description = "Prints the usage that you're reading right now.", ExpectsValue = false });
                parser.Parse(args);
            }
            catch (Exception ex)
            {
                PrintError(ex.Message, parser.UsageString);
                return;
            }

            try
            {
                // Print the usage, if requested.
                if (parser.Exists(_helpArgName))
                {
                    Console.WriteLine(parser.UsageString);
                    return;
                }

                // Run the connector to get the partition infos.
                ConnectorOutput connector = null;
                if (parser.Exists(_rerunArgName) || !ConnectorOutput.OutputExists)
                {
                    if (parser.Exists(_rerunArgName)) Console.WriteLine("Re-running CosmosDB analytical storage connector...");
                    else if (!ConnectorOutput.OutputExists) Console.WriteLine("Previous output not found. Running CosmosDB analytical storage connector...");

                    // Validate the arguments needed to run the connector.
                    if (!parser.Exists(_accountArgName) ||
                        !parser.Exists(_databaseArgName) ||
                        !parser.Exists(_containerArgName) ||
                        !parser.Exists(_keyArgName))
                    {
                        PrintError("Argument missing.", parser.UsageString);
                        return;
                    }

                    // Run the connector.
                    CosmosProperties props = new CosmosProperties()
                    {
                        Account = parser[_accountArgName],
                        Database = parser[_databaseArgName],
                        Container = parser[_containerArgName],
                        Key = parser[_keyArgName]
                    };
                    if (parser.Exists(_regionArgName)) props.Region = parser[_regionArgName];
                    connector = new ConnectorOutput(props);
                    Console.WriteLine(_separator);
                    connector.Run();
                    Console.WriteLine(_separator);
                }
                else
                {
                    Console.WriteLine($"Using previous connector output. To re-run the connector use '-{_rerunArgName}'.");
                    connector = new ConnectorOutput(new CosmosProperties());
                }

                // Get the parition information and collect some statistics.
                List<PartitionStats> stats = new List<PartitionStats>();
                foreach (var partition in connector.Partitions)
                {
                    if (parser.Exists(_archivedArgName) || !partition.IsArchived)
                    {
                        stats.Add(new PartitionStats(partition));
                    }
                }
                SaveStats(_outputFilename, stats);
                Console.WriteLine($"Wrote partition statistics to '{_outputFilename}'");
            }
            catch (Exception ex)
            {
                PrintError(ex.Message);
            }
        }

        static void PrintError(string message, string usage = null)
        {
            Console.WriteLine(_separator);
            Console.WriteLine($"ERROR: {message}");
            if (!string.IsNullOrWhiteSpace(usage))
            {
                Console.WriteLine($"\n{_separator}");
                Console.WriteLine(usage);
            }
        }

        static void SaveStats(string path, List<PartitionStats> stats)
        {
            FileMode mode = FileMode.CreateNew;
            if (File.Exists(path)) mode = FileMode.Truncate;
            using (Stream stream = new FileStream(path, mode))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.Write(JsonConvert.SerializeObject(stats));
            }
        }
    }
}
