;;; Segment .interp (0000000000400238)
0000000000400238                         2F 6C 69 62 36 34 2F 6C         /lib64/l
0000000000400240 64 2D 6C 69 6E 75 78 2D 78 38 36 2D 36 34 2E 73 d-linux-x86-64.s
0000000000400250 6F 2E 32 00                                     o.2.           
;;; Segment .note.ABI-tag (0000000000400254)
0000000000400254             04 00 00 00 10 00 00 00 01 00 00 00     ............
0000000000400260 47 4E 55 00 00 00 00 00 02 00 00 00 06 00 00 00 GNU.............
0000000000400270 20 00 00 00                                      ...           
;;; Segment .hash (0000000000400278)
0000000000400278                         03 00 00 00 06 00 00 00         ........
0000000000400280 05 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 ................
0000000000400290 00 00 00 00 01 00 00 00 02 00 00 00 03 00 00 00 ................
00000000004002A0 00 00 00 00                                     ....           
;;; Segment .dynsym (00000000004002A8)
;    0                                          00000000 00000000 00 
;    1 free                                     00000000 00000000 12 
;    2 puts                                     00000000 00000000 12 
;    3 __libc_start_main                        00000000 00000000 12 
;    4 calloc                                   00000000 00000000 12 
;    5 __gmon_start__                           00000000 00000000 20 
;;; Segment .dynstr (0000000000400338)
0000000000400338                         00 6C 69 62 63 2E 73 6F         .libc.so
0000000000400340 2E 36 00 70 75 74 73 00 63 61 6C 6C 6F 63 00 5F .6.puts.calloc._
0000000000400350 5F 6C 69 62 63 5F 73 74 61 72 74 5F 6D 61 69 6E _libc_start_main
0000000000400360 00 66 72 65 65 00 47 4C 49 42 43 5F 32 2E 32 2E .free.GLIBC_2.2.
0000000000400370 35 00 5F 5F 67 6D 6F 6E 5F 73 74 61 72 74 5F 5F 5.__gmon_start__
0000000000400380 00                                              .              
;;; Segment .gnu.version (0000000000400382)
0000000000400382       00 00 02 00 02 00 02 00 02 00 00 00         ............ 
;;; Segment .gnu.version_r (0000000000400390)
0000000000400390 01 00 01 00 01 00 00 00 10 00 00 00 00 00 00 00 ................
00000000004003A0 75 1A 69 09 00 00 02 00 2E 00 00 00 00 00 00 00 u.i.............
;;; Segment .rela.dyn (00000000004003B0)
; 00600FF0   6 00000003 0000000000000000 __libc_start_main (3)
; 00600FF8   6 00000005 0000000000000000 __gmon_start__ (5)
;;; Segment .rela.plt (00000000004003E0)
; 00601018   7 00000001 0000000000000000 free (1)
; 00601020   7 00000002 0000000000000000 puts (2)
; 00601028   7 00000004 0000000000000000 calloc (4)
;;; Segment .init (0000000000400428)

;; _init: 0000000000400428
_init proc
	sub	rsp,08
	mov	rax,[rip+00200BC5]                                     ; 0000000000600FF8
	test	rax,rax
	jz	000000000040043A

l0000000000400438:
	call	eax

l000000000040043A:
	add	rsp,08
	ret
;;; Segment .plt (0000000000400440)
0000000000400440 FF 35 C2 0B 20 00 FF 25 C4 0B 20 00 0F 1F 40 00 .5.. ..%.. ...@.
0000000000400450 FF 25 C2 0B 20 00                               .%.. .         
0000000000400456                   68 00 00 00 00 E9 E0 FF FF FF       h.........
0000000000400460 FF 25 BA 0B 20 00                               .%.. .         
0000000000400466                   68 01 00 00 00 E9 D0 FF FF FF       h.........
0000000000400470 FF 25 B2 0B 20 00                               .%.. .         
0000000000400476                   68 02 00 00 00 E9 C0 FF FF FF       h.........
;;; Segment .text (0000000000400480)

;; _start: 0000000000400480
_start proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,F0
	push	rax
	push	rsp
	mov	r8,+00400780
	mov	rcx,+00400710
	mov	rdi,+00400660
	call	dword ptr [rip+00200B46]                              ; 0000000000600FF0
	hlt
00000000004004AB                                  0F 1F 44 00 00            ..D..

