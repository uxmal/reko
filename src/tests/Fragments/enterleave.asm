;;; Test data for assembiling enter/leave instructions


foo proc
	enter
	
	mov bx,[bp+4]
	mov [bx],0
	leave
	ret
