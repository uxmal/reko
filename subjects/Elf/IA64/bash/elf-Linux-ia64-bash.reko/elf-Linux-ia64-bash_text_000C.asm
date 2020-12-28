;;; Segment .text (400000000001C480)

;; executable_file: 40000000000DCA40
executable_file proc
	Invalid
	Invalid
	Invalid

;; is_directory: 40000000000DCB00
is_directory proc
	Invalid
	Invalid
	Invalid

;; executable_or_directory: 40000000000DD140
executable_or_directory proc
	Invalid
	Invalid
	Invalid

;; find_user_command: 40000000000DD1C0
find_user_command proc
	Invalid
	addl	r33,36,r0
	nop.i	0x0

;; find_path_file: 40000000000DD200
find_path_file proc
	Invalid
	addl	r33,64,r0
	nop.i	0x0

;; search_for_command: 40000000000DD240
search_for_command proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; user_command_matches: 40000000000DD600
user_command_matches proc
	Invalid
	adds	r12,-160,r12
	Invalid

;; redirection_expand: 40000000000DE3C0
redirection_expand proc
	Invalid
	addl	r34,9208,r1
	Invalid

;; redirection_error: 40000000000E0E00
redirection_error proc
	Invalid
	adds	r14,16,r32
	Invalid

;; do_redirections: 40000000000E1640
do_redirections proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; stdin_redirects: 40000000000E1800
stdin_redirects proc
	adds	r8,0,r0
	addl	r19,1,r0
	addl	r20,2358,r0

;; set_itemlist_dirty: 40000000000E3CC0
set_itemlist_dirty proc
	Invalid
	Invalid
	nop.i	0x0

;; initialize_itemlist: 40000000000E3D00
initialize_itemlist proc
	Invalid
	adds	r14,8,r32
	Invalid

;; clean_itemlist: 40000000000E3D80
clean_itemlist proc
	Invalid
	adds	r34,16,r32
	Invalid

;; filter_stringlist: 40000000000E4400
filter_stringlist proc
	Invalid
	adds	r37,12,r32
	Invalid

;; completions_to_stringlist: 40000000000E4880
completions_to_stringlist proc
	Invalid
	Invalid
	Invalid

;; gen_compspec_completions: 40000000000E5AC0
gen_compspec_completions proc
	Invalid
	adds	r12,-112,r12
	Invalid

;; pcomp_set_readline_variables: 40000000000E7780
pcomp_set_readline_variables proc
	Invalid
	Invalid
	(p07) addl	r14,-10172,r1

;; pcomp_set_compspec_options: 40000000000E77C0
pcomp_set_compspec_options proc
	Invalid
	Invalid
	Invalid

;; programmable_completions: 40000000000E78C0
programmable_completions proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; compspec_create: 40000000000E7C00
compspec_create proc
	Invalid
	Invalid
	adds	r34,0,r1

;; compspec_dispose: 40000000000E7CC0
compspec_dispose proc
	Invalid
	Invalid
	Invalid

;; compspec_copy: 40000000000E7F80
compspec_copy proc
	Invalid
	adds	r37,0,r1
	Invalid

;; progcomp_create: 40000000000E8480
progcomp_create proc
	Invalid
	Invalid
	addl	r32,8324,r1

;; progcomp_size: 40000000000E8500
progcomp_size proc
	Invalid
	addl	r14,8324,r1
	nop.i	0x0

;; progcomp_flush: 40000000000E8540
progcomp_flush proc
	Invalid
	addl	r14,8324,r1
	Invalid

;; progcomp_dispose: 40000000000E85C0
progcomp_dispose proc
	Invalid
	addl	r32,8324,r1
	Invalid

;; progcomp_remove: 40000000000E8640
progcomp_remove proc
	Invalid
	addl	r14,8324,r1
	Invalid

;; progcomp_insert: 40000000000E8780
progcomp_insert proc
	Invalid
	addl	r34,8324,r1
	Invalid

;; progcomp_search: 40000000000E8980
progcomp_search proc
	Invalid
	addl	r14,8324,r1
	Invalid

;; progcomp_walk: 40000000000E8A40
progcomp_walk proc
	Invalid
	addl	r14,8324,r1
	Invalid

;; xmalloc: 40000000000E8CC0
xmalloc proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; xrealloc: 40000000000E8E00
xrealloc proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; xfree: 40000000000E8F80
xfree proc
	Invalid
	Invalid
	Invalid

;; alias_builtin: 40000000000E9180
alias_builtin proc
	Invalid
	addl	r14,6456,r1
	Invalid

;; unalias_builtin: 40000000000E96C0
unalias_builtin proc
	Invalid
	adds	r38,0,r1
	Invalid

;; bind_builtin: 40000000000E98C0
bind_builtin proc
	Invalid
	addl	r14,6468,r1
	Invalid

;; break_builtin: 40000000000EA800
break_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; continue_builtin: 40000000000EA9C0
continue_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; builtin_builtin: 40000000000EAB80
builtin_builtin proc
	Invalid
	adds	r37,0,r1
	Invalid

;; caller_builtin: 40000000000EAD00
caller_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; cd_builtin: 40000000000EBD80
cd_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid
