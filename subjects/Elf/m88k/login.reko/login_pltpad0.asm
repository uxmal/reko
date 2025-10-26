;;; Segment .pltpad0 (000360EC)
000360EC                                     67 FF 00 10             g...
000360F0 24 3F 00 0C 25 7F 00 08 C8 00 00 01 5D 60 00 02 $?..%.......]`..
00036100 59 6B 09 68 F5 6B 60 01 14 2B 00 04 24 3F 00 04 Yk.h.k`..+..$?..
00036110 15 6B 00 08 24 1F 00 00 F4 00 C8 0B F4 00 58 00 .k..$.........X.
00036120 F4 00 58 00 F4 00 58 00 F4 00 58 00 F4 00 58 00 ..X...X...X...X.
00036130 F4 00 58 00                                     ..X.            

;; auth_clroption: 00036134
;;   Called from:
;;     00003028 (in fn000028D0)
;;     00003038 (in fn000028D0)
auth_clroption proc
	or.u	r11,r0,5
	ld	r11,r11,0x6A70
	jmp	r11
00036140 5D 60 00 00 59 6B 00 00 C3 FF FF E9             ]`..Yk......    

;; unsetenv: 0003614C
;;   Called from:
;;     00002E6C (in fn000028D0)
;;     00002E74 (in fn000028D0)
unsetenv proc
	or.u	r11,r0,5
	ld	r11,r11,0x6A74
	jmp	r11
00036158                         5D 60 00 00 59 6B 00 0C         ]`..Yk..
00036160 C3 FF FF E3                                     ....            

;; warnx: 00036164
;;   Called from:
;;     00002AB4 (in fn000028D0)
;;     00002B7C (in fn000028D0)
;;     00002EA0 (in fn000028D0)
;;     00003300 (in fn000028D0)
;;     00003C54 (in fn000028D0)
;;     00003E34 (in fn000028D0)
warnx proc
	or.u	r11,r0,5
	ld	r11,r11,0x6A78
	jmp	r11
00036170 5D 60 00 00 59 6B 00 18 C3 FF FF DD             ]`..Yk......    

;; login_fbtab: 0003617C
;;   Called from:
;;     00003654 (in fn000028D0)
login_fbtab proc
	or.u	r11,r0,5
	ld	r11,r11,0x6A7C
	jmp	r11
00036188                         5D 60 00 00 59 6B 00 24         ]`..Yk.$
00036190 C3 FF FF D7                                     ....            

;; getrlimit: 00036194
;;   Called from:
;;     00002D7C (in fn000028D0)
getrlimit proc
	or.u	r11,r0,5
	ld	r11,r11,0x6A80
	jmp	r11
000361A0 5D 60 00 00 59 6B 00 30 C3 FF FF D1             ]`..Yk.0....    

;; printf: 000361AC
;;   Called from:
;;     000035A4 (in fn000028D0)
;;     00003850 (in fn000028D0)
;;     00003934 (in fn000028D0)
;;     000039E8 (in fn000028D0)
;;     00003A8C (in fn000028D0)
;;     00003FC8 (in fn00003FC8)
;;     00004458 (in fn000042E0)
;;     00004470 (in fn000042E0)
;;     000044EC (in fn000042E0)
;;     00004A98 (in fn000049A0)
;;     00004AAC (in fn000049A0)
;;     00004AE0 (in fn000049A0)
;;     00004B8C (in fn000049A0)
;;     00004B9C (in fn000049A0)
printf proc
	or.u	r11,r0,5
	ld	r11,r11,0x6A84
	jmp	r11
000361B8                         5D 60 00 00 59 6B 00 3C         ]`..Yk.<
000361C0 C3 FF FF CB                                     ....            

;; auth_clean: 000361C4
;;   Called from:
;;     00003018 (in fn000028D0)
auth_clean proc
	or.u	r11,r0,5
	ld	r11,r11,0x6A88
	jmp	r11
000361D0 5D 60 00 00 59 6B 00 48 C3 FF FF C5             ]`..Yk.H....    

;; geteuid: 000361DC
;;   Called from:
;;     00002D18 (in fn000028D0)
geteuid proc
	or.u	r11,r0,5
	ld	r11,r11,0x6A8C
	jmp	r11
000361E8                         5D 60 00 00 59 6B 00 54         ]`..Yk.T
000361F0 C3 FF FF BF                                     ....            

