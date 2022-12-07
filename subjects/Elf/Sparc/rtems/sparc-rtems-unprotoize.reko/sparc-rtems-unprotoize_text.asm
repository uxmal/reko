;;; Segment .text (00011498)

;; _start: 00011498
_start proc
	or	%g0,00000000,%i6
	ld	[%sp+64],%l0
	add	%sp,00000044,%l1
	sub	%sp,00000020,%sp
	orcc	%g0,%g1,%g0
	be	000114BC
	or	%g0,%g1,%o0

l000114B4:
	call	atexit
	sethi	00000000,%g0

l000114BC:
	sethi	0000005B,%o0
	or	%o0,000002E4,%o0
	call	atexit
	sethi	00000000,%g0
	call	_init
	sethi	00000000,%g0
	or	%g0,%l0,%o0
	or	%g0,%l1,%o1
	sll	%l0,00000002,%o2
	add	%o2,00000004,%o2
	add	%l1,%o2,%o2
	sethi	000000AD,%o3
	or	%o3,0000000C,%o3
	st	%o2,[%o3+%g0]
	call	main
	sethi	00000000,%g0
	call	exit
00011500 01 00 00 00 40 00 59 DF 01 00 00 00             ....@.Y.....    

;; fn0001150C: 0001150C
;;   Called from:
;;     00011520 (in __do_global_dtors_aux)
;;     000115E4 (in frame_dummy)
fn0001150C proc
	jmpl	%o7,+00000008,%g0
	add	%o7,%l7,%l7

;; __do_global_dtors_aux: 00011514
;;   Called from:
;;     00016EE8 (in _fini)
__do_global_dtors_aux proc
	save	%sp,FFFFFF90,%sp
	sethi	00000000,%o0
	sethi	00000059,%l7
	call	fn0001150C
	add	%l7,00000260,%l7
	or	%o0,00000008,%o0
	ld	[%l7+%o0],%o1
	ld	[%o1+%g0],%o2
	subcc	%o2,00000000,%g0
	bne	000115C4
	sethi	00000000,%o0

l00011540:
	or	%o0,00000004,%o0
	ld	[%l7+%o0],%o2
	ld	[%o2+%g0],%o1
	ld	[%o1+%g0],%o0
	subcc	%o0,00000000,%g0
	be	00011590
	sethi	00000000,%o0

l0001155C:
	or	%g0,%o2,%l0
	ld	[%l0+%g0],%o0
	add	%o0,00000004,%o0

l00011568:
	st	%o0,[%l0+%g0]
	ld	[%o0-4],%o1
	jmpl	%o1,%g0,%o7
	sethi	00000000,%g0
	ld	[%l0+%g0],%o0
	ld	[%o0+%g0],%o1
	subcc	%o1,00000000,%g0
	bne	00011568
	add	%o0,00000004,%o0

l0001158C:
	sethi	00000000,%o0

l00011590:
	or	%o0,0000001C,%o0
	ld	[%l7+%o0],%o1
	subcc	%o1,00000000,%g0
	be	000115B0
	sethi	00000000,%o1

l000115A4:
	or	%o1,0000000C,%o1
	call	00027C8C
	ld	[%l7+%o1],%o0

l000115B0:
	sethi	00000000,%o0
	or	%o0,00000008,%o0
	ld	[%l7+%o0],%o2
	or	%g0,00000001,%o1
	st	%o1,[%o2+%g0]

l000115C4:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; fini_dummy: 000115CC
fini_dummy proc
	save	%sp,FFFFFF90,%sp
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; frame_dummy: 000115D8
;;   Called from:
;;     00016ECC (in _init)
frame_dummy proc
	save	%sp,FFFFFF90,%sp
	sethi	00000000,%o0
	sethi	00000059,%l7
	call	fn0001150C
	add	%l7,0000019C,%l7
	or	%o0,00000018,%o0
	ld	[%l7+%o0],%o1
	subcc	%o1,00000000,%g0
	be	00011618
	sethi	00000000,%o1

l00011600:
	or	%o1,0000000C,%o1
	sethi	00000000,%o2
	ld	[%l7+%o1],%o0
	or	%o2,00000010,%o2
	call	00027C98
	ld	[%l7+%o2],%o1

l00011618:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; init_dummy: 00011620
init_dummy proc
	save	%sp,FFFFFF90,%sp
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; notice: 0001162C
;;   Called from:
;;     00011698 (in xmalloc)
;;     000116F0 (in xrealloc)
;;     00011790 (in fancy_abort)
;;     000118F0 (in safe_write)
;;     000119AC (in usage)
;;     000120EC (in abspath)
;;     000123A4 (in find_file)
;;     000123E8 (in aux_info_corrupted)
;;     00012908 (in save_def_or_dec)
;;     00012ED0 (in gen_aux_info_file)
;;     000130B4 (in process_aux_info_file)
;;     000131F0 (in process_aux_info_file)
;;     000132E0 (in process_aux_info_file)
;;     00013338 (in process_aux_info_file)
;;     00013690 (in declare_source_confusing)
;;     000136C8 (in declare_source_confusing)
;;     00013968 (in edit_fn_declaration)
;;     00013DC4 (in edit_formals_lists)
;;     00013F80 (in edit_fn_definition)
;;     00013FC8 (in edit_fn_definition)
;;     00014834 (in scan_for_missed_items)
;;     00014848 (in scan_for_missed_items)
;;     00014910 (in edit_file)
;;     00014F38 (in main)
;;     0001516C (in main)
notice proc
	save	%sp,FFFFFF90,%sp
	sethi	000000AD,%o0
	st	%i1,[%i6+72]
	st	%i2,[%i6+76]
	st	%i3,[%i6+80]
	st	%i4,[%i6+84]
	st	%i5,[%i6+88]
	or	%o0,00000240,%o0
	or	%g0,%i0,%o1
	call	vfprintf
	add	%i6,00000048,%o2
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; xstrerror: 00011660
;;   Called from:
;;     000118DC (in safe_write)
;;     00012390 (in find_file)
;;     00012F50 (in gen_aux_info_file)
;;     000130E4 (in process_aux_info_file)
;;     000131DC (in process_aux_info_file)
;;     000132CC (in process_aux_info_file)
;;     00013324 (in process_aux_info_file)
;;     00014F28 (in main)
;;     00016BD8 (in pexecute)
xstrerror proc
	save	%sp,FFFFFF90,%sp
	call	strerror
	or	%g0,%i0,%o0
	jmpl	%i7,+00000008,%g0
	restore	%g0,%o0,%o0

;; xmalloc: 00011674
;;   Called from:
;;     00011730 (in savestring)
;;     00011758 (in savestring2)
;;     000117A8 (in dupnstr)
;;     00011BCC (in string_list_cons)
;;     00011D1C (in lookup)
;;     00011D7C (in unexpand_if_needed)
;;     00012178 (in shortpath)
;;     00012330 (in find_file)
;;     00012520 (in save_def_or_dec)
;;     00012D84 (in munge_compile_params)
;;     00013250 (in process_aux_info_file)
;;     000150B8 (in main)
;;     0001537C (in getpwd)
;;     000159BC (in choose_temp_base)
;;     00015B40 (in make_temp_file)
xmalloc proc
	save	%sp,FFFFFF90,%sp
	call	malloc
	or	%g0,%i0,%o0
	subcc	%o0,00000000,%g0
	bne	000116A8
	sethi	00000000,%g0

l0001168C:
	sethi	000000A0,%o0
	ld	[%o0+336],%o1
	sethi	0000005C,%o0
	call	notice
	or	%o0,000000A8,%o0
	call	exit
000116A4             90 10 20 21                             .. !        

l000116A8:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%o0,%o0

;; xrealloc: 000116B0
;;   Called from:
;;     00011E3C (in unexpand_if_needed)
;;     00011E98 (in unexpand_if_needed)
;;     00011EEC (in unexpand_if_needed)
;;     0001383C (in output_bytes)
xrealloc proc
	save	%sp,FFFFFF90,%sp
	orcc	%i0,00000000,%o0
	be	000116D0
	or	%g0,%i1,%o1

l000116C0:
	call	realloc
	sethi	00000000,%g0
	ba	000116DC
	subcc	%o0,00000000,%g0

l000116D0:
	call	malloc
	or	%g0,%o1,%o0
	subcc	%o0,00000000,%g0

l000116DC:
	bne	00011700
	sethi	00000000,%g0

l000116E4:
	sethi	000000A0,%o0
	ld	[%o0+336],%o1
	sethi	0000005C,%o0
	call	notice
	or	%o0,000000A8,%o0
	call	exit
000116FC                                     90 10 20 21             .. !

l00011700:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%o0,%o0

;; xfree: 00011708
;;   Called from:
;;     00011D40 (in free_def_dec)
;;     00011D48 (in free_def_dec)
xfree proc
	save	%sp,FFFFFF90,%sp
	orcc	%i0,00000000,%o0
	be	00011720
	sethi	00000000,%g0

l00011718:
	call	free
	sethi	00000000,%g0

l00011720:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; savestring: 00011728
;;   Called from:
;;     00011C54 (in add_symbol)
;;     00011F20 (in unexpand_if_needed)
;;     0001213C (in abspath)
savestring proc
	save	%sp,FFFFFF90,%sp
	or	%g0,%i0,%l0
	call	xmalloc
	add	%i1,00000001,%o0
	or	%g0,%o0,%i0
	call	strcpy
	or	%g0,%l0,%o1
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; savestring2: 0001174C
;;   Called from:
;;     00012E88 (in gen_aux_info_file)
savestring2 proc
	save	%sp,FFFFFF90,%sp
	add	%i1,%i3,%o0
	or	%g0,%i0,%l0
	call	xmalloc
	add	%o0,00000001,%o0
	or	%g0,%o0,%i0
	call	strcpy
	or	%g0,%l0,%o1
	add	%i0,%i1,%o0
	call	strcpy
	or	%g0,%i2,%o1
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; fancy_abort: 00011780
fancy_abort proc
	save	%sp,FFFFFF90,%sp
	sethi	000000A0,%o0
	ld	[%o0+336],%o1
	sethi	0000005C,%o0
	call	notice
	or	%o0,000000C8,%o0
	call	exit
0001179C                                     90 10 20 21             .. !

;; dupnstr: 000117A0
;;   Called from:
;;     000127A8 (in save_def_or_dec)
;;     000129E0 (in save_def_or_dec)
;;     00012A78 (in save_def_or_dec)
;;     00012CD8 (in munge_compile_params)
;;     00013DAC (in edit_formals_lists)
dupnstr proc
	save	%sp,FFFFFF90,%sp
	or	%g0,%i0,%l0
	call	xmalloc
	add	%i1,00000001,%o0
	or	%g0,%o0,%i0
	or	%g0,%l0,%o1
	call	strncpy
	or	%g0,%i1,%o2
	stb	%g0,[%i0+%i1]
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; substr: 000117CC
;;   Called from:
;;     000138FC (in other_variable_style_function)
substr proc
	ba	00011810
	ldsb	[%o0+%g0],%g2

l000117D4:
	ldsb	[%o3+%g0],%g3
	subcc	%g3,00000000,%g0
	be	00011820
	or	%g0,%o0,%o2

l000117E4:
	ldsb	[%o2+%g0],%g2
	subcc	%g2,%g3,%g0
	bne	00011808
	add	%o3,00000001,%o3

l000117F4:
	ldsb	[%o3+%g0],%g3
	subcc	%g3,00000000,%g0
	bne	000117E4
	add	%o2,00000001,%o2

l00011804:
	ba,a	00011820

l00011808:
	add	%o0,00000001,%o0
0001180C                                     C4 4A 00 00             .J..

l00011810:
	subcc	%g2,00000000,%g0
	bne	000117D4
	or	%g0,%o1,%o3

l0001181C:
	or	%g0,00000000,%o0

l00011820:
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0

;; safe_read: 00011828
;;   Called from:
;;     00013268 (in process_aux_info_file)
safe_read proc
	save	%sp,FFFFFF90,%sp
	or	%g0,%i0,%l2
	or	%g0,%i2,%l0
	ba	00011840
	sethi	000000AD,%l1

l0001183C:
	add	%i1,%i0,%i1

l00011840:
	subcc	%l0,00000000,%g0

l00011844:
	ble	0001187C
	or	%g0,%l2,%o0

l0001184C:
	or	%g0,%i1,%o1
	call	read
	or	%g0,%l0,%o2
	orcc	%o0,00000000,%i0
	bge	00011874
	ld	[%l1+864],%o0

l00011864:
	subcc	%o0,00000004,%g0
	be	00011844
	subcc	%l0,00000000,%g0

l00011870:
	ba,a	00011880

l00011874:
	bne,a	0001183C
	sub	%l0,%i0,%l0

l0001187C:
	sub	%i2,%l0,%i0

l00011880:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; safe_write: 00011888
safe_write proc
	save	%sp,FFFFFF90,%sp
	subcc	%i2,00000000,%g0
	ble	0001190C
	sethi	000000AD,%l1

l00011898:
	sethi	0000005C,%l3
	sethi	000000A0,%l0
	or	%g0,%i0,%o0

l000118A4:
	or	%g0,%i1,%o1
	call	write
	or	%g0,%i2,%o2
	subcc	%o0,00000000,%g0
	bge,a	000118FC
	sub	%i2,%o0,%i2

l000118BC:
	ld	[%l1+864],%l2
	subcc	%l2,00000004,%g0
	be	00011900
	or	%g0,%i3,%o1

l000118CC:
	ld	[%l0+336],%l1
	call	shortpath
	or	%g0,00000000,%o0
	or	%g0,%o0,%l0
	call	xstrerror
	or	%g0,%l2,%o0
	or	%g0,%o0,%o3
	or	%l3,000000E0,%o0
	or	%g0,%l1,%o1
	call	notice
	or	%g0,%l0,%o2
	ba,a	0001190C

l000118FC:
	add	%i1,%o0,%i1

l00011900:
	subcc	%i2,00000000,%g0
	bg	000118A4
	or	%g0,%i0,%o0

l0001190C:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; save_pointers: 00011914
;;   Called from:
;;     00013930 (in edit_fn_declaration)
;;     00013EF0 (in edit_fn_definition)
save_pointers proc
	sethi	000000A0,%g2
	ld	[%g2+480],%o2
	sethi	000000A0,%g3
	ld	[%g3+528],%o1
	sethi	000000AC,%o0
	sethi	000000AC,%g2
	st	%o2,[%o0+640]
	jmpl	%o7,+00000008,%g0
	st	%o1,[%g2+656]

;; restore_pointers: 00011938
;;   Called from:
;;     00013950 (in edit_fn_declaration)
;;     00013FB0 (in edit_fn_definition)
restore_pointers proc
	sethi	000000AC,%g2
	ld	[%g2+640],%o2
	sethi	000000AC,%g3
	ld	[%g3+656],%o1
	sethi	000000A0,%o0
	sethi	000000A0,%g2
	st	%o2,[%o0+480]
	jmpl	%o7,+00000008,%g0
	st	%o1,[%g2+528]

;; is_id_char: 0001195C
;;   Called from:
;;     00011DEC (in unexpand_if_needed)
;;     00012850 (in save_def_or_dec)
;;     00013D20 (in edit_formals_lists)
;;     00014700 (in scan_for_missed_items)
;;     00014734 (in scan_for_missed_items)
is_id_char proc
	sethi	000000AD,%g2
	or	%g2,00000011,%g2
	and	%o0,000000FF,%o0
	ldub	[%o0+%g2],%g3
	andcc	%g3,00000007,%g0
	bne	0001198C
	or	%g0,00000000,%g2

l00011978:
	subcc	%o0,0000005F,%g0
	be	0001198C
	subcc	%o0,00000024,%g0

l00011984:
	bne	00011990
	sethi	00000000,%g0

l0001198C:
	or	%g0,00000001,%g2

l00011990:
	jmpl	%o7,+00000008,%g0
	or	%g0,%g2,%o0

;; usage: 00011998
;;   Called from:
;;     0001506C (in main)
;;     000151AC (in main)
usage proc
	save	%sp,FFFFFF90,%sp
	sethi	000000A0,%o0
	ld	[%o0+336],%o1
	sethi	0000005C,%o0
	or	%o0,00000108,%o0
	call	notice
	or	%g0,%o1,%o2
	call	exit
000119B8                         90 10 20 21                     .. !    

;; in_system_include_dir: 000119BC
;;   Called from:
;;     000148DC (in edit_file)
in_system_include_dir proc
	save	%sp,FFFFFF90,%sp
	ldsb	[%i0+%g0],%o0
	subcc	%o0,0000002F,%g0
	be	000119E0
	sethi	0000009F,%o0

l000119D0:
	call	abort
000119D4             01 00 00 00                             ....        

l000119D8:
	ba	00011A44
	or	%g0,00000001,%i0

l000119E0:
	ld	[%o0+720],%o1
	subcc	%o1,00000000,%g0
	be	00011A40
	or	%o0,000002D0,%l1

l000119F0:
	ld	[%l1+%g0],%l0

l000119F4:
	call	strlen
	or	%g0,%l0,%o0
	or	%g0,%o0,%o2
	or	%g0,%l0,%o1
	call	strncmp
	or	%g0,%i0,%o0
	subcc	%o0,00000000,%g0
	bne,a	00011A30
	add	%l1,00000010,%l1

l00011A18:
	call	strlen
	ld	[%l1+%g0],%o0
	ldsb	[%i0+%o0],%o1
	subcc	%o1,0000002F,%g0
	be	000119D8
	add	%l1,00000010,%l1

l00011A30:
	ld	[%l1+%g0],%o0
	subcc	%o0,00000000,%g0
	bne,a	000119F4
	ld	[%l1+%g0],%l0

l00011A40:
	or	%g0,00000000,%i0

l00011A44:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; needs_to_be_converted: 00011A4C
;;   Called from:
;;     00014880 (in edit_file)
needs_to_be_converted proc
	ld	[%o0+4],%g3
	subcc	%g3,00000000,%g0
	be	00011A80
	or	%g0,00000000,%o0

l00011A5C:
	ldsb	[%g3+32],%g2

l00011A60:
	subcc	%g2,00000000,%g0
	bne	00011A80
	or	%g0,FFFFFFFF,%o0

l00011A6C:
	ld	[%g3+%g0],%g3
	subcc	%g3,00000000,%g0
	bne,a	00011A60
	ldsb	[%g3+32],%g2

l00011A7C:
	or	%g0,00000000,%o0

l00011A80:
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0

;; directory_specified_p: 00011A88
;;   Called from:
;;     000148A4 (in edit_file)
directory_specified_p proc
	save	%sp,FFFFFF90,%sp
	sethi	000000AD,%o0
	ld	[%o0+868],%l1
	subcc	%l1,00000000,%g0
	be,a	00011B38
	or	%g0,00000000,%i0

l00011AA0:
	ld	[%l1+%g0],%l0

l00011AA4:
	call	strlen
	or	%g0,%l0,%o0
	or	%g0,%o0,%o2
	or	%g0,%l0,%o1
	call	strncmp
	or	%g0,%i0,%o0
	subcc	%o0,00000000,%g0
	bne,a	00011B28
	ld	[%l1+4],%l1

l00011AC8:
	call	strlen
	ld	[%l1+%g0],%o0
	ldsb	[%i0+%o0],%o1
	subcc	%o1,0000002F,%g0
	bne,a	00011B28
	ld	[%l1+4],%l1

l00011AE0:
	call	strlen
	ld	[%l1+%g0],%o0
	add	%i0,%o0,%o0
	ldsb	[%o0+1],%o1
	subcc	%o1,00000000,%g0
	be	00011B1C
	add	%o0,00000001,%o2

l00011AFC:
	ldsb	[%o2+%g0],%o0
	subcc	%o0,0000002F,%g0

l00011B04:
	be	00011B24
	add	%o2,00000001,%o2

l00011B0C:
	ldsb	[%o2+%g0],%o0
	subcc	%o0,00000000,%g0
	bne	00011B04
	subcc	%o0,0000002F,%g0

l00011B1C:
	ba	00011B38
	or	%g0,00000001,%i0

l00011B24:
	ld	[%l1+4],%l1

l00011B28:
	subcc	%l1,00000000,%g0
	bne,a	00011AA4
	ld	[%l1+%g0],%l0

l00011B34:
	or	%g0,00000000,%i0

l00011B38:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; file_excluded_p: 00011B40
;;   Called from:
;;     000148B8 (in edit_file)
file_excluded_p proc
	save	%sp,FFFFFF90,%sp
	call	strlen
	or	%g0,%i0,%o0
	or	%g0,%o0,%l1
	sethi	000000AD,%o0
	ld	[%o0+8],%l0
	subcc	%l0,00000000,%g0
	be,a	00011BC0
	or	%g0,00000000,%i0

l00011B64:
	add	%i0,%l1,%l2

l00011B68:
	call	strlen
	ld	[%l0+%g0],%o0
	ld	[%l0+%g0],%o1
	call	strcmp
	sub	%l2,%o0,%o0
	subcc	%o0,00000000,%g0
	bne,a	00011BB0
	ld	[%l0+4],%l0

l00011B88:
	call	strlen
	ld	[%l0+%g0],%o0
	sub	%l1,%o0,%o0
	add	%o0,%i0,%o0
	ldsb	[%o0-1],%o1
	subcc	%o1,0000002F,%g0
	bne,a	00011BB0
	ld	[%l0+4],%l0

l00011BA8:
	ba	00011BC0
	or	%g0,00000001,%i0

l00011BB0:
	subcc	%l0,00000000,%g0
	bne	00011B68
	sethi	00000000,%g0

l00011BBC:
	or	%g0,00000000,%i0

l00011BC0:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; string_list_cons: 00011BC8
;;   Called from:
;;     00014F4C (in main)
string_list_cons proc
	save	%sp,FFFFFF90,%sp
	call	xmalloc
	or	%g0,00000008,%o0
	st	%i0,[%o0+%g0]
	st	%i1,[%o0+4]
	jmpl	%i7,+00000008,%g0
	restore	%g0,%o0,%o0

;; visit_each_hash_node: 00011BE4
;;   Called from:
;;     00014E84 (in do_processing)
;;     00014E94 (in do_processing)
visit_each_hash_node proc
	save	%sp,FFFFFF90,%sp
	sethi	00000006,%o0
	add	%i0,%o0,%o0
	subcc	%i0,%o0,%g0
	bcc	00011C34
	or	%g0,%o0,%l1

l00011BFC:
	ld	[%i0+4],%o0

l00011C00:
	subcc	%o0,00000000,%g0
	be	00011C24
	add	%i0,0000000C,%l0

l00011C0C:
	jmpl	%i1,%g0,%o7
	or	%g0,%i0,%o0
	ld	[%i0+%g0],%i0
	subcc	%i0,00000000,%g0
	bne	00011C0C
	sethi	00000000,%g0

l00011C24:
	or	%g0,%l0,%i0
	subcc	%i0,%l1,%g0
	bcs,a	00011C00
	ld	[%i0+4],%o0

l00011C34:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; add_symbol: 00011C3C
;;   Called from:
;;     00011D28 (in lookup)
add_symbol proc
	save	%sp,FFFFFF90,%sp
	or	%g0,%i0,%l0
	st	%g0,[%l0+%g0]
	call	strlen
	or	%g0,%i1,%o0
	or	%g0,%o0,%o1
	call	savestring
	or	%g0,%i1,%o0
	st	%o0,[%l0+4]
	st	%g0,[%l0+8]
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; lookup: 00011C6C
;;   Called from:
;;     00012314 (in find_file)
;;     00012894 (in save_def_or_dec)
lookup proc
	save	%sp,FFFFFF90,%sp
	ldsb	[%i1+%g0],%o0
	or	%g0,%i0,%l1
	subcc	%o0,00000000,%g0
	or	%g0,00000000,%o3
	be	00011CA0
	or	%g0,%i1,%o2

l00011C88:
	ldsb	[%o2+%g0],%o1
	add	%o2,00000001,%o2
	ldsb	[%o2+%g0],%o0
	subcc	%o0,00000000,%g0
	bne	00011C88
	add	%o3,%o1,%o3

l00011CA0:
	and	%o3,000001FF,%o3
	sll	%o3,00000001,%o0
	add	%o0,%o3,%o0
	sll	%o0,00000002,%l0
	add	%l1,%l0,%i0
	ld	[%i0+4],%o0
	subcc	%o0,00000000,%g0
	bne	00011CCC
	sethi	00000000,%g0

l00011CC4:
	ba	00011D28
	or	%g0,%i0,%o0

l00011CCC:
	call	strcmp
	or	%g0,%i1,%o1
	subcc	%o0,00000000,%g0
	be	00011D34
	sethi	00000000,%g0

l00011CE0:
	ld	[%l1+%l0],%o0
	subcc	%o0,00000000,%g0
	be	00011D1C
	sethi	00000000,%g0

l00011CF0:
	ld	[%i0+%g0],%i0

l00011CF4:
	or	%g0,%i1,%o1
	call	strcmp
	ld	[%i0+4],%o0
	subcc	%o0,00000000,%g0
	be	00011D34
	sethi	00000000,%g0

l00011D0C:
	ld	[%i0+%g0],%o0
	subcc	%o0,00000000,%g0
	bne,a	00011CF4
	ld	[%i0+%g0],%i0

l00011D1C:
	call	xmalloc
	or	%g0,0000000C,%o0
	st	%o0,[%i0+%g0]

l00011D28:
	call	add_symbol
	or	%g0,%i1,%o1
	or	%g0,%o0,%i0

l00011D34:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; free_def_dec: 00011D3C
;;   Called from:
;;     00012AC0 (in save_def_or_dec)
free_def_dec proc
	save	%sp,FFFFFF90,%sp
	call	xfree
	ld	[%i0+12],%o0
	call	xfree
	or	%g0,%i0,%o0
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; unexpand_if_needed: 00011D58
unexpand_if_needed proc
	save	%sp,FFFFFF90,%sp
	sethi	0000009F,%o0
	ld	[%o0+848],%l1
	or	%g0,%o0,%l5
	subcc	%l1,00000000,%g0
	or	%g0,00000000,%i1
	bne	00011D88
	sethi	0000009F,%l6

l00011D78:
	or	%g0,00000400,%o0
	call	xmalloc
	st	%o0,[%l6+852]
	st	%o0,[%l5+848]

l00011D88:
	ldsb	[%i0+%g0],%o0
	subcc	%o0,0000000A,%g0
	be	00011EC8
	ld	[%l5+848],%l1

l00011D98:
	sethi	0000005B,%l7
	ld	[%l7+788],%o0

l00011DA0:
	subcc	%o0,00000000,%g0
	be	00011E74
	or	%l7,00000314,%l4

l00011DAC:
	ld	[%l4+%g0],%l0

l00011DB0:
	call	strlen
	or	%g0,%l0,%o0
	or	%g0,%o0,%l3
	ldsb	[%i0+%g0],%o1
	ldsb	[%l0+%g0],%o0
	subcc	%o1,%o0,%g0
	bne,a	00011E64
	add	%l4,00000008,%l4

l00011DD0:
	or	%g0,%l0,%o1
	or	%g0,%i0,%o0
	call	strncmp
	or	%g0,%l3,%o2
	subcc	%o0,00000000,%g0
	bne,a	00011E64
	add	%l4,00000008,%l4

