  ; reg00282
  ; As reported by xor2003
  ; The procedure should decompile as:
  ; foo(byte al)
  ; but reko decompiles it as:
  ; foo(word ax, byte bl)
  ; which is incorrect.

  setmemallocstrat  proc       near  
			push    ax
			movzx   bx, al
			mov     ax, 5801h
			int     21h         ; DOS - 3+ - GET/SET MEMORY ALLOCATION STRATEGY
				; AL = function code: set allocation strategy
			pop     bx
					shr     bx, 8
			mov     ax, 5803h
			int     21h         ; DOS - 3+ - GET/SET MEMORY ALLOCATION STRATEGY
				; AL = function code: (DOS 5beta) set UMB link state
			retn
			endp