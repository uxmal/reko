;;; Tests LOOP, LOOPNE, and LOOPE constructs.

.i86

foo proc
	mov si,[0x0100]
	mov cx,[si]
	inc si
	inc si
scan:
	lodsb
	cmp al,ah
	loopne scan

	mov si,[0x0100]
	lodsw
	xor al,al
zot:
	stosb
	loop zot
	
	ret
foo endp

	