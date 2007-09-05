	and ax,0xF
	and ebx,ebx
	jz done
	mov [0x300],ax
done:
	ret