l00011DEC:
	call	is_id_char
	ldsb	[%i0+%l3],%o0
	subcc	%o0,00000000,%g0
	bne,a	00011E64
	add	%l4,00000008,%l4

l00011E00:
	ld	[%l4+4],%o0
	call	strlen
	or	%g0,00000001,%i1
	or	%g0,%o0,%l2
	ld	[%l5+848],%o1
	add	%l1,%l2,%o0
	ld	[%l6+852],%o2
	sub	%o0,%o1,%o0
	subcc	%o0,%o2,%g0
	bl	00011E4C
	sll	%o2,00000001,%o2

l00011E2C:
	add	%o2,%l2,%o2
	sub	%l1,%o1,%l0
	or	%g0,%o1,%o0
	or	%g0,%o2,%o1
	call	xrealloc
	st	%o2,[%l6+852]
	add	%o0,%l0,%l1
	st	%o0,[%l5+848]

l00011E4C:
	ld	[%l4+4],%o1
	call	strcpy
	or	%g0,%l1,%o0
	add	%l1,%l2,%l1
	ba	00011EB8
	add	%i0,%l3,%i0

l00011E64:
	ld	[%l4+%g0],%o0
	subcc	%o0,00000000,%g0
	bne,a	00011DB0
	ld	[%l4+%g0],%l0

l00011E74:
	ld	[%l5+848],%o1
	ld	[%l6+852],%o0
	sub	%l1,%o1,%l0
	subcc	%l0,%o0,%g0
	bne,a	00011EAC
	ldub	[%i0+%g0],%o0

l00011E8C:
	sll	%l0,00000001,%o2
	or	%g0,%o1,%o0
	or	%g0,%o2,%o1
	call	xrealloc
	st	%o2,[%l6+852]
	add	%o0,%l0,%l1
	st	%o0,[%l5+848]
	ldub	[%i0+%g0],%o0

l00011EAC:
	stb	%o0,[%l1+%g0]
	add	%i0,00000001,%i0
	add	%l1,00000001,%l1

l00011EB8:
	ldsb	[%i0+%g0],%o0
	subcc	%o0,0000000A,%g0
	bne	00011DA0
	ld	[%l7+788],%o0

l00011EC8:
	ld	[%l5+848],%o2
	ld	[%l6+852],%o1
	add	%o2,FFFFFFFE,%o0
	sub	%l1,%o0,%o0
	subcc	%o0,%o1,%g0
	bl	00011EFC
	sll	%o1,00000001,%o1

l00011EE4:
	sub	%l1,%o2,%l0
	st	%o1,[%l6+852]
	call	xrealloc
	or	%g0,%o2,%o0
	add	%o0,%l0,%l1
	st	%o0,[%l5+848]

l00011EFC:
	or	%g0,0000000A,%o0
	stb	%o0,[%l1+%g0]
	add	%l1,00000001,%l1
	subcc	%i1,00000000,%g0
	bne	00011F1C
	stb	%g0,[%l1+%g0]

l00011F14:
	ba	00011F2C
	or	%g0,00000000,%i0

l00011F1C:
	ld	[%l5+848],%o0
	call	savestring
	sub	%l1,%o0,%o1
	or	%g0,%o0,%i0

l00011F2C:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; abspath: 00011F34
;;   Called from:
;;     0001216C (in shortpath)
;;     000124F0 (in referenced_file_is_newer)
;;     000125C4 (in save_def_or_dec)
;;     000150FC (in main)
abspath proc
	save	%sp,FFFFFF90,%sp
	subcc	%i0,00000000,%g0
	bne	00011F4C
	sethi	00000000,%g0

l00011F44:
	sethi	000000AC,%o0
	ld	[%o0+624],%i0

l00011F4C:
	call	strlen
	or	%g0,%i0,%o0
	or	%g0,%o0,%l0
	call	strlen
	or	%g0,%i1,%o0
	add	%l0,%o0,%l0
	add	%l0,00000009,%l0
	and	%l0,FFFFFFF8,%l0
	sub	%sp,%l0,%sp
	ldsb	[%i1+%g0],%o0
	add	%sp,00000060,%o4
	add	%i1,00000001,%o3
	subcc	%o0,0000002F,%g0
	be	00011FC8
	or	%g0,%o4,%o2

l00011F88:
	ldub	[%i0+%g0],%o1
	add	%o4,00000001,%o2
	sll	%o1,00000018,%o0
	subcc	%o0,00000000,%g0
	stb	%o1,[%o4+%g0]
	be	00011FC0
	add	%i0,00000001,%i0

l00011FA4:
	ldub	[%i0+%g0],%o0
	sll	%o0,00000018,%o1
	stb	%o0,[%o2+%g0]
	subcc	%o1,00000000,%g0
	add	%i0,00000001,%i0
	bne	00011FA4
	add	%o2,00000001,%o2

l00011FC0:
	or	%g0,0000002F,%o0
	stb	%o0,[%o2-1]

l00011FC8:
	ldub	[%i1+%g0],%o0
	or	%g0,%o3,%i0
	sll	%o0,00000018,%o1
	stb	%o0,[%o2+%g0]
	subcc	%o1,00000000,%g0
	add	%o2,00000001,%o2
	be	00012004
	add	%o4,00000001,%o3

l00011FE8:
	ldub	[%i0+%g0],%o0
	sll	%o0,00000018,%o1
	stb	%o0,[%o2+%g0]
	subcc	%o1,00000000,%g0
	add	%i0,00000001,%i0
	bne	00011FE8
	add	%o2,00000001,%o2

l00012004:
	ldsb	[%o4+1],%o0
	or	%g0,%o3,%o2
	subcc	%o0,00000000,%g0
	be	00012120
	or	%g0,%o2,%o1

l00012018:
	sethi	0000005C,%o5
	ldsb	[%o1+%g0],%o0
	subcc	%o0,0000002F,%g0

l00012024:
	bne	00012044
	ldub	[%o1+%g0],%o3

l0001202C:
	ldsb	[%o2-1],%o0
	subcc	%o0,0000002F,%g0
	bne	00012048
	subcc	%o3,0000002E,%g0

l0001203C:
	ba	00012110
	add	%o1,00000001,%o1

l00012044:
	subcc	%o3,0000002E,%g0

l00012048:
	bne,a	00012104
	ldub	[%o1+%g0],%o0

l00012050:
	ldsb	[%o2-1],%o0
	subcc	%o0,0000002F,%g0
	bne,a	00012104
	ldub	[%o1+%g0],%o0

l00012060:
	ldsb	[%o1+1],%o0
	subcc	%o0,00000000,%g0
	be	00012120
	subcc	%o0,0000002F,%g0

l00012070:
	bne	00012080
	subcc	%o0,0000002E,%g0

l00012078:
	ba	00012110
	add	%o1,00000002,%o1

l00012080:
	bne,a	00012104
	ldub	[%o1+%g0],%o0

l00012088:
	ldsb	[%o1+2],%o0
	subcc	%o0,00000000,%g0
	be	000120A0
	subcc	%o0,0000002F,%g0

l00012098:
	bne,a	00012104
	ldub	[%o1+%g0],%o0

l000120A0:
	subcc	%o0,0000002F,%g0
	bne	000120B0
	add	%o1,00000002,%o0

l000120AC:
	add	%o1,00000003,%o0

l000120B0:
	add	%o2,FFFFFFFE,%o2
	or	%g0,%o0,%o1

l000120B8:
	subcc	%o2,%o4,%g0
	bcs	000120D8
	sethi	00000000,%g0

l000120C4:
	ldsb	[%o2+%g0],%o0
	subcc	%o0,0000002F,%g0
	bne,a	000120B8
	add	%o2,FFFFFFFF,%o2

l000120D4:
	subcc	%o2,%o4,%g0

l000120D8:
	bcc	000120FC
	add	%o2,00000001,%o2

l000120E0:
	sethi	000000A0,%o0
	ld	[%o0+336],%o1
	or	%g0,%i1,%o2
	call	notice
	or	%o5,00000148,%o0
	call	exit
000120F8                         90 10 20 21                     .. !    

l000120FC:
	ba	00012110
	stb	%g0,[%o2+%g0]

l00012104:
	stb	%o0,[%o2+%g0]
	add	%o1,00000001,%o1
	add	%o2,00000001,%o2

l00012110:
	ldsb	[%o1+%g0],%o0
	subcc	%o0,00000000,%g0
	bne	00012024
	subcc	%o0,0000002F,%g0

l00012120:
	ldsb	[%o2-1],%o0
	subcc	%o0,0000002F,%g0
	bne	00012138
	stb	%g0,[%o2+%g0]

l00012130:
	add	%o2,FFFFFFFF,%o2
	stb	%g0,[%o2+%g0]

l00012138:
	or	%g0,%o4,%o0
	call	savestring
	sub	%o2,%o0,%o1
	jmpl	%i7,+00000008,%g0
	restore	%g0,%o0,%o0

;; shortpath: 0001214C
;;   Called from:
;;     000118D0 (in safe_write)
;;     00012384 (in find_file)
;;     00012E54 (in gen_aux_info_file)
;;     000130D8 (in process_aux_info_file)
;;     000131D0 (in process_aux_info_file)
;;     000132C0 (in process_aux_info_file)
;;     00013318 (in process_aux_info_file)
;;     0001367C (in declare_source_confusing)
;;     000136AC (in declare_source_confusing)
;;     00013D90 (in edit_formals_lists)
;;     00013F5C (in edit_fn_definition)
;;     00014814 (in scan_for_missed_items)
;;     00014900 (in edit_file)
;;     00015158 (in main)
shortpath proc
	save	%sp,FFFFFF90,%sp
	sethi	000000AC,%o0
	ld	[%o0+624],%l0
	or	%g0,00000000,%l2
	call	strlen
	or	%g0,%i1,%o0
	or	%g0,%o0,%l3
	or	%g0,%i1,%o1
	call	abspath
	or	%g0,%i0,%o0
	or	%g0,%o0,%i0
	call	xmalloc
	or	%g0,%l3,%o0
	ldsb	[%l0+%g0],%o1
	or	%g0,%o0,%l4
	ba	00012198
	or	%g0,%l4,%l1

l00012190:
	ldsb	[%l0+%g0],%o1
	add	%i0,00000001,%i0

l00012198:
	subcc	%o1,00000000,%g0
	be,a	000121B8
	ldsb	[%l0+%g0],%o0

l000121A4:
	ldsb	[%i0+%g0],%o0
	subcc	%o1,%o0,%g0
	be,a	00012190
	add	%l0,00000001,%l0

l000121B4:
	ldsb	[%l0+%g0],%o0

l000121B8:
	subcc	%o0,00000000,%g0
	bne	000121F8
	ldub	[%i0+%g0],%o1

l000121C4:
	sll	%o1,00000018,%o0
	sra	%o0,00000018,%o0
	subcc	%o0,00000000,%g0
	be	000121E0
	subcc	%o0,0000002F,%g0

l000121D8:
	bne	000121FC
	subcc	%o1,00000000,%g0

l000121E0:
	subcc	%o0,00000000,%g0
	bne,a	000122FC
	add	%i0,00000001,%i0

l000121EC:
	sethi	0000005C,%i0
	ba	000122FC
	or	%i0,00000168,%i0

l000121F8:
	subcc	%o1,00000000,%g0

l000121FC:
	be,a	0001223C
	ldsb	[%l0+%g0],%o0

l00012204:
	add	%l0,FFFFFFFF,%l0
	ldsb	[%l0+%g0],%o0
	subcc	%o0,0000002F,%g0
	bne	00012204
	add	%i0,FFFFFFFF,%i0

l00012218:
	add	%l0,00000001,%l0
	add	%i0,00000001,%i0
	ba	00012238
	or	%g0,00000001,%l2

l00012228:
	subcc	%o1,0000002F,%g0
	bne	00012238
	add	%l0,00000001,%l0

l00012234:
	add	%l2,00000001,%l2

l00012238:
	ldsb	[%l0+%g0],%o0

l0001223C:
	subcc	%o0,00000000,%g0
	bne	00012228
	ldub	[%l0+%g0],%o1

l00012248:
	sll	%l2,00000001,%l0
	add	%l0,%l2,%l0
	call	strlen
	or	%g0,%i0,%o0
	add	%l0,%o0,%l0
	subcc	%l0,%l3,%g0
	bcs	00012270
	add	%l2,FFFFFFFF,%l2

l00012268:
	ba	000122FC
	or	%g0,%i1,%i0

l00012270:
	subcc	%l2,FFFFFFFF,%g0
	be	000122B8
	add	%l4,%l3,%o3

l0001227C:
	or	%g0,%o3,%o2
	or	%g0,0000002E,%o1
	or	%g0,0000002F,%o4

l00012288:
	add	%l1,00000003,%o0
	subcc	%o2,%o0,%g0
	bleu	00012268
	add	%l2,FFFFFFFF,%l2

l00012298:
	stb	%o1,[%l1+%g0]
	add	%l1,00000001,%l1
	stb	%o1,[%l1+%g0]
	add	%l1,00000001,%l1
	stb	%o4,[%l1+%g0]
	subcc	%l2,FFFFFFFF,%g0
	bne	00012288
	add	%l1,00000001,%l1

l000122B8:
	or	%g0,%o3,%o2

l000122BC:
	subcc	%o2,%l1,%g0
	bleu,a	000122FC
	or	%g0,%i1,%i0

l000122C8:
	ldub	[%i0+%g0],%o0
	sll	%o0,00000018,%o1
	stb	%o0,[%l1+%g0]
	subcc	%o1,00000000,%g0
	add	%i0,00000001,%i0
	bne	000122BC
	add	%l1,00000001,%l1

l000122E4:
	add	%l1,FFFFFFFF,%l1
	ldsb	[%l1-1],%o0
	subcc	%o0,0000002F,%g0
	be,a	000122F8
	stb	%g0,[%l1-1]

l000122F8:
	or	%g0,%l4,%i0

l000122FC:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; find_file: 00012304
;;   Called from:
;;     000124F8 (in referenced_file_is_newer)
;;     000125CC (in save_def_or_dec)
find_file proc
	save	%sp,FFFFFF08,%sp
	or	%g0,%i0,%l1
	sethi	000000A0,%o0
	or	%o0,00000240,%o0
	call	lookup
	or	%g0,%l1,%o1
	or	%g0,%o0,%l5
	ld	[%l5+8],%i0
	subcc	%i0,00000000,%g0
	bne	000123C8
	sethi	00000000,%g0

l00012330:
	call	xmalloc
	or	%g0,0000000C,%o0
	subcc	%i1,00000000,%g0
	be	0001234C
	or	%g0,%o0,%l4

l00012344:
	ba	000123B0
	st	%g0,[%i6-88]

l0001234C:
	or	%g0,%l1,%o0
	call	stat
	add	%i6,FFFFFF68,%o1
	or	%g0,%o0,%i0
	subcc	%i0,FFFFFFFF,%g0
	bne,a	000123B4
	st	%l4,[%l5+8]

l00012368:
	sethi	000000AD,%o0
	sethi	000000A0,%o1
	ld	[%o0+864],%l2
	sethi	0000005C,%l0
	ld	[%o1+336],%l3
	or	%l0,00000170,%l0
	or	%g0,%l1,%o1
	call	shortpath
	or	%g0,00000000,%o0
	or	%g0,%o0,%l1
	call	xstrerror
	or	%g0,%l2,%o0
	or	%g0,%o0,%o3
	or	%g0,%l0,%o0
	or	%g0,%l3,%o1
	call	notice
	or	%g0,%l1,%o2
	st	%i0,[%i6-88]

l000123B0:
	st	%l4,[%l5+8]

l000123B4:
	st	%l5,[%l4+%g0]
	st	%g0,[%l4+4]
	ld	[%i6-88],%o0
	or	%g0,%l4,%i0
	st	%o0,[%l4+8]

l000123C8:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; aux_info_corrupted: 000123D0
;;   Called from:
;;     00012408 (in check_aux_info)
aux_info_corrupted proc
	save	%sp,FFFFFF90,%sp
	sethi	000000A0,%o0
	ld	[%o0+336],%o1
	sethi	000000A0,%o3
	ld	[%o3+368],%o2
	sethi	0000005C,%o0
	call	notice
	or	%o0,00000190,%o0
	call	exit
000123F4             90 10 20 21                             .. !        

;; check_aux_info: 000123F8
;;   Called from:
;;     0001246C (in referenced_file_is_newer)
;;     00012480 (in referenced_file_is_newer)
;;     00012494 (in referenced_file_is_newer)
;;     00012538 (in save_def_or_dec)
;;     0001254C (in save_def_or_dec)
;;     00012560 (in save_def_or_dec)
;;     00012648 (in save_def_or_dec)
;;     00012688 (in save_def_or_dec)
;;     000126D0 (in save_def_or_dec)
;;     000126F0 (in save_def_or_dec)
;;     00012710 (in save_def_or_dec)
;;     00012730 (in save_def_or_dec)
;;     00012780 (in save_def_or_dec)
;;     00012808 (in save_def_or_dec)
;;     00012848 (in save_def_or_dec)
;;     00012948 (in save_def_or_dec)
;;     00012960 (in save_def_or_dec)
;;     00012978 (in save_def_or_dec)
;;     00012990 (in save_def_or_dec)
;;     000129A8 (in save_def_or_dec)
;;     000129FC (in save_def_or_dec)
;;     00012A24 (in save_def_or_dec)
;;     00012A6C (in save_def_or_dec)
check_aux_info proc
	save	%sp,FFFFFF90,%sp
	subcc	%i0,00000000,%g0
	bne	00012410
	sethi	00000000,%g0

l00012408:
	call	aux_info_corrupted
	sethi	00000000,%g0

l00012410:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; find_corresponding_lparen: 00012418
;;   Called from:
;;     00012810 (in save_def_or_dec)
find_corresponding_lparen proc
	add	%o0,FFFFFFFF,%o0
	or	%g0,00000001,%g3
	ldsb	[%o0+%g0],%g2

l00012424:
	subcc	%g2,00000028,%g0
	be	00012440
	subcc	%g2,00000029,%g0

l00012430:
	be,a	00012444
	add	%g3,00000001,%g3

l00012438:
	ba	00012448
	add	%o0,FFFFFFFF,%o0

l00012440:
	add	%g3,FFFFFFFF,%g3

l00012444:
	add	%o0,FFFFFFFF,%o0

l00012448:
	subcc	%g3,00000000,%g0
	bne,a	00012424
	ldsb	[%o0+%g0],%g2

l00012454:
	jmpl	%o7,+00000008,%g0
	add	%o0,00000001,%o0

;; referenced_file_is_newer: 0001245C
referenced_file_is_newer proc
	save	%sp,FFFFFF90,%sp
	ldsb	[%i0+%g0],%o0
	xor	%o0,0000002F,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	ldsb	[%i0+1],%o0
	xor	%o0,0000002A,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	ldsb	[%i0+2],%o0
	xor	%o0,00000020,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	ldsb	[%i0+3],%o0
	add	%i0,00000003,%o1
	subcc	%o0,0000003A,%g0
	be	000124C4
	or	%g0,%o1,%l1

l000124B0:
	add	%l1,00000001,%l1

l000124B4:
	ldsb	[%l1+%g0],%o0
	subcc	%o0,0000003A,%g0
	bne,a	000124B4
	add	%l1,00000001,%l1

l000124C4:
	sub	%l1,%o1,%l1
	add	%l1,00000008,%o0
	and	%o0,FFFFFFF8,%o0
	sub	%sp,%o0,%sp
	add	%sp,00000060,%l0
	or	%g0,%l1,%o2
	call	strncpy
	or	%g0,%l0,%o0
	sethi	000000A0,%o1
	ld	[%o1+400],%o0
	stb	%g0,[%l0+%l1]
	call	abspath
	or	%g0,%l0,%o1
	call	find_file
	or	%g0,00000000,%o1
	ld	[%o0+8],%o1
	subcc	%o1,%i1,%g0
	bg	00012514
	or	%g0,00000001,%i0

l00012510:
	or	%g0,00000000,%i0

l00012514:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; save_def_or_dec: 0001251C
save_def_or_dec proc
	save	%sp,FFFFFF80,%sp
	call	xmalloc
	or	%g0,0000002C,%o0
	or	%g0,%o0,%l2
	ldsb	[%i0+%g0],%o0
	xor	%o0,0000002F,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	ldsb	[%i0+1],%o0
	xor	%o0,0000002A,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	ldsb	[%i0+2],%o0
	xor	%o0,00000020,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	ldsb	[%i0+3],%o0
	add	%i6,FFFFFFE0,%l3
	add	%i0,00000003,%i0
	subcc	%o0,0000003A,%g0
	be	00012594
	or	%g0,%i0,%o1

l00012580:
	add	%i0,00000001,%i0

l00012584:
	ldsb	[%i0+%g0],%o0
	subcc	%o0,0000003A,%g0
	bne,a	00012584
	add	%i0,00000001,%i0

l00012594:
	sub	%i0,%o1,%l1
	add	%l1,00000008,%o0
	and	%o0,FFFFFFF8,%o0
	sub	%sp,%o0,%sp
	add	%sp,00000060,%l0
	or	%g0,%l1,%o2
	call	strncpy
	or	%g0,%l0,%o0
	sethi	000000A0,%o1
	ld	[%o1+400],%o0
	add	%i0,00000001,%i0
	or	%g0,%l0,%o1
	call	abspath
	stb	%g0,[%l0+%l1]
	call	find_file
	or	%g0,%i1,%o1
	st	%o0,[%l2+4]
	ldsb	[%i0+%g0],%o0
	subcc	%o0,0000003A,%g0
	be	000125FC
	or	%g0,%i0,%o1

l000125E8:
	add	%i0,00000001,%i0

l000125EC:
	ldsb	[%i0+%g0],%o0
	subcc	%o0,0000003A,%g0
	bne,a	000125EC
	add	%i0,00000001,%i0

l000125FC:
	sub	%i0,%o1,%l0
	or	%g0,%l0,%o2
	call	strncpy
	or	%g0,%l3,%o0
	stb	%g0,[%l3+%l0]
	call	atoi
	or	%g0,%l3,%o0
	st	%o0,[%l2+8]
	add	%i0,00000001,%i0
	ldub	[%i0+%g0],%o1
	add	%o1,FFFFFFB2,%o0
	and	%o0,000000FF,%o0
	subcc	%o0,00000001,%g0
	bleu	00012644
	or	%g0,00000000,%o2

l00012638:
	subcc	%o1,00000049,%g0
	bne	00012648
	sethi	00000000,%g0

l00012644:
	or	%g0,00000001,%o2

l00012648:
	call	check_aux_info
	or	%g0,%o2,%o0
	ldsb	[%i0+%g0],%o0
	xor	%o0,0000004E,%o0
	subcc	%g0,%o0,%g0
	subx	%g0,FFFFFFFF,%o1
	stb	%o1,[%l2+32]
	add	%i0,00000001,%i0
	ldsb	[%i0+%g0],%o0
	subcc	%o0,00000043,%g0
	be	00012684
	or	%g0,00000000,%o2

l00012678:
	subcc	%o0,00000046,%g0
	bne	00012688
	sethi	00000000,%g0

l00012684:
	or	%g0,00000001,%o2

l00012688:
	call	check_aux_info
	or	%g0,%o2,%o0
	ldsb	[%i0+%g0],%o0
	or	%g0,00000000,%o1
	subcc	%o0,00000046,%g0
	be	000126B0
	add	%i0,00000001,%i0

l000126A4:
	subcc	%i1,00000000,%g0
	be,a	000126B8
	st	%o1,[%l2+20]

l000126B0:
	or	%g0,00000001,%o1
	st	%o1,[%l2+20]

l000126B8:
	ldub	[%i0+%g0],%o0
	or	%g0,00000000,%l0
	sll	%o0,00000018,%o0
	sra	%o0,00000018,%o0
	xor	%o0,00000020,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	add	%i0,00000001,%i0
	ldub	[%i0+%g0],%o0
	sll	%o0,00000018,%o0
	sra	%o0,00000018,%o0
	xor	%o0,0000002A,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	add	%i0,00000001,%i0
	ldub	[%i0+%g0],%o0
	sll	%o0,00000018,%o0
	sra	%o0,00000018,%o0
	xor	%o0,0000002F,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	add	%i0,00000001,%i0
	ldub	[%i0+%g0],%o0
	sll	%o0,00000018,%o0
	sra	%o0,00000018,%o0
	xor	%o0,00000020,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	add	%i0,00000001,%i0
	sethi	0000005C,%o1
	or	%o1,000001C8,%o1
	or	%g0,%i0,%o0
	call	strncmp
	or	%g0,00000006,%o2
	subcc	%o0,00000000,%g0
	be	00012778
	sethi	0000005C,%o1

l0001275C:
	or	%o1,000001D0,%o1
	or	%g0,%i0,%o0
	call	strncmp
	or	%g0,00000006,%o2
	subcc	%o0,00000000,%g0
	bne	00012780
	or	%g0,%l0,%o0

l00012778:
	or	%g0,00000001,%l0
	or	%g0,%l0,%o0

l00012780:
	call	check_aux_info
	or	%g0,%i0,%l0
	add	%i0,00000007,%i0

l0001278C:
	ldsb	[%i0+%g0],%o0
	subcc	%o0,0000003B,%g0
	bne,a	0001278C
	add	%i0,00000001,%i0

l0001279C:
	add	%l0,FFFFFFFF,%o1
	sub	%i0,%o1,%o1
	or	%g0,%i0,%i1
	call	dupnstr
	or	%g0,%l0,%o0
	add	%i0,FFFFFFFF,%i0
	st	%o0,[%l2+12]

l000127B8:
	subcc	%i0,%l0,%g0
	be,a	000127E4
	ldsb	[%i0+%g0],%o0

l000127C4:
	ldsb	[%i0-1],%o0
	subcc	%o0,00000020,%g0
	be,a	000127B8
	add	%i0,FFFFFFFF,%i0

l000127D4:
	subcc	%o0,00000009,%g0
	be,a	000127B8
	add	%i0,FFFFFFFF,%i0

l000127E0:
	ldsb	[%i0+%g0],%o0

l000127E4:
	subcc	%o0,00000029,%g0
	bne	00012AC0
	sethi	000000A6,%l3

l000127F0:
	ba	00012810
	st	%g0,[%l2+28]

l000127F8:
	ldsb	[%l1-3],%o0
	add	%l1,FFFFFFFD,%i0
	xor	%o0,00000029,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0

l00012810:
	call	find_corresponding_lparen
	or	%g0,%i0,%o0
	or	%g0,%o0,%l1
	ld	[%l2+28],%o0
	add	%o0,00000001,%o0
	st	%o0,[%l2+28]
	ldsb	[%l1-2],%o1
	subcc	%o1,00000029,%g0
	be	000127F8
	add	%l1,FFFFFFFE,%i0

l00012838:
	ldsb	[%i0+1],%o0
	add	%l1,FFFFFFFF,%l1
	xor	%o0,00000020,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0

l00012850:
	call	is_id_char
	ldsb	[%i0+%g0],%o0
	subcc	%o0,00000000,%g0
	bne,a	00012850
	add	%i0,FFFFFFFF,%i0

