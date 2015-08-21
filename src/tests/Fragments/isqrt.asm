mane proc
	call integer_sqrt_in_c 
	mov [0x0304],si
	ret
	endp


integer_sqrt_in_c proc 
	
;  unsigned short ax = n & 0xffff;
; unsigned short dx = (n >> 16) & 0xffff;
	mov edx,eax
	shr edx,16

; unsigned short bx = 0;
; unsigned short si = 0;
; unsigned short cx = 0;
; unsigned short cf;
	xor bx,bx
	xor si,si
	xor cx,cx
	mov di,16

iter:
	or di,di
	jz done

;  int i;
;  for (i = 0; i < 16; i++) {
	sub dx,0x4000
	sbb bx,si
	sbb cx,00
	jnc  no_carry
; sub(0x4000, dx);
; sbb(si, bx);
; sbb(0, cx);
; if (cf != 0) {
	add dx,0x4000
	adc bx,si
	adc cx,0
;      add(0x4000, dx);
;     adc(si, bx);
;     adc(0, cx);
;    }
no_carry:
;    cf = 1 - cf;
	cmc
;    si += si + cf;
	adc si,si
    add ax, ax;
    adc dx, dx;
    adc bx, bx;
    adc cx, cx;

    add ax, ax
    adc dx, dx
    adc bx, bx
    adc cx, cx
	
	dec di
	jmp iter
done:
 ; return si;
  ret
	endp
