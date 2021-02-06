;;; Segment .text (400000000001C480)

;; execute_command: 400000000005E580
execute_command proc
	Invalid
	addl	r14,7012,r1
	Invalid

;; sv_histignore: 400000000005F2C0
sv_histignore proc
	Invalid
	Invalid
	Invalid

;; sv_globignore: 400000000005F580
sv_globignore proc
	Invalid
	addl	r14,7344,r1
	Invalid

;; sv_mail: 400000000005F600
sv_mail proc
	Invalid
	adds	r32,4,r32
	Invalid

;; sv_path: 400000000005F6C0
sv_path proc
	Invalid
	Invalid
	adds	r34,0,r1

;; sh_get_home_dir: 4000000000061700
sh_get_home_dir proc
	Invalid
	addl	r32,-22276,r1
	Invalid

;; print_var_value: 4000000000061780
print_var_value proc
	Invalid
	adds	r32,8,r32
	Invalid

;; print_var_function: 40000000000619C0
print_var_function proc
	Invalid
	adds	r14,40,r32
	Invalid

;; print_assignment: 4000000000061AC0
print_assignment proc
	Invalid
	adds	r14,8,r32
	Invalid

;; print_var_list: 4000000000061CC0
print_var_list proc
	Invalid
	Invalid
	Invalid

;; print_func_list: 4000000000061DC0
print_func_list proc
	Invalid
	addl	r38,-2004,r1
	Invalid

;; get_random_number: 4000000000061EC0
get_random_number proc
	Invalid
	Invalid
	Invalid

;; var_lookup: 4000000000062200
var_lookup proc
	Invalid
	adds	r37,0,r1
	Invalid

;; find_variable_internal: 40000000000622C0
find_variable_internal proc
	Invalid
	Invalid
	Invalid

;; find_global_variable: 4000000000062480
find_global_variable proc
	Invalid
	addl	r14,7180,r1
	Invalid

;; find_variable: 4000000000062540
find_variable proc
	Invalid
	Invalid
	addl	r14,9208,r1

;; sv_xtracefd: 4000000000062600
sv_xtracefd proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; sv_strict_posix: 4000000000062880
sv_strict_posix proc
	Invalid
	addl	r33,6456,r1
	Invalid

;; sv_ignoreeof: 4000000000062980
sv_ignoreeof proc
	Invalid
	addl	r14,6620,r1
	Invalid

;; sv_histtimefmt: 4000000000062B00
sv_histtimefmt proc
	Invalid
	Invalid
	Invalid

;; sv_hostfile: 4000000000062B80
sv_hostfile proc
	Invalid
	Invalid
	Invalid

;; sv_comp_wordbreaks: 4000000000062C40
sv_comp_wordbreaks proc
	Invalid
	Invalid
	Invalid

;; sv_funcnest: 4000000000062D00
sv_funcnest proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; sv_ifs: 4000000000062E00
sv_ifs proc
	Invalid
	addl	r35,-1972,r1
	Invalid

;; make_funcname_visible: 4000000000062E80
make_funcname_visible proc
	Invalid
	addl	r36,-1964,r1
	Invalid

;; find_function: 4000000000062F40
find_function proc
	Invalid
	Invalid
	addl	r14,7164,r1

;; find_function_def: 4000000000062F80
find_function_def proc
	Invalid
	Invalid
	addl	r14,7156,r1

;; get_variable_value: 4000000000062FC0
get_variable_value proc
	Invalid
	adds	r14,40,r32
	Invalid

;; get_string_value: 40000000000630C0
get_string_value proc
	Invalid
	Invalid
	Invalid

;; sv_locale: 4000000000063180
sv_locale proc
	Invalid
	adds	r35,0,r1
	Invalid

;; sv_opterr: 4000000000063280
sv_opterr proc
	Invalid
	addl	r35,-1948,r1
	Invalid

;; sv_optind: 4000000000063380
sv_optind proc
	Invalid
	addl	r35,-1940,r1
	Invalid

;; sv_histchars: 4000000000063480
sv_histchars proc
	Invalid
	Invalid
	Invalid

