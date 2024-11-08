;;; Segment seg000C1802 (000C1802)

;; fn000C1802: 000C1802
;;   Called from:
;;     000C0FC9 (in fn000C0F9E)
;;     000C0FFD (in fn000C0F9E)
;;     000C102F (in fn000C0F9E)
;;     000C1061 (in fn000C0F9E)
;;     000C1093 (in fn000C0F9E)
;;     000C10C5 (in fn000C0F9E)
;;     000C10F7 (in fn000C0F9E)
fn000C1802 proc
	pushm	r3,r1
	mov.w	7h[sb],r1
	mulu.w	r2,r1
	mov.w	r1,r2
	mov.w	9h[sb],r1
	mulu.w	r0,r1
	add.w	r1,r2
	mov.w	r2,r3
	mov.w	7h[sb],r1
	mulu.w	r1,r0
	add.w	r3,r2
	popm	r3,r1
	rts

;; fn000C181E: 000C181E
fn000C181E proc
	enter	#8h
	mov.w	r1,-2h[fb]
	mov.w	r2,-4h[fb]
	mov.w	-2h[fb],r1
	mul.w	-4h[fb],r1
	jsr.w	fn000C1894
	mov.w	r0,-8h[fb]
	mov.w	r2,-6h[fb]
	cmp.w:q	#0h,r0
	jne	000C183E

l000C183A:
	cmp.w:q	#0h,r2
	jeq	000C1854

l000C183E:
	mov.w	-2h[fb],r0
	mul.w	-4h[fb],r0
	push.w	r0
	mov.w:q	#0h,r2
	push.w	-6h[fb]
	push.w	-8h[fb]
	jsr.a	fn000C1DB0
	add:q	#6h,usp

l000C1854:
	mov.w	-8h[fb],r0
	mov.w	-6h[fb],r2
	exitd

;; fn000C185C: 000C185C
;;   Called from:
;;     000C194C (in fn000C18E6)
fn000C185C proc
	enter	#4h
	cmp.w:q	#0h,5h[fb]
	jne	000C186B

l000C1864:
	cmp.w:q	#0h,7h[fb]
	jne	000C186B

l000C1869:
	exitd

l000C186B:
	mov.w	5h[fb],-4h[fb]
	mov.w	7h[fb],-2h[fb]
	add.w:q	#0FFF8h,-4h[fb]
	sbb.w	#0h,-2h[fb]
	mov.w	-4h[fb],a0
	mov.w	-2h[fb],a1
	add.w:q	#4h,a0
	adcf.w	a1
	lde.w	[a1a0],r2
	shl:q	#2h,r2
	push.w	-2h[fb]
	push.w	-4h[fb]
	jsr.w	fn000C1ACC
	exitd

;; fn000C1894: 000C1894
;;   Called from:
;;     000C182D (in fn000C181E)
;;     000C18EE (in fn000C18E6)
fn000C1894 proc
	enter	#6h
	cmp.w:q	#0h,r1
	jne	000C18A1

l000C189B:
	mov.w:q	#0h,r0
	mov.w:q	#0h,r2
	exitd

l000C18A1:
	add.w	#0Fh,r1
	shl:q	#-6h,r1
	mov.w	r1,-2h[fb]
	shl:q	#2h,r1
	jsr.w	fn000C195A
	mov.w	r0,-6h[fb]
	mov.w	r2,-4h[fb]
	jne	000C18C1

l000C18B7:
	cmp.w:q	#0h,r0
	jne	000C18C1

l000C18BB:
	mov.w:q	#0h,r0
	mov.w:q	#0h,r2
	exitd

