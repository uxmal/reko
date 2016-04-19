// TestReko.h
// Generated on 2015-08-06 21:12:11 by decompiling D:\dev\jkl\reko\master\subjects\regressions\reko-15\TestReko.exe
// using Decompiler version 0.5.1.0.

/*
// Equivalence classes ////////////
Eq_1: (struct "Globals" (403000 word32 dw403000) (403004 word32 dw403004) (403018 word32 dw403018) (40301C word32 dw40301C) (403020 word32 dw403020) (403024 word32 dw403024) (403028 (ptr (ptr word32)) ptr403028) (40302C int32 dw40302C) (403368 word32 dw403368) (40336C word32 dw40336C) (403378 (ptr code) ptr403378))
	globals_t (in globals : (ptr (struct "Globals")))
Eq_2: (fn void ())
	T_2 (in fn00401561 : ptr32)
	T_3 (in signature of fn00401561 : void)
Eq_5: (struct "Eq_5" (FFFFFFE4 word32 dwFFFFFFE4) (FFFFFFEC (ptr (ptr word32)) ptrFFFFFFEC) (FFFFFFF0 word32 dwFFFFFFF0) (FFFFFFFC word32 dwFFFFFFFC) (0 word32 dw0000))
	T_5 (in ebp_10 : (ptr Eq_5))
	T_20 (in fn00401770(ebx, esi, edi, dwLoc0C, 0x004021E8, 0x0000000C) : word32)
	T_97 (in ebp : (ptr Eq_5))
Eq_6: (fn (ptr Eq_5) (word32, word32, word32, word32, word32, word32))
	T_6 (in fn00401770 : ptr32)
	T_7 (in signature of fn00401770 : void)
Eq_25: (struct "Eq_25" (FFFFFFFC word32 dwFFFFFFFC) (0 word32 dw0000))
	T_25 (in esp_105 : (ptr Eq_25))
	T_28 (in fp - 0x00000008 : word32)
	T_66 (in fp + 0xFFFFFFF4 : word32)
	T_88 (in fp + 0xFFFFFFF4 : word32)
	T_111 (in esp_174 : (ptr Eq_25))
	T_113 (in esp_105 - 0x00000004 : word32)
	T_180 (in esp_152 + 0x00000004 : word32)
	T_195 (in esp_152 - 0x00000008 : word32)
Eq_26: (struct "Eq_26" (FFFFFFF0 word32 dwFFFFFFF0) (FFFFFFF4 Eq_473 uFFFFFFF4))
	T_26 (in fp : ptr32)
Eq_30: (segment "Eq_30" (18 (ptr Eq_32) ptr0018))
	T_30 (in fs : selector)
Eq_32: (struct "Eq_32" (4 word32 dw0004))
	T_32 (in Mem14[fs:0x00000018:word32] : word32)
Eq_40: (fn void ())
	T_40 (in __lock : ptr32)
Eq_42: (fn bool (word32, word32, word32, (ptr word32)))
	T_42 (in __cmpxchg : ptr32)
Eq_95: (fn void ((ptr Eq_5), word32, word32, word32, word32, word32))
	T_95 (in fn004017B5 : ptr32)
	T_96 (in signature of fn004017B5 : void)
Eq_139: (struct "Eq_139" (FFFFFFF8 word32 dwFFFFFFF8) (FFFFFFFC (ptr (ptr word32)) ptrFFFFFFFC) (0 int32 dw0000))
	T_139 (in esp_109 : (ptr Eq_139))
	T_141 (in esp_105 - 0x00000004 : word32)
Eq_157: (fn void ())
	T_157 (in fn00401000 : ptr32)
	T_158 (in signature of fn00401000 : void)
Eq_167: (struct "Eq_167" (FFFFFFF8 word32 dwFFFFFFF8) (FFFFFFFC word32 dwFFFFFFFC) (0 word32 dw0000) (4 Eq_25 t0004))
	T_167 (in esp_152 : (ptr Eq_167))
	T_169 (in esp_105 - 0x00000004 : word32)
Eq_175: (fn word32 (word32))
	T_175 (in fn00401470 : ptr32)
	T_176 (in signature of fn00401470 : void)
Eq_206: (fn void (int32))
	T_206 (in exit : ptr32)
	T_207 (in signature of exit : void)
Eq_240: (struct "Eq_240" (6 word16 w0006) (14 word16 w0014))
	T_240 (in ecx_20 : (ptr Eq_240))
	T_245 (in dwArg04->dw003C + dwArg04 : word32)
Eq_241: (struct "Eq_241" (3C word32 dw003C))
	T_241 (in dwArg04 : word32)
Eq_253: (struct "Eq_253" 0028 (0 up32 dw0000) (8 word32 dw0008) (28 Eq_253 t0028))
	T_253 (in eax_24 : (ptr Eq_253))
	T_262 (in (word32) ecx_20->w0014 + 0x00000018 + ecx_20 + 0x0000000C : word32)
	T_265 (in 0x00000000 : word32)
	T_269 (in eax_24 + 0x00000028 : word32)
Eq_284: (segment "Eq_284" (0 word32 dw0000))
	T_284 (in fs : selector)
Eq_287: (struct "Eq_287" (FFFFFFD0 word32 dwFFFFFFD0) (FFFFFFF8 word32 dwFFFFFFF8))
	T_287 (in fp : ptr32)
Eq_292: (fn word32 (word32))
	T_292 (in fn00401530 : ptr32)
	T_293 (in signature of fn00401530 : void)
Eq_303: (union "Eq_303" (ptr32 u0) ((memptr (ptr Eq_284) word32) u1))
	T_303 (in 0x00000000 : ptr32)
Eq_312: (struct "Eq_312" (24 uint32 dw0024))
	T_312 (in eax_88 : (ptr Eq_312))
	T_319 (in fn00401420(0x00400000, 0x00400000) : word32)
	T_320 (in 0x00000000 : word32)
Eq_313: (fn (ptr Eq_312) (word32, word32))
	T_313 (in fn00401420 : ptr32)
	T_314 (in signature of fn00401420 : void)
Eq_333: (union "Eq_333" (ptr32 u0) ((memptr (ptr Eq_284) word32) u1))
	T_333 (in 0x00000000 : ptr32)
Eq_338: (struct "Eq_338" (0 word16 w0000) (3C word32 dw003C))
	T_338 (in dwArg04 : word32)
Eq_344: (struct "Eq_344" (0 word32 dw0000) (18 word16 w0018))
	T_344 (in ecx_35 : (ptr Eq_344))
	T_348 (in dwArg04->dw003C + dwArg04 : word32)
Eq_368: (fn void (Eq_370))
	T_368 (in GetSystemTimeAsFileTime : ptr32)
	T_369 (in signature of GetSystemTimeAsFileTime : void)
Eq_370: LPFILETIME
	T_370 (in lpSystemTimeAsFileTime : LPFILETIME)
	T_373 (in fp - 0x00000010 : word32)
Eq_383: (fn Eq_385 ())
	T_383 (in GetCurrentThreadId : ptr32)
	T_384 (in signature of GetCurrentThreadId : void)
Eq_385: DWORD
	T_385 (in GetCurrentThreadId() : DWORD)
Eq_387: (fn Eq_389 ())
	T_387 (in GetCurrentProcessId : ptr32)
	T_388 (in signature of GetCurrentProcessId : void)
Eq_389: DWORD
	T_389 (in GetCurrentProcessId() : DWORD)
Eq_391: (fn Eq_396 ((ptr Eq_393)))
	T_391 (in QueryPerformanceCounter : ptr32)
	T_392 (in signature of QueryPerformanceCounter : void)
Eq_393: LARGE_INTEGER
	T_393 (in lpPerformanceCount : (ptr LARGE_INTEGER))
	T_395 (in fp - 0x00000018 : word32)
Eq_396: BOOL
	T_396 (in QueryPerformanceCounter(fp - 0x00000018) : BOOL)
Eq_430: (struct "Eq_430" (FFFFFFEC word32 dwFFFFFFEC) (FFFFFFF0 word32 dwFFFFFFF0) (FFFFFFF4 word32 dwFFFFFFF4) (FFFFFFF8 word32 dwFFFFFFF8) (FFFFFFFC word32 dwFFFFFFFC))
	T_430 (in esp_13 : (ptr Eq_430))
	T_435 (in fp - 0x00000008 - dwArg08 : word32)
Eq_459: (segment "Eq_459" (0 word32 dw0000))
	T_459 (in fs : selector)
Eq_466: (segment "Eq_466" (0 word32 dw0000))
	T_466 (in fs : selector)
Eq_467: (union "Eq_467" (ptr32 u0) ((memptr (ptr Eq_466) word32) u1))
	T_467 (in 0x00000000 : ptr32)
Eq_473: (union "Eq_473" (word32 u0) (Eq_25 u1))
	T_473
Eq_474: (struct "struct_1" (0 DWORD LowPart) (4 LONG HighPart))
	T_474
// Type Variables ////////////
globals_t: (in globals : (ptr (struct "Globals")))
  Class: Eq_1
  DataType: (ptr Eq_1)
  OrigDataType: (ptr (struct "Globals"))
T_2: (in fn00401561 : ptr32)
  Class: Eq_2
  DataType: (ptr Eq_2)
  OrigDataType: (ptr (fn T_4 ()))
T_3: (in signature of fn00401561 : void)
  Class: Eq_2
  DataType: (ptr Eq_2)
  OrigDataType: 
T_4: (in fn00401561() : void)
  Class: Eq_4
  DataType: void
  OrigDataType: void
T_5: (in ebp_10 : (ptr Eq_5))
  Class: Eq_5
  DataType: (ptr Eq_5)
  OrigDataType: (ptr (struct (FFFFFFE4 T_224 tFFFFFFE4) (FFFFFFEC T_216 tFFFFFFEC) (FFFFFFFC T_21 tFFFFFFFC)))
T_6: (in fn00401770 : ptr32)
  Class: Eq_6
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (fn T_20 (T_14, T_15, T_16, T_17, T_18, T_19)))
T_7: (in signature of fn00401770 : void)
  Class: Eq_6
  DataType: (ptr Eq_6)
  OrigDataType: 
T_8: (in ebx : word32)
  Class: Eq_8
  DataType: word32
  OrigDataType: word32
T_9: (in esi : word32)
  Class: Eq_9
  DataType: word32
  OrigDataType: word32
T_10: (in edi : word32)
  Class: Eq_10
  DataType: word32
  OrigDataType: word32
T_11: (in dwArg00 : word32)
  Class: Eq_11
  DataType: word32
  OrigDataType: word32
T_12: (in dwArg04 : word32)
  Class: Eq_12
  DataType: word32
  OrigDataType: word32
T_13: (in dwArg08 : word32)
  Class: Eq_13
  DataType: word32
  OrigDataType: word32
T_14: (in ebx : word32)
  Class: Eq_8
  DataType: word32
  OrigDataType: word32
T_15: (in esi : word32)
  Class: Eq_9
  DataType: word32
  OrigDataType: word32
T_16: (in edi : word32)
  Class: Eq_10
  DataType: word32
  OrigDataType: word32
T_17: (in dwLoc0C : word32)
  Class: Eq_11
  DataType: word32
  OrigDataType: word32
T_18: (in 0x004021E8 : word32)
  Class: Eq_12
  DataType: word32
  OrigDataType: word32
T_19: (in 0x0000000C : word32)
  Class: Eq_13
  DataType: word32
  OrigDataType: word32
T_20: (in fn00401770(ebx, esi, edi, dwLoc0C, 0x004021E8, 0x0000000C) : word32)
  Class: Eq_5
  DataType: (ptr Eq_5)
  OrigDataType: word32
T_21: (in 0x00000000 : word32)
  Class: Eq_21
  DataType: word32
  OrigDataType: word32
T_22: (in 0x00000004 : word32)
  Class: Eq_22
  DataType: word32
  OrigDataType: word32
T_23: (in ebp_10 - 0x00000004 : word32)
  Class: Eq_23
  DataType: word32
  OrigDataType: word32
T_24: (in Mem14[ebp_10 - 0x00000004:word32] : word32)
  Class: Eq_21
  DataType: word32
  OrigDataType: word32
T_25: (in esp_105 : (ptr Eq_25))
  Class: Eq_25
  DataType: (ptr Eq_25)
  OrigDataType: (ptr (struct (FFFFFFFC T_121 tFFFFFFFC) (0 T_117 t0000)))
T_26: (in fp : ptr32)
  Class: Eq_26
  DataType: (ptr Eq_26)
  OrigDataType: (ptr (struct (FFFFFFF0 T_84 tFFFFFFF0) (FFFFFFF4 T_59 tFFFFFFF4)))
T_27: (in 0x00000008 : word32)
  Class: Eq_27
  DataType: word32
  OrigDataType: word32
T_28: (in fp - 0x00000008 : word32)
  Class: Eq_25
  DataType: (ptr Eq_25)
  OrigDataType: word32
T_29: (in edx_17 : word32)
  Class: Eq_29
  DataType: word32
  OrigDataType: word32
T_30: (in fs : selector)
  Class: Eq_30
  DataType: (ptr Eq_30)
  OrigDataType: (ptr (segment (18 T_32 t0018)))
T_31: (in 0x00000018 : word32)
  Class: Eq_31
  DataType: (memptr (ptr Eq_30) (ptr Eq_32))
  OrigDataType: (memptr T_30 (struct (0 T_32 t0000)))
T_32: (in Mem14[fs:0x00000018:word32] : word32)
  Class: Eq_32
  DataType: (ptr Eq_32)
  OrigDataType: (ptr (struct (4 T_35 t0004)))
T_33: (in 0x00000004 : word32)
  Class: Eq_33
  DataType: word32
  OrigDataType: word32
T_34: (in Mem14[fs:0x00000018:word32] + 0x00000004 : word32)
  Class: Eq_34
  DataType: word32
  OrigDataType: word32
T_35: (in Mem14[Mem14[fs:0x00000018:word32] + 0x00000004:word32] : word32)
  Class: Eq_29
  DataType: word32
  OrigDataType: word32
T_36: (in edi_18 : word32)
  Class: Eq_36
  DataType: word32
  OrigDataType: word32
T_37: (in 0x00000000 : word32)
  Class: Eq_36
  DataType: word32
  OrigDataType: word32
T_38: (in eax_24 : word32)
  Class: Eq_29
  DataType: word32
  OrigDataType: word32
T_39: (in eax_24 != edx_17 : bool)
  Class: Eq_39
  DataType: bool
  OrigDataType: bool
T_40: (in __lock : ptr32)
  Class: Eq_40
  DataType: (ptr Eq_40)
  OrigDataType: (ptr (fn T_41 ()))
T_41: (in __lock() : void)
  Class: Eq_41
  DataType: void
  OrigDataType: void
T_42: (in __cmpxchg : ptr32)
  Class: Eq_42
  DataType: (ptr Eq_42)
  OrigDataType: (ptr (fn T_47 (T_44, T_29, T_45, T_46)))
T_43: (in 0x00403368 : ptr32)
  Class: Eq_43
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_44 t0000)))
T_44: (in Mem14[0x00403368:word32] : word32)
  Class: Eq_44
  DataType: word32
  OrigDataType: word32
T_45: (in 0x00000000 : word32)
  Class: Eq_45
  DataType: word32
  OrigDataType: word32
T_46: (in out eax_24 : word32)
  Class: Eq_46
  DataType: (ptr word32)
  OrigDataType: (ptr word32)
T_47: (in __cmpxchg(globals->dw403368, edx_17, 0x00000000, out eax_24) : bool)
  Class: Eq_47
  DataType: bool
  OrigDataType: bool
T_48: (in 0x00000000 : word32)
  Class: Eq_29
  DataType: word32
  OrigDataType: word32
T_49: (in eax_24 == 0x00000000 : bool)
  Class: Eq_49
  DataType: bool
  OrigDataType: bool
T_50: (in 0x00000001 : word32)
  Class: Eq_36
  DataType: word32
  OrigDataType: word32
T_51: (in 0x0040336C : ptr32)
  Class: Eq_51
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_52 t0000)))
T_52: (in Mem14[0x0040336C:word32] : word32)
  Class: Eq_52
  DataType: word32
  OrigDataType: word32
T_53: (in 0x00000001 : word32)
  Class: Eq_52
  DataType: word32
  OrigDataType: word32
T_54: (in globals->dw40336C != 0x00000001 : bool)
  Class: Eq_54
  DataType: bool
  OrigDataType: bool
T_55: (in 0x0040336C : ptr32)
  Class: Eq_55
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_56 t0000)))
T_56: (in Mem14[0x0040336C:word32] : word32)
  Class: Eq_52
  DataType: word32
  OrigDataType: word32
T_57: (in 0x00000000 : word32)
  Class: Eq_52
  DataType: word32
  OrigDataType: word32
T_58: (in globals->dw40336C != 0x00000000 : bool)
  Class: Eq_58
  DataType: bool
  OrigDataType: bool
T_59: (in 0x0000001F : word32)
  Class: Eq_59
  DataType: word32
  OrigDataType: word32
T_60: (in 0x0000000C : word32)
  Class: Eq_60
  DataType: word32
  OrigDataType: word32
T_61: (in fp - 0x0000000C : word32)
  Class: Eq_61
  DataType: word32
  OrigDataType: word32
T_62: (in Mem185[fp - 0x0000000C:word32] : word32)
  Class: Eq_59
  DataType: word32
  OrigDataType: word32
T_63: (in _amsg_exit : ptr32)
  Class: Eq_63
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_64: (in signature of _amsg_exit : void)
  Class: Eq_63
  DataType: (ptr code)
  OrigDataType: 
T_65: (in 0xFFFFFFF4 : word32)
  Class: Eq_65
  DataType: word32
  OrigDataType: word32
T_66: (in fp + 0xFFFFFFF4 : word32)
  Class: Eq_25
  DataType: (ptr Eq_25)
  OrigDataType: word32
T_67: (in 0x0040336C : ptr32)
  Class: Eq_67
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_68 t0000)))
T_68: (in Mem14[0x0040336C:word32] : word32)
  Class: Eq_52
  DataType: word32
  OrigDataType: word32
T_69: (in 0x00000001 : word32)
  Class: Eq_52
  DataType: word32
  OrigDataType: word32
T_70: (in globals->dw40336C != 0x00000001 : bool)
  Class: Eq_70
  DataType: bool
  OrigDataType: bool
T_71: (in 0x00000001 : word32)
  Class: Eq_71
  DataType: word32
  OrigDataType: word32
T_72: (in 0x00403018 : ptr32)
  Class: Eq_72
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_73 t0000)))
T_73: (in Mem206[0x00403018:word32] : word32)
  Class: Eq_71
  DataType: word32
  OrigDataType: word32
T_74: (in 0x00000001 : word32)
  Class: Eq_52
  DataType: word32
  OrigDataType: word32
T_75: (in 0x0040336C : ptr32)
  Class: Eq_75
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_76 t0000)))
T_76: (in Mem191[0x0040336C:word32] : word32)
  Class: Eq_52
  DataType: word32
  OrigDataType: word32
T_77: (in 0x004020B0 : word32)
  Class: Eq_59
  DataType: word32
  OrigDataType: word32
T_78: (in 0x0000000C : word32)
  Class: Eq_78
  DataType: word32
  OrigDataType: word32
T_79: (in fp - 0x0000000C : word32)
  Class: Eq_79
  DataType: word32
  OrigDataType: word32
T_80: (in Mem193[fp - 0x0000000C:word32] : word32)
  Class: Eq_59
  DataType: word32
  OrigDataType: word32
T_81: (in 0x004020A0 : word32)
  Class: Eq_81
  DataType: word32
  OrigDataType: word32
T_82: (in 0x00000010 : word32)
  Class: Eq_82
  DataType: word32
  OrigDataType: word32
T_83: (in fp - 0x00000010 : word32)
  Class: Eq_83
  DataType: word32
  OrigDataType: word32
T_84: (in Mem195[fp - 0x00000010:word32] : word32)
  Class: Eq_81
  DataType: word32
  OrigDataType: word32
T_85: (in _initterm_e : ptr32)
  Class: Eq_85
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_86: (in signature of _initterm_e : void)
  Class: Eq_85
  DataType: (ptr code)
  OrigDataType: 
T_87: (in 0xFFFFFFF4 : word32)
  Class: Eq_87
  DataType: word32
  OrigDataType: word32
T_88: (in fp + 0xFFFFFFF4 : word32)
  Class: Eq_25
  DataType: (ptr Eq_25)
  OrigDataType: word32
T_89: (in 0x00000000 : word32)
  Class: Eq_29
  DataType: word32
  OrigDataType: word32
T_90: (in eax_24 == 0x00000000 : bool)
  Class: Eq_90
  DataType: bool
  OrigDataType: bool
T_91: (in 0xFFFFFFFE : word32)
  Class: Eq_21
  DataType: word32
  OrigDataType: word32
T_92: (in 0x00000004 : word32)
  Class: Eq_92
  DataType: word32
  OrigDataType: word32
T_93: (in ebp_10 - 0x00000004 : word32)
  Class: Eq_93
  DataType: word32
  OrigDataType: word32
T_94: (in Mem204[ebp_10 - 0x00000004:word32] : word32)
  Class: Eq_21
  DataType: word32
  OrigDataType: word32
T_95: (in fn004017B5 : ptr32)
  Class: Eq_95
  DataType: (ptr Eq_95)
  OrigDataType: (ptr (fn T_108 (T_5, T_103, T_104, T_105, T_106, T_107)))
T_96: (in signature of fn004017B5 : void)
  Class: Eq_95
  DataType: (ptr Eq_95)
  OrigDataType: 
T_97: (in ebp : (ptr Eq_5))
  Class: Eq_5
  DataType: (ptr Eq_5)
  OrigDataType: (ptr (struct (FFFFFFF0 T_465 tFFFFFFF0) (0 word32 dw0000)))
T_98: (in dwArg00 : word32)
  Class: Eq_98
  DataType: word32
  OrigDataType: word32
T_99: (in dwArg04 : word32)
  Class: Eq_99
  DataType: word32
  OrigDataType: word32
T_100: (in dwArg08 : word32)
  Class: Eq_100
  DataType: word32
  OrigDataType: word32
T_101: (in dwArg0C : word32)
  Class: Eq_101
  DataType: word32
  OrigDataType: word32
T_102: (in dwArg10 : word32)
  Class: Eq_102
  DataType: word32
  OrigDataType: word32
T_103: (in 0x0000000C : word32)
  Class: Eq_98
  DataType: word32
  OrigDataType: word32
T_104: (in dwArg00 : word32)
  Class: Eq_99
  DataType: word32
  OrigDataType: word32
T_105: (in dwArg04 : word32)
  Class: Eq_100
  DataType: word32
  OrigDataType: word32
T_106: (in dwArg08 : word32)
  Class: Eq_101
  DataType: word32
  OrigDataType: word32
T_107: (in dwArg0C : word32)
  Class: Eq_102
  DataType: word32
  OrigDataType: word32
T_108: (in fn004017B5(ebp_10, 0x0000000C, dwArg00, dwArg04, dwArg08, dwArg0C) : void)
  Class: Eq_108
  DataType: void
  OrigDataType: void
T_109: (in 0x00000000 : word32)
  Class: Eq_36
  DataType: word32
  OrigDataType: word32
T_110: (in edi_18 != 0x00000000 : bool)
  Class: Eq_110
  DataType: bool
  OrigDataType: bool
T_111: (in esp_174 : (ptr Eq_25))
  Class: Eq_25
  DataType: (ptr Eq_25)
  OrigDataType: (ptr (struct (FFFFFFFC T_121 tFFFFFFFC) (0 T_117 t0000)))
T_112: (in 0x00000004 : word32)
  Class: Eq_112
  DataType: word32
  OrigDataType: word32
T_113: (in esp_105 - 0x00000004 : word32)
  Class: Eq_25
  DataType: (ptr Eq_25)
  OrigDataType: word32
T_114: (in 0x0040209C : word32)
  Class: Eq_114
  DataType: word32
  OrigDataType: word32
T_115: (in 0x00000000 : word32)
  Class: Eq_115
  DataType: word32
  OrigDataType: word32
T_116: (in esp_174 + 0x00000000 : word32)
  Class: Eq_116
  DataType: word32
  OrigDataType: word32
T_117: (in Mem175[esp_174 + 0x00000000:word32] : word32)
  Class: Eq_114
  DataType: word32
  OrigDataType: word32
T_118: (in 0x00402094 : word32)
  Class: Eq_118
  DataType: word32
  OrigDataType: word32
T_119: (in 0x00000004 : word32)
  Class: Eq_119
  DataType: word32
  OrigDataType: word32
T_120: (in esp_174 - 0x00000004 : word32)
  Class: Eq_120
  DataType: word32
  OrigDataType: word32
T_121: (in Mem177[esp_174 - 0x00000004:word32] : word32)
  Class: Eq_118
  DataType: word32
  OrigDataType: word32
T_122: (in _initterm : ptr32)
  Class: Eq_122
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_123: (in signature of _initterm : void)
  Class: Eq_122
  DataType: (ptr code)
  OrigDataType: 
T_124: (in 0x00000002 : word32)
  Class: Eq_52
  DataType: word32
  OrigDataType: word32
T_125: (in 0x0040336C : ptr32)
  Class: Eq_125
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_126 t0000)))
T_126: (in Mem183[0x0040336C:word32] : word32)
  Class: Eq_52
  DataType: word32
  OrigDataType: word32
T_127: (in 0x00403378 : ptr32)
  Class: Eq_127
  DataType: (ptr (ptr code))
  OrigDataType: (ptr (struct (0 T_128 t0000)))
T_128: (in Mem14[0x00403378:word32] : word32)
  Class: Eq_128
  DataType: (ptr code)
  OrigDataType: word32
T_129: (in 0x00000000 : word32)
  Class: Eq_128
  DataType: (ptr code)
  OrigDataType: word32
T_130: (in globals->ptr403378 == null : bool)
  Class: Eq_130
  DataType: bool
  OrigDataType: bool
T_131: (in 0x00000000 : word32)
  Class: Eq_44
  DataType: word32
  OrigDataType: word32
T_132: (in 0x00403368 : ptr32)
  Class: Eq_132
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_133 t0000)))
T_133: (in Mem172[0x00403368:word32] : word32)
  Class: Eq_44
  DataType: word32
  OrigDataType: word32
T_134: (in 0x0040302C : ptr32)
  Class: Eq_134
  DataType: (ptr int32)
  OrigDataType: (ptr (struct (0 T_135 t0000)))
T_135: (in Mem14[0x0040302C:word32] : word32)
  Class: Eq_135
  DataType: int32
  OrigDataType: word32
T_136: (in __initenv : ptr32)
  Class: Eq_136
  DataType: ptr32
  OrigDataType: ptr32
T_137: (in signature of __initenv : void)
  Class: Eq_136
  DataType: ptr32
  OrigDataType: 
T_138: (in Mem108[__initenv:word32] : word32)
  Class: Eq_135
  DataType: int32
  OrigDataType: word32
T_139: (in esp_109 : (ptr Eq_139))
  Class: Eq_139
  DataType: (ptr Eq_139)
  OrigDataType: (ptr (struct (FFFFFFF8 T_153 tFFFFFFF8) (FFFFFFFC T_148 tFFFFFFFC) (0 T_135 t0000)))
T_140: (in 0x00000004 : word32)
  Class: Eq_140
  DataType: word32
  OrigDataType: word32
T_141: (in esp_105 - 0x00000004 : word32)
  Class: Eq_139
  DataType: (ptr Eq_139)
  OrigDataType: word32
T_142: (in 0x0040302C : ptr32)
  Class: Eq_142
  DataType: (ptr int32)
  OrigDataType: (ptr (struct (0 T_143 t0000)))
T_143: (in Mem108[0x0040302C:word32] : word32)
  Class: Eq_135
  DataType: int32
  OrigDataType: word32
T_144: (in 0x00000000 : word32)
  Class: Eq_144
  DataType: word32
  OrigDataType: word32
T_145: (in esp_109 + 0x00000000 : word32)
  Class: Eq_145
  DataType: word32
  OrigDataType: word32
T_146: (in Mem110[esp_109 + 0x00000000:word32] : word32)
  Class: Eq_135
  DataType: int32
  OrigDataType: word32
T_147: (in 0x00403028 : ptr32)
  Class: Eq_147
  DataType: (ptr (ptr (ptr word32)))
  OrigDataType: (ptr (struct (0 T_148 t0000)))
T_148: (in Mem110[0x00403028:word32] : word32)
  Class: Eq_148
  DataType: (ptr (ptr word32))
  OrigDataType: word32
T_149: (in 0x00000004 : word32)
  Class: Eq_149
  DataType: word32
  OrigDataType: word32
T_150: (in esp_109 - 0x00000004 : word32)
  Class: Eq_150
  DataType: word32
  OrigDataType: word32
T_151: (in Mem112[esp_109 - 0x00000004:word32] : word32)
  Class: Eq_148
  DataType: (ptr (ptr word32))
  OrigDataType: word32
T_152: (in 0x00403024 : ptr32)
  Class: Eq_152
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_153 t0000)))
T_153: (in Mem112[0x00403024:word32] : word32)
  Class: Eq_153
  DataType: word32
  OrigDataType: word32
T_154: (in 0x00000008 : word32)
  Class: Eq_154
  DataType: word32
  OrigDataType: word32
T_155: (in esp_109 - 0x00000008 : word32)
  Class: Eq_155
  DataType: word32
  OrigDataType: word32
T_156: (in Mem114[esp_109 - 0x00000008:word32] : word32)
  Class: Eq_153
  DataType: word32
  OrigDataType: word32
T_157: (in fn00401000 : ptr32)
  Class: Eq_157
  DataType: (ptr Eq_157)
  OrigDataType: (ptr (fn T_159 ()))
T_158: (in signature of fn00401000 : void)
  Class: Eq_157
  DataType: (ptr Eq_157)
  OrigDataType: 
T_159: (in fn00401000() : void)
  Class: Eq_159
  DataType: void
  OrigDataType: void
T_160: (in 0x00000000 : word32)
  Class: Eq_160
  DataType: word32
  OrigDataType: word32
T_161: (in 0x0040301C : ptr32)
  Class: Eq_161
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_162 t0000)))
T_162: (in Mem116[0x0040301C:word32] : word32)
  Class: Eq_160
  DataType: word32
  OrigDataType: word32
T_163: (in 0x00403020 : ptr32)
  Class: Eq_163
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_164 t0000)))
T_164: (in Mem116[0x00403020:word32] : word32)
  Class: Eq_164
  DataType: word32
  OrigDataType: word32
T_165: (in 0x00000000 : word32)
  Class: Eq_164
  DataType: word32
  OrigDataType: word32
T_166: (in globals->dw403020 != 0x00000000 : bool)
  Class: Eq_166
  DataType: bool
  OrigDataType: bool
T_167: (in esp_152 : (ptr Eq_167))
  Class: Eq_167
  DataType: (ptr Eq_167)
  OrigDataType: (ptr (struct (FFFFFFF8 T_194 tFFFFFFF8) (FFFFFFFC T_190 tFFFFFFFC) (0 T_170 t0000)))
T_168: (in 0x00000004 : word32)
  Class: Eq_168
  DataType: word32
  OrigDataType: word32
T_169: (in esp_105 - 0x00000004 : word32)
  Class: Eq_167
  DataType: (ptr Eq_167)
  OrigDataType: word32
T_170: (in 0x00403378 : word32)
  Class: Eq_170
  DataType: word32
  OrigDataType: word32
T_171: (in 0x00000000 : word32)
  Class: Eq_171
  DataType: word32
  OrigDataType: word32
T_172: (in esp_152 + 0x00000000 : word32)
  Class: Eq_172
  DataType: word32
  OrigDataType: word32
T_173: (in Mem153[esp_152 + 0x00000000:word32] : word32)
  Class: Eq_170
  DataType: word32
  OrigDataType: word32
T_174: (in eax_154 : word32)
  Class: Eq_174
  DataType: word32
  OrigDataType: word32
T_175: (in fn00401470 : ptr32)
  Class: Eq_175
  DataType: (ptr Eq_175)
  OrigDataType: (ptr (fn T_178 (T_104)))
T_176: (in signature of fn00401470 : void)
  Class: Eq_175
  DataType: (ptr Eq_175)
  OrigDataType: 
T_177: (in dwArg04 : word32)
  Class: Eq_99
  DataType: word32
  OrigDataType: word32
T_178: (in fn00401470(dwArg00) : word32)
  Class: Eq_174
  DataType: word32
  OrigDataType: word32
T_179: (in 0x00000004 : word32)
  Class: Eq_179
  DataType: word32
  OrigDataType: word32
T_180: (in esp_152 + 0x00000004 : word32)
  Class: Eq_25
  DataType: (ptr Eq_25)
  OrigDataType: word32
T_181: (in 0x00000000 : word32)
  Class: Eq_174
  DataType: word32
  OrigDataType: word32
T_182: (in eax_154 == 0x00000000 : bool)
  Class: Eq_182
  DataType: bool
  OrigDataType: bool
T_183: (in 0x00000000 : word32)
  Class: Eq_170
  DataType: word32
  OrigDataType: word32
T_184: (in 0x00000000 : word32)
  Class: Eq_184
  DataType: word32
  OrigDataType: word32
T_185: (in esp_152 + 0x00000000 : word32)
  Class: Eq_185
  DataType: word32
  OrigDataType: word32
T_186: (in Mem161[esp_152 + 0x00000000:word32] : word32)
  Class: Eq_170
  DataType: word32
  OrigDataType: word32
T_187: (in 0x00000002 : word32)
  Class: Eq_187
  DataType: word32
  OrigDataType: word32
T_188: (in 0x00000004 : word32)
  Class: Eq_188
  DataType: word32
  OrigDataType: word32
T_189: (in esp_152 - 0x00000004 : word32)
  Class: Eq_189
  DataType: word32
  OrigDataType: word32
T_190: (in Mem163[esp_152 - 0x00000004:word32] : word32)
  Class: Eq_187
  DataType: word32
  OrigDataType: word32
T_191: (in 0x00000000 : word32)
  Class: Eq_191
  DataType: word32
  OrigDataType: word32
T_192: (in 0x00000008 : word32)
  Class: Eq_192
  DataType: word32
  OrigDataType: word32
T_193: (in esp_152 - 0x00000008 : word32)
  Class: Eq_193
  DataType: word32
  OrigDataType: word32
T_194: (in Mem165[esp_152 - 0x00000008:word32] : word32)
  Class: Eq_191
  DataType: word32
  OrigDataType: word32
T_195: (in esp_152 - 0x00000008 : word32)
  Class: Eq_25
  DataType: (ptr Eq_25)
  OrigDataType: word32
T_196: (in 0x00403378 : ptr32)
  Class: Eq_196
  DataType: (ptr (ptr code))
  OrigDataType: (ptr (struct (0 T_197 t0000)))
T_197: (in Mem165[0x00403378:word32] : word32)
  Class: Eq_128
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_198: (in 0x00403018 : ptr32)
  Class: Eq_198
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_199 t0000)))
T_199: (in Mem116[0x00403018:word32] : word32)
  Class: Eq_71
  DataType: word32
  OrigDataType: word32
T_200: (in 0x00000000 : word32)
  Class: Eq_71
  DataType: word32
  OrigDataType: word32
T_201: (in globals->dw403018 != 0x00000000 : bool)
  Class: Eq_201
  DataType: bool
  OrigDataType: bool
T_202: (in 0x00000000 : word32)
  Class: Eq_135
  DataType: int32
  OrigDataType: word32
T_203: (in 0x00000000 : word32)
  Class: Eq_203
  DataType: word32
  OrigDataType: word32
T_204: (in esp_109 + 0x00000000 : word32)
  Class: Eq_204
  DataType: word32
  OrigDataType: word32
T_205: (in Mem120[esp_109 + 0x00000000:word32] : word32)
  Class: Eq_135
  DataType: int32
  OrigDataType: word32
T_206: (in exit : ptr32)
  Class: Eq_206
  DataType: (ptr Eq_206)
  OrigDataType: (ptr (fn T_212 (T_211)))
T_207: (in signature of exit : void)
  Class: Eq_206
  DataType: (ptr Eq_206)
  OrigDataType: 
T_208: (in _Code : int32)
  Class: Eq_135
  DataType: int32
  OrigDataType: int32
T_209: (in 0x00000000 : word32)
  Class: Eq_209
  DataType: word32
  OrigDataType: word32
T_210: (in esp_109 + 0x00000000 : word32)
  Class: Eq_210
  DataType: word32
  OrigDataType: word32
T_211: (in Mem120[esp_109 + 0x00000000:int32] : int32)
  Class: Eq_135
  DataType: int32
  OrigDataType: int32
T_212: (in exit(esp_109->dw0000) : void)
  Class: Eq_212
  DataType: void
  OrigDataType: void
T_213: (in ecx_121 : (ptr (ptr word32)))
  Class: Eq_148
  DataType: (ptr (ptr word32))
  OrigDataType: (ptr (struct (0 T_220 t0000)))
T_214: (in 0x00000014 : word32)
  Class: Eq_214
  DataType: word32
  OrigDataType: word32
T_215: (in ebp_10 - 0x00000014 : word32)
  Class: Eq_215
  DataType: word32
  OrigDataType: word32
T_216: (in Mem120[ebp_10 - 0x00000014:word32] : word32)
  Class: Eq_148
  DataType: (ptr (ptr word32))
  OrigDataType: word32
T_217: (in eax_123 : word32)
  Class: Eq_153
  DataType: word32
  OrigDataType: word32
T_218: (in 0x00000000 : word32)
  Class: Eq_218
  DataType: word32
  OrigDataType: word32
T_219: (in ecx_121 + 0x00000000 : word32)
  Class: Eq_219
  DataType: word32
  OrigDataType: word32
T_220: (in Mem120[ecx_121 + 0x00000000:word32] : word32)
  Class: Eq_220
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_221 t0000)))
T_221: (in Mem120[Mem120[ecx_121 + 0x00000000:word32]:word32] : word32)
  Class: Eq_153
  DataType: word32
  OrigDataType: word32
T_222: (in 0x0000001C : word32)
  Class: Eq_222
  DataType: word32
  OrigDataType: word32
T_223: (in ebp_10 - 0x0000001C : word32)
  Class: Eq_223
  DataType: word32
  OrigDataType: word32
T_224: (in Mem124[ebp_10 - 0x0000001C:word32] : word32)
  Class: Eq_153
  DataType: word32
  OrigDataType: word32
T_225: (in 0x00000004 : word32)
  Class: Eq_225
  DataType: word32
  OrigDataType: word32
T_226: (in esp_109 - 0x00000004 : word32)
  Class: Eq_226
  DataType: word32
  OrigDataType: word32
T_227: (in Mem126[esp_109 - 0x00000004:word32] : word32)
  Class: Eq_148
  DataType: (ptr (ptr word32))
  OrigDataType: word32
T_228: (in 0x00000008 : word32)
  Class: Eq_228
  DataType: word32
  OrigDataType: word32
T_229: (in esp_109 - 0x00000008 : word32)
  Class: Eq_229
  DataType: word32
  OrigDataType: word32
T_230: (in Mem128[esp_109 - 0x00000008:word32] : word32)
  Class: Eq_153
  DataType: word32
  OrigDataType: word32
T_231: (in _XcptFilter : ptr32)
  Class: Eq_231
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_232: (in signature of _XcptFilter : void)
  Class: Eq_231
  DataType: (ptr code)
  OrigDataType: 
T_233: (in 0xFFFFFFFE : word32)
  Class: Eq_21
  DataType: word32
  OrigDataType: word32
T_234: (in 0x00000004 : word32)
  Class: Eq_234
  DataType: word32
  OrigDataType: word32
T_235: (in ebp_10 - 0x00000004 : word32)
  Class: Eq_235
  DataType: word32
  OrigDataType: word32
T_236: (in Mem149[ebp_10 - 0x00000004:word32] : word32)
  Class: Eq_21
  DataType: word32
  OrigDataType: word32
T_237: (in _cexit : ptr32)
  Class: Eq_237
  DataType: (ptr code)
  OrigDataType: (ptr code)
T_238: (in signature of _cexit : void)
  Class: Eq_237
  DataType: (ptr code)
  OrigDataType: 
T_239: (in eax : word32)
  Class: Eq_239
  DataType: word32
  OrigDataType: word32
T_240: (in ecx_20 : (ptr Eq_240))
  Class: Eq_240
  DataType: (ptr Eq_240)
  OrigDataType: (ptr (struct (6 T_251 t0006) (14 T_256 t0014)))
T_241: (in dwArg04 : word32)
  Class: Eq_241
  DataType: (ptr Eq_241)
  OrigDataType: (ptr (struct (3C T_244 t003C)))
T_242: (in 0x0000003C : word32)
  Class: Eq_242
  DataType: word32
  OrigDataType: word32
T_243: (in dwArg04 + 0x0000003C : word32)
  Class: Eq_243
  DataType: word32
  OrigDataType: word32
T_244: (in Mem0[dwArg04 + 0x0000003C:word32] : word32)
  Class: Eq_244
  DataType: word32
  OrigDataType: word32
T_245: (in dwArg04->dw003C + dwArg04 : word32)
  Class: Eq_240
  DataType: (ptr Eq_240)
  OrigDataType: word32
T_246: (in edx_54 : up32)
  Class: Eq_246
  DataType: up32
  OrigDataType: up32
T_247: (in 0x00000000 : word32)
  Class: Eq_246
  DataType: up32
  OrigDataType: word32
T_248: (in ebx_22 : up32)
  Class: Eq_246
  DataType: up32
  OrigDataType: up32
T_249: (in 0x00000006 : word32)
  Class: Eq_249
  DataType: word32
  OrigDataType: word32
T_250: (in ecx_20 + 0x00000006 : word32)
  Class: Eq_250
  DataType: word32
  OrigDataType: word32
T_251: (in Mem0[ecx_20 + 0x00000006:word16] : word16)
  Class: Eq_251
  DataType: word16
  OrigDataType: word16
T_252: (in (word32) ecx_20->w0006 : word32)
  Class: Eq_246
  DataType: up32
  OrigDataType: word32
T_253: (in eax_24 : (ptr Eq_253))
  Class: Eq_253
  DataType: (ptr Eq_253)
  OrigDataType: (ptr (struct 0028 (0 T_280 t0000) (8 T_274 t0008)))
T_254: (in 0x00000014 : word32)
  Class: Eq_254
  DataType: word32
  OrigDataType: word32
T_255: (in ecx_20 + 0x00000014 : word32)
  Class: Eq_255
  DataType: word32
  OrigDataType: word32
T_256: (in Mem0[ecx_20 + 0x00000014:word16] : word16)
  Class: Eq_256
  DataType: word16
  OrigDataType: word16
T_257: (in (word32) ecx_20->w0014 : word32)
  Class: Eq_257
  DataType: word32
  OrigDataType: word32
T_258: (in 0x00000018 : word32)
  Class: Eq_258
  DataType: word32
  OrigDataType: word32
T_259: (in (word32) ecx_20->w0014 + 0x00000018 : word32)
  Class: Eq_259
  DataType: word32
  OrigDataType: word32
T_260: (in (word32) ecx_20->w0014 + 0x00000018 + ecx_20 : word32)
  Class: Eq_260
  DataType: word32
  OrigDataType: word32
T_261: (in 0x0000000C : word32)
  Class: Eq_261
  DataType: word32
  OrigDataType: word32
T_262: (in (word32) ecx_20->w0014 + 0x00000018 + ecx_20 + 0x0000000C : word32)
  Class: Eq_253
  DataType: (ptr Eq_253)
  OrigDataType: word32
T_263: (in 0x00000000 : word32)
  Class: Eq_246
  DataType: up32
  OrigDataType: word32
T_264: (in ebx_22 == 0x00000000 : bool)
  Class: Eq_264
  DataType: bool
  OrigDataType: bool
T_265: (in 0x00000000 : word32)
  Class: Eq_253
  DataType: (ptr Eq_253)
  OrigDataType: word32
T_266: (in 0x00000001 : word32)
  Class: Eq_266
  DataType: word32
  OrigDataType: word32
T_267: (in edx_54 + 0x00000001 : word32)
  Class: Eq_246
  DataType: up32
  OrigDataType: word32
T_268: (in 0x00000028 : word32)
  Class: Eq_268
  DataType: word32
  OrigDataType: word32
T_269: (in eax_24 + 0x00000028 : word32)
  Class: Eq_253
  DataType: (ptr Eq_253)
  OrigDataType: word32
T_270: (in edx_54 <u ebx_22 : bool)
  Class: Eq_270
  DataType: bool
  OrigDataType: bool
T_271: (in dwArg08 : word32)
  Class: Eq_271
  DataType: up32
  OrigDataType: up32
T_272: (in 0x00000008 : word32)
  Class: Eq_272
  DataType: word32
  OrigDataType: word32
T_273: (in eax_24 + 0x00000008 : word32)
  Class: Eq_273
  DataType: word32
  OrigDataType: word32
T_274: (in Mem0[eax_24 + 0x00000008:word32] : word32)
  Class: Eq_274
  DataType: word32
  OrigDataType: word32
T_275: (in esi_56 : up32)
  Class: Eq_271
  DataType: up32
  OrigDataType: up32
T_276: (in eax_24->dw0008 + esi_56 : word32)
  Class: Eq_271
  DataType: up32
  OrigDataType: up32
T_277: (in dwArg08 <u eax_24->dw0008 + esi_56 : bool)
  Class: Eq_277
  DataType: bool
  OrigDataType: bool
T_278: (in 0x00000000 : word32)
  Class: Eq_278
  DataType: word32
  OrigDataType: word32
T_279: (in eax_24 + 0x00000000 : word32)
  Class: Eq_279
  DataType: word32
  OrigDataType: word32
T_280: (in Mem0[eax_24 + 0x00000000:word32] : word32)
  Class: Eq_271
  DataType: up32
  OrigDataType: word32
T_281: (in dwArg08 <u esi_56 : bool)
  Class: Eq_281
  DataType: bool
  OrigDataType: bool
T_282: (in eax : word32)
  Class: Eq_282
  DataType: word32
  OrigDataType: word32
T_283: (in eax_14 : word32)
  Class: Eq_283
  DataType: word32
  OrigDataType: word32
T_284: (in fs : selector)
  Class: Eq_284
  DataType: (ptr Eq_284)
  OrigDataType: (ptr (segment (0 T_283 t0000)))
T_285: (in 0x00000000 : word32)
  Class: Eq_285
  DataType: (memptr (ptr Eq_284) word32)
  OrigDataType: (memptr T_284 (struct (0 T_286 t0000)))
T_286: (in Mem0[fs:0x00000000:word32] : word32)
  Class: Eq_283
  DataType: word32
  OrigDataType: word32
T_287: (in fp : ptr32)
  Class: Eq_287
  DataType: (ptr Eq_287)
  OrigDataType: (ptr (struct (FFFFFFD0 T_311 tFFFFFFD0) (FFFFFFF8 T_299 tFFFFFFF8)))
T_288: (in 0x00000014 : word32)
  Class: Eq_288
  DataType: word32
  OrigDataType: word32
T_289: (in fp - 0x00000014 : word32)
  Class: Eq_283
  DataType: word32
  OrigDataType: word32
T_290: (in 0x00000000 : word32)
  Class: Eq_290
  DataType: (memptr (ptr Eq_284) word32)
  OrigDataType: (memptr T_284 (struct (0 T_291 t0000)))
T_291: (in Mem37[fs:0x00000000:word32] : word32)
  Class: Eq_283
  DataType: word32
  OrigDataType: word32
T_292: (in fn00401530 : ptr32)
  Class: Eq_292
  DataType: (ptr Eq_292)
  OrigDataType: (ptr (fn T_296 (T_295)))
T_293: (in signature of fn00401530 : void)
  Class: Eq_292
  DataType: (ptr Eq_292)
  OrigDataType: 
T_294: (in dwArg04 : word32)
  Class: Eq_294
  DataType: word32
  OrigDataType: word32
T_295: (in 0x00400000 : word32)
  Class: Eq_294
  DataType: word32
  OrigDataType: word32
T_296: (in fn00401530(0x00400000) : word32)
  Class: Eq_296
  DataType: word32
  OrigDataType: word32
T_297: (in 0x00000000 : word32)
  Class: Eq_296
  DataType: word32
  OrigDataType: word32
T_298: (in fn00401530(0x00400000) == 0x00000000 : bool)
  Class: Eq_298
  DataType: bool
  OrigDataType: bool
T_299: (in 0xFFFFFFFE : word32)
  Class: Eq_299
  DataType: word32
  OrigDataType: word32
T_300: (in 0x00000008 : word32)
  Class: Eq_300
  DataType: word32
  OrigDataType: word32
T_301: (in fp - 0x00000008 : word32)
  Class: Eq_301
  DataType: word32
  OrigDataType: word32
T_302: (in Mem64[fp - 0x00000008:word32] : word32)
  Class: Eq_299
  DataType: word32
  OrigDataType: word32
T_303: (in 0x00000000 : ptr32)
  Class: Eq_303
  DataType: Eq_303
  OrigDataType: (union (ptr32 u0) ((memptr T_284 (struct (0 T_304 t0000))) u1))
T_304: (in Mem69[fs:0x00000000:word32] : word32)
  Class: Eq_283
  DataType: word32
  OrigDataType: word32
T_305: (in 0x00000000 : word32)
  Class: Eq_305
  DataType: word32
  OrigDataType: word32
T_306: (in dwArg04 : word32)
  Class: Eq_306
  DataType: word32
  OrigDataType: word32
T_307: (in 0x00400000 : ptr32)
  Class: Eq_307
  DataType: ptr32
  OrigDataType: ptr32
T_308: (in dwArg04 - 0x00400000 : word32)
  Class: Eq_308
  DataType: word32
  OrigDataType: word32
T_309: (in 0x00000030 : word32)
  Class: Eq_309
  DataType: word32
  OrigDataType: word32
T_310: (in fp - 0x00000030 : word32)
  Class: Eq_310
  DataType: word32
  OrigDataType: word32
T_311: (in Mem85[fp - 0x00000030:word32] : word32)
  Class: Eq_308
  DataType: word32
  OrigDataType: word32
T_312: (in eax_88 : (ptr Eq_312))
  Class: Eq_312
  DataType: (ptr Eq_312)
  OrigDataType: (ptr (struct (24 T_325 t0024)))
T_313: (in fn00401420 : ptr32)
  Class: Eq_313
  DataType: (ptr Eq_313)
  OrigDataType: (ptr (fn T_319 (T_317, T_318)))
T_314: (in signature of fn00401420 : void)
  Class: Eq_313
  DataType: (ptr Eq_313)
  OrigDataType: 
T_315: (in dwArg04 : word32)
  Class: Eq_315
  DataType: word32
  OrigDataType: word32
T_316: (in dwArg08 : word32)
  Class: Eq_316
  DataType: word32
  OrigDataType: word32
T_317: (in 0x00400000 : word32)
  Class: Eq_315
  DataType: word32
  OrigDataType: word32
T_318: (in 0x00400000 : word32)
  Class: Eq_316
  DataType: word32
  OrigDataType: word32
T_319: (in fn00401420(0x00400000, 0x00400000) : word32)
  Class: Eq_312
  DataType: (ptr Eq_312)
  OrigDataType: word32
T_320: (in 0x00000000 : word32)
  Class: Eq_312
  DataType: (ptr Eq_312)
  OrigDataType: word32
T_321: (in eax_88 == null : bool)
  Class: Eq_321
  DataType: bool
  OrigDataType: bool
T_322: (in eax_95 : word32)
  Class: Eq_322
  DataType: word32
  OrigDataType: word32
T_323: (in 0x00000024 : word32)
  Class: Eq_323
  DataType: word32
  OrigDataType: word32
T_324: (in eax_88 + 0x00000024 : word32)
  Class: Eq_324
  DataType: word32
  OrigDataType: word32
T_325: (in Mem85[eax_88 + 0x00000024:word32] : word32)
  Class: Eq_325
  DataType: uint32
  OrigDataType: uint32
T_326: (in 0x0000001F : word32)
  Class: Eq_326
  DataType: uint32
  OrigDataType: uint32
T_327: (in eax_88->dw0024 >>u 0x0000001F : word32)
  Class: Eq_327
  DataType: uint32
  OrigDataType: uint32
T_328: (in ~(eax_88->dw0024 >>u 0x0000001F) : word32)
  Class: Eq_322
  DataType: word32
  OrigDataType: word32
T_329: (in 0xFFFFFFFE : word32)
  Class: Eq_299
  DataType: word32
  OrigDataType: word32
T_330: (in 0x00000008 : word32)
  Class: Eq_330
  DataType: word32
  OrigDataType: word32
T_331: (in fp - 0x00000008 : word32)
  Class: Eq_331
  DataType: word32
  OrigDataType: word32
T_332: (in Mem99[fp - 0x00000008:word32] : word32)
  Class: Eq_299
  DataType: word32
  OrigDataType: word32
T_333: (in 0x00000000 : ptr32)
  Class: Eq_333
  DataType: Eq_333
  OrigDataType: (union (ptr32 u0) ((memptr T_284 (struct (0 T_334 t0000))) u1))
T_334: (in Mem101[fs:0x00000000:word32] : word32)
  Class: Eq_283
  DataType: word32
  OrigDataType: word32
T_335: (in 0x00000001 : word32)
  Class: Eq_335
  DataType: word32
  OrigDataType: word32
T_336: (in eax_95 & 0x00000001 : word32)
  Class: Eq_336
  DataType: word32
  OrigDataType: word32
T_337: (in eax : word32)
  Class: Eq_337
  DataType: word32
  OrigDataType: word32
T_338: (in dwArg04 : word32)
  Class: Eq_338
  DataType: (ptr Eq_338)
  OrigDataType: (ptr (struct (0 T_341 t0000) (3C T_347 t003C)))
T_339: (in 0x00000000 : word32)
  Class: Eq_339
  DataType: word32
  OrigDataType: word32
T_340: (in dwArg04 + 0x00000000 : word32)
  Class: Eq_340
  DataType: word32
  OrigDataType: word32
T_341: (in Mem0[dwArg04 + 0x00000000:word16] : word16)
  Class: Eq_341
  DataType: word16
  OrigDataType: word16
T_342: (in 0x5A4D : word16)
  Class: Eq_341
  DataType: word16
  OrigDataType: word16
T_343: (in dwArg04->w0000 == 0x5A4D : bool)
  Class: Eq_343
  DataType: bool
  OrigDataType: bool
T_344: (in ecx_35 : (ptr Eq_344))
  Class: Eq_344
  DataType: (ptr Eq_344)
  OrigDataType: (ptr (struct (0 T_353 t0000) (18 T_359 t0018)))
T_345: (in 0x0000003C : word32)
  Class: Eq_345
  DataType: word32
  OrigDataType: word32
T_346: (in dwArg04 + 0x0000003C : word32)
  Class: Eq_346
  DataType: word32
  OrigDataType: word32
T_347: (in Mem0[dwArg04 + 0x0000003C:word32] : word32)
  Class: Eq_347
  DataType: word32
  OrigDataType: word32
T_348: (in dwArg04->dw003C + dwArg04 : word32)
  Class: Eq_344
  DataType: (ptr Eq_344)
  OrigDataType: word32
T_349: (in eax_37 : word32)
  Class: Eq_349
  DataType: word32
  OrigDataType: word32
T_350: (in 0x00000000 : word32)
  Class: Eq_349
  DataType: word32
  OrigDataType: word32
T_351: (in 0x00000000 : word32)
  Class: Eq_351
  DataType: word32
  OrigDataType: word32
T_352: (in ecx_35 + 0x00000000 : word32)
  Class: Eq_352
  DataType: word32
  OrigDataType: word32
T_353: (in Mem0[ecx_35 + 0x00000000:word32] : word32)
  Class: Eq_353
  DataType: word32
  OrigDataType: word32
T_354: (in 0x00004550 : word32)
  Class: Eq_353
  DataType: word32
  OrigDataType: word32
T_355: (in ecx_35->dw0000 != 0x00004550 : bool)
  Class: Eq_355
  DataType: bool
  OrigDataType: bool
T_356: (in 0x00000000 : word32)
  Class: Eq_356
  DataType: word32
  OrigDataType: word32
T_357: (in 0x00000018 : word32)
  Class: Eq_357
  DataType: word32
  OrigDataType: word32
T_358: (in ecx_35 + 0x00000018 : word32)
  Class: Eq_358
  DataType: word32
  OrigDataType: word32
T_359: (in Mem0[ecx_35 + 0x00000018:word16] : word16)
  Class: Eq_359
  DataType: word16
  OrigDataType: word16
T_360: (in 0x010B : word16)
  Class: Eq_359
  DataType: word16
  OrigDataType: word16
T_361: (in ecx_35->w0018 == 0x010B : bool)
  Class: Eq_361
  DataType: bool
  OrigDataType: bool
T_362: (in (word32) (ecx_35->w0018 == 0x010B) : word32)
  Class: Eq_349
  DataType: word32
  OrigDataType: word32
T_363: (in eax_16 : word32)
  Class: Eq_363
  DataType: word32
  OrigDataType: word32
T_364: (in 0x00403000 : ptr32)
  Class: Eq_364
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_365 t0000)))
T_365: (in Mem0[0x00403000:word32] : word32)
  Class: Eq_363
  DataType: word32
  OrigDataType: word32
T_366: (in 0xBB40E64E : word32)
  Class: Eq_363
  DataType: word32
  OrigDataType: word32
T_367: (in eax_16 == 0xBB40E64E : bool)
  Class: Eq_367
  DataType: bool
  OrigDataType: bool
T_368: (in GetSystemTimeAsFileTime : ptr32)
  Class: Eq_368
  DataType: (ptr Eq_368)
  OrigDataType: (ptr (fn T_374 (T_373)))
T_369: (in signature of GetSystemTimeAsFileTime : void)
  Class: Eq_368
  DataType: (ptr Eq_368)
  OrigDataType: 
T_370: (in lpSystemTimeAsFileTime : LPFILETIME)
  Class: Eq_370
  DataType: Eq_370
  OrigDataType: LPFILETIME
T_371: (in fp : ptr32)
  Class: Eq_371
  DataType: ptr32
  OrigDataType: ptr32
T_372: (in 0x00000010 : word32)
  Class: Eq_372
  DataType: word32
  OrigDataType: word32
T_373: (in fp - 0x00000010 : word32)
  Class: Eq_370
  DataType: Eq_370
  OrigDataType: word32
T_374: (in GetSystemTimeAsFileTime(fp - 0x00000010) : void)
  Class: Eq_374
  DataType: void
  OrigDataType: void
T_375: (in v14_55 : word32)
  Class: Eq_375
  DataType: word32
  OrigDataType: word32
T_376: (in dwLoc0C : word32)
  Class: Eq_376
  DataType: word32
  OrigDataType: word32
T_377: (in 0x00000000 : word32)
  Class: Eq_377
  DataType: word32
  OrigDataType: word32
T_378: (in dwLoc0C & 0x00000000 : word32)
  Class: Eq_378
  DataType: word32
  OrigDataType: word32
T_379: (in dwLoc10 : word32)
  Class: Eq_379
  DataType: word32
  OrigDataType: word32
T_380: (in 0x00000000 : word32)
  Class: Eq_380
  DataType: word32
  OrigDataType: word32
T_381: (in dwLoc10 & 0x00000000 : word32)
  Class: Eq_381
  DataType: word32
  OrigDataType: word32
T_382: (in dwLoc0C & 0x00000000 ^ dwLoc10 & 0x00000000 : word32)
  Class: Eq_382
  DataType: word32
  OrigDataType: word32
T_383: (in GetCurrentThreadId : ptr32)
  Class: Eq_383
  DataType: (ptr Eq_383)
  OrigDataType: (ptr (fn T_385 ()))
T_384: (in signature of GetCurrentThreadId : void)
  Class: Eq_383
  DataType: (ptr Eq_383)
  OrigDataType: 
T_385: (in GetCurrentThreadId() : DWORD)
  Class: Eq_385
  DataType: Eq_385
  OrigDataType: DWORD
T_386: (in dwLoc0C & 0x00000000 ^ dwLoc10 & 0x00000000 ^ GetCurrentThreadId() : word32)
  Class: Eq_386
  DataType: word32
  OrigDataType: word32
T_387: (in GetCurrentProcessId : ptr32)
  Class: Eq_387
  DataType: (ptr Eq_387)
  OrigDataType: (ptr (fn T_389 ()))
T_388: (in signature of GetCurrentProcessId : void)
  Class: Eq_387
  DataType: (ptr Eq_387)
  OrigDataType: 
T_389: (in GetCurrentProcessId() : DWORD)
  Class: Eq_389
  DataType: Eq_389
  OrigDataType: DWORD
T_390: (in dwLoc0C & 0x00000000 ^ dwLoc10 & 0x00000000 ^ GetCurrentThreadId() ^ GetCurrentProcessId() : word32)
  Class: Eq_375
  DataType: word32
  OrigDataType: word32
T_391: (in QueryPerformanceCounter : ptr32)
  Class: Eq_391
  DataType: (ptr Eq_391)
  OrigDataType: (ptr (fn T_396 (T_395)))
T_392: (in signature of QueryPerformanceCounter : void)
  Class: Eq_391
  DataType: (ptr Eq_391)
  OrigDataType: 
T_393: (in lpPerformanceCount : (ptr LARGE_INTEGER))
  Class: Eq_393
  DataType: (ptr Eq_393)
  OrigDataType: (ptr LARGE_INTEGER)
T_394: (in 0x00000018 : word32)
  Class: Eq_394
  DataType: word32
  OrigDataType: word32
T_395: (in fp - 0x00000018 : word32)
  Class: Eq_393
  DataType: (ptr Eq_393)
  OrigDataType: word32
T_396: (in QueryPerformanceCounter(fp - 0x00000018) : BOOL)
  Class: Eq_396
  DataType: Eq_396
  OrigDataType: BOOL
T_397: (in ecx_69 : word32)
  Class: Eq_363
  DataType: word32
  OrigDataType: word32
T_398: (in dwLoc14 : word32)
  Class: Eq_398
  DataType: word32
  OrigDataType: word32
T_399: (in dwLoc18 : word32)
  Class: Eq_399
  DataType: word32
  OrigDataType: word32
T_400: (in dwLoc14 ^ dwLoc18 : word32)
  Class: Eq_400
  DataType: word32
  OrigDataType: word32
T_401: (in dwLoc14 ^ dwLoc18 ^ v14_55 : word32)
  Class: Eq_401
  DataType: word32
  OrigDataType: word32
T_402: (in 0x00000008 : word32)
  Class: Eq_402
  DataType: word32
  OrigDataType: word32
T_403: (in fp - 0x00000008 : word32)
  Class: Eq_403
  DataType: word32
  OrigDataType: word32
T_404: (in dwLoc14 ^ dwLoc18 ^ v14_55 ^ fp - 0x00000008 : word32)
  Class: Eq_363
  DataType: word32
  OrigDataType: word32
T_405: (in 0xBB40E64E : word32)
  Class: Eq_363
  DataType: word32
  OrigDataType: word32
T_406: (in ecx_69 != 0xBB40E64E : bool)
  Class: Eq_406
  DataType: bool
  OrigDataType: bool
T_407: (in 0xFFFF0000 : word32)
  Class: Eq_407
  DataType: word32
  OrigDataType: word32
T_408: (in eax_16 & 0xFFFF0000 : word32)
  Class: Eq_408
  DataType: word32
  OrigDataType: word32
T_409: (in 0x00000000 : word32)
  Class: Eq_408
  DataType: word32
  OrigDataType: word32
T_410: (in (eax_16 & 0xFFFF0000) == 0x00000000 : bool)
  Class: Eq_410
  DataType: bool
  OrigDataType: bool
T_411: (in ~eax_16 : word32)
  Class: Eq_411
  DataType: word32
  OrigDataType: word32
T_412: (in 0x00403004 : ptr32)
  Class: Eq_412
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_413 t0000)))
T_413: (in Mem94[0x00403004:word32] : word32)
  Class: Eq_411
  DataType: word32
  OrigDataType: word32
T_414: (in 0xFFFF0000 : word32)
  Class: Eq_414
  DataType: word32
  OrigDataType: word32
T_415: (in ecx_69 & 0xFFFF0000 : word32)
  Class: Eq_415
  DataType: word32
  OrigDataType: word32
T_416: (in 0x00000000 : word32)
  Class: Eq_415
  DataType: word32
  OrigDataType: word32
T_417: (in (ecx_69 & 0xFFFF0000) != 0x00000000 : bool)
  Class: Eq_417
  DataType: bool
  OrigDataType: bool
T_418: (in 0xBB40E64F : word32)
  Class: Eq_363
  DataType: word32
  OrigDataType: word32
T_419: (in 0x00403000 : ptr32)
  Class: Eq_419
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_420 t0000)))
T_420: (in Mem77[0x00403000:word32] : word32)
  Class: Eq_363
  DataType: word32
  OrigDataType: word32
T_421: (in ~ecx_69 : word32)
  Class: Eq_411
  DataType: word32
  OrigDataType: word32
T_422: (in 0x00403004 : ptr32)
  Class: Eq_422
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_423 t0000)))
T_423: (in Mem79[0x00403004:word32] : word32)
  Class: Eq_411
  DataType: word32
  OrigDataType: word32
T_424: (in 0x00004711 : word32)
  Class: Eq_424
  DataType: word32
  OrigDataType: word32
T_425: (in ecx_69 | 0x00004711 : word32)
  Class: Eq_425
  DataType: word32
  OrigDataType: word32
T_426: (in 0x00000010 : word32)
  Class: Eq_426
  DataType: word32
  OrigDataType: word32
T_427: (in (ecx_69 | 0x00004711) << 0x00000010 : word32)
  Class: Eq_427
  DataType: ui32
  OrigDataType: ui32
T_428: (in ecx_69 | (ecx_69 | 0x00004711) << 0x00000010 : word32)
  Class: Eq_363
  DataType: word32
  OrigDataType: word32
T_429: (in ebp : word32)
  Class: Eq_429
  DataType: word32
  OrigDataType: word32
T_430: (in esp_13 : (ptr Eq_430))
  Class: Eq_430
  DataType: (ptr Eq_430)
  OrigDataType: (ptr (struct (FFFFFFEC T_456 tFFFFFFEC) (FFFFFFF0 T_452 tFFFFFFF0) (FFFFFFF4 T_444 tFFFFFFF4) (FFFFFFF8 T_441 tFFFFFFF8) (FFFFFFFC T_438 tFFFFFFFC)))
T_431: (in fp : ptr32)
  Class: Eq_431
  DataType: ptr32
  OrigDataType: ptr32
T_432: (in 0x00000008 : word32)
  Class: Eq_432
  DataType: word32
  OrigDataType: word32
T_433: (in fp - 0x00000008 : word32)
  Class: Eq_433
  DataType: word32
  OrigDataType: word32
T_434: (in dwArg08 : word32)
  Class: Eq_434
  DataType: word32
  OrigDataType: word32
T_435: (in fp - 0x00000008 - dwArg08 : word32)
  Class: Eq_430
  DataType: (ptr Eq_430)
  OrigDataType: word32
T_436: (in 0x00000004 : word32)
  Class: Eq_436
  DataType: word32
  OrigDataType: word32
T_437: (in esp_13 - 0x00000004 : word32)
  Class: Eq_437
  DataType: word32
  OrigDataType: word32
T_438: (in Mem16[esp_13 - 0x00000004:word32] : word32)
  Class: Eq_8
  DataType: word32
  OrigDataType: word32
T_439: (in 0x00000008 : word32)
  Class: Eq_439
  DataType: word32
  OrigDataType: word32
T_440: (in esp_13 - 0x00000008 : word32)
  Class: Eq_440
  DataType: word32
  OrigDataType: word32
T_441: (in Mem19[esp_13 - 0x00000008:word32] : word32)
  Class: Eq_9
  DataType: word32
  OrigDataType: word32
T_442: (in 0x0000000C : word32)
  Class: Eq_442
  DataType: word32
  OrigDataType: word32
T_443: (in esp_13 - 0x0000000C : word32)
  Class: Eq_443
  DataType: word32
  OrigDataType: word32
T_444: (in Mem22[esp_13 - 0x0000000C:word32] : word32)
  Class: Eq_10
  DataType: word32
  OrigDataType: word32
T_445: (in 0x00403000 : ptr32)
  Class: Eq_445
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 T_446 t0000)))
T_446: (in Mem22[0x00403000:word32] : word32)
  Class: Eq_363
  DataType: word32
  OrigDataType: word32
T_447: (in 0x00000008 : word32)
  Class: Eq_447
  DataType: word32
  OrigDataType: word32
T_448: (in fp + 0x00000008 : word32)
  Class: Eq_448
  DataType: word32
  OrigDataType: word32
T_449: (in globals->dw403000 ^ fp + 0x00000008 : word32)
  Class: Eq_449
  DataType: word32
  OrigDataType: word32
T_450: (in 0x00000010 : word32)
  Class: Eq_450
  DataType: word32
  OrigDataType: word32
T_451: (in esp_13 - 0x00000010 : word32)
  Class: Eq_451
  DataType: word32
  OrigDataType: word32
T_452: (in Mem32[esp_13 - 0x00000010:word32] : word32)
  Class: Eq_449
  DataType: word32
  OrigDataType: word32
T_453: (in dwArg00 : word32)
  Class: Eq_453
  DataType: word32
  OrigDataType: word32
T_454: (in 0x00000014 : word32)
  Class: Eq_454
  DataType: word32
  OrigDataType: word32
T_455: (in esp_13 - 0x00000014 : word32)
  Class: Eq_455
  DataType: word32
  OrigDataType: word32
T_456: (in Mem36[esp_13 - 0x00000014:word32] : word32)
  Class: Eq_453
  DataType: word32
  OrigDataType: word32
T_457: (in 0x00000008 : word32)
  Class: Eq_457
  DataType: word32
  OrigDataType: word32
T_458: (in fp - 0x00000008 : word32)
  Class: Eq_458
  DataType: word32
  OrigDataType: word32
T_459: (in fs : selector)
  Class: Eq_459
  DataType: (ptr Eq_459)
  OrigDataType: (ptr (segment (0 T_461 t0000)))
T_460: (in 0x00000000 : word32)
  Class: Eq_460
  DataType: (memptr (ptr Eq_459) word32)
  OrigDataType: (memptr T_459 (struct (0 T_461 t0000)))
T_461: (in Mem41[fs:0x00000000:word32] : word32)
  Class: Eq_458
  DataType: word32
  OrigDataType: word32
T_462: (in fp + 0x00000008 : word32)
  Class: Eq_462
  DataType: word32
  OrigDataType: word32
T_463: (in 0x00000010 : word32)
  Class: Eq_463
  DataType: word32
  OrigDataType: word32
T_464: (in ebp - 0x00000010 : word32)
  Class: Eq_464
  DataType: word32
  OrigDataType: word32
T_465: (in Mem0[ebp - 0x00000010:word32] : word32)
  Class: Eq_465
  DataType: word32
  OrigDataType: word32
T_466: (in fs : selector)
  Class: Eq_466
  DataType: (ptr Eq_466)
  OrigDataType: (ptr (segment (0 T_468 t0000)))
T_467: (in 0x00000000 : ptr32)
  Class: Eq_467
  DataType: Eq_467
  OrigDataType: (union (ptr32 u0) ((memptr T_466 (struct (0 T_468 t0000))) u1))
T_468: (in Mem5[fs:0x00000000:word32] : word32)
  Class: Eq_465
  DataType: word32
  OrigDataType: word32
T_469: (in dwArg00 : word32)
  Class: Eq_469
  DataType: word32
  OrigDataType: word32
T_470: (in 0x00000000 : word32)
  Class: Eq_470
  DataType: word32
  OrigDataType: word32
T_471: (in ebp + 0x00000000 : word32)
  Class: Eq_471
  DataType: word32
  OrigDataType: word32
T_472: (in Mem25[ebp + 0x00000000:word32] : word32)
  Class: Eq_469
  DataType: word32
  OrigDataType: word32
T_473:
  Class: Eq_473
  DataType: Eq_473
  OrigDataType: 
T_474:
  Class: Eq_474
  DataType: Eq_474
  OrigDataType: 
*/
typedef code Eq_1struct Globals {
	word32 dw403000;	// 403000
	word32 dw403004;	// 403004
	word32 dw403018;	// 403018
	word32 dw40301C;	// 40301C
	word32 dw403020;	// 403020
	word32 dw403024;	// 403024
	word32 * * ptr403028;	// 403028
	int32 dw40302C;	// 40302C
	word32 dw403368;	// 403368
	word32 dw40336C;	// 40336C
	code * ptr403378;	// 403378
} Eq_1;

