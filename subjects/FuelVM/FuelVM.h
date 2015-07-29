// FuelVM.h
// Generated on 2015-07-25 18:17:37 by decompiling D:\dev\jkl\dec\bin\FuelVM\FuelVM.exe
// using Decompiler version 0.4.5.0.

/*
// Equivalence classes ////////////
Eq_1: (struct "Globals" (40102C (ptr code) ptr40102C) (403180 Eq_6 t403180) (403184 Eq_6 t403184) (403188 Eq_6 t403188) (40318C Eq_6 t40318C) (403190 Eq_6 t403190) (403194 Eq_6 t403194) (403198 (arr Eq_456) a403198) (4031C8 word32 dw4031C8) (4031CC byte b4031CC) (4031CD Eq_399 t4031CD) (4031CE Eq_399 t4031CE) (4035CF word32 dw4035CF) (4035D7 word32 dw4035D7) (4035DF word32 dw4035DF) (4035E7 word32 dw4035E7) (4035EF word32 dw4035EF) (4035FF word32 dw4035FF) (40360F byte b40360F) (403610 byte b403610))
	globals_t (in globals : (ptr (struct "Globals")))
Eq_2: (fn Eq_6 (Eq_4))
	T_2 (in GetModuleHandleA : ptr32)
	T_3 (in signature of GetModuleHandleA : void)
Eq_4: LPCSTR
	T_4 (in lpModuleName : LPCSTR)
	T_5 (in 0x00000000 : word32)
Eq_6: (union "Eq_6" (DWORD u0) (HBITMAP u1) (HBRUSH u2) (HGLOBAL u3) (HINSTANCE u4) (HMODULE u5) (HRGN u6) (HRSRC u7) (HWND u8) (LPVOID u9))
	T_6 (in GetModuleHandleA(null) : HMODULE)
	T_8 (in Mem6[0x00403180:word32] : word32)
	T_11 (in hInstance : HINSTANCE)
	T_17 (in Mem6[0x00403180:word32] : word32)
	T_33 (in 0x00000053 : word32)
	T_37 (in Mem53[fp - 0x00000008:word32] : word32)
	T_38 (in 0x00000000 : word32)
	T_41 (in Mem55[fp - 0x0000000C:word32] : word32)
	T_42 (in 0x00000000 : word32)
	T_45 (in Mem57[fp - 0x00000010:word32] : word32)
	T_48 (in hWnd : HWND)
	T_55 (in dwArg04 : word32)
	T_63 (in 0x000007D1 : word32)
	T_66 (in Mem70[fp - 0x00000008:word32] : word32)
	T_68 (in Mem70[0x00403180:word32] : word32)
	T_71 (in Mem72[fp - 0x0000000C:word32] : word32)
	T_76 (in LoadBitmapA(0x00000000, 0x0040102C) : HBITMAP)
	T_79 (in Mem76[fp - 0x00000008:word32] : word32)
	T_83 (in CreatePatternBrush(0x0040102C) : HBRUSH)
	T_85 (in Mem79[0x00403194:word32] : word32)
	T_86 (in 0x0000000A : word32)
	T_89 (in Mem81[fp - 0x00000008:word32] : word32)
	T_90 (in 0x000007D2 : word32)
	T_93 (in Mem83[fp - 0x0000000C:word32] : word32)
	T_95 (in Mem83[0x00403180:word32] : word32)
	T_98 (in Mem85[fp - 0x00000010:word32] : word32)
	T_104 (in FindResourceA(() 0x000003EC, 0x00000000, 0x0040102C) : HRSRC)
	T_106 (in Mem88[0x00403184:word32] : word32)
	T_108 (in Mem88[0x00403184:word32] : word32)
	T_111 (in Mem90[fp - 0x00000008:word32] : word32)
	T_113 (in Mem90[0x00403180:word32] : word32)
	T_116 (in Mem92[fp - 0x0000000C:word32] : word32)
	T_121 (in LoadResource(0x00000000, 0x0040102C) : HGLOBAL)
	T_123 (in Mem95[0x00403188:word32] : word32)
	T_125 (in Mem95[0x00403184:word32] : word32)
	T_128 (in Mem97[fp - 0x00000008:word32] : word32)
	T_130 (in Mem97[0x00403180:word32] : word32)
	T_133 (in Mem99[fp - 0x0000000C:word32] : word32)
	T_138 (in SizeofResource(0x00000000, 0x0040102C) : DWORD)
	T_140 (in Mem102[0x00403190:word32] : word32)
	T_142 (in Mem102[0x00403188:word32] : word32)
	T_145 (in Mem104[fp - 0x00000008:word32] : word32)
	T_149 (in LockResource(0x0040102C) : LPVOID)
	T_151 (in Mem107[0x0040318C:word32] : word32)
	T_153 (in Mem107[0x0040318C:word32] : word32)
	T_156 (in Mem109[fp - 0x00000008:word32] : word32)
	T_158 (in Mem109[0x00403190:word32] : word32)
	T_161 (in Mem111[fp - 0x0000000C:word32] : word32)
	T_162 (in 0x00000000 : word32)
	T_165 (in Mem113[fp - 0x00000010:word32] : word32)
	T_166 (in eax_114 : Eq_6)
	T_172 (in ExtCreateRegion(() 0x000003EC, 0x00000000, 0x0040102C) : HRGN)
	T_173 (in 0x00000001 : word32)
	T_176 (in Mem117[fp - 0x00000008:word32] : word32)
	T_179 (in Mem119[fp - 0x0000000C:word32] : word32)
	T_182 (in Mem121[fp - 0x00000010:word32] : word32)
	T_189 (in 0x00000001 : word32)
	T_192 (in Mem125[fp - 0x00000008:word32] : word32)
	T_195 (in Mem127[fp - 0x0000000C:word32] : word32)
	T_205 (in 0x00000001 : word32)
	T_208 (in Mem144[fp - 0x00000008:word32] : word32)
	T_209 (in 0x00000000 : word32)
	T_212 (in Mem146[fp - 0x0000000C:word32] : word32)
	T_215 (in Mem148[fp - 0x00000010:word32] : word32)
	T_224 (in Mem152[fp - 0x00000008:word32] : word32)
	T_229 (in 0x00000001 : word32)
	T_232 (in Mem156[fp - 0x00000008:word32] : word32)
	T_235 (in Mem158[fp - 0x0000000C:word32] : word32)
	T_245 (in 0x0000000C : word32)
	T_248 (in Mem170[fp - 0x00000008:word32] : word32)
	T_249 (in 0x00403198 : word32)
	T_252 (in Mem172[fp - 0x0000000C:word32] : word32)
	T_253 (in 0x000003F8 : word32)
	T_256 (in Mem174[fp - 0x00000010:word32] : word32)
	T_259 (in hDlg : HWND)
	T_264 (in 0x0000000C : word32)
	T_267 (in Mem180[fp - 0x00000008:word32] : word32)
	T_268 (in 0x004031B0 : word32)
	T_271 (in Mem182[fp - 0x0000000C:word32] : word32)
	T_272 (in 0x000003F9 : word32)
	T_275 (in Mem184[fp - 0x00000010:word32] : word32)
	T_286 (in 0x00090000 : word32)
	T_289 (in Mem206[fp - 0x00000008:word32] : word32)
	T_290 (in 0x000003E8 : word32)
	T_293 (in Mem208[fp - 0x0000000C:word32] : word32)
	T_296 (in Mem210[fp - 0x00000010:word32] : word32)
	T_304 (in Mem210[0x00403194:word32] : word32)
	T_307 (in Mem214[fp - 0x00000008:word32] : word32)
	T_312 (in 0x00000000 : word32)
	T_315 (in Mem218[fp - 0x00000008:word32] : word32)
	T_318 (in Mem220[fp - 0x0000000C:word32] : word32)
	T_324 (in 0x00000000 : word32)
	T_327 (in Mem194[fp - 0x00000008:word32] : word32)
	T_328 (in 0x00000000 : word32)
	T_331 (in Mem196[fp - 0x0000000C:word32] : word32)
	T_332 (in 0x00000010 : word32)
	T_335 (in Mem198[fp - 0x00000010:word32] : word32)
	T_338 (in hWnd : HWND)
Eq_9: (fn Eq_22 (Eq_6, Eq_12, Eq_13, Eq_14, Eq_15))
	T_9 (in DialogBoxParamA : ptr32)
	T_10 (in signature of DialogBoxParamA : void)
Eq_12: LPCSTR
	T_12 (in lpTemplateName : LPCSTR)
	T_18 (in 0x000003EC : word32)
Eq_13: HWND
	T_13 (in hWndParent : HWND)
	T_19 (in 0x00000000 : word32)
Eq_14: DLGPROC
	T_14 (in lpDialogFunc : DLGPROC)
	T_20 (in 0x0040102C : word32)
Eq_15: LPARAM
	T_15 (in dwInitParam : LPARAM)
	T_21 (in 0x00000000 : word32)
Eq_22: INT_PTR
	T_22 (in DialogBoxParamA(globals->t403180, (CHAR) 0x000003EC, null, &globals->ptr40102C, 0x00000000) : INT_PTR)
Eq_23: (fn void (Eq_25))
	T_23 (in ExitProcess : ptr32)
	T_24 (in signature of ExitProcess : void)
Eq_25: UINT
	T_25 (in uExitCode : UINT)
	T_26 (in 0x00000000 : word32)
Eq_34: (struct "Eq_34" (FFFFFFF0 Eq_6 tFFFFFFF0) (FFFFFFF4 Eq_6 tFFFFFFF4) (FFFFFFF8 Eq_6 tFFFFFFF8))
	T_34 (in fp : ptr32)
Eq_46: (fn Eq_62 (Eq_6, Eq_49, int32, int32, Eq_52, Eq_53, Eq_54))
	T_46 (in SetWindowPos : ptr32)
	T_47 (in signature of SetWindowPos : void)
Eq_49: HWND
	T_49 (in hWndInsertAfter : HWND)
	T_56 (in 0x00000000 : word32)
Eq_52: (union "Eq_52" (HMODULE u0) (HWND u1) (UINT u2))
	T_52 (in cx : int32)
	T_59 (in 0x000003EC : word32)
	T_101 (in hModule : HMODULE)
	T_169 (in lpx : (ptr XFORM))
	T_185 (in hWnd : HWND)
	T_218 (in hWnd : HWND)
	T_260 (in nIDDlgItem : int32)
	T_299 (in hWnd : HWND)
	T_339 (in Msg : UINT)
Eq_53: (union "Eq_53" (DWORD u0) (HINSTANCE u1) (HMODULE u2) (HRGN u3) (HWND u4) (LPCSTR u5) (LPSTR u6) (WPARAM u7))
	T_53 (in cy : int32)
	T_60 (in 0x00000000 : word32)
	T_74 (in hInstance : HINSTANCE)
	T_102 (in lpName : LPCSTR)
	T_119 (in hModule : HMODULE)
	T_136 (in hModule : HMODULE)
	T_170 (in nCount : DWORD)
	T_186 (in hRgn : HRGN)
	T_198 (in hWnd : HWND)
	T_219 (in lpRect : (ptr RECT))
	T_261 (in lpString : LPSTR)
	T_300 (in dwTime : DWORD)
	T_321 (in hDlg : HWND)
	T_340 (in wParam : WPARAM)
Eq_54: (union "Eq_54" (BOOL u0) (DWORD u1) (HBITMAP u2) (HGDIOBJ u3) (HGLOBAL u4) (HRSRC u5) (HWND u6) (INT_PTR u7) (LPARAM u8) (LPCSTR u9) (UINT u10))
	T_54 (in uFlags : UINT)
	T_61 (in 0x0040102C : word32)
	T_75 (in lpBitmapName : LPCSTR)
	T_82 (in hbm : HBITMAP)
	T_103 (in lpType : LPCSTR)
	T_120 (in hResInfo : HRSRC)
	T_137 (in hResInfo : HRSRC)
	T_148 (in hResData : HGLOBAL)
	T_171 (in lpData : (ptr RGNDATA))
	T_187 (in bRedraw : BOOL)
	T_199 (in nCmdShow : int32)
	T_220 (in bErase : BOOL)
	T_227 (in hWnd : HWND)
	T_262 (in cchMax : int32)
	T_301 (in dwFlags : DWORD)
	T_310 (in ho : HGDIOBJ)
	T_322 (in nResult : INT_PTR)
	T_341 (in lParam : LPARAM)
Eq_62: BOOL
	T_62 (in SetWindowPos(dwArg04, null, 0x00000000, 0x00000000, 0x000003EC, 0x00000000, 0x0040102C) : BOOL)
Eq_72: (fn Eq_6 (Eq_53, Eq_54))
	T_72 (in LoadBitmapA : ptr32)
	T_73 (in signature of LoadBitmapA : void)
Eq_80: (fn Eq_6 (Eq_54))
	T_80 (in CreatePatternBrush : ptr32)
	T_81 (in signature of CreatePatternBrush : void)
Eq_99: (fn Eq_6 (Eq_52, Eq_53, Eq_54))
	T_99 (in FindResourceA : ptr32)
	T_100 (in signature of FindResourceA : void)
Eq_117: (fn Eq_6 (Eq_53, Eq_54))
	T_117 (in LoadResource : ptr32)
	T_118 (in signature of LoadResource : void)
Eq_134: (fn Eq_6 (Eq_53, Eq_54))
	T_134 (in SizeofResource : ptr32)
	T_135 (in signature of SizeofResource : void)
Eq_146: (fn Eq_6 (Eq_54))
	T_146 (in LockResource : ptr32)
	T_147 (in signature of LockResource : void)
Eq_167: (fn Eq_6 (Eq_52, Eq_53, Eq_54))
	T_167 (in ExtCreateRegion : ptr32)
	T_168 (in signature of ExtCreateRegion : void)
Eq_183: (fn int32 (Eq_52, Eq_53, Eq_54))
	T_183 (in SetWindowRgn : ptr32)
	T_184 (in signature of SetWindowRgn : void)
Eq_196: (fn Eq_200 (Eq_53, Eq_54))
	T_196 (in ShowWindow : ptr32)
	T_197 (in signature of ShowWindow : void)
	T_236 (in ShowWindow : ptr32)
Eq_200: BOOL
	T_200 (in ShowWindow(0x00000000, 0x0040102C) : BOOL)
	T_237 (in ShowWindow(0x00000000, 0x0040102C) : BOOL)
Eq_216: (fn Eq_221 (Eq_52, Eq_53, Eq_54))
	T_216 (in InvalidateRect : ptr32)
	T_217 (in signature of InvalidateRect : void)
Eq_221: BOOL
	T_221 (in InvalidateRect(() 0x000003EC, 0x00000000, 0x0040102C) : BOOL)
Eq_225: (fn Eq_228 (Eq_54))
	T_225 (in UpdateWindow : ptr32)
	T_226 (in signature of UpdateWindow : void)
Eq_228: BOOL
	T_228 (in UpdateWindow(0x0040102C) : BOOL)
Eq_257: (fn Eq_263 (Eq_6, Eq_52, Eq_53, Eq_54))
	T_257 (in GetDlgItemTextA : ptr32)
	T_258 (in signature of GetDlgItemTextA : void)
	T_276 (in GetDlgItemTextA : ptr32)
Eq_263: UINT
	T_263 (in GetDlgItemTextA(dwArg04, () 0x000003EC, 0x00000000, 0x0040102C) : UINT)
	T_277 (in GetDlgItemTextA(dwArg04, () 0x000003EC, 0x00000000, 0x0040102C) : UINT)
Eq_281: (fn void (word32))
	T_281 (in fn00401238 : ptr32)
	T_282 (in signature of fn00401238 : void)
Eq_297: (fn Eq_302 (Eq_52, Eq_53, Eq_54))
	T_297 (in AnimateWindow : ptr32)
	T_298 (in signature of AnimateWindow : void)
Eq_302: BOOL
	T_302 (in AnimateWindow(() 0x000003EC, 0x00000000, 0x0040102C) : BOOL)
Eq_308: (fn Eq_311 (Eq_54))
	T_308 (in DeleteObject : ptr32)
	T_309 (in signature of DeleteObject : void)
Eq_311: BOOL
	T_311 (in DeleteObject(0x0040102C) : BOOL)
Eq_319: (fn Eq_323 (Eq_53, Eq_54))
	T_319 (in EndDialog : ptr32)
	T_320 (in signature of EndDialog : void)
Eq_323: BOOL
	T_323 (in EndDialog(0x00000000, 0x0040102C) : BOOL)
Eq_336: (fn Eq_342 (Eq_6, Eq_52, Eq_53, Eq_54))
	T_336 (in SendMessageA : ptr32)
	T_337 (in signature of SendMessageA : void)
Eq_342: LRESULT
	T_342 (in SendMessageA(dwArg04, () 0x000003EC, 0x00000000, 0x0040102C) : LRESULT)
Eq_344: (struct "Eq_344" (0 byte b0000) (1 Eq_344 t0001))
	T_344 (in edi_16 : (ptr Eq_344))
	T_345 (in dwArg04 : word32)
	T_352 (in edi_33 : (ptr Eq_344))
	T_354 (in edi_33 + 0x00000001 : word32)
Eq_375: (struct "Eq_375" (403198 byte b403198))
	T_375 (in (word32) globals->b4031CC : word32)
Eq_399: (union "Eq_399" (int32 u0) (up32 u1))
	T_399 (in ecx_10 : Eq_399)
	T_404 (in fn004011C2(0x00403198) : word32)
	T_405 (in 0x00000007 : word32)
	T_408 (in Mem29[0x004031CD:word32] : word32)
	T_416 (in fn004011C2(0x00403198) : word32)
	T_417 (in 0x00000007 : word32)
	T_422 (in ecx_39 : Eq_399)
	T_423 (in 0x00000000 : word32)
	T_433 (in ecx_39 + 0x00000001 : word32)
	T_435 (in Mem52[0x004031CD:word32] : word32)
	T_438 (in Mem61[0x004031CE:word32] : word32)
Eq_400: (fn Eq_399 (word32))
	T_400 (in fn004011C2 : ptr32)
	T_401 (in signature of fn004011C2 : void)
	T_414 (in fn004011C2 : ptr32)
Eq_410: (struct "Eq_410" (FFFFFFF4 word32 dwFFFFFFF4))
	T_410 (in fp : ptr32)
Eq_424: (struct "Eq_424" 0001 (0 Eq_426 t0000))
	T_424 (in 0x00403198 : ptr32)
Eq_426: (union "Eq_426" (byte u0) (word32 u1))
	T_426 (in Mem43[ecx_39 + 0x00403198:word32] : word32)
	T_428 (in (byte) (Mem43[ecx_39 + 0x00403198:word32] ^ ecx_39) : byte)
	T_431 (in Mem52[ecx_39 + 0x00403198:byte] : byte)
Eq_429: (struct "Eq_429" 0001 (0 Eq_426 t0000))
	T_429 (in 0x00403198 : ptr32)
Eq_443: (fn void ())
	T_443 (in fn004011D8 : ptr32)
	T_444 (in signature of fn004011D8 : void)
	T_451 (in fn004011D8 : ptr32)
Eq_448: (segment "Eq_448" (0 word32 dw0000))
	T_448 (in fs : selector)
Eq_449: (union "Eq_449" (ptr32 u0) ((memptr (ptr Eq_448) word32) u1))
	T_449 (in 0x00000000 : ptr32)
Eq_453: (fn void ())
	T_453 (in int3 : ptr32)
	T_454 (in signature of int3 : void)
Eq_456: (struct "Eq_456" 0001 (0 Eq_426 t0000))
	T_456
	T_457
// Type Variables ////////////
globals_t: (in globals : (ptr (struct "Globals")))
  Class: Eq_1
  DataType: (ptr Eq_1)
  OrigDataType: (ptr (struct "Globals"))
T_2: (in GetModuleHandleA : ptr32)
  Class: Eq_2
  DataType: (ptr Eq_2)
  OrigDataType: (ptr (fn Eq_6 (Eq_4)))
T_3: (in signature of GetModuleHandleA : void)
  Class: Eq_2
  DataType: (ptr Eq_2)
  OrigDataType: 
T_4: (in lpModuleName : LPCSTR)
  Class: Eq_4
  DataType: Eq_4
  OrigDataType: LPCSTR
T_5: (in 0x00000000 : word32)
  Class: Eq_4
  DataType: Eq_4
  OrigDataType: word32
T_6: (in GetModuleHandleA(null) : HMODULE)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: HMODULE
T_7: (in 0x00403180 : word32)
  Class: Eq_7
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_8: (in Mem6[0x00403180:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_9: (in DialogBoxParamA : ptr32)
  Class: Eq_9
  DataType: (ptr Eq_9)
  OrigDataType: (ptr (fn Eq_22 (Eq_6, Eq_12, Eq_13, Eq_14, Eq_15)))
T_10: (in signature of DialogBoxParamA : void)
  Class: Eq_9
  DataType: (ptr Eq_9)
  OrigDataType: 
T_11: (in hInstance : HINSTANCE)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: HINSTANCE
T_12: (in lpTemplateName : LPCSTR)
  Class: Eq_12
  DataType: Eq_12
  OrigDataType: LPCSTR
T_13: (in hWndParent : HWND)
  Class: Eq_13
  DataType: Eq_13
  OrigDataType: HWND
T_14: (in lpDialogFunc : DLGPROC)
  Class: Eq_14
  DataType: Eq_14
  OrigDataType: DLGPROC
T_15: (in dwInitParam : LPARAM)
  Class: Eq_15
  DataType: Eq_15
  OrigDataType: LPARAM
T_16: (in 0x00403180 : ptr32)
  Class: Eq_16
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_17: (in Mem6[0x00403180:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_18: (in 0x000003EC : word32)
  Class: Eq_12
  DataType: Eq_12
  OrigDataType: word32
T_19: (in 0x00000000 : word32)
  Class: Eq_13
  DataType: Eq_13
  OrigDataType: word32
T_20: (in 0x0040102C : word32)
  Class: Eq_14
  DataType: Eq_14
  OrigDataType: word32
T_21: (in 0x00000000 : word32)
  Class: Eq_15
  DataType: Eq_15
  OrigDataType: word32
T_22: (in DialogBoxParamA(globals->t403180, (CHAR) 0x000003EC, null, &globals->ptr40102C, 0x00000000) : INT_PTR)
  Class: Eq_22
  DataType: Eq_22
  OrigDataType: INT_PTR
T_23: (in ExitProcess : ptr32)
  Class: Eq_23
  DataType: (ptr Eq_23)
  OrigDataType: (ptr (fn void (Eq_25)))
T_24: (in signature of ExitProcess : void)
  Class: Eq_23
  DataType: (ptr Eq_23)
  OrigDataType: 
T_25: (in uExitCode : UINT)
  Class: Eq_25
  DataType: Eq_25
  OrigDataType: UINT
T_26: (in 0x00000000 : word32)
  Class: Eq_25
  DataType: Eq_25
  OrigDataType: word32
T_27: (in ExitProcess(0x00000000) : void)
  Class: Eq_27
  DataType: void
  OrigDataType: void
T_28: (in dwArg08 : word32)
  Class: Eq_28
  DataType: word32
  OrigDataType: word32
T_29: (in 0x00000110 : word32)
  Class: Eq_28
  DataType: word32
  OrigDataType: word32
T_30: (in dwArg08 != 0x00000110 : bool)
  Class: Eq_30
  DataType: bool
  OrigDataType: bool
T_31: (in 0x00000136 : word32)
  Class: Eq_28
  DataType: word32
  OrigDataType: word32
T_32: (in dwArg08 != 0x00000136 : bool)
  Class: Eq_32
  DataType: bool
  OrigDataType: bool
T_33: (in 0x00000053 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_34: (in fp : ptr32)
  Class: Eq_34
  DataType: (ptr Eq_34)
  OrigDataType: (ptr (struct "Eq_34"))
T_35: (in 0x00000008 : word32)
  Class: Eq_35
  DataType: word32
  OrigDataType: word32
T_36: (in fp - 0x00000008 : word32)
  Class: Eq_36
  DataType: word32
  OrigDataType: word32
T_37: (in Mem53[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_38: (in 0x00000000 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_39: (in 0x0000000C : word32)
  Class: Eq_39
  DataType: word32
  OrigDataType: word32
T_40: (in fp - 0x0000000C : word32)
  Class: Eq_40
  DataType: word32
  OrigDataType: word32
T_41: (in Mem55[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_42: (in 0x00000000 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_43: (in 0x00000010 : word32)
  Class: Eq_43
  DataType: word32
  OrigDataType: word32
T_44: (in fp - 0x00000010 : word32)
  Class: Eq_44
  DataType: word32
  OrigDataType: word32
T_45: (in Mem57[fp - 0x00000010:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_46: (in SetWindowPos : ptr32)
  Class: Eq_46
  DataType: (ptr Eq_46)
  OrigDataType: (ptr (fn Eq_62 (Eq_6, Eq_49, int32, int32, Eq_52, Eq_53, Eq_54)))
T_47: (in signature of SetWindowPos : void)
  Class: Eq_46
  DataType: (ptr Eq_46)
  OrigDataType: 
T_48: (in hWnd : HWND)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: HWND
T_49: (in hWndInsertAfter : HWND)
  Class: Eq_49
  DataType: Eq_49
  OrigDataType: HWND
T_50: (in X : int32)
  Class: Eq_50
  DataType: int32
  OrigDataType: int32
T_51: (in Y : int32)
  Class: Eq_51
  DataType: int32
  OrigDataType: int32
T_52: (in cx : int32)
  Class: Eq_52
  DataType: Eq_52
  OrigDataType: int32
T_53: (in cy : int32)
  Class: Eq_53
  DataType: Eq_53
  OrigDataType: int32
T_54: (in uFlags : UINT)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: UINT
T_55: (in dwArg04 : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_56: (in 0x00000000 : word32)
  Class: Eq_49
  DataType: Eq_49
  OrigDataType: word32
T_57: (in 0x00000000 : word32)
  Class: Eq_50
  DataType: int32
  OrigDataType: word32
T_58: (in 0x00000000 : word32)
  Class: Eq_51
  DataType: int32
  OrigDataType: word32
T_59: (in 0x000003EC : word32)
  Class: Eq_52
  DataType: HMODULE
  OrigDataType: word32
T_60: (in 0x00000000 : word32)
  Class: Eq_53
  DataType: DWORD
  OrigDataType: word32
T_61: (in 0x0040102C : word32)
  Class: Eq_54
  DataType: BOOL
  OrigDataType: word32
T_62: (in SetWindowPos(dwArg04, null, 0x00000000, 0x00000000, 0x000003EC, 0x00000000, 0x0040102C) : BOOL)
  Class: Eq_62
  DataType: Eq_62
  OrigDataType: BOOL
T_63: (in 0x000007D1 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_64: (in 0x00000008 : word32)
  Class: Eq_64
  DataType: word32
  OrigDataType: word32
T_65: (in fp - 0x00000008 : word32)
  Class: Eq_65
  DataType: word32
  OrigDataType: word32
T_66: (in Mem70[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_67: (in 0x00403180 : ptr32)
  Class: Eq_67
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_68: (in Mem70[0x00403180:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_69: (in 0x0000000C : word32)
  Class: Eq_69
  DataType: word32
  OrigDataType: word32
T_70: (in fp - 0x0000000C : word32)
  Class: Eq_70
  DataType: word32
  OrigDataType: word32
T_71: (in Mem72[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_72: (in LoadBitmapA : ptr32)
  Class: Eq_72
  DataType: (ptr Eq_72)
  OrigDataType: (ptr (fn Eq_6 (Eq_53, Eq_54)))
T_73: (in signature of LoadBitmapA : void)
  Class: Eq_72
  DataType: (ptr Eq_72)
  OrigDataType: 
T_74: (in hInstance : HINSTANCE)
  Class: Eq_53
  DataType: Eq_53
  OrigDataType: HINSTANCE
T_75: (in lpBitmapName : LPCSTR)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: LPCSTR
T_76: (in LoadBitmapA(0x00000000, 0x0040102C) : HBITMAP)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: HBITMAP
T_77: (in 0x00000008 : word32)
  Class: Eq_77
  DataType: word32
  OrigDataType: word32
T_78: (in fp - 0x00000008 : word32)
  Class: Eq_78
  DataType: word32
  OrigDataType: word32
T_79: (in Mem76[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_80: (in CreatePatternBrush : ptr32)
  Class: Eq_80
  DataType: (ptr Eq_80)
  OrigDataType: (ptr (fn Eq_6 (Eq_54)))
T_81: (in signature of CreatePatternBrush : void)
  Class: Eq_80
  DataType: (ptr Eq_80)
  OrigDataType: 
T_82: (in hbm : HBITMAP)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: HBITMAP
T_83: (in CreatePatternBrush(0x0040102C) : HBRUSH)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: HBRUSH
T_84: (in 0x00403194 : word32)
  Class: Eq_84
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_85: (in Mem79[0x00403194:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_86: (in 0x0000000A : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_87: (in 0x00000008 : word32)
  Class: Eq_87
  DataType: word32
  OrigDataType: word32
T_88: (in fp - 0x00000008 : word32)
  Class: Eq_88
  DataType: word32
  OrigDataType: word32
T_89: (in Mem81[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_90: (in 0x000007D2 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_91: (in 0x0000000C : word32)
  Class: Eq_91
  DataType: word32
  OrigDataType: word32
T_92: (in fp - 0x0000000C : word32)
  Class: Eq_92
  DataType: word32
  OrigDataType: word32
T_93: (in Mem83[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_94: (in 0x00403180 : ptr32)
  Class: Eq_94
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_95: (in Mem83[0x00403180:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_96: (in 0x00000010 : word32)
  Class: Eq_96
  DataType: word32
  OrigDataType: word32
T_97: (in fp - 0x00000010 : word32)
  Class: Eq_97
  DataType: word32
  OrigDataType: word32
T_98: (in Mem85[fp - 0x00000010:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_99: (in FindResourceA : ptr32)
  Class: Eq_99
  DataType: (ptr Eq_99)
  OrigDataType: (ptr (fn Eq_6 (Eq_52, Eq_53, Eq_54)))
T_100: (in signature of FindResourceA : void)
  Class: Eq_99
  DataType: (ptr Eq_99)
  OrigDataType: 
T_101: (in hModule : HMODULE)
  Class: Eq_52
  DataType: Eq_52
  OrigDataType: HMODULE
T_102: (in lpName : LPCSTR)
  Class: Eq_53
  DataType: Eq_53
  OrigDataType: LPCSTR
T_103: (in lpType : LPCSTR)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: LPCSTR
T_104: (in FindResourceA(() 0x000003EC, 0x00000000, 0x0040102C) : HRSRC)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: HRSRC
T_105: (in 0x00403184 : word32)
  Class: Eq_105
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_106: (in Mem88[0x00403184:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_107: (in 0x00403184 : ptr32)
  Class: Eq_107
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_108: (in Mem88[0x00403184:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_109: (in 0x00000008 : word32)
  Class: Eq_109
  DataType: word32
  OrigDataType: word32
T_110: (in fp - 0x00000008 : word32)
  Class: Eq_110
  DataType: word32
  OrigDataType: word32
T_111: (in Mem90[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_112: (in 0x00403180 : ptr32)
  Class: Eq_112
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_113: (in Mem90[0x00403180:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_114: (in 0x0000000C : word32)
  Class: Eq_114
  DataType: word32
  OrigDataType: word32
T_115: (in fp - 0x0000000C : word32)
  Class: Eq_115
  DataType: word32
  OrigDataType: word32
T_116: (in Mem92[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_117: (in LoadResource : ptr32)
  Class: Eq_117
  DataType: (ptr Eq_117)
  OrigDataType: (ptr (fn Eq_6 (Eq_53, Eq_54)))
T_118: (in signature of LoadResource : void)
  Class: Eq_117
  DataType: (ptr Eq_117)
  OrigDataType: 
T_119: (in hModule : HMODULE)
  Class: Eq_53
  DataType: Eq_53
  OrigDataType: HMODULE
T_120: (in hResInfo : HRSRC)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: HRSRC
T_121: (in LoadResource(0x00000000, 0x0040102C) : HGLOBAL)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: HGLOBAL
T_122: (in 0x00403188 : word32)
  Class: Eq_122
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_123: (in Mem95[0x00403188:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_124: (in 0x00403184 : ptr32)
  Class: Eq_124
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_125: (in Mem95[0x00403184:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_126: (in 0x00000008 : word32)
  Class: Eq_126
  DataType: word32
  OrigDataType: word32
T_127: (in fp - 0x00000008 : word32)
  Class: Eq_127
  DataType: word32
  OrigDataType: word32
T_128: (in Mem97[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_129: (in 0x00403180 : ptr32)
  Class: Eq_129
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_130: (in Mem97[0x00403180:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_131: (in 0x0000000C : word32)
  Class: Eq_131
  DataType: word32
  OrigDataType: word32
T_132: (in fp - 0x0000000C : word32)
  Class: Eq_132
  DataType: word32
  OrigDataType: word32
T_133: (in Mem99[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_134: (in SizeofResource : ptr32)
  Class: Eq_134
  DataType: (ptr Eq_134)
  OrigDataType: (ptr (fn Eq_6 (Eq_53, Eq_54)))
T_135: (in signature of SizeofResource : void)
  Class: Eq_134
  DataType: (ptr Eq_134)
  OrigDataType: 
T_136: (in hModule : HMODULE)
  Class: Eq_53
  DataType: Eq_53
  OrigDataType: HMODULE
T_137: (in hResInfo : HRSRC)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: HRSRC
T_138: (in SizeofResource(0x00000000, 0x0040102C) : DWORD)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: DWORD
T_139: (in 0x00403190 : word32)
  Class: Eq_139
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_140: (in Mem102[0x00403190:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_141: (in 0x00403188 : ptr32)
  Class: Eq_141
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_142: (in Mem102[0x00403188:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_143: (in 0x00000008 : word32)
  Class: Eq_143
  DataType: word32
  OrigDataType: word32
T_144: (in fp - 0x00000008 : word32)
  Class: Eq_144
  DataType: word32
  OrigDataType: word32
T_145: (in Mem104[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_146: (in LockResource : ptr32)
  Class: Eq_146
  DataType: (ptr Eq_146)
  OrigDataType: (ptr (fn Eq_6 (Eq_54)))
T_147: (in signature of LockResource : void)
  Class: Eq_146
  DataType: (ptr Eq_146)
  OrigDataType: 
T_148: (in hResData : HGLOBAL)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: HGLOBAL
T_149: (in LockResource(0x0040102C) : LPVOID)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: LPVOID
T_150: (in 0x0040318C : word32)
  Class: Eq_150
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_151: (in Mem107[0x0040318C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_152: (in 0x0040318C : ptr32)
  Class: Eq_152
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_153: (in Mem107[0x0040318C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_154: (in 0x00000008 : word32)
  Class: Eq_154
  DataType: word32
  OrigDataType: word32
T_155: (in fp - 0x00000008 : word32)
  Class: Eq_155
  DataType: word32
  OrigDataType: word32
T_156: (in Mem109[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_157: (in 0x00403190 : ptr32)
  Class: Eq_157
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_158: (in Mem109[0x00403190:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_159: (in 0x0000000C : word32)
  Class: Eq_159
  DataType: word32
  OrigDataType: word32
T_160: (in fp - 0x0000000C : word32)
  Class: Eq_160
  DataType: word32
  OrigDataType: word32
T_161: (in Mem111[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_162: (in 0x00000000 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_163: (in 0x00000010 : word32)
  Class: Eq_163
  DataType: word32
  OrigDataType: word32
T_164: (in fp - 0x00000010 : word32)
  Class: Eq_164
  DataType: word32
  OrigDataType: word32
T_165: (in Mem113[fp - 0x00000010:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_166: (in eax_114 : Eq_6)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_167: (in ExtCreateRegion : ptr32)
  Class: Eq_167
  DataType: (ptr Eq_167)
  OrigDataType: (ptr (fn Eq_6 (Eq_52, Eq_53, Eq_54)))
T_168: (in signature of ExtCreateRegion : void)
  Class: Eq_167
  DataType: (ptr Eq_167)
  OrigDataType: 
T_169: (in lpx : (ptr XFORM))
  Class: Eq_52
  DataType: Eq_52
  OrigDataType: (ptr XFORM)
T_170: (in nCount : DWORD)
  Class: Eq_53
  DataType: Eq_53
  OrigDataType: DWORD
T_171: (in lpData : (ptr RGNDATA))
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: (ptr RGNDATA)
T_172: (in ExtCreateRegion(() 0x000003EC, 0x00000000, 0x0040102C) : HRGN)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: HRGN
T_173: (in 0x00000001 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_174: (in 0x00000008 : word32)
  Class: Eq_174
  DataType: word32
  OrigDataType: word32
T_175: (in fp - 0x00000008 : word32)
  Class: Eq_175
  DataType: word32
  OrigDataType: word32
T_176: (in Mem117[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_177: (in 0x0000000C : word32)
  Class: Eq_177
  DataType: word32
  OrigDataType: word32
T_178: (in fp - 0x0000000C : word32)
  Class: Eq_178
  DataType: word32
  OrigDataType: word32
T_179: (in Mem119[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_180: (in 0x00000010 : word32)
  Class: Eq_180
  DataType: word32
  OrigDataType: word32
T_181: (in fp - 0x00000010 : word32)
  Class: Eq_181
  DataType: word32
  OrigDataType: word32
T_182: (in Mem121[fp - 0x00000010:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_183: (in SetWindowRgn : ptr32)
  Class: Eq_183
  DataType: (ptr Eq_183)
  OrigDataType: (ptr (fn int32 (Eq_52, Eq_53, Eq_54)))
T_184: (in signature of SetWindowRgn : void)
  Class: Eq_183
  DataType: (ptr Eq_183)
  OrigDataType: 
T_185: (in hWnd : HWND)
  Class: Eq_52
  DataType: Eq_52
  OrigDataType: HWND
T_186: (in hRgn : HRGN)
  Class: Eq_53
  DataType: Eq_53
  OrigDataType: HRGN
T_187: (in bRedraw : BOOL)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: BOOL
T_188: (in SetWindowRgn(() 0x000003EC, 0x00000000, 0x0040102C) : int32)
  Class: Eq_188
  DataType: int32
  OrigDataType: int32
T_189: (in 0x00000001 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_190: (in 0x00000008 : word32)
  Class: Eq_190
  DataType: word32
  OrigDataType: word32
T_191: (in fp - 0x00000008 : word32)
  Class: Eq_191
  DataType: word32
  OrigDataType: word32
T_192: (in Mem125[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_193: (in 0x0000000C : word32)
  Class: Eq_193
  DataType: word32
  OrigDataType: word32
T_194: (in fp - 0x0000000C : word32)
  Class: Eq_194
  DataType: word32
  OrigDataType: word32
T_195: (in Mem127[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_196: (in ShowWindow : ptr32)
  Class: Eq_196
  DataType: (ptr Eq_196)
  OrigDataType: (ptr (fn T_200 (T_60, T_61)))
T_197: (in signature of ShowWindow : void)
  Class: Eq_196
  DataType: (ptr Eq_196)
  OrigDataType: 
T_198: (in hWnd : HWND)
  Class: Eq_53
  DataType: Eq_53
  OrigDataType: HWND
T_199: (in nCmdShow : int32)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: int32
T_200: (in ShowWindow(0x00000000, 0x0040102C) : BOOL)
  Class: Eq_200
  DataType: Eq_200
  OrigDataType: BOOL
T_201: (in 0x0000000F : word32)
  Class: Eq_28
  DataType: word32
  OrigDataType: word32
T_202: (in dwArg08 != 0x0000000F : bool)
  Class: Eq_202
  DataType: bool
  OrigDataType: bool
T_203: (in 0x00000111 : word32)
  Class: Eq_28
  DataType: word32
  OrigDataType: word32
T_204: (in dwArg08 != 0x00000111 : bool)
  Class: Eq_204
  DataType: bool
  OrigDataType: bool
T_205: (in 0x00000001 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_206: (in 0x00000008 : word32)
  Class: Eq_206
  DataType: word32
  OrigDataType: word32
T_207: (in fp - 0x00000008 : word32)
  Class: Eq_207
  DataType: word32
  OrigDataType: word32
T_208: (in Mem144[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_209: (in 0x00000000 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_210: (in 0x0000000C : word32)
  Class: Eq_210
  DataType: word32
  OrigDataType: word32
T_211: (in fp - 0x0000000C : word32)
  Class: Eq_211
  DataType: word32
  OrigDataType: word32
T_212: (in Mem146[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_213: (in 0x00000010 : word32)
  Class: Eq_213
  DataType: word32
  OrigDataType: word32
T_214: (in fp - 0x00000010 : word32)
  Class: Eq_214
  DataType: word32
  OrigDataType: word32
T_215: (in Mem148[fp - 0x00000010:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_216: (in InvalidateRect : ptr32)
  Class: Eq_216
  DataType: (ptr Eq_216)
  OrigDataType: (ptr (fn Eq_221 (Eq_52, Eq_53, Eq_54)))
T_217: (in signature of InvalidateRect : void)
  Class: Eq_216
  DataType: (ptr Eq_216)
  OrigDataType: 
T_218: (in hWnd : HWND)
  Class: Eq_52
  DataType: Eq_52
  OrigDataType: HWND
T_219: (in lpRect : (ptr RECT))
  Class: Eq_53
  DataType: Eq_53
  OrigDataType: (ptr RECT)
T_220: (in bErase : BOOL)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: BOOL
T_221: (in InvalidateRect(() 0x000003EC, 0x00000000, 0x0040102C) : BOOL)
  Class: Eq_221
  DataType: Eq_221
  OrigDataType: BOOL
T_222: (in 0x00000008 : word32)
  Class: Eq_222
  DataType: word32
  OrigDataType: word32
T_223: (in fp - 0x00000008 : word32)
  Class: Eq_223
  DataType: word32
  OrigDataType: word32
T_224: (in Mem152[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_225: (in UpdateWindow : ptr32)
  Class: Eq_225
  DataType: (ptr Eq_225)
  OrigDataType: (ptr (fn Eq_228 (Eq_54)))
T_226: (in signature of UpdateWindow : void)
  Class: Eq_225
  DataType: (ptr Eq_225)
  OrigDataType: 
T_227: (in hWnd : HWND)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: HWND
T_228: (in UpdateWindow(0x0040102C) : BOOL)
  Class: Eq_228
  DataType: Eq_228
  OrigDataType: BOOL
T_229: (in 0x00000001 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_230: (in 0x00000008 : word32)
  Class: Eq_230
  DataType: word32
  OrigDataType: word32
T_231: (in fp - 0x00000008 : word32)
  Class: Eq_231
  DataType: word32
  OrigDataType: word32
T_232: (in Mem156[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_233: (in 0x0000000C : word32)
  Class: Eq_233
  DataType: word32
  OrigDataType: word32
T_234: (in fp - 0x0000000C : word32)
  Class: Eq_234
  DataType: word32
  OrigDataType: word32
T_235: (in Mem158[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_236: (in ShowWindow : ptr32)
  Class: Eq_196
  DataType: (ptr Eq_196)
  OrigDataType: (ptr (fn T_237 (T_60, T_61)))
T_237: (in ShowWindow(0x00000000, 0x0040102C) : BOOL)
  Class: Eq_200
  DataType: Eq_200
  OrigDataType: BOOL
T_238: (in 0x00000010 : word32)
  Class: Eq_28
  DataType: word32
  OrigDataType: word32
T_239: (in dwArg08 != 0x00000010 : bool)
  Class: Eq_239
  DataType: bool
  OrigDataType: bool
T_240: (in dwArg0C : word32)
  Class: Eq_240
  DataType: word32
  OrigDataType: word32
T_241: (in 0x000003F2 : word32)
  Class: Eq_240
  DataType: word32
  OrigDataType: word32
T_242: (in dwArg0C != 0x000003F2 : bool)
  Class: Eq_242
  DataType: bool
  OrigDataType: bool
T_243: (in 0x000003FA : word32)
  Class: Eq_240
  DataType: word32
  OrigDataType: word32
T_244: (in dwArg0C != 0x000003FA : bool)
  Class: Eq_244
  DataType: bool
  OrigDataType: bool
T_245: (in 0x0000000C : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_246: (in 0x00000008 : word32)
  Class: Eq_246
  DataType: word32
  OrigDataType: word32
T_247: (in fp - 0x00000008 : word32)
  Class: Eq_247
  DataType: word32
  OrigDataType: word32
T_248: (in Mem170[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_249: (in 0x00403198 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_250: (in 0x0000000C : word32)
  Class: Eq_250
  DataType: word32
  OrigDataType: word32
T_251: (in fp - 0x0000000C : word32)
  Class: Eq_251
  DataType: word32
  OrigDataType: word32
T_252: (in Mem172[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_253: (in 0x000003F8 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_254: (in 0x00000010 : word32)
  Class: Eq_254
  DataType: word32
  OrigDataType: word32
T_255: (in fp - 0x00000010 : word32)
  Class: Eq_255
  DataType: word32
  OrigDataType: word32
T_256: (in Mem174[fp - 0x00000010:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_257: (in GetDlgItemTextA : ptr32)
  Class: Eq_257
  DataType: (ptr Eq_257)
  OrigDataType: (ptr (fn T_263 (T_55, T_59, T_60, T_61)))
T_258: (in signature of GetDlgItemTextA : void)
  Class: Eq_257
  DataType: (ptr Eq_257)
  OrigDataType: 
T_259: (in hDlg : HWND)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: HWND
T_260: (in nIDDlgItem : int32)
  Class: Eq_52
  DataType: Eq_52
  OrigDataType: int32
T_261: (in lpString : LPSTR)
  Class: Eq_53
  DataType: Eq_53
  OrigDataType: LPSTR
T_262: (in cchMax : int32)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: int32
T_263: (in GetDlgItemTextA(dwArg04, () 0x000003EC, 0x00000000, 0x0040102C) : UINT)
  Class: Eq_263
  DataType: Eq_263
  OrigDataType: UINT
T_264: (in 0x0000000C : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_265: (in 0x00000008 : word32)
  Class: Eq_265
  DataType: word32
  OrigDataType: word32
T_266: (in fp - 0x00000008 : word32)
  Class: Eq_266
  DataType: word32
  OrigDataType: word32
T_267: (in Mem180[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_268: (in 0x004031B0 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_269: (in 0x0000000C : word32)
  Class: Eq_269
  DataType: word32
  OrigDataType: word32
T_270: (in fp - 0x0000000C : word32)
  Class: Eq_270
  DataType: word32
  OrigDataType: word32
T_271: (in Mem182[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_272: (in 0x000003F9 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_273: (in 0x00000010 : word32)
  Class: Eq_273
  DataType: word32
  OrigDataType: word32
T_274: (in fp - 0x00000010 : word32)
  Class: Eq_274
  DataType: word32
  OrigDataType: word32
T_275: (in Mem184[fp - 0x00000010:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_276: (in GetDlgItemTextA : ptr32)
  Class: Eq_257
  DataType: (ptr Eq_257)
  OrigDataType: (ptr (fn T_277 (T_55, T_59, T_60, T_61)))
T_277: (in GetDlgItemTextA(dwArg04, () 0x000003EC, 0x00000000, 0x0040102C) : UINT)
  Class: Eq_263
  DataType: Eq_263
  OrigDataType: UINT
T_278: (in 0x00 : byte)
  Class: Eq_278
  DataType: byte
  OrigDataType: byte
T_279: (in 0x004031CC : ptr32)
  Class: Eq_279
  DataType: (ptr byte)
  OrigDataType: (ptr (struct (0 byte b0000)))
T_280: (in Mem189[0x004031CC:byte] : byte)
  Class: Eq_278
  DataType: byte
  OrigDataType: byte
T_281: (in fn00401238 : ptr32)
  Class: Eq_281
  DataType: (ptr Eq_281)
  OrigDataType: (ptr (fn void (word32)))
T_282: (in signature of fn00401238 : void)
  Class: Eq_281
  DataType: (ptr Eq_281)
  OrigDataType: 
T_283: (in edx : word32)
  Class: Eq_283
  DataType: word32
  OrigDataType: word32
T_284: (in edx : word32)
  Class: Eq_283
  DataType: word32
  OrigDataType: word32
T_285: (in fn00401238(edx) : void)
  Class: Eq_285
  DataType: void
  OrigDataType: void
T_286: (in 0x00090000 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_287: (in 0x00000008 : word32)
  Class: Eq_287
  DataType: word32
  OrigDataType: word32
T_288: (in fp - 0x00000008 : word32)
  Class: Eq_288
  DataType: word32
  OrigDataType: word32
T_289: (in Mem206[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_290: (in 0x000003E8 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_291: (in 0x0000000C : word32)
  Class: Eq_291
  DataType: word32
  OrigDataType: word32
T_292: (in fp - 0x0000000C : word32)
  Class: Eq_292
  DataType: word32
  OrigDataType: word32
T_293: (in Mem208[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_294: (in 0x00000010 : word32)
  Class: Eq_294
  DataType: word32
  OrigDataType: word32
T_295: (in fp - 0x00000010 : word32)
  Class: Eq_295
  DataType: word32
  OrigDataType: word32
T_296: (in Mem210[fp - 0x00000010:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_297: (in AnimateWindow : ptr32)
  Class: Eq_297
  DataType: (ptr Eq_297)
  OrigDataType: (ptr (fn Eq_302 (Eq_52, Eq_53, Eq_54)))
T_298: (in signature of AnimateWindow : void)
  Class: Eq_297
  DataType: (ptr Eq_297)
  OrigDataType: 
T_299: (in hWnd : HWND)
  Class: Eq_52
  DataType: Eq_52
  OrigDataType: HWND
T_300: (in dwTime : DWORD)
  Class: Eq_53
  DataType: Eq_53
  OrigDataType: DWORD
T_301: (in dwFlags : DWORD)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: DWORD
T_302: (in AnimateWindow(() 0x000003EC, 0x00000000, 0x0040102C) : BOOL)
  Class: Eq_302
  DataType: Eq_302
  OrigDataType: BOOL
T_303: (in 0x00403194 : ptr32)
  Class: Eq_303
  DataType: (ptr Eq_6)
  OrigDataType: (ptr (struct (0 Eq_6 t0000)))
T_304: (in Mem210[0x00403194:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_305: (in 0x00000008 : word32)
  Class: Eq_305
  DataType: word32
  OrigDataType: word32
T_306: (in fp - 0x00000008 : word32)
  Class: Eq_306
  DataType: word32
  OrigDataType: word32
T_307: (in Mem214[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_308: (in DeleteObject : ptr32)
  Class: Eq_308
  DataType: (ptr Eq_308)
  OrigDataType: (ptr (fn Eq_311 (Eq_54)))
T_309: (in signature of DeleteObject : void)
  Class: Eq_308
  DataType: (ptr Eq_308)
  OrigDataType: 
T_310: (in ho : HGDIOBJ)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: HGDIOBJ
T_311: (in DeleteObject(0x0040102C) : BOOL)
  Class: Eq_311
  DataType: Eq_311
  OrigDataType: BOOL
T_312: (in 0x00000000 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_313: (in 0x00000008 : word32)
  Class: Eq_313
  DataType: word32
  OrigDataType: word32
T_314: (in fp - 0x00000008 : word32)
  Class: Eq_314
  DataType: word32
  OrigDataType: word32
T_315: (in Mem218[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_316: (in 0x0000000C : word32)
  Class: Eq_316
  DataType: word32
  OrigDataType: word32
T_317: (in fp - 0x0000000C : word32)
  Class: Eq_317
  DataType: word32
  OrigDataType: word32
T_318: (in Mem220[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_319: (in EndDialog : ptr32)
  Class: Eq_319
  DataType: (ptr Eq_319)
  OrigDataType: (ptr (fn Eq_323 (Eq_53, Eq_54)))
T_320: (in signature of EndDialog : void)
  Class: Eq_319
  DataType: (ptr Eq_319)
  OrigDataType: 
T_321: (in hDlg : HWND)
  Class: Eq_53
  DataType: Eq_53
  OrigDataType: HWND
T_322: (in nResult : INT_PTR)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: INT_PTR
T_323: (in EndDialog(0x00000000, 0x0040102C) : BOOL)
  Class: Eq_323
  DataType: Eq_323
  OrigDataType: BOOL
T_324: (in 0x00000000 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_325: (in 0x00000008 : word32)
  Class: Eq_325
  DataType: word32
  OrigDataType: word32
T_326: (in fp - 0x00000008 : word32)
  Class: Eq_326
  DataType: word32
  OrigDataType: word32
T_327: (in Mem194[fp - 0x00000008:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_328: (in 0x00000000 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_329: (in 0x0000000C : word32)
  Class: Eq_329
  DataType: word32
  OrigDataType: word32
T_330: (in fp - 0x0000000C : word32)
  Class: Eq_330
  DataType: word32
  OrigDataType: word32
T_331: (in Mem196[fp - 0x0000000C:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_332: (in 0x00000010 : word32)
  Class: Eq_6
  DataType: DWORD
  OrigDataType: word32
T_333: (in 0x00000010 : word32)
  Class: Eq_333
  DataType: word32
  OrigDataType: word32
T_334: (in fp - 0x00000010 : word32)
  Class: Eq_334
  DataType: word32
  OrigDataType: word32
T_335: (in Mem198[fp - 0x00000010:word32] : word32)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: word32
T_336: (in SendMessageA : ptr32)
  Class: Eq_336
  DataType: (ptr Eq_336)
  OrigDataType: (ptr (fn Eq_342 (Eq_6, Eq_52, Eq_53, Eq_54)))
T_337: (in signature of SendMessageA : void)
  Class: Eq_336
  DataType: (ptr Eq_336)
  OrigDataType: 
T_338: (in hWnd : HWND)
  Class: Eq_6
  DataType: Eq_6
  OrigDataType: HWND
T_339: (in Msg : UINT)
  Class: Eq_52
  DataType: Eq_52
  OrigDataType: UINT
T_340: (in wParam : WPARAM)
  Class: Eq_53
  DataType: Eq_53
  OrigDataType: WPARAM
T_341: (in lParam : LPARAM)
  Class: Eq_54
  DataType: Eq_54
  OrigDataType: LPARAM
T_342: (in SendMessageA(dwArg04, () 0x000003EC, 0x00000000, 0x0040102C) : LRESULT)
  Class: Eq_342
  DataType: Eq_342
  OrigDataType: LRESULT
T_343: (in ecx : word32)
  Class: Eq_343
  DataType: word32
  OrigDataType: word32
T_344: (in edi_16 : (ptr Eq_344))
  Class: Eq_344
  DataType: (ptr Eq_344)
  OrigDataType: word32
T_345: (in dwArg04 : word32)
  Class: Eq_344
  DataType: (ptr Eq_344)
  OrigDataType: word32
T_346: (in ecx_11 : word32)
  Class: Eq_346
  DataType: word32
  OrigDataType: word32
T_347: (in 0x00000000 : word32)
  Class: Eq_347
  DataType: word32
  OrigDataType: word32
T_348: (in ~0x00000000 : word32)
  Class: Eq_346
  DataType: word32
  OrigDataType: word32
T_349: (in ~ecx_11 : word32)
  Class: Eq_349
  DataType: word32
  OrigDataType: word32
T_350: (in 0x00000001 : word32)
  Class: Eq_350
  DataType: word32
  OrigDataType: word32
T_351: (in ~ecx_11 - 0x00000001 : word32)
  Class: Eq_351
  DataType: word32
  OrigDataType: word32
T_352: (in edi_33 : (ptr Eq_344))
  Class: Eq_344
  DataType: (ptr Eq_344)
  OrigDataType: (ptr (struct (0 T_360 t0000)))
T_353: (in 0x00000001 : word32)
  Class: Eq_353
  DataType: word32
  OrigDataType: word32
T_354: (in edi_33 + 0x00000001 : word32)
  Class: Eq_344
  DataType: (ptr Eq_344)
  OrigDataType: word32
T_355: (in 0x00000001 : word32)
  Class: Eq_355
  DataType: word32
  OrigDataType: word32
T_356: (in ecx_11 - 0x00000001 : word32)
  Class: Eq_346
  DataType: word32
  OrigDataType: word32
T_357: (in 0x00 : byte)
  Class: Eq_357
  DataType: byte
  OrigDataType: byte
T_358: (in 0x00000000 : word32)
  Class: Eq_358
  DataType: word32
  OrigDataType: word32
T_359: (in edi_33 + 0x00000000 : word32)
  Class: Eq_359
  DataType: word32
  OrigDataType: word32
T_360: (in Mem0[edi_33 + 0x00000000:byte] : byte)
  Class: Eq_357
  DataType: byte
  OrigDataType: byte
T_361: (in 0x00 != *edi_33 : bool)
  Class: Eq_361
  DataType: bool
  OrigDataType: bool
T_362: (in 0x00000000 : word32)
  Class: Eq_346
  DataType: word32
  OrigDataType: word32
T_363: (in ecx_11 == 0x00000000 : bool)
  Class: Eq_363
  DataType: bool
  OrigDataType: bool
T_364: (in 0x00000000 : word32)
  Class: Eq_364
  DataType: word32
  OrigDataType: word32
T_365: (in 0x004035CF : ptr32)
  Class: Eq_365
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 word32 dw0000)))
T_366: (in Mem4[0x004035CF:word32] : word32)
  Class: Eq_364
  DataType: word32
  OrigDataType: word32
T_367: (in 0x00000000 : word32)
  Class: Eq_367
  DataType: word32
  OrigDataType: word32
T_368: (in 0x004035D7 : ptr32)
  Class: Eq_368
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 word32 dw0000)))
T_369: (in Mem5[0x004035D7:word32] : word32)
  Class: Eq_367
  DataType: word32
  OrigDataType: word32
T_370: (in 0x00000000 : word32)
  Class: Eq_370
  DataType: word32
  OrigDataType: word32
T_371: (in 0x004035DF : ptr32)
  Class: Eq_371
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 word32 dw0000)))
T_372: (in Mem6[0x004035DF:word32] : word32)
  Class: Eq_370
  DataType: word32
  OrigDataType: word32
T_373: (in 0x004031CC : ptr32)
  Class: Eq_373
  DataType: (ptr byte)
  OrigDataType: (ptr (struct (0 byte b0000)))
T_374: (in Mem6[0x004031CC:byte] : byte)
  Class: Eq_278
  DataType: byte
  OrigDataType: byte
T_375: (in (word32) globals->b4031CC : word32)
  Class: Eq_375
  DataType: (ptr Eq_375)
  OrigDataType: (ptr (struct "Eq_375"))
T_376: (in 0x00403198 : word32)
  Class: Eq_376
  DataType: word32
  OrigDataType: word32
T_377: (in (word32) globals->b4031CC + 0x00403198 : word32)
  Class: Eq_377
  DataType: word32
  OrigDataType: word32
T_378: (in Mem6[(word32) globals->b4031CC + 0x00403198:byte] : byte)
  Class: Eq_378
  DataType: byte
  OrigDataType: byte
T_379: (in (word32) *(word32) globals->b4031CC : word32)
  Class: Eq_379
  DataType: word32
  OrigDataType: word32
T_380: (in 0x004035E7 : word32)
  Class: Eq_380
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 word32 dw0000)))
T_381: (in Mem14[0x004035E7:word32] : word32)
  Class: Eq_379
  DataType: word32
  OrigDataType: word32
T_382: (in 0x00000032 : word32)
  Class: Eq_382
  DataType: word32
  OrigDataType: word32
T_383: (in 0x004035EF : ptr32)
  Class: Eq_383
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 word32 dw0000)))
T_384: (in Mem15[0x004035EF:word32] : word32)
  Class: Eq_382
  DataType: word32
  OrigDataType: word32
T_385: (in 0x00000000 : word32)
  Class: Eq_385
  DataType: word32
  OrigDataType: word32
T_386: (in 0x004035FF : ptr32)
  Class: Eq_386
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 word32 dw0000)))
T_387: (in Mem16[0x004035FF:word32] : word32)
  Class: Eq_385
  DataType: word32
  OrigDataType: word32
T_388: (in 0x00 : byte)
  Class: Eq_388
  DataType: byte
  OrigDataType: byte
T_389: (in 0x00403610 : ptr32)
  Class: Eq_389
  DataType: (ptr byte)
  OrigDataType: (ptr (struct (0 byte b0000)))
T_390: (in Mem17[0x00403610:byte] : byte)
  Class: Eq_388
  DataType: byte
  OrigDataType: byte
T_391: (in 0x00 : byte)
  Class: Eq_391
  DataType: byte
  OrigDataType: byte
T_392: (in 0x0040360F : ptr32)
  Class: Eq_392
  DataType: (ptr byte)
  OrigDataType: (ptr (struct (0 byte b0000)))
T_393: (in Mem18[0x0040360F:byte] : byte)
  Class: Eq_391
  DataType: byte
  OrigDataType: byte
T_394: (in 0x004031CC : ptr32)
  Class: Eq_394
  DataType: (ptr byte)
  OrigDataType: (ptr (struct (0 byte b0000)))
T_395: (in Mem18[0x004031CC:byte] : byte)
  Class: Eq_278
  DataType: byte
  OrigDataType: byte
T_396: (in 0x01 : byte)
  Class: Eq_396
  DataType: byte
  OrigDataType: byte
T_397: (in globals->b4031CC + 0x01 : byte)
  Class: Eq_278
  DataType: byte
  OrigDataType: byte
T_398: (in Mem20[0x004031CC:byte] : byte)
  Class: Eq_278
  DataType: byte
  OrigDataType: byte
T_399: (in ecx_10 : Eq_399)
  Class: Eq_399
  DataType: Eq_399
  OrigDataType: up32
T_400: (in fn004011C2 : ptr32)
  Class: Eq_400
  DataType: (ptr Eq_400)
  OrigDataType: (ptr (fn T_404 (T_403)))
T_401: (in signature of fn004011C2 : void)
  Class: Eq_400
  DataType: (ptr Eq_400)
  OrigDataType: 
T_402: (in dwArg04 : word32)
  Class: Eq_402
  DataType: word32
  OrigDataType: word32
T_403: (in 0x00403198 : word32)
  Class: Eq_402
  DataType: word32
  OrigDataType: word32
T_404: (in fn004011C2(0x00403198) : word32)
  Class: Eq_399
  DataType: Eq_399
  OrigDataType: word32
T_405: (in 0x00000007 : word32)
  Class: Eq_399
  DataType: up32
  OrigDataType: up32
T_406: (in ecx_10 >=u 0x00000007 : bool)
  Class: Eq_406
  DataType: bool
  OrigDataType: bool
T_407: (in 0x004031CD : ptr32)
  Class: Eq_407
  DataType: (ptr Eq_399)
  OrigDataType: (ptr (struct (0 Eq_399 t0000)))
T_408: (in Mem29[0x004031CD:word32] : word32)
  Class: Eq_399
  DataType: Eq_399
  OrigDataType: word32
T_409: (in 0x004031B0 : word32)
  Class: Eq_283
  DataType: word32
  OrigDataType: word32
T_410: (in fp : ptr32)
  Class: Eq_410
  DataType: (ptr Eq_410)
  OrigDataType: (ptr (struct "Eq_410"))
T_411: (in 0xFFFFFFF4 : word32)
  Class: Eq_411
  DataType: word32
  OrigDataType: word32
T_412: (in fp + 0xFFFFFFF4 : word32)
  Class: Eq_412
  DataType: word32
  OrigDataType: word32
T_413: (in Mem31[fp + 0xFFFFFFF4:word32] : word32)
  Class: Eq_283
  DataType: word32
  OrigDataType: word32
T_414: (in fn004011C2 : ptr32)
  Class: Eq_400
  DataType: (ptr Eq_400)
  OrigDataType: (ptr (fn T_416 (T_415)))
T_415: (in 0x00403198 : word32)
  Class: Eq_402
  DataType: word32
  OrigDataType: word32
T_416: (in fn004011C2(0x00403198) : word32)
  Class: Eq_399
  DataType: Eq_399
  OrigDataType: up32
T_417: (in 0x00000007 : word32)
  Class: Eq_399
  DataType: up32
  OrigDataType: up32
T_418: (in fn004011C2(0x00403198) >=u 0x00000007 : bool)
  Class: Eq_418
  DataType: bool
  OrigDataType: bool
T_419: (in 0xFFFFFFF4 : word32)
  Class: Eq_419
  DataType: word32
  OrigDataType: word32
T_420: (in fp + 0xFFFFFFF4 : word32)
  Class: Eq_420
  DataType: word32
  OrigDataType: word32
T_421: (in Mem43[fp + 0xFFFFFFF4:word32] : word32)
  Class: Eq_283
  DataType: word32
  OrigDataType: word32
T_422: (in ecx_39 : Eq_399)
  Class: Eq_399
  DataType: Eq_399
  OrigDataType: int32
T_423: (in 0x00000000 : word32)
  Class: Eq_399
  DataType: int32
  OrigDataType: word32
T_424: (in 0x00403198 : ptr32)
  Class: Eq_424
  DataType: (ptr Eq_424)
  OrigDataType: (ptr (struct "Eq_424" 0001))
T_425: (in ecx_39 + 0x00403198 : word32)
  Class: Eq_425
  DataType: word32
  OrigDataType: word32
T_426: (in Mem43[ecx_39 + 0x00403198:word32] : word32)
  Class: Eq_426
  DataType: Eq_426
  OrigDataType: word32
T_427: (in Mem43[ecx_39 + 0x00403198:word32] ^ ecx_39 : word32)
  Class: Eq_427
  DataType: word32
  OrigDataType: word32
T_428: (in (byte) (Mem43[ecx_39 + 0x00403198:word32] ^ ecx_39) : byte)
  Class: Eq_426
  DataType: Eq_426
  OrigDataType: byte
T_429: (in 0x00403198 : ptr32)
  Class: Eq_429
  DataType: (ptr Eq_429)
  OrigDataType: (ptr (struct "Eq_429" 0001))
T_430: (in ecx_39 + 0x00403198 : word32)
  Class: Eq_430
  DataType: word32
  OrigDataType: word32
T_431: (in Mem52[ecx_39 + 0x00403198:byte] : byte)
  Class: Eq_426
  DataType: Eq_426
  OrigDataType: byte
T_432: (in 0x00000001 : word32)
  Class: Eq_432
  DataType: word32
  OrigDataType: word32
T_433: (in ecx_39 + 0x00000001 : word32)
  Class: Eq_399
  DataType: Eq_399
  OrigDataType: word32
T_434: (in 0x004031CD : ptr32)
  Class: Eq_434
  DataType: (ptr Eq_399)
  OrigDataType: (ptr (struct (0 Eq_399 t0000)))
T_435: (in Mem52[0x004031CD:word32] : word32)
  Class: Eq_399
  DataType: Eq_399
  OrigDataType: int32
T_436: (in ecx_39 <= globals->t4031CD : bool)
  Class: Eq_436
  DataType: bool
  OrigDataType: bool
T_437: (in 0x004031CE : ptr32)
  Class: Eq_437
  DataType: (ptr Eq_399)
  OrigDataType: (ptr (struct (0 Eq_399 t0000)))
T_438: (in Mem61[0x004031CE:word32] : word32)
  Class: Eq_399
  DataType: Eq_399
  OrigDataType: word32
T_439: (in 0x004035FF : ptr32)
  Class: Eq_439
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 word32 dw0000)))
T_440: (in Mem61[0x004035FF:word32] : word32)
  Class: Eq_385
  DataType: word32
  OrigDataType: word32
T_441: (in 0x004031C8 : ptr32)
  Class: Eq_441
  DataType: (ptr word32)
  OrigDataType: (ptr (struct (0 word32 dw0000)))
T_442: (in Mem66[0x004031C8:word32] : word32)
  Class: Eq_385
  DataType: word32
  OrigDataType: word32
T_443: (in fn004011D8 : ptr32)
  Class: Eq_443
  DataType: (ptr Eq_443)
  OrigDataType: (ptr (fn T_445 ()))
T_444: (in signature of fn004011D8 : void)
  Class: Eq_443
  DataType: (ptr Eq_443)
  OrigDataType: 
T_445: (in fn004011D8() : void)
  Class: Eq_445
  DataType: void
  OrigDataType: void
T_446: (in 0xFFFFFFF0 : word32)
  Class: Eq_446
  DataType: word32
  OrigDataType: word32
T_447: (in fp + 0xFFFFFFF0 : word32)
  Class: Eq_447
  DataType: word32
  OrigDataType: word32
T_448: (in fs : selector)
  Class: Eq_448
  DataType: (ptr Eq_448)
  OrigDataType: (ptr (segment "Eq_448"))
T_449: (in 0x00000000 : ptr32)
  Class: Eq_449
  DataType: Eq_449
  OrigDataType: (union (ptr32 u0) ((memptr (ptr Eq_448) word32) u1))
T_450: (in Mem73[fs:0x00000000:word32] : word32)
  Class: Eq_447
  DataType: word32
  OrigDataType: word32
T_451: (in fn004011D8 : ptr32)
  Class: Eq_443
  DataType: (ptr Eq_443)
  OrigDataType: (ptr (fn T_452 ()))
T_452: (in fn004011D8() : void)
  Class: Eq_445
  DataType: void
  OrigDataType: void
T_453: (in int3 : ptr32)
  Class: Eq_453
  DataType: (ptr Eq_453)
  OrigDataType: (ptr (fn void ()))
T_454: (in signature of int3 : void)
  Class: Eq_453
  DataType: (ptr Eq_453)
  OrigDataType: 
T_455: (in int3() : void)
  Class: Eq_455
  DataType: void
  OrigDataType: void
T_456:
  Class: Eq_456
  DataType: Eq_456
  OrigDataType: (struct 0001 (0 T_426 t0000))
T_457:
  Class: Eq_456
  DataType: Eq_456
  OrigDataType: (struct 0001 (0 T_431 t0000))
T_458:
  Class: Eq_458
  DataType: int32
  OrigDataType: 
T_459:
  Class: Eq_459
  DataType: int32
  OrigDataType: 
T_460:
  Class: Eq_460
  DataType: int32
  OrigDataType: 
T_461:
  Class: Eq_461
  DataType: int32
  OrigDataType: 
T_462:
  Class: Eq_462
  DataType: int32
  OrigDataType: 
T_463:
  Class: Eq_463
  DataType: int32
  OrigDataType: 
*/
typedef code Eq_1Eq_6 Eq_1Eq_6 Eq_1Eq_6 Eq_1Eq_6 Eq_1Eq_6 Eq_1Eq_6 Eq_1Eq_456 Eq_1[]Eq_399Eq_399struct Globals {
	code * ptr40102C;	// 40102C
	Eq_6 t403180;	// 403180
	Eq_6 t403184;	// 403184
	Eq_6 t403188;	// 403188
	Eq_6 t40318C;	// 40318C
	Eq_6 t403190;	// 403190
	Eq_6 t403194;	// 403194
	Eq_456 a403198[];	// 403198
	word32 dw4031C8;	// 4031C8
	byte b4031CC;	// 4031CC
	Eq_399 t4031CD;	// 4031CD
	Eq_399 t4031CE;	// 4031CE
	word32 dw4035CF;	// 4035CF
	word32 dw4035D7;	// 4035D7
	word32 dw4035DF;	// 4035DF
	word32 dw4035E7;	// 4035E7
	word32 dw4035EF;	// 4035EF
	word32 dw4035FF;	// 4035FF
	byte b40360F;	// 40360F
	byte b403610;	// 403610
} Eq_1;

