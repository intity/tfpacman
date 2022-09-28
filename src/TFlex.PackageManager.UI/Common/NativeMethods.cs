using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

#pragma warning disable CA1049
#pragma warning disable CA1400

namespace TFlex.PackageManager.UI.Common
{
    internal static class NativeMethods
	{
        #region window messages
        public const int WM_USER        = 0x0400;
        public const int WM_SETFONT     = 0x0030;
        public const int WM_GETFONT     = 0x0031;
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_KILLFOCUS   = 0x0008;
        public const int WM_SETICON     = 0x0080;
        #endregion

        public const int GW_CHILD = 5;
        public const int MAX_PATH = 260;

        #region Shell Members
        /// <summary>
        /// Contains parameters for the SHBrowseForFolder function and 
        /// receives information about the folder selected by the user.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct BROWSEINFO
        {
            public IntPtr hwndOwner;
            public IntPtr pidlRoot;
            public string pszDisplayName;
            public string lpszTitle;
            public uint   ulFlags;
            public BrowseCallbackProc lpfn;
            public IntPtr lParam;
            public int    iImage;
        }

        public const uint 
        BIF_RETURNONLYFSDIRS    = 0x00000001,
        BIF_DONTGOBELOWDOMAIN   = 0x00000002,
        BIF_STATUSTEXT          = 0x00000004,
        BIF_RETURNFSANCESTORS   = 0x00000008,
		BIF_EDITBOX             = 0x00000010,
		BIF_VALIDATE            = 0x00000020,
		BIF_NEWDIALOGSTYLE      = 0x00000040,
        BIF_BROWSEINCLUDEURLS   = 0x00000080,
        BIF_USENEWUI            = (BIF_EDITBOX | BIF_NEWDIALOGSTYLE),
        BIF_UAHINT              = 0x00000100,
		BIF_NONEWFOLDERBUTTON   = 0x00000200,
		BIF_NOTRANSLATETARGETS  = 0x00000400,
		BIF_BROWSEFORCOMPUTER   = 0x00001000,
		BIF_BROWSEFORPRINTER    = 0x00002000,
		BIF_BROWSEINCLUDEFILES  = 0x00004000,
		BIF_SHAREABLE           = 0x00008000,
        BIF_BROWSEFILEJUNCTIONS = 0x00010000;

        /// <summary>
        /// Callback function pointer.
        /// </summary>
        /// <param name="hwnd">
        /// The window handle of the browse dialog box.
        /// </param>
        /// <param name="uMsg">
        /// The dialog box event that generated the message. 
        /// One of the following values.
        /// </param>
        /// <param name="lParam"></param>
        /// <param name="lpData"></param>
        /// <returns></returns>
        public delegate int BrowseCallbackProc(
            IntPtr hwnd,
            uint   uMsg,
            IntPtr lParam,
            IntPtr lpData);

        // message from browser
        public const uint 
        BFFM_INITIALIZED     = 1,
        BFFM_SELCHANGED      = 2,
        BFFM_VALIDATEFAILEDA = 3,
        BFFM_VALIDATEFAILEDW = 4,
        BFFM_IUNKNOWN        = 5;

        // messages to browser
        public const int 
        BFFM_SETSTATUSTEXTA  = (WM_USER + 100),
        BFFM_ENABLEOK        = (WM_USER + 101),
        BFFM_SETSELECTIONA   = (WM_USER + 102),
        BFFM_SETSELECTIONW   = (WM_USER + 103),
        BFFM_SETSTATUSTEXTW  = (WM_USER + 104),
        BFFM_SETOKTEXT       = (WM_USER + 105), // Unicode only
        BFFM_SETEXPANDED     = (WM_USER + 106); // Unicode only

        /// <summary>
        /// Displays a dialog box that enables the user to select a Shell folder.
        /// </summary>
        /// <param name="lpbi"></param>
        /// <returns></returns>
        [DllImport("shell32.dll")]
		public static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lpbi);

