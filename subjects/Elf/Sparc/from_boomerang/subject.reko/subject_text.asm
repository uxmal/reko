;;; Segment .text (00010958)

;; _start: 00010958
_start proc
	or	%g0,00000000,%i6
	ld	[%sp+64],%l0
	add	%sp,00000044,%l1
	sethi	00000083,%o1
	st	%l1,[%o1+608]
	sll	%l0,00000002,%l2
	add	%l2,00000004,%l2
	add	%l1,%l2,%l2
	sethi	00000083,%l3
	st	%l2,[%l3+572]
	sethi	00000000,%l5
	or	%l5,00000000,%l5
	orcc	%g0,%l5,%g0
	be	000109F8
	sethi	00000000,%g0

l00010994:
	sll	%l5,00000002,%l6
	and	%l6,00000300,%l7
	and	%l5,0000003F,%l6
	or	%l6,%l7,%l7
	sll	%l7,00000016,%l5
	sethi	00000083,%l4
	or	%l4,00000258,%l4
	stfsr	%fsr,[%l4+%g0]
	ld	[%l4+%g0],%l6
	sethi	000C0FFF,%l7
	or	%l7,000003FF,%l7
	and	%l6,%l7,%l6
	or	%l6,%l5,%l6
	st	%l6,[%l4+%g0]
	ldfsr	[%l4+%g0],%fsr
	sethi	00000000,%l5
	or	%l5,00000000,%l5
	and	%l5,0000003F,%l5
	subcc	%l5,00000035,%g0
	bne	000109F8
	sethi	00000000,%g0

l000109E8:
	sethi	00000083,%l5
	or	%l5,0000025C,%l5
	or	%g0,00000001,%l4
	st	%l4,[%l5+%g0]

l000109F8:
	sub	%sp,00000020,%sp
	orcc	%g0,%g1,%g0
	be	00010A10
	or	%g0,%g1,%o0

l00010A08:
	call	atexit
	sethi	00000000,%g0

l00010A10:
	sethi	00000043,%o0
	call	atexit
	or	%o0,000000B8,%o0
	call	_init
	sethi	00000000,%g0
	or	%g0,%l0,%o0
	or	%g0,%l1,%o1
	or	%g0,%l2,%o2
	or	%g0,%l3,%o3
	call	main
	sethi	00000000,%g0
	call	exit
00010A40 01 00 00 00 40 00 40 C5 01 00 00 00 00 00 00 00 ....@.@.........
00010A50 00 00 00 00 00 00 00 00 00 00 00 00             ............    

;; func1: 00010A5C
func1 proc
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0
00010A64             00 00 00 00 00 00 00 00 00 00 00 00     ............
00010A70 00 00 00 00                                     ....            

;; func2: 00010A74
func2 proc
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0
00010A7C                                     00 00 00 00             ....
00010A80 00 00 00 00 00 00 00 00 00 00 00 00             ............    

;; func3: 00010A8C
func3 proc
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0
00010A94             00 00 00 00 00 00 00 00 00 00 00 00     ............
00010AA0 00 00 00 00                                     ....            

;; func4: 00010AA4
func4 proc
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0
00010AAC                                     00 00 00 00             ....
00010AB0 00 00 00 00 00 00 00 00 00 00 00 00             ............    

;; func5: 00010ABC
func5 proc
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0
00010AC4             00 00 00 00 00 00 00 00 00 00 00 00     ............
00010AD0 00 00 00 00                                     ....            

;; func6: 00010AD4
func6 proc
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0
00010ADC                                     00 00 00 00             ....
00010AE0 00 00 00 00 00 00 00 00 00 00 00 00             ............    

;; func7: 00010AEC
func7 proc
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0
00010AF4             00 00 00 00 00 00 00 00 00 00 00 00     ............
00010B00 00 00 00 00                                     ....            

;; func8: 00010B04
func8 proc
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0

