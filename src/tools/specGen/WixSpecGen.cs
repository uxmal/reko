using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Reko.Tools.specGen
{
    internal class WixSpecGen
    {
        private readonly string outDir;
        private readonly string inputFilePath;
        private readonly string pathPrefix;

        public WixSpecGen(string projectDir, string outDir, string targetFramework, string inputFilePath)
        {
            this.outDir = outDir.NormalizePath();
            this.inputFilePath = inputFilePath;
            this.pathPrefix = this.outDir.EnsureTrailingSlash();
        }

        public static readonly Dictionary<string, string> componentGuids = new Dictionary<string, string>()
        {
            { "", "9C3E003B-43CB-47B3-B5B8-DC6373A38AE1" },
            { "os2-16", "0444AAFF-1B7C-4182-AEEF-7DFCCDBC426C" },
            { "os2-32", "17698BD8-9701-44F7-8D05-F42F450334C6" },
            { "macos", "2D226794-2492-4E46-91C7-05B51E071D0D" },
        };

        private const string NAMESPACE = "http://schemas.microsoft.com/wix/2006/wi";

        private static XmlElement CreateComponent(XmlDocument dom, string dirName)
        {
            if (!componentGuids.ContainsKey(dirName))
            {
                Console.Error.WriteLine($"No component found for {dirName}");
                return null;
            }

            string nodeId;
            if (dirName == string.Empty)
            {
                nodeId = "ProductComponent";
            } else
            {
                nodeId = dirName.Replace('-', '_');
            }

            var node = dom.CreateElement("Component", NAMESPACE);
            node.SetAttribute("Id", nodeId);
            node.SetAttribute("Directory", $"{nodeId}Folder");
            node.SetAttribute("Guid", componentGuids[dirName]);
            return node;
        }

        private static XmlElement CreateFileElement(XmlDocument dom, string rela)
        {
            var tpl = @"$(var.OUTDIR)\{0}";
            var element = dom.CreateElement("File", NAMESPACE);
            element.SetAttribute("Source", string.Format(tpl, rela));

            var fileId = Regex.Replace(rela, @"[-/\\ ]", "_");
            element.SetAttribute("Id", $"File{fileId}");
            return element;
        }

        public void Generate()
        {
            var dom = new XmlDocument();
            using (var sr = new StreamReader(inputFilePath, new UTF8Encoding(false)))
            {
                dom.Load(sr);
            }
            var root = dom.DocumentElement;

            Console.Error.WriteLine($"Scanning files in '{outDir}'");

            var componentNodes = new Dictionary<string, XmlElement>();

            var files = Directory.GetFiles(outDir, "*", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                var relaPath = f.Replace(pathPrefix, "");
                var relaDir = Path.GetDirectoryName(f)
                                  .Replace(outDir, "")
                                  .TrimSlashes();

                if(!componentNodes.TryGetValue(relaDir, out XmlElement component))
                {
                    component = CreateComponent(dom, relaDir);
                    if(component is null)
                    {
                        continue;
                    }
                    componentNodes[relaDir] = component;

                    var componentRef = dom.CreateElement("ComponentRef", NAMESPACE);
                    componentRef.SetAttribute("Id", component.GetAttribute("Id"));
                }

                var fileNode = CreateFileElement(dom, relaPath);
                component.AppendChild(fileNode);
                component.AppendChild(dom.CreateTextNode(Environment.NewLine));
            }

            foreach(var node in componentNodes)
            {
                root.AppendChild(node.Value);
            }

            var outputPath = Path.Combine(
                Path.GetDirectoryName(inputFilePath),
                Path.GetFileNameWithoutExtension(inputFilePath)
            );
            Console.WriteLine($"Writing to '{outputPath}'");
            dom.Save(outputPath);
        }
    }
}