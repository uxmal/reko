<?php
/**
 * Copyright (C) 2018 Stefano Moioli <smxdev4@gmail.com>
 * Pe2Lib - Converts PE (EXE and DLL files) to .lib files
 * The tool also generates a proper .def in the process.
 * Microsoft Symbol Servers can be used with -s
 */
define("TOOLCHAIN", "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\bin\\amd64\\");
define("DBGTOOLS", "C:\\Program Files (x86)\\Windows Kits\\10\\Debuggers\\x64\\");

$TOOLCHAIN = getLocalPath(TOOLCHAIN);
$DBGTOOLS = getLocalPath(DBGTOOLS);

function getNativePath($filePath){
	if(strtoupper(PHP_OS) == "CYGWIN"){
		return rtrim(shell_exec("cygpath -w '{$filePath}'"));
	}
	return $filePath;
}

function getLocalPath($filePath){
	if(strtoupper(PHP_OS) == "CYGWIN"){
		return rtrim(shell_exec("cygpath -u '{$filePath}'"));
	}
	return $filePath;
}

function getPdbName($inputPath){
	$arg = escapeshellarg($inputPath);
	$cmd = escapeshellarg("{$TOOLCHAIN}/dumpbin.exe");

	$h = popen("{$cmd} /headers {$arg}", "r");
	if($h === FALSE){
		throw new Exception("dumpbin failed");
	}
	$pdbName = null;
	while(!feof($h)){
		$line = rtrim(fgets($h));
		if(preg_match("/cv.*Format: (.*)/i", $line, $m)){
			$parts = explode(", ", $m[1]);
			$pdbName = array_pop($parts);
			break;
		}
	}
	pclose($h);

	if(is_null($pdbName)){
		throw new Exception("No debug info");
	}

	return $pdbName;
}

function downloadPdb($inputPath, $pdbName){
	$cmd = escapeshellarg("${DBGTOOLS}/symchk.exe");
	$pwd = getcwd();
	if(strpos($pwd, " ") !== FALSE){
		throw new Exception("You cannot download PDBs in a path with spaces");
	}

	$arg = escapeshellarg($inputPath);
	system("{$cmd} {$arg} /s SRV*{$pwd}*http://msdl.microsoft.com/download/symbols", $code);

	if(!is_dir($pdbName) || $code != 0){
		throw new Exception("PDB Download failed");
	}

	$files = glob("{$pdbName}/*/{$pdbName}");
	if(count($files) < 1){
		throw new Exception("PDB Download failed (no file downloaded)");
	}

	$pdbFile = $files[0];
	
	// move pdb from [file.pdb]/[hash]/[file.pdb] to current directory
	$pdbName = basename($pdbFile);
	copy($pdbFile, "{$pdbName}.tmp");
	rmrf($pdbName);
	rename("{$pdbName}.tmp", $pdbName);
}

function rmrf($dir) {
	foreach (glob($dir) as $file) {
		if (is_dir($file)) { 
			rmrf("$file/*");
			rmdir($file);
		} else {
			unlink($file);
		}
	}
}

function ifcopy($src, $dst){
	if(!file_exists($dst))
		copy($src, $dst);
}

$opts = getopt("s", [], $optind);
$unparsed = array_slice($argv, $optind);

if(count($unparsed) < 1 || ($argc < 3 && !file_exists($argv[1]))){
	fprintf(STDERR, "Usage: %s [-s] [file.dll|exe]\n", $argv[0]);
	return 1;
}

$inDll = $unparsed[0];
$dlPdb = isset($opts["s"]);

$info = pathinfo($inDll);

if(!is_dir($info['basename'])){
	mkdir($info['basename']);
}
chdir($info['basename']);
ifcopy($inDll, getcwd() . "/{$info['basename']}");

if($dlPdb){
	$pdbName = getPdbName($inDll);
	print("PDB Name: {$pdbName}\n");

	if(!file_exists($pdbName)){
		downloadPdb($inDll, $pdbName);
	} else if(is_dir($pdbName)){
		rmrf($pdbName);
		downloadPdb($inDll, $pdbName);
	}
}

$outBase = getcwd() . "/{$info['filename']}";
$outDef = "{$outBase}.def";
$outLib = "{$outBase}.lib";

$nativeDll = escapeshellarg(getNativePath($inDll));
$nativeDef = escapeshellarg(getNativePath($outDef));
$nativeLib = escapeshellarg(getNativePath($outLib));

$cmd = escapeshellarg("{$TOOLCHAIN}/dumpbin.exe");
system("{$cmd} /PDBPATH:verbose /EXPORTS /OUT:{$nativeDef} {$nativeDll}", $code);
if($code != 0){
	throw new Exception("dumpbin.exe failed");
}

$def = fopen($outDef, "r") or die("open fail" . PHP_EOL);
$newdef = fopen($outBase . "_fixed.def", "w+") or die("open fail" . PHP_EOL);

$found = false;
while(!feof($def)){
	$line = trim(fgets($def));
	if(empty($line))
		continue;
	if(!$found && strpos($line, "ordinal") === 0){
		$found = true;
		$libName = "{$info['filename']}.{$info['extension']}";
		fwrite($newdef, "LIBRARY\t{$libName}\r\n");
		fwrite($newdef, "EXPORTS\r\n");
		continue;
	}
	if($found){
		$parts = preg_split("/\s+/", $line);
		if(
			(count($parts) < 3) ||
			(!is_numeric($parts[0]))
		){
			//fprintf(STDERR, "Skip '%s'" . PHP_EOL, $line);
			continue;
		}

		$ordinal = array_shift($parts);
		
		$hint = null;
		$name = null;
		$noname = null;

		$last = array_pop($parts);
		// no name (nor public nor pdb)
		if($last == "[NONAME]"){
			continue;
		}

		$hint = array_shift($parts);
		$rva = array_shift($parts);
		
		switch(count($parts)){
			// public name
			case 0:
				$name = $last;
				break;
			// name from pdb
			default:
				$part = array_shift($parts);
				if($name == "[NONAME]")
					$noname = " NONAME";
				$name = array_shift($parts);
				break;
		}
		
		fwrite($newdef, "{$name} @ {$ordinal}{$noname}\r\n");
	}
}
fclose($def);
fclose($newdef);

$nativeDef = escapeshellarg(getNativePath($outBase . "_fixed.def"));

$cmd = escapeshellarg("{$TOOLCHAIN}/dumpbin.exe");
$h = popen("{$cmd} /headers {$inDll}", "r");
if($h === FALSE){
	throw new Exception("dumpbin failed");
}
$machine = null;
while(!feof($h)){
	$line = rtrim(fgets($h));
	if(preg_match("/machine \((.*)\)/i", $line, $m)){
		$machine = strtoupper($m[1]);
		break;
	}
}
pclose($h);

if(!is_null($machine)){
	$machine = "/MACHINE:{$machine}";
}

$cmd = escapeshellarg("{$TOOLCHAIN}/lib.exe");
system("{$cmd} /DEF:{$nativeDef} {$machine} /OUT:{$nativeLib}", $code);
if($code != 0){
	throw new Exception("lib.exe failed");
}
?>
