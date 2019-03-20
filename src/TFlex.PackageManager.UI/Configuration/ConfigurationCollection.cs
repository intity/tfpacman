using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The Translator type enumeration.
    /// </summary>
    public enum TranslatorType
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
    /// The ConfigurationCollection class.
    /// </summary>
    public class ConfigurationCollection
    {
        #region private fields
        private ObservableDictionary<string, Header> configurations;
        private List<string> changedCofigurations;
        private string targetDirectory;
        #endregion

        public ConfigurationCollection()
        {
            configurations       = new ObservableDictionary<string, Header>();
            configurations.CollectionChanged += Configurations_CollectionChanged;
            changedCofigurations = new List<string>();
        }

        #region internal properties
        /// <summary>
        /// The target directory a configurations.
        /// </summary>
        internal string TargetDirectory
        {
            get { return targetDirectory; }
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

        /// <summary>
        /// Has changes configurations.
        /// </summary>
        internal bool HasChanged
        {
            get { return (changedCofigurations.Count > 0); }
        }
        #endregion

        #region internal methods
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

            //Debug.WriteLine(string.Format("GetConfigurations [count: {0}]",
            //    configurations.Count));
        }

        /// <summary>
        /// Save all configurations.
        /// </summary>
        internal void SetConfigurations()
        {
            foreach (var i in configurations)
            {
                if (i.Value.IsChanged)
                    i.Value.ConfigurationTask(1);
            }

            changedCofigurations.Clear();
        }
        #endregion

        #region event handlers
        private void Configurations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Header header;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    header = ((KeyValuePair<string, Header>)e.NewItems[0]).Value as Header;
                    header.PropertyChanged += Header_PropertyChanged;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    header = ((KeyValuePair<string, Header>)e.OldItems[0]).Value as Header;
                    changedCofigurations.Remove(header.ConfigurationName);
                    break;
            }
        }

        private void Header_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Header header)
            {
                if (header.IsChanged && !header.IsInvalid)
                {
                    if (changedCofigurations.Contains(header.ConfigurationName) == false)
                        changedCofigurations.Add(header.ConfigurationName);
                }
                else
                {
                    changedCofigurations.Remove(header.ConfigurationName);
                }

                //Debug.WriteLine(string.Format("Header_PropertyChanged [name: {0}, value: {1}, total changes: {2}]", 
                //    header.ConfigurationName, 
                //    header.IsChanged, 
                //    changedCofigurations.Count));
            }
        }
        #endregion
    }
}