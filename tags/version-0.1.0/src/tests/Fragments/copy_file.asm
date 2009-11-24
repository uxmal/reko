.i86
;;; Copies a file using MSDOS system calls

copy_file proc
	push	bp
	mov		bp,sp
	sub		sp,04
	
	mov		ax,0x3D00
	mov		dx,[bp+04]
	int		0x21			;; open filedst
	mov		[bp-02],ax	
	
	mov		ax,0x3D01
	mov		dx,[bp+06]		
	int		0x21		;; open fileSrc
	mov		[bp-04],ax
	jmp		lupe
		
write_block:	
	mov		bx,[bp-04]
	mov		dx,0x800
	mov		ax,0x4000
	int		0x21

lupe:
	mov		bx,[bp-02]
	mov		dx,0x800
	mov		cx,0x800
	mov		ax,0x3F00
	int		0x21
	jc		done
	or		ax,ax
	jnz		write_block
	
done:
	mov		sp,bp
	pop		bp
	ret
	endp