typedef Eq_6 (Eq_2)(LPCSTR);

typedef LPCSTR;

typedef union Eq_6 {
	 DWORD u0;
	 HBITMAP u1;
	 HBRUSH u2;
	 HGLOBAL u3;
	 HINSTANCE u4;
	 HMODULE u5;
	 HRGN u6;
	 HRSRC u7;
	 HWND u8;
	 LPVOID u9;
} Eq_6;

typedef INT_PTR (Eq_9)(Eq_6, LPCSTR, HWND, DLGPROC, LPARAM);

typedef LPCSTR;

typedef HWND;

typedef DLGPROC;

typedef LPARAM;

typedef INT_PTR;

typedef void (Eq_23)(UINT);

typedef UINT;

typedef Eq_6 Eq_34Eq_6 Eq_34Eq_6 Eq_34struct Eq_34 {
	Eq_6 tFFFFFFF0;	// FFFFFFF0
	Eq_6 tFFFFFFF4;	// FFFFFFF4
	Eq_6 tFFFFFFF8;	// FFFFFFF8
} Eq_34;

typedef BOOL (Eq_46)(Eq_6, HWND, int32, int32, Eq_52, Eq_53, Eq_54);

typedef HWND;

typedef union Eq_52 {
	 HMODULE u0;
	 HWND u1;
	 UINT u2;
} Eq_52;

