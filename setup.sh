#!/bin/sh
cd "$(dirname "$0")"

sln="./src/Reko-decompiler.sln"

setup_monodevelop(){
	echo "=> Unix/Monodevelop"
	typeGuid='s/8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942/2857B73E-F847-4B02-9238-064979017E93/'
	fileName='s/ArmNative\.vcxproj/ArmNative\.cproj/'

	sed "${typeGuid};${fileName}" -i "${sln}"
}

setup_windows(){
	echo "=> Windows/Visual Studio"
	typeGuid='s/2857B73E-F847-4B02-9238-064979017E93/8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942/'
	fileName='s/ArmNative\.cproj/ArmNative\.vcxproj/'

	sed "${typeGuid};${fileName}" -i "${sln}"
}

case "$1" in
	-u) setup_monodevelop;;
	-w) setup_windows;;
	*) echo "Unknown option $1";;
esac
