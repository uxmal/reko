;;; Segment .text (400000000001C480)

;; signal_name: 40000000000AD3C0
signal_name proc
	Invalid
	addl	r14,-20604,r1
	Invalid

;; decode_signal: 40000000000AD480
decode_signal proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; set_sigint_handler: 40000000000AD780
set_sigint_handler proc
	Invalid
	addl	r14,19268,r1
	Invalid

;; trap_to_sighandler: 40000000000AD900
trap_to_sighandler proc
	addl	r14,19268,r1
	Invalid
	Invalid

;; set_signal: 40000000000AD980
set_signal proc
	Invalid
	Invalid
	Invalid

;; set_return_trap: 40000000000ADDC0
set_return_trap proc
	Invalid
	adds	r33,0,r32
	addl	r32,67,r0

;; set_error_trap: 40000000000ADE00
set_error_trap proc
	Invalid
	adds	r33,0,r32
	addl	r32,66,r0

;; set_debug_trap: 40000000000ADE40
set_debug_trap proc
	Invalid
	adds	r33,0,r32
	addl	r32,65,r0

;; maybe_set_sigchld_trap: 40000000000ADE80
maybe_set_sigchld_trap proc
	Invalid
	addl	r14,19268,r1
	adds	r33,0,r32

;; get_all_original_signals: 40000000000ADF40
get_all_original_signals proc
	Invalid
	addl	r14,6012,r1
	Invalid

;; set_original_signal: 40000000000AE080
set_original_signal proc
	addl	r15,23796,r1
	Invalid
	Invalid

;; restore_default_signal: 40000000000AE140
restore_default_signal proc
	Invalid
	Invalid
	Invalid

;; set_impossible_sigchld_trap: 40000000000AE4C0
set_impossible_sigchld_trap proc
	Invalid
	Invalid
	adds	r34,0,r1

;; ignore_signal: 40000000000AE580
ignore_signal proc
	Invalid
	Invalid
	Invalid

;; run_exit_trap: 40000000000AE800
run_exit_trap proc
	Invalid
	adds	r16,8,r12
	adds	r12,-48,r12

;; run_trap_cleanup: 40000000000AEC80
run_trap_cleanup proc
	addl	r14,19268,r1
	Invalid
	Invalid

;; run_debug_trap: 40000000000AECC0
run_debug_trap proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; run_error_trap: 40000000000AEF00
run_error_trap proc
	Invalid
	Invalid
	addl	r14,19268,r1

;; run_return_trap: 40000000000AEF80
run_return_trap proc
	Invalid
	addl	r14,19268,r1
	Invalid

;; run_interrupt_trap: 40000000000AF040
run_interrupt_trap proc
	Invalid
	Invalid
	addl	r33,-4308,r1

;; run_pending_traps: 40000000000AF080
run_pending_traps proc
	Invalid
	adds	r12,-256,r12
	Invalid

;; trap_handler: 40000000000AF640
trap_handler proc
	Invalid
	addl	r15,19268,r1
	Invalid

;; free_trap_strings: 40000000000AF8C0
free_trap_strings proc
	Invalid
	addl	r32,19268,r1
	Invalid

;; reset_signal_handlers: 40000000000AF9C0
reset_signal_handlers proc
	Invalid
	Invalid
	addl	r32,-4388,r1

;; restore_original_signals: 40000000000AFA00
restore_original_signals proc
	Invalid
	Invalid
	addl	r32,-4364,r1

;; maybe_call_trap_handler: 40000000000AFA40
maybe_call_trap_handler proc
	Invalid
	addl	r14,19268,r1
	Invalid

;; signal_is_trapped: 40000000000AFC00
signal_is_trapped proc
	addl	r14,19268,r1
	Invalid
	Invalid

;; signal_is_special: 40000000000AFC40
signal_is_special proc
	addl	r14,19268,r1
	Invalid
	Invalid

;; signal_is_ignored: 40000000000AFC80
signal_is_ignored proc
	addl	r14,19268,r1
	Invalid
	Invalid