typedef void (Eq_2)();

typedef struct Eq_5 {
	word32 dwFFFFFFE4;	// FFFFFFE4
	word32 * * ptrFFFFFFEC;	// FFFFFFEC
	word32 dwFFFFFFF0;	// FFFFFFF0
	word32 dwFFFFFFFC;	// FFFFFFFC
	word32 dw0000;	// 0
} Eq_5;

typedef Eq_5 * (Eq_6)(word32, word32, word32, word32, word32, word32);

typedef struct Eq_25 {
	word32 dwFFFFFFFC;	// FFFFFFFC
	word32 dw0000;	// 0
} Eq_25;

typedef Eq_473 Eq_26struct Eq_26 {
	word32 dwFFFFFFF0;	// FFFFFFF0
	Eq_473 uFFFFFFF4;	// FFFFFFF4
} Eq_26;

typedef Eq_32 Eq_30struct Eq_30 {
	Eq_32 * ptr0018;	// 18
} Eq_30;

typedef struct Eq_32 {
	word32 dw0004;	// 4
} Eq_32;

typedef void (Eq_40)();

typedef bool (Eq_42)(word32, word32, word32, word32 *);

typedef void (Eq_95)(Eq_5 *, word32, word32, word32, word32, word32);

typedef struct Eq_139 {
	word32 dwFFFFFFF8;	// FFFFFFF8
	word32 * * ptrFFFFFFFC;	// FFFFFFFC
	int32 dw0000;	// 0
} Eq_139;

