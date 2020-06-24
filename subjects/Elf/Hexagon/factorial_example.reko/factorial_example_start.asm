;;; Segment .start (00000000)
00000000 4C C0 00 58 3E C0 00 58 42 C0 00 58 00 00 00 00 L..X>..XB..X....
00000010 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
00000080 00 C0 3C 72 B8 FB 3C 71 00 C0 9C 52 00 C0 3C 72 ..<r..<q...R..<r
00000090 34 FD 3C 71 00 C0 9C 52                         4.<q...R       

;; hexagon_start_init: 00000098
hexagon_start_init proc
	{  }
	{  }
	{ p0 = cmp.eq(r0,00000002) }
	{  }
	{ r0.h = 0100 }
	{ r0.l = 0000 }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r0 = 00000000 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 00C0 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 00D0 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 00C0 }
	{  }
	{ p0 = cmp.eq(r0,00000000) }
	{ r0.h = 0000 }
	{ r0.l = 00D0 }
	{  }
	{ p1 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p3 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 90E3 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 90E3 }
	{  }
	{ p3 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 00F0 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 00E0 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 00E0 }
	{  }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{ nop }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3F10 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3F20 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3F30 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3F40 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3F80 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3F90 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3FA0 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{ r1.h = 0000 }
	{ r1.l = 3F70 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{ r1.h = 0000 }
	{ r1.l = 3F60 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3FB0 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3FC0 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3FD0 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3FE0 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3F00 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3F50 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{ r1.h = 0000 }
	{ r1.l = 3FF0 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 3F60 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 6B30 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 3FB0 }
	{  }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 3FC0 }
	{  }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 3FD0 }
	{  }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 3FE0 }
	{  }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ p0 = cmp.eq(r1,00000000) }
	{  }
	{  }
	{ r3 = 00000000 }
	{  }
	{  }
	{  }
	{  }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{  }
	{  }
	{  }
	{ p0 = cmp.eq(r1,00000002) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r3.h = 0000 }
	{ r3.l = 3F00 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3F10 }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 3F30 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ p0 = cmp.eq(r0,00000002) }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3F50 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 3F20 }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 3F40 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 3F80 }
	{  }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 3FA0 }
	{  }
	{ p3 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r2.h = 0000 }
	{ r2.l = 0000 }
	{  }
	{ r1.h = 0FC3 }
	{ r1.l = 0000 }
	{ r0.h = DC03 }
	{ r0.l = 0000 }
	{  }
	{  }
	{  }
	{  }
	{ r2 = 00000000 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0 = 00000000 }
	{  }
	{ r1 = FFFFFFFF }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 90E3 }
	{  }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 8063 }
	{ r1 = 00000020 }
	{ r28.h = 0000 }
	{ r28.l = 5841 }
	{  }
	{ r0 = 00000016 }
	{ r1.h = 0000 }
	{ r1.l = 80A3 }
	{  }
	{ r28.h = 0000 }
	{ r28.l = EEE0 }
	{  }
	{ nop }

;; __coredump: 00000770
__coredump proc
	{ r0.h = 0000 }
	{ r0.l = 3FF0 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r1 = 00000000 }
	{  }
	{ r1 = 00000000 }
	{  }
	{ r1 = 00000000 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0 = 000000CD }
	{  }
	{ r2 = FFFFFFFF }
	{ r0 = FFFFFFFF }
	{  }
	{ nop }
	{ nop }
	{ nop }

;; event_handle_reset: 00000970
event_handle_reset proc
	{  }
	{ r28.h = 0000 }
	{ r29.h = 0000 }
	{ r0.h = 0000 }
	{ r28.l = 6B90 }
	{  }
	{ r29.l = 6BF0 }
	{ r0.l = 6C50 }
	{  }
	{  }
	{  }
	{ r2 = 00000000 }
	{  }
	{  }
	{  }
	{ r2.h = 0000 }
	{ r2.l = 6B30 }
	{  }
	{  }
	{  }
	{ r2.h = 0004 }
	{ r2.l = 0000 }
	{  }
	{ r2.h = 0010 }
	{ r2.l = 0000 }
	{  }
	{ r2.h = 0000 }
	{ r2.l = 3FE0 }
	{  }
	{ p0 = !cmp.eq(r2,00000000) }
	{  }
	{  }
	{  }
	{  }
	{ r2.h = 0004 }
	{  }
	{ r2.h = 004C }
	{  }
	{  }
	{ r2.h = EAFA }
	{ r2.l = FBBE }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r31.h = 0000 }
	{  }
	{  }
	{ r31.l = 4901 }
	{ jump	00000A68 }
	{ r30 = 00000000 }
	{  }

