// ls.h
// Generated on 2015-10-12 10:10:02 by decompiling ls
// using Decompiler version 0.5.4.0.

/*
// Equivalence classes ////////////
Eq_1: (struct "Globals")
	globals_t (in globals : (ptr (struct "Globals")))
Eq_2: (fn void (word64))
	T_2 (in __align : ptr64)
Eq_7: (fn void ())
	T_7 (in __hlt : ptr64)
// Type Variables ////////////
globals_t: (in globals : (ptr (struct "Globals")))
  Class: Eq_1
  DataType: (ptr Eq_1)
  OrigDataType: (ptr (struct "Globals"))
T_2: (in __align : ptr64)
  Class: Eq_2
  DataType: (ptr Eq_2)
  OrigDataType: (ptr (fn T_6 (T_5)))
T_3: (in fp : ptr64)
  Class: Eq_3
  DataType: ptr64
  OrigDataType: ptr64
T_4: (in 0x0000000000000008 : word64)
  Class: Eq_4
  DataType: word64
  OrigDataType: word64
T_5: (in fp + 0x0000000000000008 : word64)
  Class: Eq_5
  DataType: word64
  OrigDataType: word64
T_6: (in __align(fp + 0x0000000000000008) : void)
  Class: Eq_6
  DataType: void
  OrigDataType: void
T_7: (in __hlt : ptr64)
  Class: Eq_7
  DataType: (ptr Eq_7)
  OrigDataType: (ptr (fn T_8 ()))
T_8: (in __hlt() : void)
  Class: Eq_8
  DataType: void
  OrigDataType: void
*/
typedef struct Globals {
} Eq_1;

typedef void (Eq_2)(word64);

typedef void (Eq_7)();

