using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Reko.Tools.specGen
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
            this.outDir = outDir.NormalizePath();
            this.targetFramework = targetFramework;
            this.nuspecTemplatePath = nuspecTemplatePath;

            this.pathPrefix = projectDir.NormalizePath().EnsureTrailingSlash();
        }

        public void Generate()
        {
            var dom = new XmlDocument();
            using (var sr = new StreamReader(nuspecTemplatePath, new UTF8Encoding(false)))
            {
                dom.Load(sr);
            }

            var root = dom.DocumentElement;
            Debug.Assert(root is not null);
            var filesNode = root.GetElementsByTagName("files").Item(0);
            if (filesNode is not null)
                root.RemoveChild(filesNode);

            filesNode = dom.CreateElement("files");
            filesNode.AppendChild(dom.CreateTextNode(Environment.NewLine));

            Console.Error.WriteLine($"Scanning files in '{outDir}'");

            var files = Directory.GetFiles(outDir, "*", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                var relaPath = f.Replace(pathPrefix, "");
                var relaDir = Path.GetDirectoryName(f)!
                                  .Replace(outDir, "")
                                  .TrimSlashes();

                var extension = Path.GetExtension(f) ?? "";
                var basename = Path.GetFileName(f);

                var separator = (string.IsNullOrEmpty(relaDir)) ? "" : "/";

                var targetPath = extension switch
                {
                    ".dll" => $"lib/{targetFramework}/{relaDir}{separator}{basename}",
                    _ => $"contentFiles/any/any/reko/{relaDir}{separator}{basename}"
                };
                Console.Error.WriteLine($"Adding '{relaPath}' -> '{targetPath}'");

                var fileNode = dom.CreateElement("file");
                fileNode.SetAttribute("src", relaPath);
                fileNode.SetAttribute("target", targetPath);

                filesNode.AppendChild(fileNode);
                filesNode.AppendChild(dom.CreateTextNode(Environment.NewLine));
            }

            root.AppendChild(filesNode);

            var nuspecOutputPath = Path.Combine(
                Path.GetDirectoryName(nuspecTemplatePath)!,
                Path.GetFileNameWithoutExtension(nuspecTemplatePath)
            );
            Console.WriteLine($"Writing to '{nuspecOutputPath}'");
            dom.Save(nuspecOutputPath);
        }
    }
}
