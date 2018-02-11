#!/usr/bin/python
# Run all regression tests on the subjects in the 
# $(REKO)/subjects directory tree.
# Subject binaries in the directory are identified by either:
# * having a dcproject file associated with them or
# * a subject.cmd file containing reko command lines to execute.

from optparse import OptionParser
from threading import Thread
import multiprocessing as mp
import os
import os.path
import re
import subprocess
import sys
import time

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
(options, dirs) = parser.parse_args()
if len(dirs) == 0:
    dirs = [script_dir]
(options, args) = parser.parse_args()

reko_cmdline_dir = os.path.abspath(script_dir + "/../src/Drivers/CmdLine")

start_dir = os.getcwd()

reko_cmdline = os.path.join(reko_cmdline_dir, "bin", options.platform, options.configuration, "decompile.exe")

output_extensions = [".asm", ".c", ".dis", ".h"]

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
    for pname in files:
        for ext in output_extensions:
            if pname.endswith(ext):
                os.remove(os.path.join(dir_name, pname))

def run_test(dir_name, files, pool_state):
    needClear = True
    for pname in files:
        if pname.endswith(".dcproject"):
            if needClear:
                clear_dir(dir_name, files)
                needClear = False
            execute_in_dir(execute_reko_project, dir_name, pname, pool_state)

    scr_name = os.path.join(dir_name, "subject.cmd")
    if os.path.isfile(scr_name):
        if needClear:
            clear_dir(dir_name, files)
            needClear = False
        execute_in_dir(execute_command_file, dir_name, scr_name, pool_state)

def execute_in_dir(fn, dir, fname, pool_state):
    oldDir = os.getcwd()
    os.chdir(dir)
    fn(dir, fname, pool_state)
    os.chdir(oldDir)

def execute_reko_project(dir, pname, pool_state):
    return execute_command([reko_cmdline, pname], dir, pname, pool_state)


# Remove any comment on the line
def strip_comment(line):
    return re.sub('#.*', '', line)

# Find all commands to execute.
def execute_command_file(dir, scr_name, pool_state):
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
        # Assumes the binary's name is the last item on the command line.
        execute_command(exe_and_args, dir, exe_and_args[-1], pool_state)



def processor(dir, rel_pname, exe_and_args):
    os.chdir(dir)
    output_lines = "=== " + rel_pname + "\n"
    start = time.time()
    proc = subprocess.Popen(exe_and_args,
        stdout=subprocess.PIPE,
        universal_newlines=True)
    out = proc.communicate()[0]
    output_lines += "    Time: " + str(time.time() - start) + "\n"
    if "error" in out.lower():
        output_lines += "*** " + rel_pname + "\n"
        output_lines += out
    return output_lines

def execute_command(exe_and_args, dir, pname, pool_state):
    rel_pname = os.path.join(os.path.relpath(os.getcwd(), start_dir), pname)
    if sys.platform == "linux2":
        exe_and_args.insert(0, "mono")

    (pool, queue) = pool_state
    result = pool.apply_async(processor, (dir, rel_pname, exe_and_args))
    queue.append(result)

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


TIMEOUT = 60  # seconds

if __name__ == '__main__':
    mp.freeze_support()     # Needed to keep Windows happy.

    start_time = time.time()

    pool = mp.Pool(processes=8)
    queue = []
    for dir in dirs:
        for root, subdirs, files in os.walk(dir):
            run_test(root, files, (pool,queue))
    for result in queue:
        x = result.get(timeout=TIMEOUT)
        sys.stdout.write(x)

    if options.check_output:
        check_output_files()

    print("Decompiled %s binaries in %s seconds ---" % (len(queue), time.time() - start_time))

