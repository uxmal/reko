;;; Segment .text (400000000001C480)

;; array_to_word_list: 40000000000BC580
array_to_word_list proc
	Invalid
	adds	r14,16,r32
	Invalid

;; array_keys_to_word_list: 40000000000BC700
array_keys_to_word_list proc
	Invalid
	adds	r14,16,r32
	Invalid

;; array_assign_list: 40000000000BC8C0
array_assign_list proc
	Invalid
	adds	r38,0,r1
	Invalid

;; array_from_word_list: 40000000000BC980
array_from_word_list proc
	Invalid
	Invalid
	Invalid

;; array_to_argv: 40000000000BCA00
array_to_argv proc
	Invalid
	adds	r14,16,r32
	Invalid

;; array_to_assign: 40000000000BCBC0
array_to_assign proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; array_to_string: 40000000000BD400
array_to_string proc
	Invalid
	adds	r14,16,r32
	Invalid

;; array_modcase: 40000000000BD780
array_modcase proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; array_patsub: 40000000000BDBC0
array_patsub proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; array_subrange: 40000000000BE000
array_subrange proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; convert_var_to_array: 40000000000BE8C0
convert_var_to_array proc
	Invalid
	adds	r33,8,r32
	Invalid

;; convert_var_to_assoc: 40000000000BEA40
convert_var_to_assoc proc
	Invalid
	adds	r33,8,r32
	Invalid

;; bind_array_variable: 40000000000BEC00
bind_array_variable proc
	Invalid
	addl	r14,7172,r1
	Invalid

;; bind_array_element: 40000000000BEE00
bind_array_element proc
	Invalid
	adds	r14,0,r34
	adds	r36,0,r35

;; bind_assoc_variable: 40000000000BEE40
bind_assoc_variable proc
	Invalid
	adds	r14,40,r32
	Invalid

;; find_or_make_array_variable: 40000000000BEF40
find_or_make_array_variable proc
	Invalid
	adds	r36,0,r1
	Invalid

;; assign_array_var_from_word_list: 40000000000BF180
assign_array_var_from_word_list proc
	Invalid
	adds	r14,8,r32
	Invalid

;; expand_compound_array_assignment: 40000000000BF300
expand_compound_array_assignment proc
	Invalid
	adds	r12,-32,r12
	addl	r41,-9996,r1

;; print_array_assignment: 40000000000BFAC0
print_array_assignment proc
	Invalid
	adds	r14,8,r32
	Invalid

;; print_assoc_assignment: 40000000000BFBC0
print_assoc_assignment proc
	Invalid
	adds	r14,8,r32
	Invalid

;; valid_array_reference: 40000000000BFCC0
valid_array_reference proc
	Invalid
	Invalid
	Invalid

;; array_expand_index: 40000000000BFEC0
array_expand_index proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; unbind_array_element: 40000000000C0080
unbind_array_element proc
	Invalid
	adds	r37,0,r1
	Invalid

;; assign_compound_array_list: 40000000000C0400
assign_compound_array_list proc
	Invalid
	adds	r14,40,r32
	Invalid

;; assign_array_var_from_string: 40000000000C0CC0
assign_array_var_from_string proc
	Invalid
	Invalid
	Invalid

;; assign_array_from_string: 40000000000C0D80
assign_array_from_string proc
	Invalid
	Invalid
	Invalid

;; array_variable_name: 40000000000C0E40
array_variable_name proc
	Invalid
	Invalid
	Invalid

;; assign_array_element: 40000000000C1040
assign_array_element proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; array_variable_part: 40000000000C13C0
array_variable_part proc
	Invalid
	adds	r39,0,r1
	Invalid

;; array_value: 40000000000C1D80
array_value proc
	Invalid
	Invalid
	nop.i	0x0

;; get_array_value: 40000000000C1DC0
get_array_value proc
	Invalid
	Invalid
	adds	r15,0,r33

;; array_keys: 40000000000C1E40
array_keys proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; assoc_dispose: 40000000000C2380
assoc_dispose proc
	Invalid
	Invalid
	Invalid

;; assoc_flush: 40000000000C2400
assoc_flush proc
	Invalid
	Invalid
	Invalid

;; assoc_insert: 40000000000C2480
assoc_insert proc
	Invalid
	Invalid
	Invalid

;; assoc_remove: 40000000000C2600
assoc_remove proc
	Invalid
	Invalid
	Invalid