;; snprintf: 000361F4
;;   Called from:
;;     00002D4C (in fn000028D0)
;;     0000347C (in fn000028D0)
snprintf proc
	or.u	r11,r0,5
	ld	r11,r11,0x6A90
	jmp	r11
00036200 5D 60 00 00 59 6B 00 60 C3 FF FF B9             ]`..Yk.`....    

;; getenv: 0003620C
;;   Called from:
;;     00002F2C (in fn000028D0)
;;     00002F54 (in fn000028D0)
getenv proc
	or.u	r11,r0,5
	ld	r11,r11,0x6A94
	jmp	r11
00036218                         5D 60 00 00 59 6B 00 6C         ]`..Yk.l
00036220 C3 FF FF B3 5D 60 00 05 15 6B 6A 98 F4 00 C0 0B ....]`...kj.....
00036230 5D 60 00 00 59 6B 00 78 C3 FF FF AD             ]`..Yk.x....    

;; __srget: 0003623C
;;   Called from:
;;     00004090 (in fn00003FC8)
__srget proc
	or.u	r11,r0,5
	ld	r11,r11,0x6A9C
	jmp	r11
00036248                         5D 60 00 00 59 6B 00 84         ]`..Yk..
00036250 C3 FF FF A7                                     ....            

;; setpriority: 00036254
;;   Called from:
;;     00002DF8 (in fn000028D0)
setpriority proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AA0
	jmp	r11
00036260 5D 60 00 00 59 6B 00 90 C3 FF FF A1             ]`..Yk......    

;; getc: 0003626C
;;   Called from:
;;     0000409C (in fn00003FC8)
getc proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AA4
	jmp	r11
00036278                         5D 60 00 00 59 6B 00 9C         ]`..Yk..
00036280 C3 FF FF 9B                                     ....            

;; memcpy: 00036284
;;   Called from:
;;     00004554 (in fn00004520)
memcpy proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AA8
	jmp	r11
00036290 5D 60 00 00 59 6B 00 A8 C3 FF FF 95             ]`..Yk......    

;; auth_open: 0003629C
;;   Called from:
;;     00002984 (in fn000028D0)
auth_open proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AAC
	jmp	r11
000362A8                         5D 60 00 00 59 6B 00 B4         ]`..Yk..
000362B0 C3 FF FF 8F                                     ....            

;; auth_getstate: 000362B4
;;   Called from:
;;     00002F0C (in fn000028D0)
;;     00003818 (in fn000028D0)
;;     00003CC8 (in fn000028D0)
auth_getstate proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AB0
	jmp	r11
000362C0 5D 60 00 00 59 6B 00 C0 C3 FF FF 89             ]`..Yk......    

;; puts: 000362CC
;;   Called from:
;;     00003238 (in fn000028D0)
;;     0000382C (in fn000028D0)
;;     0000385C (in fn000028D0)
;;     00003894 (in fn000028D0)
;;     00003A04 (in fn000028D0)
puts proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AB4
	jmp	r11
000362D8                         5D 60 00 00 59 6B 00 CC         ]`..Yk..
000362E0 C3 FF FF 83                                     ....            

;; getuid: 000362E4
;;   Called from:
;;     000029A8 (in fn000028D0)
getuid proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AB8
	jmp	r11
000362F0 5D 60 00 00 59 6B 00 D8 C3 FF FF 7D             ]`..Yk.....}    

;; auth_setenv: 000362FC
;;   Called from:
;;     00002F24 (in fn000028D0)
;;     0000351C (in fn000028D0)
auth_setenv proc
	or.u	r11,r0,5
	ld	r11,r11,0x6ABC
	jmp	r11
00036308                         5D 60 00 00 59 6B 00 E4         ]`..Yk..
00036310 C3 FF FF 77                                     ...w            

;; auth_clroptions: 00036314
;;   Called from:
;;     00002F6C (in fn000028D0)
auth_clroptions proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AC0
	jmp	r11
00036320 5D 60 00 00 59 6B 00 F0 C3 FF FF 71             ]`..Yk.....q    

;; endpwent: 0003632C
;;   Called from:
;;     0000339C (in fn000028D0)
endpwent proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AC4
	jmp	r11
00036338                         5D 60 00 00 59 6B 00 FC         ]`..Yk..
00036340 C3 FF FF 6B                                     ...k            

;; sleep: 00036344
;;   Called from:
;;     00004768 (in fn00004740)
sleep proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AC8
	jmp	r11
