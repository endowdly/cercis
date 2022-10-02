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
    public readonly TreeNode[] Children; 
    public readonly ulong Gen;
    public readonly string Location;

    public TreeNode(string loc, IEnumerable<string> ps, SortType st, ulong n)
    {
        sortType = st;
        Location = loc;
        Gen = n;

        if (Directory.Exists(loc))
        {
            try
            {
                name = new DirectoryInfo(loc).Name; 
                Children = ConstructBranches(ps);
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
    public bool IsSymLink => File.GetAttributes(Location).HasFlag(FileAttributes.ReparsePoint);
    public string Name =>
        IsSymLink
            ? string.Format(Formatters.TreeNode.FormatLinkName, name, GetLinkTarget(Location))
            : Directory.Exists(Location)
                ? string.Format(Formatters.TreeNode.FormatDirName, name)
                : name;

    TreeNode[] ConstructBranches(IEnumerable<string> ps)
    {
        // Do not try to traverse Junctions or SymLinks
        if (IsSymLink)
        {
            len = 0ul;
            
            return Array.Empty<TreeNode>();
        }

        var xs = Directory.EnumerateFileSystemEntries(Location);
        var ys = new Stack<TreeNode>(xs.Count());

        foreach (var x in xs)
        {
            if (string.IsNullOrWhiteSpace(x))
                continue;

            foreach (var p in ps)
            {
                if (Directory.Exists(x) && new DirectoryInfo(x).Name.StartsWith(p))
                    goto skip;
            }

            var y = new TreeNode(x, ps, sortType, Gen + 1);

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

    static string GetLinkTarget(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                return File.ResolveLinkTarget(path, true).FullName ?? string.Empty;
            }
            
            return Directory.Exists(path)
                ? Directory.ResolveLinkTarget(path, true).FullName ?? string.Empty
                : string.Empty; 
        }
        catch (IOException)
        {
            return string.Empty;
        }
    }
}
