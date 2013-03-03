;;; A slightly modified version of the famous Duff's Device. The 
;;; corresponding C code should look something like this:
;;; void duff(short * to, short * from, int count)
;;; {
;;;     int its = (c + 7) / 8;
;;;		switch (count%8) {
;;;		case 0:	do{	*to = *from++;
;;;		case 7:		*to = *from++;
;;;		case 6:		*to = *from++;
;;;		case 5:		*to = *from++;
;;;		case 4:		*to = *from++;
;;;		case 3:		*to = *from++;
;;;		case 2:		*to = *from++;
;;;		case 1:		*to = *from++;
;;;			} while(--n>0);
;;;		}
.i86
duff proc
	mov	bx,[count]
	mov cx,[count]
	and bx,0x0007
	mov si,[from]
	add bx,bx
	add cx,0x0007
	mov di,[to]
	shr cx,3
	jmp cs:[bx+duff_vector]
case_0:	movsw
case_7:	movsw
case_6:	movsw
case_5:	movsw
case_4:	movsw
case_3:	movsw
case_2:	movsw
case_1:	movsw
	dec cx
	jg case_0
	ret
	endp
		
duff_vector 
	dw offset case_0
	dw offset case_1
	dw offset case_2
	dw offset case_3
	dw offset case_4
	dw offset case_5
	dw offset case_6
	dw offset case_7
	inc ax
	inc ax

count dw 0
from  dw 0
to    dw 0
