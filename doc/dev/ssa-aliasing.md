# SSA and aliasing identifiers

Reko uses the algorithm described in "Simple and Efficient Construction of Static Single Assignment Form", by Matthias Braun et al, to convert `Procedure`s to static single-assignment (SSA) form. Conceptually the process is straightforward. The algorithm traverses the basic blocks in the procedure's control flow graph in reverse post order. Each statement in each basic block is analyzed. Identifiers that are defined in a statement (e.g. `a` in the statement `a = b + c`) are marked as the reaching definition of `a` for the current basic block, and replaced with an SSA identifier (e.g. `a_1`). To find the SSA identifier for identifiers that are used in the statement (e.g. `b` and `c` in the statement `a = b + c`), the algorithm first tries to find a reaching definition in the current basic block. If present, the search stops. Otherwise, the algorithm considers the predecessors of the current basic block. If there is exactly one predecessor, the algorithm simply proceeds to that predecessor. If there are no predecessors, the entry block has been reached. The identifier must then either be a parameter of the procedure, or represents a callee-saved register.

If there are two or more predecessors, the algorithm has to consider the possibility that an SSA phi-node must be generated. The algorithm first places a "placeholder" phi node in the current block, to break possible cycles. It then recursively visits each predecessor to get the SSA name for the identifier under consideration. If the SSA names from all the predecessors are the same, we have a trivial phi function and the place holder is eliminated. Otherwise the placeholder is filled out with the different SSA names from each of the predecessors.

## Aliasing

The Braun SSA algorithm as described above works fine for compiling source code programs into SSA form: the identifiers it reasons about are present in the source code. However, a decompiler has no access to identifiers. Instead it is presented with instructions operating on registers in which the actual values being computed are stored. The decompiler wants to separate the actual values being computed from the registers in which they happen to be stored. Specifically, we want to do SSA analysis on the values, not on the registers themselves. 

Certain processor architectures support accessing different bit ranges of the registers by providing alternate names for those parts. For instance, different bit ranges of the x86-64 64-bit register `rax` may be referred by using the register names `eax`, `ax`, `al`, and `ah`. These register names are said to _alias_ each other. Modifying `eax` will cause `ax` to change, and vice versa. The decompiler needs to track these aliases accurately or it will generate incorrect output.

### x86 aliasing example
Consider the following MSVC program:
```C
void __fastcall add_magic_number(short * pshort)
{
    *pch += 0x4711;
    return *pch;
}
```

A compiler could compile it to the following assembly language:
```
    mov     ax, WORD PTR [ecx]
    mov     edx, 4711h
    add     ax, dx
    mov     WORD PTR [ecx], ax
    ret
```
Here, the value loaded into `edx` is aliased by the `dx` in the `add` instruction. The compiler is justified in generating the `mov edx,4711h` instruction since it is shorter and executes faster than the alternative `mov dx,4711h`. It's the decompiler's job to determine that the high 16 bits of `edx` aren't actually used.

### Representing register aliasing in Reko

In order to support reasoning about register aliasing, Reko uses the distinct notions of `Identifier`, `Storage`, and `StorageDomain`. An `Identifier` is characterized by its name, its data type, and the `Storage` to which it is bound. A storage identifies a region of bits, be it in a register or in memory. The location of those bits is specified by a `StorageDomain`. 

Each register in a CPU has its own distinct `StorageDomain`, unless the register aliases another. In that case the `StorageDomain` is shared between the aliased registers. For instance, the `rax`, `eax`, `ax`, `ah`, and `al` registers all have the same `StorageDomain` value 0, while the `rcx`, `ecx`, `cx`, `ch` and `cl` registers all share the different `StorageDomain` value 1.

A `Storage` `s` is said to _cover_ another `Storage` `t` if the two `Storage`s have the same storage domain and the bit range of `s` is equal to or exceeds that of `t`. For instance, the x86 register `eax` covers `ax`, but `ax` does not cover `eax`.

### Alias assignments

