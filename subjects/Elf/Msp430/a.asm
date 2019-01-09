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
;;; Segment .bss (0000021C)
xRxedChars		; 021C
	dw	0x0000
xCharsForTx		; 021E
	dw	0x0000
sTHREEmpty		; 0220
	dw	0x0000
pxReadyTasksLists		; 0222
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00
xDelayedTaskList1		; 0262
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00
xDelayedTaskList2		; 0272
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00
pxDelayedTaskList		; 0282
	dw	0x0000
pxOverflowDelayedTaskList		; 0284
	dw	0x0000
xPendingReadyList		; 0286
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00
xTasksWaitingTermination		; 0296
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00
xHeap		; 02A6
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
	db	0x00, 0x00
__write_char		; 09B2
	dw	0x0000
;;; Segment .text (00004000)

;; fn00004000: 00004000
fn00004000 proc
	mov.w	#5A80,&0120
	mov.w	#5B78,r15
	mov.w	#0200,r14
	mov.w	#021C,r13
	cmp.w	r14,r13
	jz	00004020

l00004016:
	mov.b	@r15+,@r14
	add.w	#0001,r14
	cmp.w	r13,r14
	jnc	00004016

l00004020:
	mov.w	#021C,r15
	mov.w	#09B4,r13
	cmp.w	r15,r13
	jz	00004036

l0000402C:
	mov.b	#00,@r15
	add.w	#0001,r15
	cmp.w	r13,r15
	jnc	0000402C

l00004036:
	mov.w	#414C,pc
	mov.w	#403E,pc
	reti
	invalid
	jz	000042FA
	add.b	@r13,r4
	invalid

;; task_idle: 4048
task_idle proc
	push.w	r11
	push.w	r10
	push.w	r9
	call	481E
	mov.w	r15,r10
	add.w	#03E8,r10
	mov.w	#0000,r9
	add.w	#0001,r9
	call	481E
	mov.w	r15,r11
	mov.w	r10,r15
	sub.w	r11,r15
	cmp.w	#0001,r15
	jge	4080
	push.w	r9
	push.w	#4040
	call	5308
	mov.w	r11,r10
	add.w	#03E8,r10
	mov.w	#0000,r9
	add.w	#0004,sp
	jmp	405A
	mov.w	#0000,r15
	invalid
	add.w	#0001,r15
	cmp.w	#030D,r15
	jl	4082
	jmp	405A
	mov.w	@sp+,r9
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; task_1: 4096
task_1 proc
	sub.w	#0002,sp
	call	481E
	mov.w	r15,@sp

l40A0:
	xor.b	#01,&0031
	mov.w	#0058,r15
	call	43A2
	mov.w	#01F4,r14
	mov.w	sp,r15
	call	461A
	jmp	40A0
40B8                         21 53 30 41                     !S0A   

;; task_2: 40BC
task_2 proc
	sub.w	#0002,sp
	call	481E
	mov.w	r15,@sp

l40C6:
	xor.b	#02,&0031
	mov.w	#0059,r15
	call	43A2
	mov.w	#00FA,r14
	mov.w	sp,r15
	call	461A
	jmp	40C6
40DE                                           21 53               !S
40E0 30 41                                           0A             

;; task_3: 40E2
task_3 proc
	sub.w	#0002,sp
	call	481E
	mov.w	r15,@sp

l40EC:
	xor.b	#04,&0031
	mov.w	#005A,r15
	call	43A2
	mov.w	#0019,r14
	mov.w	sp,r15
	call	461A
	jmp	40EC
4104             21 53 30 41 31 38 3A 30 31 3A 31 37     !S0A18:01:17
4110 00 46 65 62 20 32 30 20 32 30 30 36 00 0A 0A 4D .Feb 20 2006...M
4120 53 50 34 33 30 46 31 34 38 20 46 72 65 65 52 54 SP430F148 FreeRT
4130 4F 53 20 64 65 6D 6F 20 70 72 6F 67 72 61 6D 2E OS demo program.
4140 20 28 25 73 20 25 73 29 0A 0A 00 00              (%s %s)....   

;; main: 414C
main proc
	mov.w	#0A00,sp
	mov.w	#5A80,&0120
	mov.b	#FFE0,&0056
	mov.b	#0007,&0057
	mov.b	#0007,&0032
	mov.b	#0007,&0031
	push.b	#0010
	push.w	#0000
	push.w	#8000
	mov.w	#2580,r13
	mov.w	#0000,r14
	mov.b	#0010,r15
	call	42CC
	mov.b	#00,r15
	call	439C
	push.w	#4108
	push.w	#4111
	push.w	#411D
	call	5308
	mov.b	#01,r15
	call	439C
	push.w	#0000
	push.w	#0003
	mov.w	#0000,r12
	mov.w	#0032,r13
	mov.w	#414B,r14
	mov.w	#4096,r15
	call	44B4
	push.w	#0000
	push.w	#0003
	mov.w	#0000,r12
	mov.w	#0032,r13
	mov.w	#414B,r14
	mov.w	#40BC,r15
	call	44B4
	push.w	#0000
	push.w	#0003
	mov.w	#0000,r12
	mov.w	#0032,r13
	mov.w	#414B,r14
	mov.w	#40E2,r15
	call	44B4
	push.w	#0000
	push.w	#0000
	mov.w	#0000,r12
	mov.w	#0096,r13
	mov.w	#414B,r14
	mov.w	#4048,r15
	call	44B4
	call	4702
	add.w	#001C,sp
	mov.w	#0000,r15
	mov.w	#5AD8,pc

;; msp430_compute_modulator_bits: 420E
msp430_compute_modulator_bits proc
	push.w	r11
	push.w	r10
	push.w	r9
	push.w	r8
	push.w	r7
	push.w	r6
	push.w	r5
	push.w	r4
	sub.w	#0006,sp
	mov.w	#0018,r11
	add.w	sp,r11
	mov.w	r14,r4
	mov.w	r15,r5
	mov.w	r12,r6
	mov.w	r13,r7
	mov.w	@r11,@sp
	mov.w	r14,r10
	mov.w	r15,r11
	call	5B04
	mov.w	r12,0002(sp)
	mov.w	r13,0004(sp)
	mov.w	0002(sp),r15
	mov.w	r15,r14
	mov.w	#0000,r15
	mov.w	r4,r10
	mov.w	r5,r11
	mov.w	r14,r12
	mov.w	r15,r13
	push.w	sr
	invalid
	call	5ADC
	invalid
	mov.w	r14,r8
	mov.w	r15,r9
	sub.w	r6,r8
	subc.w	r7,r9
	cmp.w	#0000,@sp
	jz	4274
	mov.w	@sp,r15
	mov.w	0002(sp),@r15
	mov.b	#00,r7
	mov.w	#0000,r10
	mov.w	#0000,r11
	mov.b	#00,r6
	add.w	r8,r10
	addc.w	r9,r11
	mov.w	r4,r14
	mov.w	r5,r15
	xor.w	#FFFF,r14
	xor.w	#FFFF,r15
	add.w	#0001,r14
	addc.w	#0000,r15
	mov.w	r10,r12
	mov.w	r11,r13
	add.w	r12,r12
	addc.w	r13,r13
	sub.w	r14,r12
	subc.w	r15,r13
	jge	42AE
	add.w	r4,r10
	addc.w	r5,r11
	mov.w	#0001,r15
	mov.b	r6,r14
	cmp.w	#0000,r14
	jz	42AC
	add.w	r15,r15
	sub.w	#0001,r14
	jnz	42A6
	bis.b	r15,r7
	add.b	#01,r6
	cmp.b	#08,r6
	jnc	427C
	mov.b	r7,r15
	add.w	#0006,sp
	mov.w	@sp+,r4
	mov.w	@sp+,r5
	mov.w	@sp+,r6
	mov.w	@sp+,r7
	mov.w	@sp+,r8
	mov.w	@sp+,r9
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; init_uart_isr: 42CC
init_uart_isr proc
	push.w	r11
	push.w	r10
	push.w	r9
	push.w	r8
	push.w	r7
	push.w	r6
	push.w	r4
	sub.w	#0002,sp
	mov.w	sp,r4
	mov.w	#0012,r12
	add.w	sp,r12
	mov.b	r15,r6
	mov.w	r13,r7
	mov.w	r14,r8
	mov.w	@r12+,r9
	mov.w	@r12+,r10
	sub.w	#0004,r12
	mov.b	0004(r12),r15
	invalid
	add.w	#0001,&0218
	mov.w	r15,r11
	mov.w	#0001,r14
	call	4CC4
	mov.w	r15,&021C
	mov.w	#0001,r14
	mov.w	r11,r15
	call	4CC4
	mov.w	r15,&021E
	mov.b	#01,&0078
	bis.b	#0010,&0078
	and.b	#0030,r6
	mov.b	r6,&0079
	push.w	r4
	mov.w	r9,r12
	mov.w	r10,r13
	mov.w	r7,r14
	mov.w	r8,r15
	call	420E
	mov.b	r15,r14
	mov.b	@r4,&007C
	mov.w	@r4,r15
	swpb	r15
	and.b	#FF,r15
	mov.b	r15,&007D
	mov.b	r14,&007B
	bis.b	#0030,&0005
	mov.b	#0010,&0078
	bis.b	#FFC0,&001B
	bis.b	#0030,&0001
	add.w	#0002,sp
	cmp.w	#0000,&0218
	jz	436E
	add.w	#FFFF,&0218
	jnz	436E
	invalid
	add.w	#0002,sp
	mov.w	@sp+,r4
	mov.w	@sp+,r6
	mov.w	@sp+,r7
	mov.w	@sp+,r8
	mov.w	@sp+,r9
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; getchar: 4380
getchar proc
	sub.w	#0002,sp
	mov.w	#0064,r14
	mov.w	sp,r15
	call	43E4
	cmp.w	#0000,r15
	jz	4396

