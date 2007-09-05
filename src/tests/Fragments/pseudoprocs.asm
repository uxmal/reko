	.i86
	lea	si,[bp+0x0C]
	out	dx,al
	ror	bx,1
	rcr	cx,1
	inc	dx
	in	al,dx
	stosb
	bt byte ptr [0x3000],al
	bt cx,ax
	bt cx,2
	bsr cx,word ptr[si]
	bsr ecx,eax
	
	ret	4
