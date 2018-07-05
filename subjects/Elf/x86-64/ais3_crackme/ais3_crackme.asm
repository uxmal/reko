;;; Segment .interp (0000000000400238)
0000000000400238                         2F 6C 69 62 36 34 2F 6C         /lib64/l
0000000000400240 64 2D 6C 69 6E 75 78 2D 78 38 36 2D 36 34 2E 73 d-linux-x86-64.s
0000000000400250 6F 2E 32 00                                     o.2.           
;;; Segment .note.ABI-tag (0000000000400254)
0000000000400254             04 00 00 00 10 00 00 00 01 00 00 00     ............
0000000000400260 47 4E 55 00 00 00 00 00 02 00 00 00 06 00 00 00 GNU.............
0000000000400270 18 00 00 00                                     ....           
;;; Segment .note.gnu.build-id (0000000000400274)
0000000000400274             04 00 00 00 14 00 00 00 03 00 00 00     ............
0000000000400280 47 4E 55 00 24 8B CF 6D 61 ED EE A9 57 45 E2 4B GNU.$..ma...WE.K
0000000000400290 7E 4C 41 3C 95 3D B1 E5                         ~LA<.=..       
;;; Segment .gnu.hash (0000000000400298)
0000000000400298                         01 00 00 00 01 00 00 00         ........
00000000004002A0 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
00000000004002B0 00 00 00 00                                     ....           
;;; Segment .dynsym (00000000004002B8)
;    0                                          00000000 00000000 00 
;    1 puts                                     00000000 00000000 12 
;    2 __libc_start_main                        00000000 00000000 12 
;    3 __gmon_start__                           00000000 00000000 20 
;;; Segment .dynstr (0000000000400318)
0000000000400318                         00 5F 5F 67 6D 6F 6E 5F         .__gmon_
0000000000400320 73 74 61 72 74 5F 5F 00 6C 69 62 63 2E 73 6F 2E start__.libc.so.
0000000000400330 36 00 70 75 74 73 00 5F 5F 6C 69 62 63 5F 73 74 6.puts.__libc_st
0000000000400340 61 72 74 5F 6D 61 69 6E 00 47 4C 49 42 43 5F 32 art_main.GLIBC_2
0000000000400350 2E 32 2E 35 00                                  .2.5.          
;;; Segment .gnu.version (0000000000400356)
0000000000400356                   00 00 02 00 02 00 00 00             ........ 
;;; Segment .gnu.version_r (0000000000400360)
0000000000400360 01 00 01 00 10 00 00 00 10 00 00 00 00 00 00 00 ................
0000000000400370 75 1A 69 09 00 00 02 00 31 00 00 00 00 00 00 00 u.i.....1.......
;;; Segment .rela.dyn (0000000000400380)
; 00600FE0   6 00000003 0000000000000000 __gmon_start__ (3)
;;; Segment .rela.plt (0000000000400398)
; 00601000   7 00000001 0000000000000000 puts (1)
; 00601008   7 00000002 0000000000000000 __libc_start_main (2)
;;; Segment .init (00000000004003C8)

;; _init: 00000000004003C8
_init proc
	sub	rsp,08
	call	000000000040043C
	add	rsp,08
	ret
;;; Segment .plt (00000000004003E0)
00000000004003E0 FF 35 0A 0C 20 00 FF 25 0C 0C 20 00 0F 1F 40 00 .5.. ..%.. ...@.
00000000004003F0 FF 25 0A 0C 20 00                               .%.. .         
00000000004003F6                   68 00 00 00 00 E9 E0 FF FF FF       h.........
0000000000400400 FF 25 02 0C 20 00                               .%.. .         
0000000000400406                   68 01 00 00 00 E9 D0 FF FF FF       h.........
;;; Segment .text (0000000000400410)

;; _start: 0000000000400410
_start proc
	xor	ebp,ebp
	mov	r9,rdx
	pop	rsi
	mov	rdx,rsp
	and	rsp,F0
	push	rax
	push	rsp
	mov	r8,+004006B0
	mov	rcx,+00400620
	mov	rdi,+004005C5
	call	0000000000400400
	hlt
000000000040043A                               90 90                       ..   

