namespace Cercis;

static class Messages
{
    public class CommandLine
    {
        public const string Help =
            @"
Usage:
    cercis [directory] [options]

Directory:
    Directory to traverse. Defaults to current working directory.

Options:
    -d, --depth            Unsigned integer indicating how any nested directory levels to display. Defaults to all.
    -p, --ignore-patterns  Comma-separated list of prefixes. Directories containing any of
                           these prefixes will not be traversed. Their memory size will be ignored.
    -P, --no-ignore        Do not ignore any directories.
    -s, --sort             Sort tree by memory-size ascending.
    -S, --sort-descending  Sort tree by memory-size descending.
    -h, --help             Displays help prompt.
";
    }

    public static class FileTree
    {
        public const string NotADir = "The root directory must be a path to a directory! Got {0}";
    }

    public static class TreeNode
    {
        public const string UnknownIOException = "IO Exception Occurred! Source: {0}";
        public const string TooManyLinks = "Too many symbolic links! Cannot find target";
    }
}

static class Literal
{
    public const char Space = ' ';
    public const char Comma = ',';
    public const string BranchSep = "  ";
    public const string Period = ".";
    public const char EscapeCharacter = (char)0x1b;
    public const char LeftBracketCharacter = (char)0x5b;

    public const char VerticalAndRight = (char)0x251c;
    public const char UpAndRight = (char)0x2514;
    public const char Horizontal = (char)0x2500;
    public const char Vertical = (char)0x2502;
}

static class Formatters
{
    static class AnsiCommand
    {
        public static readonly string Start =
            Literal.EscapeCharacter.ToString() + Literal.LeftBracketCharacter.ToString();
        public static readonly string Stop =
            Literal.EscapeCharacter.ToString() + Literal.LeftBracketCharacter.ToString() + "0m";

        public const string BoldRed = "1;31m";
        public const string DarkCyan = "36m";
        public const string DarkMagenta = "35m";
    }

    public static class FileTree
    {
        public const string SprintRow = "{0}{1} ({2})\n";

        public static readonly string SprintBranchesPrefix =
            "{0}"
            + AnsiCommand.Start
            + AnsiCommand.DarkCyan
            + Literal.VerticalAndRight
            + Literal.Horizontal
            + AnsiCommand.Stop
            + "{1}";

        public static readonly string SprintBranchesDirPrefix =
            "{0}"
            + AnsiCommand.Start
            + AnsiCommand.DarkCyan
            + Literal.Vertical
            + AnsiCommand.Stop
            + "{1}";

        public static readonly string SprintBranchesLastChildPrefix =
            "{0}"
            + AnsiCommand.Start
            + AnsiCommand.DarkCyan
            + Literal.UpAndRight
            + Literal.Horizontal
            + AnsiCommand.Stop
            + "{1}";
        public const string SprintBranchesLastChildDirPrefix = "{0}  {1}";
    }

    public static class TreeNode
    {
        public static readonly string FormatDirName =
            AnsiCommand.Start + AnsiCommand.BoldRed + "{0}" + AnsiCommand.Stop;

        public static readonly string FormatLength =
            AnsiCommand.Start + AnsiCommand.BoldRed + "{0}" + AnsiCommand.Stop;

        public static readonly string FormatLinkName =
            AnsiCommand.Start + AnsiCommand.DarkMagenta + "{0}" + AnsiCommand.Stop + " -> {1}";
    }
}