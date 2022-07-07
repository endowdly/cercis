namespace Cercis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    enum CommandLineMode
    {
        TopLevel,
        Depth,
        Patterns,
    }

    struct CommandLineArgs
    {
        public string Directory;
        public ulong Depth;
        public string Prefixes;
        public SortType SortType;
    }

    struct OptionState
    {
        public CommandLineMode Mode;
        public CommandLineArgs Options;
        public bool CanSetDir;
    }

    static class CommandLine
    {
        static void ParseTopLevel(string arg, ref OptionState state)
        {
            switch (arg)
            {
                case "--depth"
                or "-d":
                    state.Mode = CommandLineMode.Depth;
                    break;

                case "--ignore-patterns"
                or "-p":
                    state.Mode = CommandLineMode.Patterns;
                    break;

                case "--sort"
                or "-s":
                    state.Options.SortType = SortType.Ascending;
                    break;

                case "--sort-descending"
                or "-S":
                    state.Options.SortType = SortType.Descending;
                    break;

                default:
                    if (!state.CanSetDir)
                        throw new ArgumentException(
                            string.Format("{0} is not a valid option", arg)
                        );

                    state.Options.Directory = GetDirectoryFromArg(arg);
                    state.CanSetDir = false;
                    break;
            }
        }

        static void ParseDepth(string arg, ref OptionState state)
        {
            ValidateArg(arg);

            state.Options.Depth = GetDepthFromArg(arg);
            state.Mode = CommandLineMode.TopLevel;
        }

        static void ParsePatterns(string arg, ref OptionState state)
        {
            ValidateArg(arg);

            state.Options.Prefixes = arg;
            state.Mode = CommandLineMode.TopLevel;
        }

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

            var state = new OptionState
            {
                Mode = CommandLineMode.TopLevel,
                Options = clargs,
                CanSetDir = true
            };

            var first = args.First();
            var rest = args.Skip(1);

            ParseTopLevel(first, ref state);

            foreach (var arg in rest)
            {
                switch (state.Mode)
                {
                    case CommandLineMode.TopLevel:
                        ParseTopLevel(arg, ref state);
                        continue;

                    case CommandLineMode.Depth:
                        ParseDepth(arg, ref state);
                        continue;

                    case CommandLineMode.Patterns:
                        ParsePatterns(arg, ref state);
                        continue;
                }
            }

            return state.Options;
        }

        static ulong GetDepthFromArg(string s)
        {
            if (ulong.TryParse(s, out ulong result))
                return result;

            return 0;
        }

        static string GetDirectoryFromArg(string arg)
        {
            if (string.IsNullOrEmpty(Path.GetDirectoryName(arg)))
                throw new DirectoryNotFoundException();

            return Path.GetDirectoryName(arg);
        }

        static void ValidateArg(string arg)
        {
            string[] strings =
            {
                "-s",
                "-S",
                "-d",
                "-p",
                "--depth",
                "--ignore-patterns",
                "--sort",
                "--sort-descending"
            };

            if (strings.Contains(arg))
                throw new ArgumentException("Malformed command line arguments.");
        }
    }
}
