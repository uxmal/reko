;;; Segment .interp (000080F4)
000080F4             2F 6C 69 62 2F 6C 64 2D 6C 69 6E 75     /lib/ld-linu
00008100 78 2E 73 6F 2E 32 00                            x.so.2.        
;;; Segment .note.ABI-tag (00008108)
00008108                         04 00 00 00 10 00 00 00         ........
00008110 01 00 00 00 47 4E 55 00 00 00 00 00 02 00 00 00 ....GNU.........
00008120 04 00 00 00 00 00 00 00                         ........       
;;; Segment .hash (00008128)
00008128                         03 00 00 00 09 00 00 00         ........
00008130 08 00 00 00 06 00 00 00 07 00 00 00 00 00 00 00 ................
00008140 00 00 00 00 01 00 00 00 02 00 00 00 00 00 00 00 ................
00008150 03 00 00 00 04 00 00 00 05 00 00 00 00 00 00 00 ................
;;; Segment .dynsym (00008160)
; 0000                                          00000000 00000000 00 
; 0001 __fini_array_end                         0001075C 00000000 10 SHN_ABS
; 0002 abort                                    00008314 000001DC 12 
; 0003 __fini_array_start                       0001075C 00000000 10 SHN_ABS
; 0004 __libc_start_main                        00008324 00000128 12 
; 0005 __init_array_end                         0001075C 00000000 10 SHN_ABS
; 0006 __init_array_start                       0001075C 00000000 10 SHN_ABS
; 0007 _IO_stdin_used                           00008758 00000004 11 .rodata
; 0008 __gmon_start__                           00000000 00000000 20 
;;; Segment .dynstr (000081F0)
000081F0 00 5F 5F 67 6D 6F 6E 5F 73 74 61 72 74 5F 5F 00 .__gmon_start__.
00008200 6C 69 62 63 2E 73 6F 2E 36 00 61 62 6F 72 74 00 libc.so.6.abort.
00008210 5F 49 4F 5F 73 74 64 69 6E 5F 75 73 65 64 00 5F _IO_stdin_used._
00008220 5F 6C 69 62 63 5F 73 74 61 72 74 5F 6D 61 69 6E _libc_start_main
00008230 00 5F 5F 69 6E 69 74 5F 61 72 72 61 79 5F 73 74 .__init_array_st
00008240 61 72 74 00 5F 5F 69 6E 69 74 5F 61 72 72 61 79 art.__init_array
00008250 5F 65 6E 64 00 5F 5F 66 69 6E 69 5F 61 72 72 61 _end.__fini_arra
00008260 79 5F 73 74 61 72 74 00 5F 5F 66 69 6E 69 5F 61 y_start.__fini_a
00008270 72 72 61 79 5F 65 6E 64 00 47 4C 49 42 43 5F 32 rray_end.GLIBC_2
00008280 2E 30 00                                        .0.            
;;; Segment .gnu.version (00008284)
00008284             00 00 01 00 02 00 01 00 02 00 01 00     ............
00008290 01 00 01 00 00 00                               ......         
;;; Segment .gnu.version_r (00008298)
00008298                         01 00 01 00 10 00 00 00         ........
000082A0 10 00 00 00 00 00 00 00 10 69 69 0D 00 00 02 00 .........ii.....
000082B0 89 00 00 00 00 00 00 00                         ........       
;;; Segment .rel.dyn (000082B8)
; 0001085C  21 00000008 __gmon_start__
; 00010860  21 00000006 __init_array_start
; 00010864  21 00000005 __init_array_end
; 00010868  21 00000003 __fini_array_start
; 0001086C  21 00000001 __fini_array_end
;;; Segment .rel.plt (000082E0)
; 00010854  22 00000002 abort
; 00010858  22 00000004 __libc_start_main
;;; Segment .init (000082F0)

