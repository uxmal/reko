;;; Segment .text (400000000001C480)

;; pwd_builtin: 40000000000EC680
pwd_builtin proc
	Invalid
	addl	r14,7360,r1
	Invalid

;; colon_builtin: 40000000000ECA80
colon_builtin proc
	Invalid
	adds	r8,0,r0
	Invalid

;; false_builtin: 40000000000ECAC0
false_builtin proc
	Invalid
	addl	r8,1,r0
	Invalid

;; command_builtin: 40000000000ECC00
command_builtin proc
	Invalid
	adds	r40,0,r1
	Invalid

;; builtin_error: 40000000000ED580
builtin_error proc
	Invalid
	adds	r16,8,r12
	adds	r17,0,r12

;; builtin_warning: 40000000000ED680
builtin_warning proc
	Invalid
	adds	r16,8,r12
	adds	r17,0,r12

;; builtin_usage: 40000000000ED7C0
builtin_usage proc
	Invalid
	addl	r33,9036,r1
	Invalid

;; no_args: 40000000000EDA00
no_args proc
	Invalid
	Invalid
	nop.i	0x0

;; no_options: 40000000000EDAC0
no_options proc
	Invalid
	Invalid
	adds	r35,0,r1

;; sh_needarg: 40000000000EDB80
sh_needarg proc
	Invalid
	Invalid
	Invalid

;; sh_neednumarg: 40000000000EDC00
sh_neednumarg proc
	Invalid
	Invalid
	Invalid

;; sh_notfound: 40000000000EDC80
sh_notfound proc
	Invalid
	Invalid
	Invalid

;; sh_invalidopt: 40000000000EDD00
sh_invalidopt proc
	Invalid
	Invalid
	Invalid

;; sh_invalidoptname: 40000000000EDD80
sh_invalidoptname proc
	Invalid
	Invalid
	Invalid

;; sh_invalidid: 40000000000EDE00
sh_invalidid proc
	Invalid
	Invalid
	Invalid

;; sh_invalidnum: 40000000000EDE80
sh_invalidnum proc
	Invalid
	Invalid
	Invalid

;; sh_invalidsig: 40000000000EE040
sh_invalidsig proc
	Invalid
	Invalid
	Invalid

;; sh_badpid: 40000000000EE0C0
sh_badpid proc
	Invalid
	Invalid
	Invalid

;; sh_readonly: 40000000000EE140
sh_readonly proc
	Invalid
	Invalid
	Invalid

;; sh_erange: 40000000000EE1C0
sh_erange proc
	Invalid
	Invalid
	Invalid

;; sh_badjob: 40000000000EE380
sh_badjob proc
	Invalid
	Invalid
	Invalid

;; sh_nojobs: 40000000000EE400
sh_nojobs proc
	Invalid
	Invalid
	Invalid

;; sh_restricted: 40000000000EE500
sh_restricted proc
	Invalid
	Invalid
	Invalid

;; sh_notbuiltin: 40000000000EE600
sh_notbuiltin proc
	Invalid
	Invalid
	Invalid

;; sh_wrerror: 40000000000EE680
sh_wrerror proc
	Invalid
	addl	r38,-2588,r1
	Invalid

;; sh_ttyerror: 40000000000EE740
sh_ttyerror proc
	Invalid
	addl	r38,-2572,r1
	Invalid

;; sh_chkwrite: 40000000000EE800
sh_chkwrite proc
	Invalid
	addl	r33,-10260,r1
	Invalid

;; make_builtin_argv: 40000000000EE900
make_builtin_argv proc
	Invalid
	Invalid
	Invalid

;; dollar_vars_changed: 40000000000EE980
dollar_vars_changed proc
	Invalid
	addl	r14,8404,r1
	nop.i	0x0

;; set_dollar_vars_unchanged: 40000000000EE9C0
set_dollar_vars_unchanged proc
	Invalid
	addl	r14,8404,r1
	nop.i	0x0

;; set_dollar_vars_changed: 40000000000EEA00
set_dollar_vars_changed proc
	addl	r15,7148,r1
	Invalid
	addl	r14,8404,r1

;; remember_args: 40000000000EEAC0
remember_args proc
	Invalid
	Invalid
	Invalid