;; deregister_tm_clones: 00000000004004B0
deregister_tm_clones proc
	push	rbp
	mov	eax,00601040
	cmp	r8,+00601040
	mov	rbp,rsp
	jz	00000000004004D8

l00000000004004C1:
	mov	eax,00000000
	test	rax,rax
	jz	00000000004004D8

l00000000004004CB:
	pop	rbp
	mov	edi,00601040
	jmp	eax
00000000004004D3          0F 1F 44 00 00                            ..D..       

l00000000004004D8:
	pop	rbp
	ret
00000000004004DA                               66 0F 1F 44 00 00           f..D..

;; register_tm_clones: 00000000004004E0
register_tm_clones proc
	mov	esi,00601040
	push	rbp
	sub	rsi,+00601040
	mov	rbp,rsp
	sar	rsi,03
	mov	rax,rsi
	shr	rax,3F
	add	rsi,rax
	sar	rsi,01
	jz	0000000000400518

l0000000000400503:
	mov	eax,00000000
	test	rax,rax
	jz	0000000000400518

l000000000040050D:
	pop	rbp
	mov	edi,00601040
	jmp	eax
0000000000400515                0F 1F 00                              ...       

l0000000000400518:
	pop	rbp
	ret
000000000040051A                               66 0F 1F 44 00 00           f..D..

;; __do_global_dtors_aux: 0000000000400520
__do_global_dtors_aux proc
	cmp	byte ptr [rip+00200B19],00                             ; 0000000000601040
	jnz	0000000000400540

l0000000000400529:
	push	rbp
	mov	rbp,rsp
	call	00000000004004B0
	mov	byte ptr [rip+00200B07],01                             ; 0000000000601040
	pop	rbp
	ret
000000000040053B                                  0F 1F 44 00 00            ..D..

l0000000000400540:
	ret
0000000000400542       0F 1F 40 00 66 2E 0F 1F 84 00 00 00 00 00   ..@.f.........

;; frame_dummy: 0000000000400550
frame_dummy proc
	push	rbp
	mov	rbp,rsp
	pop	rbp
	jmp	00000000004004E0
0000000000400557                      66 0F 1F 84 00 00 00 00 00        f........

;; my1: 0000000000400560
my1 proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,10
	mov	[rbp-04],edi
	mov	[rbp-08],esi
	movsx	rdi,dword ptr [rbp-04]
	movsx	rsi,dword ptr [rbp-08]
	call	0000000000400470
	mov	[rbp-10],rax
	mov	rax,[rbp-10]
	add	rsp,10
	pop	rbp
	ret
0000000000400589                            0F 1F 80 00 00 00 00          .......

;; my2: 0000000000400590
my2 proc
	push	rbp
	mov	rbp,rsp
	mov	al,sil
	mov	esi,00000001
	mov	[rbp-08],rdi
	mov	[rbp-09],al
	mov	rdi,[rbp-08]
	mov	al,[rbp-09]
	mov	[rdi],al
	mov	eax,esi
	pop	rbp
	ret

;; branches: 00000000004005B0
branches proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,20
	mov	[rbp-08],edi
	mov	[rbp-0C],esi
	mov	esi,[rbp-08]
	cmp	esi,[rbp-0C]
	jge	000000000040064F

l00000000004005CA:
	mov	eax,[rbp-08]
	shl	eax,01
	mov	ecx,[rbp-0C]
	shl	ecx,01
	cmp	eax,ecx
	jge	000000000040064F

l00000000004005DE:
	imul	eax,[rbp-08],03
	imul	ecx,[rbp-0C],03
	cmp	eax,ecx
	jge	000000000040064F

l00000000004005EE:
	mov	eax,[rbp-08]
	shl	eax,02
	mov	ecx,[rbp-0C]
	shl	ecx,02
	cmp	eax,ecx
	jge	000000000040064F

l0000000000400602:
	mov	eax,00000002
	mov	ecx,[rbp-08]
	mov	[rbp-1C],eax
	mov	eax,ecx
	cdq
	mov	ecx,[rbp-1C]
	idiv	ecx
	mov	esi,[rbp-0C]
	mov	[rbp-20],eax
	mov	eax,esi
	cdq
	idiv	ecx
	mov	esi,[rbp-20]
	cmp	esi,eax
	jge	000000000040064F

l000000000040062B:
	mov	edi,[rbp-08]
	mov	esi,[rbp-0C]
	call	0000000000400560
	mov	[rbp-18],rax
	mov	rdi,[rbp-18]
	call	0000000000400450
	mov	dword ptr [rbp-04],00000000
	jmp	0000000000400656