00036350 5D 60 00 00 59 6B 01 08 C3 FF FF 65             ]`..Yk.....e    

;; login_getcapstr: 0003635C
;;   Called from:
;;     00002E54 (in fn000028D0)
;;     000033B4 (in fn000028D0)
;;     000038C0 (in fn000028D0)
;;     00004154 (in fn00004110)
;;     00004720 (in fn000046E0)
login_getcapstr proc
	or.u	r11,r0,5
	ld	r11,r11,0x6ACC
	jmp	r11
00036368                         5D 60 00 00 59 6B 01 14         ]`..Yk..
00036370 C3 FF FF 5F                                     ..._            

;; auth_checknologin: 00036374
;;   Called from:
;;     00003AA4 (in fn000028D0)
auth_checknologin proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AD0
	jmp	r11
00036380 5D 60 00 00 59 6B 01 20 C3 FF FF 59             ]`..Yk. ...Y    

;; lseek: 0003638C
;;   Called from:
;;     00004350 (in fn000042E0)
;;     0000442C (in fn000042E0)
;;     0000486C (in fn00004848)
;;     000048BC (in fn00004848)
;;     00004A20 (in fn000049A0)
;;     00004B44 (in fn000049A0)
lseek proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AD4
	jmp	r11
00036398                         5D 60 00 00 59 6B 01 2C         ]`..Yk.,
000363A0 C3 FF FF 53                                     ...S            

;; chown: 000363A4
;;   Called from:
;;     00003678 (in fn000028D0)
chown proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AD8
	jmp	r11
000363B0 5D 60 00 00 59 6B 01 38 C3 FF FF 4D             ]`..Yk.8...M    

;; freeaddrinfo: 000363BC
;;   Called from:
;;     00002B20 (in fn000028D0)
freeaddrinfo proc
	or.u	r11,r0,5
	ld	r11,r11,0x6ADC
	jmp	r11
000363C8                         5D 60 00 00 59 6B 01 44         ]`..Yk.D
000363D0 C3 FF FF 47                                     ...G            

;; alarm: 000363D4
;;   Called from:
;;     00003394 (in fn000028D0)
;;     00003DFC (in fn000028D0)
;;     00003E54 (in fn000028D0)
alarm proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AE0
	jmp	r11
000363E0 5D 60 00 00 59 6B 01 50 C3 FF FF 41             ]`..Yk.P...A    

;; strrchr: 000363EC
;;   Called from:
;;     00002D5C (in fn000028D0)
;;     00002EB4 (in fn000028D0)
;;     00003734 (in fn000028D0)
strrchr proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AE4
	jmp	r11
000363F8                         5D 60 00 00 59 6B 01 5C         ]`..Yk.\
00036400 C3 FF FF 3B                                     ...;            

;; calloc: 00036404
;;   Called from:
;;     000033DC (in fn000028D0)
calloc proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AE8
	jmp	r11
00036410 5D 60 00 00 59 6B 01 68 C3 FF FF 35             ]`..Yk.h...5    

;; setrlimit: 0003641C
;;   Called from:
;;     00002DA0 (in fn000028D0)
;;     00003774 (in fn000028D0)
setrlimit proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AEC
	jmp	r11
00036428                         5D 60 00 00 59 6B 01 74         ]`..Yk.t
00036430 C3 FF FF 2F                                     .../            

;; write: 00036434
;;   Called from:
;;     000041C8 (in fn00004110)
;;     000043A8 (in fn000042E0)
;;     00004918 (in fn00004848)
;;     00004B58 (in fn000049A0)
write proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AF0
	jmp	r11
00036440 5D 60 00 00 59 6B 01 80 C3 FF FF 29             ]`..Yk.....)    

;; ctime: 0003644C
;;   Called from:
;;     000039D4 (in fn000028D0)
;;     00004444 (in fn000042E0)
;;     00004A84 (in fn000049A0)
ctime proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AF4
	jmp	r11
00036458                         5D 60 00 00 59 6B 01 8C         ]`..Yk..
00036460 C3 FF FF 23                                     ...#            

;; chdir: 00036464
;;   Called from:
;;     00003544 (in fn000028D0)
;;     00003A68 (in fn000028D0)
chdir proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AF8
	jmp	r11
00036470 5D 60 00 00 59 6B 01 98 C3 FF FF 1D             ]`..Yk......    

;; login: 0003647C
;;   Called from:
;;     00003630 (in fn000028D0)
login proc
	or.u	r11,r0,5
	ld	r11,r11,0x6AFC
	jmp	r11
00036488                         5D 60 00 00 59 6B 01 A4         ]`..Yk..
00036490 C3 FF FF 17                                     ....            

