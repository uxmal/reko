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
    /// <summary>
    /// This program runs the Reko regression tests.
    /// </summary>
	class Program
    {
        private const int MaxDeleteAttempts = 5;
        private const string DecompilerExecutableName = "reko";
        private static readonly string[] OUTPUT_EXTENSIONS = new string[] { ".asm", ".c", ".dis", ".h" };
        private static readonly string[] SOURCE_EXTENSIONS = new string[] { ".c" };
        private const string WeightsFilename = "subject_weights.txt";

        private string configuration = "Debug";
        private bool checkOutput = false;
        private string framework = "net8.0";
        private bool stripSuffixes = true;
        private string platform = "x64";

        private readonly List<string> dirs;
        private readonly SortedList<string, string> allResults;
        private readonly Dictionary<string, TimeSpan> executionTimes;
        private CountdownEvent? jobsRemaining;
        private readonly object resultLock;
        private string reko_src;
        private string reko_cmdline_exe;
        private string subjects_dir;
        private bool verbose;

        static void Main(string[] args)
        {
            new Program().Run(args);
        }

        public Program()
        {
            this.dirs = new List<string>();
            this.allResults = new SortedList<string, string>();
            this.executionTimes = new Dictionary<string, TimeSpan>();
            this.resultLock = new object();
            this.reko_src = "";
            this.reko_cmdline_exe = "";
            this.subjects_dir = "";
        }

        private void Run(string[] args)
        {
            ProcessArgs(args);

            subjects_dir = Path.GetFullPath(Path.Combine(reko_src, "..", "subjects"));
            this.reko_cmdline_exe = DetermineDecompilerPath();

            // if no directory was specified, default to the subjects directory
            if (dirs.Count == 0)
            {
                // if no directory was specified, default to the subjects directory
                dirs.Add(subjects_dir);
            }

            /**
             * .NET appears to be using up to 2^15 workers by default, which translates into too many concurrent reko processes
             * (and a potential crash of the CI runner).
             * change the limit to use the number of available processors instead.
             */
            ThreadPool.GetAvailableThreads(out _, out var numCompletionPorts);
            ThreadPool.SetMaxThreads(Environment.ProcessorCount, numCompletionPorts);

            var watch = new Stopwatch();
            watch.Start();
            var jobs = CollectJobs(dirs);
            ExecuteJobs(jobs);
            watch.Stop();
            DisplayResults(jobs.Count, watch.Elapsed);

            if (stripSuffixes)
            {
                StripSuffixes(dirs);
            }

            if (checkOutput)
            {
                int exitCode = RunGitDiff();
                if (exitCode == 0)
                {
                    Console.WriteLine("Output files are the same as in repository");
                }
                else
                {
                    Console.WriteLine("Output files differ from repository");
                }
                Environment.Exit(exitCode);
            }
        }

        private string DetermineDecompilerPath()
        {
            string reko_cmdline_dir = Path.GetFullPath(Path.Combine(reko_src, "Drivers", "CmdLine"));
            string exeFileName = DecompilerExecutableName;
            if (File.Exists(exeFileName + ".exe"))
            {
                exeFileName += ".exe";
            }
            else
            {
                exeFileName += ".dll";
            }
            var s = Path.Combine(
                reko_cmdline_dir, "bin",
                this.platform, this.configuration, this.framework,
                exeFileName);
            return s;
        }

        private bool TryTake(IEnumerator<string> it, out string? arg)
        {
            if (!it.MoveNext())
            {
                arg = null;
                return false;
            }
            arg = it.Current;
            return true;
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
                    res = TryTake(it, out configuration!);
                    break;
                case "-o":
                case "--check-output":
                    checkOutput = true;
                    break;
                case "-p":
                case "--platform":
                    res = TryTake(it, out platform!);
                    break;
                case "-f":
                case "--framework":
                    res = TryTake(it, out framework!);
                    break;
                case "--strip-suffixes":
                    res = TryTake(it, out string? opt);
                    stripSuffixes = (opt == "yes");
                    break;
                case "-v":
                case "--verbose":
                    this.verbose = true;
                    break;
                default:
                    // first unparsed argument is the path to reko/src
                    if (unparsed_count++ == 0)
                    {
                        reko_src = arg;
                    }
                    else
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

            foreach (var f in Directory.EnumerateFiles(dir).Where(f => OUTPUT_EXTENSIONS.Any(f.EndsWith)))
            {
                try
                {
                    File.Delete(f);
                }
                catch (IOException)
                {
                    failed.Add(f);
                }
            }

            if (failed.Count > 0)
            {
                Thread.Sleep(2000);
                foreach (var f in failed)
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
            if (pos > -1)
            {
                path = path.Substring(pos + 1);
            }
            return path;
        }

        private static (string, string) GetPlatformInvocationCmdLine(string exeName, string argsString)
        {
            if (Path.GetExtension(exeName) == ".dll")
            {
                return ("dotnet", $"{exeName} {argsString}");
            }
            return (exeName, argsString);
        }

        public class Job
        {
            public Job(string name, string workdir, string args)
            {
                this.Name = name;
                this.WorkingDirectory = workdir;
                this.Arguments = args;
                this.Key = this.Name + this.Arguments;
            }

            public string Name { get; }
            public string WorkingDirectory { get; }
            public string Arguments { get; }
            public string Key { get; }
        }

        /// <summary>
        /// Enqueue the job in the thread pool.
        /// </summary>
        /// <param name="job"></param>
        private void StartJob(Job job)
        {
            (string, string) cmdline = GetPlatformInvocationCmdLine(reko_cmdline_exe, job.Arguments);
            ThreadPool.QueueUserWorkItem(x =>
            {
                var now = DateTime.Now;
                var sNow = now.ToString("HH:MM:ss.f");
                string jobVirtualPath = Path.Combine(job.WorkingDirectory, job.Name);

                Console.Error.WriteLine($"{sNow}: Starting " + Path.GetRelativePath(subjects_dir, jobVirtualPath));
                if (verbose)
                {
                    Console.Error.WriteLine($"{job.WorkingDirectory} :> {reko_cmdline_exe} {job.Arguments}");
                }
                string output;
                try
                {
                    var proc = Process.Start(new ProcessStartInfo()
                    {
                        FileName = cmdline.Item1,
                        Arguments = cmdline.Item2,
                        WorkingDirectory = job.WorkingDirectory,
                        RedirectStandardOutput = true
                    });
                    if (proc is not null)
                    {
                        output = proc.StandardOutput.ReadToEnd();
                    }
                    else 
                    {
                        output = $"*** Didn't start {cmdline.Item1} {cmdline.Item2}." + Environment.NewLine;
                    }
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("== Failed to start a decompiler process.{0}  {1}{0}  {2}{0} {3}",
                        reko_cmdline_exe,
                        Environment.NewLine,
                        job.Arguments,
                        job.Name);
                    this.jobsRemaining!.Signal();
                    return;
                }

                var elapsedTime = DateTime.Now - now;
                var relpath = Path.GetRelativePath(subjects_dir, jobVirtualPath);
                string testResult = FormatTestResult(job.Name, relpath, output);

                lock (resultLock)
                {
                    var key = job.Key;
                    allResults.Add(key, testResult);
                    executionTimes.Add(key, elapsedTime);
                }
                this.jobsRemaining!.Signal();
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
        /// Looks recursibely in the filesystem for subjects in the directory whose
        /// absolute path is <paramref name="dir"/>.
        /// </summary>
        /// <param name="dir">Directory in which a test may be located.</param>
        /// <param name="jobs">List of <see cref="Job"/>s that are collected by the recursion</param>
        private void HandleDir(string dir, List<Job> jobs)
        {
            foreach (var subdir in Directory.EnumerateDirectories(dir))
            {
                HandleDir(subdir, jobs);
            }

            // remove any .reko folder
            if (dir.EndsWith(".reko"))
            {
                AttemptDeleteDirectory(dir);
                return;
            }

            var dcProjects = Directory.GetFiles(dir, "*.dcproject");
            var subjectCmd = Path.Combine(dir, "subject.cmd");
            bool hasSubjectCmd = File.Exists(subjectCmd);

            // if there is no .dcproject and no subject.cmd, there's nothing to do
            if (dcProjects.Length == 0 && !hasSubjectCmd)
            {
                return;
            }

            // clear output files outside of .reko (created by some .dcproject)
            ClearDir(dir);

            // queue all .dcproject for execution
            foreach (var proj in dcProjects)
            {
                jobs.Add(new Job(GetFilePrefix(proj), dir, "decompile " + proj));
            }

            if (hasSubjectCmd)
            {
                ProcessSubjectCmdFile(dir, jobs, subjectCmd);
            }
        }

        private void ProcessSubjectCmdFile(string dir, List<Job> jobs, string subjectCmd)
        {
            var lines = File.ReadAllLines(subjectCmd, new UTF8Encoding(false))
                .Select(l => l.Trim())
                .Where(l => !l.StartsWith('#'));

            foreach (var line in lines)
            {
                string args = line;
                if (line.StartsWith("reko.exe"))
                {
                    // remove the exe name from the arguments string
                    args = args.Remove(0, "reko.exe".Length + 1);
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
                    jobs.Add(new Job(jobName, dir, args));
                }
            }
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
            Exception? ex = null;
            int msecSleep = 100;
            for (int i = 0; i < MaxDeleteAttempts; ++i, msecSleep *= 2)
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
                Thread.Sleep(msecSleep);
            }
            Console.Error.WriteLine("*** Unable to delete directory {0} after {1} attempts. {2}", dir, MaxDeleteAttempts, ex!.Message);
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
            if (git is not null) 
            {
                // pipe stdout to stderr (to show it on-screen, since stdout is going to regression.log)
                git.StandardOutput.BaseStream.CopyTo(Console.OpenStandardError());

                git.WaitForExit();
                return git.ExitCode;
            }
            else 
            {
                return 1;
            }
        }

        private List<Job> CollectJobs(List<string> dirs)
        {
            var jobs = new List<Job>();
            foreach (var dir in dirs)
            {
                HandleDir(dir, jobs);
            }
            return jobs;
        }

        private static bool IsInSourceDirectory(string file)
        {
            var dirname = Path.GetDirectoryName(file);
            var basename = Path.GetFileName(dirname);
            return basename!.Equals("src", StringComparison.InvariantCultureIgnoreCase);
        }

        private void ExecuteJobs(List<Job> jobs)
        {
            var orderedJobs = OrderJobsByWeight(jobs);
            this.jobsRemaining = new CountdownEvent(jobs.Count);
            foreach (var job in orderedJobs)
            {
                StartJob(job);
            }
            jobsRemaining.Wait();
            var weights = ComputeWeights(this.executionTimes);
            SaveWeights(weights);
        }

        private List<Job> OrderJobsByWeight(List<Job> jobs)
        {
            var weights = LoadWeights();
            var orderedJobs =
                from job in jobs
                join weight in weights on job.Key equals weight.Key into ws
                from weight in ws.DefaultIfEmpty()
                orderby weight.Key is not null ? weight.Value : 0 descending
                select job;
            return orderedJobs.ToList();
        }

        private Dictionary<string, double> ComputeWeights(Dictionary<string, TimeSpan> executionTimes)
        {
            return executionTimes
                .ToDictionary(de => de.Key, de => de.Value.TotalMilliseconds);
        }

        private void StripSuffixes(List<string> dirs)
        {
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

        private void DisplayResults(int cJobs, TimeSpan elapsedTime)
        {
            foreach (var result in this.allResults.Values)
            {
                Console.WriteLine(result);
            }
            Console.WriteLine("Decompiled {0} binaries in {1:0.00} seconds.", cJobs, elapsedTime.TotalSeconds);
        }

        private Dictionary<string, double> LoadWeights()
        {
            var weights = new Dictionary<string, double>();
            try
            {
                var path = Path.Combine(this.subjects_dir, WeightsFilename);
                foreach (var line in File.ReadAllLines(path))
                {
                    if (line.Length < 3)
                        continue;
                    var elems = line.Split('|');
                    if (elems.Length != 2)
                        continue;
                    var key = elems[0];
                    if (double.TryParse(elems[1], out var weight))
                    {
                        weights[key] = weight;
                    }
                }
            }
            catch
            {
                // Ignore errors; the weights are "nice-to-have".
            }
            return weights;
        }

        private void SaveWeights(Dictionary<string, double> weights)
        {
            var path = Path.Combine(this.subjects_dir, WeightsFilename);
            using var file = File.CreateText(path);
            foreach (var kv in weights)
            {
                file.WriteLine("{0}|{1}", kv.Key, kv.Value);
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
                        define .NET framework (net8.0)
  --strip-suffixes=STRIP_SUFFIXES
                        strip number suffixes from SSA identifiers (yes, no)
  -v, --verbose         produce verbose output
");
        }

    }
}