l000C18C1:
	mov.w	-2h[fb],r0
	mov.w:q	#0h,r2
	mov.w	-6h[fb],a0
	mov.w	-4h[fb],a1
	add.w:q	#4h,a0
	adcf.w	a1
	ste.w	r0,[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	r2,[a1a0]
	mov.w	-6h[fb],r0
	mov.w	-4h[fb],r2
	add.w	#8h,r0
	adcf.w	r2
	exitd

;; fn000C18E6: 000C18E6
fn000C18E6 proc
	enter	#0Ah
	mov.w	r2,-2h[fb]
	mov.w	r2,r1
	jsr.w	fn000C1894
	mov.w	r0,-0Ah[fb]
	mov.w	r2,-8h[fb]
	cmp.w:q	#0h,r0
	jne	000C18FF

l000C18FB:
	cmp.w:q	#0h,r2
	jeq	000C1951

l000C18FF:
	cmp.w:q	#0h,5h[fb]
	jne	000C1909

l000C1904:
	cmp.w:q	#0h,7h[fb]
	jeq	000C1951

l000C1909:
	mov.w	5h[fb],-6h[fb]
	mov.w	7h[fb],-4h[fb]
	add.w:q	#0FFF8h,-6h[fb]
	sbb.w	#0h,-4h[fb]
	mov.w	-6h[fb],a0
	mov.w	-4h[fb],a1
	add.w:q	#4h,a0
	adcf.w	a1
	lde.w	[a1a0],r0
	shl:q	#2h,r0
	add.w:q	#0FFF8h,r0
	cmp.w	-2h[fb],r0
	jleu	000C1931

l000C192E:
	mov.w	-2h[fb],r0

l000C1931:
	push.w	r0
	push.w	7h[fb]
	push.w	5h[fb]
	push.w	-8h[fb]
	push.w	-0Ah[fb]
	jsr.a	fn000C1D6C
	add.b	#0Ah,usp
	push.w	7h[fb]
	push.w	5h[fb]
	jsr.w	fn000C185C
	add:q	#4h,usp

l000C1951:
	mov.w	-0Ah[fb],r0
	mov.w	-8h[fb],r2
	exitd
000C1959                            04                            .      

;; fn000C195A: 000C195A
;;   Called from:
;;     000C18AC (in fn000C1894)
fn000C195A proc
	enter	#10h
	mov.w	r1,-4h[fb]
	mov.w:q	#0h,-2h[fb]
	cmp.w:q	#0h,-2h[fb]
	jgtu	000C1975

l000C1968:
	jltu	000C196F

l000C196A:
	cmp.w:q	#0h,-4h[fb]
	jgtu	000C1975

l000C196F:
	mov.w:q	#0h,r0
	mov.w:q	#0h,r2
	exitd

l000C1975:
	mov.w	-4h[fb],r0
	mov.w	-2h[fb],r2
	add.w:q	#7h,r0
	adcf.w	r2
	shl.l	#0FFFAh,r2r0
	mov.w	r0,-8h[fb]
	mov.w	r2,-6h[fb]
	mov.w	#40Ah,-0Ch[fb]
	mov.w	#0h,-0Ah[fb]
	lde.w	[40Ah],-10h[fb]
	lde.w	[40Ch],-0Eh[fb]

l000C199D:
	cmp.w:q	#0h,-10h[fb]
	jne	000C19AA

l000C19A2:
	cmp.w:q	#0h,-0Eh[fb]
	jne	000C19AA

l000C19A7:
	jmp.w	000C1A37

l000C19AA:
	mov.w	-10h[fb],a0
	mov.w	-0Eh[fb],a1
	add.w:q	#4h,a0
	adcf.w	a1
	lde.w	[a1a0],r0
	pushm	a1,a0
	add.w:q	#2h,a0
	adcf.w	a1
	lde.w	[a1a0],r2
	popm	a1,a0
	cmp.w	r2,-6h[fb]
	jgtu	000C1A1C

l000C19C5:
	jltu	000C19CC

l000C19C7:
	cmp.w	r0,-8h[fb]
	jgtu	000C1A1C

l000C19CC:
	cmp.w	-8h[fb],r0
	jne	000C19D6

l000C19D1:
	cmp.w	-6h[fb],r2
	jeq	000C19EE

l000C19D6:
	sub.w	-8h[fb],r0
	sbb.w	-6h[fb],r2
	ste.w	r0,[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	r2,[a1a0]
	shl.l	#2h,r2r0
	add.w	r0,-10h[fb]
	adc.w	r2,-0Eh[fb]
	jmp.b	000C1A0A

l000C19EE:
	mov.w	-10h[fb],a0
	mov.w	-0Eh[fb],a1
	lde.w	[a1a0],r0
	add.w:q	#2h,a0
	adcf.w	a1
	lde.w	[a1a0],r2
	mov.w	-0Ch[fb],a0
	mov.w	-0Ah[fb],a1
	ste.w	r0,[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	r2,[a1a0]

l000C1A0A:
	sub.w	-8h[fb],[40Eh]
	sbb.w	-6h[fb],[410h]
	mov.w	-10h[fb],r0
	mov.w	-0Eh[fb],r2
	exitd

l000C1A1C:
	mov.w	-10h[fb],-0Ch[fb]
	mov.w	-0Eh[fb],-0Ah[fb]
	mov.w	-10h[fb],a0
	mov.w	-0Eh[fb],a1
	lde.w	[a1a0],-10h[fb]
	add.w:q	#2h,a0
	adcf.w	a1
	lde.w	[a1a0],-0Eh[fb]
	jmp.w	000C199D

l000C1A37:
	mov.w	-8h[fb],r0
	mov.w	-6h[fb],r2
	shl.l	#2h,r2r0
	cmp.w	[44Ch],r2
	jltu	000C1A53

l000C1A45:
	jgtu	000C1A4D

l000C1A47:
	cmp.w	[44Ah],r0
	jleu	000C1A53

l000C1A4D:
	mov.w:q	#0h,r0
	mov.w:q	#0h,r2
	exitd

l000C1A53:
	mov.w	[446h],-10h[fb]
	mov.w	[448h],-0Eh[fb]
	add.w	-4h[fb],[446h]
	adc.w	-2h[fb],[448h]
	sub.w	-4h[fb],[44Ah]
	sbb.w	-2h[fb],[44Ch]
	cmp.w:q	#0h,[406h]
	jne	000C1A93

l000C1A77:
	cmp.w:q	#0h,[408h]
	jne	000C1A93

l000C1A7D:
	mov.w	-10h[fb],[402h]
	mov.w	-0Eh[fb],[404h]
	mov.w	-8h[fb],[406h]
	mov.w	-6h[fb],[408h]
	jmp.b	000C1AC3

l000C1A93:
	mov.w	[406h],r0
	mov.w	[408h],r2
	shl.l	#2h,r2r0
	mov.w	[402h],-0Ch[fb]
	mov.w	[404h],-0Ah[fb]
	add.w	r0,-0Ch[fb]
	adc.w	r2,-0Ah[fb]
	cmp.w	-0Ch[fb],-10h[fb]
	jne	000C1AC3

l000C1AB3:
	cmp.w	-0Ah[fb],-0Eh[fb]
	jne	000C1AC3

l000C1AB9:
	add.w	-8h[fb],[406h]
	adc.w	-6h[fb],[408h]

l000C1AC3:
	mov.w	-10h[fb],r0
	mov.w	-0Eh[fb],r2
	exitd
000C1ACB                                  04                        .    

;; fn000C1ACC: 000C1ACC
;;   Called from:
;;     000C188F (in fn000C185C)
fn000C1ACC proc
	enter	#1Ch
	mov.w	r2,-8h[fb]
	mov.w:q	#0h,-6h[fb]
	cmp.w:q	#0h,-6h[fb]
	jgt	000C1AE7

l000C1ADB:
	jlt	000C1AE3

l000C1ADE:
	cmp.w:q	#0h,-8h[fb]
	jgtu	000C1AE7

l000C1AE3:
	mov.w:q	#0FFFFh,r0
	exitd

l000C1AE7:
	mov.w	5h[fb],-4h[fb]
	mov.w	7h[fb],-2h[fb]
	mov.w	-8h[fb],r0
	mov.w	-6h[fb],r2
	add.w:q	#7h,r0
	adcf.w	r2
	push.w	#0h
	push.w	#8h
	jsr.a	fn000C1DE4
	add:q	#4h,usp
	mov.w	r0,-8h[fb]
	mov.w	r2,-6h[fb]
	mov.w	-8h[fb],r0
	mov.w	-6h[fb],r2
	shl.l	#2h,r2r0
	mov.w	-4h[fb],-0Ch[fb]
	mov.w	-2h[fb],-0Ah[fb]
	add.w	r0,-0Ch[fb]
	adc.w	r2,-0Ah[fb]
	add.w	-8h[fb],[40Eh]
	adc.w	-6h[fb],[410h]
	mov.w	#40Ah,-10h[fb]
	mov.w	#0h,-0Eh[fb]
	lde.w	[40Ah],-14h[fb]
	lde.w	[40Ch],-12h[fb]

l000C1B43:
	cmp.w:q	#0h,-14h[fb]
	jne	000C1B50

l000C1B48:
	cmp.w:q	#0h,-12h[fb]
	jne	000C1B50

l000C1B4D:
	jmp.w	000C1D31

l000C1B50:
	mov.w	-14h[fb],a0
	mov.w	-12h[fb],a1
	add.w:q	#4h,a0
	adcf.w	a1
	lde.w	[a1a0],r0
	add.w:q	#2h,a0
	adcf.w	a1
	lde.w	[a1a0],r2
	shl.l	#2h,r2r0
	mov.w	-14h[fb],-18h[fb]
	mov.w	-12h[fb],-16h[fb]
	add.w	r0,-18h[fb]
	adc.w	r2,-16h[fb]
	cmp.w	-0Ah[fb],-12h[fb]
	jltu	000C1BB8

l000C1B78:
	jgtu	000C1B80

l000C1B7A:
	cmp.w	-0Ch[fb],-14h[fb]
	jleu	000C1BB8

l000C1B80:
	mov.w	-4h[fb],a0
	mov.w	-2h[fb],a1
	ste.w	-14h[fb],[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	-12h[fb],[a1a0]
	mov.w	-4h[fb],a0
	mov.w	-2h[fb],a1
	add.w:q	#4h,a0
	adcf.w	a1
	ste.w	-8h[fb],[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	-6h[fb],[a1a0]
	mov.w	-10h[fb],a0
	mov.w	-0Eh[fb],a1
	ste.w	-4h[fb],[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	-2h[fb],[a1a0]
	mov.w:q	#0h,r0
	exitd

l000C1BB8:
	cmp.w	-0Ch[fb],-14h[fb]
	jne	000C1C1E

l000C1BBE:
	cmp.w	-0Ah[fb],-12h[fb]
	jne	000C1C1E

l000C1BC4:
	mov.w	-14h[fb],a0
	mov.w	-12h[fb],a1
	lde.w	[a1a0],r0
	add.w:q	#2h,a0
	adcf.w	a1
	lde.w	[a1a0],r2
	mov.w	-4h[fb],a0
	mov.w	-2h[fb],a1
	ste.w	r0,[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	r2,[a1a0]
	mov.w	-14h[fb],a0
	mov.w	-12h[fb],a1
	add.w:q	#4h,a0
	adcf.w	a1
	lde.w	[a1a0],r0
	add.w:q	#2h,a0
	adcf.w	a1
	lde.w	[a1a0],r2
	add.w	-8h[fb],r0
	adc.w	-6h[fb],r2
	mov.w	-4h[fb],a0
	mov.w	-2h[fb],a1
	add.w:q	#4h,a0
	adcf.w	a1
	ste.w	r0,[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	r2,[a1a0]
	mov.w	-10h[fb],a0
	mov.w	-0Eh[fb],a1
	ste.w	-4h[fb],[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	-2h[fb],[a1a0]
	mov.w:q	#0h,r0
	exitd

l000C1C1E:
	cmp.w	-16h[fb],-2h[fb]
	jgtu	000C1C3A

l000C1C24:
	jltu	000C1C2C

l000C1C26:
	cmp.w	-18h[fb],-4h[fb]
	jgeu	000C1C3A

l000C1C2C:
	sub.w	-8h[fb],[40Eh]
	sbb.w	-6h[fb],[410h]
	mov.w:q	#0FFFFh,r0
	exitd

l000C1C3A:
	cmp.w	-18h[fb],-4h[fb]
	jeq	000C1C43

l000C1C40:
	jmp.w	000C1D16

l000C1C43:
	cmp.w	-16h[fb],-2h[fb]
	jeq	000C1C4C

l000C1C49:
	jmp.w	000C1D16

l000C1C4C:
	mov.w	-14h[fb],a0
	mov.w	-12h[fb],a1
	lde.w	[a1a0],r0
	add.w:q	#2h,a0
	adcf.w	a1
	lde.w	[a1a0],r2
	cmp.w:q	#0h,r0
	jne	000C1C62

l000C1C5E:
	cmp.w:q	#0h,r2
	jeq	000C1C7C

l000C1C62:
	cmp.w	r2,-0Ah[fb]
	jltu	000C1C7C

l000C1C67:
	jgtu	000C1C6E

l000C1C69:
	cmp.w	r0,-0Ch[fb]
	jleu	000C1C7C

l000C1C6E:
	sub.w	-8h[fb],[40Eh]
	sbb.w	-6h[fb],[410h]
	mov.w:q	#0FFFFh,r0
	exitd

l000C1C7C:
	mov.w	-14h[fb],a0
	mov.w	-12h[fb],a1
	add.w:q	#4h,a0
	adcf.w	a1
	lde.w	[a1a0],r0
	pushm	a1,a0
	add.w:q	#2h,a0
	adcf.w	a1
	lde.w	[a1a0],r2
	popm	a1,a0
	add.w	-8h[fb],r0
	adc.w	-6h[fb],r2
	mov.w	a0,r1
	mov.w	a1,r3
	ste.w	r0,[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	r2,[a1a0]
	mov.w	-14h[fb],a0
	mov.w	-12h[fb],a1
	mov.w	r0,-1Ch[fb]
	mov.w	r2,-1Ah[fb]
	lde.w	[a1a0],r0
	add.w:q	#2h,a0
	adcf.w	a1
	lde.w	[a1a0],r2
	cmp.w:q	#0h,r0
	jne	000C1CC0

l000C1CBC:
	cmp.w:q	#0h,r2
	jeq	000C1D12

l000C1CC0:
	cmp.w	r0,-0Ch[fb]
	jne	000C1D12

l000C1CC5:
	cmp.w	r2,-0Ah[fb]
	jne	000C1D12

l000C1CCA:
	mov.w	-0Ch[fb],a0
	mov.w	-0Ah[fb],a1
	add.w:q	#4h,a0
	adcf.w	a1
	lde.w	[a1a0],r0
	add.w:q	#2h,a0
	adcf.w	a1
	lde.w	[a1a0],r2
	mov.w	r0,a0
	mov.w	r2,a1
	mov.w	-1Ch[fb],r0
	mov.w	-1Ah[fb],r2
	add.w	a0,r0
	adc.w	a1,r2
	mov.w	r1,a0
	mov.w	r3,a1
	ste.w	r0,[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	r2,[a1a0]
	mov.w	-0Ch[fb],a0
	mov.w	-0Ah[fb],a1
	lde.w	[a1a0],r0
	add.w:q	#2h,a0
	adcf.w	a1
	lde.w	[a1a0],r2
	mov.w	-14h[fb],a0
	mov.w	-12h[fb],a1
	ste.w	r0,[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	r2,[a1a0]

l000C1D12:
	mov.w:q	#0h,r0
	exitd

l000C1D16:
	mov.w	-14h[fb],-10h[fb]
	mov.w	-12h[fb],-0Eh[fb]
	mov.w	-14h[fb],a0
	mov.w	-12h[fb],a1
	lde.w	[a1a0],-14h[fb]
	add.w:q	#2h,a0
	adcf.w	a1
	lde.w	[a1a0],-12h[fb]
	jmp.w	000C1B43

l000C1D31:
	mov.w	-10h[fb],a0
	mov.w	-0Eh[fb],a1
	ste.w	-4h[fb],[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	-2h[fb],[a1a0]
	mov.w:q	#0h,r0
	mov.w:q	#0h,r2
	mov.w	-4h[fb],a0
	mov.w	-2h[fb],a1
	ste.w	r0,[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	r2,[a1a0]
	mov.w	-4h[fb],a0
	mov.w	-2h[fb],a1
	add.w:q	#4h,a0
	adcf.w	a1
	ste.w	-8h[fb],[a1a0]
	add.w:q	#2h,a0
	adcf.w	a1
	ste.w	-6h[fb],[a1a0]
	mov.w:q	#0h,r0
	exitd