;; auth_setoption: 00036494
;;   Called from:
;;     000029A0 (in fn000028D0)
;;     00002B38 (in fn000028D0)
;;     00002BF4 (in fn000028D0)
;;     00002F88 (in fn000028D0)
;;     00002FA4 (in fn000028D0)
;;     00002FD0 (in fn000028D0)
;;     00002FEC (in fn000028D0)
;;     00003CF4 (in fn000028D0)
;;     00003D54 (in fn000028D0)
;;     00003E18 (in fn000028D0)
auth_setoption proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B00
	jmp	r11
000364A0 5D 60 00 00 59 6B 01 B0 C3 FF FF 11             ]`..Yk......    

;; signal: 000364AC
;;   Called from:
;;     00002DB8 (in fn000028D0)
;;     00002DD0 (in fn000028D0)
;;     00002DDC (in fn000028D0)
;;     00002DE8 (in fn000028D0)
;;     000035D8 (in fn000028D0)
;;     000036F0 (in fn000028D0)
;;     000036FC (in fn000028D0)
;;     00003708 (in fn000028D0)
;;     00003714 (in fn000028D0)
;;     00003720 (in fn000028D0)
;;     00003CA0 (in fn000028D0)
signal proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B04
	jmp	r11
000364B8                         5D 60 00 00 59 6B 01 BC         ]`..Yk..
000364C0 C3 FF FF 0B                                     ....            

;; read: 000364C4
;;   Called from:
;;     000041AC (in fn00004110)
;;     000043F0 (in fn000042E0)
;;     0000487C (in fn00004848)
;;     00004A30 (in fn000049A0)
read proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B08
	jmp	r11
000364D0 5D 60 00 00 59 6B 01 C8 C3 FF FF 05             ]`..Yk......    

;; openlog: 000364DC
;;   Called from:
;;     00002924 (in fn000028D0)
openlog proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B0C
	jmp	r11
000364E8                         5D 60 00 00 59 6B 01 D4         ]`..Yk..
000364F0 C3 FF FE FF                                     ....            

;; strlcpy: 000364F4
;;   Called from:
;;     00002B18 (in fn000028D0)
;;     00002B48 (in fn000028D0)
;;     00003120 (in fn000028D0)
;;     00003748 (in fn000028D0)
;;     00003B80 (in fn000028D0)
;;     00003F84 (in fn000028D0)
strlcpy proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B10
	jmp	r11
00036500 5D 60 00 00 59 6B 01 E0 C3 FF FE F9             ]`..Yk......    

;; closelog: 0003650C
;;   Called from:
;;     00003EB4 (in fn000028D0)
closelog proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B14
	jmp	r11
00036518                         5D 60 00 00 59 6B 01 EC         ]`..Yk..
00036520 C3 FF FE F3                                     ....            

;; strncmp: 00036524
;;   Called from:
;;     00002F44 (in fn000028D0)
;;     000030D4 (in fn000028D0)
;;     00003BB4 (in fn000028D0)
;;     00003BE4 (in fn000028D0)
;;     00003BFC (in fn000028D0)
;;     00003C14 (in fn000028D0)
strncmp proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B18
	jmp	r11
00036530 5D 60 00 00 59 6B 01 F8 C3 FF FE ED             ]`..Yk......    

;; strncpy: 0003653C
;;   Called from:
;;     00003604 (in fn000028D0)
;;     00003628 (in fn000028D0)
;;     00003990 (in fn000028D0)
;;     00004388 (in fn000042E0)
;;     000043E0 (in fn000042E0)
;;     000048E0 (in fn00004848)
;;     000048F4 (in fn00004848)
;;     00004908 (in fn00004848)
strncpy proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B1C
	jmp	r11
00036548                         5D 60 00 00 59 6B 02 04         ]`..Yk..
00036550 C3 FF FE E7                                     ....            

;; setenv: 00036554
;;   Called from:
;;     00003400 (in fn000028D0)
;;     00003424 (in fn000028D0)
;;     00003490 (in fn000028D0)
;;     000034B4 (in fn000028D0)
;;     00003874 (in fn000028D0)
;;     00003AC4 (in fn000028D0)
;;     00003AF0 (in fn000028D0)
;;     00003B2C (in fn000028D0)
;;     00003B4C (in fn000028D0)
setenv proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B20
	jmp	r11
