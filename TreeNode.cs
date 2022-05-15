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
            ulong n
        )
        {

            location = loc;
            fileType = ft;
            fileName = fn;
            gen = n;
            sortType = st;
            children = Array.Empty<TreeNode>();

            if (IsDir)
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

        public ulong Generation => gen;

        public bool IsDir =>
            (File.GetAttributes(location) & FileAttributes.Directory) == FileAttributes.Directory;

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

        public IEnumerable<TreeNode> EnumerateChildren()
        {
            foreach (var c in children)
            {
                yield return c;
            }
        }

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

            foreach (var fse in fses)
            {
                if (string.IsNullOrWhiteSpace(fse))
                    continue;

                fType = AscertainFileType(fse);

                if (fType == FileType.Unknown)
                    continue;

                fName = fType == FileType.Dir
                    ? new DirectoryInfo(fse).Name
                    : new FileInfo(fse).Name;

                foreach (var p in ps)
                {
                    if (fType == FileType.Dir && fName.StartsWith(p))
                        goto skip;
                } 

                var newNode = new TreeNode(fse, fType, fName, ps, sortType, gen + 1);

                len += newNode.len;

                AddChild(newNode);

                skip:
                    continue;
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