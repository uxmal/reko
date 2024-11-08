;;; Segment seg000C0154 (000C0154)

;; fn000C0154: 000C0154
fn000C0154 proc
	enter	#2h
	bset	[1FE5h]
	mov.b:z	#0h,r0l
	btst	[1F24h]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#41h,#42h,[43Ch]
	mov.b:z	#0h,[43Dh]
	cmp.w	#41h,[43Ch]
	jeq	000C017A

l000C0177:
	jmp.w	000C04C9

l000C017A:
	jsr.w	fn000C0BBA
	jsr.w	fn000C0A08
	jsr.w	fn000C0BA2
	jsr.w	fn000C0978
	jsr.w	fn000C0B92
	jsr.w	fn000C098C
	jsr.w	fn000C0BE8
	jsr.w	fn000C0C46
	jsr.w	fn000C0C52
	mov.w	#55h,r1
	jsr.w	UART1_putc
	mov.w	#55h,r1
	jsr.w	UART1_putc
	mov.w	#55h,r1
	jsr.w	UART1_putc
	mov.w	#55h,r1
	jsr.w	UART1_putc
	mov.w	#55h,r1
	jsr.w	UART1_putc
	jsr.w	fn000C0E46
	mov.w:q	#5h,r2
	mov.w:q	#1h,r1
	jsr.w	fn000C1186
	push.w	#0Ch
	push.w	#12Fh
	jsr.w	fn000C1222
	add:q	#4h,usp
	push.w	#0Ch
	push.w	#0E3h
	jsr.w	fn000C1222
	add:q	#4h,usp
	push.w	#0Ch
	push.w	#0C0h
	jsr.w	fn000C1222
	add:q	#4h,usp
	mov.w	#20h,r2
	mov.w:q	#1h,r1
	jsr.w	fn000C1186
	push.w	#0Ch
	push.w	#8Ah
	jsr.w	fn000C1222
	add:q	#4h,usp
	mov.w:q	#1h,r2
	mov.w:q	#5h,r1
	jsr.w	fn000C1186

l000C0206:
	mov.w:q	#1h,r0
	jne	000C020D

l000C020A:
	jmp.w	000C04C6

l000C020D:
	btst	[1F4Eh]
	bmeq	[1F46h]
	cmp.w:q	#1h,-2h[fb]
	jne	000C0230

l000C021B:
	bset	[1F49h]
	bset	[1F4Ah]
	bset	[1F6Ch]
	bset	[1F6Eh]
	mov.w:q	#0h,-2h[fb]
	jmp.b	000C0243

l000C0230:
	bclr	[1F49h]
	bclr	[1F4Ah]
	bclr	[1F6Ch]
	bclr	[1F6Eh]
	mov.w:q	#1h,-2h[fb]

l000C0243:
	mov.w:q	#0h,r2
	mov.w:q	#5h,r1
	jsr.w	fn000C1186
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F0Dh]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F0Eh]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F0Fh]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F83h]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F84h]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3C0h],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3C2h],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3C4h],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3C6h],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3C8h],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3CAh],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w:q	#0h,r2
	mov.w	#9h,r1
	jsr.w	fn000C1186
	mov.b:s	[3E4h],r0l
	not.b:s	#72h,r0l
	mov.b:s	r0l,-28h[fb]
	mov.b:s	r0l,[2177h]
	mov.b:s	[0F500h],r0h
	stz	#0Ah,0F5h[sb]

l000C043C:
	and.b	r0l,[0C175h]
	add.b:s	r0h,r0l
	brk
	jsr.w	UART1_putc
	mov.b:s	[3E4h],r0l
	not.b:s	#72h,r0l
	mov.b:s	r0l,-28h[fb]
	mov.b:s	r0l,[2177h]
	sha.b:q	#0h,r0l
	jsr.w	fn000C0F06
	jsr.w	fn000C13CC
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F82h]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F4Eh]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	jne	000C04BC

l000C04B2:
	mov.w	#31h,r1
	jsr.w	UART1_putc
	jmp.w	000C0206

l000C04BC:
	mov.w	#30h,r1
	jsr.w	UART1_putc
	jmp.w	000C0206

l000C04C6:
	jmp.w	000C0975

l000C04C9:
	jsr.w	fn000C0BBA
	jsr.w	fn000C0B0A
	jsr.w	fn000C0BA2
	jsr.w	fn000C0978
	jsr.w	fn000C0B92
	jsr.w	fn000C0A7E
	jsr.w	fn000C0BE8
	jsr.w	fn000C0C46
	jsr.w	fn000C0C52
	mov.w	#55h,r1
	jsr.w	UART1_putc
	mov.w	#55h,r1
	jsr.w	UART1_putc
	mov.w	#55h,r1
	jsr.w	UART1_putc
	mov.w	#55h,r1
	jsr.w	UART1_putc
	mov.w	#55h,r1
	jsr.w	UART1_putc
	jsr.w	fn000C0E46
	mov.w:q	#5h,r2
	mov.w:q	#1h,r1
	jsr.w	fn000C1186
	mov.w	[3CAh],a0
	mov.b	a0,[3D8h]
	btst	[1FC0h]
	bmeq	[1F40h]
	btst	[1FC1h]
	bmeq	[1F41h]
	btst	[1FC2h]
	bmeq	[1F42h]
	btst	[1FC3h]
	bmeq	[1F43h]
	btst	[1FC4h]
	bmeq	[1F44h]
	btst	[1FC5h]
	bmeq	[1F45h]
	btst	[1F0Dh]
	bmne	[1F28h]
	btst	[1F0Eh]
	bmne	[1F29h]
	btst	[1F0Fh]
	bmne	[1F2Ah]
	btst	[1FCDh]
	bmne	[1F2Bh]
	btst	[1FCEh]
	bmne	[1F2Ch]
	btst	[1FCFh]
	bmne	[1F2Dh]
	push.w	#0Ch
	push.w	#73h
	jsr.w	fn000C1222
	add:q	#4h,usp
	push.w	#0Ch
	push.w	#2Dh
	jsr.w	fn000C1222
	add:q	#4h,usp
	push.w	#0Ch
	push.w	#0h
	jsr.w	fn000C1222
	add:q	#4h,usp
	mov.w	#2Ch,r2
	mov.w:q	#1h,r1
	jsr.w	fn000C1186
	push.w	#0Ch
	push.w	#8Ah
	jsr.w	fn000C1222
	add:q	#4h,usp
	mov.w:q	#1h,r2
	mov.w:q	#5h,r1
	jsr.w	fn000C1186

l000C05C9:
	mov.w:q	#1h,r0
	jne	000C05D0

l000C05CD:
	jmp.w	000C0975

l000C05D0:
	btst	[1FC0h]
	bmeq	[1F40h]
	btst	[1FC1h]
	bmeq	[1F41h]
	btst	[1FC2h]
	bmeq	[1F42h]
	btst	[1FC3h]
	bmeq	[1F43h]
	btst	[1FC4h]
	bmeq	[1F44h]
	btst	[1FC5h]
	bmeq	[1F45h]
	btst	[1F0Dh]
	bmne	[1F28h]
	btst	[1F0Eh]
	bmne	[1F29h]
	btst	[1F0Fh]
	bmne	[1F2Ah]
	btst	[1FCDh]
	bmne	[1F2Bh]
	btst	[1FCEh]
	bmne	[1F2Ch]
	btst	[1FCFh]
	bmne	[1F2Dh]
	cmp.w:q	#1h,-2h[fb]
	jne	000C065A

l000C0641:
	bset	[1F49h]
	bset	[1F4Ah]
	bset	[1F4Fh]
	bset	[1F4Ch]
	bset	[1F4Bh]
	mov.w:q	#0h,-2h[fb]
	jmp.b	000C0671

l000C065A:
	bclr	[1F49h]
	bclr	[1F4Ah]
	bclr	[1F4Fh]
	bclr	[1F4Ch]
	bclr	[1F4Bh]
	mov.w:q	#1h,-2h[fb]