00036560 5D 60 00 00 59 6B 02 10 C3 FF FE E1             ]`..Yk......    

;; strcasecmp: 0003656C
;;   Called from:
;;     00002C18 (in fn000028D0)
strcasecmp proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B24
	jmp	r11
00036578                         5D 60 00 00 59 6B 02 1C         ]`..Yk..
00036580 C3 FF FE DB                                     ....            

;; __cxa_atexit: 00036584
;;   Called from:
;;     00002724 (in fn00002710)
__cxa_atexit proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B28
	jmp	r11
00036590 5D 60 00 00 59 6B 02 28 C3 FF FE D5             ]`..Yk.(....    

;; execv: 0003659C
;;   Called from:
;;     00003F04 (in fn000028D0)
execv proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B2C
	jmp	r11
000365A8                         5D 60 00 00 59 6B 02 34         ]`..Yk.4
000365B0 C3 FF FE CF                                     ....            

;; execlp: 000365B4
;;   Called from:
;;     000037FC (in fn000028D0)
execlp proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B30
	jmp	r11
000365C0 5D 60 00 00 59 6B 02 40 C3 FF FE C9             ]`..Yk.@....    

;; sigaction: 000365CC
;;   Called from:
;;     0000419C (in fn00004110)
;;     000041E0 (in fn00004110)
sigaction proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B34
	jmp	r11
000365D8                         5D 60 00 00 59 6B 02 4C         ]`..Yk.L
000365E0 C3 FF FE C3                                     ....            

;; login_getcaptime: 000365E4
;;   Called from:
;;     000039B0 (in fn000028D0)
;;     00003C80 (in fn000028D0)
login_getcaptime proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B38
	jmp	r11
000365F0 5D 60 00 00 59 6B 02 58 C3 FF FE BD             ]`..Yk.X....    

;; strdup: 000365FC
;;   Called from:
;;     00002BD8 (in fn000028D0)
;;     0000309C (in fn000028D0)
strdup proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B3C
	jmp	r11
00036608                         5D 60 00 00 59 6B 02 64         ]`..Yk.d
00036610 C3 FF FE B7                                     ....            

;; login_getcapnum: 00036614
;;   Called from:
;;     00002E2C (in fn000028D0)
;;     00003188 (in fn000028D0)
;;     000031B4 (in fn000028D0)
login_getcapnum proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B40
	jmp	r11
00036620 5D 60 00 00 59 6B 02 70 C3 FF FE B1             ]`..Yk.p....    

;; getopt: 0003662C
;;   Called from:
;;     000029C4 (in fn000028D0)
getopt proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B44
	jmp	r11
00036638                         5D 60 00 00 59 6B 02 7C         ]`..Yk.|
00036640 C3 FF FE AB                                     ....            

;; memset: 00036644
;;   Called from:
;;     000035E8 (in fn000028D0)
;;     0000436C (in fn000042E0)
;;     00004894 (in fn00004848)
;;     000049DC (in fn000049A0)
memset proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B48
	jmp	r11
00036650 5D 60 00 00 59 6B 02 88 C3 FF FE A5             ]`..Yk......    

;; err: 0003665C
;;   Called from:
;;     00003810 (in fn000028D0)
;;     00003B90 (in fn000028D0)
err proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B4C
	jmp	r11
00036668                         5D 60 00 00 59 6B 02 94         ]`..Yk..
00036670 C3 FF FE 9F                                     ....            

;; auth_setpwd: 00036674
;;   Called from:
;;     00003D5C (in fn000028D0)
auth_setpwd proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B50
	jmp	r11
00036680 5D 60 00 00 59 6B 02 A0 C3 FF FE 99             ]`..Yk......    

;; __swbuf: 0003668C
;;   Called from:
;;     000044C4 (in fn000042E0)
;;     00004B64 (in fn000049A0)
__swbuf proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B54
	jmp	r11
00036698                         5D 60 00 00 59 6B 02 AC         ]`..Yk..
000366A0 C3 FF FE 93                                     ....            

;; auth_setitem: 000366A4
;;   Called from:
;;     00003000 (in fn000028D0)
;;     0000308C (in fn000028D0)
auth_setitem proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B58
	jmp	r11
000366B0 5D 60 00 00 59 6B 02 B8 C3 FF FE 8D             ]`..Yk......    

;; login_getcapbool: 000366BC
;;   Called from:
;;     00003A24 (in fn000028D0)
;;     00003A58 (in fn000028D0)
login_getcapbool proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B5C
	jmp	r11
000366C8                         5D 60 00 00 59 6B 02 C4         ]`..Yk..
000366D0 C3 FF FE 87                                     ....            