l4390:
	mov.b	@sp,r15
	sxt.w	r15
	jmp	4398

l4396:
	mov.w	#FFFF,r15
	add.w	#0002,sp
	mov.w	@sp+,pc

;; uart_putchar_isr_mode: 439C
uart_putchar_isr_mode proc
	mov.b	r15,&0200
	mov.w	@sp+,pc

;; putchar: 000043A2
putchar proc
	push.w	r11
	mov.w	r15,r11
	cmp.w	#000A,r15
	jz	000043D6
	cmp.b	#00,&0200
	jnz	000043CA
	bit.b	#01,&0079
	jz	000043B2
	mov.b	r11,&007F
	mov.w	#0001,r15
	cmp.w	#0000,r15
	jz	000043C6
	mov.w	r11,r15
	jmp	000043E0
	mov.w	#FFFF,r15
	jmp	000043E0
	mov.w	#0064,r14
	mov.b	r11,r15
	call	43FC
	jmp	000043BE
	mov.w	#000D,r15
	call	43A2
	jmp	000043AC
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; x_getchar: 43E4
x_getchar proc
	mov.w	r14,r13
	mov.w	r15,r14
	mov.w	&021C,r15
	call	4EF0
	cmp.w	#0000,r15
	jz	43F8

l43F4:
	mov.w	#0001,r15
	mov.w	@sp+,pc

l43F8:
	mov.w	#0000,r15
	mov.w	@sp+,pc

;; x_putchar: 43FC
x_putchar proc
	sub.w	#0002,sp
	mov.b	r15,@sp
	mov.w	r14,r13
	invalid
	add.w	#0001,&0218
	cmp.w	#0001,&0220
	jz	4440
	mov.w	sp,r14
	mov.w	&021E,r15
	call	4D7E
	cmp.w	#0001,&0220
	jz	4430
	cmp.w	#0000,&0218
	jz	444A
	add.w	#FFFF,&0218
	jnz	444A
	invalid
	jmp	444A
	cmp.w	#0001,r15
	jnz	4420
	mov.w	#0000,r13
	mov.w	sp,r14
	mov.w	&021E,r15
	call	4EF0
	mov.w	#0000,&0220
	mov.b	@sp,&007F
	jmp	4420
	mov.w	#0001,r15
	add.w	#0002,sp
	mov.w	@sp+,pc

;; vRxISR: 4450
vRxISR proc
	push.w	r15
	push.w	r14
	push.w	r13
	push.w	r12
	sub.w	#0002,sp
	mov.b	&007E,@sp
	mov.w	#0000,r13
	mov.w	sp,r14
	mov.w	&021C,r15
	call	4E84
	cmp.w	#0000,r15
	jz	4474
	call	523A
	add.w	#0002,sp
	mov.w	@sp+,r12
	mov.w	@sp+,r13
	mov.w	@sp+,r14
	mov.w	@sp+,r15
	reti

;; vTxISR: 4480
vTxISR proc
	push.w	r15
	push.w	r14
	push.w	r13
	push.w	r12
	sub.w	#0004,sp
	mov.w	sp,r13
	mov.w	sp,r14
	add.w	#0002,r14
	mov.w	&021E,r15
	call	4FF6
	cmp.w	#0001,r15
	jz	44A2
	mov.w	#0001,&0220
	jmp	44A8
	mov.b	0002(sp),&007F
	add.w	#0004,sp
	mov.w	@sp+,r12
	mov.w	@sp+,r13
	mov.w	@sp+,r14
	mov.w	@sp+,r15
	reti

;; xTaskCreate: 44B4
xTaskCreate proc
	push.w	r11
	push.w	r10
	push.w	r9
	push.w	r8
	push.w	r7
	push.w	r6
	push.w	r5
	mov.w	#0010,r11
	add.w	sp,r11
	mov.w	r15,r6
	mov.w	r14,r9
	mov.w	r13,r10
	mov.w	r12,r7
	mov.w	@r11,r8
	mov.w	0002(r11),r5
	mov.w	r13,r15
	call	4AC2
	mov.w	r15,r11
	cmp.w	#0000,r15
	jz	45A0
	mov.w	r8,r12
	mov.w	r9,r13
	mov.w	r10,r14
	call	49BE
	mov.w	0024(r11),r15
	add.w	r15,r15
	add.w	0002(r11),r15
	sub.w	#0002,r15
	mov.w	r7,r13
	mov.w	r6,r14
	call	519A
	mov.w	r15,@r11
	invalid
	add.w	#0001,&0218
	add.w	#0001,&0206
	cmp.w	#0001,&0206
	jz	4596
	cmp.w	#0000,&020E
	jnz	452A
	mov.w	&0202,r15
	mov.w	0006(r15),r15
	cmp.w	r15,r8
	jnc	452A
	mov.w	r11,&0202
	mov.w	0006(r11),r15
	cmp.w	r15,&020A
	jc	4538
	mov.w	r15,&020A
	mov.w	&0214,0004(r11)
	add.w	#0001,&0214
	mov.w	#0000,0008(r11)
	cmp.w	r15,&020C
	jc	4550
	mov.w	r15,&020C
	mov.w	r11,r14
	add.w	#0008,r14
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	#0222,r15
	call	4C06
	mov.w	#0001,r10
	cmp.w	#0000,&0218
	jz	4574
	add.w	#FFFF,&0218
	jnz	4574
	invalid
	cmp.w	#0001,r10
	jnz	45A4
	cmp.w	#0000,r5
	jz	4580
	mov.w	r11,@r5
	cmp.w	#0000,&020E
	jz	45A4
	mov.w	&0202,r15
	cmp.w	r8,0006(r15)
	jc	45A4
	call	523A
	jmp	45A4
	mov.w	r11,&0202
	call	4A12
	jmp	452A
	mov.w	#FFFF,r10
	jmp	4574
	mov.w	r10,r15
	mov.w	@sp+,r5
	mov.w	@sp+,r6
	mov.w	@sp+,r7
	mov.w	@sp+,r8
	mov.w	@sp+,r9
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; vTaskDelete: 45B6
vTaskDelete proc
	push.w	r11
	push.w	r10
	push.w	r9
	mov.w	r15,r9
	invalid
	add.w	#0001,&0218
	mov.w	r15,r11
	cmp.w	#0000,r15
	jz	460C
	mov.w	r11,r10
	add.w	#0008,r10
	mov.w	r10,r15
	call	4C98
	cmp.w	#0000,001A(r11)
	jnz	4600
	mov.w	r10,r14
	mov.w	#0296,r15
	call	4C06
	add.w	#0001,&0204
	cmp.w	#0000,&0218
	jz	45F6
	add.w	#FFFF,&0218
	jnz	45F6
	invalid
	cmp.w	#0000,r9
	jnz	4612
	call	523A
	jmp	4612
	add.w	#0012,r11
	mov.w	r11,r15
	call	4C98
	jmp	45DA
	mov.w	&0202,r11
	jmp	45CA
	mov.w	@sp+,r9
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; vTaskDelayUntil: 0000461A
vTaskDelayUntil proc
	push.w	r11
	push.w	r10
	push.w	r9
	mov.w	r15,r9
	mov.w	r14,r11
	mov.w	#0000,r10
	call	4742
	mov.w	@r9,r15
	add.w	r15,r11
	cmp.w	r15,&0208
	jc	00004690
	cmp.w	r15,r11
	jc	00004640
	cmp.w	r11,&0208
	jc	00004640
	mov.w	#0001,r10
	mov.w	r11,@r9
	cmp.w	#0000,r10
	jnz	00004656
	call	475C
	cmp.w	#0000,r15
	jnz	00004696
	call	523A
	jmp	00004696
	mov.w	&0202,r15
	add.w	#0008,r15
	call	4C98
	mov.w	&0202,r15
	mov.w	r11,0008(r15)
	mov.w	&0208,r15
	cmp.w	r15,r11
	jc	00004682
	mov.w	&0202,r15
	add.w	#0008,r15
	mov.w	r15,r14
	mov.w	&0284,r15
	call	4C32
	jmp	00004648
	mov.w	&0202,r15
	add.w	#0008,r15
	mov.w	r15,r14
	mov.w	&0282,r15
	jmp	0000467C
	cmp.w	r15,r11
	jnc	0000463E
	jmp	00004638
	mov.w	@sp+,r9
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; vTaskDelay: 469E
vTaskDelay proc
	push.w	r11
	mov.w	r15,r11
	mov.w	#0000,r15
	cmp.w	#0000,r11
	jnz	46B2
	cmp.w	#0000,r15
	jnz	46F8
	call	523A
	jmp	46F8
	call	4742
	add.w	&0208,r11
	mov.w	&0202,r15
	add.w	#0008,r15
	call	4C98
	mov.w	&0202,r15
	mov.w	r11,0008(r15)
	mov.w	&0208,r15
	cmp.w	r15,r11
	jc	46EA
	mov.w	&0202,r15
	add.w	#0008,r15
	mov.w	r15,r14
	mov.w	&0284,r15
	call	4C32
	call	475C
	jmp	46A8
	mov.w	&0202,r15
	add.w	#0008,r15
	mov.w	r15,r14
	mov.w	&0282,r15
	jmp	46E0
	mov.w	@sp+,r11
	mov.w	@sp+,pc
	mov.b	r4,r9
	mov.b	r5,r12
	invalid

