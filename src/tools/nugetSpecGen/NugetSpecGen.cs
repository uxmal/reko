using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Reko.Tools.nugetSpecGen
{
    public class NugetSpecGen
    {
        private readonly string pathPrefix;

        private readonly string outDir;
        private readonly string targetFramework;
        private readonly string nuspecTemplatePath;

        public NugetSpecGen(
            string projectDir, string outDir, string targetFramework, string nuspecTemplatePath
        ){
            this.outDir = Path.Combine(projectDir, outDir.TrimEnd('/', '\\'));
            this.targetFramework = targetFramework;
            this.nuspecTemplatePath = nuspecTemplatePath;

            this.pathPrefix = projectDir;
        }

        public void Generate()
        {
            var dom = new XmlDocument();
            using (var sr = new StreamReader(nuspecTemplatePath, new UTF8Encoding(false)))
            {
                dom.Load(sr);
            }

            var filesNode = dom.DocumentElement.GetElementsByTagName("files").Item(0);
            dom.DocumentElement.RemoveChild(filesNode);

            filesNode = dom.CreateElement("files");
            filesNode.AppendChild(dom.CreateTextNode(Environment.NewLine));

            var files = Directory.GetFiles(outDir, "*", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                var relaPath = f.Replace(pathPrefix, "");
                var relaDir = Path.GetDirectoryName(f).Replace(outDir, "").TrimStart('/', '\\');

                var extension = Path.GetExtension(f) ?? "";
                var basename = Path.GetFileName(f);

                var separator = (string.IsNullOrEmpty(relaDir)) ? "" : "/";

                var targetPath = extension switch
                {
                    ".dll" => $"lib/{targetFramework}/{relaDir}{separator}{basename}",
                    _ => $"contentFiles/any/any/reko/{relaDir}{separator}{basename}"
                };

                var fileNode = dom.CreateElement("file");
                fileNode.SetAttribute("src", relaPath);
                fileNode.SetAttribute("target", targetPath);

                filesNode.AppendChild(fileNode);
                filesNode.AppendChild(dom.CreateTextNode(Environment.NewLine));
            }

            dom.DocumentElement.AppendChild(filesNode);

            var nuspecOutputPath = Path.GetFileNameWithoutExtension(nuspecTemplatePath);
            dom.Save(nuspecOutputPath);
        }
    }
}
