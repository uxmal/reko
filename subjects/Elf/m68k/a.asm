;;; Segment .interp (80000134)
80000134             2F 6C 69 62 2F 6C 64 2E 73 6F 2E 31     /lib/ld.so.1
80000140 00                                              .              
;;; Segment .note.ABI-tag (80000144)
80000144             00 00 00 04 00 00 00 10 00 00 00 01     ............
80000150 47 4E 55 00 00 00 00 00 00 00 00 03 00 00 00 02 GNU.............
80000160 00 00 00 00                                     ....           
;;; Segment .note.gnu.build-id (80000164)
80000164             00 00 00 04 00 00 00 14 00 00 00 03     ............
80000170 47 4E 55 00 AF 47 BE 52 BA C5 66 8B AA 7A 45 1F GNU..G.R..f..zE.
80000180 B7 D9 BC F3 3D C9 57 01                         ....=.W.       
;;; Segment .hash (80000188)
80000188                         00 00 00 03 00 00 00 04         ........
80000190 00 00 00 01 00 00 00 03 00 00 00 02 00 00 00 00 ................
800001A0 00 00 00 00 00 00 00 00 00 00 00 00             ............   
;;; Segment .dynsym (800001AC)
; 0000                                          00000000 00000000 00 
; 0001 __gmon_start__                           00000000 00000000 20 
; 0002 _IO_stdin_used                           800005B4 00000004 11 .rodata
; 0003 __libc_start_main                        800002F4 00000000 12 
;;; Segment .dynstr (800001EC)
800001EC                                     00 5F 5F 67             .__g
800001F0 6D 6F 6E 5F 73 74 61 72 74 5F 5F 00 6C 69 62 63 mon_start__.libc
80000200 2E 73 6F 2E 36 00 5F 49 4F 5F 73 74 64 69 6E 5F .so.6._IO_stdin_
80000210 75 73 65 64 00 5F 5F 6C 69 62 63 5F 73 74 61 72 used.__libc_star
80000220 74 5F 6D 61 69 6E 00 47 4C 49 42 43 5F 32 2E 30 t_main.GLIBC_2.0
80000230 00                                              .              
;;; Segment .gnu.version (80000232)
80000232       00 00 00 00 00 01 00 02                     ........     
;;; Segment .gnu.version_r (8000023C)
8000023C                                     00 01 00 01             ....
80000240 00 00 00 10 00 00 00 10 00 00 00 00 0D 69 69 10 .............ii.
80000250 00 00 00 02 00 00 00 3B 00 00 00 00             .......;....   
;;; Segment .rela.dyn (8000025C)
; 00000000   0 00000000 00000000  (0)
; 00000000   0 00000000 00000000  (0)
; 80004014  20 00000001 00000000 __gmon_start__ (1)
;;; Segment .rela.plt (80000280)
; 8000400C  21 00000001 00000000 __gmon_start__ (1)
; 80004010  21 00000003 00000000 __libc_start_main (3)
;;; Segment .init (80000298)

;; _init: 80000298
_init proc
	link	a6,#$0000
	move.l	a5,-(a7)
	lea	(00003D60,pc),a5
	tst.l	(00000014,a5)
	beq	$800002B6

l800002B0:
	bsr	$800002E0

l800002B6:
	jsr.l	$800003F8
	jsr.l	$80000564
	movea.l	$-0004(a6),a5
	unlk	a6
	rts	
;;; Segment .plt (800002CC)
800002CC                                     2F 3B 01 70             /;.p
800002D0 00 00 3D 36 4E FB 01 71 00 00 3D 32 00 00 00 00 ..=6N..q..=2....

;; fn800002E0: 800002E0
fn800002E0 proc
	jmp.l	([00003D2A,pc])
800002E8                         2F 3C 00 00 00 00 60 FF         /<....`.
800002F0 FF FF FF DC                                     ....           

;; __libc_start_main@@GLIBC_2.0: 800002F4
__libc_start_main@@GLIBC_2.0 proc
	jmp.l	([00003D1A,pc])
800002FC                                     2F 3C 00 00             /<..
80000300 00 0C 60 FF FF FF FF C8                         ..`.....       
;;; Segment .text (80000308)

;; _start: 80000308
_start proc
	suba.l	a6,a6
	move.l	(a7)+,d0
	movea.l	a7,a0
	pea	(a7)
	pea	(a1)
	pea	$8000055C
	pea	$80000504
	pea	(a0)
	move.l	d0,-(a7)
	pea	$800004EC
	bsr	$800002F4
	illegal	

;; deregister_tm_clones: 80000330
deregister_tm_clones proc
	link	a6,#$0000
	move.l	#$80004028,d0
	cmpi.l	#$80004028,d0
	beq	$80000356

