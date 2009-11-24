;;; Another test case for mutual recursion (very inefficiently implemented, but
;;; that's not the point.

main	proc
		mov ax,3
		call even
		mov [0x300],al
		mov ax,3
		call odd
		mov [0x301],al
		ret
		endp
		
;;; sets al = 1 if ax is even, otherwise 0

even	proc
		or ax,ax
		jz is_even
		dec ax
		jmp odd
is_even:
		mov al,1
		ret
		endp

;;; odd
		
odd		proc
		or ax,ax
		jz is_not_odd
		dec ax
		jmp even
is_not_odd:
		xor al,al
		ret
		endp