;; assoc_reference: 40000000000C2700
assoc_reference proc
	Invalid
	Invalid
	Invalid

;; assoc_quote: 40000000000C27C0
assoc_quote proc
	Invalid
	adds	r14,12,r32
	Invalid

;; assoc_quote_escapes: 40000000000C2940
assoc_quote_escapes proc
	Invalid
	adds	r14,12,r32
	Invalid

;; assoc_dequote: 40000000000C2AC0
assoc_dequote proc
	Invalid
	adds	r14,12,r32
	Invalid

;; assoc_dequote_escapes: 40000000000C2C40
assoc_dequote_escapes proc
	Invalid
	adds	r14,12,r32
	Invalid

;; assoc_remove_quoted_nulls: 40000000000C2DC0
assoc_remove_quoted_nulls proc
	Invalid
	adds	r14,12,r32
	Invalid

;; assoc_to_assign: 40000000000C2F40
assoc_to_assign proc
	Invalid
	adds	r14,12,r32
	Invalid

;; assoc_to_word_list: 40000000000C3800
assoc_to_word_list proc
	Invalid
	adds	r33,0,r0
	nop.i	0x0

;; assoc_subrange: 40000000000C3840
assoc_subrange proc
	Invalid
	adds	r14,12,r32
	Invalid

;; assoc_keys_to_word_list: 40000000000C3A80
assoc_keys_to_word_list proc
	Invalid
	addl	r33,1,r0
	nop.i	0x0

;; assoc_to_string: 40000000000C3AC0
assoc_to_string proc
	Invalid
	adds	r14,12,r32
	Invalid

;; assoc_modcase: 40000000000C3E00
assoc_modcase proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; assoc_patsub: 40000000000C4240
assoc_patsub proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; brace_expand: 40000000000C5380
brace_expand proc
	Invalid
	adds	r12,-80,r12
	Invalid

;; bash_brace_completion: 40000000000C7380
bash_brace_completion proc
	Invalid
	addl	r34,-10292,r1
	addl	r14,-10564,r1

;; bash_initialize_history: 40000000000C78C0
bash_initialize_history proc
	Invalid
	addl	r14,-10300,r1
	Invalid

;; bash_history_reinit: 40000000000C7980
bash_history_reinit proc
	addl	r14,5860,r1
	Invalid
	addl	r15,1,r0

;; bash_history_disable: 40000000000C7A00
bash_history_disable proc
	addl	r14,6116,r1
	Invalid
	addl	r15,1,r0

;; bash_history_enable: 40000000000C7A40
bash_history_enable proc
	Invalid
	addl	r14,6116,r1
	Invalid

;; load_history: 40000000000C7B00
load_history proc
	Invalid
	addl	r37,-4220,r1
	Invalid

;; bash_clear_history: 40000000000C7D00
bash_clear_history proc
	Invalid
	Invalid
	adds	r34,0,r1

;; bash_delete_histent: 40000000000C7D80
bash_delete_histent proc
	Invalid
	Invalid
	Invalid

;; bash_delete_last_history: 40000000000C7E40
bash_delete_last_history proc
	Invalid
	Invalid
	adds	r35,0,r1

;; maybe_append_history: 40000000000C8000
maybe_append_history proc
	Invalid
	adds	r12,-144,r12
	Invalid

;; maybe_save_shell_history: 40000000000C82C0
maybe_save_shell_history proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; bash_add_history: 40000000000C85C0
bash_add_history proc
	Invalid
	addl	r14,6108,r1
	Invalid

;; check_add_history: 40000000000C8A80
check_add_history proc
	Invalid
	addl	r41,9168,r1
	Invalid

;; maybe_add_history: 40000000000C9180
maybe_add_history proc
	Invalid
	addl	r14,8952,r1
	Invalid

;; pre_process_line: 40000000000C93C0
pre_process_line proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; history_number: 40000000000C99C0
history_number proc
	Invalid
	Invalid
	adds	r34,0,r1

;; setup_history_ignore: 40000000000C9A80
setup_history_ignore proc
	Invalid
	Invalid
	Invalid

;; last_history_line: 40000000000C9B00
last_history_line proc
	Invalid
	Invalid
	adds	r35,0,r1

;; command_word_completion_function: 40000000000CC2C0
command_word_completion_function proc
	Invalid
	adds	r12,-16,r12
	Invalid