;; vTaskStartScheduler: 4702
vTaskStartScheduler proc
	cmp.w	#0000,&0202
	jnz	470A

l4708:
	mov.w	@sp+,pc

l470A:
	push.w	#0000
	push.w	#0000
	mov.w	#0000,r12
	mov.w	#0032,r13
	mov.w	#46FC,r14
	mov.w	#49AC,r15
	call	44B4
	add.w	#0004,sp
	cmp.w	#0001,r15
	jnz	4708
	invalid
	mov.w	#0001,&020E
	mov.w	#0000,&0208
	call	520A
	jmp	4708

;; vTaskEndScheduler: 4736
vTaskEndScheduler proc
	invalid
	mov.w	#0000,&020E
	call	5238
	mov.w	@sp+,pc

;; vTaskSuspendAll: 4742
vTaskSuspendAll proc
	invalid
	add.w	#0001,&0218
	add.w	#0001,&0210
	cmp.w	#0000,&0218
	jz	475A
	add.w	#FFFF,&0218
	jnz	475A
	invalid
	mov.w	@sp+,pc

;; xTaskResumeAll: 475C
xTaskResumeAll proc
	push.w	r11
	push.w	r10
	push.w	r9
	push.w	r8
	mov.w	#0000,r8
	invalid
	add.w	#0001,&0218
	add.w	#FFFF,&0210
	jnz	47EA
	cmp.w	#0000,&0206
	jz	47EA
	mov.w	#0000,r9
	cmp.w	#0000,&0286
	jz	480E
	mov.w	&0288,r15
	mov.w	0002(r15),r15
	mov.w	0006(r15),r11
	cmp.w	#0000,r11
	jz	47D8
	mov.w	r11,r15
	add.w	#0012,r15
	call	4C98
	mov.w	r11,r10
	add.w	#0008,r10
	mov.w	r10,r15
	call	4C98
	mov.w	#0000,0008(r11)
	mov.w	0006(r11),r15
	cmp.w	r15,&020C
	jc	47B6
	mov.w	r15,&020C
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	#0222,r15
	mov.w	r10,r14
	call	4C06
	mov.w	&0202,r15
	cmp.w	0006(r11),0006(r15)
	jc	477A
	mov.w	#0001,r9
	jmp	477A
	cmp.w	#0000,&0212
	jz	47E6
	cmp.w	#0000,&0212
	jnz	4802
	mov.w	#0001,r9
	cmp.w	#0001,r9
	jz	47FA
	cmp.w	#0000,&0218
	jz	4812
	add.w	#FFFF,&0218
	jnz	4812
	invalid
	jmp	4812
	mov.w	#0001,r8
	call	523A
	jmp	47EA
	call	484A
	add.w	#FFFF,&0212
	jnz	4802
	jmp	47E4
	mov.w	#0000,r11
	jmp	478C
	mov.w	r8,r15
	mov.w	@sp+,r8
	mov.w	@sp+,r9
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; xTaskGetTickCount: 0000481E
xTaskGetTickCount proc
	invalid
	add.w	#0001,&0218
	mov.w	&0208,r15
	jz	00004832
	add.w	#FFFF,&0218
	jnz	00004832
	invalid
	mov.w	@sp+,pc

;; uxTaskGetNumberOfTasks: 4834
uxTaskGetNumberOfTasks proc
	invalid
	add.w	#0001,&0218
	mov.w	&0206,r15
	jz	4848
	add.w	#FFFF,&0218
	jnz	4848
	invalid
	mov.w	@sp+,pc

;; vTaskIncrementTick: 484A
vTaskIncrementTick proc
	push.w	r11
	push.w	r10
	cmp.w	#0000,&0210
	jnz	48D0
	add.w	#0001,&0208
	jnz	4868
	mov.w	&0282,r15
	mov.w	&0284,&0282
	mov.w	r15,&0284
	mov.w	&0282,r15
	cmp.w	#0000,@r15
	jz	48CC
	mov.w	0002(r15),r15
	mov.w	0002(r15),r15
	mov.w	0006(r15),r11
	cmp.w	#0000,r11
	jz	48D4
	cmp.w	0008(r11),&0208
	jnc	48D4
	mov.w	r11,r10
	add.w	#0008,r10
	mov.w	r10,r15
	call	4C98
	cmp.w	#0000,001A(r11)
	jnz	48C0
	mov.w	#0000,0008(r11)
	mov.w	0006(r11),r15
	cmp.w	r15,&020C
	jc	48AC
	mov.w	r15,&020C
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	#0222,r15
	mov.w	r10,r14
	call	4C06
	jmp	4868
	mov.w	r11,r15
	add.w	#0012,r15
	call	4C98
	jmp	489A
	mov.w	#0000,r11
	jmp	487E
	add.w	#0001,&0212
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; vTaskPlaceOnEventList: 48DA
vTaskPlaceOnEventList proc
	push.w	r11
	mov.w	r15,r13
	mov.w	r14,r11
	mov.w	&0202,r15
	add.w	#0012,r15
	mov.w	r15,r14
	mov.w	r13,r15
	call	4C32
	add.w	&0208,r11
	mov.w	&0202,r15
	add.w	#0008,r15
	call	4C98
	mov.w	&0202,r15
	mov.w	r11,0008(r15)
	mov.w	&0208,r15
	cmp.w	r15,r11
	jc	491C
	mov.w	&0202,r15
	add.w	#0008,r15
	mov.w	r15,r14
	mov.w	&0284,r15
	jmp	4928
	mov.w	&0202,r15
	add.w	#0008,r15
	mov.w	r15,r14
	mov.w	&0282,r15
	call	4C32
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; xTaskRemoveFromEventList: 4930
xTaskRemoveFromEventList proc
	push.w	r11
	push.w	r10
	cmp.w	#0000,@r15
	jz	49A2
	mov.w	0002(r15),r15
	mov.w	0002(r15),r15
	mov.w	0006(r15),r10
	mov.w	r10,r11
	add.w	#0012,r11
	mov.w	r11,r15
	call	4C98
	cmp.w	#0000,&0210
	jnz	499A
	add.w	#FFF6,r11
	mov.w	r11,r15
	call	4C98
	mov.w	#0000,0008(r10)
	mov.w	0006(r10),r15
	cmp.w	r15,&020C
	jc	4974
	mov.w	r15,&020C
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	#0222,r15
	mov.w	r11,r14
	call	4C06
	mov.w	&0202,r15
	cmp.w	0006(r10),0006(r15)
	jc	4996
	mov.w	#0001,r15
	jmp	49A6
	mov.w	#0000,r15
	jmp	49A6
	mov.w	r11,r14
	mov.w	#0286,r15
	jmp	4982
	mov.w	#0000,r10
	jmp	4946
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; prvIdleTask: 49AC
prvIdleTask proc
	call	4A5E
	cmp.w	#0002,&0222
	jnc	49AC

l49B6:
	call	523A
	jmp	49AC
49BC                                     30 41                   0A 

;; prvInitialiseTCBVariables: 49BE
prvInitialiseTCBVariables proc
	push.w	r11
	push.w	r10
	mov.w	r15,r11
	mov.w	r14,r15
	mov.w	r13,r14
	mov.w	r12,r10
	mov.w	r15,0024(r11)
	mov.w	r11,r15
	add.w	#001C,r15
	mov.w	#0008,r13
	call	5962
	mov.b	#00,0023(r11)
	cmp.w	#0004,r10
	jnc	49E6
	mov.w	#0003,r10
	mov.w	r10,0006(r11)
	mov.w	r11,r15
	add.w	#0008,r15
	call	4C00
	mov.w	r11,r15
	add.w	#0012,r15
	call	4C00
	mov.w	r11,000E(r11)
	mov.w	#0004,r15
	sub.w	r10,r15
	mov.w	r15,0012(r11)
	mov.w	r11,0018(r11)
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; prvInitialiseTaskLists: 4A12
prvInitialiseTaskLists proc
	push.w	r11
	mov.w	#0000,r11
	mov.w	r11,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	#0222,r15
	call	4BD4
	add.w	#0001,r11
	cmp.w	#0004,r11
	jnc	4A16
	mov.w	#0262,r15
	call	4BD4
	mov.w	#0272,r15
	call	4BD4
	mov.w	#0286,r15
	call	4BD4
	mov.w	#0296,r15
	call	4BD4
	mov.w	#0262,&0282
	mov.w	#0272,&0284
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; prvCheckTasksWaitingTermination: 4A5E
prvCheckTasksWaitingTermination proc
	push.w	r11
	cmp.w	#0000,&0204
	jz	4ABE
	call	4742
	mov.w	#0000,r11
	cmp.w	#0000,&0296
	jnz	4A74
	mov.w	#0001,r11
	call	475C
	cmp.w	#0000,r11
	jnz	4ABE
	invalid
	add.w	#0001,&0218
	cmp.w	#0000,&0296
	jz	4ABA
	mov.w	&0298,r15
	mov.w	0002(r15),r15
	mov.w	0006(r15),r11
	mov.w	r11,r15
	add.w	#0008,r15
	call	4C98
	add.w	#FFFF,&0206
	add.w	#FFFF,&0204
	cmp.w	#0000,&0218
	jz	4AB2
	add.w	#FFFF,&0218
	jnz	4AB2
	invalid
	mov.w	r11,r15
	call	4B02
	jmp	4ABE
	mov.w	#0000,r11
	jmp	4A94
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; prvAllocateTCBAndStack: 4AC2
prvAllocateTCBAndStack proc
	push.w	r11
	push.w	r10
	mov.w	r15,r10
	mov.w	#0026,r15
	call	5156
	mov.w	r15,r11
	cmp.w	#0000,r15
	jz	4AFA
	add.w	r10,r10
	mov.w	r10,r15
	call	5156
	mov.w	r15,0002(r11)
	cmp.w	#0000,r15
	jnz	4AF0
	mov.w	r11,r15
	call	5192
	mov.w	#0000,r11
	jmp	4AFA
	mov.w	r10,r13
	mov.w	#00A5,r14
	call	5A68
	mov.w	r11,r15
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; prvDeleteTCB: 4B02
prvDeleteTCB proc
	push.w	r11
	mov.w	r15,r11
	mov.w	0002(r15),r15
	call	5192
	mov.w	r11,r15
	call	5192
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; vTaskSwitchContext: 4B18
vTaskSwitchContext proc
	cmp.w	#0000,&0210
	jnz	4BAE

