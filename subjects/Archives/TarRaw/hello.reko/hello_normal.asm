;;; Segment normal (0100)

;; hello: 0100
hello proc
	lim	gp0,#0x111
	sjs	gp15,0105
	urs	gp15

;; puts: 0105
;;   Called from:
;;     0102 (in hello)
puts proc
	lr	gp1,gp0

l0106:
	l	gp0,gp1
	bez	010D

l0109:
	sjs	gp15,010E
	aisp	gp1,#1
	br	0106

l010D:
	urs	gp15

;; putchar: 010E
;;   Called from:
;;     0109 (in puts)
putchar proc
	co	gp0
	urs	gp15
0111      0048 0065 006C 006C 006F 0021 000A   .H.e.l.l.o.!..
0118 0000                                    ..              