;; sv_history_control: 4000000000063600
sv_history_control proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; sv_histsize: 4000000000063900
sv_histsize proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; sv_terminal: 4000000000063B40
sv_terminal proc
	Invalid
	addl	r14,6512,r1
	Invalid

;; sh_get_env_value: 4000000000063C40
sh_get_env_value proc
	Invalid
	nop.i	0x0
	Invalid

;; make_variable_value: 4000000000063C80
make_variable_value proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; bind_variable_value: 4000000000064580
bind_variable_value proc
	Invalid
	adds	r35,40,r32
	Invalid

;; bind_function: 4000000000064800
bind_function proc
	Invalid
	adds	r38,0,r1
	Invalid

;; bind_function_def: 4000000000064A80
bind_function_def proc
	Invalid
	Invalid
	Invalid

;; dispose_variable: 4000000000064C00
dispose_variable proc
	Invalid
	adds	r33,40,r32
	Invalid

;; unbind_func: 4000000000064D80
unbind_func proc
	Invalid
	addl	r14,7164,r1
	Invalid

;; unbind_function_def: 4000000000064F00
unbind_function_def proc
	Invalid
	addl	r14,7156,r1
	Invalid

;; delete_all_variables: 4000000000065000
delete_all_variables proc
	Invalid
	Invalid
	Invalid

;; kill_all_local_variables: 4000000000065080
kill_all_local_variables proc
	Invalid
	addl	r14,7172,r1
	Invalid

;; map_over: 4000000000065240
map_over proc
	Invalid
	adds	r38,0,r1
	Invalid

;; map_over_funcs: 40000000000653C0
map_over_funcs proc
	Invalid
	addl	r34,7164,r1
	Invalid

;; sort_variables: 40000000000654C0
sort_variables proc
	Invalid
	Invalid
	Invalid

;; all_shell_variables: 4000000000065600
all_shell_variables proc
	Invalid
	adds	r32,0,r0
	nop.i	0x0

;; all_shell_functions: 4000000000065640
all_shell_functions proc
	Invalid
	Invalid
	adds	r35,0,r1

;; all_visible_functions: 40000000000656C0
all_visible_functions proc
	Invalid
	addl	r36,-2268,r1
	Invalid

;; all_visible_variables: 4000000000065780
all_visible_variables proc
	Invalid
	Invalid
	addl	r32,-2268,r1

;; all_exported_variables: 40000000000657C0
all_exported_variables proc
	Invalid
	Invalid
	addl	r32,-2260,r1

;; local_exported_variables: 4000000000065800
local_exported_variables proc
	Invalid
	Invalid
	addl	r32,-2244,r1

;; all_local_variables: 4000000000065840
all_local_variables proc
	Invalid
	addl	r14,7172,r1
	Invalid

;; all_array_variables: 4000000000065AC0
all_array_variables proc
	Invalid
	Invalid
	addl	r32,-2228,r1

;; all_variables_matching_prefix: 4000000000065B00
all_variables_matching_prefix proc
	Invalid
	Invalid
	Invalid

;; find_tempenv_variable: 4000000000065EC0
find_tempenv_variable proc
	Invalid
	Invalid
	addl	r14,7140,r1

;; add_or_supercede_exported_var: 4000000000065F40
add_or_supercede_exported_var proc
	Invalid
	addl	r39,7116,r1
	Invalid

;; update_export_env_inplace: 4000000000066600
update_export_env_inplace proc
	Invalid
	Invalid
	nop.i	0x0

;; put_command_name_into_env: 4000000000066840
put_command_name_into_env proc
	Invalid
	adds	r34,0,r32
	addl	r32,-1860,r1

;; new_var_context: 4000000000066880
new_var_context proc
	Invalid
	Invalid
	adds	r37,0,r1

;; make_new_assoc_variable: 4000000000067980
make_new_assoc_variable proc
	Invalid
	addl	r14,7180,r1
	Invalid

;; make_new_array_variable: 4000000000067A40
make_new_array_variable proc
	Invalid
	addl	r14,7180,r1
	Invalid

