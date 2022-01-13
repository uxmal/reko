;;; Segment 0800 (0800:0000)
0800:0000 BA DB 09 2E 89 16 F8 01 B4 30 CD 21 8B 2E 02 00 .........0.!....
0800:0010 8B 1E 2C 00 8E DA A3 92 00 8C 06 90 00 89 1E 8C ..,.............
0800:0020 00 89 2E AC 00 C7 06 96 00 FF FF E8 34 01 C4 3E ............4..>
0800:0030 8A 00 8B C7 8B D8 B9 FF 7F 26 81 3D 38 37 75 19 .........&.=87u.
0800:0040 26 8B 55 02 80 FA 3D 75 10 80 E6 DF FF 06 96 00 &.U...=u........
0800:0050 80 FE 59 75 04 FF 06 96 00 F2 AE E3 61 43 26 38 ..Yu........aC&8
0800:0060 05 75 D6 80 CD 80 F7 D9 89 0E 8A 00 B9 01 00 D3 .u..............
0800:0070 E3 83 C3 08 83 E3 F8 89 1E 8E 00 8C DA 2B EA 8B .............+..
0800:0080 3E 3C 02 81 FF 00 02 73 07 BF 00 02 89 3E 3C 02 ><.....s.....><.
0800:0090 81 C7 2E 06 72 28 03 3E 3A 02 72 22 B1 04 D3 EF ....r(.>:.r"....
0800:00A0 47 3B EF 72 19 83 3E 3C 02 00 74 07 83 3E 3A 02 G;.r..><..t..>:.
0800:00B0 00 75 0E BF 00 10 3B EF 77 07 8B FD EB 03 E9 21 .u....;.w......!
0800:00C0 01 8B DF 03 DA 89 1E A4 00 89 1E A8 00 A1 90 00 ................
0800:00D0 2B D8 8E C0 B4 4A 57 CD 21 5F D3 E7 FA 8E D2 8B +....JW.!_......
0800:00E0 E7 FB 33 C0 2E 8E 06 F8 01 BF E8 05 B9 2E 06 2B ..3............+
0800:00F0 CF F3 AA 0E FF 16 DA 05 E8 3B 02 E8 23 03 B4 00 .........;..#...
0800:0100 CD 1A 89 16 98 00 89 0E 9A 00 FF 16 DE 05 FF 36 ...............6
0800:0110 88 00 FF 36 86 00 FF 36 84 00 E8 48 01 50 E8 E0 ...6...6...H.P..
0800:0120 01                                              .               

;; __exit: 0800:0121
;;   Called from:
;;     0800:01F5 (in _abort)
;;     0800:01F5 (in fn0800_01E9)
;;     0800:032A (in _exit)
__exit proc
	mov	ds,cs:[01F8h]
	call	01A5h
	push	cs
	call	word ptr [05DCh]
	xor	ax,ax
	mov	si,ax
	mov	cx,2Fh
	nop
	cld

l0800_0137:
	add	al,[si]
	adc	ah,0h
	inc	si
	loop	0137h

l0800_013F:
	sub	ax,0D37h
	nop
	jz	014Fh

l0800_0145:
	mov	cx,19h
	nop
	mov	dx,2Fh
	call	01DAh

l0800_014F:
	mov	bp,sp
	mov	ah,4Ch
	mov	al,[bp+2h]
	int	21h
0800:0158                         B9 0E 00 90 BA 48 00 E9         .....H..
0800:0160 87 00                                           ..              

;; fn0800_0162: 0800:0162
fn0800_0162 proc
	push	ds
	mov	ax,3500h
	int	21h
	mov	[0074h],bx
	mov	[0076h],es
	mov	ax,3504h
	int	21h
	mov	[0078h],bx
	mov	[007Ah],es
	mov	ax,3505h
	int	21h
	mov	[007Ch],bx
	mov	[007Eh],es
	mov	ax,3506h
	int	21h
	mov	[0080h],bx
	mov	[0082h],es
	mov	ax,2500h
	mov	dx,cs
	mov	ds,dx
	mov	dx,158h
	int	21h
	pop	ds
	ret

;; __restorezero: 0800:01A5
;;   Called from:
;;     0800:0126 (in __exit)
__restorezero proc
	push	ds
	mov	ax,2500h
	lds	dx,[0074h]
	int	21h
	pop	ds
	push	ds
	mov	ax,2504h
	lds	dx,[0078h]
	int	21h
	pop	ds
	push	ds
	mov	ax,2505h
	lds	dx,[007Ch]
	int	21h
	pop	ds
	push	ds
	mov	ax,2506h
	lds	dx,[0080h]
	int	21h
	pop	ds
	ret
0800:01D2       C7 06 96 00 00 00 CB C3                     ........      

;; fn0800_01DA: 0800:01DA
;;   Called from:
;;     0800:014C (in __exit)
;;     0800:01EE (in _abort)
;;     0800:01EE (in fn0800_01E9)
fn0800_01DA proc
	mov	ah,40h
	mov	bx,2h
	int	21h
	ret

;; _abort: 0800:01E2
;;   Called from:
;;     0800:03E7 (in __setargv)
;;     0800:0454 (in __setenvp)
_abort proc
	mov	cx,1Eh
	nop
	mov	dx,56h

;; fn0800_01E9: 0800:01E9
fn0800_01E9 proc
	mov	ds,cs:[01F8h]
	call	01DAh
	mov	ax,3h
	push	ax
	call	0121h
	add	[bx+si],al

;; _f3: 0800:01FA
;;   Called from:
;;     0800:01F8 (in _abort)
;;     0800:01F8 (in fn0800_01E9)
;;     0800:0202 (in _f2)
;;     0800:0205 (in _f2)
;;     0800:0208 (in _f2)
;;     0800:020B (in _f2)
;;     0800:020E (in _f2)
;;     0800:0211 (in _f2)
;;     0800:0214 (in _f2)
;;     0800:0217 (in _f2)
;;     0800:021A (in _f2)
;;     0800:021D (in _f2)
_f3 proc
	push	bp
	mov	bp,sp
	pop	bp
	ret

;; _f2: 0800:01FF
;;   Called from:
;;     0800:0225 (in f1_name_overridden)
;;     0800:0228 (in f1_name_overridden)
;;     0800:022B (in f1_name_overridden)
;;     0800:022E (in f1_name_overridden)
;;     0800:0231 (in f1_name_overridden)
;;     0800:0234 (in f1_name_overridden)
;;     0800:0237 (in f1_name_overridden)
;;     0800:023A (in f1_name_overridden)
;;     0800:023D (in f1_name_overridden)
;;     0800:0240 (in f1_name_overridden)
_f2 proc
	push	bp
	mov	bp,sp
	call	01FAh
	call	01FAh
	call	01FAh
	call	01FAh
	call	01FAh
	call	01FAh
	call	01FAh
	call	01FAh
	call	01FAh
	call	01FAh
	pop	bp
	ret

;; f1_name_overridden: 0800:0222
;;   Called from:
;;     0800:0248 (in _f0)
;;     0800:024B (in _f0)
;;     0800:024E (in _f0)
;;     0800:0251 (in _f0)
;;     0800:0254 (in _f0)
;;     0800:0257 (in _f0)
;;     0800:025A (in _f0)
;;     0800:025D (in _f0)
;;     0800:0260 (in _f0)
f1_name_overridden proc
	push	bp
	mov	bp,sp
	call	01FFh
	call	01FFh
	call	01FFh
	call	01FFh
	call	01FFh
	call	01FFh
	call	01FFh
	call	01FFh
	call	01FFh
	call	01FFh
	pop	bp
	ret

;; _f0: 0800:0245
;;   Called from:
;;     0800:029C (in _main)
_f0 proc
	push	bp
	mov	bp,sp
	call	0222h
	call	0222h
	call	0222h
	call	0222h
	call	0222h
	call	0222h
	call	0222h
	call	0222h
	call	0222h
	pop	bp
	ret

;; _main: 0800:0265
_main proc
	push	bp
	mov	bp,sp
	sub	sp,8h
	mov	ax,194h
	push	ax
	call	0E4Bh
	pop	cx
	lea	ax,[bp-4h]
	push	ax
	mov	ax,1B0h
	push	ax
	call	16D4h
	pop	cx
	pop	cx
	push	word ptr [bp-2h]
	push	word ptr [bp-4h]
	mov	ax,1B4h
	push	ax
	call	0E4Bh
	add	sp,6h
	mov	word ptr [bp-6h],0h
	mov	word ptr [bp-8h],1h
	jmp	02A7h

l0800_029C:
	call	0245h
	add	word ptr [bp-8h],1h
	adc	word ptr [bp-6h],0h

l0800_02A7:
	mov	dx,[bp-6h]
	mov	ax,[bp-8h]
	cmp	dx,[bp-2h]
	jl	029Ch

l0800_02B2:
	jg	02B9h

l0800_02B4:
	cmp	ax,[bp-4h]
	jbe	029Ch

l0800_02B9:
	mov	ax,1CEh
	push	ax
	call	0E4Bh
	pop	cx
	mov	sp,bp
	pop	bp
	ret

;; __IOERROR: 0800:02C5
;;   Called from:
;;     0800:0AC8 (in __read)
;;     0800:0C21 (in __write)
;;     0800:0C4B (in _lseek)
;;     0800:0DC4 (in _eof)
__IOERROR proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+4h]
	or	si,si
	jl	02E4h

l0800_02D0:
	cmp	si,58h
	jbe	02D8h

l0800_02D5:
	mov	si,57h

l0800_02D8:
	mov	[01D8h],si
	mov	al,[si+1DAh]
	cbw
	xchg	si,ax
	jmp	02F1h

l0800_02E4:
	neg	si
	cmp	si,23h
	ja	02D5h

l0800_02EB:
	mov	word ptr [01D8h],0FFFFh

l0800_02F1:
	mov	ax,si
	mov	[0094h],ax
	mov	ax,0FFFFh
	jmp	02FBh

l0800_02FB:
	pop	si
	pop	bp
	ret	2h
0800:0300 C3                                              .               

;; _exit: 0800:0301
_exit proc
	push	bp
	mov	bp,sp
	jmp	0310h

l0800_0306:
	mov	bx,[023Eh]
	shl	bx,1h
	call	word ptr [bx+5E8h]

l0800_0310:
	mov	ax,[023Eh]
	dec	word ptr [023Eh]
	or	ax,ax
	jnz	0306h

l0800_031B:
	call	word ptr [0234h]
	call	word ptr [0236h]
	call	word ptr [0238h]
	push	word ptr [bp+4h]
	call	0121h
	pop	cx
	pop	bp
	ret
0800:0330 00 00 00 00 00 00                               ......          

;; __setargv: 0800:0336
__setargv proc
	pop	word ptr cs:[0330h]
	mov	cs:[0332h],ds
	cld
	mov	es,[0090h]
	mov	si,80h
	xor	ah,ah
	lodsb
	inc	ax
	mov	bp,es
	xchg	si,dx
	xchg	bx,ax
	mov	si,[008Ah]
	add	si,2h
	mov	cx,1h
	cmp	byte ptr [0092h],3h
	jc	0374h

l0800_0363:
	mov	es,[008Ch]
	mov	di,si
	mov	cl,7Fh
	xor	al,al

l0800_036D:
	repne scasb

l0800_036F:
	jcxz	03E7h

l0800_0371:
	xor	cl,7Fh

l0800_0374:
	sub	sp,2h
	mov	ax,1h
	add	ax,bx
	add	ax,cx
	and	ax,0FFFEh
	mov	di,sp
	sub	di,ax
	jc	03E7h

l0800_0387:
	mov	sp,di
	mov	ax,es
	mov	ds,ax
	mov	ax,ss
	mov	es,ax
	push	cx
	dec	cx

l0800_0393:
	rep movsb

l0800_0395:
	xor	al,al
	stosb
	mov	ds,bp
	xchg	dx,si
	xchg	cx,bx
	mov	ax,bx
	mov	dx,ax
	inc	bx

l0800_03A3:
	call	03BFh
	ja	03AFh

l0800_03A8:
	jc	03EAh

l0800_03AA:
	call	03BFh
	ja	03A8h

l0800_03AF:
	cmp	al,20h
	jz	03BBh

l0800_03B3:
	cmp	al,0Dh
	jz	03BBh

l0800_03B7:
	cmp	al,9h
	jnz	03A3h

l0800_03BB:
	xor	al,al
	jmp	03A3h

;; fn0800_03BF: 0800:03BF
;;   Called from:
;;     0800:03A3 (in __setargv)
;;     0800:03AA (in __setargv)
fn0800_03BF proc
	or	ax,ax
	jz	03CAh

l0800_03C3:
	inc	dx
	stosb
	or	al,al
	jnz	03CAh

l0800_03C9:
	inc	bx

l0800_03CA:
	xchg	al,ah
	xor	al,al
	stc
	jcxz	03E6h

l0800_03D1:
	lodsb
	dec	cx
	sub	al,22h
	jz	03E6h

l0800_03D7:
	add	al,22h
	cmp	al,5Ch
	jnz	03E4h

l0800_03DD:
	cmp	byte ptr [si],22h
	jnz	03E4h

