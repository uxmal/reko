;;; Tests sequences of shifts that should be coalesced as one shift.

.i86
driver proc
	mov eax,[0x100]
	mov edx,[0x104]
	shrd eax,edx,4
	shr edx,4
	mov [0x0108],eax
	mov [0x010C],edx
	
	mov eax,[0x0100]
	mov edx,[0x0104]
	shl eax,1
	rcl edx,1
	
	ret
driver endp
