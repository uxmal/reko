# Reko decompiler command line tutorial

The Reko decompiler can be operated as a command line tool. The general syntax for the command line tool is:
```
reko <major-command> [options] <filename>
```
where `major-command` is one of `decompile`, `disassemble` (or `dasm`), and `dump`, and `filename` is
the name of a binary executable file.

The `dump` command format will simply load the file and dump its contents to the standard output. The `disassemble` command
will cause the Reko decompiler engine to attempt to identify machine code routines and disassembles them.
The `decompile` command performs a full decompilation of the file.

To dump the contents of the executable file `myexe`:
```
reko dump myexe
```
To disassemble the executable file `myexe`:
```
reko disassemble myexe
```
or
```
reko dasm myexe
```
To decompile the executable file `myexe`:
```
reko decompile myexe
```

## Custom loaders
Reko will try to identify the file type of `filename` based on the metadata located inside the file. The information in
`reko.config` will identify popular file formats, but might fail on custom formats or raw ROM images. In 
such cases, you can provide either the name of a .NET assembly capable of loading the custom format:
```
reko decompile --loader Custom.ClassName,CustomAssembly filename
```
or the name of a *partial image loader* which loads the image into memory but which requires additional
information from the user, like what processor architecture the image is intended for, or which address
the image is expected to be loaded at:
```
reko decompile --loader raw --arch arm --base 001000 --entry 001010
```
This would load the file contents using the `raw` loader, which simply allocates a chunk of memory and loads
the raw contents of the file. Then Reko will process the file contents assuming the architecture is ARM (32-bit AArch32)
with the image contents starting at address 0x1000 and where the entry point of the program is at address 0x1010.

Available loaders, architectures, and environments can be listed by typing `reko --help`

## Customizing disassembler output
The command switches `--dasm-address`, `--dasm-base-instrs` and `--dasm-bytes` control the output of the disassembler.
A typical output on a Z80 executable using none of these switches might result in:
```
        ld      bc,12FC
        xor     a,e
        inc     a
```
Specifying the `--dasm-address` switch will cause the disassembler to emit an address for every disassembled
instruction, resulting in output like:
```
0000    ld      bc,12FC
0003    xor     a,e
0004    inc     a
```
Specifying the `--dasm-bytes` switch will cause the disassembler to emit the bytes of each disassembled instruction,
resulting in:
```
01 FC 12                ld      bc,12FC
AB                      xor     a,e
3C                      inc     a
```
Combining the `--dasm-address` and `--dasm-bytes` flags results in:
```
0000 01 FC 12           ld      bc,12FC
0003 AB                 xor     a,e
0004 3C                 inc     a
```
The `--dasm-base-instrs` switch can be used to prevent the disassembler from using pseudo-instructions when rendering
instructions (on architectures where this is supported). For instance, the Risc-V instruction `addi x3,x0,4` is rendered
as the pseudo-instruction `li x3,4` unless the `--dasm-base-instr` switch is specified.




