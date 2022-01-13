;;; Segment .text (00401000)
00401000 00 00 84 11 40 00 00 03 24 12 40 00 00 03 44 12 ....@...$.@...D.
00401010 40 00 00 00 00 00 00 00 EB 10 66 62 3A 43 2B 2B @.........fb:C++
00401020 48 4F 4F 4B 90 E9 98 20 40 00 A1 8B 20 40 00 C1 HOOK... @... @..
00401030 E0 02 A3 8F 20 40 00 52 6A 00 E8 0D 02 00 00 8B .... @.Rj.......
00401040 D0 E8 36 01 00 00 5A E8 26 02 00 00 E8 2F 01 00 ..6...Z.&..../..
00401050 00 6A 00 E8 58 01 00 00 59 68 34 20 40 00 6A 00 .j..X...Yh4 @.j.
00401060 E8 E7 01 00 00 A3 93 20 40 00 6A 00 E9 6D 02 00 ....... @.j..m..
00401070 00                                              .               

;; _GetExceptDLLinfo: 00401071
_GetExceptDLLinfo proc
	jmp	4011FCh
00401076                   33 C0 A0 7D 20 40 00 C3 A1 93       3..} @....
00401080 20 40 00 C3                                      @..            

;; fn00401084: 00401084
fn00401084 proc
	pusha
	mov	ebx,0BCB05000h
	push	ebx
	push	0BADh
	ret
00401091    B9 9C 00 00 00 0B C9 74 4D 83 3D 8B 20 40 00  .......tM.=. @.
004010A0 00 73 0A B8 FE 00 00 00 E8 D7 FF FF FF B9 9C 00 .s..............
004010B0 00 00 51 6A 08 E8 9E 01 00 00 50 E8 9E 01 00 00 ..Qj......P.....
004010C0 0B C0 75 0A B8 FD 00 00 00 E8 B6 FF FF FF 50 50 ..u...........PP
004010D0 FF 35 8B 20 40 00 E8 BB 01 00 00 FF 35 8B 20 40 .5. @.......5. @
004010E0 00 E8 AA 01 00 00 5F C3 B9 9C 00 00 00 0B C9 74 ......_........t
004010F0 19 E8 82 01 00 00 A3 8B 20 40 00 83 F8 00 73 91 ........ @....s.
00401100 B8 FC 00 00 00 E8 7A FF FF FF C3                ......z....     

;; fn0040110B: 0040110B
fn0040110B proc
	cmp	dword ptr [40208Bh],0h
	jc	40113Ch

l00401114:
	push	dword ptr [40208Bh]
	call	40128Ah
	or	eax,eax
	jz	40113Ch

l00401123:
	push	eax
	push	8h
	call	401258h
	push	eax
	call	401264h
	push	dword ptr [40208Bh]
	call	40127Eh

l0040113C:
	ret
0040113D                                        C3 83 3D              ..=
00401140 8B 20 40 00 00 72 10 E8 BF FF FF FF FF 35 8B 20 . @..r.......5. 
00401150 40 00 E8 2D 01 00 00 C3                         @..-....        

;; fn00401158: 00401158
;;   Called from:
;;     004011DD (in fn004011B0)
;;     00401203 (in fn004011FC)
fn00401158 proc
	mov	eax,[40208Bh]
	mov	edx,fs:[002Ch]
	mov	eax,[edx+eax*4]
	ret
00401167                      90                                .        

;; main: 00401168
main proc
	push	ebp
	mov	ebp,esp
	push	4020A4h
	call	4012F0h
	pop	ecx
	xor	eax,eax
	pop	ebp
	ret
0040117A                               90 90                       ..    

;; fn0040117C: 0040117C
fn0040117C proc
	ret
0040117D                                        90 90 90              ...

;; fn00401180: 00401180
fn00401180 proc
	ret
00401181    90 90 90 68 84 21 40 00 6A 00 E8 BC 00 00 00  ...h.!@.j......
00401190 50 E8 BC 00 00 00 A3 B4 21 40 00 83 3D B4 21 40 P.......!@..=.!@
004011A0 00 00 75 0A C7 05 B4 21 40 00 98 20 40 00 C3 90 ..u....!@.. @...

;; fn004011B0: 004011B0
fn004011B0 proc
	push	ebp
	mov	ebp,esp
	add	esp,0F8h
	push	ebx
	mov	ebx,[ebp+8h]
	test	ebx,ebx
	setnz	al
	and	eax,1h
	test	ebx,ebx
	jnz	4011D7h

l004011C6:
	test	ebx,ebx
	jnz	4011D7h

l004011CA:
	lea	edx,[ebp-8h]
	push	edx
	call	4011FCh
	pop	ecx
	mov	ebx,[ebp-4h]

l004011D7:
	push	9Ch
	push	ebx
	call	401158h
	add	eax,0h
	push	eax
	call	4012EAh
	add	esp,0Ch
	call	40126Ch
	pop	ebx
	pop	ecx
	pop	ecx
	pop	ebp
	ret
004011FB                                  90                        .    

;; fn004011FC: 004011FC
;;   Called from:
;;     00401071 (in _GetExceptDLLinfo)
;;     004011CE (in fn004011B0)
fn004011FC proc
	push	ebp
	mov	ebp,esp
	push	ebx
	mov	ebx,[ebp+8h]
	call	401158h
	add	eax,1Ch
	mov	[4020F8h],eax
	mov	dword ptr [ebx],82727349h
	mov	dword ptr [ebx+4h],4020E4h
	pop	ebx
	pop	ebp
	ret
00401223          90 E8 AF 00 00 00 A1 EC 50 40 00 8B 10    ........P@...
00401230 89 15 B8 21 40 00 8B 0D F0 50 40 00 8B 01 A3 BC ...!@....P@.....
00401240 21 40 00 C3 E8 65 00 00 00 C3 90 90 FF 25 54 50 !@...e.......%TP
00401250 40 00 FF 25 58 50 40 00 FF 25 5C 50 40 00 FF 25 @..%XP@..%\P@..%
00401260 60 50 40 00 FF 25 64 50 40 00 CC CC FF 25 CC 50 `P@..%dP@....%.P
00401270 40 00 FF 25 D0 50 40 00 FF 25 D4 50 40 00 FF 25 @..%.P@..%.P@..%
00401280 D8 50 40 00 FF 25 DC 50 40 00 FF 25 E0 50 40 00 .P@..%.P@..%.P@.
00401290 FF 25 E4 50 40 00 FF 25 E8 50 40 00 FF 25 EC 50 .%.P@..%.P@..%.P
004012A0 40 00 FF 25 F0 50 40 00 FF 25 F4 50 40 00 FF 25 @..%.P@..%.P@..%
004012B0 F8 50 40 00 FF 25 FC 50 40 00 FF 25 00 51 40 00 .P@..%.P@..%.Q@.
004012C0 FF 25 04 51 40 00 FF 25 08 51 40 00 FF 25 0C 51 .%.Q@..%.Q@..%.Q
004012D0 40 00 FF 25 10 51 40 00 FF 25 14 51 40 00 FF 25 @..%.Q@..%.Q@..%
004012E0 18 51 40 00 FF 25 1C 51 40 00 FF 25 20 51 40 00 .Q@..%.Q@..% Q@.
004012F0 FF 25 24 51 40 00 CC CC 00 00 00 00 00 00 00 00 .%$Q@...........
00401300 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................
; ...
