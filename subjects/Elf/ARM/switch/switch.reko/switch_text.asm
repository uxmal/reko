;;; Segment .text (00008334)

;; _start: 00008334
_start proc
	ldr	ip,[00008360]                                          ; [pc,#&24]
	mov	fp,#0
	pop	r1
	mov	r2,sp
	push	r2
	push	r0
	ldr	r0,[00008364]                                          ; [pc,#&10]
	ldr	r3,[00008368]                                          ; [pc,#&10]
	push	ip
	bl	$00008324
	bl	$00008314
00008360 B0 86 00 00 5C 85 00 00 54 86 00 00             ....\...T...    

;; call_gmon_start: 0000836C
;;   Called from:
;;     000082F4 (in _init)
call_gmon_start proc
	push	{r10,lr}
	ldr	r10,[00008394]                                         ; [pc,#&1C]
	ldr	r3,[00008398]                                          ; [pc,#&1C]
	add	r10,pc,r10
	ldr	r3,[r10,r3]
	cmp	r3,#0
	popeq	{r10,pc}

l00008388:
	mov	lr,pc
	mov	pc,r3
00008390 00 84 BD E8 C8 84 00 00 14 00 00 00             ............    

;; __do_global_dtors_aux: 0000839C
;;   Called from:
;;     00008750 (in _fini)
__do_global_dtors_aux proc
	push	{r4-r5,lr}
	ldr	r5,[000083F4]                                          ; [pc,#&4C]
	ldrb	r3,[r5]
	cmp	r3,#0
	popne	{r4-r5,pc}

l000083B0:
	ldr	r4,[000083F8]                                          ; [pc,#&40]
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
	push	lr
	pop	pc

;; frame_dummy: 00008404
;;   Called from:
;;     000082F8 (in _init)
frame_dummy proc
	ldr	r0,[00008424]                                          ; [pc,#&18]
	ldr	r3,[r0]
	cmp	r3,#0
	moveq	pc,lr

l00008414:
	ldr	r3,[00008428]                                          ; [pc,#&C]
	cmp	r3,#0
	moveq	pc,lr

l00008420:
	b	$00000000
00008424             44 08 01 00 00 00 00 00                 D.......    

;; call_frame_dummy: 0000842C
call_frame_dummy proc
	push	lr
	pop	pc

;; frobulate: 00008434
;;   Called from:
;;     00008498 (in bazulate)
;;     000084B4 (in bazulate)
;;     00008518 (in switcheroo)
;;     0000852C (in switcheroo)
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
	ldmdb	fp,{fp,sp,pc}

;; bazulate: 00008470
;;   Called from:
;;     0000853C (in switcheroo)
;;     00008548 (in switcheroo)
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
	ldmdb	fp,{r4,fp,sp,pc}

;; switcheroo: 000084D4
;;   Called from:
;;     00008578 (in main)
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
l000084F8	dd	0x00008514
l000084FC	dd	0x00008514
l00008500	dd	0x00008514
l00008504	dd	0x00008540
l00008508	dd	0x00008520
l0000850C	dd	0x00008540
l00008510	dd	0x00008534

l00008514:
	ldr	r0,[fp,-#&10]
	bl	$00008434
	b	$0000854C

l00008520:
	ldr	r3,[fp,-#&10]
	sub	r3,r3,#3
	mov	r0,r3
	bl	$00008434
	b	$0000854C

l00008534:
	ldr	r0,[fp,-#&10]
	ldr	r1,[fp,-#&10]
	bl	$00008470

l00008540:
	mov	r0,#0
	mov	r1,#0
	bl	$00008470

l0000854C:
	ldr	r3,[fp,-#&10]
	add	r3,r3,#1
	mov	r0,r3
	ldmdb	fp,{fp,sp,pc}

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
	ldmdb	fp,{fp,sp,pc}

;; __divsi3: 00008588
;;   Called from:
;;     00008460 (in frobulate)
;;     000084A8 (in bazulate)
;;     000084C4 (in bazulate)
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
	cmp	r1,#&10000000
	cmplo	r1,r0

l000085B8:
	lsllo	r1,r1,#4

l000085BC:
	lsllo	r3,r3,#4

l000085C0:
	blo	$000085B0

l000085C4:
	cmp	r1,#&80000000
	cmplo	r1,r0

l000085CC:
	lsllo	r1,r1,#1

l000085D0:
	lsllo	r3,r3,#1

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
	lsrsne	r3,r3,#4

l00008610:
	lsrne	r1,r1,#4

l00008614:
	bne	$000085D8

l00008618:
	mov	r0,r2
	cmp	ip,#0
	rsbmi	r0,r0,#0

l00008624:
	mov	pc,lr

l00008628:
	push	lr
	bl	$00008638
	mov	r0,#0
	pop	pc

;; __div0: 00008638
;;   Called from:
;;     0000862C (in __divsi3)
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
	ldr	r10,[000086A4]                                         ; [pc,#&40]
	add	r10,pc,r10
	bl	$000082F0
	ldr	r3,[000086A8]                                          ; [pc,#&38]
	ldr	r2,[000086AC]                                          ; [pc,#&38]
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
	ldr	r10,[00008700]                                         ; [pc,#&44]
	ldr	r3,[00008704]                                          ; [pc,#&44]
	ldr	r2,[00008708]                                          ; [pc,#&44]
	add	r10,pc,r10
	ldr	r1,[r10,r3]
	ldr	r3,[r10,r2]
	rsb	r3,r1,r3
	asr	r4,r3,#2
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
;;   Called from:
;;     000082FC (in _init)
__do_global_ctors_aux proc
	push	{r4,lr}
	ldr	r3,[00008740]                                          ; [pc,#&28]
	ldr	r2,[r3,-#&4]
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
	push	lr
	pop	pc
