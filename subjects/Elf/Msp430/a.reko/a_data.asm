;;; Segment .data (00000200)
_putchar_isr_mode		; 0200
	db	0x01
00000201    00                                            .              
pxCurrentTCB		; 0202
	dw	0x0000
uxTasksDeleted		; 0204
	dw	0x0000
uxCurrentNumberOfTasks		; 0206
	dw	0x0000
xTickCount		; 0208
	dw	0x0000
uxTopUsedPriority		; 020A
	dw	0x0000
uxTopReadyPriority		; 020C
	dw	0x0000
xSchedulerRunning		; 020E
	dw	0x0000
uxSchedulerSuspended		; 0210
	dw	0x0000
uxMissedTicks		; 0212
	dw	0x0000
uxTaskNumber.0		; 0214
	dw	0x0000
xNextFreeByte		; 0216
	dw	0x0000
usCriticalNesting		; 0218
	dw	0x000A
total_len		; 021A
	dw	0x0000