l00012864:
	add	%i0,00000001,%i0
	sub	%l1,%i0,%l1
	add	%l1,00000008,%o0
	and	%o0,FFFFFFF8,%o0
	sub	%sp,%o0,%sp
	add	%sp,00000060,%l0
	or	%g0,%l1,%o2
	or	%g0,%i0,%o1
	call	strncpy
	or	%g0,%l0,%o0
	stb	%g0,[%l0+%l1]
	or	%l3,00000240,%o0
	call	lookup
	or	%g0,%l0,%o1
	st	%o0,[%l2+16]
	ld	[%o0+8],%o2
	subcc	%o2,00000000,%g0
	be,a	00012928
	ld	[%l2+20],%o0

l000128B0:
	sethi	0000005C,%l0
	ld	[%l2+8],%o1

l000128B8:
	ld	[%o2+8],%o0
	subcc	%o1,%o0,%g0
	bne,a	00012918
	ld	[%o2+24],%o2

l000128C8:
	ld	[%l2+4],%o1
	ld	[%o2+4],%o0
	subcc	%o1,%o0,%g0
	bne,a	00012918
	ld	[%o2+24],%o2

l000128DC:
	ld	[%o2+12],%o1
	call	strcmp
	ld	[%l2+12],%o0
	subcc	%o0,00000000,%g0
	be	00012AC0
	or	%l0,000001D8,%o0

l000128F4:
	ld	[%l2+4],%o1
	ld	[%l2+16],%o2
	ld	[%o1+%g0],%o4
	ld	[%o2+4],%o3
	ld	[%o4+4],%o1
	call	notice
	ld	[%l2+8],%o2
	call	exit
00012914             90 10 20 21                             .. !        

l00012918:
	subcc	%o2,00000000,%g0
	bne,a	000128B8
	ld	[%l2+8],%o1

l00012924:
	ld	[%l2+20],%o0

l00012928:
	st	%g0,[%l2+36]
	subcc	%o0,00000000,%g0
	be	00012AB0
	st	%g0,[%l2+40]

l00012938:
	add	%i1,00000001,%i0
	ldsb	[%i0+%g0],%o0
	xor	%o0,00000020,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	add	%i0,00000001,%i0
	ldsb	[%i0+%g0],%o0
	xor	%o0,0000002F,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	add	%i0,00000001,%i0
	ldsb	[%i0+%g0],%o0
	xor	%o0,0000002A,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	add	%i0,00000001,%i0
	ldsb	[%i0+%g0],%o0
	xor	%o0,00000020,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	add	%i0,00000001,%i0
	ldsb	[%i0+%g0],%o0
	xor	%o0,00000028,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	add	%i0,00000001,%i0
	ldsb	[%i0+%g0],%o0
	or	%g0,%i0,%o1
	subcc	%o0,00000029,%g0
	be	000129D8
	add	%i0,00000001,%i0

l000129C8:
	ldsb	[%i0+%g0],%o0
	subcc	%o0,00000029,%g0
	bne	000129C8
	add	%i0,00000001,%i0

l000129D8:
	or	%g0,%o1,%o0
	add	%i0,FFFFFFFF,%i0
	call	dupnstr
	sub	%i0,%o0,%o1
	st	%o0,[%l2+36]
	add	%i0,00000001,%i0
	ldsb	[%i0+%g0],%o0
	xor	%o0,00000020,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	add	%i0,00000001,%i0
	ldsb	[%i0+%g0],%o0
	subcc	%o0,0000002A,%g0
	bne	00012A38
	or	%g0,%i0,%l0

l00012A18:
	ldsb	[%i0+1],%o0
	xor	%o0,0000002F,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	sethi	0000005C,%o0
	ba	00012A80
	or	%o0,00000218,%o0

l00012A38:
	add	%i0,00000001,%i0

l00012A3C:
	ldsb	[%i0+%g0],%o0
	subcc	%o0,0000002A,%g0
	bne,a	00012A3C
	add	%i0,00000001,%i0

l00012A4C:
	ldsb	[%i0+1],%o0
	subcc	%o0,0000002F,%g0
	bne,a	00012A3C
	add	%i0,00000001,%i0

l00012A5C:
	add	%i0,FFFFFFFF,%i0
	ldsb	[%i0+%g0],%o0
	xor	%o0,00000020,%o0
	subcc	%g0,%o0,%g0
	call	check_aux_info
	subx	%g0,FFFFFFFF,%o0
	or	%g0,%l0,%o0
	call	dupnstr
	sub	%i0,%o0,%o1

l00012A80:
	st	%o0,[%l2+40]
	ldsb	[%l2+32],%o0
	subcc	%o0,00000000,%g0
	bne,a	00012ACC
	ld	[%l2+16],%o1

l00012A94:
	ld	[%l2+36],%o0
	ldsb	[%o0+%g0],%o1
	subcc	%o1,00000000,%g0
	bne,a	00012AB4
	ldsb	[%l2+32],%o0

l00012AA8:
	or	%g0,00000001,%o0
	stb	%o0,[%l2+32]

l00012AB0:
	ldsb	[%l2+32],%o0

l00012AB4:
	subcc	%o0,00000000,%g0
	bne,a	00012ACC
	ld	[%l2+16],%o1

l00012AC0:
	call	free_def_dec
	or	%g0,%l2,%o0
	ba,a	00012B5C

l00012ACC:
	ld	[%o1+8],%o0
00012AD0 D0 24 A0 18 E4 22 60 08 D2 04 A0 04 D0 02 60 04 .$..."`.......`.
00012AE0 80 A2 20 00 32 80 00 05 92 10 00 08 E4 22 60 04 .. .2........"`.
00012AF0 10 80 00 1B C0 24 80 00 D4 02 40 00 98 10 20 00 .....$....@... .
00012B00 10 80 00 04 D6 04 A0 08 92 10 00 0A D4 02 80 00 ................
00012B10 80 A2 A0 00 02 80 00 06 D0 02 60 08 80 A2 C0 08 ..........`.....
00012B20 26 BF FF FA 98 10 00 09 D0 02 60 08 80 A2 C0 08 &.........`.....
00012B30 06 80 00 09 80 A3 20 00 02 80 00 04 D2 24 80 00 ...... ......$..
00012B40 10 80 00 07 E4 23 00 00 D0 04 A0 04 10 80 00 04 .....#..........
00012B50 E4 22 20 04 E4 22 40 00 D4 24 80 00             ." .."@..$..    

l00012B5C:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; munge_compile_params: 00012B64
;;   Called from:
;;     00012E48 (in gen_aux_info_file)
;;     0001509C (in main)
munge_compile_params proc
	save	%sp,FFFFFF90,%sp
	call	strlen
	or	%g0,%i0,%o0
	sll	%o0,00000002,%o0
	add	%o0,00000027,%o0
	and	%o0,FFFFFFF8,%o0
	sub	%sp,%o0,%sp
	sethi	0000009F,%o1
	ld	[%o1+804],%o0
	add	%sp,00000060,%l1
	st	%o0,[%l1+%g0]
	or	%g0,00000001,%l0
	sethi	000000AD,%l2

l00012B98:
	ldub	[%i0+%g0],%o0
	or	%l2,00000011,%o2
	ldub	[%o0+%o2],%o1
	sll	%l0,00000002,%g2
	andcc	%o1,00000008,%g0
	be	00012BCC
	add	%l0,00000001,%o3

l00012BB4:
	add	%i0,00000001,%i0

l00012BB8:
	ldub	[%i0+%g0],%o0
	ldub	[%o0+%o2],%o1
	andcc	%o1,00000008,%g0
	bne,a	00012BB8
	add	%i0,00000001,%i0

l00012BCC:
	ldsb	[%i0+%g0],%o0
	subcc	%o0,00000000,%g0
	be	00012D00
	ldub	[%i0+%g0],%o1

l00012BDC:
	or	%l2,00000011,%o2
	ldub	[%o1+%o2],%o0
	andcc	%o0,00000008,%g0
	bne	00012C14
	or	%g0,%i0,%o3

l00012BF0:
	add	%i0,00000001,%i0

l00012BF4:
	ldsb	[%i0+%g0],%o0
	subcc	%o0,00000000,%g0
	be	00012C14
	ldub	[%i0+%g0],%o1

l00012C04:
	ldub	[%o1+%o2],%o0
	andcc	%o0,00000008,%g0
	be,a	00012BF4
	add	%i0,00000001,%i0

l00012C14:
	ldsb	[%o3+%g0],%o0
	subcc	%o0,0000002D,%g0
	bne	00012CD8
	or	%g0,%o3,%o0

l00012C24:
	ldub	[%o3+1],%o0
	add	%o0,FFFFFFB1,%o0
	sll	%o0,00000018,%o0
	sra	%o0,00000018,%o1
	subcc	%o1,00000020,%g0
	bgu	00012CD4
	sethi	0000004B,%o0

l00012C40:
	or	%o0,000001A8,%o0
	sll	%o1,00000002,%o1
	ld	[%o0+%o1],%o2
	jmpl	%o2,%g0,%g0
	sethi	00000000,%g0
00012C54             D2 0E 00 00 94 14 A0 11 D0 0A 40 0A     ..........@.
00012C60 80 8A 20 08 22 80 00 09 D0 4E 00 00 B0 06 20 01 .. ."....N.... .
00012C70 D0 0E 00 00 D2 0A 00 0A 80 8A 60 08 32 BF FF FD ..........`.2...
00012C80 B0 06 20 01 D0 4E 00 00 80 A2 20 00 02 80 00 18 .. ..N.... .....
00012C90 D2 0E 00 00 94 14 A0 11 D0 0A 40 0A 80 8A 20 08 ..........@... .
00012CA0 32 80 00 14 D0 4E 00 00 B0 06 20 01 D0 4E 00 00 2....N.... ..N..
00012CB0 80 A2 20 00 02 80 00 0F D2 0E 00 00 D0 0A 40 0A .. ...........@.
00012CC0 80 8A 20 08 22 BF FF FA B0 06 20 01 10 80 00 09 .. ."..... .....
00012CD0 D0 4E 00 00                                     .N..            

l00012CD4:
	or	%g0,%o3,%o0

l00012CD8:
	call	dupnstr
	sub	%i0,%o0,%o1
	sll	%l0,00000002,%o1
	st	%o0,[%l1+%o1]
	add	%l0,00000001,%l0
	ldsb	[%i0+%g0],%o0
	sll	%l0,00000002,%g2
	subcc	%o0,00000000,%g0
	bne	00012B98
	add	%l0,00000001,%o3

l00012D00:
	or	%g0,%o3,%l0
	sethi	0000009F,%o0
	st	%l0,[%o0+840]
	sll	%l0,00000002,%o2
	add	%l0,00000001,%l0
	sll	%l0,00000002,%o3
	add	%l0,00000001,%l0
	sll	%l0,00000002,%o4
	add	%l0,00000001,%l0
	sethi	0000005C,%o0
	sll	%l0,00000002,%o5
	or	%o0,00000220,%o0
	st	%o0,[%l1+%g2]
	add	%l0,00000001,%l0
	sethi	0000009F,%o1
	st	%l0,[%o1+836]
	sethi	0000005C,%o0
	st	%g0,[%l1+%o2]
	or	%o0,00000230,%o0
	sethi	0000005C,%o1
	st	%o0,[%l1+%o3]
	sll	%l0,00000002,%o2
	or	%o1,00000238,%o1
	st	%o1,[%l1+%o4]
	add	%l0,00000001,%l0
	sethi	0000005C,%o0
	sll	%l0,00000002,%o1
	or	%o0,00000240,%o0
	st	%o0,[%l1+%o5]
	add	%l0,00000001,%l0
	st	%g0,[%l1+%o2]
	add	%l0,00000001,%o0
	st	%g0,[%l1+%o1]
	call	xmalloc
	sll	%o0,00000002,%o0
	sethi	0000009F,%o1
	st	%o0,[%o1+828]
	or	%g0,%l1,%o1
	call	memcpy
	sll	%l0,00000002,%o2
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0
00012DA8                         00 01 2C EC 00 01 2C D4         ..,...,.
00012DB0 00 01 2C D4 00 01 2C D4 00 01 2C EC 00 01 2C D4 ..,...,...,...,.
00012DC0 00 01 2C D4 00 01 2C D4 00 01 2C D4 00 01 2C D4 ..,...,...,...,.
00012DD0 00 01 2C D4 00 01 2C D4 00 01 2C D4 00 01 2C D4 ..,...,...,...,.
00012DE0 00 01 2C D4 00 01 2C D4 00 01 2C D4 00 01 2C D4 ..,...,...,...,.
00012DF0 00 01 2C D4 00 01 2C D4 00 01 2C EC 00 01 2C D4 ..,...,...,...,.
00012E00 00 01 2C D4 00 01 2C D4 00 01 2C EC 00 01 2C D4 ..,...,...,...,.
00012E10 00 01 2C D4 00 01 2C D4 00 01 2C D4 00 01 2C D4 ..,...,...,...,.
00012E20 00 01 2C D4 00 01 2C D4 00 01 2C 54             ..,...,...,T    

;; gen_aux_info_file: 00012E2C
;;   Called from:
;;     00013108 (in process_aux_info_file)
gen_aux_info_file proc
	save	%sp,FFFFFF78,%sp
	sethi	0000009F,%l2
	ld	[%l2+836],%o0
	subcc	%o0,00000000,%g0
	bne	00012E54
	or	%g0,%i0,%o1

l00012E44:
	sethi	0000005C,%o0
	call	munge_compile_params
	or	%o0,00000218,%o0
	or	%g0,%i0,%o1

l00012E54:
	call	shortpath
	or	%g0,00000000,%o0
	ld	[%l2+836],%o1
	sethi	0000009F,%l1
	ld	[%l1+828],%o2
	or	%g0,%o0,%l0
	sll	%o1,00000002,%o1
	call	strlen
	st	%l0,[%o2+%o1]
	or	%g0,%o0,%o1
	sethi	0000005B,%o2
	or	%g0,%l0,%o0
	or	%o2,00000300,%o2
	call	savestring2
	or	%g0,00000002,%o3
	sethi	0000009F,%o1
	ld	[%o1+840],%o2
	sethi	0000009F,%o3
	ld	[%o3+812],%o1
	sll	%o2,00000002,%o2
	ld	[%l1+828],%o4
	or	%g0,%l1,%i0
	st	%o0,[%o4+%o2]
	subcc	%o1,00000000,%g0
	bne	00012ED8
	sethi	000000A0,%l0

l00012EBC:
	ld	[%l2+836],%o1
	sethi	0000005C,%o0
	sll	%o1,00000002,%o1
	ld	[%o4+%o1],%o2
	or	%o0,00000250,%o0
	call	notice
	ld	[%l0+336],%o1

l00012ED8:
	call	choose_temp_base
	sethi	00000000,%g0
	ld	[%i0+828],%o1
	or	%g0,%o0,%o3
	ld	[%o1+%g0],%o0
	or	%g0,00000007,%o4
	ld	[%l0+336],%o2
	add	%i6,FFFFFFE8,%o5
	st	%o4,[%sp+92]
	call	pexecute
	add	%i6,FFFFFFEC,%o4
	subcc	%o0,FFFFFFFF,%g0
	bne	00012F6C
	add	%i6,FFFFFFE4,%o1

l00012F10:
	sethi	000000AD,%o0
	ld	[%o0+864],%l2
	sethi	000000AD,%l1
	or	%l1,00000240,%l1
	ld	[%l0+336],%o2
	sethi	0000005C,%o1
	or	%o1,00000268,%o1
	call	fprintf
	or	%g0,%l1,%o0
	ld	[%i6-20],%o1
	or	%g0,%l1,%o0
	ld	[%i6-24],%o2
	call	fprintf
	or	%g0,00000000,%i0
	sethi	0000005C,%l0
	or	%l0,00000270,%l0
	call	xstrerror
	or	%g0,%l2,%o0
	or	%g0,%o0,%o2
	or	%g0,%l1,%o0
	call	fprintf
	or	%g0,%l0,%o1
	ba,a	0001302C

l00012F6C:
	call	pwait
	or	%g0,00000000,%o2
00012F74             80 A2 3F FF 12 80 00 0C D0 07 BF E4     ..?.........
00012F80 E2 04 21 50 13 00 00 AD D0 02 63 60 21 00 00 5C ..!P......c`!..\
00012F90 7F FF F9 B4 A0 14 22 78 94 10 00 08 90 10 00 10 ......"x........
00012FA0 10 80 00 0E 92 10 00 11 80 8A 20 FF 02 80 00 0E .......... .....
00012FB0 94 10 00 08 11 00 00 3F 90 12 23 00 80 8A 80 08 .......?..#.....
00012FC0 12 80 00 0A 80 8A A0 FF D2 04 21 50 11 00 00 5C ..........!P...\
00012FD0 90 12 22 88 94 0A A0 7F 7F FF F9 95 B0 10 20 00 .."........... .
00012FE0 30 80 00 13 80 8A A0 FF 12 80 00 0F 91 3A A0 08 0............:..
00012FF0 98 8A 20 FF 02 80 00 0A D6 06 23 3C 11 00 00 5C .. .......#<...\
00013000 D4 02 C0 00 90 12 22 B0 D2 04 21 50 7F FF F9 88 ......"...!P....
00013010 96 10 00 0C 10 80 00 06 B0 10 20 00 10 80 00 04 .......... .....
00013020 B0 10 20 01 40 00 53 3B 01 00 00 00             .. .@.S;....    

l0001302C:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; process_aux_info_file: 00013034
;;   Called from:
;;     00014E60 (in do_processing)
process_aux_info_file proc
	save	%sp,FFFFFF08,%sp
	call	strlen
	or	%g0,%i0,%o0
	or	%g0,%o0,%i3
	add	%i3,0000000A,%o0
	and	%o0,FFFFFFF8,%o0
	sub	%sp,%o0,%sp
	add	%sp,00000060,%l4
	or	%g0,%i0,%o1
	call	strcpy
	or	%g0,%l4,%o0
	sethi	0000005B,%o1
	or	%o1,00000300,%o1
	call	strcat
	or	%g0,%l4,%o0
	or	%g0,00000000,%l2

l00013074:
	or	%g0,%l4,%o0
	call	access
	or	%g0,00000004,%o1
	subcc	%o0,FFFFFFFF,%g0
	bne	00013100
	subcc	%l2,00000000,%g0

l0001308C:
	sethi	000000AD,%o0
	ld	[%o0+864],%l3
	subcc	%l3,00000002,%g0
	bne	000130C8
	sethi	000000A0,%o0

l000130A0:
	subcc	%i2,00000000,%g0
	be	000130C0
	ld	[%o0+336],%o1

l000130AC:
	or	%g0,%l4,%o2
	sethi	0000005C,%o0
	call	notice
	or	%o0,000002D0,%o0
	ba,a	000135B8

l000130C0:
	ba	000130FC
	or	%g0,00000001,%l2

l000130C8:
	ld	[%o0+336],%l2
	sethi	0000005C,%l0
	or	%g0,%l4,%o1
	or	%l0,00000300,%l0
	call	shortpath
	or	%g0,00000000,%o0
	or	%g0,%o0,%l1
	call	xstrerror
	or	%g0,%l3,%o0
	or	%g0,%o0,%o3
	or	%g0,%l0,%o0
	ba	000131F0
	or	%g0,%l2,%o1

l000130FC:
	subcc	%l2,00000000,%g0

l00013100:
	be	00013150
	add	%i6,FFFFFF68,%l1

l00013108:
	call	gen_aux_info_file
	or	%g0,%i0,%o0
	subcc	%o0,00000000,%g0
	be	000131F8
	or	%g0,%l4,%o0

l0001311C:
	call	access
	or	%g0,00000004,%o1
	subcc	%o0,FFFFFFFF,%g0
	bne	00013150
	add	%i6,FFFFFF68,%l1

l00013130:
	sethi	000000AD,%o0
	sethi	000000A0,%o1
	ld	[%o0+864],%l2
	sethi	0000005C,%l0
	ld	[%o1+336],%l3
	or	%l0,00000300,%l0
	ba	000131D0
	or	%g0,%l4,%o1

l00013150:
	or	%g0,%l4,%o0
	call	stat
	or	%g0,%l1,%o1
	subcc	%o0,FFFFFFFF,%g0
	bne	00013188
	ld	[%i6-104],%l0

l00013168:
	sethi	000000AD,%o0
	sethi	000000A0,%o1
	ld	[%o0+864],%l2
	sethi	0000005C,%l0
	ld	[%o1+336],%l3
	or	%l0,00000328,%l0
	ba	000131D0
	or	%g0,%l4,%o1

l00013188:
	subcc	%l0,00000000,%g0
	be	000135B8
	subcc	%i2,00000000,%g0

l00013194:
	bne	00013218
	ld	[%i6-88],%l7

l0001319C:
	or	%g0,%l1,%o1
	call	stat
	or	%g0,%i0,%o0
	subcc	%o0,FFFFFFFF,%g0
	bne	0001320C
	ld	[%i6-88],%o0

l000131B4:
	sethi	000000AD,%o0
	sethi	000000A0,%o1
	ld	[%o0+864],%l2
	sethi	0000005C,%l0
	ld	[%o1+336],%l3
	or	%l0,00000328,%l0
	or	%g0,%i0,%o1

l000131D0:
	call	shortpath
	or	%g0,00000000,%o0
	or	%g0,%o0,%l1
	call	xstrerror
	or	%g0,%l2,%o0
	or	%g0,%o0,%o3
	or	%g0,%l0,%o0
	or	%g0,%l3,%o1

l000131F0:
	call	notice
	or	%g0,%l1,%o2

l000131F8:
	sethi	0000009F,%o1
	ld	[%o1+800],%o0
	add	%o0,00000001,%o0
	ba	000135B8
	st	%o0,[%o1+800]

l0001320C:
	subcc	%o0,%l7,%g0
	bg,a	00013074
	or	%g0,00000001,%l2

l00013218:
	or	%g0,%l4,%o0
	or	%g0,00000000,%o1
	call	open
	or	%g0,00000124,%o2
	or	%g0,%o0,%l5
	subcc	%l5,FFFFFFFF,%g0
	bne	00013250
	sethi	000000AD,%o0

l00013238:
	sethi	000000A0,%o1
	ld	[%o0+864],%l2
	sethi	0000005C,%l0
	ld	[%o1+336],%l3
	ba	00013314
	or	%l0,00000358,%l0

l00013250:
	call	xmalloc
	add	%l0,00000001,%o0
	or	%g0,%o0,%l6
	stb	%g0,[%l6+%l0]
	or	%g0,%l5,%o0
	or	%g0,%l6,%o1
	call	safe_read
	or	%g0,%l0,%o2
	subcc	%o0,%l0,%g0
	be	00013294
	sethi	000000AD,%o0

l0001327C:
	sethi	000000A0,%o1
	ld	[%o0+864],%l2
	sethi	0000005C,%l0
	ld	[%o1+336],%l3
	ba	000132BC
	or	%l0,00000390,%l0

l00013294:
	call	close
	or	%g0,%l5,%o0
	subcc	%o0,00000000,%g0
	be	00013344
	sethi	000000AD,%o0

l000132A8:
	sethi	000000A0,%o1
	ld	[%o0+864],%l2
	sethi	0000005C,%l0
	ld	[%o1+336],%l3
	or	%l0,000003C0,%l0

l000132BC:
	or	%g0,%l4,%o1
	call	shortpath
	or	%g0,00000000,%o0
	or	%g0,%o0,%l1
	call	xstrerror
	or	%g0,%l2,%o0
	or	%g0,%o0,%o3
	or	%g0,%l0,%o0
	or	%g0,%l3,%o1
	call	notice
	or	%g0,%l1,%o2
	call	free
	or	%g0,%l6,%o0
	call	close
	or	%g0,%l5,%o0
	ba,a	000135B8
	sethi	000000AD,%o0
00013300 13 00 00 A0 E4 02 23 60 21 00 00 5D E6 02 61 50 ......#`!..]..aP
00013310 A0 14 20 20                                     ..              

l00013314:
	or	%g0,%l4,%o1
	call	shortpath
	or	%g0,00000000,%o0
	or	%g0,%o0,%l1
	call	xstrerror
	or	%g0,%l2,%o0
	or	%g0,%o0,%o3
	or	%g0,%l0,%o0
	or	%g0,%l3,%o1
	call	notice
	or	%g0,%l1,%o2
	ba,a	000135B8

l00013344:
	subcc	%l2,00000000,%g0
