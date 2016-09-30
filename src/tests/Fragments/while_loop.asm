;; A simple while loop
;; cx is live in

.i86 
	push bp
	mov bp, sp
	xor	bx,bx
	mov ax,[bp+0x06]
	or	ax,ax
	jle	negative
positive:
	mov	cx,ax
spin: 
	add bx,cx
	loop	spin
	jmp	done
negative:
	neg	ax
	mov bx,ax
done: 
	mov [0x300],bx
	pop bp
	ret
