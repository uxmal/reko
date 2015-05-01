;;; Another test case for mutual recursion (very inefficiently implemented, but
;;; that's not the point).

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

even	proc				; Offset 0015
		or ax,ax
		jz is_even
		dec ax			; 0019
		jmp odd			; 001A
is_even:				
		mov al,1		; 001D
		ret
		endp

;;; odd
		
odd		proc
		or ax,ax		; 0020
		jz is_not_odd		; 0022
		dec ax			; 0024
		jmp even		; 0025
is_not_odd:
		xor al,al		; 0028		
		ret
		endp
