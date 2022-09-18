namespace Cercis
{
    using System;
    using System.ComponentModel;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Linq;
    using System.Text;

    enum SortType
    {
        None,
        Ascending,
        Descending
    }

    static class NativeMethods
    {
        static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        const uint FILE_READ_EA = 0x0008;
        const uint FILE_FLAG_BACKUP_SEMANTICS = 0x2000000;

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint GetFinalPathNameByHandle(
            IntPtr hFile,
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszFilePath,
            uint cchFilePath,
            uint dwFlags
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr CreateFile(
            [MarshalAs(UnmanagedType.LPWStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] uint access,
            [MarshalAs(UnmanagedType.U4)] FileShare share,
            IntPtr securityAttr,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] uint flags,
            IntPtr templateFile
        );

        public static string GetFinalPathName(string path)
        {
            var h = CreateFile(
                path,
                FILE_READ_EA,
                FileShare.ReadWrite | FileShare.Delete,
                IntPtr.Zero,
                FileMode.Open,
                FILE_FLAG_BACKUP_SEMANTICS,
                IntPtr.Zero
            );

            if (h == INVALID_HANDLE_VALUE)
                throw new Win32Exception();

            try
            {
                var sb = new StringBuilder(1024);
                var res = GetFinalPathNameByHandle(h, sb, 1024, 0);

                if (res == 0)
                    throw new Win32Exception();

                return sb.ToString();
            }
            finally
            {
                CloseHandle(h);
            }
        }
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
            Location = loc;
            Gen = n;
            sortType = st;

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
                    Console.WriteLine(
                        Messages.TreeNode.UnknownIOException,
                        ex.Source ?? string.Empty
                    );
                }
            }
            else
            {
                name = Path.GetFileName(loc);
                len = GetFileLength(loc);
            }
        }

        public string Length
        {
            get { return string.Format(Formatters.TreeNode.FormatLength, Bytes.Prettify(len)); }
        }

        // sprintf_file_name
        public string Name
        {
            get
            {
                return IsSymLink(Location)
                    ? string.Format(
                        Formatters.TreeNode.FormatLinkName,
                        name,
                        GetLinkTarget(Location)
                    )
                    : Directory.Exists(Location)
                        ? string.Format(Formatters.TreeNode.FormatDirName, name)
                        : name;
            }
        }

        TreeNode[] ConstructBranches(IEnumerable<string> ps)
        {
            var xs = Directory.EnumerateFileSystemEntries(Location);
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

        static bool IsSymLink(string path)
        {
            return (File.GetAttributes(path) & FileAttributes.ReparsePoint)
                == FileAttributes.ReparsePoint;
        }

        static string GetLinkTarget(string path)
        {
            try
            {
                return NativeMethods.GetFinalPathName(path) ?? string.Empty;
            }
            catch (IOException)
            {
                return string.Empty;
            }
        }
    }
}
