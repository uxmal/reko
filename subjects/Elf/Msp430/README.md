## Known issues

* At addres 0x59AE, the `bis.w r10,r15` ors together the two `void *` pointers of a `memcpy`, to determine whether either of their LSB's is 1. If this is not the case, then `memcpy`can use word-aligned copy instructions, otherwise it has to use byte-aligned instructions, which is slower. Because of this `or` instruction, the type analysis deduces that the input parameters containing the pointers cannot be pointers -- since we're doing violating the type rules of C -- but instead are integers. Now the `memcpy` signature has integer inputs. But all the callers in the program are passing actual pointers, so
the types are unified into a `union { A * u0; B * u1; ......, uint16 u100 }` mess.

To alleviate the problem we need to add a signature for `memcpy` here. It's difficult to say whether we should have Reko decide: "hey, this procedure is called `memcpy`, and the standard C library has a `memcpy(void *, void *, size_t)`, let's use that signature". It could be that someone writes a program that accidentally uses a name for a procedure that coincides with a standard C library function name, without it being the actual C function. I should be able to have a function that computes the factorial of a number called `printf` if I want to.

The other is to let the user discover the chaos, and resolve it by adding a type annotation.




