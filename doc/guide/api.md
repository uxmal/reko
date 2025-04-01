## Using the Reko API

You can use Reko as a class library in a .NET project. Just add a NuGet package reference from NuGet.org to your project:

https://www.nuget.org/packages/Reko.Decompiler.Runtime/

### Using Reko's disassemblers

You can use Reko in your program to disassemble machine code. To disassemble some X86 code, you can use the following code (after adding the NuGet.org package above):

```C#
using Reko.Core;
using Reko.Arch.X86;
using System;
using System.ComponentModel.Design;

class RandomX86ByteDisassembler
{
    public static void Main(string[] args)
    {
        // Generate some random bytes.
        var rnd = new Random();
        var bytes = new byte[1000];
        rnd.NextBytes(bytes);

        // Put the bytes in a ByteMemoryArea that Reko can consume.
        var mem = new ByteMemoryArea(Address.Ptr32(0x00123400), bytes);
        
        // Create an instance of an architecture whose disassembler you wish to use.
        var arch = new X86ArchitectureFlat32(new ServiceContainer(), "x86-protected-32");

        // Create an image reader, starting at offset 0 of the memory area.
        var rdr = arch.CreateImageReader(mem, 0);

        // Create a disassembler using the image reader and loop through the random bytes.
        var dasm = arch.CreateDisassembler(rdr);
        foreach (var instr in dasm) 
        {
            Console.WriteLine("{0}: {1}", instr.Address, instr);
        }
    }
}
```
