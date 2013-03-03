.i86

driver proc
	call asciiToHex
	jc argh
	mov [0x300],al
argh:
	ret
	endp
	

asciiToHex proc
	cmp	al,0x30
	jc	not_a_digit

	cmp	al,0x39
	jbe	is_a_digit

	cmp	al,0x41
	jc	not_a_digit

	cmp	al,0x46
	jbe	is_a_digit

	cmp	al,0x61
	jc	not_a_digit

	cmp	al,0x66
	ja	not_a_digit

is_a_digit:
	sub	al,0x30
	cmp	al,0x0A
	jc	done

	sub	al,0x07
	cmp	al,0x10
	jc	done

	sub	al,0x20

done:
	clc	
	ret	

not_a_digit:
	stc	
	ret	
	endp

