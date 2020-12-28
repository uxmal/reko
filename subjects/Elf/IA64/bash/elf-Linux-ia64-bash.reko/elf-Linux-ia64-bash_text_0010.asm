;;; Segment .text (400000000001C480)

;; complete_builtin: 400000000011CF40
complete_builtin proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; compgen_builtin: 400000000011D940
compgen_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; compopt_builtin: 400000000011E280
compopt_builtin proc
	Invalid
	adds	r12,-16,r12
	addl	r34,9276,r1

;; glob_pattern_p: 400000000011F840
glob_pattern_p proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; glob_vector: 400000000011FA80
glob_vector proc
	Invalid
	adds	r61,0,r12
	Invalid

;; glob_filename: 4000000000120D80
glob_filename proc
	Invalid
	Invalid
	adds	r52,0,r1

;; strmatch: 4000000000121C40
strmatch proc
	Invalid
	Invalid
	Invalid

;; wcsmatch: 4000000000121D00
wcsmatch proc
	Invalid
	Invalid
	Invalid

;; internal_strmatch: 4000000000127100
internal_strmatch proc
	Invalid
	Invalid
	Invalid

;; internal_wstrmatch: 40000000001271C0
internal_wstrmatch proc
	Invalid
	Invalid
	Invalid

;; xstrmatch: 40000000001272C0
xstrmatch proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; xmbsrtowcs: 4000000000127500
xmbsrtowcs proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; xdupmbstowcs: 40000000001278C0
xdupmbstowcs proc
	Invalid
	adds	r12,-48,r12
	Invalid

;; match_pattern_wchar: 4000000000128180
match_pattern_wchar proc
	Invalid
	Invalid
	nop.i	0x0

;; wmatchlen: 4000000000128300
wmatchlen proc
	Invalid
	addl	r17,-5748,r1
	adds	r15,4,r32

;; match_pattern_char: 4000000000128800
match_pattern_char proc
	Invalid
	Invalid
	Invalid

;; umatchlen: 40000000001289C0
umatchlen proc
	Invalid
	addl	r17,-5740,r1
	adds	r15,1,r32

;; getenv: 4000000000129000
getenv proc
	Invalid
	Invalid
	Invalid

;; _getenv: 4000000000129380
_getenv proc
	Invalid
	nop.i	0x0
	Invalid

;; putenv: 40000000001293C0
putenv proc
	Invalid
	Invalid
	Invalid

;; setenv: 4000000000129580
setenv proc
	Invalid
	adds	r37,0,r1
	Invalid

;; unsetenv: 4000000000129780
unsetenv proc
	Invalid
	Invalid
	Invalid

;; getmaxgroups: 4000000000129880
getmaxgroups proc
	Invalid
	Invalid
	addl	r32,6420,r1

;; getmaxchild: 4000000000129940
getmaxchild proc
	Invalid
	Invalid
	addl	r32,6428,r1

;; sh_setlinebuf: 40000000001299C0
sh_setlinebuf proc
	Invalid
	Invalid
	adds	r35,0,r1

;; inttostr: 4000000000129A40
inttostr proc
	Invalid
	adds	r37,0,r1
	Invalid

;; itos: 4000000000129AC0
itos proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; uinttostr: 4000000000129B80
uinttostr proc
	Invalid
	adds	r37,0,r1
	Invalid

;; uitos: 4000000000129C00
uitos proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; zread: 4000000000129CC0
zread proc
	Invalid
	Invalid
	adds	r38,0,r1

;; zreadretry: 4000000000129D80
zreadretry proc
	Invalid
	Invalid
	nop.i	0x0

;; zreadintr: 4000000000129E40
zreadintr proc
	Invalid
	adds	r37,0,r1
	Invalid

;; zreadcn: 4000000000129EC0
zreadcn proc
	Invalid
	addl	r35,8796,r1
	Invalid

;; zreadc: 400000000012A040
zreadc proc
	Invalid
	addl	r34,128,r0
	nop.i	0x0

