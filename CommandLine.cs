namespace Cercis
{

    enum CommandLineOption
    {
        None,
        Depth,
        Patterns,
        Sort,
    }

    enum CommandLineArgType
    {
        Positional,
        Optional
    }


    struct CommandLineArgs
    {
        public string? Directory;
        public ulong? Depth;
        public IEnumerable<string>? Prefixes;
        public SortType? SortType;
    }

    static class CommandLine
    {
        public static CommandLineArgs ParseArgs(IEnumerable<string> args)
        {
            var cliArgs = new CommandLineArgs();

            if (!args.Any())
                return cliArgs;

            // Move help to Main

            var state = CommandLineOption.None;
            var first = args.First();
            var rest = args.Skip(1); 
            var firstArgType = GetArgType(first);

            if (firstArgType == CommandLineArgType.Positional)
            {
                var dir = GetDirectoryFromArg(first);
                cliArgs.Directory = dir;
            }
            else 
            {
                var opt = GetOption(first);
                
                if (opt == CommandLineOption.None)
                    throw BadOption(first);
            }

            void DoNone(string s)
            {
                var opt = GetOption(s);

                if (opt == CommandLineOption.None)
                    throw BadOption(s);
                else
                    state = opt;
            }

            void DoDepth(string s)
            {
                ValidateArg(s);

                cliArgs.Depth = GetDepthFromArg(s);
                state = CommandLineOption.None;
            }

            void DoPatterns(string s)
            {
                ValidateArg(s);

                cliArgs.Prefixes = s.Split(",", StringSplitOptions.TrimEntries);
                state = CommandLineOption.None;
            }

            void DoSort(string s)
            {
                ValidateArg(s);

                if (s != "asc" || s != "desc")
                    throw new ArgumentException("-s must be <asc|desc>.");

                cliArgs.SortType = GetSortTypeFromArg(s);
                state = CommandLineOption.None;
            }

            var stateMap = new Dictionary<CommandLineOption, Action<string>>()
            {
                { CommandLineOption.None, DoNone },
                { CommandLineOption.Depth, DoDepth },
                { CommandLineOption.Patterns, DoPatterns },
                { CommandLineOption.Sort, DoSort }, 
            }; 

            foreach (var arg in rest)
            {
                var action = stateMap[state];

                action.Invoke(arg); 
            }

            return cliArgs;
        }

        static ulong GetDepthFromArg(string s)
        { 
            if (!ulong.TryParse(s, out ulong result))
                return 0;

            return result;
        }

        static CommandLineOption GetOption(string flag)
        {
            var map = new Dictionary<string, CommandLineOption>()
            {
                { "-l", CommandLineOption.Depth },
                { "-p", CommandLineOption.Patterns },
                { "-s", CommandLineOption.Sort },
                { "", CommandLineOption.None } 
            };
            
            if (map.TryGetValue(flag, out CommandLineOption value))
            {
                return value;
            } 

            throw BadOption(flag); 
        }
        
        static CommandLineArgType GetArgType(string arg)
        {
            if (arg.StartsWith("-"))
                return CommandLineArgType.Optional;

            return CommandLineArgType.Positional;
        }

        static string GetDirectoryFromArg(string arg)
        {
            return Path.GetDirectoryName(arg) ?? throw new DirectoryNotFoundException();
        }
        
        static SortType GetSortTypeFromArg(string arg)
        {
            if (arg == "asc")
                return SortType.Ascending;

            return SortType.Descending;
        } 

        static void ValidateArg(string arg)
        {
            var ss = new string[] { "-s", "-l", "-p" };

            if (ss.Contains(arg))
                throw new ArgumentException("Malformed command line arguments."); 
        }

        static ArgumentException BadOption(string arg)
        {
            return new ArgumentException(string.Format("{0} is not a valid option", arg));
        } 
    } 
}