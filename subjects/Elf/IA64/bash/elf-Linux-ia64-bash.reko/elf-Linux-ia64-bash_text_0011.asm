;;; Segment .text (400000000001C480)

;; timeval_to_secs: 400000000012C500
timeval_to_secs proc
	Invalid
	Invalid
	Invalid

;; print_timeval: 400000000012C680
print_timeval proc
	Invalid
	Invalid
	Invalid

;; sh_makepath: 400000000012C8C0
sh_makepath proc
	Invalid
	Invalid
	nop.i	0x0

;; sh_canonpath: 400000000012CE40
sh_canonpath proc
	Invalid
	Invalid
	Invalid

;; sh_physpath: 400000000012D540
sh_physpath proc
	Invalid
	addl	r17,-8208,r0
	nop.i	0x0

;; sh_realpath: 400000000012DF80
sh_realpath proc
	Invalid
	Invalid
	adds	r38,0,r1

;; sh_mktmpname: 400000000012E4C0
sh_mktmpname proc
	Invalid
	adds	r12,-144,r12
	Invalid

;; sh_mktmpfd: 400000000012E700
sh_mktmpfd proc
	Invalid
	Invalid
	adds	r46,0,r1

;; sh_mktmpfp: 400000000012EA00
sh_mktmpfp proc
	Invalid
	adds	r41,0,r33
	Invalid

;; strlist_create: 400000000012EB40
strlist_create proc
	Invalid
	adds	r36,0,r1
	Invalid

;; strlist_resize: 400000000012ECC0
strlist_resize proc
	Invalid
	adds	r34,8,r32
	Invalid

;; strlist_flush: 400000000012EE40
strlist_flush proc
	Invalid
	Invalid
	nop.i	0x0

;; strlist_dispose: 400000000012EEC0
strlist_dispose proc
	Invalid
	Invalid
	nop.i	0x0

;; strlist_remove: 400000000012EF80
strlist_remove proc
	Invalid
	Invalid
	adds	r34,12,r32

;; strlist_copy: 400000000012F080
strlist_copy proc
	Invalid
	Invalid
	Invalid

;; strlist_merge: 400000000012F280
strlist_merge proc
	Invalid
	Invalid
	Invalid

;; strlist_append: 400000000012F540
strlist_append proc
	Invalid
	Invalid
	Invalid

;; strlist_prefix_suffix: 400000000012F780
strlist_prefix_suffix proc
	Invalid
	Invalid
	Invalid

;; strlist_print: 400000000012FC00
strlist_print proc
	Invalid
	Invalid
	Invalid

;; strlist_walk: 400000000012FD00
strlist_walk proc
	Invalid
	adds	r36,12,r32
	Invalid

;; strlist_sort: 400000000012FE00
strlist_sort proc
	Invalid
	adds	r14,12,r32
	Invalid

;; strlist_from_word_list: 400000000012FEC0
strlist_from_word_list proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; strlist_to_word_list: 4000000000130000
strlist_to_word_list proc
	Invalid
	Invalid
	Invalid

;; strvec_strcmp: 40000000001300C0
strvec_strcmp proc
	Invalid
	Invalid
	adds	r36,0,r1

;; strvec_create: 4000000000130140
strvec_create proc
	Invalid
	Invalid
	Invalid

;; strvec_resize: 40000000001301C0
strvec_resize proc
	Invalid
	Invalid
	Invalid

;; strvec_len: 4000000000130240
strvec_len proc
	Invalid
	adds	r8,0,r0
	adds	r32,8,r32

;; strvec_flush: 40000000001302C0
strvec_flush proc
	Invalid
	Invalid
	Invalid

;; strvec_dispose: 4000000000130380
strvec_dispose proc
	Invalid
	Invalid
	Invalid

;; strvec_remove: 4000000000130400
strvec_remove proc
	Invalid
	Invalid
	Invalid

;; strvec_copy: 40000000001305C0
strvec_copy proc
	Invalid
	Invalid
	Invalid

;; strvec_sort: 4000000000130740
strvec_sort proc
	Invalid
	Invalid
	Invalid

;; strvec_from_word_list: 4000000000130800
strvec_from_word_list proc
	Invalid
	Invalid
	Invalid

