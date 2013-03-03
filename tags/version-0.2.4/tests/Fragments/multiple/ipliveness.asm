;;; A tangled mess of interprocedural liveness

main proc
	mov di,[0x100]
	mov si,[di]			; this instance of DI is live across the call, but not a parameter!
	call intermediate
	mov [di+02],ax		; AX is an out parameter of fn1
	
	mov bx,[0x102]		; this instance of BX is also live across call, but not parameter.
	mov si,[bx+4]
	call intermediate
	mov [bx+8],ax
	
	mov di,[0x0104]
	mov si,[di]
	call common
	mov [di+2],ax
	ret
main endp

intermediate proc
	push di
	mov di,[0x0104]
	inc word ptr [di+0x30]
	call common
	pop di
	ret
intermediate endp

common proc
	mov ax,[si]
	add ax,ax
	ret
common endp
