Getting started
==
`File > Open` to open the ELF file.
(granted, the Reko user docs are nonexistent. Yet another thing on the list of "things to do" :) )
Once the file is open, look at the area to the left, the Project Browser.
You'll see the name of the loaded ELF file. Expand this node, and you'll see beneath it a node named "PowerPC". Click on that node.
The document area will now show the settings for PowerPC. Well, there is only one setting currently, the PowerPC model this binary is targeting
Remember that the ELF file header doesn't have enough information to tell us exactly which PowerPC instruction set is being targeted, so you the user have to help Reko out
Select the model "750".
At this point, you may wish to save a Reko project so you don't have to keep entering this model value over and over
So use File > Save to save a Reko project.
Now you're ready to scan the image. Press the Scan binaries button. Reko will start trying to locate the procedures in the binary. When I do this on my machine I get > 90% recovery which is pretty decent.
This takes a minute or two.
John Källén @uxmal 00:14
After that, you can browse around. Because of the large size of the image, browsing can be slow. This is a known issue and I'm considering ways to make the browsing faster
You can select procedures and change their signature (ie. specify their parameters and return types)
Then, you can press the same button (which now has changed name to Analyze dataflow. This tries to detect what registers / stack locations are used for inputs and outputs of each function. This phase takes a long time; it's trying to undo what the optimizing compiler did to generate the binary in the first place.
On my machine it took a good 15-20 minutes.
Once that step is done, Reko will have produced signatures for the procedures and "condensed" the expressions inside so that statements like:
a = b * 2
c = a + b
now look like
c = 3 * b
The final step is to determine data types and then structure the program into pseudo-C. This will also take a while
I should add the instructions above into a "users manual" should I not? :)
_
