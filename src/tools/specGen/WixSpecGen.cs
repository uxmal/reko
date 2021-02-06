using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Reko.Tools.specGen
{
    internal class WixSpecGen
    {
        private string projectDir;
        private string outDir;
        private string targetFramework;
        private string inputFilePath;

        private readonly string pathPrefix;

        public WixSpecGen(string projectDir, string outDir, string targetFramework, string inputFilePath)
        {
            this.projectDir = projectDir;
            this.outDir = outDir;
            this.targetFramework = targetFramework;
            this.inputFilePath = inputFilePath;

            this.pathPrefix = projectDir.NormalizePath().EnsureTrailingSlash();
        }

        private static XmlElement CreateFileElement(XmlDocument dom, string relaDir)
        {
            var tpl = @"$(var.WindowsDecompiler.ProjectDir)bin\$(var.Platform)\$(var.Configuration)\netcoreapp3.1\{0}";
            var element = dom.CreateElement("File");
            element.SetAttribute("Source", string.Format(tpl, relaDir));
            return element;
        }

        public void Generate()
        {
            var dom = new XmlDocument();
            var decl = dom.CreateXmlDeclaration("1.0", "UTF-8", null);
            var root = dom.DocumentElement;
            dom.InsertBefore(decl, root);

            var filesNode = dom.CreateElement("Include");
            filesNode.AppendChild(dom.CreateTextNode(Environment.NewLine));

            Console.Error.WriteLine($"Scanning files in '{outDir}'");

            var files = Directory.GetFiles(outDir, "*", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                var relaPath = f.Replace(pathPrefix, "");
                var relaDir = Path.GetDirectoryName(f)
                                  .Replace(outDir, "")
                                  .TrimSlashes();

                var extension = Path.GetExtension(f) ?? "";
                var basename = Path.GetFileName(f);

                var separator = (string.IsNullOrEmpty(relaDir)) ? "" : "/";

                var fileNode = CreateFileElement(dom, relaPath);
                filesNode.AppendChild(fileNode);
                filesNode.AppendChild(dom.CreateTextNode(Environment.NewLine));
            }

            dom.AppendChild(filesNode);

            var outputPath = Path.Combine(
                Path.GetDirectoryName(inputFilePath),
                Path.GetFileNameWithoutExtension(inputFilePath)
            );
            Console.WriteLine($"Writing to '{outputPath}'");
            dom.Save(outputPath);
        }
    }
}