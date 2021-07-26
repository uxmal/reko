## Reko runtime
This directory contains the necessary objects for building the Reko Runtime nuget package.
The `Runtime.nuspec.template` file is used as a template for building the package.

### To build the nuget package

The easiest way to build the nuget runtime package is to navigate to `~/src/BuildTargets`. First you need to build the whole solution.
```
msbuild /t:build_solution /p:Platform=x64 /p:Configuration=Release /v:m /m
```
Note that on Unix or MacOS, you should use `UnixRelease` instead of `Release`, since the `Release` build has Windows-specific parts
that will fail to compile on non-Windows OS's.

Once the solution is built the nuspec file needs to be generated. This is done by executing:
```
msbuild /t:update_runtime_nuspec /p:Platform=x64 /p:Configuration=Release /v:m /m
```
Then the nuget package is built with:
```
msbuild /t:create_runtime_nupkg /p:Platform=x64 /p:Configuration=Release /v:m /m
```
This results in a *.nupkg file in the appropriate bin subdirectory of the `~src/Drivers/Runtime` project. This nupkg file can be uploaded to https://nuget.org or used within a local nuget repository.