;; signal_is_hard_ignored: 40000000000AFCC0
signal_is_hard_ignored proc
	addl	r14,19268,r1
	Invalid
	Invalid

;; set_signal_ignored: 40000000000AFD00
set_signal_ignored proc
	addl	r14,19268,r1
	Invalid
	addl	r16,23796,r1

;; signal_in_progress: 40000000000AFD80
signal_in_progress proc
	addl	r14,19268,r1
	Invalid
	Invalid

;; buffered_ungetchar: 40000000000AFDC0
buffered_ungetchar proc
	addl	r14,7636,r1
	Invalid
	adds	r8,0,r32

;; buffered_getchar: 40000000000AFE80
buffered_getchar proc
	Invalid
	addl	r34,7676,r1
	Invalid

;; getc_with_restart: 40000000000B0380
getc_with_restart proc
	Invalid
	addl	r34,7676,r1
	Invalid

;; ungetc_with_restart: 40000000000B0700
ungetc_with_restart proc
	addl	r15,7628,r1
	Invalid
	adds	r8,0,r32

;; set_bash_input_fd: 40000000000B0780
set_bash_input_fd proc
	addl	r14,22532,r1
	Invalid
	nop.i	0x0

;; fd_is_bash_input: 40000000000B0840
fd_is_bash_input proc
	addl	r14,22532,r1
	Invalid
	adds	r8,0,r0

;; fd_to_buffered_stream: 40000000000B0940
fd_to_buffered_stream proc
	Invalid
	adds	r12,-144,r12
	Invalid

;; open_buffered_stream: 40000000000B0C00
open_buffered_stream proc
	Invalid
	Invalid
	Invalid

;; free_buffered_stream: 40000000000B0CC0
free_buffered_stream proc
	Invalid
	adds	r14,0,r32
	Invalid

;; duplicate_buffered_stream: 40000000000B0D80
duplicate_buffered_stream proc
	Invalid
	Invalid
	Invalid

;; close_buffered_stream: 40000000000B1100
close_buffered_stream proc
	Invalid
	Invalid
	Invalid

;; close_buffered_fd: 40000000000B11C0
close_buffered_fd proc
	Invalid
	addl	r14,7644,r1
	Invalid

;; set_buffered_stream: 40000000000B1300
set_buffered_stream proc
	addl	r14,7636,r1
	Invalid
	Invalid

;; sync_buffered_stream: 40000000000B1340
sync_buffered_stream proc
	Invalid
	addl	r14,7636,r1
	Invalid

;; save_bash_input: 40000000000B1480
save_bash_input proc
	Invalid
	addl	r35,7636,r1
	Invalid

;; check_bash_input: 40000000000B1800
check_bash_input proc
	Invalid
	addl	r14,22532,r1
	Invalid

;; with_input_from_buffered_stream: 40000000000B19C0
with_input_from_buffered_stream proc
	Invalid
	adds	r38,0,r32
	Invalid

;; discard_unwind_frame: 40000000000B1D80
discard_unwind_frame proc
	Invalid
	addl	r36,7672,r1
	Invalid

;; run_unwind_frame: 40000000000B1F40
run_unwind_frame proc
	Invalid
	addl	r33,7672,r1
	Invalid

;; add_unwind_protect: 40000000000B2000
add_unwind_protect proc
	Invalid
	addl	r34,7672,r1
	Invalid

;; begin_unwind_frame: 40000000000B20C0
begin_unwind_frame proc
	Invalid
	adds	r33,0,r32
	adds	r32,0,r0

;; remove_unwind_protect: 40000000000B2100
remove_unwind_protect proc
	Invalid
	addl	r32,7672,r1
	Invalid

;; run_unwind_protects: 40000000000B21C0
run_unwind_protects proc
	Invalid
	addl	r32,7672,r1
	Invalid

;; clear_unwind_protect_list: 40000000000B2280
clear_unwind_protect_list proc
	Invalid
	addl	r34,7652,r1
	Invalid