;; make_local_variable: 4000000000067BC0
make_local_variable proc
	Invalid
	Invalid
	Invalid

;; make_local_assoc_variable: 40000000000681C0
make_local_assoc_variable proc
	Invalid
	Invalid
	Invalid

;; make_local_array_variable: 40000000000682C0
make_local_array_variable proc
	Invalid
	Invalid
	Invalid

;; bind_variable: 40000000000683C0
bind_variable proc
	Invalid
	addl	r37,7172,r1
	Invalid

;; set_var_read_only: 4000000000068680
set_var_read_only proc
	Invalid
	adds	r35,0,r1
	Invalid

;; bind_int_variable: 40000000000687C0
bind_int_variable proc
	Invalid
	adds	r37,0,r1
	Invalid

;; bind_var_to_int: 4000000000068980
bind_var_to_int proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; sh_set_lines_and_columns: 4000000000068A00
sh_set_lines_and_columns proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; set_ppid: 4000000000068B00
set_ppid proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; set_pwd: 4000000000068C00
set_pwd proc
	Invalid
	addl	r37,-1820,r1
	Invalid

;; adjust_shell_level: 4000000000069000
adjust_shell_level proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; set_if_not: 4000000000069580
set_if_not proc
	Invalid
	addl	r14,7172,r1
	Invalid

;; maybe_make_export_env: 40000000000696C0
maybe_make_export_env proc
	Invalid
	addl	r34,5780,r1
	Invalid

;; chkexport: 4000000000069B80
chkexport proc
	Invalid
	Invalid
	Invalid

;; sv_tz: 4000000000069C80
sv_tz proc
	Invalid
	Invalid
	Invalid

;; dispose_var_context: 4000000000069D40
dispose_var_context proc
	Invalid
	Invalid
	Invalid

;; push_var_context: 4000000000069E40
push_var_context proc
	Invalid
	Invalid
	Invalid

;; pop_var_context: 4000000000069F40
pop_var_context proc
	Invalid
	addl	r15,7172,r1
	Invalid

;; delete_all_contexts: 400000000006A100
delete_all_contexts proc
	Invalid
	addl	r34,7180,r1
	Invalid

;; push_scope: 400000000006A200
push_scope proc
	Invalid
	adds	r14,0,r32
	adds	r34,0,r33

;; pop_scope: 400000000006A240
pop_scope proc
	Invalid
	addl	r15,7172,r1
	Invalid

;; push_dollar_vars: 400000000006A400
push_dollar_vars proc
	Invalid
	addl	r34,7228,r1
	Invalid

;; push_context: 400000000006A580
push_context proc
	Invalid
	Invalid
	addl	r14,7148,r1

;; pop_dollar_vars: 400000000006A680
pop_dollar_vars proc
	Invalid
	addl	r33,7236,r1
	Invalid

;; pop_context: 400000000006A800
pop_context proc
	Invalid
	addl	r32,-1972,r1
	Invalid

;; dispose_saved_dollar_vars: 400000000006A8C0
dispose_saved_dollar_vars proc
	Invalid
	addl	r32,7236,r1
	Invalid

;; push_args: 400000000006A9C0
push_args proc
	Invalid
	addl	r39,-1740,r1
	Invalid

;; pop_args: 400000000006ABC0
pop_args proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; stupidly_hack_special_variables: 400000000006AEC0
stupidly_hack_special_variables proc
	Invalid
	addl	r33,7244,r1
	Invalid

;; merge_temporary_env: 400000000006B7C0
merge_temporary_env proc
	Invalid
	addl	r14,7140,r1
	addl	r32,-1268,r1

;; dispose_used_env_vars: 400000000006B840
dispose_used_env_vars proc
	Invalid
	addl	r14,7140,r1
	Invalid

;; makunbound: 400000000006B900
makunbound proc
	Invalid
	adds	r42,0,r1
	Invalid

;; unbind_variable: 400000000006BD40
unbind_variable proc
	Invalid
	Invalid
	addl	r14,7172,r1

;; initialize_shell_variables: 400000000006BD80
initialize_shell_variables proc
	Invalid
	adds	r12,-48,r12
	Invalid
