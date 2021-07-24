using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyticsStats.Utilities
{
    public class CommandLineParser
    {
        const string _nameDelim = "-";

        Dictionary<string, CommandLineArg> _allowedArguments = new Dictionary<string, CommandLineArg>();
        Dictionary<string, string> _args = new Dictionary<string, string>();

        private enum State
        {
            Name,
            Value
        }

        public string this[string argName]
        {
            get { return _args[argName]; }
        }

        public string UsageString
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Usage:");
                foreach (var pair in _allowedArguments)
                {
                    builder.AppendLine($"{_nameDelim}{pair.Key}: {pair.Value.Description}");
                }
                return builder.ToString();
            }
        }

        public void AddArgument(CommandLineArg arg)
        {
            _allowedArguments.Add(arg.Name, arg);
        }

        public bool Exists(string argName)
        {
            return _args.ContainsKey(argName);
        }

        public void Parse(string[] args)
        {
            
            State state = State.Name;
            string currName = string.Empty;
            foreach (var arg in args)
            {
                switch (state)
                {
                    case State.Name:
                        if (!arg.StartsWith(_nameDelim)) throw new Exception("Parsing failed, expecting parameter name.");
                        currName = arg.Substring(_nameDelim.Length);
                        CommandLineArg cliArg;
                        if (!_allowedArguments.TryGetValue(currName, out cliArg)) throw new Exception($"Parsing failed, '{currName}' is not an allowed argument.");
                        if (cliArg.ExpectsValue) state = State.Value;
                        else _args.Add(currName, string.Empty);
                        break;

                    case State.Value:
                        _args.Add(currName, arg);
                        state = State.Name;
                        break;
                }
            }

            if (state == State.Value)
            {
                throw new Exception("Parsing failed, expected value missing.");
            }
        }
    }
}