l4B1E:
	mov.w	&020C,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	cmp.w	#0000,0222(r15)
	jnz	4B46

l4B30:
	add.w	#FFFF,&020C
	mov.w	&020C,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	cmp.w	#0000,0222(r15)
	jz	4B30

l4B46:
	mov.w	#0222,r13
	mov.w	&020C,r14
	add.w	r14,r14
	add.w	r14,r14
	add.w	r14,r14
	add.w	r14,r14
	add.w	r13,r14
	mov.w	&020C,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r13,r15
	mov.w	0004(r15),r15
	mov.w	0002(r15),0004(r14)
	mov.w	&020C,r14
	add.w	r14,r14
	add.w	r14,r14
	add.w	r14,r14
	add.w	r14,r14
	add.w	r13,r14
	mov.w	&020C,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r13,r15
	mov.w	0004(r14),r14
	mov.w	0002(r15),r15
	cmp.w	r15,r14
	jz	4BB0

l4B98:
	mov.w	&020C,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	mov.w	0226(r15),r15
	mov.w	0006(r15),&0202

l4BAE:
	mov.w	@sp+,pc

l4BB0:
	mov.w	&020C,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	add.w	r15,r15
	mov.w	&020C,r14
	add.w	r14,r14
	add.w	r14,r14
	add.w	r14,r14
	add.w	r14,r14
	mov.w	0226(r14),r14
	mov.w	0002(r14),0226(r15)
	jmp	4B98

;; vListInitialise: 4BD4
vListInitialise proc
	push.w	r11
	mov.w	r15,r11
	add.w	#0006,r15
	mov.w	r15,0002(r11)
	mov.w	r15,0004(r11)
	mov.w	#FFFF,0006(r11)
	mov.w	r15,0008(r11)
	mov.w	r15,000A(r11)
	mov.w	#0000,000C(r11)
	call	4C00
	mov.w	#0000,@r11
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; vListInitialiseItem: 4C00
vListInitialiseItem proc
	mov.w	#0000,0008(r15)
	mov.w	@sp+,pc

;; vListInsertEnd: 4C06
vListInsertEnd proc
	mov.w	r15,r12
	mov.w	0004(r15),r13
	mov.w	0002(r13),0002(r14)
	mov.w	0004(r15),0004(r14)
	mov.w	0002(r13),r15
	mov.w	r14,0004(r15)
	mov.w	r14,0002(r13)
	mov.w	r14,0004(r12)
	mov.w	r12,0008(r14)
	add.w	#0001,@r12
	mov.w	@sp+,pc

;; vListInsert: 4C32
vListInsert proc
	push.w	r11
	mov.w	r15,r11
	mov.w	@r14,r12
	cmp.w	#FFFF,r12
	jz	4C5A
	mov.w	0002(r15),r13
	mov.w	0002(r13),r15
	mov.w	@r15,r15
	cmp.w	r15,r12
	jnc	4C76
	mov.w	0002(r13),r13
	mov.w	0002(r13),r15
	mov.w	@r15,r15
	cmp.w	r15,r12
	jc	4C4A
	jmp	4C76
	mov.w	0002(r15),r13
	mov.w	0002(r13),r15
	cmp.w	#FFFF,@r15
	jc	4C76
	mov.w	0002(r13),r13
	mov.w	0002(r13),r15
	cmp.w	r12,@r15
	jnc	4C68
	mov.w	0002(r13),0002(r14)
	mov.w	0002(r14),r15
	mov.w	r14,0004(r15)
	mov.w	r13,0004(r14)
	mov.w	r14,0002(r13)
	mov.w	r11,0008(r14)
	add.w	#0001,@r11
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; vListRemove: 4C98
vListRemove proc
	mov.w	r15,r14
	mov.w	0002(r15),r15
	mov.w	0004(r14),0004(r15)
	mov.w	0004(r14),r13
	mov.w	r15,0002(r13)
	mov.w	0008(r14),r15
	cmp.w	r14,0004(r15)
	jnz	4CBA

l4CB6:
	mov.w	r13,0004(r15)

l4CBA:
	mov.w	#0000,0008(r14)
	add.w	#FFFF,@r15
	mov.w	@sp+,pc