;; _init: 000082F0
_init proc
	str	lr,[sp,-#4]!
	bl	$0000836C
	bl	$00008404
	bl	$0000870C
	pop	{pc}
;;; Segment .plt (00008304)
00008304             04 E0 2D E5 10 E0 9F E5 0E E0 8F E0     ..-.........
00008310 08 F0 BE E5                                     ....           

;; abort: 00008314
abort proc
	ldr	ip,[pc,#4]                                             ; 00008320
	add	ip,pc,ip
	ldr	pc,[ip]
00008320 34 85 00 00                                     4...           

;; __libc_start_main: 00008324
__libc_start_main proc
	ldr	ip,[pc,#4]                                             ; 00008330
	add	ip,pc,ip
	ldr	pc,[ip]
00008330 28 85 00 00                                     (...           
;;; Segment .text (00008334)

;; _start: 00008334
_start proc
	ldr	ip,[pc,#&24]                                           ; 00008360
	mov	fp,#0
	pop	{r1}
	mov	r2,sp
	str	r2,[sp,-#4]!
	str	r0,[sp,-#4]!
	ldr	r0,[pc,#&10]                                           ; 00008364
	ldr	r3,[pc,#&10]                                           ; 00008368
	str	ip,[sp,-#4]!
	bl	$00008324
	bl	$00008314
	strheq	r8,[r0],-r0

l00008364:
	andeq	r8,r0,ip,asr r5

l00008368:
	andeq	r8,r0,r4,asr r6

;; call_gmon_start: 0000836C
call_gmon_start proc
	push	{r10,lr}
	ldr	r10,[pc,#&1C]                                          ; 00008394
	ldr	r3,[pc,#&1C]                                           ; 00008398
	add	r10,pc,r10
	ldr	r3,[r10,r3]
	cmp	r3,#0
	popeq	{r10,pc}

l00008388:
	mov	lr,pc
	mov	pc,r3
00008390 00 84 BD E8 C8 84 00 00 14 00 00 00             ............   

;; __do_global_dtors_aux: 0000839C
__do_global_dtors_aux proc
	push	{r4-r5,lr}
	ldr	r5,[pc,#&4C]                                           ; 000083F4
	ldrb	r3,[r5]
	cmp	r3,#0
	popne	{r4-r5,pc}

l000083B0:
	ldr	r4,[pc,#&40]                                           ; 000083F8
	ldr	r3,[r4]
	ldr	r2,[r3]
	cmp	r2,#0
	beq	$000083E8

l000083C4:
	ldr	r3,[r4]
	add	r3,r3,#4
	str	r3,[r4]
	mov	lr,pc
	mov	pc,r2
000083D8                         00 30 94 E5 00 20 93 E5         .0... ..
000083E0 00 00 52 E3 F6 FF FF 1A                         ..R.....       

l000083E8:
	mov	r3,#1
	strb	r3,[r5]
	pop	{r4-r5,pc}
000083F4             70 08 01 00 64 07 01 00                 p...d...   

;; call___do_global_dtors_aux: 000083FC
call___do_global_dtors_aux proc
	str	lr,[sp,-#4]!
	pop	{pc}

;; frame_dummy: 00008404
frame_dummy proc
	ldr	r0,[pc,#&18]                                           ; 00008424
	ldr	r3,[r0]
	cmp	r3,#0
	moveq	pc,lr

l00008414:
	ldr	r3,[pc,#&C]                                            ; 00008428
	cmp	r3,#0
	moveq	pc,lr

l00008420:
	b	$00000000
00008424             44 08 01 00 00 00 00 00                 D.......   

;; call_frame_dummy: 0000842C
call_frame_dummy proc
	str	lr,[sp,-#4]!
	pop	{pc}

;; frobulate: 00008434
frobulate proc
	mov	ip,sp
	push	{fp-ip,lr-pc}
	sub	fp,ip,#4
	sub	sp,sp,#4
	str	r0,[fp,-#&10]
	ldr	r2,[fp,-#&10]
	ldr	r3,[fp,-#&10]
	mul	r3,r2,r3
	mov	r0,r3
	mov	r1,#&530
	add	r1,r1,#9
	bl	$00008588
	mov	r3,r0
	mov	r0,r3
	ldmdb	fp,fp,sp,pc

;; bazulate: 00008470
bazulate proc
	mov	ip,sp
	push	{r4,fp-ip,lr-pc}
	sub	fp,ip,#4
	sub	sp,sp,#8
	str	r0,[fp,-#&14]
	str	r1,[fp,-#&18]
	ldr	r2,[fp,-#&14]
	ldr	r3,[fp,-#&18]
	add	r4,r2,r3
	ldr	r0,[fp,-#&14]
	bl	$00008434
	mov	r3,r0
	mov	r0,r4
	mov	r1,r3
	bl	$00008588
	mov	r4,r0
	ldr	r0,[fp,-#&18]
	bl	$00008434
	mov	r3,r0
	mov	r0,r4
	mov	r1,r3
	bl	$00008588
	mov	r3,r0
	mov	r0,r3
	ldmdb	fp,r4,fp,sp

;; switcheroo: 000084D4
switcheroo proc
	mov	ip,sp
	push	{fp-ip,lr-pc}
	sub	fp,ip,#4
	sub	sp,sp,#4
	str	r0,[fp,-#&10]
	ldr	r3,[fp,-#&10]
	cmp	r3,#6
	ldrls	pc,[pc,r3,lsl #2]                                    ; 000084F8

l000084F4:
	b	$00008540
000084F8                         14 85 00 00 14 85 00 00         ........
00008500 14 85 00 00 40 85 00 00 20 85 00 00 40 85 00 00 ....@... ...@...
00008510 34 85 00 00 10 00 1B E5 C5 FF FF EB 0A 00 00 EA 4...............
00008520 10 30 1B E5 03 30 43 E2 03 00 A0 E1 C0 FF FF EB .0...0C.........
00008530 05 00 00 EA 10 00 1B E5 10 10 1B E5 CB FF FF EB ................

l00008540:
	mov	r0,#0
	mov	r1,#0
	bl	$00008470
	ldr	r3,[fp,-#&10]
	add	r3,r3,#1
	mov	r0,r3
	ldmdb	fp,fp,sp,pc

;; main: 0000855C
main proc
	mov	ip,sp
	push	{fp-ip,lr-pc}
	sub	fp,ip,#4
	sub	sp,sp,#8
	str	r0,[fp,-#&10]
	str	r1,[fp,-#&14]
	ldr	r0,[fp,-#&10]
	bl	$000084D4
	mov	r3,#0
	mov	r0,r3
	ldmdb	fp,fp,sp,pc

;; __divsi3: 00008588
__divsi3 proc
	eor	ip,r0,r1
	mov	r3,#1
	mov	r2,#0
	cmp	r1,#0
	rsbmi	r1,r1,#0

l0000859C:
	beq	$00008628

l000085A0:
	cmp	r0,#0
	rsbmi	r0,r0,#0

l000085A8:
	cmp	r0,r1
	blo	$00008618

l000085B0:
	cmp	r1,#1<<28
	cmplo	r1,r0

l000085B8:
	lsllo	r1,r1,lsl #4

l000085BC:
	lsllo	r3,r3,lsl #4

l000085C0:
	blo	$000085B0

l000085C4:
	cmp	r1,#&80000000
	cmplo	r1,r0

l000085CC:
	lsllo	r1,r1,lsl #1

l000085D0:
	lsllo	r3,r3,lsl #1

l000085D4:
	blo	$000085C4

l000085D8:
	cmp	r0,r1
	subhs	r0,r0,r1

l000085E0:
	orrhs	r2,r2,r3

l000085E4:
	cmp	r0,r1,lsr #1
	subhs	r0,r0,r1,lsr #1

l000085EC:
	orrhs	r2,r2,r3,lsr #1

l000085F0:
	cmp	r0,r1,lsr #2
	subhs	r0,r0,r1,lsr #2

l000085F8:
	orrhs	r2,r2,r3,lsr #2

l000085FC:
	cmp	r0,r1,lsr #3
	subhs	r0,r0,r1,lsr #3

l00008604:
	orrhs	r2,r2,r3,lsr #3

l00008608:
	cmp	r0,#0
	lsrsne	r3,r3,lsr #4

l00008610:
	lsrne	r1,r1,lsr #4

l00008614:
	bne	$000085D8

l00008618:
	mov	r0,r2
	cmp	ip,#0
	rsbmi	r0,r0,#0

l00008624:
	mov	pc,lr

l00008628:
	str	lr,[sp,-#4]!
	bl	$00008638
	mov	r0,#0
	pop	{pc}

;; __div0: 00008638
__div0 proc
	push	{r1,lr}
	svc	#&900014
	cmn	r0,#&3E8
	pophs	{r1,pc}

l00008648:
	mov	r1,#8
	svc	#&900025
	pop	{r1,pc}

;; __libc_csu_init: 00008654
__libc_csu_init proc
	push	{r4-r6,r10,lr}
	mov	r4,#0
	ldr	r10,[pc,#&40]                                          ; 000086A4
	add	r10,pc,r10
	bl	$000082F0
	ldr	r3,[pc,#&38]                                           ; 000086A8
	ldr	r2,[pc,#&38]                                           ; 000086AC
	ldr	r1,[r10,r3]
	ldr	r3,[r10,r2]
	rsb	r3,r1,r3
	cmp	r4,r3,asr #2
	pophs	{r4-r6,r10,pc}

l00008684:
	mov	r6,r1
	mov	r5,r3
	mov	lr,pc
	ldr	pc,[r6,r4,lsl #2]
00008694             01 40 84 E2 45 01 54 E1 FA FF FF 3A     .@..E.T....:
000086A0 70 84 BD E8 E0 81 00 00 18 00 00 00 1C 00 00 00 p...............

;; __libc_csu_fini: 000086B0
__libc_csu_fini proc
	push	{r4-r5,r10,lr}
	ldr	r10,[pc,#&44]                                          ; 00008700
	ldr	r3,[pc,#&44]                                           ; 00008704
	ldr	r2,[pc,#&44]                                           ; 00008708
	add	r10,pc,r10
	ldr	r1,[r10,r3]
	ldr	r3,[r10,r2]
	rsb	r3,r1,r3
	asr	r4,r3,asr #2
	cmp	r4,#0
	sub	r4,r4,#1
	beq	$000086F8

l000086E0:
	mov	r5,r1
	mov	lr,pc
	ldr	pc,[r5,r4,lsl #2]
000086EC                                     00 00 54 E3             ..T.
000086F0 01 40 44 E2 FA FF FF 1A                         .@D.....       

l000086F8:
	pop	{r4-r5,r10,lr}
	b	$0000874C
00008700 80 81 00 00 20 00 00 00 24 00 00 00             .... ...$...   

;; __do_global_ctors_aux: 0000870C
__do_global_ctors_aux proc
	push	{r4,lr}
	ldr	r3,[pc,#&28]                                           ; 00008740
	ldr	r2,[r3,-#4]
	cmn	r2,#1
	sub	r4,r3,#4
	popeq	{r4,pc}

l00008724:
	mov	r3,r2
	mov	lr,pc
	mov	pc,r3
00008730 04 30 34 E5 01 00 73 E3 FA FF FF 1A 10 80 BD E8 .04...s.........
00008740 38 08 01 00                                     8...           

;; call___do_global_ctors_aux: 00008744
call___do_global_ctors_aux proc
	str	lr,[sp,-#4]!
	pop	{pc}
;;; Segment .fini (0000874C)

;; _fini: 0000874C
_fini proc
	str	lr,[sp,-#4]!
	bl	$0000839C
	pop	{pc}
;;; Segment .rodata (00008758)
00008758                         01 00 02 00                     ....   
;;; Segment .data (0001075C)
0001075C                                     00 00 00 00             ....
00010760 00 00 00 00 40 08 01 00                         ....@...       
;;; Segment .eh_frame (00010768)
00010768                         00 00 00 00                     ....   
;;; Segment .dynamic (0001076C)
; DT_NEEDED            libc.so.6
; DT_INIT              000082F0
; DT_DEBUG             0000874C
; DT_HASH              00008128
; DT_STRTAB            000081F0
; DT_SYMTAB            00008160
; DT_STRSZ             00000093
; DT_SYMENT                  16
; DT_DEBUG             00000000
; DT_PLTGOT            00010848
; DT_PLTRELSZ                16
; DT_PLTREL            00000011
; DT_JMPREL            000082E0
; DT_REL               000082B8
; DT_RELSZ                   40
; DT_RELENT                   8
; 6FFFFFFE             00008298
; 6FFFFFFF             00000001
; 6FFFFFF0             00008284
;;; Segment .ctors (00010834)
00010834             FF FF FF FF 00 00 00 00                 ........   
;;; Segment .dtors (0001083C)
0001083C                                     FF FF FF FF             ....
00010840 00 00 00 00                                     ....           
;;; Segment .jcr (00010844)
00010844             00 00 00 00                             ....       
;;; Segment .got (00010848)
00010848                         6C 07 01 00 00 00 00 00         l.......
00010850 00 00 00 00 04 83 00 00 04 83 00 00 00 00 00 00 ................
00010860 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
;;; Segment .sbss (00010870)
00010870 00                                              .              
00010871    00 00 00                                      ...           
