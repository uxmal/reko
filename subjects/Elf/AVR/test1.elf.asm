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
	invalid
	invalid
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
	invalid
	pop	r25
	invalid
	pop	r26
	invalid
	pop	r27
	invalid
	pop	r19
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	push	r18
	invalid
	push	r24
	invalid
	push	r25
	invalid
	push	r26
	invalid
	push	r27
	invalid
	pop	r24
	invalid
	pop	r25
	invalid
	pop	r26
	invalid
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
	invalid
	pop	r19
	invalid
	pop	r20
	invalid
	pop	r21
	invalid
	in	r24,26
	invalid
	rjmp	0156
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	out	3F,r25
	invalid
	invalid
	invalid
	eor	r18,r18
	invalid
	invalid
	invalid
	invalid
	ldi	r24,02
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	ret

;; delay: 017C
delay proc
	push	r14
	push	r15
	push	r16
	push	r17
	push	r28
	push	r29
	invalid
	invalid
	invalid
	invalid
	invalid
	rjmp	01B0
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	pop	r29
	pop	r28
	pop	r17
	pop	r16
	pop	r15
	pop	r14
	ret

;; init: 01C8
init proc
	invalid
	in	r24,24
	invalid
	out	24,r24
	in	r24,24
	invalid
	out	24,r24
	in	r24,25
	invalid
	out	25,r24
	in	r24,25
	invalid
	out	25,r24
	ldi	r30,6E
	ldi	r31,00
	invalid
	invalid
	invalid
	ldi	r30,81
	ldi	r31,00
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	ldi	r30,80
	ldi	r31,00
	invalid
	invalid
	invalid
	ldi	r30,B1
	ldi	r31,00
	invalid
	invalid
	invalid
	ldi	r30,B0
	ldi	r31,00
	invalid
	invalid
	invalid
	ldi	r30,7A
	ldi	r31,00
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	push	r1
	invalid
	ret

;; main: 023E
main proc
	invalid
	invalid
	invalid
	invalid
	invalid
	invalid
	rjmp	2246
	cli
	rjmp	224E
;;; Segment .bss (00800100)
00800100 00 00 00 00 00 00 00 00 00                      .........      