l0800_03E2:
	lodsb
	dec	cx

l0800_03E4:
	or	si,si

l0800_03E6:
	ret

l0800_03E7:
	jmp	01E2h

l0800_03EA:
	pop	cx
	add	cx,dx
	mov	ds,cs:[0332h]
	mov	[0084h],bx
	inc	bx
	add	bx,bx
	mov	si,sp
	mov	bp,sp
	sub	bp,bx
	jc	03E7h

l0800_0401:
	mov	sp,bp
	mov	[0086h],bp

l0800_0407:
	jcxz	0417h

l0800_0409:
	mov	[bp+0h],si
	add	bp,2h

l0800_040F:
	lodsb
	or	al,al
	loopne	040Fh

l0800_0415:
	jz	0407h

l0800_0417:
	xor	ax,ax
	mov	[bp+0h],ax
	jmp	word ptr cs:[0330h]

;; __setenvp: 0800:0421
__setenvp proc
	mov	cx,[008Ah]
	push	cx
	call	0570h
	pop	cx
	mov	di,ax
	or	ax,ax
	jz	0454h

l0800_0430:
	push	ds
	push	ds
	pop	es
	mov	ds,[008Ch]
	xor	si,si
	cld

l0800_043A:
	rep movsb

l0800_043C:
	pop	ds
	mov	di,ax
	push	es
	push	word ptr [008Eh]
	call	0570h
	add	sp,2h
	mov	bx,ax
	pop	es
	mov	[0088h],ax
	or	ax,ax
	jnz	0457h

l0800_0454:
	jmp	01E2h

l0800_0457:
	xor	ax,ax
	mov	cx,0FFFFh

l0800_045C:
	mov	[bx],di
	add	bx,2h

l0800_0461:
	repne scasb

l0800_0463:
	cmp	es:[di],al
	jnz	045Ch

l0800_0468:
	mov	[bx],ax
	ret
0800:046B                                  55 8B EC 83 3E            U...>
0800:0470 3E 02 20 75 05 B8 01 00 EB 15 8B 46 04 8B 1E 3E >. u.......F...>
0800:0480 02 D1 E3 89 87 E8 05 FF 06 3E 02 33 C0 EB 00 5D .........>.3...]
0800:0490 C3                                              .               

;; ___pull_free_block: 0800:0491
;;   Called from:
;;     0800:05C2 (in _malloc)
;;     0800:15FC (in fn0800_15CF)
;;     0800:1630 (in fn0800_1606)
___pull_free_block proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	di,[bp+4h]
	mov	ax,[di+6h]
	mov	[062Ah],ax
	cmp	ax,di
	jnz	04ABh

l0800_04A3:
	mov	word ptr [062Ah],0h
	jmp	04BBh

l0800_04AB:
	mov	si,[di+4h]
	mov	bx,[062Ah]
	mov	[bx+4h],si
	mov	ax,[062Ah]
	mov	[si+6h],ax

l0800_04BB:
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_04BF: 0800:04BF
;;   Called from:
;;     0800:05B4 (in _malloc)
fn0800_04BF proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	di,[bp+4h]
	mov	ax,[bp+6h]
	sub	[di],ax
	mov	si,[di]
	add	si,di
	mov	ax,[bp+6h]
	inc	ax
	mov	[si],ax
	mov	[si+2h],di
	mov	ax,[0628h]
	cmp	ax,di
	jnz	04E6h

l0800_04E0:
	mov	[0628h],si
	jmp	04EEh

l0800_04E6:
	mov	di,si
	add	di,[bp+6h]
	mov	[di+2h],si

l0800_04EE:
	mov	ax,si
	add	ax,4h
	jmp	04F5h

l0800_04F5:
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_04F9: 0800:04F9
;;   Called from:
;;     0800:05D9 (in _malloc)
fn0800_04F9 proc
	push	bp
	mov	bp,sp
	push	si
	mov	ax,[bp+4h]
	xor	dx,dx
	and	ax,0FFFFh
	and	dx,0h
	push	dx
	push	ax
	call	0607h
	pop	cx
	pop	cx
	mov	si,ax
	cmp	si,0FFh
	jnz	051Bh

l0800_0517:
	xor	ax,ax
	jmp	0533h

l0800_051B:
	mov	ax,[0628h]
	mov	[si+2h],ax
	mov	ax,[bp+4h]
	inc	ax
	mov	[si],ax
	mov	[0628h],si
	mov	ax,[0628h]
	add	ax,4h
	jmp	0533h

l0800_0533:
	pop	si
	pop	bp
	ret

;; fn0800_0536: 0800:0536
;;   Called from:
;;     0800:0597 (in _malloc)
fn0800_0536 proc
	push	bp
	mov	bp,sp
	push	si
	mov	ax,[bp+4h]
	xor	dx,dx
	and	ax,0FFFFh
	and	dx,0h
	push	dx
	push	ax
	call	0607h
	pop	cx
	pop	cx
	mov	si,ax
	cmp	si,0FFh
	jnz	0558h

l0800_0554:
	xor	ax,ax
	jmp	056Dh

l0800_0558:
	mov	[062Ch],si
	mov	[0628h],si
	mov	ax,[bp+4h]
	inc	ax
	mov	[si],ax
	mov	ax,si
	add	ax,4h
	jmp	056Dh

l0800_056D:
	pop	si
	pop	bp
	ret

;; _malloc: 0800:0570
;;   Called from:
;;     0800:0426 (in __setenvp)
;;     0800:0444 (in __setenvp)
;;     0800:09A0 (in _setvbuf)
_malloc proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	di,[bp+4h]
	or	di,di
	jz	0581h

l0800_057C:
	cmp	di,0F4h
	jbe	0585h

l0800_0581:
	xor	ax,ax
	jmp	05DFh

l0800_0585:
	mov	ax,di
	add	ax,0Bh
	and	ax,0FFF8h
	mov	di,ax
	cmp	word ptr [062Ch],0h
	jnz	059Dh

l0800_0596:
	push	di
	call	0536h
	pop	cx
	jmp	05DFh

l0800_059D:
	mov	si,[062Ah]
	mov	ax,si
	or	ax,ax
	jz	05D8h

l0800_05A7:
	mov	ax,[si]
	mov	dx,di
	add	dx,28h
	cmp	ax,dx
	jc	05BBh

l0800_05B2:
	push	di
	push	si
	call	04BFh
	pop	cx
	pop	cx
	jmp	05DFh

l0800_05BB:
	mov	ax,[si]
	cmp	ax,di
	jc	05CFh

l0800_05C1:
	push	si
	call	0491h
	pop	cx
	inc	word ptr [si]
	mov	ax,si
	add	ax,4h
	jmp	05DFh

l0800_05CF:
	mov	si,[si+6h]
	cmp	si,[062Ah]
	jnz	05A7h

l0800_05D8:
	push	di
	call	04F9h
	pop	cx
	jmp	05DFh

l0800_05DF:
	pop	di
	pop	si
	pop	bp
	ret

;; ___brk: 0800:05E3
;;   Called from:
;;     0800:0641 (in _brk)
___brk proc
	push	bp
	mov	bp,sp
	mov	ax,[bp+4h]
	mov	dx,sp
	sub	dx,100h
	cmp	ax,dx
	jnc	05FAh

l0800_05F3:
	mov	[009Eh],ax
	xor	ax,ax
	jmp	0605h

l0800_05FA:
	mov	word ptr [0094h],8h
	mov	ax,0FFFFh
	jmp	0605h

l0800_0605:
	pop	bp
	ret

;; ___sbrk: 0800:0607
;;   Called from:
;;     0800:050B (in fn0800_04F9)
;;     0800:0548 (in fn0800_0536)
___sbrk proc
	push	bp
	mov	bp,sp
	mov	ax,[bp+4h]
	mov	dx,[bp+6h]
	add	ax,[009Eh]
	adc	dx,0h
	mov	cx,ax
	add	cx,100h
	adc	dx,0h
	or	dx,dx
	jnz	062Eh

l0800_0624:
	cmp	cx,sp
	jnc	062Eh

l0800_0628:
	xchg	[009Eh],ax
	jmp	0639h

l0800_062E:
	mov	word ptr [0094h],8h
	mov	ax,0FFFFh
	jmp	0639h

l0800_0639:
	pop	bp
	ret

;; _brk: 0800:063B
;;   Called from:
;;     0800:1614 (in fn0800_1606)
;;     0800:164B (in fn0800_1606)
;;     0800:1655 (in fn0800_1606)
_brk proc
	push	bp
	mov	bp,sp
	push	word ptr [bp+4h]
	call	05E3h
	pop	cx
	jmp	0647h

l0800_0647:
	pop	bp
	ret
0800:0649                            55 8B EC 8B 46 04 99          U...F..
0800:0650 52 50 E8 B2 FF 8B E5 EB 00 5D C3                RP.......].     

;; fn0800_065B: 0800:065B
;;   Called from:
;;     0800:06C3 (in _fseek)
fn0800_065B proc
	push	bp
	mov	bp,sp
	sub	sp,2h
	push	si
	push	di
	mov	bx,[bp+4h]
	mov	si,[bx]
	mov	ax,si
	mov	[bp-2h],ax
	mov	bx,[bp+4h]
	test	word ptr [bx+2h],40h
	jz	067Bh

l0800_0677:
	mov	ax,si
	jmp	069Ah

l0800_067B:
	mov	bx,[bp+4h]
	mov	di,[bx+0Ah]
	jmp	068Eh

l0800_0683:
	mov	bx,di
	inc	di
	cmp	byte ptr [bx],0Ah
	jnz	068Eh

l0800_068B:
	inc	word ptr [bp-2h]

l0800_068E:
	mov	ax,si
	dec	si
	or	ax,ax
	jnz	0683h

l0800_0695:
	mov	ax,[bp-2h]
	jmp	069Ah

l0800_069A:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	2h

;; _fseek: 0800:06A2
;;   Called from:
;;     0800:0960 (in _setvbuf)
_fseek proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+4h]
	push	si
	call	0DCDh
	pop	cx
	or	ax,ax
	jz	06B7h

l0800_06B2:
	mov	ax,0FFFFh
	jmp	0703h

l0800_06B7:
	cmp	word ptr [bp+0Ah],1h
	jnz	06CDh

l0800_06BD:
	cmp	word ptr [si],0h
	jle	06CDh

l0800_06C2:
	push	si
	call	065Bh
	cwd
	sub	[bp+6h],ax
	sbb	[bp+8h],dx

l0800_06CD:
	and	word ptr [si+2h],0FE5Fh
	mov	word ptr [si],0h
	mov	ax,[si+8h]
	mov	[si+0Ah],ax
	push	word ptr [bp+0Ah]
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	mov	al,[si+4h]
	cbw
	push	ax
	call	0C28h
	add	sp,8h
	cmp	dx,0FFh
	jnz	06FFh

l0800_06F5:
	cmp	ax,0FFFFh
	jnz	06FFh

l0800_06FA:
	mov	ax,0FFFFh
	jmp	0701h

l0800_06FF:
	xor	ax,ax

l0800_0701:
	jmp	0703h

l0800_0703:
	pop	si
	pop	bp
	ret
0800:0706                   55 8B EC 83 EC 04 56 8B 76 04       U.....V.v.
0800:0710 56 E8 B9 06 59 0B C0 74 08 BA FF FF B8 FF FF EB V...Y..t........
0800:0720 35 8A 44 04 98 50 E8 25 16 59 89 56 FE 89 46 FC 5.D..P.%.Y.V..F.
0800:0730 83 3C 00 7E 19 8B 56 FE 8B 46 FC 52 50 56 E8 1A .<.~..V..F.RPV..
0800:0740 FF 99 8B D8 8B CA 58 5A 2B C3 1B D1 EB 06 8B 56 ......XZ+......V
0800:0750 FE 8B 46 FC EB 00 5E 8B E5 5D C3                ..F...^..].     

;; fn0800_075B: 0800:075B
;;   Called from:
;;     0800:0790 (in fn0800_0782)
;;     0800:0888 (in _fgetc)
fn0800_075B proc
	push	si
	push	di
	mov	di,14h
	mov	si,342h
	jmp	0778h

l0800_0765:
	mov	ax,[si+2h]
	and	ax,300h
	cmp	ax,300h
	jnz	0775h

l0800_0770:
	push	si
	call	0DCDh
	pop	cx

l0800_0775:
	add	si,10h

l0800_0778:
	mov	ax,di
	dec	di
	or	ax,ax
	jnz	0765h

l0800_077F:
	pop	di
	pop	si
	ret

;; fn0800_0782: 0800:0782
;;   Called from:
;;     0800:0830 (in _fgetc)
fn0800_0782 proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+4h]
	test	word ptr [si+2h],200h
	jz	0793h

l0800_0790:
	call	075Bh

l0800_0793:
	push	word ptr [si+6h]
	mov	ax,[si+8h]
	mov	[si+0Ah],ax
	push	ax
	mov	al,[si+4h]
	cbw
	push	ax
	call	09F7h
	add	sp,6h
	mov	[si],ax
	or	ax,ax
	jle	07B9h

