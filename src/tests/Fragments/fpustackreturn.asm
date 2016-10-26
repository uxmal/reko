;;; fpustackreturn.asm
;;; Test code that leaves data on the FPU stack that a caller then reads.
;;; This is the calling convention on many x86/87 based platforms when 
;;; returning floats or doubles.

main proc
    mov bx,[0x300]
	call compute_something
	fstp qword ptr [0x400]
	ret

compute_something proc
							; FPU stack contents:
	fld qword ptr [bx]		; [bx+0]
	fld qword ptr [bx+8]	; [bx+0] [bx+8]
	faddp					; sum
	ret						; At this point the FPU stack is unbalanced