;; xQueueCreate: 4CC4
xQueueCreate proc
	push.w	r11
	push.w	r10
	push.w	r9
	mov.w	r15,r10
	mov.w	r14,r9
	cmp.w	#0000,r15
	jnz	4CD6
	mov.w	#0000,r15
	jmp	4D76
	mov.w	#0032,r15
	call	5156
	mov.w	r15,r11
	cmp.w	#0000,r15
	jz	4CD2
	push.w	sr
	invalid
	invalid
	mov.w	r10,&0132
	mov.w	r9,&0138
	mov.w	&013A,r15
	invalid
	add.w	#0001,r15
	call	5156
	mov.w	r15,@r11
	cmp.w	#0000,r15
	jz	4D6E
	push.w	sr
	invalid
	invalid
	mov.w	r10,&0132
	mov.w	r9,&0138
	mov.w	&013A,r15
	invalid
	mov.w	@r11,r14
	add.w	r14,r15
	mov.w	r15,0002(r11)
	mov.w	#0000,0028(r11)
	mov.w	r14,0004(r11)
	add.w	#FFFF,r10
	push.w	sr
	invalid
	invalid
	mov.w	r10,&0132
	add.w	#0001,r10
	mov.w	r9,&0138
	mov.w	&013A,r15
	invalid
	add.w	@r11,r15
	mov.w	r15,0006(r11)
	mov.w	r10,002A(r11)
	mov.w	r9,002C(r11)
	mov.w	#FFFF,002E(r11)
	mov.w	#FFFF,0030(r11)
	mov.w	r11,r15
	add.w	#0008,r15
	call	4BD4
	mov.w	r11,r15
	add.w	#0018,r15
	call	4BD4
	mov.w	r11,r15
	jmp	4D76
	mov.w	r11,r15
	call	5192
	jmp	4CD2
	mov.w	@sp+,r9
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; xQueueSend: 4D7E
xQueueSend proc
	push.w	r11
	push.w	r10
	push.w	r9
	mov.w	r15,r11
	mov.w	r14,r9
	mov.w	r13,r10
	call	4742
	invalid
	add.w	#0001,&0218
	add.w	#0001,002E(r11)
	add.w	#0001,0030(r11)
	cmp.w	#0000,&0218
	jz	4DAA
	add.w	#FFFF,&0218
	jnz	4DAA
	invalid
	mov.w	r11,r15
	call	512E
	cmp.w	#0000,r15
	jz	4DB8
	cmp.w	#0000,r10
	jnz	4E26
	invalid
	add.w	#0001,&0218
	cmp.w	002A(r11),0028(r11)
	jnc	4DF6
	mov.w	#FFFD,r10
	cmp.w	#0000,&0218
	jz	4DD8
	add.w	#FFFF,&0218
	jnz	4DD8
	invalid
	mov.w	r11,r15
	call	5092
	cmp.w	#0000,r15
	jz	4DF0
	call	475C
	cmp.w	#0000,r15
	jnz	4E7A
	call	523A
	jmp	4E7A
	call	475C
	jmp	4E7A
	mov.w	0004(r11),r15
	mov.w	002C(r11),r13
	mov.w	r9,r14
	call	5994
	add.w	#0001,0028(r11)
	mov.w	0004(r11),r15
	add.w	002C(r11),r15
	mov.w	r15,0004(r11)
	cmp.w	0002(r11),r15
	jnc	4E1E
	mov.w	@r11,0004(r11)
	mov.w	#0001,r10
	add.w	#0001,0030(r11)
	jmp	4DCA
	mov.w	r11,r15
	add.w	#0008,r15
	mov.w	r10,r14
	call	48DA
	invalid
	add.w	#0001,&0218
	mov.w	r11,r15
	call	5092
	call	475C
	cmp.w	#0000,r15
	jz	4E74
	call	4742
	invalid
	add.w	#0001,&0218
	add.w	#0001,002E(r11)
	add.w	#0001,0030(r11)
	cmp.w	#0000,&0218
	jz	4E64
	add.w	#FFFF,&0218
	jnz	4E64
	invalid
	cmp.w	#0000,&0218
	jz	4DB8
	add.w	#FFFF,&0218
	jnz	4DB8
	invalid
	jmp	4DB8
	call	523A
	jmp	4E44
	mov.w	r10,r15
	mov.w	@sp+,r9
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; xQueueSendFromISR: 4E84
xQueueSendFromISR proc
	push.w	r11
	push.w	r10
	mov.w	r15,r11
	mov.w	r13,r10
	cmp.w	002A(r15),0028(r15)
	jnc	4E98
	mov.w	r10,r13
	jmp	4EE8
	mov.w	0004(r15),r15
	mov.w	002C(r11),r13
	call	5994
	add.w	#0001,0028(r11)
	mov.w	0004(r11),r15
	add.w	002C(r11),r15
	mov.w	r15,0004(r11)
	cmp.w	0002(r11),r15
	jnc	4EBE
	mov.w	@r11,0004(r11)
	mov.w	0030(r11),r15
	cmp.w	#FFFF,r15
	jz	4ECE
	add.w	#0001,r15
	mov.w	r15,0030(r11)
	jmp	4E94
	cmp.w	#0000,r10
	jnz	4E94
	cmp.w	#0000,0018(r11)
	jz	4E94
	add.w	#0018,r11
	mov.w	r11,r15
	call	4930
	cmp.w	#0000,r15
	jz	4E94
	mov.w	#0001,r13
	mov.w	r13,r15
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; xQueueReceive: 4EF0
xQueueReceive proc
	push.w	r11
	push.w	r10
	push.w	r9
	mov.w	r15,r11
	mov.w	r14,r9
	mov.w	r13,r10
	call	4742
	invalid
	add.w	#0001,&0218
	add.w	#0001,002E(r11)
	add.w	#0001,0030(r11)
	cmp.w	#0000,&0218
	jz	4F1C
	add.w	#FFFF,&0218
	jnz	4F1C
	invalid
	mov.w	r11,r15
	call	510C
	cmp.w	#0000,r15
	jz	4F2A
	cmp.w	#0000,r10
	jnz	4F96
	invalid
	add.w	#0001,&0218
	mov.w	0028(r11),r14
	cmp.w	#0000,r14
	jz	4F92
	mov.w	002C(r11),r13
	mov.w	0006(r11),r15
	add.w	r13,r15
	mov.w	r15,0006(r11)
	cmp.w	0002(r11),r15
	jnc	4F50
	mov.w	@r11,0006(r11)
	add.w	#FFFF,r14
	mov.w	r14,0028(r11)
	mov.w	0006(r11),r14
	mov.w	r9,r15
	call	5994
	add.w	#0001,002E(r11)
	mov.w	#0001,r10
	cmp.w	#0000,&0218
	jz	4F74
	add.w	#FFFF,&0218
	jnz	4F74
	invalid
	mov.w	r11,r15
	call	5092
	cmp.w	#0000,r15
	jz	4F8C
	call	475C
	cmp.w	#0000,r15
	jnz	4FEC
	call	523A
	jmp	4FEC
	call	475C
	jmp	4FEC
	mov.w	#0000,r10
	jmp	4F66
	mov.w	r11,r15
	add.w	#0018,r15
	mov.w	r10,r14
	call	48DA
	invalid
	add.w	#0001,&0218
	mov.w	r11,r15
	call	5092
	call	475C
	cmp.w	#0000,r15
	jz	4FE6
	call	4742
	invalid
	add.w	#0001,&0218
	add.w	#0001,002E(r11)
	add.w	#0001,0030(r11)
	cmp.w	#0000,&0218
	jz	4FD6
	add.w	#FFFF,&0218
	jnz	4FD6
	invalid
	cmp.w	#0000,&0218
	jz	4F2A
	add.w	#FFFF,&0218
	jnz	4F2A
	invalid
	jmp	4F2A
	call	523A
	jmp	4FB6
	mov.w	r10,r15
	mov.w	@sp+,r9
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; xQueueReceiveFromISR: 4FF6
xQueueReceiveFromISR proc
	push.w	r11
	push.w	r10
	mov.w	r15,r11
	mov.w	r14,r12
	mov.w	r13,r10
	mov.w	0028(r15),r14
	cmp.w	#0000,r14
	jz	5060
	mov.w	002C(r15),r13
	mov.w	0006(r15),r15
	add.w	r13,r15
	mov.w	r15,0006(r11)
	cmp.w	0002(r11),r15
	jnc	5020
	mov.w	@r11,0006(r11)
	add.w	#FFFF,r14
	mov.w	r14,0028(r11)
	mov.w	0006(r11),r14
	mov.w	r12,r15
	call	5994
	mov.w	002E(r11),r15
	cmp.w	#FFFF,r15
	jz	5042
	add.w	#0001,r15
	mov.w	r15,002E(r11)
	mov.w	#0001,r15
	jmp	5062
	cmp.w	#0000,@r10
	jnz	503E
	cmp.w	#0000,0008(r11)
	jz	503E
	add.w	#0008,r11
	mov.w	r11,r15
	call	4930
	cmp.w	#0000,r15
	jz	503E
	mov.w	#0001,@r10
	jmp	503E
	mov.w	#0000,r15
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; uxQueueMessagesWaiting: 5068
uxQueueMessagesWaiting proc
	invalid
	add.w	#0001,&0218
	mov.w	0028(r15),r15
	jz	507C
	add.w	#FFFF,&0218
	jnz	507C
	invalid
	mov.w	@sp+,pc

;; vQueueDelete: 507E
vQueueDelete proc
	push.w	r11
	mov.w	r15,r11
	mov.w	@r15,r15
	call	5192
	mov.w	r11,r15
	call	5192
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; prvUnlockQueue: 5092
prvUnlockQueue proc
	push.w	r11
	push.w	r10
	mov.w	r15,r11
	mov.w	#0000,r10
	invalid
	add.w	#0001,&0218
	add.w	#FFFF,0030(r15)
	jn	50B0
	mov.w	#FFFF,0030(r15)
	cmp.w	#0000,0018(r15)
	jnz	50F4
	cmp.w	#0000,&0218
	jz	50BE
	add.w	#FFFF,&0218
	jnz	50BE
	invalid
	invalid
	add.w	#0001,&0218
	add.w	#FFFF,002E(r11)
	jn	50D4
	mov.w	#FFFF,002E(r11)
	cmp.w	#0000,0008(r11)
	jnz	50E4
	cmp.w	#0000,&0218
	jz	5104
	add.w	#FFFF,&0218
	jnz	5104
	invalid
	jmp	5104
	add.w	#0008,r11
	mov.w	r11,r15
	call	4930
	cmp.w	#0000,r15
	jz	50D4
	mov.w	#0001,r10
	jmp	50D4
	add.w	#0018,r15
	call	4930
	cmp.w	#0000,r15
	jz	50B0
	mov.w	#0001,r10
	jmp	50B0
	mov.w	r10,r15
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; prvIsQueueEmpty: 510C
prvIsQueueEmpty proc
	invalid
	add.w	#0001,&0218
	mov.w	#0000,r14
	cmp.w	#0000,0028(r15)
	jnz	511C
	mov.w	#0001,r14
	cmp.w	#0000,&0218
	jz	512A
	add.w	#FFFF,&0218
	jnz	512A
	invalid
	mov.w	r14,r15
	mov.w	@sp+,pc

;; prvIsQueueFull: 512E
prvIsQueueFull proc
	invalid
	add.w	#0001,&0218
	mov.w	#0000,r14
	cmp.w	002A(r15),0028(r15)
	jz	514E
	cmp.w	#0000,&0218
	jz	5152
	add.w	#FFFF,&0218
	jnz	5152
	invalid
	jmp	5152
	mov.w	#0001,r14
	jmp	513E
	mov.w	r14,r15
	mov.w	@sp+,pc

;; pvPortMalloc: 5156
pvPortMalloc proc
	push.w	r11
	push.w	r10
	mov.w	r15,r11
	mov.w	#0000,r10
	and.w	#0001,r15
	jz	5166
	sub.w	r15,r11
	add.w	#0002,r11
	call	4742
	mov.w	&0216,r14
	mov.w	r14,r15
	add.w	r11,r15
	cmp.w	#0708,r15
	jc	5186
	cmp.w	r15,r14
	jc	5186
	mov.w	r14,r10
	add.w	#02AA,r10
	mov.w	r15,&0216
	call	475C
	mov.w	r10,r15
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; vPortFree: 5192
vPortFree proc
	mov.w	@sp+,pc

;; vPortInitialiseBlocks: 5194
vPortInitialiseBlocks proc
	mov.w	#0000,&0216
	mov.w	@sp+,pc