l000C0671:
	mov.w:q	#0h,r2
	mov.w:q	#5h,r1
	jsr.w	fn000C1186
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1FC0h]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1FC1h]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1FC2h]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1FC3h]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1FC4h]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1FC5h]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F0Dh]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F0Eh]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F0Fh]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1FCDh]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1FCEh]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1FCFh]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3C0h],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3C2h],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3C4h],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3C6h],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3C8h],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3CAh],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3CCh],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	[3CEh],r1
	jsr.w	fn000C0F06
	jsr.w	fn000C1616
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w:q	#0h,r2
	mov.w	#9h,r1
	jsr.w	fn000C1186
	mov.b:s	[3E4h],r0l
	not.b:s	#72h,r0l
	mov.b:s	r0l,-28h[fb]
	mov.b:s	r0l,[2177h]
	mov.b:s	[0F500h],r0h
	sub.b:s	#6h,r0h
	jsr.w	fn000C13CC
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:s	[3E4h],r0l
	not.b:s	#72h,r0l
	mov.b:s	r0l,-28h[fb]
	mov.b:s	r0l,[2177h]
	sha.b:q	#0h,r0l
	jsr.w	fn000C0F06
	jsr.w	fn000C13CC
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F82h]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F6Dh]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F83h]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	mov.b:z	#0h,r0l
	btst	[1F84h]
	bmne	#0h,r0
	mov.b:z	#0h,r0h
	cmp.w:q	#1h,r0
	stzx	#31h,#30h,r0l
	mov.b:z	#0h,r0h
	mov.w	r0,r1
	jsr.w	UART1_putc
	mov.w	#20h,r1
	jsr.w	UART1_putc
	jmp.w	000C05C9

l000C0975:
	exitd
000C0977                      04                                .        

;; fn000C0978: 000C0978
;;   Called from:
;;     000C0183 (in fn000C0154)
;;     000C04D2 (in fn000C0154)
fn000C0978 proc
	mov.w:s	#3D4h,a0
	mov.b	[a0],a1
	or.b	#1h,a1
	mov.b	a1,[a0]
	mov.b:s	#58h,[3D6h]
	mov.b:s	#23h,[3D7h]
	rts
000C098B                                  04                        .    

;; fn000C098C: 000C098C
;;   Called from:
;;     000C0189 (in fn000C0154)
fn000C098C proc
	mov.b:z	#0h,[4Eh]
	mov.b:z	#0h,[5Dh]
	mov.b:z	#0h,[5Eh]
	mov.b:z	#0h,[5Fh]
	mov.b:z	#0h,[44h]
	mov.b:z	#0h,[49h]
	mov.b:z	#0h,[48h]
	mov.b:z	#0h,[52h]
	mov.b:z	#0h,[51h]
	mov.b:s	#1h,[54h]
	mov.b:z	#0h,[53h]
	mov.b:s	#4h,[55h]
	mov.b:z	#0h,[56h]
	mov.b:z	#0h,[57h]
	mov.b:z	#0h,[58h]
	mov.b:z	#0h,[59h]
	mov.b:z	#0h,[5Ah]
	mov.b:z	#0h,[5Bh]
	mov.b:z	#0h,[5Ch]
	mov.b:z	#0h,[47h]
	mov.b:z	#0h,[46h]
	mov.b:z	#0h,[45h]
	mov.w:s	#1DFh,a0
	mov.b	[a0],a1
	or.b	#0C0h,a1
	mov.b	a1,[a0]
	bset	[0EF9h]
	bset	[0EFAh]
	bset	[0EFBh]
	bset	[0EFCh]
	bset	[0EFDh]
	bset	[0E7Bh]
	bset	[0E7Ch]
	bset	[0E7Dh]
	bset	[0E79h]
	bset	[0E78h]
	bset	[0E7Ah]
	rts
000C0A07                      04                                .        

;; fn000C0A08: 000C0A08
;;   Called from:
;;     000C017D (in fn000C0154)
fn000C0A08 proc
	mov.b:z	#0h,[3E2h]
	mov.b:z	#0h,[3E3h]
	mov.b:z	#0h,[3E6h]
	mov.b:z	#0h,[3E7h]
	mov.b:s	#40h,[3EAh]
	mov.b:z	#0h,[3EBh]
	mov.b:s	#88h,[3EEh]
	mov.b:z	#0h,[3EFh]
	bset	[52h]
	mov.b:z	#0h,[3F3h]
	bclr	[52h]
	mov.b:z	#0h,[3F6h]
	bset	[1EFFh]
	mov.b:z	#0h,[3F7h]
	mov.b:z	#0h,[3FAh]
	mov.b:z	#0h,[3FBh]
	bclr	[1FE0h]
	bclr	[1FE1h]
	bclr	[1FE2h]
	bclr	[1FE3h]
	bset	[1FE4h]
	bset	[1FE5h]
	bclr	[1FE6h]
	bclr	[1FE7h]
	bclr	[1FE8h]
	bclr	[1FE9h]
	bclr	[1FEAh]
	bclr	[1FEBh]
	bclr	[1FECh]
	bclr	[1FEDh]
	bclr	[1FEEh]
	bclr	[1FEFh]
	rts

;; fn000C0A7E: 000C0A7E
;;   Called from:
;;     000C04D8 (in fn000C0154)
fn000C0A7E proc
	mov.b:z	#0h,[4Eh]
	mov.b:s	#1h,[5Dh]
	mov.b:s	#2h,[5Eh]
	mov.b:s	#2h,[5Fh]
	mov.b:s	#3h,[44h]
	mov.b:s	#3h,[49h]
	mov.b:s	#3h,[48h]
	mov.b:z	#0h,[52h]
	mov.b:z	#0h,[51h]
	mov.b:s	#1h,[54h]
	mov.b:z	#0h,[53h]
	mov.b:s	#4h,[55h]
	mov.b:s	#1h,[56h]
	mov.b:s	#3h,[57h]
	mov.b:s	#3h,[58h]
	mov.b:s	#1h,[59h]
	mov.b:s	#1h,[5Ah]
	mov.b:s	#3h,[5Bh]
	mov.b:s	#1h,[5Ch]
	mov.b:s	#1h,[47h]
	mov.b:s	#1h,[46h]
	mov.b:s	#1h,[45h]
	mov.w:s	#1DFh,a0
	mov.b	[a0],a1
	or.b	#0C0h,a1
	mov.b	a1,[a0]
	bset	[0EF9h]
	bset	[0EFAh]
	bset	[0EFBh]
	bset	[0EFCh]
	bset	[0EFDh]
	bset	[0E7Bh]
	bset	[0E7Ch]
	bset	[0E7Dh]
	bset	[0E79h]
	bset	[0E78h]
	bset	[0E7Ah]
	rts
000C0B09                            04                            .      

;; fn000C0B0A: 000C0B0A
;;   Called from:
;;     000C04CC (in fn000C0154)
fn000C0B0A proc
	mov.b:s	#0FFh,[3E5h]
	mov.b:s	#0FFh,[3E8h]
	mov.b:z	#0h,[3E2h]
	mov.b:z	#0h,[3E3h]
	mov.b:z	#0h,[3E6h]
	mov.b:s	#0FFh,r0l
	mov.b:s	r0l,[3E7h]
	mov.b:s	r0l,[3EAh]
	mov.b:s	r0l,[3EBh]
	mov.b:s	#88h,[3EEh]
	mov.b:z	#0h,[3EFh]
	bset	[52h]
	mov.b:z	#0h,[3F3h]
	bclr	[52h]
	mov.b:z	#0h,[3F6h]
	bset	[1EFFh]
	mov.b:z	#0h,[3F7h]
	mov.b:z	#0h,[3FAh]
	mov.b:z	#0h,[3FBh]
	mov.b:s	#0FFh,[3E5h]
	mov.b:s	#0FFh,[3E8h]
	bclr	[1FE0h]
	bclr	[1FE1h]
	bclr	[1FE2h]
	bclr	[1FE3h]
	bset	[1FE4h]
	bset	[1FE5h]
	bclr	[1FE6h]
	bclr	[1FE7h]
	bclr	[1FE8h]
	bclr	[1FE9h]
	bclr	[1FEAh]
	bclr	[1FEBh]
	bclr	[1FECh]
	bclr	[1FEDh]
	bclr	[1FEEh]
	bclr	[1FEFh]
	rts
000C0B91    04                                            .              

;; fn000C0B92: 000C0B92
;;   Called from:
;;     000C0186 (in fn000C0154)
;;     000C04D5 (in fn000C0154)
fn000C0B92 proc
	bset	[1EE0h]
	mov.b:z	#0h,[3D8h]
	bset	[1EE1h]
	mov.b:z	#0h,[3DAh]
	rts