l000000000040064F:
	mov	dword ptr [rbp-04],FFFFFFFF

l0000000000400656:
	mov	eax,[rbp-04]
	add	rsp,20
	pop	rbp
	ret
000000000040065F                                              90                .

;; main: 0000000000400660
main proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,40
	lea	rax,[rbp-20]
	mov	dword ptr [rbp-04],00000000
	mov	[rbp-08],edi
	mov	[rbp-10],rsi
	mov	rsi,[00400798]
	mov	[rbp-20],rsi
	mov	rsi,[004007A0]
	mov	[rbp-18],rsi
	mov	[rbp-28],rax
	mov	rax,[rbp-28]
	mov	r11,[r8]
	mov	edi,00000001
	mov	esi,00000005
	call	00000000004006F0
	mov	[rbp-30],rax
	mov	rax,[rbp-28]
	mov	r11,[r8+08]
	mov	rdi,[rbp-30]
	mov	esi,00000078
	call	00000000004006F0
	mov	esi,004007A8
	mov	edi,esi
	mov	[rbp-34],eax
	call	0000000000400460
	mov	rdi,[rbp-30]
	mov	[rbp-38],eax
	call	0000000000400450
	xor	eax,eax
	add	rsp,40
	pop	rbp
	ret
00000000004006E9                            0F 1F 80 00 00 00 00          .......

;; __llvm_retpoline_r11: 00000000004006F0
__llvm_retpoline_r11 proc
	call	0000000000400700

l00000000004006F5:
	pause
	jmp	00000000004006F5
00000000004006FC                                     0F 1F 40 00             ..@.

;; fn0000000000400700: 0000000000400700
fn0000000000400700 proc
	mov	[rsp],r11
	ret
0000000000400705                66 2E 0F 1F 84 00 00 00 00 00 90      f..........

;; __libc_csu_init: 0000000000400710
__libc_csu_init proc
	push	r15
	push	r14
	mov	r15d,edi
	push	r13
	push	r12
	lea	r12,[rip+002006EE]                                     ; 0000000000600E10
	push	rbp
	lea	rbp,[rip+002006EE]                                     ; 0000000000600E18
	push	rbx
	mov	r14,rsi
	mov	r13,rdx
	sub	rbp,r12
	sub	rsp,08
	sar	rbp,03
	call	0000000000400428
	test	rbp,rbp
	jz	0000000000400766

l0000000000400746:
	xor	ebx,ebx
	nop	dword ptr [rax+rax+00000000]

l0000000000400750:
	mov	rdx,r13
	mov	rsi,r14
	mov	edi,r15d
	call	dword ptr [r12+rbx*8]
	add	rbx,01
	cmp	rbp,rbx
	jnz	0000000000400750

l0000000000400766:
	add	rsp,08
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
0000000000400775                90 66 2E 0F 1F 84 00 00 00 00 00      .f.........

;; __libc_csu_fini: 0000000000400780
__libc_csu_fini proc
	ret
;;; Segment .fini (0000000000400784)

;; _fini: 0000000000400784
_fini proc
	sub	rsp,08
	add	rsp,08
	ret