;; pxPortInitialiseStack: 519A
pxPortInitialiseStack proc
	mov.w	r14,@r15
	sub.w	#0002,r15
	mov.w	#0008,@r15
	sub.w	#0002,r15
	mov.w	#4444,@r15
	sub.w	#0002,r15
	mov.w	#5555,@r15
	sub.w	#0002,r15
	mov.w	#6666,@r15
	sub.w	#0002,r15
	mov.w	#7777,@r15
	sub.w	#0002,r15
	mov.w	#8888,@r15
	sub.w	#0002,r15
	mov.w	#9999,@r15
	sub.w	#0002,r15
	mov.w	#AAAA,@r15
	sub.w	#0002,r15
	mov.w	#BBBB,@r15
	sub.w	#0002,r15
	mov.w	#CCCC,@r15
	sub.w	#0002,r15
	mov.w	#DDDD,@r15
	sub.w	#0002,r15
	mov.w	#EEEE,@r15
	sub.w	#0002,r15
	mov.w	r13,@r15
	sub.w	#0002,r15
	mov.w	#0000,@r15
	mov.w	@sp+,pc

;; xPortStartScheduler: 520A
xPortStartScheduler proc
	call	528E
	mov.w	-500E(pc),r12
	mov.w	@r12,sp
	mov.w	@sp+,r15
	mov.w	r15,-5000(pc)
	mov.w	@sp+,r15
	mov.w	@sp+,r14
	mov.w	@sp+,r13
	mov.w	@sp+,r12
	mov.w	@sp+,r11
	mov.w	@sp+,r10
	mov.w	@sp+,r9
	mov.w	@sp+,r8
	mov.w	@sp+,r7
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	reti
	mov.w	#0001,r15
	mov.w	@sp+,pc

;; vPortEndScheduler: 5238
vPortEndScheduler proc
	mov.w	@sp+,pc

;; vPortYield: 523A
vPortYield proc
	push.w	sr
	invalid
	push.w	r4
	push.w	r5
	push.w	r6
	push.w	r7
	push.w	r8
	push.w	r9
	push.w	r10
	push.w	r11
	push.w	r12
	push.w	r13
	push.w	r14
	push.w	r15
	mov.w	-5040(pc),r14
	push.w	r14
	mov.w	-505C(pc),r12
	mov.w	sp,@r12
	call	4B18
	mov.w	-5068(pc),r12
	mov.w	@r12,sp
	mov.w	@sp+,r15
	mov.w	r15,-505A(pc)
	mov.w	@sp+,r15
	mov.w	@sp+,r14
	mov.w	@sp+,r13
	mov.w	@sp+,r12
	mov.w	@sp+,r11
	mov.w	@sp+,r10
	mov.w	@sp+,r9
	mov.w	@sp+,r8
	mov.w	@sp+,r7
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	reti

;; prvSetupTimerInterrupt: 528E
prvSetupTimerInterrupt proc
	mov.w	#0000,&0160
	mov.w	#0100,&0160
	bis.w	#0004,&0160
	mov.w	#0020,&0172
	mov.w	#0010,&0162
	bis.w	#0004,&0160
	bis.w	#0010,&0160
	mov.w	@sp+,pc

;; prvTickISR: 52B4
prvTickISR proc
	push.w	r4
	push.w	r5
	push.w	r6
	push.w	r7
	push.w	r8
	push.w	r9
	push.w	r10
	push.w	r11
	push.w	r12
	push.w	r13
	push.w	r14
	push.w	r15
	mov.w	-50B6(pc),r14
	push.w	r14
	mov.w	-50D2(pc),r12
	mov.w	sp,@r12
	call	484A
	call	4B18
	mov.w	-50E2(pc),r12
	mov.w	@r12,sp
	mov.w	@sp+,r15
	mov.w	r15,-50D4(pc)
	mov.w	@sp+,r15
	mov.w	@sp+,r14
	mov.w	@sp+,r13
	mov.w	@sp+,r12
	mov.w	@sp+,r11
	mov.w	@sp+,r10
	mov.w	@sp+,r9
	mov.w	@sp+,r8
	mov.w	@sp+,r7
	mov.w	@sp+,r6
	mov.w	@sp+,r5
	mov.w	@sp+,r4
	reti

;; printf: 5308
printf proc
	mov.w	#0002,r15
	add.w	sp,r15
	mov.w	@r15+,r14
	mov.w	r15,r13
	mov.w	#43A2,r15
	call	537E
	mov.w	@sp+,pc

