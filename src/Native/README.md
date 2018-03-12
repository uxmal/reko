## Reko native source code 
This directory and its subdirectories contain all the C++ source code for Reko modules.
It depends on NativeProxy.csproj being built first. That project is empty, but it does 
start `hdrgen` on `Reko.Core.dll` to generate interop header files. It then starts executing 
CMake on the contents of this directory.
