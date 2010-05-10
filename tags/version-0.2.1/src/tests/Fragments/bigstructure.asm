b1:
	mov ax,[si+0x200]
b2:
	cmp ax, 0
	jge b4

b3:
	mov bx,-1
	jmp b5
	
b4: 
	mov bx,1
b5:	
	mov [di+0x300],bx
	add si,2
	cmp si,0x100
	jl b1

b6:
	mov ax,[si+0x200]
	b7 b12
	
b7: b8 b9

b8: b9 b10

b10: b11

b12: b13

b13: b14

b14: b13 b15:

b15: b6