The two assembly language instructions:
```
    mov edx, 4711h
    add ax, dx
```
are rewritten into Reko's IL as:
```
    edx = 0x00004711
    ax = ax + dx
```
Trying to execute these two IL statements symbolically would not work correctly, as `edx` and `dx` would be treated as different identifiers, even though they in the x86 architecture refer to (different bit ranges of) the same storage domain. In order to make this relationship explicit, the analysis phase of Reko introduces _alias assignments_. These assigments don't exist in the original machine code, but are generated as needed to resolve identifiers that are aliasing each other. The example code above would be rewritten as follows:
```
    edx = 0x00004711
    dx = SLICE(edx, word16, 0)  ; alias assignment
    ax = ax + dx
```
Symbolic execution of the statements now accounts for the alising relationship between `edx` and `dx` correctly. 

The decompiler analysis stage should only introduce such alias assignments when necessary, as they tend to make the code grow larger. Determining when and where to introduce such alias assignments is part of the Reko SSA algorithm.

## Computing reaching definitions in the presence of aliased registers

As the Braun SSA algorithm traverses a basic block, it maintains a mapping `id -> ssa_id` containing, for each identifier defined in the basic block, the corresponding SSA identifier. As a new definition of each identifier is encountered, a new SSA identifier name is generated and replaces the old SSA identifier in the mapping.

However, if there are aliases present, a simple mapping from identifiers is not sufficient. To see this, let's look at the example above, rewritten into Reko IL:
 ```
    ax = Mem0[ecx:word16]
    edx = 0x00004711
    ax = ax + dx
    Mem0[ecx:word16] = ax
    return
```
In our example above, `edx` and `dx` are not the same identifier, and so the Braun SSA algorithm would generate distinct SSA names when rewriting the code:
```
    def ecx
    def dx              ; ** WRONG! dx is not live-in!
    ax_1 = Mem0[ecx:word16]
    edx_2 = 0x00004711
    ax_3 = ax_1 + dx    ; ** WRONG! Should be using (a slice of) edx_2!
    Mem4[ecx:word16] = ax_3
    return
```
The result is incorrect: the definition of `ax_3` should use the lower 16 bits of `edx_2`, but instead `dx` is treated as a register parameter to the procedure.

The Reko SSA algorithm introduces aliasing assignments to generate correct code. To do so, it uses a different mapping so that it can properly track all aliases of a particular storage domain. The type of the mapping is changed to `StorageDomain -> AliasState`. An `AliasState` in turn is defined as:
```
defs: List<ssa_id>
aliases: (id -> ssa_id)
```
The `defs` is a list of SSA identifiers that are actually defined by the original IL code in the basic block. The SSA identifiers in `defs` are ordered as they occur in the basic block. When the algorithm encounters a new identifier definition `d`, any SSA identifiers in the `defs` list that are covered by `d` are removed.

The `aliases` dictionary contains SSA identifiers that have been introduced as synthesized alias statements. The intention is to avoid generating new alias assignments unnecessarily.

### Example: definition covers use

Consider what happens as the Reko SSA algorithm processes the instructions in the IL code example, where the definition of `edx` covers the subsequent use `dx`.

 The first assignment to `ax` uses `ecx`. Since the mapping doesn't contain the storage domain for `ecx` (namely, 1), the algorithm tries to find a predecessor block that has a reaching definition of that storage domain. Since there is no predecessor, Reko assumes that `ecx` is either a register parameter or a callee-saved register, and generates a `def` statement to provide `ecx` with a defining statement. Finally, the mapping is changed to include a value for the `ecx` storage domain:
```
1: defs=[ecx] aliases={}
```
The same assignment defines `ax`. The mapping doesn't contain the storage domain for `ax` (namely 0), so the algorithm generates a new SSA id `ax_1` and we add a value for `ax` into the mapping. It now looks like this:
```
0: defs=[ax_1] aliases={}
1: defs=[ecx]  aliases={}
```
and the partially complete SSA form look like this:
```
    def ecx
    ax_1 = Mem0[ecx:word16]
    ...
```
The next assignment defines `edx`, whose storage domain is 2. The algorithm proceeds similarly to how it processed `ax`, resulting in:
```
0: defs=[ax_1]  aliases={}
1: defs=[ecx]   aliases={}
2: defs=[edx_2] aliases={}

    def ecx
    ax_1 = Mem0[ecx:word16]
    edx_2 = 0x00004711
    ...
```
The next assignment uses `dx`, which is an alias of the previously defined `edx` register. When looking up the storage domain for `dx`, we do find an alias state. We first look in the `aliases` dictionary for any alias definitions of `dx`, but we don't find any. We then look in `defs` to see if we find any parts find an alias. We find that `edx_2` covers the bit range of `dx`. We can therefore inject an alias assignment, resulting in:
```
0: defs=[ax_4]  aliases={}
1: defs=[ecx]   aliases={}
2: defs=[edx_2] aliases={ dx_3 }

    def ecx
    ax_1 = Mem0[ecx:word16]
    edx_2 = 0x00004711
    dx_3 = SLICE(edx_2, word16, 0)  ; alias
    ax_4 = ax_1 + dx_3
    ...
```