;; thread_start: 00000A64
thread_start proc
	{  }

;; event_handle_nmi: 00000A68
event_handle_nmi proc
	{ r0 = 00000001 }
	{  }
	{ jump	00000770 }

;; event_handle_error: 00000A74
event_handle_error proc
	{ r0 = 00000002 }
	{  }
	{ jump	00000770 }

;; event_handle_rsvd: 00000A80
event_handle_rsvd proc
	{ r0.h = 7AB7 }
	{ r0.l = FBBE }
	{  }
	{ jump	00000770 }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }

;; event_handle_tlbmissx: 00000AC0
event_handle_tlbmissx proc
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0.h = 0000 }
	{  }
	{ r0.l = 6E10 }
	{ r1 = 00000006 }
	{  }
	{  }
	{  }
	{  }
	{ r3.h = 0000 }
	{  }
	{  }
	{ r3.l = 3F70 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r1.h = 0003 }
	{ r0.h = C003 }
	{  }
	{  }
	{  }
	{ r4.h = 0000 }
	{ r4.l = 6E00 }
	{ r5 = 00000001 }
	{  }
	{ p0 = cmp.eq(r2,00000000) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r1 = 00000000 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r2 = memd(r29+128); r17 = memd(r29+128) }
	{  }
	{  }
	{  }
	{  }

;; event_handle_tlbmissrw: 00000BC0
event_handle_tlbmissrw proc
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0.l = 6E10 }
	{ r1 = 00000006 }
	{  }
	{ r0.h = 0000 }
	{  }
	{  }
	{  }
	{ r3.h = 0000 }
	{  }
	{ r3.l = 3F70 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r1.h = 0003 }
	{ r0.h = C003 }
	{  }
	{  }
	{  }
	{ r4.h = 0000 }
	{ r4.l = 6E00 }
	{ r5 = 00000001 }
	{  }
	{  }
	{ p1 = cmp.eq(r2,00000000) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r1 = 00000000 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r2 = memd(r29+128); r17 = memd(r29+128) }
	{  }
	{  }
	{  }
	{  }

;; event_handle_trap0: 00000CB0
event_handle_trap0 proc
	{  }
	{  }
	{  }
	{ p0 = cmp.eq(r0,00000040) }
	{  }
	{ p1 = cmp.eq(r0,00000044) }
	{ p2 = cmp.eq(r0,00000052) }
	{ r4.h = 5555 }
	{  }
	{ r3.h = 0000 }
	{ r3.l = 00F0 }
	{  }
	{ p3 = cmp.eq(r3,00000001) }
	{  }
	{  }
	{  }
	{ r4.l = 5555 }
	{  }
	{  }
	{ r3.h = 0000 }
	{ r6 = 000000CD }
	{  }
	{ r3.l = 0100 }
	{ r4.h = 0000 }
	{ p2 = cmp.eq(r0,00000018) }
	{  }
	{  }
	{  }
	{ r4.l = 0080 }
	{  }
	{  }
	{ nop }
	{ r6.h = 0008 }
	{  }
	{ p0 = cmp.eq(r7,00000000) }
	{ nop }
	{ p0 = cmp.eq(r7,00000000) }
	{  }
	{ Invalid; r7 = memw(r3) }
	{  }
	{  }
	{  }
	{ p0 = cmp.eq(r7,00000000) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r6.h = 0000 }
	{ r6.l = 00F0 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r5 = 00000000 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r2 = memd(r29+128); r3 = memd(r29+128) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r0 = 000000C1 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r17 = memd(r29+128); r2 = memd(r29+128) }
	{  }
	{  }
	{  }
	{ r6.l = 0098 }
	{  }
	{ jump	00000D44 }
	{ r1.h = 0000 }
	{ r1.l = 3FF0 }
	{  }
	{ jump	00000D40 }

;; event_handle_trap1: 00000E30
event_handle_trap1 proc
	{ r0 = 00000009 }
	{  }
	{ jump	00000770 }

