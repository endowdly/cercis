namespace Cercis
{
    class Formatters
    {
        class AnsiCommand
        {
            public static readonly string Start =
                Literal.EscapeCharacter.ToString()
                    + Literal.LeftBracketCharacter.ToString();
            public static readonly string Stop =
                Literal.EscapeCharacter.ToString()
                    + Literal.LeftBracketCharacter.ToString()
                    + "0m";

            public const string BoldRed = "1;31m";
            public const string DarkCyan = "36m";
            public const string DarkMagenta = "35m";
        }        

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
            public static readonly string FormatDirName = 
                AnsiCommand.Start + AnsiCommand.BoldRed + "{0}" + AnsiCommand.Stop;

            public static readonly string FormatLength = 
                AnsiCommand.Start + AnsiCommand.BoldRed + "{0}" + AnsiCommand.Stop;

            public static readonly string FormatLinkName = 
                AnsiCommand.Start + AnsiCommand.DarkMagenta + "{0}" + AnsiCommand.Stop + " -> {1}";
        }
    }
}