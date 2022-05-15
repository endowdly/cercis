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
            IEnumerable<string> ps,
            SortType st,
            ulong n
        )
        {
            if (!Directory.Exists(dir))
                throw new DirectoryNotFoundException(string.Format(Messages.FileTree.NotADir, dir));

            rootNode = new TreeNode(
                dir,
                FileType.Dir,
                Literal.Period,
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

            string prefix;
            string dirPrefix;

            var iterChildren = node.EnumerateChildren();
            var lastChild = iterChildren.Last();
            var children = iterChildren.SkipLast(1);

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

            if (lastChild != null)
            {
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