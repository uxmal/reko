// switch.h
// Generated on 2015-10-12 14:19:05 by decompiling D:\dev\uxmal\reko\master\subjects\regressions\snowman-51\switch.dll
// using Decompiler version 0.5.4.0.

/*
// Equivalence classes ////////////
Eq_1: (struct "Globals" (10072000 (str char) str10072000) (10072008 (str char) str10072008) (10072010 (str char) str10072010) (10072014 (str char) str10072014) (10072018 (str char) str10072018))
	globals_t (in globals : (ptr (struct "Globals")))
Eq_3: (union "Eq_3" (32 u0) (int32 u1) (up32 u2))
	T_3 (in n : Eq_3)
	T_4 (in 0xFFFFFFFE : word32)
// Type Variables ////////////
globals_t: (in globals : (ptr (struct "Globals")))
  Class: Eq_1
  DataType: (ptr Eq_1)
  OrigDataType: (ptr (struct "Globals"))
T_2: (in eax : (ptr char))
  Class: Eq_2
  DataType: (ptr char)
  OrigDataType: (ptr char)
T_3: (in n : Eq_3)
  Class: Eq_3
  DataType: Eq_3
  OrigDataType: (union (32 u0) (int32 u1))
T_4: (in 0xFFFFFFFE : word32)
  Class: Eq_3
  DataType: up32
  OrigDataType: up32
T_5: (in n > 0xFFFFFFFE : bool)
  Class: Eq_5
  DataType: bool
  OrigDataType: bool
T_6: (in 0x10072000 : ptr32)
  Class: Eq_2
  DataType: (ptr char)
  OrigDataType: ptr32
T_7: (in 0x00000001 : word32)
  Class: Eq_7
  DataType: word32
  OrigDataType: word32
T_8: (in n + 0x00000001 : word32)
  Class: Eq_8
  DataType: word32
  OrigDataType: word32
T_9: (in 0x10072018 : ptr32)
  Class: Eq_2
  DataType: (ptr char)
  OrigDataType: ptr32
T_10: (in 0x10072014 : ptr32)
  Class: Eq_2
  DataType: (ptr char)
  OrigDataType: ptr32
T_11: (in 0x10072010 : ptr32)
  Class: Eq_2
  DataType: (ptr char)
  OrigDataType: ptr32
T_12: (in 0x10072008 : ptr32)
  Class: Eq_2
  DataType: (ptr char)
  OrigDataType: ptr32
*/
typedef  Eq_1[][][][][]struct Globals {
	char str10072000[];	// 10072000
	char str10072008[];	// 10072008
	char str10072010[];	// 10072010
	char str10072014[];	// 10072014
	char str10072018[];	// 10072018
} Eq_1;

typedef union Eq_3 {
	32 u0;
	int32 u1;
	up32 u2;
} Eq_3;

