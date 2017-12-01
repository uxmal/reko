#!/usr/bin/python
# Run all regression tests on the subjects in the 
# $(REKO)/subjects directory tree.
# Subject binaries in the directory are identified by either:
# * having a dcproject file associated with them or
# * a subject.cmd file containing reko command lines to execute.

from optparse import OptionParser
import os
import os.path
import subprocess
import sys

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
    dirs = ["."]
(options, args) = parser.parse_args()

reko_cmdline_dir = os.path.abspath("../src/Drivers/CmdLine")

start_dir = os.getcwd()

reko_cmdline = os.path.join(reko_cmdline_dir, "bin", options.platform, options.configuration, "decompile.exe")
print (reko_cmdline)
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

def run_test(dir_name, files):
    needClear = True
    for pname in files:
        if pname.endswith(".dcproject"):
            if needClear:
                clear_dir(dir_name, files)
                needClear = False
            execute_in_dir(execute_reko_project, dir_name, pname)

    scr_name = os.path.join(dir_name, "subject.cmd")
    if os.path.isfile(scr_name):
        if needClear:
            clear_dir(dir_name, files)
            needClear = False
        execute_in_dir(execute_command_file, dir_name, scr_name)

def execute_in_dir(fn, dir, fname):
    oldDir = os.getcwd()
    os.chdir(dir)
    fn(dir, fname)
    os.chdir(oldDir)

def execute_reko_project(dir, pname):
    execute_command([reko_cmdline, pname], pname)

# Find all commands to execute.
def execute_command_file(dir, scr_name):
    f = open("subject.cmd")
    lines = f.readlines()
    f.close()
    if (lines is None):
        return
    for line in lines:
        exe_and_args = cmdline_split(line)
        if len(exe_and_args) <= 1:
            continue
        exe_and_args[0] = reko_cmdline
        # Assumes the binary's name is the last item on the command line.
        execute_command(exe_and_args, exe_and_args[-1])

def execute_command(exe_and_args, pname):

    rel_pname = os.path.join(os.path.relpath(os.getcwd(), start_dir), pname)
    
    if sys.platform == "linux2":
        exe_and_args.insert(0, "mono")
    print("=== " + rel_pname)
    proc = subprocess.Popen(exe_and_args,
        stdout=subprocess.PIPE,
        universal_newlines=True)
    out = proc.communicate()[0]
    if "error" in out.lower():
        print("*** " + rel_pname)
        print(out)

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

for dir in dirs:
    for root, subdirs, files in os.walk(dir):
        run_test(root, files)

if options.check_output:
    check_output_files()