;; zreadcintr: 400000000012A080
zreadcintr proc
	Invalid
	addl	r34,8796,r1
	Invalid

;; zreset: 400000000012A1C0
zreset proc
	addl	r14,8804,r1
	Invalid
	addl	r14,8796,r1

;; zsyncfd: 400000000012A200
zsyncfd proc
	Invalid
	addl	r34,8804,r1
	Invalid

;; ttgetattr: 400000000012A2C0
ttgetattr proc
	Invalid
	Invalid
	Invalid

;; ttsetattr: 400000000012A340
ttsetattr proc
	Invalid
	Invalid
	Invalid

;; ttsave: 400000000012A3C0
ttsave proc
	Invalid
	Invalid
	addl	r32,8812,r1

;; ttrestore: 400000000012A480
ttrestore proc
	Invalid
	Invalid
	addl	r32,8812,r1

;; ttattr: 400000000012A540
ttattr proc
	Invalid
	addl	r14,8812,r1
	Invalid

;; tt_setonechar: 400000000012A5C0
tt_setonechar proc
	Invalid
	adds	r18,23,r32
	addl	r19,1,r0

;; ttfd_onechar: 400000000012A680
ttfd_onechar proc
	Invalid
	Invalid
	Invalid

;; ttonechar: 400000000012A740
ttonechar proc
	Invalid
	adds	r12,-64,r12
	Invalid

;; tt_setnoecho: 400000000012A800
tt_setnoecho proc
	adds	r32,12,r32
	Invalid
	adds	r8,0,r0

;; ttfd_noecho: 400000000012A840
ttfd_noecho proc
	adds	r14,12,r33
	Invalid
	nop.i	0x0

;; ttnoecho: 400000000012A880
ttnoecho proc
	Invalid
	adds	r12,-64,r12
	Invalid

;; tt_seteightbit: 400000000012A940
tt_seteightbit proc
	Invalid
	addl	r15,-257,r0
	adds	r8,0,r0

;; ttfd_eightbit: 400000000012A9C0
ttfd_eightbit proc
	Invalid
	adds	r14,0,r33
	addl	r16,-257,r0

;; tteightbit: 400000000012AA40
tteightbit proc
	Invalid
	adds	r12,-64,r12
	Invalid

;; tt_setnocanon: 400000000012AB00
tt_setnocanon proc
	adds	r32,12,r32
	Invalid
	adds	r8,0,r0

;; ttfd_nocanon: 400000000012AB40
ttfd_nocanon proc
	adds	r14,12,r33
	Invalid
	nop.i	0x0

;; ttnocanon: 400000000012AB80
ttnocanon proc
	Invalid
	adds	r12,-64,r12
	Invalid

;; tt_setcbreak: 400000000012AC40
tt_setcbreak proc
	Invalid
	Invalid
	Invalid

;; ttfd_cbreak: 400000000012ACC0
ttfd_cbreak proc
	Invalid
	Invalid
	Invalid

;; ttcbreak: 400000000012AD80
ttcbreak proc
	Invalid
	adds	r12,-64,r12
	Invalid

;; sh_regmatch: 400000000012AE40
sh_regmatch proc
	Invalid
	adds	r12,-64,r12
	Invalid

;; sh_stat: 400000000012B200
sh_stat proc
	Invalid
	Invalid
	Invalid

;; sh_eaccess: 400000000012B440
sh_eaccess proc
	Invalid
	adds	r12,-144,r12
	Invalid

;; isnetconn: 400000000012B8C0
isnetconn proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; netopen: 400000000012B9C0
netopen proc
	Invalid
	adds	r12,-64,r12
	Invalid

;; difftimeval: 400000000012BF80
difftimeval proc
	Invalid
	adds	r14,0,r32
	adds	r8,0,r32

;; addtimeval: 400000000012C000
addtimeval proc
	Invalid
	adds	r17,8,r32
	adds	r8,0,r32

;; timeval_to_cpu: 400000000012C080
timeval_to_cpu proc
	Invalid
	Invalid
	Invalid
