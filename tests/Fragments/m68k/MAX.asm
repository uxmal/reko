	idnt	"MAX.C"
	opt	0
	opt	NQLPSMRBT
	section	"CODE",code
	public	_main
	cnop	0,4
_main
	subq.w	#8,a7
	movem.l	l7,-(a7)
	pea	l2
	jsr	___v0printf
	lea	(8+l9,a7),a0
	move.l	a0,-(a7)
	lea	(8+l9,a7),a0
	move.l	a0,-(a7)
	pea	l3
	jsr	_scanf
	add.w	#16,a7
	move.l	(0+l9,a7),d0
	cmp.l	(4+l9,a7),d0
	beq	l5
l4
	move.l	(4+l9,a7),-(a7)
	move.l	(4+l9,a7),-(a7)
	jsr	_max
	move.l	d0,-(a7)
	pea	l6
	jsr	_printf
	add.w	#16,a7
l5
l1
l7	reg
l9	equ	0
	addq.w	#8,a7
	rts
	cnop	0,4
l2
	dc.b	69
	dc.b	110
	dc.b	116
	dc.b	101
	dc.b	114
	dc.b	32
	dc.b	50
	dc.b	32
	dc.b	110
	dc.b	117
	dc.b	109
	dc.b	98
	dc.b	101
	dc.b	114
	dc.b	115
	dc.b	58
	dc.b	32
	dc.b	0
	cnop	0,4
l3
	dc.b	37
	dc.b	100
	dc.b	32
	dc.b	37
	dc.b	100
	dc.b	0
	cnop	0,4
l6
	dc.b	77
	dc.b	97
	dc.b	120
	dc.b	105
	dc.b	109
	dc.b	117
	dc.b	109
	dc.b	58
	dc.b	32
	dc.b	37
	dc.b	100
	dc.b	10
	dc.b	0
	opt	0
	opt	NQLPSMRBT
	public	_max
	cnop	0,4
_max
	movem.l	l13,-(a7)
	move.l	(8+l15,a7),d1
	move.l	(4+l15,a7),d0
	cmp.l	d0,d1
	bge	l12
l11
	bra	l10
l12
	move.l	d1,d0
l10
l13	reg
l15	equ	0
	rts
; stacksize=0
	public	_printf
	public	___v0printf
	public	_scanf
