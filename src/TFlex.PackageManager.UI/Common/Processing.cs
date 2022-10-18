using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using TFlex.Model;
using TFlex.Model.Model3D;
using TFlex.PackageManager.UI.Configuration;
using TFlex.PackageManager.UI.Model;

namespace TFlex.PackageManager.UI.Common
{
    /// <summary>
    /// Document processing class.
    /// </summary>
    internal class Processing
    {
        #region private fields
        readonly Header cfg;
        readonly Logging log;
        #endregion

        /// <summary>
        /// The Processing constructor.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="log"></param>
        public Processing(Header cfg, Logging log)
        {
            this.cfg = cfg;
            this.log = log;
        }

        #region internal methods
        /// <summary>
        /// File processing.
        /// </summary>
        /// <param name="item">The Processing Item.</param>
        internal void ProcessingFile(ProcItem item)
        {
            Document document = null;
            var tr = cfg.Translator as Translator;

            switch (tr.PMode)
            {
                case ProcessingMode.SaveAs:
                case ProcessingMode.Export:
                    if ((document = Application.OpenDocument(item.IPath, false)) != null)
                        log.WriteLine(LogLevel.INFO, 
                            string.Format("0-0 Processing [path: {0}]", 
                            item.IPath));
                    break;
                case ProcessingMode.Import:
                    var tr3d = cfg.Translator as Translator3D;
                    string prototype = tr3d.GetPrototypePath();
                    if ((document = Application.NewDocument(prototype, false)) != null)
                        log.WriteLine(LogLevel.INFO,
                            string.Format("0-1 Processing [path: {0}]", 
                            prototype));
                    break;
            }

            if (document == null)
            {
                log.WriteLine(LogLevel.ERROR, "--- The document object has a null value");
                return;
            }

            Start(document, item);
            End(document, item);

            if (Directory.GetFiles(item.Directory).Length == 0 &&
                Directory.GetDirectories(item.Directory).Length == 0)
                Directory.Delete(item.Directory, false);
        }

        /// <summary>
        /// Parents processing.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="path"></param>
        internal void ProcessingParent(ProcItem item, string path)
        {
            var parent = item.Parent;
            if (parent == null)
                return;

            var document = Application.OpenDocument(parent.OPath, false);
            if (document == null)
            {
                log.WriteLine(LogLevel.ERROR, "--- The document object has a null value");
                return;
            }

            log.WriteLine(LogLevel.INFO,
                string.Format("0-0 Processing [path: {0}]",
                document.FileName));

            if (path != null)
            {
                foreach (var link in document.FileLinks)
                {
                    if (link.FullFilePath != item.OPath)
                        continue;

                    ReplaceLink(document, link, path, parent.Directory);
                    item.OPath     = path;
                    item.Directory = path;
                }
            }

            UpdateLinks(document);
            End(document, parent);
            ProcessingParent(parent, null); // recursive call
        }
        #endregion

        #region private methods
        private string GetDirectory(ProcItem item)
        {
            //
            // get output directory
            //
            var name = Path.GetFileName(item.IPath);
            var path = item.IPath.Replace(name, "");
            item.Directory = path.Replace(cfg.InitialCatalog, cfg.TargetDirectory);

            var md_0 = cfg.Translator as Links;
            if (md_0 != null && 
                md_0.LinkTemplate.Length > 0 && item.Parent != null)
            {
                var link = md_0.GetLink(item);
                if (link != null)
                {
                    item.Directory = Path.Combine(item.Parent.Directory, 
                        link.Replace(item.FName, ""));
                }
            }

            if (Directory.Exists(item.Directory) == false)
                Directory.CreateDirectory(item.Directory);

            return item.Directory;
        }

        private void SaveAs(Document document, ProcItem item)
        {
            //
            // save copy document to output directory
            //
            if (document.SaveAs(item.OPath))
                log.WriteLine(LogLevel.INFO,
                    string.Format("0-4 Processing [path: {0}]", 
                    item.OPath));
        }

