;;; Segment .text (400000000001C480)

;; readonly_builtin: 400000000010C740
readonly_builtin proc
	Invalid
	addl	r33,2,r0
	adds	r34,0,r0

;; export_builtin: 400000000010C780
export_builtin proc
	Invalid
	addl	r33,1,r0
	adds	r34,0,r0

;; shift_builtin: 400000000010C7C0
shift_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; source_builtin: 400000000010CCC0
source_builtin proc
	Invalid
	Invalid
	Invalid

;; suspend_builtin: 400000000010D580
suspend_builtin proc
	Invalid
	adds	r36,0,r1
	Invalid

;; test_builtin: 400000000010D800
test_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; times_builtin: 400000000010D9C0
times_builtin proc
	Invalid
	adds	r12,-288,r12
	Invalid

;; trap_builtin: 400000000010DF80
trap_builtin proc
	Invalid
	Invalid
	adds	r42,0,r1

;; describe_command: 400000000010E880
describe_command proc
	Invalid
	Invalid
	Invalid

;; type_builtin: 400000000010F580
type_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; ulimit_builtin: 4000000000110080
ulimit_builtin proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; parse_symbolic_mode: 4000000000111440
parse_symbolic_mode proc
	Invalid
	Invalid
	adds	r46,0,r1

;; umask_builtin: 4000000000111900
umask_builtin proc
	Invalid
	adds	r40,0,r1
	Invalid

;; wait_builtin: 4000000000111D80
wait_builtin proc
	Invalid
	adds	r16,8,r12
	Invalid

;; getopts_reset: 4000000000112580
getopts_reset proc
	Invalid
	addl	r14,8664,r1
	nop.i	0x0

;; getopts_builtin: 40000000001125C0
getopts_builtin proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; set_login_shell: 4000000000113000
set_login_shell proc
	addl	r14,6520,r1
	Invalid
	adds	r8,0,r0

;; reset_shopt_options: 40000000001138C0
reset_shopt_options proc
	addl	r16,9140,r1
	addl	r14,1,r0
	addl	r15,6520,r1

;; get_shopt_options: 4000000000113A40
get_shopt_options proc
	Invalid
	addl	r32,-5932,r1
	Invalid

;; shopt_listopt: 4000000000113BC0
shopt_listopt proc
	Invalid
	Invalid
	Invalid

;; set_bashopts: 4000000000113D00
set_bashopts proc
	Invalid
	adds	r12,-48,r12
	Invalid

;; shopt_setopt: 40000000001142C0
shopt_setopt proc
	Invalid
	Invalid
	Invalid

;; shopt_builtin: 4000000000114380
shopt_builtin proc
	Invalid
	addl	r35,-2428,r1
	Invalid

;; parse_bashopts: 4000000000114C80
parse_bashopts proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; initialize_bashopts: 4000000000114DC0
initialize_bashopts proc
	Invalid
	Invalid
	Invalid

;; printf_builtin: 40000000001169C0
printf_builtin proc
	Invalid
	adds	r16,0,r12
	adds	r12,-336,r12

;; sh_getopt: 4000000000119D80
sh_getopt proc
	Invalid
	addl	r36,8664,r1
	Invalid

;; sh_getopt_restore_state: 400000000011A440
sh_getopt_restore_state proc
	addl	r14,8676,r1
	Invalid
	nop.i	0x0

;; internal_getopt: 400000000011A4C0
internal_getopt proc
	Invalid
	Invalid
	Invalid

;; reset_internal_getopt: 400000000011AE80
reset_internal_getopt proc
	addl	r14,9268,r1
	Invalid
	addl	r15,1,r0
