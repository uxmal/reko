;;; Segment  (00000000)
00000000 0C 94 34 00 0C 94 46 00 0C 94 46 00 0C 94 46 00 ..4...F...F...F.
00000010 0C 94 46 00 0C 94 46 00 0C 94 46 00 0C 94 46 00 ..F...F...F...F.
00000020 0C 94 46 00 0C 94 46 00 0C 94 46 00 0C 94 46 00 ..F...F...F...F.
00000030 0C 94 46 00 0C 94 46 00 0C 94 46 00 0C 94 46 00 ..F...F...F...F.
00000040 0C 94 50 00 0C 94 46 00 0C 94 46 00 0C 94 46 00 ..P...F...F...F.
00000050 0C 94 46 00 0C 94 46 00 0C 94 46 00 0C 94 46 00 ..F...F...F...F.
00000060 0C 94 46 00 0C 94 46 00 11 24 1F BE CF EF D8 E0 ..F...F..$......
00000070 DE BF CD BF 11 E0 A0 E0 B1 E0 01 C0 1D 92 A9 30 ...............0
00000080 B1 07 E1 F7 0E 94 1F 01 0C 94 26 01 0C 94 00 00 ..........&.....

;; setup: 0090
setup proc
	ret

;; loop: 0092
loop proc
	ldi	r22,E8
	ldi	r23,03
	ldi	r24,00
	ldi	r25,00
	call	0000017C
	ret

;; __vector_16: 00A0
__vector_16 proc
	push	r1
	push	r0
	in	r0,3F
	push	r0
	eor	r1,r1
	push	r18
	push	r19
	push	r24
	push	r25
	push	r26
	push	r27
	pop	r24
	movw	r0,r8
	pop	r25
	movw	r0,r10
	pop	r26
	movw	r0,r12
	pop	r27
	movw	r0,r14
	pop	r19
	movw	r0,r16
	adiw	r24,01
	adc	r26,r1
	adc	r27,r1
	mov	r19,r18
	subi	r18,FD
	cpi	r18,7D
	brcs	00DE
	subi	r18,7D
	adiw	r24,01
	adc	r26,r1
	adc	r27,r1
	push	r18
	movw	r0,r16
	push	r24
	movw	r0,r8
	push	r25
	movw	r0,r10
	push	r26
	movw	r0,r12
	push	r27
	movw	r0,r14
	pop	r24
	movw	r0,r0
	pop	r25
	movw	r0,r2
	pop	r26
	movw	r0,r4
0100 B0 91 03 01                                     ....           
0104             01 96 A1 1D                             ....       
0108                         B1                              .      
0109                            1D 80 93 00 01 90 93          .......
0110 01 01 A0 93 02 01 B0 93 03 01 BF 91 AF 91 9F 91 ................
0120 8F 91 3F 91 2F 91 0F 90 0F BE 0F 90 1F 90 18 95 ..?./...........

;; micros: 0130
micros proc
	in	r25,3F
	cli
	pop	r18
	movw	r0,r0
	pop	r19
	movw	r0,r2
	pop	r20
	movw	r0,r4
	pop	r21
	movw	r0,r6
	in	r24,26
	invalid
	rjmp	0156
	cpi	r24,FF
	breq	0154
	subi	r18,FF
	sbci	r19,FF
	sbci	r20,FF
	sbci	r21,FF
	out	3F,r25
	mov	r20,r21
	mov	r19,r20
	mov	r18,r19
	eor	r18,r18
	add	r18,r24
	adc	r19,r1
	adc	r20,r1
	adc	r21,r1
	ldi	r24,02
	add	r18,r18
	adc	r19,r19
	adc	r20,r20
	adc	r21,r21
	dec	r24
	brid	0168
	movw	r22,r18
	movw	r24,r20
	ret

;; delay: 017C
delay proc
	push	r14
	push	r15
	push	r16
	push	r17
	push	r28
	push	r29
	movw	r14,r22
	movw	r16,r24
	call	00000130
	movw	r28,r22
	rjmp	01B0
	call	00000130
	sub	r22,r28
	sbc	r23,r29
	subi	r22,E8
	sbci	r23,03
	brie	0192
	sec
	sbc	r14,r1
	sbc	r15,r1
	sbc	r16,r1
	sbc	r17,r1
	subi	r28,18
	sbci	r29,FC
	cp	r14,r1
	cpc	r15,r1
	cpc	r16,r1
	cpc	r17,r1
	brid	0192
	pop	r29
	pop	r28
	pop	r17
	pop	r16
	pop	r15
	pop	r14
	ret

;; init: 01C8
init proc
	sei
	in	r24,24
	ori	r24,02
	out	24,r24
	in	r24,24
	ori	r24,01
	out	24,r24
	in	r24,25
	ori	r24,02
	out	25,r24
	in	r24,25
	ori	r24,01
	out	25,r24
	ldi	r30,6E
	ldi	r31,00
	invalid
	ori	r24,01
	invalid
	ldi	r30,81
	ldi	r31,00
	invalid
	invalid
	ori	r24,02
	invalid
	invalid
	ori	r24,01
	invalid
	ldi	r30,80
	ldi	r31,00
	invalid
	ori	r24,01
	invalid
	ldi	r30,B1
	ldi	r31,00
	invalid
	ori	r24,04
	invalid
	ldi	r30,B0
	ldi	r31,00
	invalid
	ori	r24,01
	invalid
	ldi	r30,7A
	ldi	r31,00
	invalid
	ori	r24,04
	invalid
	invalid
	ori	r24,02
	invalid
	invalid
	ori	r24,01
	invalid
	invalid
	ori	r24,80
	invalid
	push	r1
	invalid
	ret

;; main: 023E
main proc
	call	000001C8
	call	00000090
	call	00000092
	rjmp	2246
	cli
	rjmp	224E
;;; Segment .bss (00800100)
00800100 00 00 00 00 00 00 00 00 00                      .........      