00013348                         02 80 00 1A 80 A6 60 00         ......`.
00013350 32 80 00 19 D0 4D 80 00 40 00 52 92 90 10 00 14 2....M..@.R.....
00013360 80 A2 3F FF 32 80 00 14 D0 4D 80 00 11 00 00 AD ..?.2....M......
00013370 13 00 00 A0 E4 02 23 60 21 00 00 5C E6 02 61 50 ......#`!..\..aP
00013380 A0 14 23 F0 92 10 00 14 7F FF FB 71 90 10 20 00 ..#........q.. .
00013390 A2 10 00 08 7F FF F8 B3 90 10 00 12 96 10 00 08 ................
000133A0 90 10 00 10 92 10 00 13 7F FF F8 A1 94 10 00 11 ................
000133B0 D0 4D 80 00 A2 10 00 16 80 A2 20 3A 21 00 00 A0 .M........ :!...
000133C0 02 80 00 07 2B 00 00 A0 A2 04 60 01 D0 4C 40 00 ....+.....`..L@.
000133D0 80 A2 20 3A 32 BF FF FE A2 04 60 01 A2 04 60 01 .. :2.....`...`.
000133E0 D0 4C 40 00 80 A2 20 20 22 BF FF FE A2 04 60 01 .L@...  ".....`.
000133F0 E2 24 21 90 D0 4C 40 00 80 A2 20 20 32 BF FF FE .$!..L@...  2...
00013400 A2 04 60 01 90 10 20 2F D0 2C 40 00 A2 04 60 01 ..`... /.,@...`.
00013410 C0 2C 40 00 A2 04 60 01 D0 4C 40 00 80 A2 20 0A .,@...`..L@... .
00013420 12 BF FF FE A2 04 60 01 D2 04 21 90 D0 4A 40 00 ......`...!..J@.
00013430 80 A2 20 2F 02 80 00 12 A6 10 20 00 90 24 40 09 .. /...... ..$@.
00013440 7F FF F8 8D 90 06 C0 08 A6 10 00 08 40 00 52 25 ............@.R%
00013450 92 10 00 18 90 10 00 13 40 00 52 55 92 10 20 2F ........@.RU.. /
00013460 80 A2 20 00 12 80 00 03 90 02 20 01 90 10 00 13 .. ....... .....
00013470 40 00 52 1C D2 04 21 90 E6 24 21 90 80 A6 A0 00 @.R...!..$!.....
00013480 12 80 00 29 90 10 20 02 D0 25 61 70 A0 10 00 11 ...).. ..%ap....
00013490 D0 4C 00 00 80 A2 20 00 02 80 00 23 90 10 20 02 .L.... ....#.. .
000134A0 A4 10 00 15 90 10 00 10 7F FF FB ED 92 10 00 17 ................
000134B0 80 A2 20 00 22 80 00 11 D0 4C 00 00 40 00 52 06 .. ."....L..@.R.
000134C0 90 10 00 16 7F FF F8 91 90 10 00 13 80 A6 60 00 ..............`.
000134D0 02 BF FE E9 A4 10 20 01 40 00 52 32 90 10 00 14 ...... .@.R2....
000134E0 80 A2 3F FF 02 BF FF 86 A4 10 20 01 10 BF FE E3 ..?....... .....
000134F0 90 10 00 14 D0 4C 00 00 80 A2 20 0A 12 BF FF FE .....L.... .....
00013500 A0 04 20 01 D0 04 A1 70 90 02 20 01 D0 24 A1 70 .. ....p.. ..$.p
00013510 D2 4C 00 00 80 A2 60 00 12 BF FF E4 90 10 00 10 .L....`.........
00013520 90 10 20 02 D0 25 61 70 A0 10 00 11 D0 4C 00 00 .. ..%ap.....L..
00013530 80 A2 20 00 02 80 00 1D 01 00 00 00 A4 10 00 15 .. .............
00013540 7F FF FA 06 90 10 00 10 A2 92 20 00 22 80 00 08 .......... ."...
00013550 90 10 00 10 7F FF FB F2 92 10 00 1A 40 00 51 DE ............@.Q.
00013560 90 10 00 11 10 80 00 07 D0 4C 00 00 7F FF FB EC .........L......
00013570 92 10 00 1A 10 80 00 03 D0 4C 00 00 D0 4C 00 00 .........L...L..
00013580 80 A2 20 0A 12 BF FF FE A0 04 20 01 D0 04 A1 70 .. ....... ....p
00013590 90 02 20 01 D0 24 A1 70 D2 4C 00 00 80 A2 60 00 .. ..$.p.L....`.
000135A0 12 BF FF E8 01 00 00 00 40 00 51 CB 90 10 00 16 ........@.Q.....
000135B0 7F FF F8 56 90 10 00 13                         ...V....        

l000135B8:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; reverse_def_dec_list: 000135C0
;;   Called from:
;;     00011C0C (in visit_each_hash_node)
reverse_def_dec_list proc
	ld	[%o0+8],%o1
	ld	[%o1+4],%g3
	subcc	%g3,00000000,%g0
	be	00013604
	or	%g0,%g3,%o0

l000135D4:
	ld	[%o0+%g0],%g3
	subcc	%g3,00000000,%g0
	be	00013604
	sethi	00000000,%g0

l000135E4:
	st	%g0,[%o0+%g0]
	ld	[%g3+%g0],%g2

l000135EC:
	st	%o0,[%g3+%g0]
	or	%g0,%g3,%o0
	orcc	%g2,00000000,%g3
	bne,a	000135EC
	ld	[%g3+%g0],%g2

l00013600:
	st	%o0,[%o1+4]

l00013604:
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0

;; identify_lineno: 0001360C
;;   Called from:
;;     000136B8 (in declare_source_confusing)
;;     00013D9C (in edit_formals_lists)
;;     00013F68 (in edit_fn_definition)
;;     0001464C (in scan_for_missed_items)
;;     00014820 (in scan_for_missed_items)
identify_lineno proc
	sethi	000000A0,%g2
	ld	[%g2+448],%g3
	or	%g0,%o0,%o1
	subcc	%g3,%o1,%g0
	bgu	00013644
	or	%g0,00000001,%o0

l00013624:
	ldsb	[%g3+%g0],%g2

l00013628:
	subcc	%g2,0000000A,%g0
	be,a	00013634
	add	%o0,00000001,%o0

l00013634:
	add	%g3,00000001,%g3
	subcc	%g3,%o1,%g0
	bleu,a	00013628
	ldsb	[%g3+%g0],%g2

l00013644:
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0

;; declare_source_confusing: 0001364C
;;   Called from:
;;     000136F0 (in check_source)
declare_source_confusing proc
	save	%sp,FFFFFF90,%sp
	sethi	0000009F,%o0
	ld	[%o0+812],%o1
	subcc	%o1,00000000,%g0
	bne,a	000136D4
	sethi	000000AC,%o0

l00013664:
	subcc	%i0,00000000,%g0
	bne	000136A0
	sethi	000000A0,%o0

l00013670:
	ld	[%o0+384],%o1
	sethi	0000005D,%l0
	or	%l0,00000040,%l0
	call	shortpath
	or	%g0,00000000,%o0
	sethi	000000A0,%o3
	or	%g0,%o0,%o1
	ld	[%o3+560],%o2
	call	notice
	or	%g0,%l0,%o0
	ba	000136D4
	sethi	000000AC,%o0

l000136A0:
	ld	[%o0+384],%o1
	sethi	0000005D,%l0
	or	%l0,00000040,%l0
	call	shortpath
	or	%g0,00000000,%o0
	or	%g0,%o0,%l1
	call	identify_lineno
	or	%g0,%i0,%o0
	or	%g0,%o0,%o2
	or	%g0,%l0,%o0
	call	notice
	or	%g0,%l1,%o1
	sethi	000000AC,%o0

l000136D4:
	or	%o0,00000240,%o0
	call	longjmp
	or	%g0,00000001,%o1

;; check_source: 000136E0
;;   Called from:
;;     00013764 (in seek_to_line)
;;     000137E4 (in forward_to_next_token_char)
;;     00013BB4 (in edit_formals_lists)
;;     00013BE4 (in edit_formals_lists)
;;     00013C1C (in edit_formals_lists)
;;     00013C48 (in edit_formals_lists)
;;     00013C60 (in edit_formals_lists)
;;     00013C78 (in edit_formals_lists)
;;     00013CE4 (in edit_formals_lists)
;;     00013D14 (in edit_formals_lists)
;;     00013D44 (in edit_formals_lists)
;;     00013E68 (in find_rightmost_formals_list)
;;     00013E9C (in find_rightmost_formals_list)
;;     00013ECC (in find_rightmost_formals_list)
;;     0001455C (in careful_find_l_paren)
;;     00014600 (in scan_for_missed_items)
;;     000146C4 (in scan_for_missed_items)
;;     0001472C (in scan_for_missed_items)
check_source proc
	save	%sp,FFFFFF90,%sp
	subcc	%i0,00000000,%g0
	bne	000136F8
	or	%g0,%i1,%o0

l000136F0:
	call	declare_source_confusing
	sethi	00000000,%g0

l000136F8:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; seek_to_line: 00013700
seek_to_line proc
	save	%sp,FFFFFF90,%sp
	sethi	000000A0,%o1
	ld	[%o1+560],%o0
	subcc	%i0,%o0,%g0
	bge	00013720
	sethi	00000000,%g0

l00013718:
	call	abort
0001371C                                     01 00 00 00             ....

l00013720:
	ble	000137A0
	sethi	000000A0,%l4

l00013728:
	or	%g0,%o1,%l3
	or	%g0,%l4,%l2

l00013730:
	ld	[%l2+544],%o0
	ldsb	[%o0+%g0],%o1
	subcc	%o1,0000000A,%g0
	be	00013788
	ld	[%l3+560],%o1

l00013744:
	sethi	000000A0,%l0
	sethi	000000A0,%l1
	ld	[%l0+544],%o2

l00013750:
	or	%g0,00000000,%o1
	ld	[%l1+464],%o0
	add	%o2,00000001,%o2
	subcc	%o2,%o0,%g0
	addx	%g0,00000000,%o0
	call	check_source
	st	%o2,[%l0+544]
	ld	[%l0+544],%o0
	ldsb	[%o0+%g0],%o1
	subcc	%o1,0000000A,%g0
	bne	00013750
	ld	[%l0+544],%o2

l00013780:
	ld	[%l2+544],%o0
	ld	[%l3+560],%o1

l00013788:
	add	%o0,00000001,%o0
	add	%o1,00000001,%o1
	st	%o0,[%l2+544]
	subcc	%i0,%o1,%g0
	bg	00013730
	st	%o1,[%l3+560]

l000137A0:
	ld	[%l4+544],%i0
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; forward_to_next_token_char: 000137AC
;;   Called from:
;;     00013EB4 (in find_rightmost_formals_list)
forward_to_next_token_char proc
	save	%sp,FFFFFF90,%sp
	add	%i0,00000001,%i0
	ldub	[%i0+%g0],%o0
	sethi	000000AD,%o1
	or	%o1,00000011,%o1
	ldub	[%o0+%o1],%o2
	andcc	%o2,00000008,%g0
	be	00013800
	or	%g0,%o1,%l1

l000137D0:
	sethi	000000A0,%l0
	ld	[%l0+464],%o0

l000137D8:
	add	%i0,00000001,%i0
	subcc	%i0,%o0,%g0
	addx	%g0,00000000,%o0
	call	check_source
	or	%g0,00000000,%o1
	ldub	[%i0+%g0],%o0
	ldub	[%o0+%l1],%o1
	andcc	%o1,00000008,%g0
	bne	000137D8
	ld	[%l0+464],%o0

l00013800:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; output_bytes: 00013808
;;   Called from:
;;     0001389C (in output_string)
;;     000138DC (in output_up_to)
output_bytes proc
	save	%sp,FFFFFF90,%sp
	sethi	000000A0,%l2
	ld	[%l2+528],%o1
	sethi	000000A0,%l3
	ld	[%l3+512],%l0
	add	%i1,00000001,%o0
	add	%o1,%o0,%o1
	subcc	%o1,%l0,%g0
	bcs	00013864
	sethi	000000A0,%l1

l00013830:
	ld	[%l1+496],%o0
	sub	%l0,%o0,%l0
	sll	%l0,00000001,%l0
	call	xrealloc
	or	%g0,%l0,%o1
	ld	[%l2+528],%o1
	add	%o0,%l0,%l0
	ld	[%l1+496],%o2
	st	%l0,[%l3+512]
	sub	%o1,%o2,%o1
	add	%o0,%o1,%o1
	st	%o1,[%l2+528]
	st	%o0,[%l1+496]

l00013864:
	ld	[%l2+528],%o0
	or	%g0,%i0,%o1
	add	%o0,00000001,%o0
	call	memcpy
	or	%g0,%i1,%o2
	ld	[%l2+528],%o1
	add	%o1,%i1,%o1
	st	%o1,[%l2+528]
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; output_string: 0001388C
;;   Called from:
;;     00013DE8 (in edit_formals_lists)
output_string proc
	save	%sp,FFFFFF90,%sp
	call	strlen
	or	%g0,%i0,%o0
	or	%g0,%o0,%o1
	call	output_bytes
	or	%g0,%i0,%o0
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; output_up_to: 000138AC
;;   Called from:
;;     00013DD4 (in edit_formals_lists)
;;     00013F88 (in edit_fn_definition)
;;     00013FD0 (in edit_fn_definition)
output_up_to proc
	save	%sp,FFFFFF90,%sp
	sethi	000000A0,%o0
	ld	[%o0+448],%o1
	sethi	000000A0,%l0
	ld	[%l0+480],%o2
	sethi	000000A0,%o3
	ld	[%o3+416],%o0
	sub	%o2,%o1,%o1
	add	%o0,%o1,%o0
	subcc	%i0,%o2,%o1
	be	000138E8
	add	%o0,00000001,%o0

l000138DC:
	call	output_bytes
	sethi	00000000,%g0
	st	%i0,[%l0+480]

l000138E8:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; other_variable_style_function: 000138F0
;;   Called from:
;;     00013F20 (in edit_fn_definition)
other_variable_style_function proc
	save	%sp,FFFFFF90,%sp
	sethi	0000005D,%o1
	or	%g0,%i0,%o0
	call	substr
	or	%o1,00000068,%o1
	subcc	%g0,%o0,%g0
	addx	%g0,00000000,%i0
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; edit_fn_declaration: 00013914
edit_fn_declaration proc
	save	%sp,FFFFFF88,%sp
	st	%i0,[%i6+68]
	ld	[%i0+16],%o0
	ld	[%o0+4],%o0
	st	%i1,[%i6+72]
	call	strlen
	st	%o0,[%i6-20]
	call	save_pointers
	st	%o0,[%i6-24]
	sethi	000000AC,%o0
	call	setjmp
	or	%o0,00000240,%o0
	subcc	%o0,00000000,%g0
	be	00013974
	sethi	00000000,%g0

l00013950:
	call	restore_pointers
	sethi	00000000,%g0
	sethi	000000A0,%o0
	ld	[%o0+336],%o1
	ld	[%i6-20],%o2
	sethi	0000005D,%o0
	call	notice
	or	%o0,00000070,%o0
	ba,a	00013B60

l00013974:
	ld	[%i6+72],%o0
00013978                         D2 4A 00 00 80 A2 60 0A         .J....`.
00013980 02 80 00 10 21 00 00 A0 D0 07 A0 48 92 10 20 00 ....!......H.. .
00013990 90 02 20 01 D0 27 A0 48 D0 04 21 D0 D4 07 A0 48 .. ..'.H..!....H
000139A0 80 A2 80 08 7F FF FF 4F 90 40 20 00 D0 07 A0 48 .......O.@ ....H
000139B0 D2 4A 00 00 80 A2 60 0A 12 BF FF F4 01 00 00 00 .J....`.........
000139C0 D0 07 A0 48 23 00 00 A0 90 02 3F FF D0 27 A0 48 ...H#.....?..'.H
000139D0 30 80 00 0A D0 07 A0 48 92 10 20 00 90 02 3F FF 0......H.. ...?.
000139E0 D0 27 A0 48 D0 04 61 E0 D4 07 A0 48 80 A2 00 0A .'.H..a....H....
000139F0 7F FF FF 3C 90 40 20 00 D2 07 A0 48 7F FF F7 D8 ...<.@ ....H....
00013A00 D0 4A 40 00 80 A2 20 00 02 BF FF F3 21 00 00 A0 .J@... .....!...
00013A10 30 80 00 0A D0 07 A0 48 92 10 20 00 90 02 3F FF 0......H.. ...?.
00013A20 D0 27 A0 48 D0 04 21 E0 D4 07 A0 48 80 A2 00 0A .'.H..!....H....
00013A30 7F FF FF 2C 90 40 20 00 D2 07 A0 48 7F FF F7 C8 ...,.@ ....H....
00013A40 D0 4A 40 00 80 A2 20 00 12 BF FF F3 01 00 00 00 .J@... .........
00013A50 D0 07 A0 48 D2 07 BF EC 90 02 20 01 40 00 50 B3 ...H...... .@.P.
00013A60 D4 07 BF E8 80 A2 20 00 12 BF FF E4 D0 07 BF E8 ...... .........
00013A70 D2 07 A0 48 92 02 00 09 7F FF F7 B9 D0 4A 60 01 ...H.........J`.
00013A80 80 A2 20 00 12 BF FF DD D0 07 A0 44 D2 02 20 10 .. ........D.. .
00013A90 40 00 50 A3 D0 02 60 04 D2 07 A0 48 7F FF FF 44 @.P...`....H...D
00013AA0 90 02 40 08 A0 10 00 08 D0 4C 00 00 80 A2 20 28 ..@......L.... (
00013AB0 12 BF FF D2 25 00 00 A0 23 00 00 A0 B0 04 20 01 ....%...#..... .
00013AC0 B2 10 20 01 D0 4E 00 00 80 A2 20 28 02 80 00 06 .. ..N.... (....
00013AD0 80 A2 20 29 02 80 00 06 D0 04 61 D0 10 80 00 07 .. )......a.....
00013AE0 B0 06 20 01 10 80 00 03 B2 06 60 01 B2 06 7F FF .. .......`.....
00013AF0 D0 04 61 D0 B0 06 20 01 80 A6 00 08 90 40 20 00 ..a... ......@ .
00013B00 7F FF FE F8 92 10 20 00 80 A6 60 00 32 BF FF EF ...... ...`.2...
00013B10 D0 4E 00 00 7F FF FF 66 90 10 00 10 B0 06 3F FF .N.....f......?.
00013B20 90 06 3F FF D0 24 A1 E0 7F FF FF 21 90 10 00 18 ..?..$.....!....
00013B30 92 10 00 08 D0 4A 40 00 80 A2 20 29 12 80 00 09 .....J@... )....
00013B40 01 00 00 00 7F FF FF 1A 90 10 00 09 A0 10 00 08 ................
00013B50 D0 4C 00 00 80 A2 20 28 02 BF FF DA B0 04 20 01 .L.... (...... .

l00013B60:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; edit_formals_lists: 00013B68
;;   Called from:
;;     00013C88 (in edit_formals_lists)
edit_formals_lists proc
	save	%sp,FFFFFF90,%sp
	add	%i0,FFFFFFFF,%l1
	or	%g0,00000001,%l0
	sethi	000000A0,%l2
	ldsb	[%l1+%g0],%o0

l00013B7C:
	subcc	%o0,00000028,%g0
	be	00013B98
	subcc	%o0,00000029,%g0

l00013B88:
	be	00013BA0
	ld	[%l2+480],%o0

l00013B90:
	ba	00013BAC
	add	%l1,FFFFFFFF,%l1

l00013B98:
	ba	00013BA4
	add	%l0,FFFFFFFF,%l0

l00013BA0:
	add	%l0,00000001,%l0

l00013BA4:
	ld	[%l2+480],%o0
	add	%l1,FFFFFFFF,%l1

l00013BAC:
	subcc	%o0,%l1,%g0
	addx	%g0,00000000,%o0
	call	check_source
	or	%g0,00000000,%o1
	subcc	%l0,00000000,%g0
	bne,a	00013B7C
	ldsb	[%l1+%g0],%o0

l00013BC8:
	addcc	%i1,FFFFFFFF,%i1
	be	00013CA4
	add	%l1,00000001,%l1

l00013BD4:
	ld	[%l2+480],%o0
	add	%l1,FFFFFFFF,%l0
	subcc	%o0,%l0,%g0
	addx	%g0,00000000,%o0
	call	check_source
	or	%g0,00000000,%o1
	ldub	[%l1-1],%o2
	sethi	000000AD,%o0
	or	%o0,00000011,%o0
	ldub	[%o2+%o0],%o1
	andcc	%o1,00000008,%g0
	be,a	00013C3C
	ldsb	[%l0+%g0],%o0

l00013C08:
	or	%g0,%o0,%l3
	ld	[%l2+480],%o0

l00013C10:
	add	%l0,FFFFFFFF,%l0
	subcc	%o0,%l0,%g0
	addx	%g0,00000000,%o0
	call	check_source
	or	%g0,00000000,%o1
	ldub	[%l0+%g0],%o0
	ldub	[%o0+%l3],%o1
	andcc	%o1,00000008,%g0
	bne,a	00013C10
	ld	[%l2+480],%o0

l00013C38:
	ldsb	[%l0+%g0],%o0

l00013C3C:
	or	%g0,%l0,%o1
	xor	%o0,00000029,%o0
	subcc	%g0,%o0,%g0
	call	check_source
	subx	%g0,FFFFFFFF,%o0
	ld	[%l2+480],%o0
	add	%l0,FFFFFFFF,%l0
	subcc	%o0,%l0,%g0
	addx	%g0,00000000,%o0
	call	check_source
	or	%g0,00000000,%o1
	ldsb	[%l0+%g0],%o0
	or	%g0,%l0,%o1
	xor	%o0,00000029,%o0
	subcc	%g0,%o0,%g0
	call	check_source
	subx	%g0,FFFFFFFF,%o0
	or	%g0,%l0,%o0
	or	%g0,%i1,%o1
	call	edit_formals_lists
	or	%g0,%i2,%o2
	subcc	%o0,00000000,%g0
	be	00013CA8
	subcc	%i1,00000000,%g0

l00013C9C:
	ba	00013DFC
	or	%g0,00000001,%i0

l00013CA4:
	subcc	%i1,00000000,%g0

l00013CA8:
	bne	00013DD4
	sethi	000000AD,%o0

l00013CB0:
	ldub	[%l1-1],%o2
	or	%o0,00000011,%o3
	ld	[%i2+16],%o1
	add	%l1,FFFFFFFF,%l0
	ldub	[%o2+%o3],%o0
	andcc	%o0,00000008,%g0
	be	00013D00
	ld	[%o1+4],%l5

l00013CD0:
	or	%g0,%o3,%l3
	ld	[%l2+480],%o0

l00013CD8:
	add	%l0,FFFFFFFF,%l0
	subcc	%o0,%l0,%g0
	addx	%g0,00000000,%o0
	call	check_source
	or	%g0,00000000,%o1
	ldub	[%l0+%g0],%o0
	ldub	[%o0+%l3],%o1
	andcc	%o1,00000008,%g0
	bne	00013CD8
	ld	[%l2+480],%o0

l00013D00:
	or	%g0,%l0,%l3
	ba	00013D20
	add	%l3,00000001,%l0

l00013D0C:
	or	%g0,00000000,%o1
	subcc	%o0,%l3,%g0
	call	check_source
	addx	%g0,00000000,%o0
	add	%l3,FFFFFFFF,%l3

l00013D20:
	call	is_id_char
	ldsb	[%l3+%g0],%o0
	subcc	%o0,00000000,%g0
	bne,a	00013D0C
	ld	[%l2+480],%o0

l00013D34:
	add	%l3,00000001,%l3
	subcc	%l0,%l3,%l4
	bne	00013D4C
	or	%g0,00000000,%o0

l00013D44:
	call	check_source
	or	%g0,%l3,%o1

l00013D4C:
	call	strlen
	or	%g0,%l5,%o0
	subcc	%l4,%o0,%g0
	bne,a	00013D80
	ld	[%i2+4],%o0

l00013D60:
	or	%g0,%l3,%o0
	or	%g0,%l5,%o1
	call	strncmp
	or	%g0,%l4,%o2
	subcc	%o0,00000000,%g0
	be	00013DD4
	sethi	00000000,%g0

l00013D7C:
	ld	[%i2+4],%o0

l00013D80:
	sethi	0000005D,%l0
	ld	[%o0+%g0],%o2
	or	%l0,000000A0,%l0
	ld	[%o2+4],%o1
	call	shortpath
	or	%g0,00000000,%o0
	or	%g0,%o0,%l2
	call	identify_lineno
	or	%g0,%l3,%o0
	or	%g0,%o0,%l1
	or	%g0,%l4,%o1
	call	dupnstr
	or	%g0,%l3,%o0
	or	%g0,%o0,%o3
	or	%g0,%l0,%o0
	or	%g0,%l2,%o1
	or	%g0,%l1,%o2
	call	notice
	or	%g0,%l5,%o4
	ba	00013DFC
	or	%g0,00000001,%i0

l00013DD4:
	call	output_up_to
	or	%g0,%l1,%o0
	subcc	%i1,00000000,%g0
	bne	00013DF4
	add	%i0,FFFFFFFF,%o0

l00013DE8:
	call	output_string
	ld	[%i2+36],%o0
	add	%i0,FFFFFFFF,%o0

l00013DF4:
	st	%o0,[%l2+480]
	or	%g0,00000000,%i0

l00013DFC:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; find_rightmost_formals_list: 00013E04
;;   Called from:
;;     00013F10 (in edit_fn_definition)
find_rightmost_formals_list proc
	save	%sp,FFFFFF90,%sp
	ba	00013E14
	ldsb	[%i0+%g0],%o0

l00013E10:
	ldsb	[%i0+%g0],%o0

l00013E14:
	subcc	%o0,0000000A,%g0
	bne,a	00013E10
	add	%i0,00000001,%i0

l00013E20:
	add	%i0,FFFFFFFF,%i0
	ldsb	[%i0+%g0],%o0
	subcc	%o0,00000029,%g0
	be	00013EB4
	ldub	[%i0+%g0],%o1

l00013E34:
	sethi	000000AD,%o0
	or	%g0,%o0,%l4
	or	%o0,00000011,%l3
	sethi	000000A0,%l2

l00013E44:
	ldub	[%o1+%l3],%o0
	andcc	%o0,00000008,%g0
	be	00013E8C
	sethi	000000A0,%l1

l00013E54:
	or	%l4,00000011,%l0
	ld	[%l1+480],%o0

l00013E5C:
	add	%i0,FFFFFFFF,%i0
	subcc	%o0,%i0,%g0
	addx	%g0,00000000,%o0
	call	check_source
	or	%g0,00000000,%o1
	ldub	[%i0+%g0],%o0
	ldub	[%o0+%l0],%o1
	andcc	%o1,00000008,%g0
	bne	00013E5C
	ld	[%l1+480],%o0

l00013E84:
	ba	00013EA8
	ldsb	[%i0+%g0],%o0

l00013E8C:
	ld	[%l2+480],%o0
	add	%i0,FFFFFFFF,%i0
	subcc	%o0,%i0,%g0
	addx	%g0,00000000,%o0
	call	check_source
	or	%g0,00000000,%o1
	ldsb	[%i0+%g0],%o0

l00013EA8:
	subcc	%o0,00000029,%g0
	bne	00013E44
	ldub	[%i0+%g0],%o1

l00013EB4:
	call	forward_to_next_token_char
	or	%g0,%i0,%o0
	ldsb	[%o0+%g0],%o2
	or	%g0,%o0,%o1
	xor	%o2,0000007B,%o2
	subcc	%g0,%o2,%g0
	call	check_source
	subx	%g0,FFFFFFFF,%o0
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; edit_fn_definition: 00013EDC
edit_fn_definition proc
	save	%sp,FFFFFF88,%sp
	st	%i0,[%i6+68]
	ld	[%i0+16],%o0
	ld	[%o0+4],%o0
	st	%i1,[%i6+72]
	call	save_pointers
	st	%o0,[%i6-20]
	sethi	000000AC,%o0
	call	setjmp
	or	%o0,00000240,%o0
	subcc	%o0,00000000,%g0
	bne	00013FB0
	sethi	00000000,%g0

l00013F10:
	call	find_rightmost_formals_list
	ld	[%i6+72],%o0
	ld	[%i6+68],%o1
	or	%g0,%o0,%i1
	call	other_variable_style_function
	ld	[%o1+12],%o0
	subcc	%o0,00000000,%g0
	be	00013F94
	ld	[%i6+68],%o0

l00013F34:
	sethi	0000009F,%o0
	ld	[%o0+812],%o1
	subcc	%o1,00000000,%g0
	bne	00013F88
	ld	[%i6+68],%o1

l00013F48:
	sethi	0000005D,%l0
	ld	[%o1+4],%o0
	or	%l0,00000100,%l0
	ld	[%o0+%g0],%o2
	ld	[%o2+4],%o1
	call	shortpath
	or	%g0,00000000,%o0
	or	%g0,%o0,%l1
	call	identify_lineno
	or	%g0,%i1,%o0
	or	%g0,%o0,%o2
	sethi	0000005C,%o3
	or	%g0,%l0,%o0
	or	%g0,%l1,%o1
	call	notice
	or	%o3,00000088,%o3

l00013F88:
	call	output_up_to
	or	%g0,%i1,%o0
	ba,a	000140AC

l00013F94:
	ld	[%o0+28],%o1
00013F98                         D4 07 A0 44 7F FF FE F3         ...D....
00013FA0 90 10 00 19 80 A2 20 00 02 80 00 0B 01 00 00 00 ...... .........

l00013FB0:
	call	restore_pointers
	sethi	00000000,%g0
	sethi	000000A0,%o0
	ld	[%o0+336],%o1
	ld	[%i6-20],%o2
	sethi	0000005D,%o0
	call	notice
	or	%o0,000000D0,%o0
	ba,a	000140AC
	call	output_up_to
	or	%g0,%i1,%o0
00013FDC                                     D2 07 A0 44             ...D
00013FE0 A4 10 20 00 E2 02 60 28 40 00 4F 4D 90 10 00 11 .. ...`(@.OM....
00013FF0 B0 04 40 08 80 A4 40 18 1A 80 00 18 29 00 00 5D ..@...@.....)..]
00014000 27 00 00 9F D0 4C 40 00 80 A2 20 3B 02 80 00 07 '....L@... ;....
00014010 A0 10 00 11 A0 04 20 01 D0 4C 00 00 80 A2 20 3B ...... ..L.... ;
00014020 32 BF FF FE A0 04 20 01 7F FF FE 19 90 15 21 38 2..... .......!8
00014030 7F FF FE 17 D0 04 E3 40 92 04 7F FF 90 10 00 11 .......@........
00014040 7F FF FD F2 92 24 00 09 A2 04 20 02 80 A4 40 18 .....$.... ...@.
00014050 2A BF FF EE D0 4C 40 00 D0 4E 60 01 10 80 00 09 *....L@..N`.....
00014060 A0 06 60 01 D2 02 21 D0 A0 04 20 01 80 A4 00 09 ..`...!... .....
00014070 90 40 20 00 7F FF FD 9B 92 10 20 00 D0 4C 00 00 .@ ....... ..L..
00014080 80 A2 20 7B 02 80 00 05 80 A2 20 0A 32 BF FF F6 .. {...... .2...
00014090 11 00 00 A0 A4 10 20 01 80 A4 A0 00 12 80 00 04 ...... .........
000140A0 11 00 00 5D 7F FF FD FA 90 12 21 38             ...]......!8    

