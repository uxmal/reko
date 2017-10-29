;;; Segment .interp (0000000120000238)
0000000120000238                         2F 6C 69 62 2F 6C 64 2D         /lib/ld-
0000000120000240 6C 69 6E 75 78 2E 73 6F 2E 32 00                linux.so.2.    
;;; Segment .note.ABI-tag (000000012000024C)
000000012000024C                                     04 00 00 00             ....
0000000120000250 10 00 00 00 01 00 00 00 47 4E 55 00 00 00 00 00 ........GNU.....
0000000120000260 03 00 00 00 02 00 00 00 00 00 00 00             ............   
;;; Segment .note.gnu.build-id (000000012000026C)
000000012000026C                                     04 00 00 00             ....
0000000120000270 14 00 00 00 03 00 00 00 47 4E 55 00 E8 05 45 4A ........GNU...EJ
0000000120000280 7B 3E F3 EE E6 35 93 36 37 D5 86 2D E0 0F 2F 49 {>...5.67..-../I
;;; Segment .gnu.hash (0000000120000290)
0000000120000290 02 00 00 00 04 00 00 00 01 00 00 00 06 00 00 00 ................
00000001200002A0 00 00 00 00 00 60 00 00 00 00 00 00 04 00 00 00 .....`..........
00000001200002B0 AD 4B E3 C0                                     .K..           
;;; Segment .dynsym (00000001200002B8)
;    0                                          00000000 00000000 00 
;    1 printf                                   00000000 00000000 12 
;    2 puts                                     00000000 00000000 12 
;    3 __libc_start_main                        00000000 00000000 12 
;    4 _IO_stdin_used                           120012040 00000004 11 .sdata
;;; Segment .dynstr (0000000120000330)
0000000120000330 00 6C 69 62 63 2E 73 6F 2E 36 2E 31 00 5F 49 4F .libc.so.6.1._IO
0000000120000340 5F 73 74 64 69 6E 5F 75 73 65 64 00 70 75 74 73 _stdin_used.puts
0000000120000350 00 70 72 69 6E 74 66 00 5F 5F 6C 69 62 63 5F 73 .printf.__libc_s
0000000120000360 74 61 72 74 5F 6D 61 69 6E 00 47 4C 49 42 43 5F tart_main.GLIBC_
0000000120000370 32 2E 30 00 47 4C 49 42 43 5F 32 2E 34 00       2.0.GLIBC_2.4. 
;;; Segment .gnu.version (000000012000037E)
000000012000037E                                           00 00               ..
0000000120000380 02 00 03 00 03 00 01 00                         ........       
;;; Segment .gnu.version_r (0000000120000388)
0000000120000388                         01 00 02 00 01 00 00 00         ........
0000000120000390 10 00 00 00 00 00 00 00 10 69 69 0D 00 00 03 00 .........ii.....
00000001200003A0 3A 00 00 00 10 00 00 00 14 69 69 0D 00 00 02 00 :........ii.....
00000001200003B0 44 00 00 00 00 00 00 00                         D.......       
;;; Segment .rela.plt (00000001200003B8)
; 120012018  26 00000001 0000000000000000 printf (1)
; 120012020  26 00000002 0000000000000000 puts (2)
; 120012028  26 00000003 0000000000000000 __libc_start_main (3)
;;; Segment .init (0000000120000400)

;; _init: 0000000120000400
_init proc
	ldah	r29,2(r27)
	lda	r29,-63F0(r29)

;; fn0000000120000408: 0000000120000408
fn0000000120000408 proc
	subq	r30,10,r30
	lda	r27,0(zero)
	stq	r26,0(r30)
	stq	r29,8(r30)
	beq	r27,0000000120000424

;; fn000000012000041C: 000000012000041C
fn000000012000041C proc
	jsr	r26,r27
	ldq	r29,8(r30)

;; fn0000000120000424: 0000000120000424
fn0000000120000424 proc
	ldq_u	zero,0(r30)
	br	r29,000000012000042C

l000000012000042C:
	ldah	r29,2(r29)
	lda	r29,-641C(r29)
	ldq_u	zero,0(r30)
	ldq_u	zero,0(r30)
	bsr	r26,0000000120000688
	br	r29,0000000120000444

l0000000120000444:
	ldah	r29,2(r29)
	lda	r29,-6434(r29)
	ldq_u	zero,0(r30)
	ldq_u	zero,0(r30)
	bsr	r26,0000000120000C58
	ldq	r26,0(r30)
	ldq	r29,8(r30)
	addq	r30,10,r30
	ret	zero,r26
;;; Segment .plt (0000000120000470)
0000000120000470 39 05 7C 43 01 00 9C 27 79 05 39 43 6C 1B 9C 23 9.|C...'y.9Cl..#
0000000120000480 00 00 7C A7 19 04 39 43 08 00 9C A7 00 00 FB 6B ..|...9C.......k
0000000120000490 F7 FF 9F C3 FE FF FF C3 FD FF FF C3 FC FF FF C3 ................
;;; Segment .text (00000001200004A0)

;; __start: 00000001200004A0
__start proc
	br	r29,00000001200004A4

l00000001200004A4:
	ldah	r29,2(r29)
	lda	r29,-6494(r29)
	subq	r30,10,r30
	bis	zero,00,r15
	ldq	r16,-7FD8(r29)
	ldl	r17,10(r30)
	lda	r18,18(r30)
	ldq	r19,-7FE0(r29)
	ldq	r20,-8000(r29)
	bis	zero,r0,r21
	stq	r30,0(r30)
	ldq	r27,-7FE8(r29)
	jsr	r26,r27
	halt
00000001200004DC                                     00 00 FE 2F             .../
00000001200004E0 02 00 BB 27 30 9B BD 23                         ...'0..#       

;; fn00000001200004E8: 00000001200004E8
fn00000001200004E8 proc
	lda	r30,-10(r30)
	ldah	r16,-1(r29)
	lda	r16,7FF0(r16)
	stq	r26,0(r30)
	ldah	r1,-1(r29)
	lda	r1,7FF0(r1)
	cmpeq	r1,r0,r1
	bne	r1,000000012000051C

l0000000120000508:
	lda	r27,0(zero)
	beq	r27,000000012000051C

l0000000120000510:
	jsr	r26,r27
	ldah	r29,2(r26)
	lda	r29,-6504(r29)

l000000012000051C:
	ldq	r26,0(r30)
	lda	r30,10(r30)
	ret	zero,r26
0000000120000528                         1F 04 FF 47 00 00 FE 2F         ...G.../
0000000120000530 02 00 BB 27 E0 9A BD 23                         ...'...#       

l0000000120000538:
	ldah	r16,-1(r29)
	lda	r16,7FF0(r16)
	ldah	r17,-1(r29)
	lda	r17,7FF0(r17)
	subq	r17,r0,r17
	src	r17,03,r17
	lda	r30,-10(r30)
	srl	r17,3F,r1
	stq	r26,0(r30)
	addq	r1,r8,r17
	src	r17,01,r17
	beq	r17,000000012000057C

l0000000120000568:
	lda	r27,0(zero)
	beq	r27,000000012000057C

l0000000120000570:
	jsr	r26,r27
	ldah	r29,2(r26)
	lda	r29,-6564(r29)

l000000012000057C:
	ldq	r26,0(r30)
	lda	r30,10(r30)
	ret	zero,r26
0000000120000588                         1F 04 FF 47 00 00 FE 2F         ...G.../
0000000120000590 02 00 BB 27 80 9A BD 23                         ...'...#       

;; fn0000000120000598: 0000000120000598
fn0000000120000598 proc
	lda	r30,-30(r30)
	stq	r11,18(r30)
	ldah	r11,0(r29)
	stq	r12,20(r30)
	lda	r12,-7FB8(r11)
	ldq_u	r1,-7FB8(r11)
	stq	r26,0(r30)
	stq	r9,8(r30)
	extbl	r1,r0,r1
	stq	r10,10(r30)
	stq	r13,28(r30)
	bne	r1,0000000120000654

l00000001200005C8:
	ldah	r9,-1(r29)
	ldah	r1,-1(r29)
	lda	r1,7E70(r1)
	ldah	r13,0(r29)
	lda	r9,7E78(r9)
	subq	r9,r8,r9
	ldq	r1,-7FB0(r13)
	src	r9,03,r9
	ldah	r10,-1(r29)
	lda	r9,-1(r9)
	cmpult	r1,r8,r2
	lda	r10,7E70(r10)
	beq	r2,000000012000062C

l00000001200005FC:
	ldq_u	zero,0(r30)

l0000000120000600:
	lda	r1,1(r1)
	stq	r1,-7FB0(r13)
	s8addq	r1,00,r1
	addq	r10,r8,r1
	ldq	r27,0(r1)
	jsr	r26,r27
	ldah	r29,2(r26)
	lda	r29,-6608(r29)
	ldq	r1,-7FB0(r13)
	cmpult	r1,r8,r2
	bne	r2,0000000120000600

l000000012000062C:
	ldq_u	zero,0(r30)
	bsr	r26,00000001200004E8
	ldq_u	zero,0(r30)
	ldq_u	zero,0(r30)
	lda	r1,1(zero)
	ldq_u	r2,-7FB8(r11)
	insbl	r1,r0,r1
	mskbl	r2,r0,r2
	bis	r1,r16,r1
	stq_u	r1,-7FB8(r11)

l0000000120000654:
	ldq	r26,0(r30)
	ldq	r9,8(r30)
	ldq	r10,10(r30)
	ldq	r11,18(r30)
	ldq	r12,20(r30)
	ldq	r13,28(r30)
	lda	r30,30(r30)
	ret	zero,r26
0000000120000674             00 00 FE 2F 1F 04 FF 47 00 00 FE 2F     .../...G.../
0000000120000680 02 00 BB 27 90 99 BD 23                         ...'...#       

;; fn0000000120000688: 0000000120000688
fn0000000120000688 proc
	ldq_u	zero,0(r30)
	br	zero,0000000120000538
0000000120000690 1F 04 FF 47 00 00 FE 2F 1F 04 FF 47 00 00 FE 2F ...G.../...G.../

;; getNumber: 00000001200006A0
getNumber proc
	ldah	r29,2(r27)
	lda	r29,-6690(r29)

;; fn00000001200006A8: 00000001200006A8
fn00000001200006A8 proc
	lda	r30,-20(r30)
	stq	r26,0(r30)
	stq	r15,8(r30)
	bis	zero,r16,r15
	ldah	r1,0(r29)
	ldl	r2,-7FC0(r1)
	ldah	r1,0(r29)
	ldl	r1,-7FC0(r1)
	mull	r2,r8,r1
	addl	zero,r8,r1
	stq	r1,18(r15)
	ldt	f10,18(r15)
	cvtqt	f31,f10
	trapb
	ldah	r1,-2(r29)
	ldt	f11,6D88(r1)
	divt_su	f12,f11,f10
	trapb
	cvttq_svc	f31,f10
	trapb
	stt	f11,10(r15)
	ldq	r2,10(r15)
	bis	zero,r16,r1
	addl	zero,r8,r1
	bis	zero,r8,r0
	bis	zero,r24,r30
	ldq	r26,0(r30)
	ldq	r15,8(r30)
	lda	r30,20(r30)
	ret	zero,r26

;; magic: 0000000120000720
magic proc
	ldah	r29,2(r27)
	lda	r29,-6710(r29)

;; fn0000000120000728: 0000000120000728
fn0000000120000728 proc
	lda	r30,-20(r30)
	stq	r26,0(r30)
	stq	r15,8(r30)
	bis	zero,r16,r15
	bis	zero,r0,r1
	stl	r1,10(r15)
	ldl	r1,10(r15)
	bis	zero,r8,r17
	ldah	r1,-2(r29)
	lda	r16,6CF0(r1)
	ldq	r27,-7FF8(r29)
	jsr	r26,r27
	ldah	r29,2(r26)
	lda	r29,-6748(r29)
	ldq_u	zero,0(r30)
	bsr	r26,00000001200006A8
	ldq_u	zero,0(r30)
	ldq_u	zero,0(r30)
	bis	zero,r0,r1
	subl	zero,r8,r1
	addl	zero,r8,r1
	bis	zero,r8,r0
	bis	zero,r24,r30
	ldq	r26,0(r30)
	ldq	r15,8(r30)
	lda	r30,20(r30)
	ret	zero,r26

;; numbers: 0000000120000794
numbers proc
	ldah	r29,2(r27)
	lda	r29,-6784(r29)

;; fn000000012000079C: 000000012000079C
fn000000012000079C proc
	lda	r30,-20(r30)
	stq	r26,0(r30)
	stq	r15,8(r30)
	bis	zero,r16,r15
	bis	zero,r0,r1
	stl	r1,10(r15)
	ldl	r1,10(r15)
	zapnot	r1,0F,r1
	cmpule	r1,0A,r1
	beq	r1,0000000120000870

l00000001200007C4:
	ldl	r1,10(r15)
	zapnot	r1,0F,r1
	s4addq	r1,00,r2
	ldah	r1,-2(r29)
	lda	r1,6D3C(r1)
	addq	r2,r8,r1
	ldl	r1,0(r1)
	addl	zero,r8,r1
	addq	r29,r8,r1
	jmp	zero,r1
00000001200007EC                                     FE FF 3D 24             ..=$
00000001200007F0 FA 6C 21 20 20 00 E0 C3 FE FF 3D 24 FF 6C 21 20 .l!  .....=$.l! 
0000000120000800 1D 00 E0 C3 FE FF 3D 24 03 6D 21 20 1A 00 E0 C3 ......=$.m! ....
0000000120000810 FE FF 3D 24 07 6D 21 20 17 00 E0 C3 FE FF 3D 24 ..=$.m! ......=$
0000000120000820 0C 6D 21 20 14 00 E0 C3 FE FF 3D 24 14 6D 21 20 .m! ......=$.m! 
0000000120000830 11 00 E0 C3 FE FF 3D 24 1A 6D 21 20 0E 00 E0 C3 ......=$.m! ....
0000000120000840 FE FF 3D 24 20 6D 21 20 0B 00 E0 C3 FE FF 3D 24 ..=$ m! ......=$
0000000120000850 24 6D 21 20 08 00 E0 C3 FE FF 3D 24 29 6D 21 20 $m! ......=$)m! 
0000000120000860 05 00 E0 C3 FE FF 3D 24 2F 6D 21 20 02 00 E0 C3 ......=$/m! ....

l0000000120000870:
	ldah	r1,-2(r29)
	lda	r1,6D33(r1)
	bis	zero,r8,r0
	bis	zero,r24,r30
	ldq	r26,0(r30)
	ldq	r15,8(r30)
	lda	r30,20(r30)
	ret	zero,r26

;; switchy: 0000000120000890
switchy proc
	ldah	r29,2(r27)
	lda	r29,-6880(r29)

;; fn0000000120000898: 0000000120000898
fn0000000120000898 proc
	lda	r30,-20(r30)
	stq	r26,0(r30)
	stq	r15,8(r30)
	bis	zero,r16,r15
	ldq_u	zero,0(r30)
	bsr	r26,00000001200006A8
	ldq_u	zero,0(r30)
	ldq_u	zero,0(r30)
	bis	zero,r0,r1
	stl	r1,10(r15)
	ldl	r1,10(r15)
	cmpeq	r1,51,r2
	bne	r2,00000001200009D4

l00000001200008CC:
	cmple	r1,51,r2
	beq	r2,0000000120000900

l00000001200008D4:
	cmpeq	r1,03,r2
	bne	r2,0000000120000974

l00000001200008DC:
	cmple	r1,03,r2
	beq	r2,00000001200008EC

l00000001200008E4:
	beq	r1,000000012000096C

l00000001200008E8:
	br	zero,0000000120000A6C

l00000001200008EC:
	cmpeq	r1,04,r2
	bne	r2,0000000120000994

l00000001200008F4:
	cmpeq	r1,05,r1
	bne	r1,00000001200009B4

l00000001200008FC:
	br	zero,0000000120000A6C

l0000000120000900:
	lda	r2,-7E1(r1)
	beq	r2,0000000120000A24

l0000000120000908:
	lda	r2,7E1(zero)
	cmple	r1,r16,r2
	beq	r2,0000000120000928

l0000000120000914:
	lda	r2,-539(r1)
	beq	r2,000000012000094C

l000000012000091C:
	lda	r1,-53A(r1)
	beq	r1,0000000120000944

l0000000120000924:
	br	zero,0000000120000A6C

l0000000120000928:
	lda	r2,-2334(r1)
	beq	r2,0000000120000A00

l0000000120000930:
	ldah	r2,1(zero)
	lda	r2,261F(r2)
	cmpeq	r1,r16,r1
	bne	r1,0000000120000A48

l0000000120000940:
	br	zero,0000000120000A6C

l0000000120000944:
	lda	r1,1(zero)
	br	zero,0000000120000A6C

l000000012000094C:
	ldah	r1,-2(r29)
	lda	r16,6D68(r1)
	ldq	r27,-7FF0(r29)
	jsr	r26,r27
	ldah	r29,2(r26)
	lda	r29,-694C(r29)
	bis	zero,r24,r1
	br	zero,0000000120000A6C

l000000012000096C:
	lda	r1,2(zero)
	br	zero,0000000120000A6C

l0000000120000974:
	ldah	r1,-2(r29)
	lda	r16,6D6B(r1)
	ldq	r27,-7FF0(r29)
	jsr	r26,r27
	ldah	r29,2(r26)
	lda	r29,-6974(r29)
	lda	r1,-2(zero)
	br	zero,0000000120000A6C

l0000000120000994:
	ldah	r1,-2(r29)
	lda	r16,6D6F(r1)
	ldq	r27,-7FF0(r29)
	jsr	r26,r27
	ldah	r29,2(r26)
	lda	r29,-6994(r29)
	lda	r1,-A(zero)
	br	zero,0000000120000A6C

l00000001200009B4:
	ldah	r1,-2(r29)
	lda	r16,6D73(r1)
	ldq	r27,-7FF0(r29)
	jsr	r26,r27
	ldah	r29,2(r26)
	lda	r29,-69B4(r29)
	lda	r1,1(zero)
	br	zero,0000000120000A6C

l00000001200009D4:
	ldl	r1,10(r15)
	srl	r1,3F,r2
	addq	r2,r8,r1
	src	r1,01,r1
	addl	zero,r8,r1
	bis	zero,r8,r16
	ldq_u	zero,0(r30)
	bsr	r26,0000000120000728
	ldq_u	zero,0(r30)
	ldq_u	zero,0(r30)
	br	zero,0000000120000A6C

l0000000120000A00:
	ldl	r1,10(r15)
	addq	r1,r8,r1
	addl	zero,r8,r1
	bis	zero,r8,r16
	ldq_u	zero,0(r30)
	bsr	r26,0000000120000728
	ldq_u	zero,0(r30)
	ldq_u	zero,0(r30)
	br	zero,0000000120000A6C

l0000000120000A24:
	ldl	r1,10(r15)
	subl	r1,2A,r1
	addl	zero,r8,r1
	bis	zero,r8,r16
	ldq_u	zero,0(r30)
	bsr	r26,0000000120000728
	ldq_u	zero,0(r30)
	ldq_u	zero,0(r30)
	br	zero,0000000120000A6C

l0000000120000A48:
	ldl	r1,10(r15)
	addl	r1,2A,r1
	addl	zero,r8,r1
	bis	zero,r8,r16
	ldq_u	zero,0(r30)
	bsr	r26,0000000120000728
	ldq_u	zero,0(r30)
	ldq_u	zero,0(r30)
	bis	zero,r24,zero

l0000000120000A6C:
	bis	zero,r8,r0
	bis	zero,r24,r30
	ldq	r26,0(r30)
	ldq	r15,8(r30)
	lda	r30,20(r30)
	ret	zero,r26

;; main: 0000000120000A84
main proc
	ldah	r29,2(r27)
	lda	r29,-6A74(r29)
	lda	r30,-20(r30)
	stq	r26,0(r30)
	stq	r15,8(r30)
	bis	zero,r16,r15
	ldah	r1,-2(r29)
	lda	r16,6D77(r1)
	ldq	r27,-7FF0(r29)
	jsr	r26,r27
	ldah	r29,2(r26)
	lda	r29,-6A9C(r29)
	ldq_u	zero,0(r30)
	bsr	r26,0000000120000898
	ldq_u	zero,0(r30)
	ldq_u	zero,0(r30)
	bis	zero,r0,r1
	stl	r1,10(r15)
	ldl	r4,10(r15)
	addl	zero,r0,r3
	bis	zero,r24,r2
	s4addq	r2,00,r2
	subq	r2,r24,r2
	s8addq	r2,00,r1
	subq	r1,r16,r1
	s4addq	r1,00,r1
	subq	r1,r24,r1
	s4addq	r1,00,r1
	subq	r1,r24,r1
	s8addq	r1,00,r2
	addq	r1,r16,r1
	sll	r1,0F,r2
	subq	r2,r8,r2
	s8addq	r2,00,r2
	addq	r2,r24,r2
	srl	r2,20,r1
	addl	zero,r8,r1
	src	r1,01,r2
	addl	zero,r0,r1
	src	r1,1F,r1
	subl	r2,r8,r2
	bis	zero,r16,r1
	s4addq	r1,00,r1
	subl	r1,r16,r1
	s4addq	r1,00,r1
	subl	r1,r16,r1
	subl	r4,r8,r1
	addl	zero,r8,r1
	bis	zero,r8,r16
	ldq_u	zero,0(r30)
	bsr	r26,000000012000079C
	ldq_u	zero,0(r30)
	ldq_u	zero,0(r30)
	bis	zero,r0,r1
	stq	r1,18(r15)
	bis	zero,r24,r1
	bis	zero,r8,r0
	bis	zero,r24,r30
	ldq	r26,0(r30)
	ldq	r15,8(r30)
	lda	r30,20(r30)
	ret	zero,r26
0000000120000B7C                                     00 00 FE 2F             .../

;; __libc_csu_init: 0000000120000B80
__libc_csu_init proc
	ldah	r29,2(r27)
	lda	r29,-6B70(r29)
	lda	r30,-40(r30)
	ldq_u	zero,0(r30)
	stq	r9,8(r30)
	stq	r10,10(r30)
	bis	zero,r24,r10
	stq	r11,18(r30)
	bis	zero,r16,r11
	stq	r12,20(r30)
	bis	zero,r8,r12
	stq	r13,28(r30)
	bis	zero,r0,r13
	stq	r14,30(r30)
	stq	r26,0(r30)
	bsr	r26,0000000120000408
	ldq_u	zero,0(r30)
	ldq_u	zero,0(r30)
	ldah	r9,-1(r29)
	ldah	r14,-1(r29)
	lda	r9,7E60(r9)
	lda	r14,7E60(r14)
	subq	r14,r8,r14
	src	r14,03,r14
	beq	r14,0000000120000C1C

l0000000120000BE4:
	ldq_u	zero,0(r30)
	bis	zero,r24,zero
	ldq_u	zero,0(r30)

l0000000120000BF0:
	bis	zero,r24,r18
	ldq	r27,0(r9)
	bis	zero,r0,r17
	bis	zero,r8,r16
	lda	r10,1(r10)
	jsr	r26,r27
	ldah	r29,2(r26)
	lda	r29,-6BF8(r29)
	cmpeq	r14,r16,r1
	lda	r9,8(r9)
	beq	r1,0000000120000BF0

l0000000120000C1C:
	ldq	r26,0(r30)
	ldq	r9,8(r30)
	ldq	r10,10(r30)
	ldq	r11,18(r30)
	ldq	r12,20(r30)
	ldq	r13,28(r30)
	ldq	r14,30(r30)
	lda	r30,40(r30)
	ret	zero,r26

;; __libc_csu_fini: 0000000120000C40
__libc_csu_fini proc
	ret	zero,r26
0000000120000C44             00 00 FE 2F 1F 04 FF 47 00 00 FE 2F     .../...G.../
0000000120000C50 02 00 BB 27 C0 93 BD 23                         ...'...#       

;; fn0000000120000C58: 0000000120000C58
fn0000000120000C58 proc
	lda	r30,-20(r30)
	stq	r9,8(r30)
	ldah	r9,-1(r29)
	lda	r9,7E68(r9)
	stq	r10,10(r30)
	lda	r10,-1(zero)
	stq	r26,0(r30)
	ldq	r27,-8(r9)
	cmpeq	r27,r16,r1
	bne	r1,0000000120000CAC

l0000000120000C80:
	lda	r9,-8(r9)
	ldq_u	zero,0(r30)
	bis	zero,r24,zero
	ldq_u	zero,0(r30)

l0000000120000C90:
	lda	r9,-8(r9)
	jsr	r26,r27
	ldah	r29,2(r26)
	lda	r29,-6C88(r29)
	ldq	r27,0(r9)
	cmpeq	r27,r16,r1
	beq	r1,0000000120000C90

l0000000120000CAC:
	ldq	r26,0(r30)
	ldq	r9,8(r30)
	ldq	r10,10(r30)
	lda	r30,20(r30)
	ret	zero,r26
;;; Segment .fini (0000000120000CC0)

;; _fini: 0000000120000CC0
_fini proc
	ldah	r29,2(r27)
	lda	r29,-6CB0(r29)
	subq	r30,10,r30
	stq	r26,0(r30)
	stq	r29,8(r30)
	ldq_u	zero,0(r30)
	br	r29,0000000120000CDC

l0000000120000CDC:
	ldah	r29,2(r29)
	lda	r29,-6CCC(r29)
	ldq_u	zero,0(r30)
	ldq_u	zero,0(r30)
	bsr	r26,0000000120000598
	ldq	r26,0(r30)
	ldq	r29,8(r30)
	addq	r30,10,r30
	ret	zero,r26
;;; Segment .rodata (0000000120000D00)
0000000120000D00 4E 69 63 65 20 25 64 21 0A 00 7A 65 72 6F 00 6F Nice %d!..zero.o
0000000120000D10 6E 65 00 74 77 6F 00 74 72 65 73 00 71 75 61 74 ne.two.tres.quat
0000000120000D20 74 72 6F 00 66 C3 BC 6D 66 00 73 68 65 73 74 00 tro.f..mf.shest.
0000000120000D30 73 6A 75 00 6F 74 74 6F 00 6E 75 65 76 65 00 64 sju.otto.nueve.d
0000000120000D40 69 78 00 69 6E 76 61 6C 69 64 00 00 DC 67 FE FF ix.invalid...g..
0000000120000D50 E8 67 FE FF F4 67 FE FF 00 68 FE FF 0C 68 FE FF .g...g...h...h..
0000000120000D60 18 68 FE FF 24 68 FE FF 30 68 FE FF 3C 68 FE FF .h..$h..0h..<h..
0000000120000D70 48 68 FE FF 54 68 FE FF 6F 6B 00 6E 6F 33 00 6E Hh..Th..ok.no3.n
0000000120000D80 6F 34 00 6E 6F 35 00 48 65 6C 6C 6F 20 52 65 6B o4.no5.Hello Rek
0000000120000D90 6F 00 00 00 00 00 00 00 9A 99 99 99 99 19 4C 40 o.............L@
;;; Segment .eh_frame_hdr (0000000120000DA0)
0000000120000DA0 01 1B 03 3B 4C 00 00 00 08 00 00 00 00 F7 FF FF ...;L...........
0000000120000DB0 64 00 00 00 00 F9 FF FF 8C 00 00 00 80 F9 FF FF d...............
0000000120000DC0 B0 00 00 00 F4 F9 FF FF D4 00 00 00 F0 FA FF FF ................
0000000120000DD0 F8 00 00 00 E4 FC FF FF 1C 01 00 00 E0 FD FF FF ................
0000000120000DE0 40 01 00 00 A0 FE FF FF 74 01 00 00             @.......t...   
;;; Segment .eh_frame (0000000120000DF0)
0000000120000DF0 10 00 00 00 00 00 00 00 01 7A 52 00 04 78 0F 01 .........zR..x..
0000000120000E00 1B 0D 1E 00 10 00 00 00 18 00 00 00 94 F6 FF FF ................
0000000120000E10 3C 00 00 00 00 45 0D 0F 10 00 00 00 00 00 00 00 <....E..........
0000000120000E20 01 7A 52 00 04 78 1A 01 1B 0D 1E 00 20 00 00 00 .zR..x...... ...
0000000120000E30 18 00 00 00 6C F8 FF FF 80 00 00 00 00 43 0E 20 ....l........C. 
0000000120000E40 42 9A 04 8F 03 41 0D 0F 59 CF DA 0C 1E 00 00 00 B....A..Y.......
0000000120000E50 20 00 00 00 3C 00 00 00 C8 F8 FF FF 74 00 00 00  ...<.......t...
0000000120000E60 00 43 0E 20 42 9A 04 8F 03 41 0D 0F 56 CF DA 0C .C. B....A..V...
0000000120000E70 1E 00 00 00 20 00 00 00 60 00 00 00 18 F9 FF FF .... ...`.......
0000000120000E80 FC 00 00 00 00 43 0E 20 42 9A 04 8F 03 41 0D 0F .....C. B....A..
0000000120000E90 78 CF DA 0C 1E 00 00 00 20 00 00 00 84 00 00 00 x....... .......
0000000120000EA0 F0 F9 FF FF F4 01 00 00 00 43 0E 20 42 9A 04 8F .........C. B...
0000000120000EB0 03 41 0D 0F 02 76 CF DA 0C 1E 00 00 20 00 00 00 .A...v...... ...
0000000120000EC0 A8 00 00 00 C0 FB FF FF F8 00 00 00 00 43 0E 20 .............C. 
0000000120000ED0 42 9A 04 8F 03 41 0D 0F 77 CF DA 0C 1E 00 00 00 B....A..w.......
0000000120000EE0 30 00 00 00 CC 00 00 00 98 FC FF FF C0 00 00 00 0...............
0000000120000EF0 00 43 0E 40 43 89 07 8A 06 42 8B 05 42 8C 04 42 .C.@C....B..B..B
0000000120000F00 8D 03 43 8E 02 9A 08 60 CE CD CC CB CA C9 DA 0E ..C....`........
0000000120000F10 00 00 00 00 10 00 00 00 00 01 00 00 24 FD FF FF ............$...
0000000120000F20 04 00 00 00 00 00 00 00 00 00 00 00             ............   
;;; Segment .ctors (0000000120011E70)
0000000120011E70 FF FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 ................
;;; Segment .dtors (0000000120011E80)
0000000120011E80 FF FF FF FF FF FF FF FF 00 00 00 00 00 00 00 00 ................
;;; Segment .dynamic (0000000120011E90)
; DT_NEEDED       libc.so.6.1
; DT_INIT         0000000120000400
; DT_DEBUG        0000000120000CC0
; 6FFFFEF5        0000000120000290
; DT_STRTAB       0000000120000330
; DT_SYMTAB       00000001200002B8
; DT_STRSZ        000000000000004E
; DT_SYMENT                     24
; DT_DEBUG        0000000000000000
; DT_PLTGOT       0000000120012000
; DT_PLTRELSZ                   72
; DT_PLTREL       0000000000000007
; DT_JMPREL       00000001200003B8
; 70000000        0000000000000001
; 6FFFFFFE        0000000120000388
; 6FFFFFFF        0000000000000001
; 6FFFFFF0        000000012000037E
;;; Segment .got (0000000120012000)
__libc_start_main_GOT		; 0000000120012000
	dd	0x00000000
__libc_start_main_GOT		; 0000000120012008
	dd	0x00000000
__libc_csu_fini_GOT		; 0000000120012010
	dd	0x20000C40
0000000120012018                         94 04 00 20 01 00 00 00         ... ....
0000000120012020 98 04 00 20 01 00 00 00 9C 04 00 20 01 00 00 00 ... ....... ....
__libc_csu_init_GOT		; 0000000120012030
	dd	0x20000B80
main_GOT		; 0000000120012038
	dd	0x20000A84
;;; Segment .sdata (0000000120012040)
0000000120012040 01 00 02 00                                     ....           
0000000120012044             00 00 00 00 00 00 00 00 00 00 00 00     ............
0000000120012050 39 05 00 00                                     9...           
;;; Segment .sbss (0000000120012058)
0000000120012058                         00 00 00 00 00 00 00 00         ........
; ...
