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

namespace Reko.Tools.regressionTests
{
	class Program
	{
        private const int MaxDeleteAttempts = 20;
        private const string EXE_NAME = "decompile";
        private static readonly string[] OUTPUT_EXTENSIONS = new string[] { ".asm", ".c", ".dis", ".h" };
        private static readonly string[] SOURCE_EXTENSIONS = new string[] { ".c" };

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

        private readonly List<string> dirs;
        private readonly SortedList<string, string> allResults;
        private readonly object resultLock;
        private string reko_cmdline_exe;
        private string subjects_dir;

        private int numCompleted = 0;
        private string reko_src;

        public Program()
        {
            this.dirs = new List<string>();
            this.allResults = new SortedList<string, string>();
            this.resultLock = new object();
        }
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
                    // first unparsed argument is the path to reko/src
                    if (unparsed_count++ == 0)
                    {
                        reko_src = arg;
                    } else
                    // any other argument is a list of directories to scan
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

                Console.Error.WriteLine($"{now}: Starting " + Path.GetRelativePath(subjects_dir, jobVirtualPath));
                Console.Error.WriteLine($"{workingDirectory} :> {reko_cmdline_exe} {argsString}");

                var proc = Process.Start(new ProcessStartInfo()
                {
                    FileName = reko_cmdline_exe,
                    Arguments = argsString,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true
                });
                var output = proc.StandardOutput.ReadToEnd();
                var relpath = Path.GetRelativePath(subjects_dir, jobVirtualPath);
                string testResult = FormatTestResult(jobName, relpath, output);

