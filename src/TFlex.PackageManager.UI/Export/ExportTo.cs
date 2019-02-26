using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TFlex.Model.Model3D;
using TFlex.Model;
using TFlex.PackageManager.Configuration;

namespace TFlex.PackageManager.Export
{
    /// <summary>
    /// The ExportTo base class implements the TFlexAPI extension 
    /// to perform the export to packages.
    /// </summary>
    public class ExportTo : Package
    {
        #region private fields
        private readonly Header header;
        private int countExportFiles;
        #endregion

        public ExportTo(Header header)
        {
            this.header = header;
        }

        #region internal properties
        /// <summary>
        /// Total count of exported files.
        /// </summary>
        internal int CountExportFiles
        {
            get
            {
                return (countExportFiles);
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// The export method.
        /// </summary>
        /// <param name="path"></param>
        public void Export(string path)
        {
            ProcessingFile(path);
        }

        /// <summary>
        /// The Export virtual method.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="page"></param>
        /// <param name="path">Full path file name.</param>
        public virtual bool Export(Document document, Page page, string path)
        {
            return false;
        }
        #endregion

        #region private methods
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
                    if ((argv = groups[1].Split(',')).Length < 2) return result;

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

        private string ExpressionParse(Document document, Page page, string expression)
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

        private string GetOutputFileName(Document document, Page page)
        {
            string fileName, expVal, pattern = @"\{(.*?)\}";

            if (TemplateFileName.Length > 0 && !UseDocumentNamed)
                fileName = TemplateFileName.Replace(Environment.NewLine, "");
            else
            {
                fileName = Path.GetFileNameWithoutExtension(document.FileName);
                if (FileNameSuffix.Length > 0)
                    fileName += ExpressionParse(document, page, FileNameSuffix);
                return fileName;
            }

            foreach (Match i in Regex.Matches(fileName, pattern))
            {
                if ((expVal = ExpressionParse(document, page, i.Groups[1].Value)) == null)
                    continue;

                fileName = fileName.Replace(i.Groups[0].Value, expVal);
            }

            return fileName;
        }

        private void ProcessingFile(string path)
        {
            Document document = Application.OpenDocument(path, false);
            FileInfo fileInfo = new FileInfo(path);
            string directory = fileInfo.Directory.FullName.Replace(
                header.InitialCatalog, 
                header.TargetDirectory + "\\" + (SubDirectoryName.Length > 0 ? SubDirectoryName : OutputExtension));
            string targetDirectory = Directory.Exists(directory)
                ? directory
                : Directory.CreateDirectory(directory).FullName;

            document.BeginChanges(string.Format("Processing file: {0}", fileInfo.Name));
            ProcessingPages(document, targetDirectory);

            if (document.Changed)
            {
                document.EndChanges();
                document.Save();
            }
            else
                document.CancelChanges();

            document.Close();

            if (Directory.GetFiles(targetDirectory).Length == 0 &&
                Directory.GetDirectories(targetDirectory).Length == 0)
                Directory.Delete(targetDirectory, false);
        }

        private void ProcessingPages(Document document, string targetDirectory)
        {
            RegenerateOptions options;
            int n1, n2, n3, count = 0;
            uint num;
            string path;
            IEnumerable<Page> pages = GetPagesOnType(document);

            foreach (var i in pages)
            {
                n1 = PageNames.Contains(i.Name) ? 1 : 0;
                n2 = ExcludePage ? 1 : 0;
                n3 = CheckPageType(i) ? 1 : 0;
                num = Convert.ToUInt32(Convert.ToUInt32(
                    n1.ToString() + 
                    n2.ToString() + 
                    n3.ToString(), 2).ToString("X"), 16);
                
                if (!(num == 1 || num == 5)) continue;

                if (CheckDrawingTemplate && !IsValidDrawingTemplate(document, i)) return;

                if (i.Scale.Value != (double)PageScale && PageScale != 99999)
                {
                    i.Scale = new Parameter((double)PageScale);

                    if (SavePageScale)
                    {
                        options = new RegenerateOptions
                        {
                            Full = true
                        };
                        document.Regenerate(options);
                    }
                }

                if (EnableProcessingOfProjections) ProcessingProjections(document, i.Name);

                path = targetDirectory + "\\" + GetOutputFileName(document, i);
                
                if (pages.Where(p => p.PageType == i.PageType).Count() > 1)
                {
                    path += "_" + (count + 1).ToString() + "." + OutputExtension.ToLower();
                    count++;
                }
                else
                    path += "." + OutputExtension.ToLower();

                Export(document, i, path);
                countExportFiles++;
            }
        }

        private bool IsValidDrawingTemplate(Document document, Page page)
        {
            if (document.GetFragments().Where(
                f => f.GroupType == ObjectType.Fragment && f.Page == page && 
                f.DisplayName.Contains("<Форматки>")).FirstOrDefault() != null)
                return true;
            
            return false;
        }

        private IEnumerable<Page> GetPagesOnType(Document document)
        {
            List<Page> pages = new List<Page>();

            foreach (var i in document.GetPages())
            {
                if (CheckPageType(i) && (PageNames.Count() > 0 ? PageNames.Contains(i.Name) : true))
                    pages.Add(i);
            }
            return pages;
        }

        private bool CheckPageType(Page page)
        {
            bool result = false;
            uint[,] pageTypes = new uint[,]
            {
                {
                    (uint)(PageTypes.Normal ? 1 : 0),
                    (uint)PageType.Normal
                },
                {
                    (uint)(PageTypes.Workplane ? 1 : 0),
                    (uint)PageType.Workplane
                },
                {
                    (uint)(PageTypes.Auxiliary ? 1 : 0),
                    (uint)PageType.Auxiliary
                },
                {
                    (uint)(PageTypes.Text ? 1 : 0),
                    (uint)PageType.Text
                },
                {
                    (uint)(PageTypes.BillOfMaterials ? 1 : 0),
                    (uint)PageType.BillOfMaterials
                }
            };

            for (int i = 0; i < pageTypes.GetLength(0); i++)
            {
                if (pageTypes[i, 0] > 0 && pageTypes[i, 1] == (uint)page.PageType)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private void ProcessingProjections(Document document, string pageName)
        {
            RegenerateOptions options;
            int n1, n2, n3;
            uint num;
            double scale;

            foreach (var i in document.GetProjections().Where(p => p.Page.Name == pageName))
            {
                n1 = ProjectionNames.Contains(i.Name) ? 1 : 0;
                n2 = ExcludeProjection ? 1 : 0;
                n3 = EnableProcessingOfProjections ? 1 : 0;
                num = Convert.ToUInt32(Convert.ToUInt32(
                    n1.ToString() + 
                    n2.ToString() + 
                    n3.ToString(), 2).ToString("X"), 16);

                if (!(num == 1 || num == 5)) continue;

                if (i.Scale.Value != (double)ProjectionScale)
                {
                    scale = ProjectionScale == 99999 ? Parameter.Default().Value : (double)ProjectionScale;
                    i.Scale = new Parameter(scale);

                    if (SaveProjectionScale)
                    {
                        options = new RegenerateOptions
                        {
                            Projections = true
                        };
                        document.Regenerate(options);
                    }
                }
            }
        }
        #endregion
    }
}