l000140AC:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; do_cleaning: 000140B4
do_cleaning proc
	save	%sp,FFFFFF90,%sp
	subcc	%i0,%i1,%g0
	bcc	000143BC
	or	%g0,00000000,%o5

l000140C4:
	ldub	[%i0+%g0],%o1

l000140C8:
	add	%o1,FFFFFFF8,%o0
	sll	%o0,00000018,%o0
	sra	%o0,00000018,%o2
	subcc	%o2,00000054,%g0
	bgu	000143A8
	or	%g0,%o1,%o3

l000140E0:
	sethi	00000050,%o0
	or	%o0,000003C4,%o0
	sll	%o2,00000002,%o1
	ld	[%o0+%o1],%o2
	jmpl	%o2,%g0,%g0
	sethi	00000000,%g0
000140F8                         D0 4E 20 01 80 A2 20 2A         .N ... *
00014100 12 80 00 AB 9A 10 20 01 90 10 20 20 D0 2E 20 01 ...... ...  .. .
00014110 D0 2E 00 00 B0 06 20 02 10 80 00 10 D0 4E 20 01 ...... ......N .
00014120 13 00 00 AD 92 12 60 11 D4 0A 00 09 80 8A A0 08 ......`.........
00014130 32 80 00 05 B0 06 20 01 90 10 20 20 D0 2E 00 00 2..... ...  ....
00014140 B0 06 20 01 80 A6 00 19 2A 80 00 04 D0 4E 20 01 .. .....*....N .
00014150 40 00 4E F0 01 00 00 00 80 A2 20 2F 32 BF FF F1 @.N....... /2...
00014160 D0 0E 00 00 D0 4E 00 00 80 A2 20 2A 32 BF FF ED .....N.... *2...
00014170 D0 0E 00 00 90 10 20 20 D0 2E 00 00 B0 06 20 01 ......  ...... .
00014180 10 80 00 8B D0 2E 00 00 80 A3 60 00 32 80 00 88 ..........`.2...
00014190 9A 10 20 01 D2 4E 20 01 90 10 20 20 80 A2 60 0A .. ..N ...  ..`.
000141A0 02 80 00 76 D0 2E 00 00 D0 0E 00 00 13 00 00 AD ...v............
000141B0 92 12 60 11 D4 0A 00 09 80 8A A0 08 32 80 00 05 ..`.........2...
000141C0 B0 06 20 01 90 10 20 20 D0 2E 00 00 B0 06 20 01 .. ...  ...... .
000141D0 80 A6 00 19 2A 80 00 04 D0 4E 20 01 40 00 4E CD ....*....N .@.N.
000141E0 01 00 00 00 80 A2 20 0A 32 BF FF F1 D0 0E 00 00 ...... .2.......
000141F0 D0 4E 00 00 80 A2 20 5C 22 BF FF ED D0 0E 00 00 .N.... \".......
00014200 10 80 00 5E 90 10 20 20 D0 4E 20 01 80 A2 20 27 ...^..  .N ... '
00014210 12 80 00 07 9A 10 20 01 91 2A E0 18 91 3A 20 18 ...... ..*...: .
00014220 80 A2 20 5C 12 80 00 55 90 10 20 20 D0 4E 00 00 .. \...U..  .N..
00014230 80 A2 20 5C 12 80 00 0A 19 00 00 AD D0 0E 20 01 .. \.......... .
00014240 92 13 20 11 D4 0A 00 09 80 8A A0 08 32 80 00 06 .. .........2...
00014250 D0 0E 00 00 90 10 20 20 D0 2E 20 01 D0 0E 00 00 ......  .. .....
00014260 92 13 20 11 D4 0A 00 09 80 8A A0 08 32 80 00 05 .. .........2...
00014270 B0 06 20 01 90 10 20 20 D0 2E 00 00 B0 06 20 01 .. ...  ...... .
00014280 80 A6 00 19 2A 80 00 04 D0 4E 20 01 40 00 4E A1 ....*....N .@.N.
00014290 01 00 00 00 80 A2 20 27 12 BF FF E6 D0 4E 00 00 ...... '.....N..
000142A0 80 A2 20 5C 02 BF FF E2 90 10 20 20 10 80 00 34 .. \......  ...4
000142B0 D0 2E 00 00 D0 4E 20 01 9A 10 20 01 80 A2 20 22 .....N ... ... "
000142C0 12 80 00 07 19 00 00 AD 91 2A E0 18 91 3A 20 18 .........*...: .
000142D0 80 A2 20 5C 12 80 00 23 90 0A E0 FF D0 4E 00 00 .. \...#.....N..
000142E0 80 A2 20 5C 32 80 00 0B D0 0E 00 00 D0 0E 20 01 .. \2......... .
000142F0 92 13 20 11 D4 0A 00 09 80 8A A0 08 32 80 00 06 .. .........2...
00014300 D0 0E 00 00 90 10 20 20 D0 2E 20 01 D0 0E 00 00 ......  .. .....
00014310 92 13 20 11 D4 0A 00 09 80 8A A0 08 32 80 00 05 .. .........2...
00014320 B0 06 20 01 90 10 20 20 D0 2E 00 00 B0 06 20 01 .. ...  ...... .
00014330 80 A6 00 19 2A 80 00 04 D0 4E 20 01 40 00 4E 75 ....*....N .@.Nu
00014340 01 00 00 00 80 A2 20 22 12 BF FF E6 D0 4E 00 00 ...... ".....N..
00014350 80 A2 20 5C 02 BF FF E4 D6 0E 00 00 90 0A E0 FF .. \............
00014360 92 13 20 11 D4 0A 00 09 80 8A A0 08 32 80 00 10 .. .........2...
00014370 B0 06 20 01 90 10 20 20 D0 2E 00 00 10 80 00 0C .. ...  ........
00014380 B0 06 20 01 D0 4E 20 01 80 A2 20 0A 32 80 00 08 .. ..N ... .2...
00014390 9A 10 20 01 90 10 20 20 10 80 00 05 D0 2E 00 00 .. ...  ........
000143A0 10 80 00 03 9A 10 20 00                         ...... .        

l000143A8:
	or	%g0,00000001,%o5
	add	%i0,00000001,%i0
	subcc	%i0,%i1,%g0
	bcs,a	000140C8
	ldub	[%i0+%g0],%o1

l000143BC:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0
000143C4             00 01 43 AC 00 01 43 AC 00 01 43 A0     ..C...C...C.
000143D0 00 01 43 AC 00 01 43 AC 00 01 43 AC 00 01 43 A8 ..C...C...C...C.
000143E0 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
000143F0 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
00014400 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
00014410 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
00014420 00 01 43 A8 00 01 43 AC 00 01 43 A8 00 01 42 B4 ..C...C...C...B.
00014430 00 01 41 88 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..A...C...C...C.
00014440 00 01 42 08 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..B...C...C...C.
00014450 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
00014460 00 01 40 F8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..@...C...C...C.
00014470 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
00014480 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
00014490 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
000144A0 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
000144B0 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
000144C0 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
000144D0 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
000144E0 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
000144F0 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
00014500 00 01 43 A8 00 01 43 A8 00 01 43 A8 00 01 43 A8 ..C...C...C...C.
00014510 00 01 43 A8 00 01 43 84                         ..C...C.        

;; careful_find_l_paren: 00014518
;;   Called from:
;;     00014680 (in scan_for_missed_items)
careful_find_l_paren proc
	save	%sp,FFFFFF90,%sp
	add	%i0,FFFFFFFF,%i0
	or	%g0,00000001,%l0
	sethi	000000A0,%l1
	ldsb	[%i0+%g0],%o0

l0001452C:
	subcc	%o0,00000028,%g0
	be	00014548
	subcc	%o0,00000029,%g0

l00014538:
	be,a	0001454C
	add	%l0,00000001,%l0

l00014540:
	ba	00014550
	ld	[%l1+448],%o0

l00014548:
	add	%l0,FFFFFFFF,%l0

l0001454C:
	ld	[%l1+448],%o0

l00014550:
	add	%i0,FFFFFFFF,%i0
	subcc	%i0,%o0,%g0
	subx	%g0,FFFFFFFF,%o0
	call	check_source
	or	%g0,00000000,%o1
	subcc	%l0,00000000,%g0
	bne,a	0001452C
	ldsb	[%i0+%g0],%o0

l00014570:
	jmpl	%i7,+00000008,%g0
	restore	%i0,00000001,%o0

;; scan_for_missed_items: 00014578
scan_for_missed_items proc
	save	%sp,FFFFFF88,%sp
	sethi	000000A0,%o0
	ld	[%o0+448],%o1
	sethi	000000A0,%o3
	ld	[%o3+464],%o0
	sethi	000000A0,%o4
	add	%o0,FFFFFFFD,%o0
	add	%o1,FFFFFFFF,%o2
	sethi	000000A0,%o3
	st	%o0,[%i6-20]
	st	%o2,[%o4+304]
	st	%i0,[%i6+68]
	subcc	%o1,%o0,%g0
	bcc	00014870
	st	%o1,[%o3+288]

l000145B4:
	ld	[%o3+288],%o3
	ldsb	[%o3+%g0],%o0
	subcc	%o0,00000029,%g0
	bne	00014854
	sethi	000000A0,%o1

l000145C8:
	sethi	000000A0,%o0
	st	%o3,[%o0+320]
	ldub	[%o3+1],%o2
	sethi	000000AD,%o0
	or	%o0,00000011,%o0
	ldub	[%o2+%o0],%o1
	andcc	%o1,00000008,%g0
	be	0001461C
	add	%o3,00000001,%l0

l000145EC:
	or	%g0,%o0,%l1
	ld	[%i6-20],%o0

l000145F4:
	add	%l0,00000001,%l0
	subcc	%l0,%o0,%g0
	ld	[%i6-20],%o1
	call	check_source
	addx	%g0,00000000,%o0
	ldub	[%l0+%g0],%o0
	ldub	[%o0+%l1],%o1
	andcc	%o1,00000008,%g0
	bne	000145F4
	ld	[%i6-20],%o0

l0001461C:
	sethi	000000A0,%o1
	add	%l0,FFFFFFFF,%o0
	st	%o0,[%o1+288]
	ldub	[%l0+%g0],%o2
	sethi	000000AD,%o0
	or	%o0,00000011,%o0
	ldub	[%o2+%o0],%o1
	andcc	%o1,00000003,%g0
	bne	0001464C
	subcc	%o2,0000007B,%g0

l00014644:
	bne	00014854
	sethi	000000A0,%o1

l0001464C:
	call	identify_lineno
	or	%g0,%l0,%o0
	st	%o0,[%i6-24]
	sethi	000000AC,%o0
	call	setjmp
	or	%o0,00000240,%o0
	subcc	%o0,00000000,%g0
	bne	00014854
	sethi	000000A0,%o1

l00014670:
	sethi	000000AD,%o0
	or	%g0,%o0,%l4
	or	%o0,00000011,%l3
	sethi	000000A0,%l1

l00014680:
	call	careful_find_l_paren
	ld	[%l1+320],%o0
	add	%o0,FFFFFFFF,%o1
	st	%o1,[%l1+320]
	ldub	[%o0-1],%o2
	ldub	[%o2+%l3],%o0
	andcc	%o0,00000008,%g0
	be	000146E8
	ld	[%l1+320],%o0

l000146A4:
	sethi	000000A0,%l0
	sethi	000000A0,%i0
	or	%l4,00000011,%l2
	ld	[%l0+320],%o2

l000146B4:
	ld	[%i0+304],%o1
	add	%o2,FFFFFFFF,%o2
	subcc	%o2,%o1,%g0
	subx	%g0,FFFFFFFF,%o0
	call	check_source
	st	%o2,[%l0+320]
	ld	[%l0+320],%o0
	ldub	[%o0+%g0],%o1
	ldub	[%o1+%l2],%o2
	andcc	%o2,00000008,%g0
	bne	000146B4
	ld	[%l0+320],%o2

l000146E4:
	ld	[%l1+320],%o0

l000146E8:
	ldsb	[%o0+%g0],%o1
	subcc	%o1,00000029,%g0
	be	00014680
	sethi	00000000,%g0

l000146F8:
	sethi	000000A0,%l0
	ld	[%l0+320],%o1
	call	is_id_char
	ldsb	[%o1+%g0],%o0
	subcc	%o0,00000000,%g0
	be	00014850
	ld	[%l0+320],%o0

l00014714:
	sethi	000000A0,%l1
	add	%o0,00000001,%l0
	ba	00014734
	or	%g0,%o0,%i0

l00014724:
	add	%i0,FFFFFFFF,%i0
	subcc	%i0,%o1,%g0
	call	check_source
	subx	%g0,FFFFFFFF,%o0

l00014734:
	call	is_id_char
	ldsb	[%i0+%g0],%o0
	subcc	%o0,00000000,%g0
	bne	00014724
	ld	[%l1+304],%o1

l00014748:
	add	%i0,00000001,%i0
	sethi	000000A0,%o0
	subcc	%l0,%i0,%l0
	be	00014850
	st	%i0,[%o0+304]

l0001475C:
	add	%l0,0000006B,%o0
	and	%o0,FFFFFFF8,%o0
	sub	%sp,%o0,%sp
	add	%sp,00000060,%l2
	or	%g0,%i0,%o1
	or	%g0,%l0,%o2
	call	strncpy
	or	%g0,%l2,%o0
	sethi	0000005D,%o1
	ld	[%o1+316],%o0
	stb	%g0,[%l2+%l0]
	subcc	%o0,00000000,%g0
	be	000147BC
	or	%o1,0000013C,%l0

l00014794:
	ld	[%l0+%g0],%o1

l00014798:
	call	strcmp
	or	%g0,%l2,%o0
	subcc	%o0,00000000,%g0
	be	00014850
	add	%l0,00000004,%l0

l000147AC:
	ld	[%l0+%g0],%o0
	subcc	%o0,00000000,%g0
	bne,a	00014798
	ld	[%l0+%g0],%o1

l000147BC:
	ld	[%i6+68],%o2
	ld	[%o2+4],%o1
	subcc	%o1,00000000,%g0
	be,a	00014804
	ld	[%i6+68],%o1

l000147D0:
	ld	[%o1+20],%o0

l000147D4:
	subcc	%o0,00000000,%g0
	be	000147F0
	ld	[%i6-24],%o2

l000147E0:
	ld	[%o1+8],%o0
	subcc	%o0,%o2,%g0
	be,a	00014854
	sethi	000000A0,%o1

l000147F0:
	ld	[%o1+%g0],%o1
	subcc	%o1,00000000,%g0
	bne,a	000147D4
	ld	[%o1+20],%o0

l00014800:
	ld	[%i6+68],%o1

l00014804:
	sethi	0000005D,%l0
	ld	[%o1+%g0],%o0
	or	%l0,000001A0,%l0
	ld	[%o0+4],%o1
	call	shortpath
	or	%g0,00000000,%o0
	or	%g0,%o0,%l1
	call	identify_lineno
	or	%g0,%i0,%o0
	or	%g0,%o0,%o2
	or	%g0,%l0,%o0
	or	%g0,%l1,%o1
	call	notice
	or	%g0,%l2,%o3
	sethi	000000A0,%o0
	ld	[%o0+336],%o1
	sethi	0000005D,%o0
	call	notice
	or	%o0,000001D8,%o0

l00014850:
	sethi	000000A0,%o1

l00014854:
	ld	[%o1+288],%o0
	or	%g0,%o1,%o3
	ld	[%i6-20],%o2
	add	%o0,00000001,%o0
	subcc	%o0,%o2,%g0
	bcs	000145B4
	st	%o0,[%o1+288]

l00014870:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; edit_file: 00014878
edit_file proc
	save	%sp,FFFFFF08,%sp
	ld	[%i0+8],%i0
	call	needs_to_be_converted
	or	%g0,%i0,%o0
	subcc	%o0,00000000,%g0
	be	00014E1C
	sethi	000000A0,%o2

l00014894:
	ld	[%i0+%g0],%o0
	ld	[%o0+4],%o1
	or	%g0,%o2,%l5
	or	%g0,%o1,%o0
	call	directory_specified_p
	st	%o1,[%o2+384]
	subcc	%o0,00000000,%g0
	be	000148CC
	sethi	0000009F,%o0

l000148B8:
	call	file_excluded_p
	ld	[%l5+384],%o0
	subcc	%o0,00000000,%g0
	be	0001491C
	sethi	0000009F,%o0

l000148CC:
	ld	[%o0+812],%o1
	subcc	%o1,00000000,%g0
	bne	00014E1C
	sethi	00000000,%g0

l000148DC:
	call	in_system_include_dir
	ld	[%l5+384],%o0
	subcc	%o0,00000000,%g0
	bne	00014E1C
	sethi	000000A0,%o0

l000148F0:
	ld	[%o0+336],%l1
	sethi	0000005D,%l0
	ld	[%l5+384],%o1
	or	%l0,00000200,%l0
	call	shortpath
	or	%g0,00000000,%o0
	or	%g0,%o0,%o2
	or	%g0,%l0,%o0
	call	notice
	or	%g0,%l1,%o1
	ba,a	00014E1C

l0001491C:
	sethi	0000009F,%o1
00014920 D0 02 63 30 80 A2 20 00 02 80 00 0E 23 00 00 A0 ..c0.. .....#...
00014930 E4 04 61 50 21 00 00 5D D2 05 61 80 A0 14 22 18 ..aP!..]..a...".
00014940 7F FF F6 03 90 10 20 00 94 10 00 08 90 10 00 10 ...... .........
00014950 7F FF F3 37 92 10 00 12 10 80 00 0E 2F 00 00 A0 ...7......../...
00014960 25 00 00 A0 E2 04 A1 50 21 00 00 5D D2 05 61 80 %......P!..]..a.
00014970 A0 14 22 38 7F FF F5 F6 90 10 20 00 94 10 00 08 .."8...... .....
00014980 90 10 00 10 7F FF F3 2A 92 10 00 11 2F 00 00 A0 .......*..../...
00014990 11 00 00 AD 40 00 4D 0F 90 12 22 40 D0 05 61 80 ....@.M..."@..a.
000149A0 40 00 4C E8 92 07 BF 68 80 A2 3F FF 12 80 00 08 @.L....h..?.....
000149B0 E4 07 BF 98 11 00 00 AD E2 02 23 60 21 00 00 5D ..........#`!..]
000149C0 D2 05 61 80 10 80 00 E7 A0 14 22 58 23 00 00 A0 ..a......."X#...
000149D0 A0 04 A0 02 7F FF F3 28 90 10 00 10 AC 10 00 08 .......(........
000149E0 13 00 00 A0 90 05 80 12 D0 22 61 B0 A8 10 00 09 ........."a.....
000149F0 90 10 00 10 7F FF F3 20 EC 24 61 A0 A6 10 00 08 ....... .$a.....
00014A00 A1 34 A0 02 13 00 00 A0 A0 04 80 10 90 04 FF FF .4..............
00014A10 B6 10 00 11 17 00 00 A0 15 00 00 A0 A0 24 30 00 .............$0.
00014A20 D0 22 61 E0 A2 04 C0 12 B4 10 00 0B B8 10 00 0A ."a.............
00014A30 E6 22 E1 C0 E2 22 A1 D0 7F FF F3 0F 90 04 20 02 ."..."........ .
00014A40 94 10 00 08 A0 02 80 10 A0 04 3F FF 19 00 00 A0 ..........?.....
00014A50 13 00 00 A0 9A 02 BF FF 17 00 00 A0 E0 22 62 00 ............."b.
00014A60 D4 23 21 F0 D0 05 61 80 B2 10 00 0C BA 10 00 0B .#!...a.........
00014A70 DA 22 E2 10 92 10 20 00 40 00 4C C4 94 10 21 24 .".... .@.L...!$
00014A80 A0 10 00 08 80 A4 3F FF 12 80 00 08 90 10 00 10 ......?.........
00014A90 11 00 00 AD E2 02 23 60 21 00 00 5D D2 05 61 80 ......#`!..]..a.
00014AA0 10 80 00 B0 A0 14 22 80 92 10 00 16 7F FF F3 5F ......"........_
00014AB0 94 10 00 12 80 A2 00 12 02 80 00 14 11 00 00 AD ................
00014AC0 E6 02 23 60 40 00 4C B4 90 10 00 10 D2 05 61 80 ..#`@.L.......a.
00014AD0 21 00 00 5D E4 05 E1 50 A0 14 22 B0 7F FF F5 9C !..]...P..".....
00014AE0 90 10 20 00 A2 10 00 08 7F FF F2 DE 90 10 00 13 .. .............
00014AF0 96 10 00 08 90 10 00 10 92 10 00 12 7F FF F2 CC ................
00014B00 94 10 00 11 30 80 00 C6 40 00 4C A3 90 10 00 10 ....0...@.L.....
00014B10 80 A4 A0 00 02 80 00 07 92 10 20 0A D0 05 21 B0 .......... ...!.
00014B20 D2 4A 3F FF 80 A2 60 0A 02 80 00 06 92 10 20 0A .J?...`....... .
00014B30 D2 2D 80 12 D0 05 21 B0 90 02 20 01 D0 25 21 B0 .-....!... ..%!.
00014B40 D2 06 E1 A0 90 10 00 13 D4 05 21 B0 40 00 4C 83 ..........!.@.L.
00014B50 94 22 80 09 92 10 00 11 7F FF FD 57 90 10 00 13 .".........W....
00014B60 7F FF FE 86 90 10 00 18 E0 06 20 04 15 00 00 A0 .......... .....
00014B70 D6 06 A1 C0 90 10 20 01 13 00 00 A0 D0 22 A2 30 ...... ......".0
00014B80 80 A4 20 00 02 80 00 13 D6 22 62 20 7F FF FA DD .. ......"b ....
00014B90 D0 04 20 08 92 10 00 08 D0 04 20 14 80 A2 20 00 .. ....... ... .
00014BA0 02 80 00 06 01 00 00 00 7F FF FC CD 90 10 00 10 ................
00014BB0 10 80 00 05 E0 04 00 00 7F FF FB 57 90 10 00 10 ...........W....
00014BC0 E0 04 00 00 80 A4 20 00 12 BF FF F1 01 00 00 00 ...... .........
00014BD0 D0 07 21 D0 7F FF FB 36 90 02 3F FF 13 00 00 9F ..!....6..?.....
00014BE0 D0 02 63 30 80 A2 20 00 02 80 00 09 11 00 00 9F ..c0.. .........
00014BF0 40 00 4C 39 90 10 00 16 40 00 4C 37 90 10 00 13 @.L9....@.L7....
00014C00 40 00 4C 35 D0 06 61 F0 30 80 00 85 D2 02 23 34 @.L5..a.0.....#4
00014C10 80 A2 60 00 12 80 00 3D 01 00 00 00 40 00 4C 40 ..`....=....@.L@
00014C20 D0 05 61 80 7F FF F2 94 90 02 20 07 B0 10 00 08 ..a....... .....
00014C30 40 00 4C 2C D2 05 61 80 13 00 00 5B 92 12 63 08 @.L,..a....[..c.
00014C40 40 00 4C 4C 90 10 00 18 D0 05 61 80 40 00 4C 64 @.LL......a.@.Ld
00014C50 92 10 00 18 80 A2 3F FF 12 80 00 2C 11 00 00 AD ......?....,....
00014C60 E8 02 23 60 80 A5 20 11 02 80 00 15 21 00 00 5D ..#`.. .....!..]
00014C70 E6 05 E1 50 D2 05 61 80 A0 14 23 08 7F FF F5 34 ...P..a...#....4
00014C80 90 10 20 00 A4 10 00 08 92 10 00 18 7F FF F5 30 .. ............0
00014C90 90 10 20 00 A2 10 00 08 7F FF F2 72 90 10 00 14 .. ........r....
00014CA0 98 10 00 08 90 10 00 10 92 10 00 13 94 10 00 12 ................
00014CB0 7F FF F2 5F 96 10 00 11 30 80 00 59 11 00 00 9F ..._....0..Y....
00014CC0 D2 02 23 2C 80 A2 60 00 12 80 00 10 E4 05 E1 50 ..#,..`........P
00014CD0 21 00 00 5D D2 05 61 80 A0 14 22 D8 7F FF F5 1C !..]..a...".....
00014CE0 90 10 20 00 A2 10 00 08 92 10 00 18 7F FF F5 18 .. .............
00014CF0 90 10 20 00 96 10 00 08 90 10 00 10 92 10 00 12 .. .............
00014D00 7F FF F2 4B 94 10 00 11 40 00 4C 26 D0 05 61 80 ...K....@.L&..a.
00014D10 80 A2 3F FF 12 80 00 08 D0 05 61 80 11 00 00 AD ..?.......a.....
00014D20 E2 02 23 60 21 00 00 5D D2 05 61 80 10 80 00 0D ..#`!..]..a.....
00014D30 A0 14 20 20 40 00 4C 2D 92 10 21 B6 A0 10 00 08 ..  @.L-..!.....
00014D40 80 A4 3F FF 12 80 00 13 D2 06 61 F0 11 00 00 AD ..?.......a.....
00014D50 E2 02 23 60 21 00 00 5D D2 05 61 80 A0 14 23 30 ..#`!..]..a...#0
00014D60 E6 05 E1 50 7F FF F4 FA 90 10 20 00 A4 10 00 08 ...P...... .....
00014D70 7F FF F2 3C 90 10 00 11 96 10 00 08 90 10 00 10 ...<............
00014D80 92 10 00 13 7F FF F2 2A 94 10 00 12 30 80 00 24 .......*....0..$
00014D90 90 10 00 10 D8 07 62 10 94 02 7F FF D6 05 61 80 ......b.......a.
00014DA0 7F FF F2 BA 94 23 00 0A 40 00 4B FB 90 10 00 10 .....#..@.K.....
00014DB0 40 00 4B C9 90 10 00 16 40 00 4B C7 90 10 00 13 @.K.....@.K.....
00014DC0 40 00 4B C5 D0 06 61 F0 D0 05 61 80 40 00 4C 0A @.K...a...a.@.L.
00014DD0 D2 07 BF 7C 80 A2 3F FF 12 80 00 11 11 00 00 AD ...|..?.........
00014DE0 E2 02 23 60 21 00 00 5D D2 05 61 80 A0 14 23 60 ..#`!..]..a...#`
00014DF0 E6 05 E1 50 7F FF F4 D6 90 10 20 00 A4 10 00 08 ...P...... .....
00014E00 7F FF F2 18 90 10 00 11 96 10 00 08 90 10 00 10 ................
00014E10 92 10 00 13 7F FF F2 06 94 10 00 12             ............    

