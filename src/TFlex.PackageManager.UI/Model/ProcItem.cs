namespace TFlex.PackageManager.Model
{
    /// <summary>
    /// The Processing Item class.
    /// </summary>
    internal class ProcItem
    {
        /// <summary>
        /// The Processing Item Constructor.
        /// </summary>
        /// <param name="path">Input path.</param>
        public ProcItem(string path)
        {
            IPath = path;
        }

        /// <summary>
        /// The File Name.
        /// </summary>
        public string FName { get; set; }

        /// <summary>
        /// Input Path the File.
        /// </summary>
        public string IPath { get; }

        /// <summary>
        /// Output Path the File.
        /// </summary>
        public string OPath { get; set; }

        /// <summary>
        /// The Parent Processing Item.
        /// </summary>
        public ProcItem Parent { get; set; }
    }
}
