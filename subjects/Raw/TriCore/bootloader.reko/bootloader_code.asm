;;; Segment code (00000000)

;; fn00000000: 00000000
fn00000000 proc
	j	00000014
00000004             8C 80 00 00 8C 80 00 00 8C 80 00 00     ............
00000010 8C 80 00 00                                     ....            

l00000014:
	ld.w	d1,[0003C5F0]
	jz.t	d1,#0x0,00000020

l0000001C:
	jl	fn000000EC

l00000020:
	movh.a	a10,#0xB001
	lea	a10,[a10]
	movh	d0,#0xD000
	addi	d0,d0,#0x348
	mtcr	ISP,d0
	isync
	movh	d0,#0xC000
	addi	d0,d0,#0x300
	mtcr	BTV,d0
	isync
	mfcr	d0,PSW
	or	d0,d0,#0x7F
	andn	d0,d0,#0x80
	mtcr	PSW,d0
	isync
	mfcr	d0,PSW
	or	d0,d0,#0x100
	mtcr	PSW,d0
	isync
	movh.a	a0,#0xD001
	lea	a0,[a0]-32752
	movh.a	a1,#0xC001
	lea	a1,[a1]-24744
	movh.a	a8,#0x0
	lea	a8,[a8]
	movh.a	a9,#0x0
	lea	a9,[a9]
	mfcr	d0,PSW
	andn	d0,d0,#0x100
	mtcr	PSW,d0
	isync
	movh.a	a15,#0xC000
	lea	a15,[a15]168
	nop
	ji	a15

;; fn000000A8: 000000A8
fn000000A8 proc
	jl	fn00000294
	jl	fn00000128
	jl	fn0000016E
	jl	fn000001E2
	jl	fn00000236
	call	fn00001F40
	mov	d4,#0x0
	sub.a	a8,#0x8
	st.w	[a10],d4
	st.w	[a10]4,d4
	mov.aa	a4,a10
	call	fn000012F6
	mov	d4,d2
	lea	a10,[a10]8
	mov.u	d1,#0x900D
	mov	d15,d2
	movh	d3,#0xFFFF
	or	d2,d3
	cmov	d1,d15,d2
	mov.a	a14,d1
	j	00001F0E
000000EA                               00 A0                       ..    

;; fn000000EC: 000000EC
;;   Called from:
;;     0000001C (in fn00000000)
fn000000EC proc
	ld.w	d0,[0003C5F0]
	ld.w	d1,[0003C5F4]
	movh	d2,#0x0
	addi	d2,d2,#0xFF01
	and	d0,d2
	or	d0,d0,#0xF0
	and	d4,d1,#0xC
	or	d0,d4
	st.a	[0003C5F0],a0
	movh	d4,#0x0
	addi	d4,d4,#0xFFF0
	and	d0,d4
	or	d0,d0,#0x2
	isync
	st.a	[0003C5F0],a0
	ld.w	d0,[0003C5F0]
	ji	a11

;; fn00000128: 00000128
;;   Called from:
;;     000000AC (in fn000000A8)
fn00000128 proc
	movh	d2,#0x0
	addi	d2,d2,#0xFF03
	movh	d5,#0x0
	addi	d5,d5,#0xFFF0
	mov	d6,#0x8
	st.a	[0003C5F4],a6
	ld.w	d0,[0003C5F0]
	ld.w	d1,[0003C5F4]
	and	d3,d2,d0
	or	d3,d3,#0xF0
	and	d4,d1,#0xC
	or	d3,d4
	xor	d3,d3,#0x2
	st.a	[0003C5F0],a3
	and	d3,d5
	or	d3,d3,#0x3
	st.a	[0003C5F0],a3
	ld.w	d0,[0003C5F0]
	ji	a11

;; fn0000016E: 0000016E
;;   Called from:
;;     000000B0 (in fn000000A8)
fn0000016E proc
	movh	d0,#0x0
	mtcr	PCXI,d0
	isync
	movh	d0,#0xD000
	addi	d0,d0,#0x1380
	addi	d0,d0,#0x3F
	andn	d0,d0,#0x3F
	movh	d2,#0xD000
	addi	d2,d2,#0x5380
	andn	d2,d2,#0x3F
	sub	d2,d0
	sh	d2,#0xFFFFFFFA
	mov.a	a3,d0
	extr	d0,d0,#0x1C,#0x4
	sh	d0,d0,#0x10
	mov.a	a4,#0x0
	st.a	[a3],a4
	mov.aa	a4,a3
	lea	a3,[a3]64
	mov.d	d1,a3
	extr	d1,d1,#0x6,#0x10
	or	d1,d0
	mtcr	LCX,d1
	add	d2,#0xFFFFFFFE
	mov.a	a5,d2

l000001BE:
	mov.d	d1,a4
	extr	d1,d1,#0x6,#0x10
	or	d1,d0
	st.w	[a3],d1
	mov.aa	a4,a3
	lea	a3,[a3]64
	loop	d5,000001BE

l000001D0:
	mov.d	d1,a4
	extr	d1,d1,#0x6,#0x10
	or	d1,d0
	mtcr	FCX,d1
	isync
	ji	a11

;; fn000001E2: 000001E2
;;   Called from:
;;     000000B4 (in fn000000A8)
fn000001E2 proc
	mov	d14,#0x0
	mov	d15,#0x0
	movh.a	a13,#0xC000
	lea	a13,[a13]8064

l000001EE:
	ld.a	a15,[a13+]
	ld.w	d3,[a13+]
	jeq	d3,#0xFFFFFFFF,00000234

l000001F6:
	sh	d0,d3,#0x3D
	and	d1,d3,#0x7
	jz	d0,0000020C

l00000200:
	addi	d0,d0,#0xFFFF
	mov.a	a2,d0

l00000206:
	st.b	[a15+]8,d14
	loop	d2,00000206

l0000020C:
	jeq	d1,#0x0,000001EE

l00000210:
	sh	d0,d1,#0x3E
	and	d1,d1,#0x3
	jz	d0,0000021C

l0000021A:
	st.w	[a15+],d15

l0000021C:
	jeq	d1,#0x0,000001EE

l00000220:
	sh	d0,d1,#0x3F
	and	d1,d1,#0x1
	jz	d0,0000022C

l0000022A:
	st.h	[a15+],d15

l0000022C:
	jeq	d1,#0x0,000001EE

l00000230:
	st.b	[a15],d15
	j	000001EE

l00000234:
	ji	a11

;; fn00000236: 00000236
;;   Called from:
;;     000000B8 (in fn000000A8)
fn00000236 proc
	movh.a	a13,#0xC000
	lea	a13,[a13]8104

l0000023E:
	ld.a	a15,[a13+]
	ld.a	a14,[a13+]
	ld.w	d3,[a13+]
	jeq	d3,#0xFFFFFFFF,00000292

l00000248:
	sh	d0,d3,#0x3D
	and	d1,d3,#0x7
	jz	d0,00000262

l00000252:
	addi	d0,d0,#0xFFFF
	mov.a	a2,d0

l00000258:
	ld.d	e14,[a15+]8
	st.b	[a14+]8,d14
	loop	d2,00000258

l00000262:
	jeq	d1,#0x0,0000023E

l00000266:
	sh	d0,d1,#0x3E
	and	d1,d1,#0x3
	jz	d0,00000274

l00000270:
	ld.w	d14,[a15+]
	st.w	[a14+],d14

l00000274:
	jeq	d1,#0x0,0000023E

l00000278:
	sh	d0,d1,#0x3F
	and	d1,d1,#0x1
	jz	d0,00000286

l00000282:
	ld.h	d14,[a15+]
	st.h	[a14+],d14

l00000286:
	jeq	d1,#0x0,0000023E

l0000028A:
	ld.b	d14,[a15]
	st.b	[a14],d14
	j	0000023E

l00000292:
	ji	a11

;; fn00000294: 00000294
;;   Called from:
;;     000000A8 (in fn000000A8)
fn00000294 proc
	movh.a	a15,#0xC000
	lea	a15,[a15]660
	movh.a	a14,#0xC000
	lea	a14,[a14]660
	jeq.a	a14,a15,000002BE

l000002A8:
	ld.a	a15,[a15]
	jz.a	a15,000002BE

l000002AC:
	add.a	a15,#0xFFFFFFFF

l000002AE:
	ld.a	a2,[a14+]
	ld.w	d2,[a14+]
	st.w	[a2],d2
	loop	d15,000002AE

l000002B6:
	isync
	nop
	nop

l000002BE:
	ji	a11