;; call_gmon_start: 000000000040043C
call_gmon_start proc
	sub	rsp,08
	mov	rax,[rip+00200B99]                                     ; 0000000000600FE0
	test	rax,rax
	jz	000000000040044E

l000000000040044C:
	call	rax

l000000000040044E:
	add	rsp,08
	ret
0000000000400453          90 90 90 90 90 90 90 90 90 90 90 90 90    .............

;; deregister_tm_clones: 0000000000400460
deregister_tm_clones proc
	mov	eax,0060103F
	push	rbp
	sub	r8,+00601038
	cmp	rax,0E
	mov	rbp,rsp
	ja	0000000000400477

l0000000000400475:
	pop	rbp
	ret

l0000000000400477:
	mov	eax,00000000
	test	rax,rax
	jz	0000000000400475

l0000000000400481:
	pop	rbp
	mov	edi,00601038
	jmp	rax
0000000000400489                            0F 1F 80 00 00 00 00          .......

;; register_tm_clones: 0000000000400490
register_tm_clones proc
	mov	eax,00601038
	push	rbp
	sub	r8,+00601038
	sar	rax,03
	mov	rbp,rsp
	mov	rdx,rax
	shr	rdx,3F
	add	rax,rdx
	sar	rax,01
	jnz	00000000004004B4

l00000000004004B2:
	pop	rbp
	ret

l00000000004004B4:
	mov	edx,00000000
	test	rdx,rdx
	jz	00000000004004B2

l00000000004004BE:
	pop	rbp
	mov	rsi,rax
	mov	edi,00601038
	jmp	rdx
00000000004004C9                            0F 1F 80 00 00 00 00          .......

;; __do_global_dtors_aux: 00000000004004D0
__do_global_dtors_aux proc
	cmp	byte ptr [rip+00200B61],00                             ; 0000000000601038
	jnz	00000000004004EA

l00000000004004D9:
	push	rbp
	mov	rbp,rsp
	call	0000000000400460
	pop	rbp
	mov	byte ptr [rip+00200B4E],01                             ; 0000000000601038

l00000000004004EA:
	ret
00000000004004EC                                     0F 1F 40 00             ..@.

;; frame_dummy: 00000000004004F0
frame_dummy proc
	cmp	qword ptr [rip+00200910],00                            ; 0000000000600E08
	jz	0000000000400518

l00000000004004FA:
	mov	eax,00000000
	test	rax,rax
	jz	0000000000400518

l0000000000400504:
	push	rbp
	mov	edi,00600E08
	mov	rbp,rsp
	call	rax
	pop	rbp
	jmp	0000000000400490
0000000000400515                0F 1F 00                              ...       

l0000000000400518:
	jmp	0000000000400490
000000000040051D                                        90 90 90              ...

;; verify: 0000000000400520
verify proc
	push	rbp
	mov	rbp,rsp
	mov	[rbp-18],rdi
	mov	dword ptr [rbp-04],00000000
	jmp	00000000004005A1

l0000000000400531:
	mov	eax,[rbp-04]
	movsx	rdx,eax
	mov	rax,[rbp-18]
	add	rax,rdx
	movzx	eax,byte ptr [rax]
	mov	edx,eax
	mov	eax,[rbp-04]
	xor	eax,edx
	mov	[rbp-05],al
	movzx	edx,byte ptr [rbp-05]
	mov	eax,[rbp-04]
	xor	eax,09
	and	eax,03
	mov	ecx,eax
	shl	edx,cl
	mov	eax,edx
	mov	edx,eax
	movzx	esi,byte ptr [rbp-05]
	mov	eax,[rbp-04]
	xor	eax,09
	and	eax,03
	mov	ecx,eax
	mov	eax,00000008
	sub	eax,ecx
	mov	ecx,eax
	sar	esi,cl
	mov	eax,esi
	or	eax,edx
	mov	[rbp-05],al
	add	byte ptr [rbp-05],08
	mov	eax,[rbp-04]
	cbw
	movzx	eax,byte ptr [rax+00601020]
	cmp	al,[rbp-05]
	jz	000000000040059D

l0000000000400596:
	mov	eax,00000000
	jmp	00000000004005C3

l000000000040059D:
	add	dword ptr [rbp-04],01

