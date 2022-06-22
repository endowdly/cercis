namespace Cercis
{ 
    using System;
    using System.IO;
    using System.Linq;

    class Cercis
    { 
        static void Main(string[] args)
        { 
            if (args.Contains("-h") || args.Contains("--help"))
            {
                Console.WriteLine(Messages.CommandLine.Help);
                return;
            }

            if (args.Contains("--version"))
            {
                throw new NotImplementedException(); 
            } 

            var clargs = CommandLine.ParseArgs(args); 

            new FileTree(
                clargs.Directory ?? Directory.GetCurrentDirectory(),
                clargs.Prefixes,
                clargs.SortType,
                clargs.Depth)
                .Display(); 
        } 
    }
}