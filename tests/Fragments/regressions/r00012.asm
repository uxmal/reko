main proc
	call fn1
	call fn2
	ret
	endp
	
fn1 proc
	mov	di,0000

lupe:
	mov	dx,[di+0x5388]
	push	cx
	push	di
	mov	ah,45
	call ems_release_memory
	pop	di
	pop	cx
	add	di,02
	loop	lupe

	ret
	endp	


fn2 proc
	cmp	bx,[0x53BA]
	jz	done

	cmp	bx,[0x53B8]
	jge	done

	mov	word ptr [0x53BA],bx
	add	bx,bx
	mov	dx,[bx+0x5388]
	mov	ax,0x4400
	mov	bx,0x0000
	call ems_map_memory

done:
	ret	
	endp

ems_release_memory proc
	mov [0x0110],ah
	mov [0x0112],dx
	ret
	
ems_map_memory proc
	mov [0x0114],ax
	mov [0x0116],bx
	mov [0x0118],dx
	ret
		