l0800_07AE:
	and	word ptr [si+2h],0FFDFh
	xor	ax,ax
	jmp	07DAh
0800:07B7                      EB 1C                             ..       

l0800_07B9:
	cmp	word ptr [si],0h
	jnz	07CCh

l0800_07BE:
	mov	ax,[si+2h]
	and	ax,0FE7Fh
	or	ax,20h
	mov	[si+2h],ax
	jmp	07D5h

l0800_07CC:
	mov	word ptr [si],0h
	or	word ptr [si+2h],10h

l0800_07D5:
	mov	ax,0FFFFh
	jmp	07DAh

l0800_07DA:
	pop	si
	pop	bp
	ret	2h
0800:07DF                                              55                U
0800:07E0 8B EC 56 8B 76 04 FF 04 56 E8 06 00 59 EB 00 5E ..V.v...V...Y..^
0800:07F0 5D C3                                           ].              

;; _fgetc: 0800:07F2
_fgetc proc
	push	bp
	mov	bp,sp
	sub	sp,2h
	push	si
	mov	si,[bp+4h]

l0800_07FC:
	dec	word ptr [si]
	jl	080Eh

l0800_0800:
	inc	word ptr [si+0Ah]
	mov	bx,[si+0Ah]
	mov	al,[bx-1h]
	mov	ah,0h
	jmp	08E2h

l0800_080E:
	inc	word ptr [si]
	jl	0819h

l0800_0812:
	test	word ptr [si+2h],110h
	jz	0824h

l0800_0819:
	or	word ptr [si+2h],10h
	mov	ax,0FFFFh
	jmp	08E2h

l0800_0824:
	or	word ptr [si+2h],80h
	cmp	word ptr [si+6h],0h
	jz	0842h

l0800_082F:
	push	si
	call	0782h
	or	ax,ax
	jz	083Dh

l0800_0837:
	mov	ax,0FFFFh
	jmp	08E2h

l0800_083D:
	jmp	07FCh
0800:083F                                              E9                .
0800:0840 A0 00                                           ..              

l0800_0842:
	cmp	word ptr [04AAh],0h
	jnz	0881h

l0800_0849:
	mov	ax,342h
	cmp	ax,si
	jnz	0881h

l0800_0850:
	mov	al,[si+4h]
	cbw
	push	ax
	call	08F2h
	pop	cx
	or	ax,ax
	jnz	0862h

l0800_085D:
	and	word ptr [si+2h],0FDFFh

l0800_0862:
	mov	ax,200h
	push	ax
	test	word ptr [si+2h],200h
	jz	0872h

l0800_086D:
	mov	ax,1h
	jmp	0874h

l0800_0872:
	xor	ax,ax

l0800_0874:
	push	ax
	xor	ax,ax
	push	ax
	push	si
	call	0904h
	add	sp,8h
	jmp	0824h

l0800_0881:
	test	word ptr [si+2h],200h
	jz	088Bh

l0800_0888:
	call	075Bh

l0800_088B:
	mov	ax,1h
	push	ax
	lea	ax,[bp-1h]
	push	ax
	mov	al,[si+4h]
	cbw
	push	ax
	call	0AB3h
	add	sp,6h
	cmp	ax,1h
	jz	08C9h

l0800_08A3:
	mov	al,[si+4h]
	cbw
	push	ax
	call	0D5Fh
	pop	cx
	cmp	ax,1h
	jz	08B8h

l0800_08B1:
	or	word ptr [si+2h],10h
	jmp	08C4h

l0800_08B8:
	mov	ax,[si+2h]
	and	ax,0FE7Fh
	or	ax,20h
	mov	[si+2h],ax

l0800_08C4:
	mov	ax,0FFFFh
	jmp	08E2h

l0800_08C9:
	cmp	byte ptr [bp-1h],0Dh
	jnz	08D6h

l0800_08CF:
	test	word ptr [si+2h],40h
	jz	0881h

l0800_08D6:
	and	word ptr [si+2h],0FFDFh
	mov	al,[bp-1h]
	mov	ah,0h
	jmp	08E2h

l0800_08E2:
	pop	si
	mov	sp,bp
	pop	bp
	ret
0800:08E7                      B8 42 03 50 E8 04 FF 59 EB        .B.P...Y.
0800:08F0 00 C3                                           ..              

;; _isatty: 0800:08F2
;;   Called from:
;;     0800:0855 (in _fgetc)
_isatty proc
	push	bp
	mov	bp,sp
	mov	ax,4400h
	mov	bx,[bp+4h]
	int	21h
	mov	ax,dx
	and	ax,80h
	pop	bp
	ret

;; _setvbuf: 0800:0904
;;   Called from:
;;     0800:0879 (in _fgetc)
_setvbuf proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	di,[bp+0Ah]
	mov	si,[bp+4h]
	mov	ax,[si+0Eh]
	cmp	ax,si
	jnz	0922h

l0800_0916:
	cmp	word ptr [bp+8h],2h
	jg	0922h

l0800_091C:
	cmp	di,7FFFh
	jbe	0928h

l0800_0922:
	mov	ax,0FFFFh
	jmp	09D2h

l0800_0928:
	cmp	word ptr [04ACh],0h
	jnz	093Eh

l0800_092F:
	mov	ax,352h
	cmp	ax,si
	jnz	093Eh

l0800_0936:
	mov	word ptr [04ACh],1h
	jmp	0952h

l0800_093E:
	cmp	word ptr [04AAh],0h
	jnz	0952h

l0800_0945:
	mov	ax,342h
	cmp	ax,si
	jnz	0952h

l0800_094C:
	mov	word ptr [04AAh],1h

l0800_0952:
	cmp	word ptr [si],0h
	jz	0966h

l0800_0957:
	mov	ax,1h
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	si
	call	06A2h
	add	sp,8h

l0800_0966:
	test	word ptr [si+2h],4h
	jz	0974h

l0800_096D:
	push	word ptr [si+8h]
	call	16ADh
	pop	cx

l0800_0974:
	and	word ptr [si+2h],0FFF3h
	mov	word ptr [si+6h],0h
	mov	ax,si
	add	ax,5h
	mov	[si+8h],ax
	mov	[si+0Ah],ax
	cmp	word ptr [bp+8h],2h
	jz	09CEh

l0800_098F:
	or	di,di
	jbe	09CEh

l0800_0993:
	mov	word ptr [0234h],9D6h
	cmp	word ptr [bp+6h],0h
	jnz	09B7h

l0800_099F:
	push	di
	call	0570h
	pop	cx
	mov	[bp+6h],ax
	or	ax,ax
	jz	09B2h

l0800_09AB:
	or	word ptr [si+2h],4h
	jmp	09B7h

l0800_09B2:
	mov	ax,0FFFFh
	jmp	09D2h

l0800_09B7:
	mov	ax,[bp+6h]
	mov	[si+0Ah],ax
	mov	[si+8h],ax
	mov	[si+6h],di
	cmp	word ptr [bp+8h],1h
	jnz	09CEh

l0800_09C9:
	or	word ptr [si+2h],8h

l0800_09CE:
	xor	ax,ax
	jmp	09D2h

l0800_09D2:
	pop	di
	pop	si
	pop	bp
	ret
0800:09D6                   56 57 BF 04 00 BE 42 03 EB 10       VW....B...
0800:09E0 F7 44 02 03 00 74 05 56 E8 E2 03 59 4F 83 C6 10 .D...t.V...YO...
0800:09F0 0B FF 75 EC 5F 5E C3                            ..u._^.         

;; _read: 0800:09F7
;;   Called from:
;;     0800:07A2 (in fn0800_0782)
_read proc
	push	bp
	mov	bp,sp
	sub	sp,4h
	push	si
	push	di
	mov	ax,[bp+8h]
	inc	ax
	cmp	ax,2h
	jc	0A15h

l0800_0A08:
	mov	bx,[bp+4h]
	shl	bx,1h
	test	word ptr [bx+482h],200h
	jz	0A1Ah

l0800_0A15:
	xor	ax,ax
	jmp	0AADh

l0800_0A1A:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0AB3h
	add	sp,6h
	mov	[bp-4h],ax
	mov	ax,[bp-4h]
	inc	ax
	cmp	ax,2h
	jc	0A42h

l0800_0A35:
	mov	bx,[bp+4h]
	shl	bx,1h
	test	word ptr [bx+482h],8000h
	jz	0A48h

l0800_0A42:
	mov	ax,[bp-4h]
	jmp	0AADh
0800:0A47                      90                                .        

l0800_0A48:
	mov	cx,[bp-4h]
	mov	si,[bp+6h]
	push	ds
	pop	es
	mov	di,si
	mov	bx,si
	cld

l0800_0A55:
	lodsb
	cmp	al,1Ah
	jz	0A87h

l0800_0A5A:
	cmp	al,0Dh
	jz	0A63h

l0800_0A5E:
	stosb
	loop	0A55h

l0800_0A61:
	jmp	0A7Fh

l0800_0A63:
	loop	0A55h

l0800_0A65:
	push	es
	push	bx
	mov	ax,1h
	push	ax
	lea	ax,[bp-1h]
	push	ax
	push	word ptr [bp+4h]
	call	0AB3h
	add	sp,6h
	pop	bx
	pop	es
	cld
	mov	al,[bp-1h]
	stosb

l0800_0A7F:
	cmp	di,bx
	jnz	0A85h

l0800_0A83:
	jmp	0A1Ah

l0800_0A85:
	jmp	0AA7h

l0800_0A87:
	push	bx
	mov	ax,2h
	push	ax
	neg	cx
	sbb	ax,ax
	push	ax
	push	cx
	push	word ptr [bp+4h]
	call	0C28h
	add	sp,8h
	mov	bx,[bp+4h]
	shl	bx,1h
	or	word ptr [bx+482h],200h
	pop	bx

l0800_0AA7:
	mov	ax,di
	sub	ax,bx
	jmp	0AADh

l0800_0AAD:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; __read: 0800:0AB3
;;   Called from:
;;     0800:0898 (in _fgetc)
;;     0800:0A23 (in _read)
;;     0800:0A72 (in _read)
__read proc
	push	bp
	mov	bp,sp
	mov	ah,3Fh
	mov	bx,[bp+4h]
	mov	cx,[bp+8h]
	mov	dx,[bp+6h]
	int	21h
	jc	0AC7h

l0800_0AC5:
	jmp	0ACDh

l0800_0AC7:
	push	ax
	call	02C5h
	jmp	0ACDh

l0800_0ACD:
	pop	bp
	ret

;; _write: 0800:0ACF
;;   Called from:
;;     0800:0E28 (in _fflush)
_write proc
	push	bp
	mov	bp,sp
	sub	sp,8Ah
	push	si
	push	di
	mov	ax,[bp+8h]
	inc	ax
	cmp	ax,2h
	jnc	0AE6h

l0800_0AE1:
	xor	ax,ax
	jmp	0BDCh

l0800_0AE6:
	mov	bx,[bp+4h]
	shl	bx,1h
	test	word ptr [bx+482h],8000h
	jz	0B05h

l0800_0AF3:
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	0BE2h
	add	sp,6h
	jmp	0BDCh

l0800_0B05:
	mov	bx,[bp+4h]
	shl	bx,1h
	and	word ptr [bx+482h],0FDFFh
	mov	ax,[bp+6h]
	mov	[bp+0FF7Ch],ax
	mov	ax,[bp+8h]
	mov	[bp+0FF78h],ax
	lea	si,[bp+0FF7Eh]
	jmp	0B91h

l0800_0B24:
	dec	word ptr [bp+0FF78h]
	mov	bx,[bp+0FF7Ch]
	inc	word ptr [bp+0FF7Ch]
	mov	al,[bx]
	mov	[bp+0FF7Bh],al
	cmp	al,0Ah
	jnz	0B3Eh

l0800_0B3A:
	mov	byte ptr [si],0Dh
	inc	si

l0800_0B3E:
	mov	al,[bp+0FF7Bh]
	mov	[si],al
	inc	si
	lea	ax,[bp+0FF7Eh]
	mov	dx,si
	sub	dx,ax
	cmp	dx,80h
	jl	0B91h

l0800_0B53:
	lea	ax,[bp+0FF7Eh]
	mov	di,si
	sub	di,ax
	push	di
	lea	ax,[bp+0FF7Eh]
	push	ax
	push	word ptr [bp+4h]
	call	0BE2h
	add	sp,6h
	mov	[bp+0FF76h],ax
	cmp	ax,di
	jz	0B8Dh

l0800_0B72:
	cmp	word ptr [bp+0FF76h],0h
	jnc	0B7Eh

l0800_0B79:
	mov	ax,0FFFFh
	jmp	0B8Bh

l0800_0B7E:
	mov	ax,[bp+8h]
	sub	ax,[bp+0FF78h]
	add	ax,[bp+0FF76h]
	sub	ax,di

l0800_0B8B:
	jmp	0BDCh

l0800_0B8D:
	lea	si,[bp+0FF7Eh]

l0800_0B91:
	cmp	word ptr [bp+0FF78h],0h
	jz	0B9Bh

l0800_0B98:
	jmp	0B24h

