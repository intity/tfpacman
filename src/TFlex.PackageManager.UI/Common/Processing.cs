using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TFlex.Configuration;
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
        readonly Configuration.Modules modules;
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
            this.modules = cfg.Modules as Configuration.Modules;

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
                    if ((document = Application.OpenDocument(item.IPath, false)) != null)
                        logging.WriteLine(LogLevel.INFO, ">>> Document [action: 0]");
                    break;
                case ProcessingMode.Import:
                    int iMode = (cfg.Translator as Translator3D).ImportMode;
                    string prototype = null;
                    using (Files files = new Files())
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

            ProcessingDocument(document, item);

            if (document.Changed)
            {
                if (tr.PMode == ProcessingMode.Export)
                {
                    document.CancelChanges();
                    logging.WriteLine(LogLevel.INFO, ">>> Document [action: 5]");
                }
                else
                {
                    document.EndChanges();
                    document.Save();
                    logging.WriteLine(LogLevel.INFO, ">>> Document [action: 3]");
                }
            }

            document.Close();
            logging.WriteLine(LogLevel.INFO, ">>> Document [action: 6]");

            if (Directory.GetFiles(item.Directory).Length == 0 &&
                Directory.GetDirectories(item.Directory).Length == 0)
                Directory.Delete(item.Directory, false);
        }
        #endregion

        #region private methods
        /// <summary>
        /// Extension method to split expression on tokens.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Returns expression tokens.</returns>
        private static string[] Groups(string expression)
        {
            string pattern = @"\((.*?)\)";
            string[] groups = expression.Split(new string[] { ".type", ".format" }, 
                StringSplitOptions.None);

            if (groups.Length > 1)
            {
                groups[1] = Regex.Match(expression, pattern).Groups[1].Value;
            }

            return groups;
        }

        /// <summary>
        /// Extension method to parse expression tokens.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        /// <param name="expression"></param>
        /// <returns>Returns variable value from document.</returns>
        private static string GetValue(Document document, Page page, string expression)
        {
            string result = null;
            string[] groups = Groups(expression);
            string[] argv;
            Variable variable;

            if (groups.Length > 1)
            {
                if (expression.Contains("page.type") && page != null)
                {
                    if ((argv = groups[1].Split(',')).Length < 2)
                        return result;

                    switch (argv[0])
                    {
                        case "0":
                            if (page.PageType == PageType.Normal)
                                result = argv[1];
                            break;
                        case "1":
                            if (page.PageType == PageType.Workplane)
                                result = argv[1];
                            break;
                        case "3":
                            if (page.PageType == PageType.Auxiliary)
                                result = argv[1];
                            break;
                        case "4":
                            if (page.PageType == PageType.Text)
                                result = argv[1];
                            break;
                        case "5":
                            if (page.PageType == PageType.BillOfMaterials)
                                result = argv[1];
                            break;
                    }
                }
                else if ((variable = document.FindVariable(groups[0])) != null)
                {
                    result = variable.IsText
                        ? variable.TextValue
                        : variable.RealValue.ToString(groups[1]);
                }
            }
            else
            {
                if ((variable = document.FindVariable(expression)) != null)
                {
                    result = variable.IsText
                        ? variable.TextValue
                        : variable.RealValue.ToString();
                }
            }

            return result;
        }

        /// <summary>
        /// Parse expression.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        /// <param name="expression"></param>
        /// <returns>Returns variable value from document.</returns>
        private static string ParseExpression(Document document, Page page, string expression)
        {
            string result = null;
            string pattern = @"(?:\?\?)";
            string[] tokens = new string[] { "??" };
            string[] group = expression.Split(tokens, StringSplitOptions.None).ToArray();
            MatchCollection matches = Regex.Matches(expression, pattern);

            if (group.Length > 1)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    switch (matches[i].Value)
                    {
                        case "??":

                            if ((result = GetValue(document, page, group[i])) != null ||
                                (result = GetValue(document, page, group[i + 1])) != null)
                            {
                                return result;
                            }
                            else
                                result = group[i + 1];
                            break;
                    }
                }
            }
            else
                result = GetValue(document, page, expression);

            return result ?? string.Empty;
        }

        /// <summary>
        /// Get output file name.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        /// <returns>Returns Output File name.</returns>
        private string GetFileName(Document document, Page page)
        {
            var tr = cfg.Translator as OutputFiles;
            string fileName, expVal, pattern = @"\{(.*?)\}";

            if (tr.TemplateFileName.Length > 0)
                fileName = tr.TemplateFileName.Replace(Environment.NewLine, "");
            else
            {
                fileName = Path.GetFileNameWithoutExtension(document.FileName);
                if (tr.FileNameSuffix.Length > 0)
                    fileName += ParseExpression(document, page, tr.FileNameSuffix);
                return fileName;
            }

            foreach (Match i in Regex.Matches(fileName, pattern))
            {
                if ((expVal = ParseExpression(document, page, i.Groups[1].Value)) == null)
                    continue;

                fileName = fileName.Replace(i.Groups[0].Value, expVal);
            }

            return fileName;
        }

        /// <summary>
        /// Get output directory path.
        /// </summary>
        /// <param name="item">Parent processing Item.</param>
        /// <returns>Returns Output Directory Path.</returns>
        private string GetDirectory(ProcItem item)
        {
            var directory = item.Directory;
            var tr = cfg.Translator as OutputFiles;
            if (tr.RenameSubdirectory)
            {
                var aPath = item.Directory.Split('\\');
                directory = item.Directory.Replace(aPath[aPath.Length - 1], item.FName);
            }
            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            return directory;
        }

        /// <summary>
        /// The Drawing Template exists.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private static bool DrawingTemplateExists(Document document, Page page)
        {
            if (document.GetFragments().Where(
                f => f.GroupType == ObjectType.Fragment && f.Page == page &&
                f.DisplayName.Contains("<Форматки>")).FirstOrDefault() != null)
                return true;

            return false;
        }

        /// <summary>
        /// The Page type exists.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        private bool PageTypeExists(Page page)
        {
            var tr_0 = cfg.Translator as Translator_0;

            Dictionary<PageType, bool> types = new Dictionary<PageType, bool>
            {
                { PageType.Normal,          tr_0.PageTypes.Normal },
                { PageType.Workplane,       tr_0.PageTypes.Workplane },
                { PageType.Auxiliary,       tr_0.PageTypes.Auxiliary },
                { PageType.Text,            tr_0.PageTypes.Text },
                { PageType.BillOfMaterials, tr_0.PageTypes.BillOfMaterials },
                { PageType.Circuit,         tr_0.PageTypes.Circuit }
            };

            return page.PageType != PageType.Dialog && types[page.PageType];
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
        /// The VariablesCount helper method.
        /// </summary>
        /// <returns>Variables Count.</returns>
        private int VariablesCount()
        {
            var tr_0 = cfg.Translator as Translator_0;
            List<string> variables = new List<string>();

            foreach (var i in tr_0.AddVariables)
            {
                variables.Add(i.Name);
            }

            foreach (var i in tr_0.EditVariables)
            {
                if (variables.Contains(i.Name) == false)
                    variables.Add(i.Name);
            }

            foreach (var i in tr_0.RenameVariables)
            {
                if (variables.Contains(i.Name) == false)
                    variables.Add(i.Name);
            }

            foreach (var i in tr_0.RemoveVariables)
            {
                if (variables.Contains(i.Name) == false)
                    variables.Add(i.Name);
            }

            return variables.Count;
        }

        /// <summary>
        /// Save source document as copy.
        /// </summary>
        /// <param name="document">Source document.</param>
        /// <param name="item"></param>
        private void DocumentSaveAs(Document document, ProcItem item)
        {
            if (item.FName == null)
                item.FName = GetFileName(document, null);
            if (item.OPath == null)
                item.OPath = Path.Combine(GetDirectory(item), item.FName + ".grb");

            if (!File.Exists(item.OPath) && document.SaveAs(item.OPath))
            {
                logging.WriteLine(LogLevel.INFO, 
                    string.Format("--> Document [action: 4, path: {0}]",
                    item.OPath));
            }
        }

        /// <summary>
        /// Processing document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="item">The Processing Item Object.</param>
        private void ProcessingDocument(Document document, ProcItem item)
        {
            var tr = cfg.Translator as Translator;
            string[] aPath = item.IPath.Split('\\');

            switch (tr.TMode)
            {
                case TranslatorType.Document:
                    DocumentSaveAs(document, item);
                    ProcessingLinks(document, item);
                    ProcessingPages(document, item);
                    ProcessingProjections(document, item);
                    ProcessingVariables(document);
                    break;
                case TranslatorType.Acad:
                case TranslatorType.Bitmap:
                case TranslatorType.Pdf:
                    ProcessingPages(document, item);
                    ProcessingProjections(document, item);
                    ProcessingExport(document, item);
                    break;
                case TranslatorType.Acis:
                    switch (tr.PMode)
                    {
                        case ProcessingMode.Export:
                            item.FName = GetFileName(document, null);
                            item.OPath = Path.Combine(item.Directory, item.FName + ".sat");
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
                    switch (tr.PMode)
                    {
                        case ProcessingMode.Export:
                            item.FName = GetFileName(document, null);
                            item.OPath = Path.Combine(item.Directory, item.FName + ".igs");
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
                    switch (tr.PMode)
                    {
                        case ProcessingMode.Export:
                            item.FName = GetFileName(document, null);
                            item.OPath = Path.Combine(item.Directory, item.FName + ".jt");
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
                    switch (tr.PMode)
                    {
                        case ProcessingMode.Export:
                            item.FName = GetFileName(document, null);
                            item.OPath = Path.Combine(item.Directory, item.FName + ".stp");
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
        /// The extension method to processing links.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="item"></param>
        private void ProcessingLinks(Document document, ProcItem item)
        {
            if (!modules.Links)
                return;

            if (item.Items.Count == 0)
                return;

            var len = document.FileLinks.Count();
            if (len > 0)
            {
                document.BeginChanges("Processing Links");
                logging.WriteLine(LogLevel.INFO, 
                    string.Format(">>> Processing Links [quantity: {0}]", len));
            }

            bool hasLinks = false;

            foreach (var link in document.FileLinks)
            {
                logging.WriteLine(LogLevel.INFO,
                    string.Format("--> Link [action: 0, id: 0x{0:X8}, path: {1}]",
                    link.InternalID.ToInt32(), link.FilePath));

                var path = link.FullFilePath;
                var ch_i = item.Items.Where(p => p.IPath == path).FirstOrDefault();
                if (ch_i == null)
                    continue;

                var ch_d = Application.OpenDocument(ch_i.IPath, false);
                if (ch_d == null)
                    continue;

                logging.WriteLine(LogLevel.INFO,
                    string.Format("--> Document [action: 0, path: {0}]",
                    ch_i.IPath));

                DocumentSaveAs(ch_d, ch_i);

                ch_d.Close();
                logging.WriteLine(LogLevel.INFO,
                    string.Format("--> Document [action: 6, path: {0}]",
                    ch_i.IPath));

                document.BeginChanges("Replace Link");
                ReplaceLink(link, ch_i.OPath);
                document.EndChanges();
                hasLinks = true;
            }

            if (hasLinks)
            {
                document.BeginChanges("Regenerate Links");
                document.Regenerate(new RegenerateOptions
                {
                    UpdateAllLinks = true
                });
                document.EndChanges();
                logging.WriteLine(LogLevel.INFO, 
                    ">>> Document [action: 2, mode: UpdateAllLinks]");
            }
        }

        /// <summary>
        /// The extension method to processing pages.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="item"></param>
        private void ProcessingPages(Document document, ProcItem item)
        {
            if (!modules.Pages)
                return;

            var tr_0 = cfg.Translator as Translator_0;
            int count = 0;
            string path = null;
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

                if (!PageTypeExists(p))
                    continue;

                if (tr_0.PageNames.Length > 0)
                    flags |= 0x1;

                if (flags == (flags & 0x1) && tr_0.PageNames.Contains(p.Name))
                    flags |= 0x2;

                if (tr_0.ExcludePage && flags == (flags & 0x2))
                    continue;

                if (flags == (flags & 0x1) && flags != (flags & 0x2))
                    continue;

                if (tr_0.CheckDrawingTemplate && 
                    !DrawingTemplateExists(document, p))
                    continue;

                types[p.PageType]++;
                item.Pages.Add(p, null);
            }

            var len = item.Pages.Count;
            if (len > 0)
            {
                document.BeginChanges("Processing Pages");
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
                    page.Scale = new Parameter((double)tr_0.PageScale);

                    if (tr_0.TMode == TranslatorType.Document)
                    {
                        document.Regenerate(new RegenerateOptions { Full = true });
                        logging.WriteLine(LogLevel.INFO, 
                            "--> Document [action: 2, mode: Full]");
                    }
                }

                logging.WriteLine(LogLevel.INFO, 
                    string.Format(CultureInfo.InvariantCulture,
                    "--> Page [action: {0}, id: {1:X}, name: {2}, scale: {3}, type: {4}]", 
                    action, page.ObjectId, page.Name, page.Scale.Value, page.PageType));

                if (tr_0.TMode != TranslatorType.Document)
                {
                    string suffix = string.Empty;
                    string extension = "." + tr_0.TargetExtension.ToLower();
                    path = item.Directory + "\\" + GetFileName(document, page);

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
                        path += "_" + (count + 1).ToString() + extension;
                        count++;
                    }
                    else if (item.Pages.ContainsValue(path + extension))
                        path += suffix + extension;
                    else
                        path += extension;
                }

                item.Pages[page] = path;
            }
        }

        /// <summary>
        /// The extension method to processing projections.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="item"></param>
        private void ProcessingProjections(Document document, ProcItem item)
        {
            if (!modules.Projections)
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

                if (flags == (flags & 0x1) && tr_0.ProjectionNames.Contains(i.Name))
                    flags |= 0x2;

                if (tr_0.ExcludeProjection && flags == (flags & 0x2))
                    continue;

                if (i.Scale.Value == Parameter.Default().Value && 
                    tr_0.ProjectionScale == 99999)
                    continue;

                if (i.Scale.Value == (double)tr_0.ProjectionScale)
                    continue;

                double scale = tr_0.ProjectionScale == 99999
                    ? Parameter.Default().Value 
                    : (double)tr_0.ProjectionScale;

                i.Scale = new Parameter(scale);

                if (tr_0.TMode == TranslatorType.Document)
                {
                    document.Regenerate(new RegenerateOptions { Projections = true });
                    logging.WriteLine(LogLevel.INFO, 
                        "--> Document [action: 2, mode: Projections]");
                }

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
            if (!modules.Variables)
                return;

            var tr_0 = cfg.Translator as Translator_0;
            int len = VariablesCount();
            if (len > 0)
            {
                document.BeginChanges("Processing Variables");
                logging.WriteLine(LogLevel.INFO, 
                    string.Format(">>> Processing Variables [quantity: {0}]", len));
            }

            foreach (var e in tr_0.AddVariables)
            {
                var variable = document.FindVariable(e.Name);
                if (variable == null)
                    variable = new Variable(document);
                else
                    continue;

                variable.Name = e.Name;
                variable.GroupName = e.Group;
                variable.Expression = e.Expression;
                if (variable.IsConstant)
                    variable.External = e.External;

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

                variable.GroupName = e.Group;
                variable.Expression = e.Expression;
                if (variable.IsConstant)
                    variable.External = e.External;

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

                variable.SetName(e.Name, true);
                logging.WriteLine(LogLevel.INFO, string.Format(
                        "--> Variable [action: 3, new name: {0}, old name: {1}]",
                        e.Name,
                        e.OldName));

                hasRenaming = variable.Name == e.Name;
            }

            if (hasRenaming)
                document.Regenerate(new RegenerateOptions { Full = true });

            foreach (var e in tr_0.RemoveVariables)
            {
                var variable = document.FindVariable(e.Name);
                if (variable == null)
                    continue;

                if (document.DeleteObjects(new ObjectArray(variable), new DeleteOptions(true)))
                {
                    logging.WriteLine(LogLevel.INFO, string.Format(
                        "--> Variable [action: 4, name: {0}]", e.Name));
                }
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