l80000342:
	lea	$00000000,a0
	tst.l	a0
	beq	$80000356

l8000034C:
	pea	$80004028
	jsr.l	(a0)
	addq.l	#$04,a7

l80000356:
	unlk	a6
	rts	

;; register_tm_clones: 8000035A
register_tm_clones proc
	link	a6,#$0000
	move.l	#$80004028,d0
	subi.l	#$80004028,d0
	asr.l	#$02,d0
	move.l	d0,d0
	bpl	$80000372

l80000370:
	addq.l	#$01,d0

l80000372:
	asr.l	#$01,d0
	beq	$8000038C

l80000376:
	lea	$00000000,a0
	tst.l	a0
	beq	$8000038C

l80000380:
	move.l	d0,-(a7)
	pea	$80004028
	jsr.l	(a0)
	addq.l	#$08,a7

l8000038C:
	unlk	a6
	rts	

;; __do_global_dtors_aux: 80000390
__do_global_dtors_aux proc
	link	a6,#$0000
	move.l	a2,-(a7)
	move.l	d2,-(a7)
	tst.b	$80004028
	bne	$800003E4

l800003A0:
	move.l	#$80003F34,d2
	subi.l	#$80003F30,d2
	asr.l	#$02,d2
	subq.l	#$01,d2
	lea	$80003F30,a2
	move.l	$8000402A,d0
	cmp.l	d0,d2
	bls	$800003D8

l800003C0:
	addq.l	#$01,d0
	move.l	d0,$8000402A
	movea.l	(a2,d0*4),a0
	jsr.l	(a0)
	move.l	$8000402A,d0
	cmp.l	d0,d2
	bhi	$800003C0

l800003D8:
	jsr.l	$-00A8(pc)                                           ; 80000330
	move.b	#$01,$80004028

l800003E4:
	move.l	$-0008(a6),d2
	movea.l	$-0004(a6),a2
	unlk	a6
	rts	

;; call___do_global_dtors_aux: 800003F0
call___do_global_dtors_aux proc
	link	a6,#$0000
	unlk	a6
	rts	

;; frame_dummy: 800003F8
frame_dummy proc
	link	a6,#$0000
	unlk	a6
	bra	$8000035A

;; call_frame_dummy: 80000402
call_frame_dummy proc
	link	a6,#$0000
	unlk	a6
	rts	
8000040A                               4E 71                       Nq   

;; frobulate: 8000040C
frobulate proc
	link	a6,#$0000
	move.l	d2,-(a7)
	move.l	$0008(a6),d0
	muls.l	$0008(a6),d0
	move.l	d0,d2
	muls.l	#$6208CECB,d1,d2
	moveq	#$+09,d2
	asr.l	d2,d1
	add.l	d0,d0
	subx.l	d0,d0
	move.l	d1,d2
	sub.l	d0,d2
	move.l	d2,d0
	move.l	(a7)+,d2
	unlk	a6
	rts	

;; bazulate: 8000043A
bazulate proc
	link	a6,#$0000
	move.l	d3,-(a7)
	move.l	d2,-(a7)
	move.l	$0008(a6),d2
	add.l	$000C(a6),d2
	move.l	$0008(a6),-(a7)
	jsr.l	$8000040C
	addq.l	#$04,a7
	move.l	d2,d3
	divsl.l	d0,d1,d3
	move.l	d3,d0
	move.l	d0,d2
	move.l	$000C(a6),-(a7)
	jsr.l	$8000040C
	addq.l	#$04,a7
	move.l	d2,d3
	divsl.l	d0,d1,d3
	move.l	d3,d0
	move.l	$-0008(a6),d2
	move.l	$-0004(a6),d3
	unlk	a6
	rts	

;; switcheroo: 80000480
switcheroo proc
	link	a6,#$0000
	movea.l	$0008(a6),a0
	moveq	#$+04,d0
	cmp.l	a0,d0
	beq	$800004B4

l8000048E:
	moveq	#$+04,d0
	cmp.l	a0,d0
	blt	$8000049E

l80000494:
	lea	(a0),a0
	moveq	#$+02,d0
	cmp.l	a0,d0
	bcs	$800004D6

l8000049C:
	bra	$800004A6

l8000049E:
	moveq	#$+06,d0
	cmp.l	a0,d0
	beq	$800004C6

l800004A4:
	bra	$800004D6

l800004A6:
	move.l	$0008(a6),-(a7)
	jsr.l	$8000040C
	addq.l	#$04,a7
	bra	$800004E2

