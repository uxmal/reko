# Projection propagation

Projection propagation is an analysis being developed for Reko to handle transform sequences of expressions combined with the `SEQ` operator and tame them to "simple" variables. Sequences occur frequently in architectures like Z80 and X86 where there are *register pairs* like the Z80 HL (consisting of the H and L registers) and the X86 CX register (consisting of CH and CL). In x86 code, sequences of expressions occur frequently due to the segmented addressing used in 16-bit processor modes. Expression sequences also occur where high precision computations are made with multiple registers joined together.

The goal of the projection propagation transform is to eliminate as many `SEQ` and `SLICE` operations as possible, to improve thereadability of the generated decompiled code.

## Motivating Examples

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

Note that in x86 real mode, neither the `dx` nor the `ax` register can be used to make an effective address. The register values must be copied over to `es:bx`. After rewriting to IR, SSA transformation and value propagation, the code fragments will look like this:
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


### Sequences of phi assignments

Fusing expression sequences that are propagated via phi assignments can resultin in fruitful code simplifications. In the following example, the values in the registers `ax` and `dx` are carried around in a loop:
```
    def ax
    def dx
    def cx
loop:
    ax_3 = PHI(ax, ax_2)
    dx_3 = PHI(dx, dx_2)
    cx_3 = PHI(cx, cx_2)
    if (cx_3 == 0) goto done
body:
 *  dx_ax_1 = SEQ(dx_3, ax_3)
    dx_ax_2 = dx_ax_1 + Mem[...]
    cx_2 = cx_3 - 1
    dx_2 = SLICE(dx_ax_2, word16, 16)
    ax_2 = SLICE(dx_ax_2, word16, 0)
    goto loop
    
```
The sequence `dx_ax_1` is defined by two `PHI` assigments for `dx_3` and `ax_3`. The two phi expressions are replaced with a single phi expression for the sequence, and two slicing expressions extracting the sub expressions. For each predecessor block, the algorithm inserts new `SEQ` statements. 
```
    def ax
    def dx
    def cx
*   dx_ax = SEQ(dx, ax)
loop:
*   dx_ax_1 = PHI(dx_ax, dx_ax_3)
*   ax_3 = SLICE(dx_ax_1, word16, 0)
*   dx_3 = SLICE(dx_ax_1, word16, 16)
    cx_3 = PHI(cx, cx_2)
    if (cx_3 == 0) goto done
body:
    dx_ax_2 = dx_ax_1 + Mem[...]
    cx_2 = cx_3 - 1
    dx_2 = SLICE(dx_ax_2, word16, 16)
    ax_2 = SLICE(dx_ax_2, word16, 0)
*   dx_ax_3 = SEQ(dx_2, ax_2) 
    goto loop
```
The new `SEQ` statements can be fed back into the algorithm and propagated further, yielding:
```
    def dx_ax
    def cx
*   ax = SLICE(dx_ax, word16, 0)
*   dx = SLICE(dx_ax, word16, 16)
loop:
    dx_ax_1 = PHI(dx_ax, dx_ax_3)
    ax_3 = SLICE(dx_ax_1, word16, 0)
    dx_3 = SLICE(dx_ax_1, word16, 16)
    cx_3 = PHI(cx, cx_2)
    if (cx_3 == 0) goto done
body:
    dx_ax_2 = dx_ax_1 + Mem[...]
    cx_2 = cx_3 - 1
    dx_2 = SLICE(dx_ax_2, word16, 16)
    ax_2 = SLICE(dx_ax_2, word16, 0)
*   dx_ax_3 = dx_ax_2
    goto loop
```
After value propagation and dead code elimination this becomes:
```
    def dx_ax
    def cx
loop:
    dx_ax_1 = PHI(dx_ax, dx_ax_2)
    cx_3 = PHI(cx, cx_2)
    if (cx_3 == 0) goto done
body:
    dx_ax_2 = dx_ax_1 + Mem[...]
    cx_2 = cx_3 - 1
    goto loop
```
And in high-level language:
```
void foo(word16 cx, word32 dx_ax) {
    while (cx != 3) {
        dx_ax += <some dereference>
        --cx
    }
}
```

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