000C0BA1    04                                            .              

;; fn000C0BA2: 000C0BA2
;;   Called from:
;;     000C0180 (in fn000C0154)
;;     000C04CF (in fn000C0154)
fn000C0BA2 proc
	mov.b:s	#5h,[3A8h]
	mov.b:s	#18h,[3ACh]
	mov.w:s	#3ADh,a0
	mov.b	[a0],a1
	or.b	#4h,a1
	mov.b	a1,[a0]
	mov.b:s	#19h,[3A9h]
	rts
000C0BB9                            04                            .      

;; fn000C0BBA: 000C0BBA
;;   Called from:
;;     000C017A (in fn000C0154)
;;     000C04C9 (in fn000C0154)
fn000C0BBA proc
	bset	[50h]
	bclr	[37h]
	bclr	[3Eh]
	bclr	[3Fh]
	bclr	[36h]
	bclr	[0E2h]
	bclr	[0E1h]
	bset	[0E0h]
	bset	[0E7h]
	bclr	[50h]
	mov.w:q	#0h,[43Ah]
	rts
000C0BE7                      04                                .        

;; fn000C0BE8: 000C0BE8
;;   Called from:
;;     000C018C (in fn000C0154)
;;     000C04DB (in fn000C0154)
fn000C0BE8 proc
	mov.b:s	#40h,[396h]
	mov.b:z	#0h,[397h]
	mov.b:z	#0h,[398h]
	mov.b:z	#0h,[399h]
	mov.b:z	#0h,[39Ah]
	mov.b:z	#0h,[39Bh]
	mov.b:z	#0h,[39Ch]
	mov.b:z	#0h,[39Dh]
	mov.b:z	#0h,[1DBh]
	mov.b:z	#0h,[1DCh]
	mov.b:z	#0h,[1DDh]
	mov.w	#63h,[386h]
	mov.w:q	#0FFFFh,r0
	mov.w	r0,[388h]
	mov.w	r0,[38Ah]
	mov.w	r0,[38Ch]
	mov.w	r0,[38Eh]
	mov.w	r0,[390h]
	mov.w	r0,[392h]
	mov.w	r0,[394h]
	mov.w	r0,[1D0h]
	mov.w	r0,[1D2h]
	mov.w	r0,[1D4h]
	mov.w:s	#384h,a0
	mov.b	[a0],a1
	or.b	#4h,a1
	mov.b	a1,[a0]
	rts
000C0C45                04                                    .          

;; fn000C0C46: 000C0C46
;;   Called from:
;;     000C018F (in fn000C0154)
;;     000C04DE (in fn000C0154)
fn000C0C46 proc
	mov.w:s	#380h,a0
	mov.b	[a0],a1
	or.b	#1h,a1
	mov.b	a1,[a0]
	rts
000C0C51    04                                            .              

;; fn000C0C52: 000C0C52
;;   Called from:
;;     000C0192 (in fn000C0154)
;;     000C04E1 (in fn000C0154)
fn000C0C52 proc
	mov.w:s	#3D6h,a0
	mov.b	[a0],a1
	or.b	#40h,a1
	mov.b	a1,[a0]
	rts
000C0C5D                                        04                    .  

;; fn000C0C5E: 000C0C5E
fn000C0C5E proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	popm	fb,a1,a0,r3,r2,r1,r0
	reit
000C0C65                04                                    .          

;; fn000C0C66: 000C0C66
fn000C0C66 proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	popm	fb,a1,a0,r3,r2,r1,r0
	reit
000C0C6D                                        04                    .  

;; fn000C0C6E: 000C0C6E
fn000C0C6E proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	cmp.w	#41h,[43Ch]
	jeq	000C0CB0

l000C0C7A:
	btst	[1FC0h]
	bmeq	[1F40h]
	btst	[1FC2h]
	bmeq	[1F42h]
	btst	[1FC4h]
	bmeq	[1F44h]
	btst	[1FC1h]
	bmeq	[1F41h]
	btst	[1FC3h]
	bmeq	[1F43h]
	btst	[1FC5h]
	bmeq	[1F45h]

l000C0CB0:
	popm	fb,a1,a0,r3,r2,r1,r0
	reit
000C0CB3          04                                        .            

;; fn000C0CB4: 000C0CB4
fn000C0CB4 proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	cmp.w	#41h,[43Ch]
	jeq	000C0CF6

l000C0CC0:
	btst	[1FC0h]
	bmeq	[1F40h]
	btst	[1FC2h]
	bmeq	[1F42h]
	btst	[1FC4h]
	bmeq	[1F44h]
	btst	[1FC1h]
	bmeq	[1F41h]
	btst	[1FC3h]
	bmeq	[1F43h]
	btst	[1FC5h]
	bmeq	[1F45h]

l000C0CF6:
	popm	fb,a1,a0,r3,r2,r1,r0
	reit
000C0CF9                            04                            .      

;; fn000C0CFA: 000C0CFA
fn000C0CFA proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	cmp.w	#41h,[43Ch]
	jeq	000C0D0F

l000C0D06:
	btst	[1F0Dh]
	bmne	[1F28h]

l000C0D0F:
	popm	fb,a1,a0,r3,r2,r1,r0
	reit
000C0D12       EC                                          .             

;; fn000C0D13: 000C0D13
fn000C0D13 proc
	jsr.a	000764EB
	sub.b:s	#3Ch,[4104h]
	brk
	jeq	000C0D27

l000C0D1E:
	btst	[1F0Eh]
	bmne	[1F29h]

l000C0D27:
	popm	fb,a1,a0,r3,r2,r1,r0
	reit

;; fn000C0D2A: 000C0D2A
fn000C0D2A proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	cmp.w	#41h,[43Ch]
	jeq	000C0D3F

l000C0D36:
	btst	[1F0Fh]
	bmne	[1F2Ah]

l000C0D3F:
	popm	fb,a1,a0,r3,r2,r1,r0
	reit

;; fn000C0D42: 000C0D42
fn000C0D42 proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	popm	fb,a1,a0,r3,r2,r1,r0
	reit
000C0D49                            04 EC FD EB 64 ED BF          ....d..
000C0D50 FB 04 EC FD EB 64 73 FF AE 03 36 04 73 F1 36 04 .....ds...6.s.6.
000C0D60 F5 11 06 D9 1F 00 04 ED BF FB EC FD EB 64 ED BF .............d..
000C0D70 FB 04 EC FD EB 64 77 8F 3C 04 41 00 6A 11 73 F4 .....dw.<.A.j.s.
000C0D80 CA 03 72 4F D8 03 73 F4 CA 03 72 4F DA 03 ED BF ..rO..s...rO....
000C0D90 FB 04 EC FD EB 64 F5 7D 00 ED BF FB EC FD EB 64 .....d.}.......d
000C0DA0 77 8F 3C 04 41 00 6A 0A 7E BF CE 1F 7E 2F 2C 1F w.<.A.j.~...~/,.
000C0DB0 FA ED BF FB                                     ....            

;; fn000C0DB4: 000C0DB4
fn000C0DB4 proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	cmp.w	#41h,[43Ch]
	jeq	000C0DC9

l000C0DC0:
	btst	[1FCDh]
	bmne	[1F2Bh]

l000C0DC9:
	popm	fb,a1,a0,r3,r2,r1,r0
	reit

;; fn000C0DCC: 000C0DCC
fn000C0DCC proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	popm	fb,a1,a0,r3,r2,r1,r0
	reit
000C0DD3          04                                        .            

;; fn000C0DD4: 000C0DD4
fn000C0DD4 proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	popm	fb,a1,a0,r3,r2,r1,r0
	reit
000C0DDB                                  04                        .    

;; fn000C0DDC: 000C0DDC
fn000C0DDC proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	cmp.w	#41h,[43Ch]
	jeq	000C0DF1

l000C0DE8:
	btst	[1FCFh]
	bmne	[1F2Dh]

l000C0DF1:
	popm	fb,a1,a0,r3,r2,r1,r0
	reit

;; fn000C0DF4: 000C0DF4
fn000C0DF4 proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	popm	fb,a1,a0,r3,r2,r1,r0
	reit
000C0DFB                                  04                        .    

;; fn000C0DFC: 000C0DFC
fn000C0DFC proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	popm	fb,a1,a0,r3,r2,r1,r0
	reit