l00014E1C:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; do_processing: 00014E24
;;   Called from:
;;     000151E4 (in main)
do_processing proc
	save	%sp,FFFFFF90,%sp
	sethi	0000009F,%o0
	ld	[%o0+844],%o1
	sethi	000000A0,%o2
	ld	[%o2+352],%o0
	sll	%o1,00000002,%o1
	add	%o0,%o1,%l1
	or	%g0,%o0,%l0
	subcc	%l0,%l1,%g0
	bcc,a	00014E78
	sethi	000000A0,%l0

l00014E50:
	sethi	0000009F,%l2
	ld	[%l0+%g0],%o0

l00014E58:
	or	%g0,00000000,%o2
	ld	[%l2+824],%o1
	call	process_aux_info_file
	add	%l0,00000004,%l0
	subcc	%l0,%l1,%g0
	bcs,a	00014E58
	ld	[%l0+%g0],%o0

l00014E74:
	sethi	000000A0,%l0

l00014E78:
	or	%l0,00000240,%l0
	sethi	0000004D,%o1
	or	%o1,000001C0,%o1
	call	visit_each_hash_node
	or	%g0,%l0,%o0
	sethi	00000052,%o1
	or	%g0,%l0,%o0
	call	visit_each_hash_node
	or	%o1,00000078,%o1
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; main: 00014EA4
;;   Called from:
;;     000114F4 (in _start)
main proc
	save	%sp,FFFFFF88,%sp
	st	%i1,[%i6+72]
	ld	[%i1+%g0],%o0
	sethi	0000005C,%o1
	or	%o1,00000218,%o1
	st	%o1,[%i6-24]
	or	%g0,0000002F,%o1
	call	strrchr
	st	%i0,[%i6+68]
	sethi	000000A0,%o1
	add	%o0,00000001,%o2
	subcc	%o0,00000000,%g0
	bne	00014EE8
	st	%o0,[%o1+336]

l00014EDC:
	ld	[%i6+72],%o0
	ld	[%o0+%g0],%o2
	sethi	000000A0,%o1

l00014EE8:
	st	%o2,[%o1+336]
	ld	[%i6-24],%o1
	call	setlocale
	or	%g0,00000005,%o0
	call	getpwd
	sethi	00000000,%g0
	or	%g0,%o0,%o1
	sethi	000000AC,%o0
	subcc	%o1,00000000,%g0
	bne	00014F48
	st	%o1,[%o0+624]

l00014F14:
	sethi	000000AD,%o1
	sethi	000000A0,%o2
	ld	[%o2+336],%l1
	sethi	0000005E,%l0
	ld	[%o1+864],%o0
	call	xstrerror
	or	%l0,00000010,%l0
	or	%g0,%o0,%o2
	or	%g0,%l0,%o0
	call	notice
	or	%g0,%l1,%o1
	call	exit
00014F44             90 10 20 21                             .. !        

l00014F48:
	or	%g0,%o1,%o0
	call	string_list_cons
	or	%g0,00000000,%o1
	sethi	000000AD,%o1
	st	%o0,[%o1+868]
	or	%g0,%o1,%l5
	sethi	000000A0,%l1
	sethi	000000AD,%l4
	sethi	0000009F,%i5
	sethi	0000009F,%i4
	sethi	0000009F,%l3
	sethi	0000009F,%i3
	sethi	0000009F,%i2
	sethi	0000005E,%i1
	sethi	0000009F,%i0
	add	%i6,FFFFFFEC,%l2
	sethi	0000009F,%l6
	ba	00015074
	sethi	000000A0,%l7

l00014F94:
	bne	00014FB0
	add	%o0,FFFFFFB2,%o0

l00014F9C:
	ld	[%i6-20],%o0
	add	%l0,0000000C,%o1
	sll	%o0,00000004,%o0
	ld	[%o1+%o0],%o0
	add	%o0,FFFFFFB2,%o0

l00014FB0:
	subcc	%o0,0000002A,%g0
	bgu	0001506C
	sethi	00000054,%o3

l00014FBC:
	sll	%o0,00000002,%o0
	or	%o3,0000020C,%o1
	ld	[%o1+%o0],%o2
	jmpl	%o2,%g0,%g0
	sethi	00000000,%g0
00014FD0 D0 04 60 64 19 00 00 9F 10 80 00 27 D0 23 23 24 ..`d.......'.##$
00014FE0 D2 04 60 64 7F FF F3 D4 90 10 20 00 7F FF F2 F7 ..`d...... .....
00014FF0 D2 05 63 64 10 80 00 20 D0 25 63 64 D0 04 60 64 ..cd... .%cd..`d
00015000 7F FF F2 F2 D2 05 20 08 10 80 00 1B D0 25 20 08 ...... ......% .
00015010 90 10 20 01 13 00 00 9F 10 80 00 17 D0 22 63 28 .. .........."c(
00015020 90 10 20 01 10 80 00 14 D0 27 63 2C 90 10 20 01 .. ......'c,.. .
00015030 D0 24 E3 38 10 80 00 10 D0 27 23 30 90 10 20 01 .$.8.....'#0.. .
00015040 10 80 00 0D D0 26 E3 34 90 10 20 01 10 80 00 0A .....&.4.. .....
00015050 D0 24 E3 38 D4 04 60 64 10 80 00 07 D4 27 BF E8 .$.8..`d.....'..
00015060 D0 04 60 64 10 80 00 04 D0 26 A3 40             ..`d.....&.@    

l0001506C:
	call	usage
	sethi	00000000,%g0

l00015074:
	or	%i0,00000358,%l0
	ld	[%i6+68],%o0
	or	%i1,00000038,%o2
	ld	[%i6+72],%o1
	or	%g0,%l0,%o3
	call	getopt_long
	or	%g0,%l2,%o4
	subcc	%o0,FFFFFFFF,%g0
	bne	00014F94
	subcc	%o0,00000000,%g0

l0001509C:
	call	munge_compile_params
	ld	[%i6-24],%o0
	ld	[%l7+104],%o1
	ld	[%i6+68],%o3
	sub	%o3,%o1,%o1
	add	%o1,00000001,%o0
	st	%o1,[%l6+844]
	call	xmalloc
	sll	%o0,00000002,%o0
	sethi	000000A0,%o4
	st	%o0,[%o4+352]
	ld	[%l7+104],%o1
	st	%g0,[%l6+844]
	ld	[%i6+68],%o0
	subcc	%o1,%o0,%g0
	bge	0001519C
	sethi	0000009F,%o1

l000150E0:
	or	%g0,%l6,%l4
	sethi	0000009F,%l3
	or	%g0,%l7,%l2

l000150EC:
	ld	[%l7+104],%o2
	or	%g0,00000000,%o0
	ld	[%i6+72],%o3
	sll	%o2,00000002,%o2
	call	abspath
	ld	[%o3+%o2],%o1
	call	strlen
	or	%g0,%o0,%l1
	add	%o0,%l1,%o1
	ldsb	[%o1-1],%o0
	subcc	%o0,00000063,%g0
	bne	00015150
	sethi	000000A0,%o0

l00015120:
	ldsb	[%o1-2],%o0
	subcc	%o0,0000002E,%g0
	bne	00015150
	sethi	000000A0,%o0

l00015130:
	ld	[%l4+844],%o0
	sethi	000000A0,%o4
	ld	[%o4+352],%o2
	sll	%o0,00000002,%o1
	st	%l1,[%o2+%o1]
	add	%o0,00000001,%o0
	ba	00015180
	st	%o0,[%l4+844]

l00015150:
	ld	[%o0+336],%l0
	or	%g0,%l1,%o1
	call	shortpath
	or	%g0,00000000,%o0
	or	%g0,%o0,%o2
	sethi	0000005E,%o3
	or	%o3,00000050,%o0
	call	notice
	or	%g0,%l0,%o1
	ld	[%l3+800],%o0
	add	%o0,00000001,%o0
	st	%o0,[%l3+800]

l00015180:
	ld	[%l2+104],%o0
	ld	[%i6+68],%o4
	add	%o0,00000001,%o0
	subcc	%o0,%o4,%g0
	bl	000150EC
	st	%o0,[%l2+104]

l00015198:
	sethi	0000009F,%o1

l0001519C:
	ld	[%o1+800],%o0
	subcc	%o0,00000000,%g0
	be	000151B4
	sethi	0000009F,%o2

l000151AC:
	call	usage
	sethi	00000000,%g0

l000151B4:
	ld	[%o2+808],%o0
	subcc	%o0,00000000,%g0
	be	000151E4
	sethi	000000A0,%o3

l000151C4:
	ld	[%o3+336],%o2
	sethi	000000AD,%o4
	sethi	000000A0,%o1
	or	%o4,00000240,%o0
	ld	[%o1+40],%o3
	sethi	0000005E,%o4
	call	fprintf
	or	%o4,00000080,%o1

l000151E4:
	call	do_processing
	sethi	00000000,%g0
	sethi	0000009F,%o1
	ld	[%o1+800],%o0
	subcc	%o0,00000000,%g0
	be	00015204
	or	%g0,00000000,%o1

l00015200:
	or	%g0,00000021,%o1

l00015204:
	call	exit
00015208                         90 10 00 09 00 01 50 3C         ......P<
00015210 00 01 50 6C 00 01 50 6C 00 01 50 6C 00 01 50 6C ..Pl..Pl..Pl..Pl
00015220 00 01 50 6C 00 01 50 6C 00 01 50 6C 00 01 50 10 ..Pl..Pl..Pl..P.
00015230 00 01 50 6C 00 01 50 6C 00 01 50 6C 00 01 50 6C ..Pl..Pl..Pl..Pl
00015240 00 01 50 6C 00 01 50 6C 00 01 50 6C 00 01 50 6C ..Pl..Pl..Pl..Pl
00015250 00 01 50 6C 00 01 50 6C 00 01 50 6C 00 01 50 6C ..Pl..Pl..Pl..Pl
00015260 00 01 50 54 00 01 4F E0 00 01 50 6C 00 01 50 6C ..PT..O...Pl..Pl
00015270 00 01 50 6C 00 01 50 6C 00 01 50 60 00 01 50 6C ..Pl..Pl..P`..Pl
00015280 00 01 50 48 00 01 50 6C 00 01 50 6C 00 01 50 2C ..PH..Pl..Pl..P,
00015290 00 01 50 6C 00 01 4F D0 00 01 50 20 00 01 50 6C ..Pl..O...P ..Pl
000152A0 00 01 50 6C 00 01 50 6C 00 01 50 6C 00 01 50 10 ..Pl..Pl..Pl..P.
000152B0 00 01 50 6C 00 01 4F FC                         ..Pl..O.        

;; getpwd: 000152B8
;;   Called from:
;;     00014EF8 (in main)
getpwd proc
	save	%sp,FFFFFE80,%sp
	sethi	000000AC,%o0
	ld	[%o0+672],%i0
	subcc	%i0,00000000,%g0
	bne	000153BC
	or	%g0,%o0,%l4

l000152D0:
	sethi	000000AC,%o0
	ld	[%o0+688],%o2
	sethi	000000AD,%o1
	or	%g0,%o0,%l3
	or	%g0,%o1,%l2
	subcc	%o2,00000000,%g0
	bne	000153BC
	st	%o2,[%o1+864]

l000152F0:
	sethi	0000005E,%o0
	call	getenv
	or	%o0,00000088,%o0
	orcc	%o0,00000000,%i0
	be	0001537C
	or	%g0,00000064,%l1

l00015308:
	ldsb	[%i0+%g0],%o0
	subcc	%o0,0000002F,%g0
	bne	0001537C
	or	%g0,%i0,%o0

l00015318:
	call	stat
	add	%i6,FFFFFEE0,%o1
	subcc	%o0,00000000,%g0
	bne,a	0001537C
	or	%g0,00000064,%l1

l0001532C:
	sethi	0000005E,%o0
	or	%o0,00000090,%o0
	call	stat
	add	%i6,FFFFFF68,%o1
	subcc	%o0,00000000,%g0
	bne,a	0001537C
	or	%g0,00000064,%l1

l00015348:
	ld	[%i6-136],%o1
	ld	[%i6-272],%o0
	subcc	%o1,%o0,%g0
	bne,a	0001537C
	or	%g0,00000064,%l1

l0001535C:
	ld	[%i6-152],%o1
	ld	[%i6-288],%o0
	subcc	%o1,%o0,%g0
	be,a	000153BC
	st	%i0,[%l4+672]

l00015370:
	ba	0001537C
	or	%g0,00000064,%l1

l00015378:
	sll	%l1,00000001,%l1

l0001537C:
	call	xmalloc
	or	%g0,%l1,%o0
	or	%g0,%o0,%i0
	call	getcwd
	or	%g0,%l1,%o1
	subcc	%o0,00000000,%g0
	bne	000153B8
	ld	[%l2+864],%l0

l0001539C:
	call	free
	or	%g0,%i0,%o0
	subcc	%l0,00000022,%g0
	be	00015378
	or	%g0,00000000,%i0

l000153B0:
	st	%l0,[%l2+864]
	st	%l0,[%l3+688]

l000153B8:
	st	%i0,[%l4+672]

l000153BC:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; _obstack_begin: 000153C4
_obstack_begin proc
	save	%sp,FFFFFF90,%sp
	subcc	%i2,00000000,%g0
	be,a	000153D4
	or	%g0,00000008,%i2

l000153D4:
	subcc	%i1,00000000,%g0
	be,a	000153E0
	or	%g0,00000FE8,%i1

l000153E0:
	ld	[%i0+40],%o0
	sethi	00200000,%o1
	andn	%o0,%o1,%o1
	add	%i2,FFFFFFFF,%o2
	st	%i4,[%i0+32]
	st	%o2,[%i0+24]
	subcc	%o1,00000000,%g0
	st	%i3,[%i0+28]
	st	%i1,[%i0+%g0]
	bge	00015420
	st	%o1,[%i0+40]

l0001540C:
	ld	[%i0+36],%o0
	jmpl	%i3,%g0,%o7
	or	%g0,%i1,%o1
	ba	0001542C
	or	%g0,%o0,%i1

l00015420:
	jmpl	%i3,%g0,%o7
	or	%g0,%i1,%o0
	or	%g0,%o0,%i1

l0001542C:
	subcc	%i1,00000000,%g0
	bne	00015448
	st	%i1,[%i0+4]

l00015438:
	sethi	000000A0,%o0
	ld	[%o0+44],%o1
	jmpl	%o1,%g0,%o7
	sethi	00000000,%g0

l00015448:
	ld	[%i0+%g0],%o0
	add	%i1,00000008,%o1
	st	%o1,[%i0+12]
	st	%o1,[%i0+8]
	add	%i1,%o0,%o0
	st	%o0,[%i1+%g0]
	st	%o0,[%i0+16]
	st	%g0,[%i1+4]
	ld	[%i0+40],%o2
	sethi	00100000,%o1
	andn	%o2,%o1,%o1
	sethi	00080000,%o0
	andn	%o1,%o0,%o0
	st	%o0,[%i0+40]
	jmpl	%i7,+00000008,%g0
	restore	%g0,00000001,%o0

;; _obstack_begin_1: 00015488
_obstack_begin_1 proc
	save	%sp,FFFFFF90,%sp
	subcc	%i2,00000000,%g0
	be,a	00015498
	or	%g0,00000008,%i2

l00015498:
	subcc	%i1,00000000,%g0
	be,a	000154A4
	or	%g0,00000FE8,%i1

l000154A4:
	ld	[%i0+40],%o0
	sethi	00200000,%o1
	or	%o0,%o1,%o0
	add	%i2,FFFFFFFF,%o2
	st	%i4,[%i0+32]
	st	%o2,[%i0+24]
	subcc	%o0,00000000,%g0
	st	%i3,[%i0+28]
	st	%i1,[%i0+%g0]
	st	%i5,[%i0+36]
	bge	000154E8
	st	%o0,[%i0+40]

l000154D4:
	or	%g0,%i1,%o1
	jmpl	%i3,%g0,%o7
	or	%g0,%i5,%o0
	ba	000154F4
	or	%g0,%o0,%i1

l000154E8:
	jmpl	%i3,%g0,%o7
	or	%g0,%i1,%o0
	or	%g0,%o0,%i1

l000154F4:
	subcc	%i1,00000000,%g0
	bne	00015510
	st	%i1,[%i0+4]

l00015500:
	sethi	000000A0,%o0
	ld	[%o0+44],%o1
	jmpl	%o1,%g0,%o7
	sethi	00000000,%g0

l00015510:
	ld	[%i0+%g0],%o0
	add	%i1,00000008,%o1
	st	%o1,[%i0+12]
	st	%o1,[%i0+8]
	add	%i1,%o0,%o0
	st	%o0,[%i1+%g0]
	st	%o0,[%i0+16]
	st	%g0,[%i1+4]
	ld	[%i0+40],%o2
	sethi	00100000,%o1
	andn	%o2,%o1,%o1
	sethi	00080000,%o0
	andn	%o1,%o0,%o0
	st	%o0,[%i0+40]
	jmpl	%i7,+00000008,%g0
	restore	%g0,00000001,%o0

;; _obstack_newchunk: 00015550
_obstack_newchunk proc
	save	%sp,FFFFFF90,%sp
	ld	[%i0+8],%o0
	ld	[%i0+12],%o1
	sub	%o1,%o0,%l2
	add	%l2,%i1,%i1
	sra	%l2,00000003,%o0
	ld	[%i0+%g0],%o2
	add	%i1,%o0,%i1
	add	%i1,00000064,%i1
	subcc	%i1,%o2,%g0
	bge	00015584
	ld	[%i0+4],%l1

l00015580:
	or	%g0,%o2,%i1

l00015584:
	ld	[%i0+40],%o0
	subcc	%o0,00000000,%g0
	bge,a	000155AC
	ld	[%i0+28],%o1

l00015594:
	ld	[%i0+28],%o2
	or	%g0,%i1,%o1
	jmpl	%o2,%g0,%o7
	ld	[%i0+36],%o0
	ba	000155B8
	or	%g0,%o0,%l0

l000155AC:
	jmpl	%o1,%g0,%o7
	or	%g0,%i1,%o0
	or	%g0,%o0,%l0

l000155B8:
	subcc	%l0,00000000,%g0
	bne,a	000155D8
	st	%l0,[%i0+4]

l000155C4:
	sethi	000000A0,%o0
	ld	[%o0+44],%o1
	jmpl	%o1,%g0,%o7
	sethi	00000000,%g0
	st	%l0,[%i0+4]

l000155D8:
	add	%l0,%i1,%o1
	st	%l1,[%l0+4]
	st	%o1,[%i0+16]
	st	%o1,[%l0+%g0]
	ld	[%i0+24],%o0
	add	%o0,00000001,%o0
	subcc	%o0,00000007,%g0
	ble	00015654
	srl	%l2,00000002,%o0

l000155FC:
	or	%g0,%o0,%o5
	add	%l0,00000008,%i1
	addcc	%o0,FFFFFFFF,%o3
	bneg	00015638
	add	%l1,00000008,%g2

l00015610:
	sll	%o3,00000002,%o0
	add	%o0,00000008,%o0
	add	%o0,%l0,%o4

l0001561C:
	ld	[%i0+8],%o0
	sll	%o3,00000002,%o1
	ld	[%o0+%o1],%o2
	addcc	%o3,FFFFFFFF,%o3
	st	%o2,[%o4+%g0]
	bpos	0001561C
	add	%o4,FFFFFFFC,%o4

l00015638:
	ba	00015660
	sll	%o5,00000002,%o0

l00015640:
	or	%g0,%l1,%o1
	jmpl	%o2,%g0,%o7
	ld	[%i0+36],%o0
	ba	000156D0
	ld	[%i0+40],%o0

l00015654:
	or	%g0,00000000,%o0
	add	%l0,00000008,%i1
	add	%l1,00000008,%g2

l00015660:
	or	%g0,%o0,%o3
	subcc	%o3,%l2,%g0
	bge	0001568C
	ld	[%i0+8],%o0

l00015670:
	or	%g0,%i1,%o2

l00015674:
	ldub	[%o0+%o3],%o1
	stb	%o1,[%o2+%o3]
	add	%o3,00000001,%o3
	subcc	%o3,%l2,%g0
	bl	00015674
	ld	[%i0+8],%o0

l0001568C:
	subcc	%o0,%g2,%g0
	bne	000156D0
	ld	[%i0+40],%o0

l00015698:
	srl	%o0,0000001E,%o0
	andcc	%o0,00000001,%g0
	bne,a	000156D0
	ld	[%i0+40],%o0

l000156A8:
	ld	[%l1+4],%o1
	st	%o1,[%l0+4]
	ld	[%i0+40],%o0
	subcc	%o0,00000000,%g0
	bl,a	00015640
	ld	[%i0+32],%o2

l000156C0:
	ld	[%i0+32],%o1
	jmpl	%o1,%g0,%o7
	or	%g0,%l1,%o0
	ld	[%i0+40],%o0

l000156D0:
	sethi	00100000,%o1
	add	%i1,%l2,%o2
	andn	%o0,%o1,%o1
	st	%o1,[%i0+40]
	st	%o2,[%i0+12]
	st	%i1,[%i0+8]
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; _obstack_allocated_p: 000156F0
_obstack_allocated_p proc
	ld	[%o0+4],%g3

l000156F4:
	subcc	%g3,00000000,%g0
	be	00015718
	subcc	%g3,%o1,%g0

l00015700:
	bcc,a	000156F4
	ld	[%g3+4],%g3

l00015708:
	ld	[%g3+%g0],%g2
	subcc	%g2,%o1,%g0
	bcs,a	000156F4
	ld	[%g3+4],%g3

l00015718:
	subcc	%g0,%g3,%g0
	jmpl	%o7,+00000008,%g0
	addx	%g0,00000000,%o0

;; _obstack_free: 00015724
_obstack_free proc
	save	%sp,FFFFFF90,%sp
	ba	00015774
	ld	[%i0+4],%o3

l00015730:
	subcc	%o0,00000000,%g0
	bge	00015754
	ld	[%o3+4],%l0

l0001573C:
	ld	[%i0+32],%o2
	or	%g0,%o3,%o1
	jmpl	%o2,%g0,%o7
	ld	[%i0+36],%o0
	ba	00015764
	ld	[%i0+40],%o1

l00015754:
	ld	[%i0+32],%o1
	jmpl	%o1,%g0,%o7
	or	%g0,%o3,%o0
	ld	[%i0+40],%o1

l00015764:
	sethi	00100000,%o0
	or	%o1,%o0,%o1
	or	%g0,%l0,%o3
	st	%o1,[%i0+40]

l00015774:
	subcc	%o3,00000000,%g0
	be	00015798
	subcc	%o3,%i1,%g0

l00015780:
	bcc,a	00015730
	ld	[%i0+40],%o0

l00015788:
	ld	[%o3+%g0],%o0
	subcc	%o0,%i1,%g0
	bcs,a	00015730
	ld	[%i0+40],%o0

l00015798:
	subcc	%o3,00000000,%g0
	be	000157BC
	subcc	%i1,00000000,%g0

l000157A4:
	st	%i1,[%i0+8]
	st	%i1,[%i0+12]
	ld	[%o3+%g0],%o0
	st	%o3,[%i0+4]
	ba	000157CC
	st	%o0,[%i0+16]

l000157BC:
	be	000157CC
	sethi	00000000,%g0

l000157C4:
	call	abort
000157C8                         01 00 00 00                     ....    

l000157CC:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; obstack_free: 000157D4
obstack_free proc
	save	%sp,FFFFFF90,%sp
	ba	00015824
	ld	[%i0+4],%o3

l000157E0:
	subcc	%o0,00000000,%g0
	bge	00015804
	ld	[%o3+4],%l0

l000157EC:
	ld	[%i0+32],%o2
	or	%g0,%o3,%o1
	jmpl	%o2,%g0,%o7
	ld	[%i0+36],%o0
	ba	00015814
	ld	[%i0+40],%o1

l00015804:
	ld	[%i0+32],%o1
	jmpl	%o1,%g0,%o7
	or	%g0,%o3,%o0
	ld	[%i0+40],%o1

l00015814:
	sethi	00100000,%o0
	or	%o1,%o0,%o1
	or	%g0,%l0,%o3
	st	%o1,[%i0+40]

l00015824:
	subcc	%o3,00000000,%g0
	be	00015848
	subcc	%o3,%i1,%g0

l00015830:
	bcc,a	000157E0
	ld	[%i0+40],%o0

l00015838:
	ld	[%o3+%g0],%o0
	subcc	%o0,%i1,%g0
	bcs,a	000157E0
	ld	[%i0+40],%o0

l00015848:
	subcc	%o3,00000000,%g0
	be	0001586C
	subcc	%i1,00000000,%g0

l00015854:
	st	%i1,[%i0+8]
	st	%i1,[%i0+12]
	ld	[%o3+%g0],%o0
	st	%o3,[%i0+4]
	ba	0001587C
	st	%o0,[%i0+16]

l0001586C:
	be	0001587C
	sethi	00000000,%g0

l00015874:
	call	abort
00015878                         01 00 00 00                     ....    

l0001587C:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; _obstack_memory_used: 00015884
_obstack_memory_used proc
	ld	[%o0+4],%g3
	subcc	%g3,00000000,%g0
	be	000158AC
	or	%g0,00000000,%o0

l00015894:
	ld	[%g3+%g0],%g2
	sub	%g2,%g3,%g2
	ld	[%g3+4],%g3
	subcc	%g3,00000000,%g0
	bne	00015894
	add	%o0,%g2,%o0

l000158AC:
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0

;; print_and_abort: 000158B4
print_and_abort proc
	save	%sp,FFFFFF90,%sp
	sethi	0000005E,%o0
	sethi	000000AD,%o1
	or	%o1,00000240,%o1
	call	fputs
	or	%o0,000000D0,%o0
	sethi	000000A0,%o1
	call	exit