typedef void (Eq_157)();

typedef Eq_25 Eq_167struct Eq_167 {
	word32 dwFFFFFFF8;	// FFFFFFF8
	word32 dwFFFFFFFC;	// FFFFFFFC
	word32 dw0000;	// 0
	Eq_25 t0004;	// 4
} Eq_167;

typedef word32 (Eq_175)(word32);

typedef void (Eq_206)(int32);

typedef struct Eq_240 {
	word16 w0006;	// 6
	word16 w0014;	// 14
} Eq_240;

typedef struct Eq_241 {
	word32 dw003C;	// 3C
} Eq_241;

typedef Eq_253 Eq_253struct Eq_253 {	// size: 40 28
	up32 dw0000;	// 0
	word32 dw0008;	// 8
	Eq_253 t0028;	// 28
} Eq_253;

typedef struct Eq_284 {
	word32 dw0000;	// 0
} Eq_284;

typedef struct Eq_287 {
	word32 dwFFFFFFD0;	// FFFFFFD0
	word32 dwFFFFFFF8;	// FFFFFFF8
} Eq_287;

typedef word32 (Eq_292)(word32);

typedef union Eq_303 {
	ptr32 u0;
	word32 Eq_284::* u1;
} Eq_303;

typedef struct Eq_312 {
	uint32 dw0024;	// 24
} Eq_312;

