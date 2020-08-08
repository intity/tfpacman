using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using TFlex.Model;
using TFlex.Model.Model3D;
using TFlex.PackageManager.Configuration;
using TFlex.PackageManager.Model;

namespace TFlex.PackageManager.Common
{
    /// <summary>
    /// Document processing class.
    /// </summary>
    internal class Processing
    {
        #region private fields
        readonly Header cfg;
        readonly Logging logging;
        readonly Translator_1 tr_1;
        readonly Translator_2 tr_2;
        readonly Translator_3 tr_3;
        readonly Translator_6 tr_6;
        readonly Translator_7 tr_7;
        readonly Translator_9 tr_9;
        readonly Translator_10 tr_10;
        #endregion

        /// <summary>
        /// The Processing constructor.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="logging"></param>
        public Processing(Header cfg, Logging logging)
        {
            this.cfg     = cfg;
            this.logging = logging;

            switch ((cfg.Translator as Translator).TMode)
            {
                case TranslatorType.Acad:
                    tr_1 = cfg.Translator as Translator_1;
                    break;
                case TranslatorType.Acis:
                    tr_2 = cfg.Translator as Translator_2;
                    break;
                case TranslatorType.Bitmap:
                    tr_3 = cfg.Translator as Translator_3;
                    break;
                case TranslatorType.Iges:
                    tr_6 = cfg.Translator as Translator_6;
                    break;
                case TranslatorType.Jt:
                    tr_7 = cfg.Translator as Translator_7;
                    break;
                case TranslatorType.Pdf:
                    tr_9 = cfg.Translator as Translator_9;
                    break;
                case TranslatorType.Step:
                    tr_10 = cfg.Translator as Translator_10;
                    break;
            }
        }

        #region internal methods
        /// <summary>
        /// Processing file.
        /// </summary>
        /// <param name="item">The Processing Item Object.</param>
        internal void ProcessingFile(ProcItem item)
        {
            var tr = cfg.Translator as Translator;
            Document document = null;

            switch (tr.PMode)
            {
                case ProcessingMode.SaveAs:
                case ProcessingMode.Export:
                    document = OpenDocument(item);
                    break;
                case ProcessingMode.Import:
                    int iMode = (cfg.Translator as Translator3D).ImportMode;
                    string prototype = null;
                    using (TFlex.Configuration.Files files = new TFlex.Configuration.Files())
                    {
                        prototype = iMode == 2
                            ? files.Prototype3DName
                            : files.Prototype3DAssemblyName;
                    }
                    if ((document = Application.NewDocument(prototype)) != null)
                        logging.WriteLine(LogLevel.INFO,
                            string.Format(">>> Document [action: 1, path: {0}]",
                            prototype));
                    break;
            }

            if (document == null)
            {
                logging.WriteLine(LogLevel.ERROR, "The document object has a null value");
                return;
            }

            ProcessingStart(document, item);
            ProcessingEnd(document, item);

            if (Directory.GetFiles(item.Directory).Length == 0 &&
                Directory.GetDirectories(item.Directory).Length == 0)
                Directory.Delete(item.Directory, false);
        }
        #endregion

        #region private methods
        /// <summary>
        /// Get output directory path.
        /// </summary>
        /// <param name="item">Parent processing Item.</param>
        /// <returns>Returns Output Directory Path.</returns>
        private string GetDirectory(ProcItem item)
        {
            var md_0 = cfg.Translator as Links;
            if (md_0.LinkTemplate.Length > 0)
            {
                var link = md_0.GetLink(item);
                if (link != null)
                {
                    item.Directory = Path.Combine(cfg.TargetDirectory, 
                        link.Replace(item.FName, ""));
                }
            }
            if (Directory.Exists(item.Directory) == false)
                Directory.CreateDirectory(item.Directory);

            return item.Directory;
        }

        /// <summary>
        /// The ReplaceLink helper method.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="oPath"></param>
        private void ReplaceLink(FileLink link, string oPath)
        {
            string path = cfg.TargetDirectory + "\\";
            link.FilePath = oPath.Replace(path, "");
            logging.WriteLine(LogLevel.INFO,
                string.Format("--> Link [action: 1, id: 0x{0:X8}, path: {1}]",
                link.InternalID.ToInt32(), link.FilePath));
        }

