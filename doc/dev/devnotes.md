ELF plt's and byte signatures


The challenge of SSA and Type information
---
After scanning, we have translated machine code to IL. We now want to convert the resulting IL into SSA 
form to facilitate later analyses. The problem comes when we don't know how many parameters a function has, 
what registers it trashes, and how it affects the stack pointer after it returns. For instance, the x86 
processor has two different RET instructions: the first just pops the return address from the stack and jumps 
there; the other pops the return address, adjusts the stack pointer by a number specified int the machine 
instruction, and then jumps to the return address. The former is used by C routines, while the latter is 
commonly used by the Win32 API and Pascal compilers. To see tht difference:

int __cdecl foo(int arg) {
    return arg * 2;
}

int __stdcall bar(int arg) {
	return arg * 4;
}

int t4;
int t2;

int main() {
    t2 = foo(4);
	t4 = bar(4);
}

translates to:

foo proc
	mov eax,[esp+4]
	shl eax,1
	ret
	endp

bar proc
	mov eax,[esp+4]
	shl eax,2
	ret 4
	endp

t4 dd ?
t2 dd ?

main proc
	push 4
	call foo
	add esp,4
	push 4
	call bar
	ret
	endp

When scanning foo, Reko can detect that none of the return instructions in the 
scanned function has an argument, and thus the function is a __cdecl which 
expects the caller to clean up. When scanning bar, the decompiler can also 
detect that all of the return instructions have the same return argument (4), 
and thus the the function is a __stdcall with 4 bytes of stack parameters.

Calling through function pointers really messes this up:

int foo(int arg) {
	return arg * 4;
}

int (*pfn)(int);
int t4;

int main() {
	t4 = pfn(3);
}

foo proc ; as before
t4 dd ?

main proc
	mov eax, dword ptr [0011FACC]
	push 3
	call eax
	add esp, 4
	ret

What would be interesting is to perform some rudimentary type analysis _and_ symbolic evaluation _and_ dirty propagation _at the same time_. To do so:

	mov eax, [0011FACC]
		globals
			field @ 0011FACC, T1, type word32
		eax 
			<trash>
			typed T1
		esp
			fp
	push 3
		esp
			fp - 4
		mem[fp - 4]
			typed T2, type word32
	call eax
		eax
			typed T1, pfn

Now that eax is determined to be a pfn, it would be great if we could locate all constants of type T1. Suppose a TypeVariable consisted of:

class TypeVariable
{
	int number;
	EquivalenceClass eq;
}

class EquivalenceClass
{
	DataType dt
	List<Constants>
}

Then, whenever the data type of a TypeVariable becomes a ptr<T>, we can locate all the constants and use them as pointers. Similarly, if a constant is added, 
and we know it is of type ptr<T>, then add a field  T to globals. If a constant is added to a type memptr<Base, T> then add the field T to Base. So, for each (typeable) statement, we 
associate a type variable with it:

class Assignment
{
	Identifier Dst;
	Expression Src;
	TypeVariable TypeVar;
}

So what entities have TypeVars? Assignments do, as we don't have ssa yet. Identifiers can't because of the following:

	stm1: mov eax, [0011FACC]			; eax has T1
	// do something
	stm2: mov eax, [0011FAD0]			; eax now has T2, completely unrelated to T1!
Instead, during reaching definitions, eax:
	eax: def = stm1;
later
	eax: def = stm2;

Procedure Frames do:

	call	foo

	foo.Frame = { structure {
		mem, -4, word32
	}

Each procedure signature needs a list of all its callees, so that when a change in the signature is made, all the callees change automatically. In particular
if a particular register value is determined to be trashed on exit, all callees need to be updated to reflect this (I believe we do this today),

So, in a sample program:
	eax  = 0011FFAA
	ebx = m[eax + 0x10]
	call ebx
we would deduce:

stm1:	eax = 0011FFAA
		eax.Type = T1 => EQ1 { word32, constants = { 0011FFAA } }
		eax.Def = stm1
		eax.Value = 0011FFAA
		stm1.Type = T1 (not the type of 

stm2:	ebx = m[eax + 0x10]
		[[eax]] = T1 => EQ1 { ptr(struct(T2 @ 0x10), constants = { 0011FFAA } }
				  T2 => EQ2 { word32 }
		[[ebx]] = T2
		ebx.def = stm2
		ebx.Value = mem[0011FFAA + 0x10]  { 00220000 } symbolic evaluator could determine that there is a value at 0011FFCA if it is readonly.

stm3:	call ebx;
		Register ebx is used as a function pointer
		[[ebx]] = T2 => EQ2 { ptr(func) }. If ebx was a constant, then we have found a function pointer! An address!
			1) Scan the function immediately.
			2) defer to an expensive later pass.
		call site needs a sequence of ITypeables to encode the trashed registers from the callee.
		Suppose call ebx is known to trash eax, then the call site should look like
		{
			stackBefore = 4
			eax: <

More difficult example: object with a vtable.

1.		eax = malloc(10);
2.		m[eax] = offset vtbl			; ptr to vtable 

3.		ecx = m[eax]			; ecx points to vtable entry
4.		call  [ecx+4]			; call the second entry

1.		eax = malloc(10)
		eax.Type = T1 => EQ1 { ptr(struct(10)) }
		eax.Value= <invalid>
		eax.Def = stm1;
		stm1.Type = T1

2.		m[eax] = offset vtbl
		[[offset vtbl]] = T2 => EQ2 { word32, const [vtbl] }
		eax.Type = T1 => Eq1  { ptr(struct(10, 
									T2 @ 0x0000)) }
		stm2.Type = T2

3.		ecx = m[eax]
		ecx.Type = T2 => EQ2 { word32 }
		ecx.Def = stm3;
		ecx.Value = 

4.		call [ecx+4] 
		ecx is likely a pointer
		ecx.Type = t2 => EQ2 { ptr(struct(-, T3 @ 0x0004)), const vtbl }

		[[ecx+4]] = EQ3 { ptr(fn). } 

Harder still when segmented architectures are brought to bear:

		ds.Value = 0C00
1.	es_bx = m[ds:0000]
		type(es_bx) = T1 => EQ1 { word32, constants = fetch[0C00:0000] }
		def(es_bx) = 1
		value(es_bx) = <illegal>
2.	es_bx = m[es:bx+4]
		TryGet(es,bx) => es_bx

		type (es) = T2 => EQ2 { segment16 }
		type (bx) = T3 => EQ3 { memptr16(T2, struct(-, T4 @ 0x0004) }
		type ([[es:bx+4]] = T4 => EQ4 { word32 }


[Q]: discard the type analysis or keep it for later? Can't find array bounds until SSA has been done.

We can't associate type information with the identifiers in the program directly, so they will be kept in the dataflow field where today only
the symbolic value is kept. 

omterprocedural register analysis
==
We need to do IRA for a few reasons. Firstly, and importantly, we want to know what registers a procedure appears to modify from the perspective 
of its callers. This allows us to decouple the procedures from each other and allow per-proc analysis. Secondly, we wish to know what registers and stack
locations are _used_ by the procedures. By used, we mean here some actual computation is doe with them.. That is: a straight copy doesn't count
as a use.
		mov	eax,ecx
Doesn't really use ecx, it just provides it with a new alias eax. Only when a computation or load or store is done to we consider a use. So:
		mov eax,[ecx]
ecx is used.
		lea eax,[ecx +4]
		=>
		eax := ecx + 4
ecx is used again.