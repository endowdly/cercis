namespace Cercis
{ 
    static class Messages
    {
        public class CommandLine
        {
            public const string Help = @"
Usage:
    cercis [directory] [options]

ARGUMENTS:
    directory     Directory to traverse. Defaults to current working directory.

OPTIONS:
    -d, --depth            Unsigned integer indicating many nested directory levels to display. Defaults to all.
    -p, --ignore-patterns  Comma-separated list of prefixes. Directories containing any of
                           these prefixes will not be traversed. Their memory size will also be ignored.
    -s, --sort             Sort tree by memory-size. 
    -S, --sort-descending  Sort tree by reverse memory-size.
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


}