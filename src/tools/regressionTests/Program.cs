using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;

namespace regressionTests
{
	class Program
	{
        bool TryTake(IEnumerator<string> it, out string? arg)
        {
            if (!it.MoveNext())
            {
                arg = null;
                return false;
            }

            arg = it.Current;
            return true;
        }

        private string configuration = "Debug";
        private bool check_output = false;
        private string framework = "netcoreapp3.1";
        private bool strip_suffixes = true;
        private string platform = "x64";

        private List<string> dirs = new List<string>();
        private string reko_cmdline_exe;
        private string script_dir;

        private int numCompleted = 0;
        private string reko_src;

        private void ProcessArgs(IEnumerable<string> args)
        {
            int unparsed_count = 0;

            var it = args.GetEnumerator();
            while (it.MoveNext())
            {
                bool res = true;
                string arg = it.Current;

                switch (arg)
                {
                case "-c":
                case "--configuration":
                    res = TryTake(it, out configuration);
                    break;
                case "-o":
                case "--check-output":
                    check_output = true;
                    break;
                case "-p":
                case "--platform":
                    res = TryTake(it, out platform);
                    break;
                case "-f":
                case "--framework":
                    res = TryTake(it, out framework);
                    break;
                case "--strip-suffixes":
                    res = TryTake(it, out string opt);
                    strip_suffixes = (opt == "yes");
                    break;
                default:
                    if (unparsed_count++ == 0)
                    {
                        reko_src = arg;
                    } else
                    {
                        dirs.Add(arg);
                    }
                    break;
                }

                if (!res)
                {
                    Usage();
                    Environment.Exit(1);
                }
            }
        }

        private const string EXE_NAME = "decompile";
        private readonly string[] OUTPUT_EXTENSIONS = new string[] { ".asm", ".c", ".dis", ".h" };
        private readonly string[] SOURCE_EXTENSIONS = new string[] { ".c" };

        private void ClearDir(string dir)
        {
            var failed = new List<string>();

            foreach(var f in Directory.EnumerateFiles(dir).Where(f => OUTPUT_EXTENSIONS.Any(f.EndsWith)))
            {
                try
                {
                    File.Delete(f);
                } catch(IOException)
                {
                    failed.Add(f);
                }
            }

            if(failed.Count > 0)
            {
                Thread.Sleep(2000);
                foreach(var f in failed)
                {
                    File.Delete(f);
                }
            }

        }

        private static string GetFilePrefix(string path)
        {
            // remove extension
            path = Regex.Replace(path, @"\..*$", "");

            // remove any previous path component
            int pos = path.LastIndexOf(Path.DirectorySeparatorChar);
            if(pos > -1)
            {
                path = path.Substring(pos + 1);
            }
            return path;
        }

        private void AddJob(string jobName, string workingDirectory, string argsString)
        {
            ThreadPool.QueueUserWorkItem((x) =>
            {
                var now = DateTime.Now.ToString("HH:MM:ss.f");
                string jobVirtualPath = Path.Combine(workingDirectory, jobName);
                
                Console.Error.WriteLine($"{now}: Starting " + Path.GetRelativePath(script_dir, jobVirtualPath));
                Console.Error.WriteLine($"{workingDirectory} :> {reko_cmdline_exe} {argsString}");
                var proc = Process.Start(new ProcessStartInfo()
                {
                    FileName = reko_cmdline_exe,
                    Arguments = argsString,
                    WorkingDirectory = workingDirectory
                });

                proc.WaitForExit();
                Interlocked.Increment(ref this.numCompleted);
            });
        }

        private int HandleDir(string dir)
        {
            int numJobs = 0;

            foreach (var subdir in Directory.EnumerateDirectories(dir))
            {
                numJobs += HandleDir(subdir);
            }

            if (dir.EndsWith(".reko"))
            {
                Directory.Delete(dir, true);
                return numJobs;
            }

            var dcProjects = Directory.GetFiles(dir).Where(f => f.EndsWith(".dcproject"));
            var subjectCmd = Path.Combine(dir, "subject.cmd");

            bool hasSubjectCmd = File.Exists(subjectCmd);

            if (dcProjects.Count() == 0 && !hasSubjectCmd)
            {
                return numJobs;
            }

            ClearDir(dir);
            
            foreach(var proj in dcProjects)
            {
                AddJob(GetFilePrefix(proj), dir, proj);
                numJobs++;
            }

            if (hasSubjectCmd)
            {
                var lines = File.ReadAllLines(subjectCmd, new UTF8Encoding(false))
                    .Select(l => l.Trim())
                    .Where(l => !l.StartsWith('#'));

                foreach(var line in lines)
                {
                    string args = line;
                    if (line.StartsWith("decompile.exe"))
                    {
                        args = args.Remove(0, "decompile.exe".Length + 1);
                    }
                    if (!string.IsNullOrWhiteSpace(args))
                    {
                        string jobName = GetFilePrefix(
                            args
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Last()
                            .Trim('"')
                        );

                        AddJob(jobName, dir, args);
                        numJobs++;
                    }
                }
            }

            return numJobs;
        }

        private int CollectJobs()
        {
            int numJobs = 0;
            foreach(var dir in dirs)
            {
                numJobs += HandleDir(dir);
            }
            return numJobs;
        }

        private void Run(string[] args)
        {
            ProcessArgs(args);

            string reko_cmdline_dir = Path.GetFullPath(Path.Combine(reko_src, "Drivers", "CmdLine"));

            script_dir = Path.GetFullPath(Path.Combine(reko_src, "..", "subjects"));

            string exeFileName = EXE_NAME;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                exeFileName += ".exe";
            }
            this.reko_cmdline_exe = Path.Combine(
                reko_cmdline_dir, "bin",
                this.platform, this.configuration, this.framework,
                exeFileName);

            if (dirs.Count == 0)
            {
                dirs.Add(script_dir);
            }

            int numJobs = CollectJobs();
            while(numCompleted < numJobs)
            {
                Thread.Sleep(500);
            }

            if (strip_suffixes)
            {
                var sources = dirs
                    .SelectMany(d => Directory.EnumerateFiles(d, "*.*", SearchOption.AllDirectories))
                    .Distinct()
                    .Where(f => SOURCE_EXTENSIONS.Any(f.EndsWith));

                foreach (var f in sources)
                {
                    var text = File.ReadAllText(f, new UTF8Encoding(false));
                    text = Regex.Replace(text, @"(fn\w+)_(\d+)", "$1-$2");
                    text = Regex.Replace(text, @"(\w+)_\d+", "$1_n");
                    File.WriteAllText(f, text, new UTF8Encoding(false));
                }
            }
        }

        private void Usage()
        {
            Console.Write(
@"Usage: regressionTests [options]

Options:
  -h, --help            show this help message and exit
  -c CONFIGURATION, --configuration=CONFIGURATION
                        define configuration (Debug, Release, etc.)
  -o, --check-output    check output files
  -p PLATFORM, --platform=PLATFORM
                        define platform (x86, x64)
  -f FRAMEWORK, --framework=FRAMEWORK
                        define .NET framework (netcoreapp3.1)
  --strip-suffixes=STRIP_SUFFIXES
                        strip number suffixes from SSA identifiers (yes, no)
");
        }

        static void Main(string[] args)
        {
            new Program().Run(args);
		}
	}
}