;; event_handle_int: 00000E3C
event_handle_int proc
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r1.h = 0000 }
	{  }
	{  }
	{  }
	{ r1.l = 4000 }
	{  }
	{  }
	{  }
	{ r3 = 00000000 }
	{  }
	{  }
	{  }
	{  }
	{ p0 = cmp.eq(r1,00000000) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r26.h = 0000 }
	{ Invalid; r19 = memd(r29+160) }
	{  }
	{  }
	{ r26.l = 0004 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r19 = memd(r29+160); r2 = memd(r29+160) }
	{  }
	{  }
	{ r17 = memd(r29+152); r0 = memd(r29+152) }
	{  }
	{  }
	{  }
	{  }
	{ r23 = memd(r29+144); r6 = memd(r29+144) }
	{  }
	{ r21 = memd(r29+144); r4 = memd(r29+144) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r19 = memd(r29+128); r2 = memd(r29+128) }
	{  }
	{ r17 = memd(r29+128); r0 = memd(r29+128) }
	{  }
	{  }
	{  }
	{ nop }
	{ nop }
	{ nop }

;; .NoHandler: 00000FA0
.NoHandler proc
	{  }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r3 = memw(r0) }
	{ r0 = memw(r0); r1 = memw(r0) }
	{ r0 = memw(r0); r1 = memw(r0) }
	{ r0 = memw(r0); r1 = memw(r0) }
	{ r0 = memw(r0); r1 = memw(r0) }
	{ r0 = memw(r0); r1 = memw(r0) }
	{  }
	{ r0 = memw(r0); r16 = memb(r16+3) }
	{ r0 = memw(r0); r1 = memw(r0) }
	{ r0 = memw(r0); r1 = memw(r0) }
	{ r0 = memw(r0); r1 = memw(r0) }
	{ r0 = memw(r0); r1 = memw(r0) }
	{ r0 = memw(r0); r1 = memw(r0) }
	{ r0 = memw(r0); r1 = memw(r0) }
	{ r0 = memw(r0); r1 = memw(r0) }
	{ r0 = memw(r0); r0 = memb(r16) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r18+28) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memb(r2+3) }
	{ r0 = memw(r0); r16 = memb(r18+4) }
	{ r0 = memw(r0); r0 = memb(r3+6) }
	{ r0 = memw(r0); r16 = memb(r19+7) }
	{ r0 = memw(r0); r0 = memb(r4+1) }
	{ r0 = memw(r0); r16 = memb(r20+2) }
	{ r0 = memw(r0); r0 = memw(r7+28) }
	{ r0 = memw(r0); r0 = memw(r7+28) }
	{ r0 = memw(r0); r0 = memw(r7+28) }
	{ r0 = memw(r0); r0 = memw(r7+28) }
	{ r0 = memw(r0); r0 = memw(r7+28) }
	{ r0 = memw(r0); r0 = memw(r7+28) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r3 = memw(r0+12); r0 = memw(r0+8) }
	{ r4 = memw(r0+16); r4 = memw(r0+16) }
	{ r4 = memw(r0+16); r4 = memw(r0+16) }
	{ r4 = memw(r0+16); r4 = memw(r0+16) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r0 = memw(r0) }
	{ r0 = memw(r0); r5 = memw(r0) }
	{ r7 = memw(r1); r7 = memw(r0) }
	{ r7 = memw(r3); r7 = memw(r2) }
	{ r7 = memw(r5); r7 = memw(r4) }
	{ r7 = memw(r7); r7 = memw(r6) }
	{ r7 = memw(r17); r7 = memw(r16) }
	{ r7 = memw(r19); r7 = memw(r18) }
	{ r7 = memw(r21); r7 = memw(r20) }
	{ r7 = memw(r23); r7 = memw(r22) }
	{ r7 = memw(r1+4); r7 = memw(r0+4) }
	{ r7 = memw(r3+4); r7 = memw(r2+4) }
	{ r7 = memw(r5+4); r7 = memw(r4+4) }
	{ r7 = memw(r7+4); r7 = memw(r6+4) }
	{ r7 = memw(r17+4); r7 = memw(r16+4) }
	{ r7 = memw(r19+4); r7 = memw(r18+4) }
	{ r7 = memw(r21+4); r7 = memw(r20+4) }
	{ r7 = memw(r23+4); r7 = memw(r22+4) }
	{ r7 = memw(r1+8); r7 = memw(r0+8) }
	{ r7 = memw(r3+8); r7 = memw(r2+8) }
	{ r7 = memw(r5+8); r7 = memw(r4+8) }
	{ r7 = memw(r7+8); r7 = memw(r6+8) }
	{ r7 = memw(r17+8); r7 = memw(r16+8) }
	{ r7 = memw(r19+8); r7 = memw(r18+8) }
	{ r7 = memw(r21+8); r7 = memw(r20+8) }
	{ r7 = memw(r23+8); r7 = memw(r22+8) }
	{ r7 = memw(r1+12); r7 = memw(r0+12) }
	{ r7 = memw(r3+12); r7 = memw(r2+12) }
	{ r7 = memw(r5+12); r7 = memw(r4+12) }
	{ r7 = memw(r7+12); r7 = memw(r6+12) }
	{ r7 = memw(r17+12); r7 = memw(r16+12) }
	{ r7 = memw(r19+12); r7 = memw(r18+12) }
	{ r7 = memw(r21+12); r7 = memw(r20+12) }
	{ r7 = memw(r23+12); r7 = memw(r22+12) }
	{ r7 = memw(r1+16); r7 = memw(r0+16) }
	{ r7 = memw(r3+16); r7 = memw(r2+16) }
	{ r7 = memw(r5+16); r7 = memw(r4+16) }
	{ r7 = memw(r7+16); r7 = memw(r6+16) }
	{ r7 = memw(r17+16); r7 = memw(r16+16) }
	{ r7 = memw(r19+16); r7 = memw(r18+16) }
	{ r7 = memw(r21+16); r7 = memw(r20+16) }
	{ r7 = memw(r23+16); r7 = memw(r22+16) }
	{ r7 = memw(r1+20); r7 = memw(r0+20) }
	{ r7 = memw(r3+20); r7 = memw(r2+20) }
	{ r7 = memw(r5+20); r7 = memw(r4+20) }
	{ r7 = memw(r7+20); r7 = memw(r6+20) }
	{ r7 = memw(r17+20); r7 = memw(r16+20) }
	{ r7 = memw(r19+20); r7 = memw(r18+20) }
	{ r7 = memw(r21+20); r7 = memw(r20+20) }
	{ r7 = memw(r23+20); r7 = memw(r22+20) }
	{ r7 = memw(r1+24); r7 = memw(r0+24) }
	{ r7 = memw(r3+24); r7 = memw(r2+24) }
	{ r7 = memw(r5+24); r7 = memw(r4+24) }
	{ r7 = memw(r7+24); r7 = memw(r6+24) }
	{ r7 = memw(r17+24); r7 = memw(r16+24) }
	{ r7 = memw(r19+24); r7 = memw(r18+24) }
	{ r7 = memw(r21+24); r7 = memw(r20+24) }
	{ r7 = memw(r23+24); r7 = memw(r22+24) }
	{ r7 = memw(r1+28); r7 = memw(r0+28) }
	{ r7 = memw(r3+28); r7 = memw(r2+28) }
	{ r7 = memw(r5+28); r7 = memw(r4+28) }
	{ r7 = memw(r7+28); r7 = memw(r6+28) }
	{ r7 = memw(r17+28); r7 = memw(r16+28) }
	{ r7 = memw(r19+28); r7 = memw(r18+28) }
	{ r7 = memw(r21+28); r7 = memw(r20+28) }
	{ r7 = memw(r23+28); r7 = memw(r22+28) }
	{ r7 = memw(r1); r7 = memw(r0) }
	{ r7 = memw(r3); r7 = memw(r2) }
	{ r7 = memw(r5); r7 = memw(r4) }
	{ r7 = memw(r7); r7 = memw(r6) }
	{ r7 = memw(r17); r7 = memw(r16) }
	{ r7 = memw(r19); r7 = memw(r18) }
	{ r7 = memw(r21); r7 = memw(r20) }
	{ r7 = memw(r23); r7 = memw(r22) }
	{ r7 = memw(r1+4); r7 = memw(r0+4) }
	{ r7 = memw(r3+4); r7 = memw(r2+4) }
	{ r7 = memw(r5+4); r7 = memw(r4+4) }
	{ r7 = memw(r7+4); r7 = memw(r6+4) }
	{ r7 = memw(r17+4); r7 = memw(r16+4) }
	{ r7 = memw(r19+4); r7 = memw(r18+4) }
	{ r7 = memw(r21+4); r7 = memw(r20+4) }
	{ r7 = memw(r23+4); r7 = memw(r22+4) }
	{ r7 = memw(r1+8); r7 = memw(r0+8) }
	{ r7 = memw(r3+8); r7 = memw(r2+8) }
	{ r7 = memw(r5+8); r7 = memw(r4+8) }
	{ r7 = memw(r7+8); r7 = memw(r6+8) }
	{ r7 = memw(r17+8); r7 = memw(r16+8) }
	{ r7 = memw(r19+8); r7 = memw(r18+8) }
	{ r7 = memw(r21+8); r7 = memw(r20+8) }
	{ r7 = memw(r23+8); r7 = memw(r22+8) }
	{ r7 = memw(r1+12); r7 = memw(r0+12) }
	{ r7 = memw(r3+12); r7 = memw(r2+12) }
	{ r7 = memw(r5+12); r7 = memw(r4+12) }
	{ r7 = memw(r7+12); r7 = memw(r6+12) }
	{ r7 = memw(r17+12); r7 = memw(r16+12) }
	{ r7 = memw(r19+12); r7 = memw(r18+12) }
	{ r7 = memw(r21+12); r7 = memw(r20+12) }
	{ r7 = memw(r23+12); r7 = memw(r22+12) }
	{ r7 = memw(r1+16); r7 = memw(r0+16) }
	{ r7 = memw(r3+16); r7 = memw(r2+16) }
	{ r7 = memw(r5+16); r7 = memw(r4+16) }
	{ r7 = memw(r7+16); r7 = memw(r6+16) }
	{ r7 = memw(r17+16); r7 = memw(r16+16) }
	{ r7 = memw(r19+16); r7 = memw(r18+16) }
	{ r7 = memw(r21+16); r7 = memw(r20+16) }
	{ r7 = memw(r23+16); r7 = memw(r22+16) }
	{ r7 = memw(r1+20); r7 = memw(r0+20) }
	{ r7 = memw(r3+20); r7 = memw(r2+20) }
	{ r7 = memw(r5+20); r7 = memw(r4+20) }
	{ r7 = memw(r7+20); r7 = memw(r6+20) }
	{ r7 = memw(r17+20); r7 = memw(r16+20) }
	{ r7 = memw(r19+20); r7 = memw(r18+20) }
	{ r7 = memw(r21+20); r7 = memw(r20+20) }
	{ r7 = memw(r23+20); r7 = memw(r22+20) }
	{ r7 = memw(r1+24); r7 = memw(r0+24) }
	{ r7 = memw(r3+24); r7 = memw(r2+24) }
	{ r7 = memw(r5+24); r7 = memw(r4+24) }
	{ r7 = memw(r7+24); r7 = memw(r6+24) }
	{ r7 = memw(r17+24); r7 = memw(r16+24) }
	{ r7 = memw(r19+24); r7 = memw(r18+24) }
	{ r7 = memw(r21+24); r7 = memw(r20+24) }
	{ r7 = memw(r23+24); r7 = memw(r22+24) }
	{ r7 = memw(r1+28); r7 = memw(r0+28) }
	{ r7 = memw(r3+28); r7 = memw(r2+28) }
	{ r7 = memw(r5+28); r7 = memw(r4+28) }
	{ r7 = memw(r7+28); r7 = memw(r6+28) }
	{ r7 = memw(r17+28); r7 = memw(r16+28) }
	{ r7 = memw(r19+28); r7 = memw(r18+28) }
	{ r7 = memw(r21+28); r7 = memw(r20+28) }
	{ r7 = memw(r23+28); r7 = memw(r22+28) }
	{ r7 = memb(r1); r7 = memb(r0) }
	{ r7 = memb(r3); r7 = memb(r2) }
	{ r7 = memb(r5); r7 = memb(r4) }
	{ r7 = memb(r7); r7 = memb(r6) }
	{ r7 = memb(r17); r7 = memb(r16) }
	{ r7 = memb(r19); r7 = memb(r18) }
	{ r7 = memb(r21); r7 = memb(r20) }
	{ r7 = memb(r23); r7 = memb(r22) }
	{ r7 = memb(r1+1); r7 = memb(r0+1) }
	{ r7 = memb(r3+1); r7 = memb(r2+1) }
	{ r7 = memb(r5+1); r7 = memb(r4+1) }
	{ r7 = memb(r7+1); r7 = memb(r6+1) }
	{ r7 = memb(r17+1); r7 = memb(r16+1) }
	{ r7 = memb(r19+1); r7 = memb(r18+1) }
	{ r7 = memb(r21+1); r7 = memb(r20+1) }
	{ r7 = memb(r23+1); r7 = memb(r22+1) }
	{ r7 = memb(r1+2); r7 = memb(r0+2) }
	{ r7 = memb(r3+2); r7 = memb(r2+2) }
	{ r7 = memb(r5+2); r7 = memb(r4+2) }
	{ r7 = memb(r7+2); r7 = memb(r6+2) }
	{ r7 = memb(r17+2); r7 = memb(r16+2) }
	{ r7 = memb(r19+2); r7 = memb(r18+2) }
	{ r7 = memb(r21+2); r7 = memb(r20+2) }
	{ r7 = memb(r23+2); r7 = memb(r22+2) }
	{ r7 = memb(r1+3); r7 = memb(r0+3) }
	{ r7 = memb(r3+3); r7 = memb(r2+3) }
	{ r7 = memb(r5+3); r7 = memb(r4+3) }
	{ r7 = memb(r7+3); r7 = memb(r6+3) }
	{ r7 = memb(r17+3); r7 = memb(r16+3) }
	{ r7 = memb(r19+3); r7 = memb(r18+3) }
	{ r7 = memb(r21+3); r7 = memb(r20+3) }
	{ r7 = memb(r23+3); r7 = memb(r22+3) }
	{ r7 = memb(r1+4); r7 = memb(r0+4) }
	{ r7 = memb(r3+4); r7 = memb(r2+4) }
	{ r7 = memb(r5+4); r7 = memb(r4+4) }
	{ r7 = memb(r7+4); r7 = memb(r6+4) }
	{ r7 = memb(r17+4); r7 = memb(r16+4) }
	{ r7 = memb(r19+4); r7 = memb(r18+4) }
	{ r7 = memb(r21+4); r7 = memb(r20+4) }
	{ r7 = memb(r23+4); r7 = memb(r22+4) }
	{ r7 = memb(r1+5); r7 = memb(r0+5) }
	{ r7 = memb(r3+5); r7 = memb(r2+5) }
	{ r7 = memb(r5+5); r7 = memb(r4+5) }
	{ r7 = memb(r7+5); r7 = memb(r6+5) }
	{ r7 = memb(r17+5); r7 = memb(r16+5) }
	{ r7 = memb(r19+5); r7 = memb(r18+5) }
	{ r7 = memb(r21+5); r7 = memb(r20+5) }
	{ r7 = memb(r23+5); r7 = memb(r22+5) }
	{ r7 = memb(r1+6); r7 = memb(r0+6) }
	{ r7 = memb(r3+6); r7 = memb(r2+6) }
	{ r7 = memb(r5+6); r7 = memb(r4+6) }
	{ r7 = memb(r7+6); r7 = memb(r6+6) }
	{ r7 = memb(r17+6); r7 = memb(r16+6) }
	{ r7 = memb(r19+6); r7 = memb(r18+6) }
	{ r7 = memb(r21+6); r7 = memb(r20+6) }
	{ r7 = memb(r23+6); r7 = memb(r22+6) }
	{ r7 = memb(r1+7); r7 = memb(r0+7) }
	{ r7 = memb(r3+7); r7 = memb(r2+7) }
	{ r7 = memb(r5+7); r7 = memb(r4+7) }
	{ r7 = memb(r7+7); r7 = memb(r6+7) }
	{ r7 = memb(r17+7); r7 = memb(r16+7) }
	{ r7 = memb(r19+7); r7 = memb(r18+7) }
	{ r7 = memb(r21+7); r7 = memb(r20+7) }
	{ r7 = memb(r23+7); r7 = memb(r22+7) }
	{ r7 = memb(r1); r7 = memb(r0) }
	{ r7 = memb(r3); r7 = memb(r2) }
	{ r7 = memb(r5); r7 = memb(r4) }
	{ r7 = memb(r7); r7 = memb(r6) }
	{ r7 = memb(r17); r7 = memb(r16) }
	{ r7 = memb(r19); r7 = memb(r18) }
	{ r7 = memb(r21); r7 = memb(r20) }
	{ r7 = memb(r23); r7 = memb(r22) }
	{ r7 = memb(r1+1); r7 = memb(r0+1) }
	{ r7 = memb(r3+1); r7 = memb(r2+1) }
	{ r7 = memb(r5+1); r7 = memb(r4+1) }
	{ r7 = memb(r7+1); r7 = memb(r6+1) }
	{ r7 = memb(r17+1); r7 = memb(r16+1) }
	{ r7 = memb(r19+1); r7 = memb(r18+1) }
	{ r7 = memb(r21+1); r7 = memb(r20+1) }
	{ r7 = memb(r23+1); r7 = memb(r22+1) }
	{ r7 = memb(r1+2); r7 = memb(r0+2) }
	{ r7 = memb(r3+2); r7 = memb(r2+2) }
	{ r7 = memb(r5+2); r7 = memb(r4+2) }
	{ r7 = memb(r7+2); r7 = memb(r6+2) }
	{ r7 = memb(r17+2); r7 = memb(r16+2) }
	{ r7 = memb(r19+2); r7 = memb(r18+2) }
	{ r7 = memb(r21+2); r7 = memb(r20+2) }
	{ r7 = memb(r23+2); r7 = memb(r22+2) }
	{ r7 = memb(r1+3); r7 = memb(r0+3) }
	{ r7 = memb(r3+3); r7 = memb(r2+3) }
	{ r7 = memb(r5+3); r7 = memb(r4+3) }
	{ r7 = memb(r7+3); r7 = memb(r6+3) }
	{ r7 = memb(r17+3); r7 = memb(r16+3) }
	{ r7 = memb(r19+3); r7 = memb(r18+3) }
	{ r7 = memb(r21+3); r7 = memb(r20+3) }
	{ r7 = memb(r23+3); r7 = memb(r22+3) }
	{ r7 = memb(r1+4); r7 = memb(r0+4) }
	{ r7 = memb(r3+4); r7 = memb(r2+4) }
	{ r7 = memb(r5+4); r7 = memb(r4+4) }
	{ r7 = memb(r7+4); r7 = memb(r6+4) }
	{ r7 = memb(r17+4); r7 = memb(r16+4) }
	{ r7 = memb(r19+4); r7 = memb(r18+4) }
	{ r7 = memb(r21+4); r7 = memb(r20+4) }
	{ r7 = memb(r23+4); r7 = memb(r22+4) }
	{ r7 = memb(r1+5); r7 = memb(r0+5) }
	{ r7 = memb(r3+5); r7 = memb(r2+5) }
	{ r7 = memb(r5+5); r7 = memb(r4+5) }
	{ r7 = memb(r7+5); r7 = memb(r6+5) }
	{ r7 = memb(r17+5); r7 = memb(r16+5) }
	{ r7 = memb(r19+5); r7 = memb(r18+5) }
	{ r7 = memb(r21+5); r7 = memb(r20+5) }
	{ r7 = memb(r23+5); r7 = memb(r22+5) }
	{ r7 = memb(r1+6); r7 = memb(r0+6) }
	{ r7 = memb(r3+6); r7 = memb(r2+6) }
	{ r7 = memb(r5+6); r7 = memb(r4+6) }
	{ r7 = memb(r7+6); r7 = memb(r6+6) }
	{ r7 = memb(r17+6); r7 = memb(r16+6) }
	{ r7 = memb(r19+6); r7 = memb(r18+6) }
	{ r7 = memb(r21+6); r7 = memb(r20+6) }
	{ r7 = memb(r23+6); r7 = memb(r22+6) }
	{ r7 = memb(r1+7); r7 = memb(r0+7) }
	{ r7 = memb(r3+7); r7 = memb(r2+7) }
	{ r7 = memb(r5+7); r7 = memb(r4+7) }
	{ r7 = memb(r7+7); r7 = memb(r6+7) }
	{ r7 = memb(r17+7); r7 = memb(r16+7) }
	{ r7 = memb(r19+7); r7 = memb(r18+7) }
	{ r7 = memb(r21+7); r7 = memb(r20+7) }
	{ r7 = memb(r23+7); r7 = memb(r22+7) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r7 = add(r7,r1); r7 = add(r7,r0) }
	{ r7 = add(r7,r3); r7 = add(r7,r2) }
	{ r7 = add(r7,r5); r7 = add(r7,r4) }
	{ r7 = add(r7,r7); r7 = add(r7,r6) }
	{ r7 = add(r7,r17); r7 = add(r7,r16) }
	{ r7 = add(r7,r19); r7 = add(r7,r18) }
	{ r7 = add(r7,r21); r7 = add(r7,r20) }
	{ r7 = add(r7,r23); r7 = add(r7,r22) }
	{ p0 = cmp.eq(r1,00000003); p0 = cmp.eq(r0,00000003) }
	{ p0 = cmp.eq(r3,00000003); p0 = cmp.eq(r2,00000003) }
	{ p0 = cmp.eq(r5,00000003); p0 = cmp.eq(r4,00000003) }
	{ p0 = cmp.eq(r7,00000003); p0 = cmp.eq(r6,00000003) }
	{ p0 = cmp.eq(r17,00000003); p0 = cmp.eq(r16,00000003) }
	{ p0 = cmp.eq(r19,00000003); p0 = cmp.eq(r18,00000003) }
	{ p0 = cmp.eq(r21,00000003); p0 = cmp.eq(r20,00000003) }
	{ p0 = cmp.eq(r23,00000003); p0 = cmp.eq(r22,00000003) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ jump	000BD694 }
	{ jump	001BD6D8 }
	{ jump	002BD71C }
	{ jump	003BD760 }
	{ jump	004BD7A4 }
	{ jump	005BD7E8 }
	{ jump	006BD82C }
	{ jump	007BD870 }
	{ jump	FF8BD8B4 }
	{ jump	FF9BD8F8 }
	{ jump	FFABD93C }
	{ jump	FFBBD980 }
	{ jump	FFCBD9C4 }
	{ jump	FFDBDA08 }
	{ jump	FFEBDA4C }
	{ jump	FFFBDA90 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r23.l = C49C }
	{  }
	{ r23.l = C59D }
	{  }
	{ r23.l = C69E }
	{  }
	{ r23.l = C79F }
	{  }
	{ r23.h = C89C }
	{  }
	{ r23.h = C99D }
	{  }
	{ r23.h = CA9E }
	{  }
	{ r23.h = CB9F }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ p3 = cmp.eq(r23,000001A8) }
	{ p3 = cmp.eq(r23,FFFFFFA9) }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r7 = 00002FC0 }
	{ r7 = 00002FC1 }
	{ r7 = 00006FC2 }
	{ r7 = 00006FC3 }
	{ r7 = FFFFAFC4 }
	{ r7 = FFFFAFC5 }
	{ r7 = FFFFEFC6 }
	{ r7 = FFFFEFC7 }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ nop }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000002) }
	{  }
	{ p0 = cmp.eq(r0,00000003) }
	{  }