000C0E03          04                                        .            

;; fn000C0E04: 000C0E04
fn000C0E04 proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	popm	fb,a1,a0,r3,r2,r1,r0
	reit
000C0E0B                                  04                        .    

;; fn000C0E0C: 000C0E0C
fn000C0E0C proc
	pushm	fb,a1,a0,r3,r2,r1,r0
	fset	I
	popm	fb,a1,a0,r3,r2,r1,r0
	reit
000C0E13          04                                        .            

;; fn000C0E14: 000C0E14
fn000C0E14 proc
	mov.w	[3C0h],[412h]
	mov.w	[3C2h],[414h]
	mov.w	[3C4h],[416h]
	mov.w	[3C6h],[418h]
	mov.w	[3C8h],[41Ah]
	mov.w	[3CAh],[41Ch]
	mov.w	[3CCh],[41Eh]
	mov.w	[3CEh],[420h]
	rts
000C0E45                04                                    .          

;; fn000C0E46: 000C0E46
;;   Called from:
;;     000C01B8 (in fn000C0154)
;;     000C0507 (in fn000C0154)
fn000C0E46 proc
	mov.w	#1Bh,r1
	jsr.w	UART1_putc
	mov.w	#5Bh,r1
	jsr.w	UART1_putc
	mov.w	#31h,r1
	jsr.w	UART1_putc
	mov.w	#3Bh,r1
	jsr.w	UART1_putc
	mov.w	#32h,r1
	jsr.w	UART1_putc
	mov.w	#34h,r1
	jsr.w	UART1_putc
	mov.w	#72h,r1
	jsr.w	UART1_putc
	mov.w	#1Bh,r1
	jsr.w	UART1_putc
	mov.w	#5Bh,r1
	jsr.w	UART1_putc
	mov.w	#30h,r1
	jsr.w	UART1_putc
	mov.w	#6Dh,r1
	jsr.w	UART1_putc
	mov.w	#1Bh,r1
	jsr.w	UART1_putc
	mov.w	#5Bh,r1
	jsr.w	UART1_putc
	mov.w	#32h,r1
	jsr.w	UART1_putc
	mov.w	#4Ah,r1
	jsr.w	UART1_putc
	rts

;; fn000C0EB0: 000C0EB0
fn000C0EB0 proc
	enter	#16h
	mov.w	r1,-2h[fb]
	mov.w	-2h[fb],r0
	mov.w:q	#0h,r2
	divu.w	#64h
	mov.w	r0,-10h[fb]
	mov.w	-10h[fb],r0
	mul.w	#64h,r0
	sub.w	r0,-2h[fb]
	mov.w	-2h[fb],r0
	mov.w:q	#0h,r2
	divu.w	#0Ah
	mov.w	r0,-12h[fb]
	mov.w	-12h[fb],r0
	mul.w	#0Ah,r0
	sub.w	r0,-2h[fb]
	mov.w	-2h[fb],r0
	add.w	#30h,r0
	mov.w	r0,[424h]
	mov.w	-12h[fb],r0
	add.w	#30h,r0
	mov.w	r0,[426h]
	mov.w	-10h[fb],r0
	add.w	#30h,r0
	mov.w	r0,[428h]
	exitd
000C0F05                04                                    .          

;; fn000C0F06: 000C0F06
;;   Called from:
;;     000C0390 (in fn000C0154)
;;     000C03AF (in fn000C0154)
;;     000C03C7 (in fn000C0154)
;;     000C03DF (in fn000C0154)
;;     000C03F7 (in fn000C0154)
;;     000C040F (in fn000C0154)
;;     000C0451 (in fn000C0154)
;;     000C07E1 (in fn000C0154)
;;     000C07F2 (in fn000C0154)
;;     000C0803 (in fn000C0154)
;;     000C0814 (in fn000C0154)
;;     000C0825 (in fn000C0154)
;;     000C0836 (in fn000C0154)
;;     000C0847 (in fn000C0154)
;;     000C0858 (in fn000C0154)
;;     000C0893 (in fn000C0154)
fn000C0F06 proc
	enter	#16h
	mov.w	r1,-2h[fb]
	mov.w	-2h[fb],r0
	mov.w:q	#0h,r2
	divu.w	#2710h
	mov.w	r0,-0Ch[fb]
	mov.w	-0Ch[fb],r0
	mul.w	#2710h,r0
	sub.w	r0,-2h[fb]
	mov.w	-2h[fb],r0
	mov.w:q	#0h,r2
	divu.w	#3E8h
	mov.w	r0,-0Eh[fb]
	mov.w	-0Eh[fb],r0
	mul.w	#3E8h,r0
	sub.w	r0,-2h[fb]
	mov.w	-2h[fb],r0
	mov.w:q	#0h,r2
	divu.w	#64h
	mov.w	r0,-10h[fb]
	mov.w	-10h[fb],r0
	mul.w	#64h,r0
	sub.w	r0,-2h[fb]
	mov.w	-2h[fb],r0
	mov.w:q	#0h,r2
	divu.w	#0Ah
	mov.w	r0,-12h[fb]
	mov.w	-12h[fb],r0
	mul.w	#0Ah,r0
	sub.w	r0,-2h[fb]
	mov.w	-2h[fb],r0
	add.w	#30h,r0
	mov.w	r0,[424h]
	mov.w	-12h[fb],r0
	add.w	#30h,r0
	mov.w	r0,[426h]
	mov.w	-10h[fb],r0
	add.w	#30h,r0
	mov.w	r0,[428h]
	mov.w	-0Eh[fb],r0
	add.w	#30h,r0
	mov.w	r0,[42Ah]
	mov.w	-0Ch[fb],r0
	add.w	#30h,r0
	mov.w	r0,[42Ch]
	exitd
000C0F9D                                        04                    .  

