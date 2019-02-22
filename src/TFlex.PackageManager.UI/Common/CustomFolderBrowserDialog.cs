using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TFlex.PackageManager.Common
{
    public class CustomFolderBrowserDialog
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool ShowEditbox { get; set; }
        public Environment.SpecialFolder RootFolder { get; set; }
        public string SelectedPath { get; set; }
        public bool ShowNewFolderButton { get; set; }
        public WindowStartupLocation StartupLocation { get; set; }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct InitData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Title;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Description;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = WinAPI.MAX_PATH)]
            public string InitialPath;

            public bool ShowEditbox;
            public bool ShowNewFolderButton;
            public WindowStartupLocation StartupLocation;
            public IntPtr Parent;

            public InitData(CustomFolderBrowserDialog dlg, IntPtr handle)
            {
                Title = dlg.Title;
                Description = dlg.Description;
                InitialPath = dlg.SelectedPath;
                ShowNewFolderButton = dlg.ShowNewFolderButton;
                ShowEditbox = dlg.ShowEditbox;
                StartupLocation = dlg.StartupLocation;
                Parent = handle;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CustomFolderBrowserDialog()
        {
            Title = "Browse for Folder";
            RootFolder = Environment.SpecialFolder.Desktop;
            SelectedPath = null;
            ShowEditbox = false;
            ShowNewFolderButton = false;
            StartupLocation = WindowStartupLocation.Manual;
        }

        /// <summary>
        /// Shows the form as a modal dialog box with the specified owner.
        /// </summary>
        /// <param name="owner">
        /// Any object that implements IWin32Window that represents 
        /// the top-level window that will own the modal dialog box.
        /// </param>
        /// <returns>One of the DialogResult values.</returns>
        public MessageBoxResult ShowDialog(Window owner)
        {
            IntPtr handle = new WindowInteropHelper(owner).Handle;
            InitData initData = new InitData(this, handle);

            WinAPI.BROWSEINFO bi = new WinAPI.BROWSEINFO
            {
                iImage = 0,
                hwndOwner = handle
            };

            if (0 != WinAPI.SHGetSpecialFolderLocation(handle, (int)RootFolder, ref bi.pidlRoot))
                bi.pidlRoot = IntPtr.Zero;
            bi.lpszTitle = initData.Description;
            bi.ulFlags = WinAPI.BIF_RETURNONLYFSDIRS /*| WinAPI.BIF_NEWDIALOGSTYLE*/;
            if (ShowEditbox)
                bi.ulFlags |= WinAPI.BIF_EDITBOX | WinAPI.BIF_VALIDATE;
            if (!ShowNewFolderButton)
                bi.ulFlags |= WinAPI.BIF_NONEWFOLDERBUTTON;
            bi.lpfn = new WinAPI.BrowseCallbackProc(BrowseCallbackHandler);
            //
            // Initialization data, used in BrowseCallbackHandler
            //
            IntPtr hInit = Marshal.AllocHGlobal(Marshal.SizeOf(initData));
            Marshal.StructureToPtr(initData, hInit, true);
            bi.lParam = hInit;

            IntPtr pidlSelectedPath = IntPtr.Zero;
            try
            {
                pidlSelectedPath = WinAPI.SHBrowseForFolder(ref bi);
                StringBuilder sb = new StringBuilder(256);

                if (WinAPI.SHGetPathFromIDList(pidlSelectedPath, sb))
                {
                    SelectedPath = sb.ToString();
                    return MessageBoxResult.OK;
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(pidlSelectedPath);
            }

            return MessageBoxResult.Cancel;
        }

        private int BrowseCallbackHandler(IntPtr hDlg, uint uMsg, IntPtr lParam, IntPtr lpData)
        {
            if (lpData == IntPtr.Zero) return 1;
            object obj = Marshal.PtrToStructure(lpData, typeof(InitData));
            if (obj == null) return 1;
            InitData initData = (InitData)obj;
            StringBuilder sb;

            switch (uMsg)
            {
                case WinAPI.BFFM_INITIALIZED:
                    switch (initData.StartupLocation)
                    {
                        case WindowStartupLocation.CenterScreen:
                            CenterTo(hDlg, WinAPI.GetDesktopWindow());
                            break;
                        case WindowStartupLocation.CenterOwner:
                            CenterTo(hDlg, initData.Parent);
                            break;
                    }
                    WinAPI.SetWindowText(hDlg, initData.Title);
                    WinAPI.SendMessage(hDlg, (int)WinAPI.BFFM_SETSELECTIONW, 1, initData.InitialPath);
                    break;
                case WinAPI.BFFM_SELCHANGED:
                    sb = new StringBuilder(WinAPI.MAX_PATH);
                    if (WinAPI.SHGetPathFromIDList(lParam, sb))
                    {
                        initData.InitialPath = sb.ToString();
                    }
                    break;
            }
            return 0;
        }

        private void CenterTo(IntPtr hDlg, IntPtr hRef)
        {
            WinAPI.GetWindowRect(hDlg, out WinAPI.RECT rcDlg);
            WinAPI.GetWindowRect(hRef, out WinAPI.RECT rcRef);

            int width = rcDlg.right - rcDlg.left;
            int Height = rcDlg.bottom - rcDlg.top;

            int cx = (rcRef.Width - rcDlg.Width) / 2;
            int cy = (rcRef.Height - rcDlg.Height) / 2;

            WinAPI.RECT rcNew = new WinAPI.RECT(
                rcRef.left + cx,
                rcRef.top + cy,
                rcDlg.Width,
                rcDlg.Height);

            WinAPI.MoveWindow(hDlg, rcNew, true);
        }
    }
}