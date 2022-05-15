namespace Cercis
{ 
    class Cercis
    { 
        static void Main(string[] args)
        { 
            if (args.Contains("-h"))
            {
                Console.WriteLine(Messages.CommandLine.Help);
                return;
            }

            ulong depth;
            string dir;
            string pre;
            CommandLineArgs cliArgs;
            SortType sort; 
            
            args = args.ToArray(); 
            cliArgs = CommandLine.ParseArgs(args); 
            dir = cliArgs.Directory ?? Directory.GetCurrentDirectory();
            pre = cliArgs.Prefixes ?? string.Empty;
            sort = cliArgs.SortType ?? SortType.None;
            depth = cliArgs.Depth ?? ulong.MaxValue;

            new FileTree(dir, pre, sort, depth).Display(); 
        } 
    }
}