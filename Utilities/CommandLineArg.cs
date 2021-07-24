namespace AnalyticsStats.Utilities
{
    public class CommandLineArg
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ExpectsValue { get; set; } = true;
    }
}
