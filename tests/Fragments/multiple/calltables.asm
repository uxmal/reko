;;; calltables.asm
;;; Tests to exercise analysis of call tables

.i86
main proc
	call v1_f2	; gives v1_f2 a signature.
	cmp ax,0x2
	ja done
	mov bx,ax
	add bx,bx
	call word ptr cs:[bx+vector1]
done:
	ret
main endp

vector1	dw offset v1_f0
		dw offset v1_f1
		dw offset v1_f2

v1_f0 proc
	mov ax,[si+02]
	add ax,[si+04]
	mov [si+06],ax
	ret
v1_f0 endp
	
v1_f1 proc
	mov ax,[si+02]
	sub ax,[si+04]
	mov [si+06],ax
	ret
v1_f1 endp
	
v1_f2 proc
	mov ax,[si+02]
	and ax,[bx+04]			; this should make all the functions take (si, bx) as in parameters
	mov [si+06],ax
	ret
v1_f2 endp
	
