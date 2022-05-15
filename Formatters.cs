namespace Cercis
{
    class Formatters
    {
        public class FileTree
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

        public class TreeNode
        {
            public static readonly string FormatFileName =
                AnsiCommand.Start
                    + AnsiCommand.Red
                    + "{0}"
                    + AnsiCommand.Stop;

            public static readonly string FormatFileLengthNone =
                AnsiCommand.Start
                    + AnsiCommand.Red
                    + "{0}"
                    + AnsiCommand.Stop
                    + Literal.Space
                    + AnsiCommand.Start
                    + AnsiCommand.Red
                    + "{1}"
                    + AnsiCommand.Stop;

            public static readonly string FormatFileLength =
                AnsiCommand.Start
                    + AnsiCommand.Red
                    + "{0:n2}"
                    + AnsiCommand.Stop
                    + Literal.Space
                    + AnsiCommand.Start
                    + AnsiCommand.Red
                    + "{1}"
                    + AnsiCommand.Stop;

        }
    }
}