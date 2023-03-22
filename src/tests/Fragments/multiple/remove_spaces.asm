		.i86
_main	proc
		mov		ax,offset buffer
		push	cs
		push	ax
		call	_strip_spaces
		mov		ax,offset buffer
		push	cs
		push	ax
		call	_strlen
		mov		[stripped_length],ax
		ret
		endp

; Performs an in-place removal of spaces in a string.		
_strip_spaces proc
		push	bp
		mov		bp,sp
		push	ds
		lds		si,[bp+4]
		les		di,[bp+4]

_strip_space_loop:
		lodsb
		or		al,al
		jz		_strip_done

		cmp		al,' '
		jz		_strip_space_loop
		stosb
		jmp		_strip_space_loop
_strip_done
		pop		ds
		pop		bp
		ret
		endp

_strlen	proc
		push	bp
		mov		bp,sp
		push	ds
		lds		si,[bp+4]
		xor		cx,cx
_strlen_loop:
		lodsb
		or		al,al
		jz		_strlen_done
		inc		cx
		jmp		_strlen_loop

_strlen_done:
		mov		ax,cx
		pop		ds
		pop		bp
		ret

buffer		db	'This is a string with spaces',0
stripped_length dw 0
		