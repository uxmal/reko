.i86

;;; Tests for compound conditions. Three base cases are tested:

;;; if (a == 1 && b != 2)

main proc
	cmp ax,1
	je t1more
t1common:
	mov word ptr [0x302],0
	jmp test2
t1more:
	cmp bx,2
	je t1common
	
	mov word ptr [0x302], 1

;;; if (a == 1 || b == 2)

test2:
	cmp ax,1
	je  t2common
	cmp bx,2
	je t2common

	mov word ptr [0x0304],0
	jmp test3
t2common:
	mov word ptr [0x0304],1

;;; if (a != 1 || b == 2)

test3:
	cmp ax,1
	je  t3common
	cmp bx,2
	je  t3succ
t3common:
	mov word ptr [0x0306],0
	jmp done
t3succ:
	mov word ptr [0x0306],1
	
done:
	ret
