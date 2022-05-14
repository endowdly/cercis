namespace Cercis
{
    class Formatters
    {
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