;; fn000C0F9E: 000C0F9E
fn000C0F9E proc
	enter	#28h
	push.w	#98h
	push.w	#9680h
	mov.w	5h[fb],r0
	mov.w	7h[fb],r2
	jsr.a	fn000C17B4
	add:q	#4h,usp
	mov.w	r0,-8h[fb]
	mov.w	r2,-6h[fb]
	mov.w	#9680h,r0
	mov.w	#98h,r2
	push.w	-6h[fb]
	push.w	-8h[fb]
	jsr.a	fn000C1802
	add:q	#4h,usp
	sub.w	r0,5h[fb]
	sbb.w	r2,7h[fb]
	push.w	#0Fh
	push.w	#4240h
	mov.w	5h[fb],r0
	mov.w	7h[fb],r2
	jsr.a	fn000C17B4
	add:q	#4h,usp
	mov.w	r0,-0Ch[fb]
	mov.w	r2,-0Ah[fb]
	mov.w	#4240h,r0
	mov.w	#0Fh,r2
	push.w	-0Ah[fb]
	push.w	-0Ch[fb]
	jsr.a	fn000C1802
	add:q	#4h,usp
	sub.w	r0,5h[fb]
	sbb.w	r2,7h[fb]
	push.w	#1h
	push.w	#86A0h
	mov.w	5h[fb],r0
	mov.w	7h[fb],r2
	jsr.a	fn000C17B4
	add:q	#4h,usp
	mov.w	r0,-10h[fb]
	mov.w	r2,-0Eh[fb]
	mov.w	#86A0h,r0
	mov.w:q	#1h,r2
	push.w	-0Eh[fb]
	push.w	-10h[fb]
	jsr.a	fn000C1802
	add:q	#4h,usp
	sub.w	r0,5h[fb]
	sbb.w	r2,7h[fb]
	push.w	#0h
	push.w	#2710h
	mov.w	5h[fb],r0
	mov.w	7h[fb],r2
	jsr.a	fn000C17B4
	add:q	#4h,usp
	mov.w	r0,-14h[fb]
	mov.w	r2,-12h[fb]
	mov.w	#2710h,r0
	mov.w:q	#0h,r2
	push.w	-12h[fb]
	push.w	-14h[fb]
	jsr.a	fn000C1802
	add:q	#4h,usp
	sub.w	r0,5h[fb]
	sbb.w	r2,7h[fb]
	push.w	#0h
	push.w	#3E8h
	mov.w	5h[fb],r0
	mov.w	7h[fb],r2
	jsr.a	fn000C17B4
	add:q	#4h,usp
	mov.w	r0,-18h[fb]
	mov.w	r2,-16h[fb]
	mov.w	#3E8h,r0
	mov.w:q	#0h,r2
	push.w	-16h[fb]
	push.w	-18h[fb]
	jsr.a	fn000C1802
	add:q	#4h,usp
	sub.w	r0,5h[fb]
	sbb.w	r2,7h[fb]
	push.w	#0h
	push.w	#64h
	mov.w	5h[fb],r0
	mov.w	7h[fb],r2
	jsr.a	fn000C17B4
	add:q	#4h,usp
	mov.w	r0,-1Ch[fb]
	mov.w	r2,-1Ah[fb]
	mov.w	#64h,r0
	mov.w:q	#0h,r2
	push.w	-1Ah[fb]
	push.w	-1Ch[fb]
	jsr.a	fn000C1802
	add:q	#4h,usp
	sub.w	r0,5h[fb]
	sbb.w	r2,7h[fb]
	push.w	#0h
	push.w	#0Ah
	mov.w	5h[fb],r0
	mov.w	7h[fb],r2
	jsr.a	fn000C17B4
	add:q	#4h,usp
	mov.w	r0,-20h[fb]
	mov.w	r2,-1Eh[fb]
	mov.w	#0Ah,r0
	mov.w:q	#0h,r2
	push.w	-1Eh[fb]
	push.w	-20h[fb]
	jsr.a	fn000C1802
	add:q	#4h,usp
	sub.w	r0,5h[fb]
	sbb.w	r2,7h[fb]
	mov.w	5h[fb],r0
	mov.w	7h[fb],r2
	add.w	#30h,r0
	adcf.w	r2
	mov.w	r0,[424h]
	mov.w	-20h[fb],r0
	mov.w	-1Eh[fb],r2
	add.w	#30h,r0
	adcf.w	r2
	mov.w	r0,[426h]
	mov.w	-1Ch[fb],r0
	mov.w	-1Ah[fb],r2
	add.w	#30h,r0
	adcf.w	r2
	mov.w	r0,[428h]
	mov.w	-18h[fb],r0
	mov.w	-16h[fb],r2
	add.w	#30h,r0
	adcf.w	r2
	mov.w	r0,[42Ah]
	mov.w	-14h[fb],r0
	mov.w	-12h[fb],r2
	add.w	#30h,r0
	adcf.w	r2
	mov.w	r0,[42Ch]
	mov.w	-10h[fb],r0
	mov.w	-0Eh[fb],r2
	add.w	#30h,r0
	adcf.w	r2
	mov.w	r0,[42Eh]
	mov.w	-0Ch[fb],r0
	mov.w	-0Ah[fb],r2
	add.w	#30h,r0
	adcf.w	r2
	mov.w	r0,[430h]
	mov.w	-8h[fb],r0
	mov.w	-6h[fb],r2
	add.w	#30h,r0
	adcf.w	r2
	mov.w	r0,[432h]
	exitd
000C1185                04                                    .          

;; fn000C1186: 000C1186
;;   Called from:
;;     000C01BF (in fn000C0154)
;;     000C01EF (in fn000C0154)
;;     000C0203 (in fn000C0154)
;;     000C0247 (in fn000C0154)
;;     000C0429 (in fn000C0154)
;;     000C050E (in fn000C0154)
;;     000C05B2 (in fn000C0154)
;;     000C05C6 (in fn000C0154)
;;     000C0675 (in fn000C0154)
;;     000C086B (in fn000C0154)
fn000C1186 proc
	enter	#0Ah
	mov.w	r1,-8h[fb]
	mov.w	r2,-2h[fb]
	cmp.w	#18h,-8h[fb]
	jleu	000C119B

l000C1196:
	mov.w	#18h,-8h[fb]

l000C119B:
	mov.w	-8h[fb],r0
	mov.w:q	#0h,r2
	divu.w	#0Ah
	mov.w	r0,-4h[fb]
	mov.w	-4h[fb],r0
	mul.w	#0Ah,r0
	mov.w	-8h[fb],-6h[fb]
	sub.w	r0,-6h[fb]
	cmp.w	#4Fh,-2h[fb]
	jleu	000C11C1

l000C11BC:
	mov.w	#4Fh,-2h[fb]

l000C11C1:
	mov.w	-2h[fb],r0
	mov.w:q	#0h,r2
	divu.w	#0Ah
	mov.w	r0,-8h[fb]
	mov.w	-8h[fb],r0
	mul.w	#0Ah,r0
	mov.w	-2h[fb],-0Ah[fb]
	sub.w	r0,-0Ah[fb]
	mov.w	#1Bh,r1
	jsr.w	UART1_putc
	mov.w	#5Bh,r1
	jsr.w	UART1_putc
	mov.w	-4h[fb],r1
	add.w	#30h,r1
	jsr.w	UART1_putc
	mov.w	-6h[fb],r1
	add.w	#30h,r1
	jsr.w	UART1_putc
	mov.w	#3Bh,r1
	jsr.w	UART1_putc
	mov.w	-8h[fb],r1
	add.w	#30h,r1
	jsr.w	UART1_putc
	mov.w	-0Ah[fb],r1
	add.w	#30h,r1
	jsr.w	UART1_putc
	mov.w	#48h,r1
	jsr.w	UART1_putc
	exitd
000C1221    04                                            .              

;; fn000C1222: 000C1222
;;   Called from:
;;     000C01CA (in fn000C0154)
;;     000C01D7 (in fn000C0154)
;;     000C01E4 (in fn000C0154)
;;     000C01FA (in fn000C0154)
;;     000C058D (in fn000C0154)
;;     000C059A (in fn000C0154)
;;     000C05A7 (in fn000C0154)
;;     000C05BD (in fn000C0154)
fn000C1222 proc
	enter	#2h
	mov.w:q	#1h,-2h[fb]

l000C1228:
	cmp.w	#64h,-2h[fb]
	jleu	000C1232

l000C122F:
	jmp.w	000C1370

l000C1232:
	mov.w	5h[fb],a0
	mov.w	7h[fb],a1
	lde.b	[a1a0],r0l
	cmp.b:s	#7Eh,r0l
	jeq	000C1241

l000C123E:
	jmp.w	000C1344

l000C1241:
	add.w:q	#1h,5h[fb]
	adcf.w	7h[fb]
	mov.w	5h[fb],a0
	mov.w	7h[fb],a1
	lde.b	[a1a0],r0l
	cmp.b:s	#4Eh,r0l
	jne	000C1261

l000C1253:
	mov.w	#0Ah,r1
	jsr.w	UART1_putc
	mov.w	#0Dh,r1
	jsr.w	UART1_putc

l000C1261:
	mov.w	5h[fb],a0
	mov.w	7h[fb],a1
	lde.b	[a1a0],r0l
	cmp.b:s	#7Eh,r0l
	jne	000C1274

l000C126D:
	mov.w	#7Eh,r1
	jsr.w	UART1_putc

l000C1274:
	mov.w	5h[fb],a0
	mov.w	7h[fb],a1
	lde.b	[a1a0],r0l
	cmp.b:s	#42h,r0l
	jne	000C129C

l000C1280:
	mov.w	#1Bh,r1
	jsr.w	UART1_putc
	mov.w	#5Bh,r1
	jsr.w	UART1_putc
	mov.w	#31h,r1
	jsr.w	UART1_putc
	mov.w	#6Dh,r1
	jsr.w	UART1_putc

l000C129C:
	mov.w	5h[fb],a0
	mov.w	7h[fb],a1
	lde.b	[a1a0],r0l
	cmp.b:s	#55h,r0l
	jne	000C12C4

l000C12A8:
	mov.w	#1Bh,r1
	jsr.w	UART1_putc
	mov.w	#5Bh,r1
	jsr.w	UART1_putc
	mov.w	#34h,r1
	jsr.w	UART1_putc
	mov.w	#6Dh,r1
	jsr.w	UART1_putc

l000C12C4:
	mov.w	5h[fb],a0
	mov.w	7h[fb],a1
	lde.b	[a1a0],r0l
	cmp.b:s	#52h,r0l
	jne	000C12EC

l000C12D0:
	mov.w	#1Bh,r1
	jsr.w	UART1_putc
	mov.w	#5Bh,r1
	jsr.w	UART1_putc
	mov.w	#37h,r1
	jsr.w	UART1_putc
	mov.w	#6Dh,r1
	jsr.w	UART1_putc

