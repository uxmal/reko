;;; Segment .text (00000610)

;; __start: 00000610
__start proc
	or	r0,ra,r0
	bal	0000061C
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
;;   Called from:
;;     000007A8 (in __do_global_dtors_aux)
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
;;   Called from:
;;     000007E8 (in frame_dummy)
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
	bal	00000670
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
	bal	00000588
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
