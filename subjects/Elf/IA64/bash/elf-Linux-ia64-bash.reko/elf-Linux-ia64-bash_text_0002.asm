;;; Segment .text (400000000001C480)

;; parse_string_to_word_list: 400000000003D740
parse_string_to_word_list proc
	Invalid
	addl	r43,6116,r1
	addl	r42,9156,r1

;; save_parser_state: 400000000003DC80
save_parser_state proc
	Invalid
	Invalid
	Invalid

;; restore_parser_state: 400000000003DF00
restore_parser_state proc
	Invalid
	adds	r14,0,r32
	Invalid

;; execute_variable_command: 400000000003E200
execute_variable_command proc
	Invalid
	adds	r12,-96,r12
	Invalid

;; save_input_line_state: 400000000003E4C0
save_input_line_state proc
	Invalid
	Invalid
	Invalid

;; restore_input_line_state: 400000000003E5C0
restore_input_line_state proc
	Invalid
	addl	r33,6788,r1
	Invalid

;; xparse_dolparen: 400000000003E6C0
xparse_dolparen proc
	Invalid
	adds	r12,-128,r12
	Invalid

;; posix_initialize: 400000000003EFC0
posix_initialize proc
	Invalid
	Invalid
	addl	r14,1,r0

;; string_to_rlimtype: 400000000003F080
string_to_rlimtype proc
	Invalid
	Invalid
	nop.i	0x0

;; print_rlimtype: 400000000003F2C0
print_rlimtype proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; all_digits: 400000000003F400
all_digits proc
	Invalid
	adds	r32,1,r32
	Invalid

;; legal_number: 400000000003F480
legal_number proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; legal_identifier: 400000000003F640
legal_identifier proc
	Invalid
	Invalid
	nop.i	0x0

;; check_identifier: 400000000003F7C0
check_identifier proc
	Invalid
	adds	r14,8,r32
	Invalid

;; legal_alias_name: 400000000003F940
legal_alias_name proc
	Invalid
	addl	r16,-18556,r1
	adds	r32,1,r32

;; assignment: 400000000003FAC0
assignment proc
	Invalid
	adds	r37,0,r1
	Invalid

;; sh_unset_nodelay_mode: 400000000003FE40
sh_unset_nodelay_mode proc
	Invalid
	Invalid
	Invalid

;; sh_validfd: 400000000003FF40
sh_validfd proc
	Invalid
	Invalid
	Invalid

;; fd_ispipe: 400000000003FFC0
fd_ispipe proc
	Invalid
	Invalid
	adds	r36,0,r1

;; check_dev_tty: 4000000000040080
check_dev_tty proc
	Invalid
	addl	r35,-5612,r1
	Invalid

;; same_file: 40000000000401C0
same_file proc
	Invalid
	adds	r12,-288,r12
	Invalid

;; move_to_high_fd: 4000000000040340
move_to_high_fd proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; check_binary_file: 4000000000040600
check_binary_file proc
	Invalid
	Invalid
	nop.i	0x0

;; sh_openpipe: 40000000000406C0
sh_openpipe proc
	Invalid
	Invalid
	Invalid

;; sh_closepipe: 40000000000407C0
sh_closepipe proc
	Invalid
	Invalid
	Invalid

;; file_exists: 4000000000040880
file_exists proc
	Invalid
	adds	r12,-144,r12
	Invalid

;; file_isdir: 4000000000040900
file_isdir proc
	Invalid
	adds	r12,-144,r12
	Invalid

;; file_iswdir: 40000000000409C0
file_iswdir proc
	Invalid
	Invalid
	Invalid

;; dot_or_dotdot: 4000000000040A80
dot_or_dotdot proc
	Invalid
	nop.i	0x0
	Invalid

;; absolute_pathname: 4000000000040B80
absolute_pathname proc
	Invalid
	nop.i	0x0
	Invalid

;; absolute_program: 4000000000040C80
absolute_program proc
	Invalid
	Invalid
	Invalid

;; make_absolute: 4000000000040D00
make_absolute proc
	Invalid
	adds	r36,0,r1
	Invalid

