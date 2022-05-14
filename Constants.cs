namespace Cercis
{ 
    class Messages
    {
        public class TreeNode
        {
            public const string UnknownIOException = "IO Exception Occurred! Source: {0}";
        }
    }
    class Literal
    {
        public const char Space = ' ';
        public const char EscapeCharacter = (char)0x1b;
        public const char LeftBracketCharacter = (char)0x5b;

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
    }
}