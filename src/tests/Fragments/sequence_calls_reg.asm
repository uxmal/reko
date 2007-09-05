	.i86
	
;;; Illustrates a sequence of calls, where the out registers of one function 
;;; (inititem) are the in parameters of the other.

	mov si,0x1234
	call driver
	mov [0x4321],ax
	ret

driver proc
	push si
	call inititem
	mov di,ax
	mov word ptr [di],0
	mov word ptr [di+0x02],0
	mov word ptr [di+0x04],0
	call allocitem
	mov ax,si
	pop si
	ret
driver endp

inititem proc
	push si
	mov word ptr [si], 0x0001
	mov word ptr [si+0x02], 0
	lea ax,[si+4]
	pop si
	ret
inititem endp

allocitem proc
	push di
	inc word ptr [si+02]
	mov ax,[si+02]
	shl ax,2
	lea di,[si+4]
	add di,ax
	pop ax
	stosw
	ret
allocitem endp
