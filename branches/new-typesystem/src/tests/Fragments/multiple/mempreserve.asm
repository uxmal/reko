;;; Replicates a pattern found in some old binaries, where registers are stored
;;; (in a non-reentrant fashion) in global variables around a call. Advanced 
;;; alias analysis is required to handle this case in a general fashion, but
;;; can be worked around by allowing user directives.

.i86

main proc
	mov ax,[0x300]
	call memfoo
	mov [0x0302],ax

	mov ax,[0x300]	
	call membar
	mov [0x304],ax
	ret

	endp
	
memfoo proc 
	mov [0x400],ax
	mov [0x402],bx
	mov [0x404],cx
	
	mov bx,[0x100]			; do some work
	mov cl,[bx+0x42]
	mov ax,[bx+0x40]
	shl ax,cl
	mov [0x0102],ax			; the "result" is stored in memory, and AX is dead from here on.
	
	mov ax,[0x400]			
	mov bx,[0x402]
	mov cx,[0x404]
	ret
memfoo endp 

;;; In the user directive for this function, we don't specify saved registers, so AX should be
;;; live in/out here.
membar proc
	mov [0x500],ax
	mov bx,[0x1000]
	mov ax,[bx]
	add ax,[bx+2]
	mov [bx+4],ax
	mov ax,[0x500]
	ret
	endp
