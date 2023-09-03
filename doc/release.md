## Making a release of Reko

### Preparation
Use `git log <tag-of-last-version>..HEAD` on the `master` branch to collect log information

Edit log information to highlight major features and fixes 

Collect the names of contributors for acknowledgement

Build from a fresh clone of the Reko repository to make sure all project build without errors.
Run all unit tests, run all regression tests.
Commit changes and push to GitHub, triggering the CI builds on AppVeyor and Travis-CI.
Make sure CI tests are are passing.

## Final steps
Copy the distributables (msi files)
Go to https://github.com/uxmal/reko/releases and make a new release with a tag 
following the pattern 'version-.x.x.x.x'

### Versioning
Once the release is done, decide on a new version number x.x.x.x
Change the string constants `src/Core/Properties/AssemblyInfo.cs` file to the new version
Change the assembly versions in `src/tools/xslt/Properties/AssemblyInfo.cs`
Change the assembly versions in `src/tools/makesigs/Properties/AssemblyInfo.cs`
Change the MSI Installer version in `src/WixInstaller/Properties.wxi`
Change the MSI Installer version in `src/WixInstaller/WixInstaller.wixproj`