l0800_0B9B:
	lea	ax,[bp+0FF7Eh]
	mov	di,si
	sub	di,ax
	mov	ax,di
	or	ax,ax
	jbe	0BD7h

l0800_0BA9:
	push	di
	lea	ax,[bp+0FF7Eh]
	push	ax
	push	word ptr [bp+4h]
	call	0BE2h
	add	sp,6h
	mov	[bp+0FF76h],ax
	cmp	ax,di
	jz	0BD7h

l0800_0BC0:
	cmp	word ptr [bp+0FF76h],0h
	jnc	0BCCh

l0800_0BC7:
	mov	ax,0FFFFh
	jmp	0BD5h

l0800_0BCC:
	mov	ax,[bp+8h]
	add	ax,[bp+0FF76h]
	sub	ax,di

l0800_0BD5:
	jmp	0BDCh

l0800_0BD7:
	mov	ax,[bp+8h]
	jmp	0BDCh

l0800_0BDC:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; __write: 0800:0BE2
;;   Called from:
;;     0800:0AFC (in _write)
;;     0800:0B64 (in _write)
;;     0800:0BB2 (in _write)
;;     0800:0F2B (in _fputc)
;;     0800:0F43 (in _fputc)
__write proc
	push	bp
	mov	bp,sp
	mov	bx,[bp+4h]
	shl	bx,1h
	test	word ptr [bx+482h],800h
	jz	0C02h

l0800_0BF2:
	mov	ax,2h
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp+4h]
	call	0C28h
	mov	sp,bp

l0800_0C02:
	mov	ah,40h
	mov	bx,[bp+4h]
	mov	cx,[bp+8h]
	mov	dx,[bp+6h]
	int	21h
	jc	0C20h

l0800_0C11:
	push	ax
	mov	bx,[bp+4h]
	shl	bx,1h
	or	word ptr [bx+482h],1000h
	pop	ax
	jmp	0C26h

l0800_0C20:
	push	ax
	call	02C5h
	jmp	0C26h

l0800_0C26:
	pop	bp
	ret

;; _lseek: 0800:0C28
;;   Called from:
;;     0800:06EA (in _fseek)
;;     0800:0A95 (in _read)
;;     0800:0BFD (in __write)
;;     0800:1D5C (in _tell)
_lseek proc
	push	bp
	mov	bp,sp
	mov	bx,[bp+4h]
	shl	bx,1h
	and	word ptr [bx+482h],0FDFFh
	mov	ah,42h
	mov	al,[bp+0Ah]
	mov	bx,[bp+4h]
	mov	cx,[bp+8h]
	mov	dx,[bp+6h]
	int	21h
	jc	0C4Ah

l0800_0C48:
	jmp	0C51h

l0800_0C4A:
	push	ax
	call	02C5h
	cwd
	jmp	0C51h

l0800_0C51:
	pop	bp
	ret

;; __LONGTOA: 0800:0C53
;;   Called from:
;;     0800:1303 (in __VPRINTER)
__LONGTOA proc
	push	bp
	mov	bp,sp
	sub	sp,22h
	push	si
	push	di
	push	es
	mov	di,[bp+0Ah]
	push	ds
	pop	es
	mov	bx,[bp+8h]
	cmp	bx,24h
	ja	0CC1h

l0800_0C69:
	cmp	bl,2h
	jc	0CC1h

l0800_0C6E:
	mov	ax,[bp+0Ch]
	mov	cx,[bp+0Eh]
	or	cx,cx
	jge	0C89h

l0800_0C78:
	cmp	byte ptr [bp+6h],0h
	jz	0C89h

l0800_0C7E:
	mov	byte ptr [di],2Dh
	inc	di
	neg	cx
	neg	ax
	sbb	cx,0h

l0800_0C89:
	lea	si,[bp-22h]
	jcxz	0C9Dh

l0800_0C8E:
	xchg	cx,ax
	sub	dx,dx
	div	bx
	xchg	cx,ax
	div	bx
	mov	[si],dl
	inc	si
	jcxz	0CA4h

l0800_0C9B:
	jmp	0C8Eh

l0800_0C9D:
	sub	dx,dx
	div	bx
	mov	[si],dl
	inc	si

l0800_0CA4:
	or	ax,ax
	jnz	0C9Dh

l0800_0CA8:
	lea	cx,[bp-22h]
	neg	cx
	add	cx,si
	cld

l0800_0CB0:
	dec	si
	mov	al,[si]
	sub	al,0Ah
	jnc	0CBBh

l0800_0CB7:
	add	al,3Ah
	jmp	0CBEh

l0800_0CBB:
	add	al,[bp+4h]

l0800_0CBE:
	stosb
	loop	0CB0h

l0800_0CC1:
	mov	al,0h
	stosb
	pop	es
	mov	ax,[bp+0Ah]
	jmp	0CCAh

l0800_0CCA:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	0Ch
0800:0CD2       55 8B EC 83 7E 08 0A 75 06 8B 46 04 99 EB   U...~..u..F...
0800:0CE0 05 8B 46 04 33 D2 52 50 FF 76 06 FF 76 08 B0 01 ..F.3.RP.v..v...
0800:0CF0 50 B0 61 50 E8 5C FF EB 00 5D C3 55 8B EC FF 76 P.aP.\...].U...v
0800:0D00 06 FF 76 04 FF 76 08 FF 76 0A B0 00 50 B0 61 50 ..v..v..v...P.aP
0800:0D10 E8 40 FF EB 00 5D C3 55 8B EC FF 76 06 FF 76 04 .@...].U...v..v.
0800:0D20 FF 76 08 FF 76 0A 83 7E 0A 0A 75 05 B8 01 00 EB .v..v..~..u.....
0800:0D30 02 33 C0 50 B0 61 50 E8 19 FF EB 00 5D C3 BA AE .3.P.aP.....]...
0800:0D40 04 EB 03 BA B3 04 B9 05 00 90 B4 40 BB 02 00 CD ...........@....
0800:0D50 21 B9 27 00 90 BA B8 04 B4 40 CD 21 E9 83 F4    !.'......@.!... 

;; _eof: 0800:0D5F
;;   Called from:
;;     0800:08A8 (in _fgetc)
_eof proc
	push	bp
	mov	bp,sp
	sub	sp,4h
	mov	bx,[bp+4h]
	shl	bx,1h
	test	word ptr [bx+482h],200h
	jz	0D78h

l0800_0D72:
	mov	ax,1h
	jmp	0DC9h
0800:0D77                      90                                .        

l0800_0D78:
	mov	ax,4400h
	mov	bx,[bp+4h]
	int	21h
	jc	0DC3h

l0800_0D82:
	test	dl,80h
	jnz	0DBFh

l0800_0D87:
	mov	ax,4201h
	xor	cx,cx
	xor	dx,dx
	int	21h
	jc	0DC3h

l0800_0D92:
	push	dx
	push	ax
	mov	ax,4202h
	xor	cx,cx
	xor	dx,dx
	int	21h
	mov	[bp-4h],ax
	mov	[bp-2h],dx
	pop	dx
	pop	cx
	jc	0DC3h

l0800_0DA7:
	mov	ax,4200h
	int	21h
	jc	0DC3h

l0800_0DAE:
	cmp	dx,[bp-2h]
	jc	0DBFh

l0800_0DB3:
	ja	0DBAh

l0800_0DB5:
	cmp	ax,[bp-4h]
	jc	0DBFh

l0800_0DBA:
	mov	ax,1h
	jmp	0DC9h

l0800_0DBF:
	xor	ax,ax
	jmp	0DC9h

l0800_0DC3:
	push	ax
	call	02C5h
	jmp	0DC9h

l0800_0DC9:
	mov	sp,bp
	pop	bp
	ret

;; _fflush: 0800:0DCD
;;   Called from:
;;     0800:06AA (in _fseek)
;;     0800:0771 (in fn0800_075B)
;;     0800:0EB1 (in _fputc)
;;     0800:0EF3 (in _fputc)
_fflush proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	si,[bp+4h]
	mov	ax,[si+0Eh]
	cmp	ax,si
	jz	0DE1h

l0800_0DDC:
	mov	ax,0FFFFh
	jmp	0E47h

l0800_0DE1:
	cmp	word ptr [si],0h
	jl	0E13h

l0800_0DE6:
	test	word ptr [si+2h],8h
	jnz	0DF9h

l0800_0DED:
	mov	ax,[si+0Ah]
	mov	dx,si
	add	dx,5h
	cmp	ax,dx
	jnz	0E0Fh

l0800_0DF9:
	mov	word ptr [si],0h
	mov	ax,[si+0Ah]
	mov	dx,si
	add	dx,5h
	cmp	ax,dx
	jnz	0E0Fh

l0800_0E09:
	mov	ax,[si+8h]
	mov	[si+0Ah],ax

l0800_0E0F:
	xor	ax,ax
	jmp	0E47h

l0800_0E13:
	mov	di,[si+6h]
	add	di,[si]
	inc	di
	sub	[si],di
	push	di
	mov	ax,[si+8h]
	mov	[si+0Ah],ax
	push	ax
	mov	al,[si+4h]
	cbw
	push	ax
	call	0ACFh
	add	sp,6h
	cmp	ax,di
	jz	0E43h

l0800_0E32:
	test	word ptr [si+2h],200h
	jnz	0E43h

l0800_0E39:
	or	word ptr [si+2h],10h
	mov	ax,0FFFFh
	jmp	0E47h

l0800_0E43:
	xor	ax,ax
	jmp	0E47h

l0800_0E47:
	pop	di
	pop	si
	pop	bp
	ret

;; _printf: 0800:0E4B
;;   Called from:
;;     0800:026F (in _main)
;;     0800:028A (in _main)
;;     0800:02BD (in _main)
_printf proc
	push	bp
	mov	bp,sp
	mov	ax,0F81h
	push	ax
	mov	ax,352h
	push	ax
	push	word ptr [bp+4h]
	lea	ax,[bp+6h]
	push	ax
	call	1073h
	jmp	0E62h

l0800_0E62:
	pop	bp
	ret

;; __fputc: 0800:0E64
__fputc proc
	push	bp
	mov	bp,sp
	mov	bx,[bp+6h]
	dec	word ptr [bx]
	push	word ptr [bp+6h]
	mov	al,[bp+4h]
	cbw
	push	ax
	call	0E7Dh
	mov	sp,bp
	jmp	0E7Bh

l0800_0E7B:
	pop	bp
	ret

;; _fputc: 0800:0E7D
;;   Called from:
;;     0800:0E74 (in __fputc)
_fputc proc
	push	bp
	mov	bp,sp
	sub	sp,2h
	push	si
	mov	si,[bp+6h]
	mov	al,[bp+4h]
	mov	[bp-1h],al

l0800_0E8D:
	inc	word ptr [si]
	jge	0EC7h

l0800_0E91:
	mov	al,[bp-1h]
	inc	word ptr [si+0Ah]
	mov	bx,[si+0Ah]
	mov	[bx-1h],al
	test	word ptr [si+2h],8h
	jz	0EBFh

l0800_0EA4:
	cmp	byte ptr [bp-1h],0Ah
	jz	0EB0h

l0800_0EAA:
	cmp	byte ptr [bp-1h],0Dh
	jnz	0EBFh

l0800_0EB0:
	push	si
	call	0DCDh
	pop	cx
	or	ax,ax
	jz	0EBFh

l0800_0EB9:
	mov	ax,0FFFFh
	jmp	0F66h

l0800_0EBF:
	mov	al,[bp-1h]
	mov	ah,0h
	jmp	0F66h

l0800_0EC7:
	dec	word ptr [si]
	test	word ptr [si+2h],90h
	jnz	0ED7h

l0800_0ED0:
	test	word ptr [si+2h],2h
	jnz	0EE2h

l0800_0ED7:
	or	word ptr [si+2h],10h
	mov	ax,0FFFFh
	jmp	0F66h

l0800_0EE2:
	or	word ptr [si+2h],100h
	cmp	word ptr [si+6h],0h
	jz	0F11h

l0800_0EED:
	cmp	word ptr [si],0h
	jz	0F02h

l0800_0EF2:
	push	si
	call	0DCDh
	pop	cx
	or	ax,ax
	jz	0F00h

l0800_0EFB:
	mov	ax,0FFFFh
	jmp	0F66h

l0800_0F00:
	jmp	0F0Ch

l0800_0F02:
	mov	ax,[si+6h]
	mov	dx,0FFFFh
	sub	dx,ax
	mov	[si],dx

l0800_0F0C:
	jmp	0E8Dh
0800:0F0F                                              EB                .
0800:0F10 55                                              U               

l0800_0F11:
	cmp	byte ptr [bp-1h],0Ah
	jnz	0F36h

l0800_0F17:
	test	word ptr [si+2h],40h
	jnz	0F36h

l0800_0F1E:
	mov	ax,1h
	push	ax
	mov	ax,4E0h
	push	ax
	mov	al,[si+4h]
	cbw
	push	ax
	call	0BE2h
	add	sp,6h
	cmp	ax,1h
	jnz	0F4Eh

