;;; Segment .comment (00000000)
00000000 7F 45 4C 46 01 02 01 00 00 00 00 00 00 00 00 00 .ELF............
00000010 00 03 00 08 00 00 00 01 00 00 06 10             ............   
;;; Segment .interp (00000174)
00000174             2F 6C 69 62 2F 6C 64 2E 73 6F 2E 31     /lib/ld.so.1
00000180 00                                              .              
;;; Segment .note.ABI-tag (00000184)
00000184             00 00 00 04 00 00 00 10 00 00 00 01     ............
00000190 47 4E 55 00 00 00 00 00 00 00 00 03 00 00 00 02 GNU.............
000001A0 00 00 00 00                                     ....           
;;; Segment .MIPS.abiflags (000001A8)
000001A8                         00 00 20 02 01 01 00 05         .. .....
000001B0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
;;; Segment .reginfo (000001C0)
000001C0 B2 00 01 F6 00 00 00 00 00 00 00 00 00 00 00 00 ................
000001D0 00 00 00 00 00 01 8A 80                         ........       
;;; Segment .note.gnu.build-id (000001D8)
000001D8                         00 00 00 04 00 00 00 14         ........
000001E0 00 00 00 03 47 4E 55 00 90 C3 92 90 10 80 71 47 ....GNU.......qG
000001F0 15 5F 7C BA 41 C1 06 79 5C A1 DB 0E             ._|.A..y\...   
;;; Segment .dynamic (000001FC)
; DT_NEEDED            libc.so.6
; DT_INIT              00000588
; DT_DEBUG             00000A10
; DT_HASH              000002F4
; DT_STRTAB            00000448
; DT_SYMTAB            00000348
; DT_STRSZ             000000DD
; DT_SYMENT                  16
; DT_MIPS_RLD_MAP_REL  00010844
; DT_DEBUG             00000000
; DT_PLTGOT            00010A90
; DT_REL               00000578
; DT_RELSZ                   16
; DT_RELENT                   8
; DT_MIPS_RLD_VERSION  00000001
; DT_MIPS_FLAGS        00000002
; DT_MIPS_BASE_ADDRESS 00000000
; DT_MIPS_LOCAL_GOTNO        16
; DT_MIPS_SYMTABNO           16
; DT_MIPS_UNREFEXTNO         30
; DT_MIPS_GOTSYM              9
; DT_FLAGS_1           08000000
; DT_VERNEED           00000548
; DT_VERNEEDNUM               1
; DT_VERSYM            00000526
;;; Segment .hash (000002F4)
000002F4             00 00 00 03 00 00 00 10 00 00 00 08     ............
00000300 00 00 00 09 00 00 00 05 00 00 00 00 00 00 00 00 ................
00000310 00 00 00 00 00 00 00 00 00 00 00 0F 00 00 00 04 ................
00000320 00 00 00 0C 00 00 00 06 00 00 00 0B 00 00 00 0A ................
00000330 00 00 00 07 00 00 00 0D 00 00 00 0E 00 00 00 02 ................
00000340 00 00 00 03 00 00 00 00                         ........       
;;; Segment .dynsym (00000348)
; 0000                                          00000000 00000000 00 
; 0001                                          00000588 00000000 03 .init
; 0002 __libc_csu_fini                          00000964 00000008 12 .text
; 0003 _DYNAMIC_LINKING                         00000001 00000000 13 SHN_ABS
; 0004 _IO_stdin_used                           00000A58 00000004 11 .rodata
; 0005 __RLD_MAP                                00010A80 00000000 11 .rld_map
; 0006 __libc_csu_init                          000008C0 000000A4 12 .text
; 0007 main                                     000007F0 000000CC 12 .text
; 0008 _init                                    00000588 00000000 12 .init
; 0009 calloc                                   000009F0 00000000 12 
; 000A _ITM_registerTMCloneTable                00000000 00000000 20 
; 000B memset                                   000009E0 00000000 12 
; 000C __libc_start_main                        000009D0 00000000 12 
; 000D __gmon_start__                           00000000 00000000 22 
; 000E _ITM_deregisterTMCloneTable              00000000 00000000 20 
; 000F __cxa_finalize                           00000000 00000000 22 
;;; Segment .dynstr (00000448)
00000448                         00 5F 44 59 4E 41 4D 49         ._DYNAMI
00000450 43 5F 4C 49 4E 4B 49 4E 47 00 5F 5F 52 4C 44 5F C_LINKING.__RLD_
00000460 4D 41 50 00 5F 5F 6C 69 62 63 5F 63 73 75 5F 69 MAP.__libc_csu_i
00000470 6E 69 74 00 5F 5F 6C 69 62 63 5F 63 73 75 5F 66 nit.__libc_csu_f
00000480 69 6E 69 00 5F 5F 6C 69 62 63 5F 73 74 61 72 74 ini.__libc_start
00000490 5F 6D 61 69 6E 00 5F 5F 67 6D 6F 6E 5F 73 74 61 _main.__gmon_sta
000004A0 72 74 5F 5F 00 5F 49 54 4D 5F 64 65 72 65 67 69 rt__._ITM_deregi
000004B0 73 74 65 72 54 4D 43 6C 6F 6E 65 54 61 62 6C 65 sterTMCloneTable
000004C0 00 5F 49 54 4D 5F 72 65 67 69 73 74 65 72 54 4D ._ITM_registerTM
000004D0 43 6C 6F 6E 65 54 61 62 6C 65 00 5F 5F 63 78 61 CloneTable.__cxa
000004E0 5F 66 69 6E 61 6C 69 7A 65 00 6D 65 6D 73 65 74 _finalize.memset
000004F0 00 63 61 6C 6C 6F 63 00 6C 69 62 63 2E 73 6F 2E .calloc.libc.so.
00000500 36 00 5F 49 4F 5F 73 74 64 69 6E 5F 75 73 65 64 6._IO_stdin_used
00000510 00 47 4C 49 42 43 5F 32 2E 30 00 47 4C 49 42 43 .GLIBC_2.0.GLIBC
00000520 5F 32 2E 32 00                                  _2.2.          
;;; Segment .gnu.version (00000526)
00000526                   00 00 00 00 00 01 00 01 00 01       ..........
00000530 00 01 00 01 00 01 00 01 00 03 00 00 00 03 00 03 ................
00000540 00 00 00 00 00 02                               ......         
;;; Segment .gnu.version_r (00000548)
00000548                         00 01 00 02 00 00 00 B0         ........
00000550 00 00 00 10 00 00 00 00 0D 69 69 10 00 00 00 03 .........ii.....
00000560 00 00 00 C9 00 00 00 10 0D 69 69 12 00 00 00 02 .........ii.....
00000570 00 00 00 D3 00 00 00 00                         ........       
;;; Segment .rel.dyn (00000578)
; 00000000   0 00000000 
; 00010AEC   3 00000000 
;;; Segment .init (00000588)

