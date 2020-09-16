#!/usr/bin/python
# Run all regression tests on the subjects in the 
# $(REKO)/subjects directory tree.
# Subject binaries in the directory are identified by either:
# * having a dcproject file associated with them or
# * a subject.cmd file containing reko command lines to execute.
# For debugging purposes a file called trace.txt can be placed
# in the same directory. It should contain the names of procedures
# you wish to trace.

from __future__ import print_function
from optparse import OptionParser
from threading import Thread
from datetime import datetime
import multiprocessing as mp
import os
import os.path
import re
import subprocess
import sys
import time
import fileinput

script_dir = os.path.dirname(os.path.realpath(__file__))
os.chdir(script_dir)

parser = OptionParser()
parser.add_option("-c", "--configuration", dest="configuration",
                  help="define configuration (Debug, Release, etc.)",
                  default="Debug", metavar="CONFIGURATION")
parser.add_option("-o", "--check-output", dest="check_output",
                  action="store_true",
                  help="check output files", default=False)
parser.add_option("-p", "--platform", dest="platform",
                  help="define platform (x86, x64)",
                  default="x64")
parser.add_option("--strip-suffixes", dest="strip_suffixes",
                  help="strip number suffixes from SSA identifiers (yes, no)",
                  default="yes")
(options, dirs) = parser.parse_args()
if len(dirs) == 0:
    dirs = [script_dir]
(options, args) = parser.parse_args()
options.strip_suffixes = (options.strip_suffixes != 'no')

reko_cmdline_dir = os.path.abspath(script_dir + "/../src/Drivers/CmdLine")

start_dir = os.getcwd()

reko_cmdline = os.path.join(reko_cmdline_dir, "bin", options.platform, options.configuration, "decompile.exe")

output_extensions = [".asm", ".c", ".dis", ".h"]
source_extensions = [".c"]

class Job:
    def __init__(self, dir, rel_pname, exe_and_args):
        self.dir = dir
        self.rel_pname = rel_pname
        self.exe_and_args = exe_and_args

# Split a command line, but allow quotation marks to
# delimit file names containing spaces.
def cmdline_split(s):
    a = []
    inquotes = False
    sub = ""
    for c in s:
        if c.isspace():
            if not inquotes:
                if len(sub):
                    a.append(sub)
                    sub = ""
            else:
                sub += c
        elif c == '"':
            if not inquotes:
                inquotes = True
            else:
                inquotes = False
                a.append(sub)
                sub = ""
        else:
            sub += c
    if len(sub):
        a.append(sub)
    return a

# Remove output files
def clear_dir(dir_name, files):
    failedFiles = []
    for pname in files:
        for ext in output_extensions:
            if pname.endswith(ext):
                filename = os.path.join(dir_name, pname)
                try:
                    os.remove(filename)
                except:
                    # File may be held open by another program, try again later.
                    failedFiles.append(filename)
    # Retry all failed files with a delay. Let the error propagate if persists.
    if failedFiles:
        time.sleep(2)   # seconds
        for filename in failedFiles:
            if os.path.exists(filename):
                os.remove(filename)

def strip_id_nums(dirs):
    for dir in dirs:
        for root, subdirs, files in os.walk(dir):
            strip_id_nums_for_dir(root, files)

def strip_id_nums_for_dir(dir_name, files):
    for pname in files:
        for ext in source_extensions:
            if pname.endswith(ext):
                strip_id_nums_for_file(os.path.join(dir_name, pname))

numbered_id_regexp = re.compile('(?P<id_name>\w+)_\d+')
fn_seg_name_regexp = re.compile('(?P<seg_name>fn\w+)_(?P<offset_name>\d+)')

def strip_id_nums_for_file(file_name):
    try:
        file = fileinput.FileInput(file_name, inplace=True)
        for line in file:
            #remove EOLN
            line = line[:-1]
            line = fn_seg_name_regexp.sub('\g<seg_name>-\g<offset_name>', line)
            print(numbered_id_regexp.sub('\g<id_name>_n', line))
    except UnicodeDecodeError:
        print("Unicode decoding error in "+ file_name, file=sys.stderr)

def collect_jobs(dir_name, files, pool_state):
    needClear = True
    if dir_name.endswith(".reko"):
        clear_dir(dir_name, files)
        needClear = False
    for pname in files:
        if pname.endswith(".dcproject"):
            if needClear:
                clear_dir(dir_name, files)
                needClear = False
            collect_job_in_dir(collect_reko_project, dir_name, pname, pool_state)

    scr_name = os.path.join(dir_name, "subject.cmd")
    if os.path.isfile(scr_name):
        if needClear:
            clear_dir(dir_name, files)
            needClear = False
        collect_job_in_dir(collect_command_file, dir_name, scr_name, pool_state)

def collect_job_in_dir(fn, dir, fname, pool_state):
    oldDir = os.getcwd()
    os.chdir(dir)
    fn(dir, fname, pool_state)
    os.chdir(oldDir)