l0800_0F36:
	mov	ax,1h
	push	ax
	lea	ax,[bp+4h]
	push	ax
	mov	al,[si+4h]
	cbw
	push	ax
	call	0BE2h
	add	sp,6h
	cmp	ax,1h
	jz	0F5Fh

l0800_0F4E:
	test	word ptr [si+2h],200h
	jnz	0F5Fh

l0800_0F55:
	or	word ptr [si+2h],10h
	mov	ax,0FFFFh
	jmp	0F66h

l0800_0F5F:
	mov	al,[bp-1h]
	mov	ah,0h
	jmp	0F66h

l0800_0F66:
	pop	si
	mov	sp,bp
	pop	bp
	ret
0800:0F6B                                  55 8B EC 56 8B            U..V.
0800:0F70 76 04 B8 52 03 50 56 E8 03 FF 59 59 EB 00 5E 5D v..R.PV...YY..^]
0800:0F80 C3 55 8B EC 83 EC 02 56 57 8B 76 04 8B 7E 06 89 .U.....VW.v..~..
0800:0F90 7E FE F7 44 02 08 00 74 26 EB 1A 56 8B 5E 08 FF ~..D...t&..V.^..
0800:0FA0 46 08 8A 07 98 50 E8 D4 FE 59 59 3D FF FF 75 05 F....P...YY=..u.
0800:0FB0 33 C0 E9 87 00 8B C7 4F 0B C0 75 DF E9 78 00 F7 3......O..u..x..
0800:0FC0 44 02 40 00 74 38 83 7C 06 00 74 32 8B 44 06 3B D.@.t8.|..t2.D.;
0800:0FD0 C7 73 2B 83 3C 00 74 0D 56 E8 F1 FD 59 0B C0 74 .s+.<.t.V...Y..t
0800:0FE0 04 33 C0 EB 57 57 FF 76 08 8A 44 04 98 50 E8 F1 .3..WW.v..D..P..
0800:0FF0 FB 83 C4 06 3B C7 73 04 33 C0 EB 40 EB 39 EB 30 ....;.s.3..@.9.0
0800:1000 FF 04 7D 15 8B 5E 08 FF 46 08 8A 07 FF 44 0A 8B ..}..^..F....D..
0800:1010 5C 0A 88 47 FF B4 00 EB 0E 56 8B 5E 08 FF 46 08 \..G.....V.^..F.
0800:1020 FF 37 E8 3F FE 59 59 3D FF FF 75 04 33 C0 EB 0C .7.?.YY=..u.3...
0800:1030 8B C7 4F 0B C0 75 C9 8B 46 FE EB 00 5F 5E 8B E5 ..O..u..F..._^..
0800:1040 5D C2 06 00                                     ]...            

;; __REALCVT: 0800:1044
;;   Called from:
;;     0800:1418 (in __VPRINTER)
__REALCVT proc
	jmp	word ptr [05E0h]

;; fn0800_1048: 0800:1048
;;   Called from:
;;     0800:133C (in __VPRINTER)
;;     0800:1344 (in __VPRINTER)
fn0800_1048 proc
	push	bp
	mov	bp,sp
	mov	dx,[bp+4h]
	mov	cx,0F04h
	mov	bx,4E9h
	cld
	mov	al,dh
	shr	al,cl
	xlat
	stosb
	mov	al,dh
	and	al,ch
	xlat
	stosb
	mov	al,dl
	shr	al,cl
	xlat
	stosb
	mov	al,dl
	and	al,ch
	xlat
	stosb
	jmp	106Fh

l0800_106F:
	pop	bp
	ret	2h

;; __VPRINTER: 0800:1073
;;   Called from:
;;     0800:0E5D (in _printf)
__VPRINTER proc
	push	bp
	mov	bp,sp
	sub	sp,98h
	push	si
	push	di
	mov	word ptr [bp-58h],0h
	mov	byte ptr [bp-55h],50h
	mov	word ptr [bp-2h],0h
	jmp	10CDh

;; fn0800_108C: 0800:108C
;;   Called from:
;;     0800:13B8 (in __VPRINTER)
;;     0800:1437 (in __VPRINTER)
;;     0800:1465 (in __VPRINTER)
fn0800_108C proc
	push	di
	mov	cx,0FFFFh
	xor	al,al

l0800_1092:
	repne scasb

l0800_1094:
	not	cx
	dec	cx
	pop	di
	ret

;; fn0800_1099: 0800:1099
;;   Called from:
;;     0800:12E9 (in __VPRINTER)
;;     0800:14C5 (in __VPRINTER)
;;     0800:14D7 (in __VPRINTER)
;;     0800:14DE (in __VPRINTER)
;;     0800:14FE (in __VPRINTER)
;;     0800:1509 (in __VPRINTER)
;;     0800:152B (in __VPRINTER)
;;     0800:156A (in __VPRINTER)
fn0800_1099 proc
	mov	[di],al
	inc	di
	dec	byte ptr [bp-55h]
	jle	10CCh

;; fn0800_10A1: 0800:10A1
;;   Called from:
;;     0800:109F (in fn0800_1099)
;;     0800:10EE (in __VPRINTER)
;;     0800:151E (in __VPRINTER)
;;     0800:1578 (in __VPRINTER)
fn0800_10A1 proc
	push	bx
	push	cx
	push	dx
	push	es
	lea	ax,[bp-54h]
	sub	di,ax
	lea	ax,[bp-54h]
	push	ax
	push	di
	push	word ptr [bp+8h]
	call	word ptr [bp+0Ah]
	or	ax,ax
	jnz	10BEh

l0800_10B9:
	mov	word ptr [bp-2h],1h

l0800_10BE:
	mov	byte ptr [bp-55h],50h
	add	[bp-58h],di
	lea	di,[bp-54h]
	pop	es
	pop	dx
	pop	cx
	pop	bx

l0800_10CC:
	ret

l0800_10CD:
	push	es
	cld
	lea	di,[bp-54h]
	mov	[bp+0FF6Ah],di

l0800_10D6:
	mov	di,[bp+0FF6Ah]

l0800_10DA:
	mov	si,[bp+6h]

l0800_10DD:
	lodsb
	or	al,al
	jz	10F3h

l0800_10E2:
	cmp	al,25h
	jz	10F6h

l0800_10E6:
	mov	[di],al
	inc	di
	dec	byte ptr [bp-55h]
	jg	10DDh

l0800_10EE:
	call	10A1h
	jmp	10DDh

l0800_10F3:
	jmp	1572h

l0800_10F6:
	mov	[bp+0FF76h],si
	lodsb
	cmp	al,25h
	jz	10E6h

l0800_10FF:
	mov	[bp+0FF6Ah],di
	xor	cx,cx
	mov	[bp+0FF74h],cx
	mov	[bp+0FF68h],cx
	mov	[bp+0FF73h],cl
	mov	word ptr [bp+0FF6Eh],0FFFFh
	mov	word ptr [bp+0FF70h],0FFFFh
	jmp	1120h

l0800_111F:
	lodsb

l0800_1120:
	xor	ah,ah
	mov	dx,ax
	mov	bx,ax
	sub	bl,20h
	cmp	bl,60h
	jnc	1175h

l0800_112E:
	mov	bl,[bx+4F9h]
	mov	ax,bx
	cmp	ax,17h
	jbe	113Ch

l0800_1139:
	jmp	1560h

l0800_113C:
	mov	bx,ax
	shl	bx,1h
	jmp	word ptr cs:[bx+1145h]
l0800_1145	dw	0x1190
l0800_1147	dw	0x1178
l0800_1149	dw	0x11D1
l0800_114B	dw	0x1184
l0800_114D	dw	0x11F6
l0800_114F	dw	0x1200
l0800_1151	dw	0x1242
l0800_1153	dw	0x124C
l0800_1155	dw	0x125C
l0800_1157	dw	0x11B7
l0800_1159	dw	0x1291
l0800_115B	dw	0x126C
l0800_115D	dw	0x1270
l0800_115F	dw	0x1274
l0800_1161	dw	0x1316
l0800_1163	dw	0x13C8
l0800_1165	dw	0x1369
l0800_1167	dw	0x1389
l0800_1169	dw	0x1533
l0800_116B	dw	0x1560
l0800_116D	dw	0x1560
l0800_116F	dw	0x1560
l0800_1171	dw	0x11A3
l0800_1173	dw	0x11AD

l0800_1175:
	jmp	1560h

l0800_1178:
	cmp	ch,0h
	ja	1175h

l0800_117D:
	or	word ptr [bp+0FF68h],1h
	jmp	111Fh

l0800_1184:
	cmp	ch,0h
	ja	1175h

l0800_1189:
	or	word ptr [bp+0FF68h],2h
	jmp	111Fh

l0800_1190:
	cmp	ch,0h
	ja	1175h

l0800_1195:
	cmp	byte ptr [bp+0FF73h],2Bh
	jz	11A0h

l0800_119C:
	mov	[bp+0FF73h],dl

l0800_11A0:
	jmp	111Fh

l0800_11A3:
	and	word ptr [bp+0FF68h],0DFh
	mov	ch,5h
	jmp	111Fh

l0800_11AD:
	or	word ptr [bp+0FF68h],20h
	mov	ch,5h
	jmp	111Fh

l0800_11B7:
	cmp	ch,0h
	ja	1200h

l0800_11BC:
	test	word ptr [bp+0FF68h],2h
	jnz	11E5h

l0800_11C4:
	or	word ptr [bp+0FF68h],8h
	mov	ch,1h
	jmp	111Fh

l0800_11CE:
	jmp	1560h

l0800_11D1:
	mov	di,[bp+4h]
	mov	ax,[di]
	add	word ptr [bp+4h],2h
	cmp	ch,2h
	jnc	11E8h

l0800_11DF:
	mov	[bp+0FF6Eh],ax
	mov	ch,3h

l0800_11E5:
	jmp	111Fh

l0800_11E8:
	cmp	ch,4h
	jnz	11CEh

l0800_11ED:
	mov	[bp+0FF70h],ax
	inc	ch
	jmp	111Fh

l0800_11F6:
	cmp	ch,4h
	jnc	11CEh

l0800_11FB:
	mov	ch,4h
	jmp	111Fh

l0800_1200:
	xchg	dx,ax
	sub	al,30h
	cbw
	cmp	ch,2h
	ja	1224h

l0800_1209:
	mov	ch,2h
	xchg	[bp+0FF6Eh],ax
	or	ax,ax
	jl	11E5h

l0800_1213:
	shl	ax,1h
	mov	dx,ax
	shl	ax,1h
	shl	ax,1h
	add	ax,dx
	add	[bp+0FF6Eh],ax
	jmp	111Fh

l0800_1224:
	cmp	ch,4h
	jnz	11CEh

l0800_1229:
	xchg	[bp+0FF70h],ax
	or	ax,ax
	jl	11E5h

l0800_1231:
	shl	ax,1h
	mov	dx,ax
	shl	ax,1h
	shl	ax,1h
	add	ax,dx
	add	[bp+0FF70h],ax
	jmp	111Fh

l0800_1242:
	or	word ptr [bp+0FF68h],10h
	mov	ch,5h
	jmp	111Fh

l0800_124C:
	or	word ptr [bp+0FF68h],100h
	and	word ptr [bp+0FF68h],0EFh
	mov	ch,5h
	jmp	111Fh

l0800_125C:
	and	word ptr [bp+0FF68h],0EFh
	or	word ptr [bp+0FF68h],80h
	mov	ch,5h
	jmp	111Fh

l0800_126C:
	mov	bh,8h
	jmp	127Ah

l0800_1270:
	mov	bh,0Ah
	jmp	127Fh

l0800_1274:
	mov	bh,10h
	mov	bl,0E9h
	add	bl,dl

l0800_127A:
	mov	byte ptr [bp+0FF73h],0h

l0800_127F:
	mov	byte ptr [bp+0FF6Dh],0h
	mov	[bp+0FF6Ch],dl
	mov	di,[bp+4h]
	mov	ax,[di]
	xor	dx,dx
	jmp	12A2h

l0800_1291:
	mov	bh,0Ah
	mov	byte ptr [bp+0FF6Dh],1h
	mov	[bp+0FF6Ch],dl
	mov	di,[bp+4h]
	mov	ax,[di]
	cwd

l0800_12A2:
	inc	di
	inc	di
	mov	[bp+6h],si
	test	word ptr [bp+0FF68h],10h
	jz	12B3h

l0800_12AF:
	mov	dx,[di]
	inc	di
	inc	di

l0800_12B3:
	mov	[bp+4h],di
	lea	di,[bp+0FF79h]
	or	ax,ax
	jnz	12F1h

l0800_12BE:
	or	dx,dx
	jnz	12F1h

l0800_12C2:
	cmp	word ptr [bp+0FF70h],0h
	jnz	12F6h

l0800_12C9:
	mov	di,[bp+0FF6Ah]
	mov	cx,[bp+0FF6Eh]
	jcxz	12EEh

l0800_12D3:
	cmp	cx,0FFh
	jz	12EEh

l0800_12D8:
	mov	ax,[bp+0FF68h]
	and	ax,8h
	jz	12E5h

l0800_12E1:
	mov	dl,30h
	jmp	12E7h

l0800_12E5:
	mov	dl,20h

