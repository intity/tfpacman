using System.IO;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Package type enumeration.
    /// </summary>
    public enum PackageType
    {
        Default,
        Acad,
        Acis,
        Bitmap,
        Bmf,
        Emf,
        Iges,
        Jt,
        Parasolid,
        Pdf,
        Step,
        Stl
    }

    /// <summary>
    /// The PackageCollection class.
    /// </summary>
    public class PackageCollection
    {
        #region private fields
        private ObservableDictionary<string, Header> configurations;
        private string targetDirectory;
        #endregion

        public PackageCollection(Common.Options options)
        {
            configurations  = new ObservableDictionary<string, Header>();
            targetDirectory = options.TargetDirectory;

            GetConfigurations();
        }

        #region internal properties
        /// <summary>
        /// The target directory a configurations.
        /// </summary>
        internal string TargetDirectory
        {
            get { return (targetDirectory); }
            set
            {
                if (targetDirectory != value)
                {
                    targetDirectory = value;

                    configurations.Clear();
                    GetConfigurations();
                }
            }
        }

        /// <summary>
        /// Configuration list.
        /// </summary>
        internal ObservableDictionary<string, Header> Configurations
        {
            get { return (configurations); }
        }
        #endregion        

        #region methods
        /// <summary>
        /// Load all configurations from xml files.
        /// </summary>
        internal void GetConfigurations()
        {
            if (Directory.Exists(targetDirectory) == false)
                Directory.CreateDirectory(targetDirectory);

            foreach (var i in Directory.GetFiles(targetDirectory, "*.config"))
            {
                string name = Path.GetFileNameWithoutExtension(i);

                Header header = new Header
                {
                    UserDirectory = targetDirectory,
                    ConfigurationName = name
                };

                header.ConfigurationTask(0);
                configurations.Add(name, header);
            }
        }

        /// <summary>
        /// Save all configurations.
        /// </summary>
        internal void SetConfigurations()
        {
            foreach (var i in configurations)
            {
                i.Value.ConfigurationTask(1);
            }
        }
        #endregion
    }
}