        /// <summary>
        /// Open document.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private Document OpenDocument(ProcItem item)
        {
            var document = Application.OpenDocument(item.IPath, false);
            if (document == null)
                return null;
            var md_4 = cfg.Translator as Files;
            if (md_4.PMode == ProcessingMode.SaveAs)
            {
                item.FName = md_4.GetFileName(document, null);
                item.OPath = Path.Combine(GetDirectory(item), item.FName + ".grb");
            }
            if (item.OPath != null && File.Exists(item.OPath))
            {
                document.Close();
                document = Application.OpenDocument(item.OPath, false);
            }
            if (document != null)
            {
                logging.WriteLine(LogLevel.INFO,
                    string.Format(">>> Document [action: 0, path: {0}]",
                    item.IPath));
            }
            return document;
        }

        /// <summary>
        /// Save source document as copy.
        /// </summary>
        /// <param name="document">Source document.</param>
        /// <param name="item"></param>
        private void DocumentSaveAs(Document document, ProcItem item)
        {
            if ((item.Flags & 0x4) == 0x4)
                return;
            if (item.OPath != null && File.Exists(item.OPath))
                return;

            var md_4 = cfg.Translator as Files;
            item.FName = md_4.GetFileName(document, null);
            item.OPath = Path.Combine(GetDirectory(item), item.FName + ".grb");
            if (document.SaveAs(item.OPath))
            {
                logging.WriteLine(LogLevel.INFO, 
                    string.Format(">>> Document [action: 4, path: {0}]",
                    item.OPath));
            }
        }

