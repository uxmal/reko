# Projection propagation

Projection propagation is an analysis being developed for Reko to handle transform sequences of expressions combined with the `SEQ` operator and tame them to "simple" variables. Sequences occur frequently in architectures like Z80 and X86 where there are *register pairs* like the Z80 HL (consisting of the H and L registers) and the X86 CX register (consisting of CH and CL). In x86 code, sequences of expressions occur frequently due to the segmented addressing used in 16-bit processor modes. Expression sequences also occur where high precision computations are made with multiple registers joined together.

The goal of the projection propagation transform is to eliminate as many `SEQ` and `SLICE` operations as possible, to improve thereadability of the generated decompiled code.

## Examples

### Fusing def -> SEQ

A common idiom for adding two 64-bit numbers on a 32-bit processor is to perform the additions of the least significant 32-bit components of the 64-bit numbers, and then use the processor's carry flag when adding the most significant 32-bit components.
```
    mov eax,[esp+8]
    mov edx,[esp+0Ch]
    add [00123400h],eax
    adc [00123404h],edx
    ...
```

In the code fragment above, the 64-bit addition code is using a 64-bit word that was passed on the stack, but has to be accessed as separate 32-bit operations. After rewriting to Reko's intermediate format, the example will look like the following:
```
    def dwArg04
    def dwArg08
    eax_1 = dwArg04
    edx_2 = dwArg08
    v3 = Mem0[00123400:word32] + eax_1
    Mem4[00123400:word32] = v3
    SCZO_5 = cond(v3)
    C_6 = SLICE(SZCO_5, bit, 4)
    v7 = Mem4[00123404:word32] + edx_2 + C_6
    Mem8[00123404:word32] = v7
    SCZO_9 = cond(v7)
    ...
```

Reko's "long add" transform will turn the code to:
```
    def dwArg04
    def dwArg08
*   edx_eax_9 = SEQ(dwArg08,dwArg04) 
*   eax_1 = SLICE(edx_eax_9, word32, 0) 
*   edx_2 = SLICE(edx_eax_9, word32, 32) 
*   v10 = Mem4[00123400:word64] + edx_eax_9
*   Mem7[00123400:word64] = v10
*   SCZO_8 = cond(v10)
```
The register sequence `edx_eax_9` was formed as a result of generating the long addition. What we want to do now is project the `SEQ(dwArg08,dwArg04)` statement "upstream" by locating the definitions of `dwArg08` and `dwArg04` and fusing them there. In this case, the variable definitions are `def` statements, implying they are passed in by a caller to the current procedure. After performing the projection transformation we will have:
```
*   def qwArg04
*   dwArg04 = SLICE(qwArg04, word32, 0)
*   dwArg08 = SLICE(qwArg04, word32, 32)
*   edx_eax_9 = qwArg04
    eax_1 = SLICE(edx_eax_9, word32, 0) 
    edx_2 = SLICE(edx_eax_9, word32, 32) 
    v10 = Mem0[00123400:word64] + edx_eax_9
    Mem7[00123400:word64] = v10
    SCZO_8 = cond(v10)
```

After further value propagation, dead code elimination and expression coalescing this becomes:
```
    def qwArg08
*   Mem7[00123400:word64] = Mem7[00123400:word64] + edx_eax_9
```

The final high-level code is:
```
    g_qw00123400 += edx_eax_9;
```

### Fusing segmented memory accesses

When dealing with x86 16-bit code, it's desirable to get rid of the infamous segmented register addressing. Ideally, we want to treat the (segment,offset) pairs in an X86 calculation as a far (long) pointer.
```
    les bx,ss:[bp-42]
    mov ax,es:[bx+04]
```

After rewriting, the example above becomes:
```
    es_bx_2 = dwLoc2A_1
    es_3 = SLICE(es_bx_2, word16, 16) 
    bx_4 = SLICE(es_bx_2, word16, 0) 
    ax_5 = Mem0[es_3:bx_4 + 0x0004:word16]
```

Here we can detect that `es_3` and `bx_4` are sliced from the same variable `es_bx_2`, and so we can transform to:
```
    es_bx_2 = dwLoc2A_1
    es_3 = SLICE(es_bx_2, word16, 16) 
    bx_4 = SLICE(es_bx_2, word16, 0) 
*   ax_5 = Mem0[es_bx_2 + 0x0004:word16]
```

The segmented memory access has been transformed into a linear memory access. After further transformations, this becomes:
```
*   ax_5 = Mem0[dwLoc2A_1 + 0x0004:word16]
```

or, in high-level language:
```
    ax_5 = dwLoc2A_1->w0004;
```

which is much more legible than what would have resulted without projection propagation:
```
    ax_5 = dwLoc2A_1.w0002->dwLoc2A_1.w0000->w0004;
```

### Fusing returned values

