;;; Segment code (0000)

;; fn0000: 0000
fn0000 proc
	mov	[0xFF12],8004
	nop
	mov	[0xFF12],8004
	nop
	jmps	00,0010

l0000_0010:
	mov	DPP0,0000
	mov	DPP1,0001
	mov	DPP2,0002
	mov	DPP3,0003
	mov	PSW,0000
	bclr	TFR:15
	mov	r0,0000
	push	r0
	push	r0
	mov	r0,0034
	push	r0
	reti

;; fn0034: 0034
fn0034 proc
	mov	CP,FC00
	mov	SP,FC00
	mov	STKOV,FA00
	mov	STKUN,FC00
	bset	P3:13
	bset	DP3:13
	mov	ADDRSEL1,1008
	bset	DP3:10
	mov	S0CON,8011
	mov	S0BG,001F
	mov	S0TIC,0000
	mov	S0RIC,0000
	mov	S0EIC,0000
	diswdt
	einit

l006A:
	mov	r1,013E
	calla	cc_UC,0128
	calla	cc_UC,011C
	and	r0,00DF
	cmp	r0,0044
	jmpr	cc_Z,0098

l0080:
	cmp	r0,0047
	jmpr	cc_Z,0088

l0086:
	jmpr	cc_UC,006A

l0088:
	calla	cc_UC,011C
	movb	rh1,rl0
	calla	cc_UC,011C
	movb	rl1,rl0
	push	r1
	ret

l0098:
	calla	cc_UC,011C
	cmpb	rl0,3A
	jmpr	cc_NZ,0098

l00A2:
	calla	cc_UC,011C
	cmpb	rl0,00
	jmpr	cc_Z,00DA

l00AA:
	mov	r2,r0
	and	r2,00FF
	calla	cc_UC,011C
	movb	rh1,rl0
	calla	cc_UC,011C
	movb	rl1,rl0
	calla	cc_UC,011C

l00C0:
	calla	cc_UC,011C
	movb	[r1],rl0
	add	r1,01
	cmpd1	r2,0001
	jmpr	cc_UGT,00C0

l00CC:
	calla	cc_UC,011C
	mov	r0,002E
	calla	cc_UC,0110
	jmpr	cc_UC,0098

l00DA:
	calla	cc_UC,011C
	calla	cc_UC,011C
	calla	cc_UC,011C
	calla	cc_UC,011C
	mov	r0,002E
	calla	cc_UC,0110
	jmpa	cc_UC,006A
00F6                   EA 00 6A 00 EC F0 E6 F0 0D 00       ..j.......
0100 CA 00 10 01 E6 F0 0A 00 CA 00 10 01 FC F0 CB 00 ................

;; fn0110: 0110
;;   Called from:
;;     00D4 (in fn0034)
;;     00EE (in fn0034)
;;     0132 (in fn0128)
fn0110 proc
	mov	[0xFEB0],r0

l0114:
	jnb	S0TIC:7,0114

l0118:
	bclr	S0TIC:7
	ret

;; fn011C: 011C
;;   Called from:
;;     0072 (in fn0034)
;;     0088 (in fn0034)
;;     008E (in fn0034)
;;     0098 (in fn0034)
;;     00A2 (in fn0034)
;;     00B0 (in fn0034)
;;     00B6 (in fn0034)
;;     00BC (in fn0034)
;;     00C0 (in fn0034)
;;     00CC (in fn0034)
;;     00DA (in fn0034)
;;     00DE (in fn0034)
;;     00E2 (in fn0034)
;;     00E6 (in fn0034)
fn011C proc
	jnb	S0RIC:7,011C

l0120:
	mov	r0,[0xFEB2]
	bclr	S0RIC:7
	ret

;; fn0128: 0128
;;   Called from:
;;     006E (in fn0034)
fn0128 proc
	movb	rl0,[r1]
	cmpb	rl0,00
	jmpa	cc_Z,013C

l0132:
	calla	cc_UC,0110
	add	r1,0001
	jmpr	cc_UC,0128

l013C:
	ret
013E                                           0D 0A               ..
0140 43 31 36 37 20 6D 69 6E 6D 6F 6E 20 3E 20 00 00 C167 minmon > ..
