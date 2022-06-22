namespace Cercis
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    class FileTree
    {
        readonly ulong depth;
        readonly TreeNode rootNode;

        public FileTree(
            string rootPath,
            string ignoredPrefixes,
            SortType sortType,
            ulong maxDepth
        )
        {
            if (!Directory.Exists(rootPath))
                throw new DirectoryNotFoundException(string.Format(Messages.FileTree.NotADir, rootPath));

            var ps = string.IsNullOrWhiteSpace(ignoredPrefixes)
                ? new string[0] 
                : ignoredPrefixes.Split(Literal.Comma).Select(x => x.Trim());
 
            rootNode = new TreeNode(
                rootPath,
                ps,
                sortType,
                0); 

            depth = maxDepth;
        }

        static void SprintRow(TreeNode node, string prefix, ref StringBuilder sb)
        {
            var fmtRow = string.Format(Formatters.FileTree.SprintRow, prefix, node.Name, node.Length);

            sb.Append(fmtRow); 
        }

        static void SprintBranches(TreeNode node, string basePrefix, ulong depth, ref StringBuilder sb)
        {
            if (node.gen >= depth)
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
                children = node.EnumerateChildren().TakeWhile(child => child != lastChild);
            }
            else 
            {
                lastChild = node.EnumerateChildren().First();
                children = new TreeNode[0];
            } 

            foreach (var child in children)
            {
                prefix = string.Format(
                    Formatters.FileTree.SprintBranchesPrefix,
                    basePrefix,
                    Literal.BranchSep
                );

                SprintRow(child, prefix, ref sb); 

                if (Directory.Exists(child.location))
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

            if (Directory.Exists(lastChild.location))
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