### Example: definition does not cover use

Consider next an example where the reaching definition(s) do not cover the uses. The following x86 real-mode code fragment is similar to what was found in an MS-DOS program hand-written in assembler:
```
    mov bl,ds:[4231h]
    mov bh,ds:[423Ah]
    mov al,es:[bx]
```
or, rewritten in Reko IL:
```
    bl = Mem0[ds:0x4321:byte]
    bh = Mem0[ds:0x432A:byte]
    al = Mem0[es:bx:byte]
```
Here the use of `bx` is affected by both the definitions of `bl` and `bh`.

When the first assignment is encountered, there is no existing definition of the `bl`'s storage domain (3). The algorithm generates:
```
3: defs=[bl_1] aliases={}
8: defs=[ds]   aliases={}

    def ds
    bl_1 = Mem0[ds:0x4321:byte]
    ...
```
When the assignment to `bh` is encountered, there does exist a definition of `bh`'s storage domain -- it's the same 3 as `bl`'s storage domain. However we don't find `bh` in either `aliases` or `defs`. Neither does `bl` cover `bh`: in fact it doesn't even overlap `bh`. The algorithm proceeds by generating a new SSA name for `bh` and adding it to `defs`:
```
3: defs=[bl_1, bh_2] aliases={}
8: defs=[ds]   aliases={}

    def ds
    bl_1 = Mem0[ds:0x4321:byte]
    bh_2 = Mem0[ds:0x432A:byte]
    ...
```
The next statement contains a use of `bx`, whose storage domain again is 3. There is no existing alias for `bx`, but the bit ranges of both `bl_1` and `bh_2` overlap with `bx`. The algorithm discovers that an alias assignment needs to be introduced which builds up `bx` by making a sequence of `bh_2` and `bl_1`, resulting in:
```
3: defs=[bl_1, bh_2] aliases={ bx_3 }
8: defs=[ds]   aliases={}
9: defs=[es]   aliases={}


    def ds
    def es
    bl_1 = Mem0[ds:0x4321:byte]
    bh_2 = Mem0[ds:0x432A:byte]
    bx_3 = SEQ(bh_2, bl_1)      ; alias
    al_5 = Mem4[es:bx_3:byte]
    ...
```
### Example: sub-register write

Consider the IL fragment derived from x86 machine code:
```
    eax = Mem0[ecx + 4:word32]
    ax = Mem0[edx + 8:word16]
    Mem0[ebx:word32] = eax
```
After the first assignment:
```
0: defs=[eax_1] aliases={}

    eax_1 = Mem0[ecx + 4:word32]
    ...
```
When processing the assignment to `ax`, the algorithm discovers an existing `AliasState` for the storage domain 0 of `ax`. However, `ax` doesn't cover the bit range of `eax_1`, so a record must be kept of both `eax_1` and the newly generated SSA identifier `ax_2`.
```
0: defs=[eax_1, ax_2] aliases={ }

    eax_1 = Mem0[ecx + 4:word32]
    ax_2 = Mem0[edx + 8:word16]
    ...
```
When using `eax`, the `AliasState` for storage domain 0 is consulted again. No alias exists in `aliases`, so the algorithm works backwards in `defs` finding all SSA identifiers that overlap `eax`. It finds `ax_2` and `eax_1`. Because `ax_2` overlaps a part of `eax_1`, a `SLICE` operation must be generated to extract only the part of `eax_1` that `ax_2` does not cover. The algorithm now has to create new alias statements to correctly model the code:
```
0: defs=[eax_1, ax_2] aliases={ eax_4 }

    eax_1 = Mem0[ecx + 4:word32]
    ax_2 = Mem0[edx + 8:word16]
    tmp_3 = SLICE(eax_1, word16, 16)
    eax_4 = SEQ(tmp_3, ax_2)        ; alias
    Mem5[ebx:word32] = eax_4
```