        /// <summary>
        /// Converts an item identifier list to a file system path.
        /// </summary>
        /// <param name="pidl">
        /// The address of an item identifier list that specifies a file or 
        /// directory location relative to the root of the namespace (the desktop).
        /// </param>
        /// <param name="pszPath">
        /// The address of a buffer to receive the file system path. 
        /// This buffer must be at least MAX_PATH characters in size.
        /// </param>
        /// <returns>Returns TRUE if successful; otherwise, FALSE.</returns>
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
		public static extern bool SHGetPathFromIDList(
            IntPtr pidl, 
            [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszPath);

		[DllImport("shell32.dll", SetLastError = true)]
		public static extern int SHGetSpecialFolderLocation(
            IntPtr hwndOwner, 
            int nFolder, 
            ref IntPtr ppidl);

        [Flags]
        public enum FILE_ATTRIBUTE : uint
        {
            READONLY              = 0x000001,
            HIDDEN                = 0x000002,
            SYSTEM                = 0x000004,
            DIRECTORY             = 0x000010,
            ARCHIVE               = 0x000020,
            DEVICE                = 0x000040,
            NORMAL                = 0x000080,
            TEMPORARY             = 0x000100,
            SPARSE_FILE           = 0x000200,
            REPARSE_POINT         = 0x000400,
            COMPRESSED            = 0x000800,
            OFFLINE               = 0x001000,
            NOT_CONTENT_INDEXED   = 0x002000,
            ENCRYPTED             = 0x004000,
            INTEGRITY_STREAM      = 0x008000,
            VIRTUAL               = 0x010000,
            NO_SCRUB_DATA         = 0x020000,
            RECALL_ON_OPEN        = 0x040000,
            PINNED                = 0x080000,
            UNPINNED              = 0x100000,
            RECALL_ON_DATA_ACCESS = 0x400000
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [Flags]
        public enum SHGFI
        {
            LARGEICON         = 0x000000000,
            SMALLICON         = 0x000000001,
            OPENICON          = 0x000000002,
            SHELLICONSIZE     = 0x000000004,
            PIDL              = 0x000000008,
            USEFILEATTRIBUTES = 0x000000010,
            ADDOVERLAYS       = 0x000000020,
            OVERLAYINDEX      = 0x000000040,
            ICON              = 0x000000100,
            DISPLAYNAME       = 0x000000200,
            TYPENAME          = 0x000000400,
            ATTRIBUTES        = 0x000000800,
            ICONLOCATION      = 0x000001000,
            EXETYPE           = 0x000002000,
            SYSICONINDEX      = 0x000004000,
            LINKOVERLAY       = 0x000008000,
            SELECTED          = 0x000010000,
            ATTR_SPECIFIED    = 0x000020000
        };

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            out SHFILEINFO psfi,
            uint cbFileInfo,
            SHGFI uFlags);

        public static Icon GetIcon(string path, bool small)
        {
            Icon icon = null;
            SHFILEINFO info = new SHFILEINFO();
            uint cbFileInfo = (uint)Marshal.SizeOf(info);
            uint dwFileAttr = (uint)FILE_ATTRIBUTE.TEMPORARY;
            SHGFI flags;

            if (small)
            {
                flags = SHGFI.ICON | SHGFI.SMALLICON | SHGFI.USEFILEATTRIBUTES;
            }
            else
            {
                flags = SHGFI.ICON | SHGFI.LARGEICON | SHGFI.USEFILEATTRIBUTES;
            }

            var result = SHGetFileInfo(path, dwFileAttr, out info, cbFileInfo, flags);
            if (result != IntPtr.Zero)
            {
                icon = Icon.FromHandle(info.hIcon);
                DeleteObject(info.hIcon);
            }
            
            return icon;
        }
        #endregion

        [DllImport("user32.dll", EntryPoint = "CallWindowProc", CharSet = CharSet.Auto)]
        public static extern IntPtr CallWindowProc(
            IntPtr lpPrevWndFunc, 
            IntPtr hWnd, 
            uint   uMsg, 
            IntPtr wParam, 
            IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(
            IntPtr hWnd, 
            int msg, 
            IntPtr wParam, 
            IntPtr lParam);

        public const int 
        GWLP_WNDPROC   = (-4),
        GWLP_HINSTANCE = (-6),
        GWLP_ID        = (-12),
        GWLP_USERDATA  = (-21);

        /// <summary>
        /// Callback function pointer.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="uMsg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate IntPtr WndProc(
            IntPtr hWnd, 
            uint   uMsg, 
            IntPtr wParam, 
            IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool SetWindowText(IntPtr hwnd, string lpString);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// MoveWindow function.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="X">The new position of the left side of the window.</param>
        /// <param name="Y">The new position of the top of the window.</param>
        /// <param name="nWidth">The new width of the window.</param>
        /// <param name="nHeight">The new height of the window.</param>
        /// <param name="bRepaint">Indicates whether the window is to be repainted.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
		public static extern bool MoveWindow(
            IntPtr hWnd, 
            int    X, 
            int    Y, 
            int    nWidth, 
            int    nHeight, 
            bool   bRepaint);

		public static void MoveWindow(IntPtr hWnd, RECT rect, bool bRepaint)
		{
			MoveWindow(hWnd, rect.left, rect.top, rect.Width, rect.Height, bRepaint);
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
            public int left;
            public int top;
            public int right;
            public int bottom;

			public RECT(int left, int top, int width, int height)
			{
				this.left   = left;
				this.top    = top;
                this.right  = left + width;
                this.bottom = top + height;
            }

			public int Height
            {
                get { return bottom - top; }
            }

			public int Width
            {
                get { return right - left; }
            }
		}

        [DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y)
			{
				X = x;
				Y = y;
			}

			public static implicit operator Point(POINT p)
			{
				return new Point(p.X, p.Y);
			}

			public static implicit operator POINT(Point p)
			{
				return new POINT(p.X, p.Y);
			}
		}

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);
    }
}