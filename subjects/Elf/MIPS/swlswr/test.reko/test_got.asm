;;; Segment .got (00010A90)
00010A90 00 00 00 00 80 00 00 00                         ........        
main_GOT		; 00010A98
	dd	0x000007F0
__libc_csu_init_GOT		; 00010A9C
	dd	0x000008C0
__libc_csu_fini_GOT		; 00010AA0
	dd	0x00000964
00010AA4             00 01 0A 84 00 01 00 00 00 01 0A EC     ............
00010AB0 00 01 0A 6C 00 00 00 00                         ...l....        
_init_GOT		; 00010AB8
	dd	0x00000588
00010ABC                                     00 01 0A 60             ...`
00010AC0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
calloc_GOT		; 00010AD0
	dd	0x000009F0
00010AD4             00 00 00 00                             ....        
memset_GOT		; 00010AD8
	dd	0x000009E0
__libc_start_main_GOT		; 00010ADC
	dd	0x000009D0
__gmon_start___GOT		; 00010AE0
	dd	0x00000000
00010AE4             00 00 00 00                             ....        
__cxa_finalize_GOT		; 00010AE8
	dd	0x00000000