;; PRINT: 531A
PRINT proc
	push.w	r11
	push.w	r10
	mov.w	r15,r10
	mov.w	r14,r11
	cmp.w	#0000,r14
	jnz	532A
	mov.w	#0001,r15
	jmp	5344
	mov.b	@r10,r15
	sxt.w	r15
	add.w	#0001,r10
	call	09B2
	cmp.w	#0000,r15
	jl	5342
	add.w	#0001,&021A
	add.w	#FFFF,r11
	jnz	532A
	jmp	5326
	mov.w	#FFFF,r15
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; __write_pad: 534A
__write_pad proc
	push.w	r11
	push.w	r10
	push.w	r9
	mov.b	r15,r9
	mov.b	r14,r11
	cmp.b	#01,r14
	jl	5370
	mov.b	r15,r10
	sxt.w	r10
	mov.w	r10,r15
	call	09B2
	cmp.w	#0000,r15
	jl	5374
	add.w	#0001,&021A
	add.b	#FF,r11
	cmp.b	#01,r11
	jge	535C
	mov.b	r9,r15
	jmp	5376
	mov.w	#FFFF,r15
	mov.w	@sp+,r9
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; vuprintf: 537E
vuprintf proc
	push.w	r11
	push.w	r10
	push.w	r9
	push.w	r8
	push.w	r7
	push.w	r6
	push.w	r5
	push.w	r4
	sub.w	#003C,sp
	mov.w	r13,r5
	mov.w	#0000,0030(sp)
	mov.w	#0000,0032(sp)
	mov.w	#0000,&021A
	mov.w	r15,&09B2
	mov.w	r14,r6
	mov.w	r6,r15
	mov.b	@r6,r7
	cmp.b	#00,r7
	jz	53C2
	cmp.b	#0025,r7
	jz	53C2
	add.w	#0001,r6
	mov.b	@r6,r7
	cmp.b	#00,r7
	jz	53C2
	cmp.b	#0025,r7
	jnz	53B4
	mov.w	r6,r13
	sub.w	r15,r13
	jz	53CC
	mov.w	#5916,pc
	cmp.b	#00,r7
	jnz	53D4
	mov.w	#5924,pc
	add.w	#0001,r6
	mov.b	#00,002E(sp)
	mov.b	#00,0035(sp)
	mov.b	#00,002F(sp)
	mov.b	#FF,r11
	mov.b	#00,0028(sp)
	mov.b	@r6,r7
	add.w	#0001,r6
	cmp.b	#0075,r7
	jnz	53F6
	mov.w	#58F0,pc
	mov.b	r7,r15
	bis.b	#0020,r15
	cmp.b	#0078,r15
	jnz	5406
	mov.w	#58F0,pc
	cmp.b	#0020,r7
	jnz	5410
	mov.w	#58DC,pc
	cmp.b	#0023,r7
	jnz	541A
	mov.w	#58D4,pc
	cmp.b	#002A,r7
	jnz	5424
	mov.w	#58B8,pc
	cmp.b	#002D,r7
	jnz	542E
	mov.w	#58A8,pc
	cmp.b	#002B,r7
	jnz	5438
	mov.w	#589E,pc
	cmp.b	#002E,r7
	jnz	5442
	mov.w	#5838,pc
	cmp.b	#0030,r7
	jnz	544C
	mov.w	#5822,pc
	mov.b	r7,r15
	add.b	#FFCF,r15
	cmp.b	#0009,r15
	jc	548C
	mov.w	#0000,r13
	mov.w	r13,r15
	add.w	r15,r15
	add.w	r15,r15
	mov.w	r13,r14
	add.w	r14,r14
	mov.w	r15,r13
	add.w	r14,r13
	add.w	r14,r13
	add.w	r14,r13
	mov.b	r7,r15
	sxt.w	r15
	add.w	r15,r13
	add.w	#FFD0,r13
	mov.b	@r6,r7
	add.w	#0001,r6
	mov.b	r7,r15
	add.b	#FFD0,r15
	cmp.b	#000A,r15
	jnc	545A
	mov.b	r13,002F(sp)
	jmp	53EC
	cmp.b	#0068,r7
	jz	581A
	cmp.b	#006C,r7
	jnz	549E
	bis.b	#01,002E(sp)
	jmp	53E8
	cmp.b	#0063,r7
	jz	580C
	cmp.b	#0044,r7
	jz	5806
	cmp.b	#0064,r7
	jz	57AE
	cmp.b	#0069,r7
	jz	57AE
	cmp.b	#004F,r7
	jz	57A8
	cmp.b	#006F,r7
	jz	57A2
	cmp.b	#0070,r7
	jz	5784
	cmp.b	#0073,r7
	jz	570C
	cmp.b	#0055,r7
	jz	5706
	cmp.b	#0075,r7
	jz	56FE
	cmp.b	#0058,r7
	jz	55FE
	cmp.b	#0078,r7
	jz	55FE
	cmp.b	#00,r7
	jnz	54EE
	mov.w	#5924,pc
	mov.w	sp,002C(sp)
	mov.b	r7,@sp
	mov.b	#01,r9
	mov.b	#00,0028(sp)
	mov.b	r9,r11
	mov.b	0035(sp),r10
	sub.b	r9,r10
	jn	55FA
	mov.b	0028(sp),r14
	cmp.b	#00,r14
	jz	55EE
	add.b	#01,r11
	add.b	r10,r11
	mov.b	002E(sp),r8
	and.b	#0030,r8
	jnz	5542
	mov.b	002F(sp),r13
	sxt.w	r13
	mov.b	r11,r15
	sxt.w	r15
	sub.w	r15,r13
	cmp.w	#0001,r13
	jl	5542
	mov.b	r13,r14
	mov.b	#0020,r15
	call	534A
	cmp.w	#0000,r15
	jge	553E
	mov.w	#5924,pc
	mov.b	0028(sp),r14
	cmp.b	#00,r14
	jnz	55E4
	bit.b	#0040,002E(sp)
	jz	5576
	mov.w	002A(sp),r15
	and.w	#FF00,r15
	bis.w	#0030,r15
	mov.w	r15,002A(sp)
	mov.b	r7,002B(sp)
	mov.w	#0002,r14
	mov.w	sp,r15
	add.w	#002A,r15
	call	531A
	cmp.w	#0000,r15
	jge	5576
	mov.w	#5924,pc
	cmp.b	#0020,r8
	jz	55C4
	mov.b	r10,r14
	mov.b	#0030,r15
	call	534A
	cmp.w	#0000,r15
	jl	5924
	mov.b	r9,r15
	sxt.w	r15
	mov.w	r15,r14
	mov.w	002C(sp),r15
	call	531A
	cmp.w	#0000,r15
	jl	5924
	bit.b	#0010,002E(sp)
	jz	53A6
	mov.b	002F(sp),r13
	sxt.w	r13
	mov.b	r11,r15
	sxt.w	r15
	sub.w	r15,r13
	cmp.w	#0001,r13
	jl	53A6
	mov.b	r13,r14
	mov.b	#0020,r15
	call	534A
	cmp.w	#0000,r15
	jge	53A6
	jmp	5924
	mov.b	002F(sp),r13
	sxt.w	r13
	mov.b	r11,r15
	sxt.w	r15
	sub.w	r15,r13
	cmp.w	#0001,r13
	jl	557C
	mov.b	r13,r14
	mov.b	#0030,r15
	call	534A
	cmp.w	#0000,r15
	jge	557C
	jmp	5924
	mov.w	#0001,r14
	mov.w	sp,r15
	add.w	#0028,r15
	jmp	556A
	bit.b	#0040,002E(sp)
	jz	5510
	add.b	#02,r11
	jmp	5510
	mov.b	#00,r10
	jmp	5506
	mov.b	#0010,0034(sp)
	bit.b	#08,002E(sp)
	jz	561C
	cmp.w	#0000,0030(sp)
	jnz	5616
	cmp.w	#0000,0032(sp)
	jz	561C
	bis.b	#0040,002E(sp)
	mov.b	#00,0028(sp)
	mov.b	r11,0035(sp)
	cmp.b	#00,r11
	jl	562E
	and.b	#FFDF,002E(sp)
	mov.w	sp,r15
	add.w	#0028,r15
	mov.w	r15,002C(sp)
	cmp.w	#0000,0030(sp)
	jnz	564A
	cmp.w	#0000,0032(sp)
	jnz	564A
	cmp.b	#00,0035(sp)
	jz	56C6
	mov.b	0034(sp),0038(sp)
	mov.b	#00,0039(sp)
	mov.w	#0000,003A(sp)
	mov.b	#00,0036(sp)
	mov.w	0030(sp),r14
	mov.w	0032(sp),r15
	sub.w	0038(sp),r14
	subc.w	003A(sp),r15
	jnc	5672
	mov.b	#01,0036(sp)
	mov.w	0030(sp),r12
	mov.w	0032(sp),r13
	mov.w	0038(sp),r10
	mov.w	003A(sp),r11
	call	5B4E
	mov.b	r14,r4
	cmp.b	#000A,r14
	jc	56EE
	add.b	#0030,r4
	add.w	#FFFF,002C(sp)
	mov.w	002C(sp),r15
	mov.b	r4,@r15
	mov.w	0030(sp),r12
	mov.w	0032(sp),r13
	mov.w	0038(sp),r10
	mov.w	003A(sp),r11
	call	5B4E
	mov.w	r12,0030(sp)
	mov.w	r13,0032(sp)
	cmp.b	#00,0036(sp)
	jnz	5658
	cmp.b	#08,0034(sp)
	jz	56D2
	mov.b	sp,r9
	sub.b	002C(sp),r9
	add.b	#0028,r9
	jmp	54FC
	bit.b	#08,002E(sp)
	jz	56C6
	cmp.b	#0030,r4
	jz	56C6
	add.w	#FFFF,002C(sp)
	mov.w	002C(sp),r15
	mov.b	#0030,@r15
	jmp	56C6
	add.b	#0057,r4
	cmp.b	#0058,r7
	jnz	5692
	and.b	#FFDF,r4
	jmp	5692
	mov.b	#000A,0034(sp)
	jmp	561C
	bis.b	#01,002E(sp)
	jmp	56FE
	mov.w	r5,r15
	add.w	#0002,r5
	mov.w	@r15,002C(sp)
	cmp.w	#0000,002C(sp)
	jz	5756
	cmp.b	#00,r11
	jl	5740
	mov.b	r11,r15
	sxt.w	r15
	mov.w	r15,r13
	mov.w	#0000,r14
	mov.w	002C(sp),r15
	call	593E
	cmp.w	#0000,r15
	jz	573C
	mov.b	r15,r9
	sub.b	002C(sp),r9
	cmp.b	r9,r11
	jge	54F8
	mov.b	r11,r9
	jmp	54F8
	mov.w	002C(sp),r15
	sub.w	#0001,r15
	add.w	#0001,r15
	cmp.b	#00,@r15
	jnz	5746
	mov.b	r15,r9
	sub.b	002C(sp),r9
	jmp	54F8
	mov.w	sp,002C(sp)
	mov.b	#0028,@sp
	mov.b	#006E,0001(sp)
	mov.b	#0075,0002(sp)
	mov.b	#006C,0003(sp)
	mov.b	#006C,0004(sp)
	mov.b	#0029,0005(sp)
	mov.b	#00,0006(sp)
	jmp	571A
	mov.w	r5,r15
	add.w	#0002,r5
	mov.w	@r15,0030(sp)
	mov.w	#0000,0032(sp)
	mov.b	#0010,0034(sp)
	bis.b	#0040,002E(sp)
	mov.b	#0078,r7
	jmp	561C
	mov.b	#08,0034(sp)
	jmp	561C
	bis.b	#01,002E(sp)
	jmp	57A2
	bit.b	#01,002E(sp)
	jz	57E6
	mov.w	r5,r15
	add.w	#0004,r5
	mov.w	@r15+,0030(sp)
	mov.w	@r15+,0032(sp)
	cmp.w	#0000,0032(sp)
	jl	57CE
	mov.b	#000A,0034(sp)
	jmp	5620
	xor.w	#FFFF,0030(sp)
	xor.w	#FFFF,0032(sp)
	add.w	#0001,0030(sp)
	addc.w	#0000,0032(sp)
	mov.b	#002D,0028(sp)
	jmp	57C6
	mov.w	r5,r15
	add.w	#0002,r5
	mov.w	@r15,0030(sp)
	mov.w	0030(sp),0032(sp)
	add.w	0032(sp),0032(sp)
	subc.w	0032(sp),0032(sp)
	xor.w	#FFFF,0032(sp)
	jmp	57C0
	bis.b	#01,002E(sp)
	jmp	57AE
	mov.w	sp,002C(sp)
	mov.w	r5,r15
	add.w	#0002,r5
	mov.b	@r15,@sp
	jmp	54F6
	bis.b	#04,002E(sp)
	mov.w	#53E8,pc
	bit.b	#0010,002E(sp)
	jz	582E
	mov.w	#53E8,pc
	bis.b	#0020,002E(sp)
	mov.w	#53E8,pc
	mov.b	@r6,r7
	add.w	#0001,r6
	cmp.b	#002A,r7
	jz	588A
	mov.w	#0000,r13
	mov.b	r7,r15
	add.b	#FFD0,r15
	cmp.b	#000A,r15
	jc	587C
	mov.w	r13,r15
	add.w	r15,r15
	add.w	r15,r15
	mov.w	r13,r14
	add.w	r14,r14
	mov.w	r15,r13
	add.w	r14,r13
	add.w	r14,r13
	add.w	r14,r13
	mov.b	r7,r15
	sxt.w	r15
	add.w	r15,r13
	add.w	#FFD0,r13
	mov.b	@r6,r7
	add.w	#0001,r6
	mov.b	r7,r15
	add.b	#FFD0,r15
	cmp.b	#000A,r15
	jnc	5850
	mov.w	r13,r15
	cmp.w	#FFFF,r13
	jge	5884
	mov.w	#FFFF,r15
	mov.b	r15,r11
	mov.w	#53EC,pc
	mov.w	r5,r15
	add.w	#0002,r5
	mov.w	@r15,r13
	mov.w	r13,r15
	cmp.w	#FFFF,r13
	jge	5898
	mov.w	#FFFF,r15
	mov.b	r15,r11
	mov.w	#53E8,pc
	mov.b	#002B,0028(sp)
	mov.w	#53E8,pc
	bis.b	#0010,002E(sp)
	and.b	#FFDF,002E(sp)
	mov.w	#53E8,pc
	mov.w	r5,r15
	add.w	#0002,r5
	mov.b	@r15,002F(sp)
	cmp.b	#00,002F(sp)
	jl	58CA
	mov.w	#53E8,pc
	xor.b	#FF,002F(sp)
	add.b	#01,002F(sp)
	jmp	58A8
	bis.b	#08,002E(sp)
	mov.w	#53E8,pc
	cmp.b	#00,0028(sp)
	jz	58E6
	mov.w	#53E8,pc
	mov.b	#0020,0028(sp)
	mov.w	#53E8,pc
	bit.b	#01,002E(sp)
	jz	5906
	mov.w	r5,r15
	add.w	#0004,r5
	mov.w	@r15+,0030(sp)
	mov.w	@r15+,0032(sp)
	mov.w	#5406,pc
	mov.w	r5,r15
	add.w	#0002,r5
	mov.w	@r15,0030(sp)
	mov.w	#0000,0032(sp)
	mov.w	#5406,pc
	mov.w	r13,r14
	call	531A
	cmp.w	#0000,r15
	jl	5924
	mov.w	#53CC,pc
	mov.w	&021A,r15
	add.w	#003C,sp
	mov.w	@sp+,r4
	mov.w	@sp+,r5
	mov.w	@sp+,r6
	mov.w	@sp+,r7
	mov.w	@sp+,r8
	mov.w	@sp+,r9
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; memchr: 593E
memchr proc
	push.w	r11
	mov.w	r15,r11
	mov.b	r14,r12
	cmp.w	#0000,r13
	jz	5956
	mov.w	r11,r14
	mov.b	@r14,r15
	add.w	#0001,r14
	cmp.b	r12,r15
	jz	595A
	add.w	#FFFF,r13
	jnz	594A
	mov.w	#0000,r15
	jmp	595E
	mov.w	r14,r15
	add.w	#FFFF,r15
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; strncpy: 5962
strncpy proc
	push.w	r11
	mov.w	r15,r11
	cmp.w	#0000,r13
	jz	598E
	mov.w	r15,r12
	mov.b	@r14,@r12
	mov.b	@r12,r15
	add.w	#0001,r14
	add.w	#0001,r12
	cmp.b	#00,r15
	jz	5980
	add.w	#FFFF,r13
	jnz	596C
	jmp	598E
	add.w	#FFFF,r13
	jz	598E
	mov.b	#00,@r12
	add.w	#0001,r12
	add.w	#FFFF,r13
	jnz	5984
	mov.w	r11,r15
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; memcpy: 5994
memcpy proc
	push.w	r11
	push.w	r10
	mov.w	r15,r10
	mov.w	r13,r11
	mov.w	r15,r13
	mov.w	r14,r12
	cmp.w	#0000,r11
	jz	5A60
	cmp.w	r14,r15
	jz	5A60
	cmp.w	r14,r15
	jc	5A02
	mov.w	r14,r15
	bis.w	r10,r15
	and.w	#0001,r15
	jz	59D0
	mov.w	r14,r15
	xor.w	r10,r15
	and.w	#0001,r15
	jnz	59C0
	cmp.w	#0002,r11
	jc	59F8
	mov.w	r11,r14
	sub.w	r14,r11
	mov.b	@r12,@r13
	add.w	#0001,r12
	add.w	#0001,r13
	add.w	#FFFF,r14
	jnz	59C4
	mov.w	r11,r14
	invalid
	rrc.w	r14
	cmp.w	#0000,r14
	jz	59E4
	mov.w	@r12+,@r13
	add.w	#0002,r13
	add.w	#FFFF,r14
	jnz	59DA
	mov.w	r11,r14
	and.w	#0001,r14
	jz	5A60
	mov.b	@r12,@r13
	add.w	#0001,r12
	add.w	#0001,r13
	add.w	#FFFF,r14
	jnz	59EA
	jmp	5A60
	mov.w	r14,r15
	and.w	#0001,r15
	mov.w	#0002,r14
	sub.w	r15,r14
	jmp	59C2
	mov.w	r14,r12
	add.w	r11,r12
	mov.w	r15,r13
	add.w	r11,r13
	mov.w	r12,r15
	bis.w	r13,r15
	and.w	#0001,r15
	jz	5A30
	mov.w	r12,r15
	xor.w	r13,r15
	and.w	#0001,r15
	jnz	5A20
	cmp.w	#0003,r11
	jc	5A5A
	mov.w	r11,r14
	sub.w	r14,r11
	add.w	#FFFF,r13
	add.w	#FFFF,r12
	mov.b	@r12,@r13
	add.w	#FFFF,r14
	jnz	5A24
	mov.w	r11,r14
	invalid
	rrc.w	r14
	cmp.w	#0000,r14
	jz	5A46
	sub.w	#0002,r12
	sub.w	#0002,r13
	mov.w	@r12,@r13
	add.w	#FFFF,r14
	jnz	5A3A
	mov.w	r11,r14
	and.w	#0001,r14
	jz	5A60
	add.w	#FFFF,r13
	add.w	#FFFF,r12
	mov.b	@r12,@r13
	add.w	#FFFF,r14
	jnz	5A4C
	jmp	5A60
	mov.w	r12,r14
	and.w	#0001,r14
	jmp	5A22
	mov.w	r10,r15
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc

