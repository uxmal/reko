;;; mini_msfloats_regs.asm
		.i86
		
;;; This example uses mini-versions of the Microsoft floating point format
;;; used prior to adoption of IEEE format. Exponent is a 8-bit signed number,
;;; mantissa is a signed word with explicit leading 1 bit. No infinities or NaNs.

		mov ax,01
		mov cl,00
		mov bx,01
		mov ch,02
		call addition
		mov bx,05
		mov ch,03
		call multiply
		mov [0x0120],ax
		mov [0x0122],cl
		ret

addition proc
		sub cl,ch
		jc  bx_dominates
		sar bx,cl
		jmp addem

bx_dominates:
		neg cl
		sar ax,cl
		neg cl

addem:
		add ax,bx
		add cl,ch
		ret
addition endp

multiply proc
		imul bx
		add cl,ch
		ret
