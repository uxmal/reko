;;; Used to test the coalescer to make sure that improper
;;; coalesces aren't performed. This includes:
;;; * Moving a load beyond a store that affects the first load
;;; * moving loads and stores across calls.

	.i86
foo proc
	mov ax,[si+0x200]
	mov word ptr [0x200],0		;; this conflicts with the statement above (si *might* be 0)
	mov [0x300],ax				;; should not be coalesced with two lines above.
	
	mov ax,[si+0x202]			;; these two are OK to coalesce
	mov [0x202],ax
	
	mov ax,[si+0x204]
	add [0x204],ax
	
	;;; Unless we know that a function call doesn't modify memory, it's 
	;;; unsafe to move loads or stores across calls.
	
	mov ax,[0x200]
	call memtrasher
	mov [0x200],ax
	
	ret
foo endp
	
memtrasher proc
	push ax
	mov word ptr [0x200],0xFFFF
	pop ax
	ret
memtrasher endp
