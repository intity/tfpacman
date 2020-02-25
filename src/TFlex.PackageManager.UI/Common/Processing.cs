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
        private readonly Header header;
        private readonly List<ProcItem> items;
        private readonly Logging logging;
        private readonly Package package;
        private Category_3   categoryFile;
        private Translator_0 translator_0;
        private Translator_1 translator_1;
        private Translator_2 translator_2;
        private Translator_3 translator_3;
        private Translator_6 translator_6;
        private Translator_7 translator_7;
        private Translator_9 translator_9;
        private Translator_10 translator_10;
        private readonly TranslatorType t_mode;
        private readonly ProcessingMode p_mode;
        private readonly string mode;
        #endregion

        /// <summary>
        /// The Processing constructor.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="items"></param>
        /// <param name="t_mode"></param>
        /// <param name="p_mode"></param>
        /// <param name="logging"></param>
        public Processing(Header header, List<ProcItem> items, TranslatorType t_mode, ProcessingMode p_mode, Logging logging)
        {
            this.header  = header;
            this.items   = items;
            this.p_mode  = p_mode;
            this.t_mode  = t_mode;
            this.logging = logging;

            mode = Enum.GetName(typeof(TranslatorType), t_mode);
            package = new Package(header, t_mode);
        }

        #region internal methods
        /// <summary>
        /// Processing file.
        /// </summary>
        /// <param name="translator"></param>
        /// <param name="item">The Processing Item Object.</param>
        internal void ProcessingFile(object translator, ProcItem item)
        {
            translator_0 = translator as Translator_0;
            categoryFile = translator as Category_3;
            int importMode = 0;

            Document document = null;
            FileInfo fileInfo = new FileInfo(item.IPath);
            string directory = fileInfo.Directory.FullName.Replace(
                header.InitialCatalog,
                header.TargetDirectory + "\\" + mode);
            string targetDirectory = Directory.Exists(directory)
                ? directory
                : Directory.CreateDirectory(directory).FullName;

            switch (t_mode)
            {
                case TranslatorType.Acad:
                    translator_1 = translator as Translator_1;
                    break;
                case TranslatorType.Acis:
                    translator_2 = translator as Translator_2;
                    importMode = translator_2.ImportMode;
                    break;
                case TranslatorType.Bitmap:
                    translator_3 = translator as Translator_3;
                    break;
                case TranslatorType.Iges:
                    translator_6 = translator as Translator_6;
                    importMode = translator_6.ImportMode;
                    break;
                case TranslatorType.Jt:
                    translator_7 = translator as Translator_7;
                    importMode = translator_7.ImportMode;
                    break;
                case TranslatorType.Pdf:
                    translator_9 = translator as Translator_9;
                    break;
                case TranslatorType.Step:
                    translator_10 = translator as Translator_10;
                    importMode = translator_10.ImportMode;
                    break;
            }

            switch (p_mode)
            {
                case ProcessingMode.SaveAs:
                case ProcessingMode.Export:
                    if ((document = Application.OpenDocument(item.IPath, false)) != null)
                        logging.WriteLine(LogLevel.INFO, ">>> Document Opened");
                    break;
                case ProcessingMode.Import:
                    string prototype = null;
                    using (Files files = new Files())
                    {
                        prototype = importMode == 2
                            ? files.Prototype3DName
                            : files.Prototype3DAssemblyName;
                    }

                    if ((document = Application.NewDocument(prototype)) != null)
                        logging.WriteLine(LogLevel.INFO, 
                            string.Format(">>> Allocated new Document from Prototype: [path: {0};", 
                            prototype));
                    break;
            }

            if (document == null)
            {
                logging.WriteLine(LogLevel.ERROR, "The document object has a null value");
                return;
            }

            ProcessingDocument(document, targetDirectory, item);

            if (document.Changed)
            {
                if (p_mode == ProcessingMode.Export)
                    document.CancelChanges();
                else
                {
                    document.EndChanges();
                    document.Save();
                    logging.WriteLine(LogLevel.INFO, ">>> Document Saved");
                }
            }

            document.Close();
            logging.WriteLine(LogLevel.INFO, ">>> Document Closed");

            if (Directory.GetFiles(targetDirectory).Length == 0 &&
                Directory.GetDirectories(targetDirectory).Length == 0)
                Directory.Delete(targetDirectory, false);
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
            string[] groups = expression.Split(new string[] { ".type", ".format" }, StringSplitOptions.None);

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
        /// Get Output File name.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        /// <returns>Returns Output File name.</returns>
        private string GetOutputFileName(Document document, Page page)
        {
            string fileName, expVal, pattern = @"\{(.*?)\}";

            if (categoryFile.TemplateFileName.Length > 0)
                fileName = categoryFile.TemplateFileName.Replace(Environment.NewLine, "");
            else
            {
                fileName = Path.GetFileNameWithoutExtension(document.FileName);
                if (categoryFile.FileNameSuffix.Length > 0)
                    fileName += ParseExpression(document, page, categoryFile.FileNameSuffix);
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
        /// Get Output Directoty Path.
        /// </summary>
        /// <param name="targetDirectory"></param>
        /// <param name="item"></param>
        /// <returns>Returns Output Directory Path.</returns>
        private string GetOutputDirectory(string targetDirectory, ProcItem item)
        {
            var aPath = targetDirectory.Split('\\');
            var pName = package.GetParentName(item);
            var oPath = pName != null
                ? targetDirectory.Replace(aPath[aPath.Length - 1], pName)
                : targetDirectory;

            if (oPath != targetDirectory)
                Directory.CreateDirectory(oPath);

            return oPath;
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
            Dictionary<PageType, bool> types = new Dictionary<PageType, bool>
            {
                { PageType.Normal,          translator_0.PageTypes.Normal },
                { PageType.Workplane,       translator_0.PageTypes.Workplane },
                { PageType.Auxiliary,       translator_0.PageTypes.Auxiliary },
                { PageType.Text,            translator_0.PageTypes.Text },
                { PageType.BillOfMaterials, translator_0.PageTypes.BillOfMaterials },
                { PageType.Circuit,         translator_0.PageTypes.Circuit }
            };

            return page.PageType != PageType.Dialog && types[page.PageType];
        }

        /// <summary>
        /// The ReplaceLink helper method.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="link"></param>
        /// <param name="oPath"></param>
        private void ReplaceLink(Document document, FileLink link, string oPath)
        {
            link.FilePath = oPath.Replace(header.TargetDirectory + "\\" + mode + "\\", "");
            logging.WriteLine(LogLevel.INFO,
                string.Format("--> Link Replaced [id: 0x{0:X8}, path: {1}]",
                link.InternalID.ToInt32(), link.FilePath));
            document.Regenerate(new RegenerateOptions { UpdateAllLinks = true });
            logging.WriteLine(LogLevel.INFO, "--> Links Updated");
        }

        /// <summary>
        /// The VariablesCount helper method.
        /// </summary>
        /// <returns>Variables Count.</returns>
        private int VariablesCount()
        {
            List<string> variables = new List<string>();

            foreach (var i in translator_0.AddVariables)
            {
                variables.Add(i.Name);
            }

            foreach (var i in translator_0.EditVariables)
            {
                if (variables.Contains(i.Name) == false)
                    variables.Add(i.Name);
            }

            foreach (var i in translator_0.RenameVariables)
            {
                if (variables.Contains(i.Name) == false)
                    variables.Add(i.Name);
            }

            foreach (var i in translator_0.RemoveVariables)
            {
                if (variables.Contains(i.Name) == false)
                    variables.Add(i.Name);
            }

            return variables.Count;
        }

        /// <summary>
        /// Processing document.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="item">The Processing Item Object.</param>
        private void ProcessingDocument(Document document, string targetDirectory, ProcItem item)
        {
            string[] aPath = item.IPath.Split('\\');

            switch (t_mode)
            {
                case TranslatorType.Document:
                    var oDir   = GetOutputDirectory(targetDirectory, item);
                    item.FName = GetOutputFileName(document, null);
                    item.OPath = Path.Combine(oDir, item.FName + ".grb");

                    if (document.SaveAs(item.OPath))
                    {
                        logging.WriteLine(LogLevel.INFO, string.Format(">>> Document Saved As [path: {0}]", 
                            item.OPath));

                        package.SetMetadata(item);

                        ProcessingLinks(document, item);
                        ProcessingPages(document, null);
                        ProcessingVariables(document);
                    }
                    break;
                case TranslatorType.Acad:
                case TranslatorType.Bitmap:
                case TranslatorType.Pdf:
                    ProcessingPages(document, targetDirectory);
                    break;
                case TranslatorType.Acis:
                    switch(p_mode)
                    {
                        case ProcessingMode.Export:
                            item.FName = GetOutputFileName(document, null);
                            item.OPath = Path.Combine(targetDirectory, item.FName + ".sat");
                            translator_2.Export(document, item.OPath, logging);
                            break;
                        case ProcessingMode.Import:
                            if (translator_2.ImportMode > 0)
                            {
                                item.FName = aPath[aPath.Length - 1].Replace(".sat", ".grb");
                                item.OPath = Path.Combine(targetDirectory, item.FName);
                                document.SaveAs(item.OPath);
                            }
                            translator_2.Import(document, targetDirectory, item.IPath, logging);
                            logging.WriteLine(LogLevel.INFO, string.Format(">>> Document Saved [path: {0}]", 
                                document.FileName));
                            break;
                    }
                    break;
                case TranslatorType.Iges:
                    switch (p_mode)
                    {
                        case ProcessingMode.Export:
                            item.FName = GetOutputFileName(document, null);
                            item.OPath = Path.Combine(targetDirectory, item.FName + ".igs");
                            translator_6.Export(document, item.OPath, logging);
                            break;
                        case ProcessingMode.Import:
                            if (translator_6.ImportMode > 0)
                            {
                                item.FName = aPath[aPath.Length - 1].Replace(".igs", ".grb");
                                item.OPath = Path.Combine(targetDirectory, item.FName);
                                document.SaveAs(item.OPath);
                            }
                            translator_6.Import(document, targetDirectory, item.IPath, logging);
                            logging.WriteLine(LogLevel.INFO, string.Format(">>> Document Saved [path: {0}]", 
                                document.FileName));
                            break;
                    }
                    break;
                case TranslatorType.Jt:
                    switch (p_mode)
                    {
                        case ProcessingMode.Export:
                            item.FName = GetOutputFileName(document, null);
                            item.OPath = Path.Combine(targetDirectory, item.FName + ".jt");
                            translator_7.Export(document, item.OPath, logging);
                            break;
                        case ProcessingMode.Import:
                            if (translator_7.ImportMode > 0)
                            {
                                item.FName = aPath[aPath.Length - 1].Replace(".jt", ".grb");
                                item.OPath = Path.Combine(targetDirectory, item.FName);
                                document.SaveAs(item.OPath);
                            }
                            translator_7.Import(document, targetDirectory, item.IPath, logging);
                            logging.WriteLine(LogLevel.INFO, string.Format(">>> Document Saved [path: {0}]", 
                                document.FileName));
                            break;
                    }
                    break;
                case TranslatorType.Step:
                    switch (p_mode)
                    {
                        case ProcessingMode.Export:
                            item.FName = GetOutputFileName(document, null);
                            item.OPath = Path.Combine(targetDirectory, item.FName + ".stp");
                            translator_10.Export(document, item.OPath, logging);
                            break;
                        case ProcessingMode.Import:
                            if (translator_10.ImportMode > 0)
                            {
                                item.FName = aPath[aPath.Length - 1].Replace(".stp", ".grb");
                                item.OPath = Path.Combine(targetDirectory, item.FName);
                                document.SaveAs(item.OPath);
                            }
                            translator_10.Import(document, targetDirectory, item.IPath, logging);
                            logging.WriteLine(LogLevel.INFO, string.Format(">>> Document Saved [path: {0}]", 
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
            var len = document.FileLinks.Count();
            if (len > 0)
            {
                document.BeginChanges("Processing Links");
                logging.WriteLine(LogLevel.INFO, string.Format(">>> Processing Links: {0}", len));
            }

            foreach (var link in document.FileLinks)
            {
                logging.WriteLine(LogLevel.INFO,
                    string.Format("--> Link [id: 0x{0:X8}, path: {1}]",
                    link.InternalID.ToInt32(), link.FilePath));

                foreach (var i in package.Items)
                {
                    var iPath = i.Attribute("path");
                    var oPath = i.Element("output")?.Attribute("path");
                    if (oPath != null && link.FullFilePath == iPath.Value)
                    {
                        ReplaceLink(document, link, oPath.Value);
                        break;
                    }
                }
            }

            if (item.Parent == null || item.Parent.OPath == null)
                return;

            var parent = TFlex.Application.OpenDocument(item.Parent.OPath, false);
            if (parent == null)
                return;

            logging.WriteLine(LogLevel.INFO,
                string.Format("--> Parent Document Opened [path: {0}]", item.Parent.OPath));
            parent.BeginChanges("Replace Link");

            foreach (var link in parent.FileLinks)
            {
                if (link.FullFilePath == item.IPath)
                {
                    ReplaceLink(parent, link, item.OPath);
                    break;
                }
            }

            if (parent.Changed)
            {
                parent.EndChanges();
                parent.Save();
                logging.WriteLine(LogLevel.INFO, "--> Parent Document Saved");
            }

            parent.Close();
            logging.WriteLine(LogLevel.INFO, "--> Parent Document Closed");
        }

        /// <summary>
        /// The extension method to processing pages.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="targetDirectory"></param>
        private void ProcessingPages(Document document, string targetDirectory)
        {
            int count = 0;
            uint flags;
            string path = null;
            Dictionary<Page, string> o_pages = new Dictionary<Page, string>();
            Dictionary<PageType, int> types = new Dictionary<PageType, int>
            {
                { PageType.Normal,          0 },
                { PageType.Workplane,       0 },
                { PageType.Auxiliary,       0 },
                { PageType.Text,            0 },
                { PageType.BillOfMaterials, 0 },
                { PageType.Circuit,         0 }
            };
            List<Page> pages = new List<Page>();

            foreach (var p in document.GetPages())
            {
                flags = 0x0000;
                flags |= (uint)(translator_0.PageNames.Length > 0       ? 0x0001 : 0x0000);
                flags |= (uint)(translator_0.PageNames.Contains(p.Name) ? 0x0002 : 0x0000);
                flags |= (uint)(translator_0.ExcludePage                ? 0x0004 : 0x0000);
                flags |= (uint)(PageTypeExists(p)                       ? 0x0008 : 0x0000);

                if (flags == 0x0000 || 
                    flags == 0x0001 || 
                    flags == 0x0003 || 
                    flags == 0x0005 || 
                    flags == 0x0007 || 
                    flags == 0x0009 || 
                    flags == 0x000F)
                    continue;

                if (translator_0.CheckDrawingTemplate && !DrawingTemplateExists(document, p))
                    continue;

                types[p.PageType]++;
                pages.Add(p);
            }

            var len = pages.Count();
            if (len > 0)
            {
                document.BeginChanges("Processing Pages");
                logging.WriteLine(LogLevel.INFO, string.Format(">>> Processing Pages: {0}", len));
            }

            foreach (var i in pages)
            {
                if (i.Scale.Value != (double)translator_0.PageScale && translator_0.PageScale != 99999)
                {
                    i.Scale = new Parameter((double)translator_0.PageScale);

                    if (t_mode == TranslatorType.Document)
                    {
                        document.Regenerate(new RegenerateOptions { Full = true });
                    }
                }

                logging.WriteLine(LogLevel.INFO, string.Format(CultureInfo.InvariantCulture,
                    "--> Page [id: {0:X}, name: {1}, scale: {2}, type: {3}]", 
                    i.ObjectId, i.Name, i.Scale.Value, i.PageType));

                //Debug.WriteLine(string.Format("Page name: {0}, flags: {1:X4}", i.Name, flags));

                if (translator_0.PageScale != 99999)
                {
                    ProcessingProjections(document, i);
                }

                if (t_mode != TranslatorType.Document)
                {
                    string suffix = string.Empty;
                    string extension = "." + translator_0.TargetExtension.ToLower();
                    path = targetDirectory + "\\" + GetOutputFileName(document, i);

                    switch (i.PageType)
                    {
                        case PageType.Normal:          suffix = "_T0"; break;
                        case PageType.Workplane:       suffix = "_T1"; break;
                        case PageType.Auxiliary:       suffix = "_T3"; break;
                        case PageType.Text:            suffix = "_T4"; break;
                        case PageType.BillOfMaterials: suffix = "_T5"; break;
                        case PageType.Circuit:         suffix = "_T6"; break;
                    }

                    if (types[i.PageType] > 1)
                    {
                        path += "_" + (count + 1).ToString() + extension;
                        count++;
                    }
                    else if (o_pages.ContainsValue(path + extension))
                        path += suffix + extension;
                    else
                        path += extension;
                }

                o_pages.Add(i, path);
            }

            switch (t_mode)
            {
                case TranslatorType.Acad:
                    translator_1.Export(document, o_pages, logging);
                    break;
                case TranslatorType.Bitmap:
                    translator_3.Export(document, o_pages, logging);
                    break;
                case TranslatorType.Pdf:
                    translator_9.Export(document, o_pages, logging);
                    break;
            }
        }

        /// <summary>
        /// The extension method to processing projections.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        private void ProcessingProjections(Document document, Page page)
        {
            uint flags;
            var projections = document.GetProjections().Where(p => p.Page == page);
            if (projections.Count() > 0)
            {
                logging.WriteLine(LogLevel.INFO, string.Format(">>> Processing Projections: {0}", 
                    projections.Count()));
            }

            foreach (var i in projections)
            {
                flags = 0x0000;
                flags |= (uint)(translator_0.ProjectionNames.Length > 0       ? 0x0001 : 0x0000);
                flags |= (uint)(translator_0.ProjectionNames.Contains(i.Name) ? 0x0002 : 0x0000);
                flags |= (uint)(translator_0.ExcludeProjection                ? 0x0004 : 0x0000);

                if (flags == 0x0001 || flags == 0x0007)
                    continue;

                if (i.Scale.Value == Parameter.Default().Value && translator_0.ProjectionScale == 99999)
                    continue;

                if (i.Scale.Value == (double)translator_0.ProjectionScale)
                    continue;

                double scale = translator_0.ProjectionScale == 99999
                    ? Parameter.Default().Value 
                    : (double)translator_0.ProjectionScale;

                i.Scale = new Parameter(scale);

                if (t_mode == TranslatorType.Document)
                {
                    document.Regenerate(new RegenerateOptions { Projections = true });
                }

                logging.WriteLine(LogLevel.INFO, string.Format(CultureInfo.InvariantCulture, 
                    "--> Projection [id: {0}, name: {1}, scale: {2}]", 
                    i.ObjectId, i.Name, scale));

                //Debug.WriteLine(string.Format("Projection [name: {0}, flags: {1:X4}]", i.Name, flags));
            }
        }

        /// <summary>
        /// The extension method to processing variables.
        /// </summary>
        /// <param name="document"></param>
        private void ProcessingVariables(Document document)
        {
            int len = VariablesCount();
            if (len > 0)
            {
                document.BeginChanges("Processing Variables");
                logging.WriteLine(LogLevel.INFO, string.Format(">>> Processing Variables: {0}", len));
            }

            foreach (var e in translator_0.AddVariables)
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
                        "--> Variable [action: add, name: {0}, group: {1}, expression: {2}, external: {3}]",
                        variable.Name,
                        variable.GroupName,
                        variable.Expression,
                        variable.External));
            }

            foreach (var e in translator_0.EditVariables)
            {
                var variable = document.FindVariable(e.Name);
                if (variable == null)
                    continue;

                variable.GroupName = e.Group;
                variable.Expression = e.Expression;
                if (variable.IsConstant)
                    variable.External = e.External;

                logging.WriteLine(LogLevel.INFO, string.Format(
                        "--> Variable [action: edit, name: {0}, group: {1}, expression: {2}, external: {3}]",
                        variable.Name,
                        variable.GroupName,
                        variable.Expression,
                        variable.External));
            }

            bool hasRenaming = false;
            foreach (var e in translator_0.RenameVariables)
            {
                var variable = document.FindVariable(e.OldName);
                if (variable == null)
                    continue;

                variable.SetName(e.Name, true);
                logging.WriteLine(LogLevel.INFO, string.Format(
                        "--> Variable [action: rename, new_name: {0}, old_name: {1}]",
                        e.Name,
                        e.OldName));

                hasRenaming = variable.Name == e.Name;
            }

            if (hasRenaming)
                document.Regenerate(new RegenerateOptions { Full = true });

            foreach (var e in translator_0.RemoveVariables)
            {
                var variable = document.FindVariable(e.Name);
                if (variable == null)
                    continue;

                if (document.DeleteObjects(new ObjectArray(variable), new DeleteOptions(true)))
                {
                    logging.WriteLine(LogLevel.INFO, string.Format(
                        "--> Variable [action: remove, name: {0}]", e.Name));
                }
            }
        }
        #endregion
    }
}