#!/usr/bin/python
# Run all regression tests on the subjects in the 
# $(REKO)/subjects directory tree.
# Subject files are identified by either having a dcproject file associated
# with them or a subject.cmd file containing reko command lines to execute.

import os
import os.path
import subprocess

reko_cmdline =  os.path.abspath("../src/Drivers/CmdLine/bin/Debug/decompile.exe")

def run_test(dir_name, files):
    for pname in files:
        if pname.endswith(".dcproject"):
            execute_in_dir(execute_reko_project, dir_name, pname)

    scr_name = os.path.join(dir_name, "subject.cmd")
    if os.path.isfile(scr_name):
        execute_in_dir(execute_command_file, dir_name, scr_name)

def execute_in_dir(fn, dir, fname):
    oldDir = os.getcwd()
    os.chdir(dir)
    fn(dir, fname)
    os.chdir(oldDir)

def execute_reko_project(dir, pname):
    print("=== "+ pname)
    proc = subprocess.Popen(
        [ reko_cmdline, pname ],
        stdout=subprocess.PIPE,
        universal_newlines=True)
    out = proc.communicate()[0]
    if "error" in out.lower():
        print("*** " + pname)
        print(out)

# Find all commands to execute.
def execute_command_file(dir, scr_name):
    f = open("subject.cmd")
    lines = f.readlines()
    f.close()
    if (lines is None):
        return
    for line in lines:
        exe_and_args = line.split()
        if len(exe_and_args) <= 1:
            continue
        print("=== "+ exe_and_args[1])
        exe_and_args[0] = reko_cmdline
        proc = subprocess.Popen(
            exe_and_args,
            stdout=subprocess.PIPE,
            universal_newlines=True)
        out = proc.communicate()[0]
        if "error" in out.lower():
            print("*** " + exe_and_args[1])
            print(out)

for root, subdirs, files in os.walk("."):
    run_test(root, files)
