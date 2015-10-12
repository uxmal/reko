#!/usr/bin/python
# Run all regression tests on the subjects in the 
# $(REKO)/subjects directory tree.
# Subject files are identified by either having a dcproject file associated
# with them or a subject.cmd file containing reko command lines to execute.

import os
import os.path
import subprocess

reko_cmdline =  os.path.abspath("../src/Drivers/CmdLine/bin/Release/decompile.exe")


def run_test(dir_name):
    pname = os.path.join(dir_name, "subject.dcproject")
    if os.path.isfile(pname):
        execute_reko_project(dir_name, pname)
        return

    scr_name = os.path.join(dir_name, "subject.cmd")
    if os.path.isfile(scr_name):
        execute_command_file(dir_name, scr_name)


def execute_reko_project(dir, pname):
    proc = subprocess.Popen([
        reko_cmdline,
        pname
        ],
        stdout=subprocess.PIPE,
        universal_newlines=True)
    out = proc.communicate()[0]
    if "error" in out.lower():
        print("*** " + dir)
        print(out)

def execute_command_file(dir, scr_name):
    oldDir = os.getcwd()
    os.chdir(dir)
    f = open("subject.cmd")
    lines = f.readlines()
    f.close()
    if (lines is None):
        return
    for line in lines:
        exe_and_args = line.split()
        exe_and_args[0] = reko_cmdline
        proc = subprocess.Popen(
            exe_and_args,
            stdout=subprocess.PIPE,
            universal_newlines=True)
        out = proc.communicate()[0]
        if "error" in out.lower():
            print("*** " + dir)
            print(out)
    os.chdir(oldDir)

for root, subdirs, files in os.walk("."):
    run_test(root)