l00000000004005A1:
	mov	eax,[rbp-04]
	movsx	rdx,eax
	mov	rax,[rbp-18]
	add	rax,rdx
	movzx	eax,byte ptr [rax]
	test	al,al
	jnz	0000000000400531

l00000000004005B9:
	cmp	dword ptr [rbp-04],17
	setz	al
	movzx	eax,al

l00000000004005C3:
	pop	rbp
	ret

;; main: 00000000004005C5
main proc
	push	rbp
	mov	rbp,rsp
	sub	rsp,10
	mov	[rbp-04],edi
	mov	[rbp-10],rsi
	cmp	dword ptr [rbp-04],02
	jz	00000000004005EB

l00000000004005DA:
	mov	edi,004006C8
	call	00000000004003F0
	mov	eax,FFFFFFFF
	jmp	000000000040061D

l00000000004005EB:
	mov	rax,[rbp-10]
	add	rax,08
	mov	rax,[rax]
	mov	rdi,rax
	call	0000000000400520
	test	eax,eax
	jz	000000000040060E

l0000000000400602:
	mov	edi,004006F0
	call	00000000004003F0
	jmp	0000000000400618

l000000000040060E:
	mov	edi,00400718
	call	00000000004003F0

l0000000000400618:
	mov	eax,00000000

l000000000040061D:
	leave
	ret
000000000040061F                                              90                .

;; __libc_csu_init: 0000000000400620
__libc_csu_init proc
	mov	[rsp-28],rbp
	mov	[rsp-20],r12
	lea	rbp,[rip+002007CF]                                     ; 0000000000600E00
	lea	r12,[rip+002007C0]                                     ; 0000000000600DF8
	mov	[rsp-18],r13
	mov	[rsp-10],r14
	mov	[rsp-08],r15
	mov	[rsp-30],rbx
	sub	rsp,38
	sub	rbp,r12
	mov	r13d,edi
	mov	r14,rsi
	sar	rbp,03
	mov	r15,rdx
	call	00000000004003C8
	test	rbp,rbp
	jz	0000000000400686

l000000000040066A:
	xor	ebx,ebx
	nop	dword ptr [rax+00]

l0000000000400670:
	mov	rdx,r15
	mov	rsi,r14
	mov	edi,r13d
	call	qword ptr [r12+rbx*8]
	add	rbx,01
	cmp	rbx,rbp
	jnz	0000000000400670

l0000000000400686:
	mov	rbx,[rsp+08]
	mov	rbp,[rsp+10]
	mov	r12,[rsp+18]
	mov	r13,[rsp+20]
	mov	r14,[rsp+28]
	mov	r15,[rsp+30]
	add	rsp,38
	ret
00000000004006A9                            0F 1F 80 00 00 00 00          .......

;; __libc_csu_fini: 00000000004006B0
__libc_csu_fini proc
	ret
00000000004006B2       90 90                                       ..           
;;; Segment .fini (00000000004006B4)

;; _fini: 00000000004006B4
_fini proc
	sub	rsp,08
	add	rsp,08
	ret