l000C12EC:
	mov.w	5h[fb],a0
	mov.w	7h[fb],a1
	lde.b	[a1a0],r0l
	cmp.b:s	#4Fh,r0l
	jne	000C1314

l000C12F8:
	mov.w	#1Bh,r1
	jsr.w	UART1_putc
	mov.w	#5Bh,r1
	jsr.w	UART1_putc
	mov.w	#30h,r1
	jsr.w	UART1_putc
	mov.w	#6Dh,r1
	jsr.w	UART1_putc

l000C1314:
	mov.w	5h[fb],a0
	mov.w	7h[fb],a1
	lde.b	[a1a0],r0l
	cmp.b:s	#53h,r0l
	jne	000C133C

l000C1320:
	mov.w	#1Bh,r1
	jsr.w	UART1_putc
	mov.w	#5Bh,r1
	jsr.w	UART1_putc
	mov.w	#35h,r1
	jsr.w	UART1_putc
	mov.w	#6Dh,r1
	jsr.w	UART1_putc

l000C133C:
	add.w:q	#1h,5h[fb]
	adcf.w	7h[fb]
	jmp.b	000C136A

l000C1344:
	mov.w	5h[fb],a0
	mov.w	7h[fb],a1
	lde.b	[a1a0],r0l
	jne	000C1355

l000C134E:
	mov.w	#64h,-2h[fb]
	jmp.b	000C136A

l000C1355:
	mov.w	5h[fb],a0
	mov.w	7h[fb],a1
	lde.b	[a1a0],r0l
	mov.b	r0l,r1l
	mov.b:q	#0h,r1h
	jsr.w	UART1_putc
	add.w:q	#1h,5h[fb]
	adcf.w	7h[fb]

l000C136A:
	add.w:q	#1h,-2h[fb]
	jmp.w	000C1228

l000C1370:
	exitd

;; UART1_putc: 000C1372
;;   Called from:
;;     000C0199 (in fn000C0154)
;;     000C01A0 (in fn000C0154)
;;     000C01A7 (in fn000C0154)
;;     000C01AE (in fn000C0154)
;;     000C01B5 (in fn000C0154)
;;     000C024E (in fn000C0154)
;;     000C0255 (in fn000C0154)
;;     000C026A (in fn000C0154)
;;     000C0271 (in fn000C0154)
;;     000C0278 (in fn000C0154)
;;     000C027F (in fn000C0154)
;;     000C0286 (in fn000C0154)
;;     000C029B (in fn000C0154)
;;     000C02A2 (in fn000C0154)
;;     000C02A9 (in fn000C0154)
;;     000C02B0 (in fn000C0154)
;;     000C02B7 (in fn000C0154)
;;     000C02CC (in fn000C0154)
;;     000C02D3 (in fn000C0154)
;;     000C02DA (in fn000C0154)
;;     000C02E1 (in fn000C0154)
;;     000C02E8 (in fn000C0154)
;;     000C02EF (in fn000C0154)
;;     000C02F6 (in fn000C0154)
;;     000C030B (in fn000C0154)
;;     000C0312 (in fn000C0154)
;;     000C0319 (in fn000C0154)
;;     000C0320 (in fn000C0154)
;;     000C0327 (in fn000C0154)
;;     000C032E (in fn000C0154)
;;     000C0335 (in fn000C0154)
;;     000C033C (in fn000C0154)
;;     000C0343 (in fn000C0154)
;;     000C034A (in fn000C0154)
;;     000C035F (in fn000C0154)
;;     000C0366 (in fn000C0154)
;;     000C036D (in fn000C0154)
;;     000C0374 (in fn000C0154)
;;     000C037B (in fn000C0154)
;;     000C0382 (in fn000C0154)
;;     000C0389 (in fn000C0154)
;;     000C039A (in fn000C0154)
;;     000C03A1 (in fn000C0154)
;;     000C03A8 (in fn000C0154)
;;     000C03B9 (in fn000C0154)
;;     000C03C0 (in fn000C0154)
;;     000C03D1 (in fn000C0154)
;;     000C03D8 (in fn000C0154)
;;     000C03E9 (in fn000C0154)
;;     000C03F0 (in fn000C0154)
;;     000C0401 (in fn000C0154)
;;     000C0408 (in fn000C0154)
;;     000C0419 (in fn000C0154)
;;     000C0420 (in fn000C0154)
;;     000C0442 (in fn000C0154)
;;     000C045B (in fn000C0154)
;;     000C0462 (in fn000C0154)
;;     000C0469 (in fn000C0154)
;;     000C047E (in fn000C0154)
;;     000C0485 (in fn000C0154)
;;     000C048C (in fn000C0154)
;;     000C0493 (in fn000C0154)
;;     000C049A (in fn000C0154)
;;     000C04A1 (in fn000C0154)
;;     000C04B6 (in fn000C0154)
;;     000C04C0 (in fn000C0154)
;;     000C04E8 (in fn000C0154)
;;     000C04EF (in fn000C0154)
;;     000C04F6 (in fn000C0154)
;;     000C04FD (in fn000C0154)
;;     000C0504 (in fn000C0154)
;;     000C067C (in fn000C0154)
;;     000C0691 (in fn000C0154)
;;     000C0698 (in fn000C0154)
;;     000C06AD (in fn000C0154)
;;     000C06B4 (in fn000C0154)
;;     000C06C9 (in fn000C0154)
;;     000C06D0 (in fn000C0154)
;;     000C06E5 (in fn000C0154)
;;     000C06EC (in fn000C0154)
;;     000C0701 (in fn000C0154)
;;     000C0708 (in fn000C0154)
;;     000C071D (in fn000C0154)
;;     000C0724 (in fn000C0154)
;;     000C072B (in fn000C0154)
;;     000C0740 (in fn000C0154)
;;     000C0747 (in fn000C0154)
;;     000C075C (in fn000C0154)
;;     000C0763 (in fn000C0154)
;;     000C0778 (in fn000C0154)
;;     000C077F (in fn000C0154)
;;     000C0794 (in fn000C0154)
;;     000C079B (in fn000C0154)
;;     000C07B0 (in fn000C0154)
;;     000C07B7 (in fn000C0154)
;;     000C07CC (in fn000C0154)
;;     000C07D3 (in fn000C0154)
;;     000C07DA (in fn000C0154)
;;     000C07EB (in fn000C0154)
;;     000C07FC (in fn000C0154)
;;     000C080D (in fn000C0154)
;;     000C081E (in fn000C0154)
;;     000C082F (in fn000C0154)
;;     000C0840 (in fn000C0154)
;;     000C0851 (in fn000C0154)
;;     000C0862 (in fn000C0154)
;;     000C0884 (in fn000C0154)
;;     000C089D (in fn000C0154)
;;     000C08A4 (in fn000C0154)
;;     000C08AB (in fn000C0154)
;;     000C08C0 (in fn000C0154)
;;     000C08C7 (in fn000C0154)
;;     000C08CE (in fn000C0154)
;;     000C08D5 (in fn000C0154)
;;     000C08DC (in fn000C0154)
;;     000C08E3 (in fn000C0154)
;;     000C08F8 (in fn000C0154)
;;     000C08FF (in fn000C0154)
;;     000C0906 (in fn000C0154)
;;     000C090D (in fn000C0154)
;;     000C0914 (in fn000C0154)
;;     000C091B (in fn000C0154)
;;     000C0930 (in fn000C0154)
;;     000C0937 (in fn000C0154)
;;     000C093E (in fn000C0154)
;;     000C0945 (in fn000C0154)
;;     000C094C (in fn000C0154)
;;     000C0953 (in fn000C0154)
;;     000C0968 (in fn000C0154)
;;     000C096F (in fn000C0154)
;;     000C0E4A (in fn000C0E46)
;;     000C0E51 (in fn000C0E46)
;;     000C0E58 (in fn000C0E46)
;;     000C0E5F (in fn000C0E46)
;;     000C0E66 (in fn000C0E46)
;;     000C0E6D (in fn000C0E46)
;;     000C0E74 (in fn000C0E46)
;;     000C0E7B (in fn000C0E46)
;;     000C0E82 (in fn000C0E46)
;;     000C0E89 (in fn000C0E46)
;;     000C0E90 (in fn000C0E46)
;;     000C0E97 (in fn000C0E46)
;;     000C0E9E (in fn000C0E46)
;;     000C0EA5 (in fn000C0E46)
;;     000C0EAC (in fn000C0E46)
;;     000C11DF (in fn000C1186)
;;     000C11E6 (in fn000C1186)
;;     000C11F0 (in fn000C1186)
;;     000C11FA (in fn000C1186)
;;     000C1201 (in fn000C1186)
;;     000C120B (in fn000C1186)
;;     000C1215 (in fn000C1186)
;;     000C121C (in fn000C1186)
;;     000C1257 (in fn000C1222)
;;     000C125E (in fn000C1222)
;;     000C1271 (in fn000C1222)
;;     000C1284 (in fn000C1222)
;;     000C128B (in fn000C1222)
;;     000C1292 (in fn000C1222)
;;     000C1299 (in fn000C1222)
;;     000C12AC (in fn000C1222)
;;     000C12B3 (in fn000C1222)
;;     000C12BA (in fn000C1222)
;;     000C12C1 (in fn000C1222)
;;     000C12D4 (in fn000C1222)
;;     000C12DB (in fn000C1222)
;;     000C12E2 (in fn000C1222)
;;     000C12E9 (in fn000C1222)
;;     000C12FC (in fn000C1222)
;;     000C1303 (in fn000C1222)
;;     000C130A (in fn000C1222)
;;     000C1311 (in fn000C1222)
;;     000C1324 (in fn000C1222)
;;     000C132B (in fn000C1222)
;;     000C1332 (in fn000C1222)
;;     000C1339 (in fn000C1222)
;;     000C1361 (in fn000C1222)
;;     000C1396 (in fn000C1392)
;;     000C139D (in fn000C1392)
;;     000C13A4 (in fn000C1392)
;;     000C13AB (in fn000C1392)
;;     000C13B2 (in fn000C1392)
;;     000C13B9 (in fn000C1392)
;;     000C13C0 (in fn000C1392)
;;     000C13C7 (in fn000C1392)
;;     000C1412 (in fn000C13CC)
;;     000C1419 (in fn000C13CC)
;;     000C1420 (in fn000C13CC)
;;     000C1427 (in fn000C13CC)
;;     000C142E (in fn000C13CC)
;;     000C1478 (in fn000C1432)
;;     000C147F (in fn000C1432)
;;     000C1486 (in fn000C1432)
;;     000C148D (in fn000C1432)
;;     000C1494 (in fn000C1432)
;;     000C149B (in fn000C1432)
;;     000C14A4 (in fn000C14A0)
;;     000C14AB (in fn000C14A0)
;;     000C14B2 (in fn000C14A0)
;;     000C14B9 (in fn000C14A0)
;;     000C14C0 (in fn000C14A0)
;;     000C14C8 (in fn000C14C4)
;;     000C14CF (in fn000C14C4)
;;     000C14D6 (in fn000C14C4)
;;     000C14DD (in fn000C14C4)
;;     000C14E4 (in fn000C14C4)
;;     000C14EB (in fn000C14C4)
;;     000C1536 (in fn000C14F0)
;;     000C153D (in fn000C14F0)
;;     000C1544 (in fn000C14F0)
;;     000C154B (in fn000C14F0)
;;     000C1552 (in fn000C14F0)
;;     000C1559 (in fn000C14F0)
;;     000C1562 (in fn000C155E)
;;     000C1569 (in fn000C155E)
;;     000C1570 (in fn000C155E)
;;     000C1577 (in fn000C155E)
;;     000C157E (in fn000C155E)
;;     000C1585 (in fn000C155E)
;;     000C158E (in fn000C158A)
;;     000C1595 (in fn000C158A)
;;     000C159C (in fn000C158A)
;;     000C15A3 (in fn000C158A)
;;     000C15AA (in fn000C158A)
;;     000C15B1 (in fn000C158A)
;;     000C15FC (in fn000C15B6)
;;     000C1603 (in fn000C15B6)
;;     000C160A (in fn000C15B6)
;;     000C1611 (in fn000C15B6)
;;     000C161A (in fn000C1616)
;;     000C1621 (in fn000C1616)
;;     000C1628 (in fn000C1616)
;;     000C162F (in fn000C1616)
;;     000C165C (in fn000C1634)
;;     000C1663 (in fn000C1634)
;;     000C166A (in fn000C1634)
;;     000C1672 (in fn000C166E)
;;     000C1679 (in fn000C166E)
;;     000C1680 (in fn000C166E)
;;     000C1696 (in fn000C1684)
;;     000C169D (in fn000C1684)
;;     000C16A4 (in fn000C1684)
;;     000C16AB (in fn000C1684)
;;     000C1710 (in fn000C16B0)
;;     000C1717 (in fn000C16B0)
;;     000C171E (in fn000C16B0)
;;     000C1725 (in fn000C16B0)
;;     000C172C (in fn000C16B0)
;;     000C1742 (in fn000C1730)
;;     000C1749 (in fn000C1730)
;;     000C1750 (in fn000C1730)
;;     000C1757 (in fn000C1730)
;;     000C176E (in fn000C175C)
;;     000C1775 (in fn000C175C)
;;     000C177E (in fn000C177A)
;;     000C1785 (in fn000C177A)
;;     000C178C (in fn000C177A)
;;     000C1794 (in fn000C1790)
;;     000C179B (in fn000C1790)
;;     000C17A2 (in fn000C1790)
;;     000C17A9 (in fn000C1790)
;;     000C17B0 (in fn000C1790)
UART1_putc proc
	enter	#2h
	mov.w	r1,-2h[fb]

