;;; Segment .text (00000600)

;; _start: 00000600
_start proc
	la	r4,8(r15)
	lg	r3,(r15)
	lghi	r0,-00000010
	ngr	r15,r0
	aghi	r15,-000000B0
	xc	(8,r15),(r15)
	stmg	r14,r15,160(r15)
	la	r7,160(r15)
	larl	r6,00000888
	larl	r5,00000820
	larl	r2,00002040
	lg	r2,(r2)
	brasl	r14,000005E0
	invalid
	nopr	r7

;; deregister_tm_clones: 00000648
;;   Called from:
;;     00000708 (in __do_global_dtors_aux)
deregister_tm_clones proc
	larl	r5,000008C8
	larl	r1,00002068
	larl	r2,00002068
	la	r1,7(r1)
	sgr	r1,r2
	clg	r1,(r5)
	bler	r14

l0000066A:
	larl	r1,00002030
	lg	r1,(r1)
	ltgr	r1,r1
	ber	r14

l0000067C:
	br	r1
0000067E                                           07 07               ..

;; register_tm_clones: 00000680
;;   Called from:
;;     00000746 (in frame_dummy)
register_tm_clones proc
	larl	r1,00002068
	larl	r3,00002068
	sgr	r3,r1
	srag	r3,r3,00000003
	srlg	r1,r3,0000003F
	agr	r3,r1
	srag	r3,r3,00000001
	ber	r14

l000006A8:
	larl	r1,00002050
	lg	r1,(r1)
	ltgr	r1,r1
	ber	r14

l000006BA:
	larl	r2,00002068
	br	r1
000006C2       07 07 07 07 07 07                           ......       

;; __do_global_dtors_aux: 000006C8
__do_global_dtors_aux proc
	stmg	r11,r15,88(r15)
	larl	r13,000008D0
	aghi	r15,-000000A0
	larl	r11,00002068
	cli	(r11),00
	jne	00000712

l000006E6:
	larl	r1,00002028
	clc	(8,r13),(r1)
	je	00000708

l000006F6:
	larl	r1,00002060
	lg	r2,(r1)
	brasl	r14,000005C0

l00000708:
	brasl	r14,00000648
	mvi	(r11),01

l00000712:
	lg	r4,272(r15)
	lmg	r11,r15,248(r15)
	br	r4

;; frame_dummy: 00000720
frame_dummy proc
	stmg	r13,r15,104(r15)
	larl	r13,000008D8
	aghi	r15,-000000A0
	larl	r2,00001E18
	clc	(8,r13),(r2)
	jne	0000074C

l00000740:
	lmg	r13,r15,264(r15)
	jg	00000680

l0000074C:
	larl	r1,00002048
	lg	r1,(r1)
	ltgr	r1,r1
	je	00000740

l00000760:
	basr	r14,r1
	j	00000740
00000766                   07 07                               ..       

;; fib: 00000768
;;   Called from:
;;     000007A2 (in fib)
;;     000007BE (in fib)
;;     00000800 (in main)
fib proc
	stmg	r10,r15,80(r15)
	aghi	r15,-000000A8
	lgr	r11,r15
	lgr	r1,r2
	st	r1,164(r11)
	l	r1,164(r11)
	chi	r1,+00000001
	jh	00000792

l0000078A:
	l	r1,164(r11)
	j	000007CA

l00000792:
	l	r1,164(r11)
	ahi	r1,-00000001
	lgfr	r1,r1
	lgr	r2,r1
	brasl	r14,00000768
	lgr	r1,r2
	lr	r10,r1
	l	r1,164(r11)
	ahi	r1,-00000002
	lgfr	r1,r1
	lgr	r2,r1
	brasl	r14,00000768
	lgr	r1,r2
	ar	r1,r10

l000007CA:
	lgfr	r1,r1
	lgr	r2,r1
	lg	r4,280(r11)
	lmg	r10,r15,248(r11)
	br	r4

;; main: 000007E0
main proc
	stmg	r11,r15,88(r15)
	aghi	r15,-000000B0
	lgr	r11,r15
	lgr	r1,r2
	stg	r3,160(r11)
	st	r1,172(r11)
	lghi	r2,+0000000A
	brasl	r14,00000768
	lgr	r1,r2
	lgfr	r1,r1
	lgr	r2,r1
	lg	r4,288(r11)
	lmg	r11,r15,264(r11)
	br	r4

;; __libc_csu_init: 00000820
__libc_csu_init proc
	stmg	r7,r15,56(r15)
	aghi	r15,-000000A0
	lgr	r10,r2
	lgr	r9,r3
	lgr	r8,r4
	brasl	r14,00000560
	larl	r11,00001E10
	larl	r1,00001E08
	sgr	r11,r1
	srag	r11,r11,00000003
	je	00000876

l00000856:
	lgr	r7,r1

l0000085A:
	lg	r1,(r7)
	lgr	r4,r8
	lgr	r3,r9
	lgr	r2,r10
	aghi	r7,+00000008
	basr	r14,r1
	brctg	r11,0000085A

l00000876:
	lg	r4,272(r15)
	lmg	r7,r15,216(r15)
	br	r4
00000884             07 07 07 07                             ....       

;; __libc_csu_fini: 00000888
__libc_csu_fini proc
	br	r14
0000088A                               07 07 07 07 07 07           ......
