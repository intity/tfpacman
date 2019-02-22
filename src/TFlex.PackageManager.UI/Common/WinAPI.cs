using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace TFlex.PackageManager.Common
{
	public class WinAPI
	{
        #region window messages
        public const uint WM_USER        = 0x0400;
        public const uint WM_SETFONT     = 0x0030;
        public const uint WM_GETFONT     = 0x0031;
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_KILLFOCUS   = 0x0008;
        public const uint WM_SETICON     = 0x0080;
        #endregion

        public const int GW_CHILD = 5;
        public const int MAX_PATH = 260;

        #region BROWSEINFO
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
        #endregion

        #region BFFCALLBACK
        /// <summary>
        /// BFFCALLBACK function pointer
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
        public const uint BFFM_INITIALIZED     = 1;
        public const uint BFFM_SELCHANGED      = 2;
        public const uint BFFM_VALIDATEFAILEDA = 3;
        public const uint BFFM_VALIDATEFAILEDW = 4;
        public const uint BFFM_IUNKNOWN        = 5;

        // messages to browser
        public const uint BFFM_SETSTATUSTEXTA = (WM_USER + 100);
        public const uint BFFM_ENABLEOK       = (WM_USER + 101);
        public const uint BFFM_SETSELECTIONA  = (WM_USER + 102);
        public const uint BFFM_SETSELECTIONW  = (WM_USER + 103);
        public const uint BFFM_SETSTATUSTEXTW = (WM_USER + 104);
        public const uint BFFM_SETOKTEXT      = (WM_USER + 105); // Unicode only
        public const uint BFFM_SETEXPANDED    = (WM_USER + 106); // Unicode only
        #endregion

        /// <summary>
        /// Used by the SHGetSetSettings function to specify which members 
        /// of its SHELLSTATE structure should be set or retrived.
        /// </summary>
        [Flags]
        public enum SSF
        {
            SSF_SHOWALLOBJECTS       = 0x00000001, // The fShowAllObjects member is being requested.
            SSF_SHOWEXTENSIONS       = 0x00000002, // The fShowExtensions member is being requested.
            SSF_SHOWCOMPCOLOR        = 0x00000008, // The fShowCompColor member is being requested.
            SSF_SORTCOLUMNS          = 0x00000010, // The lParamSort and iSortDirection members are being requested.
            SSF_SHOWSYSFILES         = 0x00000020, // The fShowSysFiles member is being requested.
            SSF_DOUBLECLICKINWEBVIEW = 0x00000080, // The fDoubleClickInWebView member is being requested.
            SSF_SHOWATTRIBCOL        = 0x00000100, // The fShowAttribCol member is being requested.
            #region windows vista: not used.
            SSF_DESKTOPHTML          = 0x00000200, // The fDesktopHTML member is being requested.
            SSF_WIN95CLASSIC         = 0x00000400, // The fWin95Classic member is being requested.
            SSF_DONTPRETTYPATH       = 0x00000800, // The fDontPrettyPath member is being requested.
            SSF_MAPNETDRVBUTTON      = 0x00001000, // The fMapNetDrvBtn member is being requested.
            SSF_SHOWINFOTIP          = 0x00002000, // The fShowInfoTip member is being requested.
            SSF_HIDEICONS            = 0x00004000, // The fHideIcons member is being requested.
            SSF_NOCONFIRMRECYCLE     = 0x00008000, // The fNoConfirmRecycle member is being requested.
            SSF_FILTER               = 0x00010000, // The fFilter member is being requested.
            #endregion
            SSF_WEBVIEW              = 0x00020000, // The fWebView member is being requested.
            SSF_SHOWSUPERHIDDEN      = 0x00040000, // The fShowSuperHidden member is being requested.
            SSF_SEPPROCESS           = 0x00080000, // The fSepProcess member is being requested.
            SSF_NONETCRAWLING        = 0x00100000, // Windows XP and later. The fNoNetCrawling member is being requested.
            SSF_STARTPANELON         = 0x00200000, // Windows XP and later. The fStartPanelOn member is being requested.
            SSF_AUTOCHECKSELECT      = 0x00800000, // Windows Vista and later. The fAutoCheckSelect member is being requested.
            SSF_ICONSONLY            = 0x01000000, // Windows Vista and later. The fIconsOnly member is being requested.
            SSF_SHOWTYPEOVERLAY      = 0x02000000, // Windows Vista and later. The fShowTypeOverlay member is being requested.
            SSF_SHOWSTATUSBAR        = 0x04000000  // Windows 8 and later: The fShowStatusBar member is being requested.
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SHELLSTATE
        {
            public uint fFlags; // first: includes alignment padding
            public uint dwWin95Unused;
            public uint uWin95Unused;
            public int  lParamSort;
            public int  iSortDirection;
            public uint version;
            public uint uNotUsed;
            public uint sFlags; // second: includes spares at end to prevent access violation

            public bool fShowAllObjects       => (fFlags & 0x00000001) == 0x00000001;
            public bool fShowExtensions       => (fFlags & 0x00000002) == 0x00000002;
            public bool fNoConfirmRecycle     => (fFlags & 0x00000004) == 0x00000004;
            public bool fShowSysFiles         => (fFlags & 0x00000008) == 0x00000008;
            public bool fShowCompColor        => (fFlags & 0x00000010) == 0x00000010;
            public bool fDoubleClickInWebView => (fFlags & 0x00000020) == 0x00000020;
            public bool fDesktopHTML          => (fFlags & 0x00000040) == 0x00000040;
            public bool fWin95Classic         => (fFlags & 0x00000080) == 0x00000080;
            public bool fDontPrettyPath       => (fFlags & 0x00000100) == 0x00000100;
            public bool fShowAttribCol        => (fFlags & 0x00000200) == 0x00000200;
            public bool fMapNetDrvBtn         => (fFlags & 0x00000400) == 0x00000400;
            public bool fShowInfoTip          => (fFlags & 0x00000800) == 0x00000800;
            public bool fHideIcons            => (fFlags & 0x00001000) == 0x00001000;
            public bool fWebView              => (fFlags & 0x00002000) == 0x00002000;
            public bool fFilter               => (fFlags & 0x00004000) == 0x00004000;
            public bool fShowSuperHidden      => (fFlags & 0x00008000) == 0x00008000;
            public bool fNoNetCrawling        => (fFlags & 0x00010000) == 0x00010000;
            public bool fSepProcess           => (sFlags & 0x00000001) == 0x00000001;
            public bool fStartPanelOn         => (sFlags & 0x00000002) == 0x00000002;
            public bool fShowStartPage        => (sFlags & 0x00000004) == 0x00000004;
            public bool fAutoCheckSelect      => (sFlags & 0x00000008) == 0x00000008;
            public bool fIconsOnly            => (sFlags & 0x00000010) == 0x00000010;
            public bool fShowTypeOverlay      => (sFlags & 0x00000020) == 0x00000020;
            public bool fShowStatusBar        => (sFlags & 0x00000040) == 0x00000040;

            public override string ToString() => $"SHELLSTATE Version: {version}";
        }

        [DllImport("shell32.dll")]
        public static extern void SHGetSetSettings(ref SHELLSTATE lpss, SSF dwMask, bool bSet);

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

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(
            IntPtr hWnd,
            int msg,
            int wParam,
            int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(
            HandleRef hWnd, 
            int msg, 
            int wParam, 
            string lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(
            IntPtr hWnd, 
            int msg, 
            int wParam, 
            string lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(
            IntPtr hWnd, 
            UInt32 msg, 
            IntPtr wParam, 
            IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool SendMessage(
            IntPtr hWnd,
            int msg,
            IntPtr wParam,
            ref IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void SendMessage(
            IntPtr hWnd, 
            int msg, 
            int wParam, 
            ref RECT lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool SetWindowText(IntPtr hwnd, String lpString);

        /// <summary>
        /// Retrieves a handle to a control in the specified dialog box.
        /// </summary>
        /// <param name="hDlg">
        /// A handle to the dialog box that contains the control.
        /// </param>
        /// <param name="nIDDlgItem">
        /// The identifier of the control to be retrieved.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the window handle of the specified control.
        /// </returns>
		[DllImport("user32.dll")]
		public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);

		public const int SW_HIDE = 0;
		public const int SW_SHOW = 5;

        [DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        #region SetWindowPos
        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. 
        /// These windows are ordered according to their appearance on the screen. 
        /// The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="hWndInsertAfter">
        /// A handle to the window to precede the positioned window in the Z order. 
        /// This parameter must be a window handle or one of the following values.
        /// </param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="uFlags"></param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool SetWindowPos(
            IntPtr hWnd, 
            IntPtr hWndInsertAfter, 
            int x, 
            int y, 
            int cx, 
            int cy, 
            uint uFlags);

        public const int HWND_TOP       = 0;
        public const int HWND_BOTTOM    = 1;
        public const int HWND_TOPMOST   = -1;
        public const int HWND_NOTOPMOST = -2;
        //
        // The window sizing and positioning flags. 
        // This parameter can be a combination of the following values.
        //
        public const int 
        SWP_ASYNCWINDOWPOS = 0x4000,
        SWP_DEFERERASE     = 0x2000,
        SWP_DRAWFRAME      = 0x0020,
        SWP_FRAMECHANGED   = 0x0020,
        SWP_HIDEWINDOW     = 0x0080,
        SWP_NOACTIVATE     = 0x0010,
        SWP_NOCOPYBITS     = 0x0100,
        SWP_NOMOVE         = 0x0002,
        SWP_NOOWNERZORDER  = 0x0200,
        SWP_NOREDRAW       = 0x0008,
        SWP_NOREPOSITION   = 0x0200,
        SWP_NOSENDCHANGING = 0x0400,
        SWP_NOSIZE         = 0x0001,
        SWP_NOZORDER       = 0x0004,
        SWP_SHOWWINDOW     = 0x0040;
        #endregion

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

        /// <summary>
        /// The ScreenToClient function converts the screen coordinates 
        /// of a specified point on the screen to client-area coordinates.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window whose client area will be used for the conversion.
        /// </param>
        /// <param name="lpPoint">
        /// A pointer to a POINT structure that specifies the screen coordinates to be converted.
        /// </param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
		[DllImport("user32.dll")]
		public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

		public static bool ScreenToClient(IntPtr hWnd, ref RECT rc)
		{
			POINT pt1 = new POINT(rc.left, rc.top);
			if (!ScreenToClient(hWnd, ref pt1))
				return false;
			POINT pt2 = new POINT(rc.right, rc.bottom);
			if(!ScreenToClient(hWnd, ref pt2))
				return false;

			rc.left   = pt1.X;
			rc.top    = pt1.Y;
			rc.right  = pt2.X;
			rc.bottom = pt2.Y;

			return true;
		}

        // Window field offsets for GetWindowLong()
        public const int GWL_WNDPROC    = (-4);
		public const int GWL_HINSTANCE  = (-6);
		public const int GWL_HWNDPARENT = (-8);
		public const int GWL_STYLE      = (-16);
		public const int GWL_EXSTYLE    = (-20);
		public const int GWL_USERDATA   = (-21);
		public const int GWL_ID         = (-12);

        // Dialog Styles
        public const ulong DS_CONTEXTHELP = 0x2000L;

		[DllImport("user32.dll", SetLastError = true)]
		public static extern ulong GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, ulong dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(
            IntPtr hwndParent, 
            IntPtr hwndChildAfter, 
            string lpszClass, 
            string lpszWindow);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(
            IntPtr hwndParent, 
            IntPtr hwndChildAfter, 
            string lpszClass, 
            IntPtr windowTitle);

		public const uint SHGFI_PIDL       = 0x000000008;
		public const uint SHGFI_ATTRIBUTES = 0x000000800;
		public const uint SFGAO_LINK       = 0x00010000;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct SHFILEINFO
		{
			public IntPtr hIcon;
			public int    iIcon;
			public uint   dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		}

		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SHGetFileInfo(
            IntPtr         pidlPath, 
            uint           dwFileAttributes, 
            ref SHFILEINFO psfi, 
            int            cbFileInfo, 
            uint           uFlags);

		[DllImport("user32.dll")]
		public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

		[DllImport("gdi32.dll")]
		public static extern bool GetTextExtentPoint32(
            IntPtr   hdc, 
            string   lpString, 
            int      cbString, 
            out Size lpSize);

        [DllImport("gdi32.dll")]
        public static extern IntPtr GetStockObject(int fnObject);

        public const int DEFAULT_GUI_FONT = 17;

        [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		public static extern IntPtr GetDesktopWindow();

		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		public static extern IntPtr GetParent(IntPtr hWnd);
        
        /// <summary>
        /// CreateWindowEx function
        /// </summary>
        /// <param name="dwExStyle">
        /// The extended window style of the window being created. 
        /// For a list of possible values, see Extended Window Styles.
        /// </param>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <param name="dwStyle">
        /// The style of the window being created. 
        /// This parameter can be a combination of the window style values, 
        /// plus the control styles indicated in the Remarks section.
        /// </param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="hWndParent"></param>
        /// <param name="hMenu"></param>
        /// <param name="hInstance"></param>
        /// <param name="lpParam"></param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the new window.
        /// </returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr CreateWindowEx(
		   ulong  dwExStyle, 
           string lpClassName,
		   string lpWindowName,
		   ulong  dwStyle,
           int    x,
		   int    y,
		   int    nWidth,
		   int    nHeight,
		   IntPtr hWndParent,
		   IntPtr hMenu,
		   IntPtr hInstance,
		   IntPtr lpParam);

        #region window Styles
        public const ulong 
        WS_BORDER           = 0x00800000L,
        WS_CAPTION          = 0x00C00000L,
        WS_CHILD            = 0x40000000L,
        WS_CHILDWINDOW      = 0x40000000L,
        WS_CLIPCHILDREN     = 0x02000000L,
        WS_CLIPSIBLINGS     = 0x04000000L,
        WS_DISABLED         = 0x08000000L,
        WS_DLGFRAME         = 0x00400000L,
        WS_GROUP            = 0x00020000L,
        WS_HSCROLL          = 0x00100000L,
        WS_ICONIC           = 0x20000000L,
        WS_MAXIMIZE         = 0x01000000L,
        WS_MAXIMIZEBOX      = 0x00010000L,
        WS_MINIMIZE         = 0x20000000L,
        WS_MINIMIZEBOX      = 0x00020000L,
        WS_OVERLAPPED       = 0x00000000L,
        WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
        WS_POPUP            = 0x80000000L,
        WS_POPUPWINDOW      = (WS_POPUP | WS_BORDER | WS_SYSMENU),
        WS_SIZEBOX          = 0x00040000L,
        WS_SYSMENU          = 0x00080000L,
        WS_TABSTOP          = 0x00010000L,
        WS_THICKFRAME       = 0x00040000L,
        WS_TILED            = 0x00000000L,
        WS_TILEDWINDOW      = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX),
        WS_VISIBLE          = 0x10000000L,
        WS_VSCROLL          = 0x00200000L;
        #endregion

        #region extended window styles
        public const ulong 
        WS_EX_ACCEPTFILES         = 0x00000010L,
        WS_EX_APPWINDOW           = 0x00040000L,
        WS_EX_CLIENTEDGE          = 0x00000200L,
        WS_EX_COMPOSITED          = 0x02000000L,
        WS_EX_CONTEXTHELP         = 0x00000400L,
        WS_EX_CONTROLPARENT       = 0x00010000L,
        WS_EX_DLGMODALFRAME       = 0x00000001L,
        WS_EX_LAYERED             = 0x00080000L,
        WS_EX_LAYOUTRTL           = 0x00400000L,
        WS_EX_LEFT                = 0x00000000L,
        WS_EX_LEFTSCROLLBAR       = 0x00004000L,
        WS_EX_LTRREADING          = 0x00000000L,
        WS_EX_MDICHILD            = 0x00000040L,
        WS_EX_NOACTIVATE          = 0x08000000L,
        WS_EX_NOINHERITLAYOUT     = 0x00100000L,
        WS_EX_NOPARENTNOTIFY      = 0x00000004L,
        WS_EX_NOREDIRECTIONBITMAP = 0x00200000L,
        WS_EX_OVERLAPPEDWINDOW    = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
        WS_EX_PALETTEWINDOW       = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
        WS_EX_RIGHT               = 0x00001000L,
        WS_EX_RIGHTSCROLLBAR      = 0x00000000L,
        WS_EX_RTLREADING          = 0x00002000L,
        WS_EX_STATICEDGE          = 0x00020000L,
        WS_EX_TOOLWINDOW          = 0x00000080L,
        WS_EX_TOPMOST             = 0x00000008L,
        WS_EX_TRANSPARENT         = 0x00000020L,
        WS_EX_WINDOWEDGE          = 0x00000100L;
        #endregion

        #region button styles
        public const ulong 
        BS_TEXT          = 0x00000000L,
        BS_DEFPUSHBUTTON = 0x00000001L;
        #endregion
    }
}