;; base_pathname: 4000000000040E00
base_pathname proc
	Invalid
	Invalid
	Invalid

;; polite_directory_format: 4000000000040F00
polite_directory_format proc
	Invalid
	addl	r38,-5604,r1
	Invalid

;; trim_pathname: 4000000000041080
trim_pathname proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; extract_colon_unit: 40000000000414C0
extract_colon_unit proc
	Invalid
	Invalid
	Invalid

;; tilde_initialize: 4000000000041740
tilde_initialize proc
	Invalid
	addl	r14,6852,r1
	Invalid

;; bash_tilde_find_word: 4000000000041940
bash_tilde_find_word proc
	Invalid
	Invalid
	Invalid

;; bash_tilde_expand: 4000000000041BC0
bash_tilde_expand proc
	Invalid
	addl	r34,7672,r1
	Invalid

;; full_pathname: 4000000000041E80
full_pathname proc
	Invalid
	Invalid
	Invalid

;; group_member: 4000000000041FC0
group_member proc
	Invalid
	addl	r14,-22276,r1
	Invalid

;; get_group_list: 4000000000042140
get_group_list proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; get_group_array: 4000000000042380
get_group_array proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; cmd_init: 40000000000425C0
cmd_init proc
	Invalid
	Invalid
	adds	r35,0,r1

;; alloc_word_desc: 4000000000042680
alloc_word_desc proc
	Invalid
	addl	r16,14044,r1
	Invalid

;; make_bare_word: 4000000000042780
make_bare_word proc
	Invalid
	Invalid
	adds	r36,0,r1

;; make_word_flags: 4000000000042880
make_word_flags proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; make_word: 4000000000042B40
make_word proc
	Invalid
	Invalid
	Invalid

;; make_word_from_token: 4000000000042BC0
make_word_from_token proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; make_word_list: 4000000000042C40
make_word_list proc
	Invalid
	addl	r15,14028,r1
	Invalid

;; make_command: 4000000000042E00
make_command proc
	Invalid
	Invalid
	adds	r36,0,r1

;; command_connect: 4000000000042E80
command_connect proc
	Invalid
	Invalid
	Invalid

;; make_for_command: 4000000000042F40
make_for_command proc
	Invalid
	Invalid
	Invalid

;; make_select_command: 4000000000043000
make_select_command proc
	Invalid
	Invalid
	Invalid

;; make_arith_for_command: 40000000000430C0
make_arith_for_command proc
	Invalid
	adds	r41,8,r32
	Invalid

;; make_group_command: 4000000000043680
make_group_command proc
	Invalid
	Invalid
	Invalid

;; make_case_command: 4000000000043700
make_case_command proc
	Invalid
	Invalid
	Invalid

;; make_pattern_list: 4000000000043800
make_pattern_list proc
	Invalid
	Invalid
	adds	r37,0,r1

;; make_if_command: 4000000000043900
make_if_command proc
	Invalid
	Invalid
	Invalid

;; make_while_command: 40000000000439C0
make_while_command proc
	Invalid
	Invalid
	Invalid

;; make_until_command: 4000000000043A80
make_until_command proc
	Invalid
	Invalid
	Invalid

;; make_arith_command: 4000000000043B40
make_arith_command proc
	Invalid
	Invalid
	adds	r36,0,r1

;; make_cond_node: 4000000000043C40
make_cond_node proc
	Invalid
	Invalid
	adds	r38,0,r1

;; make_cond_command: 4000000000043D00
make_cond_command proc
	Invalid
	Invalid
	adds	r35,0,r1

;; make_bare_simple_command: 4000000000043DC0
make_bare_simple_command proc
	Invalid
	Invalid
	adds	r35,0,r1

;; make_simple_command: 4000000000043EC0
make_simple_command proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; make_here_document: 4000000000044100
make_here_document proc
	Invalid
	adds	r34,24,r32
	Invalid

;; make_redirection: 40000000000447C0
make_redirection proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; make_function_def: 4000000000044B40
make_function_def proc
	Invalid
	Invalid
	Invalid

;; make_subshell_command: 4000000000044D00
make_subshell_command proc
	Invalid
	Invalid
	Invalid

