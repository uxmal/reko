	.i86
;;; stackvars.asm
;;;
;;; This fragment exercises the passing of parameters on stacks
;;; Includes the turbo pascal convention of leaving parameters on stacks for later
;;; calls, and passing a frame pointer as a parameter for lexical scoping.

main proc
	push bp
	mov bp,sp
	
	les bx,[bp+06]
	push es
	push bx
	call _fnPtr
	add  sp,4

	push ax
	les bx,[bp+06]
	push es
	push bx
	call _fnPtrInt
	add sp,6
		
	mov [0x200],ax
	pop bp
	ret
main endp

;;; int __cdecl _fnPtr(foo _far *);

_fnPtr proc
	push bp
	mov	bp,sp
	
	les bx,[bp+04]	
	mov ax,es:[bx+04]
	
	mov sp,bp
	pop bp
	ret
_fnPtr endp

;;; int __cdecl _fnPtrInt(foo _far *, int)

_fnPtrInt proc
	push bp
	mov bp,sp
	
	les bx,[bp+04]
	mov ax,[bp+08]
	add ax,es:[bx+06]
	
	mov sp,bp
	pop bp
	ret
_fnPtrInt endp