def collect_reko_project(dir, pname, pool_state):
    exe_and_args = [reko_cmdline, pname]
    exe_and_args = add_traces(dir, exe_and_args)
    return collect_job(exe_and_args, dir, pname, pool_state)

# Add procedures to be traced. If a file called 'trace.txt' is 
# placed in the same directory as the binary, its contents are
# assumed to be the names of procedures to be traced, single
# procedure on each line.
def add_traces(dir, exe_and_args):
    if os.path.isfile("trace.txt"):
        f = open("trace.txt")
        lines = [line.rstrip() for line in f.readlines()]
        f.close()
        exe_and_args.insert(1, "--debug-trace-proc")
        exe_and_args.insert(2, ",".join(lines))
    return exe_and_args

# Remove any comment on the line
def strip_comment(line):
    return re.sub('#.*', '', line)

# Find all commands to execute in a subject.cmd file
def collect_command_file(dir, scr_name, jobs):
    f = open("subject.cmd")
    lines = f.readlines()
    f.close()
    if (lines is None):
        return
    for line in lines:
        line = strip_comment(line)
        exe_and_args = cmdline_split(line)
        if len(exe_and_args) <= 1:
            continue
        exe_and_args[0] = reko_cmdline
        exe_and_args = add_traces(dir, exe_and_args)
        # Assumes the binary's name is the last item on the command line.
        collect_job(exe_and_args, dir, exe_and_args[-1], jobs)


def collect_job(exe_and_args, dir, pname, jobs):
    jobs.append(Job(dir, pname, exe_and_args))



def start_jobs(jobs, pool):
    results = []
    for job in jobs:
        results.append(pool.apply_async(processor, (job.dir, job.rel_pname, job.exe_and_args)))
    return results

# Order jobs by descending weight; long-running jobs will be started first.
def schedule_jobs(jobs, weights):
    for job in jobs:
        if job.rel_pname in weights:
            job.weight = weights[job.rel_pname]
        else:
            job.weight = 1.0
    return sorted(jobs,key=lambda j: j.weight,reverse=True)

def processor(dir, rel_pname, exe_and_args):
    os.chdir(dir)
    # print("Processor %s %s %s" % (dir, rel_pname, exe_and_args))
    banner = os.path.join(os.path.relpath(dir, start_dir), rel_pname)
    if sys.platform.startswith("linux") or sys.platform == "darwin":
        exe_and_args.insert(0, "mono")
        exe_and_args.insert(1, "--debug") # enables line numbers in stack traces
    output_lines = "=== " + banner + "\n"
    start = time.time()
    sys.stderr.write("%s: Starting %s\n" % (datetime.now().strftime("%H:%M:%S.%f"), banner))

    proc = subprocess.Popen(exe_and_args,
        stdout=subprocess.PIPE,
        universal_newlines=True)
    out = proc.communicate()[0]
    new_weight = time.time() - start
    if "error" in out.lower():
        output_lines += "*** " + banner + "\n"
        output_lines += out
    return (rel_pname, new_weight, output_lines)

def check_output_files():
    proc = subprocess.Popen(["git", "status", "."],
        stdout=subprocess.PIPE,
        universal_newlines=True)
    out = proc.communicate()[0]
    print(out)
    directoryClean = False
    if "working directory clean" in out.lower():
        directoryClean = True
    if "working tree clean" in out.lower():
        directoryClean = True
    if directoryClean:
        print("Output files are the same as in repository")
    else:
        print("Output files differ from repository")
        exit(1)


def load_weights(filename):
    if os.path.isfile(filename):
        with open(filename) as f:
            lines = f.readlines()
            splits= [line.split("|") for line in lines]
            weights = { path: float(weight) for (path, weight) in splits }
            return weights
    else:
        return {}

def save_weights(weights, filename):
    with open(filename, "w") as f:
        for k in weights:
            f.write("%s|%r\n" % (k, weights[k]))

TIMEOUT = 120  # seconds
WEIGHTS_FILENAME = "subject_weights.txt"

if __name__ == '__main__':
    mp.freeze_support()     # Needed to keep Windows happy.

    start_time = time.time()

    weights = load_weights(WEIGHTS_FILENAME)

    jobs = []
    for dir in dirs:
        for root, subdirs, files in os.walk(dir):
            collect_jobs(root, files, jobs)

    jobs = schedule_jobs(jobs, weights)
    pool = mp.Pool(processes=8)
    queue = start_jobs(jobs, pool)
    new_weights = {}
    outputs = []
    for (i, result) in enumerate(queue):
        try:
            x = result.get(timeout=TIMEOUT)
            new_weights[x[0]] = x[1]
            outputs.append(x[2])
        except:
            outputs.append("!!! " + jobs[i].rel_pname + " timed out\n")
    for output in sorted(outputs):
        sys.stdout.write(output)

    save_weights(new_weights, WEIGHTS_FILENAME)
    if options.strip_suffixes:
        print("Stripping SSA identifier numbers")
        strip_id_nums(dirs)
    if options.check_output:
        check_output_files()

    print("Decompiled %s binaries in %.2f seconds ---" % (len(queue), time.time() - start_time))