;; make_coproc_command: 4000000000044DC0
make_coproc_command proc
	Invalid
	Invalid
	Invalid

;; clean_simple_command: 4000000000044EC0
clean_simple_command proc
	Invalid
	Invalid
	Invalid

;; connect_async_list: 40000000000450C0
connect_async_list proc
	Invalid
	adds	r14,24,r32
	Invalid

;; print_word_list: 40000000000477C0
print_word_list proc
	Invalid
	adds	r36,0,r1
	Invalid

;; xtrace_set: 4000000000047880
xtrace_set proc
	Invalid
	Invalid
	Invalid

;; xtrace_init: 4000000000047A80
xtrace_init proc
	Invalid
	Invalid
	addl	r14,-10652,r1

;; xtrace_reset: 4000000000047B00
xtrace_reset proc
	Invalid
	addl	r33,5740,r1
	Invalid

;; xtrace_fdchk: 4000000000047C40
xtrace_fdchk proc
	addl	r14,5740,r1
	Invalid
	nop.i	0x0

;; indirection_level_string: 4000000000047C80
indirection_level_string proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; xtrace_print_assignment: 4000000000048140
xtrace_print_assignment proc
	Invalid
	addl	r36,6932,r1
	Invalid

;; xtrace_print_word_list: 4000000000048480
xtrace_print_word_list proc
	Invalid
	addl	r36,6932,r1
	Invalid

;; print_for_command_head: 40000000000488C0
print_for_command_head proc
	Invalid
	adds	r14,8,r32
	Invalid

;; xtrace_print_for_command_head: 4000000000048980
xtrace_print_for_command_head proc
	Invalid
	addl	r35,6932,r1
	Invalid

;; print_select_command_head: 4000000000048A80
print_select_command_head proc
	Invalid
	adds	r14,8,r32
	Invalid

;; xtrace_print_select_command_head: 4000000000048B40
xtrace_print_select_command_head proc
	Invalid
	addl	r35,6932,r1
	Invalid

;; print_case_command_head: 4000000000048C40
print_case_command_head proc
	Invalid
	adds	r14,8,r32
	addl	r32,-5100,r1

;; xtrace_print_case_command_head: 4000000000048CC0
xtrace_print_case_command_head proc
	Invalid
	addl	r34,6932,r1
	Invalid

;; print_arith_command: 4000000000048DC0
print_arith_command proc
	Invalid
	addl	r36,-5084,r1
	Invalid

;; print_cond_command: 4000000000048E80
print_cond_command proc
	Invalid
	adds	r33,0,r32
	Invalid

;; xtrace_print_cond_term: 4000000000048F00
xtrace_print_cond_term proc
	Invalid
	addl	r37,6932,r1
	Invalid

;; xtrace_print_arith_cmd: 4000000000049380
xtrace_print_arith_cmd proc
	Invalid
	addl	r34,6932,r1
	Invalid

;; print_simple_command: 4000000000049540
print_simple_command proc
	Invalid
	adds	r33,16,r32
	Invalid

;; make_command_string: 400000000004B0C0
make_command_string proc
	Invalid
	addl	r14,6956,r1
	Invalid

;; print_command: 400000000004B140
print_command proc
	Invalid
	addl	r14,6940,r1
	Invalid

;; named_function_string: 400000000004B1C0
named_function_string proc
	Invalid
	addl	r14,6956,r1
	Invalid

;; dispose_word: 400000000004B6C0
dispose_word proc
	Invalid
	Invalid
	Invalid

;; dispose_cond_node: 400000000004B8C0
dispose_cond_node proc
	Invalid
	adds	r14,24,r32
	Invalid

;; dispose_word_desc: 400000000004BA00
dispose_word_desc proc
	Invalid
	addl	r15,14044,r1
	Invalid

;; dispose_words: 400000000004BBC0
dispose_words proc
	Invalid
	addl	r38,14028,r1
	Invalid

;; dispose_redirects: 400000000004BE00
dispose_redirects proc
	Invalid
	adds	r39,0,r1
	Invalid

;; dispose_command: 400000000004BFC0
dispose_command proc
	Invalid
	adds	r14,16,r32
	Invalid
