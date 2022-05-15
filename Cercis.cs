namespace Cercis
{ 
    class Cercis
    { 
        static void Main(string[] args)
        { 
            ulong depth;
            string dir;
            CommandLineArgs cliArgs;
            IEnumerable<string> pre;
            SortType sort;
            
            if (args.Length <= 1)
            {
                var ft = new FileTree();

                ft.Display();
                return;
            } 
            else
            {
                args = args.Skip(1).ToArray(); 
                cliArgs = CommandLine.ParseArgs(args); 
                dir = cliArgs.Directory ?? Directory.GetCurrentDirectory();
                pre = cliArgs.Prefixes ?? Array.Empty<string>();
                sort = cliArgs.SortType ?? SortType.None;
                depth = cliArgs.Depth ?? ulong.MaxValue;
            }

            new FileTree(dir, pre, sort, depth).Display(); 
        } 
    }
}