;;; Segment .rodata (00000000004006C0)
00000000004006C0 01 00 02 00                                     ....           
00000000004006C4             00 00 00 00 59 6F 75 20 6E 65 65 64     ....You need
00000000004006D0 20 74 6F 20 65 6E 74 65 72 20 74 68 65 20 73 65  to enter the se
00000000004006E0 63 72 65 74 20 6B 65 79 21 00 00 00 00 00 00 00 cret key!.......
00000000004006F0 43 6F 72 72 65 63 74 21 20 74 68 61 74 20 69 73 Correct! that is
0000000000400700 20 74 68 65 20 73 65 63 72 65 74 20 6B 65 79 21  the secret key!
0000000000400710 00 00 00 00 00 00 00 00 49 27 6D 20 73 6F 72 72 ........I'm sorr
0000000000400720 79 2C 20 74 68 61 74 27 73 20 74 68 65 20 77 72 y, that's the wr
0000000000400730 6F 6E 67 20 73 65 63 72 65 74 20 6B 65 79 21 00 ong secret key!.
;;; Segment .eh_frame_hdr (0000000000400740)
0000000000400740 01 1B 03 3B 34 00 00 00 05 00 00 00 A0 FC FF FF ...;4...........
0000000000400750 50 00 00 00 E0 FD FF FF 78 00 00 00 85 FE FF FF P.......x.......
0000000000400760 98 00 00 00 E0 FE FF FF B8 00 00 00 70 FF FF FF ............p...
0000000000400770 E0 00 00 00                                     ....           
;;; Segment .eh_frame (0000000000400778)
0000000000400778                         14 00 00 00 00 00 00 00         ........
0000000000400780 01 7A 52 00 01 78 10 01 1B 0C 07 08 90 01 00 00 .zR..x..........
0000000000400790 24 00 00 00 1C 00 00 00 48 FC FF FF 30 00 00 00 $.......H...0...
00000000004007A0 00 0E 10 46 0E 18 4A 0F 0B 77 08 80 00 3F 1A 3B ...F..J..w...?.;
00000000004007B0 2A 33 24 22 00 00 00 00 1C 00 00 00 44 00 00 00 *3$"........D...
00000000004007C0 60 FD FF FF A5 00 00 00 00 41 0E 10 86 02 43 0D `........A....C.
00000000004007D0 06 02 A0 0C 07 08 00 00 1C 00 00 00 64 00 00 00 ............d...
00000000004007E0 E5 FD FF FF 5A 00 00 00 00 41 0E 10 86 02 43 0D ....Z....A....C.
00000000004007F0 06 02 55 0C 07 08 00 00 24 00 00 00 84 00 00 00 ..U.....$.......
0000000000400800 20 FE FF FF 89 00 00 00 00 51 8C 05 86 06 5F 0E  ........Q...._.
0000000000400810 40 83 07 8F 02 8E 03 8D 04 02 58 0E 08 00 00 00 @.........X.....
0000000000400820 14 00 00 00 AC 00 00 00 88 FE FF FF 02 00 00 00 ................
0000000000400830 00 00 00 00 00 00 00 00 00 00 00 00             ............   
;;; Segment .init_array (0000000000600DF8)
0000000000600DF8                         F0 04 40 00 00 00 00 00         ..@.....
;;; Segment .fini_array (0000000000600E00)
0000000000600E00 D0 04 40 00 00 00 00 00                         ..@.....       
;;; Segment .jcr (0000000000600E08)
0000000000600E08                         00 00 00 00 00 00 00 00         ........
;;; Segment .dynamic (0000000000600E10)
; DT_NEEDED       libc.so.6
; DT_INIT         00000000004003C8
; DT_DEBUG        00000000004006B4
; DT_INIT_ARRAY   0000000000600DF8
; DT_INIT_ARRAYSZ 0000000000000008
; DT_FINI_ARRAY   0000000000600E00
; DT_FINI_ARRAYSZ 0000000000000008
; 6FFFFEF5        0000000000400298
; DT_STRTAB       0000000000400318
; DT_SYMTAB       00000000004002B8
; DT_STRSZ        000000000000003D
; DT_SYMENT                     24
; DT_DEBUG        0000000000000000
; DT_PLTGOT       0000000000600FE8
; DT_PLTRELSZ                   48
; DT_PLTREL       0000000000000007
; DT_JMPREL       0000000000400398
; DT_RELA         0000000000400380
; DT_RELASZ                     24
; DT_RELAENT                    24
; 6FFFFFFE        0000000000400360
; 6FFFFFFF        0000000000000001
; 6FFFFFF0        0000000000400356
;;; Segment .got (0000000000600FE0)
__gmon_start___GOT		; 0000000000600FE0
	dq	0x0000000000000000
;;; Segment .got.plt (0000000000600FE8)
0000000000600FE8                         10 0E 60 00 00 00 00 00         ..`.....
0000000000600FF0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
puts_GOT		; 0000000000601000
	dq	0x00000000004003F6
__libc_start_main_GOT		; 0000000000601008
	dq	0x0000000000400406
;;; Segment .data (0000000000601010)
0000000000601010 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
0000000000601020 CA 70 93 C8 06 54 D2 D5 DA 6A D1 59 DE 45 F9 B5 .p...T...j.Y.E..
0000000000601030 A6 87 19 A5 56 6E 63 00                         ....Vnc.       
;;; Segment .bss (0000000000601038)
0000000000601038                         00 00 00 00 00 00 00 00         ........
