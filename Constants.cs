namespace Cercis
{ 
    class Messages
    {
        public class CommandLine
        {
            public const string Help = @"
Usage:
    cercis [directory] [options]

ARGUMENTS:
    directory     Directory to traverse. Defaults to current working directory.

OPTIONS:
    -l            Unsigned integer indicating many nested directory levels to display. Defaults to all.
    -p            Comma-separated list of prefixes. Directories containing any of
                  these prefixes will not be traversed. Their memory size will also be ignored.
    -s [asc|desc] Sort tree by memory-size. 
    -h            Displays help prompt.
";            
        }
        public class FileTree
        {
            public const string NotADir = "The root directory must be a path to a directory! Got {0}";
        }
        public class TreeNode
        {
            public const string UnknownIOException = "IO Exception Occurred! Source: {0}";
        }

    }

    class Literal
    {
        public const char Space = ' ';
        public const string BranchSep = "  ";
        public const string Period = ".";
        public const char EscapeCharacter = (char)0x1b;
        public const char LeftBracketCharacter = (char)0x5b; 

        public const char VerticalAndRight = (char)0x251c;
        public const char UpAndRight = (char)0x2514;
        public const char Horizontal = (char)0x2500;
        public const char Vertical = (char)0x2502;
    }

    class AnsiCommand
    {
        public static readonly string Start =
            Literal.EscapeCharacter.ToString()
                + Literal.LeftBracketCharacter.ToString();
        public static readonly string Stop =
            Literal.EscapeCharacter.ToString()
                + Literal.LeftBracketCharacter.ToString()
                + "0m";

        public const string Red = "1;31m";
        public const string Green = "1;32m";
        public const string Yellow = "1;33m";
        public const string DarkCyan = "36m";
    }
}