;; _init: 00000588
_init proc
	lui	r28,+0002
	addiu	r28,r28,-00007B08
	addu	r28,r28,r25
	addiu	sp,sp,-00000020
	sw	r28,0010(sp)
	sw	ra,001C(sp)
	lw	r2,-7FA0(r28)
	beq	r2,r0,000005B8
	nop

l000005AC:
	lw	r25,-7FA0(r28)
	jalr	ra,r25
	nop

l000005B8:
	bgezal	r0,000005C0
	nop
	lui	r28,+0002
	addiu	r28,r28,-00007B40
	addu	r28,r28,ra
	lw	r25,-7FCC(r28)
	addiu	r25,r25,+000007D4
	jalr	ra,r25
	nop
	bgezal	r0,000005E4
	nop
	lui	r28,+0002
	addiu	r28,r28,-00007B64
	addu	r28,r28,ra
	lw	r25,-7FCC(r28)
	addiu	r25,r25,+00000970
	jalr	ra,r25
	nop
	lw	ra,001C(sp)
	jr	ra
	addiu	sp,sp,+00000020
;;; Segment .text (00000610)

;; __start: 00000610
__start proc
	or	r0,ra,r0
	bgezal	r0,0000061C
	nop
	lui	r28,+0002
	addiu	r28,r28,-00007B9C
	addu	r28,r28,ra
	or	ra,r0,r0
	lw	r4,-7FE8(r28)
	lw	r5,0000(sp)
	addiu	r6,sp,+00000004
	addiu	r1,r0,-00000008
	and	sp,sp,r1
	addiu	sp,sp,-00000020
	lw	r7,-7FE4(r28)
	lw	r8,-7FE0(r28)
	sw	r8,0010(sp)
	sw	r2,0014(sp)
	sw	sp,0018(sp)
	lw	r25,-7FA4(r28)
	jalr	ra,r25
	nop

