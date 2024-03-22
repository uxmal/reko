using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Reko.Tools.specGen
{
    /// <summary>
    /// This script is used to make sure the WiX installer spec and NuGet spec file are 
    /// in sync. It expects to be run in the $(REKO)/src/NuGetPackage 
    /// directory.
    ///
    /// The code is not a marvel of beauty: it manipulates XML using regular expressions,
    /// which already puts it in a state of sin. However it does the job correctly, so 
    /// there is no need to gold-plate it.
    /// </summary>

    public class ScriptGenerator
    {
        private const string netVersion = "net6.0";
        
        public static List<string> LoadComponentItems(TextReader f)
        {
            var items = new List<string>();
            var line = f.ReadLine();
            while (!(line is null))
            {
                var m = Regex.Match(line, ".*</Component>.*");
                if (m.Success)
                {
                    break;
                }
                items.Add(line);
                line = f.ReadLine();
            }
            return items;
        }

        // Load all the <var...> elements.
        public static Dictionary<string, string> LoadVarElements(TextReader f)
        {
            var vars = new Dictionary<string, string> { };
            var line = f.ReadLine();
            while (!(line is null))
            {
                var m = Regex.Match(line, ".*</vars>.*");
                if (m.Success)
                {
                    break;
                }
                m = Regex.Match(line, ".*<var id=\"(.*?)\" *>(.*)</var>.*");
                if (m.Success)
                {
                    vars[m.Groups[1].Value] = m.Groups[2].Value;
                }
                line = f.ReadLine();
            }
            return vars;
        }

        // Reads in all the file items and variables from the reko.xml file
        public static (Dictionary<string, string>, Dictionary<string, List<string>>) LoadRekoFileSpec(string filename)
        {
            var spec = new Dictionary<string, List<string>> { };
            var vars = new Dictionary<string, string> { };
            using (var f = File.OpenText(filename))
            {
                var line = f.ReadLine();
                while (!(line is null))
                {
                    var m = Regex.Match(line, ".*<Component Id=\"(.*?)\"");
                    if (m.Success)
                    {
                        var componentName = m.Groups[1].Value;
                        spec[componentName] = LoadComponentItems(f);
                    }
                    else
                    {
                        m = Regex.Match(line, ".*<vars>.*");
                        if (m.Success)
                        {
                            vars = LoadVarElements(f);
                        }
                    }
                    line = f.ReadLine();
                }
            }
            return (vars, spec);
        }

        // Collects all references to variables that are missing in the <vars> element.
        public static HashSet<string> CollectMissingVariables(
            Dictionary<string, string> mpVars,
            Dictionary<string, List<string>> mpSpec)
        {
            var missingVars = new HashSet<string>();
            foreach (var (k, v) in mpSpec)
            {
                foreach (var item in v)
                {
                    var m = Regex.Match(item, @".*\$\((var.*)\).*");
                    if (m.Success)
                    {
                        var var = m.Groups[1].Value;
                        if (!mpVars.ContainsKey(var))
                        {
                            missingVars.Add(var);
                        }
                    }
                }
            }
            return missingVars;
        }

        private static void PrintMissingVariables(IEnumerable<string> vars)
        {
            foreach (var var in vars.OrderBy(v => v))
            {
                Console.WriteLine($"<var id=\"{var}\">@@@</var>");
            }
        }

        // Validates that all the files referred to by the `vars` dictionary
        // exist in the filesystem.
        public static bool ValidateVariables(string configuration, string platform, Dictionary<string, string> vars, string solutionDir)
        {
            var filesMissing = false;
            foreach (var (k, vv) in vars)
            {
                if (vv.Contains("x64"))
                    Console.WriteLine("****** {0}", vv);
                    Console.WriteLine("   platform: {0}", platform);
                var v = vv;
                v = v.Replace("$TargetDir$", $"bin/$Configuration$/{netVersion}");
                v = v.Replace("$TargetFwkDir$", $"bin/$Configuration$/{netVersion}");
                v = v.Replace("$TargetNetWinForms$", $"bin/$Configuration$/{netVersion}-windows");
                v = v.Replace("$TargetDir_x64$", $"bin/$Platform$/$Configuration$/{netVersion}");
                v = v.Replace("$Configuration$", configuration);
                v = v.Replace("$Platform$", platform);
                v = v.Replace("$SolutionDir$", solutionDir);
                if (v.EndsWith('/'))
                {
                    if (!Directory.Exists(v))
                    {
                        Console.WriteLine("Missing dir:  " + v);
                        filesMissing = true;
                    }
                }
                else if (!File.Exists(v))
                {
                    Console.WriteLine("Missing file: " + v);
                    filesMissing = true;
                }
            }
            return filesMissing;
        }

        // Replace macros inside of the variable definitions. Macros have the syntax
        // $macroname$. 
        // Only a set of well-known macros are replaced.
        public static Dictionary<string, string> ExpandVariables(string configuration, string platform, string solutionDir, Dictionary<string, string> vars)
        {
            var result = new Dictionary<string, string> { };
            foreach (var (k, vv) in vars)
            {
                var v = vv;
                v = v.Replace("$TargetDir$", $"bin/$Configuration$/{netVersion}");
                v = v.Replace("$TargetFwkDir$", $"bin/$Configuration$/{netVersion}");
                v = v.Replace("$TargetFwkWindowsDir$", $"bin/$Configuration$/{netVersion}-windows");
                v = v.Replace("$TargetFwkWindowsDir_x64$", $"bin/$Platform$/$Configuration$/{netVersion}-windows");
                v = v.Replace("$TargetDir_x64$", $"bin/$Platform$/$Configuration$/{netVersion}");
                v = v.Replace("$Configuration$", configuration);
                v = v.Replace("$Platform$", platform);
                v = v.Replace("$SolutionDir$", solutionDir);
                result[k] = v;
            }
            return result;
        }

        // Parses a single line into attribute-value pairs
        public static Dictionary<string, string> ParseLineAttributes(string line)
        {
            var re = new Regex(" ([a-zA-Z].+?)=\"(.*?)\"");
            return re.Matches(line)
                .Cast<Match>()
                .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value);
        }

        // For each given source line, parses it, extracts attributes, and either
        // copies it verbatim to the output if there were no relevant attributes, or generate a 
        // WiX <File... element and writes it to the output.
        public static void InjectItemIntoWixFile(IEnumerable<string> items, Dictionary<string,string> vars, TextWriter output)
        {
            foreach (var item in items)
            {
                var attrs = ParseLineAttributes(item);
                var wix_item = RenderWixLine(attrs, vars);
                if (!(wix_item is null))
                {
                    output.WriteLine(wix_item);
                }
                else
                {
                    output.WriteLine(item);
                }
            }
        }

        // Locates the <Component...> elements in the WiX file template, and copies
        // all items into that element.
        public static void InjectFileItemsIntoWixFile(Dictionary<string, List<string>> spec, Dictionary<string,string> vars,  string templateFile, string targetFile)
        {
            var reComponent = new Regex(".*<Component Id=\"(.*?)\".*");
            using var input = File.OpenText(templateFile);
            using var output = File.CreateText(targetFile);
            var line = input.ReadLine();
            while (!(line is null))
            {
                output.WriteLine(line);
                var m = reComponent.Match(line);
                if (m.Success && spec.ContainsKey(m.Groups[1].Value))
                {
                    output.WriteLine("<!-- Caution! Do not edit these values, they are generated automatically");
                    output.WriteLine("     by a script. Edit reko-files.xml instead. -->");
                    InjectItemIntoWixFile(spec[m.Groups[1].Value], vars, output);
                }
                line = input.ReadLine();
            }
        }

        // Generates NuSpec <file...> elements from the `spec` items.
        public static void InjectItemsIntoNuspecFilesElement(
            Dictionary<string, List<string>> spec,
            Dictionary<string, string> vars,
            TextWriter output)
        {
            foreach (var comp in new List<string> {
            "ProductComponent",
            "Comp_Os2_16",
            "Comp_Os2_32"
        })
            {
                foreach (var line in spec[comp])
                {
                    var attrs = ParseLineAttributes(line);
                    var newLine = RenderNuspecLine(attrs, vars);
                    if (!(newLine is null))
                    {
                        output.WriteLine(newLine);
                    }
                }
            }
        }

        // Locate the <files> element of the nuspec file template, and copies all
        // the items into that element.
        public static void InjectFilesIntoNuspecFile(
            Dictionary<string, List<string>> spec,
            Dictionary<string, string> vars,
            string templateFile,
            string targetFile)
        {
            var reFiles = new Regex(".*<files>.*");
            using var input = File.OpenText(templateFile);
            using var output = File.CreateText(targetFile);
            var line = input.ReadLine();
            while (!(line is null))
            {
                output.WriteLine(line);
                var m = reFiles.Match(line);
                if (m.Success)
                {
                    output.WriteLine("<!-- Caution! Do not edit these values, they are generated automatically");
                    output.WriteLine("     by a script. Edit reko-files.xml instead. -->");
                    InjectItemsIntoNuspecFilesElement(spec, vars, output);
                }
                line = input.ReadLine();
            }
        }

        // Renders a WiX <File... element if there is a 'Source' attribute present.
        public static string RenderWixLine(Dictionary<string, string> attrs, Dictionary<string,string> vars)
        {
            if (attrs.ContainsKey("Source"))
            {
                var line = new StringBuilder();
                line.AppendFormat("<File Source=\"{0}\"", InterpolateVariables(attrs["Source"], vars));
                if (attrs.ContainsKey("Id"))
                {
                    line.AppendFormat(" Id=\"{0}\"", attrs["Id"]);
                }
                if (attrs.ContainsKey("Name"))
                {
                    line.AppendFormat(" Name=\"{0}\"", attrs["Name"]);
                }
                line.AppendLine(" />");
                return line.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Given a string, interpolates all $(varname) macros with their values from 
        /// the `vars` dictionary.
        /// No attempt is made to make this code robust in corner cases. YAGNI.
        /// </summary>
        public static string InterpolateVariables(string sWithVariables, Dictionary<string, string> vars)
        {
            var s = "";
            var sawDollar = false;
            var insideVariable = false;
            var var = "";
            foreach (var c in sWithVariables)
            {
                if (c == '$')
                {
                    // Let's assume no '$$' are present.
                    sawDollar = true;
                }
                else if (c == '(')
                {
                    insideVariable = sawDollar;
                    sawDollar = false;
                }
                else if (c == ')')
                {
                    insideVariable = false;
                    sawDollar = false;
                    if (vars.ContainsKey(var))
                    {
                        s += vars[var];
                        var = "";
                    }
                    else
                    {
                        throw new InvalidOperationException($"Variable '{var}' in '{sWithVariables}' is not defined.");
                    }
                }
                else
                {
                    if (insideVariable)
                    {
                        var += c;
                    }
                    else if (sawDollar)
                    {
                        s += '$';
                        s += c;
                    }
                    else
                    {
                        s += c;
                    }
                    sawDollar = false;
                }
            }
            return s;
        }

        // Renders a NuGet <file... element as a single line if the 'nuget_target' attribute
        // is present.
        public static string RenderNuspecLine(
            Dictionary<string, string> attrs,
            Dictionary<string, string> vars)
        {
            if (attrs.ContainsKey("nuget_target"))
            {
                var source = InterpolateVariables(attrs["Source"], vars);
                var target = attrs["nuget_target"];
                target = target.Replace("f:", "lib\\" + netVersion);
                target = target.Replace("c:", "contentFiles/any/any/reko");
                target = target.Replace("i:", "images\\");
                target = target.Replace("d:", "docs\\");
                var line = $"<file src=\"{source}\" target=\"{target}\" />";
                return line;
            }
            else
            {
                return null;
            }
        }

        // Generates a WiX installer script and a NuGet spec file from the controlling
        // file 'reko-files.xml'
        public static void UpdateWixFile(string configuration, string platform, string masterSpecFile, string template, string generatedFile, string solutionDir)
        {
            var (vars, spec) = LoadSpecification(configuration, platform, masterSpecFile, solutionDir);
            InjectFileItemsIntoWixFile(spec, vars, template, generatedFile);
        }

        public static void UpdateNugetFile(string configuration, string platmform, string masterSpecFile, string template, string generatedFile, string solutionDir)
        {
            var (vars, spec) = LoadSpecification(configuration, platmform, masterSpecFile, solutionDir);
            InjectFilesIntoNuspecFile(spec, vars, template, generatedFile);
        }

        private static (Dictionary<string,string>, Dictionary<string, List<string>> spec) LoadSpecification(string configuration, string platform, string specPath, string solutionDir)
        {
            var (vars, spec) = LoadRekoFileSpec(specPath);
            var missing_vars = CollectMissingVariables(vars, spec);
            //if (missing_vars.Count > 0)
            //{
            //    Console.WriteLine("Missing variables");
            //    PrintMissingVariables(missing_vars);
            //    Environment.Exit(-1);
            //}
            //if (ValidateVariables(configuration, vars, solutionDir))
            //{
            //    Console.WriteLine("Some files are missing. Stopping.");
            //    Environment.Exit(-1);
            //}
            //$TODO: inject platform as a variable from command line.
            vars.Add("var.Platform", platform);
            vars.Add("var.Configuration", configuration);
            vars = ExpandVariables(configuration, platform, solutionDir, vars);
            return (vars, spec);
        }
    }
}