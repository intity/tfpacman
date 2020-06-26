using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace TFlex.PackageManager.Common
{
    /// <summary>
    /// Custom folder browser dialog.
    /// </summary>
    public class CustomFolderBrowserDialog
    {
        #region private fields
        private Window owner;
        #endregion

        #region public properties
        /// <summary>
        /// The dialog box title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The dialog box description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Any object that implements Window that represents
        /// the top-level window that will own the modal dialog box.
        /// </summary>
        public Window Owner
        {
            get { return owner; }
            set
            {
                if (owner != value)
                {
                    owner = value;
                }
            }
        }

        /// <summary>
        /// Specifies enumeration constant value 
        /// the directory paths to system special folders.
        /// </summary>
        public Environment.SpecialFolder RootFolder { get; set; }

        /// <summary>
        /// Specifies value selected path.
        /// </summary>
        public string SelectedPath { get; set; }

        /// <summary>
        /// Specifies value startup location the dialog box.
        /// </summary>
        public WindowStartupLocation StartupLocation { get; set; }
        #endregion

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct InitData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Title;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Description;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NativeMethods.MAX_PATH)]
            public string InitialPath;

            public WindowStartupLocation StartupLocation;
            public IntPtr Parent;

            public InitData(CustomFolderBrowserDialog dlg, IntPtr handle)
            {
                Title           = dlg.Title;
                Description     = dlg.Description;
                InitialPath     = dlg.SelectedPath;
                StartupLocation = dlg.StartupLocation;
                Parent          = handle;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CustomFolderBrowserDialog()
        {
            Title           = "Browse for Folder";
            RootFolder      = Environment.SpecialFolder.Desktop;
            StartupLocation = WindowStartupLocation.Manual;
            SelectedPath    = string.Empty;
        }

        /// <summary>
        /// Shows the form as a modal dialog box.
        /// </summary>
        /// <returns>One of the DialogResult values.</returns>
        public bool ShowDialog()
        {
            IntPtr handle = owner != null 
                ? new WindowInteropHelper(owner).Handle 
                : NativeMethods.GetDesktopWindow();
            InitData data = new InitData(this, handle);

            NativeMethods.BROWSEINFO bi = new NativeMethods.BROWSEINFO
            {
                hwndOwner = handle
            };

            if (0 != NativeMethods.SHGetSpecialFolderLocation(handle, (int)RootFolder, ref bi.pidlRoot))
                bi.pidlRoot = IntPtr.Zero;

            bi.lpszTitle = data.Description;
            bi.ulFlags   = NativeMethods.BIF_RETURNONLYFSDIRS | 
                           NativeMethods.BIF_NONEWFOLDERBUTTON | 
                           NativeMethods.BIF_EDITBOX;
            bi.lpfn      = new NativeMethods.BrowseCallbackProc(BrowseCallbackHandler);
            //
            // Initialization data, used in BrowseCallbackHandler
            //
            IntPtr hInit = Marshal.AllocHGlobal(Marshal.SizeOf(data));
            Marshal.StructureToPtr(data, hInit, true);
            bi.lParam = hInit;

            IntPtr pidlSelectedPath = IntPtr.Zero;
            try
            {
                pidlSelectedPath = NativeMethods.SHBrowseForFolder(ref bi);
                StringBuilder sb = new StringBuilder(256);

                if (NativeMethods.SHGetPathFromIDList(pidlSelectedPath, sb))
                {
                    SelectedPath = sb.ToString();
                    return true;
                }
            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                Marshal.FreeCoTaskMem(hInit);
                Marshal.FreeCoTaskMem(pidlSelectedPath);
            }

            return false;
        }

        #region private methods
        private int BrowseCallbackHandler(IntPtr hDlg, uint uMsg, IntPtr lParam, IntPtr lpData)
        {
            if (lpData == IntPtr.Zero) return 1;
            object obj = Marshal.PtrToStructure(lpData, typeof(InitData));
            if (obj == null) return 1;
            InitData initData = (InitData)obj;
            IntPtr path;

            switch (uMsg)
            {
                case NativeMethods.BFFM_INITIALIZED:
                    switch (initData.StartupLocation)
                    {
                        case WindowStartupLocation.CenterScreen:
                            CenterTo(hDlg, NativeMethods.GetDesktopWindow());
                            break;
                        case WindowStartupLocation.CenterOwner:
                            CenterTo(hDlg, initData.Parent);
                            break;
                    }
                    NativeMethods.SetWindowText(hDlg, initData.Title);
                    path = Marshal.StringToHGlobalUni(initData.InitialPath);
                    NativeMethods.SendMessage(hDlg, NativeMethods.BFFM_SETSELECTIONW, new IntPtr(1), path);
                    Marshal.FreeHGlobal(path);
                    break;
                case NativeMethods.BFFM_SELCHANGED:
                    StringBuilder sb = new StringBuilder(NativeMethods.MAX_PATH);
                    if (NativeMethods.SHGetPathFromIDList(lParam, sb))
                    {
                        initData.InitialPath = sb.ToString();
                    }
                    break;
            }
            return 0;
        }

        private static void CenterTo(IntPtr hDlg, IntPtr hRef)
        {
            NativeMethods.GetWindowRect(hDlg, out NativeMethods.RECT rcDlg);
            NativeMethods.GetWindowRect(hRef, out NativeMethods.RECT rcRef);

            int cx = (rcRef.Width - rcDlg.Width) / 2;
            int cy = (rcRef.Height - rcDlg.Height) / 2;

            NativeMethods.RECT rcNew = new NativeMethods.RECT(
                rcRef.left + cx,
                rcRef.top + cy,
                rcDlg.Width,
                rcDlg.Height);

            NativeMethods.MoveWindow(hDlg, rcNew, true);
        }
        #endregion
    }
}