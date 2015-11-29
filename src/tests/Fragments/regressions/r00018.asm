; r00018.asm
; the following fragment was reported by nemerle
; it crashed the Backwalker when trying to determine the 
; index used in the call table.
		.i386

		mov     ecx, [esi+0x6C]
		test    ecx, ecx
		jnz     short non_zero_ecx
		test    ebp, ebp

		jz      someplace_else

		cmp     eax, 0x29A

		jz      someplace_else

 non_zero_ecx:  
		mov     eax, [esi+0x7C]		
		push    ebp
		lea     ecx, [eax+eax*2]
		push    esi
		call    dword ptr [call_table + ecx*4]	
someplace_else:
		ret

call_table: dd offset _deflate_stored
		    dd 0x40004
		    dd 0x40008
		    dd offset _deflate_fast
		    dd 0x50004
		    dd 0x80010
		    dd offset _deflate_fast
		    dd 0x060004
		    dd 0x200020
		    dd offset _deflate_fast
		    dd 0x40004, 0x100010
		    dd offset _deflate_slow
		    dd 0x100008, 0x200020
		    dd offset _deflate_slow
		    dd 0x100008, 0x800080
			dd offset _deflate_slow

; Dummy procedures, irrelevant to code.
_deflate_stored proc
		ret
		endp

_deflate_fast proc
		ret
		endp

_deflate_slow proc
		ret
		endp
