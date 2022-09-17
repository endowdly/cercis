namespace Cercis;

enum SortType
{
    None,
    Ascending,
    Descending
}

class TreeNode
{
    ulong len;
    readonly string name;
    readonly SortType sortType;
    public readonly TreeNode[] children;

    public readonly ulong gen;
    public readonly string location;

    public TreeNode(string loc, IEnumerable<string> ps, SortType st, ulong n)
    {
        location = loc;
        gen = n;
        sortType = st;

        if (Directory.Exists(loc))
        {
            try
            {
                name = new DirectoryInfo(loc).Name;

                children = ConstructBranches(ps);
            }
            catch (UnauthorizedAccessException)
            {
                return;
            }
            catch (IOException ex)
            {
                Console.WriteLine(Messages.TreeNode.UnknownIOException, ex.Source ?? string.Empty);
            }
        }
        else
        {
            name = Path.GetFileName(loc);
            len = GetFileLength(loc);
        }
    }

    public string Length => string.Format(Formatters.TreeNode.FormatLength, Bytes.Prettify(len));
    public string Name =>
        IsSymLink(location)
            ? string.Format(Formatters.TreeNode.FormatLinkName, name, GetLinkTarget(location))
            : Directory.Exists(location)
                ? string.Format(Formatters.TreeNode.FormatDirName, name)
                : name;

    TreeNode[] ConstructBranches(IEnumerable<string> ps)
    {
        var xs = Directory.EnumerateFileSystemEntries(location);
        var ys = new Stack<TreeNode>();

        foreach (var x in xs)
        {
            if (string.IsNullOrWhiteSpace(x))
                continue;

            foreach (var p in ps)
            {
                if (Directory.Exists(x) && new DirectoryInfo(x).Name.StartsWith(p))
                    goto skip;
            }

            var y = new TreeNode(x, ps, sortType, gen + 1);

            len += y.len;

            ys.Push(y);

            skip:
            continue;
        }

        if (sortType == SortType.None)
            return ys.ToArray();

        return sortType == SortType.Ascending
            ? ys.OrderBy(child => child.len).ToArray()
            : ys.OrderByDescending(child => child.len).ToArray();
    }

    static ulong GetFileLength(string path)
    {
        var fi = new FileInfo(path);

        return fi.Exists ? (ulong)fi.Length : 0ul;
    }

    static bool IsSymLink(string path)
    {
        return (File.GetAttributes(path) & FileAttributes.ReparsePoint)
            == FileAttributes.ReparsePoint;
    }

    static string GetLinkTarget(string path)
    {
        try
        {
            return File.Exists(path)
                ? File.ResolveLinkTarget(path, true).FullName ?? string.Empty
                : Directory.Exists(path)
                    ? Directory.ResolveLinkTarget(path, true).FullName ?? string.Empty
                    : string.Empty;
        }
        catch (IOException)
        {
            return string.Empty;
        }
    }
}