;; memset: 5A68
memset proc
	push.w	r11
	push.w	r10
	push.w	r9
	mov.w	r15,r9
	mov.w	r14,r10
	mov.w	r15,r14
	cmp.w	#0006,r13
	jc	5A8A
	cmp.w	#0000,r13
	jz	5ACE
	mov.b	r10,@r14
	add.w	#0001,r14
	add.w	#FFFF,r13
	jnz	5A7E
	jmp	5ACE
	mov.b	r10,r11
	cmp.w	#0000,r11
	jz	5A96
	mov.w	r11,r15
	swpb	r15
	bis.w	r15,r11
	mov.w	r9,r12
	and.w	#0001,r12
	jz	5AAE
	mov.w	#0002,r15
	sub.w	r12,r15
	mov.w	r15,r12
	sub.w	r15,r13
	mov.b	r10,@r14
	add.w	#0001,r14
	add.w	#FFFF,r12
	jnz	5AA4
	mov.w	r13,r12
	invalid
	rrc.w	r12
	mov.w	r11,@r14
	add.w	#0002,r14
	add.w	#FFFF,r12
	jnz	5AB4
	mov.w	r13,r12
	and.w	#0001,r12
	jz	5ACE
	mov.b	r10,@r14
	add.w	#0001,r14
	add.w	#FFFF,r12
	jnz	5AC4
	mov.w	r9,r15
	mov.w	@sp+,r9
	mov.w	@sp+,r10
	mov.w	@sp+,r11
	mov.w	@sp+,pc
	invalid
	jmp	5AD8
	mov.w	r12,&0130
	mov.w	r10,&0138
	mov.w	r12,&0134
	mov.w	&013A,r14
	mov.w	&013C,&013A
	mov.w	r11,&0138
	mov.w	r13,&0134
	mov.w	r10,&0138
	mov.w	&013A,r15
	mov.w	@sp+,pc
	mov.w	#0000,r8
	bit.w	#8000,r13
	jz	5B16
	xor.w	#FFFF,r13
	xor.w	#FFFF,r12
	add.w	#0001,r12
	addc.w	#0000,r13
	bis.w	#0004,r8
	bit.w	#8000,r11
	jz	5B26
	xor.w	#FFFF,r11
	xor.w	#FFFF,r10
	add.w	#0001,r10
	addc.w	#0000,r11
	bis.w	#0008,r8
	call	5B4E
	rrc.w	r8
	bit.w	#0004,r8
	jz	5B40
	xor.w	#FFFF,r14
	xor.w	#FFFF,r15
	add.w	#0001,r14
	addc.w	#0000,r15
	xor.w	#FFFF,r12
	xor.w	#FFFF,r13
	add.w	#0001,r12
	addc.w	#0000,r13
	bit.w	#0008,r8
	jz	5B4C
	xor.w	#FFFF,r12
	xor.w	#FFFF,r13
	add.w	#0001,r12
	addc.w	#0000,r13
	mov.w	@sp+,pc
	xor.w	r15,r15
	xor.w	r14,r14
	mov.w	#0021,r9
	jmp	5B6C
	rrc.w	r8
	addc.w	r14,r14
	addc.w	r15,r15
	cmp.w	r11,r15
	jnc	5B6C
	jnz	5B68
	cmp.w	r10,r14
	jnc	5B6C
	sub.w	r10,r14
	subc.w	r11,r15
	addc.w	r12,r12
	addc.w	r13,r13
	addc.w	r8,r8
	sub.w	#0001,r9
	jnz	5B58
	mov.w	@sp+,pc
;;; Segment .vectors (0000FFE0)
InterruptVectors		; FFE0
	db	0x3A, 0x40, 0x3A, 0x40, 0x80, 0x44, 0x50, 0x44, 0x3A, 0x40, 0x3A, 0x40, 0xB4, 0x52, 0x3A, 0x40
	db	0x3A, 0x40, 0x3A, 0x40, 0x3A, 0x40, 0x3A, 0x40, 0x3A, 0x40, 0x3A, 0x40, 0x3A, 0x40, 0x00, 0x40
