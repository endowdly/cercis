namespace Cercis
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

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
        public string Directory;
        public ulong Depth;
        public string Prefixes;
        public SortType SortType;
    }

    static class CommandLine
    {
        public static CommandLineArgs ParseArgs(IEnumerable<string> args)
        {
            var clargs = new CommandLineArgs
            {
                Directory = null,
                Depth = ulong.MaxValue,
                Prefixes = string.Empty,
                SortType = SortType.None
            };

            if (!args.Any())
                return clargs;

            var state = CommandLineOption.None;
            var first = args.First();
            var rest = args.Skip(1); 
            var firstArgType = GetArgType(first);

            if (firstArgType == CommandLineArgType.Positional)
            {
                var dir = GetDirectoryFromArg(first);
                clargs.Directory = dir;
            }
            else 
            {
                var _ = GetOption(first);
            }

            Action<string> none = s => 
            {
                state = GetOption(s);
            };
            Action<string> depth = s =>
            {
                ValidateArg(s);
                clargs.Depth = GetDepthFromArg(s);
                state = CommandLineOption.None;
            };
            Action<string> patterns = s =>
            {
                ValidateArg(s); 
                clargs.Prefixes = s;
                state = CommandLineOption.None;
            };
            Action<string> sort = s =>
            {
                SortType st;

                ValidateArg(s); 
                var map = new Dictionary<string, SortType>()
                {
                    { "asc", SortType.Ascending },
                    { "desc", SortType.Descending }, 
                }; 

                if (!map.TryGetValue(s, out st)) 
                { 
                    throw new ArgumentException("SortType must be 'asc' or 'desc'.");
                } 

                clargs.SortType = st;
                state = CommandLineOption.None;
            };

            var stateMap = new Dictionary<CommandLineOption, Action<string>>()
            {
                { CommandLineOption.None, none},
                { CommandLineOption.Depth, depth },
                { CommandLineOption.Patterns, patterns },
                { CommandLineOption.Sort, sort }, 
            }; 

            foreach (var arg in rest)
            {
                stateMap[state].Invoke(arg); 
            }

            return clargs;
        }

        static ulong GetDepthFromArg(string s)
        { 
            ulong result;

            if (ulong.TryParse(s, out result))
                return result;

            return 0;
        }

        static CommandLineOption GetOption(string flag)
        {
            CommandLineOption value; 

            var map = new Dictionary<string, CommandLineOption>()
            {
                { "-l", CommandLineOption.Depth },
                { "-p", CommandLineOption.Patterns },
                { "-s", CommandLineOption.Sort }, 
                { "--depth", CommandLineOption.Depth },
                { "--ignore-patterns", CommandLineOption.Patterns },
                { "--sort", CommandLineOption.Sort },
            }; 
            
            if (map.TryGetValue(flag, out value))
            {
                return value;
            } 

            throw BadOption(flag); 
        }
        
        static CommandLineArgType GetArgType(string arg)
        {
            return arg.StartsWith("-") || arg.StartsWith("--")
                ? CommandLineArgType.Optional
                : CommandLineArgType.Positional;
        }

        static string GetDirectoryFromArg(string arg)
        {
            if (string.IsNullOrEmpty(Path.GetDirectoryName(arg)))
                throw new DirectoryNotFoundException();

            return Path.GetDirectoryName(arg);
        }

        static void ValidateArg(string arg)
        {
            string[] strings = { "-s", "-l", "-p", "--depth", "--ignore-patterns", "--sort" };

            if (strings.Contains(arg))
                throw new ArgumentException("Malformed command line arguments."); 
        }

        static ArgumentException BadOption(string arg)
        {
            return new ArgumentException(string.Format("{0} is not a valid option", arg));
        } 
    } 
}