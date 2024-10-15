;;; Segment .text (00400230)

;; __start: 00400230
__start proc
	bltzal	r0,00400238
	nop
	lui	r28,%hi(FFFF8628)
	addiu	r28,r28,%lo(FFFF8628)
	addu	r28,r28,ra
	or	ra,r0,r0
	lw	r4,0000(sp)
	addiu	r5,sp,+00000004
	addiu	r6,r4,+00000001
	sll	r6,r6,02
	add	r6,r6,r5
	lw	r7,-7E78(r28)
	addiu	sp,sp,-00000018
	lw	r2,-7F40(r28)
	nop
	sw	r2,0010(sp)
	lw	r25,-7F98(r28)
	nop
	jalr	ra,r25
	nop
	addiu	sp,sp,+00000018

l00400284:
	beq	r0,r0,00400284
	nop
0040028C                                     00 00 00 00             ....

;; strcpy: 00400290
strcpy proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+000000AC

;; printf: 004002A0
printf proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+000000A7

;; recv: 004002B0
recv proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+000000A6

;; connect: 004002C0
connect proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+000000A5

;; strerror: 004002D0
strerror proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+000000A2

;; __xpg_basename: 004002E0
__xpg_basename proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+000000A1

;; snprintf: 004002F0
snprintf proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+000000A0

;; getpid: 00400300
getpid proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000098

;; fgets: 00400310
fgets proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000096

;; memcpy: 00400320
memcpy proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000095

;; tolower: 00400330
tolower proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000092

;; malloc: 00400340
malloc proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000090

;; vsnprintf: 00400350
vsnprintf proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000008F

;; strtoul: 00400360
strtoul proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000008E

;; socket: 00400370
socket proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000008A

;; select: 00400380
select proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000089

;; fflush: 00400390
fflush proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000086

;; strncasecmp: 004003A0
strncasecmp proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000081

;; bzero: 004003B0
bzero proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000080

;; send: 004003C0
send proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000007F

;; accept: 004003D0
accept proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000007C

;; rename: 004003E0
rename proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000079

;; strrchr: 004003F0
strrchr proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000078

;; fprintf: 00400400
fprintf proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000074

;; strcat: 00400410
strcat proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000073

;; bind: 00400420
bind proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000072

;; inet_addr: 00400430
inet_addr proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000071

;; signal: 00400440
signal proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000006B

;; read: 00400450
read proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000006A

;; strncmp: 00400460
strncmp proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000068

;; strncpy: 00400470
strncpy proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000066

;; unlink: 00400480
unlink proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000065

;; strcasecmp: 00400490
strcasecmp proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000063

;; listen: 004004A0
listen proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000005E

;; fork: 004004B0
fork proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000005B

;; gettimeofday: 004004C0
gettimeofday proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000057

;; fopen: 004004D0
fopen proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000056

;; ftell: 004004E0
ftell proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000054

;; fclose: 004004F0
fclose proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000052

;; inet_ntoa: 00400500
inet_ntoa proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000050

;; time: 00400510
time proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000004F

;; strcspn: 00400520
strcspn proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000004A

;; strsep: 00400530
strsep proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000047

;; getsockopt: 00400540
getsockopt proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000044

;; strftime: 00400550
strftime proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000043

;; __errno_location: 00400560
__errno_location proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000041

;; exit: 00400570
exit proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000003F

;; gmtime: 00400580
gmtime proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000039

;; strspn: 00400590
strspn proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000038

;; strlen: 004005A0
strlen proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000036

;; __uClibc_start_main: 004005B0
__uClibc_start_main proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000035

;; toupper: 004005C0
toupper proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000034

;; strchr: 004005D0
strchr proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000031

;; close: 004005E0
close proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+0000002A

;; strpbrk: 004005F0
strpbrk proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000027

;; free: 00400600
free proc
	lw	r25,-7FF0(r28)
	addu	r15,ra,r0
	jalr	ra,r25
	addiu	r24,r0,+00000026
	nop
	nop
	nop
	nop

;; __do_global_dtors_aux: 00400620
;;   Called from:
;;     0040061C (in strcpy)
;;     0040061C (in strerror)
;;     0040061C (in vsnprintf)
;;     0040061C (in read)
;;     0040061C (in free)
__do_global_dtors_aux proc
	lui	r28,%hi(FFFF8240)
	addiu	r28,r28,%lo(FFFF8240)
	addu	r28,r28,r25
	addiu	sp,sp,-00000020
	sw	r28,0010(sp)
	lw	r2,-7FE8(r28)
	nop
	addiu	r2,r2,+00000008
	lw	r2,0000(r2)
	nop
	bne	r2,r0,004006F4
	sw	ra,001C(sp)

l00400650:
	lw	r3,-7FE8(r28)
	nop
	addiu	r3,r3,+00000004
	lw	r3,0000(r3)
	nop
	lw	r2,0000(r3)
	nop
	beq	r2,r0,004006B8
	addiu	r2,r3,+00000004

l00400674:
	lw	r25,0000(r3)
	lw	r1,-7FE8(r28)
	nop
	addiu	r1,r1,+00000004
	jalr	ra,r25
	sw	r2,0000(r1)
	lw	r28,0010(sp)
	nop
	lw	r3,-7FE8(r28)
	nop
	addiu	r3,r3,+00000004
	lw	r3,0000(r3)
	nop
	lw	r2,0000(r3)
	nop
	bne	r2,r0,00400674
	addiu	r2,r3,+00000004

l004006B8:
	lw	r25,-7EAC(r28)
	nop
	beq	r25,r0,004006E4
	addiu	r2,r0,+00000001

l004006C8:
	lw	r4,-7FE8(r28)
	nop
	addiu	r4,r4,+00000850
	jalr	ra,r25
	nop
	lw	r28,0010(sp)
	addiu	r2,r0,+00000001

l004006E4:
	lw	r1,-7FE8(r28)
	nop
	addiu	r1,r1,+00000008
	sw	r2,0000(r1)

