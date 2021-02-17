Getting started
==
So, you're one of the tl;dr crowd and can't be bother to read long tutorial? Here's some quick steps to get you started.

First, install reko as explained [here](reko.md). 

Start the GUI client.

Use `File > Open` to open an binary executable  file.

Once the file is open, look at the area to the left. That is the Project Browser. You'll see the name of the loaded binary file. Expand this node. Depending on the kind of binary file you opened, you will see the various sections the binary file consists of. You can click on the sections in the Project Browser to quickly navigate to the start of each such section and look at both the data in the section and also a disassembly of that data using the processor architecture specified by the binary file.

Now you're ready to scan the file to find where the executable code is located in. Press the "Scan binaries" button. 

Reko will start trying to locate the procedures in the binary. It does so by starting at the program [entry points](glossary.md#entryPoint), and then simulates the execution of the machine code instructions one after another. When a `jump` machine code instruction is encountered, the scanner will stop scanning the current list of instructions and start scanning at the destination of the `jump`. If a `call` machine code instruction is encountered, the scanner will note that a procedure has been found, and recursively starts scanning the code starting at the destination of the `call`. This process repeats itself recursively until Reko stops finding new machine code instructions to simulate.

Once the scanner is done, you can use the "Procedure list" to browse the discovered procedures. You can select procedures and change their name and signature (ie. specify their parameters and return types) using the `Edit Signature...` context menu item. 

Then, press the "Analyze button". Reko will try to detect what registers / stack locations are used for inputs and outputs of each procedure.

Reko will have produced signatures for the procedures and "condensed" the expressions inside so that statements like:

```C
a = b * 2
c = a + b
```
now look like
```
c = 3 * b
```
The final step is pressing the "Reconstruct data types" button. This determines high-level data types and then structures all the procedures into code that is almost C/C++.




