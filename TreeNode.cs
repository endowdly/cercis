namespace Cercis
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    enum FileType
    {
        Unknown,
        File,
        Dir,
        Symlink
    }

    // Note: would like to keep this as a struct for speed
    // But it is self-referencing, even in the rust impl. 
    class TreeNode
    {
        ulong len;
        readonly ulong gen;
        readonly string location;
        readonly string fileName;
        readonly FileType fileType;
        readonly SortType sortType;
        TreeNode[] children;

        public TreeNode(
            string loc,
            FileType ft,
            string fn,
            IEnumerable<string> ps,
            SortType st,
            ulong n)
        {

            location = loc;
            fileType = ft;
            fileName = fn;
            gen = n;
            sortType = st;
            children = Array.Empty<TreeNode>();

            if (IsDir(loc))
            {
                try
                {
                    ConstructBranches(ps);
                }
                catch (UnauthorizedAccessException)
                {
                    return;
                }
                catch (IOException e)
                {
                    Console.WriteLine(Messages.TreeNode.UnknownIOException, e.Source ?? string.Empty);
                }
                catch (ArgumentException e)
                {
                    throw e;
                }
            }
            else
            {
                len = IsSymLink(loc)
                    ? 0ul
                    : GetFileLength(loc);
            }
        }

        public string Length
        {
            get
            {
                var unit = Bytes.PrettyUnit(len);
                var presentLen = Bytes.Convert(len, UnitPrefix.None, unit);

                return unit == UnitPrefix.None
                    ? string.Format(Formatters.TreeNode.FormatFileLengthNone, presentLen, unit.GetDescription())
                    : string.Format(Formatters.TreeNode.FormatFileLength, presentLen, unit.GetDescription());
            }
        }

        // sprintf_file_name
        public string Name =>
            fileType == FileType.Dir
                    ? string.Format(Formatters.TreeNode.FormatFileName, fileName)
                    : fileName;

        // sprintf_len
        static FileType AscertainFileType(string entry)
        {
            if (IsSymLink(entry))
                return FileType.Symlink;

            return File.Exists(entry)
                ? FileType.File
                : Directory.Exists(entry)
                    ? FileType.Dir
                    : FileType.Unknown;
        }

        static ulong GetFileLength(string path)
        {
            var fi = new FileInfo(path);

            return fi.Exists
                ? (ulong)fi.Length
                : 0ul;
        }

        static bool IsDir(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;
        }

        static bool IsSymLink(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
        }

        void AddChild(TreeNode child)
        {
            var xs = new Stack<TreeNode>(children);

            xs.Push(child);

            children = xs.ToArray();
        }

        void ConstructBranches(IEnumerable<string> ps)
        {
            string fName;
            FileType fType;

            var fses = Directory.EnumerateFileSystemEntries(location);

            entries:
            foreach (var fse in fses)
            {
                if (string.IsNullOrWhiteSpace(fse))
                    continue;

                fType = AscertainFileType(fse);

                if (fType == FileType.Unknown)
                    continue;

                foreach (var p in ps)
                {
                    if (string.IsNullOrWhiteSpace(p))
                        continue;

                    if (fType == FileType.Dir)
                    {
                        if (Path.GetDirectoryName(fse)!.StartsWith(p))
                            goto entries;
                    }
                }

                fName = fType == FileType.Dir
                    ? Path.GetDirectoryName(fse)!
                    : Path.GetFileName(fse)!;

                var newNode = new TreeNode(fse, fType, fName, ps, sortType, gen + 1);

                len += newNode.len;

                AddChild(newNode);
            }

            if (sortType != SortType.None)
                SortChildren();
        }

        void SortChildren()
        {
            var xs = new List<TreeNode>(children);
            var ys = sortType == SortType.Ascending
                ? xs.OrderBy(child => child.len).ToList()
                : xs.OrderByDescending(child => child.len).ToList();

            children = ys.ToArray();
        }
    }
}