l00000664:
	beq	r0,r0,00000664
	nop
0000066C                                     00 00 00 00             ....

;; deregister_tm_clones: 00000670
deregister_tm_clones proc
	lui	r28,+0002
	addiu	r28,r28,-00007BF0
	addu	r28,r28,r25
	lw	r4,-7FD8(r28)
	lw	r2,-7FDC(r28)
	addiu	r4,r4,+00000A84
	beq	r2,r4,000006A0
	lw	r25,-7F9C(r28)

l00000690:
	beq	r25,r0,000006A0
	nop

l00000698:
	jr	r25
	nop

l000006A0:
	jr	ra
	nop

;; register_tm_clones: 000006A8
register_tm_clones proc
	lui	r28,+0002
	addiu	r28,r28,-00007C28
	addu	r28,r28,r25
	lw	r4,-7FD8(r28)
	lw	r5,-7FDC(r28)
	addiu	r4,r4,+00000A84
	subu	r5,r5,r4
	sra	r5,r5,02
	srl	r2,r5,1F
	addu	r5,r2,r5
	sra	r5,r5,01
	beq	r5,r0,000006EC
	lw	r25,-7FAC(r28)

l000006DC:
	beq	r25,r0,000006EC
	nop

l000006E4:
	jr	r25
	nop

l000006EC:
	jr	ra
	nop

;; __do_global_dtors_aux: 000006F4
__do_global_dtors_aux proc
	lui	r28,+0002
	addiu	r28,r28,-00007C74
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r19,0028(sp)
	lw	r19,-7FD8(r28)
	sw	r28,0010(sp)
	sw	ra,002C(sp)
	sw	r18,0024(sp)
	sw	r17,0020(sp)
	sw	r16,001C(sp)
	lbu	r2,0AF0(r19)
	bne	r2,r0,000007B8
	lw	r2,-7F98(r28)

l0000072C:
	beq	r2,r0,00000744
	lw	r2,-7FD4(r28)

l00000734:
	lw	r25,-7F98(r28)
	jalr	ra,r25
	lw	r4,0000(r2)
	lw	r28,0010(sp)

l00000744:
	lw	r2,-7FD8(r28)
	lw	r16,-7FD0(r28)
	lw	r17,-7FD8(r28)
	addiu	r3,r2,+00000A68
	addiu	r18,r2,+00000A68
	subu	r16,r16,r3
	lw	r2,0AF4(r17)
	sra	r16,r16,02
	addiu	r16,r16,-00000001
	sltu	r3,r2,r16
	beq	r3,r0,000007A4
	lw	r25,-7FCC(r28)

l00000774:
	addiu	r2,r2,+00000001
	sll	r3,r2,02
	sw	r2,0AF4(r17)
	addu	r2,r18,r3
	lw	r25,0000(r2)
	jalr	ra,r25
	nop
	lw	r2,0AF4(r17)
	sltu	r3,r2,r16
	bne	r3,r0,00000774
	lw	r28,0010(sp)

l000007A0:
	lw	r25,-7FCC(r28)