;;; Segment .rodata (0000000000400790)
0000000000400790 01 00 02 00                                     ....           
0000000000400794             00 00 00 00 60 05 40 00 00 00 00 00     ....`.@.....
00000000004007A0 90 05 40 00 00 00 00 00 64 6F 6E 65 0A 00       ..@.....done.. 
;;; Segment .eh_frame_hdr (00000000004007B0)
00000000004007B0 01 1B 03 3B 4C 00 00 00 08 00 00 00 90 FC FF FF ...;L...........
00000000004007C0 98 00 00 00 D0 FC FF FF 68 00 00 00 B0 FD FF FF ........h.......
00000000004007D0 C0 00 00 00 E0 FD FF FF DC 00 00 00 00 FE FF FF ................
00000000004007E0 F8 00 00 00 B0 FE FF FF 14 01 00 00 60 FF FF FF ............`...
00000000004007F0 30 01 00 00 D0 FF FF FF 78 01 00 00             0.......x...   
;;; Segment .eh_frame (0000000000400800)
0000000000400800 14 00 00 00 00 00 00 00 01 7A 52 00 01 78 10 01 .........zR..x..
0000000000400810 1B 0C 07 08 90 01 07 10 14 00 00 00 1C 00 00 00 ................
0000000000400820 60 FC FF FF 2B 00 00 00 00 00 00 00 00 00 00 00 `...+...........
0000000000400830 14 00 00 00 00 00 00 00 01 7A 52 00 01 78 10 01 .........zR..x..
0000000000400840 1B 0C 07 08 90 01 00 00 24 00 00 00 1C 00 00 00 ........$.......
0000000000400850 F0 FB FF FF 40 00 00 00 00 0E 10 46 0E 18 4A 0F ....@......F..J.
0000000000400860 0B 77 08 80 00 3F 1A 3B 2A 33 24 22 00 00 00 00 .w...?.;*3$"....
0000000000400870 18 00 00 00 44 00 00 00 E8 FC FF FF 29 00 00 00 ....D.......)...
0000000000400880 00 41 0E 10 86 02 43 0D 06 00 00 00 18 00 00 00 .A....C.........
0000000000400890 60 00 00 00 FC FC FF FF 20 00 00 00 00 41 0E 10 `....... ....A..
00000000004008A0 86 02 43 0D 06 00 00 00 18 00 00 00 7C 00 00 00 ..C.........|...
00000000004008B0 00 FD FF FF AF 00 00 00 00 41 0E 10 86 02 43 0D .........A....C.
00000000004008C0 06 00 00 00 18 00 00 00 98 00 00 00 94 FD FF FF ................
00000000004008D0 89 00 00 00 00 41 0E 10 86 02 43 0D 06 00 00 00 .....A....C.....
00000000004008E0 44 00 00 00 B4 00 00 00 28 FE FF FF 65 00 00 00 D.......(...e...
00000000004008F0 00 42 0E 10 8F 02 42 0E 18 8E 03 45 0E 20 8D 04 .B....B....E. ..
0000000000400900 42 0E 28 8C 05 48 0E 30 86 06 48 0E 38 83 07 4D B.(..H.0..H.8..M
0000000000400910 0E 40 72 0E 38 41 0E 30 41 0E 28 42 0E 20 42 0E .@r.8A.0A.(B. B.
0000000000400920 18 42 0E 10 42 0E 08 00 10 00 00 00 FC 00 00 00 .B..B...........
0000000000400930 50 FE FF FF 02 00 00 00 00 00 00 00 00 00 00 00 P...............
;;; Segment .init_array (0000000000600E10)
0000000000600E10 50 05 40 00 00 00 00 00                         P.@.....       
;;; Segment .fini_array (0000000000600E18)
0000000000600E18                         20 05 40 00 00 00 00 00          .@.....
;;; Segment .dynamic (0000000000600E20)
; DT_NEEDED       libc.so.6
; DT_INIT         0000000000400428
; DT_DEBUG        0000000000400784
; DT_INIT_ARRAY   0000000000600E10
; DT_INIT_ARRAYSZ 0000000000000008
; DT_FINI_ARRAY   0000000000600E18
; DT_FINI_ARRAYSZ 0000000000000008
; DT_HASH         0000000000400278
; DT_STRTAB       0000000000400338
; DT_SYMTAB       00000000004002A8
; DT_STRSZ        0000000000000049
; DT_SYMENT                     24
; DT_DEBUG        0000000000000000
; DT_PLTGOT       0000000000601000
; DT_PLTRELSZ                   72
; DT_PLTREL       0000000000000007
; DT_JMPREL       00000000004003E0
; DT_RELA         00000000004003B0
; DT_RELASZ                     48
; DT_RELAENT                    24
; 6FFFFFFE        0000000000400390
; 6FFFFFFF        0000000000000001
; 6FFFFFF0        0000000000400382
;;; Segment .got (0000000000600FF0)
__libc_start_main_GOT		; 0000000000600FF0
	dq	0x0000000000000000
__gmon_start___GOT		; 0000000000600FF8
	dq	0x0000000000000000
;;; Segment .got.plt (0000000000601000)
0000000000601000 20 0E 60 00 00 00 00 00 00 00 00 00 00 00 00 00  .`.............
0000000000601010 00 00 00 00 00 00 00 00                         ........       
free_GOT		; 0000000000601018
	dq	0x0000000000400456
puts_GOT		; 0000000000601020
	dq	0x0000000000400466
calloc_GOT		; 0000000000601028
	dq	0x0000000000400476
;;; Segment .data (0000000000601030)
0000000000601030 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
;;; Segment .bss (0000000000601040)
0000000000601040 00 00 00 00 00 00 00 00                         ........       