;; time: 000366D4
;;   Called from:
;;     000035F0 (in fn000028D0)
;;     00004374 (in fn000042E0)
;;     000048D0 (in fn00004848)
time proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B60
	jmp	r11
000366E0 5D 60 00 00 59 6B 02 D0 C3 FF FE 81             ]`..Yk......    

;; syslog: 000366EC
;;   Called from:
;;     00002CB8 (in fn000028D0)
;;     00002E94 (in fn000028D0)
;;     0000335C (in fn000028D0)
;;     00003384 (in fn000028D0)
;;     000036DC (in fn000028D0)
;;     000038A4 (in fn000028D0)
;;     00003964 (in fn000028D0)
;;     00003C48 (in fn000028D0)
;;     00003D08 (in fn000028D0)
;;     00003D70 (in fn000028D0)
;;     00003DD8 (in fn000028D0)
;;     00003E60 (in fn000028D0)
;;     00003E80 (in fn000028D0)
;;     00003F3C (in fn000028D0)
;;     00003F6C (in fn000028D0)
syslog proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B64
	jmp	r11
000366F8                         5D 60 00 00 59 6B 02 DC         ]`..Yk..
00036700 C3 FF FE 7B                                     ...{            

;; auth_check_expire: 00036704
;;   Called from:
;;     000035B0 (in fn000028D0)
auth_check_expire proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B68
	jmp	r11
00036710 5D 60 00 00 59 6B 02 E8 C3 FF FE 75             ]`..Yk.....u    

;; seteuid: 0003671C
;;   Called from:
;;     00003538 (in fn000028D0)
;;     00003574 (in fn000028D0)
seteuid proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B6C
	jmp	r11
00036728                         5D 60 00 00 59 6B 02 F4         ]`..Yk..
00036730 C3 FF FE 6F                                     ...o            

;; putc: 00036734
;;   Called from:
;;     000044D8 (in fn000042E0)
;;     00004B78 (in fn000049A0)
putc proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B70
	jmp	r11
00036740 5D 60 00 00 59 6B 03 00 C3 FF FE 69             ]`..Yk.....i    

;; strcmp: 0003674C
;;   Called from:
;;     00003564 (in fn000028D0)
;;     00003D90 (in fn000028D0)
strcmp proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B74
	jmp	r11
00036758                         5D 60 00 00 59 6B 03 0C         ]`..Yk..
00036760 C3 FF FE 63                                     ...c            

;; ttyname: 00036764
;;   Called from:
;;     00002D20 (in fn000028D0)
ttyname proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B78
	jmp	r11
00036770 5D 60 00 00 59 6B 03 18 C3 FF FE 5D             ]`..Yk.....]    

;; getpwnam: 0003677C
;;   Called from:
;;     0000312C (in fn000028D0)
getpwnam proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B7C
	jmp	r11
00036788                         5D 60 00 00 59 6B 03 24         ]`..Yk.$
00036790 C3 FF FE 57                                     ...W            

;; gethostname: 00036794
;;   Called from:
;;     00002930 (in fn000028D0)
gethostname proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B80
	jmp	r11
000367A0 5D 60 00 00 59 6B 03 30 C3 FF FE 51             ]`..Yk.0...Q    

;; auth_call: 000367AC
;;   Called from:
;;     00002F04 (in fn000028D0)
auth_call proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B84
	jmp	r11
000367B8                         5D 60 00 00 59 6B 03 3C         ]`..Yk.<
000367C0 C3 FF FE 4B                                     ...K            

;; auth_approval: 000367C4
;;   Called from:
;;     000037CC (in fn000028D0)
auth_approval proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B88
	jmp	r11
000367D0 5D 60 00 00 59 6B 03 48 C3 FF FE 45             ]`..Yk.H...E    

;; getttynam: 000367DC
;;   Called from:
;;     000040DC (in fn000040D0)
;;     000046F4 (in fn000046E0)
getttynam proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B8C
	jmp	r11
000367E8                         5D 60 00 00 59 6B 03 54         ]`..Yk.T
000367F0 C3 FF FE 3F                                     ...?            

;; auth_verify: 000367F4
;;   Called from:
;;     00003CC0 (in fn000028D0)
auth_verify proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B90
	jmp	r11