l0800_12E7:
	mov	al,dl
	call	1099h
	loop	12E7h

l0800_12EE:
	jmp	10DAh

l0800_12F1:
	or	word ptr [bp+0FF68h],4h

l0800_12F6:
	push	dx
	push	ax
	push	di
	mov	al,bh
	cbw
	push	ax
	mov	al,[bp+0FF6Dh]
	push	ax
	push	bx
	call	0C53h
	push	ss
	pop	es
	mov	dx,[bp+0FF70h]
	or	dx,dx
	jg	1313h

l0800_1310:
	jmp	1427h

l0800_1313:
	jmp	1437h

l0800_1316:
	mov	[bp+0FF6Ch],dl
	mov	[bp+6h],si
	lea	di,[bp+0FF78h]
	mov	bx,[bp+4h]
	push	word ptr [bx]
	inc	bx
	inc	bx
	mov	[bp+4h],bx
	test	word ptr [bp+0FF68h],20h
	jz	1342h

l0800_1333:
	push	word ptr [bx]
	inc	bx
	inc	bx
	mov	[bp+4h],bx
	push	ss
	pop	es
	call	1048h
	mov	al,3Ah
	stosb

l0800_1342:
	push	ss
	pop	es
	call	1048h
	mov	byte ptr [di],0h
	mov	byte ptr [bp+0FF6Dh],0h
	and	word ptr [bp+0FF68h],0FBh
	lea	cx,[bp+0FF78h]
	sub	di,cx
	xchg	di,cx
	mov	dx,[bp+0FF70h]
	cmp	dx,cx
	jg	1366h

l0800_1364:
	mov	dx,cx

l0800_1366:
	jmp	1427h

l0800_1369:
	mov	[bp+6h],si
	mov	[bp+0FF6Ch],dl
	mov	di,[bp+4h]
	mov	ax,[di]
	add	word ptr [bp+4h],2h
	push	ss
	pop	es
	lea	di,[bp+0FF79h]
	xor	ah,ah
	mov	[di],ax
	mov	cx,1h
	jmp	1468h

l0800_1389:
	mov	[bp+6h],si
	mov	[bp+0FF6Ch],dl
	mov	di,[bp+4h]
	test	word ptr [bp+0FF68h],20h
	jnz	13A7h

l0800_139B:
	mov	di,[di]
	add	word ptr [bp+4h],2h
	push	ds
	pop	es
	or	di,di
	jmp	13B1h

l0800_13A7:
	les	di,[di]
	add	word ptr [bp+4h],4h
	mov	ax,es
	or	ax,di

l0800_13B1:
	jnz	13B8h

l0800_13B3:
	push	ds
	pop	es
	mov	di,4E2h

l0800_13B8:
	call	108Ch
	cmp	cx,[bp+0FF70h]
	jbe	13C5h

l0800_13C1:
	mov	cx,[bp+0FF70h]

l0800_13C5:
	jmp	1468h

l0800_13C8:
	mov	[bp+6h],si
	mov	[bp+0FF6Ch],dl
	mov	di,[bp+4h]
	mov	cx,[bp+0FF70h]
	or	cx,cx
	jge	13DDh

l0800_13DA:
	mov	cx,6h

l0800_13DD:
	push	di
	push	cx
	lea	bx,[bp+0FF79h]
	push	bx
	push	dx
	mov	ax,1h
	and	ax,[bp+0FF68h]
	push	ax
	mov	ax,[bp+0FF68h]
	test	ax,80h
	jz	1400h

l0800_13F6:
	mov	ax,2h
	mov	word ptr [bp-4h],4h
	jmp	1417h

l0800_1400:
	test	ax,100h
	jz	140Fh

l0800_1405:
	mov	ax,8h
	mov	word ptr [bp-4h],0Ah
	jmp	1417h

l0800_140F:
	mov	word ptr [bp-4h],8h
	mov	ax,6h

l0800_1417:
	push	ax
	call	1044h
	mov	ax,[bp-4h]
	add	[bp+4h],ax
	push	ss
	pop	es
	lea	di,[bp+0FF79h]

l0800_1427:
	test	word ptr [bp+0FF68h],8h
	jz	1449h

l0800_142F:
	mov	dx,[bp+0FF6Eh]
	or	dx,dx
	jle	1449h

l0800_1437:
	call	108Ch
	cmp	byte ptr es:[di],2Dh
	jnz	1441h

l0800_1440:
	dec	cx

l0800_1441:
	sub	dx,cx
	jle	1449h

l0800_1445:
	mov	[bp+0FF74h],dx

l0800_1449:
	mov	al,[bp+0FF73h]
	or	al,al
	jz	1465h

l0800_1451:
	cmp	byte ptr es:[di],2Dh
	jz	1465h

l0800_1457:
	sub	word ptr [bp+0FF74h],1h
	adc	word ptr [bp+0FF74h],0h
	dec	di
	mov	es:[di],al

l0800_1465:
	call	108Ch

l0800_1468:
	mov	si,di
	mov	di,[bp+0FF6Ah]
	mov	bx,[bp+0FF6Eh]
	mov	ax,5h
	and	ax,[bp+0FF68h]
	cmp	ax,5h
	jnz	1494h

l0800_147E:
	mov	ah,[bp+0FF6Ch]
	cmp	ah,6Fh
	jnz	1497h

l0800_1487:
	cmp	word ptr [bp+0FF74h],0h
	jg	1494h

l0800_148E:
	mov	word ptr [bp+0FF74h],1h

l0800_1494:
	jmp	14B5h
0800:1496                   90                                  .         

l0800_1497:
	cmp	ah,78h
	jz	14A1h

l0800_149C:
	cmp	ah,58h
	jnz	14B5h

l0800_14A1:
	or	word ptr [bp+0FF68h],40h
	dec	bx
	dec	bx
	sub	word ptr [bp+0FF74h],2h
	jge	14B5h

l0800_14AF:
	mov	word ptr [bp+0FF74h],0h

l0800_14B5:
	add	cx,[bp+0FF74h]
	test	word ptr [bp+0FF68h],2h
	jnz	14CDh

l0800_14C1:
	jmp	14C9h

l0800_14C3:
	mov	al,20h
	call	1099h
	dec	bx

l0800_14C9:
	cmp	bx,cx
	jg	14C3h

l0800_14CD:
	test	word ptr [bp+0FF68h],40h
	jz	14E1h

l0800_14D5:
	mov	al,30h
	call	1099h
	mov	al,[bp+0FF6Ch]
	call	1099h

l0800_14E1:
	mov	dx,[bp+0FF74h]
	or	dx,dx
	jle	1510h

l0800_14E9:
	sub	cx,dx
	sub	bx,dx
	mov	al,es:[si]
	cmp	al,2Dh
	jz	14FCh

l0800_14F4:
	cmp	al,20h
	jz	14FCh

l0800_14F8:
	cmp	al,2Bh
	jnz	1503h

l0800_14FC:
	lodsb
	call	1099h
	dec	cx
	dec	bx

l0800_1503:
	xchg	dx,cx
	jcxz	150Eh

l0800_1507:
	mov	al,30h
	call	1099h
	loop	1507h

l0800_150E:
	xchg	dx,cx

l0800_1510:
	jcxz	1523h

l0800_1512:
	sub	bx,cx

l0800_1514:
	lodsb
	mov	[di],al
	inc	di
	dec	byte ptr [bp-55h]
	jg	1521h

l0800_151E:
	call	10A1h

l0800_1521:
	loop	1514h

l0800_1523:
	or	bx,bx
	jle	1530h

l0800_1527:
	mov	cx,bx

l0800_1529:
	mov	al,20h
	call	1099h
	loop	1529h

l0800_1530:
	jmp	10DAh

l0800_1533:
	mov	[bp+6h],si
	mov	di,[bp+4h]
	test	word ptr [bp+0FF68h],20h
	jnz	154Bh

l0800_1541:
	mov	di,[di]
	add	word ptr [bp+4h],2h
	push	ds
	pop	es
	jmp	1551h

l0800_154B:
	les	di,[di]
	add	word ptr [bp+4h],4h

l0800_1551:
	mov	ax,50h
	sub	al,[bp-55h]
	add	ax,[bp-58h]
	mov	es:[di],ax
	jmp	10D6h

l0800_1560:
	mov	si,[bp+0FF76h]
	mov	di,[bp+0FF6Ah]
	mov	al,25h

l0800_156A:
	call	1099h
	lodsb
	or	al,al
	jnz	156Ah

l0800_1572:
	cmp	byte ptr [bp-55h],50h
	jge	157Bh

l0800_1578:
	call	10A1h

l0800_157B:
	pop	es
	cmp	word ptr [bp-2h],0h
	jz	1589h

l0800_1582:
	mov	ax,0FFFFh
	jmp	158Eh
0800:1587                      EB 05                             ..       

l0800_1589:
	mov	ax,[bp-58h]
	jmp	158Eh

l0800_158E:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret	8h

;; fn0800_1596: 0800:1596
;;   Called from:
;;     0800:1691 (in fn0800_165F)
fn0800_1596 proc
	push	bp
	mov	bp,sp
	push	si
	push	di
	mov	si,[bp+4h]
	cmp	word ptr [062Ah],0h
	jz	15C1h

l0800_15A5:
	mov	bx,[062Ah]
	mov	di,[bx+6h]
	mov	bx,[062Ah]
	mov	[bx+6h],si
	mov	[di+4h],si
	mov	[si+6h],di
	mov	ax,[062Ah]
	mov	[si+4h],ax
	jmp	15CBh

l0800_15C1:
	mov	[062Ah],si
	mov	[si+4h],si
	mov	[si+6h],si

l0800_15CB:
	pop	di
	pop	si
	pop	bp
	ret

;; fn0800_15CF: 0800:15CF
;;   Called from:
;;     0800:16A2 (in fn0800_165F)
fn0800_15CF proc
	push	bp
	mov	bp,sp
	sub	sp,2h
	push	si
	push	di
	mov	si,[bp+6h]
	mov	di,[bp+4h]
	mov	ax,[si]
	add	[di],ax
	mov	ax,[0628h]
	cmp	ax,si
	jnz	15EEh

l0800_15E8:
	mov	[0628h],di
	jmp	15FBh

l0800_15EE:
	mov	ax,[si]
	add	ax,si
	mov	[bp-2h],ax
	mov	bx,[bp-2h]
	mov	[bx+2h],di

l0800_15FB:
	push	si
	call	0491h
	pop	cx
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; fn0800_1606: 0800:1606
;;   Called from:
;;     0800:16C7 (in _free)
fn0800_1606 proc
	push	si
	mov	ax,[062Ch]
	cmp	ax,[0628h]
	jnz	1622h

l0800_1610:
	push	word ptr [062Ch]
	call	063Bh
	pop	cx
	xor	ax,ax
	mov	[0628h],ax
	mov	[062Ch],ax
	jmp	165Dh

l0800_1622:
	mov	bx,[0628h]
	mov	si,[bx+2h]
	test	word ptr [si],1h
	jnz	1651h

l0800_162F:
	push	si
	call	0491h
	pop	cx
	cmp	si,[062Ch]
	jnz	1644h

l0800_163A:
	xor	ax,ax
	mov	[0628h],ax
	mov	[062Ch],ax
	jmp	164Ah

l0800_1644:
	mov	ax,[si+2h]
	mov	[0628h],ax

l0800_164A:
	push	si
	call	063Bh
	pop	cx
	jmp	165Dh

l0800_1651:
	push	word ptr [0628h]
	call	063Bh
	pop	cx
	mov	[0628h],si

l0800_165D:
	pop	si
	ret

;; fn0800_165F: 0800:165F
;;   Called from:
;;     0800:16CD (in _free)
fn0800_165F proc
	push	bp
	mov	bp,sp
	sub	sp,2h
	push	si
	push	di
	mov	si,[bp+4h]
	dec	word ptr [si]
	mov	ax,[si]
	add	ax,si
	mov	[bp-2h],ax
	mov	di,[si+2h]
	test	word ptr [di],1h
	jnz	1690h

l0800_167C:
	cmp	si,[062Ch]
	jz	1690h

l0800_1682:
	mov	ax,[si]
	add	[di],ax
	mov	bx,[bp-2h]
	mov	[bx+2h],di
	mov	si,di
	jmp	1695h

l0800_1690:
	push	si
	call	1596h
	pop	cx

l0800_1695:
	mov	bx,[bp-2h]
	test	word ptr [bx],1h
	jnz	16A7h

l0800_169E:
	push	word ptr [bp-2h]
	push	si
	call	15CFh
	pop	cx
	pop	cx

l0800_16A7:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; _free: 0800:16AD
;;   Called from:
;;     0800:0970 (in _setvbuf)
_free proc
	push	bp
	mov	bp,sp
	push	si
	mov	si,[bp+4h]
	or	si,si
	jnz	16BAh

l0800_16B8:
	jmp	16D1h

l0800_16BA:
	mov	ax,si
	add	ax,0FFFCh
	mov	si,ax
	cmp	si,[0628h]
	jnz	16CCh

l0800_16C7:
	call	1606h
	jmp	16D1h