        /// <summary>
        /// The processing start method.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="item">The Processing Item.</param>
        private void ProcessingStart(Document document, ProcItem item)
        {
            var md_4 = cfg.Translator as Files;
            string[] aPath = item.IPath.Split('\\');

            switch (md_4.TMode)
            {
                case TranslatorType.Document:
                    DocumentSaveAs(document, item);
                    ProcessingItems(document, item);
                    break;
                case TranslatorType.Acad:
                case TranslatorType.Bitmap:
                case TranslatorType.Pdf:
                    ProcessingItems(document, item);
                    break;
                case TranslatorType.Acis:
                    switch (md_4.PMode)
                    {
                        case ProcessingMode.Export:
                            item.FName = md_4.GetFileName(document, null);
                            item.OPath = Path.Combine(item.Directory, item.FName + tr_2.OExtension);
                            tr_2.Export(document, item.OPath, logging);
                            break;
                        case ProcessingMode.Import:
                            if (tr_2.ImportMode > 0)
                            {
                                item.FName = aPath[aPath.Length - 1].Replace(".sat", ".grb");
                                item.OPath = Path.Combine(item.Directory, item.FName);
                                document.SaveAs(item.OPath);
                            }
                            tr_2.Import(document, item.Directory, item.IPath, logging);
                            logging.WriteLine(LogLevel.INFO,
                                string.Format(">>> Document Saved [path: {0}]",
                                document.FileName));
                            break;
                    }
                    break;
                case TranslatorType.Iges:
                    switch (md_4.PMode)
                    {
                        case ProcessingMode.Export:
                            item.FName = md_4.GetFileName(document, null);
                            item.OPath = Path.Combine(item.Directory, item.FName + tr_6.OExtension);
                            tr_6.Export(document, item.OPath, logging);
                            break;
                        case ProcessingMode.Import:
                            if (tr_6.ImportMode > 0)
                            {
                                item.FName = aPath[aPath.Length - 1].Replace(".igs", ".grb");
                                item.OPath = Path.Combine(item.Directory, item.FName);
                                document.SaveAs(item.OPath);
                            }
                            tr_6.Import(document, item.Directory, item.IPath, logging);
                            logging.WriteLine(LogLevel.INFO,
                                string.Format(">>> Document Saved [path: {0}]",
                                document.FileName));
                            break;
                    }
                    break;
                case TranslatorType.Jt:
                    switch (md_4.PMode)
                    {
                        case ProcessingMode.Export:
                            item.FName = md_4.GetFileName(document, null);
                            item.OPath = Path.Combine(item.Directory, item.FName + tr_7.OExtension);
                            tr_7.Export(document, item.OPath, logging);
                            break;
                        case ProcessingMode.Import:
                            if (tr_7.ImportMode > 0)
                            {
                                item.FName = aPath[aPath.Length - 1].Replace(".jt", ".grb");
                                item.OPath = Path.Combine(item.Directory, item.FName);
                                document.SaveAs(item.OPath);
                            }
                            tr_7.Import(document, item.Directory, item.IPath, logging);
                            logging.WriteLine(LogLevel.INFO,
                                string.Format(">>> Document Saved [path: {0}]",
                                document.FileName));
                            break;
                    }
                    break;
                case TranslatorType.Step:
                    switch (md_4.PMode)
                    {
                        case ProcessingMode.Export:
                            item.FName = md_4.GetFileName(document, null);
                            item.OPath = Path.Combine(item.Directory, item.FName + tr_10.OExtension);
                            tr_10.Export(document, item.OPath, logging);
                            break;
                        case ProcessingMode.Import:
                            if (tr_10.ImportMode > 0)
                            {
                                item.FName = aPath[aPath.Length - 1].Replace(".stp", ".grb");
                                item.OPath = Path.Combine(item.Directory, item.FName);
                                document.SaveAs(item.OPath);
                            }
                            tr_10.Import(document, item.Directory, item.IPath, logging);
                            logging.WriteLine(LogLevel.INFO, 
                                string.Format(">>> Document Saved [path: {0}]", 
                                document.FileName));
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// The processing end method.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="item">The Processing Item.</param>
        private void ProcessingEnd(Document document, ProcItem item)
        {
            if (document.Changed)
            {
                if (document.FileName.Contains(cfg.InitialCatalog))
                {
                    document.CancelChanges();
                    logging.WriteLine(LogLevel.INFO,
                        string.Format(">>> Document [action: 5, path: {0}]",
                        item.IPath));
                }
                else
                {
                    document.Save();
                    logging.WriteLine(LogLevel.INFO,
                        string.Format(">>> Document [action: 3, path: {0}]",
                        item.IPath));
                }
            }
            document.Close();
            logging.WriteLine(LogLevel.INFO, 
                string.Format(">>> Document [action: 6, path: {0}]", 
                item.IPath));
        }

        /// <summary>
        /// The extension method to processing Items.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="item"></param>
        private void ProcessingItems(Document document, ProcItem item)
        {
            var tr = cfg.Translator as Translator;
            if ((item.Flags & 0x4) != 0x4)
            {
                ProcessingLinks(document);
                ProcessingPages(document, item);
                ProcessingProjections(document, item);
                ProcessingVariables(document);
                ProcessingExport(document, item);
            }

            foreach (var i in item.Items)
            {
                if ((i.Flags & 0x1) != 0x1)
                    continue;
                if ((i.Flags & 0x2) == 0x2)
                    continue;
                var ch_d = Application.OpenDocument(i.IPath, false);
                if (ch_d == null)
                    continue;

                logging.WriteLine(LogLevel.INFO,
                    string.Format(">>> Document [action: 0, path: {0}]",
                    i.IPath));

                if (tr.TMode == TranslatorType.Document)
                {
                    ProcessingLinks(document, ch_d, i);
                }

                ProcessingItems(ch_d, i); // recursive call
                ProcessingEnd(ch_d, i);

                i.Flags |= 0x2;
            }

            if (item.Links.Count > 0 && (item.Flags & 0x4) != 0x4)
            {
                document.BeginChanges("Regenerate Links");
                document.Regenerate(new RegenerateOptions
                {
                    UpdateAllLinks = true
                });
                document.EndChanges();
                logging.WriteLine(LogLevel.INFO, 
                    string.Format(">>> Document [action: 2, path: {0}, mode: UpdateAllLinks]", 
                    item.OPath));
            }
        }

        /// <summary>
        /// The extension method for logging links.
        /// </summary>
        /// <param name="document"></param>
        private void ProcessingLinks(Document document)
        {
            if (!(cfg.Modules as Modules).Links)
                return;

            var len = document.FileLinks.Count();
            if (len > 0)
            {
                logging.WriteLine(LogLevel.INFO,
                    string.Format(">>> Processing Links [quantity: {0}]", len));
            }
            foreach (var link in document.FileLinks)
            {
                logging.WriteLine(LogLevel.INFO,
                    string.Format("--> Link [action: 0, id: 0x{0:X8}, path: {1}]",
                    link.InternalID.ToInt32(), link.FilePath));
            }
        }

        /// <summary>
        /// The extension method to processing links.
        /// </summary>
        /// <param name="parent">Parent document.</param>
        /// <param name="child">Child document.</param>
        /// <param name="item">The processing subitem.</param>
        private void ProcessingLinks(Document parent, Document child, ProcItem item)
        {
            if (!(cfg.Modules as Modules).Links)
                return;
            if ((item.Flags & 0x4) == 0x4)
                return;

            foreach (var link in parent.FileLinks)
            {
                if (link.FullFilePath == item.IPath)
                {
                    DocumentSaveAs(child, item);
                    parent.BeginChanges("Replace Link");
                    ReplaceLink(link, item.OPath);
                    parent.EndChanges();
                    item.Parent.Links.Add(link);
                    break;
                }
            }
        }

        /// <summary>
        /// The extension method to processing pages.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="item"></param>
        private void ProcessingPages(Document document, ProcItem item)
        {
            if (!(cfg.Modules as Modules).Pages)
                return;

            var tr_0 = cfg.Translator as Translator_0;
            int count = 0;
            Dictionary<PageType, int> types = new Dictionary<PageType, int>
            {
                { PageType.Normal,          0 },
                { PageType.Workplane,       0 },
                { PageType.Auxiliary,       0 },
                { PageType.Text,            0 },
                { PageType.BillOfMaterials, 0 },
                { PageType.Circuit,         0 }
            };

            foreach (var p in document.GetPages())
            {
                uint flags = 0x0;

                if (!tr_0.PageTypeExists(p))
                    continue;

                if (tr_0.PageNames.Length > 0)
                    flags |= 0x1;

                if ((flags & 0x1) == 0x1 && tr_0.PageNames.Contains(p.Name))
                    flags |= 0x2;

                if (tr_0.ExcludePage && (flags & 0x2) == 0x2)
                    continue;

                if ((flags & 0x1) == 0x1 && (flags & 0x2) != 0x2)
                    continue;

                if (tr_0.CheckDrawingTemplate && !tr_0.DrawingTemplateExists(document, p))
                    continue;

                types[p.PageType]++;
                item.Pages.Add(p, null);
            }

            var len = item.Pages.Count;
            if (len > 0)
            {
                logging.WriteLine(LogLevel.INFO, 
                    string.Format(">>> Processing Pages [quantity: {0}]", len));
            }

            int action = 0;
            for (int i = 0; i < len; i++)
            {
                var page = item.Pages.ElementAt(i).Key;
                if (page.Scale.Value != (double)tr_0.PageScale && tr_0.PageScale != 99999)
                {
                    action = 1;

                    document.BeginChanges("Changing the page scale");
                    page.Scale = new Parameter((double)tr_0.PageScale);

                    if (tr_0.TMode == TranslatorType.Document)
                    {
                        document.Regenerate(new RegenerateOptions { Full = true });
                        logging.WriteLine(LogLevel.INFO, 
                            "--> Document [action: 2, mode: Full]");
                    }
                    document.EndChanges();
                }

                logging.WriteLine(LogLevel.INFO, 
                    string.Format(CultureInfo.InvariantCulture,
                    "--> Page [action: {0}, id: {1:X}, name: {2}, scale: {3}, type: {4}]", 
                    action, page.ObjectId, page.Name, page.Scale.Value, page.PageType));

                if (tr_0.TMode == TranslatorType.Document)
                    continue;

                string suffix = string.Empty;
                string ext = tr_0.OExtension;
                item.FName = tr_0.GetFileName(document, page);
                item.OPath = Path.Combine(GetDirectory(item), item.FName);

                switch (page.PageType)
                {
                    case PageType.Normal:          suffix = "_T0"; break;
                    case PageType.Workplane:       suffix = "_T1"; break;
                    case PageType.Auxiliary:       suffix = "_T3"; break;
                    case PageType.Text:            suffix = "_T4"; break;
                    case PageType.BillOfMaterials: suffix = "_T5"; break;
                    case PageType.Circuit:         suffix = "_T6"; break;
                }

                if (types[page.PageType] > 1)
                {
                    item.OPath += "_" + (count + 1).ToString() + ext;
                    count++;
                }
                else if (item.Pages.ContainsValue(item.OPath + ext))
                    item.OPath += suffix + ext;
                else
                    item.OPath += ext;

                item.Pages[page] = item.OPath;
            }
        }

        /// <summary>
        /// The extension method to processing projections.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="item"></param>
        private void ProcessingProjections(Document document, ProcItem item)
        {
            if (!(cfg.Modules as Modules).Projections)
                return;

            var tr_0 = cfg.Translator as Translator_0;
            ICollection<Projection> projections = null;

            if (item.Pages.Count == 0)
            {
                projections = document.GetProjections();
            }
            else if (tr_0.PageScale != 99999)
            {
                projections = new List<Projection>();
                foreach (var i in document.GetProjections())
                {
                    if (item.Pages.ContainsKey(i.Page))
                        projections.Add(i);
                }
            }

            if (projections == null)
                return;

            var len = projections.Count();
            if (len > 0)
            {
                logging.WriteLine(LogLevel.INFO, 
                    string.Format(">>> Processing Projections [quantity: {0}]", len));
            }

            foreach (var i in projections)
            {
                uint flags = 0x0;

                if (tr_0.ProjectionNames.Length > 0)
                    flags |= 0x1;

                if ((flags & 0x1) == 0x1 && tr_0.ProjectionNames.Contains(i.Name))
                    flags |= 0x2;

                if (tr_0.ExcludeProjection && (flags & 0x2) == 0x2)
                    continue;

                if (i.Scale.Value == Parameter.Default().Value && 
                    tr_0.ProjectionScale == 99999)
                    continue;

                if (i.Scale.Value == (double)tr_0.ProjectionScale)
                    continue;

                double scale = tr_0.ProjectionScale == 99999
                    ? Parameter.Default().Value 
                    : (double)tr_0.ProjectionScale;

                document.BeginChanges("Changing the projection scale");
                i.Scale = new Parameter(scale);

                if (tr_0.TMode == TranslatorType.Document)
                {
                    document.Regenerate(new RegenerateOptions { Projections = true });
                    logging.WriteLine(LogLevel.INFO, 
                        "--> Document [action: 2, mode: Projections]");
                }
                document.EndChanges();

                logging.WriteLine(LogLevel.INFO, 
                    string.Format(CultureInfo.InvariantCulture, 
                    "--> Projection [action: 1, id: {0}, name: {1}, scale: {2}]", 
                    i.ObjectId, i.Name, scale));
            }
        }

        /// <summary>
        /// The extension method to processing variables.
        /// </summary>
        /// <param name="document"></param>
        private void ProcessingVariables(Document document)
        {
            if (!(cfg.Modules as Modules).Variables)
                return;

            var tr_0 = cfg.Translator as Translator_0;
            int len = tr_0.VariablesCount();
            if (len > 0)
            {
                logging.WriteLine(LogLevel.INFO, 
                    string.Format(">>> Processing Variables [quantity: {0}]", len));
            }

            foreach (var e in tr_0.AddVariables)
            {
                var variable = document.FindVariable(e.Name);
                if (variable == null)
                {
                    document.BeginChanges("Create new Variable");
                    variable = new Variable(document);
                }
                else
                    continue;

                variable.Name = e.Name;
                variable.GroupName = e.Group;
                variable.Expression = e.Expression;
                if (variable.IsConstant)
                    variable.External = e.External;
                document.EndChanges();

                logging.WriteLine(LogLevel.INFO, string.Format(
                        "--> Variable [action: 1, name: {0}, group: {1}, expression: {2}, external: {3}]",
                        variable.Name,
                        variable.GroupName,
                        variable.Expression,
                        variable.External));
            }

            foreach (var e in tr_0.EditVariables)
            {
                var variable = document.FindVariable(e.Name);
                if (variable == null)
                    continue;

                document.BeginChanges("Edit Variable");
                variable.GroupName = e.Group;
                variable.Expression = e.Expression;
                if (variable.IsConstant)
                    variable.External = e.External;
                document.EndChanges();

                logging.WriteLine(LogLevel.INFO, string.Format(
                        "--> Variable [action: 2, name: {0}, group: {1}, expression: {2}, external: {3}]",
                        variable.Name,
                        variable.GroupName,
                        variable.Expression,
                        variable.External));
            }

            bool hasRenaming = false;
            foreach (var e in tr_0.RenameVariables)
            {
                var variable = document.FindVariable(e.OldName);
                if (variable == null)
                    continue;
                if (variable.Name == e.Name)
                    continue;

                document.BeginChanges("Rename Variable");
                variable.SetName(e.Name, true);
                document.EndChanges();

                logging.WriteLine(LogLevel.INFO, string.Format(
                        "--> Variable [action: 3, new name: {0}, old name: {1}]",
                        e.Name,
                        e.OldName));

                hasRenaming = true;
            }

            if (hasRenaming)
            {
                document.BeginChanges("Regeterate Full");
                document.Regenerate(new RegenerateOptions { Full = true });
                document.EndChanges();
            }

            foreach (var e in tr_0.RemoveVariables)
            {
                var variable = document.FindVariable(e.Name);
                if (variable == null)
                    continue;

                document.BeginChanges("Remove Variable");
                if (document.DeleteObjects(new ObjectArray(variable), new DeleteOptions(true)))
                {
                    logging.WriteLine(LogLevel.INFO, string.Format(
                        "--> Variable [action: 4, name: {0}]", e.Name));
                }
                document.EndChanges();
            }
        }

        /// <summary>
        /// Processing the export.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="item"></param>
        private void ProcessingExport(Document document, ProcItem item)
        {
            var tr = cfg.Translator as Translator;

            switch (tr.TMode)
            {
                case TranslatorType.Acad:
                    tr_1.Export(document, item.Pages, logging);
                    break;
                case TranslatorType.Bitmap:
                    tr_3.Export(document, item.Pages, logging);
                    break;
                case TranslatorType.Pdf:
                    tr_9.Export(document, item.Pages, logging);
                    break;
            }
        }
        #endregion
    }
}