In this example the `ds` register is used in conjunction with both the `si` and the `di` registers. Unless we can prove that `si` and `di` are related by a constant offset, we must assume that `si` and `di` are separate "near pointers" (in x86 parlance) into the memory segment referred to by `ds`. In this case, `ds` needs to be treated as a separate identifier, and we cannot fuse the sequences. The end result will be:
```
    ax_2 = Mem0[ds:si:word16] + Mem0[ds:di:word16]
```

Once this is translated to high-level language, we have:
```
    ax_2 = ds->*si + ds->*di
```

## Sketch of approach

The algorithm is implemented as an analysis stage followed by synthesis stage. The analysis examines the procedure in question to find all fusion candidate expressions. A candidate expression is:
* a `SEQ(x_0,x_1,...,x_n-1)` expression which fuses `n` expressions.
* a segmented memory access `Mem[seg:off]` or `Mem[seg:off+e]` expression, where `seg` and `off` are `SLICE` and `(cast)` expressions. It can be regarded as a special case of a `SEQ` expression for the purposes of this algorithm.

For each `SEQ` in the procedure, we trace the definition of all its subexpressions. If all these are defined in the same basic block, we generate a _fusion candidate_ node, which keeps track of all the definitions in the proposed fused sequence, and all `SEQ`s or `Mem`s using the proposed fused sequence. For example, the code 
```
block1:
    v_1 = ...
    v_2 = ...
...
block2:
    v_3 = SEQ(v_1, v_2)
    ...
block3:
    v_4 = SEQ(v_1, v_2)
```
would result in the candidate node, keyed on the definition set `{ v1, v2 }`:
```
fusion_candidate[{v_1, v_2}]:
    block: block1
    uses: { v_3, v_2}
```
We also need to keep track of, for each variable, which candidates it's involved in. 
```
v_1: [ fusion_candidate[{v_1, v_2}]
```
If a variable is involved in more than one fusion candidate, then we have to reject the fusion candidate and remove it from the set under consideration. All surviving fusion candidates are then used to rewrite the code according to the following rules:

### Def statements
Multiple `def` statements whose variables participate in a fusion candidate generate a `def` of a new fused identifier. The original `def` statements are replaced with `SLICE` operations:
```
    def v1
    def v2
    ...
    def vn
...
block_a:
    ... = SEQ(vn, ... v2, v1)
...
block_b:
    ... = SEQ(vn, ... v2, v1)
```
becomes
```
    def vn_v2_v1
    v1 = SLICE(vn_v2_v1, ...)
    v2 = SLICE(vn_v2_v1, ...)
    ...
    vn = SLICE(vn_v2_v1, ...)
...
block_a:
    ... = vn_v2_v1
...
block_b:
    ... = vn_v2_v1
```
The new definitions of the identifiers `v1`, `v2`, ... `vn` are now likely dead variables, since the analysis phase has discovered that those identifiers are solely used to build the same candidate. Any calls to the procedure will be affected as well. The caller:
```
    call foo
        use:v1,v2,..vn
```
will need to change to:
```
    vn_v2_v1 = SEQ(vn,v2,...,v1)
    call foo
        use:vn_v2_v1
```
The newly created `SEQ` in the caller can now be fed back into the algorithm.

### Assignments
Multiple assignments whose variables participate in a fusion candidate generate an assignment to a fresh identifier with the `SEQ` of the sources of the assignments. The original assignments are replaced with `SLICE` assignments:
```
    v1 = compute_v1
    v2 = compute_v2
    ...
    vn = compute_vn
...
block_a:
    ... = SEQ(vn, ... v2, v1)
...
block_b:
    ... = SEQ(vn, ... v2, v1)
```
becomes
```
    vn_v2_v1 = SEQ(compute_vn,compute_v2,compute_v1)
    v1 = SLICE(vn_v2_v1, ...)
    v2 = SLICE(vn_v2_v1, ...)
    ...
    vn = SLICE(vn_v2_v1, ...)
...
block_a:
    ... = vn_v2_v1 
...
block_b:
    ... = vn_v2_v1
```