                Interlocked.Increment(ref this.numCompleted);
                lock (resultLock)
                {
                    allResults.Add(jobName + argsString, testResult);
                }
            });
        }

        private static string FormatTestResult(string jobName, string relpath, string output)
        {
            var bannerPrefix = "=== ";
            if (output.Contains("error", StringComparison.InvariantCultureIgnoreCase))
            {
                bannerPrefix = "*** ";
            }
            var testResult = $"{bannerPrefix}{relpath}{Environment.NewLine}{output}";
            return testResult;
        }

        /// <summary>
        /// Looks for subjects in the directory whose absolute path is <paramref name="dir"/>.
        /// </summary>
        /// <returns>The number of subjects found.</returns>
        private int HandleDir(string dir) 
        {
            int numJobs = 0;
            foreach (var subdir in Directory.EnumerateDirectories(dir))
            {
                numJobs += HandleDir(subdir);
            }

            // remove any .reko folder
            if (dir.EndsWith(".reko"))
            {
                AttemptDeleteDirectory(dir);
                return numJobs;
            }

            var dcProjects = Directory.GetFiles(dir).Where(f => f.EndsWith(".dcproject"));
            var subjectCmd = Path.Combine(dir, "subject.cmd");

            bool hasSubjectCmd = File.Exists(subjectCmd);

            // if there is no .dcproject and no subject.cmd, there's nothing to do
            if (dcProjects.Count() == 0 && !hasSubjectCmd)
            {
                return numJobs;
            }

            // clear output files outside of .reko (created by some .dcproject)
            ClearDir(dir);
            
            // queue all .dcproject for execution
            foreach (var proj in dcProjects)
            {
                AddJob(GetFilePrefix(proj), dir, proj);
                numJobs++;
            }

            if (hasSubjectCmd)
            {
                // read subject.cmd and skip all comments
                var lines = File.ReadAllLines(subjectCmd, new UTF8Encoding(false))
                    .Select(l => l.Trim())
                    .Where(l => !l.StartsWith('#'));

                foreach (var line in lines)
                {
                    string args = line;
                    if (line.StartsWith("decompile.exe"))
                    {
                        // remove the exe name from the arguments string
                        args = args.Remove(0, "decompile.exe".Length + 1);
                    }

                    if (!string.IsNullOrWhiteSpace(args))
                    {
                        // assume the last argument is going to be the name of the binary
                        string jobName = GetFilePrefix(
                            args
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Last()
                            .Trim('"')
                        );

                        // queue subject.cmd for execution
                        AddJob(jobName, dir, args);
                        numJobs++;
                    }
                }
            }
            return numJobs;
        }

        /// <summary>
        /// Attempt to delete a directory a few times until a maximal number
        /// of retries.
        /// </summary>
        /// <remarks>
        /// The regrettable reality of the universe is that antivirus programs, 
        /// Git, and various other processes could be sitting on the files we 
        /// wish to delete. Sometimes, good enough needs to precede perfect.
        /// </remarks>
        private void AttemptDeleteDirectory(string dir)
        {
            Exception ex = null;
            for (int i = 0; i < MaxDeleteAttempts; ++i)
            {
                try
                {
                    Directory.Delete(dir, true);
                    return;
                }
                catch (Exception e)
                {
                    ex = e;
                }
                Thread.Sleep(100);
            }
            Console.Error.WriteLine("*** Unable to delete directory {0} after {1} attempts. {2}", dir, MaxDeleteAttempts, ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
        }

        private int RunGitDiff()
        {
            var git = Process.Start(new ProcessStartInfo()
            {
                FileName = "git",
                Arguments = "diff --stat --exit-code",
                WorkingDirectory = reko_src,
                RedirectStandardOutput = true,
                // capture stderr, as we're going to silence it
                RedirectStandardError = true
            });

            // pipe stdout to stderr (to show it on-screen, since stdout is going to regression.log)
            git.StandardOutput.BaseStream.CopyTo(Console.OpenStandardError());

            git.WaitForExit();
            return git.ExitCode;
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

        private static bool IsInSourceDirectory(string file)
        {
            var dirname = Path.GetDirectoryName(file);
            var basename = Path.GetFileName(dirname);
            return basename.Equals("src", StringComparison.InvariantCultureIgnoreCase);
        }

        private void Run(string[] args)
        {
            ProcessArgs(args);

            string reko_cmdline_dir = Path.GetFullPath(Path.Combine(reko_src, "Drivers", "CmdLine"));

            subjects_dir = Path.GetFullPath(Path.Combine(reko_src, "..", "subjects"));

            string exeFileName = EXE_NAME;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                exeFileName += ".exe";
            }
            this.reko_cmdline_exe = Path.Combine(
                reko_cmdline_dir, "bin",
                this.platform, this.configuration, this.framework,
                exeFileName);

            // if no directory was specified, default to the subjects directory
            if (dirs.Count == 0)
            {
                dirs.Add(subjects_dir);
            }

            int numJobs = CollectJobs();

            // wait for queued jobs to complete
            while (numCompleted < numJobs)
            {
                Thread.Sleep(500);
            }

            // All jobs should have completed execution, display results.
            DisplayResults();

            if (strip_suffixes)
            {
                // collect the list of files to be stripped (starting from the project directory)
                var sources = dirs
                    .SelectMany(d => Directory.EnumerateFiles(d, "*.*", SearchOption.AllDirectories))
                    .Distinct()
                    .Where(f => !IsInSourceDirectory(f))
                    .Where(f => SOURCE_EXTENSIONS.Any(f.EndsWith));


                foreach (var f in sources)
                {
                    var text = File.ReadAllText(f, new UTF8Encoding(false));
                    text = Regex.Replace(text, @"(fn\w+)_(\d+)", "$1-$2");
                    text = Regex.Replace(text, @"(\w+)_\d+", "$1_n");
                    File.WriteAllText(f, text, new UTF8Encoding(false));
                }
            }

            if (check_output)
            {
                int exitCode = RunGitDiff();
                if(exitCode == 0){
                    Console.Error.WriteLine("Output files are the same as in repository");
                } else
                {
                    Console.Error.WriteLine("Output files differ from repository");
                }
                Environment.Exit(exitCode);
            }
        }

        private void DisplayResults()
        {
            foreach (var result in this.allResults.Values)
            {
                Console.WriteLine(result);
            }
        }
        private void Usage()
        {
            Console.Write(
@"Usage: regressionTests [options] [path_to_reko/src] [dirs...]

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
