;;; String instructions.

main	proc
		lea eax,[eax+eax*4]
		mov [0x2FC],eax
		lodsd 
		mov	[0x300],eax
		lodsd
		mov [0x304],eax
		lodsw
		mov [0x0308],ax
		lodsw
		mov [0x030A],ax
		
		call loops
		ret
main endp


loops	proc
		mov cx,[0x400]
		mov al,0x00
		mov di,[0x402]
		rep scasb
		mov [0x404],di
		ret
		endp
