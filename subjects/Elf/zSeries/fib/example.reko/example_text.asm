;;; Segment .text (0000000000000600)

;; _start: 0000000000000600
_start proc
	la	r4,8(r15)
	lg	r3,(r15)
	lghi	r0,-00000010
	ngr	r15,r0
	aghi	r15,-000000B0
	xc	(8,r15),(r15)
	stmg	r14,r15,160(r15)
	la	r7,160(r15)
	larl	r6,__libc_csu_fini
	larl	r5,__libc_csu_init
	larl	r2,__libc_csu_init_GOT
	lg	r2,(r2)
	brasl	r14,__libc_start_main
	invalid
	nopr	r7

;; deregister_tm_clones: 0000000000000648
;;   Called from:
;;     0000000000000708 (in __do_global_dtors_aux)
deregister_tm_clones proc
	larl	r5,00000000000008C8
	larl	r1,__TMC_END__
	larl	r2,__TMC_END__
	la	r1,7(r1)
	sgr	r1,r2
	clg	r1,(r5)
	bler	r14

l000000000000066A:
	larl	r1,0000000000002030
	lg	r1,(r1)
	ltgr	r1,r1
	ber	r14

l000000000000067C:
	br	r1
000000000000067E                                           07 07               ..

;; register_tm_clones: 0000000000000680
;;   Called from:
;;     0000000000000746 (in frame_dummy)
register_tm_clones proc
	larl	r1,__TMC_END__
	larl	r3,__TMC_END__
	sgr	r3,r1
	srag	r3,r3,00000003
	srlg	r1,r3,0000003F
	agr	r3,r1
	srag	r3,r3,00000001
	ber	r14

l00000000000006A8:
	larl	r1,0000000000002050
	lg	r1,(r1)
	ltgr	r1,r1
	ber	r14

l00000000000006BA:
	larl	r2,__TMC_END__
	br	r1
00000000000006C2       07 07 07 07 07 07                           ......        

;; __do_global_dtors_aux: 00000000000006C8
__do_global_dtors_aux proc
	stmg	r11,r15,88(r15)
	larl	r13,00000000000008D0
	aghi	r15,-000000A0
	larl	r11,__TMC_END__
	cli	(r11),00
	jne	0000000000000712

l00000000000006E6:
	larl	r1,0000000000002028
	clc	(8,r13),(r1)
	je	0000000000000708

l00000000000006F6:
	larl	r1,__dso_handle
	lg	r2,(r1)
	brasl	r14,__cxa_finalize

l0000000000000708:
	brasl	r14,deregister_tm_clones
	mvi	(r11),01

l0000000000000712:
	lg	r4,272(r15)
	lmg	r11,r15,248(r15)
	br	r4

;; frame_dummy: 0000000000000720
frame_dummy proc
	stmg	r13,r15,104(r15)
	larl	r13,00000000000008D8
	aghi	r15,-000000A0
	larl	r2,__JCR_END__
	clc	(8,r13),(r2)
	jne	000000000000074C

l0000000000000740:
	lmg	r13,r15,264(r15)
	jg	register_tm_clones

l000000000000074C:
	larl	r1,0000000000002048
	lg	r1,(r1)
	ltgr	r1,r1
	je	0000000000000740

l0000000000000760:
	basr	r14,r1
	j	0000000000000740
0000000000000766                   07 07                               ..        

;; fib: 0000000000000768
;;   Called from:
;;     00000000000007A2 (in fib)
;;     00000000000007BE (in fib)
;;     0000000000000800 (in main)
fib proc
	stmg	r10,r15,80(r15)
	aghi	r15,-000000A8
	lgr	r11,r15
	lgr	r1,r2
	st	r1,164(r11)
	l	r1,164(r11)
	chi	r1,+00000001
	jh	0000000000000792

l000000000000078A:
	l	r1,164(r11)
	j	00000000000007CA

l0000000000000792:
	l	r1,164(r11)
	ahi	r1,-00000001
	lgfr	r1,r1
	lgr	r2,r1
	brasl	r14,fib
	lgr	r1,r2
	lr	r10,r1
	l	r1,164(r11)
	ahi	r1,-00000002
	lgfr	r1,r1
	lgr	r2,r1
	brasl	r14,fib
	lgr	r1,r2
	ar	r1,r10

l00000000000007CA:
	lgfr	r1,r1
	lgr	r2,r1
	lg	r4,280(r11)
	lmg	r10,r15,248(r11)
	br	r4

;; main: 00000000000007E0
main proc
	stmg	r11,r15,88(r15)
	aghi	r15,-000000B0
	lgr	r11,r15
	lgr	r1,r2
	stg	r3,160(r11)
	st	r1,172(r11)
	lghi	r2,+0000000A
	brasl	r14,fib
	lgr	r1,r2
	lgfr	r1,r1
	lgr	r2,r1
	lg	r4,288(r11)
	lmg	r11,r15,264(r11)
	br	r4

;; __libc_csu_init: 0000000000000820
__libc_csu_init proc
	stmg	r7,r15,56(r15)
	aghi	r15,-000000A0
	lgr	r10,r2
	lgr	r9,r3
	lgr	r8,r4
	brasl	r14,_init
	larl	r11,__do_global_dtors_aux_fini_array_entry
	larl	r1,__frame_dummy_init_array_entry
	sgr	r11,r1
	srag	r11,r11,00000003
	je	0000000000000876

l0000000000000856:
	lgr	r7,r1

l000000000000085A:
	lg	r1,(r7)
	lgr	r4,r8
	lgr	r3,r9
	lgr	r2,r10
	aghi	r7,+00000008
	basr	r14,r1
	brctg	r11,000000000000085A

l0000000000000876:
	lg	r4,272(r15)
	lmg	r7,r15,216(r15)
	br	r4
0000000000000884             07 07 07 07                             ....        

;; __libc_csu_fini: 0000000000000888
__libc_csu_fini proc
	br	r14
000000000000088A                               07 07 07 07 07 07           ......