l800004B4:
	move.l	$0008(a6),d0
	subq.l	#$03,d0
	move.l	d0,-(a7)
	jsr.l	$8000040C
	addq.l	#$04,a7
	bra	$800004E2

l800004C6:
	move.l	$0008(a6),-(a7)
	move.l	$0008(a6),-(a7)
	jsr.l	$8000043A
	addq.l	#$08,a7

l800004D6:
	clr.l	-(a7)
	clr.l	-(a7)
	jsr.l	$8000043A
	addq.l	#$08,a7

l800004E2:
	move.l	$0008(a6),d0
	addq.l	#$01,d0
	unlk	a6
	rts	

;; main: 800004EC
main proc
	link	a6,#$0000
	move.l	$0008(a6),-(a7)
	jsr.l	$80000480
	addq.l	#$04,a7
	clr.l	d0
	unlk	a6
	rts	
80000502       4E 71                                       Nq           

;; __libc_csu_init: 80000504
__libc_csu_init proc
	link	a6,#$0000
	movem.l	d2-d6/a2/a5,-(a7)
	lea	(00003AF2,pc),a5
	move.l	$0008(a6),d4
	move.l	$000C(a6),d5
	move.l	$0010(a6),d6
	bsr	$80000298
	movea.l	(00000018,a5),a2
	move.l	(0000001C,a5),d3
	sub.l	a2,d3
	asr.l	#$02,d3
	beq	$80000552

l8000053C:
	clr.l	d2

l8000053E:
	movea.l	(a2)+,a0
	move.l	d6,-(a7)
	move.l	d5,-(a7)
	move.l	d4,-(a7)
	jsr.l	(a0)
	addq.l	#$01,d2
	lea	$000C(a7),a7
	cmp.l	d3,d2
	bne	$8000053E

l80000552:
	movem.l	$-001C(a6),d2-d6/a2/a5
	unlk	a6
	rts	

;; __libc_csu_fini: 8000055C
__libc_csu_fini proc
	link	a6,#$0000
	unlk	a6
	rts	

;; __do_global_ctors_aux: 80000564
__do_global_ctors_aux proc
	link	a6,#$0000
	move.l	a2,-(a7)
	movea.l	$80003F28,a0
	moveq	#$-01,d0
	cmp.l	a0,d0
	beq	$80000586

l80000576:
	lea	$80003F28,a2

l8000057C:
	jsr.l	(a0)
	movea.l	-(a2),a0
	moveq	#$-01,d0
	cmp.l	a0,d0
	bne	$8000057C

l80000586:
	movea.l	$-0004(a6),a2
	unlk	a6
	rts	

;; call___do_global_ctors_aux: 8000058E
call___do_global_ctors_aux proc
	link	a6,#$0000
	unlk	a6
	rts	
80000596                   4E 71                               Nq       
;;; Segment .fini (80000598)

;; _fini: 80000598
_fini proc
	link	a6,#$0000
	move.l	a5,-(a7)
	lea	(00003A60,pc),a5
	jsr.l	$80000390
	movea.l	$-0004(a6),a5
	unlk	a6
	rts	
;;; Segment .rodata (800005B4)
800005B4             00 02 00 01                             ....       
;;; Segment .eh_frame (800005B8)
800005B8                         00 00 00 00                     ....   
;;; Segment .ctors (80003F28)
80003F28                         FF FF FF FF 00 00 00 00         ........
;;; Segment .dtors (80003F30)
80003F30 FF FF FF FF 00 00 00 00                         ........       
;;; Segment .dynamic (80003F38)
; DT_NEEDED            libc.so.6
; DT_INIT              80000298
; DT_DEBUG             80000598
; DT_HASH              80000188
; DT_STRTAB            800001EC
; DT_SYMTAB            800001AC
; DT_STRSZ             00000045
; DT_SYMENT                  16
; DT_DEBUG             00000000
; DT_PLTGOT            80004000
; DT_PLTRELSZ                24
; DT_PLTREL            00000007
; DT_JMPREL            80000280
; DT_RELA              8000025C
; DT_RELASZ                  36
; DT_RELAENT                 12
; 6FFFFFFE             8000023C
; 6FFFFFFF             00000001
; 6FFFFFF0             80000232
;;; Segment .got (80004000)
80004000 80 00 3F 38 00 00 00 00 00 00 00 00 80 00 02 E8 ..?8............
80004010 80 00 02 FC 00 00 00 00 80 00 3F 28 80 00 3F 28 ..........?(..?(
;;; Segment .data (80004020)
80004020 00 00 00 00 00 00 00 00                         ........       
;;; Segment .bss (80004028)
80004028                         00 00                           ..     
8000402A                               00 00 00 00                 .... 
8000402E                                           00 00               ..
