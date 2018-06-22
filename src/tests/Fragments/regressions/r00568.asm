main proc
    mov si, offset the_string
    call atoi
    mov the_value,ax
    ret

atoi proc
	xor	dx,dx
	xor	cx,cx
	xor	ah,ah
	cmp	byte ptr [si],0x2D
	jnz	l52A7

l52A3:
	inc	si
	dec	cx
	jmp	l52AD

l52A7:
	cmp	byte ptr [si],0x2B
	jnz	l52AD

l52AC:
	inc	si

l52AD:
	lodsb
	cmp	al,00
	jz	l52C7

l52B2:
	sub	al,0x30
	jc	l52C7

l52B6:
	cmp	al,0x09
	ja	l52C7

l52BA:
	add	dx,dx
	mov	bx,dx
	shl	dx,0x02
	add	dx,bx
	add	dx,ax
	jmp	l52AD

l52C7:
	and	cx,cx
	jns	l52CD

l52CB:
	neg	dx

l52CD:
	mov	ax,dx
	ret

the_string db '-42',0
the_value dw 0
