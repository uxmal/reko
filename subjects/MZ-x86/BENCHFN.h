// BENCHFN.h
// Generated on 2015-08-20 15:08:17 by decompiling D:\dev\uxmal\reko\master\subjects\MZ-x86\BENCHFN.EXE
// using Decompiler version 0.5.1.0.

/*
// Equivalence classes ////////////
// Type Variables ////////////
globals_t: (in globals : (ptr (struct "Globals")))
  Class: Eq_1
  DataType: (ptr (struct "Globals"))
  OrigDataType: (ptr (struct "Globals"))
T_2: (in fp : ptr16)
  Class: Eq_2
  DataType: ptr16
  OrigDataType: ptr16
T_3: (in sp : word16)
  Class: Eq_2
  DataType: (union (uint16 u0) ((memptr T_18 (struct (0 T_218 t0000))) u1) ((memptr T_27 (struct (0 T_2 t0000))) u2) ((memptr T_30 (struct (0 T_285 t0000))) u3) ((union (ptr16 u0) ((memptr T_30 (struct (0 T_85 t0000) (2 T_52 t0002))) u1)) u4))
  OrigDataType: (union (uint16 u0) ((memptr T_18 (struct (0 T_218 t0000))) u1) ((memptr T_27 (struct (0 T_2 t0000))) u2) ((memptr T_30 (struct (0 T_285 t0000))) u3) ((union (ptr16 u0) ((memptr T_30 (struct (0 T_85 t0000) (2 T_52 t0002))) u1)) u4))
T_4: (in 0x09DB : word16)
  Class: Eq_4
  DataType: word16
  OrigDataType: word16
T_5: (in dx : word16)
  Class: Eq_4
  DataType: (ptr (segment (8A T_43 t008A) (8C T_34 t008C) (8E T_122 t008E) (90 T_32 t0090) (92 T_29 t0092) (96 T_37 t0096) (AC T_36 t00AC)))
  OrigDataType: (ptr (segment (8A T_43 t008A) (8C T_34 t008C) (8E T_122 t008E) (90 T_32 t0090) (92 T_29 t0092) (96 T_37 t0096) (AC T_36 t00AC)))
T_6: (in 0x09DB : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_7: (in 0x0800 : selector)
  Class: Eq_7
  DataType: (ptr (segment (1F8 T_2 t01F8) (330 T_655 t0330) (332 T_653 t0332)))
  OrigDataType: (ptr (segment (1F8 T_2 t01F8) (330 T_655 t0330) (332 T_653 t0332)))
T_8: (in 0x01F8 : word16)
  Class: Eq_8
  DataType: (memptr T_7 (struct (0 T_9 t0000)))
  OrigDataType: (memptr T_7 (struct (0 T_9 t0000)))
T_9: (in Mem0[0x0800:0x01F8:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_10: (in 0x30 : byte)
  Class: Eq_10
  DataType: byte
  OrigDataType: byte
T_11: (in ah : byte)
  Class: Eq_10
  DataType: byte
  OrigDataType: byte
T_12: (in msdos_get_dos_version : ptr32)
  Class: Eq_12
  DataType: (ptr (fn T_16 (T_15)))
  OrigDataType: (ptr (fn T_16 (T_15)))
T_13: (in signature of msdos_get_dos_version : void)
  Class: Eq_12
  DataType: 
  OrigDataType: 
T_14: (in ahOut : ptr16)
  Class: Eq_14
  DataType: ptr16
  OrigDataType: ptr16
T_15: (in out ah : ptr16)
  Class: Eq_14
  DataType: (ptr byte)
  OrigDataType: (ptr byte)
T_16: (in msdos_get_dos_version(out ah) : byte)
  Class: Eq_16
  DataType: byte
  OrigDataType: byte
T_17: (in al : byte)
  Class: Eq_16
  DataType: byte
  OrigDataType: byte
T_18: (in ds : selector)
  Class: Eq_2
  DataType: (ptr (segment (2 T_20 t0002) (2C T_23 t002C) (84 T_270 t0084) (86 T_263 t0086) (88 T_256 t0088) (98 T_247 t0098) (9A T_249 t009A) (5DA T_234 t05DA) (5DE T_251 t05DE)))
  OrigDataType: (ptr (segment (2 T_20 t0002) (2C T_23 t002C) (84 T_270 t0084) (86 T_263 t0086) (88 T_256 t0088) (98 T_247 t0098) (9A T_249 t009A) (5DA T_234 t05DA) (5DE T_251 t05DE)))
T_19: (in 0x0002 : word16)
  Class: Eq_19
  DataType: (memptr T_18 (struct (0 T_20 t0000)))
  OrigDataType: (memptr T_18 (struct (0 T_20 t0000)))
T_20: (in Mem0[ds:0x0002:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_21: (in bp : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_22: (in 0x002C : word16)
  Class: Eq_22
  DataType: (memptr T_18 (struct (0 T_23 t0000)))
  OrigDataType: (memptr T_18 (struct (0 T_23 t0000)))
T_23: (in Mem0[ds:0x002C:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_24: (in bx : word16)
  Class: Eq_2
  DataType: (union (uint16 u0) ((memptr T_18 (struct (0 T_218 t0000))) u1) ((memptr T_27 (struct (0 T_2 t0000))) u2) ((memptr T_30 (struct (0 T_285 t0000))) u3) ((union (ptr16 u0) ((memptr T_30 (struct (0 T_85 t0000) (2 T_52 t0002))) u1)) u4))
  OrigDataType: (union (uint16 u0) ((memptr T_18 (struct (0 T_218 t0000))) u1) ((memptr T_27 (struct (0 T_2 t0000))) u2) ((memptr T_30 (struct (0 T_285 t0000))) u3) ((union (ptr16 u0) ((memptr T_30 (struct (0 T_85 t0000) (2 T_52 t0002))) u1)) u4))
T_25: (in 0x09DB : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_26: (in ax : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_27: (in 0x09DB : selector)
  Class: Eq_4
  DataType: (ptr (segment (8A T_43 t008A) (8C T_34 t008C) (8E T_122 t008E) (90 T_2 t0090) (92 T_29 t0092) (96 T_37 t0096) (A4 T_173 t00A4) (A8 T_175 t00A8) (AC T_36 t00AC) (23A T_140 t023A) (23C T_2 t023C)))
  OrigDataType: (ptr (segment (8A T_43 t008A) (8C T_34 t008C) (8E T_122 t008E) (90 T_2 t0090) (92 T_29 t0092) (96 T_37 t0096) (A4 T_173 t00A4) (A8 T_175 t00A8) (AC T_36 t00AC) (23A T_140 t023A) (23C T_2 t023C)))
T_28: (in 0x0092 : word16)
  Class: Eq_28
  DataType: (memptr T_27 (struct (0 T_29 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_29 t0000)))
T_29: (in Mem0[0x09DB:0x0092:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_30: (in es : selector)
  Class: Eq_2
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_31: (in 0x0090 : word16)
  Class: Eq_31
  DataType: (memptr T_27 (struct (0 T_32 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_32 t0000)))
T_32: (in Mem0[0x09DB:0x0090:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_33: (in 0x008C : word16)
  Class: Eq_33
  DataType: (memptr T_27 (struct (0 T_34 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_34 t0000)))
T_34: (in Mem0[0x09DB:0x008C:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_35: (in 0x00AC : word16)
  Class: Eq_35
  DataType: (memptr T_27 (struct (0 T_36 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_36 t0000)))
T_36: (in Mem0[0x09DB:0x00AC:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_37: (in 0xFFFF : word16)
  Class: Eq_37
  DataType: word16
  OrigDataType: word16
T_38: (in 0x0096 : word16)
  Class: Eq_38
  DataType: (memptr T_27 (struct (0 T_39 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_39 t0000)))
T_39: (in Mem0[0x09DB:0x0096:word16] : word16)
  Class: Eq_37
  DataType: word16
  OrigDataType: word16
T_40: (in fn0800_0162 : ptr32)
  Class: Eq_40
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_41: (in signature of fn0800_0162 : void)
  Class: Eq_40
  DataType: 
  OrigDataType: 
T_42: (in 0x008A : word16)
  Class: Eq_42
  DataType: (memptr T_27 (struct (0 T_43 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_43 t0000)))
T_43: (in Mem0[0x09DB:0x008A:segptr32] : segptr32)
  Class: Eq_43
  DataType: segptr32
  OrigDataType: segptr32
T_44: (in es_di : ptr32)
  Class: Eq_43
  DataType: (union (ptr32 u0) (segptr32 u1))
  OrigDataType: (union (ptr32 u0) (segptr32 u1))
T_45: (in di : word16)
  Class: Eq_2
  DataType: (union (uint16 u0) ((memptr T_18 (struct (0 T_218 t0000))) u1) ((memptr T_27 (struct (0 T_2 t0000))) u2) ((memptr T_30 (struct (0 T_285 t0000))) u3) ((union (ptr16 u0) ((memptr T_30 (struct (0 T_85 t0000) (2 T_52 t0002))) u1)) u4))
  OrigDataType: (union (uint16 u0) ((memptr T_18 (struct (0 T_218 t0000))) u1) ((memptr T_27 (struct (0 T_2 t0000))) u2) ((memptr T_30 (struct (0 T_285 t0000))) u3) ((union (ptr16 u0) ((memptr T_30 (struct (0 T_85 t0000) (2 T_52 t0002))) u1)) u4))
T_46: (in 0x7FFF : word16)
  Class: Eq_43
  DataType: word16
  OrigDataType: word16
T_47: (in cx : word16)
  Class: Eq_43
  DataType: int16
  OrigDataType: int16
T_48: (in 0x0000 : word16)
  Class: Eq_43
  DataType: word16
  OrigDataType: word16
T_49: (in cx == 0x0000 : bool)
  Class: Eq_49
  DataType: bool
  OrigDataType: bool
T_50: (in 0x0002 : word16)
  Class: Eq_50
  DataType: word16
  OrigDataType: word16
T_51: (in di + 0x0002 : word16)
  Class: Eq_51
  DataType: word16
  OrigDataType: word16
T_52: (in Mem0[es:di + 0x0002:word16] : word16)
  Class: Eq_4
  DataType: word16
  OrigDataType: word16
T_53: (in dl : byte)
  Class: Eq_53
  DataType: byte
  OrigDataType: byte
T_54: (in 0x3D : byte)
  Class: Eq_54
  DataType: byte
  OrigDataType: byte
T_55: (in dl - 0x3D : byte)
  Class: Eq_55
  DataType: byte
  OrigDataType: byte
T_56: (in cond(dl - 0x3D) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_57: (in SCZO : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_58: (in Z : byte)
  Class: Eq_58
  DataType: byte
  OrigDataType: byte
T_59: (in Test(NE,Z) : bool)
  Class: Eq_59
  DataType: bool
  OrigDataType: bool
T_60: (in dh : byte)
  Class: Eq_60
  DataType: byte
  OrigDataType: byte
T_61: (in 0xDF : byte)
  Class: Eq_61
  DataType: byte
  OrigDataType: byte
T_62: (in dh & 0xDF : byte)
  Class: Eq_60
  DataType: byte
  OrigDataType: byte
T_63: (in 0x0096 : word16)
  Class: Eq_63
  DataType: (memptr T_27 (struct (0 T_37 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_37 t0000)))
T_64: (in Mem0[0x09DB:0x0096:word16] : word16)
  Class: Eq_37
  DataType: word16
  OrigDataType: word16
T_65: (in 0x0001 : word16)
  Class: Eq_65
  DataType: word16
  OrigDataType: word16
T_66: (in Mem0[0x09DB:0x0096:word16] + 0x0001 : word16)
  Class: Eq_37
  DataType: word16
  OrigDataType: word16
T_67: (in v20 : word16)
  Class: Eq_37
  DataType: word16
  OrigDataType: word16
T_68: (in Mem0[0x09DB:0x0096:word16] : word16)
  Class: Eq_37
  DataType: word16
  OrigDataType: word16
T_69: (in 0x59 : byte)
  Class: Eq_69
  DataType: byte
  OrigDataType: byte
T_70: (in dh - 0x59 : byte)
  Class: Eq_70
  DataType: byte
  OrigDataType: byte
T_71: (in cond(dh - 0x59) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_72: (in Test(NE,Z) : bool)
  Class: Eq_72
  DataType: bool
  OrigDataType: bool
T_73: (in 0x0096 : word16)
  Class: Eq_73
  DataType: (memptr T_27 (struct (0 T_37 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_37 t0000)))
T_74: (in Mem0[0x09DB:0x0096:word16] : word16)
  Class: Eq_37
  DataType: word16
  OrigDataType: word16
T_75: (in 0x0001 : word16)
  Class: Eq_75
  DataType: word16
  OrigDataType: word16
T_76: (in Mem0[0x09DB:0x0096:word16] + 0x0001 : word16)
  Class: Eq_37
  DataType: word16
  OrigDataType: word16
T_77: (in v21 : word16)
  Class: Eq_37
  DataType: word16
  OrigDataType: word16
T_78: (in Mem0[0x09DB:0x0096:word16] : word16)
  Class: Eq_37
  DataType: word16
  OrigDataType: word16
T_79: (in cond(v21) : byte)
  Class: Eq_79
  DataType: byte
  OrigDataType: byte
T_80: (in SZO : byte)
  Class: Eq_79
  DataType: byte
  OrigDataType: byte
T_81: (in 0x0000 : word16)
  Class: Eq_43
  DataType: word16
  OrigDataType: word16
T_82: (in cx == 0x0000 : bool)
  Class: Eq_82
  DataType: bool
  OrigDataType: bool
T_83: (in 0x0000 : word16)
  Class: Eq_83
  DataType: word16
  OrigDataType: word16
T_84: (in di + 0x0000 : word16)
  Class: Eq_84
  DataType: word16
  OrigDataType: word16
T_85: (in Mem0[es:di + 0x0000:byte] : byte)
  Class: Eq_85
  DataType: byte
  OrigDataType: byte
T_86: (in al - Mem0[es:di + 0x0000:byte] : byte)
  Class: Eq_86
  DataType: byte
  OrigDataType: byte
T_87: (in cond(al - Mem0[es:di + 0x0000:byte]) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_88: (in 0x0001 : word16)
  Class: Eq_88
  DataType: word16
  OrigDataType: word16
T_89: (in di + 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_90: (in 0x0001 : word16)
  Class: Eq_90
  DataType: word16
  OrigDataType: word16
T_91: (in cx - 0x0001 : word16)
  Class: Eq_43
  DataType: word16
  OrigDataType: word16
T_92: (in Test(NE,Z) : bool)
  Class: Eq_92
  DataType: bool
  OrigDataType: bool
T_93: (in 0x0001 : word16)
  Class: Eq_93
  DataType: word16
  OrigDataType: word16
T_94: (in bx + 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_95: (in 0x0000 : word16)
  Class: Eq_95
  DataType: word16
  OrigDataType: word16
T_96: (in di + 0x0000 : word16)
  Class: Eq_96
  DataType: word16
  OrigDataType: word16
T_97: (in Mem0[es:di + 0x0000:byte] : byte)
  Class: Eq_85
  DataType: byte
  OrigDataType: byte
T_98: (in Mem0[es:di + 0x0000:byte] - al : byte)
  Class: Eq_98
  DataType: byte
  OrigDataType: byte
T_99: (in cond(Mem0[es:di + 0x0000:byte] - al) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_100: (in Test(NE,Z) : bool)
  Class: Eq_100
  DataType: bool
  OrigDataType: bool
T_101: (in 0x0000 : word16)
  Class: Eq_101
  DataType: word16
  OrigDataType: word16
T_102: (in di + 0x0000 : word16)
  Class: Eq_102
  DataType: word16
  OrigDataType: word16
T_103: (in Mem0[es:di + 0x0000:word16] : word16)
  Class: Eq_85
  DataType: word16
  OrigDataType: word16
T_104: (in 0x3738 : word16)
  Class: Eq_104
  DataType: word16
  OrigDataType: word16
T_105: (in Mem0[es:di + 0x0000:word16] - 0x3738 : word16)
  Class: Eq_105
  DataType: word16
  OrigDataType: word16
T_106: (in cond(Mem0[es:di + 0x0000:word16] - 0x3738) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_107: (in Test(NE,Z) : bool)
  Class: Eq_107
  DataType: bool
  OrigDataType: bool
T_108: (in ch : byte)
  Class: Eq_108
  DataType: byte
  OrigDataType: byte
T_109: (in 0x80 : byte)
  Class: Eq_109
  DataType: byte
  OrigDataType: byte
T_110: (in ch | 0x80 : byte)
  Class: Eq_108
  DataType: byte
  OrigDataType: byte
T_111: (in -cx : word16)
  Class: Eq_43
  DataType: int16
  OrigDataType: int16
T_112: (in 0x008A : word16)
  Class: Eq_112
  DataType: (memptr T_27 (struct (0 T_113 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_113 t0000)))
T_113: (in Mem0[0x09DB:0x008A:word16] : word16)
  Class: Eq_43
  DataType: word16
  OrigDataType: word16
T_114: (in 0x0001 : word16)
  Class: Eq_43
  DataType: word16
  OrigDataType: word16
T_115: (in cl : byte)
  Class: Eq_115
  DataType: byte
  OrigDataType: byte
T_116: (in bx << cl : word16)
  Class: Eq_2
  DataType: ui16
  OrigDataType: ui16
T_117: (in 0x0008 : word16)
  Class: Eq_117
  DataType: word16
  OrigDataType: word16
T_118: (in bx + 0x0008 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_119: (in 0xFFF8 : word16)
  Class: Eq_119
  DataType: word16
  OrigDataType: word16
T_120: (in bx & 0xFFF8 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_121: (in 0x008E : word16)
  Class: Eq_121
  DataType: (memptr T_27 (struct (0 T_122 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_122 t0000)))
T_122: (in Mem0[0x09DB:0x008E:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_123: (in bp - 0x09DB : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_124: (in 0x023C : word16)
  Class: Eq_124
  DataType: (memptr T_27 (struct (0 T_125 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_125 t0000)))
T_125: (in Mem0[0x09DB:0x023C:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_126: (in 0x0200 : word16)
  Class: Eq_126
  DataType: word16
  OrigDataType: word16
T_127: (in di - 0x0200 : word16)
  Class: Eq_127
  DataType: word16
  OrigDataType: word16
T_128: (in cond(di - 0x0200) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_129: (in C : byte)
  Class: Eq_129
  DataType: bool
  OrigDataType: bool
T_130: (in Test(UGE,C) : bool)
  Class: Eq_130
  DataType: bool
  OrigDataType: bool
T_131: (in 0x062E : word16)
  Class: Eq_131
  DataType: word16
  OrigDataType: word16
T_132: (in di + 0x062E : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_133: (in cond(di) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_134: (in Test(ULT,C) : bool)
  Class: Eq_134
  DataType: bool
  OrigDataType: bool
T_135: (in 0x0200 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_136: (in 0x0200 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_137: (in 0x023C : word16)
  Class: Eq_137
  DataType: (memptr T_27 (struct (0 T_138 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_138 t0000)))
T_138: (in Mem0[0x09DB:0x023C:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_139: (in 0x023A : word16)
  Class: Eq_139
  DataType: (memptr T_27 (struct (0 T_140 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_140 t0000)))
T_140: (in Mem0[0x09DB:0x023A:word16] : word16)
  Class: Eq_140
  DataType: word16
  OrigDataType: word16
T_141: (in di + Mem0[0x09DB:0x023A:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_142: (in cond(di) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_143: (in Test(ULT,C) : bool)
  Class: Eq_143
  DataType: bool
  OrigDataType: bool
T_144: (in 0x04 : byte)
  Class: Eq_115
  DataType: byte
  OrigDataType: byte
T_145: (in 0x04 : byte)
  Class: Eq_145
  DataType: uint8
  OrigDataType: uint8
T_146: (in di >>u 0x04 : word16)
  Class: Eq_2
  DataType: uint16
  OrigDataType: uint16
T_147: (in 0x0001 : word16)
  Class: Eq_147
  DataType: word16
  OrigDataType: word16
T_148: (in di + 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_149: (in bp - di : word16)
  Class: Eq_149
  DataType: word16
  OrigDataType: word16
T_150: (in cond(bp - di) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_151: (in Test(ULT,C) : bool)
  Class: Eq_151
  DataType: bool
  OrigDataType: bool
T_152: (in 0x023C : word16)
  Class: Eq_152
  DataType: (memptr T_27 (struct (0 T_153 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_153 t0000)))
T_153: (in Mem0[0x09DB:0x023C:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_154: (in 0x0000 : word16)
  Class: Eq_154
  DataType: word16
  OrigDataType: word16
T_155: (in Mem0[0x09DB:0x023C:word16] - 0x0000 : word16)
  Class: Eq_155
  DataType: word16
  OrigDataType: word16
T_156: (in cond(Mem0[0x09DB:0x023C:word16] - 0x0000) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_157: (in Test(EQ,Z) : bool)
  Class: Eq_157
  DataType: bool
  OrigDataType: bool
T_158: (in 0x1000 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_159: (in 0x1000 : word16)
  Class: Eq_159
  DataType: word16
  OrigDataType: word16
T_160: (in bp - 0x1000 : word16)
  Class: Eq_160
  DataType: word16
  OrigDataType: word16
T_161: (in cond(bp - 0x1000) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_162: (in CZ : byte)
  Class: Eq_162
  DataType: byte
  OrigDataType: byte
T_163: (in Test(UGT,CZ) : bool)
  Class: Eq_163
  DataType: bool
  OrigDataType: bool
T_164: (in 0x023A : word16)
  Class: Eq_164
  DataType: (memptr T_27 (struct (0 T_165 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_165 t0000)))
T_165: (in Mem0[0x09DB:0x023A:word16] : word16)
  Class: Eq_140
  DataType: word16
  OrigDataType: word16
T_166: (in 0x0000 : word16)
  Class: Eq_166
  DataType: word16
  OrigDataType: word16
T_167: (in Mem0[0x09DB:0x023A:word16] - 0x0000 : word16)
  Class: Eq_167
  DataType: word16
  OrigDataType: word16
T_168: (in cond(Mem0[0x09DB:0x023A:word16] - 0x0000) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_169: (in Test(NE,Z) : bool)
  Class: Eq_169
  DataType: bool
  OrigDataType: bool
T_170: (in 0x09DB : word16)
  Class: Eq_170
  DataType: word16
  OrigDataType: word16
T_171: (in bx + 0x09DB : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_172: (in 0x00A4 : word16)
  Class: Eq_172
  DataType: (memptr T_27 (struct (0 T_173 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_173 t0000)))
T_173: (in Mem0[0x09DB:0x00A4:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_174: (in 0x00A8 : word16)
  Class: Eq_174
  DataType: (memptr T_27 (struct (0 T_175 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_175 t0000)))
T_175: (in Mem0[0x09DB:0x00A8:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_176: (in 0x0090 : word16)
  Class: Eq_176
  DataType: (memptr T_27 (struct (0 T_177 t0000)))
  OrigDataType: (memptr T_27 (struct (0 T_177 t0000)))
T_177: (in Mem0[0x09DB:0x0090:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_178: (in bx - ax : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_179: (in 0x4A : byte)
  Class: Eq_10
  DataType: byte
  OrigDataType: byte
T_180: (in 0x0002 : word16)
  Class: Eq_180
  DataType: word16
  OrigDataType: word16
T_181: (in fp - 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_182: (in wLoc02 : word16)
  Class: Eq_2
  DataType: (union (uint16 u0) ((memptr T_18 (struct (0 T_218 t0000))) u1) ((memptr T_27 (struct (0 T_2 t0000))) u2) ((memptr T_30 (struct (0 T_285 t0000))) u3) ((union (ptr16 u0) ((memptr T_30 (struct (0 T_85 t0000) (2 T_52 t0002))) u1)) u4))
  OrigDataType: (union (uint16 u0) ((memptr T_18 (struct (0 T_218 t0000))) u1) ((memptr T_27 (struct (0 T_2 t0000))) u2) ((memptr T_30 (struct (0 T_285 t0000))) u3) ((union (ptr16 u0) ((memptr T_30 (struct (0 T_85 t0000) (2 T_52 t0002))) u1)) u4))
T_183: (in msdos_resize_memory_block : ptr32)
  Class: Eq_183
  DataType: (ptr (fn T_189 (T_30, T_24, T_188)))
  OrigDataType: (ptr (fn T_189 (T_30, T_24, T_188)))
T_184: (in signature of msdos_resize_memory_block : void)
  Class: Eq_183
  DataType: 
  OrigDataType: 
T_185: (in es : selector)
  Class: Eq_2
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_186: (in bx : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_187: (in bxOut : ptr16)
  Class: Eq_187
  DataType: ptr16
  OrigDataType: ptr16
T_188: (in out bx : ptr16)
  Class: Eq_187
  DataType: (ptr (union (uint16 u0) ((memptr T_18 (struct (0 T_218 t0000))) u1) ((memptr T_27 (struct (0 T_2 t0000))) u2) ((memptr T_30 (struct (0 T_285 t0000))) u3) ((union (ptr16 u0) ((memptr T_30 (struct (0 T_85 t0000) (2 T_52 t0002))) u1)) u4)))
  OrigDataType: (ptr (union (uint16 u0) ((memptr T_18 (struct (0 T_218 t0000))) u1) ((memptr T_27 (struct (0 T_2 t0000))) u2) ((memptr T_30 (struct (0 T_285 t0000))) u3) ((union (ptr16 u0) ((memptr T_30 (struct (0 T_85 t0000) (2 T_52 t0002))) u1)) u4)))
T_189: (in msdos_resize_memory_block(es, bx, out bx) : bool)
  Class: Eq_129
  DataType: bool
  OrigDataType: bool
T_190: (in 0x04 : byte)
  Class: Eq_190
  DataType: byte
  OrigDataType: byte
T_191: (in di << 0x04 : word16)
  Class: Eq_2
  DataType: ui16
  OrigDataType: ui16
T_192: (in __cli : ptr32)
  Class: Eq_192
  DataType: (ptr (fn T_193 ()))
  OrigDataType: (ptr (fn T_193 ()))
T_193: (in __cli() : void)
  Class: Eq_193
  DataType: void
  OrigDataType: void
T_194: (in 0x09DB : word16)
  Class: Eq_194
  DataType: word16
  OrigDataType: word16
T_195: (in ss : selector)
  Class: Eq_194
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_196: (in 0x0000 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_197: (in 0x01F8 : word16)
  Class: Eq_197
  DataType: (memptr T_7 (struct (0 T_198 t0000)))
  OrigDataType: (memptr T_7 (struct (0 T_198 t0000)))
T_198: (in Mem0[0x0800:0x01F8:selector] : selector)
  Class: Eq_2
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_199: (in 0x05E8 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_200: (in 0x062E : word16)
  Class: Eq_43
  DataType: word16
  OrigDataType: word16
T_201: (in 0x0046 : word16)
  Class: Eq_43
  DataType: word16
  OrigDataType: word16
T_202: (in 0x0046 : word16)
  Class: Eq_202
  DataType: word16
  OrigDataType: word16
T_203: (in cond(0x0046) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_204: (in 0x001E : word16)
  Class: Eq_43
  DataType: word16
  OrigDataType: word16
T_205: (in 0x0056 : word16)
  Class: Eq_4
  DataType: word16
  OrigDataType: word16
T_206: (in 0x01F8 : word16)
  Class: Eq_206
  DataType: (memptr T_7 (struct (0 T_207 t0000)))
  OrigDataType: (memptr T_7 (struct (0 T_207 t0000)))
T_207: (in Mem0[0x0800:0x01F8:selector] : selector)
  Class: Eq_2
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_208: (in fn0800_01DA : ptr32)
  Class: Eq_208
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_209: (in signature of fn0800_01DA : void)
  Class: Eq_208
  DataType: 
  OrigDataType: 
T_210: (in 0x0003 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_211: (in 0x0002 : word16)
  Class: Eq_211
  DataType: word16
  OrigDataType: word16
T_212: (in fp - 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_213: (in 0x0003 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_214: (in fn0800_0121 : ptr32)
  Class: Eq_214
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_215: (in signature of fn0800_0121 : void)
  Class: Eq_214
  DataType: 
  OrigDataType: 
T_216: (in si : word16)
  Class: Eq_216
  DataType: word16
  OrigDataType: word16
T_217: (in bx + si : word16)
  Class: Eq_217
  DataType: word16
  OrigDataType: word16
T_218: (in Mem0[ds:bx + si:byte] : byte)
  Class: Eq_218
  DataType: byte
  OrigDataType: byte
T_219: (in 0x03 : byte)
  Class: Eq_219
  DataType: byte
  OrigDataType: byte
T_220: (in Mem0[ds:bx + si:byte] + 0x03 : byte)
  Class: Eq_218
  DataType: byte
  OrigDataType: byte
T_221: (in v27 : byte)
  Class: Eq_218
  DataType: byte
  OrigDataType: byte
T_222: (in bx + si : word16)
  Class: Eq_222
  DataType: word16
  OrigDataType: word16
T_223: (in Mem0[ds:bx + si:byte] : byte)
  Class: Eq_218
  DataType: byte
  OrigDataType: byte
T_224: (in cond(v27) : byte)
  Class: Eq_56
  DataType: byte
  OrigDataType: byte
T_225: (in 0x0004 : word16)
  Class: Eq_225
  DataType: word16
  OrigDataType: word16
T_226: (in fp - 0x0004 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_227: (in wLoc04 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_228: (in 0x0002 : word16)
  Class: Eq_228
  DataType: word16
  OrigDataType: word16
T_229: (in fp - 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_230: (in 0x0002 : word16)
  Class: Eq_230
  DataType: word16
  OrigDataType: word16
T_231: (in sp - 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_232: (in cs : selector)
  Class: Eq_232
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_233: (in 0x05DA : word16)
  Class: Eq_233
  DataType: (memptr T_18 (struct (0 T_234 t0000)))
  OrigDataType: (memptr T_18 (struct (0 T_234 t0000)))
T_234: (in Mem0[ds:0x05DA:word16] : word16)
  Class: Eq_234
  DataType: word16
  OrigDataType: word16
T_235: (in SEQ(cs, Mem0[ds:0x05DA:word16]) : ptr32)
  Class: Eq_235
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_236: (in fn0800_0336 : ptr32)
  Class: Eq_236
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_237: (in signature of fn0800_0336 : void)
  Class: Eq_236
  DataType: 
  OrigDataType: 
T_238: (in fn0800_0421 : ptr32)
  Class: Eq_238
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_239: (in signature of fn0800_0421 : void)
  Class: Eq_238
  DataType: 
  OrigDataType: 
T_240: (in 0x00 : byte)
  Class: Eq_10
  DataType: byte
  OrigDataType: byte
T_241: (in bios_get_system_time : ptr32)
  Class: Eq_241
  DataType: (ptr (fn T_245 (T_244)))
  OrigDataType: (ptr (fn T_245 (T_244)))
T_242: (in signature of bios_get_system_time : void)
  Class: Eq_241
  DataType: 
  OrigDataType: 
T_243: (in dxOut : ptr16)
  Class: Eq_243
  DataType: ptr16
  OrigDataType: ptr16
T_244: (in out dx : ptr16)
  Class: Eq_243
  DataType: (ptr (ptr (segment (8A T_43 t008A) (8C T_34 t008C) (8E T_122 t008E) (90 T_32 t0090) (92 T_29 t0092) (96 T_37 t0096) (AC T_36 t00AC))))
  OrigDataType: (ptr (ptr (segment (8A T_43 t008A) (8C T_34 t008C) (8E T_122 t008E) (90 T_32 t0090) (92 T_29 t0092) (96 T_37 t0096) (AC T_36 t00AC))))
T_245: (in bios_get_system_time(out dx) : word16)
  Class: Eq_43
  DataType: word16
  OrigDataType: word16
T_246: (in 0x0098 : word16)
  Class: Eq_246
  DataType: (memptr T_18 (struct (0 T_247 t0000)))
  OrigDataType: (memptr T_18 (struct (0 T_247 t0000)))
T_247: (in Mem0[ds:0x0098:word16] : word16)
  Class: Eq_4
  DataType: word16
  OrigDataType: word16
T_248: (in 0x009A : word16)
  Class: Eq_248
  DataType: (memptr T_18 (struct (0 T_249 t0000)))
  OrigDataType: (memptr T_18 (struct (0 T_249 t0000)))
T_249: (in Mem0[ds:0x009A:word16] : word16)
  Class: Eq_43
  DataType: word16
  OrigDataType: word16
T_250: (in 0x05DE : word16)
  Class: Eq_250
  DataType: (memptr T_18 (struct (0 T_251 t0000)))
  OrigDataType: (memptr T_18 (struct (0 T_251 t0000)))
T_251: (in Mem0[ds:0x05DE:word16] : word16)
  Class: Eq_251
  DataType: word16
  OrigDataType: word16
T_252: (in SEQ(cs, Mem0[ds:0x05DE:word16]) : ptr32)
  Class: Eq_252
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_253: (in 0x0002 : word16)
  Class: Eq_253
  DataType: word16
  OrigDataType: word16
T_254: (in sp - 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_255: (in 0x0088 : word16)
  Class: Eq_255
  DataType: (memptr T_18 (struct (0 T_256 t0000)))
  OrigDataType: (memptr T_18 (struct (0 T_256 t0000)))
T_256: (in Mem0[ds:0x0088:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_257: (in 0x0000 : word16)
  Class: Eq_257
  DataType: word16
  OrigDataType: word16
T_258: (in sp + 0x0000 : word16)
  Class: Eq_258
  DataType: word16
  OrigDataType: word16
T_259: (in Mem0[0x09DB:sp + 0x0000:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_260: (in 0x0002 : word16)
  Class: Eq_260
  DataType: word16
  OrigDataType: word16
T_261: (in sp - 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_262: (in 0x0086 : word16)
  Class: Eq_262
  DataType: (memptr T_18 (struct (0 T_263 t0000)))
  OrigDataType: (memptr T_18 (struct (0 T_263 t0000)))
T_263: (in Mem0[ds:0x0086:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_264: (in 0x0000 : word16)
  Class: Eq_264
  DataType: word16
  OrigDataType: word16
T_265: (in sp + 0x0000 : word16)
  Class: Eq_265
  DataType: word16
  OrigDataType: word16
T_266: (in Mem0[0x09DB:sp + 0x0000:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_267: (in 0x0002 : word16)
  Class: Eq_267
  DataType: word16
  OrigDataType: word16
T_268: (in sp - 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_269: (in 0x0084 : word16)
  Class: Eq_269
  DataType: (memptr T_18 (struct (0 T_270 t0000)))
  OrigDataType: (memptr T_18 (struct (0 T_270 t0000)))
T_270: (in Mem0[ds:0x0084:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_271: (in 0x0000 : word16)
  Class: Eq_271
  DataType: word16
  OrigDataType: word16
T_272: (in sp + 0x0000 : word16)
  Class: Eq_272
  DataType: word16
  OrigDataType: word16
T_273: (in Mem0[0x09DB:sp + 0x0000:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_274: (in fn0800_0265 : ptr32)
  Class: Eq_274
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_275: (in signature of fn0800_0265 : void)
  Class: Eq_274
  DataType: 
  OrigDataType: 
T_276: (in 0x0002 : word16)
  Class: Eq_276
  DataType: word16
  OrigDataType: word16
T_277: (in sp - 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_278: (in 0x0000 : word16)
  Class: Eq_278
  DataType: word16
  OrigDataType: word16
T_279: (in sp + 0x0000 : word16)
  Class: Eq_279
  DataType: word16
  OrigDataType: word16
T_280: (in Mem0[0x09DB:sp + 0x0000:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_281: (in fn0800_0301 : ptr32)
  Class: Eq_281
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_282: (in signature of fn0800_0301 : void)
  Class: Eq_281
  DataType: 
  OrigDataType: 
T_283: (in 0x0000 : word16)
  Class: Eq_283
  DataType: word16
  OrigDataType: word16
T_284: (in di + 0x0000 : word16)
  Class: Eq_284
  DataType: word16
  OrigDataType: word16
T_285: (in Mem0[es:di + 0x0000:byte] : byte)
  Class: Eq_16
  DataType: byte
  OrigDataType: byte
T_286: (in 0x0001 : word16)
  Class: Eq_286
  DataType: word16
  OrigDataType: word16
T_287: (in di + 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_288: (in 0x0001 : word16)
  Class: Eq_288
  DataType: word16
  OrigDataType: word16
T_289: (in cx - 0x0001 : word16)
  Class: Eq_43
  DataType: word16
  OrigDataType: word16
T_290: (in 0x0000 : word16)
  Class: Eq_43
  DataType: word16
  OrigDataType: word16
T_291: (in cx == 0x0000 : bool)
  Class: Eq_291
  DataType: bool
  OrigDataType: bool
T_292: (in fn0800_0121 : ptr32)
  Class: Eq_214
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_293: (in fp : ptr16)
  Class: Eq_293
  DataType: ptr16
  OrigDataType: ptr16
T_294: (in sp : word16)
  Class: Eq_293
  DataType: ptr16
  OrigDataType: ptr16
T_295: (in 0x01F8 : word16)
  Class: Eq_295
  DataType: (memptr T_7 (struct (0 T_296 t0000)))
  OrigDataType: (memptr T_7 (struct (0 T_296 t0000)))
T_296: (in Mem0[0x0800:0x01F8:selector] : selector)
  Class: Eq_2
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_297: (in ds : selector)
  Class: Eq_2
  DataType: (ptr (segment (5DC T_304 t05DC)))
  OrigDataType: (ptr (segment (5DC T_304 t05DC)))
T_298: (in fn0800_01A5 : ptr32)
  Class: Eq_298
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_299: (in signature of fn0800_01A5 : void)
  Class: Eq_298
  DataType: 
  OrigDataType: 
T_300: (in 0x0002 : word16)
  Class: Eq_300
  DataType: word16
  OrigDataType: word16
T_301: (in fp - 0x0002 : word16)
  Class: Eq_293
  DataType: word16
  OrigDataType: word16
T_302: (in cs : selector)
  Class: Eq_302
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_303: (in 0x05DC : word16)
  Class: Eq_303
  DataType: (memptr T_297 (struct (0 T_304 t0000)))
  OrigDataType: (memptr T_297 (struct (0 T_304 t0000)))
T_304: (in Mem0[ds:0x05DC:word16] : word16)
  Class: Eq_304
  DataType: word16
  OrigDataType: word16
T_305: (in SEQ(cs, Mem0[ds:0x05DC:word16]) : ptr32)
  Class: Eq_305
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_306: (in 0x0000 : word16)
  Class: Eq_306
  DataType: word16
  OrigDataType: word16
T_307: (in ax : word16)
  Class: Eq_306
  DataType: word16
  OrigDataType: word16
T_308: (in 0x0000 : word16)
  Class: Eq_308
  DataType: word16
  OrigDataType: word16
T_309: (in cond(0x0000) : byte)
  Class: Eq_309
  DataType: byte
  OrigDataType: byte
T_310: (in SZO : byte)
  Class: Eq_309
  DataType: byte
  OrigDataType: byte
T_311: (in false : bool)
  Class: Eq_311
  DataType: bool
  OrigDataType: bool
T_312: (in C : byte)
  Class: Eq_311
  DataType: bool
  OrigDataType: bool
T_313: (in 0x0000 : word16)
  Class: Eq_313
  DataType: word16
  OrigDataType: word16
T_314: (in si : word16)
  Class: Eq_313
  DataType: (memptr T_297 (struct (0 T_322 t0000)))
  OrigDataType: (memptr T_297 (struct (0 T_322 t0000)))
T_315: (in 0x002F : word16)
  Class: Eq_315
  DataType: word16
  OrigDataType: word16
T_316: (in cx : word16)
  Class: Eq_315
  DataType: word16
  OrigDataType: word16
T_317: (in false : bool)
  Class: Eq_317
  DataType: bool
  OrigDataType: bool
T_318: (in D : byte)
  Class: Eq_317
  DataType: bool
  OrigDataType: bool
T_319: (in al : byte)
  Class: Eq_319
  DataType: byte
  OrigDataType: byte
T_320: (in 0x0000 : word16)
  Class: Eq_320
  DataType: word16
  OrigDataType: word16
T_321: (in si + 0x0000 : word16)
  Class: Eq_321
  DataType: word16
  OrigDataType: word16
T_322: (in Mem0[ds:si + 0x0000:byte] : byte)
  Class: Eq_322
  DataType: byte
  OrigDataType: byte
T_323: (in al + Mem0[ds:si + 0x0000:byte] : byte)
  Class: Eq_319
  DataType: byte
  OrigDataType: byte
T_324: (in cond(al) : byte)
  Class: Eq_324
  DataType: byte
  OrigDataType: byte
T_325: (in SCZO : byte)
  Class: Eq_324
  DataType: byte
  OrigDataType: byte
T_326: (in ah : byte)
  Class: Eq_326
  DataType: (union (byte u0) (word16 u1))
  OrigDataType: (union (byte u0) (word16 u1))
T_327: (in 0x00 : byte)
  Class: Eq_327
  DataType: byte
  OrigDataType: byte
T_328: (in ah + 0x00 : word16)
  Class: Eq_328
  DataType: word16
  OrigDataType: word16
T_329: (in ah + 0x00 + C : word16)
  Class: Eq_326
  DataType: word16
  OrigDataType: word16
T_330: (in cond(ah) : byte)
  Class: Eq_324
  DataType: byte
  OrigDataType: byte
T_331: (in 0x0001 : word16)
  Class: Eq_331
  DataType: word16
  OrigDataType: word16
T_332: (in si + 0x0001 : word16)
  Class: Eq_313
  DataType: word16
  OrigDataType: word16
T_333: (in cond(si) : byte)
  Class: Eq_309
  DataType: byte
  OrigDataType: byte
T_334: (in 0x0001 : word16)
  Class: Eq_334
  DataType: word16
  OrigDataType: word16
T_335: (in cx - 0x0001 : word16)
  Class: Eq_315
  DataType: word16
  OrigDataType: word16
T_336: (in 0x0000 : word16)
  Class: Eq_315
  DataType: word16
  OrigDataType: word16
T_337: (in cx != 0x0000 : bool)
  Class: Eq_337
  DataType: bool
  OrigDataType: bool
T_338: (in 0x0D37 : word16)
  Class: Eq_338
  DataType: word16
  OrigDataType: word16
T_339: (in ax - 0x0D37 : word16)
  Class: Eq_306
  DataType: word16
  OrigDataType: word16
T_340: (in cond(ax) : byte)
  Class: Eq_324
  DataType: byte
  OrigDataType: byte
T_341: (in Z : byte)
  Class: Eq_341
  DataType: byte
  OrigDataType: byte
T_342: (in Test(EQ,Z) : bool)
  Class: Eq_342
  DataType: bool
  OrigDataType: bool
T_343: (in fp - 0x0002 : word16)
  Class: Eq_343
  DataType: word16
  OrigDataType: word16
T_344: (in bp : word16)
  Class: Eq_343
  DataType: word16
  OrigDataType: word16
T_345: (in 0x4C : byte)
  Class: Eq_326
  DataType: byte
  OrigDataType: byte
T_346: (in bArg00 : byte)
  Class: Eq_319
  DataType: byte
  OrigDataType: byte
T_347: (in msdos_terminate : ptr32)
  Class: Eq_347
  DataType: (ptr (fn T_350 (T_319)))
  OrigDataType: (ptr (fn T_350 (T_319)))
T_348: (in signature of msdos_terminate : void)
  Class: Eq_347
  DataType: 
  OrigDataType: 
T_349: (in al : byte)
  Class: Eq_319
  DataType: byte
  OrigDataType: byte
T_350: (in msdos_terminate(al) : void)
  Class: Eq_350
  DataType: void
  OrigDataType: void
T_351: (in 0x0019 : word16)
  Class: Eq_315
  DataType: word16
  OrigDataType: word16
T_352: (in 0x002F : word16)
  Class: Eq_352
  DataType: word16
  OrigDataType: word16
T_353: (in dx : word16)
  Class: Eq_352
  DataType: word16
  OrigDataType: word16
T_354: (in fn0800_01DA : ptr32)
  Class: Eq_208
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_355: (in fp : ptr16)
  Class: Eq_355
  DataType: ptr16
  OrigDataType: ptr16
T_356: (in sp : word16)
  Class: Eq_355
  DataType: ptr16
  OrigDataType: ptr16
T_357: (in 0x0002 : word16)
  Class: Eq_357
  DataType: word16
  OrigDataType: word16
T_358: (in fp - 0x0002 : word16)
  Class: Eq_355
  DataType: word16
  OrigDataType: word16
T_359: (in ds : selector)
  Class: Eq_359
  DataType: (ptr (segment (74 T_369 t0074) (76 T_372 t0076) (78 T_376 t0078) (7A T_378 t007A) (7C T_382 t007C) (7E T_384 t007E) (80 T_388 t0080) (82 T_390 t0082)))
  OrigDataType: (ptr (segment (74 T_369 t0074) (76 T_372 t0076) (78 T_376 t0078) (7A T_378 t007A) (7C T_382 t007C) (7E T_384 t007E) (80 T_388 t0080) (82 T_390 t0082)))
T_360: (in wLoc02 : word16)
  Class: Eq_359
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_361: (in msdos_get_interrupt_vector : ptr32)
  Class: Eq_361
  DataType: (ptr (fn T_365 (T_364)))
  OrigDataType: (ptr (fn T_365 (T_364)))
T_362: (in signature of msdos_get_interrupt_vector : void)
  Class: Eq_361
  DataType: 
  OrigDataType: 
T_363: (in al : byte)
  Class: Eq_363
  DataType: byte
  OrigDataType: byte
T_364: (in al : byte)
  Class: Eq_363
  DataType: byte
  OrigDataType: byte
T_365: (in msdos_get_interrupt_vector(al) : word32)
  Class: Eq_365
  DataType: word32
  OrigDataType: word32
T_366: (in es_bx : word32)
  Class: Eq_365
  DataType: word32
  OrigDataType: word32
T_367: (in bx : word16)
  Class: Eq_367
  DataType: word16
  OrigDataType: word16
T_368: (in 0x0074 : word16)
  Class: Eq_368
  DataType: (memptr T_359 (struct (0 T_369 t0000)))
  OrigDataType: (memptr T_359 (struct (0 T_369 t0000)))
T_369: (in Mem0[ds:0x0074:word16] : word16)
  Class: Eq_367
  DataType: word16
  OrigDataType: word16
T_370: (in es : selector)
  Class: Eq_370
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_371: (in 0x0076 : word16)
  Class: Eq_371
  DataType: (memptr T_359 (struct (0 T_372 t0000)))
  OrigDataType: (memptr T_359 (struct (0 T_372 t0000)))
T_372: (in Mem0[ds:0x0076:word16] : word16)
  Class: Eq_370
  DataType: word16
  OrigDataType: word16
T_373: (in msdos_get_interrupt_vector : ptr32)
  Class: Eq_361
  DataType: (ptr (fn T_374 (T_364)))
  OrigDataType: (ptr (fn T_374 (T_364)))
T_374: (in msdos_get_interrupt_vector(al) : word32)
  Class: Eq_365
  DataType: word32
  OrigDataType: word32
T_375: (in 0x0078 : word16)
  Class: Eq_375
  DataType: (memptr T_359 (struct (0 T_376 t0000)))
  OrigDataType: (memptr T_359 (struct (0 T_376 t0000)))
T_376: (in Mem0[ds:0x0078:word16] : word16)
  Class: Eq_367
  DataType: word16
  OrigDataType: word16
T_377: (in 0x007A : word16)
  Class: Eq_377
  DataType: (memptr T_359 (struct (0 T_378 t0000)))
  OrigDataType: (memptr T_359 (struct (0 T_378 t0000)))
T_378: (in Mem0[ds:0x007A:word16] : word16)
  Class: Eq_370
  DataType: word16
  OrigDataType: word16
T_379: (in msdos_get_interrupt_vector : ptr32)
  Class: Eq_361
  DataType: (ptr (fn T_380 (T_364)))
  OrigDataType: (ptr (fn T_380 (T_364)))
T_380: (in msdos_get_interrupt_vector(al) : word32)
  Class: Eq_365
  DataType: word32
  OrigDataType: word32
T_381: (in 0x007C : word16)
  Class: Eq_381
  DataType: (memptr T_359 (struct (0 T_382 t0000)))
  OrigDataType: (memptr T_359 (struct (0 T_382 t0000)))
T_382: (in Mem0[ds:0x007C:word16] : word16)
  Class: Eq_367
  DataType: word16
  OrigDataType: word16
T_383: (in 0x007E : word16)
  Class: Eq_383
  DataType: (memptr T_359 (struct (0 T_384 t0000)))
  OrigDataType: (memptr T_359 (struct (0 T_384 t0000)))
T_384: (in Mem0[ds:0x007E:word16] : word16)
  Class: Eq_370
  DataType: word16
  OrigDataType: word16
T_385: (in msdos_get_interrupt_vector : ptr32)
  Class: Eq_361
  DataType: (ptr (fn T_386 (T_364)))
  OrigDataType: (ptr (fn T_386 (T_364)))
T_386: (in msdos_get_interrupt_vector(al) : word32)
  Class: Eq_365
  DataType: word32
  OrigDataType: word32
T_387: (in 0x0080 : word16)
  Class: Eq_387
  DataType: (memptr T_359 (struct (0 T_388 t0000)))
  OrigDataType: (memptr T_359 (struct (0 T_388 t0000)))
T_388: (in Mem0[ds:0x0080:word16] : word16)
  Class: Eq_367
  DataType: word16
  OrigDataType: word16
T_389: (in 0x0082 : word16)
  Class: Eq_389
  DataType: (memptr T_359 (struct (0 T_390 t0000)))
  OrigDataType: (memptr T_359 (struct (0 T_390 t0000)))
T_390: (in Mem0[ds:0x0082:word16] : word16)
  Class: Eq_370
  DataType: word16
  OrigDataType: word16
T_391: (in 0x2500 : word16)
  Class: Eq_391
  DataType: word16
  OrigDataType: word16
T_392: (in ax : word16)
  Class: Eq_391
  DataType: word16
  OrigDataType: word16
T_393: (in cs : selector)
  Class: Eq_359
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_394: (in dx : word16)
  Class: Eq_359
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_395: (in 0x0158 : word16)
  Class: Eq_359
  DataType: word16
  OrigDataType: word16
T_396: (in msdos_set_interrupt_vector : ptr32)
  Class: Eq_396
  DataType: (ptr (fn T_401 (T_364, T_400)))
  OrigDataType: (ptr (fn T_401 (T_364, T_400)))
T_397: (in signature of msdos_set_interrupt_vector : void)
  Class: Eq_396
  DataType: 
  OrigDataType: 
T_398: (in al : byte)
  Class: Eq_363
  DataType: byte
  OrigDataType: byte
T_399: (in ds_dx : word32)
  Class: Eq_399
  DataType: word32
  OrigDataType: word32
T_400: (in ds_dx : word32)
  Class: Eq_399
  DataType: word32
  OrigDataType: word32
T_401: (in msdos_set_interrupt_vector(al, ds_dx) : void)
  Class: Eq_401
  DataType: void
  OrigDataType: void
T_402: (in fp : ptr16)
  Class: Eq_402
  DataType: ptr16
  OrigDataType: ptr16
T_403: (in sp : word16)
  Class: Eq_402
  DataType: ptr16
  OrigDataType: ptr16
T_404: (in 0x0002 : word16)
  Class: Eq_404
  DataType: word16
  OrigDataType: word16
T_405: (in fp - 0x0002 : word16)
  Class: Eq_402
  DataType: word16
  OrigDataType: word16
T_406: (in ds : selector)
  Class: Eq_406
  DataType: (ptr (segment (74 T_409 t0074) (78 T_417 t0078) (7C T_423 t007C) (80 T_431 t0080)))
  OrigDataType: (ptr (segment (74 T_409 t0074) (78 T_417 t0078) (7C T_423 t007C) (80 T_431 t0080)))
T_407: (in wLoc02 : word16)
  Class: Eq_406
  DataType: (ptr (segment (74 T_409 t0074) (78 T_417 t0078) (7C T_423 t007C)))
  OrigDataType: (ptr (segment (74 T_409 t0074) (78 T_417 t0078) (7C T_423 t007C)))
T_408: (in 0x0074 : word16)
  Class: Eq_408
  DataType: (memptr T_406 (struct (0 T_409 t0000)))
  OrigDataType: (memptr T_406 (struct (0 T_409 t0000)))
T_409: (in Mem0[ds:0x0074:segptr32] : segptr32)
  Class: Eq_399
  DataType: segptr32
  OrigDataType: segptr32
T_410: (in ds_dx : ptr32)
  Class: Eq_399
  DataType: (union (ptr32 u0) (segptr32 u1))
  OrigDataType: (union (ptr32 u0) (segptr32 u1))
T_411: (in msdos_set_interrupt_vector : ptr32)
  Class: Eq_396
  DataType: (ptr (fn T_413 (T_412, T_410)))
  OrigDataType: (ptr (fn T_413 (T_412, T_410)))
T_412: (in al : byte)
  Class: Eq_363
  DataType: byte
  OrigDataType: byte
T_413: (in msdos_set_interrupt_vector(al, ds_dx) : void)
  Class: Eq_413
  DataType: void
  OrigDataType: void
T_414: (in 0x0002 : word16)
  Class: Eq_414
  DataType: word16
  OrigDataType: word16
T_415: (in fp - 0x0002 : word16)
  Class: Eq_402
  DataType: word16
  OrigDataType: word16
T_416: (in 0x0078 : word16)
  Class: Eq_416
  DataType: (memptr T_406 (struct (0 T_417 t0000)))
  OrigDataType: (memptr T_406 (struct (0 T_417 t0000)))
T_417: (in Mem0[ds:0x0078:segptr32] : segptr32)
  Class: Eq_399
  DataType: segptr32
  OrigDataType: segptr32
T_418: (in msdos_set_interrupt_vector : ptr32)
  Class: Eq_396
  DataType: (ptr (fn T_419 (T_412, T_410)))
  OrigDataType: (ptr (fn T_419 (T_412, T_410)))
T_419: (in msdos_set_interrupt_vector(al, ds_dx) : void)
  Class: Eq_419
  DataType: void
  OrigDataType: void
T_420: (in 0x0002 : word16)
  Class: Eq_420
  DataType: word16
  OrigDataType: word16
T_421: (in fp - 0x0002 : word16)
  Class: Eq_402
  DataType: word16
  OrigDataType: word16
T_422: (in 0x007C : word16)
  Class: Eq_422
  DataType: (memptr T_406 (struct (0 T_423 t0000)))
  OrigDataType: (memptr T_406 (struct (0 T_423 t0000)))
T_423: (in Mem0[ds:0x007C:segptr32] : segptr32)
  Class: Eq_399
  DataType: segptr32
  OrigDataType: segptr32
T_424: (in msdos_set_interrupt_vector : ptr32)
  Class: Eq_396
  DataType: (ptr (fn T_425 (T_412, T_410)))
  OrigDataType: (ptr (fn T_425 (T_412, T_410)))
T_425: (in msdos_set_interrupt_vector(al, ds_dx) : void)
  Class: Eq_425
  DataType: void
  OrigDataType: void
T_426: (in 0x0002 : word16)
  Class: Eq_426
  DataType: word16
  OrigDataType: word16
T_427: (in fp - 0x0002 : word16)
  Class: Eq_402
  DataType: word16
  OrigDataType: word16
T_428: (in 0x2506 : word16)
  Class: Eq_428
  DataType: word16
  OrigDataType: word16
T_429: (in ax : word16)
  Class: Eq_428
  DataType: word16
  OrigDataType: word16
T_430: (in 0x0080 : word16)
  Class: Eq_430
  DataType: (memptr T_406 (struct (0 T_431 t0000)))
  OrigDataType: (memptr T_406 (struct (0 T_431 t0000)))
T_431: (in Mem0[ds:0x0080:segptr32] : segptr32)
  Class: Eq_399
  DataType: segptr32
  OrigDataType: segptr32
T_432: (in msdos_set_interrupt_vector : ptr32)
  Class: Eq_396
  DataType: (ptr (fn T_433 (T_412, T_410)))
  OrigDataType: (ptr (fn T_433 (T_412, T_410)))
T_433: (in msdos_set_interrupt_vector(al, ds_dx) : void)
  Class: Eq_433
  DataType: void
  OrigDataType: void
T_434: (in fp : ptr16)
  Class: Eq_434
  DataType: ptr16
  OrigDataType: ptr16
T_435: (in sp : word16)
  Class: Eq_434
  DataType: ptr16
  OrigDataType: ptr16
T_436: (in 0x40 : byte)
  Class: Eq_436
  DataType: byte
  OrigDataType: byte
T_437: (in ah : byte)
  Class: Eq_436
  DataType: byte
  OrigDataType: byte
T_438: (in 0x0002 : word16)
  Class: Eq_438
  DataType: word16
  OrigDataType: word16
T_439: (in bx : word16)
  Class: Eq_438
  DataType: word16
  OrigDataType: word16
T_440: (in msdos_write_file : ptr32)
  Class: Eq_440
  DataType: (ptr (fn T_451 (T_446, T_447, T_448, T_450)))
  OrigDataType: (ptr (fn T_451 (T_446, T_447, T_448, T_450)))
T_441: (in signature of msdos_write_file : void)
  Class: Eq_440
  DataType: 
  OrigDataType: 
T_442: (in bx : word16)
  Class: Eq_442
  DataType: word16
  OrigDataType: word16
T_443: (in cx : word16)
  Class: Eq_443
  DataType: word16
  OrigDataType: word16
T_444: (in ds_dx : word32)
  Class: Eq_444
  DataType: word32
  OrigDataType: word32
T_445: (in axOut : ptr16)
  Class: Eq_445
  DataType: ptr16
  OrigDataType: ptr16
T_446: (in 0x0002 : word16)
  Class: Eq_442
  DataType: word16
  OrigDataType: word16
T_447: (in cx : word16)
  Class: Eq_443
  DataType: word16
  OrigDataType: word16
T_448: (in ds_dx : word32)
  Class: Eq_444
  DataType: word32
  OrigDataType: word32
T_449: (in ax : word16)
  Class: Eq_449
  DataType: word16
  OrigDataType: word16
T_450: (in out ax : ptr16)
  Class: Eq_445
  DataType: (ptr word16)
  OrigDataType: (ptr word16)
T_451: (in msdos_write_file(0x0002, cx, ds_dx, out ax) : bool)
  Class: Eq_451
  DataType: bool
  OrigDataType: bool
T_452: (in C : bool)
  Class: Eq_451
  DataType: bool
  OrigDataType: bool
T_453: (in fp : ptr16)
  Class: Eq_453
  DataType: ptr16
  OrigDataType: ptr16
T_454: (in sp : word16)
  Class: Eq_453
  DataType: ptr16
  OrigDataType: ptr16
T_455: (in 0x0002 : word16)
  Class: Eq_455
  DataType: word16
  OrigDataType: word16
T_456: (in fp - 0x0002 : word16)
  Class: Eq_453
  DataType: word16
  OrigDataType: word16
T_457: (in bp : word16)
  Class: Eq_457
  DataType: word16
  OrigDataType: word16
T_458: (in wLoc02 : word16)
  Class: Eq_457
  DataType: word16
  OrigDataType: word16
T_459: (in fp : ptr16)
  Class: Eq_459
  DataType: ptr16
  OrigDataType: ptr16
T_460: (in sp : word16)
  Class: Eq_459
  DataType: ptr16
  OrigDataType: ptr16
T_461: (in 0x0002 : word16)
  Class: Eq_461
  DataType: word16
  OrigDataType: word16
T_462: (in fp - 0x0002 : word16)
  Class: Eq_459
  DataType: word16
  OrigDataType: word16
T_463: (in bp : word16)
  Class: Eq_463
  DataType: word16
  OrigDataType: word16
T_464: (in wLoc02 : word16)
  Class: Eq_463
  DataType: word16
  OrigDataType: word16
T_465: (in fp - 0x0002 : word16)
  Class: Eq_463
  DataType: word16
  OrigDataType: word16
T_466: (in fn0800_01FA : ptr32)
  Class: Eq_466
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_467: (in signature of fn0800_01FA : void)
  Class: Eq_466
  DataType: 
  OrigDataType: 
T_468: (in fn0800_01FA : ptr32)
  Class: Eq_466
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_469: (in fn0800_01FA : ptr32)
  Class: Eq_466
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_470: (in fn0800_01FA : ptr32)
  Class: Eq_466
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_471: (in fn0800_01FA : ptr32)
  Class: Eq_466
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_472: (in fn0800_01FA : ptr32)
  Class: Eq_466
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_473: (in fn0800_01FA : ptr32)
  Class: Eq_466
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_474: (in fn0800_01FA : ptr32)
  Class: Eq_466
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_475: (in fn0800_01FA : ptr32)
  Class: Eq_466
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_476: (in fn0800_01FA : ptr32)
  Class: Eq_466
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_477: (in fp : ptr16)
  Class: Eq_477
  DataType: ptr16
  OrigDataType: ptr16
T_478: (in sp : word16)
  Class: Eq_477
  DataType: ptr16
  OrigDataType: ptr16
T_479: (in 0x0002 : word16)
  Class: Eq_479
  DataType: word16
  OrigDataType: word16
T_480: (in fp - 0x0002 : word16)
  Class: Eq_477
  DataType: word16
  OrigDataType: word16
T_481: (in bp : word16)
  Class: Eq_481
  DataType: word16
  OrigDataType: word16
T_482: (in wLoc02 : word16)
  Class: Eq_481
  DataType: word16
  OrigDataType: word16
T_483: (in fp - 0x0002 : word16)
  Class: Eq_481
  DataType: word16
  OrigDataType: word16
T_484: (in fn0800_01FF : ptr32)
  Class: Eq_484
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_485: (in signature of fn0800_01FF : void)
  Class: Eq_484
  DataType: 
  OrigDataType: 
T_486: (in fn0800_01FF : ptr32)
  Class: Eq_484
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_487: (in fn0800_01FF : ptr32)
  Class: Eq_484
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_488: (in fn0800_01FF : ptr32)
  Class: Eq_484
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_489: (in fn0800_01FF : ptr32)
  Class: Eq_484
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_490: (in fn0800_01FF : ptr32)
  Class: Eq_484
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_491: (in fn0800_01FF : ptr32)
  Class: Eq_484
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_492: (in fn0800_01FF : ptr32)
  Class: Eq_484
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_493: (in fn0800_01FF : ptr32)
  Class: Eq_484
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_494: (in fn0800_01FF : ptr32)
  Class: Eq_484
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_495: (in fp : ptr16)
  Class: Eq_495
  DataType: ptr16
  OrigDataType: ptr16
T_496: (in sp : word16)
  Class: Eq_495
  DataType: ptr16
  OrigDataType: ptr16
T_497: (in 0x0002 : word16)
  Class: Eq_497
  DataType: word16
  OrigDataType: word16
T_498: (in fp - 0x0002 : word16)
  Class: Eq_495
  DataType: word16
  OrigDataType: word16
T_499: (in bp : word16)
  Class: Eq_499
  DataType: word16
  OrigDataType: word16
T_500: (in wLoc02 : word16)
  Class: Eq_499
  DataType: word16
  OrigDataType: word16
T_501: (in fp - 0x0002 : word16)
  Class: Eq_499
  DataType: word16
  OrigDataType: word16
T_502: (in fn0800_0222 : ptr32)
  Class: Eq_502
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_503: (in signature of fn0800_0222 : void)
  Class: Eq_502
  DataType: 
  OrigDataType: 
T_504: (in fn0800_0222 : ptr32)
  Class: Eq_502
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_505: (in fn0800_0222 : ptr32)
  Class: Eq_502
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_506: (in fn0800_0222 : ptr32)
  Class: Eq_502
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_507: (in fn0800_0222 : ptr32)
  Class: Eq_502
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_508: (in fn0800_0222 : ptr32)
  Class: Eq_502
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_509: (in fn0800_0222 : ptr32)
  Class: Eq_502
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_510: (in fn0800_0222 : ptr32)
  Class: Eq_502
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_511: (in fn0800_0222 : ptr32)
  Class: Eq_502
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_512: (in fp : ptr16)
  Class: Eq_512
  DataType: ptr16
  OrigDataType: ptr16
T_513: (in sp : word16)
  Class: Eq_512
  DataType: ptr16
  OrigDataType: ptr16
T_514: (in 0x0002 : word16)
  Class: Eq_514
  DataType: word16
  OrigDataType: word16
T_515: (in fp - 0x0002 : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_516: (in bp : word16)
  Class: Eq_516
  DataType: word16
  OrigDataType: word16
T_517: (in wLoc02 : word16)
  Class: Eq_516
  DataType: word16
  OrigDataType: word16
T_518: (in fp - 0x0002 : word16)
  Class: Eq_516
  DataType: word16
  OrigDataType: word16
T_519: (in 0x000A : word16)
  Class: Eq_519
  DataType: word16
  OrigDataType: word16
T_520: (in fp - 0x000A : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_521: (in fp - 0x000A : word16)
  Class: Eq_521
  DataType: word16
  OrigDataType: word16
T_522: (in cond(fp - 0x000A) : byte)
  Class: Eq_522
  DataType: byte
  OrigDataType: byte
T_523: (in SCZO : byte)
  Class: Eq_522
  DataType: byte
  OrigDataType: byte
T_524: (in 0x0194 : word16)
  Class: Eq_524
  DataType: word16
  OrigDataType: word16
T_525: (in ax : word16)
  Class: Eq_524
  DataType: word16
  OrigDataType: word16
T_526: (in 0x000C : word16)
  Class: Eq_526
  DataType: word16
  OrigDataType: word16
T_527: (in fp - 0x000C : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_528: (in 0x0194 : word16)
  Class: Eq_528
  DataType: word16
  OrigDataType: word16
T_529: (in wLoc0C : word16)
  Class: Eq_528
  DataType: word16
  OrigDataType: word16
T_530: (in fn0800_0E4B : ptr32)
  Class: Eq_530
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_531: (in signature of fn0800_0E4B : void)
  Class: Eq_530
  DataType: 
  OrigDataType: 
T_532: (in cx : word16)
  Class: Eq_528
  DataType: word16
  OrigDataType: word16
T_533: (in 0x000A : word16)
  Class: Eq_533
  DataType: word16
  OrigDataType: word16
T_534: (in fp - 0x000A : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_535: (in 0x0006 : word16)
  Class: Eq_535
  DataType: word16
  OrigDataType: word16
T_536: (in fp - 0x0006 : word16)
  Class: Eq_524
  DataType: word16
  OrigDataType: word16
T_537: (in 0x000C : word16)
  Class: Eq_537
  DataType: word16
  OrigDataType: word16
T_538: (in fp - 0x000C : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_539: (in fp - 0x0006 : word16)
  Class: Eq_528
  DataType: word16
  OrigDataType: word16
T_540: (in 0x01B0 : word16)
  Class: Eq_524
  DataType: word16
  OrigDataType: word16
T_541: (in 0x000E : word16)
  Class: Eq_541
  DataType: word16
  OrigDataType: word16
T_542: (in fp - 0x000E : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_543: (in 0x01B0 : word16)
  Class: Eq_528
  DataType: word16
  OrigDataType: word16
T_544: (in wLoc0E : word16)
  Class: Eq_528
  DataType: word16
  OrigDataType: word16
T_545: (in fn0800_16D4 : ptr32)
  Class: Eq_545
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_546: (in signature of fn0800_16D4 : void)
  Class: Eq_545
  DataType: 
  OrigDataType: 
T_547: (in 0x000C : word16)
  Class: Eq_547
  DataType: word16
  OrigDataType: word16
T_548: (in fp - 0x000C : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_549: (in 0x000A : word16)
  Class: Eq_549
  DataType: word16
  OrigDataType: word16
T_550: (in fp - 0x000A : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_551: (in 0x000C : word16)
  Class: Eq_551
  DataType: word16
  OrigDataType: word16
T_552: (in fp - 0x000C : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_553: (in wLoc04 : word16)
  Class: Eq_528
  DataType: word16
  OrigDataType: word16
T_554: (in 0x000E : word16)
  Class: Eq_554
  DataType: word16
  OrigDataType: word16
T_555: (in fp - 0x000E : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_556: (in wLoc06 : word16)
  Class: Eq_528
  DataType: word16
  OrigDataType: word16
T_557: (in 0x01B4 : word16)
  Class: Eq_524
  DataType: word16
  OrigDataType: word16
T_558: (in 0x0010 : word16)
  Class: Eq_558
  DataType: word16
  OrigDataType: word16
T_559: (in fp - 0x0010 : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_560: (in 0x01B4 : word16)
  Class: Eq_560
  DataType: word16
  OrigDataType: word16
T_561: (in wLoc10 : word16)
  Class: Eq_560
  DataType: word16
  OrigDataType: word16
T_562: (in fn0800_0E4B : ptr32)
  Class: Eq_530
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_563: (in 0x000A : word16)
  Class: Eq_563
  DataType: word16
  OrigDataType: word16
T_564: (in fp - 0x000A : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_565: (in fp - 0x000A : word16)
  Class: Eq_565
  DataType: word16
  OrigDataType: word16
T_566: (in cond(fp - 0x000A) : byte)
  Class: Eq_522
  DataType: byte
  OrigDataType: byte
T_567: (in 0x0000 : word16)
  Class: Eq_567
  DataType: word16
  OrigDataType: word16
T_568: (in wLoc08 : word16)
  Class: Eq_567
  DataType: word16
  OrigDataType: word16
T_569: (in 0x0001 : word16)
  Class: Eq_524
  DataType: word16
  OrigDataType: word16
T_570: (in wLoc0A : word16)
  Class: Eq_524
  DataType: word16
  OrigDataType: word16
T_571: (in dx : word16)
  Class: Eq_567
  DataType: word16
  OrigDataType: word16
T_572: (in dx - wLoc04 : word16)
  Class: Eq_572
  DataType: word16
  OrigDataType: word16
T_573: (in cond(dx - wLoc04) : byte)
  Class: Eq_522
  DataType: byte
  OrigDataType: byte
T_574: (in SO : byte)
  Class: Eq_574
  DataType: byte
  OrigDataType: byte
T_575: (in Test(LT,SO) : bool)
  Class: Eq_575
  DataType: bool
  OrigDataType: bool
T_576: (in fn0800_0245 : ptr32)
  Class: Eq_576
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_577: (in signature of fn0800_0245 : void)
  Class: Eq_576
  DataType: 
  OrigDataType: 
T_578: (in 0x0001 : word16)
  Class: Eq_578
  DataType: word16
  OrigDataType: word16
T_579: (in wLoc0A + 0x0001 : word16)
  Class: Eq_524
  DataType: word16
  OrigDataType: word16
T_580: (in v12 : word16)
  Class: Eq_524
  DataType: word16
  OrigDataType: word16
T_581: (in cond(v12) : byte)
  Class: Eq_522
  DataType: byte
  OrigDataType: byte
T_582: (in 0x00 : byte)
  Class: Eq_582
  DataType: byte
  OrigDataType: byte
T_583: (in wLoc08 + 0x00 : word16)
  Class: Eq_583
  DataType: word16
  OrigDataType: word16
T_584: (in C : byte)
  Class: Eq_584
  DataType: byte
  OrigDataType: byte
T_585: (in wLoc08 + 0x00 + C : word16)
  Class: Eq_567
  DataType: word16
  OrigDataType: word16
T_586: (in v14 : word16)
  Class: Eq_567
  DataType: word16
  OrigDataType: word16
T_587: (in cond(v14) : byte)
  Class: Eq_522
  DataType: byte
  OrigDataType: byte
T_588: (in SZO : byte)
  Class: Eq_588
  DataType: byte
  OrigDataType: byte
T_589: (in Test(GT,SZO) : bool)
  Class: Eq_589
  DataType: bool
  OrigDataType: bool
T_590: (in 0x01CE : word16)
  Class: Eq_524
  DataType: word16
  OrigDataType: word16
T_591: (in 0x000C : word16)
  Class: Eq_591
  DataType: word16
  OrigDataType: word16
T_592: (in fp - 0x000C : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_593: (in 0x01CE : word16)
  Class: Eq_528
  DataType: word16
  OrigDataType: word16
T_594: (in fn0800_0E4B : ptr32)
  Class: Eq_530
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_595: (in fp - 0x0002 : word16)
  Class: Eq_512
  DataType: word16
  OrigDataType: word16
T_596: (in ax - wLoc06 : word16)
  Class: Eq_596
  DataType: word16
  OrigDataType: word16
T_597: (in cond(ax - wLoc06) : byte)
  Class: Eq_522
  DataType: byte
  OrigDataType: byte
T_598: (in CZ : byte)
  Class: Eq_598
  DataType: byte
  OrigDataType: byte
T_599: (in Test(ULE,CZ) : bool)
  Class: Eq_599
  DataType: bool
  OrigDataType: bool
T_600: (in fp : ptr16)
  Class: Eq_600
  DataType: ptr16
  OrigDataType: ptr16
T_601: (in sp : word16)
  Class: Eq_600
  DataType: ptr16
  OrigDataType: ptr16
T_602: (in 0x0002 : word16)
  Class: Eq_602
  DataType: word16
  OrigDataType: word16
T_603: (in fp - 0x0002 : word16)
  Class: Eq_600
  DataType: word16
  OrigDataType: word16
T_604: (in bp : word16)
  Class: Eq_604
  DataType: word16
  OrigDataType: word16
T_605: (in wLoc02 : word16)
  Class: Eq_604
  DataType: word16
  OrigDataType: word16
T_606: (in fp - 0x0002 : word16)
  Class: Eq_604
  DataType: word16
  OrigDataType: word16
T_607: (in ds : selector)
  Class: Eq_607
  DataType: (ptr (segment (234 T_637 t0234) (236 T_640 t0236) (238 T_643 t0238) (23E T_609 t023E)))
  OrigDataType: (ptr (segment (234 T_637 t0234) (236 T_640 t0236) (238 T_643 t0238) (23E T_609 t023E)))
T_608: (in 0x023E : word16)
  Class: Eq_608
  DataType: (memptr T_607 (struct (0 T_609 t0000)))
  OrigDataType: (memptr T_607 (struct (0 T_609 t0000)))
T_609: (in Mem0[ds:0x023E:word16] : word16)
  Class: Eq_609
  DataType: word16
  OrigDataType: word16
T_610: (in ax : word16)
  Class: Eq_609
  DataType: word16
  OrigDataType: word16
T_611: (in 0x023E : word16)
  Class: Eq_611
  DataType: (memptr T_607 (struct (0 T_609 t0000)))
  OrigDataType: (memptr T_607 (struct (0 T_609 t0000)))
T_612: (in Mem0[ds:0x023E:word16] : word16)
  Class: Eq_609
  DataType: word16
  OrigDataType: word16
T_613: (in 0x0001 : word16)
  Class: Eq_613
  DataType: word16
  OrigDataType: word16
T_614: (in Mem0[ds:0x023E:word16] - 0x0001 : word16)
  Class: Eq_609
  DataType: word16
  OrigDataType: word16
T_615: (in v7 : word16)
  Class: Eq_609
  DataType: word16
  OrigDataType: word16
T_616: (in Mem0[ds:0x023E:word16] : word16)
  Class: Eq_609
  DataType: word16
  OrigDataType: word16
T_617: (in ax | ax : word16)
  Class: Eq_609
  DataType: word16
  OrigDataType: word16
T_618: (in cond(ax) : byte)
  Class: Eq_618
  DataType: byte
  OrigDataType: byte
T_619: (in SZO : byte)
  Class: Eq_618
  DataType: byte
  OrigDataType: byte
T_620: (in false : bool)
  Class: Eq_620
  DataType: bool
  OrigDataType: bool
T_621: (in C : byte)
  Class: Eq_620
  DataType: bool
  OrigDataType: bool
T_622: (in Z : byte)
  Class: Eq_622
  DataType: byte
  OrigDataType: byte
T_623: (in Test(NE,Z) : bool)
  Class: Eq_623
  DataType: bool
  OrigDataType: bool
T_624: (in 0x023E : word16)
  Class: Eq_624
  DataType: (memptr T_607 (struct (0 T_625 t0000)))
  OrigDataType: (memptr T_607 (struct (0 T_625 t0000)))
T_625: (in Mem0[ds:0x023E:word16] : word16)
  Class: Eq_609
  DataType: word16
  OrigDataType: word16
T_626: (in bx : word16)
  Class: Eq_609
  DataType: (union (ui16 u0) ((memptr T_607 (struct (5E8 T_634 t05E8))) u1))
  OrigDataType: (union (ui16 u0) ((memptr T_607 (struct (5E8 T_634 t05E8))) u1))
T_627: (in 0x0001 : word16)
  Class: Eq_627
  DataType: word16
  OrigDataType: word16
T_628: (in bx << 0x0001 : word16)
  Class: Eq_609
  DataType: ui16
  OrigDataType: ui16
T_629: (in cond(bx) : byte)
  Class: Eq_629
  DataType: byte
  OrigDataType: byte
T_630: (in SCZO : byte)
  Class: Eq_629
  DataType: byte
  OrigDataType: byte
T_631: (in cs : selector)
  Class: Eq_631
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_632: (in 0x05E8 : word16)
  Class: Eq_632
  DataType: word16
  OrigDataType: word16
T_633: (in bx + 0x05E8 : word16)
  Class: Eq_633
  DataType: word16
  OrigDataType: word16
T_634: (in Mem0[ds:bx + 0x05E8:word16] : word16)
  Class: Eq_634
  DataType: word16
  OrigDataType: word16
T_635: (in SEQ(cs, Mem0[ds:bx + 0x05E8:word16]) : ptr32)
  Class: Eq_635
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_636: (in 0x0234 : word16)
  Class: Eq_636
  DataType: (memptr T_607 (struct (0 T_637 t0000)))
  OrigDataType: (memptr T_607 (struct (0 T_637 t0000)))
T_637: (in Mem0[ds:0x0234:word16] : word16)
  Class: Eq_637
  DataType: word16
  OrigDataType: word16
T_638: (in SEQ(cs, Mem0[ds:0x0234:word16]) : ptr32)
  Class: Eq_638
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_639: (in 0x0236 : word16)
  Class: Eq_639
  DataType: (memptr T_607 (struct (0 T_640 t0000)))
  OrigDataType: (memptr T_607 (struct (0 T_640 t0000)))
T_640: (in Mem0[ds:0x0236:word16] : word16)
  Class: Eq_640
  DataType: word16
  OrigDataType: word16
T_641: (in SEQ(cs, Mem0[ds:0x0236:word16]) : ptr32)
  Class: Eq_641
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_642: (in 0x0238 : word16)
  Class: Eq_642
  DataType: (memptr T_607 (struct (0 T_643 t0000)))
  OrigDataType: (memptr T_607 (struct (0 T_643 t0000)))
T_643: (in Mem0[ds:0x0238:word16] : word16)
  Class: Eq_643
  DataType: word16
  OrigDataType: word16
T_644: (in SEQ(cs, Mem0[ds:0x0238:word16]) : ptr32)
  Class: Eq_644
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_645: (in 0x0004 : word16)
  Class: Eq_645
  DataType: word16
  OrigDataType: word16
T_646: (in fp - 0x0004 : word16)
  Class: Eq_600
  DataType: word16
  OrigDataType: word16
T_647: (in wArg02 : word16)
  Class: Eq_647
  DataType: word16
  OrigDataType: word16
T_648: (in wLoc04 : word16)
  Class: Eq_647
  DataType: word16
  OrigDataType: word16
T_649: (in fn0800_0121 : ptr32)
  Class: Eq_214
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_650: (in cx : word16)
  Class: Eq_647
  DataType: word16
  OrigDataType: word16
T_651: (in 0x0002 : word16)
  Class: Eq_651
  DataType: word16
  OrigDataType: word16
T_652: (in fp - 0x0002 : word16)
  Class: Eq_600
  DataType: word16
  OrigDataType: word16
T_653: (in fp : ptr16)
  Class: Eq_2
  DataType: ptr16
  OrigDataType: ptr16
T_654: (in sp : word16)
  Class: Eq_2
  DataType: (union ((union (ptr16 u0) ((memptr T_739 (struct (0 T_744 t0000))) u1)) u0) ((ptr (segment)) u1) ((memptr T_660 (struct (0 T_673 t0000))) u2) ((memptr T_667 (struct (0 T_724 t0000))) u3) ((memptr T_739 (struct (0 T_2 t0000))) u4))
  OrigDataType: (union ((union (ptr16 u0) ((memptr T_739 (struct (0 T_744 t0000))) u1)) u0) ((ptr (segment)) u1) ((memptr T_660 (struct (0 T_673 t0000))) u2) ((memptr T_667 (struct (0 T_724 t0000))) u3) ((memptr T_739 (struct (0 T_2 t0000))) u4))
T_655: (in wArg00 : word16)
  Class: Eq_655
  DataType: word16
  OrigDataType: word16
T_656: (in 0x0330 : word16)
  Class: Eq_656
  DataType: (memptr T_7 (struct (0 T_657 t0000)))
  OrigDataType: (memptr T_7 (struct (0 T_657 t0000)))
T_657: (in Mem0[0x0800:0x0330:word16] : word16)
  Class: Eq_655
  DataType: word16
  OrigDataType: word16
T_658: (in 0x0002 : word16)
  Class: Eq_658
  DataType: word16
  OrigDataType: word16
T_659: (in fp + 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_660: (in ds : selector)
  Class: Eq_2
  DataType: (ptr (segment (80 T_673 t0080) (84 T_792 t0084) (86 T_849 t0086) (8A T_687 t008A) (8C T_712 t008C) (90 T_666 t0090) (92 T_693 t0092)))
  OrigDataType: (ptr (segment (80 T_673 t0080) (84 T_792 t0084) (86 T_849 t0086) (8A T_687 t008A) (8C T_712 t008C) (90 T_666 t0090) (92 T_693 t0092)))
T_661: (in 0x0332 : word16)
  Class: Eq_661
  DataType: (memptr T_7 (struct (0 T_662 t0000)))
  OrigDataType: (memptr T_7 (struct (0 T_662 t0000)))
T_662: (in Mem0[0x0800:0x0332:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_663: (in false : bool)
  Class: Eq_663
  DataType: bool
  OrigDataType: bool
T_664: (in D : byte)
  Class: Eq_663
  DataType: bool
  OrigDataType: bool
T_665: (in 0x0090 : word16)
  Class: Eq_665
  DataType: (memptr T_660 (struct (0 T_666 t0000)))
  OrigDataType: (memptr T_660 (struct (0 T_666 t0000)))
T_666: (in Mem0[ds:0x0090:selector] : selector)
  Class: Eq_2
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_667: (in es : selector)
  Class: Eq_2
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_668: (in 0x0080 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_669: (in si : word16)
  Class: Eq_2
  DataType: (union ((union (ptr16 u0) ((memptr T_739 (struct (0 T_744 t0000))) u1)) u0) ((ptr (segment)) u1) ((memptr T_660 (struct (0 T_673 t0000))) u2) ((memptr T_667 (struct (0 T_724 t0000))) u3) ((memptr T_739 (struct (0 T_2 t0000))) u4))
  OrigDataType: (union ((union (ptr16 u0) ((memptr T_739 (struct (0 T_744 t0000))) u1)) u0) ((ptr (segment)) u1) ((memptr T_660 (struct (0 T_673 t0000))) u2) ((memptr T_667 (struct (0 T_724 t0000))) u3) ((memptr T_739 (struct (0 T_2 t0000))) u4))
T_670: (in 0x00 : byte)
  Class: Eq_670
  DataType: byte
  OrigDataType: byte
T_671: (in ah : byte)
  Class: Eq_670
  DataType: byte
  OrigDataType: byte
T_672: (in 0x0080 : word16)
  Class: Eq_672
  DataType: (memptr T_660 (struct (0 T_673 t0000)))
  OrigDataType: (memptr T_660 (struct (0 T_673 t0000)))
T_673: (in Mem0[ds:0x0080:byte] : byte)
  Class: Eq_673
  DataType: byte
  OrigDataType: byte
T_674: (in al : byte)
  Class: Eq_673
  DataType: byte
  OrigDataType: byte
T_675: (in 0x0081 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_676: (in ax : word16)
  Class: Eq_2
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_677: (in 0x0001 : word16)
  Class: Eq_677
  DataType: word16
  OrigDataType: word16
T_678: (in ax + 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_679: (in bp : word16)
  Class: Eq_2
  DataType: (union ((union (ptr16 u0) ((memptr T_739 (struct (0 T_744 t0000))) u1)) u0) ((ptr (segment)) u1) ((memptr T_660 (struct (0 T_673 t0000))) u2) ((memptr T_667 (struct (0 T_724 t0000))) u3) ((memptr T_739 (struct (0 T_2 t0000))) u4))
  OrigDataType: (union ((union (ptr16 u0) ((memptr T_739 (struct (0 T_744 t0000))) u1)) u0) ((ptr (segment)) u1) ((memptr T_660 (struct (0 T_673 t0000))) u2) ((memptr T_667 (struct (0 T_724 t0000))) u3) ((memptr T_739 (struct (0 T_2 t0000))) u4))
T_680: (in 0x0081 : word16)
  Class: Eq_680
  DataType: word16
  OrigDataType: word16
T_681: (in v14 : word16)
  Class: Eq_680
  DataType: word16
  OrigDataType: word16
T_682: (in 0x0081 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_683: (in dx : word16)
  Class: Eq_2
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_684: (in v16 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_685: (in bx : word16)
  Class: Eq_2
  DataType: (memptr T_660 (struct (0 T_829 t0000)))
  OrigDataType: (memptr T_660 (struct (0 T_829 t0000)))
T_686: (in 0x008A : word16)
  Class: Eq_686
  DataType: (memptr T_660 (struct (0 T_687 t0000)))
  OrigDataType: (memptr T_660 (struct (0 T_687 t0000)))
T_687: (in Mem0[ds:0x008A:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_688: (in 0x0002 : word16)
  Class: Eq_688
  DataType: word16
  OrigDataType: word16
T_689: (in si + 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_690: (in 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_691: (in cx : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_692: (in 0x0092 : word16)
  Class: Eq_692
  DataType: (memptr T_660 (struct (0 T_693 t0000)))
  OrigDataType: (memptr T_660 (struct (0 T_693 t0000)))
T_693: (in Mem0[ds:0x0092:byte] : byte)
  Class: Eq_693
  DataType: byte
  OrigDataType: byte
T_694: (in 0x03 : byte)
  Class: Eq_694
  DataType: byte
  OrigDataType: byte
T_695: (in Mem0[ds:0x0092:byte] - 0x03 : byte)
  Class: Eq_695
  DataType: byte
  OrigDataType: byte
T_696: (in cond(Mem0[ds:0x0092:byte] - 0x03) : byte)
  Class: Eq_696
  DataType: byte
  OrigDataType: byte
T_697: (in SCZO : byte)
  Class: Eq_696
  DataType: byte
  OrigDataType: byte
T_698: (in C : byte)
  Class: Eq_698
  DataType: bool
  OrigDataType: bool
T_699: (in Test(ULT,C) : bool)
  Class: Eq_699
  DataType: bool
  OrigDataType: bool
T_700: (in 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_701: (in 0x0001 : word16)
  Class: Eq_701
  DataType: word16
  OrigDataType: word16
T_702: (in 0x0001 + bx : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_703: (in ax + cx : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_704: (in 0xFFFE : word16)
  Class: Eq_704
  DataType: word16
  OrigDataType: word16
T_705: (in ax & 0xFFFE : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_706: (in di : word16)
  Class: Eq_2
  DataType: (union ((union (ptr16 u0) ((memptr T_739 (struct (0 T_744 t0000))) u1)) u0) ((ptr (segment)) u1) ((memptr T_660 (struct (0 T_673 t0000))) u2) ((memptr T_667 (struct (0 T_724 t0000))) u3) ((memptr T_739 (struct (0 T_2 t0000))) u4))
  OrigDataType: (union ((union (ptr16 u0) ((memptr T_739 (struct (0 T_744 t0000))) u1)) u0) ((ptr (segment)) u1) ((memptr T_660 (struct (0 T_673 t0000))) u2) ((memptr T_667 (struct (0 T_724 t0000))) u3) ((memptr T_739 (struct (0 T_2 t0000))) u4))
T_707: (in ax & 0xFFFE : word16)
  Class: Eq_707
  DataType: word16
  OrigDataType: word16
T_708: (in fp - (ax & 0xFFFE) : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_709: (in cond(di) : byte)
  Class: Eq_696
  DataType: byte
  OrigDataType: byte
T_710: (in Test(ULT,C) : bool)
  Class: Eq_710
  DataType: bool
  OrigDataType: bool
T_711: (in 0x008C : word16)
  Class: Eq_711
  DataType: (memptr T_660 (struct (0 T_712 t0000)))
  OrigDataType: (memptr T_660 (struct (0 T_712 t0000)))
T_712: (in Mem0[ds:0x008C:selector] : selector)
  Class: Eq_2
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_713: (in 0x7F : byte)
  Class: Eq_713
  DataType: byte
  OrigDataType: byte
T_714: (in cl : byte)
  Class: Eq_713
  DataType: byte
  OrigDataType: byte
T_715: (in 0x00 : byte)
  Class: Eq_673
  DataType: byte
  OrigDataType: byte
T_716: (in 0x00 : byte)
  Class: Eq_716
  DataType: byte
  OrigDataType: byte
T_717: (in cond(0x00) : byte)
  Class: Eq_717
  DataType: byte
  OrigDataType: byte
T_718: (in SZO : byte)
  Class: Eq_717
  DataType: byte
  OrigDataType: byte
T_719: (in false : bool)
  Class: Eq_698
  DataType: bool
  OrigDataType: bool
T_720: (in 0x0000 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_721: (in cx == 0x0000 : bool)
  Class: Eq_721
  DataType: bool
  OrigDataType: bool
T_722: (in 0x0000 : word16)
  Class: Eq_722
  DataType: word16
  OrigDataType: word16
T_723: (in di + 0x0000 : word16)
  Class: Eq_723
  DataType: word16
  OrigDataType: word16
T_724: (in Mem0[es:di + 0x0000:byte] : byte)
  Class: Eq_673
  DataType: byte
  OrigDataType: byte
T_725: (in al - Mem0[es:di + 0x0000:byte] : byte)
  Class: Eq_725
  DataType: byte
  OrigDataType: byte
T_726: (in cond(al - Mem0[es:di + 0x0000:byte]) : byte)
  Class: Eq_696
  DataType: byte
  OrigDataType: byte
T_727: (in 0x0001 : word16)
  Class: Eq_727
  DataType: word16
  OrigDataType: word16
T_728: (in di + 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_729: (in 0x0001 : word16)
  Class: Eq_729
  DataType: word16
  OrigDataType: word16
T_730: (in cx - 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_731: (in Z : byte)
  Class: Eq_731
  DataType: byte
  OrigDataType: byte
T_732: (in Test(NE,Z) : bool)
  Class: Eq_732
  DataType: bool
  OrigDataType: bool
T_733: (in 0x0000 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_734: (in cx == 0x0000 : bool)
  Class: Eq_734
  DataType: bool
  OrigDataType: bool
T_735: (in 0x7F : byte)
  Class: Eq_735
  DataType: byte
  OrigDataType: byte
T_736: (in cl ^ 0x7F : byte)
  Class: Eq_713
  DataType: byte
  OrigDataType: byte
T_737: (in cond(cl) : byte)
  Class: Eq_717
  DataType: byte
  OrigDataType: byte
T_738: (in false : bool)
  Class: Eq_698
  DataType: bool
  OrigDataType: bool
T_739: (in ss : selector)
  Class: Eq_2
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_740: (in 0x0002 : word16)
  Class: Eq_740
  DataType: word16
  OrigDataType: word16
T_741: (in sp - 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_742: (in 0x0000 : word16)
  Class: Eq_742
  DataType: word16
  OrigDataType: word16
T_743: (in sp + 0x0000 : word16)
  Class: Eq_743
  DataType: word16
  OrigDataType: word16
T_744: (in Mem0[ss:sp + 0x0000:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_745: (in 0x0001 : word16)
  Class: Eq_745
  DataType: word16
  OrigDataType: word16
T_746: (in cx - 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_747: (in cond(cx) : byte)
  Class: Eq_717
  DataType: byte
  OrigDataType: byte
T_748: (in 0x00 : byte)
  Class: Eq_673
  DataType: byte
  OrigDataType: byte
T_749: (in false : bool)
  Class: Eq_698
  DataType: bool
  OrigDataType: bool
T_750: (in 0x00 : byte)
  Class: Eq_673
  DataType: byte
  OrigDataType: byte
T_751: (in 0x0000 : word16)
  Class: Eq_751
  DataType: word16
  OrigDataType: word16
T_752: (in di + 0x0000 : word16)
  Class: Eq_752
  DataType: word16
  OrigDataType: word16
T_753: (in Mem0[es:di + 0x0000:byte] : byte)
  Class: Eq_673
  DataType: byte
  OrigDataType: byte
T_754: (in 0x0001 : word16)
  Class: Eq_754
  DataType: word16
  OrigDataType: word16
T_755: (in di + 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_756: (in 0x0081 : word16)
  Class: Eq_756
  DataType: word16
  OrigDataType: word16
T_757: (in v24 : word16)
  Class: Eq_756
  DataType: word16
  OrigDataType: word16
T_758: (in 0x0081 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_759: (in v25 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_760: (in 0x0001 : word16)
  Class: Eq_760
  DataType: word16
  OrigDataType: word16
T_761: (in bx + 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_762: (in cond(bx) : byte)
  Class: Eq_717
  DataType: byte
  OrigDataType: byte
T_763: (in 0x0000 : word16)
  Class: Eq_763
  DataType: word16
  OrigDataType: word16
T_764: (in si + 0x0000 : word16)
  Class: Eq_764
  DataType: word16
  OrigDataType: word16
T_765: (in Mem0[ds:si + 0x0000:byte] : byte)
  Class: Eq_673
  DataType: byte
  OrigDataType: byte
T_766: (in v23 : byte)
  Class: Eq_673
  DataType: byte
  OrigDataType: byte
T_767: (in 0x0000 : word16)
  Class: Eq_767
  DataType: word16
  OrigDataType: word16
T_768: (in di + 0x0000 : word16)
  Class: Eq_768
  DataType: word16
  OrigDataType: word16
T_769: (in Mem0[es:di + 0x0000:byte] : byte)
  Class: Eq_673
  DataType: byte
  OrigDataType: byte
T_770: (in 0x0001 : word16)
  Class: Eq_770
  DataType: word16
  OrigDataType: word16
T_771: (in si + 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_772: (in 0x0001 : word16)
  Class: Eq_772
  DataType: word16
  OrigDataType: word16
T_773: (in di + 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_774: (in 0x0001 : word16)
  Class: Eq_774
  DataType: word16
  OrigDataType: word16
T_775: (in cx - 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_776: (in 0x0000 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_777: (in cx == 0x0000 : bool)
  Class: Eq_777
  DataType: bool
  OrigDataType: bool
T_778: (in 0x20 : byte)
  Class: Eq_778
  DataType: byte
  OrigDataType: byte
T_779: (in al - 0x20 : byte)
  Class: Eq_779
  DataType: byte
  OrigDataType: byte
T_780: (in cond(al - 0x20) : byte)
  Class: Eq_696
  DataType: byte
  OrigDataType: byte
T_781: (in Test(EQ,Z) : bool)
  Class: Eq_781
  DataType: bool
  OrigDataType: bool
T_782: (in Test(ULT,C) : bool)
  Class: Eq_782
  DataType: bool
  OrigDataType: bool
T_783: (in 0x0000 : word16)
  Class: Eq_783
  DataType: word16
  OrigDataType: word16
T_784: (in sp + 0x0000 : word16)
  Class: Eq_784
  DataType: word16
  OrigDataType: word16
T_785: (in Mem0[ss:sp + 0x0000:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_786: (in 0x0002 : word16)
  Class: Eq_786
  DataType: word16
  OrigDataType: word16
T_787: (in sp + 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_788: (in cx + dx : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_789: (in 0x0332 : word16)
  Class: Eq_789
  DataType: (memptr T_7 (struct (0 T_790 t0000)))
  OrigDataType: (memptr T_7 (struct (0 T_790 t0000)))
T_790: (in Mem0[0x0800:0x0332:selector] : selector)
  Class: Eq_2
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_791: (in 0x0084 : word16)
  Class: Eq_791
  DataType: (memptr T_660 (struct (0 T_792 t0000)))
  OrigDataType: (memptr T_660 (struct (0 T_792 t0000)))
T_792: (in Mem0[ds:0x0084:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_793: (in 0x0001 : word16)
  Class: Eq_793
  DataType: word16
  OrigDataType: word16
T_794: (in bx + 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_795: (in bx + bx : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_796: (in bp - bx : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_797: (in cond(bp) : byte)
  Class: Eq_696
  DataType: byte
  OrigDataType: byte
T_798: (in Test(ULT,C) : bool)
  Class: Eq_798
  DataType: bool
  OrigDataType: bool
T_799: (in fn0800_03BF : ptr32)
  Class: Eq_799
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_800: (in signature of fn0800_03BF : void)
  Class: Eq_799
  DataType: 
  OrigDataType: 
T_801: (in CZ : byte)
  Class: Eq_801
  DataType: byte
  OrigDataType: byte
T_802: (in Test(UGT,CZ) : bool)
  Class: Eq_802
  DataType: bool
  OrigDataType: bool
T_803: (in 0x00 : byte)
  Class: Eq_673
  DataType: byte
  OrigDataType: byte
T_804: (in 0x00 : byte)
  Class: Eq_804
  DataType: byte
  OrigDataType: byte
T_805: (in cond(0x00) : byte)
  Class: Eq_717
  DataType: byte
  OrigDataType: byte
T_806: (in false : bool)
  Class: Eq_698
  DataType: bool
  OrigDataType: bool
T_807: (in 0x0D : byte)
  Class: Eq_807
  DataType: byte
  OrigDataType: byte
T_808: (in al - 0x0D : byte)
  Class: Eq_808
  DataType: byte
  OrigDataType: byte
T_809: (in cond(al - 0x0D) : byte)
  Class: Eq_696
  DataType: byte
  OrigDataType: byte
T_810: (in Test(EQ,Z) : bool)
  Class: Eq_810
  DataType: bool
  OrigDataType: bool
T_811: (in 0x09 : byte)
  Class: Eq_811
  DataType: byte
  OrigDataType: byte
T_812: (in al - 0x09 : byte)
  Class: Eq_812
  DataType: byte
  OrigDataType: byte
T_813: (in cond(al - 0x09) : byte)
  Class: Eq_696
  DataType: byte
  OrigDataType: byte
T_814: (in Test(NE,Z) : bool)
  Class: Eq_814
  DataType: bool
  OrigDataType: bool
T_815: (in fn0800_03BF : ptr32)
  Class: Eq_799
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_816: (in Test(UGT,CZ) : bool)
  Class: Eq_816
  DataType: bool
  OrigDataType: bool
T_817: (in 0x001E : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_818: (in 0x0056 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_819: (in 0x01F8 : word16)
  Class: Eq_819
  DataType: (memptr T_7 (struct (0 T_820 t0000)))
  OrigDataType: (memptr T_7 (struct (0 T_820 t0000)))
T_820: (in Mem0[0x0800:0x01F8:selector] : selector)
  Class: Eq_2
  DataType: (ptr (segment))
  OrigDataType: (ptr (segment))
T_821: (in 0x0003 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_822: (in 0x0002 : word16)
  Class: Eq_822
  DataType: word16
  OrigDataType: word16
T_823: (in sp - 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_824: (in 0x0003 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_825: (in 0x0000 : word16)
  Class: Eq_825
  DataType: word16
  OrigDataType: word16
T_826: (in sp + 0x0000 : word16)
  Class: Eq_826
  DataType: word16
  OrigDataType: word16
T_827: (in Mem0[ss:sp + 0x0000:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_828: (in bx + si : word16)
  Class: Eq_828
  DataType: word16
  OrigDataType: word16
T_829: (in Mem0[ds:bx + si:byte] : byte)
  Class: Eq_829
  DataType: byte
  OrigDataType: byte
T_830: (in 0x03 : byte)
  Class: Eq_830
  DataType: byte
  OrigDataType: byte
T_831: (in Mem0[ds:bx + si:byte] + 0x03 : byte)
  Class: Eq_831
  DataType: byte
  OrigDataType: byte
T_832: (in v27 : byte)
  Class: Eq_831
  DataType: byte
  OrigDataType: byte
T_833: (in v27 : byte)
  Class: Eq_829
  DataType: byte
  OrigDataType: byte
T_834: (in bx + si : word16)
  Class: Eq_834
  DataType: word16
  OrigDataType: word16
T_835: (in Mem0[ds:bx + si:byte] : byte)
  Class: Eq_829
  DataType: byte
  OrigDataType: byte
T_836: (in v27 : byte)
  Class: Eq_836
  DataType: byte
  OrigDataType: byte
T_837: (in cond(v27) : byte)
  Class: Eq_696
  DataType: byte
  OrigDataType: byte
T_838: (in 0x0002 : word16)
  Class: Eq_838
  DataType: word16
  OrigDataType: word16
T_839: (in sp - 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_840: (in 0x0000 : word16)
  Class: Eq_840
  DataType: word16
  OrigDataType: word16
T_841: (in sp + 0x0000 : word16)
  Class: Eq_841
  DataType: word16
  OrigDataType: word16
T_842: (in Mem0[ss:sp + 0x0000:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_843: (in 0x0000 : word16)
  Class: Eq_843
  DataType: word16
  OrigDataType: word16
T_844: (in sp + 0x0000 : word16)
  Class: Eq_844
  DataType: word16
  OrigDataType: word16
T_845: (in Mem0[ss:sp + 0x0000:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_846: (in 0x0002 : word16)
  Class: Eq_846
  DataType: word16
  OrigDataType: word16
T_847: (in sp + 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_848: (in 0x0086 : word16)
  Class: Eq_848
  DataType: (memptr T_660 (struct (0 T_849 t0000)))
  OrigDataType: (memptr T_660 (struct (0 T_849 t0000)))
T_849: (in Mem0[ds:0x0086:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_850: (in 0x0000 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_851: (in 0x0000 : word16)
  Class: Eq_851
  DataType: word16
  OrigDataType: word16
T_852: (in cond(0x0000) : byte)
  Class: Eq_717
  DataType: byte
  OrigDataType: byte
T_853: (in false : bool)
  Class: Eq_698
  DataType: bool
  OrigDataType: bool
T_854: (in 0x0000 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_855: (in 0x0000 : word16)
  Class: Eq_855
  DataType: word16
  OrigDataType: word16
T_856: (in bp + 0x0000 : word16)
  Class: Eq_856
  DataType: word16
  OrigDataType: word16
T_857: (in Mem0[ss:bp + 0x0000:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_858: (in 0x0330 : word16)
  Class: Eq_858
  DataType: (memptr T_7 (struct (0 T_859 t0000)))
  OrigDataType: (memptr T_7 (struct (0 T_859 t0000)))
T_859: (in Mem0[0x0800:0x0330:word16] : word16)
  Class: Eq_655
  DataType: (union (word16 u0) ((ptr code) u1))
  OrigDataType: (union (word16 u0) ((ptr code) u1))
T_860: (in 0x0000 : word16)
  Class: Eq_860
  DataType: word16
  OrigDataType: word16
T_861: (in bp + 0x0000 : word16)
  Class: Eq_861
  DataType: word16
  OrigDataType: word16
T_862: (in Mem0[ss:bp + 0x0000:word16] : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_863: (in 0x0002 : word16)
  Class: Eq_863
  DataType: word16
  OrigDataType: word16
T_864: (in bp + 0x0002 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_865: (in cond(bp) : byte)
  Class: Eq_696
  DataType: byte
  OrigDataType: byte
T_866: (in 0x0000 : word16)
  Class: Eq_866
  DataType: word16
  OrigDataType: word16
T_867: (in si + 0x0000 : word16)
  Class: Eq_867
  DataType: word16
  OrigDataType: word16
T_868: (in Mem0[ds:si + 0x0000:byte] : byte)
  Class: Eq_673
  DataType: byte
  OrigDataType: byte
T_869: (in 0x0001 : word16)
  Class: Eq_869
  DataType: word16
  OrigDataType: word16
T_870: (in si + 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_871: (in al | al : byte)
  Class: Eq_673
  DataType: byte
  OrigDataType: byte
T_872: (in cond(al) : byte)
  Class: Eq_717
  DataType: byte
  OrigDataType: byte
T_873: (in false : bool)
  Class: Eq_698
  DataType: bool
  OrigDataType: bool
T_874: (in 0x0001 : word16)
  Class: Eq_874
  DataType: word16
  OrigDataType: word16
T_875: (in cx - 0x0001 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_876: (in Test(NE,Z) : bool)
  Class: Eq_876
  DataType: bool
  OrigDataType: bool
T_877: (in 0x0000 : word16)
  Class: Eq_2
  DataType: word16
  OrigDataType: word16
T_878: (in cx != 0x0000 : bool)
  Class: Eq_878
  DataType: bool
  OrigDataType: bool
T_879: (in Test(NE,Z) && cx != 0x0000 : bool)
  Class: Eq_879
  DataType: 
  OrigDataType: 
T_880: (in Test(EQ,Z) : bool)
  Class: Eq_880
  DataType: 
  OrigDataType: 
T_881: (in 0x0000 : word16)
  Class: Eq_2
  DataType: 
  OrigDataType: 
T_882: (in cx == 0x0000 : bool)
  Class: Eq_882
  DataType: 
  OrigDataType: 
T_883: (in fp : ptr16)
  Class: Eq_883
  DataType: 
  OrigDataType: 
T_884: (in sp : word16)
  Class: Eq_883
  DataType: 
  OrigDataType: 
T_885: (in ax : word16)
  Class: Eq_885
  DataType: 
  OrigDataType: 
T_886: (in ax | ax : word16)
  Class: Eq_885
  DataType: 
  OrigDataType: 
T_887: (in cond(ax) : byte)
  Class: Eq_887
  DataType: 
  OrigDataType: 
T_888: (in SZO : byte)
  Class: Eq_887
  DataType: 
  OrigDataType: 
T_889: (in false : bool)
  Class: Eq_889
  DataType: 
  OrigDataType: 
T_890: (in C : byte)
  Class: Eq_889
  DataType: 
  OrigDataType: 
T_891: (in Z : byte)
  Class: Eq_891
  DataType: 
  OrigDataType: 
T_892: (in Test(EQ,Z) : bool)
  Class: Eq_892
  DataType: 
  OrigDataType: 
T_893: (in al : byte)
  Class: Eq_893
  DataType: 
  OrigDataType: 
T_894: (in v12 : word16)
  Class: Eq_893
  DataType: 
  OrigDataType: 
T_895: (in ah : byte)
  Class: Eq_893
  DataType: 
  OrigDataType: 
T_896: (in 0x00 : byte)
  Class: Eq_893
  DataType: 
  OrigDataType: 
T_897: (in 0x00 : byte)
  Class: Eq_897
  DataType: 
  OrigDataType: 
T_898: (in cond(0x00) : byte)
  Class: Eq_887
  DataType: 
  OrigDataType: 
T_899: (in true : bool)
  Class: Eq_889
  DataType: 
  OrigDataType: 
T_900: (in cx : word16)
  Class: Eq_900
  DataType: 
  OrigDataType: 
T_901: (in 0x0000 : word16)
  Class: Eq_900
  DataType: 
  OrigDataType: 
T_902: (in cx == 0x0000 : bool)
  Class: Eq_902
  DataType: 
  OrigDataType: 
T_903: (in dx : word16)
  Class: Eq_903
  DataType: 
  OrigDataType: 
T_904: (in 0x0001 : word16)
  Class: Eq_904
  DataType: 
  OrigDataType: 
T_905: (in dx + 0x0001 : word16)
  Class: Eq_903
  DataType: 
  OrigDataType: 
T_906: (in es : selector)
  Class: Eq_906
  DataType: 
  OrigDataType: 
T_907: (in di : word16)
  Class: Eq_907
  DataType: 
  OrigDataType: 
T_908: (in 0x0000 : word16)
  Class: Eq_908
  DataType: 
  OrigDataType: 
T_909: (in di + 0x0000 : word16)
  Class: Eq_909
  DataType: 
  OrigDataType: 
T_910: (in Mem0[es:di + 0x0000:byte] : byte)
  Class: Eq_893
  DataType: 
  OrigDataType: 
T_911: (in 0x0001 : word16)
  Class: Eq_911
  DataType: 
  OrigDataType: 
T_912: (in di + 0x0001 : word16)
  Class: Eq_907
  DataType: 
  OrigDataType: 
T_913: (in al | al : byte)
  Class: Eq_893
  DataType: 
  OrigDataType: 
T_914: (in cond(al) : byte)
  Class: Eq_887
  DataType: 
  OrigDataType: 
T_915: (in false : bool)
  Class: Eq_889
  DataType: 
  OrigDataType: 
T_916: (in Test(NE,Z) : bool)
  Class: Eq_916
  DataType: 
  OrigDataType: 
T_917: (in bx : word16)
  Class: Eq_917
  DataType: 
  OrigDataType: 
T_918: (in 0x0001 : word16)
  Class: Eq_918
  DataType: 
  OrigDataType: 
T_919: (in bx + 0x0001 : word16)
  Class: Eq_917
  DataType: 
  OrigDataType: 
T_920: (in cond(bx) : byte)
  Class: Eq_887
  DataType: 
  OrigDataType: 
T_921: (in ds : selector)
  Class: Eq_921
  DataType: 
  OrigDataType: 
T_922: (in si : word16)
  Class: Eq_922
  DataType: 
  OrigDataType: 
T_923: (in 0x0000 : word16)
  Class: Eq_923
  DataType: 
  OrigDataType: 
T_924: (in si + 0x0000 : word16)
  Class: Eq_924
  DataType: 
  OrigDataType: 
T_925: (in Mem0[ds:si + 0x0000:byte] : byte)
  Class: Eq_893
  DataType: 
  OrigDataType: 
T_926: (in 0x0001 : word16)
  Class: Eq_926
  DataType: 
  OrigDataType: 
T_927: (in si + 0x0001 : word16)
  Class: Eq_922
  DataType: 
  OrigDataType: 
T_928: (in 0x0001 : word16)
  Class: Eq_928
  DataType: 
  OrigDataType: 
T_929: (in cx - 0x0001 : word16)
  Class: Eq_900
  DataType: 
  OrigDataType: 
T_930: (in 0x22 : byte)
  Class: Eq_930
  DataType: 
  OrigDataType: 
T_931: (in al - 0x22 : byte)
  Class: Eq_893
  DataType: 
  OrigDataType: 
T_932: (in cond(al) : byte)
  Class: Eq_932
  DataType: 
  OrigDataType: 
T_933: (in SCZO : byte)
  Class: Eq_932
  DataType: 
  OrigDataType: 
T_934: (in Test(EQ,Z) : bool)
  Class: Eq_934
  DataType: 
  OrigDataType: 
T_935: (in 0x22 : byte)
  Class: Eq_935
  DataType: 
  OrigDataType: 
T_936: (in al + 0x22 : byte)
  Class: Eq_893
  DataType: 
  OrigDataType: 
T_937: (in 0x5C : byte)
  Class: Eq_937
  DataType: 
  OrigDataType: 
T_938: (in al - 0x5C : byte)
  Class: Eq_938
  DataType: 
  OrigDataType: 
T_939: (in cond(al - 0x5C) : byte)
  Class: Eq_932
  DataType: 
  OrigDataType: 
T_940: (in Test(NE,Z) : bool)
  Class: Eq_940
  DataType: 
  OrigDataType: 
T_941: (in si | si : word16)
  Class: Eq_922
  DataType: 
  OrigDataType: 
T_942: (in cond(si) : byte)
  Class: Eq_887
  DataType: 
  OrigDataType: 
T_943: (in false : bool)
  Class: Eq_889
  DataType: 
  OrigDataType: 
T_944: (in 0x0000 : word16)
  Class: Eq_944
  DataType: 
  OrigDataType: 
T_945: (in si + 0x0000 : word16)
  Class: Eq_945
  DataType: 
  OrigDataType: 
T_946: (in Mem0[ds:si + 0x0000:byte] : byte)
  Class: Eq_946
  DataType: 
  OrigDataType: 
T_947: (in 0x22 : byte)
  Class: Eq_947
  DataType: 
  OrigDataType: 
T_948: (in Mem0[ds:si + 0x0000:byte] - 0x22 : byte)
  Class: Eq_948
  DataType: 
  OrigDataType: 
T_949: (in cond(Mem0[ds:si + 0x0000:byte] - 0x22) : byte)
  Class: Eq_932
  DataType: 
  OrigDataType: 
T_950: (in Test(NE,Z) : bool)
  Class: Eq_950
  DataType: 
  OrigDataType: 
T_951: (in 0x0000 : word16)
  Class: Eq_951
  DataType: 
  OrigDataType: 
T_952: (in si + 0x0000 : word16)
  Class: Eq_952
  DataType: 
  OrigDataType: 
T_953: (in Mem0[ds:si + 0x0000:byte] : byte)
  Class: Eq_893
  DataType: 
  OrigDataType: 
T_954: (in 0x0001 : word16)
  Class: Eq_954
  DataType: 
  OrigDataType: 
T_955: (in si + 0x0001 : word16)
  Class: Eq_922
  DataType: 
  OrigDataType: 
T_956: (in 0x0001 : word16)
  Class: Eq_956
  DataType: 
  OrigDataType: 
T_957: (in cx - 0x0001 : word16)
  Class: Eq_900
  DataType: 
  OrigDataType: 
T_958: (in cond(cx) : byte)
  Class: Eq_887
  DataType: 
  OrigDataType: 
T_959: (in fp : ptr16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_960: (in sp : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_961: (in ds : selector)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_962: (in 0x008A : word16)
  Class: Eq_962
  DataType: 
  OrigDataType: 
T_963: (in Mem0[ds:0x008A:word16] : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_964: (in cx : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_965: (in 0x0002 : word16)
  Class: Eq_965
  DataType: 
  OrigDataType: 
T_966: (in fp - 0x0002 : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_967: (in wLoc02 : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_968: (in fn0800_0570 : ptr32)
  Class: Eq_968
  DataType: 
  OrigDataType: 
T_969: (in signature of fn0800_0570 : void)
  Class: Eq_968
  DataType: 
  OrigDataType: 
T_970: (in ax : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_971: (in di : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_972: (in ax | ax : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_973: (in cond(ax) : byte)
  Class: Eq_973
  DataType: 
  OrigDataType: 
T_974: (in SZO : byte)
  Class: Eq_973
  DataType: 
  OrigDataType: 
T_975: (in false : bool)
  Class: Eq_975
  DataType: 
  OrigDataType: 
T_976: (in C : byte)
  Class: Eq_975
  DataType: 
  OrigDataType: 
T_977: (in Z : byte)
  Class: Eq_977
  DataType: 
  OrigDataType: 
T_978: (in Test(EQ,Z) : bool)
  Class: Eq_978
  DataType: 
  OrigDataType: 
T_979: (in 0x0002 : word16)
  Class: Eq_979
  DataType: 
  OrigDataType: 
T_980: (in fp - 0x0002 : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_981: (in 0x0004 : word16)
  Class: Eq_981
  DataType: 
  OrigDataType: 
T_982: (in fp - 0x0004 : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_983: (in wLoc04 : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_984: (in es : selector)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_985: (in 0x0002 : word16)
  Class: Eq_985
  DataType: 
  OrigDataType: 
T_986: (in fp - 0x0002 : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_987: (in 0x008C : word16)
  Class: Eq_987
  DataType: 
  OrigDataType: 
T_988: (in Mem0[ds:0x008C:selector] : selector)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_989: (in 0x0000 : word16)
  Class: Eq_989
  DataType: 
  OrigDataType: 
T_990: (in si : word16)
  Class: Eq_989
  DataType: 
  OrigDataType: 
T_991: (in 0x0000 : word16)
  Class: Eq_991
  DataType: 
  OrigDataType: 
T_992: (in cond(0x0000) : byte)
  Class: Eq_973
  DataType: 
  OrigDataType: 
T_993: (in false : bool)
  Class: Eq_975
  DataType: 
  OrigDataType: 
T_994: (in false : bool)
  Class: Eq_994
  DataType: 
  OrigDataType: 
T_995: (in D : byte)
  Class: Eq_994
  DataType: 
  OrigDataType: 
T_996: (in 0x0002 : word16)
  Class: Eq_996
  DataType: 
  OrigDataType: 
T_997: (in fp - 0x0002 : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_998: (in 0x0004 : word16)
  Class: Eq_998
  DataType: 
  OrigDataType: 
T_999: (in fp - 0x0004 : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_1000: (in 0x008E : word16)
  Class: Eq_1000
  DataType: 
  OrigDataType: 
T_1001: (in Mem0[ds:0x008E:word16] : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_1002: (in fn0800_0570 : ptr32)
  Class: Eq_968
  DataType: 
  OrigDataType: 
T_1003: (in 0x0002 : word16)
  Class: Eq_1003
  DataType: 
  OrigDataType: 
T_1004: (in fp - 0x0002 : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_1005: (in bx : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_1006: (in 0x0088 : word16)
  Class: Eq_1006
  DataType: 
  OrigDataType: 
T_1007: (in Mem0[ds:0x0088:word16] : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_1008: (in ax | ax : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_1009: (in cond(ax) : byte)
  Class: Eq_973
  DataType: 
  OrigDataType: 
T_1010: (in false : bool)
  Class: Eq_975
  DataType: 
  OrigDataType: 
T_1011: (in Test(NE,Z) : bool)
  Class: Eq_1011
  DataType: 
  OrigDataType: 
T_1012: (in 0x0000 : word16)
  Class: Eq_1012
  DataType: 
  OrigDataType: 
T_1013: (in si + 0x0000 : word16)
  Class: Eq_1013
  DataType: 
  OrigDataType: 
T_1014: (in Mem0[ds:si + 0x0000:byte] : byte)
  Class: Eq_1014
  DataType: 
  OrigDataType: 
T_1015: (in v14 : byte)
  Class: Eq_1014
  DataType: 
  OrigDataType: 
T_1016: (in 0x0000 : word16)
  Class: Eq_1016
  DataType: 
  OrigDataType: 
T_1017: (in di + 0x0000 : word16)
  Class: Eq_1017
  DataType: 
  OrigDataType: 
T_1018: (in Mem0[es:di + 0x0000:byte] : byte)
  Class: Eq_1014
  DataType: 
  OrigDataType: 
T_1019: (in 0x0001 : word16)
  Class: Eq_1019
  DataType: 
  OrigDataType: 
T_1020: (in si + 0x0001 : word16)
  Class: Eq_989
  DataType: 
  OrigDataType: 
T_1021: (in 0x0001 : word16)
  Class: Eq_1021
  DataType: 
  OrigDataType: 
T_1022: (in di + 0x0001 : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_1023: (in 0x0001 : word16)
  Class: Eq_1023
  DataType: 
  OrigDataType: 
T_1024: (in cx - 0x0001 : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_1025: (in 0x0000 : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_1026: (in cx == 0x0000 : bool)
  Class: Eq_1026
  DataType: 
  OrigDataType: 
T_1027: (in 0x001E : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_1028: (in 0x0056 : word16)
  Class: Eq_1028
  DataType: 
  OrigDataType: 
T_1029: (in dx : word16)
  Class: Eq_1028
  DataType: 
  OrigDataType: 
T_1030: (in 0x01F8 : word16)
  Class: Eq_1030
  DataType: 
  OrigDataType: 
T_1031: (in Mem0[0x0800:0x01F8:selector] : selector)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_1032: (in 0x0003 : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_1033: (in 0x0002 : word16)
  Class: Eq_1033
  DataType: 
  OrigDataType: 
T_1034: (in fp - 0x0002 : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_1035: (in 0x0003 : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_1036: (in bx + si : word16)
  Class: Eq_1036
  DataType: 
  OrigDataType: 
T_1037: (in Mem0[ds:bx + si:byte] : byte)
  Class: Eq_1037
  DataType: 
  OrigDataType: 
T_1038: (in al : byte)
  Class: Eq_1038
  DataType: 
  OrigDataType: 
T_1039: (in Mem0[ds:bx + si:byte] + al : byte)
  Class: Eq_1039
  DataType: 
  OrigDataType: 
T_1040: (in v27 : byte)
  Class: Eq_1039
  DataType: 
  OrigDataType: 
T_1041: (in v27 : byte)
  Class: Eq_1041
  DataType: 
  OrigDataType: 
T_1042: (in bx + si : word16)
  Class: Eq_1042
  DataType: 
  OrigDataType: 
T_1043: (in Mem0[ds:bx + si:byte] : byte)
  Class: Eq_1041
  DataType: 
  OrigDataType: 
T_1044: (in v27 : byte)
  Class: Eq_1044
  DataType: 
  OrigDataType: 
T_1045: (in cond(v27) : byte)
  Class: Eq_1045
  DataType: 
  OrigDataType: 
T_1046: (in SCZO : byte)
  Class: Eq_1045
  DataType: 
  OrigDataType: 
T_1047: (in 0x0004 : word16)
  Class: Eq_1047
  DataType: 
  OrigDataType: 
T_1048: (in fp - 0x0004 : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_1049: (in bp : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_1050: (in 0x0002 : word16)
  Class: Eq_1050
  DataType: 
  OrigDataType: 
T_1051: (in fp - 0x0002 : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_1052: (in 0x0000 : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_1053: (in 0x0000 : word16)
  Class: Eq_1053
  DataType: 
  OrigDataType: 
T_1054: (in cond(0x0000) : byte)
  Class: Eq_973
  DataType: 
  OrigDataType: 
T_1055: (in false : bool)
  Class: Eq_975
  DataType: 
  OrigDataType: 
T_1056: (in 0xFFFF : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_1057: (in 0x001E : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_1058: (in 0x0056 : word16)
  Class: Eq_1028
  DataType: 
  OrigDataType: 
T_1059: (in 0x01F8 : word16)
  Class: Eq_1059
  DataType: 
  OrigDataType: 
T_1060: (in Mem0[0x0800:0x01F8:selector] : selector)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_1061: (in 0x0003 : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_1062: (in 0x0002 : word16)
  Class: Eq_1062
  DataType: 
  OrigDataType: 
T_1063: (in fp - 0x0002 : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_1064: (in 0x0003 : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_1065: (in bx + si : word16)
  Class: Eq_1065
  DataType: 
  OrigDataType: 
T_1066: (in Mem0[ds:bx + si:byte] : byte)
  Class: Eq_1066
  DataType: 
  OrigDataType: 
T_1067: (in Mem0[ds:bx + si:byte] + al : byte)
  Class: Eq_1067
  DataType: 
  OrigDataType: 
T_1068: (in v27 : byte)
  Class: Eq_1067
  DataType: 
  OrigDataType: 
T_1069: (in v27 : byte)
  Class: Eq_1069
  DataType: 
  OrigDataType: 
T_1070: (in bx + si : word16)
  Class: Eq_1070
  DataType: 
  OrigDataType: 
T_1071: (in Mem0[ds:bx + si:byte] : byte)
  Class: Eq_1069
  DataType: 
  OrigDataType: 
T_1072: (in v27 : byte)
  Class: Eq_1072
  DataType: 
  OrigDataType: 
T_1073: (in cond(v27) : byte)
  Class: Eq_1045
  DataType: 
  OrigDataType: 
T_1074: (in 0x0004 : word16)
  Class: Eq_1074
  DataType: 
  OrigDataType: 
T_1075: (in fp - 0x0004 : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_1076: (in 0x0002 : word16)
  Class: Eq_1076
  DataType: 
  OrigDataType: 
T_1077: (in fp - 0x0002 : word16)
  Class: Eq_959
  DataType: 
  OrigDataType: 
T_1078: (in 0x0000 : word16)
  Class: Eq_1078
  DataType: 
  OrigDataType: 
T_1079: (in di + 0x0000 : word16)
  Class: Eq_1079
  DataType: 
  OrigDataType: 
T_1080: (in Mem0[es:di + 0x0000:byte] : byte)
  Class: Eq_1080
  DataType: 
  OrigDataType: 
T_1081: (in Mem0[es:di + 0x0000:byte] - al : byte)
  Class: Eq_1081
  DataType: 
  OrigDataType: 
T_1082: (in cond(Mem0[es:di + 0x0000:byte] - al) : byte)
  Class: Eq_1045
  DataType: 
  OrigDataType: 
T_1083: (in Test(NE,Z) : bool)
  Class: Eq_1083
  DataType: 
  OrigDataType: 
T_1084: (in 0x0000 : word16)
  Class: Eq_1084
  DataType: 
  OrigDataType: 
T_1085: (in di + 0x0000 : word16)
  Class: Eq_1085
  DataType: 
  OrigDataType: 
T_1086: (in Mem0[es:di + 0x0000:byte] : byte)
  Class: Eq_1086
  DataType: 
  OrigDataType: 
T_1087: (in al - Mem0[es:di + 0x0000:byte] : byte)
  Class: Eq_1087
  DataType: 
  OrigDataType: 
T_1088: (in cond(al - Mem0[es:di + 0x0000:byte]) : byte)
  Class: Eq_1045
  DataType: 
  OrigDataType: 
T_1089: (in 0x0001 : word16)
  Class: Eq_1089
  DataType: 
  OrigDataType: 
T_1090: (in di + 0x0001 : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_1091: (in 0x0001 : word16)
  Class: Eq_1091
  DataType: 
  OrigDataType: 
T_1092: (in cx - 0x0001 : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_1093: (in Test(NE,Z) : bool)
  Class: Eq_1093
  DataType: 
  OrigDataType: 
T_1094: (in 0x0000 : word16)
  Class: Eq_961
  DataType: 
  OrigDataType: 
T_1095: (in cx == 0x0000 : bool)
  Class: Eq_1095
  DataType: 
  OrigDataType: 
T_1096: (in 0x0000 : word16)
  Class: Eq_1096
  DataType: 
  OrigDataType: 
T_1097: (in bx + 0x0000 : word16)
  Class: Eq_1097
  DataType: 
  OrigDataType: 
T_1098: (in Mem0[ds:bx + 0x0000:word16] : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_1099: (in 0x0002 : word16)
  Class: Eq_1099
  DataType: 
  OrigDataType: 
T_1100: (in bx + 0x0002 : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_1101: (in cond(bx) : byte)
  Class: Eq_1045
  DataType: 
  OrigDataType: 
T_1102: (in 0x0000 : word16)
  Class: Eq_1102
  DataType: 
  OrigDataType: 
T_1103: (in bx + 0x0000 : word16)
  Class: Eq_1103
  DataType: 
  OrigDataType: 
T_1104: (in Mem0[ds:bx + 0x0000:word16] : word16)
  Class: Eq_970
  DataType: 
  OrigDataType: 
T_1105: (in fp : ptr16)
  Class: Eq_1105
  DataType: 
  OrigDataType: 
T_1106: (in sp : word16)
  Class: Eq_1105
  DataType: 
  OrigDataType: 
T_1107: (in 0x0002 : word16)
  Class: Eq_1107
  DataType: 
  OrigDataType: 
T_1108: (in fp - 0x0002 : word16)
  Class: Eq_1105
  DataType: 
  OrigDataType: 
T_1109: (in bp : word16)
  Class: Eq_1109
  DataType: 
  OrigDataType: 
T_1110: (in wLoc02 : word16)
  Class: Eq_1109
  DataType: 
  OrigDataType: 
T_1111: (in fp - 0x0002 : word16)
  Class: Eq_1109
  DataType: 
  OrigDataType: 
T_1112: (in 0x0004 : word16)
  Class: Eq_1112
  DataType: 
  OrigDataType: 
T_1113: (in fp - 0x0004 : word16)
  Class: Eq_1105
  DataType: 
  OrigDataType: 
T_1114: (in si : word16)
  Class: Eq_1114
  DataType: 
  OrigDataType: 
T_1115: (in wLoc04 : word16)
  Class: Eq_1114
  DataType: 
  OrigDataType: 
T_1116: (in 0x0006 : word16)
  Class: Eq_1116
  DataType: 
  OrigDataType: 
T_1117: (in fp - 0x0006 : word16)
  Class: Eq_1105
  DataType: 
  OrigDataType: 
T_1118: (in di : word16)
  Class: Eq_1118
  DataType: 
  OrigDataType: 
T_1119: (in wLoc06 : word16)
  Class: Eq_1118
  DataType: 
  OrigDataType: 
T_1120: (in wArg02 : word16)
  Class: Eq_1118
  DataType: 
  OrigDataType: 
T_1121: (in ds : selector)
  Class: Eq_1121
  DataType: 
  OrigDataType: 
T_1122: (in 0x0006 : word16)
  Class: Eq_1122
  DataType: 
  OrigDataType: 
T_1123: (in di + 0x0006 : word16)
  Class: Eq_1123
  DataType: 
  OrigDataType: 
T_1124: (in Mem0[ds:di + 0x0006:word16] : word16)
  Class: Eq_1124
  DataType: 
  OrigDataType: 
T_1125: (in ax : word16)
  Class: Eq_1124
  DataType: 
  OrigDataType: 
T_1126: (in 0x062A : word16)
  Class: Eq_1126
  DataType: 
  OrigDataType: 
T_1127: (in Mem0[ds:0x062A:word16] : word16)
  Class: Eq_1124
  DataType: 
  OrigDataType: 
T_1128: (in ax - di : word16)
  Class: Eq_1128
  DataType: 
  OrigDataType: 
T_1129: (in cond(ax - di) : byte)
  Class: Eq_1129
  DataType: 
  OrigDataType: 
T_1130: (in SCZO : byte)
  Class: Eq_1129
  DataType: 
  OrigDataType: 
T_1131: (in Z : byte)
  Class: Eq_1131
  DataType: 
  OrigDataType: 
T_1132: (in Test(NE,Z) : bool)
  Class: Eq_1132
  DataType: 
  OrigDataType: 
T_1133: (in 0x0004 : word16)
  Class: Eq_1133
  DataType: 
  OrigDataType: 
T_1134: (in di + 0x0004 : word16)
  Class: Eq_1134
  DataType: 
  OrigDataType: 
T_1135: (in Mem0[ds:di + 0x0004:word16] : word16)
  Class: Eq_1114
  DataType: 
  OrigDataType: 
T_1136: (in 0x062A : word16)
  Class: Eq_1136
  DataType: 
  OrigDataType: 
T_1137: (in Mem0[ds:0x062A:word16] : word16)
  Class: Eq_1137
  DataType: 
  OrigDataType: 
T_1138: (in bx : word16)
  Class: Eq_1137
  DataType: 
  OrigDataType: 
T_1139: (in 0x0004 : word16)
  Class: Eq_1139
  DataType: 
  OrigDataType: 
T_1140: (in bx + 0x0004 : word16)
  Class: Eq_1140
  DataType: 
  OrigDataType: 
T_1141: (in Mem0[ds:bx + 0x0004:word16] : word16)
  Class: Eq_1114
  DataType: 
  OrigDataType: 
T_1142: (in 0x062A : word16)
  Class: Eq_1142
  DataType: 
  OrigDataType: 
T_1143: (in Mem0[ds:0x062A:word16] : word16)
  Class: Eq_1124
  DataType: 
  OrigDataType: 
T_1144: (in 0x0006 : word16)
  Class: Eq_1144
  DataType: 
  OrigDataType: 
T_1145: (in si + 0x0006 : word16)
  Class: Eq_1145
  DataType: 
  OrigDataType: 
T_1146: (in Mem0[ds:si + 0x0006:word16] : word16)
  Class: Eq_1124
  DataType: 
  OrigDataType: 
T_1147: (in 0x0000 : word16)
  Class: Eq_1147
  DataType: 
  OrigDataType: 
T_1148: (in 0x062A : word16)
  Class: Eq_1148
  DataType: 
  OrigDataType: 
T_1149: (in Mem0[ds:0x062A:word16] : word16)
  Class: Eq_1147
  DataType: 
  OrigDataType: 
T_1150: (in 0x0004 : word16)
  Class: Eq_1150
  DataType: 
  OrigDataType: 
T_1151: (in fp - 0x0004 : word16)
  Class: Eq_1105
  DataType: 
  OrigDataType: 
T_1152: (in 0x0002 : word16)
  Class: Eq_1152
  DataType: 
  OrigDataType: 
T_1153: (in fp - 0x0002 : word16)
  Class: Eq_1105
  DataType: 
  OrigDataType: 
T_1154: (in fp : ptr16)
  Class: Eq_1154
  DataType: 
  OrigDataType: 
T_1155: (in sp : word16)
  Class: Eq_1154
  DataType: 
  OrigDataType: 
T_1156: (in 0x0002 : word16)
  Class: Eq_1156
  DataType: 
  OrigDataType: 
T_1157: (in fp - 0x0002 : word16)
  Class: Eq_1154
  DataType: 
  OrigDataType: 
T_1158: (in bp : word16)
  Class: Eq_1158
  DataType: 
  OrigDataType: 
T_1159: (in wLoc02 : word16)
  Class: Eq_1158
  DataType: 
  OrigDataType: 
T_1160: (in fp - 0x0002 : word16)
  Class: Eq_1158
  DataType: 
  OrigDataType: 
T_1161: (in 0x0004 : word16)
  Class: Eq_1161
  DataType: 
  OrigDataType: 
T_1162: (in fp - 0x0004 : word16)
  Class: Eq_1154
  DataType: 
  OrigDataType: 
T_1163: (in si : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1164: (in wLoc04 : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1165: (in 0x0006 : word16)
  Class: Eq_1165
  DataType: 
  OrigDataType: 
T_1166: (in fp - 0x0006 : word16)
  Class: Eq_1154
  DataType: 
  OrigDataType: 
T_1167: (in di : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1168: (in wLoc06 : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1169: (in wArg02 : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1170: (in wArg04 : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1171: (in ax : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1172: (in ds : selector)
  Class: Eq_1172
  DataType: 
  OrigDataType: 
T_1173: (in 0x0000 : word16)
  Class: Eq_1173
  DataType: 
  OrigDataType: 
T_1174: (in di + 0x0000 : word16)
  Class: Eq_1174
  DataType: 
  OrigDataType: 
T_1175: (in Mem0[ds:di + 0x0000:word16] : word16)
  Class: Eq_1175
  DataType: 
  OrigDataType: 
T_1176: (in Mem0[ds:di + 0x0000:word16] - ax : word16)
  Class: Eq_1176
  DataType: 
  OrigDataType: 
T_1177: (in v9 : word16)
  Class: Eq_1176
  DataType: 
  OrigDataType: 
T_1178: (in 0x0000 : word16)
  Class: Eq_1178
  DataType: 
  OrigDataType: 
T_1179: (in di + 0x0000 : word16)
  Class: Eq_1179
  DataType: 
  OrigDataType: 
T_1180: (in Mem0[ds:di + 0x0000:word16] : word16)
  Class: Eq_1176
  DataType: 
  OrigDataType: 
T_1181: (in 0x0000 : word16)
  Class: Eq_1181
  DataType: 
  OrigDataType: 
T_1182: (in di + 0x0000 : word16)
  Class: Eq_1182
  DataType: 
  OrigDataType: 
T_1183: (in Mem0[ds:di + 0x0000:word16] : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1184: (in si + di : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1185: (in 0x0001 : word16)
  Class: Eq_1185
  DataType: 
  OrigDataType: 
T_1186: (in ax + 0x0001 : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1187: (in 0x0000 : word16)
  Class: Eq_1187
  DataType: 
  OrigDataType: 
T_1188: (in si + 0x0000 : word16)
  Class: Eq_1188
  DataType: 
  OrigDataType: 
T_1189: (in Mem0[ds:si + 0x0000:word16] : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1190: (in 0x0002 : word16)
  Class: Eq_1190
  DataType: 
  OrigDataType: 
T_1191: (in si + 0x0002 : word16)
  Class: Eq_1191
  DataType: 
  OrigDataType: 
T_1192: (in Mem0[ds:si + 0x0002:word16] : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1193: (in 0x0628 : word16)
  Class: Eq_1193
  DataType: 
  OrigDataType: 
T_1194: (in Mem0[ds:0x0628:word16] : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1195: (in ax - di : word16)
  Class: Eq_1195
  DataType: 
  OrigDataType: 
T_1196: (in cond(ax - di) : byte)
  Class: Eq_1196
  DataType: 
  OrigDataType: 
T_1197: (in SCZO : byte)
  Class: Eq_1196
  DataType: 
  OrigDataType: 
T_1198: (in Z : byte)
  Class: Eq_1198
  DataType: 
  OrigDataType: 
T_1199: (in Test(NE,Z) : bool)
  Class: Eq_1199
  DataType: 
  OrigDataType: 
T_1200: (in di + wArg04 : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1201: (in cond(di) : byte)
  Class: Eq_1196
  DataType: 
  OrigDataType: 
T_1202: (in 0x0002 : word16)
  Class: Eq_1202
  DataType: 
  OrigDataType: 
T_1203: (in di + 0x0002 : word16)
  Class: Eq_1203
  DataType: 
  OrigDataType: 
T_1204: (in Mem0[ds:di + 0x0002:word16] : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1205: (in 0x0628 : word16)
  Class: Eq_1205
  DataType: 
  OrigDataType: 
T_1206: (in Mem0[ds:0x0628:word16] : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1207: (in 0x0004 : word16)
  Class: Eq_1207
  DataType: 
  OrigDataType: 
T_1208: (in ax + 0x0004 : word16)
  Class: Eq_1163
  DataType: 
  OrigDataType: 
T_1209: (in cond(ax) : byte)
  Class: Eq_1196
  DataType: 
  OrigDataType: 
T_1210: (in 0x0004 : word16)
  Class: Eq_1210
  DataType: 
  OrigDataType: 
T_1211: (in fp - 0x0004 : word16)
  Class: Eq_1154
  DataType: 
  OrigDataType: 
T_1212: (in 0x0002 : word16)
  Class: Eq_1212
  DataType: 
  OrigDataType: 
T_1213: (in fp - 0x0002 : word16)
  Class: Eq_1154
  DataType: 
  OrigDataType: 
T_1214: (in fp : ptr16)
  Class: Eq_1214
  DataType: 
  OrigDataType: 
T_1215: (in sp : word16)
  Class: Eq_1214
  DataType: 
  OrigDataType: 
T_1216: (in 0x0002 : word16)
  Class: Eq_1216
  DataType: 
  OrigDataType: 
T_1217: (in fp - 0x0002 : word16)
  Class: Eq_1214
  DataType: 
  OrigDataType: 
T_1218: (in bp : word16)
  Class: Eq_1218
  DataType: 
  OrigDataType: 
T_1219: (in wLoc02 : word16)
  Class: Eq_1218
  DataType: 
  OrigDataType: 
T_1220: (in fp - 0x0002 : word16)
  Class: Eq_1218
  DataType: 
  OrigDataType: 
T_1221: (in 0x0004 : word16)
  Class: Eq_1221
  DataType: 
  OrigDataType: 
T_1222: (in fp - 0x0004 : word16)
  Class: Eq_1214
  DataType: 
  OrigDataType: 
T_1223: (in si : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1224: (in wLoc04 : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1225: (in wArg02 : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1226: (in ax : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1227: (in 0x0000 : word16)
  Class: Eq_1227
  DataType: 
  OrigDataType: 
T_1228: (in dx : word16)
  Class: Eq_1227
  DataType: 
  OrigDataType: 
T_1229: (in 0xFFFF : word16)
  Class: Eq_1229
  DataType: 
  OrigDataType: 
T_1230: (in ax & 0xFFFF : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1231: (in 0x0000 : word16)
  Class: Eq_1227
  DataType: 
  OrigDataType: 
T_1232: (in 0x0000 : word16)
  Class: Eq_1232
  DataType: 
  OrigDataType: 
T_1233: (in cond(0x0000) : byte)
  Class: Eq_1233
  DataType: 
  OrigDataType: 
T_1234: (in SZO : byte)
  Class: Eq_1233
  DataType: 
  OrigDataType: 
T_1235: (in false : bool)
  Class: Eq_1235
  DataType: 
  OrigDataType: 
T_1236: (in C : byte)
  Class: Eq_1235
  DataType: 
  OrigDataType: 
T_1237: (in 0x0006 : word16)
  Class: Eq_1237
  DataType: 
  OrigDataType: 
T_1238: (in fp - 0x0006 : word16)
  Class: Eq_1214
  DataType: 
  OrigDataType: 
T_1239: (in 0x0000 : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1240: (in wLoc06 : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1241: (in 0x0008 : word16)
  Class: Eq_1241
  DataType: 
  OrigDataType: 
T_1242: (in fp - 0x0008 : word16)
  Class: Eq_1214
  DataType: 
  OrigDataType: 
T_1243: (in wLoc08 : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1244: (in fn0800_0607 : ptr32)
  Class: Eq_1244
  DataType: 
  OrigDataType: 
T_1245: (in signature of fn0800_0607 : void)
  Class: Eq_1244
  DataType: 
  OrigDataType: 
T_1246: (in cx : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1247: (in 0x0006 : word16)
  Class: Eq_1247
  DataType: 
  OrigDataType: 
T_1248: (in fp - 0x0006 : word16)
  Class: Eq_1214
  DataType: 
  OrigDataType: 
T_1249: (in 0x0004 : word16)
  Class: Eq_1249
  DataType: 
  OrigDataType: 
T_1250: (in fp - 0x0004 : word16)
  Class: Eq_1214
  DataType: 
  OrigDataType: 
T_1251: (in 0xFFFF : word16)
  Class: Eq_1251
  DataType: 
  OrigDataType: 
T_1252: (in si - 0xFFFF : word16)
  Class: Eq_1252
  DataType: 
  OrigDataType: 
T_1253: (in cond(si - 0xFFFF) : byte)
  Class: Eq_1253
  DataType: 
  OrigDataType: 
T_1254: (in SCZO : byte)
  Class: Eq_1253
  DataType: 
  OrigDataType: 
T_1255: (in Z : byte)
  Class: Eq_1255
  DataType: 
  OrigDataType: 
T_1256: (in Test(NE,Z) : bool)
  Class: Eq_1256
  DataType: 
  OrigDataType: 
T_1257: (in ds : selector)
  Class: Eq_1257
  DataType: 
  OrigDataType: 
T_1258: (in 0x0628 : word16)
  Class: Eq_1258
  DataType: 
  OrigDataType: 
T_1259: (in Mem0[ds:0x0628:word16] : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1260: (in 0x0002 : word16)
  Class: Eq_1260
  DataType: 
  OrigDataType: 
T_1261: (in si + 0x0002 : word16)
  Class: Eq_1261
  DataType: 
  OrigDataType: 
T_1262: (in Mem0[ds:si + 0x0002:word16] : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1263: (in 0x0001 : word16)
  Class: Eq_1263
  DataType: 
  OrigDataType: 
T_1264: (in ax + 0x0001 : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1265: (in 0x0000 : word16)
  Class: Eq_1265
  DataType: 
  OrigDataType: 
T_1266: (in si + 0x0000 : word16)
  Class: Eq_1266
  DataType: 
  OrigDataType: 
T_1267: (in Mem0[ds:si + 0x0000:word16] : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1268: (in 0x0628 : word16)
  Class: Eq_1268
  DataType: 
  OrigDataType: 
T_1269: (in Mem0[ds:0x0628:word16] : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1270: (in 0x0628 : word16)
  Class: Eq_1270
  DataType: 
  OrigDataType: 
T_1271: (in Mem0[ds:0x0628:word16] : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1272: (in 0x0004 : word16)
  Class: Eq_1272
  DataType: 
  OrigDataType: 
T_1273: (in ax + 0x0004 : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1274: (in cond(ax) : byte)
  Class: Eq_1253
  DataType: 
  OrigDataType: 
T_1275: (in 0x0000 : word16)
  Class: Eq_1223
  DataType: 
  OrigDataType: 
T_1276: (in 0x0000 : word16)
  Class: Eq_1276
  DataType: 
  OrigDataType: 
T_1277: (in cond(0x0000) : byte)
  Class: Eq_1233
  DataType: 
  OrigDataType: 
T_1278: (in false : bool)
  Class: Eq_1235
  DataType: 
  OrigDataType: 
T_1279: (in 0x0002 : word16)
  Class: Eq_1279
  DataType: 
  OrigDataType: 
T_1280: (in fp - 0x0002 : word16)
  Class: Eq_1214
  DataType: 
  OrigDataType: 
T_1281: (in fp : ptr16)
  Class: Eq_1281
  DataType: 
  OrigDataType: 
T_1282: (in sp : word16)
  Class: Eq_1281
  DataType: 
  OrigDataType: 
T_1283: (in 0x0002 : word16)
  Class: Eq_1283
  DataType: 
  OrigDataType: 
T_1284: (in fp - 0x0002 : word16)
  Class: Eq_1281
  DataType: 
  OrigDataType: 
T_1285: (in bp : word16)
  Class: Eq_1285
  DataType: 
  OrigDataType: 
T_1286: (in wLoc02 : word16)
  Class: Eq_1285
  DataType: 
  OrigDataType: 
T_1287: (in fp - 0x0002 : word16)
  Class: Eq_1285
  DataType: 
  OrigDataType: 
T_1288: (in 0x0004 : word16)
  Class: Eq_1288
  DataType: 
  OrigDataType: 
T_1289: (in fp - 0x0004 : word16)
  Class: Eq_1281
  DataType: 
  OrigDataType: 
T_1290: (in si : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1291: (in wLoc04 : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1292: (in wArg02 : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1293: (in ax : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1294: (in 0x0000 : word16)
  Class: Eq_1294
  DataType: 
  OrigDataType: 
T_1295: (in dx : word16)
  Class: Eq_1294
  DataType: 
  OrigDataType: 
T_1296: (in 0xFFFF : word16)
  Class: Eq_1296
  DataType: 
  OrigDataType: 
T_1297: (in ax & 0xFFFF : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1298: (in 0x0000 : word16)
  Class: Eq_1294
  DataType: 
  OrigDataType: 
T_1299: (in 0x0000 : word16)
  Class: Eq_1299
  DataType: 
  OrigDataType: 
T_1300: (in cond(0x0000) : byte)
  Class: Eq_1300
  DataType: 
  OrigDataType: 
T_1301: (in SZO : byte)
  Class: Eq_1300
  DataType: 
  OrigDataType: 
T_1302: (in false : bool)
  Class: Eq_1302
  DataType: 
  OrigDataType: 
T_1303: (in C : byte)
  Class: Eq_1302
  DataType: 
  OrigDataType: 
T_1304: (in 0x0006 : word16)
  Class: Eq_1304
  DataType: 
  OrigDataType: 
T_1305: (in fp - 0x0006 : word16)
  Class: Eq_1281
  DataType: 
  OrigDataType: 
T_1306: (in 0x0000 : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1307: (in wLoc06 : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1308: (in 0x0008 : word16)
  Class: Eq_1308
  DataType: 
  OrigDataType: 
T_1309: (in fp - 0x0008 : word16)
  Class: Eq_1281
  DataType: 
  OrigDataType: 
T_1310: (in wLoc08 : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1311: (in fn0800_0607 : ptr32)
  Class: Eq_1244
  DataType: 
  OrigDataType: 
T_1312: (in cx : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1313: (in 0x0006 : word16)
  Class: Eq_1313
  DataType: 
  OrigDataType: 
T_1314: (in fp - 0x0006 : word16)
  Class: Eq_1281
  DataType: 
  OrigDataType: 
T_1315: (in 0x0004 : word16)
  Class: Eq_1315
  DataType: 
  OrigDataType: 
T_1316: (in fp - 0x0004 : word16)
  Class: Eq_1281
  DataType: 
  OrigDataType: 
T_1317: (in 0xFFFF : word16)
  Class: Eq_1317
  DataType: 
  OrigDataType: 
T_1318: (in si - 0xFFFF : word16)
  Class: Eq_1318
  DataType: 
  OrigDataType: 
T_1319: (in cond(si - 0xFFFF) : byte)
  Class: Eq_1319
  DataType: 
  OrigDataType: 
T_1320: (in SCZO : byte)
  Class: Eq_1319
  DataType: 
  OrigDataType: 
T_1321: (in Z : byte)
  Class: Eq_1321
  DataType: 
  OrigDataType: 
T_1322: (in Test(NE,Z) : bool)
  Class: Eq_1322
  DataType: 
  OrigDataType: 
T_1323: (in ds : selector)
  Class: Eq_1323
  DataType: 
  OrigDataType: 
T_1324: (in 0x062C : word16)
  Class: Eq_1324
  DataType: 
  OrigDataType: 
T_1325: (in Mem0[ds:0x062C:word16] : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1326: (in 0x0628 : word16)
  Class: Eq_1326
  DataType: 
  OrigDataType: 
T_1327: (in Mem0[ds:0x0628:word16] : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1328: (in 0x0001 : word16)
  Class: Eq_1328
  DataType: 
  OrigDataType: 
T_1329: (in ax + 0x0001 : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1330: (in 0x0000 : word16)
  Class: Eq_1330
  DataType: 
  OrigDataType: 
T_1331: (in si + 0x0000 : word16)
  Class: Eq_1331
  DataType: 
  OrigDataType: 
T_1332: (in Mem0[ds:si + 0x0000:word16] : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1333: (in 0x0004 : word16)
  Class: Eq_1333
  DataType: 
  OrigDataType: 
T_1334: (in ax + 0x0004 : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1335: (in cond(ax) : byte)
  Class: Eq_1319
  DataType: 
  OrigDataType: 
T_1336: (in 0x0000 : word16)
  Class: Eq_1290
  DataType: 
  OrigDataType: 
T_1337: (in 0x0000 : word16)
  Class: Eq_1337
  DataType: 
  OrigDataType: 
T_1338: (in cond(0x0000) : byte)
  Class: Eq_1300
  DataType: 
  OrigDataType: 
T_1339: (in false : bool)
  Class: Eq_1302
  DataType: 
  OrigDataType: 
T_1340: (in 0x0002 : word16)
  Class: Eq_1340
  DataType: 
  OrigDataType: 
T_1341: (in fp - 0x0002 : word16)
  Class: Eq_1281
  DataType: 
  OrigDataType: 
T_1342: (in fp : ptr16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1343: (in sp : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1344: (in 0x0002 : word16)
  Class: Eq_1344
  DataType: 
  OrigDataType: 
T_1345: (in fp - 0x0002 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1346: (in bp : word16)
  Class: Eq_1346
  DataType: 
  OrigDataType: 
T_1347: (in wLoc02 : word16)
  Class: Eq_1346
  DataType: 
  OrigDataType: 
T_1348: (in fp - 0x0002 : word16)
  Class: Eq_1346
  DataType: 
  OrigDataType: 
T_1349: (in 0x0004 : word16)
  Class: Eq_1349
  DataType: 
  OrigDataType: 
T_1350: (in fp - 0x0004 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1351: (in si : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1352: (in wLoc04 : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1353: (in 0x0006 : word16)
  Class: Eq_1353
  DataType: 
  OrigDataType: 
T_1354: (in fp - 0x0006 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1355: (in di : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1356: (in wLoc06 : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1357: (in wArg02 : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1358: (in di | di : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1359: (in cond(di) : byte)
  Class: Eq_1359
  DataType: 
  OrigDataType: 
T_1360: (in SZO : byte)
  Class: Eq_1359
  DataType: 
  OrigDataType: 
T_1361: (in false : bool)
  Class: Eq_1361
  DataType: 
  OrigDataType: 
T_1362: (in C : byte)
  Class: Eq_1361
  DataType: 
  OrigDataType: 
T_1363: (in Z : byte)
  Class: Eq_1363
  DataType: 
  OrigDataType: 
T_1364: (in Test(EQ,Z) : bool)
  Class: Eq_1364
  DataType: 
  OrigDataType: 
T_1365: (in 0x0000 : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1366: (in ax : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1367: (in 0x0000 : word16)
  Class: Eq_1367
  DataType: 
  OrigDataType: 
T_1368: (in cond(0x0000) : byte)
  Class: Eq_1359
  DataType: 
  OrigDataType: 
T_1369: (in false : bool)
  Class: Eq_1361
  DataType: 
  OrigDataType: 
T_1370: (in 0xFFF4 : word16)
  Class: Eq_1370
  DataType: 
  OrigDataType: 
T_1371: (in di - 0xFFF4 : word16)
  Class: Eq_1371
  DataType: 
  OrigDataType: 
T_1372: (in cond(di - 0xFFF4) : byte)
  Class: Eq_1372
  DataType: 
  OrigDataType: 
T_1373: (in SCZO : byte)
  Class: Eq_1372
  DataType: 
  OrigDataType: 
T_1374: (in CZ : byte)
  Class: Eq_1374
  DataType: 
  OrigDataType: 
T_1375: (in Test(ULE,CZ) : bool)
  Class: Eq_1375
  DataType: 
  OrigDataType: 
T_1376: (in 0x000B : word16)
  Class: Eq_1376
  DataType: 
  OrigDataType: 
T_1377: (in ax + 0x000B : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1378: (in 0xFFF8 : word16)
  Class: Eq_1378
  DataType: 
  OrigDataType: 
T_1379: (in ax & 0xFFF8 : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1380: (in ds : selector)
  Class: Eq_1380
  DataType: 
  OrigDataType: 
T_1381: (in 0x062C : word16)
  Class: Eq_1381
  DataType: 
  OrigDataType: 
T_1382: (in Mem0[ds:0x062C:word16] : word16)
  Class: Eq_1382
  DataType: 
  OrigDataType: 
T_1383: (in 0x0000 : word16)
  Class: Eq_1383
  DataType: 
  OrigDataType: 
T_1384: (in Mem0[ds:0x062C:word16] - 0x0000 : word16)
  Class: Eq_1384
  DataType: 
  OrigDataType: 
T_1385: (in cond(Mem0[ds:0x062C:word16] - 0x0000) : byte)
  Class: Eq_1372
  DataType: 
  OrigDataType: 
T_1386: (in Test(NE,Z) : bool)
  Class: Eq_1386
  DataType: 
  OrigDataType: 
T_1387: (in 0x0004 : word16)
  Class: Eq_1387
  DataType: 
  OrigDataType: 
T_1388: (in fp - 0x0004 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1389: (in 0x0002 : word16)
  Class: Eq_1389
  DataType: 
  OrigDataType: 
T_1390: (in fp - 0x0002 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1391: (in 0x062A : word16)
  Class: Eq_1391
  DataType: 
  OrigDataType: 
T_1392: (in Mem0[ds:0x062A:word16] : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1393: (in ax | ax : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1394: (in cond(ax) : byte)
  Class: Eq_1359
  DataType: 
  OrigDataType: 
T_1395: (in false : bool)
  Class: Eq_1361
  DataType: 
  OrigDataType: 
T_1396: (in Test(EQ,Z) : bool)
  Class: Eq_1396
  DataType: 
  OrigDataType: 
T_1397: (in 0x0008 : word16)
  Class: Eq_1397
  DataType: 
  OrigDataType: 
T_1398: (in fp - 0x0008 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1399: (in wLoc08 : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1400: (in fn0800_0536 : ptr32)
  Class: Eq_1400
  DataType: 
  OrigDataType: 
T_1401: (in signature of fn0800_0536 : void)
  Class: Eq_1400
  DataType: 
  OrigDataType: 
T_1402: (in cx : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1403: (in 0x0006 : word16)
  Class: Eq_1403
  DataType: 
  OrigDataType: 
T_1404: (in fp - 0x0006 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1405: (in 0x0008 : word16)
  Class: Eq_1405
  DataType: 
  OrigDataType: 
T_1406: (in fp - 0x0008 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1407: (in fn0800_04F9 : ptr32)
  Class: Eq_1407
  DataType: 
  OrigDataType: 
T_1408: (in signature of fn0800_04F9 : void)
  Class: Eq_1407
  DataType: 
  OrigDataType: 
T_1409: (in 0x0006 : word16)
  Class: Eq_1409
  DataType: 
  OrigDataType: 
T_1410: (in fp - 0x0006 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1411: (in 0x0008 : word16)
  Class: Eq_1411
  DataType: 
  OrigDataType: 
T_1412: (in fp - 0x0008 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1413: (in fn0800_04F9 : ptr32)
  Class: Eq_1407
  DataType: 
  OrigDataType: 
T_1414: (in 0x0006 : word16)
  Class: Eq_1414
  DataType: 
  OrigDataType: 
T_1415: (in fp - 0x0006 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1416: (in 0x0000 : word16)
  Class: Eq_1416
  DataType: 
  OrigDataType: 
T_1417: (in si + 0x0000 : word16)
  Class: Eq_1417
  DataType: 
  OrigDataType: 
T_1418: (in Mem0[ds:si + 0x0000:word16] : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1419: (in dx : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1420: (in 0x0028 : word16)
  Class: Eq_1420
  DataType: 
  OrigDataType: 
T_1421: (in dx + 0x0028 : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1422: (in ax - dx : word16)
  Class: Eq_1422
  DataType: 
  OrigDataType: 
T_1423: (in cond(ax - dx) : byte)
  Class: Eq_1372
  DataType: 
  OrigDataType: 
T_1424: (in Test(ULT,C) : bool)
  Class: Eq_1424
  DataType: 
  OrigDataType: 
T_1425: (in 0x0000 : word16)
  Class: Eq_1425
  DataType: 
  OrigDataType: 
T_1426: (in si + 0x0000 : word16)
  Class: Eq_1426
  DataType: 
  OrigDataType: 
T_1427: (in Mem0[ds:si + 0x0000:word16] : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1428: (in ax - di : word16)
  Class: Eq_1428
  DataType: 
  OrigDataType: 
T_1429: (in cond(ax - di) : byte)
  Class: Eq_1372
  DataType: 
  OrigDataType: 
T_1430: (in Test(ULT,C) : bool)
  Class: Eq_1430
  DataType: 
  OrigDataType: 
T_1431: (in 0x0008 : word16)
  Class: Eq_1431
  DataType: 
  OrigDataType: 
T_1432: (in fp - 0x0008 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1433: (in 0x000A : word16)
  Class: Eq_1433
  DataType: 
  OrigDataType: 
T_1434: (in fp - 0x000A : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1435: (in wLoc0A : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1436: (in fn0800_04BF : ptr32)
  Class: Eq_1436
  DataType: 
  OrigDataType: 
T_1437: (in signature of fn0800_04BF : void)
  Class: Eq_1436
  DataType: 
  OrigDataType: 
T_1438: (in 0x0008 : word16)
  Class: Eq_1438
  DataType: 
  OrigDataType: 
T_1439: (in fp - 0x0008 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1440: (in 0x0006 : word16)
  Class: Eq_1440
  DataType: 
  OrigDataType: 
T_1441: (in fp - 0x0006 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1442: (in 0x0006 : word16)
  Class: Eq_1442
  DataType: 
  OrigDataType: 
T_1443: (in si + 0x0006 : word16)
  Class: Eq_1443
  DataType: 
  OrigDataType: 
T_1444: (in Mem0[ds:si + 0x0006:word16] : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1445: (in 0x062A : word16)
  Class: Eq_1445
  DataType: 
  OrigDataType: 
T_1446: (in Mem0[ds:0x062A:word16] : word16)
  Class: Eq_1446
  DataType: 
  OrigDataType: 
T_1447: (in si - Mem0[ds:0x062A:word16] : word16)
  Class: Eq_1447
  DataType: 
  OrigDataType: 
T_1448: (in cond(si - Mem0[ds:0x062A:word16]) : byte)
  Class: Eq_1372
  DataType: 
  OrigDataType: 
T_1449: (in Test(NE,Z) : bool)
  Class: Eq_1449
  DataType: 
  OrigDataType: 
T_1450: (in 0x0008 : word16)
  Class: Eq_1450
  DataType: 
  OrigDataType: 
T_1451: (in fp - 0x0008 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1452: (in fn0800_0491 : ptr32)
  Class: Eq_1452
  DataType: 
  OrigDataType: 
T_1453: (in signature of fn0800_0491 : void)
  Class: Eq_1452
  DataType: 
  OrigDataType: 
T_1454: (in 0x0006 : word16)
  Class: Eq_1454
  DataType: 
  OrigDataType: 
T_1455: (in fp - 0x0006 : word16)
  Class: Eq_1342
  DataType: 
  OrigDataType: 
T_1456: (in 0x0000 : word16)
  Class: Eq_1456
  DataType: 
  OrigDataType: 
T_1457: (in si + 0x0000 : word16)
  Class: Eq_1457
  DataType: 
  OrigDataType: 
T_1458: (in Mem0[ds:si + 0x0000:word16] : word16)
  Class: Eq_1458
  DataType: 
  OrigDataType: 
T_1459: (in 0x0001 : word16)
  Class: Eq_1459
  DataType: 
  OrigDataType: 
T_1460: (in Mem0[ds:si + 0x0000:word16] + 0x0001 : word16)
  Class: Eq_1460
  DataType: 
  OrigDataType: 
T_1461: (in v16 : word16)
  Class: Eq_1460
  DataType: 
  OrigDataType: 
T_1462: (in 0x0000 : word16)
  Class: Eq_1462
  DataType: 
  OrigDataType: 
T_1463: (in si + 0x0000 : word16)
  Class: Eq_1463
  DataType: 
  OrigDataType: 
T_1464: (in Mem0[ds:si + 0x0000:word16] : word16)
  Class: Eq_1460
  DataType: 
  OrigDataType: 
T_1465: (in 0x0004 : word16)
  Class: Eq_1465
  DataType: 
  OrigDataType: 
T_1466: (in ax + 0x0004 : word16)
  Class: Eq_1351
  DataType: 
  OrigDataType: 
T_1467: (in cond(ax) : byte)
  Class: Eq_1372
  DataType: 
  OrigDataType: 
T_1468: (in fp : ptr16)
  Class: Eq_1468
  DataType: 
  OrigDataType: 
T_1469: (in sp : word16)
  Class: Eq_1468
  DataType: 
  OrigDataType: 
T_1470: (in 0x0002 : word16)
  Class: Eq_1470
  DataType: 
  OrigDataType: 
T_1471: (in fp - 0x0002 : word16)
  Class: Eq_1468
  DataType: 
  OrigDataType: 
T_1472: (in bp : word16)
  Class: Eq_1472
  DataType: 
  OrigDataType: 
T_1473: (in wLoc02 : word16)
  Class: Eq_1472
  DataType: 
  OrigDataType: 
T_1474: (in fp - 0x0002 : word16)
  Class: Eq_1472
  DataType: 
  OrigDataType: 
T_1475: (in wArg02 : word16)
  Class: Eq_1475
  DataType: 
  OrigDataType: 
T_1476: (in ax : word16)
  Class: Eq_1475
  DataType: 
  OrigDataType: 
T_1477: (in wArg04 : word16)
  Class: Eq_1477
  DataType: 
  OrigDataType: 
T_1478: (in dx : word16)
  Class: Eq_1477
  DataType: 
  OrigDataType: 
T_1479: (in ds : selector)
  Class: Eq_1479
  DataType: 
  OrigDataType: 
T_1480: (in 0x009E : word16)
  Class: Eq_1480
  DataType: 
  OrigDataType: 
T_1481: (in Mem0[ds:0x009E:word16] : word16)
  Class: Eq_1481
  DataType: 
  OrigDataType: 
T_1482: (in ax + Mem0[ds:0x009E:word16] : word16)
  Class: Eq_1475
  DataType: 
  OrigDataType: 
T_1483: (in cond(ax) : byte)
  Class: Eq_1483
  DataType: 
  OrigDataType: 
T_1484: (in SCZO : byte)
  Class: Eq_1483
  DataType: 
  OrigDataType: 
T_1485: (in 0x00 : byte)
  Class: Eq_1485
  DataType: 
  OrigDataType: 
T_1486: (in dx + 0x00 : word16)
  Class: Eq_1486
  DataType: 
  OrigDataType: 
T_1487: (in C : byte)
  Class: Eq_1487
  DataType: 
  OrigDataType: 
T_1488: (in dx + 0x00 + C : word16)
  Class: Eq_1477
  DataType: 
  OrigDataType: 
T_1489: (in cx : word16)
  Class: Eq_1475
  DataType: 
  OrigDataType: 
T_1490: (in 0x0100 : word16)
  Class: Eq_1490
  DataType: 
  OrigDataType: 
T_1491: (in cx + 0x0100 : word16)
  Class: Eq_1475
  DataType: 
  OrigDataType: 
T_1492: (in cond(cx) : byte)
  Class: Eq_1483
  DataType: 
  OrigDataType: 
T_1493: (in 0x00 : byte)
  Class: Eq_1493
  DataType: 
  OrigDataType: 
T_1494: (in dx + 0x00 : word16)
  Class: Eq_1494
  DataType: 
  OrigDataType: 
T_1495: (in dx + 0x00 + C : word16)
  Class: Eq_1477
  DataType: 
  OrigDataType: 
T_1496: (in dx | dx : word16)
  Class: Eq_1477
  DataType: 
  OrigDataType: 
T_1497: (in cond(dx) : byte)
  Class: Eq_1497
  DataType: 
  OrigDataType: 
T_1498: (in SZO : byte)
  Class: Eq_1497
  DataType: 
  OrigDataType: 
T_1499: (in false : bool)
  Class: Eq_1487
  DataType: 
  OrigDataType: 
T_1500: (in Z : byte)
  Class: Eq_1500
  DataType: 
  OrigDataType: 
T_1501: (in Test(NE,Z) : bool)
  Class: Eq_1501
  DataType: 
  OrigDataType: 
T_1502: (in 0x0008 : word16)
  Class: Eq_1502
  DataType: 
  OrigDataType: 
T_1503: (in 0x0094 : word16)
  Class: Eq_1503
  DataType: 
  OrigDataType: 
T_1504: (in Mem0[ds:0x0094:word16] : word16)
  Class: Eq_1502
  DataType: 
  OrigDataType: 
T_1505: (in 0xFFFF : word16)
  Class: Eq_1475
  DataType: 
  OrigDataType: 
T_1506: (in fp - 0x0002 : word16)
  Class: Eq_1506
  DataType: 
  OrigDataType: 
T_1507: (in cx - (fp - 0x0002) : word16)
  Class: Eq_1507
  DataType: 
  OrigDataType: 
T_1508: (in cond(cx - (fp - 0x0002)) : byte)
  Class: Eq_1483
  DataType: 
  OrigDataType: 
T_1509: (in Test(UGE,C) : bool)
  Class: Eq_1509
  DataType: 
  OrigDataType: 
T_1510: (in 0x009E : word16)
  Class: Eq_1510
  DataType: 
  OrigDataType: 
T_1511: (in Mem0[ds:0x009E:word16] : word16)
  Class: Eq_1475
  DataType: 
  OrigDataType: 
T_1512: (in v13 : word16)
  Class: Eq_1475
  DataType: 
  OrigDataType: 
T_1513: (in Mem0[ds:0x009E:word16] : word16)
  Class: Eq_1475
  DataType: 
  OrigDataType: 
T_1514: (in fp : ptr16)
  Class: Eq_1514
  DataType: 
  OrigDataType: 
T_1515: (in sp : word16)
  Class: Eq_1514
  DataType: 
  OrigDataType: 
T_1516: (in 0x0002 : word16)
  Class: Eq_1516
  DataType: 
  OrigDataType: 
T_1517: (in fp - 0x0002 : word16)
  Class: Eq_1514
  DataType: 
  OrigDataType: 
T_1518: (in bp : word16)
  Class: Eq_1518
  DataType: 
  OrigDataType: 
T_1519: (in wLoc02 : word16)
  Class: Eq_1518
  DataType: 
  OrigDataType: 
T_1520: (in fp - 0x0002 : word16)
  Class: Eq_1518
  DataType: 
  OrigDataType: 
T_1521: (in 0x0F81 : word16)
  Class: Eq_1521
  DataType: 
  OrigDataType: 
T_1522: (in ax : word16)
  Class: Eq_1521
  DataType: 
  OrigDataType: 
T_1523: (in 0x0004 : word16)
  Class: Eq_1523
  DataType: 
  OrigDataType: 
T_1524: (in fp - 0x0004 : word16)
  Class: Eq_1514
  DataType: 
  OrigDataType: 
T_1525: (in 0x0F81 : word16)
  Class: Eq_1525
  DataType: 
  OrigDataType: 
T_1526: (in wLoc04 : word16)
  Class: Eq_1525
  DataType: 
  OrigDataType: 
T_1527: (in 0x0352 : word16)
  Class: Eq_1521
  DataType: 
  OrigDataType: 
T_1528: (in 0x0006 : word16)
  Class: Eq_1528
  DataType: 
  OrigDataType: 
T_1529: (in fp - 0x0006 : word16)
  Class: Eq_1514
  DataType: 
  OrigDataType: 
T_1530: (in 0x0352 : word16)
  Class: Eq_1530
  DataType: 
  OrigDataType: 
T_1531: (in wLoc06 : word16)
  Class: Eq_1530
  DataType: 
  OrigDataType: 
T_1532: (in 0x0008 : word16)
  Class: Eq_1532
  DataType: 
  OrigDataType: 
T_1533: (in fp - 0x0008 : word16)
  Class: Eq_1514
  DataType: 
  OrigDataType: 
T_1534: (in wArg02 : word16)
  Class: Eq_1534
  DataType: 
  OrigDataType: 
T_1535: (in wLoc08 : word16)
  Class: Eq_1534
  DataType: 
  OrigDataType: 
T_1536: (in 0x0004 : word16)
  Class: Eq_1536
  DataType: 
  OrigDataType: 
T_1537: (in fp + 0x0004 : word16)
  Class: Eq_1521
  DataType: 
  OrigDataType: 
T_1538: (in 0x000A : word16)
  Class: Eq_1538
  DataType: 
  OrigDataType: 
T_1539: (in fp - 0x000A : word16)
  Class: Eq_1514
  DataType: 
  OrigDataType: 
T_1540: (in fp + 0x0004 : word16)
  Class: Eq_1540
  DataType: 
  OrigDataType: 
T_1541: (in wLoc0A : word16)
  Class: Eq_1540
  DataType: 
  OrigDataType: 
T_1542: (in fn0800_1073 : ptr32)
  Class: Eq_1542
  DataType: 
  OrigDataType: 
T_1543: (in signature of fn0800_1073 : void)
  Class: Eq_1542
  DataType: 
  OrigDataType: 
T_1544: (in 0x0002 : word16)
  Class: Eq_1544
  DataType: 
  OrigDataType: 
T_1545: (in fp - 0x0002 : word16)
  Class: Eq_1514
  DataType: 
  OrigDataType: 
T_1546: (in fp : ptr16)
  Class: Eq_1546
  DataType: 
  OrigDataType: 
T_1547: (in sp : word16)
  Class: Eq_1546
  DataType: 
  OrigDataType: 
T_1548: (in 0x0002 : word16)
  Class: Eq_1548
  DataType: 
  OrigDataType: 
T_1549: (in fp - 0x0002 : word16)
  Class: Eq_1546
  DataType: 
  OrigDataType: 
T_1550: (in bp : word16)
  Class: Eq_1550
  DataType: 
  OrigDataType: 
T_1551: (in wLoc02 : word16)
  Class: Eq_1550
  DataType: 
  OrigDataType: 
T_1552: (in fp - 0x0002 : word16)
  Class: Eq_1550
  DataType: 
  OrigDataType: 
T_1553: (in 0x009A : word16)
  Class: Eq_1553
  DataType: 
  OrigDataType: 
T_1554: (in fp - 0x009A : word16)
  Class: Eq_1546
  DataType: 
  OrigDataType: 
T_1555: (in fp - 0x009A : word16)
  Class: Eq_1555
  DataType: 
  OrigDataType: 
T_1556: (in cond(fp - 0x009A) : byte)
  Class: Eq_1556
  DataType: 
  OrigDataType: 
T_1557: (in SCZO : byte)
  Class: Eq_1556
  DataType: 
  OrigDataType: 
T_1558: (in 0x009C : word16)
  Class: Eq_1558
  DataType: 
  OrigDataType: 
T_1559: (in fp - 0x009C : word16)
  Class: Eq_1546
  DataType: 
  OrigDataType: 
T_1560: (in si : word16)
  Class: Eq_1560
  DataType: 
  OrigDataType: 
T_1561: (in wLoc9C : word16)
  Class: Eq_1560
  DataType: 
  OrigDataType: 
T_1562: (in 0x009E : word16)
  Class: Eq_1562
  DataType: 
  OrigDataType: 
T_1563: (in fp - 0x009E : word16)
  Class: Eq_1546
  DataType: 
  OrigDataType: 
T_1564: (in di : word16)
  Class: Eq_1564
  DataType: 
  OrigDataType: 
T_1565: (in wLoc9E : word16)
  Class: Eq_1564
  DataType: 
  OrigDataType: 
T_1566: (in 0x0000 : word16)
  Class: Eq_1566
  DataType: 
  OrigDataType: 
T_1567: (in wLoc5A : word16)
  Class: Eq_1566
  DataType: 
  OrigDataType: 
T_1568: (in 0x50 : byte)
  Class: Eq_1568
  DataType: 
  OrigDataType: 
T_1569: (in bLoc57 : byte)
  Class: Eq_1568
  DataType: 
  OrigDataType: 
T_1570: (in 0x0000 : word16)
  Class: Eq_1570
  DataType: 
  OrigDataType: 
T_1571: (in wLoc04 : word16)
  Class: Eq_1570
  DataType: 
  OrigDataType: 
T_1572: (in 0x00A0 : word16)
  Class: Eq_1572
  DataType: 
  OrigDataType: 
T_1573: (in fp - 0x00A0 : word16)
  Class: Eq_1546
  DataType: 
  OrigDataType: 
T_1574: (in es : selector)
  Class: Eq_1574
  DataType: 
  OrigDataType: 
T_1575: (in wLocA0 : word16)
  Class: Eq_1574
  DataType: 
  OrigDataType: 
T_1576: (in false : bool)
  Class: Eq_1576
  DataType: 
  OrigDataType: 
T_1577: (in D : byte)
  Class: Eq_1576
  DataType: 
  OrigDataType: 
T_1578: (in 0x0056 : word16)
  Class: Eq_1578
  DataType: 
  OrigDataType: 
T_1579: (in fp - 0x0056 : word16)
  Class: Eq_1564
  DataType: 
  OrigDataType: 
T_1580: (in fp - 0x0056 : word16)
  Class: Eq_1564
  DataType: 
  OrigDataType: 
T_1581: (in wLoc98 : word16)
  Class: Eq_1564
  DataType: 
  OrigDataType: 
T_1582: (in wArg04 : word16)
  Class: Eq_1560
  DataType: 
  OrigDataType: 
T_1583: (in al : byte)
  Class: Eq_1583
  DataType: 
  OrigDataType: 
T_1584: (in 0x25 : byte)
  Class: Eq_1584
  DataType: 
  OrigDataType: 
T_1585: (in al - 0x25 : byte)
  Class: Eq_1585
  DataType: 
  OrigDataType: 
T_1586: (in cond(al - 0x25) : byte)
  Class: Eq_1556
  DataType: 
  OrigDataType: 
T_1587: (in Z : byte)
  Class: Eq_1587
  DataType: 
  OrigDataType: 
T_1588: (in Test(EQ,Z) : bool)
  Class: Eq_1588
  DataType: 
  OrigDataType: 
T_1589: (in wLoc8C : word16)
  Class: Eq_1560
  DataType: 
  OrigDataType: 
T_1590: (in ds : selector)
  Class: Eq_1590
  DataType: 
  OrigDataType: 
T_1591: (in 0x0000 : word16)
  Class: Eq_1591
  DataType: 
  OrigDataType: 
T_1592: (in si + 0x0000 : word16)
  Class: Eq_1592
  DataType: 
  OrigDataType: 
T_1593: (in Mem0[ds:si + 0x0000:byte] : byte)
  Class: Eq_1583
  DataType: 
  OrigDataType: 
T_1594: (in 0x0001 : word16)
  Class: Eq_1594
  DataType: 
  OrigDataType: 
T_1595: (in si + 0x0001 : word16)
  Class: Eq_1560
  DataType: 
  OrigDataType: 
T_1596: (in 0x25 : byte)
  Class: Eq_1596
  DataType: 
  OrigDataType: 
T_1597: (in al - 0x25 : byte)
  Class: Eq_1597
  DataType: 
  OrigDataType: 
T_1598: (in cond(al - 0x25) : byte)
  Class: Eq_1556
  DataType: 
  OrigDataType: 
T_1599: (in Test(EQ,Z) : bool)
  Class: Eq_1599
  DataType: 
  OrigDataType: 
T_1600: (in 0x0000 : word16)
  Class: Eq_1600
  DataType: 
  OrigDataType: 
T_1601: (in di + 0x0000 : word16)
  Class: Eq_1601
  DataType: 
  OrigDataType: 
T_1602: (in Mem0[ds:di + 0x0000:byte] : byte)
  Class: Eq_1583
  DataType: 
  OrigDataType: 
T_1603: (in 0x0001 : word16)
  Class: Eq_1603
  DataType: 
  OrigDataType: 
T_1604: (in di + 0x0001 : word16)
  Class: Eq_1564
  DataType: 
  OrigDataType: 
T_1605: (in 0x01 : byte)
  Class: Eq_1605
  DataType: 
  OrigDataType: 
T_1606: (in bLoc57 - 0x01 : byte)
  Class: Eq_1568
  DataType: 
  OrigDataType: 
T_1607: (in v15 : byte)
  Class: Eq_1568
  DataType: 
  OrigDataType: 
T_1608: (in cond(v15) : byte)
  Class: Eq_1608
  DataType: 
  OrigDataType: 
T_1609: (in SZO : byte)
  Class: Eq_1608
  DataType: 
  OrigDataType: 
T_1610: (in Test(GT,SZO) : bool)
  Class: Eq_1610
  DataType: 
  OrigDataType: 
T_1611: (in 0x0000 : word16)
  Class: Eq_1611
  DataType: 
  OrigDataType: 
T_1612: (in si + 0x0000 : word16)
  Class: Eq_1612
  DataType: 
  OrigDataType: 
T_1613: (in Mem0[ds:si + 0x0000:byte] : byte)
  Class: Eq_1583
  DataType: 
  OrigDataType: 
T_1614: (in 0x0001 : word16)
  Class: Eq_1614
  DataType: 
  OrigDataType: 
T_1615: (in si + 0x0001 : word16)
  Class: Eq_1560
  DataType: 
  OrigDataType: 
T_1616: (in al | al : byte)
  Class: Eq_1583
  DataType: 
  OrigDataType: 
T_1617: (in cond(al) : byte)
  Class: Eq_1608
  DataType: 
  OrigDataType: 
T_1618: (in false : bool)
  Class: Eq_1618
  DataType: 
  OrigDataType: 
T_1619: (in C : byte)
  Class: Eq_1618
  DataType: 
  OrigDataType: 
T_1620: (in Test(EQ,Z) : bool)
  Class: Eq_1620
  DataType: 
  OrigDataType: 
T_1621: (in fn0800_10A1 : ptr32)
  Class: Eq_1621
  DataType: 
  OrigDataType: 
T_1622: (in signature of fn0800_10A1 : void)
  Class: Eq_1621
  DataType: 
  OrigDataType: 
T_1623: (in 0x50 : byte)
  Class: Eq_1623
  DataType: 
  OrigDataType: 
T_1624: (in bLoc57 - 0x50 : byte)
  Class: Eq_1624
  DataType: 
  OrigDataType: 
T_1625: (in cond(bLoc57 - 0x50) : byte)
  Class: Eq_1556
  DataType: 
  OrigDataType: 
T_1626: (in SO : byte)
  Class: Eq_1626
  DataType: 
  OrigDataType: 
T_1627: (in Test(GE,SO) : bool)
  Class: Eq_1627
  DataType: 
  OrigDataType: 
T_1628: (in 0x50 : byte)
  Class: Eq_1628
  DataType: 
  OrigDataType: 
T_1629: (in bLoc57 - 0x50 : byte)
  Class: Eq_1629
  DataType: 
  OrigDataType: 
T_1630: (in cond(bLoc57 - 0x50) : byte)
  Class: Eq_1556
  DataType: 
  OrigDataType: 
T_1631: (in Test(GE,SO) : bool)
  Class: Eq_1631
  DataType: 
  OrigDataType: 
T_1632: (in ss : selector)
  Class: Eq_1632
  DataType: 
  OrigDataType: 
T_1633: (in 0x0000 : word16)
  Class: Eq_1633
  DataType: 
  OrigDataType: 
T_1634: (in sp + 0x0000 : word16)
  Class: Eq_1634
  DataType: 
  OrigDataType: 
T_1635: (in Mem0[ss:sp + 0x0000:selector] : selector)
  Class: Eq_1574
  DataType: 
  OrigDataType: 
T_1636: (in 0x0002 : word16)
  Class: Eq_1636
  DataType: 
  OrigDataType: 
T_1637: (in sp + 0x0002 : word16)
  Class: Eq_1546
  DataType: 
  OrigDataType: 
T_1638: (in cond(0x0000) : byte)
  Class: Eq_1556
  DataType: 
  OrigDataType: 
T_1639: (in Test(EQ,Z) : bool)
  Class: Eq_1639
  DataType: 
  OrigDataType: 
T_1640: (in fn0800_10A1 : ptr32)
  Class: Eq_1621
  DataType: 
  OrigDataType: 
T_1641: (in fn0800_10A1 : ptr32)
  Class: Eq_1621
  DataType: 
  OrigDataType: 
T_1642: (in 0x0000 : word16)
  Class: Eq_1642
  DataType: 
  OrigDataType: 
T_1643: (in cx : word16)
  Class: Eq_1642
  DataType: 
  OrigDataType: 
T_1644: (in 0x0000 : word16)
  Class: Eq_1644
  DataType: 
  OrigDataType: 
T_1645: (in cond(0x0000) : byte)
  Class: Eq_1608
  DataType: 
  OrigDataType: 
T_1646: (in false : bool)
  Class: Eq_1618
  DataType: 
  OrigDataType: 
T_1647: (in 0x0000 : word16)
  Class: Eq_1647
  DataType: 
  OrigDataType: 
T_1648: (in wLoc8E : word16)
  Class: Eq_1647
  DataType: 
  OrigDataType: 
T_1649: (in 0x0000 : word16)
  Class: Eq_1649
  DataType: 
  OrigDataType: 
T_1650: (in wLoc9A : word16)
  Class: Eq_1649
  DataType: 
  OrigDataType: 
T_1651: (in cl : byte)
  Class: Eq_1651
  DataType: 
  OrigDataType: 
T_1652: (in bLoc8F : byte)
  Class: Eq_1651
  DataType: 
  OrigDataType: 
T_1653: (in 0xFFFF : word16)
  Class: Eq_1653
  DataType: 
  OrigDataType: 
T_1654: (in wLoc94 : word16)
  Class: Eq_1653
  DataType: 
  OrigDataType: 
T_1655: (in 0xFFFF : word16)
  Class: Eq_1655
  DataType: 
  OrigDataType: 
T_1656: (in wLoc92 : word16)
  Class: Eq_1655
  DataType: 
  OrigDataType: 
T_1657: (in 0x00 : byte)
  Class: Eq_1657
  DataType: 
  OrigDataType: 
T_1658: (in ah : byte)
  Class: Eq_1657
  DataType: 
  OrigDataType: 
T_1659: (in ax : word16)
  Class: Eq_1566
  DataType: 
  OrigDataType: 
T_1660: (in dx : word16)
  Class: Eq_1566
  DataType: 
  OrigDataType: 
T_1661: (in bx : word16)
  Class: Eq_1566
  DataType: 
  OrigDataType: 
T_1662: (in bl : byte)
  Class: Eq_1662
  DataType: 
  OrigDataType: 
T_1663: (in 0x20 : byte)
  Class: Eq_1663
  DataType: 
  OrigDataType: 
T_1664: (in bl - 0x20 : byte)
  Class: Eq_1662
  DataType: 
  OrigDataType: 
T_1665: (in 0x60 : byte)
  Class: Eq_1665
  DataType: 
  OrigDataType: 
T_1666: (in bl - 0x60 : byte)
  Class: Eq_1666
  DataType: 
  OrigDataType: 
T_1667: (in cond(bl - 0x60) : byte)
  Class: Eq_1556
  DataType: 
  OrigDataType: 
T_1668: (in Test(UGE,C) : bool)
  Class: Eq_1668
  DataType: 
  OrigDataType: 
T_1669: (in 0xFFFF : word16)
  Class: Eq_1566
  DataType: 
  OrigDataType: 
T_1670: (in 0x0000 : word16)
  Class: Eq_1670
  DataType: 
  OrigDataType: 
T_1671: (in sp + 0x0000 : word16)
  Class: Eq_1671
  DataType: 
  OrigDataType: 
T_1672: (in Mem0[ss:sp + 0x0000:word16] : word16)
  Class: Eq_1564
  DataType: 
  OrigDataType: 
T_1673: (in 0x0002 : word16)
  Class: Eq_1673
  DataType: 
  OrigDataType: 
T_1674: (in sp + 0x0002 : word16)
  Class: Eq_1546
  DataType: 
  OrigDataType: 
T_1675: (in 0x0000 : word16)
  Class: Eq_1675
  DataType: 
  OrigDataType: 
T_1676: (in sp + 0x0000 : word16)
  Class: Eq_1676
  DataType: 
  OrigDataType: 
T_1677: (in Mem0[ss:sp + 0x0000:word16] : word16)
  Class: Eq_1560
  DataType: 
  OrigDataType: 
T_1678: (in fp - 0x0002 : word16)
  Class: Eq_1546
  DataType: 
  OrigDataType: 
T_1679: (in 0x04F9 : word16)
  Class: Eq_1679
  DataType: 
  OrigDataType: 
T_1680: (in bx + 0x04F9 : word16)
  Class: Eq_1680
  DataType: 
  OrigDataType: 
T_1681: (in Mem0[ds:bx + 0x04F9:byte] : byte)
  Class: Eq_1662
  DataType: 
  OrigDataType: 
T_1682: (in 0x0017 : word16)
  Class: Eq_1682
  DataType: 
  OrigDataType: 
T_1683: (in ax - 0x0017 : word16)
  Class: Eq_1683
  DataType: 
  OrigDataType: 
T_1684: (in cond(ax - 0x0017) : byte)
  Class: Eq_1556
  DataType: 
  OrigDataType: 
T_1685: (in CZ : byte)
  Class: Eq_1685
  DataType: 
  OrigDataType: 
T_1686: (in Test(ULE,CZ) : bool)
  Class: Eq_1686
  DataType: 
  OrigDataType: 
T_1687: (in 0x0001 : word16)
  Class: Eq_1687
  DataType: 
  OrigDataType: 
T_1688: (in bx << 0x0001 : word16)
  Class: Eq_1566
  DataType: 
  OrigDataType: 
T_1689: (in cond(bx) : byte)
  Class: Eq_1556
  DataType: 
  OrigDataType: 
T_1690: (in 0x25 : byte)
  Class: Eq_1583
  DataType: 
  OrigDataType: 
T_1691: (in fn0800_1099 : ptr32)
  Class: Eq_1691
  DataType: 
  OrigDataType: 
T_1692: (in signature of fn0800_1099 : void)
  Class: Eq_1691
  DataType: 
  OrigDataType: 
T_1693: (in 0xFFFE : word16)
  Class: Eq_1693
  DataType: 
  OrigDataType: 
T_1694: (in sp + 0xFFFE : word16)
  Class: Eq_1546
  DataType: 
  OrigDataType: 
T_1695: (in 0x0000 : word16)
  Class: Eq_1695
  DataType: 
  OrigDataType: 
T_1696: (in si + 0x0000 : word16)
  Class: Eq_1696
  DataType: 
  OrigDataType: 
T_1697: (in Mem0[ds:si + 0x0000:byte] : byte)
  Class: Eq_1583
  DataType: 
  OrigDataType: 
T_1698: (in 0x0001 : word16)
  Class: Eq_1698
  DataType: 
  OrigDataType: 
T_1699: (in si + 0x0001 : word16)
  Class: Eq_1560
  DataType: 
  OrigDataType: 
T_1700: (in al | al : byte)
  Class: Eq_1583
  DataType: 
  OrigDataType: 
T_1701: (in cond(al) : byte)
  Class: Eq_1608
  DataType: 
  OrigDataType: 
T_1702: (in false : bool)
  Class: Eq_1618
  DataType: 
  OrigDataType: 
T_1703: (in Test(NE,Z) : bool)
  Class: Eq_1703
  DataType: 
  OrigDataType: 
T_1704: (in fp : ptr16)
  Class: Eq_1704
  DataType: 
  OrigDataType: 
T_1705: (in sp : word16)
  Class: Eq_1704
  DataType: 
  OrigDataType: 
T_1706: (in al : byte)
  Class: Eq_1706
  DataType: 
  OrigDataType: 
T_1707: (in ds : selector)
  Class: Eq_1707
  DataType: 
  OrigDataType: 
T_1708: (in di : word16)
  Class: Eq_1708
  DataType: 
  OrigDataType: 
T_1709: (in 0x0000 : word16)
  Class: Eq_1709
  DataType: 
  OrigDataType: 
T_1710: (in di + 0x0000 : word16)
  Class: Eq_1710
  DataType: 
  OrigDataType: 
T_1711: (in Mem0[ds:di + 0x0000:byte] : byte)
  Class: Eq_1706
  DataType: 
  OrigDataType: 
T_1712: (in 0x0001 : word16)
  Class: Eq_1712
  DataType: 
  OrigDataType: 
T_1713: (in di + 0x0001 : word16)
  Class: Eq_1708
  DataType: 
  OrigDataType: 
T_1714: (in ss : selector)
  Class: Eq_1714
  DataType: 
  OrigDataType: 
T_1715: (in bp : word16)
  Class: Eq_1715
  DataType: 
  OrigDataType: 
T_1716: (in 0x0055 : word16)
  Class: Eq_1716
  DataType: 
  OrigDataType: 
T_1717: (in bp - 0x0055 : word16)
  Class: Eq_1717
  DataType: 
  OrigDataType: 
T_1718: (in Mem0[ss:bp - 0x0055:byte] : byte)
  Class: Eq_1718
  DataType: 
  OrigDataType: 
T_1719: (in 0x01 : byte)
  Class: Eq_1719
  DataType: 
  OrigDataType: 
T_1720: (in Mem0[ss:bp - 0x0055:byte] - 0x01 : byte)
  Class: Eq_1720
  DataType: 
  OrigDataType: 
T_1721: (in v9 : byte)
  Class: Eq_1720
  DataType: 
  OrigDataType: 
T_1722: (in 0x0055 : word16)
  Class: Eq_1722
  DataType: 
  OrigDataType: 
T_1723: (in bp - 0x0055 : word16)
  Class: Eq_1723
  DataType: 
  OrigDataType: 
T_1724: (in Mem0[ss:bp - 0x0055:byte] : byte)
  Class: Eq_1720
  DataType: 
  OrigDataType: 
T_1725: (in cond(v9) : byte)
  Class: Eq_1725
  DataType: 
  OrigDataType: 
T_1726: (in SZO : byte)
  Class: Eq_1725
  DataType: 
  OrigDataType: 
T_1727: (in Test(LE,SZO) : bool)
  Class: Eq_1727
  DataType: 
  OrigDataType: 
T_1728: (in fn0800_10A1 : ptr32)
  Class: Eq_1621
  DataType: 
  OrigDataType: 
T_1729: (in fn0800_10A1 : ptr32)
  Class: Eq_1621
  DataType: 
  OrigDataType: 
T_1730: (in fp : ptr16)
  Class: Eq_1730
  DataType: 
  OrigDataType: 
T_1731: (in sp : word16)
  Class: Eq_1730
  DataType: 
  OrigDataType: 
T_1732: (in 0x0002 : word16)
  Class: Eq_1732
  DataType: 
  OrigDataType: 
T_1733: (in fp - 0x0002 : word16)
  Class: Eq_1730
  DataType: 
  OrigDataType: 
T_1734: (in bx : word16)
  Class: Eq_1734
  DataType: 
  OrigDataType: 
T_1735: (in wLoc02 : word16)
  Class: Eq_1734
  DataType: 
  OrigDataType: 
T_1736: (in 0x0004 : word16)
  Class: Eq_1736
  DataType: 
  OrigDataType: 
T_1737: (in fp - 0x0004 : word16)
  Class: Eq_1730
  DataType: 
  OrigDataType: 
T_1738: (in cx : word16)
  Class: Eq_1738
  DataType: 
  OrigDataType: 
T_1739: (in wLoc04 : word16)
  Class: Eq_1738
  DataType: 
  OrigDataType: 
T_1740: (in 0x0006 : word16)
  Class: Eq_1740
  DataType: 
  OrigDataType: 
T_1741: (in fp - 0x0006 : word16)
  Class: Eq_1730
  DataType: 
  OrigDataType: 
T_1742: (in dx : word16)
  Class: Eq_1742
  DataType: 
  OrigDataType: 
T_1743: (in wLoc06 : word16)
  Class: Eq_1742
  DataType: 
  OrigDataType: 
T_1744: (in 0x0008 : word16)
  Class: Eq_1744
  DataType: 
  OrigDataType: 
T_1745: (in fp - 0x0008 : word16)
  Class: Eq_1730
  DataType: 
  OrigDataType: 
T_1746: (in es : selector)
  Class: Eq_1734
  DataType: 
  OrigDataType: 
T_1747: (in wLoc08 : word16)
  Class: Eq_1734
  DataType: 
  OrigDataType: 
T_1748: (in bp : word16)
  Class: Eq_1748
  DataType: 
  OrigDataType: 
T_1749: (in 0x0054 : word16)
  Class: Eq_1749
  DataType: 
  OrigDataType: 
T_1750: (in bp - 0x0054 : word16)
  Class: Eq_1738
  DataType: 
  OrigDataType: 
T_1751: (in ax : word16)
  Class: Eq_1738
  DataType: 
  OrigDataType: 
T_1752: (in di : word16)
  Class: Eq_1742
  DataType: 
  OrigDataType: 
T_1753: (in di - ax : word16)
  Class: Eq_1742
  DataType: 
  OrigDataType: 
T_1754: (in cond(di) : byte)
  Class: Eq_1754
  DataType: 
  OrigDataType: 
T_1755: (in SCZO : byte)
  Class: Eq_1754
  DataType: 
  OrigDataType: 
T_1756: (in 0x0054 : word16)
  Class: Eq_1756
  DataType: 
  OrigDataType: 
T_1757: (in bp - 0x0054 : word16)
  Class: Eq_1738
  DataType: 
  OrigDataType: 
T_1758: (in 0x000A : word16)
  Class: Eq_1758
  DataType: 
  OrigDataType: 
T_1759: (in fp - 0x000A : word16)
  Class: Eq_1730
  DataType: 
  OrigDataType: 
T_1760: (in wLoc0A : word16)
  Class: Eq_1738
  DataType: 
  OrigDataType: 
T_1761: (in 0x000C : word16)
  Class: Eq_1761
  DataType: 
  OrigDataType: 
T_1762: (in fp - 0x000C : word16)
  Class: Eq_1730
  DataType: 
  OrigDataType: 
T_1763: (in wLoc0C : word16)
  Class: Eq_1742
  DataType: 
  OrigDataType: 
T_1764: (in 0x000E : word16)
  Class: Eq_1764
  DataType: 
  OrigDataType: 
T_1765: (in fp - 0x000E : word16)
  Class: Eq_1730
  DataType: 
  OrigDataType: 
T_1766: (in ss : selector)
  Class: Eq_1766
  DataType: 
  OrigDataType: 
T_1767: (in 0x0008 : word16)
  Class: Eq_1767
  DataType: 
  OrigDataType: 
T_1768: (in bp + 0x0008 : word16)
  Class: Eq_1768
  DataType: 
  OrigDataType: 
T_1769: (in Mem0[ss:bp + 0x0008:word16] : word16)
  Class: Eq_1734
  DataType: 
  OrigDataType: 
T_1770: (in wLoc0E : word16)
  Class: Eq_1734
  DataType: 
  OrigDataType: 
T_1771: (in cs : selector)
  Class: Eq_1771
  DataType: 
  OrigDataType: 
T_1772: (in 0x000A : word16)
  Class: Eq_1772
  DataType: 
  OrigDataType: 
T_1773: (in bp + 0x000A : word16)
  Class: Eq_1773
  DataType: 
  OrigDataType: 
T_1774: (in Mem0[ss:bp + 0x000A:word16] : word16)
  Class: Eq_1774
  DataType: 
  OrigDataType: 
T_1775: (in SEQ(cs, Mem0[ss:bp + 0x000A:word16]) : ptr32)
  Class: Eq_1775
  DataType: 
  OrigDataType: 
T_1776: (in ax | ax : word16)
  Class: Eq_1738
  DataType: 
  OrigDataType: 
T_1777: (in cond(ax) : byte)
  Class: Eq_1777
  DataType: 
  OrigDataType: 
T_1778: (in SZO : byte)
  Class: Eq_1777
  DataType: 
  OrigDataType: 
T_1779: (in false : bool)
  Class: Eq_1779
  DataType: 
  OrigDataType: 
T_1780: (in C : byte)
  Class: Eq_1779
  DataType: 
  OrigDataType: 
T_1781: (in Z : byte)
  Class: Eq_1781
  DataType: 
  OrigDataType: 
T_1782: (in Test(NE,Z) : bool)
  Class: Eq_1782
  DataType: 
  OrigDataType: 
T_1783: (in 0x50 : byte)
  Class: Eq_1783
  DataType: 
  OrigDataType: 
T_1784: (in 0x0055 : word16)
  Class: Eq_1784
  DataType: 
  OrigDataType: 
T_1785: (in bp - 0x0055 : word16)
  Class: Eq_1785
  DataType: 
  OrigDataType: 
T_1786: (in Mem0[ss:bp - 0x0055:byte] : byte)
  Class: Eq_1783
  DataType: 
  OrigDataType: 
T_1787: (in 0x0058 : word16)
  Class: Eq_1787
  DataType: 
  OrigDataType: 
T_1788: (in bp - 0x0058 : word16)
  Class: Eq_1788
  DataType: 
  OrigDataType: 
T_1789: (in Mem0[ss:bp - 0x0058:word16] : word16)
  Class: Eq_1789
  DataType: 
  OrigDataType: 
T_1790: (in Mem0[ss:bp - 0x0058:word16] + di : word16)
  Class: Eq_1790
  DataType: 
  OrigDataType: 
T_1791: (in v16 : word16)
  Class: Eq_1790
  DataType: 
  OrigDataType: 
T_1792: (in 0x0058 : word16)
  Class: Eq_1792
  DataType: 
  OrigDataType: 
T_1793: (in bp - 0x0058 : word16)
  Class: Eq_1793
  DataType: 
  OrigDataType: 
T_1794: (in Mem0[ss:bp - 0x0058:word16] : word16)
  Class: Eq_1790
  DataType: 
  OrigDataType: 
T_1795: (in cond(v16) : byte)
  Class: Eq_1754
  DataType: 
  OrigDataType: 
T_1796: (in 0x0054 : word16)
  Class: Eq_1796
  DataType: 
  OrigDataType: 
T_1797: (in bp - 0x0054 : word16)
  Class: Eq_1742
  DataType: 
  OrigDataType: 
T_1798: (in 0x000C : word16)
  Class: Eq_1798
  DataType: 
  OrigDataType: 
T_1799: (in fp - 0x000C : word16)
  Class: Eq_1730
  DataType: 
  OrigDataType: 
T_1800: (in 0x000A : word16)
  Class: Eq_1800
  DataType: 
  OrigDataType: 
T_1801: (in fp - 0x000A : word16)
  Class: Eq_1730
  DataType: 
  OrigDataType: 
T_1802: (in 0x0008 : word16)
  Class: Eq_1802
  DataType: 
  OrigDataType: 
T_1803: (in fp - 0x0008 : word16)
  Class: Eq_1730
  DataType: 
  OrigDataType: 
T_1804: (in 0x0006 : word16)
  Class: Eq_1804
  DataType: 
  OrigDataType: 
T_1805: (in fp - 0x0006 : word16)
  Class: Eq_1730
  DataType: 
  OrigDataType: 
T_1806: (in 0x0001 : word16)
  Class: Eq_1806
  DataType: 
  OrigDataType: 
T_1807: (in 0x0002 : word16)
  Class: Eq_1807
  DataType: 
  OrigDataType: 
T_1808: (in bp - 0x0002 : word16)
  Class: Eq_1808
  DataType: 
  OrigDataType: 
T_1809: (in Mem0[ss:bp - 0x0002:word16] : word16)
  Class: Eq_1806
  DataType: 
  OrigDataType: 
T_1810: (in fp : ptr16)
  Class: Eq_1810
  DataType: 
  OrigDataType: 
T_1811: (in sp : word16)
  Class: Eq_1810
  DataType: 
  OrigDataType: 
T_1812: (in 0x0002 : word16)
  Class: Eq_1812
  DataType: 
  OrigDataType: 
T_1813: (in fp - 0x0002 : word16)
  Class: Eq_1810
  DataType: 
  OrigDataType: 
T_1814: (in bp : word16)
  Class: Eq_1814
  DataType: 
  OrigDataType: 
T_1815: (in wLoc02 : word16)
  Class: Eq_1814
  DataType: 
  OrigDataType: 
T_1816: (in fp - 0x0002 : word16)
  Class: Eq_1814
  DataType: 
  OrigDataType: 
T_1817: (in 0x0004 : word16)
  Class: Eq_1817
  DataType: 
  OrigDataType: 
T_1818: (in fp + 0x0004 : word16)
  Class: Eq_1818
  DataType: 
  OrigDataType: 
T_1819: (in ax : word16)
  Class: Eq_1818
  DataType: 
  OrigDataType: 
T_1820: (in 0x0004 : word16)
  Class: Eq_1820
  DataType: 
  OrigDataType: 
T_1821: (in fp - 0x0004 : word16)
  Class: Eq_1810
  DataType: 
  OrigDataType: 
T_1822: (in fp + 0x0004 : word16)
  Class: Eq_1822
  DataType: 
  OrigDataType: 
T_1823: (in wLoc04 : word16)
  Class: Eq_1822
  DataType: 
  OrigDataType: 
T_1824: (in 0x0006 : word16)
  Class: Eq_1824
  DataType: 
  OrigDataType: 
T_1825: (in fp - 0x0006 : word16)
  Class: Eq_1810
  DataType: 
  OrigDataType: 
T_1826: (in wArg02 : word16)
  Class: Eq_1826
  DataType: 
  OrigDataType: 
T_1827: (in wLoc06 : word16)
  Class: Eq_1826
  DataType: 
  OrigDataType: 
T_1828: (in 0x0342 : word16)
  Class: Eq_1818
  DataType: 
  OrigDataType: 
T_1829: (in 0x0008 : word16)
  Class: Eq_1829
  DataType: 
  OrigDataType: 
T_1830: (in fp - 0x0008 : word16)
  Class: Eq_1810
  DataType: 
  OrigDataType: 
T_1831: (in 0x0342 : word16)
  Class: Eq_1831
  DataType: 
  OrigDataType: 
T_1832: (in wLoc08 : word16)
  Class: Eq_1831
  DataType: 
  OrigDataType: 
T_1833: (in 0x1D65 : word16)
  Class: Eq_1818
  DataType: 
  OrigDataType: 
T_1834: (in 0x000A : word16)
  Class: Eq_1834
  DataType: 
  OrigDataType: 
T_1835: (in fp - 0x000A : word16)
  Class: Eq_1810
  DataType: 
  OrigDataType: 
T_1836: (in 0x1D65 : word16)
  Class: Eq_1836
  DataType: 
  OrigDataType: 
T_1837: (in wLoc0A : word16)
  Class: Eq_1836
  DataType: 
  OrigDataType: 
T_1838: (in 0x07F2 : word16)
  Class: Eq_1818
  DataType: 
  OrigDataType: 
T_1839: (in 0x000C : word16)
  Class: Eq_1839
  DataType: 
  OrigDataType: 
T_1840: (in fp - 0x000C : word16)
  Class: Eq_1810
  DataType: 
  OrigDataType: 
T_1841: (in 0x07F2 : word16)
  Class: Eq_1841
  DataType: 
  OrigDataType: 
T_1842: (in wLoc0C : word16)
  Class: Eq_1841
  DataType: 
  OrigDataType: 
T_1843: (in fn0800_16F3 : ptr32)
  Class: Eq_1843
  DataType: 
  OrigDataType: 
T_1844: (in signature of fn0800_16F3 : void)
  Class: Eq_1843
  DataType: 
  OrigDataType: 
T_1845: (in fp - 0x0002 : word16)
  Class: Eq_1810
  DataType: 
  OrigDataType: 
T_1846: (in fp : ptr16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1847: (in sp : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1848: (in 0x0002 : word16)
  Class: Eq_1848
  DataType: 
  OrigDataType: 
T_1849: (in fp - 0x0002 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1850: (in bp : word16)
  Class: Eq_1850
  DataType: 
  OrigDataType: 
T_1851: (in wLoc02 : word16)
  Class: Eq_1850
  DataType: 
  OrigDataType: 
T_1852: (in fp - 0x0002 : word16)
  Class: Eq_1850
  DataType: 
  OrigDataType: 
T_1853: (in 0x002C : word16)
  Class: Eq_1853
  DataType: 
  OrigDataType: 
T_1854: (in fp - 0x002C : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1855: (in fp - 0x002C : word16)
  Class: Eq_1855
  DataType: 
  OrigDataType: 
T_1856: (in cond(fp - 0x002C) : byte)
  Class: Eq_1856
  DataType: 
  OrigDataType: 
T_1857: (in SCZO : byte)
  Class: Eq_1856
  DataType: 
  OrigDataType: 
T_1858: (in 0x002E : word16)
  Class: Eq_1858
  DataType: 
  OrigDataType: 
T_1859: (in fp - 0x002E : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1860: (in si : word16)
  Class: Eq_1860
  DataType: 
  OrigDataType: 
T_1861: (in wLoc2E : word16)
  Class: Eq_1860
  DataType: 
  OrigDataType: 
T_1862: (in 0x0030 : word16)
  Class: Eq_1862
  DataType: 
  OrigDataType: 
T_1863: (in fp - 0x0030 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1864: (in di : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1865: (in wLoc30 : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1866: (in 0x0000 : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1867: (in wLoc2A : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1868: (in 0x0000 : word16)
  Class: Eq_1868
  DataType: 
  OrigDataType: 
T_1869: (in wLoc28 : word16)
  Class: Eq_1868
  DataType: 
  OrigDataType: 
T_1870: (in 0x0032 : word16)
  Class: Eq_1870
  DataType: 
  OrigDataType: 
T_1871: (in fp - 0x0032 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1872: (in es : selector)
  Class: Eq_1872
  DataType: 
  OrigDataType: 
T_1873: (in wLoc32 : word16)
  Class: Eq_1872
  DataType: 
  OrigDataType: 
T_1874: (in false : bool)
  Class: Eq_1874
  DataType: 
  OrigDataType: 
T_1875: (in D : byte)
  Class: Eq_1874
  DataType: 
  OrigDataType: 
T_1876: (in wArg08 : word16)
  Class: Eq_1860
  DataType: 
  OrigDataType: 
T_1877: (in al : byte)
  Class: Eq_1877
  DataType: 
  OrigDataType: 
T_1878: (in 0x25 : byte)
  Class: Eq_1878
  DataType: 
  OrigDataType: 
T_1879: (in al - 0x25 : byte)
  Class: Eq_1879
  DataType: 
  OrigDataType: 
T_1880: (in cond(al - 0x25) : byte)
  Class: Eq_1856
  DataType: 
  OrigDataType: 
T_1881: (in Z : byte)
  Class: Eq_1881
  DataType: 
  OrigDataType: 
T_1882: (in Test(EQ,Z) : bool)
  Class: Eq_1882
  DataType: 
  OrigDataType: 
T_1883: (in 0xFFFF : word16)
  Class: Eq_1883
  DataType: 
  OrigDataType: 
T_1884: (in wLoc24 : word16)
  Class: Eq_1883
  DataType: 
  OrigDataType: 
T_1885: (in 0x00 : byte)
  Class: Eq_1885
  DataType: 
  OrigDataType: 
T_1886: (in bLoc2B : byte)
  Class: Eq_1885
  DataType: 
  OrigDataType: 
T_1887: (in ds : selector)
  Class: Eq_1887
  DataType: 
  OrigDataType: 
T_1888: (in 0x0000 : word16)
  Class: Eq_1888
  DataType: 
  OrigDataType: 
T_1889: (in si + 0x0000 : word16)
  Class: Eq_1889
  DataType: 
  OrigDataType: 
T_1890: (in Mem0[ds:si + 0x0000:byte] : byte)
  Class: Eq_1877
  DataType: 
  OrigDataType: 
T_1891: (in 0x0001 : word16)
  Class: Eq_1891
  DataType: 
  OrigDataType: 
T_1892: (in si + 0x0001 : word16)
  Class: Eq_1860
  DataType: 
  OrigDataType: 
T_1893: (in (int16) al : int16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1894: (in ax : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1895: (in v29 : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1896: (in di | di : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1897: (in cond(di) : byte)
  Class: Eq_1897
  DataType: 
  OrigDataType: 
T_1898: (in SZO : byte)
  Class: Eq_1897
  DataType: 
  OrigDataType: 
T_1899: (in false : bool)
  Class: Eq_1899
  DataType: 
  OrigDataType: 
T_1900: (in C : byte)
  Class: Eq_1899
  DataType: 
  OrigDataType: 
T_1901: (in SO : byte)
  Class: Eq_1901
  DataType: 
  OrigDataType: 
T_1902: (in Test(LT,SO) : bool)
  Class: Eq_1902
  DataType: 
  OrigDataType: 
T_1903: (in (int16) al : int16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1904: (in v16 : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1905: (in 0x0001 : word16)
  Class: Eq_1905
  DataType: 
  OrigDataType: 
T_1906: (in wLoc28 + 0x0001 : word16)
  Class: Eq_1868
  DataType: 
  OrigDataType: 
T_1907: (in v17 : word16)
  Class: Eq_1868
  DataType: 
  OrigDataType: 
T_1908: (in cond(v17) : byte)
  Class: Eq_1897
  DataType: 
  OrigDataType: 
T_1909: (in 0x0034 : word16)
  Class: Eq_1909
  DataType: 
  OrigDataType: 
T_1910: (in fp - 0x0034 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1911: (in wArg06 : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1912: (in wLoc34 : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1913: (in cs : selector)
  Class: Eq_1913
  DataType: 
  OrigDataType: 
T_1914: (in ss : selector)
  Class: Eq_1914
  DataType: 
  OrigDataType: 
T_1915: (in 0x0004 : word16)
  Class: Eq_1915
  DataType: 
  OrigDataType: 
T_1916: (in bp + 0x0004 : word16)
  Class: Eq_1916
  DataType: 
  OrigDataType: 
T_1917: (in Mem0[ss:bp + 0x0004:word16] : word16)
  Class: Eq_1917
  DataType: 
  OrigDataType: 
T_1918: (in SEQ(cs, Mem0[ss:bp + 0x0004:word16]) : ptr32)
  Class: Eq_1918
  DataType: 
  OrigDataType: 
T_1919: (in cx : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1920: (in 0x0032 : word16)
  Class: Eq_1920
  DataType: 
  OrigDataType: 
T_1921: (in fp - 0x0032 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1922: (in ax | ax : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1923: (in cond(ax) : byte)
  Class: Eq_1897
  DataType: 
  OrigDataType: 
T_1924: (in false : bool)
  Class: Eq_1899
  DataType: 
  OrigDataType: 
T_1925: (in Test(LT,SO) : bool)
  Class: Eq_1925
  DataType: 
  OrigDataType: 
T_1926: (in di | di : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1927: (in cond(di) : byte)
  Class: Eq_1897
  DataType: 
  OrigDataType: 
T_1928: (in false : bool)
  Class: Eq_1899
  DataType: 
  OrigDataType: 
T_1929: (in S : byte)
  Class: Eq_1929
  DataType: 
  OrigDataType: 
T_1930: (in Test(SG,S) : bool)
  Class: Eq_1930
  DataType: 
  OrigDataType: 
T_1931: (in ax - di : word16)
  Class: Eq_1931
  DataType: 
  OrigDataType: 
T_1932: (in cond(ax - di) : byte)
  Class: Eq_1856
  DataType: 
  OrigDataType: 
T_1933: (in Test(EQ,Z) : bool)
  Class: Eq_1933
  DataType: 
  OrigDataType: 
T_1934: (in 0x055A : word16)
  Class: Eq_1934
  DataType: 
  OrigDataType: 
T_1935: (in di + 0x055A : word16)
  Class: Eq_1935
  DataType: 
  OrigDataType: 
T_1936: (in Mem0[ds:di + 0x055A:byte] : byte)
  Class: Eq_1936
  DataType: 
  OrigDataType: 
T_1937: (in 0x01 : byte)
  Class: Eq_1937
  DataType: 
  OrigDataType: 
T_1938: (in Mem0[ds:di + 0x055A:byte] - 0x01 : byte)
  Class: Eq_1938
  DataType: 
  OrigDataType: 
T_1939: (in cond(Mem0[ds:di + 0x055A:byte] - 0x01) : byte)
  Class: Eq_1856
  DataType: 
  OrigDataType: 
T_1940: (in Test(NE,Z) : bool)
  Class: Eq_1940
  DataType: 
  OrigDataType: 
T_1941: (in v22 : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1942: (in bx : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1943: (in bl : byte)
  Class: Eq_1943
  DataType: 
  OrigDataType: 
T_1944: (in bl | bl : byte)
  Class: Eq_1943
  DataType: 
  OrigDataType: 
T_1945: (in cond(bl) : byte)
  Class: Eq_1897
  DataType: 
  OrigDataType: 
T_1946: (in false : bool)
  Class: Eq_1899
  DataType: 
  OrigDataType: 
T_1947: (in Test(SG,S) : bool)
  Class: Eq_1947
  DataType: 
  OrigDataType: 
T_1948: (in 0x0034 : word16)
  Class: Eq_1948
  DataType: 
  OrigDataType: 
T_1949: (in fp - 0x0034 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1950: (in 0x0036 : word16)
  Class: Eq_1950
  DataType: 
  OrigDataType: 
T_1951: (in fp - 0x0036 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1952: (in wLoc36 : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1953: (in 0x0006 : word16)
  Class: Eq_1953
  DataType: 
  OrigDataType: 
T_1954: (in bp + 0x0006 : word16)
  Class: Eq_1954
  DataType: 
  OrigDataType: 
T_1955: (in Mem0[ss:bp + 0x0006:word16] : word16)
  Class: Eq_1955
  DataType: 
  OrigDataType: 
T_1956: (in SEQ(cs, Mem0[ss:bp + 0x0006:word16]) : ptr32)
  Class: Eq_1956
  DataType: 
  OrigDataType: 
T_1957: (in 0x0034 : word16)
  Class: Eq_1957
  DataType: 
  OrigDataType: 
T_1958: (in fp - 0x0034 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1959: (in 0x0032 : word16)
  Class: Eq_1959
  DataType: 
  OrigDataType: 
T_1960: (in fp - 0x0032 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1961: (in 0x0001 : word16)
  Class: Eq_1961
  DataType: 
  OrigDataType: 
T_1962: (in wLoc28 - 0x0001 : word16)
  Class: Eq_1868
  DataType: 
  OrigDataType: 
T_1963: (in v27 : word16)
  Class: Eq_1868
  DataType: 
  OrigDataType: 
T_1964: (in cond(v27) : byte)
  Class: Eq_1897
  DataType: 
  OrigDataType: 
T_1965: (in 0x055A : word16)
  Class: Eq_1965
  DataType: 
  OrigDataType: 
T_1966: (in bx + 0x055A : word16)
  Class: Eq_1966
  DataType: 
  OrigDataType: 
T_1967: (in Mem0[ds:bx + 0x055A:byte] : byte)
  Class: Eq_1967
  DataType: 
  OrigDataType: 
T_1968: (in 0x01 : byte)
  Class: Eq_1968
  DataType: 
  OrigDataType: 
T_1969: (in Mem0[ds:bx + 0x055A:byte] - 0x01 : byte)
  Class: Eq_1969
  DataType: 
  OrigDataType: 
T_1970: (in cond(Mem0[ds:bx + 0x055A:byte] - 0x01) : byte)
  Class: Eq_1856
  DataType: 
  OrigDataType: 
T_1971: (in Test(NE,Z) : bool)
  Class: Eq_1971
  DataType: 
  OrigDataType: 
T_1972: (in 0x0001 : word16)
  Class: Eq_1972
  DataType: 
  OrigDataType: 
T_1973: (in wLoc28 + 0x0001 : word16)
  Class: Eq_1868
  DataType: 
  OrigDataType: 
T_1974: (in v25 : word16)
  Class: Eq_1868
  DataType: 
  OrigDataType: 
T_1975: (in cond(v25) : byte)
  Class: Eq_1897
  DataType: 
  OrigDataType: 
T_1976: (in 0x0034 : word16)
  Class: Eq_1976
  DataType: 
  OrigDataType: 
T_1977: (in fp - 0x0034 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1978: (in 0x0004 : word16)
  Class: Eq_1978
  DataType: 
  OrigDataType: 
T_1979: (in bp + 0x0004 : word16)
  Class: Eq_1979
  DataType: 
  OrigDataType: 
T_1980: (in Mem0[ss:bp + 0x0004:word16] : word16)
  Class: Eq_1980
  DataType: 
  OrigDataType: 
T_1981: (in SEQ(cs, Mem0[ss:bp + 0x0004:word16]) : ptr32)
  Class: Eq_1981
  DataType: 
  OrigDataType: 
T_1982: (in 0x0032 : word16)
  Class: Eq_1982
  DataType: 
  OrigDataType: 
T_1983: (in fp - 0x0032 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1984: (in ax | ax : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1985: (in cond(ax) : byte)
  Class: Eq_1897
  DataType: 
  OrigDataType: 
T_1986: (in false : bool)
  Class: Eq_1899
  DataType: 
  OrigDataType: 
T_1987: (in Test(GT,SZO) : bool)
  Class: Eq_1987
  DataType: 
  OrigDataType: 
T_1988: (in 0x0034 : word16)
  Class: Eq_1988
  DataType: 
  OrigDataType: 
T_1989: (in fp - 0x0034 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1990: (in 0xFFFF : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1991: (in 0x0036 : word16)
  Class: Eq_1991
  DataType: 
  OrigDataType: 
T_1992: (in fp - 0x0036 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_1993: (in 0xFFFF : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_1994: (in 0x0006 : word16)
  Class: Eq_1994
  DataType: 
  OrigDataType: 
T_1995: (in bp + 0x0006 : word16)
  Class: Eq_1995
  DataType: 
  OrigDataType: 
T_1996: (in Mem0[ss:bp + 0x0006:word16] : word16)
  Class: Eq_1996
  DataType: 
  OrigDataType: 
T_1997: (in SEQ(cs, Mem0[ss:bp + 0x0006:word16]) : ptr32)
  Class: Eq_1997
  DataType: 
  OrigDataType: 
T_1998: (in 0x0034 : word16)
  Class: Eq_1998
  DataType: 
  OrigDataType: 
T_1999: (in fp - 0x0034 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_2000: (in 0x0032 : word16)
  Class: Eq_2000
  DataType: 
  OrigDataType: 
T_2001: (in fp - 0x0032 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_2002: (in 0xFFFF : word16)
  Class: Eq_2002
  DataType: 
  OrigDataType: 
T_2003: (in cond(0xFFFF) : byte)
  Class: Eq_1856
  DataType: 
  OrigDataType: 
T_2004: (in 0x0000 - C : word16)
  Class: Eq_2004
  DataType: 
  OrigDataType: 
T_2005: (in v26 : word16)
  Class: Eq_2004
  DataType: 
  OrigDataType: 
T_2006: (in 0x0028 : word16)
  Class: Eq_2006
  DataType: 
  OrigDataType: 
T_2007: (in bp - 0x0028 : word16)
  Class: Eq_2007
  DataType: 
  OrigDataType: 
T_2008: (in Mem0[ss:bp - 0x0028:word16] : word16)
  Class: Eq_2004
  DataType: 
  OrigDataType: 
T_2009: (in cond(v26) : byte)
  Class: Eq_1856
  DataType: 
  OrigDataType: 
T_2010: (in 0x0030 : word16)
  Class: Eq_2010
  DataType: 
  OrigDataType: 
T_2011: (in fp - 0x0030 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_2012: (in 0x002E : word16)
  Class: Eq_2012
  DataType: 
  OrigDataType: 
T_2013: (in fp - 0x002E : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_2014: (in fp - 0x0002 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_2015: (in 0x0000 : word16)
  Class: Eq_2015
  DataType: 
  OrigDataType: 
T_2016: (in si + 0x0000 : word16)
  Class: Eq_2016
  DataType: 
  OrigDataType: 
T_2017: (in Mem0[ds:si + 0x0000:byte] : byte)
  Class: Eq_1877
  DataType: 
  OrigDataType: 
T_2018: (in 0x0001 : word16)
  Class: Eq_2018
  DataType: 
  OrigDataType: 
T_2019: (in si + 0x0001 : word16)
  Class: Eq_1860
  DataType: 
  OrigDataType: 
T_2020: (in al | al : byte)
  Class: Eq_1877
  DataType: 
  OrigDataType: 
T_2021: (in cond(al) : byte)
  Class: Eq_1897
  DataType: 
  OrigDataType: 
T_2022: (in false : bool)
  Class: Eq_1899
  DataType: 
  OrigDataType: 
T_2023: (in Test(EQ,Z) : bool)
  Class: Eq_2023
  DataType: 
  OrigDataType: 
T_2024: (in 0x0034 : word16)
  Class: Eq_2024
  DataType: 
  OrigDataType: 
T_2025: (in fp - 0x0034 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_2026: (in 0x0036 : word16)
  Class: Eq_2026
  DataType: 
  OrigDataType: 
T_2027: (in fp - 0x0036 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_2028: (in 0x0006 : word16)
  Class: Eq_2028
  DataType: 
  OrigDataType: 
T_2029: (in bp + 0x0006 : word16)
  Class: Eq_2029
  DataType: 
  OrigDataType: 
T_2030: (in Mem0[ss:bp + 0x0006:word16] : word16)
  Class: Eq_2030
  DataType: 
  OrigDataType: 
T_2031: (in SEQ(cs, Mem0[ss:bp + 0x0006:word16]) : ptr32)
  Class: Eq_2031
  DataType: 
  OrigDataType: 
T_2032: (in 0x0034 : word16)
  Class: Eq_2032
  DataType: 
  OrigDataType: 
T_2033: (in fp - 0x0034 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_2034: (in 0x0032 : word16)
  Class: Eq_2034
  DataType: 
  OrigDataType: 
T_2035: (in fp - 0x0032 : word16)
  Class: Eq_1846
  DataType: 
  OrigDataType: 
T_2036: (in 0x0001 : word16)
  Class: Eq_2036
  DataType: 
  OrigDataType: 
T_2037: (in wLoc28 - 0x0001 : word16)
  Class: Eq_1868
  DataType: 
  OrigDataType: 
T_2038: (in v28 : word16)
  Class: Eq_1868
  DataType: 
  OrigDataType: 
T_2039: (in cond(v28) : byte)
  Class: Eq_1897
  DataType: 
  OrigDataType: 
T_2040: (in 0x055A : word16)
  Class: Eq_2040
  DataType: 
  OrigDataType: 
T_2041: (in di + 0x055A : word16)
  Class: Eq_2041
  DataType: 
  OrigDataType: 
T_2042: (in Mem0[ds:di + 0x055A:byte] : byte)
  Class: Eq_1943
  DataType: 
  OrigDataType: 
T_2043: (in 0x00 : byte)
  Class: Eq_2043
  DataType: 
  OrigDataType: 
T_2044: (in bh : byte)
  Class: Eq_2043
  DataType: 
  OrigDataType: 
T_2045: (in 0x0015 : word16)
  Class: Eq_2045
  DataType: 
  OrigDataType: 
T_2046: (in ax - 0x0015 : word16)
  Class: Eq_2046
  DataType: 
  OrigDataType: 
T_2047: (in cond(ax - 0x0015) : byte)
  Class: Eq_1856
  DataType: 
  OrigDataType: 
T_2048: (in CZ : byte)
  Class: Eq_2048
  DataType: 
  OrigDataType: 
T_2049: (in Test(ULE,CZ) : bool)
  Class: Eq_2049
  DataType: 
  OrigDataType: 
T_2050: (in 0x0001 : word16)
  Class: Eq_2050
  DataType: 
  OrigDataType: 
T_2051: (in bx << 0x0001 : word16)
  Class: Eq_1864
  DataType: 
  OrigDataType: 
T_2052: (in cond(bx) : byte)
  Class: Eq_1856
  DataType: 
  OrigDataType: 
*/
