;;; Integration test that exercises the ability to handle register pairs as 
;;; return values.
;;; The two calls to fn1 vary in the liveness of the out parameters, but we 
;;; expect the resulting return storage to be dx:ax, which is a lattice join
;;; of {ax} and {dx:ax}

.i86

main proc
	push cs
	pop ds
	call fn1		; only ax should be live-out
	mov [fn1_sliced_result],ax

	call fn1		; dx:ax should be live-out
	add ax,3		; this is a 'long add'
	adc dx,0
	mov [fn1_result],ax
	mov [fn1_result+2],dx
	ret

fn1 proc
	mov ax,[double_word]
	mov dx,[double_word+2]
	ret

double_word	dd 0x12345678
fn1_result	dd 0
fn1_sliced_result dw 0
