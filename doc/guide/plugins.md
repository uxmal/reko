# Reko plugins

If you want to extend Reko with a custom extension, consider writing a Reko plugin. Reko will load plugins automatically from a `plugins` subdirectory relative to the main executable's assemblies.

### Prerequisites
To make a Reko plugin, you will need to have some familiarity with C# and .NET programming. Before proceeding, make sure that you've either built Reko from the source code, or installed it
using an official installer. Reko consists of various "drivers" or front ends: e.g. a command line
client and a GUI client. 

Pick the front end you wish to extend, and locate the directory in which
the executable file is located. If you built Reko from source, it will be under the directory 
`$(REKO_DIR)/src/Drivers/{front-end}/bin/{platform}/{configuration}/{net-version}` where:
* `REKO_DIR` is the root directory of the Reko source code
* `front-end` is the front end you're extending (e.g. `CmdLine`)
* `platform` is the host CPU architecture (e.g. `x64`)
* `configuration` is the configuration you built (e.g. `Debug` or `Release`)
* `net-version` is the version of the .NET SDK you built Reko with (e.g. `net8.0`)

If you installed Reko using the MSI installer on Windows, the default location of the installation directory is `C:\Program Files\jklSoft\Reko`.

There should be a `plugins` subdirectory relative to the executable directory. This is the place where
your custom plugin needs to be installed.

## Sample code and instructions

To create a Reko plugin, follow the instructions below.

* Create a .NET class library project:
```
dotnet create classlib
```
* Edit the `.csproj` file and add the following NuGet reference. (Note: adjust the `Version` attribute
to an appropriate value. The Reko decompiler runtime is hosted by NuGet.org [here](https://www.nuget.org/packages/Reko.Decompiler.Runtime))
```xml
<PackageReference Include="Reko.Decompiler.Runtime" Version="0.11.6" />
```

* Add the following C# source file to the project:
```C#
using Reko.Core.Configuration;
using Reko.Core.Plugins;
using System.Collections.Generic;

namespace MyRekoPlugins;

public class Plugin : IPlugin {

    // The Architectures property exposes a list of IProcessorArchitecture
    // implementations supported by this plugin.
    public IReadOnlyCollection<ArchitectureDefinition> Architectures
    {
        get
        {
            return new[] {
                new ArchitectureDefinition
                {
                    Name = "myCustomArch",
                    Description = "My Custom CPU Architecture",
                    // The following line is mandatory and essential.
                    // Reko uses the Type property to create an instance
                    // of the custom architecture.
                    Type = typeof(MyArchitecture)
                }
            };
        }
    }

    // This sample doesn't provide any custom loaders.
    public IReadOnlyCollection<LoaderDefinition> Loaders => Array.Empty<LoaderDefinition>();
}
```
The code above declares that this plugin can provide an implementation of
`Reko.Core.IProcessorArchitecture` that Reko can use to disassemble a file. It does so
by setting the `Type` property of an `ArchitectureDefinition` class.

* Add the following (incomplete) C# source file to the project. The code is provided here as 
a starting point, and will not work properly until you've implemented some of the methods
that are stubbed out and throwing `NotImplementedException`s.
```C#
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.Rtl;

namespace MyRekoPlugins;

public class MyArchitecture : ProcessorArchitecture
{
    public MyArchitecture(IServiceProvider services, string archId, Dictionary<string, object> options)
        : base(services, archId, options, new(), new())
    {
        // If you see 'Hello world' in your standard output, you've successfully loaded the
        // plugin and this architecture.
        System.Console.WriteLine("Hello world. This is the MyArchitecture plugin.");
    }

    public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
        => throw new NotImplementedException();

    public override IEqualityComparer<MachineInstruction>? CreateInstructionComparer(Normalize norm)
        => throw new NotImplementedException();

    public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
        => throw new NotImplementedException();

    public override ProcessorState CreateProcessorState()
        => throw new NotImplementedException();

    public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder binder, IRewriterHost host)
        => throw new NotImplementedException();

    public override FlagGroupStorage? GetFlagGroup(RegisterStorage flagRegister, uint grf)
        => throw new NotImplementedException();

    public override FlagGroupStorage? GetFlagGroup(string name)
        => throw new NotImplementedException();

    public override SortedList<string, int> GetMnemonicNames()
        => throw new NotImplementedException();

    public override int? GetMnemonicNumber(string name)
        => throw new NotImplementedException();

    public override RegisterStorage[] GetRegisters()
        => throw new NotImplementedException();

    public override string GrfToString(RegisterStorage flagRegister, string prefix, uint grf)
        => throw new NotImplementedException();

    public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
        => throw new NotImplementedException();

    public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
        => throw new NotImplementedException();

    public override bool TryParseAddress(string? txtAddr, [MaybeNullWhen(false)] out Address addr)
        => throw new NotImplementedException();
}
```

* Compile your .NET project. The end result should be a `.dll` file, somewhere in the `bin` subdirectory.

* Copy your plugin's `.dll` file to the Reko `plugins` folder you identified before. 

* Start Reko. If you are using the command line client, you can use the following command to
load your plugin:
```
reko disassemble --arch myCustomArch --base 0 somefile.exe
```
You should see the message
```
Hello world. This is the MyArchitecture plugin.
```
followed by a crash. If you see this message, you've successfully created the beginning
of your plugin. Now you get to fill in the missing method implementations to create
a properly working architecture. Have fun!