        private void Start(Document document, ProcItem item)
        {
            //
            // start processing
            //
            var md_4 = cfg.Translator as Files;
            var tr3d = cfg.Translator as Translator3D;

            switch (md_4.PMode)
            {
                case ProcessingMode.SaveAs:
                case ProcessingMode.Export:
                    if (md_4.TMode == TranslatorType.Document || tr3d != null)
                    {
                        item.FName = md_4.GetFileName(document);
                        item.OPath = Path.Combine(GetDirectory(item), 
                            item.FName + md_4.OExtension);
                    }
                    break;
                case ProcessingMode.Import:
                    item.FName = Path.GetFileNameWithoutExtension(item.IPath);
                    item.OPath = Path.Combine(GetDirectory(item), 
                        item.FName + md_4.OExtension);
                    if (tr3d.ImportMode > 0)
                    {
                        SaveAs(document, item);
                    }
                    break;
            }

            switch (md_4.TMode)
            {
                case TranslatorType.Document:
                    SaveAs(document, item);
                    ProcessingItems(document, item);
                    break;
                case TranslatorType.Acad:
                case TranslatorType.Bitmap:
                case TranslatorType.Pdf:
                    ProcessingItems(document, item);
                    break;
                case TranslatorType.Acis:
                    var tr_02 = cfg.Translator as Translator_2;
                    switch (md_4.PMode)
                    {
                        case ProcessingMode.Export:
                            tr_02.Export(document, item, log);
                            break;
                        case ProcessingMode.Import:
                            tr_02.Import(document, item, log);
                            break;
                    }
                    break;
                case TranslatorType.Iges:
                    var tr_06 = cfg.Translator as Translator_6;
                    switch (md_4.PMode)
                    {
                        case ProcessingMode.Export:
                            tr_06.Export(document, item, log);
                            break;
                        case ProcessingMode.Import:
                            tr_06.Import(document, item, log);
                            break;
                    }
                    break;
                case TranslatorType.Jt:
                    var tr_07 = cfg.Translator as Translator_7;
                    switch (md_4.PMode)
                    {
                        case ProcessingMode.Export:
                            tr_07.Export(document, item, log);
                            break;
                        case ProcessingMode.Import:
                            tr_07.Import(document, item, log);
                            break;
                    }
                    break;
                case TranslatorType.Step:
                    var tr_10 = cfg.Translator as Translator_10;
                    switch (md_4.PMode)
                    {
                        case ProcessingMode.Export:
                            tr_10.Export(document, item, log);
                            break;
                        case ProcessingMode.Import:
                            tr_10.Import(document, item, log);
                            break;
                    }
                    break;
            }

            if (md_4.PMode == ProcessingMode.Import)
            {
                log.WriteLine(LogLevel.INFO, 
                    string.Format("0-3 Processing [path: {0}]", 
                    document.FileName));
            }
        }

        private void End(Document document, ProcItem item)
        {
            string path = document.FileName;
            //
            // ending processing document
            //
            if (document.Changed)
            {
                if (document.FileName.Contains(cfg.InitialCatalog))
                {
                    document.CancelChanges();
                    log.WriteLine(LogLevel.INFO,
                        string.Format("0-5 Processing [path: {0}]",
                        path));
                }
                else if (document.Save())
                {
                    log.WriteLine(LogLevel.INFO,
                        string.Format("0-3 Processing [path: {0}]",
                        path));
                }
            }
            item.Pages.Clear();
            item.Links.Clear();
            document.Close();
            log.WriteLine(LogLevel.INFO, 
                string.Format("0-6 Processing [path: {0}]", 
                path));
        }

        private void UpdateLinks(Document document)
        {
            document.BeginChanges("Update Links");
            document.Regenerate(new RegenerateOptions
            {
                UpdateAllLinks = true
            });
            document.EndChanges();
            log.WriteLine(LogLevel.INFO,
                string.Format("0-2 Processing [path: {0}]",
                document.FileName));
        }

        private void ReplaceLink(Document document, FileLink link, string path, string target)
        {
            log.WriteLine(LogLevel.INFO, 
                string.Format("1-0 Processing [path: {0}, link: {1}]", 
                link.Document.FileName, 
                link.FilePath));

            document.BeginChanges("Replace Link");
            link.FilePath = path.Replace(target + "\\", "");
            log.WriteLine(LogLevel.INFO,
                string.Format("1-1 Processing [path: {0}, link: {1}]",
                link.Document.FileName,
                link.FilePath));
            document.EndChanges();
        }

        private void ProcessingItems(Document document, ProcItem item)
        {
            //
            // processing items (recursive method)
            //
            var md_4 = cfg.Translator as Files;

            ProcessingPages(document, item);
            ProcessingProjections(document, item);
            ProcessingVariables(document);
            ProcessingExport(document, item);

            foreach (var i in item.Items)
            {
                if ((i.Flags & 0x1) != 0x1)
                    continue;

                var ch_d = Application.OpenDocument(i.IPath, false);
                if (ch_d == null)
                    continue;

                log.WriteLine(LogLevel.INFO,
                    string.Format("0-0 Processing [path: {0}]",
                    i.IPath));

                if (md_4.TMode == TranslatorType.Document)
                {
                    i.FName = md_4.GetFileName(ch_d);
                    i.OPath = Path.Combine(GetDirectory(i),
                        i.FName + md_4.OExtension);
                    ProcessingLinks(document, ch_d, i);
                }

                ProcessingItems(ch_d, i); // recursive call
                End(ch_d, i);
            }

            if (item.Links.Count > 0)
                UpdateLinks(document);
        }

