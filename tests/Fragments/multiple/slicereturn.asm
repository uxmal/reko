;;; Handles case when only a part of the returned value of a procedure is used.

.i86

main	proc
		mov si,0x200
		call	fetchWord
		mov [0x202],ax			;; use the full word.
		mov	si,0x300
		call	fetchWord
		mov [0x302],al			;; use only the low half of the fetched word!
		ret
main	endp

fetchWord proc
		mov ax,[si]
		ret
		endp
