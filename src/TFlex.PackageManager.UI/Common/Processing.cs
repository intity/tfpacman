using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TFlex.Model;
using TFlex.Model.Model3D;
using TFlex.PackageManager.Configuration;

namespace TFlex.PackageManager.Common
{
    /// <summary>
    /// Document processing class.
    /// </summary>
    internal class Processing
    {
        #region private fields
        private readonly Header header;
        private readonly LogFile logFile;
        private Translator_0 translator_0;
        private Translator_1 translator_1;
        private Translator_3 translator_3;
        private Translator_9 translator_9;
        private TranslatorType translator_t;
        #endregion

        public Processing(Header header, LogFile logFile)
        {
            this.header  = header;
            this.logFile = logFile;
        }

        #region internal methods
        /// <summary>
        /// Processing file.
        /// </summary>
        /// <param name="translator"></param>
        /// <param name="type"></param>
        /// <param name="path"></param>
        internal void ProcessingFile(object translator, TranslatorType type, string path)
        {
            translator_0 = translator as Translator_0;
            translator_1 = translator as Translator_1;
            translator_3 = translator as Translator_3;
            translator_9 = translator as Translator_9;
            translator_t = type;

            Document document = Application.OpenDocument(path, false);
            logFile.AppendLine(string.Format("Open document:\t{0}", path));

            if (document == null)
            {
                logFile.AppendLine("Processing failed: the document object has a null value.");
                return;
            }

            FileInfo fileInfo = new FileInfo(path);
            string directory = fileInfo.Directory.FullName.Replace(
                header.InitialCatalog,
                header.TargetDirectory + "\\" + (translator_0.SubDirectoryName.Length > 0 
                ? translator_0.SubDirectoryName 
                : translator_0.OutputExtension));
            string targetDirectory = Directory.Exists(directory)
                ? directory
                : Directory.CreateDirectory(directory).FullName;

            document.BeginChanges(string.Format("Processing file: {0}", fileInfo.Name));
            ProcessingPages(document, targetDirectory);

            if (document.Changed)
            {
                document.EndChanges();
                document.Save();
                logFile.AppendLine("Document saved");
            }
            else
                document.CancelChanges();

            document.Close();
            logFile.AppendLine("Document closed");

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
        private string[] Groups(string expression)
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
        private string GetValue(Document document, Page page, string expression)
        {
            string result = null;
            string[] groups = Groups(expression);
            string[] argv;
            Variable variable;

            if (groups.Length > 1)
            {
                if (expression.Contains("page.type"))
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
        private string ParseExpression(Document document, Page page, string expression)
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

            return result = result ?? string.Empty;
        }

        /// <summary>
        /// Get output file name.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        /// <returns>Returns output file name.</returns>
        private string GetOutputFileName(Document document, Page page)
        {
            string fileName, expVal, pattern = @"\{(.*?)\}";

            if (translator_0.TemplateFileName.Length > 0 && !translator_0.UseDocumentNamed)
                fileName = translator_0.TemplateFileName.Replace(Environment.NewLine, "");
            else
            {
                fileName = Path.GetFileNameWithoutExtension(document.FileName);
                if (translator_0.FileNameSuffix.Length > 0)
                    fileName += ParseExpression(document, page, translator_0.FileNameSuffix);
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
        /// The Drawing Template exists.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private bool DrawingTemplateExists(Document document, Page page)
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
            uint[,] pt = new uint[,]
            {
                {
                    (uint)(translator_0.PageTypes.Normal ? 1 : 0),
                    (uint)PageType.Normal
                },
                {
                    (uint)(translator_0.PageTypes.Workplane ? 1 : 0),
                    (uint)PageType.Workplane
                },
                {
                    (uint)(translator_0.PageTypes.Auxiliary ? 1 : 0),
                    (uint)PageType.Auxiliary
                },
                {
                    (uint)(translator_0.PageTypes.Text ? 1 : 0),
                    (uint)PageType.Text
                },
                {
                    (uint)(translator_0.PageTypes.BillOfMaterials ? 1 : 0),
                    (uint)PageType.BillOfMaterials
                }
            };

            for (int i = 0; i < pt.GetLength(0); i++)
            {
                if (pt[i, 0] > 0 && pt[i, 1] == (uint)page.PageType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get pages on type.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        private List<Page> GetPagesOnType(Document document)
        {
            List<Page> pages = new List<Page>();

            foreach (var i in document.GetPages())
            {
                if (PageTypeExists(i) && (translator_0.PageNames.Count() > 0 ? translator_0.PageNames.Contains(i.Name) : true))
                    pages.Add(i);
            }
            return pages;
        }

        /// <summary>
        /// Extension method to processing pages.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="targetDirectory"></param>
        private void ProcessingPages(Document document, string targetDirectory)
        {
            int count = 0;
            uint flags;
            string path;
            RegenerateOptions ro;
            IEnumerable<Page> pages = GetPagesOnType(document);
            Dictionary<Page, string> processingPages = new Dictionary<Page, string>();

            foreach (var i in pages)
            {
                flags = 0x0000;

                flags |= (uint)(translator_0.PageNames.Contains(i.Name) ? 0x0001 : 0x0000);
                flags |= (uint)(translator_0.ExcludePage                ? 0x0002 : 0x0000);
                flags |= (uint)(PageTypeExists(i)                       ? 0x0004 : 0x0000);

                if (flags == 0x0000 || flags == 0x0007)
                    continue;

                if (translator_0.CheckDrawingTemplate && !DrawingTemplateExists(document, i))
                    continue;

                logFile.AppendLine(string.Format("Page name:\t{0}", i.Name));
                logFile.AppendLine(string.Format("Page type:\t{0}", i.PageType));
                //Debug.WriteLine(string.Format("Page name: {0}, flags: {1:X4}", i.Name, flags));

                if (i.Scale.Value != (double)translator_0.PageScale && translator_0.PageScale != 99999)
                {
                    i.Scale = new Parameter((double)translator_0.PageScale);

                    if (translator_0.SavePageScale)
                    {
                        ro = new RegenerateOptions
                        {
                            Full = true
                        };
                        document.Regenerate(ro);
                    }
                }

                if (translator_0.EnableProcessingOfProjections)
                {
                    ProcessingProjections(document, i.Name);
                }

                path = targetDirectory + "\\" + GetOutputFileName(document, i);

                if (pages.Where(p => p.PageType == i.PageType).Count() > 1)
                {
                    path += "_" + (count + 1).ToString() + "." + translator_0.OutputExtension.ToLower();
                    count++;
                }
                else
                    path += "." + translator_0.OutputExtension.ToLower();

                processingPages.Add(i, path);
            }

            switch (translator_t)
            {
                case TranslatorType.Acad:
                    translator_1.Export(document, processingPages, logFile);
                    break;
                case TranslatorType.Bitmap:
                    translator_3.Export(document, processingPages, logFile);
                    break;
                case TranslatorType.Pdf:
                    translator_9.Export(document, processingPages, logFile);
                    break;
            }
        }

        /// <summary>
        /// Extension method to processing projections.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="pageName"></param>
        private void ProcessingProjections(Document document, string pageName)
        {
            uint flags;
            double scale;
            RegenerateOptions ro;

            foreach (var i in document.GetProjections().Where(p => p.Page.Name == pageName))
            {
                flags = 0x0000;

                flags |= (uint)(translator_0.ProjectionNames.Contains(i.Name) ? 0x0001 : 0x0000);
                flags |= (uint)(translator_0.ExcludeProjection                ? 0x0002 : 0x0000);
                flags |= (uint)(translator_0.EnableProcessingOfProjections    ? 0x0004 : 0x0000);

                if (flags == 0x0000 || flags == 0x0007)
                    continue;

                logFile.AppendLine(string.Format("Projection name:{0}", i.Name));
                //Debug.WriteLine(string.Format("Projection name: {0}, flags: {1:X4}", i.Name, flags));

                if (i.Scale.Value != (double)translator_0.ProjectionScale)
                {
                    scale = translator_0.ProjectionScale == 99999 
                        ? Parameter.Default().Value 
                        : (double)translator_0.ProjectionScale;
                    i.Scale = new Parameter(scale);

                    if (translator_0.SaveProjectionScale)
                    {
                        ro = new RegenerateOptions
                        {
                            Projections = true
                        };
                        document.Regenerate(ro);
                    }
                }
            }
        }
        #endregion
    }
}