l004006F4:
	lw	ra,001C(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000020

;; fini_dummy: 00400704
fini_dummy proc
	lui	r28,%hi(FFFF815C)
	addiu	r28,r28,%lo(FFFF815C)
	addu	r28,r28,r25
	addiu	sp,sp,-00000020
	sw	r28,0010(sp)
	sw	ra,001C(sp)
	lw	ra,001C(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000020

;; frame_dummy: 0040072C
frame_dummy proc
	lui	r28,%hi(FFFF8134)
	addiu	r28,r28,%lo(FFFF8134)
	addu	r28,r28,r25
	addiu	sp,sp,-00000020
	sw	r28,0010(sp)
	lw	r25,-7FBC(r28)
	nop
	beq	r25,r0,00400774
	sw	ra,001C(sp)

l00400750:
	lw	r4,-7FE8(r28)
	nop
	addiu	r4,r4,+00000850
	lw	r5,-7FE8(r28)
	nop
	addiu	r5,r5,+00000AB0
	jalr	ra,r25
	nop
	lw	r28,0010(sp)

l00400774:
	lw	ra,001C(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000020

;; init_dummy: 00400784
init_dummy proc
	lui	r28,%hi(FFFF80DC)
	addiu	r28,r28,%lo(FFFF80DC)
	addu	r28,r28,r25
	addiu	sp,sp,-00000020
	sw	r28,0010(sp)
	sw	ra,001C(sp)
	lw	ra,001C(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000020
004007AC                                     00 00 00 00             ....

;; handle_stop: 004007B0
handle_stop proc
	lui	r28,%hi(FFFF80B0)
	addiu	r28,r28,%lo(FFFF80B0)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0018(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	sw	r2,0010(sp)
	addiu	r4,r0,+00000003
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-0000622C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00006208
	addiu	r7,r0,+00000050
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	addiu	r2,r0,+00000001
	lw	r1,-7FA0(r28)
	nop
	sw	r2,0000(r1)
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; handle_log_rotate: 0040083C
handle_log_rotate proc
	lui	r28,%hi(FFFF8024)
	addiu	r28,r28,%lo(FFFF8024)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0018(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	sw	r2,0010(sp)
	addiu	r4,r0,+00000003
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000061FC
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-000061D0
	addiu	r7,r0,+0000005A
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r25,-7DF0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; main: 004008D0
main proc
	lui	r28,%hi(0FC07F90)
	addiu	r28,r28,%lo(0FC07F90)
	addu	r28,r28,r25
	addiu	sp,sp,-000001A0
	sw	r28,0038(sp)
	sw	ra,019C(sp)
	sw	r30,0198(sp)
	sw	r16,0194(sp)
	or	r30,sp,r0
	sw	r4,01A0(r30)
	sw	r5,01A4(r30)
	addiu	r2,r0,+00000002
	sw	r2,0040(r30)
	lw	r2,-7F4C(r28)
	nop
	lw	r2,0000(r2)
	lw	r1,-7E40(r28)
	nop
	sw	r2,0000(r1)
	addiu	r4,r0,+00000002
	lw	r5,-7F18(r28)
	lw	r25,-7EC0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	addiu	r4,r0,+00000001
	lw	r5,-7E3C(r28)
	lw	r25,-7EC0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r2,01A4(r30)
	nop
	lw	r4,0000(r2)
	or	r5,r0,r0
	lw	r25,-7EB8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r4,01A0(r30)
	lw	r5,01A4(r30)
	lw	r25,-7E04(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r2,-7E20(r28)
	nop
	lw	r2,04BC(r2)
	nop
	beq	r2,r0,00400A54
	nop

l004009BC:
	lw	r25,-7F00(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	sw	r2,0174(r30)
	lw	r2,0174(r30)
	nop
	beq	r2,r0,00400A00
	nop

l004009E4:
	or	r4,r0,r0
	lw	r25,-7F70(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop

l00400A00:
	or	r4,r0,r0
	lw	r25,-7FC4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	addiu	r4,r0,+00000001
	lw	r25,-7FC4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	addiu	r4,r0,+00000002
	lw	r25,-7FC4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop

l00400A54:
	lw	r2,-7E20(r28)
	nop
	addiu	r2,r2,+00000695
	sw	r2,0068(r30)
	lw	r2,0068(r30)
	nop
	beq	r2,r0,00400B20
	nop

l00400A74:
	lw	r2,0068(r30)
	nop
	lb	r2,0000(r2)
	nop
	beq	r2,r0,00400B20
	nop

l00400A8C:
	lw	r4,0068(r30)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000061BC
	lw	r25,-7F14(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	sw	r2,0174(r30)
	lw	r2,0174(r30)
	nop
	beq	r2,r0,00400B20
	nop

l00400AC4:
	lw	r25,-7E0C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	lw	r4,0174(r30)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000061B8
	or	r6,r2,r0
	lw	r25,-7E9C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r4,0174(r30)
	lw	r25,-7F24(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop

l00400B20:
	lw	r2,-7E20(r28)
	nop
	lw	r2,038C(r2)
	lw	r1,-7F6C(r28)
	nop
	sw	r2,0000(r1)
	lw	r2,-7E20(r28)
	nop
	addiu	r2,r2,+0000039D
	lw	r1,-7FC0(r28)
	nop
	sw	r2,0000(r1)
	lw	r2,-7E20(r28)
	nop
	lw	r2,0684(r2)
	nop
	sll	r2,r2,0A
	lw	r1,-7E38(r28)
	nop
	sw	r2,0000(r1)
	lw	r2,-7E20(r28)
	nop
	lw	r2,05EC(r2)
	lw	r1,-7F54(r28)
	nop
	sw	r2,0000(r1)
	lw	r4,-7FE0(r28)
	nop
	addiu	r4,r4,-000061B4
	lw	r25,-7F90(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r2,01A4(r30)
	nop
	lw	r4,0000(r2)
	lw	r25,-7DE8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	sw	r2,017C(r30)
	lw	r2,-7E20(r28)
	nop
	addiu	r2,r2,+0000000D
	sw	r2,0010(sp)
	lw	r2,-7E20(r28)
	nop
	lw	r2,012C(r2)
	nop
	sw	r2,0014(sp)
	lw	r2,-7E20(r28)
	nop
	lw	r2,01C4(r2)
	nop
	sw	r2,0018(sp)
	lw	r2,-7E20(r28)
	nop
	addiu	r2,r2,+000001D5
	sw	r2,001C(sp)
	lw	r2,-7E20(r28)
	nop
	lw	r2,02F4(r2)
	nop
	sw	r2,0020(sp)
	lw	r2,-7FC0(r28)
	nop
	lw	r2,0000(r2)
	nop
	sw	r2,0024(sp)
	lw	r2,-7F6C(r28)
	nop
	lw	r2,0000(r2)
	nop
	sw	r2,0028(sp)
	lw	r2,-7E38(r28)
	nop
	lw	r2,0000(r2)
	nop
	sw	r2,0180(r30)
	lw	r2,0180(r30)
	nop
	bgez	r2,00400C88
	nop

l00400C78:
	lw	r3,0180(r30)
	nop
	addiu	r3,r3,+000003FF
	sw	r3,0180(r30)

l00400C88:
	lw	r25,0180(r30)
	nop
	sra	r2,r25,0A
	sw	r2,002C(sp)
	lw	r2,-7E20(r28)
	nop
	lw	r2,0554(r2)
	nop
	sw	r2,0030(sp)
	addiu	r4,r0,+00000003
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000061B0
	lw	r6,017C(r30)
	lw	r7,-7E00(r28)
	nop
	lw	r7,0000(r7)
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r25,-7F58(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	sw	r2,004C(r30)
	lw	r2,004C(r30)
	nop
	bne	r2,r0,00400D28
	nop

l00400D0C:
	addiu	r4,r0,-00000001
	lw	r25,-7F70(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop

l00400D28:
	lw	r25,-7E94(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	sw	r2,0054(r30)
	lw	r2,0054(r30)
	nop
	bne	r2,r0,00400D6C
	nop

l00400D50:
	addiu	r4,r0,-00000001
	lw	r25,-7F70(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop

l00400D6C:
	lw	r4,-7E20(r28)
	nop
	addiu	r4,r4,+0000000D
	lw	r5,-7E20(r28)
	nop
	lw	r5,012C(r5)
	lw	r6,-7E20(r28)
	nop
	lw	r6,0554(r6)
	or	r7,r0,r0
	lw	r25,-7E58(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	sw	r2,0050(r30)
	lw	r4,0054(r30)
	lw	r5,0050(r30)
	lw	r25,-7FB0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r4,-7E20(r28)
	nop
	addiu	r4,r4,+0000000D
	lw	r5,-7E20(r28)
	nop
	lw	r5,01C4(r5)
	lw	r6,-7E20(r28)
	nop
	lw	r6,0554(r6)
	addiu	r7,r0,+00000001
	lw	r25,-7E58(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	sw	r2,0050(r30)
	lw	r4,0054(r30)
	lw	r5,0050(r30)
	lw	r25,-7FB0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	addiu	r2,r30,+00000058
	or	r4,r2,r0
	addiu	r5,r0,+00000010
	lw	r25,-7E6C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	addiu	r2,r0,+00000002
	sh	r2,0058(r30)
	lw	r2,-7E20(r28)
	nop
	lhu	r2,02F6(r2)
	nop
	sh	r2,005A(r30)
	lw	r4,-7E20(r28)
	nop
	addiu	r4,r4,+000001D5
	lw	r25,-7EA8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	sw	r2,005C(r30)
	lw	r2,0054(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0050(r30)

l00400EA4:
	lw	r2,0050(r30)
	nop
	bne	r2,r0,00400EBC
	nop

l00400EB4:
	beq	r0,r0,00400F10
	nop

l00400EBC:
	lw	r4,0050(r30)
	lw	r25,-7DDC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	bne	r2,r0,00400EF8
	nop

l00400EDC:
	addiu	r4,r0,-00000001
	lw	r25,-7F70(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop

l00400EF8:
	lw	r2,0050(r30)
	nop
	lw	r2,0018(r2)
	nop
	beq	r0,r0,00400EA4
	sw	r2,0050(r30)

l00400F10:
	lw	r2,-7FA0(r28)
	nop
	lw	r2,0000(r2)
	nop
	beq	r2,r0,00400F30
	nop

l00400F28:
	beq	r0,r0,00401AD4
	nop

l00400F30:
	sw	r0,0040(r30)
	or	r4,r0,r0
	lw	r25,-7F30(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	sw	r2,0170(r30)
	addiu	r2,r30,+00000070
	sw	r2,0178(r30)
	sw	r0,0174(r30)

l00400F5C:
	lw	r2,0174(r30)
	nop
	sltiu	r2,r2,+00000020
	bne	r2,r0,00400F78
	nop

l00400F70:
	beq	r0,r0,00400FA4
	nop

l00400F78:
	lw	r3,0178(r30)
	lw	r2,0174(r30)
	nop
	sll	r2,r2,02
	addu	r2,r3,r2
	sw	r0,0000(r2)
	lw	r2,0174(r30)
	nop
	addiu	r2,r2,+00000001
	beq	r0,r0,00400F5C
	sw	r2,0174(r30)

l00400FA4:
	addiu	r2,r30,+000000F0
	sw	r2,0174(r30)
	sw	r0,0178(r30)

l00400FB0:
	lw	r2,0178(r30)
	nop
	sltiu	r2,r2,+00000020
	bne	r2,r0,00400FCC
	nop

l00400FC4:
	beq	r0,r0,00400FF8
	nop

l00400FCC:
	lw	r3,0174(r30)
	lw	r2,0178(r30)
	nop
	sll	r2,r2,02
	addu	r2,r3,r2
	sw	r0,0000(r2)
	lw	r2,0178(r30)
	nop
	addiu	r2,r2,+00000001
	beq	r0,r0,00400FB0
	sw	r2,0178(r30)

l00400FF8:
	lw	r2,0054(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0050(r30)

l0040100C:
	lw	r2,0050(r30)
	nop
	bne	r2,r0,00401024
	nop

l0040101C:
	beq	r0,r0,004010CC
	nop

l00401024:
	lw	r2,0050(r30)
	nop
	lw	r2,0000(r2)
	nop
	srl	r4,r2,05
	or	r2,r4,r0
	sll	r3,r2,02
	addiu	r2,r30,+00000070
	addu	r5,r2,r3
	or	r2,r4,r0
	sll	r3,r2,02
	addiu	r2,r30,+00000070
	addu	r4,r2,r3
	lw	r2,0050(r30)
	nop
	lw	r2,0000(r2)
	nop
	andi	r3,r2,0000001F
	addiu	r2,r0,+00000001
	sllv	r3,r2,r3
	lw	r2,0000(r4)
	nop
	or	r2,r2,r3
	sw	r2,0000(r5)
	lw	r2,0050(r30)
	lw	r3,0040(r30)
	lw	r2,0000(r2)
	nop
	slt	r2,r3,r2
	beq	r2,r0,004010B4
	nop

l004010A0:
	lw	r2,0050(r30)
	nop
	lw	r2,0000(r2)
	nop
	sw	r2,0040(r30)

l004010B4:
	lw	r2,0050(r30)
	nop
	lw	r2,0018(r2)
	nop
	beq	r0,r0,0040100C
	sw	r2,0050(r30)

l004010CC:
	lw	r2,004C(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0048(r30)

l004010E0:
	lw	r2,0048(r30)
	nop
	bne	r2,r0,004010F8
	nop

l004010F0:
	beq	r0,r0,00401358
	nop

l004010F8:
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	bltz	r2,0040121C
	nop

l00401110:
	lw	r2,0048(r30)
	lw	r3,0040(r30)
	lw	r2,0000(r2)
	nop
	slt	r2,r3,r2
	beq	r2,r0,00401140
	nop

l0040112C:
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	sw	r2,0040(r30)

l00401140:
	lw	r2,0048(r30)
	nop
	lw	r2,0FAC(r2)
	nop
	bne	r2,r0,004011BC
	nop

l00401158:
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	srl	r4,r2,05
	or	r2,r4,r0
	sll	r3,r2,02
	addiu	r2,r30,+00000070
	addu	r5,r2,r3
	or	r2,r4,r0
	sll	r3,r2,02
	addiu	r2,r30,+00000070
	addu	r4,r2,r3
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	andi	r3,r2,0000001F
	addiu	r2,r0,+00000001
	sllv	r3,r2,r3
	lw	r2,0000(r4)
	nop
	or	r2,r2,r3
	beq	r0,r0,0040121C
	sw	r2,0000(r5)

l004011BC:
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	srl	r4,r2,05
	or	r2,r4,r0
	sll	r3,r2,02
	addiu	r2,r30,+000000F0
	addu	r5,r2,r3
	or	r2,r4,r0
	sll	r3,r2,02
	addiu	r2,r30,+000000F0
	addu	r4,r2,r3
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	andi	r3,r2,0000001F
	addiu	r2,r0,+00000001
	sllv	r3,r2,r3
	lw	r2,0000(r4)
	nop
	or	r2,r2,r3
	sw	r2,0000(r5)

l0040121C:
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	bltz	r2,00401340
	nop

l00401234:
	lw	r2,0048(r30)
	lw	r3,0040(r30)
	lw	r2,0004(r2)
	nop
	slt	r2,r3,r2
	beq	r2,r0,00401264
	nop

l00401250:
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	sw	r2,0040(r30)

l00401264:
	lw	r2,0048(r30)
	nop
	lw	r2,178C(r2)
	nop
	bne	r2,r0,004012E0
	nop

l0040127C:
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	srl	r4,r2,05
	or	r2,r4,r0
	sll	r3,r2,02
	addiu	r2,r30,+00000070
	addu	r5,r2,r3
	or	r2,r4,r0
	sll	r3,r2,02
	addiu	r2,r30,+00000070
	addu	r4,r2,r3
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	andi	r3,r2,0000001F
	addiu	r2,r0,+00000001
	sllv	r3,r2,r3
	lw	r2,0000(r4)
	nop
	or	r2,r2,r3
	beq	r0,r0,00401340
	sw	r2,0000(r5)

l004012E0:
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	srl	r4,r2,05
	or	r2,r4,r0
	sll	r3,r2,02
	addiu	r2,r30,+000000F0
	addu	r5,r2,r3
	or	r2,r4,r0
	sll	r3,r2,02
	addiu	r2,r30,+000000F0
	addu	r4,r2,r3
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	andi	r3,r2,0000001F
	addiu	r2,r0,+00000001
	sllv	r3,r2,r3
	lw	r2,0000(r4)
	nop
	or	r2,r2,r3
	sw	r2,0000(r5)

l00401340:
	lw	r2,0048(r30)
	nop
	lw	r2,17B8(r2)
	nop
	beq	r0,r0,004010E0
	sw	r2,0048(r30)

l00401358:
	lw	r2,0040(r30)
	nop
	addiu	r2,r2,+00000001
	addiu	r3,r30,+00000070
	sw	r0,0010(sp)
	or	r4,r2,r0
	or	r5,r3,r0
	or	r6,r0,r0
	or	r7,r0,r0
	lw	r25,-7E48(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	bgez	r2,00401454
	nop

l00401398:
	lw	r25,-7F68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	lw	r3,0000(r2)
	addiu	r2,r0,+00000004
	bne	r3,r2,004013C4
	nop

l004013BC:
	beq	r0,r0,00400F10
	nop

l004013C4:
	lw	r25,-7F68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	or	r16,r2,r0
	lw	r25,-7F68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	lw	r4,0000(r2)
	lw	r25,-7DE4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	or	r4,r0,r0
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00006150
	lw	r6,0000(r16)
	or	r7,r2,r0
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	addiu	r4,r0,-00000001
	lw	r25,-7F70(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop

l00401454:
	lw	r2,0054(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0050(r30)

l00401468:
	lw	r2,0050(r30)
	nop
	bne	r2,r0,00401480
	nop

l00401478:
	beq	r0,r0,004015B4
	nop

l00401480:
	lw	r2,0050(r30)
	nop
	lw	r2,0000(r2)
	nop
	srl	r2,r2,05
	sll	r3,r2,02
	addiu	r2,r30,+00000070
	addu	r4,r2,r3
	lw	r2,0050(r30)
	nop
	lw	r2,0000(r2)
	nop
	andi	r3,r2,0000001F
	lw	r2,0000(r4)
	nop
	srav	r2,r2,r3
	andi	r2,r2,00000001
	beq	r2,r0,0040159C
	nop

l004014CC:
	lw	r2,0050(r30)
	nop
	addiu	r2,r2,+00000008
	addiu	r3,r30,+00000058
	lw	r6,0050(r30)
	lw	r7,0050(r30)
	or	r4,r2,r0
	or	r5,r3,r0
	lw	r6,0000(r6)
	lw	r7,0020(r7)
	lw	r25,-7F28(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	sw	r2,0044(r30)
	lw	r2,0044(r30)
	nop
	bne	r2,r0,00401524
	nop

l0040151C:
	beq	r0,r0,0040159C
	nop

l00401524:
	lw	r4,004C(r30)
	lw	r5,0044(r30)
	lw	r25,-7F48(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	bne	r2,r0,0040159C
	nop

l00401548:
	addiu	r4,r0,+00000001
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00006134
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00006108
	addiu	r7,r0,+000000E0
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r4,0044(r30)
	lw	r25,-7F84(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop

l0040159C:
	lw	r2,0050(r30)
	nop
	lw	r2,0018(r2)
	nop
	beq	r0,r0,00401468
	sw	r2,0050(r30)

l004015B4:
	lw	r2,004C(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0048(r30)

l004015C8:
	lw	r2,0048(r30)
	nop
	bne	r2,r0,004015E0
	nop

l004015D8:
	beq	r0,r0,00400F10
	nop

l004015E0:
	lw	r2,0048(r30)
	nop
	beq	r2,r0,00401724
	nop

l004015F0:
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	bltz	r2,00401724
	nop

l00401608:
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	srl	r2,r2,05
	sll	r3,r2,02
	addiu	r2,r30,+00000070
	addu	r4,r2,r3
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	andi	r3,r2,0000001F
	lw	r2,0000(r4)
	nop
	srav	r2,r2,r3
	andi	r2,r2,00000001
	beq	r2,r0,00401724
	nop

l00401654:
	lw	r4,0048(r30)
	lw	r25,-7EF8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r2,0048(r30)
	nop
	lw	r2,07D8(r2)
	nop
	bgtz	r2,004016D8
	nop

l00401688:
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	bltz	r2,00401724
	nop

l004016A0:
	lw	r2,0048(r30)
	nop
	lw	r4,0000(r2)
	lw	r25,-7FC4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r3,0048(r30)
	addiu	r2,r0,-00000001
	sw	r2,0000(r3)
	beq	r0,r0,00401724
	nop

l004016D8:
	lw	r2,0048(r30)
	nop
	lw	r2,17C0(r2)
	nop
	sw	r2,0184(r30)
	lw	r4,0048(r30)
	lw	r25,0184(r30)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r4,0048(r30)
	lw	r25,-7EB0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop

l00401724:
	lw	r2,0048(r30)
	nop
	beq	r2,r0,0040184C
	nop

l00401734:
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	bltz	r2,0040184C
	nop

l0040174C:
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	srl	r2,r2,05
	sll	r3,r2,02
	addiu	r2,r30,+00000070
	addu	r4,r2,r3
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	andi	r3,r2,0000001F
	lw	r2,0000(r4)
	nop
	srav	r2,r2,r3
	andi	r2,r2,00000001
	beq	r2,r0,0040184C
	nop

l00401798:
	lw	r4,0048(r30)
	lw	r25,-7E10(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r2,0048(r30)
	nop
	lw	r2,178C(r2)
	nop
	bgtz	r2,0040181C
	nop

l004017CC:
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	bltz	r2,0040184C
	nop

l004017E4:
	lw	r2,0048(r30)
	nop
	lw	r4,0004(r2)
	lw	r25,-7FC4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r3,0048(r30)
	addiu	r2,r0,-00000001
	sw	r2,0004(r3)
	beq	r0,r0,0040184C
	nop

l0040181C:
	lw	r2,0048(r30)
	nop
	lw	r2,17C4(r2)
	nop
	sw	r2,0188(r30)
	lw	r4,0048(r30)
	lw	r25,0188(r30)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop

l0040184C:
	lw	r2,0048(r30)
	nop
	beq	r2,r0,004018F0
	nop

l0040185C:
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	bltz	r2,004018F0
	nop

l00401874:
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	srl	r2,r2,05
	sll	r3,r2,02
	addiu	r2,r30,+000000F0
	addu	r4,r2,r3
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	andi	r3,r2,0000001F
	lw	r2,0000(r4)
	nop
	srav	r2,r2,r3
	andi	r2,r2,00000001
	beq	r2,r0,004018F0
	nop

l004018C0:
	lw	r2,0048(r30)
	nop
	lw	r2,17C4(r2)
	nop
	sw	r2,018C(r30)
	lw	r4,0048(r30)
	lw	r25,018C(r30)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop

l004018F0:
	lw	r2,0048(r30)
	nop
	beq	r2,r0,00401980
	nop

l00401900:
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	bltz	r2,00401980
	nop

l00401918:
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	srl	r2,r2,05
	sll	r3,r2,02
	addiu	r2,r30,+000000F0
	addu	r4,r2,r3
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	andi	r3,r2,0000001F
	lw	r2,0000(r4)
	nop
	srav	r2,r2,r3
	andi	r2,r2,00000001
	beq	r2,r0,00401980
	nop

l00401964:
	lw	r4,0048(r30)
	lw	r25,-7EB0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop

l00401980:
	lw	r2,0048(r30)
	nop
	beq	r2,r0,00401AA4
	nop

l00401990:
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	bgez	r2,004019C0
	nop

l004019A8:
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	bltz	r2,00401A80
	nop

l004019C0:
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	bgez	r2,00401A08
	nop

l004019D8:
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	bltz	r2,00401A08
	nop

l004019F0:
	lw	r2,0048(r30)
	nop
	lw	r2,178C(r2)
	nop
	blez	r2,00401A80
	nop

l00401A08:
	lw	r2,0048(r30)
	nop
	lw	r2,0000(r2)
	nop
	bgez	r2,00401A50
	nop

l00401A20:
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	bltz	r2,00401A50
	nop

l00401A38:
	lw	r2,0048(r30)
	nop
	lw	r2,0FAC(r2)
	nop
	blez	r2,00401A80
	nop

l00401A50:
	lw	r4,0048(r30)
	lw	r5,0170(r30)
	lw	r25,-7FC8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	slti	r2,r2,+00000003
	beq	r2,r0,00401A80
	nop

l00401A78:
	beq	r0,r0,00401AA4
	nop

l00401A80:
	lw	r4,004C(r30)
	lw	r5,0048(r30)
	lw	r25,-7E4C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	sw	r0,0048(r30)

l00401AA4:
	lw	r2,0048(r30)
	nop
	bne	r2,r0,00401ABC
	nop

l00401AB4:
	beq	r0,r0,00400F10
	nop

l00401ABC:
	lw	r2,0048(r30)
	nop
	lw	r2,17B8(r2)
	nop
	beq	r0,r0,004015C8
	sw	r2,0048(r30)

l00401AD4:
	lw	r4,004C(r30)
	lw	r25,-7ED0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r4,0054(r30)
	lw	r25,-7FB8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	lw	r2,0068(r30)
	nop
	beq	r2,r0,00401B50
	nop

l00401B1C:
	lw	r2,0068(r30)
	nop
	lb	r2,0000(r2)
	nop
	beq	r2,r0,00401B50
	nop

l00401B34:
	lw	r4,0068(r30)
	lw	r25,-7ED8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop

l00401B50:
	or	r4,r0,r0
	lw	r25,-7F70(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0038(r30)
	nop
	nop

;; client_new: 00401B70
;;   Called from:
;;     00401B6C (in main)
client_new proc
	lui	r28,%hi(0FC06CF0)
	addiu	r28,r28,%lo(0FC06CF0)
	addu	r28,r28,r25
	addiu	sp,sp,-00000048
	sw	r28,0020(sp)
	sw	ra,0040(sp)
	sw	r30,003C(sp)
	or	r30,sp,r0
	sw	r4,0048(r30)
	sw	r5,004C(r30)
	sw	r6,0050(r30)
	sw	r7,0054(r30)
	addiu	r4,r0,+000017D4
	lw	r25,-7E2C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	sw	r2,002C(r30)
	lw	r2,002C(r30)
	nop
	bne	r2,r0,00401BFC
	sw	r2,002C(r30)

l00401BCC:
	or	r4,r0,r0
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00006100
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop
	beq	r0,r0,00402014
	or	r2,r0,r0

l00401BFC:
	addiu	r2,r0,+00000010
	sw	r2,0028(r30)
	lw	r2,002C(r30)
	nop
	addiu	r2,r2,+00001798
	or	r4,r2,r0
	lw	r5,0028(r30)
	lw	r25,-7E6C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop
	lw	r3,002C(r30)
	addiu	r2,r0,+00000002
	sh	r2,1798(r3)
	lw	r3,002C(r30)
	lw	r2,0048(r30)
	nop
	lhu	r2,0002(r2)
	nop
	sh	r2,179A(r3)
	lw	r3,002C(r30)
	lw	r2,0048(r30)
	nop
	lw	r2,0004(r2)
	nop
	sw	r2,179C(r3)
	lw	r2,002C(r30)
	nop
	addiu	r2,r2,+00001798
	lw	r4,0050(r30)
	or	r5,r2,r0
	addiu	r6,r30,+00000028
	lw	r25,-7E7C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	or	r3,r2,r0
	lw	r2,002C(r30)
	nop
	sw	r3,0000(r2)
	addiu	r2,r0,+00000010
	sw	r2,0028(r30)
	lw	r3,002C(r30)
	addiu	r2,r0,+00000002
	sh	r2,17A8(r3)
	lw	r3,002C(r30)
	lw	r2,004C(r30)
	nop
	lhu	r2,0002(r2)
	nop
	sh	r2,17AA(r3)
	lw	r3,002C(r30)
	lw	r2,004C(r30)
	nop
	lw	r2,0004(r2)
	nop
	sw	r2,17AC(r3)
	lw	r3,002C(r30)
	lw	r2,0054(r30)
	nop
	sw	r2,1790(r3)
	lw	r2,002C(r30)
	nop
	addiu	r2,r2,+000017A8
	or	r4,r2,r0
	lw	r25,-7F7C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	or	r3,r2,r0
	lw	r2,002C(r30)
	nop
	sw	r3,0004(r2)
	lw	r2,002C(r30)
	nop
	lw	r2,0004(r2)
	nop
	bgez	r2,00401DDC
	nop

l00401D48:
	lw	r2,002C(r30)
	nop
	lw	r2,0000(r2)
	nop
	sw	r2,0010(sp)
	addiu	r4,r0,+00000001
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000060D8
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-000060AC
	addiu	r7,r0,+00000037
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop
	lw	r2,002C(r30)
	nop
	lw	r4,0000(r2)
	lw	r25,-7FC4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop
	lw	r4,002C(r30)
	lw	r25,-7FD4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop
	beq	r0,r0,00402014
	or	r2,r0,r0

l00401DDC:
	lw	r2,002C(r30)
	nop
	lw	r4,179C(r2)
	lw	r25,-7F2C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	sw	r2,0010(sp)
	lw	r2,002C(r30)
	nop
	lw	r2,0000(r2)
	nop
	sw	r2,0014(sp)
	lw	r2,002C(r30)
	nop
	lw	r2,0004(r2)
	nop
	sw	r2,0018(sp)
	addiu	r4,r0,+00000003
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000060A0
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-000060AC
	addiu	r7,r0,+0000003D
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop
	lw	r2,002C(r30)
	nop
	sw	r0,07D8(r2)
	lw	r2,002C(r30)
	nop
	sw	r0,0FAC(r2)
	lw	r2,002C(r30)
	nop
	sw	r0,178C(r2)
	lw	r3,002C(r30)
	lw	r2,002C(r30)
	nop
	addiu	r2,r2,+000007DC
	sw	r2,0FB0(r3)
	lw	r3,002C(r30)
	lw	r2,002C(r30)
	nop
	addiu	r2,r2,+00000FB8
	sw	r2,1788(r3)
	lw	r2,002C(r30)
	nop
	sw	r0,1794(r2)
	lw	r2,002C(r30)
	nop
	sw	r0,0FB4(r2)
	lw	r2,002C(r30)
	nop
	sw	r0,17BC(r2)
	lw	r2,002C(r30)
	nop
	sw	r0,17B8(r2)
	or	r4,r0,r0
	lw	r25,-7F30(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	or	r3,r2,r0
	lw	r2,002C(r30)
	nop
	sw	r3,17C8(r2)
	or	r4,r0,r0
	lw	r25,-7F30(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	or	r3,r2,r0
	lw	r2,002C(r30)
	nop
	sw	r3,17CC(r2)
	or	r4,r0,r0
	lw	r25,-7F30(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	or	r3,r2,r0
	lw	r2,002C(r30)
	nop
	sw	r3,17D0(r2)
	lw	r2,0054(r30)
	nop
	sw	r2,0030(r30)
	lw	r3,0030(r30)
	nop
	beq	r3,r0,00401F8C
	nop

l00401F70:
	addiu	r2,r0,+00000001
	lw	r3,0030(r30)
	nop
	beq	r3,r2,00401FB4
	nop

l00401F84:
	beq	r0,r0,0040200C
	nop

l00401F8C:
	lw	r3,002C(r30)
	lw	r2,-7EFC(r28)
	nop
	sw	r2,17C0(r3)
	lw	r3,002C(r30)
	lw	r2,-7F08(r28)
	nop
	sw	r2,17C4(r3)
	beq	r0,r0,0040200C
	nop

l00401FB4:
	lw	r3,002C(r30)
	lw	r2,-7E90(r28)
	nop
	sw	r2,17C0(r3)
	lw	r3,002C(r30)
	lw	r2,-7EBC(r28)
	nop
	sw	r2,17C4(r3)
	lw	r4,002C(r30)
	lw	r25,-7E08(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop
	lw	r4,002C(r30)
	lw	r25,-7EB0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop

l0040200C:
	lw	r2,002C(r30)
	nop

l00402014:
	or	sp,r30,r0
	lw	ra,0040(sp)
	lw	r30,003C(sp)
	jr	ra
	addiu	sp,sp,+00000048

;; client_prepare_connect: 00402028
client_prepare_connect proc
	lui	r28,%hi(0FC06838)
	addiu	r28,r28,%lo(0FC06838)
	addu	r28,r28,r25
	addiu	sp,sp,-00000140
	sw	r28,0020(sp)
	sw	ra,013C(sp)
	sw	r30,0138(sp)
	sw	r16,0134(sp)
	or	r30,sp,r0
	sw	r4,0140(r30)
	addiu	r2,r0,+00000010
	sw	r2,0128(r30)
	lw	r3,0140(r30)
	lw	r2,0140(r30)
	nop
	addiu	r7,r2,+00001798
	addiu	r2,r30,+00000128
	sw	r2,0010(sp)
	lw	r4,0000(r3)
	or	r5,r0,r0
	addiu	r6,r0,+00000050
	lw	r25,-7F5C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop
	lw	r2,0140(r30)
	nop
	lw	r4,179C(r2)
	lw	r25,-7F2C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	or	r16,r2,r0
	lw	r2,0140(r30)
	nop
	lw	r4,179C(r2)
	lw	r25,-7F2C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	or	r3,r2,r0
	lw	r2,0140(r30)
	nop
	lhu	r2,179A(r2)
	nop
	sw	r2,0010(sp)
	sw	r3,0014(sp)
	lw	r2,0140(r30)
	nop
	lhu	r2,179A(r2)
	nop
	sw	r2,0018(sp)
	addiu	r4,r30,+00000028
	addiu	r5,r0,+00000100
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-0000606C
	or	r7,r16,r0
	lw	r25,-7DEC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	sw	r2,0128(r30)
	lw	r2,0128(r30)
	nop
	slti	r2,r2,+00000100
	bne	r2,r0,00402158
	nop

l0040214C:
	sb	r0,0127(r30)
	addiu	r2,r0,+00000100
	sw	r2,0128(r30)

l00402158:
	lw	r2,0140(r30)
	lw	r4,0140(r30)
	lw	r5,0FB0(r2)
	addiu	r6,r30,+00000028
	lw	r7,0128(r30)
	lw	r25,-7E80(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop
	or	r2,r0,r0
	or	sp,r30,r0
	lw	ra,013C(sp)
	lw	r30,0138(sp)
	lw	r16,0134(sp)
	jr	ra
	addiu	sp,sp,+00000140

;; client_copy_request: 004021A0
client_copy_request proc
	lui	r28,%hi(0FC066C0)
	addiu	r28,r28,%lo(0FC066C0)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	ra,0020(sp)
	sw	r30,001C(sp)
	or	r30,sp,r0
	sw	r4,0028(r30)
	lw	r2,0028(r30)
	nop
	addiu	r4,r2,+000007DC
	lw	r2,0028(r30)
	nop
	addiu	r2,r2,+00000008
	lw	r3,0028(r30)
	or	r5,r2,r0
	lw	r6,07D8(r3)
	lw	r25,-7E18(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r3,0028(r30)
	lw	r2,0028(r30)
	nop
	addiu	r2,r2,+000007DC
	sw	r2,0FB0(r3)
	lw	r3,0028(r30)
	lw	r2,0028(r30)
	nop
	lw	r2,07D8(r2)
	nop
	sw	r2,0FAC(r3)
	addiu	r2,r0,+00000001
	or	sp,r30,r0
	lw	ra,0020(sp)
	lw	r30,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028

;; client_parse_request: 00402244
client_parse_request proc
	lui	r28,%hi(0FC0661C)
	addiu	r28,r28,%lo(0FC0661C)
	addu	r28,r28,r25
	addiu	sp,sp,-00000838
	sw	r28,0018(sp)
	sw	ra,0830(sp)
	sw	r30,082C(sp)
	or	r30,sp,r0
	sw	r4,0838(r30)
	sw	r0,0814(r30)
	lw	r2,0838(r30)
	nop
	addiu	r2,r2,+00000008
	sw	r2,07F8(r30)
	lw	r2,0838(r30)
	nop
	lw	r2,07D8(r2)
	nop
	sw	r2,0020(r30)
	lw	r2,0838(r30)
	nop
	addiu	r2,r2,+000007DC
	sw	r2,0810(r30)
	lw	r2,0838(r30)
	nop
	addiu	r2,r2,+00000008
	sw	r2,0804(r30)
	lw	r2,0838(r30)
	nop
	lw	r2,0FB4(r2)
	nop
	sw	r2,0818(r30)

l004022C4:
	lw	r2,0020(r30)
	nop
	bgtz	r2,004022DC
	nop

l004022D4:
	beq	r0,r0,00402A48
	nop

l004022DC:
	lw	r2,0020(r30)
	nop
	sw	r2,080C(r30)
	addiu	r2,r30,+00000028
	or	r4,r2,r0
	lw	r5,07F8(r30)
	addiu	r6,r30,+00000020
	lw	r25,-7FB4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,07FC(r30)
	lw	r3,080C(r30)
	lw	r2,0020(r30)
	nop
	subu	r2,r3,r2
	sw	r2,0808(r30)
	addiu	r2,r30,+00000028
	sw	r2,0800(r30)
	sw	r0,081C(r30)
	lw	r2,0800(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000A
	beq	r3,r2,00402368
	nop

l00402348:
	lw	r2,0800(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000D
	beq	r3,r2,00402368
	nop

l00402360:
	beq	r0,r0,00402370
	nop

l00402368:
	addiu	r2,r0,+00000001
	sw	r2,081C(r30)

l00402370:
	lw	r3,081C(r30)
	nop
	sw	r3,0814(r30)
	lw	r2,0838(r30)
	nop
	lw	r2,1794(r2)
	nop
	sw	r2,0820(r30)
	addiu	r2,r0,+00000001
	lw	r3,0820(r30)
	nop
	beq	r3,r2,00402604
	nop

l004023A4:
	lw	r3,0820(r30)
	nop
	slti	r2,r3,+00000002
	beq	r2,r0,004023D0
	nop

l004023B8:
	lw	r2,0820(r30)
	nop
	beq	r2,r0,00402400
	nop

l004023C8:
	beq	r0,r0,00402A34
	nop

l004023D0:
	addiu	r2,r0,+00000002
	lw	r3,0820(r30)
	nop
	beq	r3,r2,004028C4
	nop

l004023E4:
	addiu	r2,r0,+00000003
	lw	r3,0820(r30)
	nop
	beq	r3,r2,004029C4
	nop

l004023F8:
	beq	r0,r0,00402A34
	nop

l00402400:
	lw	r2,0814(r30)
	nop
	bne	r2,r0,004025D0
	nop

l00402410:
	lw	r2,0838(r30)
	nop
	lw	r2,0FB4(r2)
	nop
	beq	r2,r0,004024FC
	nop

l00402428:
	addiu	r4,r0,+00000004
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005FF8
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005FDC
	addiu	r7,r0,+000000A5
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r2,0838(r30)
	nop
	lw	r2,0FB4(r2)
	nop
	sw	r2,0818(r30)
	addiu	r2,r30,+00000028
	lw	r4,0818(r30)
	or	r5,r2,r0
	lw	r25,-7EE4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	bne	r2,r0,0040251C
	nop

l0040249C:
	addiu	r4,r0,+00000004
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005FC4
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005FDC
	addiu	r7,r0,+000000A9
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r4,0818(r30)
	lw	r25,-7DE0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	sw	r0,0818(r30)
	beq	r0,r0,0040251C
	nop

l004024FC:
	addiu	r2,r30,+00000028
	or	r4,r2,r0
	lw	r25,-7DCC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0818(r30)

l0040251C:
	lw	r2,0838(r30)
	lw	r3,0818(r30)
	nop
	sw	r3,0FB4(r2)
	lw	r2,0818(r30)
	nop
	beq	r2,r0,00402550
	nop

l0040253C:
	lw	r3,0838(r30)
	addiu	r2,r0,+00000001
	sw	r2,1794(r3)
	beq	r0,r0,00402A34
	nop

l00402550:
	addiu	r2,r30,+00000028
	sw	r2,0010(sp)
	addiu	r4,r0,+00000003
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005F8C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005FDC
	addiu	r7,r0,+000000B5
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	addiu	r2,r30,+00000028
	lw	r4,0838(r30)
	lw	r5,0810(r30)
	or	r6,r2,r0
	lw	r7,0808(r30)
	lw	r25,-7E80(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r3,0838(r30)
	addiu	r2,r0,+00000002
	sw	r2,1794(r3)
	beq	r0,r0,00402A34
	nop

l004025D0:
	addiu	r2,r30,+00000028
	lw	r4,0838(r30)
	lw	r5,0810(r30)
	or	r6,r2,r0
	lw	r7,0808(r30)
	lw	r25,-7E80(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00402A34
	nop

l00402604:
	lw	r2,0814(r30)
	nop
	beq	r2,r0,00402660
	nop

l00402614:
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005F4C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005FDC
	addiu	r7,r0,+000000C3
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r2,0838(r30)
	nop
	sw	r0,1794(r2)
	beq	r0,r0,00402A4C
	or	r2,r0,r0

l00402660:
	lw	r2,0808(r30)
	nop
	slti	r2,r2,+00000007
	bne	r2,r0,004027F8
	nop

l00402674:
	addiu	r2,r30,+00000028
	or	r4,r2,r0
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00006044
	addiu	r6,r0,+00000006
	lw	r25,-7E68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	bne	r2,r0,004027F8
	nop

l004026A8:
	lw	r2,0818(r30)
	nop
	bne	r2,r0,004026F8
	nop

l004026B8:
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-0000603C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005FDC
	addiu	r7,r0,+000000CC
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00402A34
	nop

l004026F8:
	addiu	r2,r30,+00000028
	lw	r4,0818(r30)
	or	r5,r2,r0
	lw	r25,-7FAC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r4,0818(r30)
	lw	r25,-7E28(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r2,0818(r30)
	nop
	addiu	r2,r2,+00000214
	or	r4,r2,r0
	lw	r25,-7F94(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r3,r2,r0
	lw	r2,0818(r30)
	nop
	addiu	r2,r2,+00000214
	lw	r4,0838(r30)
	lw	r5,0810(r30)
	or	r6,r2,r0
	or	r7,r3,r0
	lw	r25,-7E80(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0810(r30)
	lw	r4,0818(r30)
	lw	r5,0838(r30)
	lw	r6,0810(r30)
	lw	r25,-7F74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0810(r30)
	addiu	r2,r30,+00000028
	lw	r4,0838(r30)
	lw	r5,0810(r30)
	or	r6,r2,r0
	lw	r7,0808(r30)
	lw	r25,-7E80(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0810(r30)
	lw	r3,0838(r30)
	addiu	r2,r0,+00000002
	sw	r2,1794(r3)
	beq	r0,r0,00402A34
	nop

l004027F8:
	lw	r2,0808(r30)
	nop
	slti	r2,r2,+0000000B
	bne	r2,r0,00402894
	nop

l0040280C:
	addiu	r2,r30,+00000028
	lw	r4,-7FE0(r28)
	nop
	addiu	r4,r4,-00006020
	or	r5,r2,r0
	addiu	r6,r0,+0000000A
	lw	r25,-7E68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	bne	r2,r0,00402894
	nop

l00402840:
	addiu	r2,r30,+00000028
	lw	r4,0818(r30)
	or	r5,r2,r0
	lw	r25,-7E64(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	addiu	r2,r30,+00000028
	lw	r4,0818(r30)
	or	r5,r2,r0
	lw	r6,0808(r30)
	lw	r25,-7F0C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00402A34
	nop

l00402894:
	addiu	r2,r30,+00000028
	lw	r4,0818(r30)
	or	r5,r2,r0
	lw	r6,0808(r30)
	lw	r25,-7F0C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00402A34
	nop

l004028C4:
	addiu	r2,r30,+00000028
	lw	r4,0838(r30)
	lw	r5,0810(r30)
	or	r6,r2,r0
	lw	r7,0808(r30)
	lw	r25,-7E80(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0810(r30)
	lw	r2,0814(r30)
	nop
	beq	r2,r0,00402950
	nop

l00402900:
	lw	r2,0818(r30)
	nop
	beq	r2,r0,0040293C
	nop

l00402910:
	lw	r2,0818(r30)
	nop
	lw	r2,0554(r2)
	nop
	bne	r2,r0,0040293C
	nop

l00402928:
	lw	r2,0838(r30)
	nop
	sw	r0,1794(r2)
	beq	r0,r0,00402A34
	nop

l0040293C:
	lw	r3,0838(r30)
	addiu	r2,r0,+00000003
	sw	r2,1794(r3)
	beq	r0,r0,00402A34
	nop

l00402950:
	lw	r2,0808(r30)
	nop
	slti	r2,r2,+0000000B
	bne	r2,r0,00402A34
	nop

l00402964:
	addiu	r2,r30,+00000028
	lw	r4,-7FE0(r28)
	nop
	addiu	r4,r4,-0000600C
	or	r5,r2,r0
	addiu	r6,r0,+0000000A
	lw	r25,-7E68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	bne	r2,r0,00402A34
	nop

l00402998:
	addiu	r2,r30,+00000028
	lw	r4,0818(r30)
	or	r5,r2,r0
	lw	r25,-7E64(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00402A34
	nop

l004029C4:
	addiu	r2,r30,+00000028
	lw	r4,0838(r30)
	lw	r5,0810(r30)
	or	r6,r2,r0
	lw	r7,0808(r30)
	lw	r25,-7E80(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0810(r30)
	lw	r4,0818(r30)
	lw	r2,0818(r30)
	nop
	lw	r3,0554(r2)
	lw	r2,0808(r30)
	nop
	subu	r2,r3,r2
	sw	r2,0554(r4)
	lw	r2,0818(r30)
	nop
	lw	r2,0554(r2)
	nop
	bne	r2,r0,00402A34
	nop

l00402A28:
	lw	r2,0838(r30)
	nop
	sw	r0,1794(r2)

l00402A34:
	lw	r2,07FC(r30)
	nop
	sw	r2,07F8(r30)
	beq	r0,r0,004022C4
	nop

l00402A48:
	addiu	r2,r0,+00000001

l00402A4C:
	or	sp,r30,r0
	lw	ra,0830(sp)
	lw	r30,082C(sp)
	jr	ra
	addiu	sp,sp,+00000838

;; add_to_request: 00402A60
add_to_request proc
	lui	r28,%hi(0FC05E00)
	addiu	r28,r28,%lo(0FC05E00)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	ra,0020(sp)
	sw	r30,001C(sp)
	or	r30,sp,r0
	sw	r4,0028(r30)
	sw	r5,002C(r30)
	sw	r6,0030(r30)
	sw	r7,0034(r30)
	lw	r2,002C(r30)
	nop
	bne	r2,r0,00402AE0
	nop

l00402AA0:
	addiu	r4,r0,+00000001
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005F1C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005EEC
	addiu	r7,r0,+00000100
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	beq	r0,r0,00402C08
	or	r2,r0,r0

l00402AE0:
	lw	r2,0030(r30)
	nop
	beq	r2,r0,00402B08
	nop

l00402AF0:
	lw	r2,0034(r30)
	nop
	blez	r2,00402B08
	nop

l00402B00:
	beq	r0,r0,00402B4C
	nop

l00402B08:
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005EDC
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005EEC
	addiu	r7,r0,+00000105
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r2,002C(r30)
	beq	r0,r0,00402C08
	nop

l00402B4C:
	lw	r4,0028(r30)
	lw	r2,0028(r30)
	nop
	lw	r3,0FAC(r2)
	lw	r2,0034(r30)
	nop
	addu	r2,r3,r2
	sw	r2,0FAC(r4)
	lw	r2,0028(r30)
	nop
	lw	r2,0FAC(r2)
	nop
	slti	r2,r2,+000007D0
	beq	r2,r0,00402BC0
	nop

l00402B88:
	lw	r4,002C(r30)
	lw	r5,0030(r30)
	lw	r6,0034(r30)
	lw	r25,-7E18(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r2,002C(r30)
	lw	r3,0034(r30)
	nop
	beq	r0,r0,00402C08
	addu	r2,r2,r3

l00402BC0:
	addiu	r4,r0,+00000001
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005EB8
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005EEC
	addiu	r7,r0,+0000010F
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r2,0028(r30)
	nop
	sw	r0,0FAC(r2)
	or	r2,r0,r0

l00402C08:
	or	sp,r30,r0
	lw	ra,0020(sp)
	lw	r30,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028

;; get_a_line: 00402C1C
get_a_line proc
	lui	r28,%hi(0FC05C44)
	addiu	r28,r28,%lo(0FC05C44)
	addu	r28,r28,r25
	addiu	sp,sp,-00000010
	sw	r28,0000(sp)
	sw	r30,000C(sp)
	or	r30,sp,r0
	sw	r4,0010(r30)
	sw	r5,0014(r30)
	sw	r6,0018(r30)

l00402C44:
	lw	r2,0018(r30)
	nop
	lw	r2,0000(r2)
	nop
	blez	r2,00402CE8
	nop

l00402C5C:
	lw	r2,0014(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000A
	beq	r3,r2,00402CE8
	nop

l00402C74:
	lw	r2,0014(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000D
	bne	r3,r2,00402C94
	nop

l00402C8C:
	beq	r0,r0,00402CE8
	nop

l00402C94:
	lw	r4,0010(r30)
	addiu	r5,r30,+00000014
	lw	r2,0000(r5)
	nop
	lbu	r3,0000(r2)
	nop
	sb	r3,0000(r4)
	addiu	r2,r2,+00000001
	sw	r2,0000(r5)
	addiu	r4,r4,+00000001
	sw	r4,0010(r30)
	lw	r2,0018(r30)
	lw	r3,0018(r30)
	lw	r2,0018(r30)
	nop
	lw	r2,0000(r2)
	nop
	addiu	r2,r2,-00000001
	sw	r2,0000(r3)
	beq	r0,r0,00402C44
	nop

l00402CE8:
	lw	r2,0018(r30)
	nop
	lw	r2,0000(r2)
	nop
	blez	r2,00402D84
	nop

l00402D00:
	lw	r2,0014(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000A
	beq	r3,r2,00402D38
	nop

l00402D18:
	lw	r2,0014(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000D
	beq	r3,r2,00402D38
	nop

l00402D30:
	beq	r0,r0,00402D84
	nop

l00402D38:
	lw	r4,0010(r30)
	addiu	r5,r30,+00000014
	lw	r2,0000(r5)
	nop
	lbu	r3,0000(r2)
	nop
	sb	r3,0000(r4)
	addiu	r2,r2,+00000001
	sw	r2,0000(r5)
	addiu	r4,r4,+00000001
	sw	r4,0010(r30)
	lw	r2,0018(r30)
	lw	r3,0018(r30)
	lw	r2,0018(r30)
	nop
	lw	r2,0000(r2)
	nop
	addiu	r2,r2,-00000001
	sw	r2,0000(r3)

l00402D84:
	lw	r2,0018(r30)
	nop
	lw	r2,0000(r2)
	nop
	blez	r2,00402E20
	nop

l00402D9C:
	lw	r2,0014(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000A
	beq	r3,r2,00402DD4
	nop

l00402DB4:
	lw	r2,0014(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000D
	beq	r3,r2,00402DD4
	nop

l00402DCC:
	beq	r0,r0,00402E20
	nop

l00402DD4:
	lw	r4,0010(r30)
	addiu	r5,r30,+00000014
	lw	r2,0000(r5)
	nop
	lbu	r3,0000(r2)
	nop
	sb	r3,0000(r4)
	addiu	r2,r2,+00000001
	sw	r2,0000(r5)
	addiu	r4,r4,+00000001
	sw	r4,0010(r30)
	lw	r2,0018(r30)
	lw	r3,0018(r30)
	lw	r2,0018(r30)
	nop
	lw	r2,0000(r2)
	nop
	addiu	r2,r2,-00000001
	sw	r2,0000(r3)

l00402E20:
	lw	r2,0010(r30)
	nop
	sb	r0,0000(r2)
	lw	r2,0014(r30)
	nop
	or	sp,r30,r0
	lw	r30,000C(sp)
	jr	ra
	addiu	sp,sp,+00000010

;; client_read_request: 00402E44
client_read_request proc
	lui	r28,%hi(0FC05A1C)
	addiu	r28,r28,%lo(0FC05A1C)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	ra,0020(sp)
	sw	r30,001C(sp)
	or	r30,sp,r0
	sw	r4,0028(r30)
	or	r4,r0,r0
	lw	r25,-7F30(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	or	r3,r2,r0
	lw	r2,0028(r30)
	nop
	sw	r3,17CC(r2)
	lw	r3,0028(r30)
	lw	r2,0028(r30)
	nop
	addiu	r2,r2,+00000008
	lw	r4,0000(r3)
	or	r5,r2,r0
	addiu	r6,r0,+000006D0
	addiu	r7,r0,+00004000
	lw	r25,-7DD4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	or	r3,r2,r0
	lw	r2,0028(r30)
	nop
	sw	r3,07D8(r2)
	lw	r2,0028(r30)
	nop
	lw	r2,07D8(r2)
	nop
	or	sp,r30,r0
	lw	ra,0020(sp)
	lw	r30,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028

;; client_send_request: 00402EF8
client_send_request proc
	lui	r28,%hi(0FC05968)
	addiu	r28,r28,%lo(0FC05968)
	addu	r28,r28,r25
	addiu	sp,sp,-00000038
	sw	r28,0018(sp)
	sw	ra,0030(sp)
	sw	r30,002C(sp)
	or	r30,sp,r0
	sw	r4,0038(r30)
	sw	r0,0020(r30)
	or	r4,r0,r0
	lw	r25,-7F30(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r3,r2,r0
	lw	r2,0038(r30)
	nop
	sw	r3,17CC(r2)
	lw	r2,0038(r30)
	nop
	lw	r2,0FAC(r2)
	nop
	bgtz	r2,00402F68
	nop

l00402F60:
	beq	r0,r0,00403068
	or	r2,r0,r0

l00402F68:
	lw	r2,0038(r30)
	lw	r3,0038(r30)
	lw	r6,0038(r30)
	lw	r4,0004(r2)
	lw	r5,0FB0(r3)
	lw	r6,0FAC(r6)
	or	r7,r0,r0
	lw	r25,-7E70(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0020(r30)
	lw	r2,0038(r30)
	lw	r3,0020(r30)
	lw	r2,0FAC(r2)
	nop
	bne	r3,r2,00402FDC
	nop

l00402FB4:
	lw	r2,0038(r30)
	nop
	sw	r0,0FAC(r2)
	lw	r3,0038(r30)
	lw	r2,0038(r30)
	nop
	addiu	r2,r2,+000007DC
	sw	r2,0FB0(r3)
	beq	r0,r0,00403060
	nop

l00402FDC:
	lw	r4,0038(r30)
	lw	r2,0038(r30)
	nop
	lw	r3,0FB0(r2)
	lw	r2,0020(r30)
	nop
	addu	r2,r3,r2
	sw	r2,0FB0(r4)
	lw	r4,0038(r30)
	lw	r2,0038(r30)
	nop
	lw	r3,0FAC(r2)
	lw	r2,0020(r30)
	nop
	subu	r2,r3,r2
	sw	r2,0FAC(r4)
	lw	r2,0020(r30)
	nop
	sw	r2,0010(sp)
	addiu	r4,r0,+00000004
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005E8C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005E6C
	addiu	r7,r0,+0000014D
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop

l00403060:
	lw	r2,0020(r30)
	nop

l00403068:
	or	sp,r30,r0
	lw	ra,0030(sp)
	lw	r30,002C(sp)
	jr	ra
	addiu	sp,sp,+00000038

;; client_read_reply: 0040307C
client_read_reply proc
	lui	r28,%hi(0FC057E4)
	addiu	r28,r28,%lo(0FC057E4)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	ra,0020(sp)
	sw	r30,001C(sp)
	or	r30,sp,r0
	sw	r4,0028(r30)
	or	r4,r0,r0
	lw	r25,-7F30(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	or	r3,r2,r0
	lw	r2,0028(r30)
	nop
	sw	r3,17D0(r2)
	lw	r3,0028(r30)
	lw	r2,0028(r30)
	nop
	addiu	r2,r2,+00000FB8
	lw	r4,0004(r3)
	or	r5,r2,r0
	addiu	r6,r0,+000007CF
	lw	r25,-7EC4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	or	r3,r2,r0
	lw	r2,0028(r30)
	nop
	sw	r3,178C(r2)
	lw	r3,0028(r30)
	lw	r2,0028(r30)
	nop
	addiu	r2,r2,+00000FB8
	sw	r2,1788(r3)
	lw	r2,0028(r30)
	nop
	lw	r2,178C(r2)
	nop
	or	sp,r30,r0
	lw	ra,0020(sp)
	lw	r30,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028

;; client_parse_reply: 00403140
client_parse_reply proc
	lui	r28,%hi(0FC05720)
	addiu	r28,r28,%lo(0FC05720)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	ra,0020(sp)
	sw	r30,001C(sp)
	or	r30,sp,r0
	sw	r4,0028(r30)
	lw	r2,0028(r30)
	nop
	lw	r2,178C(r2)
	nop
	bgtz	r2,00403184
	nop

l0040317C:
	beq	r0,r0,004031C8
	or	r2,r0,r0

l00403184:
	lw	r4,0028(r30)
	lw	r25,-7DC8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	beq	r2,r0,004031AC
	nop

l004031A4:
	beq	r0,r0,004031C8
	or	r2,r0,r0

l004031AC:
	lw	r4,0028(r30)
	lw	r25,-7F08(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop

l004031C8:
	or	sp,r30,r0
	lw	ra,0020(sp)
	lw	r30,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028

;; client_copy_reply: 004031DC
client_copy_reply proc
	lui	r28,%hi(0FC05684)
	addiu	r28,r28,%lo(0FC05684)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,178C(r2)
	nop
	bgtz	r2,00403220
	nop

l00403218:
	beq	r0,r0,00403304
	or	r2,r0,r0

l00403220:
	or	r4,r0,r0
	lw	r25,-7F30(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	or	r3,r2,r0
	lw	r2,0030(r30)
	nop
	sw	r3,17D0(r2)
	lw	r2,0030(r30)
	lw	r3,0030(r30)
	lw	r6,0030(r30)
	lw	r4,0000(r2)
	lw	r5,1788(r3)
	lw	r6,178C(r6)
	or	r7,r0,r0
	lw	r25,-7E70(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,0018(r30)
	lw	r2,0030(r30)
	lw	r3,0018(r30)
	lw	r2,178C(r2)
	nop
	bne	r3,r2,004032BC
	nop

l00403294:
	lw	r3,0030(r30)
	lw	r2,0030(r30)
	nop
	addiu	r2,r2,+00000FB8
	sw	r2,1788(r3)
	lw	r2,0030(r30)
	nop
	sw	r0,178C(r2)
	beq	r0,r0,004032FC
	nop

l004032BC:
	lw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r3,1788(r2)
	lw	r2,0018(r30)
	nop
	addu	r2,r3,r2
	sw	r2,1788(r4)
	lw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r3,178C(r2)
	lw	r2,0018(r30)
	nop
	subu	r2,r3,r2
	sw	r2,178C(r4)

l004032FC:
	lw	r2,0018(r30)
	nop

l00403304:
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; client_check_reply_http: 00403318
client_check_reply_http proc
	lui	r28,%hi(0FC05548)
	addiu	r28,r28,%lo(0FC05548)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,1788(r2)
	nop
	sw	r2,0018(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,178C(r2)
	nop
	sw	r2,001C(r30)

l00403364:
	lw	r2,001C(r30)
	nop
	blez	r2,004033D0
	nop

l00403374:
	lw	r2,0018(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000A
	beq	r3,r2,004033AC
	nop

l0040338C:
	lw	r2,0018(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000D
	beq	r3,r2,004033AC
	nop

l004033A4:
	beq	r0,r0,004033D0
	nop

l004033AC:
	lw	r2,0018(r30)
	nop
	addiu	r2,r2,+00000001
	sw	r2,0018(r30)
	lw	r2,001C(r30)
	nop
	addiu	r2,r2,-00000001
	beq	r0,r0,00403364
	sw	r2,001C(r30)

l004033D0:
	lw	r2,001C(r30)
	nop
	slti	r2,r2,+00000005
	bne	r2,r0,004036A4
	nop

l004033E4:
	lw	r4,0018(r30)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005E58
	addiu	r6,r0,+00000004
	lw	r25,-7E68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	bne	r2,r0,004036A4
	nop

l00403414:
	lw	r3,0030(r30)
	lw	r2,-7F08(r28)
	nop
	sw	r2,17C4(r3)

l00403424:
	lw	r2,001C(r30)
	nop
	bgtz	r2,0040345C
	nop

l00403434:
	lw	r3,0030(r30)
	lw	r2,0030(r30)
	nop
	addiu	r2,r2,+00000FB8
	sw	r2,1788(r3)
	lw	r2,0030(r30)
	nop
	sw	r0,178C(r2)
	beq	r0,r0,004036A8
	addiu	r2,r0,+00000001

l0040345C:
	lw	r2,001C(r30)
	nop
	blez	r2,004034CC
	nop

l0040346C:
	lw	r2,0018(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000A
	beq	r3,r2,004034CC
	nop

l00403484:
	lw	r2,0018(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000D
	bne	r3,r2,004034A4
	nop

l0040349C:
	beq	r0,r0,004034CC
	nop

l004034A4:
	lw	r2,001C(r30)
	nop
	addiu	r2,r2,-00000001
	sw	r2,001C(r30)
	lw	r2,0018(r30)
	nop
	addiu	r2,r2,+00000001
	sw	r2,0018(r30)
	beq	r0,r0,0040345C
	nop

l004034CC:
	lw	r2,001C(r30)
	nop
	slti	r2,r2,+00000002
	bne	r2,r0,00403424
	nop

l004034E0:
	lw	r2,0018(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000A
	beq	r3,r2,00403518
	nop

l004034F8:
	lw	r2,0018(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000D
	beq	r3,r2,00403518
	nop

l00403510:
	beq	r0,r0,00403424
	nop

l00403518:
	lw	r2,0018(r30)
	nop
	addiu	r2,r2,+00000001
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000A
	beq	r3,r2,00403558
	nop

l00403534:
	lw	r2,0018(r30)
	nop
	addiu	r2,r2,+00000001
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000D
	beq	r3,r2,00403558
	nop

l00403550:
	beq	r0,r0,00403424
	nop

l00403558:
	lw	r2,0018(r30)
	nop
	addiu	r2,r2,+00000001
	sw	r2,0018(r30)
	lw	r2,0018(r30)
	nop
	addiu	r2,r2,+00000001
	sw	r2,0018(r30)
	lw	r2,001C(r30)
	nop
	addiu	r2,r2,-00000002
	sw	r2,001C(r30)
	lw	r2,001C(r30)
	nop
	slti	r2,r2,+00000002
	bne	r2,r0,00403424
	nop

l0040359C:
	lw	r2,0018(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000A
	beq	r3,r2,004035D4
	nop

l004035B4:
	lw	r2,0018(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000D
	beq	r3,r2,004035D4
	nop

l004035CC:
	beq	r0,r0,00403424
	nop

l004035D4:
	lw	r2,0018(r30)
	nop
	addiu	r2,r2,+00000001
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000A
	beq	r3,r2,00403614
	nop

l004035F0:
	lw	r2,0018(r30)
	nop
	addiu	r2,r2,+00000001
	lb	r3,0000(r2)
	addiu	r2,r0,+0000000D
	beq	r3,r2,00403614
	nop

l0040360C:
	beq	r0,r0,00403424
	nop

l00403614:
	lw	r2,0018(r30)
	nop
	addiu	r2,r2,+00000001
	sw	r2,0018(r30)
	lw	r2,0018(r30)
	nop
	addiu	r2,r2,+00000001
	sw	r2,0018(r30)
	lw	r2,001C(r30)
	nop
	addiu	r2,r2,-00000002
	sw	r2,001C(r30)
	lw	r2,001C(r30)
	nop
	blez	r2,0040367C
	nop

l00403654:
	lw	r3,0030(r30)
	lw	r2,0018(r30)
	nop
	sw	r2,1788(r3)
	lw	r3,0030(r30)
	lw	r2,001C(r30)
	nop
	sw	r2,178C(r3)
	beq	r0,r0,004036A8
	or	r2,r0,r0

l0040367C:
	lw	r3,0030(r30)
	lw	r2,0030(r30)
	nop
	addiu	r2,r2,+00000FB8
	sw	r2,1788(r3)
	lw	r2,0030(r30)
	nop
	sw	r0,178C(r2)
	beq	r0,r0,004036A8
	addiu	r2,r0,+00000001

l004036A4:
	or	r2,r0,r0

l004036A8:
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; client_close: 004036BC
client_close proc
	lui	r28,%hi(0FC051A4)
	addiu	r28,r28,%lo(0FC051A4)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	ra,0020(sp)
	sw	r30,001C(sp)
	or	r30,sp,r0
	sw	r4,0028(r30)
	lw	r2,0028(r30)
	nop
	beq	r2,r0,00403780
	nop

l004036F0:
	lw	r2,0028(r30)
	nop
	lw	r2,0000(r2)
	nop
	bltz	r2,00403738
	nop

l00403708:
	lw	r2,0028(r30)
	nop
	lw	r4,0000(r2)
	lw	r25,-7FC4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r3,0028(r30)
	addiu	r2,r0,-00000001
	sw	r2,0000(r3)

l00403738:
	lw	r2,0028(r30)
	nop
	lw	r2,0004(r2)
	nop
	bltz	r2,00403780
	nop

l00403750:
	lw	r2,0028(r30)
	nop
	lw	r4,0004(r2)
	lw	r25,-7FC4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r3,0028(r30)
	addiu	r2,r0,-00000001
	sw	r2,0004(r3)

l00403780:
	or	sp,r30,r0
	lw	ra,0020(sp)
	lw	r30,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028

;; client_destroy: 00403794
client_destroy proc
	lui	r28,%hi(0FC050CC)
	addiu	r28,r28,%lo(0FC050CC)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	ra,0020(sp)
	sw	r30,001C(sp)
	or	r30,sp,r0
	sw	r4,0028(r30)
	lw	r4,0028(r30)
	lw	r25,-7FA4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r2,0028(r30)
	nop
	lw	r2,0FB4(r2)
	nop
	beq	r2,r0,00403810
	nop

l004037EC:
	lw	r2,0028(r30)
	nop
	lw	r4,0FB4(r2)
	lw	r25,-7DE0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop

l00403810:
	lw	r4,0028(r30)
	lw	r25,-7FD4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	or	sp,r30,r0
	lw	ra,0020(sp)
	lw	r30,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028

;; client_check_activ: 00403840
client_check_activ proc
	lui	r28,%hi(0FC05020)
	addiu	r28,r28,%lo(0FC05020)
	addu	r28,r28,r25
	addiu	sp,sp,-00000040
	sw	r28,0020(sp)
	sw	ra,0038(sp)
	sw	r30,0034(sp)
	or	r30,sp,r0
	sw	r4,0040(r30)
	sw	r5,0044(r30)
	sw	r0,0028(r30)
	lw	r2,0044(r30)
	nop
	bgez	r2,00403898
	nop

l0040387C:
	or	r4,r0,r0
	lw	r25,-7F30(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	sw	r2,0044(r30)

l00403898:
	lw	r2,0040(r30)
	lw	r3,0044(r30)
	lw	r2,17CC(r2)
	nop
	subu	r3,r3,r2
	lw	r2,-7F54(r28)
	nop
	lw	r2,0000(r2)
	nop
	slt	r2,r2,r3
	beq	r2,r0,00403948
	nop

l004038C8:
	lw	r2,0040(r30)
	nop
	lw	r2,0000(r2)
	nop
	sw	r2,0010(sp)
	lw	r2,0040(r30)
	nop
	lw	r2,0004(r2)
	nop
	sw	r2,0014(sp)
	lw	r2,0040(r30)
	lw	r3,0044(r30)
	lw	r2,17CC(r2)
	nop
	subu	r2,r3,r2
	sw	r2,0018(sp)
	addiu	r4,r0,+00000003
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005E50
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005E20
	addiu	r7,r0,+000001D3
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop
	addiu	r2,r0,+00000001
	sw	r2,0028(r30)

l00403948:
	lw	r2,0040(r30)
	lw	r3,0044(r30)
	lw	r2,17D0(r2)
	nop
	subu	r3,r3,r2
	lw	r2,-7F54(r28)
	nop
	lw	r2,0000(r2)
	nop
	slt	r2,r2,r3
	beq	r2,r0,00403A00
	nop

l00403978:
	lw	r2,0040(r30)
	nop
	lw	r2,0000(r2)
	nop
	sw	r2,0010(sp)
	lw	r2,0040(r30)
	nop
	lw	r2,0004(r2)
	nop
	sw	r2,0014(sp)
	lw	r2,0040(r30)
	lw	r3,0044(r30)
	lw	r2,17D0(r2)
	nop
	subu	r2,r3,r2
	sw	r2,0018(sp)
	addiu	r4,r0,+00000003
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005E0C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005E20
	addiu	r7,r0,+000001D9
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop
	lw	r2,0028(r30)
	nop
	addiu	r2,r2,+00000002
	sw	r2,0028(r30)

l00403A00:
	lw	r2,0028(r30)
	nop
	or	sp,r30,r0
	lw	ra,0038(sp)
	lw	r30,0034(sp)
	jr	ra
	addiu	sp,sp,+00000040

;; open_destination: 00403A1C
open_destination proc
	lui	r28,%hi(0FC04E44)
	addiu	r28,r28,%lo(0FC04E44)
	addu	r28,r28,r25
	addiu	sp,sp,-00000038
	sw	r28,0018(sp)
	sw	ra,0034(sp)
	sw	r30,0030(sp)
	sw	r16,002C(sp)
	or	r30,sp,r0
	sw	r4,0038(r30)
	addiu	r4,r0,+00000002
	addiu	r5,r0,+00000002
	addiu	r6,r0,+00000006
	lw	r25,-7E44(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0020(r30)
	lw	r2,0020(r30)
	nop
	bgez	r2,00403B10
	nop

l00403A78:
	lw	r25,-7F68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r16,r2,r0
	lw	r25,-7F68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	lw	r4,0000(r2)
	lw	r25,-7DE4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r3,r2,r0
	lw	r2,0000(r16)
	nop
	sw	r2,0010(sp)
	sw	r3,0014(sp)
	addiu	r4,r0,+00000001
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005DDC
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005DAC
	addiu	r7,r0,+000001E9
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00403BDC
	addiu	r2,r0,-00000001

l00403B10:
	lw	r2,0038(r30)
	lw	r4,0020(r30)
	or	r5,r2,r0
	addiu	r6,r0,+00000010
	lw	r25,-7DD8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	bgez	r2,00403BD4
	nop

l00403B3C:
	lw	r25,-7F68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r16,r2,r0
	lw	r25,-7F68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	lw	r4,0000(r2)
	lw	r25,-7DE4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r3,r2,r0
	lw	r2,0000(r16)
	nop
	sw	r2,0010(sp)
	sw	r3,0014(sp)
	addiu	r4,r0,+00000001
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005D98
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005DAC
	addiu	r7,r0,+000001F0
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00403BDC
	addiu	r2,r0,-00000001

l00403BD4:
	lw	r2,0020(r30)
	nop

l00403BDC:
	or	sp,r30,r0
	lw	ra,0034(sp)
	lw	r30,0030(sp)
	lw	r16,002C(sp)
	jr	ra
	addiu	sp,sp,+00000038
00403BF4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; clist_new: 00403C00
clist_new proc
	lui	r28,%hi(0FC04C60)
	addiu	r28,r28,%lo(0FC04C60)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	addiu	r4,r0,+0000000C
	lw	r25,-7E2C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,0018(r30)
	lw	r2,0018(r30)
	nop
	bne	r2,r0,00403C7C
	sw	r2,0018(r30)

l00403C4C:
	or	r4,r0,r0
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005D60
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	beq	r0,r0,00403CA8
	or	r2,r0,r0

l00403C7C:
	lw	r2,0018(r30)
	nop
	sw	r0,0008(r2)
	lw	r2,0018(r30)
	nop
	sw	r0,0000(r2)
	lw	r2,0018(r30)
	nop
	sw	r0,0004(r2)
	lw	r2,0018(r30)
	nop

l00403CA8:
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; clist_add: 00403CBC
clist_add proc
	lui	r28,%hi(0FC04BA4)
	addiu	r28,r28,%lo(0FC04BA4)
	addu	r28,r28,r25
	addiu	sp,sp,-00000010
	sw	r28,0000(sp)
	sw	r30,000C(sp)
	or	r30,sp,r0
	sw	r4,0010(r30)
	sw	r5,0014(r30)
	lw	r2,0010(r30)
	nop
	lw	r2,0008(r2)
	nop
	beq	r2,r0,00403D28
	nop

l00403CF8:
	lw	r2,0010(r30)
	nop
	lw	r3,0008(r2)
	lw	r2,0014(r30)
	nop
	sw	r2,17BC(r3)
	lw	r3,0014(r30)
	lw	r2,0010(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,17B8(r3)

l00403D28:
	lw	r2,0014(r30)
	nop
	sw	r0,17BC(r2)
	lw	r3,0010(r30)
	lw	r2,0014(r30)
	nop
	sw	r2,0008(r3)
	lw	r2,0010(r30)
	lw	r3,0010(r30)
	lw	r2,0010(r30)
	nop
	lw	r2,0000(r2)
	nop
	addiu	r2,r2,+00000001
	sw	r2,0000(r3)
	lw	r2,0010(r30)
	lw	r3,0010(r30)
	lw	r4,0000(r2)
	lw	r2,0004(r3)
	nop
	slt	r2,r2,r4
	beq	r2,r0,00403D9C
	nop

l00403D84:
	lw	r2,0010(r30)
	lw	r3,0010(r30)
	nop
	lw	r3,0000(r3)
	nop
	sw	r3,0004(r2)

l00403D9C:
	lw	r2,0010(r30)
	nop
	lw	r2,0000(r2)
	nop
	or	sp,r30,r0
	lw	r30,000C(sp)
	jr	ra
	addiu	sp,sp,+00000010

;; clist_remove: 00403DBC
clist_remove proc
	lui	r28,%hi(0FC04AA4)
	addiu	r28,r28,%lo(0FC04AA4)
	addu	r28,r28,r25
	addiu	sp,sp,-00000010
	sw	r28,0000(sp)
	sw	r30,000C(sp)
	or	r30,sp,r0
	sw	r4,0010(r30)
	sw	r5,0014(r30)
	lw	r2,0014(r30)
	nop
	beq	r2,r0,00403EA0
	nop

l00403DF0:
	lw	r2,0010(r30)
	lw	r3,0014(r30)
	lw	r2,0008(r2)
	nop
	bne	r3,r2,00403E28
	nop

l00403E08:
	lw	r2,0010(r30)
	lw	r3,0014(r30)
	nop
	lw	r3,17B8(r3)
	nop
	sw	r3,0008(r2)
	beq	r0,r0,00403E48
	nop

l00403E28:
	lw	r2,0014(r30)
	nop
	lw	r3,17BC(r2)
	lw	r2,0014(r30)
	nop
	lw	r2,17B8(r2)
	nop
	sw	r2,17B8(r3)

l00403E48:
	lw	r2,0014(r30)
	nop
	lw	r2,17B8(r2)
	nop
	beq	r2,r0,00403E80
	nop

l00403E60:
	lw	r2,0014(r30)
	nop
	lw	r3,17B8(r2)
	lw	r2,0014(r30)
	nop
	lw	r2,17BC(r2)
	nop
	sw	r2,17BC(r3)

l00403E80:
	lw	r2,0010(r30)
	lw	r3,0010(r30)
	lw	r2,0010(r30)
	nop
	lw	r2,0000(r2)
	nop
	addiu	r2,r2,-00000001
	sw	r2,0000(r3)

l00403EA0:
	lw	r2,0010(r30)
	nop
	lw	r2,0000(r2)
	nop
	or	sp,r30,r0
	lw	r30,000C(sp)
	jr	ra
	addiu	sp,sp,+00000010

;; clist_find_fdin: 00403EC0
clist_find_fdin proc
	lui	r28,%hi(0FC049A0)
	addiu	r28,r28,%lo(0FC049A0)
	addu	r28,r28,r25
	addiu	sp,sp,-00000018
	sw	r28,0000(sp)
	sw	r30,0014(sp)
	or	r30,sp,r0
	sw	r4,0018(r30)
	sw	r5,001C(r30)
	lw	r2,0018(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0008(r30)

l00403EF8:
	lw	r2,0008(r30)
	nop
	beq	r2,r0,00403F44
	nop

l00403F08:
	lw	r2,0008(r30)
	nop
	lw	r3,0000(r2)
	lw	r2,001C(r30)
	nop
	bne	r3,r2,00403F2C
	nop

l00403F24:
	beq	r0,r0,00403F44
	nop

l00403F2C:
	lw	r2,0008(r30)
	nop
	lw	r2,17B8(r2)
	nop
	beq	r0,r0,00403EF8
	sw	r2,0008(r30)

l00403F44:
	lw	r2,0008(r30)
	nop
	or	sp,r30,r0
	lw	r30,0014(sp)
	jr	ra
	addiu	sp,sp,+00000018

;; clist_find_fdout: 00403F5C
clist_find_fdout proc
	lui	r28,%hi(0FC04904)
	addiu	r28,r28,%lo(0FC04904)
	addu	r28,r28,r25
	addiu	sp,sp,-00000018
	sw	r28,0000(sp)
	sw	r30,0014(sp)
	or	r30,sp,r0
	sw	r4,0018(r30)
	sw	r5,001C(r30)
	lw	r2,0018(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0008(r30)

l00403F94:
	lw	r2,0008(r30)
	nop
	beq	r2,r0,00403FE0
	nop

l00403FA4:
	lw	r2,0008(r30)
	nop
	lw	r3,0004(r2)
	lw	r2,001C(r30)
	nop
	bne	r3,r2,00403FC8
	nop

l00403FC0:
	beq	r0,r0,00403FE0
	nop

l00403FC8:
	lw	r2,0008(r30)
	nop
	lw	r2,17B8(r2)
	nop
	beq	r0,r0,00403F94
	sw	r2,0008(r30)

l00403FE0:
	lw	r2,0008(r30)
	nop
	or	sp,r30,r0
	lw	r30,0014(sp)
	jr	ra
	addiu	sp,sp,+00000018

;; clist_delete: 00403FF8
clist_delete proc
	lui	r28,%hi(0FC04868)
	addiu	r28,r28,%lo(0FC04868)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	ra,0020(sp)
	sw	r30,001C(sp)
	or	r30,sp,r0
	sw	r4,0028(r30)
	sw	r5,002C(r30)
	lw	r4,0028(r30)
	lw	r5,002C(r30)
	lw	r25,-7E1C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r4,002C(r30)
	lw	r25,-7F84(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	or	sp,r30,r0
	lw	ra,0020(sp)
	lw	r30,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028

;; clist_close_all: 00404070
clist_close_all proc
	lui	r28,%hi(0FC047F0)
	addiu	r28,r28,%lo(0FC047F0)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0018(r30)

l004040A8:
	lw	r2,0018(r30)
	nop
	bne	r2,r0,004040C0
	nop

l004040B8:
	beq	r0,r0,004040F4
	nop

l004040C0:
	lw	r4,0018(r30)
	lw	r25,-7FA4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r2,0018(r30)
	nop
	lw	r2,17B8(r2)
	nop
	beq	r0,r0,004040A8
	sw	r2,0018(r30)

l004040F4:
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; clist_destroy_all: 00404108
clist_destroy_all proc
	lui	r28,%hi(0FC04758)
	addiu	r28,r28,%lo(0FC04758)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0018(r30)

l00404140:
	lw	r2,0030(r30)
	nop
	lw	r2,0008(r2)
	nop
	bne	r2,r0,00404160
	nop

l00404158:
	beq	r0,r0,0040419C
	nop

l00404160:
	lw	r2,0030(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0018(r30)
	lw	r4,0030(r30)
	lw	r5,0018(r30)
	lw	r25,-7E4C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	beq	r0,r0,00404140
	nop

l0040419C:
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; clist_destroy: 004041B0
clist_destroy proc
	lui	r28,%hi(0FC046B0)
	addiu	r28,r28,%lo(0FC046B0)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	ra,0020(sp)
	sw	r30,001C(sp)
	or	r30,sp,r0
	sw	r4,0028(r30)
	lw	r4,0028(r30)
	lw	r25,-7F38(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r4,0028(r30)
	lw	r25,-7F34(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r4,0028(r30)
	lw	r25,-7FD4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	or	sp,r30,r0
	lw	ra,0020(sp)
	lw	r30,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028
0040423C                                     00 00 00 00             ....

;; request_new: 00404240
request_new proc
	lui	r28,%hi(0FC04620)
	addiu	r28,r28,%lo(0FC04620)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	addiu	r4,r0,+0000055C
	lw	r25,-7E2C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,0018(r30)
	lw	r2,0018(r30)
	nop
	bne	r2,r0,004042C0
	sw	r2,0018(r30)

l00404290:
	or	r4,r0,r0
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005CF8
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	beq	r0,r0,00404364
	or	r2,r0,r0

l004042C0:
	lw	r2,0018(r30)
	nop
	sb	r0,0000(r2)
	lw	r2,0018(r30)
	nop
	sb	r0,0114(r2)
	lw	r2,0018(r30)
	nop
	sb	r0,0314(r2)
	lw	r2,0018(r30)
	nop
	sb	r0,0214(r2)
	lw	r2,0018(r30)
	nop
	sb	r0,0014(r2)
	lw	r2,0018(r30)
	nop
	sw	r0,0414(r2)
	lw	r3,0018(r30)
	addiu	r2,r0,-00000001
	sw	r2,0558(r3)
	lw	r2,0018(r30)
	nop
	sw	r0,0554(r2)
	lw	r2,0030(r30)
	nop
	beq	r2,r0,0040435C
	nop

l00404330:
	lw	r4,0018(r30)
	lw	r5,0030(r30)
	lw	r25,-7EE4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	bne	r2,r0,0040435C
	nop

l00404354:
	beq	r0,r0,00404364
	or	r2,r0,r0

l0040435C:
	lw	r2,0018(r30)
	nop

l00404364:
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; request_parse_line: 00404378
request_parse_line proc
	lui	r28,%hi(0FC044E8)
	addiu	r28,r28,%lo(0FC044E8)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	sw	r5,0034(r30)
	lw	r4,0030(r30)
	lw	r5,0034(r30)
	lw	r25,-7EC8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,0018(r30)
	lw	r4,0030(r30)
	lw	r25,-7DC0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	bne	r2,r0,004043E8
	nop

l004043E0:
	beq	r0,r0,00404444
	or	r2,r0,r0

l004043E8:
	lw	r2,0030(r30)
	nop
	addiu	r2,r2,+00000114
	or	r4,r2,r0
	lw	r5,0018(r30)
	lw	r25,-7F20(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,0018(r30)
	lw	r2,0030(r30)
	nop
	addiu	r2,r2,+00000314
	or	r4,r2,r0
	lw	r5,0018(r30)
	lw	r25,-7F78(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,0018(r30)
	addiu	r2,r0,+00000001

l00404444:
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; request_save_line: 00404458
request_save_line proc
	lui	r28,%hi(0FC04408)
	addiu	r28,r28,%lo(0FC04408)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0018(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	sw	r5,0034(r30)
	sw	r6,0038(r30)
	lw	r2,0030(r30)
	nop
	bne	r2,r0,004044D4
	nop

l00404494:
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005C68
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005CD0
	addiu	r7,r0,+0000004B
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,004046B0
	nop

l004044D4:
	lw	r2,0030(r30)
	nop
	lw	r2,0558(r2)
	nop
	slti	r2,r2,+0000003F
	bne	r2,r0,00404544
	nop

l004044F0:
	lw	r2,0030(r30)
	nop
	lw	r2,0558(r2)
	nop
	sw	r2,0010(sp)
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005CBC
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005CD0
	addiu	r7,r0,+00000050
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,004046B0
	nop

l00404544:
	lw	r2,0030(r30)
	lw	r3,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,0558(r2)
	nop
	addiu	r2,r2,+00000001
	sw	r2,0558(r3)
	lw	r4,0038(r30)
	lw	r25,-7E2C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r5,r2,r0
	lw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,0558(r2)
	nop
	sll	r2,r2,02
	addiu	r3,r2,+00000410
	addiu	r2,r4,+00000004
	addu	r2,r2,r3
	sw	r5,0000(r2)
	lw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,0558(r2)
	nop
	sll	r2,r2,02
	addiu	r3,r2,+00000410
	addiu	r2,r4,+00000004
	addu	r2,r2,r3
	lw	r2,0000(r2)
	nop
	bne	r2,r0,0040463C
	nop

l004045DC:
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005C9C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005CD0
	addiu	r7,r0,+00000057
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r2,0030(r30)
	lw	r3,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,0558(r2)
	nop
	addiu	r2,r2,-00000001
	sw	r2,0558(r3)
	beq	r0,r0,004046B0
	nop

l0040463C:
	lw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,0558(r2)
	nop
	addiu	r3,r2,+00000510
	addiu	r2,r4,+00000004
	addu	r3,r2,r3
	lbu	r2,003B(r30)
	nop
	sb	r2,0000(r3)
	lw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,0558(r2)
	nop
	sll	r2,r2,02
	addiu	r3,r2,+00000410
	addiu	r2,r4,+00000004
	addu	r2,r2,r3
	lw	r4,0000(r2)
	lw	r5,0034(r30)
	lw	r6,0038(r30)
	lw	r25,-7E18(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop

l004046B0:
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; request_make_url: 004046C4
request_make_url proc
	lui	r28,%hi(0FC0419C)
	addiu	r28,r28,%lo(0FC0419C)
	addu	r28,r28,r25
	addiu	sp,sp,-00000038
	sw	r28,0020(sp)
	sw	ra,0030(sp)
	sw	r30,002C(sp)
	or	r30,sp,r0
	sw	r4,0038(r30)
	lw	r2,0038(r30)
	nop
	bne	r2,r0,00404738
	nop

l004046F8:
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005C3C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005C18
	addiu	r7,r0,+00000066
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop
	beq	r0,r0,004048C8
	nop

l00404738:
	lw	r2,0038(r30)
	nop
	addiu	r2,r2,+00000114
	or	r4,r2,r0
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005C04
	addiu	r6,r0,+00000007
	lw	r25,-7E68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	bne	r2,r0,004047D4
	nop

l00404774:
	lw	r2,0038(r30)
	nop
	addiu	r3,r2,+00000214
	lw	r2,0038(r30)
	nop
	addiu	r2,r2,+00000114
	sw	r2,0010(sp)
	lw	r2,0038(r30)
	nop
	addiu	r2,r2,+00000314
	sw	r2,0014(sp)
	or	r4,r3,r0
	addiu	r5,r0,+00000100
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005BFC
	lw	r7,0038(r30)
	lw	r25,-7DEC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	beq	r0,r0,004048C8
	nop

l004047D4:
	lw	r2,0038(r30)
	nop
	lb	r3,0114(r2)
	addiu	r2,r0,+0000002F
	bne	r3,r2,0040485C
	nop

l004047EC:
	lw	r2,0038(r30)
	nop
	addiu	r3,r2,+00000214
	lw	r2,0038(r30)
	nop
	addiu	r2,r2,+00000014
	sw	r2,0010(sp)
	lw	r2,0038(r30)
	nop
	addiu	r2,r2,+00000114
	sw	r2,0014(sp)
	lw	r2,0038(r30)
	nop
	addiu	r2,r2,+00000314
	sw	r2,0018(sp)
	or	r4,r3,r0
	addiu	r5,r0,+00000100
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005BF0
	lw	r7,0038(r30)
	lw	r25,-7DEC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	beq	r0,r0,004048C8
	nop

l0040485C:
	lw	r2,0038(r30)
	nop
	addiu	r3,r2,+00000214
	lw	r2,0038(r30)
	nop
	addiu	r2,r2,+00000014
	sw	r2,0010(sp)
	lw	r2,0038(r30)
	nop
	addiu	r2,r2,+00000114
	sw	r2,0014(sp)
	lw	r2,0038(r30)
	nop
	addiu	r2,r2,+00000314
	sw	r2,0018(sp)
	or	r4,r3,r0
	addiu	r5,r0,+00000100
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005BDC
	lw	r7,0038(r30)
	lw	r25,-7DEC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0020(r30)
	nop

l004048C8:
	or	sp,r30,r0
	lw	ra,0030(sp)
	lw	r30,002C(sp)
	jr	ra
	addiu	sp,sp,+00000038

;; request_add_lines: 004048DC
request_add_lines proc
	lui	r28,%hi(0FC03F84)
	addiu	r28,r28,%lo(0FC03F84)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	sw	r5,0034(r30)
	sw	r6,0038(r30)
	sw	r0,0018(r30)

l0040490C:
	lw	r2,0030(r30)
	lw	r3,0018(r30)
	lw	r2,0558(r2)
	nop
	slt	r2,r2,r3
	beq	r2,r0,00404930
	nop

l00404928:
	beq	r0,r0,00404AA4
	nop

l00404930:
	lw	r4,0030(r30)
	lw	r2,0018(r30)
	nop
	sll	r2,r2,02
	addiu	r3,r2,+00000410
	addiu	r2,r4,+00000004
	addu	r2,r2,r3
	lw	r2,0000(r2)
	nop
	beq	r2,r0,00404A90
	nop

l0040495C:
	lw	r5,0034(r30)
	lw	r3,0030(r30)
	lw	r2,0018(r30)
	nop
	addiu	r4,r2,+00000510
	addiu	r2,r3,+00000004
	addu	r2,r2,r4
	lb	r3,0000(r2)
	lw	r2,0FAC(r5)
	nop
	addu	r2,r3,r2
	slti	r2,r2,+000007D0
	beq	r2,r0,00404A58
	nop

l00404994:
	lw	r6,0034(r30)
	lw	r5,0034(r30)
	lw	r3,0030(r30)
	lw	r2,0018(r30)
	nop
	addiu	r4,r2,+00000510
	addiu	r2,r3,+00000004
	addu	r2,r2,r4
	lb	r3,0000(r2)
	lw	r2,0FAC(r5)
	nop
	addu	r2,r2,r3
	sw	r2,0FAC(r6)
	lw	r4,0030(r30)
	lw	r2,0018(r30)
	nop
	sll	r2,r2,02
	addiu	r3,r2,+00000410
	addiu	r2,r4,+00000004
	addu	r5,r2,r3
	lw	r3,0030(r30)
	lw	r2,0018(r30)
	nop
	addiu	r4,r2,+00000510
	addiu	r2,r3,+00000004
	addu	r2,r2,r4
	lb	r2,0000(r2)
	lw	r4,0038(r30)
	lw	r5,0000(r5)
	or	r6,r2,r0
	lw	r25,-7E18(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r3,0030(r30)
	lw	r2,0018(r30)
	nop
	addiu	r4,r2,+00000510
	addiu	r2,r3,+00000004
	addu	r2,r2,r4
	lb	r3,0000(r2)
	lw	r2,0038(r30)
	nop
	addu	r2,r2,r3
	sw	r2,0038(r30)
	beq	r0,r0,00404A90
	nop

l00404A58:
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005BC4
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005BB0
	addiu	r7,r0,+00000084
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop

l00404A90:
	lw	r2,0018(r30)
	nop
	addiu	r2,r2,+00000001
	beq	r0,r0,0040490C
	sw	r2,0018(r30)

l00404AA4:
	lw	r2,0030(r30)
	nop
	lw	r2,0558(r2)
	nop
	bgez	r2,00404AC4
	nop

l00404ABC:
	beq	r0,r0,00404B18
	nop

l00404AC4:
	lw	r6,0030(r30)
	lw	r2,0030(r30)
	nop
	addiu	r5,r2,+00000558
	lw	r3,0000(r5)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addiu	r4,r2,+00000410
	addiu	r2,r6,+00000004
	addu	r2,r2,r4
	addiu	r3,r3,-00000001
	sw	r3,0000(r5)
	lw	r4,0000(r2)
	lw	r25,-7FD4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	beq	r0,r0,00404AA4
	nop

l00404B18:
	lw	r2,0038(r30)
	nop
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; get_method: 00404B34
get_method proc
	lui	r28,%hi(0FC03D2C)
	addiu	r28,r28,%lo(0FC03D2C)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	sw	r5,0034(r30)
	addiu	r2,r30,+00000034
	or	r4,r2,r0
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005B9C
	lw	r25,-7F50(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,0018(r30)
	lw	r4,0030(r30)
	lw	r5,0018(r30)
	addiu	r6,r0,+00000100
	lw	r25,-7ED4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r2,0034(r30)
	nop
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; get_uri: 00404BC8
get_uri proc
	lui	r28,%hi(0FC03C98)
	addiu	r28,r28,%lo(0FC03C98)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	sw	r5,0034(r30)
	addiu	r2,r30,+00000034
	or	r4,r2,r0
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005B9C
	lw	r25,-7F50(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,0018(r30)
	lw	r4,0030(r30)
	lw	r5,0018(r30)
	addiu	r6,r0,+00000100
	lw	r25,-7ED4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r2,0034(r30)
	nop
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; get_version: 00404C5C
get_version proc
	lui	r28,%hi(0FC03C04)
	addiu	r28,r28,%lo(0FC03C04)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	sw	r5,0034(r30)
	addiu	r2,r30,+00000034
	or	r4,r2,r0
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005B98
	lw	r25,-7F50(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,0018(r30)
	lw	r4,0030(r30)
	lw	r5,0018(r30)
	addiu	r6,r0,+00000100
	lw	r25,-7ED4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r2,0034(r30)
	nop
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; request_get_host: 00404CF0
request_get_host proc
	lui	r28,%hi(0FC03B70)
	addiu	r28,r28,%lo(0FC03B70)
	addu	r28,r28,r25
	addiu	sp,sp,-00000038
	sw	r28,0018(sp)
	sw	ra,0030(sp)
	sw	r30,002C(sp)
	or	r30,sp,r0
	sw	r4,0038(r30)
	sw	r5,003C(r30)
	lw	r2,0038(r30)
	nop
	beq	r2,r0,00404F6C
	nop

l00404D28:
	lw	r3,0038(r30)
	addiu	r2,r0,-00000014
	beq	r3,r2,00404F6C
	nop

l00404D38:
	lw	r4,003C(r30)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005B94
	lw	r25,-7FD0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0020(r30)
	lw	r2,0020(r30)
	nop
	bne	r2,r0,00404DB0
	nop

l00404D70:
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005B90
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005B68
	addiu	r7,r0,+000000BF
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00404FAC
	or	r2,r0,r0

l00404DB0:
	lw	r2,003C(r30)
	nop
	addiu	r3,r2,+00000100
	lw	r2,0020(r30)
	nop
	sltu	r2,r2,r3
	beq	r2,r0,00404E1C
	nop

l00404DD0:
	lw	r2,0020(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+00000020
	beq	r3,r2,00404E08
	nop

l00404DE8:
	lw	r2,0020(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+00000009
	beq	r3,r2,00404E08
	nop

l00404E00:
	beq	r0,r0,00404E1C
	nop

l00404E08:
	lw	r2,0020(r30)
	nop
	addiu	r2,r2,+00000001
	beq	r0,r0,00404DB0
	sw	r2,0020(r30)

l00404E1C:
	lw	r4,0020(r30)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005B98
	lw	r25,-7FD0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0024(r30)
	lw	r2,0024(r30)
	nop
	bne	r2,r0,00404E94
	nop

l00404E54:
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005B54
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005B68
	addiu	r7,r0,+000000C6
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00404FAC
	or	r2,r0,r0

l00404E94:
	lw	r3,0024(r30)
	lw	r2,0020(r30)
	nop
	subu	r2,r3,r2
	slti	r2,r2,+00000101
	bne	r2,r0,00404F04
	nop

l00404EB0:
	lw	r2,0024(r30)
	lw	r3,0020(r30)
	nop
	subu	r2,r2,r3
	sw	r2,0010(sp)
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005B30
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005B68
	addiu	r7,r0,+000000CA
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00404FAC
	or	r2,r0,r0

l00404F04:
	lw	r2,0038(r30)
	nop
	addiu	r4,r2,+00000014
	lw	r3,0024(r30)
	lw	r2,0020(r30)
	nop
	subu	r2,r3,r2
	lw	r5,0020(r30)
	or	r6,r2,r0
	lw	r25,-7ED4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r4,0038(r30)
	lw	r3,0024(r30)
	lw	r2,0020(r30)
	nop
	subu	r2,r3,r2
	addiu	r3,r2,+00000010
	addiu	r2,r4,+00000004
	addu	r2,r2,r3
	sb	r0,0000(r2)
	beq	r0,r0,00404FA4
	nop

l00404F6C:
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005B10
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005B68
	addiu	r7,r0,+000000D0
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop

l00404FA4:
	lw	r2,0024(r30)
	nop

l00404FAC:
	or	sp,r30,r0
	lw	ra,0030(sp)
	lw	r30,002C(sp)
	jr	ra
	addiu	sp,sp,+00000038

;; request_get_content_length: 00404FC0
request_get_content_length proc
	lui	r28,%hi(0FC038A0)
	addiu	r28,r28,%lo(0FC038A0)
	addu	r28,r28,r25
	addiu	sp,sp,-00000038
	sw	r28,0018(sp)
	sw	ra,0030(sp)
	sw	r30,002C(sp)
	or	r30,sp,r0
	sw	r4,0038(r30)
	sw	r5,003C(r30)
	lw	r2,0038(r30)
	nop
	beq	r2,r0,00405210
	nop

l00404FF8:
	lw	r3,0038(r30)
	addiu	r2,r0,-00000014
	beq	r3,r2,00405210
	nop

l00405008:
	lw	r4,003C(r30)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005B94
	lw	r25,-7FD0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0020(r30)
	lw	r2,0020(r30)
	nop
	bne	r2,r0,00405080
	nop

l00405040:
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005A9C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005AE4
	addiu	r7,r0,+000000E2
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00405250
	or	r2,r0,r0

l00405080:
	lw	r2,003C(r30)
	nop
	addiu	r3,r2,+00000100
	lw	r2,0020(r30)
	nop
	sltu	r2,r2,r3
	beq	r2,r0,004050EC
	nop

l004050A0:
	lw	r2,0020(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+00000020
	beq	r3,r2,004050D8
	nop

l004050B8:
	lw	r2,0020(r30)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+00000009
	beq	r3,r2,004050D8
	nop

l004050D0:
	beq	r0,r0,004050EC
	nop

l004050D8:
	lw	r2,0020(r30)
	nop
	addiu	r2,r2,+00000001
	beq	r0,r0,00405080
	sw	r2,0020(r30)

l004050EC:
	lw	r4,0020(r30)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005B98
	lw	r25,-7FD0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0024(r30)
	lw	r2,0024(r30)
	nop
	bne	r2,r0,00405164
	nop

l00405124:
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005B54
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005AE4
	addiu	r7,r0,+000000EB
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00405250
	or	r2,r0,r0

l00405164:
	lw	r3,0024(r30)
	lw	r2,0020(r30)
	nop
	subu	r2,r3,r2
	slti	r2,r2,+00000101
	bne	r2,r0,004051D4
	nop

l00405180:
	lw	r2,0024(r30)
	lw	r3,0020(r30)
	nop
	subu	r2,r2,r3
	sw	r2,0010(sp)
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005B30
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005AE4
	addiu	r7,r0,+000000F0
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00405250
	or	r2,r0,r0

l004051D4:
	addiu	r2,r30,+00000024
	lw	r4,0020(r30)
	or	r5,r2,r0
	addiu	r6,r0,+0000000A
	lw	r25,-7E34(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r3,r2,r0
	lw	r2,0038(r30)
	nop
	sw	r3,0554(r2)
	beq	r0,r0,00405248
	nop

l00405210:
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005AC8
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005AE4
	addiu	r7,r0,+000000F6
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop

l00405248:
	lw	r2,0024(r30)
	nop

l00405250:
	or	sp,r30,r0
	lw	ra,0030(sp)
	lw	r30,002C(sp)
	jr	ra
	addiu	sp,sp,+00000038

;; is_a_method: 00405264
is_a_method proc
	lui	r28,%hi(0FC035FC)
	addiu	r28,r28,%lo(0FC035FC)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	sw	r0,0018(r30)

l0040528C:
	lw	r2,0018(r30)
	nop
	sll	r3,r2,02
	lw	r2,-7E98(r28)
	nop
	addu	r2,r2,r3
	lw	r2,0000(r2)
	nop
	bne	r2,r0,004052BC
	nop

l004052B4:
	beq	r0,r0,00405310
	nop

l004052BC:
	lw	r3,0018(r30)
	nop
	or	r2,r3,r0
	sll	r4,r2,02
	lw	r2,-7E98(r28)
	nop
	addu	r2,r2,r4
	addiu	r3,r3,+00000001
	sw	r3,0018(r30)
	lw	r4,0000(r2)
	lw	r5,0030(r30)
	addiu	r6,r0,+00000014
	lw	r25,-7ECC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	bne	r2,r0,0040528C
	nop

l00405308:
	beq	r0,r0,00405314
	addiu	r2,r0,+00000001

l00405310:
	or	r2,r0,r0

l00405314:
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; request_destroy: 00405328
request_destroy proc
	lui	r28,%hi(0FC03538)
	addiu	r28,r28,%lo(0FC03538)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	ra,0020(sp)
	sw	r30,001C(sp)
	or	r30,sp,r0
	sw	r4,0028(r30)

l0040534C:
	lw	r2,0028(r30)
	nop
	lw	r2,0558(r2)
	nop
	bgez	r2,0040536C
	nop

l00405364:
	beq	r0,r0,004053C0
	nop

l0040536C:
	lw	r6,0028(r30)
	lw	r2,0028(r30)
	nop
	addiu	r5,r2,+00000558
	lw	r3,0000(r5)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addiu	r4,r2,+00000410
	addiu	r2,r6,+00000004
	addu	r2,r2,r4
	addiu	r3,r3,-00000001
	sw	r3,0000(r5)
	lw	r4,0000(r2)
	lw	r25,-7FD4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	beq	r0,r0,0040534C
	nop

l004053C0:
	lw	r4,0028(r30)
	lw	r25,-7FD4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	or	sp,r30,r0
	lw	ra,0020(sp)
	lw	r30,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028

;; properties_parse_int: 004053F0
properties_parse_int proc
	lui	r28,%hi(0FC03470)
	addiu	r28,r28,%lo(0FC03470)
	addu	r28,r28,r25
	addiu	sp,sp,-00000038
	sw	r28,0018(sp)
	sw	ra,0030(sp)
	sw	r30,002C(sp)
	or	r30,sp,r0
	sw	r4,0038(r30)
	sw	r5,003C(r30)
	lw	r4,0038(r30)
	addiu	r5,r30,+00000020
	or	r6,r0,r0
	lw	r25,-7E34(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0024(r30)
	lw	r2,0020(r30)
	nop
	lb	r2,0000(r2)
	nop
	beq	r2,r0,004054B4
	nop

l00405454:
	lw	r3,0020(r30)
	lw	r2,003C(r30)
	nop
	beq	r3,r2,004054B4
	nop

l00405468:
	lw	r2,0038(r30)
	nop
	sw	r2,0010(sp)
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005A70
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005A58
	addiu	r7,r0,+0000001F
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,004054BC
	or	r2,r0,r0

l004054B4:
	lw	r2,0024(r30)
	nop

l004054BC:
	or	sp,r30,r0
	lw	ra,0030(sp)
	lw	r30,002C(sp)
	jr	ra
	addiu	sp,sp,+00000038

;; properties_load: 004054D0
properties_load proc
	lui	r28,%hi(0FC03390)
	addiu	r28,r28,%lo(0FC03390)
	addu	r28,r28,r25
	addiu	sp,sp,-00000170
	sw	r28,0018(sp)
	sw	ra,0168(sp)
	sw	r30,0164(sp)
	or	r30,sp,r0
	sw	r4,0170(r30)
	sw	r5,0174(r30)
	addiu	r2,r30,+00000130
	addiu	r3,r30,+00000130
	lw	r2,-7FE0(r28)
	nop
	addiu	r2,r2,-00005A20
	nop
	lwl	r4,0000(r2)
	lwr	r4,0003(r2)
	lwl	r5,0004(r2)
	lwr	r5,0007(r2)
	lb	r6,0008(r2)
	lb	r7,0009(r2)
	swl	r4,0000(r3)
	swr	r4,0003(r3)
	swl	r5,0004(r3)
	swr	r5,0007(r3)
	sb	r6,0008(r3)
	sb	r7,0009(r3)
	sw	r0,0140(r30)
	lw	r2,0170(r30)
	nop
	bne	r2,r0,0040555C
	nop

l00405554:
	beq	r0,r0,00405CBC
	or	r2,r0,r0

l0040555C:
	lw	r2,0174(r30)
	nop
	bne	r2,r0,00405698
	nop

l0040556C:
	lw	r4,0170(r30)
	lw	r25,-7F94(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	addiu	r2,r2,+0000000B
	or	r4,r2,r0
	lw	r25,-7E2C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0174(r30)
	lw	r2,0174(r30)
	nop
	bne	r2,r0,004055F4
	nop

l004055B4:
	addiu	r4,r0,+00000001
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005A14
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-000059FC
	addiu	r7,r0,+00000039
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00405CBC
	or	r2,r0,r0

l004055F4:
	addiu	r2,r0,+00000001
	sw	r2,0140(r30)
	lw	r4,0174(r30)
	lw	r5,0170(r30)
	lw	r25,-7DBC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r4,0174(r30)
	addiu	r5,r0,+0000002F
	lw	r25,-7E8C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0128(r30)
	lw	r2,0128(r30)
	nop
	beq	r2,r0,00405668
	nop

l0040564C:
	lw	r2,0128(r30)
	nop
	addiu	r2,r2,+00000001
	sw	r2,0128(r30)
	sb	r0,0000(r2)
	beq	r0,r0,00405674
	nop

l00405668:
	lw	r2,0174(r30)
	nop
	sw	r2,0128(r30)

l00405674:
	addiu	r2,r30,+00000130
	lw	r4,0128(r30)
	or	r5,r2,r0
	lw	r25,-7DBC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop

l00405698:
	lw	r4,0174(r30)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000059EC
	lw	r25,-7F14(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0020(r30)
	lw	r2,0020(r30)
	nop
	bne	r2,r0,0040571C
	nop

l004056D0:
	lw	r2,0174(r30)
	nop
	sw	r2,0010(sp)
	addiu	r4,r0,+00000001
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000059E8
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-000059FC
	addiu	r7,r0,+0000004B
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00405CBC
	or	r2,r0,r0

l0040571C:
	lw	r2,0140(r30)
	nop
	beq	r2,r0,00405748
	nop

l0040572C:
	lw	r4,0174(r30)
	lw	r25,-7FD4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop

l00405748:
	addiu	r2,r30,+00000028
	or	r4,r2,r0
	addiu	r5,r0,+000000FF
	lw	r6,0020(r30)
	lw	r25,-7E14(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0128(r30)
	lw	r2,0128(r30)
	nop
	bne	r2,r0,00405788
	nop

l00405780:
	beq	r0,r0,00405C9C
	nop

l00405788:
	lw	r4,0128(r30)
	addiu	r5,r0,+0000000A
	lw	r25,-7FA8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0148(r30)
	lw	r2,0148(r30)
	nop
	beq	r2,r0,004057C4
	nop

l004057B8:
	lw	r2,0148(r30)
	nop
	sb	r0,0000(r2)

l004057C4:
	lw	r4,0128(r30)
	addiu	r5,r0,+00000023
	lw	r25,-7FA8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0148(r30)
	lw	r2,0148(r30)
	nop
	beq	r2,r0,00405800
	nop

l004057F4:
	lw	r2,0148(r30)
	nop
	sb	r0,0000(r2)

l00405800:
	lw	r4,0128(r30)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000059C0
	lw	r25,-7F8C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r3,r2,r0
	lw	r2,0128(r30)
	nop
	addu	r2,r2,r3
	sw	r2,0128(r30)
	lw	r2,0128(r30)
	nop
	lb	r2,0000(r2)
	nop
	bne	r2,r0,00405858
	nop

l00405850:
	beq	r0,r0,00405748
	nop

l00405858:
	lw	r2,0128(r30)
	nop
	sw	r2,014C(r30)
	lw	r4,0128(r30)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000059BC
	lw	r25,-7F44(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r3,r2,r0
	lw	r2,0128(r30)
	nop
	addu	r2,r2,r3
	sw	r2,0128(r30)
	lw	r2,0128(r30)
	nop
	sw	r2,0150(r30)
	lw	r4,0128(r30)
	addiu	r5,r0,+0000003D
	lw	r25,-7FA8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0128(r30)
	lw	r2,0128(r30)
	nop
	bne	r2,r0,00405920
	nop

l004058D8:
	addiu	r2,r30,+00000028
	sw	r2,0010(sp)
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000059B8
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-000059FC
	addiu	r7,r0,+0000006E
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00405748
	nop

l00405920:
	lw	r2,0128(r30)
	nop
	addiu	r2,r2,+00000001
	sw	r2,0128(r30)
	lw	r4,0128(r30)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000059C0
	lw	r25,-7F8C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r3,r2,r0
	lw	r2,0128(r30)
	nop
	addu	r2,r2,r3
	sw	r2,0128(r30)
	lw	r2,0128(r30)
	nop
	sw	r2,0154(r30)
	lw	r4,0128(r30)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000059C0
	lw	r25,-7F44(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r3,r2,r0
	lw	r2,0128(r30)
	nop
	addu	r2,r2,r3
	sw	r2,0128(r30)
	lw	r2,0128(r30)
	nop
	sb	r0,0000(r2)
	lw	r2,0150(r30)
	nop
	sb	r0,0000(r2)
	sw	r0,0144(r30)
	sw	r0,0024(r30)

l004059CC:
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r2,r2,+00000008
	lw	r3,-7E20(r28)
	nop
	addu	r3,r3,r2
	lb	r3,0004(r3)
	addiu	r2,r0,+00000020
	beq	r3,r2,00405C40
	nop

l00405A0C:
	lw	r2,0144(r30)
	nop
	bne	r2,r0,00405C40
	nop

l00405A1C:
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r3,r2,03
	lw	r2,-7E20(r28)
	nop
	addiu	r2,r2,+00000004
	addu	r2,r2,r3
	lw	r4,014C(r30)
	lw	r5,0000(r2)
	lw	r25,-7EE0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	bne	r2,r0,00405C2C
	nop

l00405A70:
	addiu	r2,r0,+00000001
	sw	r2,0144(r30)
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r3,r2,+00000008
	lw	r2,-7E20(r28)
	nop
	addu	r2,r2,r3
	lw	r2,0000(r2)
	nop
	sw	r2,0158(r30)
	addiu	r2,r0,+00000001
	lw	r3,0158(r30)
	nop
	beq	r3,r2,00405B54
	nop

l00405AC8:
	lw	r4,0158(r30)
	nop
	sltiu	r2,r4,+00000001
	bne	r2,r0,00405AF8
	nop

l00405ADC:
	addiu	r2,r0,+00000002
	lw	r5,0158(r30)
	nop
	beq	r5,r2,00405BBC
	nop

l00405AF0:
	beq	r0,r0,00405C2C
	nop

l00405AF8:
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r3,r2,03
	lw	r2,-7E20(r28)
	nop
	addiu	r2,r2,+0000000D
	addu	r2,r3,r2
	or	r4,r2,r0
	lw	r5,0154(r30)
	addiu	r6,r0,+00000014
	lw	r25,-7ED4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00405C2C
	nop

l00405B54:
	lw	r4,0154(r30)
	or	r5,r0,r0
	lw	r25,-7FE4(r28)
	nop
	addiu	r25,r25,+000053F0
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r4,r2,r0
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r3,r2,+00000090
	lw	r2,-7E20(r28)
	nop
	addiu	r2,r2,+00000004
	addu	r2,r2,r3
	sw	r4,0000(r2)
	beq	r0,r0,00405C2C
	nop

l00405BBC:
	lw	r2,0154(r30)
	nop
	lb	r2,0000(r2)
	lw	r4,-7FE0(r28)
	nop
	addiu	r4,r4,-00005A40
	or	r5,r2,r0
	lw	r25,-7FA8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r4,r2,r0
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r3,r2,+00000090
	lw	r2,-7E20(r28)
	nop
	addiu	r2,r2,+00000004
	addu	r3,r2,r3
	sltu	r2,r0,r4
	sw	r2,0000(r3)

l00405C2C:
	lw	r2,0024(r30)
	nop
	addiu	r2,r2,+00000001
	beq	r0,r0,004059CC
	sw	r2,0024(r30)

l00405C40:
	lw	r2,0144(r30)
	nop
	bne	r2,r0,00405748
	nop

l00405C50:
	lw	r2,014C(r30)
	nop
	sw	r2,0010(sp)
	addiu	r4,r0,+00000001
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005A3C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-000059FC
	addiu	r7,r0,+0000008E
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00405748
	nop

l00405C9C:
	lw	r4,0020(r30)
	lw	r25,-7F24(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	addiu	r2,r0,+00000001

l00405CBC:
	or	sp,r30,r0
	lw	ra,0168(sp)
	lw	r30,0164(sp)
	jr	ra
	addiu	sp,sp,+00000170

;; properties_parse_command_line: 00405CD0
properties_parse_command_line proc
	lui	r28,%hi(0FC02B90)
	addiu	r28,r28,%lo(0FC02B90)
	addu	r28,r28,r25
	addiu	sp,sp,-00000048
	sw	r28,0018(sp)
	sw	ra,0044(sp)
	sw	r30,0040(sp)
	sw	r16,003C(sp)
	or	r30,sp,r0
	sw	r4,0048(r30)
	sw	r5,004C(r30)
	sw	r0,0020(r30)

l00405D00:
	lw	r2,0020(r30)
	lw	r3,0048(r30)
	nop
	slt	r2,r2,r3
	bne	r2,r0,00405D20
	nop

l00405D18:
	beq	r0,r0,00406270
	nop

l00405D20:
	lw	r2,0020(r30)
	nop
	sll	r3,r2,02
	lw	r2,004C(r30)
	nop
	addu	r2,r3,r2
	lw	r2,0000(r2)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000002D
	bne	r3,r2,0040625C
	nop

l00405D50:
	lw	r2,0020(r30)
	nop
	sll	r3,r2,02
	lw	r2,004C(r30)
	nop
	addu	r2,r3,r2
	lw	r2,0000(r2)
	nop
	addiu	r2,r2,+00000001
	lbu	r2,0000(r2)
	nop
	sb	r2,002C(r30)
	lb	r2,002C(r30)
	nop
	beq	r2,r0,00405DF4
	nop

l00405D90:
	lw	r2,0020(r30)
	nop
	sll	r3,r2,02
	lw	r2,004C(r30)
	nop
	addu	r2,r3,r2
	lw	r2,0000(r2)
	nop
	addiu	r2,r2,+00000002
	lb	r2,0000(r2)
	nop
	beq	r2,r0,00405DF4
	nop

l00405DC4:
	lw	r2,0020(r30)
	nop
	sll	r3,r2,02
	lw	r2,004C(r30)
	nop
	addu	r2,r3,r2
	lw	r2,0000(r2)
	nop
	addiu	r2,r2,+00000002
	sw	r2,0030(r30)
	beq	r0,r0,00405E54
	nop

l00405DF4:
	lw	r2,0048(r30)
	lw	r3,0020(r30)
	nop
	slt	r2,r3,r2
	beq	r2,r0,00405E40
	nop

l00405E0C:
	lw	r2,0020(r30)
	nop
	addiu	r2,r2,+00000001
	sw	r2,0020(r30)
	sll	r3,r2,02
	lw	r2,004C(r30)
	nop
	addu	r2,r3,r2
	lw	r2,0000(r2)
	nop
	sw	r2,0030(r30)
	beq	r0,r0,00405E54
	nop

l00405E40:
	lw	r2,-7FE0(r28)
	nop
	addiu	r2,r2,-00005990
	nop
	sw	r2,0030(r30)

l00405E54:
	lb	r3,002C(r30)
	addiu	r2,r0,+00000068
	bne	r3,r2,00405EA4
	nop

l00405E64:
	lw	r2,004C(r30)
	nop
	lw	r4,0000(r2)
	lw	r25,-7DC4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	or	r4,r0,r0
	lw	r25,-7F70(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop

l00405EA4:
	sw	r0,0028(r30)
	sw	r0,0024(r30)

l00405EAC:
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r2,r2,+00000008
	lw	r3,-7E20(r28)
	nop
	addu	r3,r3,r2
	lb	r3,0004(r3)
	addiu	r2,r0,+00000020
	beq	r3,r2,004061DC
	nop

l00405EEC:
	lw	r2,0028(r30)
	nop
	bne	r2,r0,004061DC
	nop

l00405EFC:
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r2,r2,+00000008
	lb	r3,002C(r30)
	lw	r1,-7E20(r28)
	nop
	addu	r1,r1,r2
	lb	r2,0004(r1)
	nop
	beq	r3,r2,00406004
	nop

l00405F40:
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r3,r2,+00000008
	lw	r2,-7E20(r28)
	nop
	addu	r2,r2,r3
	lw	r3,0000(r2)
	addiu	r2,r0,+00000002
	bne	r3,r2,004061C8
	nop

l00405F80:
	lb	r2,002C(r30)
	nop
	or	r4,r2,r0
	lw	r25,-7F9C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r16,r2,r0
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r2,r2,+00000008
	lw	r1,-7E20(r28)
	nop
	addu	r1,r1,r2
	lb	r2,0004(r1)
	nop
	or	r4,r2,r0
	lw	r25,-7F9C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	beq	r16,r2,00406004
	nop

l00405FFC:
	beq	r0,r0,004061C8
	nop

l00406004:
	addiu	r2,r0,+00000001
	sw	r2,0028(r30)
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r3,r2,+00000008
	lw	r2,-7E20(r28)
	nop
	addu	r2,r2,r3
	lw	r2,0000(r2)
	nop
	sw	r2,0034(r30)
	addiu	r2,r0,+00000001
	lw	r3,0034(r30)
	nop
	beq	r3,r2,004060E8
	nop

l0040605C:
	lw	r3,0034(r30)
	nop
	sltiu	r2,r3,+00000001
	bne	r2,r0,0040608C
	nop

l00406070:
	addiu	r2,r0,+00000002
	lw	r3,0034(r30)
	nop
	beq	r3,r2,00406150
	nop

l00406084:
	beq	r0,r0,004061C8
	nop

l0040608C:
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r3,r2,03
	lw	r2,-7E20(r28)
	nop
	addiu	r2,r2,+0000000D
	addu	r2,r3,r2
	or	r4,r2,r0
	lw	r5,0030(r30)
	addiu	r6,r0,+00000014
	lw	r25,-7ED4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,004061C8
	nop

l004060E8:
	lw	r4,0030(r30)
	or	r5,r0,r0
	lw	r25,-7FE4(r28)
	nop
	addiu	r25,r25,+000053F0
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r4,r2,r0
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r3,r2,+00000090
	lw	r2,-7E20(r28)
	nop
	addiu	r2,r2,+00000004
	addu	r2,r2,r3
	sw	r4,0000(r2)
	beq	r0,r0,004061C8
	nop

l00406150:
	lw	r3,0024(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r3,r2,+00000090
	lw	r2,-7E20(r28)
	nop
	addiu	r2,r2,+00000004
	addu	r4,r2,r3
	lb	r2,002C(r30)
	nop
	sll	r3,r2,01
	lw	r2,-7DF4(r28)
	nop
	lw	r2,0000(r2)
	nop
	addu	r2,r3,r2
	lhu	r2,0000(r2)
	nop
	andi	r2,r2,00000001
	andi	r2,r2,0000FFFF
	sw	r2,0000(r4)
	lw	r2,0020(r30)
	nop
	addiu	r2,r2,-00000001
	sw	r2,0020(r30)

l004061C8:
	lw	r2,0024(r30)
	nop
	addiu	r2,r2,+00000001
	beq	r0,r0,00405EAC
	sw	r2,0024(r30)

l004061DC:
	lw	r2,0028(r30)
	nop
	bne	r2,r0,0040625C
	nop

l004061EC:
	lb	r2,002C(r30)
	nop
	sw	r2,0010(sp)
	addiu	r4,r0,+00000001
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-0000598C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005974
	addiu	r7,r0,+000000C8
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r2,004C(r30)
	nop
	lw	r4,0000(r2)
	lw	r25,-7DC4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00406274
	or	r2,r0,r0

l0040625C:
	lw	r2,0020(r30)
	nop
	addiu	r2,r2,+00000001
	beq	r0,r0,00405D00
	sw	r2,0020(r30)

l00406270:
	addiu	r2,r0,+00000001

l00406274:
	or	sp,r30,r0
	lw	ra,0044(sp)
	lw	r30,0040(sp)
	lw	r16,003C(sp)
	jr	ra
	addiu	sp,sp,+00000048

;; properties_print_usage: 0040628C
properties_print_usage proc
	lui	r28,%hi(0FC025D4)
	addiu	r28,r28,%lo(0FC025D4)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	beq	r2,r0,00406304
	nop

l004062C0:
	lw	r2,0030(r30)
	nop
	lb	r2,0000(r2)
	nop
	beq	r2,r0,00406304
	nop

l004062D8:
	lw	r4,-7FE0(r28)
	nop
	addiu	r4,r4,-00005954
	lw	r5,0030(r30)
	lw	r25,-7DD0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	beq	r0,r0,00406328
	nop

l00406304:
	lw	r4,-7FE0(r28)
	nop
	addiu	r4,r4,-00005944
	lw	r25,-7DD0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop

l00406328:
	sw	r0,0018(r30)

l0040632C:
	lw	r3,0018(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r2,r2,+00000008
	lw	r3,-7E20(r28)
	nop
	addu	r3,r3,r2
	lb	r3,0004(r3)
	addiu	r2,r0,+00000020
	bne	r3,r2,00406374
	nop

l0040636C:
	beq	r0,r0,004065D0
	nop

l00406374:
	lw	r3,0018(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r3,r2,+00000008
	lw	r2,-7E20(r28)
	nop
	addu	r2,r2,r3
	lw	r3,0000(r2)
	addiu	r2,r0,+00000002
	bne	r3,r2,00406530
	nop

l004063B4:
	lw	r3,0018(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r2,r2,+00000008
	lw	r1,-7E20(r28)
	nop
	addu	r1,r1,r2
	lb	r2,0004(r1)
	nop
	sll	r3,r2,01
	lw	r2,-7DF4(r28)
	nop
	lw	r2,0000(r2)
	nop
	addu	r2,r3,r2
	lhu	r2,0000(r2)
	nop
	andi	r2,r2,00000001
	andi	r2,r2,0000FFFF
	beq	r2,r0,00406474
	nop

l0040641C:
	lw	r3,0018(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r2,r2,+00000008
	lw	r1,-7E20(r28)
	nop
	addu	r1,r1,r2
	lb	r2,0004(r1)
	nop
	or	r4,r2,r0
	lw	r25,-7E24(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	beq	r0,r0,004064C8
	sw	r2,001C(r30)

l00406474:
	lw	r3,0018(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r2,r2,+00000008
	lw	r1,-7E20(r28)
	nop
	addu	r1,r1,r2
	lb	r2,0004(r1)
	nop
	or	r4,r2,r0
	lw	r25,-7F9C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,001C(r30)

l004064C8:
	lw	r3,0018(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r2,r2,+00000008
	lw	r1,-7E20(r28)
	nop
	addu	r1,r1,r2
	lb	r2,0004(r1)
	lw	r4,-7FE0(r28)
	nop
	addiu	r4,r4,-00005930
	or	r5,r2,r0
	lw	r6,001C(r30)
	lw	r25,-7DD0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	beq	r0,r0,004065BC
	nop

l00406530:
	lw	r3,0018(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r2,r2,03
	addiu	r2,r2,+00000008
	lw	r5,-7E20(r28)
	nop
	addu	r5,r5,r2
	lb	r5,0004(r5)
	lw	r3,0018(r30)
	nop
	or	r2,r3,r0
	sll	r2,r2,02
	addu	r2,r2,r3
	sll	r2,r2,02
	subu	r2,r2,r3
	sll	r3,r2,03
	lw	r2,-7E20(r28)
	nop
	addiu	r2,r2,+00000004
	addu	r2,r2,r3
	lw	r4,-7FE0(r28)
	nop
	addiu	r4,r4,-00005928
	lw	r6,0000(r2)
	lw	r25,-7DD0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop

l004065BC:
	lw	r2,0018(r30)
	nop
	addiu	r2,r2,+00000001
	beq	r0,r0,0040632C
	sw	r2,0018(r30)

l004065D0:
	lw	r4,-7FE0(r28)
	nop
	addiu	r4,r4,-00005920
	lw	r25,-7DD0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030
00406608                         00 00 00 00 00 00 00 00         ........

;; print_log: 00406610
print_log proc
	lui	r28,%hi(0FC02250)
	addiu	r28,r28,%lo(0FC02250)
	addu	r28,r28,r25
	addiu	sp,sp,-00000438
	sw	r28,0010(sp)
	sw	r5,043C(sp)
	sw	r6,0440(sp)
	sw	r7,0444(sp)
	sw	ra,0430(sp)
	sw	r30,042C(sp)
	or	r30,sp,r0
	sw	r4,0438(r30)
	sw	r5,043C(r30)
	lw	r2,0438(r30)
	lw	r3,-7F6C(r28)
	nop
	lw	r3,0000(r3)
	nop
	slt	r2,r3,r2
	bne	r2,r0,004066D4
	nop

l00406664:
	addiu	r2,r30,+00000440
	sw	r2,0018(r30)
	addiu	r2,r30,+00000020
	or	r4,r2,r0
	addiu	r5,r0,+000003FF
	lw	r6,043C(r30)
	lw	r7,0018(r30)
	lw	r25,-7E30(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,0420(r30)
	addiu	r2,r30,+00000020
	lw	r4,-7E40(r28)
	nop
	lw	r4,0000(r4)
	lw	r5,0438(r30)
	lw	r6,0420(r30)
	or	r7,r2,r0
	lw	r25,-7FE4(r28)
	nop
	addiu	r25,r25,+0000697C
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop

l004066D4:
	or	sp,r30,r0
	lw	ra,0430(sp)
	lw	r30,042C(sp)
	jr	ra
	addiu	sp,sp,+00000438

;; open_log: 004066E8
open_log proc
	lui	r28,%hi(0FC02178)
	addiu	r28,r28,%lo(0FC02178)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	ra,0020(sp)
	sw	r30,001C(sp)
	or	r30,sp,r0
	sw	r4,0028(r30)
	lw	r2,-7FC0(r28)
	nop
	lw	r2,0000(r2)
	nop
	lb	r2,0000(r2)
	nop
	beq	r2,r0,004067EC
	nop

l0040672C:
	lw	r2,-7FC0(r28)
	nop
	lw	r2,0000(r2)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000002D
	beq	r3,r2,004067EC
	nop

l0040674C:
	lw	r4,-7FC0(r28)
	nop
	lw	r4,0000(r4)
	lw	r5,0028(r30)
	lw	r25,-7F14(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r1,-7E40(r28)
	nop
	sw	r2,0000(r1)
	lw	r2,-7E40(r28)
	nop
	lw	r2,0000(r2)
	nop
	bne	r2,r0,00406834
	nop

l00406798:
	lw	r2,-7F4C(r28)
	nop
	lw	r2,0000(r2)
	lw	r1,-7E40(r28)
	nop
	sw	r2,0000(r1)
	or	r4,r0,r0
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000058FC
	lw	r6,-7FC0(r28)
	nop
	lw	r6,0000(r6)
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	beq	r0,r0,00406834
	nop

l004067EC:
	lw	r2,-7F4C(r28)
	nop
	lw	r2,0000(r2)
	lw	r1,-7E40(r28)
	nop
	sw	r2,0000(r1)
	lw	r4,-7FC0(r28)
	nop
	lw	r4,0000(r4)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000058C4
	lw	r25,-7DBC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop

l00406834:
	or	sp,r30,r0
	lw	ra,0020(sp)
	lw	r30,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028

;; log_rotate: 00406848
log_rotate proc
	lui	r28,%hi(0FC02018)
	addiu	r28,r28,%lo(0FC02018)
	addu	r28,r28,r25
	addiu	sp,sp,-00000128
	sw	r28,0010(sp)
	sw	ra,0120(sp)
	sw	r30,011C(sp)
	or	r30,sp,r0
	lw	r4,-7E40(r28)
	nop
	lw	r4,0000(r4)
	lw	r25,-7F24(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r2,-7FC0(r28)
	nop
	lw	r2,0000(r2)
	nop
	lb	r2,0000(r2)
	nop
	beq	r2,r0,00406944
	nop

l004068AC:
	lw	r2,-7FC0(r28)
	nop
	lw	r2,0000(r2)
	nop
	lb	r3,0000(r2)
	addiu	r2,r0,+0000002D
	beq	r3,r2,00406944
	nop

l004068CC:
	addiu	r4,r30,+00000018
	lw	r5,-7FC0(r28)
	nop
	lw	r5,0000(r5)
	lw	r25,-7DBC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	addiu	r4,r30,+00000018
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000058BC
	lw	r25,-7EA0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r4,-7FC0(r28)
	nop
	lw	r4,0000(r4)
	addiu	r5,r30,+00000018
	lw	r25,-7E88(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop

l00406944:
	lw	r4,-7FE0(r28)
	nop
	addiu	r4,r4,-000058C0
	lw	r25,-7F90(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	or	sp,r30,r0
	lw	ra,0120(sp)
	lw	r30,011C(sp)
	jr	ra
	addiu	sp,sp,+00000128

;; log_string: 0040697C
log_string proc
	lui	r28,%hi(0FC01EE4)
	addiu	r28,r28,%lo(0FC01EE4)
	addu	r28,r28,r25
	addiu	sp,sp,-00000068
	sw	r28,0018(sp)
	sw	ra,0060(sp)
	sw	r30,005C(sp)
	sw	r17,0058(sp)
	sw	r16,0054(sp)
	or	r30,sp,r0
	sw	r4,0068(r30)
	or	r16,r5,r0
	or	r2,r6,r0
	or	r17,r7,r0
	addiu	r2,r30,+00000040
	or	r4,r2,r0
	or	r5,r0,r0
	lw	r25,-7F10(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	addiu	r2,r30,+00000040
	or	r4,r2,r0
	lw	r25,-7F88(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	addiu	r4,r30,+00000020
	addiu	r5,r0,+0000001C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-000058B4
	or	r7,r2,r0
	lw	r25,-7F60(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	or	r2,r16,r0
	sll	r3,r2,02
	lw	r2,-7F64(r28)
	nop
	addu	r6,r2,r3
	lw	r4,0044(r30)
	lui	r2,+1062
	ori	r2,r2,00004DD3
	mult	r4,r2
	mfhi	r2
	sra	r3,r2,06
	sra	r2,r4,1F
	subu	r2,r3,r2
	sw	r2,0010(sp)
	sw	r17,0014(sp)
	lw	r4,0068(r30)
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000058A0
	lw	r6,0000(r6)
	addiu	r7,r30,+00000020
	lw	r25,-7E9C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r4,0068(r30)
	lw	r25,-7E54(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r2,-7FE8(r28)
	nop
	addiu	r2,r2,+00000824
	lw	r2,0000(r2)
	nop
	addiu	r2,r2,+00000001
	lw	r1,-7FE8(r28)
	nop
	addiu	r1,r1,+00000824
	sw	r2,0000(r1)
	lw	r2,-7E38(r28)
	nop
	lw	r2,0000(r2)
	nop
	beq	r2,r0,00406BCC
	nop

l00406AEC:
	lw	r2,-7E38(r28)
	nop
	lw	r2,0000(r2)
	nop
	sw	r2,0048(r30)
	lw	r2,0048(r30)
	nop
	bgez	r2,00406B20
	nop

l00406B10:
	lw	r3,0048(r30)
	nop
	addiu	r3,r3,+000003FF
	sw	r3,0048(r30)

l00406B20:
	lw	r3,0048(r30)
	nop
	sra	r2,r3,0A
	lw	r3,-7FE8(r28)
	nop
	addiu	r3,r3,+00000824
	lw	r3,0000(r3)
	nop
	slt	r2,r2,r3
	beq	r2,r0,00406BCC
	nop

l00406B4C:
	lw	r4,0068(r30)
	lw	r25,-7F1C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r3,-7E38(r28)
	nop
	lw	r3,0000(r3)
	nop
	slt	r2,r2,r3
	bne	r2,r0,00406BA4
	nop

l00406B84:
	lw	r4,0068(r30)
	lw	r25,-7DF0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	beq	r0,r0,00406BCC
	nop

l00406BA4:
	lw	r2,-7FE8(r28)
	nop
	addiu	r2,r2,+00000824
	lw	r2,0000(r2)
	nop
	addiu	r2,r2,-0000000A
	lw	r1,-7FE8(r28)
	nop
	addiu	r1,r1,+00000824
	sw	r2,0000(r1)

l00406BCC:
	or	sp,r30,r0
	lw	ra,0060(sp)
	lw	r30,005C(sp)
	lw	r17,0058(sp)
	lw	r16,0054(sp)
	jr	ra
	addiu	sp,sp,+00000068
00406BE8                         00 00 00 00 00 00 00 00         ........

;; server_new: 00406BF0
server_new proc
	lui	r28,%hi(0FC01C70)
	addiu	r28,r28,%lo(0FC01C70)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	sw	r5,0034(r30)
	sw	r6,0038(r30)
	sw	r7,003C(r30)
	addiu	r4,r0,+00000024
	lw	r25,-7E2C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,0018(r30)
	lw	r2,0018(r30)
	nop
	bne	r2,r0,00406C7C
	sw	r2,0018(r30)

l00406C4C:
	or	r4,r0,r0
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005890
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	beq	r0,r0,00406D38
	or	r2,r0,r0

l00406C7C:
	lw	r3,0018(r30)
	addiu	r2,r0,-00000001
	sw	r2,0000(r3)
	lw	r3,0018(r30)
	lw	r2,003C(r30)
	nop
	sw	r2,0020(r3)
	lw	r2,0018(r30)
	nop
	sw	r0,0018(r2)
	lw	r2,0018(r30)
	nop
	sw	r0,001C(r2)
	lw	r3,0018(r30)
	lw	r2,0038(r30)
	nop
	sw	r2,0004(r3)
	lw	r2,0018(r30)
	nop
	addiu	r2,r2,+00000008
	or	r4,r2,r0
	addiu	r5,r0,+00000010
	lw	r25,-7E6C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r3,0018(r30)
	addiu	r2,r0,+00000002
	sh	r2,0008(r3)
	lw	r3,0018(r30)
	lhu	r2,0036(r30)
	nop
	sh	r2,000A(r3)
	lw	r4,0030(r30)
	lw	r25,-7EA8(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	or	r3,r2,r0
	lw	r2,0018(r30)
	nop
	sw	r3,000C(r2)
	lw	r2,0018(r30)
	nop

l00406D38:
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; server_open: 00406D4C
server_open proc
	lui	r28,%hi(0FC01B14)
	addiu	r28,r28,%lo(0FC01B14)
	addu	r28,r28,r25
	addiu	sp,sp,-00000038
	sw	r28,0018(sp)
	sw	ra,0034(sp)
	sw	r30,0030(sp)
	sw	r16,002C(sp)
	or	r30,sp,r0
	sw	r4,0038(r30)
	addiu	r4,r0,+00000002
	addiu	r5,r0,+00000002
	addiu	r6,r0,+00000006
	lw	r25,-7E44(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	sw	r2,0020(r30)
	lw	r2,0020(r30)
	nop
	bgez	r2,00406E40
	nop

l00406DA8:
	lw	r25,-7F68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r16,r2,r0
	lw	r25,-7F68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	lw	r4,0000(r2)
	lw	r25,-7DE4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r3,r2,r0
	lw	r2,0000(r16)
	nop
	sw	r2,0010(sp)
	sw	r3,0014(sp)
	addiu	r4,r0,+00000001
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005864
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005838
	addiu	r7,r0,+00000031
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00406FE0
	or	r2,r0,r0

l00406E40:
	lw	r2,0038(r30)
	nop
	addiu	r2,r2,+00000008
	lw	r4,0020(r30)
	or	r5,r2,r0
	addiu	r6,r0,+00000010
	lw	r25,-7EA4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	bgez	r2,00406F0C
	nop

l00406E74:
	lw	r25,-7F68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r16,r2,r0
	lw	r25,-7F68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	lw	r4,0000(r2)
	lw	r25,-7DE4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r3,r2,r0
	lw	r2,0000(r16)
	nop
	sw	r2,0010(sp)
	sw	r3,0014(sp)
	addiu	r4,r0,+00000001
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-0000582C
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005838
	addiu	r7,r0,+00000037
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00406FE0
	or	r2,r0,r0

l00406F0C:
	lw	r2,0038(r30)
	lw	r4,0020(r30)
	lw	r5,0004(r2)
	lw	r25,-7EF4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	bgez	r2,00406FCC
	nop

l00406F34:
	lw	r25,-7F68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r16,r2,r0
	lw	r25,-7F68(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	lw	r4,0000(r2)
	lw	r25,-7DE4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	or	r3,r2,r0
	lw	r2,0000(r16)
	nop
	sw	r2,0010(sp)
	sw	r3,0014(sp)
	addiu	r4,r0,+00000001
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005804
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005838
	addiu	r7,r0,+00000041
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	beq	r0,r0,00406FE0
	or	r2,r0,r0

l00406FCC:
	lw	r3,0038(r30)
	lw	r2,0020(r30)
	nop
	sw	r2,0000(r3)
	addiu	r2,r0,+00000001

l00406FE0:
	or	sp,r30,r0
	lw	ra,0034(sp)
	lw	r30,0030(sp)
	lw	r16,002C(sp)
	jr	ra
	addiu	sp,sp,+00000038

;; server_close: 00406FF8
server_close proc
	lui	r28,%hi(0FC01868)
	addiu	r28,r28,%lo(0FC01868)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	ra,0020(sp)
	sw	r30,001C(sp)
	or	r30,sp,r0
	sw	r4,0028(r30)
	lw	r2,0028(r30)
	nop
	beq	r2,r0,00407074
	nop

l0040702C:
	lw	r2,0028(r30)
	nop
	lw	r2,0000(r2)
	nop
	blez	r2,00407074
	nop

l00407044:
	lw	r2,0028(r30)
	nop
	lw	r4,0000(r2)
	lw	r25,-7FC4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r3,0028(r30)
	addiu	r2,r0,-00000001
	sw	r2,0000(r3)

l00407074:
	addiu	r2,r0,-00000001
	or	sp,r30,r0
	lw	ra,0020(sp)
	lw	r30,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028

;; server_destroy: 0040708C
server_destroy proc
	lui	r28,%hi(0FC017D4)
	addiu	r28,r28,%lo(0FC017D4)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0018(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,0020(r2)
	nop
	sw	r2,0010(sp)
	addiu	r4,r0,+00000004
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000057E4
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-000057C8
	addiu	r7,r0,+0000005A
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r4,0030(r30)
	lw	r25,-7EEC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r4,0030(r30)
	lw	r25,-7FD4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030
00407148                         00 00 00 00 00 00 00 00         ........

;; slist_new: 00407150
slist_new proc
	lui	r28,%hi(0FC01710)
	addiu	r28,r28,%lo(0FC01710)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	addiu	r4,r0,+0000000C
	lw	r25,-7E2C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	sw	r2,0018(r30)
	lw	r2,0018(r30)
	nop
	bne	r2,r0,004071CC
	sw	r2,0018(r30)

l0040719C:
	or	r4,r0,r0
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-000057B0
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	beq	r0,r0,004071F8
	or	r2,r0,r0

l004071CC:
	lw	r2,0018(r30)
	nop
	sw	r0,0008(r2)
	lw	r2,0018(r30)
	nop
	sw	r0,0000(r2)
	lw	r2,0018(r30)
	nop
	sw	r0,0004(r2)
	lw	r2,0018(r30)
	nop

l004071F8:
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; slist_add: 0040720C
slist_add proc
	lui	r28,%hi(0FC01654)
	addiu	r28,r28,%lo(0FC01654)
	addu	r28,r28,r25
	addiu	sp,sp,-00000010
	sw	r28,0000(sp)
	sw	r30,000C(sp)
	or	r30,sp,r0
	sw	r4,0010(r30)
	sw	r5,0014(r30)
	lw	r2,0010(r30)
	nop
	lw	r2,0008(r2)
	nop
	beq	r2,r0,00407278
	nop

l00407248:
	lw	r2,0010(r30)
	nop
	lw	r3,0008(r2)
	lw	r2,0014(r30)
	nop
	sw	r2,001C(r3)
	lw	r3,0014(r30)
	lw	r2,0010(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0018(r3)

l00407278:
	lw	r2,0014(r30)
	nop
	sw	r0,001C(r2)
	lw	r3,0010(r30)
	lw	r2,0014(r30)
	nop
	sw	r2,0008(r3)
	lw	r2,0010(r30)
	lw	r3,0010(r30)
	lw	r2,0010(r30)
	nop
	lw	r2,0000(r2)
	nop
	addiu	r2,r2,+00000001
	sw	r2,0000(r3)
	lw	r2,0010(r30)
	lw	r3,0010(r30)
	lw	r4,0000(r2)
	lw	r2,0004(r3)
	nop
	slt	r2,r2,r4
	beq	r2,r0,004072EC
	nop

l004072D4:
	lw	r2,0010(r30)
	lw	r3,0010(r30)
	nop
	lw	r3,0000(r3)
	nop
	sw	r3,0004(r2)

l004072EC:
	lw	r2,0010(r30)
	nop
	lw	r2,0000(r2)
	nop
	or	sp,r30,r0
	lw	r30,000C(sp)
	jr	ra
	addiu	sp,sp,+00000010

;; slist_remove: 0040730C
slist_remove proc
	lui	r28,%hi(0FC01554)
	addiu	r28,r28,%lo(0FC01554)
	addu	r28,r28,r25
	addiu	sp,sp,-00000010
	sw	r28,0000(sp)
	sw	r30,000C(sp)
	or	r30,sp,r0
	sw	r4,0010(r30)
	sw	r5,0014(r30)
	lw	r2,0014(r30)
	nop
	beq	r2,r0,004073F0
	nop

l00407340:
	lw	r2,0010(r30)
	lw	r3,0014(r30)
	lw	r2,0008(r2)
	nop
	bne	r3,r2,00407378
	nop

l00407358:
	lw	r2,0010(r30)
	lw	r3,0014(r30)
	nop
	lw	r3,0018(r3)
	nop
	sw	r3,0008(r2)
	beq	r0,r0,00407398
	nop

l00407378:
	lw	r2,0014(r30)
	nop
	lw	r3,001C(r2)
	lw	r2,0014(r30)
	nop
	lw	r2,0018(r2)
	nop
	sw	r2,0018(r3)

l00407398:
	lw	r2,0014(r30)
	nop
	lw	r2,0018(r2)
	nop
	beq	r2,r0,004073D0
	nop

l004073B0:
	lw	r2,0014(r30)
	nop
	lw	r3,0018(r2)
	lw	r2,0014(r30)
	nop
	lw	r2,001C(r2)
	nop
	sw	r2,001C(r3)

l004073D0:
	lw	r2,0010(r30)
	lw	r3,0010(r30)
	lw	r2,0010(r30)
	nop
	lw	r2,0000(r2)
	nop
	addiu	r2,r2,-00000001
	sw	r2,0000(r3)

l004073F0:
	lw	r2,0010(r30)
	nop
	lw	r2,0000(r2)
	nop
	or	sp,r30,r0
	lw	r30,000C(sp)
	jr	ra
	addiu	sp,sp,+00000010

;; slist_find_fd: 00407410
slist_find_fd proc
	lui	r28,%hi(0FC01450)
	addiu	r28,r28,%lo(0FC01450)
	addu	r28,r28,r25
	addiu	sp,sp,-00000018
	sw	r28,0000(sp)
	sw	r30,0014(sp)
	or	r30,sp,r0
	sw	r4,0018(r30)
	sw	r5,001C(r30)
	lw	r2,0018(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0008(r30)

l00407448:
	lw	r2,0008(r30)
	nop
	beq	r2,r0,00407494
	nop

l00407458:
	lw	r2,0008(r30)
	nop
	lw	r3,0000(r2)
	lw	r2,001C(r30)
	nop
	bne	r3,r2,0040747C
	nop

l00407474:
	beq	r0,r0,00407494
	nop

l0040747C:
	lw	r2,0008(r30)
	nop
	lw	r2,0018(r2)
	nop
	beq	r0,r0,00407448
	sw	r2,0008(r30)

l00407494:
	lw	r2,0008(r30)
	nop
	or	sp,r30,r0
	lw	r30,0014(sp)
	jr	ra
	addiu	sp,sp,+00000018

;; slist_delete: 004074AC
slist_delete proc
	lui	r28,%hi(0FC013B4)
	addiu	r28,r28,%lo(0FC013B4)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0018(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	sw	r5,0034(r30)
	lw	r4,0030(r30)
	lw	r5,0034(r30)
	lw	r25,-7EF0(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r2,0034(r30)
	nop
	sw	r2,0010(sp)
	addiu	r4,r0,+00000002
	lw	r5,-7FE0(r28)
	nop
	addiu	r5,r5,-00005784
	lw	r6,-7FE0(r28)
	nop
	addiu	r6,r6,-00005768
	addiu	r7,r0,+00000055
	lw	r25,-7E74(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	lw	r4,0034(r30)
	lw	r25,-7E60(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0018(r30)
	nop
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; slist_close_all: 00407568
slist_close_all proc
	lui	r28,%hi(0FC012F8)
	addiu	r28,r28,%lo(0FC012F8)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0018(r30)

l004075A0:
	lw	r2,0018(r30)
	nop
	bne	r2,r0,004075B8
	nop

l004075B0:
	beq	r0,r0,004075EC
	nop

l004075B8:
	lw	r4,0018(r30)
	lw	r25,-7EEC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r2,0018(r30)
	nop
	lw	r2,0018(r2)
	nop
	beq	r0,r0,004075A0
	sw	r2,0018(r30)

l004075EC:
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; slist_destroy_all: 00407600
slist_destroy_all proc
	lui	r28,%hi(0FC01260)
	addiu	r28,r28,%lo(0FC01260)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	ra,0028(sp)
	sw	r30,0024(sp)
	or	r30,sp,r0
	sw	r4,0030(r30)
	lw	r2,0030(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0018(r30)

l00407638:
	lw	r2,0030(r30)
	nop
	lw	r2,0008(r2)
	nop
	bne	r2,r0,00407658
	nop

l00407650:
	beq	r0,r0,00407694
	nop

l00407658:
	lw	r2,0030(r30)
	nop
	lw	r2,0008(r2)
	nop
	sw	r2,0018(r30)
	lw	r4,0030(r30)
	lw	r5,0018(r30)
	lw	r25,-7F80(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	beq	r0,r0,00407638
	nop

l00407694:
	or	sp,r30,r0
	lw	ra,0028(sp)
	lw	r30,0024(sp)
	jr	ra
	addiu	sp,sp,+00000030

;; slist_destroy: 004076A8
slist_destroy proc
	lui	r28,%hi(0FC011B8)
	addiu	r28,r28,%lo(0FC011B8)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	ra,0020(sp)
	sw	r30,001C(sp)
	or	r30,sp,r0
	sw	r4,0028(r30)
	lw	r4,0028(r30)
	lw	r25,-7EDC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r4,0028(r30)
	lw	r25,-7E5C(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	lw	r4,0028(r30)
	lw	r25,-7FD4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(r30)
	nop
	or	sp,r30,r0
	lw	ra,0020(sp)
	lw	r30,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028
00407734             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __fixunsdfsi: 00407740
__fixunsdfsi proc
	lui	r28,%hi(0FC01120)
	addiu	r28,r28,%lo(0FC01120)
	addu	r28,r28,r25
	lui	r1,+41E0
	mtc1	r1,f1
	mtc1	r0,f0
	nop
	c.le.d	cc0,f0,f12
	cfc1	r3,FCSR
	nop
	ori	r1,r3,00000003
	xori	r1,r1,00000002
	ctc1	r1,FCSR
	nop
	cvt.w.d	f0,f12
	ctc1	r3,FCSR
	nop
	mfc1	r2,f0
	bc1f	cc0,004077D4
	nop

l00407790:
	lui	r1,-3E20
	mtc1	r1,f1
	mtc1	r0,f0
	lui	r2,-8000
	add.d	f0,f12,f0
	cfc1	r3,FCSR
	nop
	ori	r1,r3,00000003
	xori	r1,r1,00000002
	ctc1	r1,FCSR
	nop
	cvt.w.d	f2,f0
	ctc1	r3,FCSR
	nop
	mfc1	r3,f2
	nop
	addu	r2,r3,r2

l004077D4:
	jr	ra
	nop
004077DC                                     00 00 00 00             ....

;; _fpadd_parts: 004077E0
_fpadd_parts proc
	lui	r28,%hi(0FC01080)
	addiu	r28,r28,%lo(0FC01080)
	addu	r28,r28,r25
	or	r11,r4,r0
	lw	r7,0000(r11)
	nop
	sltiu	r3,r7,+00000002
	bne	r3,r0,00407A30
	or	r2,r11,r0

l00407804:
	lw	r4,0000(r5)
	nop
	sltiu	r3,r4,+00000002
	bne	r3,r0,00407A30
	or	r2,r5,r0

l00407818:
	xori	r2,r7,00000004
	bne	r2,r0,0040784C
	xori	r3,r4,00000004

l00407824:
	xori	r2,r4,00000004
	bne	r2,r0,00407844
	nop

l00407830:
	lw	r4,0004(r5)
	lw	r3,0004(r11)
	lw	r2,-7F04(r28)
	bne	r3,r4,00407A30
	nop

l00407844:
	jr	ra
	or	r2,r11,r0

l0040784C:
	beq	r3,r0,00407A30
	or	r2,r5,r0

l00407854:
	xori	r2,r4,00000002
	bne	r2,r0,004078B4
	xori	r3,r7,00000002

l00407860:
	xori	r2,r7,00000002
	bne	r2,r0,004078AC
	nop

l0040786C:
	sw	r7,0000(r6)
	lw	r4,0004(r11)
	or	r2,r6,r0
	sw	r4,0004(r6)
	lw	r3,0008(r11)
	nop
	sw	r3,0008(r6)
	lw	r4,000C(r11)
	nop
	sw	r4,000C(r6)
	lw	r3,0004(r11)
	lw	r4,0004(r5)
	nop
	and	r3,r3,r4
	jr	ra
	sw	r3,0004(r6)

l004078AC:
	jr	ra
	or	r2,r11,r0

l004078B4:
	beq	r3,r0,00407A30
	or	r2,r5,r0

l004078BC:
	lw	r8,0008(r11)
	lw	r7,0008(r5)
	lw	r10,000C(r11)
	subu	r2,r8,r7
	lw	r9,000C(r5)
	bgez	r2,004078DC
	nop

l004078D8:
	subu	r2,r0,r2

l004078DC:
	slti	r2,r2,+00000020
	beq	r2,r0,0040793C
	slt	r2,r7,r8

l004078E8:
	beq	r2,r0,00407910
	slt	r2,r8,r7

l004078F0:
	subu	r7,r8,r7

l004078F4:
	srl	r3,r9,01
	andi	r2,r9,00000001
	addiu	r7,r7,-00000001
	bne	r7,r0,004078F4
	or	r9,r2,r3

l00407908:
	or	r7,r8,r0
	slt	r2,r8,r7

l00407910:
	nop
	beq	r2,r0,00407958
	nop

l0040791C:
	addiu	r8,r8,+00000001
	srl	r2,r10,01
	andi	r3,r10,00000001
	slt	r4,r8,r7
	bne	r4,r0,0040791C
	or	r10,r3,r2

l00407934:
	beq	r0,r0,00407958
	nop

l0040793C:
	nop
	beq	r2,r0,00407950
	nop

l00407948:
	beq	r0,r0,00407958
	or	r9,r0,r0

l00407950:
	or	r8,r7,r0
	or	r10,r0,r0

l00407958:
	lw	r4,0004(r11)
	lw	r2,0004(r5)
	nop
	beq	r4,r2,004079F0
	nop

l0040796C:
	bne	r4,r0,00407978
	subu	r3,r9,r10

l00407974:
	subu	r3,r10,r9

l00407978:
	bltz	r3,00407990
	addiu	r2,r0,+00000001

l00407980:
	sw	r8,0008(r6)
	sw	r3,000C(r6)
	beq	r0,r0,004079A0
	sw	r0,0004(r6)

l00407990:
	subu	r3,r0,r3
	sw	r2,0004(r6)
	sw	r8,0008(r6)
	sw	r3,000C(r6)

l004079A0:
	lw	r7,000C(r6)
	lui	r2,+3FFF
	addiu	r3,r7,-00000001
	ori	r2,r2,0000FFFE
	sltu	r2,r2,r3
	bne	r2,r0,00407A08
	addiu	r2,r0,+00000003

l004079BC:
	sll	r5,r7,01
	lw	r2,0008(r6)
	lui	r3,+3FFF
	addiu	r4,r5,-00000001
	ori	r3,r3,0000FFFE
	addiu	r2,r2,-00000001
	sltu	r3,r3,r4
	sw	r2,0008(r6)
	sw	r5,000C(r6)
	beq	r3,r0,004079BC
	or	r7,r5,r0

l004079E8:
	beq	r0,r0,00407A08
	addiu	r2,r0,+00000003

l004079F0:
	addu	r2,r10,r9
	sw	r4,0004(r6)
	sw	r8,0008(r6)
	sw	r2,000C(r6)
	or	r7,r2,r0
	addiu	r2,r0,+00000003

l00407A08:
	bgez	r7,00407A2C
	sw	r2,0000(r6)

l00407A10:
	lw	r2,0008(r6)
	srl	r4,r7,01
	andi	r3,r7,00000001
	or	r3,r3,r4
	addiu	r2,r2,+00000001
	sw	r3,000C(r6)
	sw	r2,0008(r6)

l00407A2C:
	or	r2,r6,r0

l00407A30:
	jr	ra
	nop

;; __addsf3: 00407A38
__addsf3 proc
	lui	r28,%hi(0FC00E28)
	addiu	r28,r28,%lo(0FC00E28)
	addu	r28,r28,r25
	addiu	sp,sp,-00000060
	sw	r28,0010(sp)
	sw	r17,0058(sp)
	lw	r17,-7E84(r28)
	sw	ra,005C(sp)
	sw	r16,0054(sp)
	addiu	r4,sp,+00000048
	addiu	r5,sp,+00000018
	swc1	f12,0048(sp)
	or	r25,r17,r0
	jalr	ra,r25
	swc1	f14,004C(sp)
	lw	r28,0010(sp)
	addiu	r16,sp,+00000028
	addiu	r4,sp,+0000004C
	or	r25,r17,r0
	jalr	ra,r25
	or	r5,r16,r0
	lw	r28,0010(sp)
	nop
	lw	r25,-7FE4(r28)
	nop
	addiu	r25,r25,+000077E0
	or	r5,r16,r0
	addiu	r4,sp,+00000018
	jalr	ra,r25
	addiu	r6,sp,+00000038
	lw	r28,0010(sp)
	nop
	lw	r25,-7EB4(r28)
	nop
	jalr	ra,r25
	or	r4,r2,r0
	lw	r28,0010(sp)
	lw	ra,005C(sp)
	lw	r17,0058(sp)
	lw	r16,0054(sp)
	jr	ra
	addiu	sp,sp,+00000060

;; __subsf3: 00407AE0
__subsf3 proc
	lui	r28,%hi(0FC00D80)
	addiu	r28,r28,%lo(0FC00D80)
	addu	r28,r28,r25
	addiu	sp,sp,-00000060
	sw	r28,0010(sp)
	sw	r17,0058(sp)
	lw	r17,-7E84(r28)
	sw	ra,005C(sp)
	sw	r16,0054(sp)
	addiu	r4,sp,+00000048
	addiu	r5,sp,+00000018
	swc1	f12,0048(sp)
	or	r25,r17,r0
	jalr	ra,r25
	swc1	f14,004C(sp)
	lw	r28,0010(sp)
	addiu	r16,sp,+00000028
	addiu	r4,sp,+0000004C
	or	r25,r17,r0
	jalr	ra,r25
	or	r5,r16,r0
	lw	r28,0010(sp)
	nop
	lw	r25,-7FE4(r28)
	nop
	addiu	r25,r25,+000077E0
	lw	r2,002C(sp)
	or	r5,r16,r0
	xori	r2,r2,00000001
	addiu	r4,sp,+00000018
	addiu	r6,sp,+00000038
	jalr	ra,r25
	sw	r2,002C(sp)
	lw	r28,0010(sp)
	nop
	lw	r25,-7EB4(r28)
	nop
	jalr	ra,r25
	or	r4,r2,r0
	lw	r28,0010(sp)
	lw	ra,005C(sp)
	lw	r17,0058(sp)
	lw	r16,0054(sp)
	jr	ra
	addiu	sp,sp,+00000060
00407B94             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __mulsf3: 00407BA0
__mulsf3 proc
	lui	r28,%hi(0FC00CC0)
	addiu	r28,r28,%lo(0FC00CC0)
	addu	r28,r28,r25
	addiu	sp,sp,-00000060
	sw	r28,0010(sp)
	sw	r17,0058(sp)
	lw	r17,-7E84(r28)
	sw	ra,005C(sp)
	sw	r16,0054(sp)
	addiu	r4,sp,+00000048
	addiu	r5,sp,+00000018
	swc1	f12,0048(sp)
	or	r25,r17,r0
	jalr	ra,r25
	swc1	f14,004C(sp)
	lw	r28,0010(sp)
	addiu	r16,sp,+00000028
	addiu	r4,sp,+0000004C
	or	r25,r17,r0
	jalr	ra,r25
	or	r5,r16,r0
	lw	r28,0010(sp)
	lw	r4,0018(sp)
	nop
	sltiu	r2,r4,+00000002
	bne	r2,r0,00407C64
	addiu	r7,sp,+00000018

l00407C0C:
	lw	r3,0028(sp)
	nop
	sltiu	r2,r3,+00000002
	bne	r2,r0,00407C8C
	xori	r2,r4,00000004

l00407C20:
	bne	r2,r0,00407C40
	xori	r2,r3,00000004

l00407C28:
	xori	r2,r3,00000002
	lw	r4,-7F04(r28)
	beq	r2,r0,00407DA4
	nop

l00407C38:
	beq	r0,r0,00407C64
	nop

l00407C40:
	bne	r2,r0,00407C5C
	xori	r2,r4,00000002

l00407C48:
	lw	r4,-7F04(r28)
	beq	r2,r0,00407DA4
	nop

l00407C54:
	beq	r0,r0,00407C8C
	nop

l00407C5C:
	bne	r2,r0,00407C80
	xori	r2,r3,00000002

l00407C64:
	lw	r2,0004(r7)
	lw	r3,002C(sp)
	or	r4,r7,r0
	xor	r2,r2,r3
	sltu	r2,r0,r2
	beq	r0,r0,00407DA4
	sw	r2,0004(r7)

l00407C80:
	nop
	bne	r2,r0,00407CA8
	nop

l00407C8C:
	lw	r2,0004(r7)
	lw	r3,002C(sp)
	or	r4,r16,r0
	xor	r2,r2,r3
	sltu	r2,r0,r2
	beq	r0,r0,00407DA4
	sw	r2,002C(sp)

l00407CA8:
	lw	r4,000C(r7)
	lw	r6,0034(sp)
	lw	r2,0004(r7)
	multu	r4,r6
	lw	r3,0008(r7)
	lw	r7,002C(sp)
	lw	r6,0030(sp)
	xor	r2,r2,r7
	addu	r3,r3,r6
	sltu	r2,r0,r2
	addiu	r7,r3,+00000002
	sw	r2,003C(sp)
	sw	r7,0040(sp)
	mfhi	r4
	mflo	r5
	sra	r3,r4,00
	sra	r2,r4,1F
	or	r6,r3,r0
	bgez	r6,00407D24
	or	r4,r5,r0

l00407CF8:
	or	r3,r7,r0
	lui	r5,-8000
	andi	r2,r6,00000001

l00407D04:
	addiu	r3,r3,+00000001
	beq	r2,r0,00407D18
	srl	r6,r6,01

l00407D10:
	srl	r4,r4,01
	or	r4,r4,r5

l00407D18:
	bltz	r6,00407D04
	andi	r2,r6,00000001

l00407D20:
	sw	r3,0040(sp)

l00407D24:
	lui	r2,+3FFF
	ori	r2,r2,0000FFFF
	sltu	r2,r2,r6
	bne	r2,r0,00407D6C
	nop

l00407D38:
	lui	r5,+3FFF
	lw	r3,0040(sp)
	lui	r7,-8000
	ori	r5,r5,0000FFFF

l00407D48:
	and	r2,r4,r7
	sll	r6,r6,01
	beq	r2,r0,00407D5C
	addiu	r3,r3,-00000001

l00407D58:
	ori	r6,r6,00000001

l00407D5C:
	sltu	r2,r5,r6
	beq	r2,r0,00407D48
	sll	r4,r4,01

l00407D68:
	sw	r3,0040(sp)

l00407D6C:
	andi	r3,r6,0000007F
	addiu	r2,r0,+00000040
	bne	r3,r2,00407D98
	addiu	r2,r0,+00000003

l00407D7C:
	andi	r2,r6,00000080
	bne	r2,r0,00407D90
	nop

l00407D88:
	beq	r4,r0,00407D98
	addiu	r2,r0,+00000003

l00407D90:
	addiu	r6,r6,+00000040
	addiu	r2,r0,+00000003

l00407D98:
	sw	r6,0044(sp)
	sw	r2,0038(sp)
	addiu	r4,sp,+00000038

l00407DA4:
	lw	r25,-7EB4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(sp)
	lw	ra,005C(sp)
	lw	r17,0058(sp)
	lw	r16,0054(sp)
	jr	ra
	addiu	sp,sp,+00000060
00407DCC                                     00 00 00 00             ....

;; __divsf3: 00407DD0
__divsf3 proc
	lui	r28,%hi(0FC00A90)
	addiu	r28,r28,%lo(0FC00A90)
	addu	r28,r28,r25
	addiu	sp,sp,-00000050
	sw	r28,0010(sp)
	sw	r17,0048(sp)
	lw	r17,-7E84(r28)
	sw	ra,004C(sp)
	sw	r16,0044(sp)
	addiu	r4,sp,+00000038
	addiu	r5,sp,+00000018
	swc1	f12,0038(sp)
	or	r25,r17,r0
	jalr	ra,r25
	swc1	f14,003C(sp)
	lw	r28,0010(sp)
	addiu	r16,sp,+00000028
	addiu	r4,sp,+0000003C
	or	r25,r17,r0
	jalr	ra,r25
	or	r5,r16,r0
	lw	r28,0010(sp)
	lw	r6,0018(sp)
	addiu	r8,sp,+00000018
	sltiu	r2,r6,+00000002
	bne	r2,r0,00407F3C
	or	r4,r8,r0

l00407E3C:
	lw	r5,0028(sp)
	nop
	sltiu	r2,r5,+00000002
	bne	r2,r0,00407F3C
	or	r4,r16,r0

l00407E50:
	lw	r2,0004(r8)
	lw	r3,002C(sp)
	xori	r4,r6,00000004
	xor	r2,r2,r3
	beq	r4,r0,00407E74
	sw	r2,0004(r8)

l00407E68:
	xori	r2,r6,00000002
	bne	r2,r0,00407E88
	xori	r2,r5,00000004

l00407E74:
	lw	r4,-7F04(r28)
	beq	r6,r5,00407F3C
	nop

l00407E80:
	beq	r0,r0,00407F3C
	or	r4,r8,r0

l00407E88:
	bne	r2,r0,00407EA0
	xori	r2,r5,00000002

l00407E90:
	sw	r0,000C(r8)
	sw	r0,0008(r8)
	beq	r0,r0,00407F3C
	or	r4,r8,r0

l00407EA0:
	bne	r2,r0,00407EB4
	addiu	r2,r0,+00000004

l00407EA8:
	sw	r2,0018(sp)
	beq	r0,r0,00407F3C
	or	r4,r8,r0

l00407EB4:
	lw	r3,0008(r8)
	lw	r2,0030(sp)
	lw	r7,0034(sp)
	lw	r4,000C(r8)
	subu	r2,r3,r2
	sltu	r5,r4,r7
	beq	r5,r0,00407EE4
	sw	r2,0008(r8)

l00407ED4:
	addiu	r2,r2,-00000001
	sll	r4,r4,01
	sw	r2,0008(r8)
	sltu	r5,r4,r7

l00407EE4:
	lui	r2,+4000
	or	r6,r0,r0

l00407EEC:
	nop
	bne	r5,r0,00407F00
	nop

l00407EF8:
	or	r6,r6,r2
	subu	r4,r4,r7

l00407F00:
	sll	r4,r4,01
	srl	r2,r2,01
	bne	r2,r0,00407EEC
	sltu	r5,r4,r7

l00407F10:
	andi	r3,r6,0000007F
	addiu	r2,r0,+00000040
	bne	r3,r2,00407F34
	andi	r2,r6,00000080

l00407F20:
	bne	r2,r0,00407F30
	nop

l00407F28:
	beq	r4,r0,00407F34
	nop

l00407F30:
	addiu	r6,r6,+00000040

l00407F34:
	sw	r6,000C(r8)
	or	r4,r8,r0

l00407F3C:
	lw	r25,-7EB4(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(sp)
	lw	ra,004C(sp)
	lw	r17,0048(sp)
	lw	r16,0044(sp)
	jr	ra
	addiu	sp,sp,+00000050
00407F64             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __floatsisf: 00407F70
__floatsisf proc
	lui	r28,%hi(0FC008F0)
	addiu	r28,r28,%lo(0FC008F0)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	addiu	r2,r0,+00000003
	srl	r3,r4,1F
	sw	ra,002C(sp)
	sw	r2,0018(sp)
	bne	r4,r0,00407FA8
	sw	r3,001C(sp)

l00407F9C:
	addiu	r2,r0,+00000002
	beq	r0,r0,00408014
	sw	r2,0018(sp)

l00407FA8:
	addiu	r2,r0,+0000001E
	beq	r3,r0,00407FD0
	sw	r2,0020(sp)

l00407FB4:
	lui	r2,-8000
	lui	r1,-3100
	mtc1	r1,f0
	beq	r4,r2,00408028
	subu	r2,r0,r4

l00407FC8:
	beq	r0,r0,00407FD4
	sw	r2,0024(sp)

l00407FD0:
	sw	r4,0024(sp)

l00407FD4:
	lw	r6,0024(sp)
	lui	r2,+3FFF
	ori	r2,r2,0000FFFF
	sltu	r2,r2,r6
	bne	r2,r0,00408014
	nop

l00407FEC:
	lui	r5,+3FFF
	lw	r4,0020(sp)
	ori	r5,r5,0000FFFF

l00407FF8:
	sll	r3,r6,01
	sltu	r2,r5,r3
	addiu	r4,r4,-00000001
	beq	r2,r0,00407FF8
	or	r6,r3,r0

l0040800C:
	sw	r4,0020(sp)
	sw	r3,0024(sp)

l00408014:
	lw	r25,-7EB4(r28)
	nop
	jalr	ra,r25
	addiu	r4,sp,+00000018
	lw	r28,0010(sp)

l00408028:
	lw	ra,002C(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000030
00408038                         00 00 00 00 00 00 00 00         ........

;; __fixsfsi: 00408040
__fixsfsi proc
	lui	r28,%hi(0FC00820)
	addiu	r28,r28,%lo(0FC00820)
	addu	r28,r28,r25
	addiu	sp,sp,-00000038
	sw	r28,0010(sp)
	lw	r25,-7E84(r28)
	addiu	r4,sp,+00000028
	addiu	r5,sp,+00000018
	sw	ra,0034(sp)
	jalr	ra,r25
	swc1	f12,0028(sp)
	lw	r28,0010(sp)
	lw	r4,0018(sp)
	nop
	xori	r3,r4,00000002
	beq	r3,r0,004080E8
	or	r2,r0,r0

l00408084:
	sltiu	r3,r4,+00000002
	bne	r3,r0,004080E8
	nop

l00408090:
	xori	r2,r4,00000004
	beq	r2,r0,004080B8
	nop

l0040809C:
	lw	r5,0020(sp)
	nop
	bltz	r5,004080E8
	or	r2,r0,r0

l004080AC:
	slti	r2,r5,+0000001F
	bne	r2,r0,004080CC
	nop

l004080B8:
	lw	r2,001C(sp)
	lui	r3,-8000
	sltiu	r2,r2,+00000001
	beq	r0,r0,004080E8
	subu	r2,r3,r2

l004080CC:
	addiu	r2,r0,+0000001E
	lw	r3,0024(sp)
	lw	r4,001C(sp)
	subu	r2,r2,r5
	beq	r4,r0,004080E8
	srlv	r2,r3,r2

l004080E4:
	subu	r2,r0,r2

l004080E8:
	lw	ra,0034(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000038
004080F8                         00 00 00 00 00 00 00 00         ........

;; __extendsfdf2: 00408100
__extendsfdf2 proc
	lui	r28,%hi(0FC00760)
	addiu	r28,r28,%lo(0FC00760)
	addu	r28,r28,r25
	addiu	sp,sp,-00000040
	sw	r28,0018(sp)
	sw	ra,003C(sp)
	lw	r25,-7E84(r28)
	addiu	r4,sp,+00000030
	addiu	r5,sp,+00000020
	jalr	ra,r25
	swc1	f12,0030(sp)
	lw	r28,0018(sp)
	or	r2,r0,r0
	lw	r3,002C(sp)
	lw	r4,0020(sp)
	sll	r2,r2,1E
	srl	r7,r3,02
	or	r2,r2,r7
	sll	r3,r3,1E
	lw	r5,0024(sp)
	lw	r6,0028(sp)
	lw	r25,-7DF8(r28)
	sw	r2,0010(sp)
	jalr	ra,r25
	sw	r3,0014(sp)
	lw	r28,0018(sp)
	lw	ra,003C(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000040
00408178                         00 00 00 00 00 00 00 00         ........

;; _fpadd_parts: 00408180
_fpadd_parts proc
	lui	r28,%hi(0FC006E0)
	addiu	r28,r28,%lo(0FC006E0)
	addu	r28,r28,r25
	or	r24,r4,r0
	lw	r7,0000(r24)
	or	r25,r5,r0
	sltiu	r3,r7,+00000002
	or	r13,r6,r0
	bne	r3,r0,004084F4
	or	r2,r24,r0

l004081A8:
	lw	r4,0000(r25)
	nop
	sltiu	r3,r4,+00000002
	bne	r3,r0,004084F4
	or	r2,r25,r0

l004081BC:
	xori	r2,r7,00000004
	bne	r2,r0,004081F0
	xori	r3,r4,00000004

l004081C8:
	xori	r2,r4,00000004
	bne	r2,r0,004081E8
	nop

l004081D4:
	lw	r4,0004(r25)
	lw	r3,0004(r24)
	lw	r2,-7F3C(r28)
	bne	r3,r4,004084F4
	nop

l004081E8:
	jr	ra
	or	r2,r24,r0

l004081F0:
	beq	r3,r0,004084F4
	or	r2,r25,r0

l004081F8:
	xori	r2,r4,00000002
	bne	r2,r0,00408270
	xori	r3,r7,00000002

l00408204:
	xori	r2,r7,00000002
	bne	r2,r0,00408268
	nop

l00408210:
	sw	r7,0000(r13)
	lw	r4,0004(r24)
	or	r2,r13,r0
	sw	r4,0004(r13)
	lw	r3,0008(r24)
	nop
	sw	r3,0008(r13)
	lw	r4,000C(r24)
	nop
	sw	r4,000C(r13)
	lw	r3,0010(r24)
	nop
	sw	r3,0010(r13)
	lw	r4,0014(r24)
	nop
	sw	r4,0014(r13)
	lw	r3,0004(r24)
	lw	r4,0004(r25)
	nop
	and	r3,r3,r4
	jr	ra
	sw	r3,0004(r13)

l00408268:
	jr	ra
	or	r2,r24,r0

l00408270:
	beq	r3,r0,004084F4
	or	r2,r25,r0

l00408278:
	lw	r12,0008(r24)
	lw	r7,0008(r25)
	lw	r10,0010(r24)
	lw	r11,0014(r24)
	subu	r2,r12,r7
	lw	r8,0010(r25)
	lw	r9,0014(r25)
	bgez	r2,004082A0
	nop

l0040829C:
	subu	r2,r0,r2

l004082A0:
	slti	r2,r2,+00000040
	beq	r2,r0,00408338
	slt	r2,r7,r12

l004082AC:
	beq	r2,r0,004082F0
	slt	r2,r12,r7

l004082B4:
	addiu	r14,r0,+00000000
	addiu	r15,r0,+00000001
	subu	r7,r12,r7

l004082C0:
	srl	r5,r9,01
	sll	r6,r8,1F
	and	r2,r8,r14
	or	r5,r5,r6
	and	r3,r9,r15
	srl	r4,r8,01
	addiu	r7,r7,-00000001
	or	r8,r2,r4
	bne	r7,r0,004082C0
	or	r9,r3,r5

l004082E8:
	or	r7,r12,r0
	slt	r2,r12,r7

l004082F0:
	nop
	beq	r2,r0,0040835C
	nop

l004082FC:
	addiu	r14,r0,+00000000
	addiu	r15,r0,+00000001

l00408304:
	sll	r6,r10,1F
	srl	r5,r11,01
	addiu	r12,r12,+00000001
	or	r5,r5,r6
	and	r2,r10,r14
	srl	r4,r10,01
	and	r3,r11,r15
	slt	r6,r12,r7
	or	r10,r2,r4
	bne	r6,r0,00408304
	or	r11,r3,r5

l00408330:
	beq	r0,r0,0040835C
	nop

l00408338:
	nop
	beq	r2,r0,00408350
	nop

l00408344:
	or	r8,r0,r0
	beq	r0,r0,0040835C
	or	r9,r0,r0

l00408350:
	or	r12,r7,r0
	or	r10,r0,r0
	or	r11,r0,r0

l0040835C:
	lw	r6,0004(r24)
	lw	r2,0004(r25)
	nop
	beq	r6,r2,00408470
	sltu	r2,r9,r11

l00408370:
	subu	r5,r9,r11
	subu	r4,r8,r10
	bne	r6,r0,00408390
	subu	r4,r4,r2

l00408380:
	sltu	r2,r11,r9
	subu	r5,r11,r9
	subu	r4,r10,r8
	subu	r4,r4,r2

l00408390:
	bltz	r4,004083AC
	addiu	r2,r0,+00000001

l00408398:
	sw	r12,0008(r13)
	sw	r4,0010(r13)
	sw	r5,0014(r13)
	beq	r0,r0,004083CC
	sw	r0,0004(r13)

l004083AC:
	subu	r5,r0,r5
	subu	r4,r0,r4
	sltu	r3,r0,r5
	subu	r4,r4,r3
	sw	r2,0004(r13)
	sw	r12,0008(r13)
	sw	r4,0010(r13)
	sw	r5,0014(r13)

l004083CC:
	lw	r8,0010(r13)
	lw	r9,0014(r13)
	lui	r3,+0FFF
	addiu	r5,r9,-00000001
	sltiu	r2,r5,-00000001
	addiu	r4,r8,-00000001
	addu	r4,r4,r2
	ori	r3,r3,0000FFFF
	sltu	r2,r3,r4
	bne	r2,r0,00408490
	nop

l004083F8:
	bne	r4,r3,00408410
	srl	r2,r9,1F

l00408400:
	sltiu	r2,r5,-00000001
	beq	r2,r0,00408490
	nop

l0040840C:
	srl	r2,r9,1F

l00408410:
	sll	r6,r8,01

l00408414:
	or	r6,r6,r2
	lw	r3,0008(r13)
	sll	r7,r9,01
	lui	r10,+0FFF
	addiu	r5,r7,-00000001
	sltiu	r2,r5,-00000001
	addiu	r4,r6,-00000001
	addu	r4,r4,r2
	ori	r10,r10,0000FFFF
	addiu	r3,r3,-00000001
	sltu	r2,r10,r4
	sw	r3,0008(r13)
	sw	r6,0010(r13)
	sw	r7,0014(r13)
	bne	r2,r0,00408490
	sltiu	r11,r5,-00000001

l00408454:
	or	r8,r6,r0
	bne	r4,r10,0040840C
	or	r9,r7,r0

l00408460:
	beq	r11,r0,00408490
	srl	r2,r9,1F

l00408468:
	beq	r0,r0,00408414
	sll	r6,r8,01

l00408470:
	addu	r3,r11,r9
	sltu	r4,r3,r9
	addu	r2,r10,r8
	addu	r2,r2,r4
	sw	r6,0004(r13)
	sw	r12,0008(r13)
	sw	r2,0010(r13)
	sw	r3,0014(r13)

l00408490:
	lw	r2,0010(r13)
	lui	r3,+1FFF
	ori	r3,r3,0000FFFF
	addiu	r4,r0,+00000003
	sltu	r3,r3,r2
	beq	r3,r0,004084F0
	sw	r4,0000(r13)

l004084AC:
	lw	r4,0010(r13)
	lw	r5,0014(r13)
	addiu	r2,r0,+00000000
	addiu	r3,r0,+00000001
	and	r2,r4,r2
	and	r3,r5,r3
	srl	r5,r5,01
	sll	r7,r4,1F
	or	r5,r5,r7
	lw	r6,0008(r13)
	srl	r4,r4,01
	or	r2,r2,r4
	or	r3,r3,r5
	addiu	r6,r6,+00000001
	sw	r2,0010(r13)
	sw	r3,0014(r13)
	sw	r6,0008(r13)

l004084F0:
	or	r2,r13,r0

l004084F4:
	jr	ra
	nop

;; __adddf3: 004084FC
__adddf3 proc
	lui	r28,%hi(0FC00364)
	addiu	r28,r28,%lo(0FC00364)
	addu	r28,r28,r25
	addiu	sp,sp,-00000088
	sw	r28,0010(sp)
	mfc1	r3,f12
	mfc1	r2,f13
	mfc1	r7,f14
	mfc1	r6,f15
	addiu	r8,sp,+00000060
	sw	r18,007C(sp)
	sw	r17,0078(sp)
	lw	r18,-7E50(r28)
	addiu	r17,sp,+00000068
	sw	r2,0060(sp)
	sw	r6,0068(sp)
	sw	ra,0080(sp)
	sw	r3,0004(r8)
	sw	r16,0074(sp)
	or	r4,r8,r0
	addiu	r5,sp,+00000018
	or	r25,r18,r0
	jalr	ra,r25
	sw	r7,0004(r17)
	lw	r28,0010(sp)
	addiu	r16,sp,+00000030
	or	r4,r17,r0
	or	r25,r18,r0
	jalr	ra,r25
	or	r5,r16,r0
	lw	r28,0010(sp)
	nop
	lw	r25,-7FE0(r28)
	nop
	addiu	r25,r25,-00007E80
	or	r5,r16,r0
	addiu	r4,sp,+00000018
	jalr	ra,r25
	addiu	r6,sp,+00000048
	lw	r28,0010(sp)
	nop
	lw	r25,-7FCC(r28)
	nop
	jalr	ra,r25
	or	r4,r2,r0
	lw	r28,0010(sp)
	lw	ra,0080(sp)
	lw	r18,007C(sp)
	lw	r17,0078(sp)
	lw	r16,0074(sp)
	jr	ra
	addiu	sp,sp,+00000088

;; __subdf3: 004085CC
__subdf3 proc
	lui	r28,%hi(0FC00294)
	addiu	r28,r28,%lo(0FC00294)
	addu	r28,r28,r25
	addiu	sp,sp,-00000088
	sw	r28,0010(sp)
	mfc1	r3,f12
	mfc1	r2,f13
	mfc1	r7,f14
	mfc1	r6,f15
	addiu	r8,sp,+00000060
	sw	r18,007C(sp)
	sw	r17,0078(sp)
	lw	r18,-7E50(r28)
	addiu	r17,sp,+00000068
	sw	r2,0060(sp)
	sw	r6,0068(sp)
	sw	ra,0080(sp)
	sw	r3,0004(r8)
	sw	r16,0074(sp)
	or	r4,r8,r0
	addiu	r5,sp,+00000018
	or	r25,r18,r0
	jalr	ra,r25
	sw	r7,0004(r17)
	lw	r28,0010(sp)
	addiu	r16,sp,+00000030
	or	r4,r17,r0
	or	r25,r18,r0
	jalr	ra,r25
	or	r5,r16,r0
	lw	r28,0010(sp)
	nop
	lw	r25,-7FE0(r28)
	nop
	addiu	r25,r25,-00007E80
	lw	r2,0034(sp)
	or	r5,r16,r0
	xori	r2,r2,00000001
	addiu	r4,sp,+00000018
	addiu	r6,sp,+00000048
	jalr	ra,r25
	sw	r2,0034(sp)
	lw	r28,0010(sp)
	nop
	lw	r25,-7FCC(r28)
	nop
	jalr	ra,r25
	or	r4,r2,r0
	lw	r28,0010(sp)
	lw	ra,0080(sp)
	lw	r18,007C(sp)
	lw	r17,0078(sp)
	lw	r16,0074(sp)
	jr	ra
	addiu	sp,sp,+00000088
004086A8                         00 00 00 00 00 00 00 00         ........

;; __muldf3: 004086B0
__muldf3 proc
	lui	r28,%hi(0FC001B0)
	addiu	r28,r28,%lo(0FC001B0)
	addu	r28,r28,r25
	addiu	sp,sp,-000000A0
	sw	r28,0010(sp)
	mfc1	r3,f12
	mfc1	r2,f13
	mfc1	r7,f14
	mfc1	r6,f15
	addiu	r8,sp,+00000060
	sw	r18,0084(sp)
	sw	r16,007C(sp)
	lw	r18,-7E50(r28)
	addiu	r16,sp,+00000068
	sw	r2,0060(sp)
	sw	r6,0068(sp)
	sw	ra,0098(sp)
	sw	r3,0004(r8)
	sw	r22,0094(sp)
	sw	r21,0090(sp)
	sw	r20,008C(sp)
	sw	r19,0088(sp)
	sw	r17,0080(sp)
	or	r4,r8,r0
	addiu	r5,sp,+00000018
	or	r25,r18,r0
	jalr	ra,r25
	sw	r7,0004(r16)
	lw	r28,0010(sp)
	addiu	r17,sp,+00000030
	or	r4,r16,r0
	or	r25,r18,r0
	jalr	ra,r25
	or	r5,r17,r0
	lw	r28,0010(sp)
	lw	r4,0018(sp)
	nop
	sltiu	r2,r4,+00000002
	bne	r2,r0,004087A8
	addiu	r22,sp,+00000018

l00408750:
	lw	r3,0030(sp)
	nop
	sltiu	r2,r3,+00000002
	bne	r2,r0,004087D0
	xori	r2,r4,00000004

l00408764:
	bne	r2,r0,00408784
	xori	r2,r3,00000004

l0040876C:
	xori	r2,r3,00000002
	lw	r4,-7F3C(r28)
	beq	r2,r0,00408B58
	nop

l0040877C:
	beq	r0,r0,004087A8
	nop

l00408784:
	bne	r2,r0,004087A0
	xori	r2,r4,00000002

l0040878C:
	lw	r4,-7F3C(r28)
	beq	r2,r0,00408B58
	nop

l00408798:
	beq	r0,r0,004087D0
	nop

l004087A0:
	bne	r2,r0,004087C4
	xori	r2,r3,00000002

l004087A8:
	lw	r2,0004(r22)
	lw	r3,0034(sp)
	or	r4,r22,r0
	xor	r2,r2,r3
	sltu	r2,r0,r2
	beq	r0,r0,00408B58
	sw	r2,0004(r22)

l004087C4:
	nop
	bne	r2,r0,004087EC
	nop

l004087D0:
	lw	r2,0004(r22)
	lw	r3,0034(sp)
	or	r4,r17,r0
	xor	r2,r2,r3
	sltu	r2,r0,r2
	beq	r0,r0,00408B58
	sw	r2,0034(sp)

l004087EC:
	lw	r6,0010(r22)
	lw	r7,0014(r22)
	addiu	r2,r0,+00000000
	addiu	r3,r0,-00000001
	lw	r4,0040(sp)
	lw	r5,0044(sp)
	srl	r11,r6,00
	and	r6,r6,r2
	and	r7,r7,r3
	and	r8,r4,r2
	and	r9,r5,r3
	srl	r5,r4,00
	or	r4,r0,r0
	mult	r5,r6
	or	r10,r0,r0
	or	r2,r0,r0
	or	r3,r0,r0
	sw	r2,0070(sp)
	sw	r3,0074(sp)
	mflo	r13
	nop
	nop
	mult	r9,r10
	mflo	r14
	nop
	nop
	multu	r5,r7
	mfhi	r24
	mflo	r25
	addu	r24,r24,r13
	nop
	multu	r9,r11
	mfhi	r2
	mflo	r3
	addu	r2,r2,r14
	nop
	mult	r7,r4
	mflo	r12
	addu	r24,r24,r12
	nop
	mult	r11,r8
	mflo	r13
	addu	r2,r2,r13
	addu	r21,r25,r3
	mult	r9,r6
	sltu	r15,r21,r3
	addu	r20,r24,r2
	addu	r20,r20,r15
	sltu	r12,r20,r24
	mflo	r14
	nop
	nop
	mult	r5,r10
	mflo	r2
	nop
	nop
	multu	r9,r7
	mfhi	r16
	mflo	r17
	addu	r16,r16,r14
	nop
	multu	r5,r11
	mfhi	r18
	mflo	r19
	addu	r18,r18,r2
	nop
	mult	r11,r4
	mflo	r4
	addu	r18,r18,r4
	nop
	mult	r7,r8
	mflo	r8
	bne	r12,r0,00408924
	addu	r16,r16,r8

l00408914:
	bne	r24,r20,00408934
	sltu	r2,r21,r25

l0040891C:
	beq	r2,r0,00408934
	nop

l00408924:
	addiu	r2,r0,+00000001
	addiu	r3,r0,+00000000
	sw	r2,0070(sp)
	sw	r3,0074(sp)

l00408934:
	addiu	r2,r0,-00000001
	addiu	r3,r0,+00000000
	nop
	or	r5,r0,r0
	and	r24,r4,r2
	and	r25,r5,r3
	addu	r11,r17,r25
	sltu	r6,r11,r25
	addu	r10,r16,r24
	addu	r10,r10,r6
	sltu	r2,r10,r16
	bne	r2,r0,00408978
	nop

l00408968:
	bne	r16,r10,00408998
	sltu	r2,r11,r17

l00408970:
	beq	r2,r0,00408998
	nop

l00408978:
	lw	r4,0070(sp)
	lw	r5,0074(sp)
	nop
	addiu	r5,r5,+00000001
	sltiu	r2,r5,+00000001
	addu	r4,r4,r2
	sw	r4,0070(sp)
	sw	r5,0074(sp)

l00408998:
	addiu	r2,r0,+00000000
	addiu	r3,r0,-00000001
	srl	r5,r20,00
	or	r4,r0,r0
	and	r4,r4,r2
	and	r5,r5,r3
	addu	r5,r5,r19
	sltu	r9,r5,r19
	lw	r12,0070(sp)
	lw	r13,0074(sp)
	addu	r4,r4,r18
	addu	r4,r4,r9
	lw	r6,0004(r22)
	lw	r3,0008(r22)
	lw	r7,0038(sp)
	addu	r13,r13,r5
	lw	r8,0034(sp)
	sltu	r9,r13,r5
	lui	r2,+1FFF
	addu	r12,r12,r4
	addu	r3,r3,r7
	addu	r12,r12,r9
	xor	r6,r6,r8
	ori	r2,r2,0000FFFF
	sltu	r6,r0,r6
	addiu	r3,r3,+00000004
	sltu	r2,r2,r12
	sw	r6,004C(sp)
	sw	r3,0050(sp)
	or	r6,r10,r0
	beq	r2,r0,00408A7C
	or	r7,r11,r0

l00408A18:
	lui	r8,+1FFF
	or	r5,r3,r0
	addiu	r14,r0,+00000000
	addiu	r15,r0,+00000001
	lui	r10,-8000
	addiu	r11,r0,+00000000
	ori	r8,r8,0000FFFF
	and	r2,r12,r14

l00408A38:
	and	r3,r13,r15
	srl	r13,r13,01
	sll	r4,r12,1F
	or	r13,r13,r4
	srl	r12,r12,01
	sltu	r4,r8,r12
	beq	r3,r0,00408A70
	addiu	r5,r5,+00000001

l00408A58:
	srl	r7,r7,01
	sll	r2,r6,1F
	or	r7,r7,r2
	srl	r6,r6,01
	or	r6,r6,r10
	or	r7,r7,r11

l00408A70:
	bne	r4,r0,00408A38
	and	r2,r12,r14

l00408A78:
	sw	r5,0050(sp)

l00408A7C:
	lui	r2,+0FFF
	ori	r2,r2,0000FFFF
	sltu	r2,r2,r12
	bne	r2,r0,00408AF4
	nop

l00408A90:
	lui	r8,+0FFF
	lw	r5,0050(sp)
	lui	r14,-8000
	addiu	r15,r0,+00000000
	addiu	r10,r0,+00000000
	addiu	r11,r0,+00000001
	ori	r8,r8,0000FFFF

l00408AAC:
	sll	r12,r12,01
	and	r2,r6,r14
	srl	r4,r13,1F
	and	r3,r7,r15
	or	r12,r12,r4
	or	r2,r3,r2
	sll	r13,r13,01
	beq	r2,r0,00408AD8
	addiu	r5,r5,-00000001

l00408AD0:
	or	r12,r12,r10
	or	r13,r13,r11

l00408AD8:
	sll	r6,r6,01
	srl	r3,r7,1F
	or	r6,r6,r3
	sltu	r2,r8,r12
	beq	r2,r0,00408AAC
	sll	r7,r7,01

l00408AF0:
	sw	r5,0050(sp)

l00408AF4:
	addiu	r2,r0,+00000000
	addiu	r3,r0,+000000FF
	and	r4,r12,r2
	bne	r4,r0,00408B44
	and	r5,r13,r3

l00408B08:
	addiu	r2,r0,+00000080
	bne	r5,r2,00408B48
	addiu	r2,r0,+00000003

l00408B14:
	addiu	r2,r0,+00000000
	addiu	r3,r0,+00000100
	and	r2,r12,r2
	and	r3,r13,r3
	or	r2,r3,r2
	bne	r2,r0,00408B38
	or	r2,r7,r6

l00408B30:
	beq	r2,r0,00408B48
	addiu	r2,r0,+00000003

l00408B38:
	addiu	r13,r13,+00000080
	sltiu	r2,r13,+00000080
	addu	r12,r12,r2

l00408B44:
	addiu	r2,r0,+00000003

l00408B48:
	sw	r12,0058(sp)
	sw	r13,005C(sp)
	sw	r2,0048(sp)
	addiu	r4,sp,+00000048

l00408B58:
	lw	r25,-7FCC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(sp)
	lw	ra,0098(sp)
	lw	r22,0094(sp)
	lw	r21,0090(sp)
	lw	r20,008C(sp)
	lw	r19,0088(sp)
	lw	r18,0084(sp)
	lw	r17,0080(sp)
	lw	r16,007C(sp)
	jr	ra
	addiu	sp,sp,+000000A0
00408B94             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __divdf3: 00408BA0
__divdf3 proc
	lui	r28,%hi(FFFFFCC0)
	addiu	r28,r28,%lo(FFFFFCC0)
	addu	r28,r28,r25
	addiu	sp,sp,-00000070
	sw	r28,0010(sp)
	mfc1	r3,f12
	mfc1	r2,f13
	mfc1	r7,f14
	mfc1	r6,f15
	addiu	r8,sp,+00000048
	sw	r18,0064(sp)
	sw	r16,005C(sp)
	lw	r18,-7E50(r28)
	addiu	r16,sp,+00000050
	sw	r2,0048(sp)
	sw	r6,0050(sp)
	sw	ra,0068(sp)
	sw	r3,0004(r8)
	sw	r17,0060(sp)
	or	r4,r8,r0
	addiu	r5,sp,+00000018
	or	r25,r18,r0
	jalr	ra,r25
	sw	r7,0004(r16)
	lw	r28,0010(sp)
	addiu	r17,sp,+00000030
	or	r4,r16,r0
	or	r25,r18,r0
	jalr	ra,r25
	or	r5,r17,r0
	lw	r28,0010(sp)
	lw	r6,0018(sp)
	addiu	r15,sp,+00000018
	sltiu	r2,r6,+00000002
	bne	r2,r0,00408DDC
	or	r4,r15,r0

l00408C30:
	lw	r5,0030(sp)
	nop
	sltiu	r2,r5,+00000002
	bne	r2,r0,00408DDC
	or	r4,r17,r0

l00408C44:
	lw	r2,0004(r15)
	lw	r3,0034(sp)
	xori	r4,r6,00000004
	xor	r2,r2,r3
	beq	r4,r0,00408C68
	sw	r2,0004(r15)

l00408C5C:
	xori	r2,r6,00000002
	bne	r2,r0,00408C7C
	xori	r2,r5,00000004

l00408C68:
	lw	r4,-7F3C(r28)
	beq	r6,r5,00408DDC
	nop

l00408C74:
	beq	r0,r0,00408DDC
	or	r4,r15,r0

l00408C7C:
	bne	r2,r0,00408CA0
	xori	r2,r5,00000002

l00408C84:
	or	r2,r0,r0
	or	r3,r0,r0
	sw	r2,0010(r15)
	sw	r3,0014(r15)
	sw	r0,0008(r15)
	beq	r0,r0,00408DDC
	or	r4,r15,r0

l00408CA0:
	bne	r2,r0,00408CB4
	addiu	r2,r0,+00000004

l00408CA8:
	sw	r2,0018(sp)
	beq	r0,r0,00408DDC
	or	r4,r15,r0

l00408CB4:
	lw	r3,0008(r15)
	lw	r2,0038(sp)
	lw	r8,0040(sp)
	lw	r9,0044(sp)
	lw	r4,0010(r15)
	lw	r5,0014(r15)
	subu	r3,r3,r2
	sltu	r14,r4,r8
	bne	r14,r0,00408CEC
	sw	r3,0008(r15)

l00408CDC:
	bne	r8,r4,00408D08
	sltu	r2,r5,r9

l00408CE4:
	beq	r2,r0,00408D08
	nop

l00408CEC:
	sll	r4,r4,01
	addiu	r2,r3,-00000001
	srl	r3,r5,1F
	or	r4,r4,r3
	sll	r5,r5,01
	sw	r2,0008(r15)
	sltu	r14,r4,r8

l00408D08:
	lui	r10,+1000
	addiu	r11,r0,+00000000
	or	r12,r0,r0
	or	r13,r0,r0

l00408D18:
	nop
	bne	r14,r0,00408D4C
	nop

l00408D24:
	bne	r8,r4,00408D38
	sltu	r2,r5,r9

l00408D2C:
	sltu	r2,r5,r9
	bne	r2,r0,00408D4C
	sltu	r2,r5,r9

l00408D38:
	subu	r5,r5,r9
	subu	r4,r4,r8
	or	r12,r12,r10
	subu	r4,r4,r2
	or	r13,r13,r11

l00408D4C:
	srl	r11,r11,01
	sll	r2,r10,1F
	or	r11,r11,r2
	sll	r6,r4,01
	srl	r3,r5,1F
	srl	r10,r10,01
	or	r6,r6,r3
	or	r2,r11,r10
	sll	r7,r5,01
	or	r4,r6,r0
	sltu	r14,r6,r8
	bne	r2,r0,00408D18
	or	r5,r7,r0

l00408D80:
	addiu	r2,r0,+00000000
	addiu	r3,r0,+000000FF
	and	r4,r12,r2
	bne	r4,r0,00408DD0
	and	r5,r13,r3

l00408D94:
	addiu	r2,r0,+00000080
	bne	r5,r2,00408DD0
	nop

l00408DA0:
	addiu	r2,r0,+00000000
	addiu	r3,r0,+00000100
	and	r2,r12,r2
	and	r3,r13,r3
	or	r2,r3,r2
	bne	r2,r0,00408DC4
	or	r2,r7,r6

l00408DBC:
	beq	r2,r0,00408DD0
	nop

l00408DC4:
	addiu	r13,r13,+00000080
	sltiu	r2,r13,+00000080
	addu	r12,r12,r2

l00408DD0:
	sw	r12,0010(r15)
	sw	r13,0014(r15)
	or	r4,r15,r0

l00408DDC:
	lw	r25,-7FCC(r28)
	nop
	jalr	ra,r25
	nop
	lw	r28,0010(sp)
	lw	ra,0068(sp)
	lw	r18,0064(sp)
	lw	r17,0060(sp)
	lw	r16,005C(sp)
	jr	ra
	addiu	sp,sp,+00000070
00408E08                         00 00 00 00 00 00 00 00         ........

;; __eqdf2: 00408E10
__eqdf2 proc
	lui	r28,%hi(FFFFFA50)
	addiu	r28,r28,%lo(FFFFFA50)
	addu	r28,r28,r25
	addiu	sp,sp,-00000070
	sw	r28,0010(sp)
	mfc1	r3,f12
	mfc1	r2,f13
	mfc1	r7,f14
	mfc1	r6,f15
	addiu	r8,sp,+00000048
	sw	r18,0064(sp)
	sw	r16,005C(sp)
	lw	r18,-7E50(r28)
	addiu	r16,sp,+00000050
	sw	r2,0048(sp)
	sw	r6,0050(sp)
	sw	ra,0068(sp)
	sw	r3,0004(r8)
	sw	r17,0060(sp)
	or	r4,r8,r0
	addiu	r5,sp,+00000018
	or	r25,r18,r0
	jalr	ra,r25
	sw	r7,0004(r16)
	lw	r28,0010(sp)
	addiu	r17,sp,+00000030
	or	r4,r16,r0
	or	r25,r18,r0
	jalr	ra,r25
	or	r5,r17,r0
	lw	r28,0010(sp)
	lw	r3,0018(sp)
	nop
	sltiu	r3,r3,+00000002
	bne	r3,r0,00408EC8
	addiu	r2,r0,+00000001

l00408EA0:
	lw	r3,0030(sp)
	nop
	sltiu	r3,r3,+00000002
	bne	r3,r0,00408EC8
	addiu	r4,sp,+00000018

l00408EB4:
	lw	r25,-7EE8(r28)
	nop
	jalr	ra,r25
	or	r5,r17,r0
	lw	r28,0010(sp)

l00408EC8:
	lw	ra,0068(sp)
	lw	r18,0064(sp)
	lw	r17,0060(sp)
	lw	r16,005C(sp)
	jr	ra
	addiu	sp,sp,+00000070

;; __nedf2: 00408EE0
__nedf2 proc
	lui	r28,%hi(FFFFF980)
	addiu	r28,r28,%lo(FFFFF980)
	addu	r28,r28,r25
	addiu	sp,sp,-00000070
	sw	r28,0010(sp)
	mfc1	r3,f12
	mfc1	r2,f13
	mfc1	r7,f14
	mfc1	r6,f15
	addiu	r8,sp,+00000048
	sw	r18,0064(sp)
	sw	r16,005C(sp)
	lw	r18,-7E50(r28)
	addiu	r16,sp,+00000050
	sw	r2,0048(sp)
	sw	r6,0050(sp)
	sw	ra,0068(sp)
	sw	r3,0004(r8)
	sw	r17,0060(sp)
	or	r4,r8,r0
	addiu	r5,sp,+00000018
	or	r25,r18,r0
	jalr	ra,r25
	sw	r7,0004(r16)
	lw	r28,0010(sp)
	addiu	r17,sp,+00000030
	or	r4,r16,r0
	or	r25,r18,r0
	jalr	ra,r25
	or	r5,r17,r0
	lw	r28,0010(sp)
	lw	r3,0018(sp)
	nop
	sltiu	r3,r3,+00000002
	bne	r3,r0,00408F98
	addiu	r2,r0,+00000001

l00408F70:
	lw	r3,0030(sp)
	nop
	sltiu	r3,r3,+00000002
	bne	r3,r0,00408F98
	addiu	r4,sp,+00000018

l00408F84:
	lw	r25,-7EE8(r28)
	nop
	jalr	ra,r25
	or	r5,r17,r0
	lw	r28,0010(sp)

l00408F98:
	lw	ra,0068(sp)
	lw	r18,0064(sp)
	lw	r17,0060(sp)
	lw	r16,005C(sp)
	jr	ra
	addiu	sp,sp,+00000070

;; __gedf2: 00408FB0
__gedf2 proc
	lui	r28,%hi(FFFFF8B0)
	addiu	r28,r28,%lo(FFFFF8B0)
	addu	r28,r28,r25
	addiu	sp,sp,-00000070
	sw	r28,0010(sp)
	mfc1	r3,f12
	mfc1	r2,f13
	mfc1	r7,f14
	mfc1	r6,f15
	addiu	r8,sp,+00000048
	sw	r18,0064(sp)
	sw	r16,005C(sp)
	lw	r18,-7E50(r28)
	addiu	r16,sp,+00000050
	sw	r2,0048(sp)
	sw	r6,0050(sp)
	sw	ra,0068(sp)
	sw	r3,0004(r8)
	sw	r17,0060(sp)
	or	r4,r8,r0
	addiu	r5,sp,+00000018
	or	r25,r18,r0
	jalr	ra,r25
	sw	r7,0004(r16)
	lw	r28,0010(sp)
	addiu	r17,sp,+00000030
	or	r4,r16,r0
	or	r25,r18,r0
	jalr	ra,r25
	or	r5,r17,r0
	lw	r28,0010(sp)
	lw	r3,0018(sp)
	nop
	sltiu	r3,r3,+00000002
	bne	r3,r0,00409068
	addiu	r2,r0,-00000001

l00409040:
	lw	r3,0030(sp)
	nop
	sltiu	r3,r3,+00000002
	bne	r3,r0,00409068
	addiu	r4,sp,+00000018

l00409054:
	lw	r25,-7EE8(r28)
	nop
	jalr	ra,r25
	or	r5,r17,r0
	lw	r28,0010(sp)

l00409068:
	lw	ra,0068(sp)
	lw	r18,0064(sp)
	lw	r17,0060(sp)
	lw	r16,005C(sp)
	jr	ra
	addiu	sp,sp,+00000070

;; __ltdf2: 00409080
__ltdf2 proc
	lui	r28,%hi(FFFFF7E0)
	addiu	r28,r28,%lo(FFFFF7E0)
	addu	r28,r28,r25
	addiu	sp,sp,-00000070
	sw	r28,0010(sp)
	mfc1	r3,f12
	mfc1	r2,f13
	mfc1	r7,f14
	mfc1	r6,f15
	addiu	r8,sp,+00000048
	sw	r18,0064(sp)
	sw	r16,005C(sp)
	lw	r18,-7E50(r28)
	addiu	r16,sp,+00000050
	sw	r2,0048(sp)
	sw	r6,0050(sp)
	sw	ra,0068(sp)
	sw	r3,0004(r8)
	sw	r17,0060(sp)
	or	r4,r8,r0
	addiu	r5,sp,+00000018
	or	r25,r18,r0
	jalr	ra,r25
	sw	r7,0004(r16)
	lw	r28,0010(sp)
	addiu	r17,sp,+00000030
	or	r4,r16,r0
	or	r25,r18,r0
	jalr	ra,r25
	or	r5,r17,r0
	lw	r28,0010(sp)
	lw	r3,0018(sp)
	nop
	sltiu	r3,r3,+00000002
	bne	r3,r0,00409138
	addiu	r2,r0,+00000001

l00409110:
	lw	r3,0030(sp)
	nop
	sltiu	r3,r3,+00000002
	bne	r3,r0,00409138
	addiu	r4,sp,+00000018

l00409124:
	lw	r25,-7EE8(r28)
	nop
	jalr	ra,r25
	or	r5,r17,r0
	lw	r28,0010(sp)

l00409138:
	lw	ra,0068(sp)
	lw	r18,0064(sp)
	lw	r17,0060(sp)
	lw	r16,005C(sp)
	jr	ra
	addiu	sp,sp,+00000070

;; __floatsidf: 00409150
__floatsidf proc
	lui	r28,%hi(FFFFF710)
	addiu	r28,r28,%lo(FFFFF710)
	addu	r28,r28,r25
	addiu	sp,sp,-00000038
	sw	r28,0010(sp)
	addiu	r2,r0,+00000003
	srl	r3,r4,1F
	sw	ra,0034(sp)
	sw	r2,0018(sp)
	bne	r4,r0,00409188
	sw	r3,001C(sp)

l0040917C:
	addiu	r2,r0,+00000002
	beq	r0,r0,0040922C
	sw	r2,0018(sp)

l00409188:
	addiu	r2,r0,+0000003C
	beq	r3,r0,004091C0
	sw	r2,0020(sp)

l00409194:
	lui	r2,-8000
	lui	r1,-3E20
	mtc1	r1,f1
	mtc1	r0,f0
	beq	r4,r2,00409240
	subu	r2,r0,r4

l004091AC:
	or	r3,r2,r0
	sra	r2,r2,1F
	sw	r2,0028(sp)
	beq	r0,r0,004091D0
	sw	r3,002C(sp)

l004091C0:
	or	r3,r4,r0
	sra	r2,r4,1F
	sw	r2,0028(sp)
	sw	r3,002C(sp)

l004091D0:
	lw	r3,0028(sp)
	lui	r2,+0FFF
	ori	r2,r2,0000FFFF
	sltu	r2,r2,r3
	bne	r2,r0,0040922C
	nop

l004091E8:
	lui	r6,+0FFF
	lw	r5,0020(sp)
	ori	r6,r6,0000FFFF

l004091F4:
	lw	r2,0028(sp)
	lw	r3,002C(sp)
	sll	r2,r2,01
	srl	r4,r3,1F
	or	r2,r2,r4
	sll	r3,r3,01
	sw	r2,0028(sp)
	sw	r3,002C(sp)
	lw	r4,0028(sp)
	nop
	sltu	r4,r6,r4
	beq	r4,r0,004091F4
	addiu	r5,r5,-00000001

l00409228:
	sw	r5,0020(sp)

l0040922C:
	lw	r25,-7FCC(r28)
	nop
	jalr	ra,r25
	addiu	r4,sp,+00000018
	lw	r28,0010(sp)

l00409240:
	lw	ra,0034(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000038

;; __negdf2: 00409250
__negdf2 proc
	lui	r28,%hi(FFFFF610)
	addiu	r28,r28,%lo(FFFFF610)
	addu	r28,r28,r25
	addiu	sp,sp,-00000040
	sw	r28,0010(sp)
	mfc1	r3,f12
	mfc1	r2,f13
	addiu	r6,sp,+00000030
	sw	r2,0030(sp)
	sw	ra,003C(sp)
	or	r4,r6,r0
	lw	r25,-7E50(r28)
	addiu	r5,sp,+00000018
	jalr	ra,r25
	sw	r3,0004(r6)
	lw	r28,0010(sp)
	nop
	lw	r25,-7FCC(r28)
	addiu	r3,sp,+00000018
	lw	r2,0004(r3)
	or	r4,r3,r0
	sltiu	r2,r2,+00000001
	jalr	ra,r25
	sw	r2,0004(r3)
	lw	r28,0010(sp)
	lw	ra,003C(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000040
004092C4             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __make_dp: 004092D0
__make_dp proc
	lui	r28,%hi(FFFFF590)
	addiu	r28,r28,%lo(FFFFF590)
	addu	r28,r28,r25
	addiu	sp,sp,-00000038
	sw	r28,0010(sp)
	lw	r2,0048(sp)
	lw	r3,004C(sp)
	sw	r4,0018(sp)
	lw	r25,-7FCC(r28)
	sw	ra,0034(sp)
	sw	r5,001C(sp)
	sw	r6,0020(sp)
	sw	r2,0028(sp)
	sw	r3,002C(sp)
	jalr	ra,r25
	addiu	r4,sp,+00000018
	lw	r28,0010(sp)
	lw	ra,0034(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000038
00409324             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __truncdfsf2: 00409330
__truncdfsf2 proc
	lui	r28,%hi(FFFFF530)
	addiu	r28,r28,%lo(FFFFF530)
	addu	r28,r28,r25
	addiu	sp,sp,-00000040
	sw	r28,0010(sp)
	mfc1	r3,f12
	mfc1	r2,f13
	addiu	r6,sp,+00000030
	sw	r2,0030(sp)
	sw	ra,003C(sp)
	or	r4,r6,r0
	lw	r25,-7E50(r28)
	addiu	r5,sp,+00000018
	jalr	ra,r25
	sw	r3,0004(r6)
	lw	r28,0010(sp)
	addiu	r2,r0,+00000000
	lui	r3,+3FFF
	ori	r3,r3,0000FFFF
	lw	r4,0028(sp)
	lw	r5,002C(sp)
	lw	r25,-7DFC(r28)
	and	r2,r4,r2
	and	r3,r5,r3
	srl	r5,r5,1E
	sll	r6,r4,02
	or	r5,r5,r6
	srl	r4,r4,1E
	or	r2,r3,r2
	beq	r2,r0,004093B0
	or	r7,r5,r0

l004093AC:
	ori	r7,r7,00000001

l004093B0:
	lw	r4,0018(sp)
	lw	r5,001C(sp)
	lw	r6,0020(sp)
	jalr	ra,r25
	nop
	lw	r28,0010(sp)
	lw	ra,003C(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000040
004093D8                         00 00 00 00 00 00 00 00         ........

;; __pack_f: 004093E0
__pack_f proc
	or	r6,r4,r0
	lw	r3,0000(r6)
	lw	r4,000C(r6)
	sltiu	r2,r3,+00000002
	lw	r7,0004(r6)
	beq	r2,r0,0040940C
	or	r5,r0,r0

l004093FC:
	lui	r2,+0010
	or	r4,r4,r2
	beq	r0,r0,0040950C
	addiu	r5,r0,+000000FF

l0040940C:
	xori	r2,r3,00000004
	beq	r2,r0,004094C0
	nop

l00409418:
	xori	r2,r3,00000002
	bne	r2,r0,0040942C
	nop

l00409424:
	beq	r0,r0,0040950C
	or	r4,r0,r0

l0040942C:
	beq	r4,r0,00409510
	lui	r2,+007F

l00409434:
	lw	r5,0008(r6)
	nop
	slti	r2,r5,-0000007E
	beq	r2,r0,004094B4
	slti	r2,r5,+00000080

l00409448:
	addiu	r2,r0,-0000007E
	subu	r2,r2,r5
	addiu	r3,r0,+00000001
	sllv	r3,r3,r2
	addiu	r3,r3,-00000001
	and	r3,r4,r3
	sltu	r3,r0,r3
	srlv	r4,r4,r2
	slti	r2,r2,+0000001A
	bne	r2,r0,00409478
	or	r4,r4,r3

l00409474:
	or	r4,r0,r0

l00409478:
	andi	r3,r4,0000007F
	addiu	r2,r0,+00000040
	bne	r3,r2,0040949C
	nop

l00409488:
	andi	r2,r4,00000080
	beq	r2,r0,004094A0
	nop

l00409494:
	beq	r0,r0,004094A0
	addiu	r4,r4,+00000040

l0040949C:
	addiu	r4,r4,+0000003F

l004094A0:
	lui	r2,+3FFF
	ori	r2,r2,0000FFFF
	sltu	r5,r2,r4
	beq	r0,r0,0040950C
	srl	r4,r4,07

l004094B4:
	nop
	bne	r2,r0,004094CC
	nop

l004094C0:
	addiu	r5,r0,+000000FF
	beq	r0,r0,0040950C
	or	r4,r0,r0

l004094CC:
	andi	r3,r4,0000007F
	addiu	r2,r0,+00000040
	bne	r3,r2,004094F0
	addiu	r5,r5,+0000007F

l004094DC:
	andi	r2,r4,00000080
	beq	r2,r0,004094F4
	nop

l004094E8:
	beq	r0,r0,004094F4
	addiu	r4,r4,+00000040

l004094F0:
	addiu	r4,r4,+0000003F

l004094F4:
	nop
	bgez	r4,00409508
	nop

l00409500:
	srl	r4,r4,01
	addiu	r5,r5,+00000001

l00409508:
	srl	r4,r4,07

l0040950C:
	lui	r2,+007F

l00409510:
	andi	r3,r5,000000FF
	ori	r2,r2,0000FFFF
	and	r2,r4,r2
	sll	r3,r3,17
	or	r2,r2,r3
	sll	r4,r7,1F
	or	r2,r2,r4
	mtc1	r2,f0
	jr	ra
	nop
00409538                         00 00 00 00 00 00 00 00         ........

;; __unpack_f: 00409540
__unpack_f proc
	lw	r3,0000(r4)
	lui	r2,+007F
	srl	r4,r3,17
	or	r6,r5,r0
	ori	r2,r2,0000FFFF
	srl	r5,r3,1F
	andi	r4,r4,000000FF
	sw	r5,0004(r6)
	bne	r4,r0,004095C4
	and	r5,r3,r2

l00409568:
	bne	r5,r0,0040957C
	lui	r2,+3FFF

l00409570:
	addiu	r2,r0,+00000002
	jr	ra
	sw	r2,0000(r6)

l0040957C:
	sll	r5,r5,07
	ori	r2,r2,0000FFFF
	addiu	r3,r0,-0000007E
	addiu	r4,r0,+00000003
	sltu	r2,r2,r5
	sw	r3,0008(r6)
	bne	r2,r0,004095BC
	sw	r4,0000(r6)

l0040959C:
	lui	r3,+3FFF
	ori	r3,r3,0000FFFF
	addiu	r4,r0,-0000007E

l004095A8:
	sll	r5,r5,01
	sltu	r2,r3,r5
	beq	r2,r0,004095A8
	addiu	r4,r4,-00000001

l004095B8:
	sw	r4,0008(r6)

l004095BC:
	jr	ra
	sw	r5,000C(r6)

l004095C4:
	addiu	r2,r0,+000000FF
	bne	r4,r2,00409604
	lui	r2,+4000

l004095D0:
	bne	r5,r0,004095E4
	lui	r2,+0010

l004095D8:
	addiu	r2,r0,+00000004
	jr	ra
	sw	r2,0000(r6)

l004095E4:
	and	r2,r5,r2
	beq	r2,r0,004095F8
	addiu	r2,r0,+00000001

l004095F0:
	beq	r0,r0,004095FC
	sw	r2,0000(r6)

l004095F8:
	sw	r0,0000(r6)

l004095FC:
	jr	ra
	sw	r5,000C(r6)

l00409604:
	sll	r3,r5,07
	or	r3,r3,r2
	addiu	r4,r4,-0000007F
	addiu	r2,r0,+00000003
	sw	r3,000C(r6)
	sw	r4,0008(r6)
	jr	ra
	sw	r2,0000(r6)
00409624             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __make_fp: 00409630
__make_fp proc
	lui	r28,%hi(FFFFF230)
	addiu	r28,r28,%lo(FFFFF230)
	addu	r28,r28,r25
	addiu	sp,sp,-00000030
	sw	r28,0010(sp)
	sw	r4,0018(sp)
	lw	r25,-7EB4(r28)
	addiu	r4,sp,+00000018
	sw	ra,002C(sp)
	sw	r5,001C(sp)
	sw	r6,0020(sp)
	jalr	ra,r25
	sw	r7,0024(sp)
	lw	r28,0010(sp)
	lw	ra,002C(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000030
00409678                         00 00 00 00 00 00 00 00         ........

;; __pack_d: 00409680
__pack_d proc
	lui	r28,%hi(FFFFF1E0)
	addiu	r28,r28,%lo(FFFFF1E0)
	addu	r28,r28,r25
	lw	r3,0000(r4)
	lw	r10,0010(r4)
	lw	r11,0014(r4)
	sltiu	r2,r3,+00000002
	lw	r8,0004(r4)
	beq	r2,r0,004096C0
	or	r9,r0,r0

l004096A8:
	lui	r2,+0008
	addiu	r3,r0,+00000000
	or	r10,r10,r2
	or	r11,r11,r3
	beq	r0,r0,004098B8
	addiu	r9,r0,+000007FF

l004096C0:
	xori	r2,r3,00000004
	beq	r2,r0,00409814
	nop

l004096CC:
	xori	r2,r3,00000002
	bne	r2,r0,004096E4
	nop

l004096D8:
	or	r10,r0,r0
	beq	r0,r0,004098B8
	or	r11,r0,r0

l004096E4:
	or	r2,r11,r10
	beq	r2,r0,004098B8
	nop

l004096F0:
	lw	r4,0008(r4)
	nop
	slti	r2,r4,-000003FE
	beq	r2,r0,00409808
	slti	r2,r4,+00000400

l00409704:
	addiu	r2,r0,-000003FE
	subu	r4,r2,r4
	slti	r3,r4,+00000039
	bne	r3,r0,00409724
	addiu	r2,r0,+00000001

l00409718:
	or	r10,r0,r0
	beq	r0,r0,00409794
	or	r11,r0,r0

l00409724:
	sllv	r2,r2,r4
	addiu	r2,r2,-00000001
	or	r3,r2,r0
	sra	r2,r2,1F
	and	r2,r10,r2
	and	r3,r11,r3
	or	r2,r3,r2
	beq	r2,r0,0040974C
	or	r5,r0,r0

l00409748:
	addiu	r5,r0,+00000001

l0040974C:
	sll	r6,r4,1A
	bgez	r6,00409764
	nop

l00409758:
	srlv	r15,r10,r4
	beq	r0,r0,0040977C
	or	r14,r0,r0

l00409764:
	beq	r6,r0,00409778
	srlv	r15,r11,r4

l0040976C:
	subu	r6,r0,r4
	sllv	r6,r10,r6
	or	r15,r15,r6

l00409778:
	srlv	r14,r10,r4

l0040977C:
	or	r3,r5,r0
	or	r2,r0,r0
	or	r4,r14,r0
	or	r5,r15,r0
	or	r10,r4,r2
	or	r11,r5,r3

l00409794:
	addiu	r2,r0,+00000000
	addiu	r3,r0,+000000FF
	and	r4,r10,r2
	bne	r4,r0,004097E0
	and	r5,r11,r3

l004097A8:
	addiu	r2,r0,+00000080
	bne	r5,r2,004097E0
	nop

l004097B4:
	addiu	r2,r0,+00000000
	addiu	r3,r0,+00000100
	and	r2,r10,r2
	and	r3,r11,r3
	or	r2,r3,r2
	beq	r2,r0,004097F0
	lui	r2,%hi(0FFF0080)

l004097D0:
	addiu	r11,r11,+00000080
	sltiu	r2,r11,+00000080
	beq	r0,r0,004097EC
	addu	r10,r10,r2

l004097E0:
	addiu	r11,r11,+0000007F
	sltiu	r2,r11,+0000007F
	addu	r10,r10,r2

l004097EC:
	lui	r2,+0FFF

l004097F0:
	ori	r2,r2,0000FFFF
	sltu	r2,r2,r10
	beq	r2,r0,004098A8
	nop

l00409800:
	beq	r0,r0,004098A8
	addiu	r9,r0,+00000001

l00409808:
	nop
	bne	r2,r0,00409824
	nop

l00409814:
	or	r10,r0,r0
	or	r11,r0,r0
	beq	r0,r0,004098B8
	addiu	r9,r0,+000007FF

l00409824:
	addiu	r2,r0,+00000000
	addiu	r3,r0,+000000FF
	and	r6,r10,r2
	and	r7,r11,r3
	bne	r6,r0,00409874
	addiu	r9,r4,+000003FF

l0040983C:
	addiu	r2,r0,+00000080
	bne	r7,r2,00409874
	nop

l00409848:
	addiu	r2,r0,+00000000
	addiu	r3,r0,+00000100
	and	r2,r10,r2
	and	r3,r11,r3
	or	r2,r3,r2
	beq	r2,r0,00409884
	lui	r2,%hi(1FFF0080)

l00409864:
	addiu	r11,r11,+00000080
	sltiu	r2,r11,+00000080
	beq	r0,r0,00409880
	addu	r10,r10,r2

l00409874:
	addiu	r11,r11,+0000007F
	sltiu	r2,r11,+0000007F
	addu	r10,r10,r2

l00409880:
	lui	r2,+1FFF

l00409884:
	ori	r2,r2,0000FFFF
	sltu	r2,r2,r10
	beq	r2,r0,004098A8
	nop

l00409894:
	srl	r11,r11,01
	sll	r2,r10,1F
	or	r11,r11,r2
	srl	r10,r10,01
	addiu	r9,r9,+00000001

l004098A8:
	srl	r11,r11,08
	sll	r2,r10,18
	or	r11,r11,r2
	srl	r10,r10,08

l004098B8:
	lui	r6,+000F
	ori	r6,r6,0000FFFF
	addiu	r7,r0,-00000001
	andi	r2,r9,000007FF
	and	r6,r10,r6
	or	r5,r2,r0
	and	r7,r11,r7
	or	r4,r0,r0
	or	r13,r7,r0
	or	r12,r6,r0
	sll	r4,r5,14
	or	r5,r0,r0
	or	r4,r12,r4
	andi	r8,r8,00000001
	or	r5,r13,r5
	or	r3,r8,r0
	or	r2,r0,r0
	or	r13,r5,r0
	or	r12,r4,r0
	sll	r2,r3,1F
	or	r3,r0,r0
	or	r2,r12,r2
	or	r3,r13,r3
	or	r13,r3,r0
	or	r12,r2,r0
	mtc1	r13,f0
	mtc1	r12,f1
	jr	ra
	nop
0040992C                                     00 00 00 00             ....

;; __unpack_d: 00409930
__unpack_d proc
	lui	r28,%hi(FFFFEF30)
	addiu	r28,r28,%lo(FFFFEF30)
	addu	r28,r28,r25
	lw	r2,0000(r4)
	lw	r3,0004(r4)
	or	r10,r5,r0
	srl	r7,r2,14
	srl	r5,r2,1F
	or	r6,r0,r0
	or	r4,r0,r0
	andi	r8,r5,00000001
	andi	r7,r7,000007FF
	lui	r4,+000F
	ori	r4,r4,0000FFFF
	addiu	r5,r0,-00000001
	sw	r8,0004(r10)
	and	r8,r2,r4
	bne	r7,r0,004099F8
	and	r9,r3,r5

l0040997C:
	or	r2,r9,r8
	bne	r2,r0,00409994
	lui	r2,+0FFF

l00409988:
	addiu	r2,r0,+00000002
	jr	ra
	sw	r2,0000(r10)

l00409994:
	sll	r8,r8,08
	srl	r3,r9,18
	or	r8,r8,r3
	sll	r9,r9,08
	ori	r2,r2,0000FFFF
	addiu	r3,r0,-000003FE
	addiu	r4,r0,+00000003
	sltu	r2,r2,r8
	sw	r3,0008(r10)
	bne	r2,r0,004099EC
	sw	r4,0000(r10)

l004099C0:
	lui	r4,+0FFF
	ori	r4,r4,0000FFFF

l004099C8:
	sll	r8,r8,01
	srl	r3,r9,1F
	lw	r2,0008(r10)
	or	r8,r8,r3
	sll	r9,r9,01
	addiu	r2,r2,-00000001
	sltu	r3,r4,r8
	beq	r3,r0,004099C8
	sw	r2,0008(r10)

l004099EC:
	sw	r8,0010(r10)
	jr	ra
	sw	r9,0014(r10)

l004099F8:
	addiu	r2,r0,+000007FF
	bne	r7,r2,00409A4C
	srl	r6,r9,18

l00409A04:
	or	r2,r9,r8
	bne	r2,r0,00409A18
	addiu	r2,r0,+00000004

l00409A10:
	jr	ra
	sw	r2,0000(r10)

l00409A18:
	lui	r2,+0008
	addiu	r3,r0,+00000000
	and	r2,r8,r2
	and	r3,r9,r3
	or	r2,r3,r2
	beq	r2,r0,00409A3C
	addiu	r2,r0,+00000001

l00409A34:
	beq	r0,r0,00409A40
	sw	r2,0000(r10)

l00409A3C:
	sw	r0,0000(r10)

l00409A40:
	sw	r8,0010(r10)
	jr	ra
	sw	r9,0014(r10)

l00409A4C:
	sll	r4,r8,08
	lui	r2,+1000
	addiu	r3,r0,+00000000
	or	r4,r4,r6
	sll	r5,r9,08
	or	r4,r4,r2
	or	r5,r5,r3
	addiu	r6,r7,-000003FF
	addiu	r2,r0,+00000003
	sw	r4,0010(r10)
	sw	r5,0014(r10)
	sw	r6,0008(r10)
	jr	ra
	sw	r2,0000(r10)
00409A84             00 00 00 00 00 00 00 00 00 00 00 00     ............

;; __fpcmp_parts_d: 00409A90
__fpcmp_parts_d proc
	lw	r6,0000(r4)
	nop
	sltiu	r2,r6,+00000002
	bne	r2,r0,00409AB8
	nop

l00409AA4:
	lw	r3,0000(r5)
	nop
	sltiu	r2,r3,+00000002
	beq	r2,r0,00409AC0
	nop

l00409AB8:
	jr	ra
	addiu	r2,r0,+00000001

l00409AC0:
	xori	r2,r6,00000004
	bne	r2,r0,00409AE4
	xori	r2,r3,00000004

l00409ACC:
	bne	r2,r0,00409B2C
	nop

l00409AD4:
	lw	r3,0004(r5)
	lw	r2,0004(r4)
	jr	ra
	subu	r2,r3,r2

l00409AE4:
	nop
	beq	r2,r0,00409B08
	nop

l00409AF0:
	xori	r2,r6,00000002
	bne	r2,r0,00409B20
	nop

l00409AFC:
	xori	r3,r3,00000002
	beq	r3,r0,00409C00
	or	r2,r0,r0

l00409B08:
	lw	r2,0004(r5)
	nop
	sltiu	r2,r2,+00000001
	subu	r2,r0,r2
	jr	ra
	ori	r2,r2,00000001

l00409B20:
	xori	r2,r3,00000002
	bne	r2,r0,00409B44
	nop

l00409B2C:
	lw	r2,0004(r4)
	nop
	sltu	r2,r0,r2
	subu	r2,r0,r2
	jr	ra
	ori	r2,r2,00000001

l00409B44:
	lw	r8,0004(r4)
	lw	r2,0004(r5)
	nop
	bne	r8,r2,00409BB4
	sltu	r2,r0,r8

l00409B58:
	lw	r6,0008(r4)
	lw	r3,0008(r5)
	nop
	slt	r2,r3,r6
	bne	r2,r0,00409BB4
	sltu	r2,r0,r8

l00409B70:
	slt	r2,r6,r3
	bne	r2,r0,00409BF0
	sltiu	r2,r8,+00000001

l00409B7C:
	lw	r7,0010(r4)
	lw	r6,0010(r5)
	nop
	sltu	r2,r6,r7
	bne	r2,r0,00409BB4
	sltu	r2,r0,r8

l00409B94:
	bne	r7,r6,00409BC4
	sltu	r2,r7,r6

l00409B9C:
	lw	r3,0014(r4)
	lw	r2,0014(r5)
	nop
	sltu	r2,r2,r3
	beq	r2,r0,00409BC0
	sltu	r2,r0,r8

l00409BB4:
	subu	r2,r0,r2
	jr	ra
	ori	r2,r2,00000001

l00409BC0:
	sltu	r2,r7,r6

l00409BC4:
	bne	r2,r0,00409BF0
	sltiu	r2,r8,+00000001

l00409BCC:
	bne	r6,r7,00409C00
	or	r2,r0,r0

l00409BD4:
	lw	r3,0014(r5)
	lw	r2,0014(r4)
	nop
	sltu	r2,r2,r3
	beq	r2,r0,00409BFC
	nop

l00409BEC:
	sltiu	r2,r8,+00000001

l00409BF0:
	subu	r2,r0,r2
	jr	ra
	ori	r2,r2,00000001

l00409BFC:
	or	r2,r0,r0

l00409C00:
	jr	ra
	nop
00409C08                         00 00 00 00 00 00 00 00         ........

;; __do_global_ctors_aux: 00409C10
__do_global_ctors_aux proc
	lui	r28,%hi(FFFFEC50)
	addiu	r28,r28,%lo(FFFFEC50)
	addu	r28,r28,r25
	addiu	sp,sp,-00000028
	sw	r28,0010(sp)
	sw	r16,001C(sp)
	lw	r2,-7FE8(r28)
	nop
	addiu	r2,r2,+00000858
	sw	ra,0024(sp)
	sw	r17,0020(sp)
	lw	r25,-0004(r2)
	addiu	r3,r0,-00000001
	beq	r25,r3,00409C70
	addiu	r16,r2,-00000004

l00409C4C:
	addiu	r17,r0,-00000001

l00409C50:
	jalr	ra,r25
	nop
	lw	r28,0010(sp)
	addiu	r16,r16,-00000004
	lw	r2,0000(r16)
	nop
	bne	r2,r17,00409C50
	or	r25,r2,r0

l00409C70:
	lw	ra,0024(sp)
	lw	r17,0020(sp)
	lw	r16,001C(sp)
	jr	ra
	addiu	sp,sp,+00000028

;; init_dummy: 00409C84
init_dummy proc
	lui	r28,%hi(FFFFEBDC)
	addiu	r28,r28,%lo(FFFFEBDC)
	addu	r28,r28,r25
	addiu	sp,sp,-00000020
	sw	r28,0010(sp)
	sw	ra,001C(sp)
	lw	ra,001C(sp)
	nop
	jr	ra
	addiu	sp,sp,+00000020
00409CAC                                     00 00 00 00             ....