typedef union Eq_53 {
	 DWORD u0;
	 HINSTANCE u1;
	 HMODULE u2;
	 HRGN u3;
	 HWND u4;
	 LPCSTR u5;
	 LPSTR u6;
	 WPARAM u7;
} Eq_53;

typedef union Eq_54 {
	 BOOL u0;
	 DWORD u1;
	 HBITMAP u2;
	 HGDIOBJ u3;
	 HGLOBAL u4;
	 HRSRC u5;
	 HWND u6;
	 INT_PTR u7;
	 LPARAM u8;
	 LPCSTR u9;
	 UINT u10;
} Eq_54;

typedef BOOL;

typedef Eq_6 (Eq_72)(Eq_53, Eq_54);

typedef Eq_6 (Eq_80)(Eq_54);

typedef Eq_6 (Eq_99)(Eq_52, Eq_53, Eq_54);

typedef Eq_6 (Eq_117)(Eq_53, Eq_54);

typedef Eq_6 (Eq_134)(Eq_53, Eq_54);

typedef Eq_6 (Eq_146)(Eq_54);

typedef Eq_6 (Eq_167)(Eq_52, Eq_53, Eq_54);

typedef int32 (Eq_183)(Eq_52, Eq_53, Eq_54);

typedef BOOL (Eq_196)(Eq_53, Eq_54);

