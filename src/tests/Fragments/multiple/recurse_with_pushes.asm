; recurse_with_pushes.asm
.i86

main proc
;	mov si,0x1230
	call recurse
	ret
	endp
	
recurse proc
	push si
	call stomp_esi
	pop si
	mov ax,[si+40]
	add [0x1444],ax
	cmp [di+0x30],0
	je  recurse_done
	mov di,[di+0x30]
	call recurse
	
	
recurse_done:
	ret
	endp
	
stomp_esi proc
	mov esi,0x1234
	mov [0x4321],esi
	ret
	endp
	