;; main: 00010B0C
;;   Called from:
;;     00010A34 (in _start)
main proc
	save	%sp,FFFFFFA0,%sp
	or	%g0,00000001,%o0
	subcc	%i0,00000001,%g0
	ble,a	00010B20
	or	%g0,00000000,%o0

l00010B20:
	orcc	%g0,%o0,%g0
	be	00010B34
	sethi	00000042,%o0

l00010B2C:
	ba	00010B3C
	add	%o0,0000025C,%o0

l00010B34:
	sethi	%hi(00420274),%o0
	add	%o0,%lo(00420274),%o0

l00010B3C:
	be	00010B4C
	sethi	00000042,%o1

l00010B44:
	ba	00010B54
	add	%o1,0000028C,%o3

l00010B4C:
	sethi	%hi(004202A4),%o1
	add	%o1,%lo(004202A4),%o3

l00010B54:
	be	00010B64
	sethi	00000042,%o1

l00010B5C:
	ba	00010B6C
	add	%o1,000002BC,%o2

l00010B64:
	sethi	%hi(004202D4),%o1
	add	%o1,%lo(004202D4),%o2

l00010B6C:
	be	00010B7C
	sethi	00000042,%o1

l00010B74:
	ba	00010B84
	add	%o1,000002EC,%o1

l00010B7C:
	sethi	%hi(00420304),%o1
	add	%o1,%lo(00420304),%o1

l00010B84:
	be	00010BE8
	sethi	%hi(0042025C),%o4

l00010B8C:
	add	%o4,0000025C,%o4
	subcc	%o0,%o4,%g0
	bne	00010BDC
	sethi	%hi(0042028C),%o0

l00010B9C:
	add	%o0,0000028C,%o0
	subcc	%o3,%o0,%g0
	bne	00010BE0
	or	%g0,00000000,%i1

l00010BAC:
	sethi	%hi(004202BC),%o0
	add	%o0,%lo(004202BC),%o0
	subcc	%o2,%o0,%g0
	bne	00010BE0
	or	%g0,00000000,%i1

l00010BC0:
	sethi	%hi(004202EC),%o0
	add	%o0,%lo(004202EC),%o0
	subcc	%o1,%o0,%g0
	bne	00010BE0
	or	%g0,00000000,%i1

l00010BD4:
	ba	00010C40
	or	%g0,00000001,%i1

l00010BDC:
	or	%g0,00000000,%i1

l00010BE0:
	ba	00010C44
	subcc	%i1,00000000,%g0

l00010BE8:
	sethi	%hi(00420274),%o4
	add	%o4,%lo(00420274),%o4
	subcc	%o0,%o4,%g0
	bne	00010C3C
	sethi	%hi(004202A4),%o0

l00010BFC:
	add	%o0,000002A4,%o0
	subcc	%o3,%o0,%g0
	bne	00010C3C
	sethi	00000000,%g0

l00010C0C:
	sethi	%hi(004202D4),%o0
	add	%o0,%lo(004202D4),%o0
	subcc	%o2,%o0,%g0
	bne	00010C3C
	sethi	00000000,%g0

l00010C20:
	sethi	%hi(00420304),%o0
	add	%o0,%lo(00420304),%o0
	subcc	%o1,%o0,%g0
	bne	00010C3C
	sethi	00000000,%g0

l00010C34:
	ba	00010C40
	or	%g0,00000001,%i1

l00010C3C:
	or	%g0,00000000,%i1

l00010C40:
	subcc	%i1,00000000,%g0

l00010C44:
	be	00010C5C
	sethi	00000043,%g1

l00010C4C:
	call	printf
	add	%g1,000000F8,%o0
	ba	00010C6C
	or	%g0,00000000,%i0

l00010C5C:
	sethi	00000043,%g1
	call	printf
	add	%g1,00000100,%o0
	or	%g0,00000000,%i0

l00010C6C:
	subcc	%i1,00000000,%g0
	be,a	00010C78
	or	%g0,00000001,%i0

l00010C78:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0