        private void ProcessingLinks(Document parent, Document child, ProcItem item)
        {
            //
            // links processing
            //
            if (!(cfg.Modules as Modules).Links)
                return;

            foreach (var link in parent.FileLinks)
            {
                if (link.FullFilePath != item.IPath)
                    continue;

                SaveAs(child, item);
                ReplaceLink(parent, link, item.OPath, item.Parent.Directory);
                item.Parent.Links.Add(link);
                break;
            }
        }

        private void ProcessingPages(Document document, ProcItem item)
        {
            //
            // pages processing
            //
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
                { PageType.Circuit,         0 },
                { PageType.Projection,      0 }
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

            int len = item.Pages.Count;
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
                        log.WriteLine(LogLevel.INFO, 
                            string.Format("0-2 Processing [path:{0}]", 
                            document.FileName));
                    }
                    document.EndChanges();
                }

                log.WriteLine(LogLevel.INFO, 
                    string.Format(CultureInfo.InvariantCulture,
                    "2-{0} Processing [name: {1}, id: {2:X}, scale: {3}, type: {4}]", 
                    action, 
                    page.Name, 
                    page.ObjectId, 
                    page.Scale.Value, 
                    page.PageType));

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
                    case PageType.Projection:      suffix = "_T7"; break;
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

        private void ProcessingProjections(Document document, ProcItem item)
        {
            //
            // projections processing
            //
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
                    log.WriteLine(LogLevel.INFO, 
                        string.Format("0-2 Processing [path: {0}]", 
                        document.FileName));
                }
                document.EndChanges();

                log.WriteLine(LogLevel.INFO, 
                    string.Format(CultureInfo.InvariantCulture,
                    "3-1 Processing [name: {0}, id: {1}, scale: {2}]",
                    i.Name, 
                    i.ObjectId, 
                    scale));
            }
        }

        private void ProcessingVariables(Document document)
        {
            //
            // variables processing
            //
            if (!(cfg.Modules as Modules).Variables)
                return;

            var tr_0 = cfg.Translator as Translator_0;

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

                log.WriteLine(LogLevel.INFO, 
                    string.Format("4-1 Processing [name: {0}, group: {1}, expression: {2}, external: {3}]", 
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

                log.WriteLine(LogLevel.INFO, 
                    string.Format("4-2 Processing [name: {0}, group: {1}, expression: {2}, external: {3}]", 
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

                log.WriteLine(LogLevel.INFO, 
                    string.Format("4-3 Processing [new_name: {0}, old_name: {1}]", 
                    e.Name,
                    e.OldName));

                hasRenaming = true;
            }

            if (hasRenaming)
            {
                document.BeginChanges("Regeterate Full");
                document.Regenerate(new RegenerateOptions { Full = true });
                document.EndChanges();
                log.WriteLine(LogLevel.INFO,
                    string.Format("0-2 Processing [path:{0}]", 
                    document.FileName));
            }

            foreach (var e in tr_0.RemoveVariables)
            {
                var variable = document.FindVariable(e.Name);
                if (variable == null)
                    continue;

                document.BeginChanges("Remove Variable");
                if (document.DeleteObjects(new ObjectArray(variable), new DeleteOptions(true)))
                {
                    log.WriteLine(LogLevel.INFO, 
                        string.Format("4-4 Processing [name: {0}]", 
                        e.Name));
                }
                document.EndChanges();
            }
        }

        private void ProcessingExport(Document document, ProcItem item)
        {
            //
            // export pages
            //
            var tr = cfg.Translator as Translator;

            switch (tr.TMode)
            {
                case TranslatorType.Acad:
                    var tr_1 = cfg.Translator as Translator_1;
                    tr_1.Export(document, item, log);
                    break;
                case TranslatorType.Bitmap:
                    var tr_3 = cfg.Translator as Translator_3;
                    tr_3.Export(document, item, log);
                    break;
                case TranslatorType.Pdf:
                    var tr_9 = cfg.Translator as Translator_9;
                    tr_9.Export(document, item, log);
                    break;
            }
        }
        #endregion
    }
}