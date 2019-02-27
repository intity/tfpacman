using System.IO;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The PackageCollection class.
    /// </summary>
    public class PackageCollection
    {
        #region private fields
        private Header header;
        private ObservableDictionary<string, Header> newConfigurations;
        private ObservableDictionary<string, Header> oldConfigurations;
        private string targetDirectory;
        #endregion

        public PackageCollection()
        {
            newConfigurations = new ObservableDictionary<string, Header>();
            oldConfigurations = new ObservableDictionary<string, Header>();
            targetDirectory   = Resource.UserDirectory;
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
                    targetDirectory = value;
            }
        }

        /// <summary>
        /// Configuration list.
        /// </summary>
        internal ObservableDictionary<string, Header> Configurations
        {
            get { return (newConfigurations); }
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
                header = new Header { ConfigurationName = name };
                header.ConfigurationTask(0);
                newConfigurations.Add(name, header);
            }
        }

        internal void SetConfigurations()
        {
            foreach (var i in newConfigurations)
            {
                i.Value.ConfigurationTask(1);
                RemoveOldConfiguration(i.Key);
            }
        }

        /// <summary>
        /// Rename key from configuration dictionary.
        /// </summary>
        /// <param name="oldKey"></param>
        /// <param name="newKey"></param>
        internal void RenameConfiguration(string oldKey, string newKey)
        {
            if (newConfigurations[oldKey].IsLoaded)
            {
                newConfigurations[oldKey].OldName = oldKey;
                oldConfigurations.Add(oldKey, new Header { ConfigurationName = oldKey });
            }

            newConfigurations.RenameKey(oldKey, newKey);
        }

        /// <summary>
        /// Remove old registry key.
        /// </summary>
        /// <param name="newKey"></param>
        /// <returns></returns>
        internal bool RemoveOldConfiguration(string newKey)
        {
            string oldKey = newConfigurations[newKey].OldName;

            if (oldKey != null)
            {
                oldConfigurations[oldKey].ConfigurationTask(2);
                newConfigurations[newKey].OldName = null;
                return oldConfigurations.Remove(oldKey);
            }

            return false;
        }
        #endregion
    }
}