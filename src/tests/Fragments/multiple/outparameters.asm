;;; tests several output parameter scenarios. All procedures should have the signature
;;; al = foo(char * si, char ** siOut)


main	proc
		mov si,[0x200]
		call proc25
		mov [0x310],ax
		call proc27
		mov [0x320],ax
		call proc3
		mov [0x330],ax
		call proc4
		mov [0x340],ax
		mov [0x400],si
		ret
		endp
		
;;; Straight-line output parameter scenario.

proc25	proc
		lodsw
		ret
		endp
		
;;; There are two paths to the returned siOut parameter, one directly from the caller and one modified
;;; by the lodsb instruction.

proc27	proc
		xor ax,ax
		or si,si
		jz proc27_done
		lodsw
proc27_done:
		ret
		endp

;;; Here, the si register is used after it is modified.

proc3	proc
		lodsw
		mov word ptr [si+02],0
		ret
		endp
		
;;; The si register is modified as the result of calling another function with the si
;;; register as an out parameter. The AX value of lodsw is ignored.

proc4	proc
		lodsw
		call proc25
		ret
		endp