### Phi expressions
Multiple `PHI` assignments involved in a fused sequence generate a single `PHI` assignment to a fresh identifier. The original `PHI` assignments are replaced by `SLICE` assignments. Additionally, new `SEQ` assignments are added to the end of each predecessor block of the `PHI` statements. These new `SEQ` assignments are fed back into the algorithm.
```
block_a:
    v1_a = compute_v1
    v2_a = compute_v2
    ...
    vn_a = compute_vn
...
block_b:
    v1_b = compute_v1
    v2_b = compute_v2
    ...
    vn_b = compute_vn
...
block_z:
    v1_z = compute_v1
    v2_z = compute_v2
    ...
    vn_z = compute_vn
...
block_join:
    v1 = PHI(v1_a, v1_b, ... v1_z)
    v2 = PHI(v2_a, v2_b, ... v2_z)
    ...
    vn = PHI(vn_a, vn_b, ... vn_z)
    ...
block_use:
    ... = SEQ(vn,...,v2,v1)
```
becomes
```
block_a:
    v1_a = compute_v1
    v2_a = compute_v2
    ...
    vn_a = compute_vn
    vn_v2_v1_a = SEQ(vn_a,...,v2_a,v1_a)
...
block_b:
    v1_b = compute_v1
    v2_b = compute_v2
    ...
    vn_b = compute_vn
    vn_v2_v1_b = SEQ(vn_b,...,v2_b,v1_b)
...
block_z:
    v1_z = compute_v1
    v2_z = compute_v2
    ...
    vn_z = compute_vn
    vn_v2_v1_z = SEQ(vn_z,...,v2_z,v1_z)
...
block_join:
    vn_v2_v1 = PHI(vn_v2_v1_a, vn_v2_v1_b, vn_v2_v1_c)
    v1 = SLICE(vn_v2_v1, ...)
    v2 = SLICE(vn_v2_v1, ...)
    ...
    vn = SLICE(vn_v2_v1, ...)
    ...
block_use:
    ... = vn_v2_v1
```

### Call instructions
Definitions in `call` instructions are fused. In the called function, the `use` statements of the components of the fused sequence are replaced with a single `use` of the fused identifier. New `SEQ` assignments are introduced in the exit blocks of the called function:
```
callee_proc()
    ...
callee_proc_exit:
    use v1
    use v2
    ...
    use vn

caller_proc()
    ...
    call callee_proc
        def: v1,v2,..,vn
    ...
    ... = SEQ(vn,...v2,v1)
```
becomes
```
callee_proc()
    ...
callee_proc_exit:
    vn_v2_v1 = SEQ(vn,...,v2,v1)
    use vn_v2_v1

caller_proc()
    ...
    call callee_proc
        def: vn_v2_v1
    ...
    ... = vn_v2_v1
```
The newly added `SEQ` expressions are fed back into the algorithm.

## Pseudocode

A rough pseudocode of this algorithm follows:
```Python
    # Build initial work list
    wl = {}
    for stm in proc:
        seq = find_seq(stm) # find a SEQ expression
        if seq:
            wl.add((stm, seq))
    used_in_candidate = { }
    fusion_candidates = { }
    rejected_candidates = { }
    while len(wl):
        while len(wl):
            (stm, seq) = next(wl)
            wl -= cand
            defStms = get_defining_statements(seq)
            if not all_stms_in_same_block(defStms):
                continue
            # Make a key from the variables in the SEQ
            key = key_from_seq_variables(seq.expressions)
            if fusion_candidates.contains(key):
                fusion_cand = fusion_candidates[key]
            else:
                # Make sure none of the variables in the SEQ
                # is used in another candidate
                for seq_id in seq.expressions:
                    if used_in_candidate.Contains(seq_id):
                        rejected_candidates.add(key)
                        continue
                fusion_cand = make_fusion_candidate(seq, stms)
                fusion_candidates.add(key, fusion_cand)
            fusion_candidate.usingSeqs.append(stm)

        # We now have a set of candidates to work with
        for fusion_cand in fusion_candidates:
            if rejected_candidates.contains(fusion_cand):
                continue
            new_seqs = modify_code(fusion_cand)
            # Newly discovered candidates are fed back into 
            # the algorithm.
            wl += new_seqs
```