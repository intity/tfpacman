using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
using TFlex.PackageManager.Common;

namespace TFlex.PackageManager.Configuration
{
    /// <summary>
    /// The ConfigurationCollection class.
    /// </summary>
    public class ConfigurationCollection
    {
        #region private fields
        readonly List<string> changedCofigurations;
        string directory;
        #endregion

        public ConfigurationCollection()
        {
            Configurations       = new ObservableDictionary<string, Header>();
            Configurations.CollectionChanged += Configurations_CollectionChanged;
            changedCofigurations = new List<string>();
        }

        #region internal properties
        /// <summary>
        /// User directory for the file configurations.
        /// </summary>
        internal string UserDirectory
        {
            get => directory;
            set
            {
                if (directory != value)
                {
                    directory = value;

                    Configurations.Clear();
                    GetConfigurations();
                }
            }
        }

        /// <summary>
        /// Configuration list.
        /// </summary>
        internal ObservableDictionary<string, Header> Configurations { get; }

        /// <summary>
        /// Has changes configurations.
        /// </summary>
        internal bool HasChanged => changedCofigurations.Count > 0;
        #endregion

        #region internal methods
        /// <summary>
        /// Load all configurations from xml files.
        /// </summary>
        internal void GetConfigurations()
        {
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            foreach (var i in Directory.GetFiles(directory, "*.config"))
            {
                var name = Path.GetFileNameWithoutExtension(i);
                var cfg = GetConfiguration(name);
                if (cfg != null)
                {
                    Configurations.Add(name, cfg);
                }
            }

            //Debug.WriteLine(string.Format("GetConfigurations [count: {0}]",
            //    configurations.Count));
        }

        /// <summary>
        /// Get configuration object.
        /// </summary>
        /// <param name="name">Configuration name.</param>
        /// <returns></returns>
        internal Header GetConfiguration(string name)
        {
            Header cfg = null;
            var data = new XmlSerializer(typeof(Header));
            var path = Path.Combine(directory, name + ".config");
            using (var fs = new FileStream(path, FileMode.Open))
            {
                cfg = data.Deserialize(fs) as Header;
                fs.Close();
            }
            return cfg;
        }

        /// <summary>
        /// Create new configuration file.
        /// </summary>
        /// <param name="name">Configuration name.</param>
        internal void SetConfiguration(string name)
        {
            var data = new XmlSerializer(typeof(Header));
            var path = Path.Combine(directory, name + ".config");
            using (var fs = new FileStream(path, FileMode.Create))
            {
                data.Serialize(fs, Configurations[name]);
                fs.Close();
            }
        }

        /// <summary>
        /// Save all configurations.
        /// </summary>
        internal void SetConfigurations()
        {
            foreach (var i in Configurations)
            {
                if (i.Value.IsChanged)
                {
                    SetConfiguration(i.Key);
                }
            }

            changedCofigurations.Clear();
        }
        #endregion

        #region event handlers
        private void Configurations_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Header cfg;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    cfg = ((KeyValuePair<string, Header>)e.NewItems[0]).Value;
                    cfg.PropertyChanged += Header_PropertyChanged;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    cfg = ((KeyValuePair<string, Header>)e.OldItems[0]).Value;
                    changedCofigurations.Remove(cfg.ConfigName);
                    var path = Path.Combine(directory, cfg.ConfigName + ".config");
                    File.Delete(path);
                    break;
            }
        }

        private void Header_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Header cfg)
            {
                if (cfg.IsChanged && !cfg.IsInvalid)
                {
                    if (changedCofigurations.Contains(cfg.ConfigName) == false)
                        changedCofigurations.Add(cfg.ConfigName);
                }
                else
                {
                    changedCofigurations.Remove(cfg.ConfigName);
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