typedef Eq_312 * (Eq_313)(word32, word32);

typedef union Eq_333 {
	ptr32 u0;
	word32 Eq_284::* u1;
} Eq_333;

typedef struct Eq_338 {
	word16 w0000;	// 0
	word32 dw003C;	// 3C
} Eq_338;

typedef struct Eq_344 {
	word32 dw0000;	// 0
	word16 w0018;	// 18
} Eq_344;

typedef void (Eq_368)(LPFILETIME);

typedef LPFILETIME;

typedef DWORD (Eq_383)();

typedef DWORD;

typedef DWORD (Eq_387)();

typedef DWORD;

typedef BOOL (Eq_391)(LARGE_INTEGER *);

typedef LARGE_INTEGER;

typedef BOOL;

typedef struct Eq_430 {
	word32 dwFFFFFFEC;	// FFFFFFEC
	word32 dwFFFFFFF0;	// FFFFFFF0
	word32 dwFFFFFFF4;	// FFFFFFF4
	word32 dwFFFFFFF8;	// FFFFFFF8
	word32 dwFFFFFFFC;	// FFFFFFFC
} Eq_430;

typedef struct Eq_459 {
	word32 dw0000;	// 0
} Eq_459;

typedef struct Eq_466 {
	word32 dw0000;	// 0
} Eq_466;

typedef union Eq_467 {
	ptr32 u0;
	word32 Eq_466::* u1;
} Eq_467;

typedef union Eq_473 {
	word32 u0;
	Eq_25 u1;
} Eq_473;

typedef DWORDLONGstruct struct_1 {
	DWORD;	// 0
	LONG;	// 4
} Eq_474;

