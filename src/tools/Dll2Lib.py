TOOLCHAIN = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\bin\\amd64\\"

def getNativePath(filePath):
	if strtoupper(PHP_OS) == "CYGWIN":
		return rtrim(shell_exec("cygpath -w '{$filePath}'"))
	return filePath

def getLocalPath(filePath):
	if(strtoupper(PHP_OS) == "CYGWIN"):
		return rtrim(shell_exec("cygpath -u '{$filePath}'"))
	return filePath

localToolchain = getLocalPath(TOOLCHAIN)
print("TOOLCHAIN @ %s\n", localToolchain);

inDll = argv[1]
outBase = getcwd() + "/" + pathinfo(inDll, PATHINFO_FILENAME)
outDef = outBase + ".def"
outLib = outBase + ".lib"

nativeDll = escapeshellarg(getNativePath(inDll))
nativeDef = escapeshellarg(getNativePath(outDef))
nativeLib = escapeshellarg(getNativePath(outLib))
#//system("{$localToolchain}/dumpbin.exe /EXPORTS /OUT:{$nativeDef} {$nativeDll}");

#_def = fopen($outDef, "r") or die("open fail" . PHP_EOL);
#newdef = fopen($outBase + "_fixed.def", "w+") or die("open fail" . PHP_EOL);

found = false
while not feof(_def):
	line = trim(fgets(_def))
	if empty(line):
		continue
	if not found and strpos(line, "ordinal") == 0:
		found = true
		libName = pathinfo(outBase, PATHINFO_FILENAME)
		fprintf(newdef, "LIBRARY\t$libName\r\n")
		fprintf(newdef, "EXPORTS\r\n")
		continue
	
	if found:
		parts = preg_split("/\s+/", line)
		if (len(parts) < 4) or not is_numeric(parts[0]):
			fprintf(STDERR, "Skip '%s'\n", line)
			continue
		
		if parts[2] == "[NONAME]":
			parts[2] = substr(parts[2], 1, strlen(parts[2]) - 2)

		fprintf(newdef, "%s @ %d %s\r\n", parts[3], parts[0], parts[2])
	

fclose(_def)
fclose(newdef)

nativeDef = escapeshellarg(getNativePath(outBase + "_fixed.def"))
cmd = escapeshellarg("{$localToolchain}/lib.exe")
system("{$cmd} /DEF:{$nativeDef} /OUT:{$nativeLib}")

#//unlink($outDef);
