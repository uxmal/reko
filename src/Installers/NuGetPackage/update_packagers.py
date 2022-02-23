# This script is used to make sure the WiX installer spec and NuGet spec file are 
# in sync. It expects to be run in the $(REKO)/src/NuGetPackage 
# directory.
#
# The code is not a marvel of beauty: it manipulates XML using regular expressions,
# which already puts it in a state of sin. However it does the job correctly, so 
# there is no need to gold-plate it.

import io
import os
import re

def load_component_items(f):
    items = []
    line = f.readline()
    while line:
        m = re.match(".*</Component>.*", line)
        if m:
            break
        items.append(line)
        line = f.readline()
    return items

# Load all the <var...> elements.
def load_vars(f):
    vars = {}
    line = f.readline()
    while line:
        m = re.match(".*</vars>.*", line)
        if m:
            break
        m = re.match(".*<var id=\"(.*?)\" *>(.*)</var>.*", line)
        if m:
            vars[m[1]] = m[2]
        line = f.readline()
    return vars

# Reads in all the file items and variables from the reko.xml file
def load_reko_file_spec(filename):
    spec = {}
    vars = {}
    with io.open(filename, "rt") as f:
        line = f.readline()
        while line:
            m = re.match(".*<Component Id=\"(.*?)\"", line)
            if m:
                componentName = m[1]
                spec[componentName] = load_component_items(f)
            else:
                m = re.match(".*<vars>.*", line)
                if m:
                    vars = load_vars(f)
            line = f.readline()
    return (vars, spec)

# Collects all references to variables that are missing in the <vars> element.
def generate_missing_vars(mpVars, mpSpec):
    missingVars = set()
    for (k, v) in mpSpec.items():
        for item in v:
            m = re.match(".*\$\((var.*)\).*", item)
            if m:
                var = m[1]
                if (not var in mpSpec):
                    missingVars.add(var)
    return missingVars

# def print_missing_vars(vars):
#     for var in sorted(vars):
#         print('<var id="%s">@@@</var>' % (var,))


# Validates that all the files referred to by the `vars` dictionary
# exist in the filesystem.
def validate_vars(vars):
    filesMissing = False
    for (k, v) in vars.items():
        v = v.replace('$TargetDir$', 'bin/Debug/net5.0')
        v = v.replace('$TargetFwkDir$', 'bin/Debug/net5.0')
        v = v.replace('$TargetFwk472Dir$', 'bin/Debug/net5.0')
        v = v.replace('$TargetDir_x64$', 'bin/x64/Debug/net5.0')
        v = v.replace('$Configuration$', 'Debug')
        if v[-1] == '/':
            if not os.path.isdir(v):
                print("Missing dir:  " + v)
                filesMissing = True
        else:
            if not os.path.isfile(v):
                print("Missing file: " + v)
                filesMissing = True
    return filesMissing

# Replace macros inside of the variable definitions. Macros have the syntax
# $macroname$. 
# Only a set of well-known macros are replaced.
def expand_vars(vars):
    result = {}
    for k,v in vars.items():
        v = v.replace('$TargetDir$', 'bin/$Configuration$/net5.0')
        v = v.replace('$TargetFwkDir$', 'bin/$Configuration$/net5.0')
        v = v.replace('$TargetFwk472Dir$', 'bin/$Configuration$/net5.0')
        v = v.replace('$TargetDir_x64$', 'bin/x64/$Configuration$/net5.0')
        result[k] = v
    return result

# Parses a single line into attribute-value pairs
def parse_line(line):
    return {k:v for (k, v) in re.findall(' ([a-zA-Z].*?)="(.*?)"', line)}

# For each given source line, parses it, extracts attributes, and either
# copies it verbatim to the output if there were no relevant attributes, or generate a 
# WiX <File... element and writes it to the output.
def inject_item_into_wix_file(items, output):
    for item in items:
        attrs = parse_line(item)
        wix_item = render_wix_line(attrs)
        if wix_item:
            output.write(wix_item)
        else:
            output.write(item)