l0800_16CC:
	push	si
	call	165Fh
	pop	cx

l0800_16D1:
	pop	si
	pop	bp
	ret

;; _scanf: 0800:16D4
;;   Called from:
;;     0800:027B (in _main)
_scanf proc
	push	bp
	mov	bp,sp
	lea	ax,[bp+6h]
	push	ax
	push	word ptr [bp+4h]
	mov	ax,342h
	push	ax
	mov	ax,1D65h
	push	ax
	mov	ax,7F2h
	push	ax
	call	16F3h
	mov	sp,bp
	jmp	16F1h

l0800_16F1:
	pop	bp
	ret

;; __scanner: 0800:16F3
;;   Called from:
;;     0800:16EA (in _scanf)
__scanner proc
	push	bp
	mov	bp,sp
	sub	sp,2Ah
	push	si
	push	di
	mov	word ptr [bp-28h],0h
	mov	word ptr [bp-26h],0h
	jmp	1721h
0800:1707                      90                                .        

;; fn0800_1708: 0800:1708
;;   Called from:
;;     0800:187E (in __scanner)
;;     0800:1935 (in __scanner)
;;     0800:19CE (in __scanner)
;;     0800:1A74 (in __scanner)
fn0800_1708 proc
	mov	di,[bp+0Ch]
	test	byte ptr [bp-29h],20h
	jz	1718h

l0800_1711:
	les	di,[di]
	add	word ptr [bp+0Ch],4h
	ret

l0800_1718:
	mov	di,[di]
	push	ds
	pop	es
	add	word ptr [bp+0Ch],2h
	ret

l0800_1721:
	push	es
	cld

l0800_1723:
	mov	si,[bp+0Ah]

l0800_1726:
	lodsb
	or	al,al
	jz	1788h

l0800_172B:
	cmp	al,25h
	jz	178Bh

l0800_172F:
	cbw
	xchg	di,ax
	inc	word ptr [bp-26h]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	or	ax,ax
	jl	1764h

l0800_173F:
	or	di,di
	js	1775h

l0800_1743:
	cmp	byte ptr [di+55Ah],1h
	jnz	1775h

l0800_174A:
	xchg	bx,ax
	or	bl,bl
	js	1767h

l0800_174F:
	cmp	byte ptr [bx+55Ah],1h
	jnz	1767h

l0800_1756:
	inc	word ptr [bp-26h]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	or	ax,ax
	jg	174Ah

l0800_1764:
	jmp	1AEBh

l0800_1767:
	push	word ptr [bp+8h]
	push	bx
	call	word ptr [bp+6h]
	pop	cx
	pop	cx
	dec	word ptr [bp-26h]
	jmp	1726h

l0800_1775:
	cmp	ax,di
	jz	1726h

l0800_1779:
	push	word ptr [bp+8h]
	push	ax
	call	word ptr [bp+6h]
	pop	cx
	pop	cx
	dec	word ptr [bp-26h]
	jmp	1AFFh

l0800_1788:
	jmp	1AFFh

l0800_178B:
	mov	word ptr [bp-22h],0FFFFh
	mov	byte ptr [bp-29h],0h

l0800_1794:
	lodsb
	cbw
	mov	[bp+0Ah],si
	xchg	di,ax
	or	di,di
	jl	17E6h

l0800_179E:
	mov	bl,[di+55Ah]
	xor	bh,bh
	mov	ax,bx
	cmp	ax,15h
	jbe	17AEh

l0800_17AB:
	jmp	1AEBh

l0800_17AE:
	mov	bx,ax
	shl	bx,1h
	jmp	word ptr cs:[bx+17B7h]
l0800_17B7	dw	0x17E6
l0800_17B9	dw	0x17E6
l0800_17BB	dw	0x17E6
l0800_17BD	dw	0x17E3
l0800_17BF	dw	0x17E9
l0800_17C1	dw	0x17EF
l0800_17C3	dw	0x19C8
l0800_17C5	dw	0x1837
l0800_17C7	dw	0x1837
l0800_17C9	dw	0x1841
l0800_17CB	dw	0x1908
l0800_17CD	dw	0x1803
l0800_17CF	dw	0x180F
l0800_17D1	dw	0x1809
l0800_17D3	dw	0x1832
l0800_17D5	dw	0x196C
l0800_17D7	dw	0x1A06
l0800_17D9	dw	0x1824
l0800_17DB	dw	0x183C
l0800_17DD	dw	0x1895
l0800_17DF	dw	0x1816
l0800_17E1	dw	0x181D

l0800_17E3:
	jmp	172Fh

l0800_17E6:
	jmp	1AFFh

l0800_17E9:
	or	byte ptr [bp-29h],1h
	jmp	1794h

l0800_17EF:
	sub	di,30h
	xchg	[bp-22h],di
	or	di,di
	jl	1794h

l0800_17F9:
	mov	ax,0Ah
	mul	di
	add	[bp-22h],ax
	jmp	1794h

l0800_1803:
	or	byte ptr [bp-29h],8h
	jmp	1794h

l0800_1809:
	or	byte ptr [bp-29h],4h
	jmp	1794h

l0800_180F:
	or	byte ptr [bp-29h],2h
	jmp	1794h

l0800_1816:
	and	byte ptr [bp-29h],0DFh
	jmp	1794h

l0800_181D:
	or	byte ptr [bp-29h],20h
	jmp	1794h

l0800_1824:
	mov	ax,[bp-26h]
	sub	dx,dx
	test	byte ptr [bp-29h],1h
	jz	187Eh

l0800_182F:
	jmp	1794h

l0800_1832:
	mov	si,8h
	jmp	1844h

l0800_1837:
	mov	si,0Ah
	jmp	1844h

l0800_183C:
	mov	si,10h
	jmp	1844h

l0800_1841:
	mov	si,0h

l0800_1844:
	test	di,20h
	jnz	184Eh

l0800_184A:
	or	byte ptr [bp-29h],4h

l0800_184E:
	lea	ax,[bp-24h]
	push	ax
	lea	ax,[bp-26h]
	push	ax
	mov	ax,[bp-22h]
	and	ax,7FFFh
	push	ax
	mov	ax,si
	push	ax
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	1BCCh
	add	sp,0Eh
	cmp	word ptr [bp-24h],0h
	jle	188Dh

l0800_1875:
	test	byte ptr [bp-29h],1h
	jnz	188Ah

l0800_187B:
	inc	word ptr [bp-28h]

l0800_187E:
	call	1708h
	stosw
	test	byte ptr [bp-29h],4h
	jz	188Ah

l0800_1888:
	xchg	dx,ax
	stosw

l0800_188A:
	jmp	1723h

l0800_188D:
	jl	1892h

l0800_188F:
	jmp	1AFFh

l0800_1892:
	jmp	1AEBh

l0800_1895:
	call	1898h

;; fn0800_1898: 0800:1898
;;   Called from:
;;     0800:1895 (in __scanner)
;;     0800:1895 (in __scanner)
fn0800_1898 proc
	jmp	1B06h
0800:189B                                  FF 76 08 50 FF            .v.P.
0800:18A0 56 06 59 59 FF 4E DA 81 66 DE FF 7F E8 00 00    V.YY.N..f...... 

;; fn0800_18AF: 0800:18AF
fn0800_18AF proc
	jmp	1B2Ch
0800:18B2       52 3C 3A 74 15 0B C0 7E 0C FF 76 08 50 FF   R<:t...~..v.P.
0800:18C0 56 06 59 59 FF 4E DA 5A 8C DB EB 1B E8 00 00    V.YY.N.Z....... 

;; fn0800_18CF: 0800:18CF
fn0800_18CF proc
	jmp	1B2Ch
