# How to add support for a new processor architecture in Reko

##  Creating the bare minimum files and integrating into the build system.

For the example below, the fictitious processor **Foo-2000** is used. Naturally, you would use the real name of your processor instead.

1. Add a new `Foo2000.csproj` file under $(REKO)/src/Arch
2. Add the new project to the `Architectures` solution folder.
3. In the new project's properties, set the assembly name to `Reko.Arch.Foo2000` and the namespace to `Reko.Arch.Foo2000`.
4. Add the following classes for your project:
  Foo2000Architecture - the central class that Reko will use to create architecture-specific objects.
  Foo2000Disassembler - a disassembler that knows how to disassemble Foo2000 machine code
  Foo2000Instruction - the instruction type for your architecture.
  Foo2000ProcessorState - a class that models the processor state.
  Foo2000Rewriter - a class that rewrites a stream of `Foo2000Instruction`s into a stream of RTL instruction clusters.
5. Pick a short alias for your processor architecture (e.g. "foo2000")
6. In the file $(REKO)/src/Drivers/reko.config, add a new `<Architecture>` element under the `<Architectures>` element. It should look like this:
```xml
<Architecture Name="foo2000" Description="Foo-2000" Type="Reko.Arch.Foo2000.Foo2000Architecture,Reko.Arch.Foo2000" />
```
The `Name` attribute should be the short alias you picked in step 5. The `Description` should be a brief description of the architecture. Finally,
the `Type` attribute must be the fully qualified name of the architecture class you created in step 4.
7. Edit the `$(REKO)/src/Drivers/Common.items` file, adding a new `<Architectures>` item following the pattern of the existing ones.
8. Add unit test files for at least the disassambler and rewriter classes under $(REKO)/src/UnitTests/Arch:
    Foo2000DisassemblerTests.cs
    Foo2000RewriterTests.cs
9. Add a reference to the `Foo2000.csproj` to the WiX installer project.
10. Add a `<File>` element in the section commented `<!-- Architectures -->`

The `Foo2000` architecture is now part of the Reko solution. Now it's time to write the minimal code to make it load.

## Implementing the bare minimum interfaces

1. Start with `Foo2000Architecture`. All Reko architectures implement the `Reko.Core.IProcessorArchitecture` interface. You can choose to implement this interface directly or use the abstract base class `Reko.Core.ProcessorArchitecture`. Most architecture implementations choose the latter.


