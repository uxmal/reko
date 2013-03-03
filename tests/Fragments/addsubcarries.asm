;;; 
.i86

driver proc

	;;; Straight-line instructions
	
	mov ax,[0x200]
	mov dx,[0x202]
	mov cx,[0x204]
	mov bx,[0x206]
	add ax,cx
	adc dx,bx
	mov [0x0208],ax
	mov [0x020A],dx
	
	;;; using LES to load a DWORD (like some versions of 16-bit Turbo Pascal do)
	
	mov ax,[0x210]
	mov dx,[0x212]
	les cx,[0x214]
	mov bx,es
	sub ax,cx
	mov [0x0218],ax		;; interleaving store instructions with adc/sbc is perfectly legal but
	sbb dx,bx			;; complicates peephole analysis.
	mov [0x021A],dx

	;;; this is not a composite add like the one above, but a way to construct a bitset from two carry flag values.
	;;; The result should look something like:
	;;; w0222 = (b0220 < 0x30) * 2 + (b0221 >= 0x3A);
	
	xor ax,ax
	mov cl,[0x220]
	cmp cl,0x30
	adc ax,ax
	mov cl,[0x221]
	cmp cl,0x3A
	cmc
	adc ax,ax
	mov [0x222],ax
	
	ret	
driver endp