;; hexagon_pre_main: 00003BB8
hexagon_pre_main proc
	{ r30 = 00000000 }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{ r1.h = 0000 }
	{ r1.l = 8063 }
	{  }
	{  }
	{ r4.h = 0000 }
	{ r4.l = 8063 }
	{  }
	{ r5.h = 0000 }
	{ r5.l = 0000 }
	{ p0 = cmp.eq(r5,00000000) }
	{ r0.h = 1000 }
	{ r0.l = 0000 }
	{  }
	{  }
	{  }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 8073 }
	{  }
	{  }
	{ r7.h = 0000 }
	{ r7.l = 0000 }
	{ p0 = cmp.eq(r7,00000000) }
	{ r0.h = 0040 }
	{ r0.l = 0000 }
	{  }
	{  }
	{  }
	{ r0.l = 8083 }
	{ r0.h = 0000 }
	{  }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 8083 }
	{  }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{ r1.h = 0000 }
	{ r1.l = 8083 }
	{  }
	{  }
	{ r6.h = 0000 }
	{ r6.l = 8083 }
	{  }
	{  }
	{  }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 8093 }
	{  }
	{  }
	{ r1.h = 0000 }
	{ r1.l = 0000 }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 8083 }
	{  }
	{  }
	{ r0.h = 0004 }
	{ r0.l = 0000 }
	{  }
	{ r28.h = 0000 }
	{ r28.l = 4B01 }
	{ r0.h = 0000 }
	{ r0.l = 80B3 }
	{ r1 = 00000400 }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 9C03 }
	{ r2.h = 0000 }
	{ r2.l = C903 }
	{ r28.h = 0000 }
	{ r28.l = 4E41 }
	{ r1 = 00000000 }
	{  }
	{  }
	{ r0.h = 0004 }
	{ r0.l = 0400 }
	{ r2.h = 0004 }
	{ r2.l = 0400 }
	{ r28.h = 0000 }
	{ r28.l = 4E41 }
	{ r1 = 00000000 }
	{  }
	{  }

;; hexagon_start_main: 00003D34
hexagon_start_main proc
	{ r2.h = 0000 }
	{ r2.l = 0000 }
	{ r3.h = 0000 }
	{ r3.l = 90C3 }
	{  }
	{  }
	{  }
	{  }
	{ r2.h = 0000 }
	{ r2.l = F700 }
	{ r3.h = 0000 }
	{ r3.l = CD82 }
	{ r4.h = EAFA }
	{ r4.l = FBBE }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{  }
	{ r28.h = 0000 }
	{ r28.l = 5301 }
	{  }
	{ r0.h = 0000 }
	{ r0.l = 0000 }
	{ p0 = cmp.eq(r0,00000000) }
	{  }
	{ r0 = 0000003F }
	{  }