000002C0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000300 00 A0 BB 00 00 E0 42 FE 60 EE 11 DE EA ED 1D 00 ......B.`.......
00000310 00 0E 3C 00 00 00 00 80 00 00 00 00 00 00 00 00 ..<.............
00000320 00 A0 BB 00 10 E0 42 FE 60 EE 11 DE EA ED 1D 00 ......B.`.......
00000330 F0 0D 3C 00 00 00 00 80 00 00 00 00 00 00 00 00 ..<.............
00000340 00 A0 BB 00 20 E0 42 FE 60 EE 11 DE EA ED 1D 00 .... .B.`.......
00000350 E0 0D 3C 00 00 00 00 80 00 00 00 00 00 00 00 00 ..<.............
00000360 00 A0 BB 00 30 E0 42 FE 60 EE 11 DE EA ED 1D 00 ....0.B.`.......
00000370 D0 0D 3C 00 00 00 00 80 00 00 00 00 00 00 00 00 ..<.............
00000380 00 A0 BB 00 40 E0 42 FE 60 EE 11 DE EA ED 1D 00 ....@.B.`.......
00000390 C0 0D 3C 00 00 00 00 80 00 00 00 00 00 00 00 00 ..<.............
000003A0 00 A0 BB 00 50 E0 42 FE 60 EE 11 DE EA ED 1D 00 ....P.B.`.......
000003B0 B0 0D 3C 00 00 00 00 80 00 00 00 00 00 00 00 00 ..<.............
000003C0 00 A0 BB 00 60 E0 42 FE 60 EE 11 DE EA ED 1D 00 ....`.B.`.......
000003D0 A0 0D 3C 00 00 00 00 80 00 00 00 00 00 00 00 00 ..<.............
000003E0 00 A0 BB 00 70 E0 42 FE 60 EE 11 DE EA ED 1D 00 ....p.B.`.......
000003F0 90 0D 3C 00 00 00 00 80 00 00 00 00 00 00 00 00 ..<.............

;; fn00000400: 00000400
;;   Called from:
;;     00000CC4 (in fn00000B04)
fn00000400 proc
	ld.w	d4,[0003C5F0]
	sub.a	a8,#0x8
	andn	d4,d4,#0x1
	call	fn00000AC0

l0000040E:
	ld.w	d15,[0003C5F0]
	jnz.t	d15,#0x0,0000040E

l00000416:
	mov	d15,#0x8
	movh.a	a15,#0xF000
	st.w	[a15]16384,d15
	ld.w	d15,[a15]16384
	st.w	[a10]4,d15
	lea	a15,[a15]16384

l0000042A:
	ld.w	d15,[a15]
	jnz.t	d15,#0x1,0000042A

l00000430:
	mov	d15,#0x43FF
	movh.a	a15,#0xF000
	st.w	[a15]16396,d15
	ld.w	d15,[a15]16396
	st.w	[a10]4,d15
	ld.w	d4,[0003C5F0]
	or	d4,d4,#0x1
	call	fn00000AC0
	movh.a	a3,#0xF000
	lea	a3,[a3]16836
	mov.aa	a2,a3

l00000458:
	ld.w	d15,[a3]
	mov.aa	a15,a2
	extr	d15,d15,#0x8,#0x1
	jnz	d15,00000458

l00000462:
	mov	d2,#0x41
	movh	d3,#0x60
	movh.a	a2,#0xF000
	st.w	[a2]16896,d2
	movh.a	a3,#0xF000
	st.w	[a3]16904,d15
	movh.a	a3,#0xF000
	st.w	[a3]16916,d3
	movh.a	a3,#0xF000
	st.w	[a3]16908,d15
	ld.w	d3,[0003D218]
	movh.a	a3,#0xF000
	insert	d3,d3,#0x2,#0x4,#0x4
	st.a	[0003D218],a3
	ld.w	d3,[0003D218]
	insert	d3,d3,#0x9,#0xC,#0x4
	st.a	[0003D218],a3
	mov	d3,#0x1653
	st.w	[a3]16912,d3
	movh.a	a3,#0xF000
	st.w	[a3]16920,d15
	movh.a	a3,#0xF000
	movh	d15,#0x100
	st.w	[a3]17152,d2
	add	d15,#0x2
	movh.a	a3,#0xF000
	st.w	[a3]17408,d2
	movh.a	a3,#0xF000
	st.w	[a3]17664,d2
	st.w	[a15],d15
	mov.aa	a3,a15

l000004D8:
	ld.w	d15,[a15]
	jnz.t	d15,#0x8,000004D8

l000004DE:
	movh	d15,#0x101
	add	d15,#0x2
	st.w	[a3],d15

l000004E6:
	ld.w	d15,[a15]
	extr	d15,d15,#0x8,#0x1
	jnz	d15,000004E6

l000004EE:
	movh	d2,#0xA0
	movh.a	a15,#0xF000
	movh	d3,#0x800
	st.w	[a15]20508,d2
	mov	d2,#0xFFFFFFFF
	movh.a	a3,#0xF000
	sh	d2,#0xFFFFFFFD
	st.w	[a3]20480,d3
	movh.a	a3,#0xF000
	st.w	[a3]20484,d15
	movh.a	a3,#0xF000
	st.w	[a3]20492,d2
	movh	d2,#0xCC00
	movh.a	a3,#0xF000
	movh	d4,#0xEA8
	st.w	[a3]20504,d2
	movh	d2,#0x20
	movh.a	a3,#0xF000
	st.w	[a3]20488,d15
	st.w	[a15]20508,d2
	movh.a	a15,#0xF000
	st.w	[a15]20540,d4
	movh.a	a3,#0xF000
	st.w	[a3]20512,d3
	mov	d3,#0xFFFFFFFF
	sh	d3,#0xFFFFFFFE
	movh.a	a3,#0xF000
	st.w	[a3]20516,d15
	movh.a	a3,#0xF000
	st.w	[a3]20524,d3
	movh	d3,#0xD000
	movh.a	a3,#0xF000
	movh.a	a6,#0xD000
	st.w	[a3]20536,d3
	movh.a	a3,#0xF000
	st.w	[a3]20528,d15
	movh.a	a3,#0xF000
	st.w	[a3]20532,d15
	mov	d15,#0x100
	movh.a	a3,#0xF000
	movh.a	a5,#0xD000
	st.w	[a3]20520,d15
	st.w	[a15]20540,d2
	lea	a6,[a6]152
	mov	d2,#0x0
	lea	a5,[a5]24
	lha	a15,[0000007E]

l000005A0:
	addi	d15,d2,#0x280
	addih	d15,d15,#0x780
	sh	d15,#0x5
	mov.a	a3,d15
	addsc.a	a4,a6,d2,#0x0
	ld.w	d15,[a3]4
	st.b	[a4],d15
	ld.w	d15,[a3]4
	addsc.a	a3,a5,d2,#0x0
	add	d2,#0x1
	st.b	[a3],d15
	loop	d15,000005A0

l000005C0:
	ld.w	d15,[a2]16896
	andn	d15,d15,#0x41
	st.w	[a2]16896,d15
	ret

;; fn000005CE: 000005CE
;;   Called from:
;;     000010D0 (in fn000010B0)
;;     0000131A (in fn000012F6)
fn000005CE proc
	sh	d15,d4,#0x5
	mov.a	a2,d15
	mov	d15,#0x0
	lea	a15,[a2]20480
	addih.a	a15,a15,#0xF000
	ld.w	d2,[a15]
	extr	d2,d2,#0x18,#0x4
	st.h	[a4],d2

l000005E6:
	and	d3,d15,#0xFF
	add	d4,d15,#0x1
	jge.u	d3,d2,00000602

l000005F0:
	and	d15,#0xFF
	addsc.a	a3,a15,d15,#0x0
	addsc.a	a2,a4,d15,#0x0
	ld.bu	d3,[a3]16
	st.b	[a2]12,d3
	mov	d15,d4
	j	000005E6

l00000602:
	sh	d2,#0x4
	ld.w	d15,[a15]28
	st.h	[a4],d2
	jz.t	d15,#0xB,00000610

l0000060A:
	or	d2,d2,#0x8
	st.h	[a4],d2

l00000610:
	ld.w	d15,[a15]24
	Invalid
	jltz	d0,00000614
	ld.w	d15,[a15]24
	insert	d15,d15,#0x0,#0x1D,#0x3
	st.w	[a4]4,d15
	ld.w	d15,[a15]12
	insert	d15,d15,#0x0,#0x1D,#0x3
	st.w	[a4]8,d15
	ld.h	d15,[a4]
	or	d15,#0x4
	st.h	[a4],d15
	j	0000063E
	ld.w	d15,[a15]24
	extr	d15,d15,#0x12,#0xB
	st.w	[a4]4,d15
	ld.w	d15,[a15]12
	extr	d15,d15,#0x12,#0xB
	st.w	[a4]8,d15
	ld.w	d15,[a15]8
	sh	d15,d15,#0x30
	st.h	[a4]20,d15
	ret

;; fn00000648: 00000648
;;   Called from:
;;     00000CD2 (in fn00000CCE)
fn00000648 proc
	sh	d15,d4,#0x5
	mov.a	a2,d15
	mov	d2,#0x0
	lea	a15,[a2]20480
	addih.a	a15,a15,#0xF000
	ld.w	d15,[a15]28
	jnz.t	d15,#0x8,00000662

l0000065C:
	mov	d15,#0x20
	mov	d2,#0x1
	st.w	[a15]28,d15

l00000662:
	ret

;; fn00000664: 00000664
;;   Called from:
;;     000010C6 (in fn000010B0)
;;     0000130E (in fn000012F6)
fn00000664 proc
	sh	d15,d4,#0x5
	mov.a	a2,d15
	lea	a15,[a2]20480
	addih.a	a15,a15,#0xF000
	ld.w	d2,[a15]28
	extr	d2,d2,#0x3,#0x1
	ret

l0000067A:
	sh	d15,d4,#0x5
	mov.a	a2,d15
	movh	d15,#0x700
	lea	a15,[a2]20480
	addih.a	a15,a15,#0xF000
	st.w	[a15]28,d15
	ret

;; fn00000690: 00000690
fn00000690 proc
	sh	d15,d4,#0x5
	mov.a	a2,d15
	mov	d15,#0x20
	lea	a15,[a2]20480
	addih.a	a15,a15,#0xF000
	st.w	[a15]28,d15
	ld.w	d15,[a15]24
	ld.hu	d2,[a4]
	insert	d15,d15,#0x0,#0x0,#0x1E
	ld.w	d4,[a4]4
	ld.w	d3,[a4]8
	st.w	[a15]24,d15
	ld.w	d15,[a15]24
	jz.t	d2,#0x2,000006D0

l000006BC:
	or	d4,d15
	insert	d4,d4,#0xF,#0x1D,#0x1
	st.w	[a15]24,d4
	ld.w	d15,[a15]12
	insert	d15,d15,#0x0,#0x0,#0x1D
	st.w	[a15]12,d15
	ld.w	d15,[a15]12
	j	000006E6

l000006D0:
	sh	d4,d4,#0x12
	or	d4,d15
	sh	d3,d3,#0x12
	st.w	[a15]24,d4
	ld.w	d15,[a15]12
	insert	d15,d15,#0x0,#0x0,#0x1D
	st.w	[a15]12,d15
	ld.w	d15,[a15]12

l000006E6:
	or	d3,d15
	and	d2,d2,#0xF0
	sh	d2,d2,#0x14
	st.w	[a15]12,d3
	ld.w	d15,[a15]8
	insert	d15,d15,#0x0,#0x10,#0x10
	st.w	[a15]8,d15
	ld.hu	d15,[a4]20
	ld.w	d3,[a15]8
	sh	d15,d15,#0x10
	or	d15,d3
	st.w	[a15]8,d15
	ld.w	d15,[a15]
	insert	d15,d15,#0x0,#0x18,#0x4
	or	d15,d2
	st.w	[a15],d15
	ld.w	d15,[a15]28
	jz.t	d15,#0xB,00000742

l00000718:
	mov	d4,#0x0

l0000071A:
	ld.h	d3,[a4]
	mov	d15,d4
	extr	d3,d3,#0x4,#0x4
	and	d15,#0xFF
	mov	d2,d15
	add	d4,#0x1
	jge	d15,d3,0000073C

l0000072C:
	addsc.a	a2,a4,d2,#0x0
	ld.bu	d15,[a2]12
	addsc.a	a2,a15,d2,#0x0
	st.b	[a2]16,d15
	j	0000071A

l0000073C:
	movh	d15,#0x628
	j	00000746

l00000742:
	movh	d15,#0x20

l00000746:
	addi	d15,d15,#0x40
	st.w	[a15]28,d15
	ret

;; fn0000074E: 0000074E
;;   Called from:
;;     00000CDE (in fn00000CCE)
fn0000074E proc
	sh	d15,d4,#0x5
	mov.a	a2,d15
	movh	d15,#0x8
	lea	a15,[a2]20480
	addih.a	a15,a15,#0xF000
	st.w	[a15]28,d15
	mov.aa	a2,a4

l00000764:
	sub.a	a3,a2,a4
	mov.d	d15,a3
	and	d2,d15,#0xFF
	ld.w	d15,[a15]
	extr	d15,d15,#0x18,#0x4
	jge.u	d2,d15,00000786

l00000778:
	addsc.a	a3,a15,d2,#0x0
	ld.bu	d15,[a2]
	st.b	[a3]16,d15
	add.a	a2,#0x1
	j	00000764

l00000786:
	movh	d15,#0x20
	addi	d15,d15,#0x40
	st.w	[a15]28,d15
	ret

;; fn00000792: 00000792
fn00000792 proc
	sh	d15,d4,#0x5
	mov.a	a2,d15
	mov	d2,#0x0
	lea	a15,[a2]20480
	addih.a	a15,a15,#0xF000
	ld.w	d15,[a15]28
	jz.t	d15,#0x4,000007AC

l000007A6:
	mov	d15,#0x10
	mov	d2,#0x1
	st.w	[a15]28,d15

l000007AC:
	ret

;; fn000007AE: 000007AE
fn000007AE proc
	sh	d15,d4,#0x5
	mov.a	a2,d15
	mov	d2,#0x0
	lea	a15,[a2]20480
	addih.a	a15,a15,#0xF000
	ld.w	d15,[a15]28
	and	d15,d15,#0x108
	jnz	d15,000007CC

l000007C6:
	mov	d15,#0x20
	mov	d2,#0x1
	st.w	[a15]28,d15

l000007CC:
	ret

;; fn000007CE: 000007CE
;;   Called from:
;;     000010D6 (in fn000010B0)
;;     00001320 (in fn000012F6)
fn000007CE proc
	sh	d15,d4,#0x5
	mov.a	a2,d15
	mov	d15,#0x8
	lea	a15,[a2]20480
	addih.a	a15,a15,#0xF000
	st.w	[a15]28,d15
	ret

;; fn000007E2: 000007E2
fn000007E2 proc
	sh	d15,d4,#0x5
	mov.a	a2,d15
	movh	d15,#0x20
	lea	a15,[a2]20480
	addih.a	a15,a15,#0xF000
	st.w	[a15]28,d15
	ret

;; fn000007F8: 000007F8
fn000007F8 proc
	sh	d15,d4,#0x5
	mov.a	a5,d15
	mov	d2,#0x2
	lea	a2,[a5]20480
	addih.a	a2,a2,#0xF000
	ld.w	d15,[a2]
	and	d15,#0xF
	jne	d15,#0x2,000008BC

l00000810:
	movh.a	a3,#0xD000
	lea	a3,[a3]152
	addsc.a	a3,a3,d4,#0x0
	ld.bu	d15,[a3]
	sh	d2,d15,#0x5
	mov.a	a5,d2
	mov	d2,#0x0
	lea	a15,[a5]20480
	addih.a	a15,a15,#0xF000
	ld.w	d3,[a15]28
	jnz.t	d3,#0x8,000008BC

l00000834:
	ld.w	d2,[a2]4
	extr	d2,d2,#0x8,#0x8
	jne	d15,d3,00000842

l0000083E:
	ld.w	d15,[a2]4
	j	00000848

l00000842:
	ld.w	d15,[a15]28
	sh	d15,d15,#0x28

l00000848:
	st.b	[a3],d15
	mov	d15,#0x8
	ld.w	d2,[a4]4
	st.w	[a15]28,d15
	ld.w	d15,[a15]24
	Invalid
	Invalid
	insert	d15,d15,#0x0,#0x0,#0x1E
	st.w	[a15]24,d15
	ld.w	d15,[a15]24
	or	d2,d15
	insert	d2,d2,#0xF,#0x1D,#0x1
	j	0000087A
	ld.w	d15,[a15]24
	sh	d2,d2,#0x12
	insert	d15,d15,#0x0,#0x0,#0x1E
	st.w	[a15]24,d15
	ld.w	d15,[a15]24
	or	d2,d15
	st.w	[a15]24,d2
	ld.w	d15,[a15]
	ld.h	d2,[a4]
	insert	d15,d15,#0x0,#0x18,#0x4
	and	d2,d2,#0xF0
	sh	d2,d2,#0x14
	or	d15,d2
	mov	d4,#0x0
	st.w	[a15],d15
	ld.h	d3,[a4]
	mov	d15,d4
	extr	d3,d3,#0x4,#0x4
	and	d15,#0xFF
	mov	d2,d15
	add	d4,#0x1
	jge	d15,d3,000008B4
	addsc.a	a2,a4,d2,#0x0
	ld.bu	d15,[a2]12
	addsc.a	a2,a15,d2,#0x0
	st.b	[a2]16,d15
	j	00000892
	movh	d15,#0x128
	mov	d2,#0x1
	st.w	[a15]28,d15

l000008BC:
	ret

;; fn000008BE: 000008BE
fn000008BE proc
	sh	d15,d4,#0x5
	mov.a	a5,d15
	mov	d2,#0x2
	lea	a2,[a5]20480
	addih.a	a2,a2,#0xF000
	ld.w	d15,[a2]
	and	d15,#0xF
	jne	d15,#0x1,00000988

l000008D6:
	movh.a	a3,#0xD000
	lea	a3,[a3]24
	addsc.a	a3,a3,d4,#0x0
	ld.bu	d15,[a3]
	sh	d2,d15,#0x5
	mov.a	a5,d2
	mov	d2,#0x0
	lea	a15,[a5]20480
	addih.a	a15,a15,#0xF000
	ld.w	d3,[a15]28
	jz.t	d3,#0x3,00000988

l000008FA:
	mov	d2,#0x8
	st.w	[a15]28,d2
	ld.w	d2,[a2]4
	extr	d2,d2,#0x8,#0x8
	jne	d15,d3,0000090E

l0000090A:
	ld.w	d15,[a2]4
	j	00000914

l0000090E:
	ld.w	d15,[a15]28
	sh	d15,d15,#0x28

l00000914:
	st.b	[a3],d15
	ld.w	d15,[a15]28
	jz.t	d15,#0x4,00000922

l0000091A:
	mov	d15,#0x10
	mov	d2,#0x3
	st.w	[a15]28,d15
	ret

l00000922:
	ld.w	d2,[a15]
	mov	d15,#0x0
	extr	d2,d2,#0x18,#0x4
	st.h	[a4],d2

l0000092C:
	and	d3,d15,#0xFF
	add	d4,d15,#0x1
	jge.u	d3,d2,00000948

l00000936:
	and	d15,#0xFF
	addsc.a	a3,a15,d15,#0x0
	addsc.a	a2,a4,d15,#0x0
	ld.bu	d3,[a3]16
	st.b	[a2]12,d3
	mov	d15,d4
	j	0000092C

l00000948:
	sh	d2,#0x4
	ld.w	d15,[a15]28
	st.h	[a4],d2
	jz.t	d15,#0xB,00000956

l00000950:
	or	d2,d2,#0x8
	st.h	[a4],d2

l00000956:
	ld.w	d15,[a15]24
	Invalid
	Invalid
	ld.w	d15,[a15]24
	insert	d15,d15,#0x0,#0x1D,#0x3
	st.w	[a4]4,d15
	ld.h	d15,[a4]
	or	d15,#0x4
	st.h	[a4],d15
	j	00000974
	ld.w	d15,[a15]24
	extr	d15,d15,#0x12,#0xB
	st.w	[a4]4,d15
	ld.w	d15,[a15]8
	mov	d2,#0x1
	sh	d15,d15,#0x30
	st.h	[a4]20,d15
	ld.w	d15,[a15]28
	jz.t	d15,#0x3,00000988
	mov	d15,#0x8
	mov	d2,#0x4
	st.w	[a15]28,d15

l00000988:
	ret

;; fn0000098A: 0000098A
;;   Called from:
;;     00000EF6 (in fn00000EB6)
fn0000098A proc
	mov	d15,#0xF5
	mov.a	a15,d5
	st.w	[a15]21844,d15
	mov	d15,#0xAA
	lea	a2,[a15]-21848
	addih.a	a2,a2,#0x1
	st.w	[a15]21844,d15
	mov	d15,#0x55
	st.w	[a2],d15
	ld.w	d15,[a10]
	st.w	[a15]21820,d15
	st.w	[a2],d6
	st.w	[a2],d7
	st.w	[a15]21848,d4
	nop
	nop
	nop
	ret

;; fn000009BA: 000009BA
;;   Called from:
;;     00000F8C (in fn00000F40)
fn000009BA proc
	mov	d15,#0xF5
	mov.a	a15,d4
	mov	d2,#0xAA
	st.w	[a15]21844,d15
	mov	d15,#0x55
	lea	a2,[a15]-21848
	mov	d3,#0x80
	st.w	[a15]21844,d2
	addih.a	a2,a2,#0x1
	st.w	[a2],d15
	st.w	[a15]21844,d3
	st.w	[a15]21844,d2
	st.w	[a2],d15
	mov	d15,#0x30
	mov.a	a15,d5
	st.w	[a15],d15

l000009EA:
	ld.w	d15,[a4]
	jnz.t	d15,#0x0,000009F8

l000009EE:
	ld.w	d15,[a4]
	jnz.t	d15,#0x2,000009F8

l000009F2:
	ld.w	d15,[a4]
	jz.t	d15,#0x3,000009EA

l000009F8:
	ld.w	d15,[a4]
	jnz.t	d15,#0x0,000009F8

l000009FE:
	ld.w	d15,[a4]
	jnz.t	d15,#0x2,000009F8

l00000A04:
	ld.w	d15,[a4]
	jnz.t	d15,#0x3,000009F8

l00000A0A:
	ret

;; fn00000A0C: 00000A0C
;;   Called from:
;;     0000108E (in fn0000101C)
fn00000A0C proc
	mov	d15,#0xF5
	mov.a	a15,d4
	mov.a	a6,d5
	st.w	[a15]21844,d15
	mov	d15,#0x50
	lea	a3,[a15]22000
	lha	a2,[0000001F]
	st.w	[a15]21844,d15
	ld.bu	d15,[a5]2
	ld.bu	d2,[a5]1
	sh	d7,d15,#0x30
	sh	d6,d15,#0x10
	madd.u	d2,d6,d2,#0xFF00
	ld.bu	d5,[a5]
	movh	d15,#0x100
	madd.u	d4,d2,d5,#0x1
	ld.bu	d2,[a5]3
	Invalid
	ld.bu	d4,[a5]4
	mov	d15,d3
	add	d15,d4
	ld.bu	d4,[a5]5
	mov	d3,#0x100
	Invalid
	ld.bu	d4,[a5]6
	movh	d3,#0x1
	Invalid
	ld.bu	d4,[a5]7
	movh	d3,#0x100
	Invalid
	st.w	[a3],d2
	lea	a5,[a5]8
	st.w	[a3]4,d15
	loop	a2,00000A24
	mov	d15,#0xAA
	mov	d2,#0x55
	lea	a2,[a15]-21848
	st.w	[a15]21844,d15
	addih.a	a2,a2,#0x1
	st.w	[a2],d2
	mov	d2,#0xA0
	st.w	[a15]21844,d2
	st.w	[a6],d15
	ld.w	d15,[a4]
	jnz.t	d15,#0x0,00000AAC
	ld.w	d15,[a4]
	jnz.t	d15,#0x2,00000AAC
	ld.w	d15,[a4]
	jz.t	d15,#0x3,00000A9E
	ld.w	d15,[a4]
	jnz.t	d15,#0x0,00000AAC
	ld.w	d15,[a4]
	jnz.t	d15,#0x2,00000AAC
	ld.w	d15,[a4]
	jnz.t	d15,#0x3,00000AAC
	ret

;; fn00000AC0: 00000AC0
;;   Called from:
;;     0000040A (in fn00000400)
;;     0000044A (in fn00000400)
;;     00000B36 (in fn00000B04)
;;     00000B62 (in fn00000B04)
;;     00000B76 (in fn00000B04)
;;     00000BBA (in fn00000B04)
;;     00000BCE (in fn00000B04)
;;     00000BEE (in fn00000B04)
;;     00000BFA (in fn00000B04)
;;     00000C16 (in fn00000B04)
;;     00000C2A (in fn00000B04)
;;     00000C56 (in fn00000B04)
;;     00000C9A (in fn00000B04)
;;     00000CC0 (in fn00000B04)
fn00000AC0 proc
	ld.w	d2,[0003C5F0]
	ld.w	d3,[0003C5F4]
	andn	d15,d2,#0x8
	extr	d3,d3,#0x3,#0x1
	or	d2,d2,#0xF8
	or	d15,#0xF0
	seln	d15,d3,d15,d2
	ld.w	d3,[0003C5F4]
	or	d2,d15,#0x4
	extr	d3,d3,#0x2,#0x1
	andn	d15,d15,#0x4
	seln	d15,d3,d15,d2
	andn	d15,d15,#0x2
	andn	d4,d4,#0xC
	or	d4,d4,#0xF2
	st.a	[0003C5F0],a15
	st.a	[0003C5F0],a4
	ret

;; fn00000B04: 00000B04
;;   Called from:
;;     00001300 (in fn000012F6)
fn00000B04 proc
	ld.w	d2,[0003C518]
	movh	d15,#0x100
	addi	d15,d15,#0x4E00
	and	d2,d15
	jne	d2,d15,00000B2E

l00000B16:
	ld.w	d2,[0003C51C]
	movh	d15,#0x2
	add	d15,#0x1
	and	d2,d15
	jne	d2,d15,00000B2E

l00000B26:
	ld.w	d15,[0003C530]
	jnz.t	d15,#0x0,00000BF2

l00000B2E:
	ld.w	d4,[0003C5F0]
	andn	d4,d4,#0x1
	call	fn00000AC0

l00000B3A:
	ld.w	d15,[0003C5F0]
	jnz.t	d15,#0x0,00000B3A

l00000B42:
	ld.w	d15,[0003C518]
	andn	d15,d15,#0x1
	st.a	[0003C518],a15
	ld.w	d15,[0003C518]
	insert	d15,d15,#0x1,#0x4,#0x1
	st.a	[0003C518],a15
	ld.w	d4,[0003C5F0]
	or	d4,d4,#0x1
	call	fn00000AC0
	ld.w	d15,[0003C514]
	jnz.t	d15,#0x1,00000BF2

l00000B6E:
	ld.w	d4,[0003C5F0]
	andn	d4,d4,#0x1
	call	fn00000AC0

l00000B7A:
	ld.w	d15,[0003C5F0]
	jnz.t	d15,#0x0,00000B7A

l00000B82:
	mov	d15,#0x1
	st.a	[0003C530],a15
	ld.w	d15,[0003C518]
	insert	d15,d15,#0x1,#0x0,#0x1
	st.a	[0003C518],a15

l00000B94:
	ld.w	d15,[0003C514]
	jz.t	d15,#0x0,00000B94

l00000B9C:
	movh	d15,#0x105
	addi	d15,d15,#0x4E21
	st.a	[0003C518],a15
	movh	d15,#0x2
	add	d15,#0x1
	st.a	[0003C51C],a15
	ld.w	d4,[0003C5F0]
	or	d4,d4,#0x1
	call	fn00000AC0

l00000BBE:
	ld.w	d15,[0003C514]
	jz.t	d15,#0x2,00000BBE

l00000BC6:
	ld.w	d4,[0003C5F0]
	andn	d4,d4,#0x1
	call	fn00000AC0

l00000BD2:
	ld.w	d15,[0003C5F0]
	jnz.t	d15,#0x0,00000BD2

l00000BDA:
	ld.w	d15,[0003C518]
	andn	d15,d15,#0x1
	st.a	[0003C518],a15
	ld.w	d4,[0003C5F0]
	or	d4,d4,#0x1
	call	fn00000AC0

l00000BF2:
	ld.w	d4,[0003C5F0]
	andn	d4,d4,#0x1
	call	fn00000AC0

l00000BFE:
	ld.w	d15,[0003C5F0]
	jnz.t	d15,#0x0,00000BFE

l00000C06:
	mov	d15,#0x8
	st.a	[0003C5F4],a15
	ld.w	d4,[0003C5F0]
	mov	d15,#0x0
	or	d4,d4,#0x1
	call	fn00000AC0
	mtcr	ICR,d15
	isync
	ld.w	d4,[0003C5F0]
	andn	d4,d4,#0x1
	call	fn00000AC0

l00000C2E:
	ld.w	d15,[0003C5F0]
	and	d15,#0x1
	jnz	d15,00000C2E

l00000C36:
	mov	d2,#0x200
	movh.a	a15,#0xF004
	st.w	[a15]16128,d15
	movh.a	a15,#0xF004
	st.w	[a15]16144,d2
	ld.w	d4,[0003C5F0]
	movh.a	a15,#0xF004
	or	d4,d4,#0x1
	call	fn00000AC0
	st.w	[a15]16160,d15
	movh.a	a15,#0xF004
	st.w	[a15]16164,d15
	mov	d15,#0x1000
	movh.a	a15,#0xF004
	st.w	[a15]16364,d15
	movh.a	a15,#0xF004
	st.w	[a15]16360,d15
	movh.a	a15,#0xF004
	st.w	[a15]16356,d15
	movh.a	a15,#0xF004
	st.w	[a15]16352,d15
	movh.a	a15,#0xF004
	st.w	[a15]16348,d15
	ld.w	d4,[0003C5F0]
	andn	d4,d4,#0x1
	call	fn00000AC0

l00000C9E:
	ld.w	d15,[0003C5F0]
	jnz.t	d15,#0x0,00000C9E

l00000CA6:
	mov	d15,#0x8
	movh.a	a15,#0xD000
	st.a	[0003FC00],a15
	ld.w	d15,[0003FC00]
	st.w	[a15]576,d15
	ld.w	d4,[0003C5F0]
	or	d4,d4,#0x1
	call	fn00000AC0
	call	fn00000400
	disable
	ret

;; fn00000CCE: 00000CCE
;;   Called from:
;;     00000D0A (in fn00000CE8)
;;     00000F3C (in fn00000EB6)
;;     00000FBA (in fn00000F40)
;;     00001018 (in fn00000FBE)
;;     000010A8 (in fn0000101C)
;;     000011AA (in fn000010F0)
;;     000011E6 (in fn000010F0)
fn00000CCE proc
	mov.aa	a15,a4

l00000CD0:
	mov	d4,#0x1
	call	fn00000648
	jne	d2,#0x1,00000CD0

l00000CDA:
	mov	d4,#0x1
	mov.aa	a4,a15
	call	fn0000074E
	mov	d4,#0x1
	j	0000067A

;; fn00000CE8: 00000CE8
fn00000CE8 proc
	mov	d15,#0x1
	sub.a	a8,#0x8
	lha	a15,[00034000]
	st.b	[a10],d15
	mov	d15,#0x0
	mov.a	a2,#0x5
	st.b	[a10]1,d15

l00000CF8:
	lha	a3,[0000C002]
	add.a	a3,a10
	add.a	a3,a15
	ld.bu	d15,[a15+]
	st.b	[a3],d15
	loop	d2,00000CF8

l00000D06:
	mov	d15,#0x1
	mov.aa	a4,a10
	call	fn00000CCE
	st.b	[a10]1,d15
	ld.bu	d15,[a15]
	st.b	[a10]2,d15
	ld.b	d15,[00034000]
	orn.t	d10,d4,#0xC,d0,#0x6
	ld.b	d15,[00034000]
	ld.bu	d4,[a15]
	st.b	[a10]4,d15
	ld.b	d15,[00034000]
	ld.w	d4,[+a0]-340
	ld.b	d15,[00034000]
	Invalid
	st.b	[a10]6,d15
	ld.b	d15,[00034000]
	Invalid
	st.b	[a10]7,d15
	j	fn00000CCE
	ld.bu	d2,[a4]13
	ld.bu	d15,[a4]14
	sh	d2,d2,#0x18
	sh	d15,d15,#0x10
	or	d2,d15
	ld.bu	d15,[a4]16
	sub.a	a8,#0x8
	or	d2,d15
	ld.bu	d15,[a4]15
	mov.aa	a4,a10
	sh	d15,d15,#0x8
	or	d15,d2
	mov.a	a15,d15
	mov	d2,#0x2
	ld.w	d15,[a15]
	st.b	[a10]1,d15
	st.b	[a10],d2
	sh	d2,d15,#0x28
	st.b	[a10]4,d2
	sh	d2,d15,#0x30
	st.b	[a10]3,d2
	sh	d2,d15,#0x38
	mov	d15,#0xFFFFFFFF
	st.b	[a10]2,d2
	st.b	[a10]5,d15
	st.b	[a10]6,d15
	st.b	[a10]7,d15
	j	fn00000CCE
	ld.bu	d3,[a4]13
	ld.bu	d15,[a4]14
	sh	d3,d3,#0x18
	sh	d15,d15,#0x10
	or	d15,d3
	ld.bu	d3,[a4]16
	sub.a	a8,#0x8
	or	d3,d15
	ld.bu	d15,[a4]15
	movh.a	a15,#0xD000
	sh	d2,d15,#0x8
	or	d15,d3,d2
	mov	d2,#0x3
	mov.aa	a4,a10
	lea	a15,[a15]564
	st.b	[a10],d2
	sh	d2,d15,#0x28
	st.b	[a10]1,d15
	st.b	[a10]4,d2
	sh	d2,d15,#0x30
	st.b	[a10]3,d2
	sh	d2,d15,#0x38
	st.b	[a10]2,d2
	mov	d2,#0xFFFFFFFF
	st.b	[a10]5,d2
	st.b	[a10]6,d2
	st.b	[a10]7,d2
	call	fn00000CCE
	st.w	[a15]4,d15
	ret
	ld.bu	d3,[a4]13
	ld.bu	d15,[a4]14
	sh	d3,d3,#0x18
	sh	d15,d15,#0x10
	or	d15,d3
	ld.bu	d3,[a4]16
	movh.a	a15,#0xD000
	or	d3,d15
	ld.bu	d15,[a4]15
	lea	a15,[a15]564
	sh	d2,d15,#0x8
	or	d15,d3,d2
	ld.a	a15,[a15]4
	mov	d2,#0x3
	sub.a	a8,#0x8
	st.w	[a15],d15
	st.b	[a10],d2
	sh	d2,d15,#0x28
	st.b	[a10]1,d15
	mov.aa	a4,a10
	st.b	[a10]4,d2
	sh	d2,d15,#0x30
	st.b	[a10]3,d2
	sh	d2,d15,#0x38
	mov	d15,#0xFFFFFFFF
	st.b	[a10]2,d2
	st.b	[a10]5,d15
	st.b	[a10]6,d15
	st.b	[a10]7,d15
	j	fn00000CCE
	ld.bu	d3,[a4]13
	ld.bu	d15,[a4]14
	sh	d3,d3,#0x18
	sh	d15,d15,#0x10
	or	d15,d3
	ld.bu	d3,[a4]16
	sub.a	a8,#0x8
	or	d3,d15
	ld.bu	d15,[a4]15
	ld.bu	d9,[a4]17
	sh	d2,d15,#0x8
	or	d15,d3,d2
	mov	d2,#0x4
	ld.bu	d10,[a4]18
	ld.bu	d8,[a4]19
	st.b	[a10],d2
	sh	d2,d15,#0x28
	mov.aa	a4,a10
	st.b	[a10]1,d15
	st.b	[a10]4,d2
	sh	d2,d15,#0x30
	st.b	[a10]3,d2
	sh	d2,d15,#0x38
	st.b	[a10]2,d2
	mov	d2,#0xFFFFFFFF
	st.b	[a10]5,d2
	st.b	[a10]6,d2
	st.b	[a10]7,d2
	call	fn00000CCE
	movh.a	a2,#0xD000
	lea	a15,[a2]556
	st.b	[a15]6,d10
	st.b	[a15]5,d9
	st.w	[a2]556,d15
	st.b	[a15]4,d8
	ret

;; fn00000EB6: 00000EB6
fn00000EB6 proc
	ld.bu	d7,[a4]13
	ld.bu	d15,[a4]14
	sh	d7,d7,#0x18
	sh	d15,d15,#0x10
	or	d7,d15
	ld.bu	d15,[a4]16
	movh.a	a2,#0xD000
	lea	a15,[a2]556
	or	d7,d15
	ld.bu	d15,[a4]15
	ld.bu	d5,[a15]4
	ld.bu	d2,[a15]6
	sub.a	a0,#0x10
	ld.bu	d4,[a15]5
	sh	d15,d15,#0x8
	st.w	[a10],d2
	movh	d3,#0x8000
	movh	d2,#0x8080
	ld.w	d6,[a2]556
	or	d7,d15
	sel	d5,d5,d2,d3
	call	fn0000098A
	ld.bu	d15,[a15]4
	movh.a	a15,#0xF800
	lea	a15,[a15]16400
	jnz	d15,00000F0E

l00000F06:
	movh.a	a15,#0xF800
	lea	a15,[a15]8208

l00000F0E:
	mov	d2,#0x4
	ld.w	d15,[a15]
	st.b	[a10]9,d15
	st.b	[a10]8,d2
	sh	d2,d15,#0x28
	lea	a4,[a10]8
	st.b	[a10]12,d2
	sh	d2,d15,#0x30
	st.b	[a10]11,d2
	sh	d2,d15,#0x38
	mov	d15,#0xFFFFFFFF
	st.b	[a10]10,d2
	st.b	[a10]13,d15
	st.b	[a10]14,d15
	st.b	[a10]15,d15
	j	fn00000CCE

;; fn00000F40: 00000F40
fn00000F40 proc
	ld.bu	d5,[a4]14
	ld.bu	d2,[a4]13
	sh	d15,d5,#0x10
	sh	d2,d2,#0x18
	or	d5,d2,d15
	ld.bu	d2,[a4]16
	movh.a	a15,#0xF800
	or	d2,d5
	ld.bu	d5,[a4]15
	sub.a	a8,#0x8
	sh	d15,d5,#0x8
	or	d5,d2,d15
	movh	d15,#0x8080
	add	d15,#0xFFFFFFFF
	movh	d4,#0x8000
	lea	a15,[a15]8208
	jge.u	d15,d5,00000F8A

l00000F7E:
	movh.a	a15,#0xF800
	movh	d4,#0x8080
	lea	a15,[a15]16400

l00000F8A:
	mov.aa	a4,a15
	call	fn000009BA
	mov	d2,#0x5
	ld.w	d15,[a15]
	st.b	[a10]1,d15
	st.b	[a10],d2
	sh	d2,d15,#0x28
	mov.aa	a4,a10
	st.b	[a10]4,d2
	sh	d2,d15,#0x30
	st.b	[a10]3,d2
	sh	d2,d15,#0x38
	mov	d15,#0xFFFFFFFF
	st.b	[a10]2,d2
	st.b	[a10]5,d15
	st.b	[a10]6,d15
	st.b	[a10]7,d15
	j	fn00000CCE

;; fn00000FBE: 00000FBE
fn00000FBE proc
	ld.bu	d3,[a4]13
	ld.bu	d15,[a4]14
	sh	d3,d3,#0x18
	sh	d15,d15,#0x10
	or	d15,d3
	ld.bu	d3,[a4]16
	movh.a	a2,#0xD000
	or	d3,d15
	ld.bu	d15,[a4]15
	lea	a15,[a2]292
	sh	d2,d15,#0x8
	or	d15,d3,d2
	mov	d2,#0x0
	sub.a	a8,#0x8
	st.w	[a2]292,d15
	st.w	[a15]4,d2
	mov	d2,#0x6
	st.b	[a10]1,d15
	mov.aa	a4,a10
	st.b	[a10],d2
	sh	d2,d15,#0x28
	st.b	[a10]4,d2
	sh	d2,d15,#0x30
	st.b	[a10]3,d2
	sh	d2,d15,#0x38
	mov	d15,#0xFFFFFFFF
	st.b	[a10]2,d2
	st.b	[a10]5,d15
	st.b	[a10]6,d15
	st.b	[a10]7,d15
	j	fn00000CCE

;; fn0000101C: 0000101C
fn0000101C proc
	movh.a	a2,#0xD000
	lea	a2,[a2]292
	ld.w	d15,[a2]4
	sub.a	a8,#0x8
	mov.a	a6,#0x1
	mov.aa	a3,a2
	mov.a	a15,#0x6

l0000102E:
	add.a	a5,a4,a6
	ld.bu	d2,[a5]12
	addsc.a	a5,a2,d15,#0x0
	st.b	[a5]8,d2
	ne	d2,d15,#0xFF
	jnz	d2,00001050

l00001042:
	st.w	[a3]4,d15
	ld.w	d15,[a2]4
	eq	d15,d15,#0xFF
	jnz	d15,0000105A

l0000104C:
	mov	d2,#0x0
	ret

l00001050:
	add	d15,#0x1
	add.a	a6,#0x1
	loop	a15,0000102E

l00001058:
	j	00001042

l0000105A:
	movh.a	a15,#0xD000
	movh	d15,#0x8080
	ld.w	d5,[a15]292
	movh.a	a4,#0xF800
	add	d15,#0xFFFFFFFF
	movh	d4,#0x8000
	lea	a4,[a4]8208
	jge.u	d15,d5,00001084

l00001078:
	movh.a	a4,#0xF800
	movh	d4,#0x8080
	lea	a4,[a4]16400

l00001084:
	mov	d15,#0x6
	movh.a	a5,#0xD000
	lea	a5,[a5]300
	call	fn00000A0C
	st.b	[a10],d15
	mov	d15,#0x0
	mov.aa	a4,a10
	st.b	[a10]4,d15
	st.b	[a10]3,d15
	st.b	[a10]2,d15
	st.b	[a10]1,d15
	mov	d15,#0xFFFFFFFF
	st.b	[a10]5,d15
	st.b	[a10]6,d15
	st.b	[a10]7,d15
	call	fn00000CCE
	mov	d2,#0x1
	ret

;; fn000010B0: 000010B0
fn000010B0 proc
	movh	d15,#0x2
	addi	d15,d15,#0x86A0
	mul	d15,d4
	ld.w	d9,[0003C210]
	sub.a	a8,#0x18
	mov.u	d8,#0xAC07

l000010C4:
	mov	d4,#0x0
	call	fn00000664
	jz	d2,000010E4

l000010CC:
	mov	d4,#0x0
	mov.aa	a4,a10
	call	fn000005CE
	mov	d4,#0x0
	call	fn000007CE
	ld.hu	d2,[a10]12
	jne	d2,d8,000010C4

l000010E2:
	ret

l000010E4:
	ld.w	d2,[0003C210]
	sub	d2,d9
	jge.u	d15,d2,000010C4

l000010EE:
	ret

;; fn000010F0: 000010F0
fn000010F0 proc
	ld.bu	d15,[a4]17
	ld.bu	d4,[a4]13
	sh	d2,d15,#0x10
	ld.bu	d15,[a4]18
	sh	d4,d4,#0x18
	sh	d15,d15,#0x8
	or	d15,d2
	ld.bu	d2,[a4]19
	movh.a	a15,#0xD000
	or	d15,d2
	ld.bu	d2,[a4]14
	lea	a12,[a15]280
	sh	d2,d2,#0x10
	or	d2,d4
	ld.bu	d4,[a4]16
	st.w	[a12]8,d15
	or	d4,d2
	ld.bu	d2,[a4]15
	mov	d15,#0x0
	sh	d3,d2,#0x8
	or	d2,d4,d3
	lea	a10,[a10]-4136
	st.w	[a12]4,d15
	st.w	[a15]280,d2
	mov	d9,#0x7
	ld.w	d15,[a12]4
	ld.w	d4,[a12]8
	ld.w	d8,[a12]
	sub	d4,d15
	mov	d2,#0x1000
	min.u	d4,d4,d2
	add	d8,d15
	add	d15,d4
	st.w	[a12]4,d15
	jeq	d4,#0x0,000011F8

l00001160:
	mov.a	a4,d8
	lea	a5,[a10]8
	mov	d5,#0x1020
	call	fn00001DB0
	mov	d15,d2
	jlt	d2,#0x1,000011F8

l00001174:
	sh	d2,d8,#0x28
	mov.aa	a4,a10
	st.b	[a10]4,d8
	st.b	[a10]1,d2
	sh	d2,d8,#0x30
	st.b	[a10],d9
	st.b	[a10]7,d15
	st.b	[a10]2,d2
	sh	d2,d8,#0x38
	mov	d10,#0x0
	mov	d8,#0x0
	st.b	[a10]3,d2
	sha	d2,d15,#0x30
	st.b	[a10]5,d2
	sha	d2,d15,#0x38
	st.b	[a10]6,d2
	call	fn00000CCE
	mov.a	a2,d8
	lea	a15,[a10]8
	add.a	a15,a2
	ld.bu	d2,[a15]
	st.b	[a10]2,d2
	ld.bu	d2,[a15]1
	st.b	[a10]3,d2
	ld.bu	d2,[a15]2
	st.b	[a10]4,d2
	add	d10,#0x1
	ld.bu	d2,[a15]3
	st.b	[a10]5,d2
	ld.bu	d2,[a15]4
	st.b	[a10]6,d2
	mov.aa	a4,a10
	ld.bu	d2,[a15]5
	add	d8,#0x6
	st.b	[a10],d9
	st.b	[a10]1,d10
	st.b	[a10]7,d2
	call	fn00000CCE
	Invalid
	mul	d15,d7
	mov	d4,#0x2710
	call	fn000010B0
	j	00001144

l000011F8:
	ret

;; fn000011FA: 000011FA
;;   Called from:
;;     00001326 (in fn000012F6)
fn000011FA proc
	movh.a	a12,#0xD000
	ld.bu	d15,[a12]564
	sub.a	a8,#0x8
	jge.u	d15,#0x4,000012F4

l00001208:
	movh.a	a2,#0xC000
	lea	a2,[a2]4636
	addsc.a	a2,a2,d15,#0x2
	lea	a15,[a12]564
	ld.bu	d15,[a4]12
	ji	a2
0000121A                               00 00 1D 00 08 00           ......
00001220 1D 00 32 00 1D 00 4C 00 1D 00 43 00 28 1F C2 FF ..2...L...C.(...
00001230 FF 7F 62 80 91 00 00 FC D9 FF 00 91 90 FF DC 0F ..b.............
00001240 1D 00 0E 00 1D 00 0E 00 1D 00 0E 00 1D 00 10 00 ................
00001250 1D 00 12 00 1D 00 12 00 1D 00 14 00 1D FF 46 FD ..............F.
00001260 1D FF 6C FD 6D FF 92 FD 82 1F 3C 32 6D FF EB FD ..l.m.....<2m...
00001270 82 3F 3C 2E 1D FF 66 FE 6D FF A3 FE 82 2F 3C 28 .?<...f.m..../<(
00001280 1D FF 38 FF 28 1F 5E 34 6D FF B0 FD 3C 0E DA 4F ..8.(.^4m...<..O
00001290 2C A0 82 FF 40 A4 2C A4 2C A3 2C A2 2C A1 2C A5 ,...@.,.,.,.,.,.
000012A0 2C A6 2C A7 6D FF 15 FD 82 0F 68 1F 3C 11 28 1F ,.,.m.....h.<.(.
000012B0 5E 44 6D FF 02 FE 3C F9 DA 5F 3C EB 28 1F 91 00 ^Dm...<.._<.(...
000012C0 00 CD 5E 69 6D FF AC FE DF 02 16 00 82 0F E9 CF ..^im...........
000012D0 34 80 00 90 82 0F 40 A4 E9 CF 34 80 DA 6F 2C A0 4.....@...4..o,.
000012E0 82 FF 2C A4 2C A3 2C A2 2C A1 2C A5 2C A6 2C A7 ..,.,.,.,.,.,.,.
000012F0 6D FF EF FC                                     m...            

l000012F4:
	ret

;; fn000012F6: 000012F6
;;   Called from:
;;     000000CC (in fn000000A8)
fn000012F6 proc
	movh.a	a15,#0xD000
	sub.a	a8,#0x18
	lea	a15,[a15]564
	call	fn00000B04
	mov	d15,#0x0
	st.w	[a15+],d15
	st.w	[a15+],d15
	st.w	[a15+],d15

l0000130C:
	mov	d4,#0x0
	call	fn00000664
	jeq	d2,#0x0,0000130C

l00001316:
	mov.aa	a4,a10
	mov	d4,#0x0
	call	fn000005CE
	mov	d4,#0x0
	call	fn000007CE
	mov.aa	a4,a10
	call	fn000011FA
	j	0000130C

;; fn0000132C: 0000132C
fn0000132C proc
	ld.bu	d2,[a4]1
	ld.bu	d15,[a4]
	sh	d2,d2,#0x8
	or	d3,d2,d15
	ld.bu	d15,[a4]2
	ne	d4,d4,#0x3
	sh	d15,d15,#0x10
	or	d2,d15,d3
	ld.bu	d15,[a4]3
	sh	d15,d15,#0x18
	or	d15,d2
	movh	d2,#0x9E37
	addi	d2,d2,#0x79B1
	mul	d15,d2
	sh	d2,d15,#0x2C
	sh	d3,d15,#0x2D
	sel	d2,d4,d2,d3
	ret
00001368                         DF 25 09 00 DF 35 0D 00         .%...5..
00001370 01 54 02 56 DF 15 0F 80 3C 05 01 54 02 56 01 64 .T.V....<..T.V.d
00001380 20 40 F4 54 00 90 01 64 20 40 01 54 01 56 80 4F  @.T...d @.T.V.O
00001390 AC 50 00 90 02 4F 40 4F 40 5D 40 6C 5C C8 40 F4 .P...O@O@]@l\.@.
000013A0 02 24 40 D5 02 F5 40 C6 3C E0 80 62 1B D2 FF 3F .$@...@.<..b...?
000013B0 80 4F 7F 3F 32 80 39 45 01 00 14 44 8F 85 00 50 .O.?2.9E...D...P
000013C0 0F 45 A0 60 39 44 02 00 0C 43 8F 04 01 40 0F 64 .E.`9D...C...@.d
000013D0 A0 50 8F 8F 01 F0 0F 5F A0 40 39 55 01 00 0C 50 .P....._.@9U...P
000013E0 8F 85 00 50 0F F5 A0 60 0C 52 D9 4F 04 00 8F 0F ...P...`.R.O....
000013F0 01 F0 0F 6F A0 50 0C 53 8F 8F 01 F0 A6 5F C6 4F ...o.P.S....._.O
00001400 6E 32 8B 0F 00 21 26 2F 0F 0F B0 F1 8B FF 01 F1 n2...!&/........
00001410 8F DF 1F 20 00 90 40 4F 80 FF 7F 3F 34 80 08 15 ... ..@O...?4...
00001420 08 04 8F 85 00 50 0F 45 A0 60 08 24 0C F3 8F 04 .....P.E.`.$....
00001430 01 40 0F 64 A0 50 8F 8F 01 F0 0F 5F A0 40 39 55 .@.d.P....._.@9U
00001440 01 00 0C 50 8F 85 00 50 0F F5 A0 60 0C 52 8F 0F ...P...P...`.R..
00001450 01 F0 0F 6F A0 50 0C 53 8F 8F 01 F0 A6 5F C6 4F ...o.P.S....._.O
00001460 EE 04 B0 4F B0 45 3C D9 8B 0F 00 21 26 2F 0F 0F ...O.E<....!&/..
00001470 B0 F1 8B FF 01 F1 06 DF 10 FF 01 4F 20 F0 80 F2 ...........O ...
00001480 00 90 9A F2 80 F3 7F F3 11 80 39 53 01 00 0C F1 ..........9S....
00001490 14 55 08 04 8F 83 00 30 8F 8F 00 F0 A6 53 A6 4F .U.....0.....S.O
000014A0 5F F3 04 80 B0 2F B0 25 80 FF 7F 2F 08 80 14 52 _..../.%.../...R
000014B0 0C F0 3A F2 80 F2 8A 12 60 2F 01 4F 20 40 80 42 ..:.....`/.O @.B
000014C0 00 90 39 4F 04 04 6E 27 FE 59 5E 39 19 42 00 04 ..9O..n'.Y^9.B..
000014D0 BB F0 FF 3F 42 42 7F 32 12 80 3C 09 5E 28 7B 00 ...?BB.2..<.^({.
000014E0 00 24 19 43 00 04 C2 12 7F 23 09 80 3B 00 00 21 .$.C.....#..;..!
000014F0 0B 24 40 41 8B 1F E0 44 76 4E A0 0F 40 42 D9 FF .$@A...DvN..@B..
00001500 00 04 82 0F 3C 02 24 2F FC FF 82 0F 59 4F 00 04 ....<.$/....YO..
00001510 59 4F 04 04 19 42 00 04 8B 02 20 F2 8B 25 00 F4 YO...B.... ..%..
00001520 6E 05 9B 12 00 20 59 42 00 04 82 0F 59 4F 0C 04 n.... YB....YO..
00001530 59 4F 08 04 59 4F 10 04 00 90 20 38 39 A3 38 00 YO..YO.... 89.8.
00001540 7B 00 E0 F7 59 A3 30 00 C2 1F 3F F4 04 80 82 02 {...Y.0...?.....
00001550 00 90 8B 15 40 22 F6 4B 8B 06 20 F2 26 2F EE F8 ....@".K.. .&/..
00001560 2C 60 82 12 DF 26 12 83 6C 70 00 90 BA 26 19 40 ,`...&..lp...&.@
00001570 00 04 19 41 10 04 78 03 26 2F EE EA 8B 37 00 32 ...A..x.&/...7.2
00001580 7B 10 00 20 1B B2 00 20 0B 24 40 F1 59 A3 10 00 {.. ... .$@.Y...
00001590 26 3F EE DE 80 5F 42 4F 02 43 0B 14 00 20 78 07 &?..._BO.C... x.
000015A0 80 6F 42 5F 42 04 59 A6 24 00 B5 A7 34 00 B5 A6 .oB_B.Y.$...4...
000015B0 14 00 B5 A5 04 00 78 02 59 42 10 04 59 44 00 04 ......x.YB..YD..
000015C0 59 47 04 04 8B D3 40 32 02 7C 80 4D 40 6D 80 5A YG....@2.|.M@m.Z
000015D0 DF 03 89 82 80 5F A2 0F 19 A3 1C 00 40 54 78 00 ....._......@Tx.
000015E0 1B 53 FF 3F 5A 10 D4 A6 60 D5 02 74 78 0A 59 A3 .S.?Z...`..tx.Y.
000015F0 20 00 6D FF D1 FE 19 AE 04 00 02 C4 C2 1E 60 E4  .m...........`.
00001600 6D FF 96 FE 58 0F 99 AD 14 00 06 6F 19 AA 04 00 m...X......o....
00001610 02 28 78 0B 19 A3 2C 00 DF 1C 07 00 58 0C A0 1F .(x...,.....X...
00001620 BA 1F 78 06 3C 75 82 1F 78 06 60 3F 86 A3 58 06 ..x.<u..x.`?..X.
00001630 B0 1F 59 A3 18 00 19 A3 20 00 42 EF 3F F3 53 82 ..Y..... .B.?.S.
00001640 60 D2 60 F4 01 28 02 36 82 14 D4 3E 6D FF 70 FE `.`..(.6...>m.p.
00001650 D4 A6 60 E4 60 D5 02 84 82 15 60 2C 6D FF 86 FE ..`.`.....`,m...
00001660 80 E3 C2 F3 9B 13 00 30 7F E3 06 80 80 F3 02 FE .......0........
00001670 80 C8 3C DC 39 E3 01 00 39 E8 02 00 14 E9 8F 83 ..<.9...9.......
00001680 00 30 60 E4 A6 39 8F 08 01 30 39 E8 03 00 A6 39 .0`..9...09....9
00001690 8F 88 01 30 39 48 01 00 A6 39 14 4B 8F 88 00 30 ...09H...9.K...0
000016A0 39 48 02 00 A6 3B 8F 08 01 30 39 48 03 00 A6 3B 9H...;...09H...;
000016B0 8F 88 01 80 0F B8 A0 30 5F 39 DA FF 3C 6A 58 04 .......0_9..<jX.
000016C0 82 0B 6E 06 60 D2 01 28 01 36 B9 3B 00 00 60 3C ..n.`..(.6.;..`<
000016D0 86 A3 80 FF 60 3F 19 A3 20 00 42 EF B0 1C 3F F3 ....`?.. .B...?.
000016E0 02 82 60 F4 02 C4 6D FF 23 FE DF 2C 1C 80 60 D2 ..`...m.#..,..`.
000016F0 01 28 02 36 74 39 19 A4 28 00 0B 4B 30 31 19 A4 .(.6t9..(..K01..
00001700 18 00 26 43 DF 03 16 00 80 C3 02 FE 02 28 58 00 ..&C.........(X.
00001710 02 E9 A2 F9 DF 2C D5 FF 60 D2 01 28 02 36 54 3B .....,..`..(.6T;
00001720 3C D7 DF 3C EA FF 60 D4 01 48 01 36 B4 39 3C E4 <..<..`..H.6.9<.
00001730 DF 3C 08 00 1B FB FF 8F 9B 18 00 80 3F 98 E6 FF .<..........?...
00001740 D4 AE 60 B2 60 E4 30 2E 39 E3 01 00 39 E8 02 00 ..`.`.0.9...9...
00001750 14 E9 8F 83 00 30 A6 39 8F 08 01 30 39 E8 03 00 .....0.9...09...
00001760 A6 39 8F 88 01 30 39 48 01 00 0F 93 A0 70 14 44 .9...09H.....p.D
00001770 8F 88 00 30 39 48 02 00 0F 43 A0 90 8F 08 01 30 ...09H...C.....0
00001780 39 48 03 00 A6 93 8F 88 01 80 A6 83 5F 37 BE FF 9H.........._7..
00001790 60 EF 80 F2 58 01 80 E4 0B 2A 30 31 0B 4F 30 32 `...X....*01.O02
000017A0 F6 3B 19 A3 24 00 80 FF 80 FB A2 AF D9 DC 01 00 .;..$...........
000017B0 DF 13 1B 80 3C 0A 39 F1 FF FF 39 E3 FF FF 5F 31 ....<.9...9..._1
000017C0 F2 FF B0 FF B0 FE 3C E6 3B F0 0F 00 4B 0F 11 02 ......<.;...K...
000017D0 1B 8F 00 30 80 C4 42 03 19 A2 08 00 42 43 7F 32 ...0..B.....BC.2
000017E0 17 80 1D FF B6 FE 19 A3 0C 00 DF 03 11 00 1B 0F ................
000017F0 0F 80 3B F0 0F 00 4B 08 11 02 1B BF 00 30 19 A4 ..;...K......0..
00001800 08 00 42 30 80 C3 42 03 3F 34 6D 81 BF FF 1C 80 ..B0..B.?4m.....
00001810 1B 1F FF 4F 3B 00 FF 3F 3B F0 0F 00 4B 04 11 02 ...O;..?;...K...
00001820 34 D3 8B 04 80 32 82 F7 AB 00 80 33 D9 C3 01 00 4....2.....3....
00001830 9F 03 05 80 34 C4 40 3C 3C 09 34 C7 1B 14 F0 4F ....4.@<<.4....O
00001840 40 3C 3C F5 8F 4F 00 30 34 D3 10 CF 60 A3 40 C2 @<<..O.04...`.@.
00001850 40 34 D9 CC 08 00 A0 75 04 48 24 28 FC 5E 80 C2 @4.....u.H$(.^..
00001860 80 F3 D9 33 08 00 3F 32 F4 FF 58 07 C2 BF 19 A3 ...3..?2..X.....
00001870 0C 00 76 38 80 F4 19 A2 08 00 1B B4 00 30 3F 32 ..v8.........0?2
00001880 32 81 80 E4 02 B3 A2 43 37 03 70 30 60 B4 60 F6 2......C7.p0`.`.
00001890 B0 44 28 03 06 83 D9 E5 04 00 D9 FC 02 00 28 13 .D(...........(.
000018A0 6D FF 85 FD 1B 42 00 30 0B 3B 00 A0 19 A3 24 00 m....B.0.;....$.
000018B0 DF 03 3A 00 1B 02 0F 30 3B F0 0F 00 4B 03 11 02 ..:....0;...K...
000018C0 19 A4 08 00 80 C3 C2 60 42 03 7F 34 2D 80 19 A3 .......`B..4-...
000018D0 0C 00 DF 03 3E 7E 02 49 80 C4 A2 49 53 F9 2F 90 ....>~.I...IS./.
000018E0 1B 49 A1 9F 0B 29 80 20 42 2A 02 92 3F AE 1C 80 .I...). B*..?...
000018F0 02 A8 60 84 02 C4 6D FF 1B FD DF 2C 06 00 DF 3C ..`...m....,...<
00001900 0A 00 DF 1C 0D 80 60 D2 01 22 02 36 82 02 74 32 ......`..".6..t2
00001910 3C 06 60 D2 01 22 01 36 82 02 B4 32 C2 18 7F 8E <.`..".6...2....
00001920 EA FF 02 92 14 D3 BF F2 28 80 1B F3 00 30 1B 12 ........(....0..
00001930 FF 2F 3B C0 3F 40 34 D3 4B 42 11 02 82 F3 28 23 ./;.?@4.KB....(#
00001940 28 33 28 43 28 53 60 0F FD F0 0C 00 3B F0 0F 30 (3(C(S`.....;..0
00001950 4B 32 11 22 01 C2 00 C6 D9 CD 01 00 34 C3 3C 0F K2."........4.<.
00001960 89 C3 04 04 E9 C3 01 00 E9 C3 02 00 E9 C3 03 00 ................
00001970 1B 42 C0 2F 3C EA 42 23 34 D3 40 CD 19 A3 20 00 .B./<.B#4.@... .
00001980 7F 3A B1 80 60 A4 D4 A6 60 D5 B0 E4 02 C4 6D FF .:..`...`.....m.
00001990 03 FD 60 A4 DF 1C 38 80 82 14 6D FF C9 FC 60 D2 ..`...8...m...`.
000019A0 D4 A6 01 22 02 36 60 A4 D4 3E 60 D5 82 14 6D FF ...".6`..>`...m.
000019B0 F3 FC 80 E3 C2 F3 9B 13 00 30 3F A3 8B 80 39 E3 .........0?...9.
000019C0 01 00 14 E1 8F 83 00 30 0F 13 A0 80 39 E1 02 00 .......0....9...
000019D0 60 A4 8F 01 01 10 0F 81 A0 30 39 E1 03 00 39 48 `........09...9H
000019E0 01 00 8F 81 01 10 A6 31 14 43 8F 88 00 80 0F 38 .......1.C.....8
000019F0 A0 90 39 43 02 00 8F 03 01 30 0F 93 A0 80 39 43 ..9C.....0....9C
00001A00 03 00 3C 5B 02 C4 6D FF 93 FC 54 A4 02 A3 A2 43 ..<[..m...T....C
00001A10 DF 2C 07 80 60 D2 01 22 02 36 54 35 3C 0A 19 A4 .,..`..".6T5<...
00001A20 10 00 82 05 76 46 60 D2 01 22 01 36 B9 35 00 00 ....vF`..".6.5..
00001A30 D4 AE 60 54 30 4E DF 2C 09 00 DF 3C 0B 80 60 DF ..`T0N.,...<..`.
00001A40 01 F2 01 36 B4 33 3C 05 60 D5 01 52 02 36 74 33 ...6.3<.`..R.6t3
00001A50 19 A2 28 00 19 A4 30 00 0B 25 50 11 8B 14 00 15 ..(...0..%P.....
00001A60 DF 01 38 00 19 A2 10 00 F6 26 C2 F5 9B 15 00 50 ..8......&.....P
00001A70 3F 35 30 80 39 E3 01 00 14 E1 8F 83 00 30 0F 13 ?50.9........0..
00001A80 A0 80 39 E1 02 00 60 A2 8F 01 01 10 0F 81 A0 30 ..9...`........0
00001A90 39 E1 03 00 39 28 01 00 8F 81 01 10 A6 31 14 23 9...9(.......1.#
00001AA0 8F 88 00 80 0F 38 A0 90 39 23 02 00 8F 03 01 30 .....8..9#.....0
00001AB0 0F 93 A0 80 39 23 03 00 8F 83 01 30 A6 83 5F 31 ....9#.....0.._1
00001AC0 09 80 82 03 D9 DF 01 00 02 AB 34 D3 1D FF D1 FE ..........4.....
00001AD0 1B 1A 00 E0 60 E4 02 C4 6D FF 2A FC 02 28 1D FF ....`...m.*..(..
00001AE0 9B FD 58 07 19 A3 24 00 A2 AF DF 03 1B 00 1B 0F ..X...$.........
00001AF0 0F 30 3B F0 0F 40 4B 43 11 42 92 16 80 D2 42 64 .0;..@KC.B....Bd
00001B00 19 A3 08 00 42 24 7F 43 0D 80 58 03 DF 0F 21 7D ....B$.C..X...!}
00001B10 02 35 A2 25 1B F5 FF 4F 1B 05 0F 50 06 85 5A 54 .5.%...O...P..ZT
00001B20 D9 D2 01 00 BF FF 19 80 3B 00 FF 4F 1B 1F FF 3F ........;..O...?
00001B30 3B F0 0F 20 34 D4 4B 23 11 42 82 F6 60 4F D9 23 ;.. 4.K#.B..`O.#
00001B40 01 00 FD F0 05 00 40 3F 34 23 3C 0A 34 26 1B 13 ......@?4#<.4&..
00001B50 F0 3F 40 32 3C F5 8F 4F 00 30 40 2F 34 D3 60 A5 .?@2<..O.0@/4.`.
00001B60 40 F4 02 F4 6D 00 CD 01 19 A3 0C 00 10 FF 76 38 @...m.........v8
00001B70 19 A4 04 00 12 A3 5A 43 99 A2 34 00 6C 20 99 A4 ......ZC..4.l ..
00001B80 14 00 01 4F 20 40 80 42 00 90 3B 70 A9 22 00 90 ...O @.B..;p."..
00001B90 91 00 00 2C D9 22 D8 D1 00 90                   ...,."....      

;; fn00001B9A: 00001B9A
;;   Called from:
;;     00001D0A (in fn00001DB0)
fn00001B9A proc
	movh	d15,#0x7E00
	add	d15,#0x1
	mov	d2,#0x0
	jge.u	d4,d15,00001BB4

l00001BA6:
	mov	d2,#0xFF
	div	e2,d4,d2
	add	d2,d4
	addi	d2,d2,#0x10

l00001BB4:
	ret
00001BB6                   3B 00 02 24 00 90 7B 10 00 30       ;..$..{..0
00001BC0 0B 65 10 88 C2 13 0B 38 80 21 20 08 8B 18 80 82 .e.....8.! .....
00001BD0 40 4F 40 5C 40 6D 02 4F AB 12 80 88 6D FF DF FF @O@\@m.O....m...
00001BE0 3F 29 35 00 7B 10 00 20 1B B2 00 20 7F 2F 16 00 ?)5.{.. ... ./..
00001BF0 40 F4 02 F4 82 35 6D FF 66 FC 19 F2 00 04 76 22 @....5m.f.....v"
00001C00 82 12 74 A2 59 A8 04 00 40 F4 40 C5 40 D6 02 F4 ..t.Y...@.@.@...
00001C10 A0 07 82 05 82 06 3C 33 91 10 00 20 82 12 01 2C ......<3... ...,
00001C20 30 94 AB 22 80 99 0B F9 10 48 40 F4 6D FF 4B FC 0..".....H@.m.K.
00001C30 82 02 59 A8 04 00 40 F4 74 A2 40 C5 40 D6 02 F4 ..Y...@.t.@.@...
00001C40 A0 07 82 05 82 06 02 97 3C 34 7B 10 00 20 1B B2 ........<4{.. ..
00001C50 00 20 7F 2F 17 00 40 F4 02 F4 82 35 6D FF 33 FC . ./..@....5m.3.
00001C60 19 F2 00 04 76 22 82 12 74 A2 59 A8 04 00 40 F4 ....v"..t.Y...@.
00001C70 40 C5 40 D6 02 F4 A0 07 02 95 82 16 82 37 3C 19 @.@..........7<.
00001C80 91 10 00 20 82 12 01 2C 30 A4 AB 22 80 AA 0B FA ... ...,0.."....
00001C90 10 48 40 F4 6D FF 17 FC 82 02 59 A8 04 00 40 F4 .H@.m.....Y...@.
00001CA0 74 A2 40 C5 40 D6 02 F4 A0 07 02 95 82 16 02 A7 t.@.@...........
00001CB0 1D FF 45 FC                                     ..E.            

;; fn00001CB4: 00001CB4
;;   Called from:
;;     00001CF0 (in fn00001DB0)
fn00001CB4 proc
	mov	d2,#0x4020
	lt.u	d4,d4,d2
	mov.d	d15,a4
	or.eq	d4,d15,#0x0
	mov.a	a2,#0x0
	jnz	d4,00001CDC

l00001CC6:
	and	d15,#0x3
	jnz	d15,00001CDC

l00001CCA:
	mov.a	a15,#0x0
	mov.aa	a2,a4
	lea	a15,[a15]16404
	mov	d2,#0x0
	j	00001CD8

l00001CD6:
	st.b	[a2+],d2

l00001CD8:
	loop	d15,00001CD6

l00001CDA:
	mov.aa	a2,a4

l00001CDC:
	ret

l00001CDE:
	sub.a	a0,#0x10
	mov	d15,d4
	mov	d4,#0x4020
	mov	e8,d5,d6
	st.a	[a10]12,d6
	mov.aa	a15,a5
	call	fn00001CB4
	movh	d3,#0x1
	add	d3,#0x1
	min	d2,d8,d3
	mov	d4,d15
	ge	d8,d8,#0x1
	mov.aa	a12,a2
	sel	d8,d8,d2,#0x1
	call	fn00001B9A
	ld.a	a6,[a10]12
	Invalid
	adds	d0,d0
	movh	d2,#0x1
	addi	d2,d2,#0xB
	jge	d15,d2,00001D38
	mov	d2,#0x0
	st.w	[a10]4,d8
	mov.aa	a4,a12
	st.w	[a10],d2
	mov.aa	a5,a15
	mov	d4,d15
	mov.a	a7,#0x0
	mov	d5,#0x0
	mov	d6,#0x0
	j	00001D76
	mov	d3,#0x0
	movh.a	a2,#0x1
	ge.a	d2,a15,a2
	st.w	[a10],d3
	st.w	[a10]4,d8
	mov.aa	a4,a12
	mov.aa	a5,a15
	mov	d4,d15
	mov.a	a7,#0x0
	mov	d5,#0x0
	mov	d6,#0x0
	j	00001D96
	movh	d2,#0x1
	addi	d2,d2,#0xB
	jge	d15,d2,00001D7A
	mov	d2,#0x0
	st.w	[a10]4,d8
	mov.aa	a4,a12
	st.w	[a10],d2
	mov.aa	a5,a15
	mov	d4,d15
	mov.a	a7,#0x0
	mov	d5,d9
	mov	d6,#0x1
	mov	d7,#0x3
	j	00001D9C
	mov	d3,#0x0
	movh.a	a2,#0x1
	ge.a	d2,a15,a2
	st.w	[a10],d3
	st.w	[a10]4,d8
	mov.aa	a4,a12
	mov.aa	a5,a15
	mov	d4,d15
	mov.a	a7,#0x0
	mov	d5,d9
	mov	d6,#0x1
	mov	d7,#0x1
	sel	d7,d2,d7,#0x2
	j	0000153A

l00001DA0:
	mov.aa	a15,a4
	lea	a10,[a10]-16416
	mov.aa	a6,a5
	mov.aa	a4,a10
	mov.aa	a5,a15
	j	00001CDE

;; fn00001DB0: 00001DB0
;;   Called from:
;;     0000116A (in fn000010F0)
fn00001DB0 proc
	mov	d6,#0x1
	j	00001DA0
00001DB6                   D9 AA D8 FB 40 6D 02 48 40 4F       ....@m.H@O
00001DC0 3B 00 02 44 D9 A4 08 00 40 5C 6D FF 75 FF 4C D0 ;..D....@\m.u.L.
00001DD0 02 F4 6D FF E4 FE 3F 28 0B 00 D9 A4 08 00 40 F5 ..m...?(......@.
00001DE0 40 C6 0B F8 10 48 82 16 1D FF 7B FF 7B 10 00 20 @....H....{.{.. 
00001DF0 1B B2 00 20 7F 2F 11 00 82 02 D9 A4 08 00 40 F5 ... ./........@.
00001E00 74 A2 82 12 40 C6 02 F4 59 A2 04 00 40 D7 02 85 t...@...Y...@...
00001E10 82 26 82 37 3C 14 82 12 82 03 91 10 00 20 01 2F .&.7<........ ./
00001E20 30 74 74 A3 59 A2 04 00 D9 A4 08 00 40 F5 40 C6 0tt.Y.......@.@.
00001E30 02 F4 40 D7 02 85 82 26 AB 22 80 77 1D FF 7F FB ..@....&.".w....
00001E40 91 00 00 4C 7B 00 00 FC D9 44 E0 D1 1B 0F F6 F1 ...L{....D......
00001E50 C2 3F 80 42 A2 2F BF 7F 08 80 91 00 00 F0 D9 FF .?.B./..........
00001E60 00 00 BC F2 DC 0F 00 90                         ........        

l00001E68:
	movh.a	a4,#0xC000
	lea	a4,[a4]8032
	movh	d4,#0xC000
	mov.d	d15,a4
	addi	d4,d4,#0x1F60
	sub	d4,d15
	sha	d4,#0xFFFFFFFE
	mov	d15,#0x2
	div	e4,d4,d15
	jz	d4,00001E92

l00001E86:
	movh.a	a15,#0x0
	lea	a15,[a15]
	jz.a	a15,00001E92

l00001E90:
	ji	a15

l00001E92:
	ret
00001E94             91 00 00 FD 39 FF 10 00 EE 21 91 00     ....9....!..
00001EA0 00 DC 80 D2 7B 00 00 8C 1B 42 F7 F1 1B 08 F8 81 ....{....B......
00001EB0 A2 F8 86 E8 91 00 00 CD 60 FD C2 F8 D9 CC 14 00 ........`.......
00001EC0 4C C0 7F 8F 09 80 C2 1F 90 D2 D4 22 6C C0 2D 02 L.........."l.-.
00001ED0 00 00 3C F7 82 1F 6D FF B5 FF E9 FF 10 00 00 90 ..<...m.........

;; fn00001EE0: 00001EE0
;;   Called from:
;;     00001F40 (in fn00001F40)
fn00001EE0 proc
	movh.a	a4,#0xC000
	lea	a4,[a4]8032
	ld.w	d15,[a4]
	jz	d15,00001EFA

l00001EEC:
	movh.a	a15,#0x0
	lea	a15,[a15]
	jz.a	a15,00001EFA

l00001EF6:
	calli	a15

l00001EFA:
	j	00001E68
00001EFE                                           40 42               @B
00001F00 76 46 60 43 B0 F3 04 54 24 44 FC 3E 00 90       vF`C...T$D.>..  

l00001F0E:
	jz	d4,00001F16

l00001F10:
	mov.a	a14,d4
	debug

l00001F14:
	j	00001F14

l00001F16:
	mov.u	d15,#0x900D
	mov.a	a14,d15
	debug
	j	00001F14

;; fn00001F20: 00001F20
;;   Called from:
;;     00001F44 (in fn00001F40)
fn00001F20 proc
	movh.a	a12,#0xC000
	mov	d15,#0x0
	lea	a12,[a12]8036

l00001F2A:
	addsc.a	a15,a12,d15,#0x0
	add	d15,#0xFFFFFFFC
	ld.a	a15,[a15]
	mov.d	d2,a15
	jeq	d2,#0xFFFFFFFF,00001F3C

l00001F36:
	calli	a15
	j	00001F2A

l00001F3C:
	ret
00001F3E                                           00 00               ..

;; fn00001F40: 00001F40
;;   Called from:
;;     000000BC (in fn000000A8)
fn00001F40 proc
	call	fn00001EE0
	call	fn00001F20
	ret
00001F4A                               00 00 00 00 00 00           ......
00001F50 6D FF A2 FF 00 90 00 00 31 2E 39 2E 33 00 00 00 m.......1.9.3...
00001F60 02 00 00 00 FF FF FF FF 00 00 00 00 00 00 00 00 ................
00001F70 02 00 00 00 FF FF FF FF 00 00 00 00 00 00 00 00 ................
00001F80 10 00 00 D0 38 02 00 00 10 00 00 D0 00 00 00 00 ....8...........
00001F90 10 00 00 D0 00 00 00 00 10 00 00 D0 00 00 00 00 ................
00001FA0 FF FF FF FF FF FF FF FF 60 1F 00 C0 10 00 00 D0 ........`.......
00001FB0 00 00 00 00 58 1F 00 C0 10 00 00 D0 00 00 00 00 ....X...........
00001FC0 58 1F 00 C0 0C 00 00 D0 04 00 00 00 58 1F 00 C0 X...........X...
00001FD0 10 00 00 D0 00 00 00 00 FF FF FF FF FF FF FF FF ................
00001FE0 FF FF FF FF                                     ....            