# Locates the <Compoent...> elements in the WiX file template, and copies
# all items into that element.
def inject_files_into_wix_file(spec, templateFile, targetFile):
    reComponent = re.compile('.*<Component Id="(.*?)".*')
    with io.open(templateFile, "rt") as input:
        with io.open(targetFile, "wt") as output:
            line = input.readline()
            while line:
                output.write(line)
                m = reComponent.match(line)
                if m and m[1] in spec:
                    output.write("<!-- Caution! Do not edit these values, they are generated automatically\n")
                    output.write("     by a script. Edit reko-files.xml instead. -->\n")
                    inject_item_into_wix_file(spec[m[1]], output)
                line = input.readline()

# Generates NuSpec <file...> elements from the `spec` items.
def inject_items_into_nuspec_files_element(spec, vars, output):
    for comp in ['ProductComponent', 'Comp_Os2_16', 'Comp_Os2_32' ]:
        for line in spec[comp]:
            attrs = parse_line(line)
            newLine = render_nuspec_line(attrs, vars)
            if newLine:
                output.write(newLine)

# Locate the <files> element of the nuspec file template, and copies all
# the items into that element.
def inject_files_into_nuspec_file(spec, vars, templateFile, targetFile):
    reFiles = re.compile('.*<files>.*')
    with io.open(templateFile, "rt") as input:
        with io.open(targetFile, "wt") as output:
            line = input.readline()
            while line:
                output.write(line)
                m = reFiles.match(line)
                if m:
                    output.write("<!-- Caution! Do not edit these values, they are generated automatically\n")
                    output.write("     by a script. Edit reko-files.xml instead. -->\n")
                    inject_items_into_nuspec_files_element(spec, vars, output)
                line = input.readline()

# Renders a WiX <File... element if there is a 'Source' attribute present.
def render_wix_line(attrs):
    if 'Source' in attrs:
        line = '<File Source="%s"' % (attrs['Source'],)
        if 'Id' in attrs:
            line += ' Id="%s"' % (attrs['Id'],)
        if 'Name' in attrs:
            line += ' Name="%s"' % (attrs['Name'],)
        return line + " />\n"
    else:
        return None

# Given a string, interpolates all $(varname) macros with their values from 
# the `vars` dictionary.
# No attempt is made to make this code robust in corner cases. YAGNI.
def interpolate(sWithVariables, vars):
    s = ""
    sawDollar = False
    insideVariable = False
    var = ""
    for c in sWithVariables:
        if c == '$':
            # Let's assume no '$$' are present.
            sawDollar = True
        elif c == '(':
            insideVariable = sawDollar
            sawDollar = False
        elif c == ')':
            insideVariable = False
            sawDollar = False
            if var in vars:
                s += vars[var]
                var = ""
            else:
                raise ValueError("Variable '%s' in '%s' is not defined." % (var, sWithVariables))
        else:
            if insideVariable:
                var += c
            else:
                s += c
            sawDollar = False
    return s

# Renders a NuGet <file... element as a single lie if the 'nuget_target' attribute
# is present.
def render_nuspec_line(attrs, vars):
    if 'nuget_target' in attrs:
        source = interpolate(attrs['Source'], vars)
        target = attrs['nuget_target']
        target = target.replace('f:', 'lib\\netstandard2.0')
        target = target.replace('c:', 'contentFiles/any/any/reko')
        target = target.replace('i:', 'images\\')
        line = '<file src="%s" target="%s" />\n' % (source, target)
        return line
    else:
        return None


# Generates a WiX installer script and a NuGet spec file from the controlling
# file 'reko-files.xml'
(vars, spec) = load_reko_file_spec("reko-files.xml")
missing_vars = generate_missing_vars(vars, spec)
# print_missing_vars(missing_vars)
if validate_vars(vars):
    print("Some files are missing. Stopping.")
    exit()
vars = expand_vars(vars)
inject_files_into_wix_file(spec, "./WixInstallerProduct.template", "../WixInstaller/Product.wxs")
inject_files_into_nuspec_file(spec, vars, "./NuGetPackage.template", "NuGetPackage.nuspec")
