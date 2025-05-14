## Reko.Core.Analysis

The purpose of the classes in this directory is to provide fundamental data 
structures and interfaces to support IR data flow analysis and code
transformations. Central to these classes is the represntation of 
Static Single Assignment (SSA), and the IAnalysis abstraction.

The classes have been moved to `Reko.Core` so that processor-specific
data flow analyses can be implemented without requiring that their 
respective .NET assemblies have a reference to Reko.Decompiler.dll
