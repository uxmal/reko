  ; reg00282
  ; As reported by xor2003
  ; The procedure should decompile as:
  ; foo(word ax)
  ; but reko decompiles it as:
  ; foo(word ax, byte bl)
  ; which is incorrect.

  setmemallocstrat  proc       near  
			push    ax
			movzx   bx, al
			mov     ax, 0x5801
			int     0x21         ; DOS - 3+ - GET/SET MEMORY ALLOCATION STRATEGY
				; AL = function code: set allocation strategy
			pop     bx
					shr     bx, 8
			mov     ax, 0x5803
			int     0x21         ; DOS - 3+ - GET/SET MEMORY ALLOCATION STRATEGY
				; AL = function code: (DOS 5beta) set UMB link state
			ret
			endp