000158D4             D0 02 60 30                             ..`0        

;; try: 000158D8
;;   Called from:
;;     00015928 (in choose_temp_base)
;;     00015940 (in choose_temp_base)
;;     00015958 (in choose_temp_base)
;;     0001596C (in choose_temp_base)
;;     00015980 (in choose_temp_base)
;;     00015994 (in choose_temp_base)
;;     00015A90 (in make_temp_file)
;;     00015AA8 (in make_temp_file)
;;     00015AC0 (in make_temp_file)
;;     00015AD4 (in make_temp_file)
;;     00015AE8 (in make_temp_file)
;;     00015AFC (in make_temp_file)
try proc
	save	%sp,FFFFFF90,%sp
	subcc	%i1,00000000,%g0
	be	000158F0
	subcc	%i0,00000000,%g0

l000158E8:
	ba	00015910
	or	%g0,%i1,%i0

l000158F0:
	be	0001590C
	or	%g0,%i0,%o0

l000158F8:
	call	access
	or	%g0,00000007,%o1
	subcc	%o0,00000000,%g0
	be	00015910
	sethi	00000000,%g0

l0001590C:
	or	%g0,00000000,%i0

l00015910:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; choose_temp_base: 00015918
;;   Called from:
;;     00012ED8 (in gen_aux_info_file)
choose_temp_base proc
	save	%sp,FFFFFF90,%sp
	sethi	0000005E,%o0
	call	getenv
	or	%o0,000000E8,%o0
	call	try
	or	%g0,00000000,%o1
	or	%g0,%o0,%l0
	sethi	0000005E,%o0
	call	getenv
	or	%o0,000000F0,%o0
	call	try
	or	%g0,%l0,%o1
	or	%g0,%o0,%l0
	sethi	0000005E,%o0
	call	getenv
	or	%o0,000000F8,%o0
	call	try
	or	%g0,%l0,%o1
	or	%g0,%o0,%l0
	sethi	0000005E,%o0
	or	%g0,%l0,%o1
	call	try
	or	%o0,00000100,%o0
	or	%g0,%o0,%l0
	sethi	000000A0,%o0
	or	%g0,%l0,%o1
	call	try
	or	%o0,00000040,%o0
	or	%g0,%o0,%l0
	sethi	000000A0,%o0
	or	%g0,%l0,%o1
	call	try
	or	%o0,00000038,%o0
	orcc	%o0,00000000,%l0
	bne	000159B0
	sethi	00000000,%g0

l000159A8:
	sethi	0000005E,%o0
	or	%o0,00000110,%l0

l000159B0:
	call	strlen
	or	%g0,%l0,%o0
	or	%g0,%o0,%l1
	call	xmalloc
	add	%l1,0000000A,%o0
	or	%g0,%o0,%i0
	call	strcpy
	or	%g0,%l0,%o1
	subcc	%l1,00000000,%g0
	be	00015A00
	sethi	0000005E,%o3

l000159DC:
	add	%l1,%i0,%o0
	ldsb	[%o0-1],%o1
	subcc	%o1,0000002F,%g0
	be,a	00015A04
	ldub	[%o3+280],%o0

l000159F0:
	or	%g0,0000002F,%o0
	stb	%o0,[%l1+%i0]
	add	%l1,00000001,%l1
	sethi	0000005E,%o3

l00015A00:
	ldub	[%o3+280],%o0

l00015A04:
	add	%i0,%l1,%o4
	stb	%o0,[%i0+%l1]
	or	%o3,00000118,%o3
	ldub	[%o3+1],%o1
	or	%g0,%i0,%o0
	stb	%o1,[%o4+1]
	ldub	[%o3+2],%o2
	stb	%o2,[%o4+2]
	ldub	[%o3+3],%o1
	stb	%o1,[%o4+3]
	ldub	[%o3+4],%o2
	stb	%o2,[%o4+4]
	ldub	[%o3+5],%o1
	stb	%o1,[%o4+5]
	ldub	[%o3+6],%o2
	stb	%o2,[%o4+6]
	ldub	[%o3+7],%o1
	stb	%o1,[%o4+7]
	ldub	[%o3+8],%o2
	call	mktemp
	stb	%o2,[%o4+8]
	call	strlen
	or	%g0,%i0,%o0
	subcc	%o0,00000000,%g0
	bne	00015A74
	sethi	00000000,%g0

l00015A6C:
	call	abort
00015A70 01 00 00 00                                     ....            

l00015A74:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; make_temp_file: 00015A7C
make_temp_file proc
	save	%sp,FFFFFF90,%sp
	sethi	0000005E,%o0
	or	%g0,%i0,%l3
	call	getenv
	or	%o0,000000E8,%o0
	call	try
	or	%g0,00000000,%o1
	or	%g0,%o0,%l0
	sethi	0000005E,%o0
	call	getenv
	or	%o0,000000F0,%o0
	call	try
	or	%g0,%l0,%o1
	or	%g0,%o0,%l0
	sethi	0000005E,%o0
	call	getenv
	or	%o0,000000F8,%o0
	call	try
	or	%g0,%l0,%o1
	or	%g0,%o0,%l0
	sethi	0000005E,%o0
	or	%g0,%l0,%o1
	call	try
	or	%o0,00000100,%o0
	or	%g0,%o0,%l0
	sethi	000000A0,%o0
	or	%g0,%l0,%o1
	call	try
	or	%o0,00000058,%o0
	or	%g0,%o0,%l0
	sethi	000000A0,%o0
	or	%g0,%l0,%o1
	call	try
	or	%o0,00000050,%o0
	orcc	%o0,00000000,%l0
	bne	00015B14
	sethi	0000005E,%o0

l00015B10:
	or	%o0,00000110,%l0

l00015B14:
	call	strlen
	or	%g0,%l0,%o0
	subcc	%l3,00000000,%g0
	be	00015B38
	or	%g0,%o0,%l1

l00015B28:
	call	strlen
	or	%g0,%l3,%o0
	ba	00015B3C
	or	%g0,%o0,%l2

l00015B38:
	or	%g0,00000000,%l2

l00015B3C:
	add	%l1,%l2,%o0
	call	xmalloc
	add	%o0,0000000A,%o0
	or	%g0,%o0,%i0
	call	strcpy
	or	%g0,%l0,%o1
	subcc	%l1,00000000,%g0
	be	00015B78
	add	%l1,%i0,%o0

l00015B60:
	ldsb	[%o0-1],%o1
	subcc	%o1,0000002F,%g0
	be	00015B78
	or	%g0,0000002F,%o0

l00015B70:
	stb	%o0,[%l1+%i0]
	add	%l1,00000001,%l1

l00015B78:
	sethi	0000005E,%o2
	ldub	[%o2+280],%o0
	add	%i0,%l1,%o3
	stb	%o0,[%i0+%l1]
	or	%o2,00000118,%o2
	ldub	[%o2+1],%o1
	subcc	%l3,00000000,%g0
	stb	%o1,[%o3+1]
	ldub	[%o2+2],%o4
	stb	%o4,[%o3+2]
	ldub	[%o2+3],%o0
	stb	%o0,[%o3+3]
	ldub	[%o2+4],%o1
	stb	%o1,[%o3+4]
	ldub	[%o2+5],%o0
	stb	%o0,[%o3+5]
	ldub	[%o2+6],%o1
	stb	%o1,[%o3+6]
	ldub	[%o2+7],%o0
	stb	%o0,[%o3+7]
	ldub	[%o2+8],%o1
	be	00015BE0
	stb	%o1,[%o3+8]

l00015BD4:
	or	%g0,%l3,%o1
	call	strcat
	or	%g0,%i0,%o0

l00015BE0:
	or	%g0,%l2,%o1
	call	mkstemps
	or	%g0,%i0,%o0
	subcc	%o0,FFFFFFFF,%g0
	bne	00015C00
	sethi	00000000,%g0

l00015BF8:
	call	abort
00015BFC                                     01 00 00 00             ....

l00015C00:
	call	close
	sethi	00000000,%g0
	subcc	%o0,00000000,%g0
	be	00015C1C
	sethi	00000000,%g0

l00015C14:
	call	abort
00015C18                         01 00 00 00                     ....    

l00015C1C:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; my_index: 00015C24
;;   Called from:
;;     000160FC (in _getopt_internal)
;;     000163F8 (in _getopt_internal)
;;     000164B8 (in _getopt_internal)
my_index proc
	ba	00015C48
	ldsb	[%o0+%g0],%g2

l00015C2C:
	sll	%g3,00000018,%g2
	sra	%g2,00000018,%g2
	subcc	%g2,%o1,%g0
	be	00015C58
	sethi	00000000,%g0

l00015C40:
	add	%o0,00000001,%o0
	ldsb	[%o0+%g0],%g2

l00015C48:
	subcc	%g2,00000000,%g0
	bne	00015C2C
	ldub	[%o0+%g0],%g3

l00015C54:
	or	%g0,00000000,%o0

l00015C58:
	jmpl	%o7,+00000008,%g0
	sethi	00000000,%g0

;; exchange: 00015C60
;;   Called from:
;;     00015EFC (in _getopt_internal)
;;     00015FD0 (in _getopt_internal)
exchange proc
	save	%sp,FFFFFF90,%sp
	sethi	000000AC,%g2
	sethi	000000A0,%i1
	sethi	000000AC,%g3
	ld	[%g2+768],%g1
	or	%g0,%g2,%o1
	ld	[%i1+104],%i5
	or	%g0,%i1,%o2
	ld	[%g3+752],%i4
	subcc	%i5,%g1,%g0
	ble	00015D40
	or	%g0,%g3,%o0

l00015C90:
	subcc	%g1,%i4,%g0
	ble	00015D44
	ld	[%o2+104],%g2

l00015C9C:
	sll	%g1,00000002,%o7
	sub	%i5,%g1,%g3

l00015CA4:
	sub	%g1,%i4,%g2
	subcc	%g3,%g2,%g0
	ble	00015CF0
	subcc	%g2,00000000,%g0

l00015CB4:
	ble	00015D2C
	sub	%i5,%g2,%i5

l00015CBC:
	or	%g0,%g2,%i3
	sll	%i5,00000002,%i1
	sll	%i4,00000002,%g3

l00015CC8:
	ld	[%g3+%i0],%i2
	addcc	%i3,FFFFFFFF,%i3
	ld	[%i1+%i0],%g2
	st	%g2,[%g3+%i0]
	st	%i2,[%i1+%i0]
	add	%i1,00000004,%i1
	bne	00015CC8
	add	%g3,00000004,%g3

l00015CE8:
	ba	00015D30
	subcc	%i5,%g1,%g0

l00015CF0:
	subcc	%g3,00000000,%g0
	ble	00015D28
	add	%i4,%g3,%g4

l00015CFC:
	or	%g0,%g3,%i3
	sll	%i4,00000002,%i1
	or	%g0,%o7,%g3

l00015D08:
	ld	[%i1+%i0],%i2
	addcc	%i3,FFFFFFFF,%i3
	ld	[%g3+%i0],%g2
	st	%g2,[%i1+%i0]
	st	%i2,[%g3+%i0]
	add	%g3,00000004,%g3
	bne	00015D08
	add	%i1,00000004,%i1

l00015D28:
	or	%g0,%g4,%i4

l00015D2C:
	subcc	%i5,%g1,%g0

l00015D30:
	ble	00015D40
	subcc	%g1,%i4,%g0

l00015D38:
	bg	00015CA4
	sub	%i5,%g1,%g3

l00015D40:
	ld	[%o2+104],%g2

l00015D44:
	ld	[%o1+768],%g3
	ld	[%o0+752],%i0
	sub	%g2,%g3,%g3
	add	%i0,%g3,%i0
	st	%i0,[%o0+752]
	st	%g2,[%o1+768]
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; _getopt_initialize: 00015D64
;;   Called from:
;;     00015E5C (in _getopt_internal)
_getopt_initialize proc
	save	%sp,FFFFFF90,%sp
	sethi	000000A0,%o0
	ld	[%o0+104],%o2
	sethi	000000AC,%o1
	sethi	000000AC,%o3
	sethi	000000AC,%o4
	sethi	0000005E,%o0
	st	%o2,[%o1+752]
	or	%g0,%i2,%i0
	st	%o2,[%o3+768]
	st	%g0,[%o4+704]
	call	getenv
	or	%o0,00000128,%o0
	or	%g0,%o0,%o1
	sethi	000000AC,%o0
	st	%o1,[%o0+736]
	ldsb	[%i0+%g0],%o0
	subcc	%o0,0000002D,%g0
	bne	00015DC8
	subcc	%o0,0000002B,%g0

l00015DB4:
	sethi	000000AC,%o1
	or	%g0,00000002,%o0
	st	%o0,[%o1+720]
	ba	00015DFC
	add	%i0,00000001,%i0

l00015DC8:
	bne	00015DE0
	subcc	%o1,00000000,%g0

l00015DD0:
	sethi	000000AC,%o0
	st	%g0,[%o0+720]
	ba	00015DFC
	add	%i0,00000001,%i0

l00015DE0:
	be	00015DF0
	sethi	000000AC,%o0

l00015DE8:
	ba	00015DFC
	st	%g0,[%o0+720]

l00015DF0:
	sethi	000000AC,%o1
	or	%g0,00000001,%o0
	st	%o0,[%o1+720]

l00015DFC:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; _getopt_internal: 00015E04
;;   Called from:
;;     00016998 (in getopt)
;;     000169C0 (in getopt_long)
;;     000169E8 (in getopt_long_only)
_getopt_internal proc
	save	%sp,FFFFFF90,%sp
	sethi	000000A0,%o0
	ld	[%o0+104],%o2
	sethi	000000A0,%o1
	or	%g0,%o0,%l5
	st	%i0,[%i6+68]
	st	%i3,[%i6+80]
	st	%g0,[%o1+100]
	subcc	%o2,00000000,%g0
	be	00015E44
	sethi	000000A0,%l0

l00015E30:
	ld	[%l0+108],%o0
	subcc	%o0,00000000,%g0
	bne	00015E74
	sethi	000000AC,%o0

l00015E40:
	subcc	%o2,00000000,%g0

l00015E44:
	bne	00015E58
	ld	[%i6+68],%o0

l00015E4C:
	or	%g0,00000001,%o0
	st	%o0,[%l5+104]
	ld	[%i6+68],%o0

l00015E58:
	or	%g0,%i2,%o2
	call	_getopt_initialize
	or	%g0,%i1,%o1
	or	%g0,00000001,%o1
	st	%o1,[%l0+108]
	or	%g0,%o0,%i2
	sethi	000000AC,%o0

l00015E74:
	or	%g0,%o0,%l6
	ld	[%o0+704],%o0
	subcc	%o0,00000000,%g0
	be,a	00015E9C
	sethi	000000AC,%o0

l00015E88:
	ldsb	[%o0+%g0],%o0
	subcc	%o0,00000000,%g0
	bne	000160C0
	ld	[%i6+80],%o0

l00015E98:
	sethi	000000AC,%o0

l00015E9C:
	ld	[%o0+768],%o1
	ld	[%l5+104],%o2
	subcc	%o1,%o2,%g0
	ble	00015EB4
	or	%g0,%o0,%l0

l00015EB0:
	st	%o2,[%l0+768]

l00015EB4:
	sethi	000000AC,%o1
	ld	[%o1+752],%o0
	subcc	%o0,%o2,%g0
	ble	00015ECC
	or	%g0,%o1,%l1

l00015EC8:
	st	%o2,[%l1+752]

l00015ECC:
	sethi	000000AC,%o1
	ld	[%o1+720],%o0
	subcc	%o0,00000001,%g0
	bne	00015F78
	or	%g0,%o1,%l2

l00015EE0:
	ld	[%l1+752],%o0
	ld	[%l0+768],%o1
	subcc	%o0,%o1,%g0
	be	00015F0C
	subcc	%o1,%o2,%g0

l00015EF4:
	be	00015F10
	ld	[%l0+768],%o0

l00015EFC:
	call	exchange
	or	%g0,%i1,%o0
	ba	00015F24
	sethi	000000A0,%o3

l00015F0C:
	ld	[%l0+768],%o0

l00015F10:
	ld	[%l5+104],%o1
	subcc	%o0,%o1,%g0
	bne,a	00015F20
	st	%o1,[%l1+752]

l00015F20:
	sethi	000000A0,%o3

l00015F24:
	ld	[%o3+104],%o2
	ld	[%i6+68],%o0
	ba	00015F48
	subcc	%o2,%o0,%g0

l00015F34:
	ld	[%i6+68],%o1
	add	%o0,00000001,%o0
	or	%g0,%o0,%o2
	st	%o0,[%o3+104]
	subcc	%o2,%o1,%g0

l00015F48:
	bge	00015F74
	sll	%o2,00000002,%o0

l00015F50:
	ld	[%i1+%o0],%o0
	ldsb	[%o0+%g0],%o1
	subcc	%o1,0000002D,%g0
	bne,a	00015F34
	ld	[%o3+104],%o0

l00015F64:
	ldsb	[%o0+1],%o0
	subcc	%o0,00000000,%g0
	be	00015F34
	ld	[%o3+104],%o0

l00015F74:
	st	%o2,[%l0+768]

l00015F78:
	ld	[%l5+104],%o1
	ld	[%i6+68],%o2
	subcc	%o1,%o2,%g0
	be	00016008
	sll	%o1,00000002,%o1

l00015F8C:
	ld	[%i1+%o1],%o0
	sethi	0000005E,%o1
	call	strcmp
	or	%o1,00000138,%o1
	subcc	%o0,00000000,%g0
	bne	0001600C
	ld	[%l5+104],%o2

l00015FA8:
	ld	[%l5+104],%o0
	ld	[%l1+752],%o1
	add	%o0,00000001,%o0
	ld	[%l0+768],%o2
	subcc	%o1,%o2,%g0
	be	00015FE0
	st	%o0,[%l5+104]

l00015FC4:
	subcc	%o2,%o0,%g0
	be	00015FE4
	ld	[%l1+752],%o1

l00015FD0:
	call	exchange
	or	%g0,%i1,%o0
	ba	00016000
	ld	[%i6+68],%o0

l00015FE0:
	ld	[%l1+752],%o1

l00015FE4:
	ld	[%l0+768],%o0
	subcc	%o1,%o0,%g0
	bne	00016000
	ld	[%i6+68],%o0

l00015FF4:
	ld	[%l5+104],%o0
	st	%o0,[%l1+752]
	ld	[%i6+68],%o0

l00016000:
	st	%o0,[%l0+768]
	st	%o0,[%l5+104]

l00016008:
	ld	[%l5+104],%o2

l0001600C:
	ld	[%i6+68],%o1
	subcc	%o2,%o1,%g0
	bne	00016038
	sll	%o2,00000002,%o0

l0001601C:
	ld	[%l1+752],%o1
	ld	[%l0+768],%o0
	subcc	%o1,%o0,%g0
	bne,a	0001606C
	st	%o1,[%l5+104]

l00016030:
	ba	00016978
	or	%g0,FFFFFFFF,%i0

l00016038:
	ld	[%i1+%o0],%o3
	ldsb	[%o3+%g0],%o1
	subcc	%o1,0000002D,%g0
	bne	00016060
	ld	[%l2+720],%o0

l0001604C:
	ldsb	[%o3+1],%o0
	subcc	%o0,00000000,%g0
	bne,a	00016098
	ld	[%i6+80],%o2

l0001605C:
	ld	[%l2+720],%o0

l00016060:
	subcc	%o0,00000000,%g0
	bne	00016074
	add	%o2,00000001,%o0

l0001606C:
	ba	00016978
	or	%g0,FFFFFFFF,%i0

l00016074:
	sethi	000000A0,%o2
	st	%o3,[%o2+100]
	st	%o0,[%l5+104]
	ba	00016978
	or	%g0,00000001,%i0

l00016088:
	or	%g0,%l1,%l3
	or	%g0,%l4,%l7
	ba	000161B8
	or	%g0,00000001,%i3

l00016098:
	subcc	%o2,00000000,%g0
	be	000160B0
	or	%g0,00000000,%o1

l000160A4:
	xor	%o0,0000002D,%o0
	subcc	%g0,%o0,%g0
	subx	%g0,FFFFFFFF,%o1

l000160B0:
	add	%o1,00000001,%o0
	add	%o3,%o0,%o0
	st	%o0,[%l6+704]
	ld	[%i6+80],%o0

l000160C0:
	subcc	%o0,00000000,%g0
	be	0001649C
	ld	[%l5+104],%o0

l000160CC:
	sll	%o0,00000002,%o0
	ld	[%i1+%o0],%o0
	ldsb	[%o0+1],%o1
	subcc	%o1,0000002D,%g0
	be	00016110
	subcc	%i5,00000000,%g0

l000160E4:
	be,a	000164A0
	ld	[%l6+704],%o1

l000160EC:
	ldsb	[%o0+2],%o0
	subcc	%o0,00000000,%g0
	bne	00016114
	ld	[%l6+704],%l2

l000160FC:
	call	my_index
	or	%g0,%i2,%o0
	subcc	%o0,00000000,%g0
	bne	000164A0
	ld	[%l6+704],%o1

l00016110:
	ld	[%l6+704],%l2

l00016114:
	or	%g0,00000000,%l3
	ld	[%i6+80],%o2
	or	%g0,00000000,%i3
	ldsb	[%l2+%g0],%o0
	or	%g0,00000000,%i0
	ld	[%o2+%g0],%o1
	ba	00016138
	or	%g0,FFFFFFFF,%l7

l00016134:
	ldsb	[%l2+%g0],%o0

l00016138:
	subcc	%o0,00000000,%g0
	be	0001614C
	subcc	%o0,0000003D,%g0

l00016144:
	bne,a	00016134
	add	%l2,00000001,%l2

l0001614C:
	ld	[%i6+80],%l1
	subcc	%o1,00000000,%g0
	be	000161B8
	or	%g0,00000000,%l4

l0001615C:
	ld	[%l6+704],%o1
	ld	[%l1+%g0],%o0
	call	strncmp
	sub	%l2,%o1,%o2
	subcc	%o0,00000000,%g0
	bne,a	000161A8
	add	%l1,00000010,%l1

l00016178:
	ld	[%l6+704],%l0
	ld	[%l1+%g0],%o0
	call	strlen
	sub	%l2,%l0,%l0
	subcc	%l0,%o0,%g0
	be	00016088
	subcc	%l3,00000000,%g0

l00016194:
	bne,a	000161A4
	or	%g0,00000001,%i0

l0001619C:
	or	%g0,%l1,%l3
	or	%g0,%l4,%l7

l000161A4:
	add	%l1,00000010,%l1

l000161A8:
	ld	[%l1+%g0],%o0
	subcc	%o0,00000000,%g0
	bne	0001615C
	add	%l4,00000001,%l4

l000161B8:
	subcc	%i0,00000000,%g0
	be	00016230
	subcc	%i3,00000000,%g0

l000161C4:
	bne	00016234
	subcc	%l3,00000000,%g0

l000161CC:
	sethi	000000A0,%o0
	ld	[%o0+112],%o1
	subcc	%o1,00000000,%g0
	be	00016200
	ld	[%l5+104],%o1

l000161E0:
	sethi	000000AD,%o0
	sll	%o1,00000002,%o1
	ld	[%i1+%o1],%o3
	or	%o0,00000240,%o0
	sethi	0000005E,%o1
	ld	[%i1+%g0],%o2
	call	fprintf
	or	%o1,00000140,%o1

l00016200:
	ld	[%l6+704],%o0
	call	strlen
	or	%g0,0000003F,%i0
	ld	[%l6+704],%o1
	sethi	000000A0,%o3
	ld	[%l5+104],%o2
	add	%o1,%o0,%o1
	add	%o2,00000001,%o2
	st	%o1,[%l6+704]
	st	%o2,[%l5+104]
	ba	00016978
	st	%g0,[%o3+116]

l00016230:
	subcc	%l3,00000000,%g0

l00016234:
	be	000163D0
	ld	[%l5+104],%o2

l0001623C:
	add	%o2,00000001,%o3
	st	%o3,[%l5+104]
	ldsb	[%l2+%g0],%o0
	subcc	%o0,00000000,%g0
	be	000162FC
	ld	[%l3+4],%o0

l00016254:
	subcc	%o0,00000000,%g0
	be	0001626C
	add	%l2,00000001,%o0

l00016260:
	sethi	000000A0,%o1
	ba	00016398
	st	%o0,[%o1+100]

l0001626C:
	sethi	000000A0,%o0
	ld	[%o0+112],%o1
	subcc	%o1,00000000,%g0
	be	00016398
	sll	%o3,00000002,%o0

l00016280:
	add	%o0,%i1,%o0
	ld	[%o0-4],%o3
	ldsb	[%o3+1],%o1
	subcc	%o1,0000002D,%g0
	bne	000162B8
	sethi	000000AD,%o0

l00016298:
	sethi	0000005E,%o1
	ld	[%i1+%g0],%o2
	or	%o0,00000240,%o0
	ld	[%l3+%g0],%o3
	call	fprintf
	or	%o1,00000160,%o1
	ba	000162D8
	ld	[%l6+704],%o0

l000162B8:
	sethi	0000005E,%o1
	ld	[%i1+%g0],%o2
	or	%o0,00000240,%o0
	ldsb	[%o3+%g0],%o3
	or	%o1,00000190,%o1
	call	fprintf
	ld	[%l3+%g0],%o4
	ld	[%l6+704],%o0

l000162D8:
	call	strlen
	or	%g0,0000003F,%i0
	ld	[%l6+704],%o1
	sethi	000000A0,%o3
	ld	[%l3+12],%o2
	add	%o1,%o0,%o1
	st	%o1,[%l6+704]
	ba	00016978
	st	%o2,[%o3+116]

l000162FC:
	subcc	%o0,00000001,%g0
	bne	00016398
	ld	[%i6+68],%o0

l00016308:
	subcc	%o3,%o0,%g0
	bge	00016330
	sethi	000000A0,%o0

l00016314:
	sll	%o3,00000002,%o0
	ld	[%i1+%o0],%o1
	add	%o2,00000002,%o2
	sethi	000000A0,%o0
	st	%o1,[%o0+100]
	ba	00016398
	st	%o2,[%l5+104]

l00016330:
	ld	[%o0+112],%o1
	subcc	%o1,00000000,%g0
	be	00016360
	sll	%o3,00000002,%o1

l00016340:
	add	%o1,%i1,%o1
	ld	[%o1-4],%o3
	sethi	000000AD,%o0
	sethi	0000005E,%o1
	ld	[%i1+%g0],%o2
	or	%o0,00000240,%o0
	call	fprintf
	or	%o1,000001C0,%o1

l00016360:
	call	strlen
	ld	[%l6+704],%o0
	ld	[%l6+704],%o1
	sethi	000000A0,%o3
	ld	[%l3+12],%o2
	add	%o1,%o0,%o1
	st	%o1,[%l6+704]
	st	%o2,[%o3+116]
	ldsb	[%i2+%g0],%o0
	or	%g0,0000003F,%i0

l00016388:
	subcc	%o0,0000003A,%g0
	be,a	00016978
	or	%g0,0000003A,%i0

l00016394:
	ba,a	00016978

l00016398:
	call	strlen
	ld	[%l6+704],%o0
000163A0 D2 05 A2 C0 80 A7 20 00 92 02 40 08 02 80 00 03 ...... ...@.....
000163B0 D2 25 A2 C0                                     .%..            

l000163B4:
	st	%l7,[%i4+%g0]

l000163B8:
	ld	[%l3+8],%o1
	subcc	%o1,00000000,%g0
	bne,a	00016874
	ld	[%l3+12],%o0

l000163C8:
	ba	00016978
	ld	[%l3+12],%i0

l000163D0:
	subcc	%i5,00000000,%g0
	be	0001640C
	ld	[%l5+104],%o0

l000163DC:
	sll	%o0,00000002,%o0
	ld	[%i1+%o0],%o1
	ldsb	[%o1+1],%o2
	subcc	%o2,0000002D,%g0
	be	0001640C
	ld	[%l6+704],%o2

l000163F4:
	or	%g0,%i2,%o0
	call	my_index
	ldsb	[%o2+%g0],%o1
	subcc	%o0,00000000,%g0
	bne	000164A0
	ld	[%l6+704],%o1

l0001640C:
	sethi	000000A0,%o0
	ld	[%o0+112],%o1
	subcc	%o1,00000000,%g0
	be	00016474
	ld	[%l5+104],%o0

l00016420:
	sll	%o0,00000002,%o0
	ld	[%i1+%o0],%o3
	ldsb	[%o3+1],%o1
	subcc	%o1,0000002D,%g0
	bne	00016458
	sethi	000000AD,%o0

l00016438:
	sethi	0000005E,%o1
	ld	[%i1+%g0],%o2
	or	%o0,00000240,%o0
	ld	[%l6+704],%o3
	call	fprintf
	or	%o1,000001E8,%o1
	ba	00016478
	ld	[%l5+104],%o0

l00016458:
	sethi	0000005E,%o1
	ld	[%i1+%g0],%o2
	or	%o0,00000240,%o0
	ldsb	[%o3+%g0],%o3
	or	%o1,00000208,%o1
	call	fprintf
	ld	[%l6+704],%o4

l00016474:
	ld	[%l5+104],%o0

l00016478:
	sethi	0000005E,%o1
	or	%o1,00000228,%o1
	add	%o0,00000001,%o0
	sethi	000000A0,%o2
	st	%o1,[%l6+704]
	st	%o0,[%l5+104]
	st	%g0,[%o2+116]
	ba	00016978
	or	%g0,0000003F,%i0

l0001649C:
	ld	[%l6+704],%o1

l000164A0:
	or	%g0,%i2,%o0
	ldub	[%o1+%g0],%i0
	add	%o1,00000001,%o1
	sll	%i0,00000018,%o2
	sra	%o2,00000018,%l0
	st	%o1,[%l6+704]
	call	my_index
	or	%g0,%l0,%o1
	ld	[%l6+704],%o3
	or	%g0,%o0,%o2
	ldsb	[%o3+%g0],%o0
	subcc	%o0,00000000,%g0
	bne	000164E4
	subcc	%o2,00000000,%g0

l000164D8:
	ld	[%l5+104],%o0
	add	%o0,00000001,%o0
	st	%o0,[%l5+104]

l000164E4:
	be	000164F4
	subcc	%l0,0000003A,%g0

l000164EC:
	bne,a	00016568
	ldsb	[%o2+%g0],%o0

l000164F4:
	sethi	000000A0,%o0
	ld	[%o0+112],%o1
	subcc	%o1,00000000,%g0
	be	00016550
	sethi	000000AC,%o0

l00016508:
	ld	[%o0+736],%o1
	subcc	%o1,00000000,%g0
	be	00016538
	sethi	000000AD,%o0

l00016518:
	sethi	0000005E,%o1
	ld	[%i1+%g0],%o2
	or	%o0,00000240,%o0
	or	%o1,00000230,%o1
	call	fprintf
	or	%g0,%l0,%o3
	ba	00016554
	sll	%i0,00000018,%o0

l00016538:
	sethi	0000005E,%o1
	ld	[%i1+%g0],%o2
	or	%o0,00000240,%o0
	or	%o1,00000250,%o1
	call	fprintf
	or	%g0,%l0,%o3

l00016550:
	sll	%i0,00000018,%o0

l00016554:
	sra	%o0,00000018,%o0
	sethi	000000A0,%o1
	st	%o0,[%o1+116]
	ba	00016978
	or	%g0,0000003F,%i0

l00016568:
	subcc	%o0,00000057,%g0
	bne	00016888
	ldub	[%o2+1],%o1

l00016574:
	subcc	%o1,0000003B,%g0
	bne	0001688C
	sll	%o1,00000018,%o0

l00016580:
	ldsb	[%o3+%g0],%o0
	or	%g0,00000000,%l3
	subcc	%o0,00000000,%g0
	or	%g0,00000000,%i0
	or	%g0,00000000,%l4
	be	000165B4
	or	%g0,00000000,%l7

l0001659C:
	ld	[%l5+104],%o0
	sethi	000000A0,%o1
	add	%o0,00000001,%o0
	st	%o3,[%o1+100]
	ba	0001663C
	st	%o0,[%l5+104]

l000165B4:
	ld	[%l5+104],%o2
	ld	[%i6+68],%o0
	subcc	%o2,%o0,%g0
	bne	00016624
	sll	%o2,00000002,%o0

l000165C8:
	sethi	000000A0,%o0
	ld	[%o0+112],%o1
	subcc	%o1,00000000,%g0
	be	000165F4
	sethi	000000AD,%o0

l000165DC:
	sethi	0000005E,%o1
	ld	[%i1+%g0],%o2
	or	%o0,00000240,%o0
	or	%o1,00000270,%o1
	call	fprintf
	or	%g0,%l0,%o3

l000165F4:
	sethi	000000A0,%o0
	st	%l0,[%o0+116]
	ldsb	[%i2+%g0],%o1
	subcc	%o1,0000003A,%g0
	bne	00016970
	or	%g0,0000003F,%i0

l0001660C:
	ba	00016970
	or	%g0,0000003A,%i0

l00016614:
	or	%g0,%i3,%l3
	or	%g0,%l2,%l7
	ba	000166CC
	or	%g0,00000001,%i0

l00016624:
	ld	[%i1+%o0],%o1
	add	%o2,00000001,%o2
	sethi	000000A0,%o0
	st	%o1,[%o0+100]
	st	%o2,[%l5+104]
	sethi	000000A0,%o1

l0001663C:
	ld	[%o1+100],%l1
	st	%l1,[%l6+704]

l00016644:
	ldsb	[%l1+%g0],%o0
	subcc	%o0,00000000,%g0
	be	0001665C
	subcc	%o0,0000003D,%g0

l00016654:
	bne,a	00016644
	add	%l1,00000001,%l1

l0001665C:
	ld	[%i6+80],%i3
	ld	[%i3+%g0],%o0
	subcc	%o0,00000000,%g0
	be	000166CC
	or	%g0,00000000,%l2

l00016670:
	ld	[%l6+704],%o1
	ld	[%i3+%g0],%o0
	call	strncmp
	sub	%l1,%o1,%o2
	subcc	%o0,00000000,%g0
	bne,a	000166BC
	add	%i3,00000010,%i3

l0001668C:
	ld	[%l6+704],%l0
	ld	[%i3+%g0],%o0
	call	strlen
	sub	%l1,%l0,%l0
	subcc	%l0,%o0,%g0
	be	00016614
	subcc	%l3,00000000,%g0

l000166A8:
	bne,a	000166B8
	or	%g0,00000001,%l4

l000166B0:
	or	%g0,%i3,%l3
	or	%g0,%l2,%l7

l000166B8:
	add	%i3,00000010,%i3

l000166BC:
	ld	[%i3+%g0],%o0
	subcc	%o0,00000000,%g0
	bne	00016670
	add	%l2,00000001,%l2

l000166CC:
	subcc	%l4,00000000,%g0
	be	0001673C
	subcc	%i0,00000000,%g0

l000166D8:
	bne	00016740
	subcc	%l3,00000000,%g0

l000166E0:
	sethi	000000A0,%o0
	ld	[%o0+112],%o1
	subcc	%o1,00000000,%g0
	be	00016714
	ld	[%l5+104],%o1

l000166F4:
	sethi	000000AD,%o0
	sll	%o1,00000002,%o1
	ld	[%i1+%o1],%o3
	or	%o0,00000240,%o0
	sethi	0000005E,%o1
	ld	[%i1+%g0],%o2
	call	fprintf
	or	%o1,00000298,%o1

l00016714:
	ld	[%l6+704],%o0
	call	strlen
	or	%g0,0000003F,%i0
	ld	[%l6+704],%o1
	ld	[%l5+104],%o2
	add	%o1,%o0,%o1
	add	%o2,00000001,%o2
	st	%o1,[%l6+704]
	ba	00016978
	st	%o2,[%l5+104]

l0001673C:
	subcc	%l3,00000000,%g0

l00016740:
	be,a	00016880
	st	%g0,[%l6+704]

l00016748:
	ldsb	[%l1+%g0],%o0
	subcc	%o0,00000000,%g0
	be	000167B8
	ld	[%l3+4],%o0

l00016758:
	subcc	%o0,00000000,%g0
	be	00016770
	add	%l1,00000001,%o0

l00016764:
	sethi	000000A0,%o2
	ba	00016840
	st	%o0,[%o2+100]

l00016770:
	sethi	000000A0,%o0
	ld	[%o0+112],%o1
	subcc	%o1,00000000,%g0
	be	0001679C
	sethi	000000AD,%o0

l00016784:
	sethi	0000005E,%o1
	ld	[%i1+%g0],%o2
	or	%o0,00000240,%o0
	ld	[%l3+%g0],%o3
	call	fprintf
	or	%o1,000002C0,%o1

l0001679C:
	ld	[%l6+704],%o0
	call	strlen
	or	%g0,0000003F,%i0
	ld	[%l6+704],%o1
	add	%o1,%o0,%o1
	ba	00016978
	st	%o1,[%l6+704]

l000167B8:
	subcc	%o0,00000001,%g0
	bne	00016840
	ld	[%l5+104],%o2

l000167C4:
	ld	[%i6+68],%o0
	subcc	%o2,%o0,%g0
	bge	000167F0
	sethi	000000A0,%o0

l000167D4:
	sll	%o2,00000002,%o0
	ld	[%i1+%o0],%o1
	add	%o2,00000001,%o2
	sethi	000000A0,%o0
	st	%o1,[%o0+100]
	ba	00016840
	st	%o2,[%l5+104]

l000167F0:
	ld	[%o0+112],%o1
	subcc	%o1,00000000,%g0
	be	00016820
	sll	%o2,00000002,%o1

l00016800:
	add	%o1,%i1,%o1
	ld	[%o1-4],%o3
	sethi	000000AD,%o0
	sethi	0000005E,%o1
	ld	[%i1+%g0],%o2
	or	%o0,00000240,%o0
	call	fprintf
	or	%o1,000001C0,%o1

l00016820:
	call	strlen
	ld	[%l6+704],%o0
	ld	[%l6+704],%o1
	or	%g0,0000003F,%i0
	add	%o1,%o0,%o1
	st	%o1,[%l6+704]
	ba	00016388
	ldsb	[%i2+%g0],%o0

l00016840:
	call	strlen
	ld	[%l6+704],%o0
	ld	[%l6+704],%o1
	subcc	%i4,00000000,%g0
	add	%o1,%o0,%o1
	be	00016860
	st	%o1,[%l6+704]

l0001685C:
	st	%l7,[%i4+%g0]

l00016860:
	ld	[%l3+8],%o1
	subcc	%o1,00000000,%g0
	be,a	00016978
	ld	[%l3+12],%i0

l00016870:
	ld	[%l3+12],%o0

l00016874:
	or	%g0,00000000,%i0
	ba	00016978
	st	%o0,[%o1+%g0]

l00016880:
	ba	00016978
	or	%g0,00000057,%i0

l00016888:
	sll	%o1,00000018,%o0

l0001688C:
	sra	%o0,00000018,%o0
	subcc	%o0,0000003A,%g0
	bne,a	00016974
	sll	%i0,00000018,%i0

l0001689C:
	ldsb	[%o2+2],%o0
	subcc	%o0,0000003A,%g0
	bne	000168C8
	ld	[%l6+704],%o1

l000168AC:
	ldsb	[%o1+%g0],%o0
	subcc	%o0,00000000,%g0
	bne	000168D8
	ld	[%l5+104],%o0

l000168BC:
	sethi	000000A0,%o0
	ba	0001696C
	st	%g0,[%o0+100]

l000168C8:
	ldsb	[%o1+%g0],%o0
	subcc	%o0,00000000,%g0
	be	000168EC
	ld	[%l5+104],%o0

l000168D8:
	sethi	000000A0,%o2
	add	%o0,00000001,%o0
	st	%o1,[%o2+100]
	ba	0001696C
	st	%o0,[%l5+104]

l000168EC:
	ld	[%l5+104],%o2
	ld	[%i6+68],%o0
	subcc	%o2,%o0,%g0
	bne	00016958
	sll	%o2,00000002,%o0

l00016900:
	sethi	000000A0,%o0
	ld	[%o0+112],%o1
	subcc	%o1,00000000,%g0
	be	00016930
	sethi	000000AD,%o0

l00016914:
	sethi	0000005E,%o1
	sll	%i0,00000018,%o3
	ld	[%i1+%g0],%o2
	or	%o0,00000240,%o0
	or	%o1,00000270,%o1
	call	fprintf
	sra	%o3,00000018,%o3

l00016930:
	sll	%i0,00000018,%o0
	sra	%o0,00000018,%o0
	sethi	000000A0,%o1
	st	%o0,[%o1+116]
	ldsb	[%i2+%g0],%o2
	subcc	%o2,0000003A,%g0
	bne	0001696C
	or	%g0,0000003F,%i0

l00016950:
	ba	0001696C
	or	%g0,0000003A,%i0

l00016958:
	ld	[%i1+%o0],%o1
	add	%o2,00000001,%o2
	sethi	000000A0,%o0
	st	%o1,[%o0+100]
	st	%o2,[%l5+104]

l0001696C:
	st	%g0,[%l6+704]

l00016970:
	sll	%i0,00000018,%i0

l00016974:
	sra	%i0,00000018,%i0

l00016978:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; getopt: 00016980
getopt proc
	save	%sp,FFFFFF90,%sp
	or	%g0,%i0,%o0
	or	%g0,%i1,%o1
	or	%g0,%i2,%o2
	or	%g0,00000000,%o3
	or	%g0,00000000,%o4
	call	_getopt_internal
	or	%g0,00000000,%o5
	jmpl	%i7,+00000008,%g0
	restore	%g0,%o0,%o0

;; getopt_long: 000169A8
;;   Called from:
;;     00015088 (in main)
getopt_long proc
	save	%sp,FFFFFF90,%sp
	or	%g0,%i0,%o0
	or	%g0,%i1,%o1
	or	%g0,%i2,%o2
	or	%g0,%i3,%o3
	or	%g0,%i4,%o4
	call	_getopt_internal
	or	%g0,00000000,%o5
	jmpl	%i7,+00000008,%g0
	restore	%g0,%o0,%o0

;; getopt_long_only: 000169D0
getopt_long_only proc
	save	%sp,FFFFFF90,%sp
	or	%g0,%i0,%o0
	or	%g0,%i1,%o1
	or	%g0,%i2,%o2
	or	%g0,%i3,%o3
	or	%g0,%i4,%o4
	call	_getopt_internal
	or	%g0,00000001,%o5
	jmpl	%i7,+00000008,%g0
	restore	%g0,%o0,%o0

;; pexecute: 000169F8
;;   Called from:
;;     00012EFC (in gen_aux_info_file)
pexecute proc
	save	%sp,FFFFFF70,%sp
	ld	[%i6+92],%o1
	st	%i0,[%i6+68]
	st	%i1,[%i6+72]
	st	%i2,[%i6+76]
	st	%i4,[%i6+84]
	andcc	%o1,00000004,%g0
	be	00016A28
	st	%i5,[%i6+88]

l00016A1C:
	sethi	0000009F,%o0
	ba	00016A30
	or	%o0,0000023C,%o0

l00016A28:
	sethi	0000009F,%o0
	or	%o0,00000248,%o0

l00016A30:
	andcc	%o1,00000001,%g0
	be	00016A44
	st	%o0,[%i6-28]

l00016A3C:
	sethi	000000AC,%o0
	st	%g0,[%o0+784]

l00016A44:
	sethi	000000AC,%l0
	ld	[%l0+784],%o0
	andcc	%o1,00000002,%g0
	bne	00016A8C
	st	%o0,[%i6-32]

l00016A58:
	call	pipe
	add	%i6,FFFFFFE8,%o0
	subcc	%o0,00000000,%g0
	bge,a	00016A7C
	ld	[%i6-24],%o0

l00016A6C:
	ld	[%i6+84],%o1
	sethi	0000005E,%o0
	ba	00016B10
	or	%o0,00000318,%o0

l00016A7C:
	ld	[%i6-20],%o1
	st	%o0,[%l0+784]
	ba	00016A98
	st	%o1,[%i6-36]

l00016A8C:
	or	%g0,00000001,%o2
	st	%g0,[%l0+784]
	st	%o2,[%i6-36]

l00016A98:
	or	%g0,00000001,%o0
	st	%o0,[%i6-44]
	ba	00016AC8
	st	%g0,[%i6-40]

l00016AA8:
	call	sleep
	ld	[%i6-44],%o0
	ld	[%i6-44],%o1
	ld	[%i6-40],%o2
	sll	%o1,00000001,%o1
	add	%o2,00000001,%o2
	st	%o1,[%i6-44]
	st	%o2,[%i6-40]

l00016AC8:
	ld	[%i6-40],%o0
	subcc	%o0,00000003,%g0
	bg	00016AEC
	subcc	%l1,FFFFFFFF,%g0

l00016AD8:
	call	vfork
	sethi	00000000,%g0
	orcc	%o0,00000000,%l1
	bl	00016AA8
	subcc	%l1,FFFFFFFF,%g0

l00016AEC:
	be	00016B04
	subcc	%l1,00000000,%g0

l00016AF4:
	be	00016B24
	ld	[%i6-32],%o0

l00016AFC:
	ba	00016BF8
	subcc	%o0,00000000,%g0

l00016B04:
	ld	[%i6+84],%o1
	sethi	0000005E,%o0
	or	%o0,00000320,%o0

l00016B10:
	st	%o0,[%o1+%g0]
	ld	[%i6+88],%o2
	or	%g0,FFFFFFFF,%i0
	ba	00016C24
	st	%g0,[%o2+%g0]

l00016B24:
	subcc	%o0,00000000,%g0
	be	00016B4C
	ld	[%i6-36],%o1

l00016B30:
	call	close
	or	%g0,00000000,%o0
	call	dup
	ld	[%i6-32],%o0
	call	close
	ld	[%i6-32],%o0
	ld	[%i6-36],%o1

l00016B4C:
	subcc	%o1,00000001,%g0
	be	00016B74
	sethi	000000AC,%o0

l00016B58:
	call	close
	or	%g0,00000001,%o0
	call	dup
	ld	[%i6-36],%o0
	call	close
	ld	[%i6-36],%o0
	sethi	000000AC,%o0

l00016B74:
	ld	[%o0+784],%o0
	subcc	%o0,00000000,%g0
	be,a	00016B90
	ld	[%i6+72],%o1

l00016B84:
	call	close
	sethi	00000000,%g0
	ld	[%i6+72],%o1

l00016B90:
	ld	[%i6-28],%o2
	jmpl	%o2,%g0,%o7
	ld	[%i6+68],%o0
	sethi	000000AD,%l1
	ld	[%i6+76],%o2
	or	%l1,00000240,%l1
	sethi	0000005E,%o1
	or	%o1,00000328,%o1
	call	fprintf
	or	%g0,%l1,%o0
	sethi	000000A0,%o2
	ld	[%o2+120],%o1
	or	%g0,%l1,%o0
	call	fprintf
	ld	[%i6+68],%o2
	sethi	000000AD,%o1
	ld	[%o1+864],%o0
	sethi	0000005E,%l0
	call	xstrerror
	or	%l0,00000330,%l0
	or	%g0,%o0,%o2
	or	%g0,%l1,%o0
	call	fprintf
	or	%g0,%l0,%o1
	call	exit
00016BF4             90 10 3F FF                             ..?.        

l00016BF8:
	be	00016C0C
	ld	[%i6-36],%o1

l00016C00:
	call	close
	sethi	00000000,%g0
	ld	[%i6-36],%o1

l00016C0C:
	subcc	%o1,00000001,%g0
	be	00016C24
	or	%g0,%l1,%i0

l00016C18:
	call	close
	or	%g0,%o1,%o0
	or	%g0,%l1,%i0

l00016C24:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; pwait: 00016C2C
;;   Called from:
;;     00012F68 (in gen_aux_info_file)
;;     00012F6C (in gen_aux_info_file)
pwait proc
	save	%sp,FFFFFF90,%sp
	call	wait
	or	%g0,%i1,%o0
	jmpl	%i7,+00000008,%g0
	restore	%g0,%o0,%o0

;; mkstemps: 00016C40
;;   Called from:
;;     00015BE4 (in make_temp_file)
mkstemps proc
	save	%sp,FFFFFF88,%sp
	call	strlen
	or	%g0,%i0,%o0
	or	%g0,%o0,%o1
	add	%i1,00000006,%o0
	subcc	%o1,%o0,%g0
	bl	00016E58
	sub	%o1,%o0,%o0

l00016C60:
	add	%i0,%o0,%i1
	sethi	0000005E,%o1
	or	%o1,00000378,%o1
	or	%g0,%i1,%o0
	call	strncmp
	or	%g0,00000006,%o2
	subcc	%o0,00000000,%g0
	be	00016C94
	or	%g0,00000000,%o1

l00016C84:
	ba	00016E5C
	or	%g0,FFFFFFFF,%i0

l00016C8C:
	ba	00016E5C
	or	%g0,%o0,%i0

l00016C94:
	call	gettimeofday
	add	%i6,FFFFFFE8,%o0
	call	getpid
	or	%g0,00000000,%l3
	ld	[%i6-20],%o1
	or	%g0,%o0,%g3
	or	%g0,%o1,%o3
	ld	[%i6-24],%o7
	sra	%o1,0000001F,%o2
	srl	%o3,00000010,%g2
	sll	%o2,00000010,%o1
	or	%g0,%o7,%l1
	or	%g2,%o1,%o4
	sra	%o7,0000001F,%l0
	sethi	000000AD,%o7
	sll	%o3,00000010,%o5
	sra	%o0,0000001F,%g2
	ldd	[%o7+0],%o0
	xor	%o5,%l1,%o5
	xor	%o4,%l0,%o4
	xor	%o5,%g3,%o5
	addcc	%o1,%o5,%o1
	xor	%o4,%g2,%o4
	addx	%o0,%o4,%o0
	sethi	00000011,%o2
	sethi	0000005E,%o3
	or	%g0,%o7,%l4
	or	%o2,000000A7,%l5
	or	%o3,00000338,%l2
	std	%o0,[%o7+0]

l00016D0C:
	ldd	[%l4+0],%l0
	or	%g0,00000000,%o2
	or	%g0,0000003E,%o3
	or	%g0,%l0,%o0
	call	__urem64
	or	%g0,%l1,%o1
	ldub	[%l2+%o1],%o2
	stb	%o2,[%i1+%g0]
	or	%g0,%l0,%o0
	or	%g0,%l1,%o1
	or	%g0,00000000,%o2
	call	__udiv64
	or	%g0,0000003E,%o3
	or	%g0,%o0,%l0
	or	%g0,%o1,%l1
	or	%g0,00000000,%o2
	call	__urem64
	or	%g0,0000003E,%o3
	ldub	[%l2+%o1],%o2
	stb	%o2,[%i1+1]
	or	%g0,%l0,%o0
	or	%g0,%l1,%o1
	or	%g0,00000000,%o2
	call	__udiv64
	or	%g0,0000003E,%o3
	or	%g0,%o0,%l0
	or	%g0,%o1,%l1
	or	%g0,00000000,%o2
	call	__urem64
	or	%g0,0000003E,%o3
	ldub	[%l2+%o1],%o2
	stb	%o2,[%i1+2]
	or	%g0,%l0,%o0
	or	%g0,%l1,%o1
	or	%g0,00000000,%o2
	call	__udiv64
	or	%g0,0000003E,%o3
	or	%g0,%o0,%l0
	or	%g0,%o1,%l1
	or	%g0,00000000,%o2
	call	__urem64
	or	%g0,0000003E,%o3
	ldub	[%l2+%o1],%o2
	stb	%o2,[%i1+3]
	or	%g0,%l0,%o0
	or	%g0,%l1,%o1
	or	%g0,00000000,%o2
	call	__udiv64
	or	%g0,0000003E,%o3
	or	%g0,%o0,%l0
	or	%g0,%o1,%l1
	or	%g0,00000000,%o2
	call	__urem64
	or	%g0,0000003E,%o3
	ldub	[%l2+%o1],%o2
	stb	%o2,[%i1+4]
	or	%g0,00000000,%o2
	or	%g0,0000003E,%o3
	or	%g0,%l0,%o0
	call	__udiv64
	or	%g0,%l1,%o1
	or	%g0,00000000,%o2
	call	__urem64
	or	%g0,0000003E,%o3
	ldub	[%l2+%o1],%o2
	stb	%o2,[%i1+5]
	or	%g0,%i0,%o0
	or	%g0,00000502,%o1
	call	open
	or	%g0,00000180,%o2
	subcc	%o0,00000000,%g0
	bge	00016C8C
	ldd	[%l4+0],%o2

l00016E30:
	sethi	00000007,%o1
	or	%o1,00000261,%o1
	addcc	%o3,%o1,%o3
	or	%g0,00000000,%o0
	addx	%o2,%o0,%o2
	add	%l3,00000001,%l3
	subcc	%l3,%l5,%g0
	ble	00016D0C
	std	%o2,[%l4+0]

l00016E54:
	stb	%g0,[%i0+%g0]

l00016E58:
	or	%g0,FFFFFFFF,%i0

l00016E5C:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; fn00016E64: 00016E64
;;   Called from:
;;     00016E78 (in __do_global_ctors_aux)
fn00016E64 proc
	jmpl	%o7,+00000008,%g0
	add	%o7,%l7,%l7

;; __do_global_ctors_aux: 00016E6C
;;   Called from:
;;     00016ED4 (in _init)
__do_global_ctors_aux proc
	save	%sp,FFFFFF90,%sp
	sethi	00000000,%o0
	sethi	00000043,%l7
	call	fn00016E64
	add	%l7,00000108,%l7
	or	%o0,00000014,%o0
	ld	[%l7+%o0],%o1
	ld	[%o1-4],%o2
	subcc	%o2,FFFFFFFF,%g0
	be	00016EB4
	add	%o1,FFFFFFFC,%l0

l00016E98:
	ld	[%l0+%g0],%o0
	jmpl	%o0,%g0,%o7
	add	%l0,FFFFFFFC,%l0
	ld	[%l0+%g0],%o0
	subcc	%o0,FFFFFFFF,%g0
	bne	00016E98
	sethi	00000000,%g0

l00016EB4:
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0

;; init_dummy: 00016EBC
init_dummy proc
	save	%sp,FFFFFF90,%sp
	jmpl	%i7,+00000008,%g0
	restore	%g0,%g0,%g0