l000C1378:
	mov.b:s	[3ADh],r0l
	mov.b:z	#0h,r0h
	btst	#1h,r0
	jeq	000C1378

l000C1381:
	mov.b:s	[3ADh],a0
	or.b	#1h,a0
	mov.b	a0,[3ADh]
	mov.w	-2h[fb],[3AAh]
	exitd

;; fn000C1392: 000C1392
fn000C1392 proc
	mov.w	[432h],r1
	jsr.w	UART1_putc
	mov.w	[430h],r1
	jsr.w	UART1_putc
	mov.w	[42Eh],r1
	jsr.w	UART1_putc
	mov.w	[42Ch],r1
	jsr.w	UART1_putc
	mov.w	[42Ah],r1
	jsr.w	UART1_putc
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts
000C13CB                                  04                        .    

;; fn000C13CC: 000C13CC
;;   Called from:
;;     000C0454 (in fn000C0154)
;;     000C087D (in fn000C0154)
;;     000C0896 (in fn000C0154)
fn000C13CC proc
	cmp.w	#30h,[42Ch]
	jne	000C13DA

l000C13D4:
	mov.w	#20h,[42Ch]

l000C13DA:
	cmp.w	#20h,[42Ch]
	jne	000C13F0

l000C13E2:
	cmp.w	#30h,[42Ah]
	jne	000C13F0

l000C13EA:
	mov.w	#20h,[42Ah]

l000C13F0:
	cmp.w	#20h,[42Ch]
	jne	000C140E

l000C13F8:
	cmp.w	#20h,[42Ah]
	jne	000C140E

l000C1400:
	cmp.w	#30h,[428h]
	jne	000C140E

l000C1408:
	mov.w	#20h,[428h]

l000C140E:
	mov.w	[42Ch],r1
	jsr.w	UART1_putc
	mov.w	[42Ah],r1
	jsr.w	UART1_putc
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts

;; fn000C1432: 000C1432
fn000C1432 proc
	cmp.w	#30h,[42Ch]
	jne	000C1440

l000C143A:
	mov.w	#20h,[42Ch]

l000C1440:
	cmp.w	#20h,[42Ch]
	jne	000C1456

l000C1448:
	cmp.w	#30h,[42Ah]
	jne	000C1456

l000C1450:
	mov.w	#20h,[42Ah]

l000C1456:
	cmp.w	#20h,[42Ch]
	jne	000C1474

l000C145E:
	cmp.w	#20h,[42Ah]
	jne	000C1474

l000C1466:
	cmp.w	#30h,[428h]
	jne	000C1474

l000C146E:
	mov.w	#20h,[428h]

l000C1474:
	mov.w	[42Ch],r1
	jsr.w	UART1_putc
	mov.w	[42Ah],r1
	jsr.w	UART1_putc
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	#2Eh,r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts
000C149F                                              04                .

