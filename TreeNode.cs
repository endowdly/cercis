namespace Cercis
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    enum FileType 
    {
        None,
        File,
        Dir, 
        SymLink
    }

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
        Stack<TreeNode> children;
        public readonly ulong gen;
        public readonly string location;

        public TreeNode(
            string loc,
            IEnumerable<string> ps,
            SortType st,
            ulong n
        )
        { 
            location = loc;
            gen = n;
            sortType = st;
            children = new Stack<TreeNode>();

            if (Directory.Exists(loc))
            {
                try
                {
                    name = new DirectoryInfo(loc).Name;

                    ConstructBranches(ps);
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
                name = new FileInfo(loc).Name;
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
        public string Name 
        {
            get
            {
                return Directory.Exists(location)
                    ? string.Format(Formatters.TreeNode.FormatFileName, name)
                    : name;
            }
        }

        public IEnumerable<TreeNode> EnumerateChildren()
        {
            foreach (var c in children)
            {
                yield return c;
            }
        }

        // sprintf_len
        static FileType GetFileType(string entry)
        {
            if (IsSymLink(entry))
                return FileType.SymLink;

            return File.Exists(entry)
                ? FileType.File
                : Directory.Exists(entry)
                    ? FileType.Dir
                    : FileType.None;
        }

        static ulong GetFileLength(string path)
        {
            var fi = new FileInfo(path);

            return fi.Exists
                ? (ulong) fi.Length
                : 0ul;
        }

        static bool IsSymLink(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
        }

        void AddChild(TreeNode child)
        { 
            children.Push(child); 
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

                fType = GetFileType(fse);

                if (fType == FileType.None)
                    continue;

                fName = fType == FileType.Dir
                    ? new DirectoryInfo(fse).Name
                    : new FileInfo(fse).Name;

                foreach (var p in ps)
                {
                    if (fType == FileType.Dir && fName.StartsWith(p))
                        goto skip;
                } 

                var newNode = new TreeNode(fse, ps, sortType, gen + 1);

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
            var xs = sortType == SortType.Ascending
                ? children.OrderBy(child => child.len)
                : children.OrderByDescending(child => child.len);

            children = new Stack<TreeNode>(xs);
        }
    }
}