// flags3.h
// Generated by decompiling flags3
// using Reko decompiler version 0.12.1.0.

/*
// Equivalence classes ////////////
Eq_1: (struct "Globals")
	globals_t (in globals : (ptr64 (struct "Globals")))
// Type Variables ////////////
globals_t: (in globals : (ptr64 (struct "Globals")))
  Class: Eq_1
  DataType: (ptr64 Eq_1)
  OrigDataType: (ptr64 (struct "Globals"))
T_2: (in rdi : (ptr64 int32))
  Class: Eq_2
  DataType: (ptr64 int32)
  OrigDataType: (ptr64 (struct (0 T_5 t0000)))
T_3: (in 0<64> @ 0000000000000FB4 : word64)
  Class: Eq_3
  DataType: word64
  OrigDataType: word64
T_4: (in rdi + 0<64> @ 0000000000000FB4 : word64)
  Class: Eq_4
  DataType: word64
  OrigDataType: word64
T_5: (in Mem0[rdi + 0<64>:word32] @ 0000000000000FB4 : word32)
  Class: Eq_5
  DataType: int32
  OrigDataType: int32
T_6: (in 0x101<32> @ 0000000000000FB4 : word32)
  Class: Eq_5
  DataType: int32
  OrigDataType: int32
T_7: (in *rdi < 0x101<32> @ 0000000000000FB4 : bool)
  Class: Eq_7
  DataType: bool
  OrigDataType: bool
*/
typedef struct Globals {
} Eq_1;

