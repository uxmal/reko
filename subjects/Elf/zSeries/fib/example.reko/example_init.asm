;;; Segment .init (00000560)

;; _init: 00000560
;;   Called from:
;;     00000836 (in __libc_csu_init)
_init proc
	stmg	r6,r15,48(r15)
	lgr	r1,r15
	aghi	r15,-000000A0
	stg	r1,(r15)
	larl	r12,00002000
	larl	r1,00002038
	lg	r1,(r1)
	ltgr	r1,r1
	je	00000590

l0000058E:
	basr	r14,r1

l00000590:
	lg	r4,272(r15)
	lmg	r6,r15,208(r15)
	br	r4
0000059E                                           07 07               ..
