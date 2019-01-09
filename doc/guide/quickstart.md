Getting started
==
So, you're one of the tl;dr crowd and can't be bother to read long tutorial? Here's some quick steps to get you started.

First, install reko as explained [here](reko.md). 

Start the GUI client.

Use `File > Open` to open an binary executable  file.

Once the file is open, look at the area to the left. That is the Project Browser. You'll see the name of the loaded binary file. Expand this node.

You'll see beneath it a node named "". Click on that node.
The document area will now show the settings for PowerPC. Well, there is only one setting currently, the PowerPC model this binary is targeting
Remember that the ELF file header doesn't have enough information to tell us exactly which PowerPC instruction set is being targeted, so you the user have to help Reko out
Select the model "750".
At this point, you may wish to save a Reko project so you don't have to keep entering this model value over and over

Use `File > Save` to save a Reko project.

Now you're ready to scan the file to find where the executable code is located in. Press the Scan binaries button. 

Reko will start trying to locate the procedures in the binary.

After that, you can use the Project Browser to look at the discovered procedures.

You can select procedures and change their name and signature (ie. specify their parameters and return types)

Then, press the Analyze  button dataflow. Reko will try to detect what registers / stack locations are used for inputs and outputs of each procedure.

Reko will have produced signatures for the procedures and "condensed" the expressions inside so that statements like:

```C
a = b * 2
c = a + b
```
now look like
```
c = 3 * b
```
The final step is pressing the <> button. This determines data types and then structures the program into pseudo-C.