l000007A4:
	addiu	r25,r25,+00000670
	bgezal	r0,00000670
	nop
	addiu	r2,r0,+00000001
	sb	r2,0AF0(r19)

l000007B8:
	lw	ra,002C(sp)
	lw	r19,0028(sp)
	lw	r18,0024(sp)
	lw	r17,0020(sp)
	lw	r16,001C(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; frame_dummy: 000007D4
frame_dummy proc
	lui	r28,+0002
	addiu	r28,r28,-00007D54
	addu	r28,r28,r25
	lw	r25,-7FCC(r28)
	addiu	r25,r25,+000006A8
	beq	r0,r0,000006A8
	nop

;; main: 000007F0
main proc
	lui	r28,+0002
	addiu	r28,r28,-00007D70
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	ra,002C(sp)
	sw	r30,0028(sp)
	or	r30,sp,r0
	sw	r28,0010(sp)
	sw	r4,0030(r30)
	sw	r5,0034(r30)
	addiu	r2,r30,+0000001C
	addiu	r6,r0,+00000005
	or	r5,r0,r0
	or	r4,r2,r0
	lw	r2,-7FA8(r28)
	or	r25,r2,r0
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	addiu	r5,r0,+00000005
	addiu	r4,r0,+00000001
	lw	r2,-7FB0(r28)
	or	r25,r2,r0
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,0018(r30)
	lw	r2,0018(r30)
	lw	r3,001C(r30)
	swl	r3,0000(r2)
	swr	r3,0003(r2)
	lbu	r3,0020(r30)
	sb	r3,0004(r2)
	lw	r2,0018(r30)
	addiu	r3,r0,+0000000C
	sb	r3,0000(r2)
	lw	r2,0018(r30)
	swl	r0,0001(r2)
	swr	r0,0004(r2)
	addiu	r2,r0,+00000042
	sb	r2,001C(r30)
	lw	r2,0018(r30)
	swl	r2,001D(r30)
	swr	r2,0020(r30)
	or	r2,r0,r0
	or	sp,r30,r0
	lw	ra,002C(sp)
	lw	r30,0028(sp)
	addiu	sp,sp,+00000030
	jr	ra
	nop
000008BC                                     00 00 00 00             ....

;; __libc_csu_init: 000008C0
__libc_csu_init proc
	lui	r28,+0002
	addiu	r28,r28,-00007E40
	addu	r28,r28,r25
	addiu	sp,sp,-00000038
	lw	r25,-7FC8(r28)
	sw	r28,0010(sp)
	sw	r21,0030(sp)
	or	r21,r6,r0
	sw	r20,002C(sp)
	or	r20,r5,r0
	sw	r19,0028(sp)
	or	r19,r4,r0
	sw	r18,0024(sp)
	sw	r16,001C(sp)
	sw	ra,0034(sp)
	bgezal	r0,00000588
	sw	r17,0020(sp)
	lw	r28,0010(sp)
	lw	r16,-7FC4(r28)
	lw	r18,-7FC4(r28)
	subu	r18,r18,r16
	sra	r18,r18,02
	beq	r18,r0,00000940
	or	r17,r0,r0

l00000920:
	lw	r25,0000(r16)
	addiu	r17,r17,+00000001
	or	r6,r21,r0
	or	r5,r20,r0
	jalr	ra,r25
	or	r4,r19,r0
	bne	r18,r17,00000920
	addiu	r16,r16,+00000004

l00000940:
	lw	ra,0034(sp)
	lw	r21,0030(sp)
	lw	r20,002C(sp)
	lw	r19,0028(sp)
	lw	r18,0024(sp)
	lw	r17,0020(sp)
	lw	r16,001C(sp)
	jr	ra
	addiu	sp,sp,+00000038

;; __libc_csu_fini: 00000964
__libc_csu_fini proc
	jr	ra
	nop
0000096C                                     00 00 00 00             ....

;; __do_global_ctors_aux: 00000970
__do_global_ctors_aux proc
	lui	r28,+0002
	addiu	r28,r28,-00007EF0
	addu	r28,r28,r25
	lw	r3,-7FD8(r28)
	addiu	sp,sp,-00000028
	addiu	r2,r0,-00000001
	sw	r28,0010(sp)
	sw	ra,0024(sp)
	sw	r17,0020(sp)
	sw	r16,001C(sp)
	lw	r25,0A60(r3)
	beq	r25,r2,000009BC
	addiu	r17,r0,-00000001

l000009A4:
	addiu	r16,r3,+00000A60

l000009A8:
	jalr	ra,r25
	addiu	r16,r16,-00000004
	lw	r25,0000(r16)
	bne	r25,r17,000009A8
	nop

l000009BC:
	lw	ra,0024(sp)
	lw	r17,0020(sp)
	lw	r16,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028
;;; Segment .MIPS.stubs (000009D0)

;; __libc_start_main: 000009D0
__libc_start_main proc
	lw	r25,-7FF0(r28)
	or	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000000C

;; memset: 000009E0
memset proc
	lw	r25,-7FF0(r28)
	or	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000000B

;; calloc: 000009F0
calloc proc
	lw	r25,-7FF0(r28)
	or	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000009
	nop
	nop
	nop
	nop
;;; Segment .fini (00000A10)

;; _fini: 00000A10
_fini proc
	lui	r28,+0002
	addiu	r28,r28,-00007F90
	addu	r28,r28,r25
	addiu	sp,sp,-00000020
	sw	r28,0010(sp)
	sw	ra,001C(sp)
	bgezal	r0,00000A30
	nop
	lui	r28,+0002
	addiu	r28,r28,-00007FB0
	addu	r28,r28,ra
	lw	r25,-7FCC(r28)
	addiu	r25,r25,+000006F4
	jalr	ra,r25
	nop
	lw	ra,001C(sp)
	jr	ra
	addiu	sp,sp,+00000020
;;; Segment .rodata (00000A58)
_IO_stdin_used		; 00000A58
	dd	0x00020001
;;; Segment .eh_frame (00000A5C)
00000A5C                                     00 00 00 00             ....
;;; Segment .ctors (00010A60)
00010A60 FF FF FF FF 00 00 00 00                         ........       
;;; Segment .dtors (00010A68)
00010A68                         FF FF FF FF 00 00 00 00         ........
;;; Segment .data (00010A70)
00010A70 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
;;; Segment .rld_map (00010A80)
00010A80 00 00 00 00                                     ....           
;;; Segment .got (00010A90)
00010A90 00 00 00 00 80 00 00 00                         ........       
main_GOT		; 00010A98
	dd	0x000007F0
__libc_csu_init_GOT		; 00010A9C
	dd	0x000008C0
__libc_csu_fini_GOT		; 00010AA0
	dd	0x00000964
00010AA4             00 01 0A 84 00 01 00 00 00 01 0A EC     ............
00010AB0 00 01 0A 6C 00 00 00 00                         ...l....       
_init_GOT		; 00010AB8
	dd	0x00000588
00010ABC                                     00 01 0A 60             ...`
00010AC0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
calloc_GOT		; 00010AD0
	dd	0x000009F0
00010AD4             00 00 00 00                             ....       
memset_GOT		; 00010AD8
	dd	0x000009E0
__libc_start_main_GOT		; 00010ADC
	dd	0x000009D0
__gmon_start___GOT		; 00010AE0
	dd	0x00000000
00010AE4             00 00 00 00                             ....       
__cxa_finalize_GOT		; 00010AE8
	dd	0x00000000
;;; Segment .sdata (00010AEC)
00010AEC                                     00 01 0A EC             ....
;;; Segment .bss (00010AF0)
completed.6256		; 00010AF0
	db	0x00
00010AF1    00 00 00                                      ...           
dtor_idx.6258		; 00010AF4
	dd	0x00000000
00010AF8                         00 00 00 00 00 00 00 00         ........
