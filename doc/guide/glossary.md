# Glossary

Jargon is inevitable in any sufficiently advanced field of endeavor. Here are some terms
frequently encountered when working with decompilation.

### Assembly language
A human-readable rendering of **machine code**. Assembly language
consists of statements, which in turn typically consist of an optional label, an operation
or pseudo-operation, and optional arguments:
```
; label             operation       operand(s)
program_start:      push            ebp
                    mov             ebp,esp
```

### Binary file
  A binary file, or simply **binary** is the input that Reko decompiles. These typically consist of machine
  code. Many binary file format contain additional metadata which assists the decompilation
  process by providing more specific information about the contents of a file. For instance,
  such metadata can specify that some sections of a binary are executable while others aren't;
  symbol information can specify the precise locations of objects the decompiler wants to
  discover.

### Intermediate representation
  Intermediate representation (IR) is a machine-indenpendent representation of executable code.
  Reko converts machine code from different architectures into this representation so that it can
  use common algorithms during the decompilation process.

### Machine code
  Machine code consists of instructions directly executable by a computer. The 
  instructions are encoded in an architecture specific way. On some architectures,
  every instruction is encoded in units of the same size (e.g. bytes, 16-bit
  words, 32-words); on other architectures, instructions can be encoded using
  sequences varying numbers of these units.

### Rewriting
Rewriting (called 'lifting' elsewhere) is the process of converting machine code to 
intermediate representation.



