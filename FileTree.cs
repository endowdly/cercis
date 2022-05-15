namespace Cercis
{
    using System.Text;
    struct FileTree
    {
        readonly ulong depth;
        readonly string root;
        readonly TreeNode rootNode;

        public FileTree(
            string dir,
            string s,
            SortType st,
            ulong n
        )
        {
            if (!Directory.Exists(dir))
                throw new DirectoryNotFoundException(string.Format(Messages.FileTree.NotADir, dir));

            var ps = string.IsNullOrWhiteSpace(s)
                ? Array.Empty<string>()
                : s.Split(",").Select(x => x.Trim());
 
            rootNode = new TreeNode(
                dir,
                FileType.Dir,
                new DirectoryInfo(dir).Name,
                ps,
                st,
                0); 

            root = dir;
            depth = n;
        }

        public string Root => Path.GetFullPath(root);

        static void SprintRow(TreeNode node, string prefix, ref StringBuilder sb)
        {
            var fmtRow = string.Format(Formatters.FileTree.SprintRow, prefix, node.Name, node.Length);

            sb.Append(fmtRow); 
        }
        void SprintBranches(TreeNode node, string basePrefix, ulong depth, ref StringBuilder sb)
        {
            if (node.Generation >= depth)
                return;

            if (!node.EnumerateChildren().Any())
                return;    

            string prefix;
            string dirPrefix;
            TreeNode lastChild;
            IEnumerable<TreeNode> children; 

            if (node.EnumerateChildren().Count() > 1)
            {
                lastChild = node.EnumerateChildren().Last(); 
                children = node.EnumerateChildren().SkipLast(1);
            }
            else 
            {
                lastChild = node.EnumerateChildren().First();
                children = Array.Empty<TreeNode>();
            } 

            foreach (var child in children)
            {
                prefix = string.Format(
                    Formatters.FileTree.SprintBranchesPrefix,
                    basePrefix,
                    Literal.BranchSep
                );

                SprintRow(child, prefix, ref sb); 

                if (child.IsDir)
                { 
                    dirPrefix = string.Format(
                        Formatters.FileTree.SprintBranchesDirPrefix,
                        basePrefix,
                        Literal.BranchSep
                    );

                    SprintBranches(child, dirPrefix, depth, ref sb);
                }
            }

            prefix = string.Format(
                Formatters.FileTree.SprintBranchesLastChildPrefix,
                basePrefix,
                Literal.BranchSep
            );

            SprintRow(lastChild, prefix, ref sb);

            if (lastChild.IsDir)
            {
                dirPrefix = string.Format(
                    Formatters.FileTree.SprintBranchesLastChildDirPrefix,
                    basePrefix,
                    Literal.BranchSep
                );

                SprintBranches(lastChild, dirPrefix, depth, ref sb);
            }
        }

        public void Display()
        {
            var sb = new StringBuilder();

            SprintRow(rootNode, string.Empty, ref sb);
            SprintBranches(rootNode, string.Empty, depth, ref sb); 

            Console.Write(sb.ToString());
        }
    }
}