;; fn000C14A0: 000C14A0
fn000C14A0 proc
	mov.w	[42Ch],r1
	jsr.w	UART1_putc
	mov.w	[42Ah],r1
	jsr.w	UART1_putc
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts

;; fn000C14C4: 000C14C4
fn000C14C4 proc
	mov.w	[42Ch],r1
	jsr.w	UART1_putc
	mov.w	[42Ah],r1
	jsr.w	UART1_putc
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	#2Eh,r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts
000C14EF                                              04                .

;; fn000C14F0: 000C14F0
fn000C14F0 proc
	cmp.w	#30h,[42Ch]
	jne	000C14FE

l000C14F8:
	mov.w	#20h,[42Ch]

l000C14FE:
	cmp.w	#20h,[42Ch]
	jne	000C1514

l000C1506:
	cmp.w	#30h,[42Ah]
	jne	000C1514

l000C150E:
	mov.w	#20h,[42Ah]

l000C1514:
	cmp.w	#20h,[42Ch]
	jne	000C1532

l000C151C:
	cmp.w	#20h,[42Ah]
	jne	000C1532

l000C1524:
	cmp.w	#30h,[428h]
	jne	000C1532

l000C152C:
	mov.w	#20h,[428h]

l000C1532:
	mov.w	[42Ch],r1
	jsr.w	UART1_putc
	mov.w	[42Ah],r1
	jsr.w	UART1_putc
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	#2Eh,r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts
000C155D                                        04                    .  

;; fn000C155E: 000C155E
fn000C155E proc
	mov.w	[42Ch],r1
	jsr.w	UART1_putc
	mov.w	[42Ah],r1
	jsr.w	UART1_putc
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	#2Eh,r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts
000C1589                            04                            .      

;; fn000C158A: 000C158A
fn000C158A proc
	mov.w	[42Ch],r1
	jsr.w	UART1_putc
	mov.w	[42Ah],r1
	jsr.w	UART1_putc
	mov.w	#2Eh,r1
	jsr.w	UART1_putc
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts
000C15B5                04                                    .          

;; fn000C15B6: 000C15B6
fn000C15B6 proc
	cmp.w	#30h,[42Ah]
	jne	000C15C4

l000C15BE:
	mov.w	#20h,[42Ah]

l000C15C4:
	cmp.w	#20h,[42Ah]
	jne	000C15DA

l000C15CC:
	cmp.w	#30h,[428h]
	jne	000C15DA

l000C15D4:
	mov.w	#20h,[428h]

l000C15DA:
	cmp.w	#20h,[42Ah]
	jne	000C15F8

l000C15E2:
	cmp.w	#20h,[428h]
	jne	000C15F8

l000C15EA:
	cmp.w	#30h,[426h]
	jne	000C15F8

l000C15F2:
	mov.w	#20h,[426h]

l000C15F8:
	mov.w	[42Ah],r1
	jsr.w	UART1_putc
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts
000C1615                04                                    .          

;; fn000C1616: 000C1616
;;   Called from:
;;     000C0393 (in fn000C0154)
;;     000C03B2 (in fn000C0154)
;;     000C03CA (in fn000C0154)
;;     000C03E2 (in fn000C0154)
;;     000C03FA (in fn000C0154)
;;     000C0412 (in fn000C0154)
;;     000C07E4 (in fn000C0154)
;;     000C07F5 (in fn000C0154)
;;     000C0806 (in fn000C0154)
;;     000C0817 (in fn000C0154)
;;     000C0828 (in fn000C0154)
;;     000C0839 (in fn000C0154)
;;     000C084A (in fn000C0154)
;;     000C085B (in fn000C0154)
fn000C1616 proc
	mov.w	[42Ah],r1
	jsr.w	UART1_putc
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts
000C1633          04                                        .            

;; fn000C1634: 000C1634
fn000C1634 proc
	cmp.w	#30h,[428h]
	jne	000C1642

l000C163C:
	mov.w	#20h,[428h]

l000C1642:
	cmp.w	#20h,[428h]
	jne	000C1658

l000C164A:
	cmp.w	#30h,[426h]
	jne	000C1658

l000C1652:
	mov.w	#20h,[426h]

l000C1658:
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts

;; fn000C166E: 000C166E
fn000C166E proc
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts

;; fn000C1684: 000C1684
fn000C1684 proc
	cmp.w	#30h,[428h]
	jne	000C1692

l000C168C:
	mov.w	#20h,[428h]

l000C1692:
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	#2Eh,r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts
000C16AF                                              04                .

;; fn000C16B0: 000C16B0
fn000C16B0 proc
	cmp.w	#30h,[42Ah]
	jne	000C16BE

l000C16B8:
	mov.w	#20h,[42Ah]

l000C16BE:
	cmp.w	#20h,[42Ah]
	jne	000C16D4

l000C16C6:
	cmp.w	#30h,[428h]
	jne	000C16D4

l000C16CE:
	mov.w	#20h,[428h]

l000C16D4:
	cmp.w	#2Dh,[42Ah]
	jne	000C16F0

l000C16DC:
	cmp.w	#30h,[428h]
	jne	000C16F0

l000C16E4:
	mov.w	#20h,[42Ah]
	mov.w	#2Dh,[428h]

l000C16F0:
	cmp.w	#2Bh,[42Ah]
	jne	000C170C

l000C16F8:
	cmp.w	#30h,[428h]
	jne	000C170C

l000C1700:
	mov.w	#20h,[42Ah]
	mov.w	#2Bh,[428h]

l000C170C:
	mov.w	[42Ah],r1
	jsr.w	UART1_putc
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	#2Eh,r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts

;; fn000C1730: 000C1730
fn000C1730 proc
	cmp.w	#30h,[428h]
	jne	000C173E

l000C1738:
	mov.w	#20h,[428h]

l000C173E:
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	#2Eh,r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts
000C175B                                  04                        .    

;; fn000C175C: 000C175C
fn000C175C proc
	cmp.w	#30h,[426h]
	jne	000C176A

l000C1764:
	mov.w	#20h,[426h]

l000C176A:
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts
000C1779                            04                            .      

;; fn000C177A: 000C177A
fn000C177A proc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	#2Eh,r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts

;; fn000C1790: 000C1790
fn000C1790 proc
	mov.w	[42Ah],r1
	jsr.w	UART1_putc
	mov.w	[428h],r1
	jsr.w	UART1_putc
	mov.w	[426h],r1
	jsr.w	UART1_putc
	mov.w	#2Eh,r1
	jsr.w	UART1_putc
	mov.w	[424h],r1
	jsr.w	UART1_putc
	rts

;; fn000C17B4: 000C17B4
;;   Called from:
;;     000C0FAF (in fn000C0F9E)
;;     000C0FE3 (in fn000C0F9E)
;;     000C1017 (in fn000C0F9E)
;;     000C1049 (in fn000C0F9E)
;;     000C107B (in fn000C0F9E)
;;     000C10AD (in fn000C0F9E)
;;     000C10DF (in fn000C0F9E)
fn000C17B4 proc
	pushm	a1,a0,r3,r1
	mov.w:q	#0h,a0
	mov.w:q	#0h,a1
	mov.w	0Bh[sb],r1
	mov.w	0Dh[sb],r3
	jne	000C17D8

l000C17C2:
	cmp.w:q	#0h,r2
	jeq	000C17D2

l000C17C6:
	mov.w	r0,a1
	mov.w	r2,r0
	mov.w:q	#0h,r2
	divu.w	r1
	mov.w	r0,r3
	mov.w	a1,r0

l000C17D2:
	divu.w	r1
	mov.w	r3,r2
	jmp.b	000C17FE

l000C17D8:
	jn	000C17E5

l000C17DA:
	cmp.w	r3,r2
	jleu	000C17E5

l000C17DE:
	inc.w	a1
	shl:q	#0h,r1
	rolc.w	r3
	jpz	000C17DA

l000C17E5:
	sub.w	r1,r0
	sbb.w	r3,r2
	jgeu	000C17F1

l000C17EB:
	add.w	r1,r0
	adc.w	r3,r2
	fclr	C

l000C17F1:
	rolc.w	a0
	shl:q	#-8h,r3
	rorc.w	r1
	dec.w	a1
	jpz	000C17E5

l000C17FA:
	mov.w	a0,r0
	mov.w:q	#0h,r2

l000C17FE:
	popm	a1,a0,r3,r1
	rts