0800:18D2       5B 0B C0 7E 10 52 53 FF 76 08 50 FF 56 06   [..~.RS.v.P.V.
0800:18E0 59 59 FF 4E DA 5B 5A F6 46 D7 01 75 10 E8 18 FE YY.N.[Z.F..u....
0800:18F0 FF 46 D8 92 AB F6 46 D7 20 74 02 93 AB E9 23 FE .F....F. t....#.
0800:1900 7C 03 E9 FA 01                                  |....           

l0800_1905:
	jmp	1AEBh

l0800_1908:
	lea	ax,[bp-24h]
	push	ax
	lea	ax,[bp-26h]
	push	ax
	mov	ax,7FFFh
	and	ax,[bp-22h]
	push	ax
	push	word ptr [bp+8h]
	push	word ptr [bp+6h]
	push	word ptr [bp+4h]
	call	1B92h
	add	sp,0Ch
	cmp	word ptr [bp-24h],0h
	jle	1964h

l0800_192C:
	test	byte ptr [bp-29h],1h
	jz	1935h

l0800_1932:
	jmp	195Eh
0800:1934             90                                      .           

l0800_1935:
	call	1708h
	inc	word ptr [bp-28h]
	test	byte ptr [bp-29h],4h
	jz	1946h

l0800_1941:
	mov	ax,4h
	jmp	1953h

l0800_1946:
	test	byte ptr [bp-29h],8h
	jz	1951h

l0800_194C:
	mov	ax,8h
	jmp	1953h

l0800_1951:
	xor	ax,ax

l0800_1953:
	push	ax
	push	di
	call	1B96h
	add	sp,4h
	jmp	1723h

l0800_195E:
	call	1B9Ah
	jmp	1723h

l0800_1964:
	call	1B9Ah
	jl	1905h

l0800_1969:
	jmp	1AFFh

l0800_196C:
	call	196Fh

;; fn0800_196F: 0800:196F
;;   Called from:
;;     0800:196C (in __scanner)
;;     0800:196C (in __scanner)
fn0800_196F proc
	jmp	1B06h
0800:1972       F6 46 D7 01 75 06 E8 8D FD FF 46 D8 81 66   .F..u.....F..f
0800:1980 DE FF 7F 74 29 F6 46 D7 01 75 01 AA FF 46 DA 06 ...t).F..u...F..
0800:1990 FF 76 08 FF 56 04 59 07 0B C0 7E 12 0A C0 78 09 .v..V.Y...~...x.
0800:19A0 93 80 BF 5A 05 01 93 7E 05 FF 4E DE 7F D7 06 FF ...Z...~..N.....
0800:19B0 76 08 50 FF 56 06 59 59 07 FF 4E DA F6 46 D7 01 v.P.V.YY..N..F..
0800:19C0 75 03 B0 00 AA E9 5B FD                         u.....[.        

l0800_19C8:
	test	byte ptr [bp-29h],1h
	jnz	19D1h

l0800_19CE:
	call	1708h

l0800_19D1:
	mov	si,[bp-22h]
	or	si,si
	jge	19DBh

l0800_19D8:
	mov	si,1h

l0800_19DB:
	jz	19F7h

l0800_19DD:
	inc	word ptr [bp-26h]
	push	es
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	es
	test	byte ptr [bp-29h],1h
	jnz	19F0h

l0800_19EF:
	stosb

l0800_19F0:
	or	ax,ax
	jl	1A03h

l0800_19F4:
	dec	si
	jg	19DDh

l0800_19F7:
	test	byte ptr [bp-29h],1h
	jnz	1A00h

l0800_19FD:
	inc	word ptr [bp-28h]

l0800_1A00:
	jmp	1723h

l0800_1A03:
	jmp	1AEBh

l0800_1A06:
	sub	ax,ax
	cld
	push	ss
	pop	es
	lea	di,[bp-20h]
	mov	cx,10h

l0800_1A11:
	rep stosw

l0800_1A13:
	lodsb
	and	byte ptr [bp-29h],0EFh
	cmp	al,5Eh
	jnz	1A21h

l0800_1A1C:
	or	byte ptr [bp-29h],10h
	lodsb

l0800_1A21:
	mov	ah,0h

l0800_1A23:
	mov	dl,al
	mov	di,ax
	mov	cl,3h
	shr	di,cl
	mov	cx,107h
	and	cl,dl
	shl	ch,cl
	or	[bp+di-20h],ch

l0800_1A35:
	lodsb
	cmp	al,0h
	jz	1A60h

l0800_1A3A:
	cmp	al,5Dh
	jz	1A63h

l0800_1A3E:
	cmp	al,2Dh
	jnz	1A23h

l0800_1A42:
	cmp	dl,[si]
	ja	1A23h

l0800_1A46:
	cmp	byte ptr [si],5Dh
	jz	1A23h

l0800_1A4B:
	lodsb
	sub	al,dl
	jz	1A35h

l0800_1A50:
	add	dl,al

l0800_1A52:
	rol	ch,1h
	adc	di,0h
	or	[bp+di-20h],ch
	dec	al
	jnz	1A52h

l0800_1A5E:
	jmp	1A35h

l0800_1A60:
	jmp	1AFFh

l0800_1A63:
	mov	[bp+0Ah],si
	and	word ptr [bp-22h],7FFFh
	mov	si,[bp-22h]
	test	byte ptr [bp-29h],1h
	jnz	1A77h

l0800_1A74:
	call	1708h

l0800_1A77:
	dec	si
	jl	1ACAh

l0800_1A7A:
	inc	word ptr [bp-26h]
	push	es
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	es
	or	ax,ax
	jl	1AD9h

l0800_1A8A:
	xchg	si,ax
	mov	bx,si
	mov	cl,3h
	shr	si,cl
	mov	cx,107h
	and	cl,bl
	shl	ch,cl
	test	[bp+si-20h],ch
	xchg	si,ax
	xchg	bx,ax
	jz	1AA7h

l0800_1A9F:
	test	byte ptr [bp-29h],10h
	jz	1AADh

l0800_1AA5:
	jmp	1AB6h

l0800_1AA7:
	test	byte ptr [bp-29h],10h
	jz	1AB6h

l0800_1AAD:
	test	byte ptr [bp-29h],1h
	jnz	1A77h

l0800_1AB3:
	stosb
	jmp	1A77h

l0800_1AB6:
	push	es
	push	word ptr [bp+8h]
	push	ax
	call	word ptr [bp+6h]
	pop	cx
	pop	cx
	pop	es
	dec	word ptr [bp-26h]
	inc	si
	cmp	si,[bp-22h]
	jge	1AD3h

l0800_1ACA:
	test	byte ptr [bp-29h],1h
	jnz	1AD6h

l0800_1AD0:
	inc	word ptr [bp-28h]

l0800_1AD3:
	mov	al,0h
	stosb

l0800_1AD6:
	jmp	1723h

l0800_1AD9:
	inc	si
	cmp	si,[bp-22h]
	jge	1AEBh

l0800_1ADF:
	test	byte ptr [bp-29h],1h
	jnz	1AEBh

l0800_1AE5:
	mov	al,0h
	stosb
	inc	word ptr [bp-28h]

;; fn0800_1AEB: 0800:1AEB
;;   Called from:
;;     0800:1764 (in __scanner)
;;     0800:1785 (in __scanner)
;;     0800:1788 (in __scanner)
;;     0800:17AB (in __scanner)
;;     0800:17E6 (in __scanner)
;;     0800:1892 (in __scanner)
;;     0800:1905 (in __scanner)
;;     0800:1A03 (in __scanner)
;;     0800:1ADD (in __scanner)
;;     0800:1AE3 (in __scanner)
;;     0800:1AE8 (in __scanner)
;;     0800:1B2A (in fn0800_1B06)
;;     0800:1B89 (in fn0800_1B2C)
fn0800_1AEB proc
	push	word ptr [bp+8h]
	mov	ax,0FFFFh
	push	ax
	call	word ptr [bp+6h]
	pop	cx
	pop	cx
	cmp	word ptr [bp-28h],1h
	sbb	word ptr [bp-28h],0h

;; fn0800_1AFF: 0800:1AFF
;;   Called from:
;;     0800:188F (in __scanner)
;;     0800:1969 (in __scanner)
;;     0800:1A60 (in __scanner)
;;     0800:1AFB (in fn0800_1AEB)
fn0800_1AFF proc
	pop	es
	mov	ax,[bp-28h]
	jmp	1B8Ch

;; fn0800_1B06: 0800:1B06
;;   Called from:
;;     0800:1898 (in fn0800_1898)
;;     0800:196F (in fn0800_196F)
fn0800_1B06 proc
	inc	word ptr [bp-26h]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	or	ax,ax
	jle	1B27h

l0800_1B14:
	or	al,al
	js	1B21h

l0800_1B18:
	xchg	bx,ax
	cmp	byte ptr [bx+55Ah],1h
	xchg	bx,ax
	jz	1B06h

l0800_1B21:
	pop	cx
	add	cx,3h
	jmp	cx

l0800_1B27:
	jz	1B21h

l0800_1B29:
	pop	cx
	jmp	1AEBh

;; fn0800_1B2C: 0800:1B2C
;;   Called from:
;;     0800:18AF (in fn0800_18AF)
;;     0800:18CF (in fn0800_18CF)
fn0800_1B2C proc
	sub	dx,dx
	mov	cx,4h

l0800_1B31:
	dec	word ptr [bp-22h]
	jl	1B7Bh

l0800_1B36:
	push	dx
	push	cx
	inc	word ptr [bp-26h]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	pop	cx
	pop	dx
	or	ax,ax
	jle	1B7Dh

l0800_1B48:
	dec	cl
	jl	1B7Dh

l0800_1B4C:
	mov	ch,al
	sub	ch,30h
	jc	1B7Dh

l0800_1B53:
	cmp	ch,0Ah
	jc	1B6Fh

l0800_1B58:
	sub	ch,11h
	jc	1B7Dh

l0800_1B5D:
	cmp	ch,6h
	jc	1B6Ch

l0800_1B62:
	sub	ch,20h
	jc	1B7Dh

l0800_1B67:
	cmp	ch,6h
	jnc	1B7Dh

l0800_1B6C:
	add	ch,0Ah

l0800_1B6F:
	shl	dx,1h
	shl	dx,1h
	shl	dx,1h
	shl	dx,1h
	add	dl,ch
	jmp	1B31h

l0800_1B7B:
	sub	ax,ax

l0800_1B7D:
	cmp	cl,4h
	jz	1B88h

l0800_1B82:
	pop	cx
	add	cx,3h
	jmp	cx

l0800_1B88:
	pop	cx
	jmp	1AEBh

l0800_1B8C:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; __scantod: 0800:1B92
;;   Called from:
;;     0800:1920 (in __scanner)
__scantod proc
	jmp	word ptr [05E2h]

;; __scanrslt: 0800:1B96
;;   Called from:
;;     0800:1955 (in __scanner)
__scanrslt proc
	jmp	word ptr [05E4h]

;; __scanpop: 0800:1B9A
;;   Called from:
;;     0800:195E (in __scanner)
;;     0800:1964 (in __scanner)
__scanpop proc
	jmp	word ptr [05E6h]

;; fn0800_1B9E: 0800:1B9E
;;   Called from:
;;     0800:1CAB (in __scantol)
;;     0800:1CD1 (in __scantol)
;;     0800:1CFD (in __scantol)
fn0800_1B9E proc
	push	bx
	sub	bl,30h
	jc	1BC7h

l0800_1BA4:
	cmp	bl,9h
	jbe	1BBBh

l0800_1BA9:
	cmp	bl,2Ah
	ja	1BB3h

l0800_1BAE:
	sub	bl,7h
	jmp	1BB6h

l0800_1BB3:
	sub	bl,27h

l0800_1BB6:
	cmp	bl,9h
	jbe	1BC7h

l0800_1BBB:
	cmp	bl,cl
	jnc	1BC7h

l0800_1BBF:
	add	sp,2h
	clc
	mov	bh,0h

l0800_1BC5:
	jmp	1BCBh

l0800_1BC7:
	pop	bx
	stc
	jmp	1BC5h

l0800_1BCB:
	ret

;; __scantol: 0800:1BCC
;;   Called from:
;;     0800:1869 (in __scanner)
__scantol proc
	push	bp
	mov	bp,sp
	sub	sp,6h
	push	si
	push	di
	mov	byte ptr [bp-5h],0h
	mov	word ptr [bp-4h],0h
	mov	word ptr [bp-2h],1h
	push	es
	mov	di,241h

l0800_1BE6:
	inc	word ptr [bp-4h]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	or	ax,ax
	jl	1C5Ah

l0800_1BF4:
	cbw
	xchg	bx,ax
	test	bl,80h
	jnz	1C00h

l0800_1BFB:
	test	byte ptr [bx+di],1h
	jnz	1BE6h

l0800_1C00:
	xchg	bx,ax
	dec	word ptr [bp+0Ch]
	jl	1C61h

l0800_1C06:
	cmp	al,2Bh
	jz	1C11h

l0800_1C0A:
	cmp	al,2Dh
	jnz	1C24h

l0800_1C0E:
	inc	byte ptr [bp-5h]

l0800_1C11:
	dec	word ptr [bp+0Ch]
	jl	1C61h

l0800_1C16:
	inc	word ptr [bp-4h]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	or	ax,ax
	jl	1C5Ah

l0800_1C24:
	sub	si,si
	mov	di,si
	mov	cx,[bp+0Ah]
	jcxz	1C7Bh

l0800_1C2D:
	cmp	cx,24h
	ja	1C61h

l0800_1C32:
	cmp	cl,2h
	jc	1C61h

l0800_1C37:
	cmp	al,30h
	jnz	1CA7h

l0800_1C3B:
	cmp	cl,10h
	jnz	1CA5h

l0800_1C40:
	dec	word ptr [bp+0Ch]
	jl	1C78h

l0800_1C45:
	inc	word ptr [bp-4h]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	cmp	al,78h
	jz	1CA5h

l0800_1C53:
	cmp	al,58h
	jz	1CA5h

l0800_1C57:
	jmp	1CCDh
0800:1C59                            90                            .      

l0800_1C5A:
	mov	word ptr [bp-2h],0FFFFh
	jmp	1C66h

l0800_1C61:
	mov	word ptr [bp-2h],0h

l0800_1C66:
	push	word ptr [bp+8h]
	push	ax
	call	word ptr [bp+6h]
	pop	cx
	pop	cx
	dec	word ptr [bp-4h]
	sub	ax,ax
	cwd
	jmp	1D1Fh

l0800_1C78:
	jmp	1D0Fh

l0800_1C7B:
	cmp	al,30h
	mov	word ptr [bp+0Ah],0Ah
	jnz	1CA7h

l0800_1C84:
	dec	word ptr [bp+0Ch]
	jl	1C78h

l0800_1C89:
	inc	word ptr [bp-4h]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	mov	word ptr [bp+0Ah],8h
	cmp	al,78h
	jz	1CA0h

l0800_1C9C:
	cmp	al,58h
	jnz	1CCDh

l0800_1CA0:
	mov	word ptr [bp+0Ah],10h

l0800_1CA5:
	jmp	1CBEh

l0800_1CA7:
	mov	cx,[bp+0Ah]
	xchg	bx,ax
	call	1B9Eh
	xchg	bx,ax
	jc	1C61h

l0800_1CB1:
	xchg	si,ax
	jmp	1CBEh

l0800_1CB4:
	xchg	si,ax
	mul	word ptr [bp+0Ah]
	add	si,ax
	adc	di,dx
	jnz	1CEAh

l0800_1CBE:
	dec	word ptr [bp+0Ch]
	jl	1D0Fh

l0800_1CC3:
	inc	word ptr [bp-4h]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx

l0800_1CCD:
	mov	cx,[bp+0Ah]
	xchg	bx,ax
	call	1B9Eh
	xchg	bx,ax
	jnc	1CB4h

l0800_1CD7:
	jmp	1D03h

l0800_1CD9:
	xchg	si,ax
	mul	cx
	xchg	di,ax
	xchg	dx,cx
	mul	dx
	add	si,di
	adc	ax,cx
	xchg	di,ax
	adc	dl,dh
	jnz	1D32h

l0800_1CEA:
	dec	word ptr [bp+0Ch]
	jl	1D0Fh

l0800_1CEF:
	inc	word ptr [bp-4h]
	push	word ptr [bp+8h]
	call	word ptr [bp+4h]
	pop	cx
	mov	cx,[bp+0Ah]
	xchg	bx,ax
	call	1B9Eh
	xchg	bx,ax
	jnc	1CD9h

l0800_1D03:
	push	word ptr [bp+8h]
	push	ax
	call	word ptr [bp+6h]
	pop	cx
	pop	cx
	dec	word ptr [bp-4h]

l0800_1D0F:
	mov	dx,di
	xchg	si,ax
	cmp	byte ptr [bp-5h],0h
	jz	1D1Fh

l0800_1D18:
	neg	dx
	neg	ax
	sbb	dx,0h

l0800_1D1F:
	mov	di,[bp+0Eh]
	mov	bx,[bp-4h]
	add	[di],bx
	mov	di,[bp+10h]
	mov	bx,[bp-2h]
	mov	[di],bx
	pop	es
	jmp	1D48h

l0800_1D32:
	mov	ax,0FFFFh
	mov	dx,7FFFh
	add	al,[bp-5h]
	adc	ah,0h
	adc	dx,0h
	mov	word ptr [bp-2h],2h
	jmp	1D1Fh

l0800_1D48:
	pop	di
	pop	si
	mov	sp,bp
	pop	bp
	ret

;; _tell: 0800:1D4E
_tell proc
	push	bp
	mov	bp,sp
	mov	ax,1h
	push	ax
	xor	ax,ax
	push	ax
	push	ax
	push	word ptr [bp+4h]
	call	0C28h
	mov	sp,bp
	jmp	1D63h

l0800_1D63:
	pop	bp
	ret
0800:1D65                55 8B EC 56 8B 76 06 83 7E 04 FF      U..V.v..~..
0800:1D70 74 35 FF 04 8B 04 3D 01 00 7E 11 8A 46 04 FF 4C t5....=..~..F..L
0800:1D80 0A 8B 5C 0A 88 07 B4 00 EB 22 EB 1B 83 3C 01 75 ..\......"...<.u
0800:1D90 14 8B C6 05 05 00 89 44 0A 8A 46 04 88 44 05 B4 .......D..F..D..
0800:1DA0 00 EB 09 EB 02 FF 0C B8 FF FF EB 00 5E 5D C3 00 ............^]..