;; have_unwind_protects: 40000000000B2380
have_unwind_protects proc
	addl	r14,7652,r1
	Invalid
	nop.i	0x0

;; unwind_protect_mem: 40000000000B23C0
unwind_protect_mem proc
	Invalid
	addl	r35,7672,r1
	Invalid

;; unquoted_glob_pattern_p: 40000000000B26C0
unquoted_glob_pattern_p proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; glob_char_p: 40000000000B2A40
glob_char_p proc
	Invalid
	addl	r16,1,r0
	adds	r14,-33,r14

;; quote_string_for_globbing: 40000000000B2B40
quote_string_for_globbing proc
	Invalid
	Invalid
	Invalid

;; quote_globbing_chars: 40000000000B2F00
quote_globbing_chars proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; should_ignore_glob_matches: 40000000000B32C0
should_ignore_glob_matches proc
	addl	r14,-20052,r1
	Invalid
	nop.i	0x0

;; ignore_glob_matches: 40000000000B3300
ignore_glob_matches proc
	Invalid
	addl	r14,-20052,r1
	Invalid

;; shell_glob_filename: 40000000000B3600
shell_glob_filename proc
	Invalid
	addl	r14,9140,r1
	Invalid

;; setup_ignore_patterns: 40000000000B3840
setup_ignore_patterns proc
	Invalid
	adds	r45,0,r1
	Invalid

;; setup_glob_ignore: 40000000000B3D80
setup_glob_ignore proc
	Invalid
	addl	r33,-20052,r1
	Invalid

;; sigwinch_sighandler: 40000000000B3E80
sigwinch_sighandler proc
	addl	r15,1,r0
	Invalid
	addl	r14,7680,r1

;; initialize_terminating_signals: 40000000000B3EC0
initialize_terminating_signals proc
	Invalid
	adds	r12,-288,r12
	Invalid

;; reset_terminating_signals: 40000000000B41C0
reset_terminating_signals proc
	Invalid
	adds	r12,-144,r12
	Invalid

;; top_level_cleanup: 40000000000B4380
top_level_cleanup proc
	Invalid
	addl	r32,8416,r1
	Invalid

;; jump_to_top_level: 40000000000B44C0
jump_to_top_level proc
	Invalid
	addl	r36,25252,r1
	Invalid

;; throw_to_top_level: 40000000000B4500
throw_to_top_level proc
	Invalid
	addl	r14,7684,r1
	Invalid

;; sigint_sighandler: 40000000000B4A40
sigint_sighandler proc
	addl	r14,7684,r1
	Invalid
	addl	r15,7672,r1

;; set_signal_handler: 40000000000B4B00
set_signal_handler proc
	Invalid
	adds	r12,-288,r12
	Invalid

;; unset_sigwinch_handler: 40000000000B4BC0
unset_sigwinch_handler proc
	Invalid
	Invalid
	addl	r14,7700,r1

;; set_sigwinch_handler: 40000000000B4C00
set_sigwinch_handler proc
	Invalid
	addl	r36,-10052,r1
	Invalid

;; termsig_handler: 40000000000B4C80
termsig_handler proc
	Invalid
	addl	r14,7692,r1
	Invalid

;; termsig_sighandler: 40000000000B5080
termsig_sighandler proc
	adds	r14,-1,r32
	Invalid
	nop.i	0x0

;; initialize_signals: 40000000000B52C0
initialize_signals proc
	Invalid
	addl	r32,6516,r1
	Invalid

;; binary_test: 40000000000B5C00
binary_test proc
	Invalid
	Invalid
	Invalid

;; unary_test: 40000000000B6400
unary_test proc
	Invalid
	adds	r12,-160,r12
	Invalid

;; test_binop: 40000000000B6D00
test_binop proc
	Invalid
	Invalid
	nop.i	0x0

;; test_unop: 40000000000B7300
test_unop proc
	Invalid
	adds	r8,0,r0
	Invalid