typedef BOOL;

typedef BOOL (Eq_216)(Eq_52, Eq_53, Eq_54);

typedef BOOL;

typedef BOOL (Eq_225)(Eq_54);

typedef BOOL;

typedef UINT (Eq_257)(Eq_6, Eq_52, Eq_53, Eq_54);

typedef UINT;

typedef void (Eq_281)(word32);

typedef BOOL (Eq_297)(Eq_52, Eq_53, Eq_54);

typedef BOOL;

typedef BOOL (Eq_308)(Eq_54);

typedef BOOL;

typedef BOOL (Eq_319)(Eq_53, Eq_54);

typedef BOOL;

typedef LRESULT (Eq_336)(Eq_6, Eq_52, Eq_53, Eq_54);

typedef LRESULT;

typedef Eq_344 Eq_344struct Eq_344 {
	byte b0000;	// 0
	Eq_344 t0001;	// 1
} Eq_344;

typedef struct Eq_375 {
	byte b403198;	// 403198
} Eq_375;

typedef union Eq_399 {
	int32 u0;
	up32 u1;
} Eq_399;

typedef Eq_399 (Eq_400)(word32);

typedef struct Eq_410 {
	word32 dwFFFFFFF4;	// FFFFFFF4
} Eq_410;

typedef Eq_426 Eq_424struct Eq_424 {	// size: 1 1
	Eq_426 t0000;	// 0
} Eq_424;

typedef union Eq_426 {
	byte u0;
	word32 u1;
} Eq_426;

typedef Eq_426 Eq_429struct Eq_429 {	// size: 1 1
	Eq_426 t0000;	// 0
} Eq_429;

typedef void (Eq_443)();

typedef struct Eq_448 {
	word32 dw0000;	// 0
} Eq_448;

typedef union Eq_449 {
	ptr32 u0;
	word32 Eq_448::* u1;
} Eq_449;

typedef void (Eq_453)();

typedef Eq_426 Eq_456struct Eq_456 {	// size: 1 1
	Eq_426 t0000;	// 0
} Eq_456;

