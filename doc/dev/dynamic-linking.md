## Reko and dynamic linking

When scanning a binary, Reko will encounter calls to **trampolines**, short sequences of simple instructions ended by an indirect jump. These trampolines are commonly used when making calls to dynamically linked binaries (PE DLL's, ELF .so, etc ). Trampolines are also used in some platform/image format combinations to support position indepenent code (PIC). Finally, some trampolines are used as dispatch mechanisms when making calls to the operating environment (e.g the `CALL 0005h` system service call on CP/M).

We want Reko to able to translate calls to trampolines into direct calls to the destination code wherever possible, to make it appear as close to what the original source code may have looked like. To make this happen, Reko needs to recognize the trampolines and replace them with calls to the actual destination. Often, there is information available to compute the actual destination.

## Trampolines

In Reko parlance a **trampoline** is a short sequence of instructions ending in an indirect jump. The instruction sequence is responsible for loading the address of an external procedure from memory and then jumping to that address. This import address is usually located in a table, called variously the Import Address Table (IAT - in PE binaries) or the Global Offset Table (GOT - in ELF binaries). These tables contain addresses, each stored in a separate table slot. Each table slot corresponds to an imported procedure.  

In pseudocode, a trampoline will look like this:
```
trampoline_start:
    scratch_register = <compute_address_of_jump_slot expression>
    foreign_address = Mem[scratch_register]
    jump foreign_address
```
The `compute_address_of_jump_slot` expression may vary in complexity depending on the processor architecture and whether or not the calling binary is PIC or not. Sometimes, a scratch register is not even necessary as the instruction architecture may support expressing the indirection in a single instruction. For instance, on x86 32-bit executables, the trampoline often looks like this: 
```
    jmp [address_inside_an_IAT]    ; IAT = IMport address Table, see below.
```
In general, Reko cannot assume that it can find the locations of all these stubs. Although sometimes a binary will be nice enough to contain symbolic information identifying the location of the trampolines (e.g. sections named `.plt` in ELF binaries), this is by no means always the case.

Assuming the worst case, the only thing Reko can rely on is that there will be information inside the executable binary that will refer to slots in the address tables. The strategy used by Reko is to collect the determine for each slot in the address table, the imported procedure it corresponds to. Later, when it encounters a call to a trampoline, it evaluates the code in the trampoline, which should result in the address of an import slot (which contains an address to a procedure). It then replaces the call to the trampoline with a direct call to the procedure.

## ELF examples

In ELF binaries, the trampoline implementation varies by CPU architecture. In general, though, the trampoline will evaluate some instructions resulting in a slot address. This slot address is dereferenced, fetching the address of the desired procedure, after which control is transferred to said procedure. These trampolines are called **PLT stubs** and are typically located in a section called `.plt` in the binary. However, section names may be stripped so Reko may not be able to find the trampolines "eagerly" by traversing a `.plt` section. The PLT stubs are often sequentially arranged in memory like this:
```
        +-------------------------------+   .plt section
001100  | load address from GOT slot #1 |      puts@plt: PLT stub for imported
        | jmp to slot address           |      procedure "puts"
        +-------------------------------+ 
001110  | load address from GOT slot #2 |      printf@plt: PLT stub for imported
        | jmp to slot address           |      procedure "printf"
        +-------------------------------+ 
001120  | load address from GOT slot #3 |      my_internal_func@plt: PLT stub for 
        | jmp to slot address           |      an internal function.
        +-------------------------------+ 
        |    ...                        |
```

Each PLT stub is a trampoline that jumps to an address fetched from a slot in the GOT. The GOT slots for the PLT stubs are arranged in a table, in the same order as the PLT stubs:
```
        +--------------+
0020000 | some pointer |
0020004 | some pointer |
0020008 | some pointer |
002000C | some pointer |
          ...
```
Reko doesn't care about the exact values in the GOT slots, but does care about the _addresses_ of those stubs (0020000, 0020004...)

The GOT slots are referenced by the ELF relocation records in the ELF binary. Each ELF relocation record is associated with a GOT slot address and an ELF symbol. 
```
    +------------------------+      Relocation table
    | 0020000                |          Address of "puts" GOT slot
    | symbol #1              |          Symbol #1 is 'puts'
    +------------------------+
    | address of GOT slot #2 |          Address of "printf" GOT slot 
    | symbol #2              |          Symbol #2 is 'printf'
    +------------------------+
    | address of GOT slot #3 |          Address of "my_internal_func" GOT slot 
    | symbol #3              |          Symbol #2 is 'my_internal_func'
    +------------------------+
    | ...                    |
```

Each ELF symbol in turn has a value. If the ELF symbol value for a relocation is zero, it means that the relocation is referring to an _external_ procedure residing in some shared library. If the ELF symbol value for a relocation is not zero, it means the relocation is referring to an _internal_ procedure. The symbol value will then be the address of the actual procedure.
```
    +------------------+
    | ''               |    Symbol #0 is always the empty string
    | 0x00000000       |
    +------------------+
    | puts             |    Symbol #1 is "puts"
    | 0x00000000       |    It has no value
    +------------------+
    | printf           |    Symbol #2 is "printf"
    | 0x00000000       |    It also has no value
    +------------------+
    | my_internal_func |    Symbol #3 is "my_internal_func"
    | 0x0001300        |    Its value is the address of local procedure.   
    +------------------+
    | ...              |
```

When loading the ELF binary, Reko will iterate through the relocation records. If it encounters a relocation record whose symbol has a non-zero value, it remembers that the slot address the relocation is associated withwith a non-zero symbol

### 32-bit x86
On i386, the PLT stub in a PIC binary looks like this:
```
puts@plt:           ; this is a stub for calling the puts function in libc
    jmp
## Windows example

Different OS's and image loaders approach dynamic loading in different ways. On Windows, a call to a DLL function `CreateWindowA` is typically generated like this:
```
    call CreateWindowA      ; user code calling CreateWindowA
    ...
CreateWindowA proc          ; The linker fetched this small trampoline from USER32.LIB
    jmp ds:[CreateWindowA__imp]
```
where `CreateWindowA__imp` is a symbolic name for an entry into the Import Address Table (IAT), located in the PE executable. Each DLL the executable is linked against will have a seperate IAT. IN the loaded DLL, the IAT looks like:
```
USER32.DLL_IAT:
ShowMessageBoxA:    dd  <pointer to code inside USER32.dll>
CreateWindowA:      dd  <pointer to code inside USER32.dll>
        ....
```