A particularity of x86 real mode code is that, according to most calling conventions, 32-bit values are returned in the dx:ax register pair. A common epilog of a program returning a 32-bit value looks like this:
```
    ...
    les ax,es:[bx+42]
    mov dx,es
    ret
```
The `mov dx,es` statement effectively copies the `es:ax` register pair into `dx:ax` in order to conform with the calling convention.

The calling code looks like this:
```
    ...
    call func_returning_32bit_value
    mov es,dx
    mov bx,ax
    mov ax,[es:bx+32]
    ...
```

Because neither the `dx` nor the `ax` register can be used when accessing memory, they must be copied over to `es:bx`. After rewriting to IR, SSA transformation and value propagation, the code fragments will look like this:
```
    ...
    es_ax_42 = Mem41[es_40:bx_32 + 42:word32]
    ax_43 = SLICE(es_ax_42, word16, 0)
    es_44 = SLICE(es_ax_42, word16, 16)
    dx_45 = SLICE(es_ax_42, word16, 16)
    return
exit_block:
    use ax_43
    use es_44
    use dx_45

    ...
    call func_returning_32bit_value
        def: es:es_14, ax:ax_15, dx:dx_16
    es_17 = dx_16
    bx_18 = ax_15
    ax_19 = Mem20[dx_16:ax_15 + 32:word16]
    ...
```

The projection propagation analysis discovers that `dx_16` and `ax_15` in the calling procedure are both defined in the same `call` statement. Therefore those two identifiers become candidates for fusing. After fusing `dx_16` and `ax_15` into `dx_ax_21`:
```
    ...
    call func_returning_32bit_value
*       def: es:es_14, dx_ax_21
*   es_17 = SLICE(dx_ax_21, word16, 16)
*   bx_18 = SLICE(dx_ax_21, word16, 0)
*   ax_19 = Mem20[dx_ax_21 + 32:word16]
    ...
```

The fusion of `dx_16` and `ax_15` into `dx_ax_21` of the `call` instruction tells us that this register pair is used as a single value in at least one procedure calling `func_returning_32bit_value`. We therefore propagate the register pair into the exit block of the callee procedure. This results in:
```
    ...
    es_ax_42 = Mem41[es_40:bx_32 + 42:word32]
    ax_43 = SLICE(es_ax_42, word16, 0)
    es_44 = SLICE(es_ax_42, word16, 16)
    dx_45 = SLICE(es_ax_42, word16, 16)
*   dx_ax_46 = SEQ(dx_45, ax_43)
    return
exit_block:
*   use dx_ax_46
    use es_44
```

Another projection propagation pushes `dx_ax_46` "upward", eliminating a `SEQ` operation:
```
    es_ax_42 = Mem41[es_40:bx_32 + 42:word32]
    ax_43 = SLICE(es_ax_42, word16, 0)
    es_44 = SLICE(es_ax_42, word16, 16)
    dx_45 = SLICE(es_ax_42, word16, 16)
*   dx_ax_46 = es_ax_42
    return
exit_block:
    use dx_ax_46
    use es_44
```

After an additional value propagation and dead variable elimination transformation, we end up with:
```
    ...
    es_ax_42 = Mem41[es_40:bx_32 + 42:word32]
    es_44 = SLICE(es_ax_42, word16, 16)
    dx_ax_46 = es_ax_42
    return
exit_block:
    use dx_ax_46
    use es_44

    ...
    call func_returning_32bit_value
        def: es:es_14, dx_ax_21
    ax_19 = Mem20[dx_ax_21 + 32:word16]
    ...
```

After further analysis and translation to high-level language:
```
    ...
    return es_bx_48->dw0042;

    ...
    ax_19 = func_returning_32bit_value()->w0032;
    ...
```

Because `func_returning_32bit_value` has changed the storages it trashes from `es,dx,ax` to `es,dx:ax`, all other callers to `func_returning_32bit_value` need to be re-analyzed.

### Avoiding over-eager fusion

There are cases when we should avoid fusing a register pair. 
```
    mov ax,ds:[si]
    add ax,ds:[di]
    ...
```
when rewritten to RTL / SSA becomes:
```
    ax_1 = Mem0[ds:si:word16]
    ax_2 = ax_1 + Mem0[ds:di:word16]
    ...
```

In this example the `ds` register is used in conjunction with the `si` and `di` registers. Unless we can prove that `si` and `di` are related by a constant offset, we must assume that `si` and `di` are separate "near pointers" (in x86 parlance) into the memory segment referred to by `ds`. In this case, `ds` needs to be treated as a separate identifier, and we cannot fuse the sequences. The end result will be:
```
    ax_2 = Mem0[ds:si:word16] + Mem0[ds:di:word16]
```

Once this is translated to high-level language, we have:
```
    ax_2 = ds->*si + ds->*di
```

## Sketch of approach

