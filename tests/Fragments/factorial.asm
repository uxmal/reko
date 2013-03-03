.i86

;;; A straight-forward factorial function + a driver program to ensure the return value 
;;; is USE'd.

	mov cx,0100
	push cx
	call factorial
	add sp,2
	mov [0100],ax
	ret

factorial proc
	push bp
	mov bp,sp

	mov ax,[bp+4]
	dec ax
	jz  base_case

	push ax
	call factorial
	inc sp
	inc sp
	mov dx,[bp+4]
	imul dx
	jmp  done

base_case:
	mov ax,1
done:
	pop	bp
	ret
factorial endp
