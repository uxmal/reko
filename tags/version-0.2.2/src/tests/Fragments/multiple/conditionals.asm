;;; A sampler of different types of conditions on x86 processors.

.i86
foo proc

	;; Good old fashioned compare
	mov	ax,[0x100]
	cmp [0x102],ax
	je	l1
	mov byte ptr[0xF00],1
l1:
	;; an arithmetic operation with condition code side effect.
	mov	ax,[0x104]
	sub [0x106],ax
	jle l2
	mov byte ptr [0xF01],1
l2:
	;; An arithmetic operation used solely for testing
	mov ax,[0x106]
	and ax,[0x108]			;; ax should be dead after this
	jz l3
	mov byte ptr [0xF02],1
	
l3:
	;; A more complex chain
	mov	ax,[0x10A]
	cmp	ax,[0x10C]
	cmc
	jc l4
	mov byte ptr [0xF03],1

l4:
	;;; Call to a function returning flag in carry.
	push 0x3
	call bar
	jnc	l5
	mov byte ptr [0xF04],1
	
l5:
	;;; Caused a failure in spacesim.
	test word ptr [0x10E],0x1FF
	jz l6
	mov byte ptr [0xF05],1
	
l6:
	;;; Another arithmetic operation with cc side effect
	sub	bp,0x10
	jl  l7
	mov	byte ptr	[bp+06],0x1

l7:
	;;; Overflows are a special case, since they don't map to C or 
	;;; any other high-level language.
	add bp,bp
	jno l8
	mov byte ptr [0xF07],1

l8:
	;;; Add more test cases here if necessary
	ret
foo endp

bar proc
	push bp
	mov bp,sp
	
	cmp word ptr [bp+4],0
	pop bp
	ret
bar endp
