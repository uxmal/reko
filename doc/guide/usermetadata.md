# User Metadata
Sometimes Reko cannot recover the original program code while decompiling a binary file. The loss of information that occurs when compiling a high-level program to machine code is irreversible; many of the transformations done by optimizing compilers are not only irreversible, but also confuse Reko's decompilation algorithms. 

Often, a user's domain knowledge or understanding of the machine code can be provided to Reko to assist the decompiler in its generation of code. A user can supply metadata in various forms to assist Reko.

## Custom C header files with attributes
Reko supports adding C-syntax header files to the `.dcproject` file. These header files are written in plain C, with user metadata specified using C++ attribute syntax. All of the Reko attributes are written in the format:
```C++
[[reko::<attribute-name>(<arguments...>)]]
```

### Specifying the address of procedure
To specify the address of a procedure, use the followng syntax:
```C++
[[reko::address("012AC438")]] void my_function(int arg1, char * test);
```
Note that the address is specified as a hexadecimal string. For architectures with segmented addressing (like real-mode x86), use a colon to separate segment from offset:
```C++
[[reko::address("012A:C438")]] void my_function(int arg1, char * test);
```
Naturally, you can specify the exact types of the procedure's parameters and return value. Doing so will result in more accurate decompilation results.

### Specifying non-standard calling conventions
By default, Reko will use the standard calling convention of the processor architecture and the operating environment. However, in some Microsoft environments procedures may have varying calling conventions. These calling conventions are specified with the appropriate calling convention declarators:
```C++
[[reko::address("00123400")]] void __cdecl my_caller_cleanup_fn(int arg1, float arg2);
[[reko::address("00123410")]] void __pascal my_callee_cleanup_fn1(int arg1, float arg2);
[[reko::address("00123420")]] void __stdcall my_callee_cleanup_fn2(int arg1, float arg2);
[[reko::address("00123430")]] void __fastcall my_register_argument_fn(int arg1, float arg2);
```
On the SysV platform, the calling convention `i386kernel` is used to indicate the calling convention used by Linux i386 kernels. In this case, the `[[reko::convention]]` syntax is used:
```C++
[[reko::address("00123400")]] [[reko::convention(i387kernel)]] int my_kernel_syscall(char * arg1);
```

### Specifying non-standard argument and return registers
Some executables will contain hand-written machine code procedures that do not conform with any calling convention.
You can specify parameters and return values to use custom registers with the `[[reko::arg]]` attribute. This attribute specifies the kind of storage used for the parameter -- single register, sequence of registers, or fpu (for 8087 FPU stack) -- and the particular storage used for a parameter.

To specify a non-standard return register, use the `[[reko::returns]]` attribute and specify the name of the register as a string literal.

Note that if an non-standard argument or return value is present, appropriate `[[reko::arg]]` and/or `[[reko::returns]]` attributes must be present even on arguments that do comply with the default calling conventions.

Here are some examples:
```C++
// A procedure taking non-standard registers as input argument and returns
// a value in a non-standard register
[[reko::returns(register, "ah")]] char odd_msdos_procedure(
    [[reko::arg(register, "bx")]] char __near * stringPtr,
    [[reko::arg(register, "si")]] int intArg);

// A procedure taking the non-standard register sequence es:bx as input argument
// and returns a 32-bit value in the ds:ax register sequence.
[[reko::returns(seq, "dx", "ax")]] long read_int32_value(
    [[reko::arg(seq, "es", "bx")]] char __far * es_bx);

// A procedure taking two arguments on the 8087 FPU stack and returning a single value on the FPU stack.
// The net effect is shrinking the FPU stack by one item.
[[reko::returns(fpu)]] double _CIatan2([[reko::arg(fpu)]]double, [[reko::arg(fpu)]] double);
```

### Specifying procedure characteristics
Procedures may have characteristics that cannot be expressed in C/C++ but that might help Reko during decompilation. The `[[reko::characteristics]]` attribute can be used to express this type of extra-linguistic information. The syntax of this attribute is 
```C++
[[reko::characteristics({name1:value1,name2:value2...})]]
```
The name-value pairs are expressed using the same syntax as a TypeScript dictionary initializer.

#### Specifying a procedure as `alloca`
The following example shows how to indicate that a procedure is an implementation of alloca, which
allocates memory from the stack.
```C++ 
[[reko::address("00471100")]]
    [[reko::characteristics({alloca:true})]]
    [[reko::returns(register,"rsp")]]
char *alloca([[reko::arg(register,"rax")]] size_t ax );
```
When encountering calls to this procedure, Reko will substitute them with in-line adjustments of the stack pointer, statically mimicing the behaviour of `alloca`. The machine code fragment:
```
    mov rax,0x48
    call 00471100
    mov rbx,esp
```
allocates 0x48 bytes of memory and sets the `rbx` to point at that memory. With the `alloca` characterisic specified as above, the code will be rewritten to:
```
    rax = 0x48<64>
    rsp = rsp - 0x48<64>
    rbx = rsp
```

#### Specifying a non-returning procedure
The following example specifies a non-returning procedure:
```C++
[[reko::address("00123400")]]
    [[reko::characteristics({terminates:true})]]
exit(int exit_code);
```
Reko is sensitive to this characteristic and will use it during both the scanning phase and the data flow analysis phase.
**Note:**: C++ 11 defines the standard attribute `[[noreturn]]` which has the same semantics as `[[reko::characteristics({terminates:true})]]`. A future version of Reko will support `[[noreturn]]`.

### Specifying service procedures
A service procedure is a procedure where one or more of the input parameters are used to choose different functionality. The MS-DOS `int 21h` service vector is a well-known example of this technique. To specify a service procedure based on an interrupt or system call, use the `[[reko::service]]` attribute. This attribute specifies the interrupt or system call number, and the register(s) used to select a particular function.

The following example is a vastly simplified representation of the 32-bit i386 Linux `int 80h` service vector:
```C++
[[reko::service(vector=0x80, regs={eax:0x01})]]
    [[reko::convention(i386kernel)]]
    [[reko::characteristics({terminates:true})]]
int sys_exit(int error_code);

[[reko::service(vector=0x80, regs={eax:0x02})]]
    [[reko::convention(i386kernel)]]
int sys_fork();

[[reko::service(vector=0x80, regs={eax:0x03})]]
    [[reko::convention(i386kernel)]]
int sys_read(unsigned int fd, char *buf, size_t count);

[[reko::service(vector=0x80, regs={eax:0x04})]]
    [[reko::convention(i386kernel)]]
int sys_write(unsigned int fd, const char *buf, size_t count);

[[reko::service(vector=0x80, regs={eax:0x05})]]
    [[reko::convention(i386kernel)]] 
int sys_open(const char *filename, int flags, umode_t mode);

[[reko::service(vector=0x80, regs={eax:0x06})]]
    [[reko::convention(i386kernel)]]
int sys_close(unsigned int fd);
```
