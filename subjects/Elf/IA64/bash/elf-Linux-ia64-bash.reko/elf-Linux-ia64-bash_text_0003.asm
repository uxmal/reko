;;; Segment .text (400000000001C480)

;; dispose_function_def_contents: 400000000004C700
dispose_function_def_contents proc
	Invalid
	adds	r14,8,r32
	Invalid

;; dispose_function_def: 400000000004C7C0
dispose_function_def proc
	Invalid
	Invalid
	Invalid

;; close_fd_bitmap: 400000000004CF00
close_fd_bitmap proc
	Invalid
	Invalid
	Invalid

;; dispose_fd_bitmap: 400000000004D0C0
dispose_fd_bitmap proc
	Invalid
	adds	r14,8,r32
	Invalid

;; new_fd_bitmap: 400000000004EE40
new_fd_bitmap proc
	Invalid
	Invalid
	adds	r36,0,r1

;; executing_line_number: 400000000004EF40
executing_line_number proc
	addl	r14,6488,r1
	Invalid
	nop.i	0x0

;; dispose_exec_redirects: 400000000004F0C0
dispose_exec_redirects proc
	Invalid
	addl	r32,7060,r1
	Invalid

;; getcoprocbypid: 400000000004F140
getcoprocbypid proc
	addl	r14,-21372,r1
	Invalid
	nop.i	0x0

;; getcoprocbyname: 400000000004F1C0
getcoprocbyname proc
	Invalid
	addl	r14,-21372,r1
	Invalid

;; coproc_init: 400000000004F2C0
coproc_init proc
	adds	r15,0,r32
	addl	r14,-1,r0
	adds	r20,16,r32

;; coproc_alloc: 400000000004F340
coproc_alloc proc
	Invalid
	addl	r34,-21372,r1
	Invalid

;; coproc_close: 400000000004F440
coproc_close proc
	Invalid
	adds	r34,12,r32
	Invalid

;; coproc_closeall: 400000000004F540
coproc_closeall proc
	Invalid
	Invalid
	addl	r32,-21372,r1

;; coproc_rclose: 400000000004F580
coproc_rclose proc
	Invalid
	adds	r32,12,r32
	Invalid

;; coproc_wclose: 400000000004F640
coproc_wclose proc
	Invalid
	adds	r32,16,r32
	Invalid

;; coproc_fdsave: 400000000004F700
coproc_fdsave proc
	adds	r15,12,r32
	adds	r14,16,r32
	adds	r16,20,r32

;; coproc_fdrestore: 400000000004F740
coproc_fdrestore proc
	adds	r15,20,r32
	adds	r14,24,r32
	adds	r16,12,r32

;; coproc_pidchk: 400000000004F780
coproc_pidchk proc
	addl	r14,-21372,r1
	Invalid
	nop.i	0x0

;; coproc_setvars: 400000000004F800
coproc_setvars proc
	Invalid
	Invalid
	Invalid

;; coproc_fdclose: 400000000004FC40
coproc_fdclose proc
	Invalid
	Invalid
	Invalid

;; coproc_checkfd: 400000000004FCC0
coproc_checkfd proc
	adds	r14,12,r32
	Invalid
	nop.i	0x0

;; coproc_fdchk: 400000000004FD80
coproc_fdchk proc
	Invalid
	adds	r33,0,r32
	addl	r32,-21372,r1

;; coproc_unsetvars: 400000000004FDC0
coproc_unsetvars proc
	Invalid
	Invalid
	Invalid

;; coproc_dispose: 400000000004FF00
coproc_dispose proc
	Invalid
	Invalid
	Invalid

;; coproc_reap: 4000000000050040
coproc_reap proc
	Invalid
	Invalid
	addl	r32,-21372,r1

;; coproc_flush: 40000000000500C0
coproc_flush proc
	Invalid
	Invalid
	addl	r32,-21372,r1

;; setup_async_signals: 4000000000050100
setup_async_signals proc
	Invalid
	addl	r14,5868,r1
	Invalid

;; shell_execve: 40000000000501C0
shell_execve proc
	Invalid
	adds	r12,-80,r12
	Invalid

;; execute_command_internal: 4000000000053C40
execute_command_internal proc
	Invalid
	adds	r12,-48,r12
	Invalid

;; execute_shell_function: 400000000005B380
execute_shell_function proc
	Invalid
	addl	r35,-2988,r1
	Invalid