00036800 5D 60 00 00 59 6B 03 60 C3 FF FE 39             ]`..Yk.`...9    

;; getaddrinfo: 0003680C
;;   Called from:
;;     00002B00 (in fn000028D0)
getaddrinfo proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B94
	jmp	r11
00036818                         5D 60 00 00 59 6B 03 6C         ]`..Yk.l
00036820 C3 FF FE 33                                     ...3            

;; login_getstyle: 00036824
;;   Called from:
;;     00003160 (in fn000028D0)
login_getstyle proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B98
	jmp	r11
00036830 5D 60 00 00 59 6B 03 78 C3 FF FE 2D             ]`..Yk.x...-    

;; auth_setstate: 0003683C
;;   Called from:
;;     00002EC8 (in fn000028D0)
;;     000037E4 (in fn000028D0)
auth_setstate proc
	or.u	r11,r0,5
	ld	r11,r11,0x6B9C
	jmp	r11
00036848                         5D 60 00 00 59 6B 03 84         ]`..Yk..
00036850 C3 FF FE 27                                     ...'            

;; stat: 00036854
;;   Called from:
;;     000038D8 (in fn000028D0)
stat proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BA0
	jmp	r11
00036860 5D 60 00 00 59 6B 03 90 C3 FF FE 21             ]`..Yk.....!    

;; fwrite: 0003686C
;;   Called from:
;;     00002C98 (in fn000028D0)
;;     00004088 (in fn00003FC8)
fwrite proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BA4
	jmp	r11
00036878                         5D 60 00 00 59 6B 03 9C         ]`..Yk..
00036880 C3 FF FE 1B                                     ....            

;; access: 00036884
;;   Called from:
;;     00003A38 (in fn000028D0)
access proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BA8
	jmp	r11
00036890 5D 60 00 00 59 6B 03 A8 C3 FF FE 15             ]`..Yk......    

;; syslog_r: 0003689C
;;   Called from:
;;     000045BC (in fn00004520)
;;     00004618 (in fn00004520)
;;     00004664 (in fn00004520)
;;     0000469C (in fn00004520)
syslog_r proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BAC
	jmp	r11
000368A8                         5D 60 00 00 59 6B 03 B4         ]`..Yk..
000368B0 C3 FF FE 0F                                     ....            

;; exit: 000368B4
;;   Called from:
;;     00002674 (in fn00002570)
;;     00004770 (in fn00004740)
;;     000047A4 (in fn00004780)
exit proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BB0
	jmp	r11
000368C0 5D 60 00 00 59 6B 03 C0 C3 FF FE 09             ]`..Yk......    

;; login_getclass: 000368CC
;;   Called from:
;;     00002E00 (in fn000028D0)
;;     00003150 (in fn000028D0)
login_getclass proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BB4
	jmp	r11
000368D8                         5D 60 00 00 59 6B 03 CC         ]`..Yk..
000368E0 C3 FF FE 03                                     ....            

;; getgrnam: 000368E4
;;   Called from:
;;     00003660 (in fn000028D0)
getgrnam proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BB8
	jmp	r11
000368F0 5D 60 00 00 59 6B 03 D8 C3 FF FD FD             ]`..Yk......    

;; _exit: 000368FC
;;   Called from:
;;     00003F1C (in fn000028D0)
;;     000047DC (in fn00004780)
_exit proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BBC
	jmp	r11
00036908                         5D 60 00 00 59 6B 03 E4         ]`..Yk..
00036910 C3 FF FD F7                                     ....            

;; auth_cat: 00036914
;;   Called from:
;;     000038CC (in fn000028D0)
auth_cat proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BC0
	jmp	r11
00036920 5D 60 00 00 59 6B 03 F0 C3 FF FD F1             ]`..Yk......    

;; __muldi3: 0003692C
;;   Called from:
;;     0000433C (in fn000042E0)
;;     00004414 (in fn000042E0)
;;     00004854 (in fn00004848)
;;     000048A8 (in fn00004848)
;;     00004A0C (in fn000049A0)
;;     00004B30 (in fn000049A0)
__muldi3 proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BC4
	jmp	r11
00036938                         5D 60 00 00 59 6B 03 FC         ]`..Yk..
00036940 C3 FF FD EB                                     ....            

;; setusercontext: 00036944
;;   Called from:
;;     0000350C (in fn000028D0)
;;     000037A4 (in fn000028D0)
setusercontext proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BC8
	jmp	r11
00036950 5D 60 00 00 59 6B 04 08 C3 FF FD E5             ]`..Yk......    

