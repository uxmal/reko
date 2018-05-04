# Reko Type System
This document describes the Reko type system and how it is used to generate
high-level language from low-level machine language. 

# Data types in binary programs
In a source code program written in a statically typed language like C, every
identifier and constant has a data type. Thinking of this visually, the type
information is located in the leaves of expression trees like:
```
a + 3
b.array[b.index]
```
where the leaves `a`, `3`, `b`, `array` and `index` are associated with some
C type.

When the source program is compiled to a binary, the type information in the 
"leaves" is pulled towards the inner nodes of the expression trees. Those
inner nodes eventually become translated to machine code instructions. In 
the resulting machine code, machine registers and data stored in memory have
no other type information than their size: they are simply sequences of bits.

Part of the decompiler's job is to analyze at how the machine code uses registers
and memory locations, and collect "evidence" for what type may be located in 
the register. The evidence is collected from the machine code operations, and 
tries to undo what the compiler did by pushing the type information from the
machine code operations back into the leaves of the expression trees. For
instance, in the x86 instruction:
```
add eax,eax
```
the mnemonic `add` gives us evidence that the value in `eax` was a signed 
or unsigned 32-bit integer before the addition and the resulting value is 
also a signed or unsigned 32-bit integer.

In a given machine language program, a machine register will be reused many
times. Each time the register is used, it may contain values of varying types.
It's important that the different uses of a register don't "contaminate" each
other. For instance, in the machine code fragment:
```
    mov eax,[ebx+4]     1)
    add [ebx+8],eax
    mov eax,[ebx+0Ch]   2)
    mov eax,[eax]
    mov [eax],32
```  
the value loaded into `eax` in statement 1) is used to perform a two's
complement addition, but the value in statement 2) is used to perform
a memory access. Fortunately, implementing static single assignment 
(SSA) form makes the different values explicit:
```
    mov eax_1,[ebx+4]
    add [ebx+8],eax_1
    mov eax_2,[ebx+0Ch]
    mov eax_3,[eax_2]
    mov [eax_3],32
```
Now it's clear that the three loads of the register `eax` correspond
to three different values, with potentially different data types.

## The type system tree
From the instructions and metadata such as image-provided symbols, Reko 
infers data types. The following are members of the Reko type system.

The types in the system form a tree, rooted in the abstract base type `DataType`,
and the edges from the root leading to ever more specialized types.
Types can be either **primitive**, implying that they have no known 
substructure, **composite**, implying that the type can be obtained
by applying one or more type contstructors to other types, or **unknown**,
implying that nothing is known about the type.

### Primitives
Each primtive type can be regarded as a combination of a size -- in bits --
and a member of one or more type domains. Some typical type domains are
`SignedInteger`, `UnsignedInteger`, `Character` and `Real`.

When all Reko knows about a value is its size in bits, it denotes it with the 
data type `byte` or `word<nn>` where `nn` is the size in bits. Strictly 
speaking, `word8` could be used instead of `byte`. Values of this kind are
regarded structureless bit vectors, and are member of all applicable type
domains.

If Reko can prove that a value is being used as an integer, the 
resulting type is either a signed integer, an unsigned integer, or a
"signless" integer, which is a union of the `SignedInteger` and 
`UnsignedInteger` domains.

### Composite types
The `Pointer` type is composed of a "pointee" type and the size of the
pointer itself in bits.

A `StructType` consists of 0 or more fields located at various offsets. 
The size of the `StructType` is either specified when it is created,
or is calculated from considering the size of the field at the highest
offset.

A `UnionType` models A C/C++ union type.

### Unknown
There are cases when Reko encounters symbols or machine instructions when absolutely
nothing is known about the type, not even its size. For instance, this occurs when 
demangling GCC C++ function names. The GCC demangling scheme doesn't record the return type
of the function, so Reko has no idea what data type is returned from the function.