;; strvec_to_word_list: 4000000000130A00
strvec_to_word_list proc
	Invalid
	adds	r17,8,r32
	Invalid

;; spname: 4000000000130C80
spname proc
	Invalid
	addl	r17,-8208,r0
	nop.i	0x0

;; dirspell: 4000000000131300
dirspell proc
	Invalid
	Invalid
	Invalid

;; sh_single_quote: 4000000000131400
sh_single_quote proc
	Invalid
	adds	r37,0,r32
	Invalid

;; sh_double_quote: 40000000001315C0
sh_double_quote proc
	Invalid
	adds	r37,0,r32
	Invalid

;; sh_mkdoublequoted: 4000000000131800
sh_mkdoublequoted proc
	Invalid
	Invalid
	Invalid

;; sh_un_double_quote: 4000000000131940
sh_un_double_quote proc
	Invalid
	Invalid
	Invalid

;; sh_backslash_quote: 4000000000131B00
sh_backslash_quote proc
	Invalid
	Invalid
	Invalid

;; sh_backslash_quote_for_double_quotes: 4000000000131D00
sh_backslash_quote_for_double_quotes proc
	Invalid
	Invalid
	Invalid

;; sh_contains_shell_metas: 4000000000131EC0
sh_contains_shell_metas proc
	Invalid
	Invalid
	addl	r17,-788,r1

;; ansicstr: 4000000000132040
ansicstr proc
	Invalid
	Invalid
	Invalid

;; ansic_quote: 4000000000132B80
ansic_quote proc
	Invalid
	Invalid
	Invalid

;; ansic_shouldquote: 4000000000132F80
ansic_shouldquote proc
	Invalid
	Invalid
	nop.i	0x0

;; ansiexpand: 4000000000133080
ansiexpand proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; mailstat: 4000000000133240
mailstat proc
	Invalid
	addl	r17,-16672,r0
	nop.i	0x0

;; fmtulong: 4000000000133AC0
fmtulong proc
	Invalid
	Invalid
	Invalid

;; fmtumax: 4000000000134380
fmtumax proc
	Invalid
	Invalid
	Invalid

;; zcatfd: 4000000000134C40
zcatfd proc
	Invalid
	adds	r12,-128,r12
	Invalid

;; zmapfd: 4000000000134D40
zmapfd proc
	Invalid
	adds	r12,-128,r12
	Invalid

;; get_new_window_size: 4000000000134FC0
get_new_window_size proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; fpurge: 4000000000135180
fpurge proc
	Invalid
	Invalid
	Invalid

;; zgetline: 4000000000135200
zgetline proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; uconvert: 4000000000135540
uconvert proc
	Invalid
	Invalid
	Invalid

;; falarm: 4000000000135AC0
falarm proc
	Invalid
	adds	r12,-64,r12
	Invalid

;; fsleep: 4000000000135BC0
fsleep proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; sh_modcase: 4000000000135C40
sh_modcase proc
	Invalid
	adds	r12,-80,r12
	Invalid

;; input_avail: 4000000000136D00
input_avail proc
	Invalid
	adds	r12,-272,r12
	Invalid

;; fnx_tofs: 4000000000136F00
fnx_tofs proc
	Invalid
	adds	r8,0,r32
	Invalid

;; fnx_fromfs: 4000000000136F40
fnx_fromfs proc
	Invalid
	adds	r8,0,r32
	Invalid

;; u32tochar: 4000000000136F80
u32tochar proc
	Invalid
	Invalid
	Invalid

;; u32toutf8: 4000000000137080
u32toutf8 proc
	Invalid
	addl	r14,1,r0
	addl	r8,1,r0

;; u32cconv: 4000000000137180
u32cconv proc
	Invalid
	Invalid
	Invalid

;; mbstrlen: 4000000000137200
mbstrlen proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; mbsmbchar: 4000000000137400
mbsmbchar proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; mbschr: 40000000001375C0
mbschr proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; zwrite: 4000000000137780
zwrite proc
	Invalid
	adds	r41,0,r1
	Invalid

;; __libc_csu_init: 4000000000137D00
__libc_csu_init proc
	Invalid
	Invalid
	adds	r38,0,r1

;; __libc_csu_fini: 4000000000137E00
__libc_csu_fini proc
	Invalid
	nop.i	0x0
	Invalid
