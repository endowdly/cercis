using System.Text;

namespace Cercis;

class FileTree
{
    readonly ulong depth;
    readonly TreeNode rootNode;

    public FileTree(string rootPath, string ignoredPrefixes, SortType sortType, ulong maxDepth)
    {
        if (!Directory.Exists(rootPath))
            throw new DirectoryNotFoundException(
                string.Format(Messages.FileTree.NotADir, rootPath)
            );

        var ps = string.IsNullOrWhiteSpace(ignoredPrefixes)
            ? Array.Empty<string>()
            : ignoredPrefixes.Split(Literal.Comma).Select(x => x.Trim());

        rootNode = new TreeNode(rootPath, ps, sortType, 0);

        depth = maxDepth;
    }

    static void SprintRow(TreeNode node, string prefix, ref StringBuilder sb)
    {
        var fmtRow = string.Format(Formatters.FileTree.SprintRow, prefix, node.Name, node.Length);

        if (fmtRow.Length + sb.Length > sb.MaxCapacity)
            return;

        sb.Append(fmtRow);
    }

    static void SprintBranches(TreeNode node, string basePrefix, ulong depth, ref StringBuilder sb)
    {
        string prefix;
        string dirPrefix;
        TreeNode[] children;
        TreeNode lastChild;

        if (node.Gen >= depth)
            return;

        if (node.Children.Length < 1)
            return;

        if (node.Children.Length > 1)
        {
            children = node.Children[0..^2];
            lastChild = node.Children[^1];

            foreach (var child in children)
            {
                prefix = string.Format(
                    Formatters.FileTree.SprintBranchesPrefix,
                    basePrefix,
                    Literal.BranchSep
                );

                SprintRow(child, prefix, ref sb);

                if (Directory.Exists(child.Location))
                {
                    dirPrefix = string.Format(
                        Formatters.FileTree.SprintBranchesDirPrefix,
                        basePrefix,
                        Literal.BranchSep
                    );

                    SprintBranches(child, dirPrefix, depth, ref sb);
                }
            }
        }
        else
        {
            lastChild = node.Children[0];
        }

        prefix = string.Format(
            Formatters.FileTree.SprintBranchesLastChildPrefix,
            basePrefix,
            Literal.BranchSep
        );

        SprintRow(lastChild, prefix, ref sb);

        if (Directory.Exists(lastChild.Location))
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
