;;; Segment .interp (0000000000000238)
0000000000000238                         2F 6C 69 62 36 34 2F 6C         /lib64/l
0000000000000240 64 2D 6C 69 6E 75 78 2D 78 38 36 2D 36 34 2E 73 d-linux-x86-64.s
0000000000000250 6F 2E 32 00                                     o.2.           
;;; Segment .note.ABI-tag (0000000000000254)
0000000000000254             04 00 00 00 10 00 00 00 01 00 00 00     ............
0000000000000260 47 4E 55 00 00 00 00 00 02 00 00 00 06 00 00 00 GNU.............
0000000000000270 20 00 00 00                                      ...           
;;; Segment .note.gnu.build-id (0000000000000274)
0000000000000274             04 00 00 00 14 00 00 00 03 00 00 00     ............
0000000000000280 47 4E 55 00 36 C3 C3 D8 CA 28 DC C5 25 FA 79 4C GNU.6....(..%.yL
0000000000000290 8F E2 2C AD 95 F1 0F 2E                         ..,.....       
;;; Segment .gnu.hash (0000000000000298)
0000000000000298                         01 00 00 00 01 00 00 00         ........
00000000000002A0 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000000000002B0 00 00 00 00                                     ....           
;;; Segment .dynsym (00000000000002B8)
;    0                                          00000000 00000000 00 
;    1 free                                     00000000 00000000 12 
;    2 _ITM_deregisterTMCloneTable              00000000 00000000 20 
;    3 printf                                   00000000 00000000 12 
;    4 __libc_start_main                        00000000 00000000 12 
;    5 __gmon_start__                           00000000 00000000 20 
;    6 malloc                                   00000000 00000000 12 
;    7 posix_memalign                           00000000 00000000 12 
;    8 _ITM_registerTMCloneTable                00000000 00000000 20 
;    9 __cxa_finalize                           00000000 00000000 22 
;;; Segment .dynstr (00000000000003A8)
00000000000003A8                         00 6C 69 62 63 2E 73 6F         .libc.so
00000000000003B0 2E 36 00 70 6F 73 69 78 5F 6D 65 6D 61 6C 69 67 .6.posix_memalig
00000000000003C0 6E 00 70 72 69 6E 74 66 00 6D 61 6C 6C 6F 63 00 n.printf.malloc.
00000000000003D0 5F 5F 63 78 61 5F 66 69 6E 61 6C 69 7A 65 00 5F __cxa_finalize._
00000000000003E0 5F 6C 69 62 63 5F 73 74 61 72 74 5F 6D 61 69 6E _libc_start_main
00000000000003F0 00 66 72 65 65 00 47 4C 49 42 43 5F 32 2E 32 2E .free.GLIBC_2.2.
0000000000000400 35 00 5F 49 54 4D 5F 64 65 72 65 67 69 73 74 65 5._ITM_deregiste
0000000000000410 72 54 4D 43 6C 6F 6E 65 54 61 62 6C 65 00 5F 5F rTMCloneTable.__
0000000000000420 67 6D 6F 6E 5F 73 74 61 72 74 5F 5F 00 5F 49 54 gmon_start__._IT
0000000000000430 4D 5F 72 65 67 69 73 74 65 72 54 4D 43 6C 6F 6E M_registerTMClon
0000000000000440 65 54 61 62 6C 65 00                            eTable.        
;;; Segment .gnu.version (0000000000000448)
0000000000000448                         00 00 02 00 00 00 02 00         ........
0000000000000450 02 00 00 00 02 00 02 00 00 00 02 00             ............   
;;; Segment .gnu.version_r (0000000000000460)
0000000000000460 01 00 01 00 01 00 00 00 10 00 00 00 00 00 00 00 ................
0000000000000470 75 1A 69 09 00 00 02 00 4E 00 00 00 00 00 00 00 u.i.....N.......
;;; Segment .rela.dyn (0000000000000480)
; 00200DE8   8 00000000 0000000000000720  (0)
; 00200DF0   8 00000000 00000000000006E0  (0)
; 00201040   8 00000000 0000000000201040  (0)
; 00200FD8   6 00000002 0000000000000000 _ITM_deregisterTMCloneTable (2)
; 00200FE0   6 00000004 0000000000000000 __libc_start_main (4)
; 00200FE8   6 00000005 0000000000000000 __gmon_start__ (5)
; 00200FF0   6 00000008 0000000000000000 _ITM_registerTMCloneTable (8)
; 00200FF8   6 00000009 0000000000000000 __cxa_finalize (9)
;;; Segment .rela.plt (0000000000000540)
; 00201018   7 00000001 0000000000000000 free (1)
; 00201020   7 00000003 0000000000000000 printf (3)
; 00201028   7 00000006 0000000000000000 malloc (6)
; 00201030   7 00000007 0000000000000000 posix_memalign (7)
;;; Segment .init (00000000000005A0)

;; _init: 00000000000005A0
_init proc
	sub	rsp,08
	mov	rax,[rip+00200A3D]                                     ; 0000000000200FE8
	test	rax,rax
	jz	00000000000005B2

l00000000000005B0:
	call	rax

l00000000000005B2:
	add	rsp,08
	ret
;;; Segment .plt (00000000000005C0)
00000000000005C0 FF 35 42 0A 20 00 FF 25 44 0A 20 00 0F 1F 40 00 .5B. ..%D. ...@.
00000000000005D0 FF 25 42 0A 20 00 68 00 00 00 00 E9 E0 FF FF FF .%B. .h.........
00000000000005E0 FF 25 3A 0A 20 00 68 01 00 00 00 E9 D0 FF FF FF .%:. .h.........
00000000000005F0 FF 25 32 0A 20 00 68 02 00 00 00 E9 C0 FF FF FF .%2. .h.........
0000000000000600 FF 25 2A 0A 20 00 68 03 00 00 00 E9 B0 FF FF FF .%*. .h.........
;;; Segment .plt.got (0000000000000610)
0000000000000610 FF 25 E2 09 20 00 66 90                         .%.. .f.       
;;; Segment .text (0000000000000620)

;; _start: 0000000000000620
_start proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,F0
	push	rax
	push	rsp
	lea	r8,[rip+000004AA]                                      ; 0000000000000AE0
	lea	rcx,[rip+00000433]                                     ; 0000000000000A70
	lea	rdi,[rip+00000254]                                     ; 0000000000000898
	call	qword ptr [rip+00200996]                              ; 0000000000200FE0
	hlt
000000000000064B                                  0F 1F 44 00 00            ..D..

;; deregister_tm_clones: 0000000000000650
deregister_tm_clones proc
	lea	rdi,[rip+002009F1]                                     ; 0000000000201048
	push	rbp
	lea	rax,[rip+002009E9]                                     ; 0000000000201048
	cmp	rax,rdi
	mov	rbp,rsp
	jz	0000000000000680

l0000000000000667:
	mov	rax,[rip+0020096A]                                     ; 0000000000200FD8
	test	rax,rax
	jz	0000000000000680

l0000000000000673:
	pop	rbp
	jmp	rax
0000000000000676                   66 2E 0F 1F 84 00 00 00 00 00       f.........

l0000000000000680:
	pop	rbp
	ret
0000000000000682       0F 1F 40 00 66 2E 0F 1F 84 00 00 00 00 00   ..@.f.........

;; register_tm_clones: 0000000000000690
register_tm_clones proc
	lea	rdi,[rip+002009B1]                                     ; 0000000000201048
	lea	rsi,[rip+002009AA]                                     ; 0000000000201048
	push	rbp
	sub	rsi,rdi
	mov	rbp,rsp
	sar	rsi,03
	mov	rax,rsi
	shr	rax,3F
	add	rsi,rax
	sar	rsi,01
	jz	00000000000006D0

l00000000000006B8:
	mov	rax,[rip+00200931]                                     ; 0000000000200FF0
	test	rax,rax
	jz	00000000000006D0

l00000000000006C4:
	pop	rbp
	jmp	rax
00000000000006C7                      66 0F 1F 84 00 00 00 00 00        f........

l00000000000006D0:
	pop	rbp
	ret
00000000000006D2       0F 1F 40 00 66 2E 0F 1F 84 00 00 00 00 00   ..@.f.........

;; __do_global_dtors_aux: 00000000000006E0
__do_global_dtors_aux proc
	cmp	byte ptr [rip+00200961],00                             ; 0000000000201048
	jnz	0000000000000718

l00000000000006E9:
	cmp	qword ptr [rip+00200907],00                            ; 0000000000200FF8
	push	rbp
	mov	rbp,rsp
	jz	0000000000000703

l00000000000006F7:
	mov	rdi,[rip+00200942]                                     ; 0000000000201040
	call	0000000000000610

l0000000000000703:
	call	0000000000000650
	mov	byte ptr [rip+00200939],01                             ; 0000000000201048
	pop	rbp
	ret
0000000000000711    0F 1F 80 00 00 00 00                          .......       

l0000000000000718:
	ret
000000000000071A                               66 0F 1F 44 00 00           f..D..

;; frame_dummy: 0000000000000720
frame_dummy proc
	push	rbp
	mov	rbp,rsp
	pop	rbp
	jmp	0000000000000690

;; _mm_malloc: 000000000000072A
_mm_malloc proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,20
	mov	[rbp-18],rdi
	mov	[rbp-20],rsi
	cmp	qword ptr [rbp-20],01
	jnz	000000000000074F

l0000000000000741:
	mov	rax,[rbp-18]
	mov	rdi,rax
	call	00000000000005F0
	jmp	000000000000078B

l000000000000074F:
	cmp	qword ptr [rbp-20],02
	jz	000000000000075D

l0000000000000756:
	cmp	qword ptr [rbp-20],04
	jnz	0000000000000765

l000000000000075D:
	mov	qword ptr [rbp-20],+00000008

l0000000000000765:
	mov	rdx,[rbp-18]
	mov	rcx,[rbp-20]
	lea	rax,[rbp-08]
	mov	rsi,rcx
	mov	rdi,rax
	call	0000000000000600
	test	eax,eax
	jnz	0000000000000786

l0000000000000780:
	mov	rax,[rbp-08]
	jmp	000000000000078B

l0000000000000786:
	mov	eax,00000000

l000000000000078B:
	leave
	ret

;; _mm_free: 000000000000078D
_mm_free proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,10
	mov	[rbp-08],rdi
	mov	rax,[rbp-08]
	mov	rdi,rax
	call	00000000000005D0
	nop
	leave
	ret

;; vec_add: 00000000000007A8
vec_add proc
	lea	r10,[rsp+08]
	and	rsp,E0
	push	qword ptr [rdx-08]
	push	rbp
	mov	rbp,rsp
	push	r10
	sub	rsp,30
	mov	[rbp-00000098],rdi
	mov	[rbp-000000A0],rsi
	mov	[rbp-000000A8],rdx
	mov	[rbp-000000B0],rcx
	mov	rcx,[rip+0000031E]                                     ; 0000000000000B00
	mov	rax,[rbp-00000098]
	mov	edx,00000000
	div	rcx
	mov	[rbp-20],rax
	mov	rax,[rbp-000000A0]
	mov	[rbp-28],rax
	mov	rax,[rbp-000000A8]
	mov	[rbp-30],rax
	mov	rax,[rbp-000000B0]
	mov	[rbp-38],rax
	mov	qword ptr [rbp-18],+00000000
	jmp	0000000000000881

l0000000000000820:
	mov	rax,[rbp-18]
	shl	rax,05
	mov	rdx,rax
	mov	rax,[rbp-38]
	add	rax,rdx
	vmovapd	ymm0,[rax]
	mov	rax,[rbp-18]
	shl	rax,05
	mov	rdx,rax
	mov	rax,[rbp-30]
	add	rax,rdx
	vmovapd	ymm1,[rax]
	mov	rax,[rbp-18]
	shl	rax,05
	mov	rdx,rax
	mov	rax,[rbp-28]
	add	rax,rdx
	vmovapd	[rbp-70],ymm1
	vmovapd	[rbp-00000090],ymm0
	vmovapd	ymm0,[rbp-70]
	vaddpd	ymm0,ymm0,[rbp-00000090]
	vmovapd	[rax],ymm0
	add	qword ptr [rbp-18],01

l0000000000000881:
	mov	rax,[rbp-18]
	cmp	rax,[rbp-20]
	jc	0000000000000820

l000000000000088B:
	nop
	add	rsp,30
	pop	r10
	pop	rbp
	lea	rsp,[rdx-08]
	ret

;; main: 0000000000000898
main proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,50
	mov	qword ptr [rbp-28],+00000400
	mov	rax,[rbp-28]
	shl	rax,03
	mov	esi,00000020
	mov	rdi,rax
	call	000000000000072A
	mov	[rbp-30],rax
	mov	rax,[rbp-28]
	shl	rax,03
	mov	esi,00000020
	mov	rdi,rax
	call	000000000000072A
	mov	[rbp-38],rax
	mov	rax,[rbp-28]
	shl	rax,03
	mov	esi,00000020
	mov	rdi,rax
	call	000000000000072A
	mov	[rbp-40],rax
	mov	qword ptr [rbp-08],+00000000
	jmp	000000000000093F

l00000000000008FD:
	mov	rax,[rbp-08]
	lea	rdx,[00000000+rax*8]
	mov	rax,[rbp-30]
	lea	rcx,[rdx+rax]
	mov	rax,[rbp-08]
	test	rax,rax
	js	0000000000000921

l000000000000091A:
	vcvtsi2sd	xmm0,xmm0,rax
	jmp	0000000000000936

l0000000000000921:
	mov	rdx,rax
	shr	rdx,01
	and	eax,01
	or	rdx,rax
	vcvtsi2sd	xmm0,xmm0,rdx
	vaddsd	xmm0,xmm0,xmm0

l0000000000000936:
	vmovsd	double ptr [rcx],xmm0
	add	qword ptr [rbp-08],01

l000000000000093F:
	mov	rax,[rbp-08]
	cmp	rax,[rbp-28]
	jc	00000000000008FD

l0000000000000949:
	mov	qword ptr [rbp-10],+00000000
	jmp	0000000000000998

l0000000000000953:
	mov	rax,[rbp-10]
	add	rax,01
	mov	rdx,[rbp-10]
	lea	rcx,[00000000+rdx*8]
	mov	rdx,[rbp-38]
	add	rcx,rdx
	test	rax,rax
	js	000000000000097A

l0000000000000973:
	vcvtsi2sd	xmm0,xmm0,rax
	jmp	000000000000098F

l000000000000097A:
	mov	rdx,rax
	shr	rdx,01
	and	eax,01
	or	rdx,rax
	vcvtsi2sd	xmm0,xmm0,rdx
	vaddsd	xmm0,xmm0,xmm0

l000000000000098F:
	vmovsd	double ptr [rcx],xmm0
	add	qword ptr [rbp-10],01

l0000000000000998:
	mov	rax,[rbp-10]
	cmp	rax,[rbp-28]
	jc	0000000000000953

l00000000000009A2:
	mov	qword ptr [rbp-18],+00000000
	jmp	00000000000009CC

l00000000000009AC:
	mov	rax,[rbp-18]
	lea	rdx,[00000000+rax*8]
	mov	rax,[rbp-40]
	add	rax,rdx
	vxorpd	xmm0,xmm0,xmm0
	vmovsd	double ptr [rax],xmm0
	add	qword ptr [rbp-18],01

l00000000000009CC:
	mov	rax,[rbp-18]
	cmp	rax,[rbp-28]
	jc	00000000000009AC

l00000000000009D6:
	mov	rcx,[rbp-38]
	mov	rdx,[rbp-30]
	mov	rsi,[rbp-40]
	mov	rax,[rbp-28]
	mov	rdi,rax
	call	00000000000007A8
	mov	qword ptr [rbp-20],+00000000
	jmp	0000000000000A2D

l00000000000009F8:
	mov	rax,[rbp-20]
	lea	rdx,[00000000+rax*8]
	mov	rax,[rbp-40]
	add	rax,rdx
	mov	rax,[rax]
	mov	[rbp-48],rax
	vmovsd	xmm0,double ptr [rbp-48]
	lea	rdi,[rip+000000DA]                                     ; 0000000000000AF8
	mov	eax,00000001
	call	00000000000005E0
	add	qword ptr [rbp-20],01

l0000000000000A2D:
	mov	rax,[rbp-20]
	cmp	rax,[rbp-28]
	jc	00000000000009F8

l0000000000000A37:
	mov	rax,[rbp-30]
	mov	rdi,rax
	call	000000000000078D
	mov	rax,[rbp-38]
	mov	rdi,rax
	call	000000000000078D
	mov	rax,[rbp-40]
	mov	rdi,rax
	call	000000000000078D
	mov	eax,00000000
	leave
	ret
0000000000000A62       66 2E 0F 1F 84 00 00 00 00 00 0F 1F 40 00   f...........@.

;; __libc_csu_init: 0000000000000A70
__libc_csu_init proc
	push	r15
	push	r14
	mov	r15d,edi
	push	r13
	push	r12
	lea	r12,[rip+00200366]                                     ; 0000000000200DE8
	push	rbp
	lea	rbp,[rip+00200366]                                     ; 0000000000200DF0
	push	rbx
	mov	r14,rsi
	mov	r13,rdx
	sub	rbp,r12
	sub	rsp,08
	sar	rbp,03
	call	00000000000005A0
	test	rbp,rbp
	jz	0000000000000AC6

l0000000000000AA6:
	xor	ebx,ebx
	nop	dword ptr [rax+rax+00000000]

l0000000000000AB0:
	mov	rdx,r13
	mov	rsi,r14
	mov	edi,r15d
	call	qword ptr [r12+rbx*8]
	add	rbx,01
	cmp	rbp,rbx
	jnz	0000000000000AB0

l0000000000000AC6:
	add	rsp,08
	pop	rbx
	pop	rbp
	pop	r12
	pop	r13
	pop	r14
	pop	r15
	ret
0000000000000AD5                90 66 2E 0F 1F 84 00 00 00 00 00      .f.........

;; __libc_csu_fini: 0000000000000AE0
__libc_csu_fini proc
	ret
;;; Segment .fini (0000000000000AE4)

;; _fini: 0000000000000AE4
_fini proc
	sub	rsp,08
	add	rsp,08
	ret
;;; Segment .rodata (0000000000000AF0)
0000000000000AF0 01 00 02 00                                     ....           
0000000000000AF4             00 00 00 00 25 67 0A 00 00 00 00 00     ....%g......
0000000000000B00 04 00 00 00 00 00 00 00                         ........       
;;; Segment .eh_frame_hdr (0000000000000B08)
0000000000000B08                         01 1B 03 3B 54 00 00 00         ...;T...
0000000000000B10 09 00 00 00 B8 FA FF FF A0 00 00 00 08 FB FF FF ................
0000000000000B20 C8 00 00 00 18 FB FF FF 70 00 00 00 22 FC FF FF ........p..."...
0000000000000B30 E0 00 00 00 85 FC FF FF 00 01 00 00 A0 FC FF FF ................
0000000000000B40 20 01 00 00 90 FD FF FF 4C 01 00 00 68 FF FF FF  .......L...h...
0000000000000B50 70 01 00 00 D8 FF FF FF B8 01 00 00             p...........   
;;; Segment .eh_frame (0000000000000B60)
0000000000000B60 14 00 00 00 00 00 00 00 01 7A 52 00 01 78 10 01 .........zR..x..
0000000000000B70 1B 0C 07 08 90 01 07 10 14 00 00 00 1C 00 00 00 ................
0000000000000B80 A0 FA FF FF 2B 00 00 00 00 00 00 00 00 00 00 00 ....+...........
0000000000000B90 14 00 00 00 00 00 00 00 01 7A 52 00 01 78 10 01 .........zR..x..
0000000000000BA0 1B 0C 07 08 90 01 00 00 24 00 00 00 1C 00 00 00 ........$.......
0000000000000BB0 10 FA FF FF 50 00 00 00 00 0E 10 46 0E 18 4A 0F ....P......F..J.
0000000000000BC0 0B 77 08 80 00 3F 1A 3B 2A 33 24 22 00 00 00 00 .w...?.;*3$"....
0000000000000BD0 14 00 00 00 44 00 00 00 38 FA FF FF 08 00 00 00 ....D...8.......
0000000000000BE0 00 00 00 00 00 00 00 00 1C 00 00 00 5C 00 00 00 ............\...
0000000000000BF0 3A FB FF FF 63 00 00 00 00 41 0E 10 86 02 43 0D :...c....A....C.
0000000000000C00 06 02 5E 0C 07 08 00 00 1C 00 00 00 7C 00 00 00 ..^.........|...
0000000000000C10 7D FB FF FF 1B 00 00 00 00 41 0E 10 86 02 43 0D }........A....C.
0000000000000C20 06 56 0C 07 08 00 00 00 28 00 00 00 9C 00 00 00 .V......(.......
0000000000000C30 78 FB FF FF F0 00 00 00 00 45 0C 0A 00 49 10 06 x........E...I..
0000000000000C40 02 76 00 45 0F 03 76 78 06 02 D7 0C 0A 00 45 0C .v.E..vx......E.
0000000000000C50 07 08 00 00 20 00 00 00 C8 00 00 00 3C FC FF FF .... .......<...
0000000000000C60 CA 01 00 00 00 41 0E 10 86 02 43 0D 06 03 C5 01 .....A....C.....
0000000000000C70 0C 07 08 00 00 00 00 00 44 00 00 00 EC 00 00 00 ........D.......
0000000000000C80 F0 FD FF FF 65 00 00 00 00 42 0E 10 8F 02 42 0E ....e....B....B.
0000000000000C90 18 8E 03 45 0E 20 8D 04 42 0E 28 8C 05 48 0E 30 ...E. ..B.(..H.0
0000000000000CA0 86 06 48 0E 38 83 07 4D 0E 40 72 0E 38 41 0E 30 ..H.8..M.@r.8A.0
0000000000000CB0 41 0E 28 42 0E 20 42 0E 18 42 0E 10 42 0E 08 00 A.(B. B..B..B...
0000000000000CC0 10 00 00 00 34 01 00 00 18 FE FF FF 02 00 00 00 ....4...........
0000000000000CD0 00 00 00 00 00 00 00 00                         ........       
;;; Segment .init_array (0000000000200DE8)
0000000000200DE8                         20 07 00 00 00 00 00 00          .......
;;; Segment .fini_array (0000000000200DF0)
0000000000200DF0 E0 06 00 00 00 00 00 00                         ........       
;;; Segment .dynamic (0000000000200DF8)
; DT_NEEDED       libc.so.6
; DT_INIT         00000000000005A0
; DT_DEBUG        0000000000000AE4
; DT_INIT_ARRAY   0000000000200DE8
; DT_INIT_ARRAYSZ 0000000000000008
; DT_FINI_ARRAY   0000000000200DF0
; DT_FINI_ARRAYSZ 0000000000000008
; 6FFFFEF5        0000000000000298
; DT_STRTAB       00000000000003A8
; DT_SYMTAB       00000000000002B8
; DT_STRSZ        000000000000009F
; DT_SYMENT                     24
; DT_DEBUG        0000000000000000
; DT_PLTGOT       0000000000201000
; DT_PLTRELSZ                   96
; DT_PLTREL       0000000000000007
; DT_JMPREL       0000000000000540
; DT_RELA         0000000000000480
; DT_RELASZ                    192
; DT_RELAENT                    24
; 6FFFFFFB        0000000008000000
; 6FFFFFFE        0000000000000460
; 6FFFFFFF        0000000000000001
; 6FFFFFF0        0000000000000448
; 6FFFFFF9        0000000000000003
;;; Segment .got (0000000000200FD8)
_ITM_deregisterTMCloneTable_GOT		; 0000000000200FD8
	dq	0x0000000000000000
__libc_start_main_GOT		; 0000000000200FE0
	dq	0x0000000000000000
__gmon_start___GOT		; 0000000000200FE8
	dq	0x0000000000000000
_ITM_registerTMCloneTable_GOT		; 0000000000200FF0
	dq	0x0000000000000000
__cxa_finalize_GOT		; 0000000000200FF8
	dq	0x0000000000000000
;;; Segment .got.plt (0000000000201000)
0000000000201000 F8 0D 20 00 00 00 00 00 00 00 00 00 00 00 00 00 .. .............
0000000000201010 00 00 00 00 00 00 00 00 D6 05 00 00 00 00 00 00 ................
0000000000201020 E6 05 00 00 00 00 00 00 F6 05 00 00 00 00 00 00 ................
0000000000201030 06 06 00 00 00 00 00 00                         ........       
;;; Segment .data (0000000000201038)
0000000000201038                         00 00 00 00 00 00 00 00         ........
0000000000201040 40 10 20 00 00 00 00 00                         @. .....       
;;; Segment .bss (0000000000201048)
0000000000201048                         00 00 00 00 00 00 00 00         ........