;; test_command: 40000000000B8540
test_command proc
	Invalid
	adds	r16,8,r12
	Invalid

;; shell_version_string: 40000000000B8C00
shell_version_string proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; show_shell_version: 40000000000B8D00
show_shell_version proc
	Invalid
	addl	r38,-8860,r1
	Invalid

;; initialize_aliases: 40000000000B91C0
initialize_aliases proc
	Invalid
	Invalid
	addl	r32,7740,r1

;; find_alias: 40000000000B9240
find_alias proc
	Invalid
	addl	r14,7740,r1
	Invalid

;; get_alias_value: 40000000000B9300
get_alias_value proc
	Invalid
	addl	r14,7740,r1
	Invalid

;; add_alias: 40000000000B93C0
add_alias proc
	Invalid
	addl	r36,7740,r1
	Invalid

;; remove_alias: 40000000000B9700
remove_alias proc
	Invalid
	addl	r34,7740,r1
	Invalid

;; delete_all_aliases: 40000000000B9840
delete_all_aliases proc
	Invalid
	addl	r32,7740,r1
	Invalid

;; all_aliases: 40000000000B9940
all_aliases proc
	Invalid
	addl	r32,7740,r1
	Invalid

;; alias_expand_word: 40000000000B9B40
alias_expand_word proc
	Invalid
	Invalid
	Invalid

;; alias_expand: 40000000000B9C00
alias_expand proc
	Invalid
	adds	r50,0,r1
	Invalid

;; array_walk: 40000000000BA880
array_walk proc
	Invalid
	adds	r14,16,r32
	Invalid

;; array_quote: 40000000000BA980
array_quote proc
	Invalid
	adds	r36,24,r32
	Invalid

;; array_quote_escapes: 40000000000BAB00
array_quote_escapes proc
	Invalid
	adds	r36,24,r32
	Invalid

;; array_dequote: 40000000000BAC80
array_dequote proc
	Invalid
	adds	r36,24,r32
	Invalid

;; array_dequote_escapes: 40000000000BAE00
array_dequote_escapes proc
	Invalid
	adds	r36,24,r32
	Invalid

;; array_remove_quoted_nulls: 40000000000BAF80
array_remove_quoted_nulls proc
	Invalid
	adds	r35,24,r32
	Invalid

;; array_create_element: 40000000000BB0C0
array_create_element proc
	Invalid
	Invalid
	adds	r37,0,r1

;; array_rshift: 40000000000BB1C0
array_rshift proc
	Invalid
	Invalid
	Invalid

;; array_shift_element: 40000000000BB400
array_shift_element proc
	Invalid
	adds	r34,0,r33
	addl	r33,1,r0

;; array_create: 40000000000BB440
array_create proc
	Invalid
	Invalid
	adds	r35,0,r1

;; array_slice: 40000000000BB500
array_slice proc
	Invalid
	Invalid
	adds	r41,0,r1

;; array_copy: 40000000000BB680
array_copy proc
	Invalid
	Invalid
	Invalid

;; array_dispose_element: 40000000000BB800
array_dispose_element proc
	Invalid
	adds	r14,8,r32
	Invalid

;; array_flush: 40000000000BB8C0
array_flush proc
	Invalid
	adds	r34,24,r32
	Invalid

;; array_shift: 40000000000BBA00
array_shift proc
	Invalid
	adds	r18,16,r32
	Invalid

;; array_unshift_element: 40000000000BBD00
array_unshift_element proc
	Invalid
	addl	r33,1,r0
	adds	r34,0,r0

;; array_dispose: 40000000000BBD40
array_dispose proc
	Invalid
	Invalid
	Invalid

;; array_insert: 40000000000BBE00
array_insert proc
	Invalid
	Invalid
	Invalid

;; array_remove: 40000000000BC1C0
array_remove proc
	adds	r16,16,r32
	Invalid
	adds	r14,24,r32

;; array_reference: 40000000000BC380
array_reference proc
	adds	r14,16,r32
	Invalid
	nop.i	0x0