;; get_numeric_arg: 40000000000EED00
get_numeric_arg proc
	Invalid
	Invalid
	Invalid

;; get_exitstat: 40000000000EEFC0
get_exitstat proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; read_octal: 40000000000EF200
read_octal proc
	adds	r8,0,r0
	addl	r18,511,r0
	adds	r14,0,r32

;; get_working_directory: 40000000000EF2C0
get_working_directory proc
	Invalid
	addl	r14,7360,r1
	Invalid

;; set_working_directory: 40000000000EF5C0
set_working_directory proc
	Invalid
	addl	r33,8380,r1
	Invalid

;; get_job_by_name: 40000000000EF680
get_job_by_name proc
	Invalid
	Invalid
	Invalid

;; get_job_spec: 40000000000EFB00
get_job_spec proc
	Invalid
	Invalid
	Invalid

;; display_signal_list: 40000000000EFDC0
display_signal_list proc
	Invalid
	adds	r12,-16,r12
	addl	r39,-9572,r1

;; builtin_address_internal: 40000000000F0440
builtin_address_internal proc
	Invalid
	addl	r14,6156,r1
	Invalid

;; find_shell_builtin: 40000000000F0600
find_shell_builtin proc
	Invalid
	Invalid
	Invalid

;; builtin_address: 40000000000F06C0
builtin_address proc
	Invalid
	Invalid
	Invalid

;; find_special_builtin: 40000000000F0780
find_special_builtin proc
	Invalid
	Invalid
	Invalid

;; initialize_shell_builtins: 40000000000F0880
initialize_shell_builtins proc
	Invalid
	addl	r14,6156,r1
	Invalid

;; declare_builtin: 40000000000F2400
declare_builtin proc
	Invalid
	adds	r33,0,r0
	nop.i	0x0

;; local_builtin: 40000000000F2440
local_builtin proc
	Invalid
	addl	r14,7148,r1
	Invalid

;; echo_builtin: 40000000000F2500
echo_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; enable_builtin: 40000000000F2CC0
enable_builtin proc
	Invalid
	Invalid
	Invalid

;; eval_builtin: 40000000000F4040
eval_builtin proc
	Invalid
	adds	r35,0,r1
	Invalid

;; maybe_execute_file: 40000000000F5BC0
maybe_execute_file proc
	Invalid
	Invalid
	Invalid

;; fc_execute_file: 40000000000F5C80
fc_execute_file proc
	Invalid
	addl	r33,161,r0
	nop.i	0x0

;; source_file: 40000000000F5CC0
source_file proc
	Invalid
	addl	r14,6456,r1
	Invalid

;; parse_and_execute_cleanup: 40000000000F61C0
parse_and_execute_cleanup proc
	Invalid
	addl	r14,9124,r1
	Invalid

;; parse_and_execute: 40000000000F6300
parse_and_execute proc
	Invalid
	adds	r16,8,r12
	Invalid

;; parse_string: 40000000000F7580
parse_string proc
	Invalid
	adds	r16,8,r12
	Invalid

;; exec_builtin: 40000000000F7C40
exec_builtin proc
	Invalid
	addl	r33,8876,r1
	Invalid

;; bash_logout: 40000000000F8680
bash_logout proc
	Invalid
	addl	r15,6520,r1
	Invalid

;; logout_builtin: 40000000000F8C80
logout_builtin proc
	Invalid
	addl	r14,6520,r1
	Invalid

;; exit_builtin: 40000000000F8D40
exit_builtin proc
	Invalid
	addl	r14,6516,r1
	Invalid

;; fc_builtin: 40000000000F93C0
fc_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; fg_builtin: 40000000000FB140
fg_builtin proc
	Invalid
	addl	r14,5868,r1
	Invalid

;; bg_builtin: 40000000000FB3C0
bg_builtin proc
	Invalid
	addl	r14,5868,r1
	Invalid

;; hash_builtin: 40000000000FB640
hash_builtin proc
	Invalid
	addl	r14,5864,r1
	Invalid

;; help_builtin: 40000000000FC280
help_builtin proc
	Invalid
	adds	r12,-144,r12
	Invalid