;; strlen: 0003695C
;;   Called from:
;;     000030EC (in fn000028D0)
;;     00003C30 (in fn000028D0)
strlen proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BCC
	jmp	r11
00036968                         5D 60 00 00 59 6B 04 14         ]`..Yk..
00036970 C3 FF FD DF                                     ....            

;; auth_close: 00036974
;;   Called from:
;;     000037EC (in fn000028D0)
;;     00003EAC (in fn000028D0)
;;     00004760 (in fn00004740)
;;     000047B0 (in fn00004780)
auth_close proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BD0
	jmp	r11
00036980 5D 60 00 00 59 6B 04 20 C3 FF FD D9             ]`..Yk. ....    

;; open: 0003698C
;;   Called from:
;;     0000415C (in fn00004110)
;;     0000431C (in fn000042E0)
;;     00004838 (in fn00004780)
;;     00004838 (in fn000047F0)
;;     000049F0 (in fn000049A0)
open proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BD4
	jmp	r11
00036998                         5D 60 00 00 59 6B 04 2C         ]`..Yk.,
000369A0 C3 FF FD D3                                     ....            

;; strchr: 000369A4
;;   Called from:
;;     00002964 (in fn000028D0)
;;     00002C04 (in fn000028D0)
;;     00003058 (in fn000028D0)
;;     000030B8 (in fn000028D0)
;;     00003F50 (in fn000028D0)
strchr proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BD8
	jmp	r11
000369B0 5D 60 00 00 59 6B 04 38 C3 FF FD CD             ]`..Yk.8....    

;; setegid: 000369BC
;;   Called from:
;;     0000352C (in fn000028D0)
;;     0000357C (in fn000028D0)
setegid proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BDC
	jmp	r11
000369C8                         5D 60 00 00 59 6B 04 44         ]`..Yk.D
000369D0 C3 FF FD C7                                     ....            

;; warn: 000369D4
;;   Called from:
;;     00002C48 (in fn000028D0)
;;     0000343C (in fn000028D0)
;;     000034C8 (in fn000028D0)
;;     0000387C (in fn000028D0)
;;     00003AAC (in fn000028D0)
;;     00003AD8 (in fn000028D0)
;;     00003B08 (in fn000028D0)
;;     00003D10 (in fn000028D0)
;;     00003D78 (in fn000028D0)
;;     00003DE0 (in fn000028D0)
;;     00003F14 (in fn000028D0)
warn proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BE0
	jmp	r11
000369E0 5D 60 00 00 59 6B 04 50 C3 FF FD C1             ]`..Yk.P....    

;; warnc: 000369EC
;;   Called from:
;;     00002B54 (in fn000028D0)
;;     00002BB0 (in fn000028D0)
;;     00002C5C (in fn000028D0)
;;     00002CE0 (in fn000028D0)
warnc proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BE4
	jmp	r11
000369F8                         5D 60 00 00 59 6B 04 5C         ]`..Yk.\
00036A00 C3 FF FD BB                                     ....            

;; close: 00036A04
;;   Called from:
;;     000041E8 (in fn00004110)
;;     000043B0 (in fn000042E0)
;;     00004920 (in fn00004848)
;;     00004A40 (in fn000049A0)
close proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BE8
	jmp	r11
00036A10 5D 60 00 00 59 6B 04 68 C3 FF FD B5             ]`..Yk.h....    

;; closefrom: 00036A1C
;;   Called from:
;;     000037D8 (in fn000028D0)
;;     00003EB8 (in fn000028D0)
closefrom proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BEC
	jmp	r11
00036A28                         5D 60 00 00 59 6B 04 74         ]`..Yk.t
00036A30 C3 FF FD AF                                     ....            

;; auth_getvalue: 00036A34
;;   Called from:
;;     000032D4 (in fn000028D0)
;;     0000358C (in fn000028D0)
auth_getvalue proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BF0
	jmp	r11
00036A40 5D 60 00 00 59 6B 04 80 C3 FF FD A9             ]`..Yk......    

;; free: 00036A4C
;;   Called from:
;;     00002BD0 (in fn000028D0)
;;     00003078 (in fn000028D0)
free proc
	or.u	r11,r0,5
	ld	r11,r11,0x6BF4
	jmp	r11
00036A58                         5D 60 00 00 59 6B 04 8C         ]`..Yk..
00036A60 C3 FF FD A3                                     ....            
