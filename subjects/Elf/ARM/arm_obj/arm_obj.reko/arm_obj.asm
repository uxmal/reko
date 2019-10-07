;;; Segment .text (08048000)

;; my_using_code: 08048000
my_using_code proc
	mvn	r0,#&29
	b	$08048010
08048008                         00 00 00 00 00 00 00 00         ........

l08048010:
	andeq	r0,r0,r0

l08048014:
	andeq	r0,r0,r0

l08048018:
	andeq	r0,r0,r0

l0804801C:
	andeq	r0,r0,r0

l08048020:
	andeq	r0,r0,r0

l08048024:
	andeq	r0,r0,r0
