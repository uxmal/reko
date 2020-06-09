;;; Segment .text (400000000001C480)

;; history_builtin: 40000000000FDCC0
history_builtin proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; jobs_builtin: 40000000000FEEC0
jobs_builtin proc
	Invalid
	adds	r12,-256,r12
	Invalid

;; disown_builtin: 40000000000FF780
disown_builtin proc
	Invalid
	adds	r12,-272,r12
	Invalid

;; kill_builtin: 40000000000FFDC0
kill_builtin proc
	Invalid
	adds	r12,-272,r12
	Invalid

;; let_builtin: 4000000000100840
let_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; mapfile_builtin: 4000000000100AC0
mapfile_builtin proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; dirs_builtin: 4000000000101D00
dirs_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; popd_builtin: 40000000001029C0
popd_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; pushd_builtin: 4000000000103080
pushd_builtin proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; get_dirstack_from_string: 4000000000103B80
get_dirstack_from_string proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; set_dirstack_element: 4000000000103D80
set_dirstack_element proc
	Invalid
	Invalid
	Invalid

;; get_directory_stack: 4000000000103EC0
get_directory_stack proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; read_builtin: 4000000000104680
read_builtin proc
	Invalid
	adds	r16,0,r12
	Invalid

;; return_builtin: 40000000001084C0
return_builtin proc
	Invalid
	Invalid
	Invalid

;; minus_o_option_value: 4000000000108DC0
minus_o_option_value proc
	Invalid
	addl	r40,-8564,r1
	Invalid

;; list_minus_o_opts: 4000000000108FC0
list_minus_o_opts proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; get_minus_o_opts: 4000000000109240
get_minus_o_opts proc
	Invalid
	adds	r34,0,r1
	Invalid

;; set_minus_o_option: 4000000000109300
set_minus_o_option proc
	Invalid
	addl	r41,-8564,r1
	Invalid

;; set_shellopts: 4000000000109580
set_shellopts proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; parse_shellopts: 4000000000109AC0
parse_shellopts proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; initialize_shell_options: 4000000000109BC0
initialize_shell_options proc
	Invalid
	Invalid
	Invalid

;; reset_shell_options: 4000000000109D40
reset_shell_options proc
	addl	r15,6112,r1
	Invalid
	addl	r14,1,r0

;; set_builtin: 4000000000109DC0
set_builtin proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; unset_builtin: 400000000010A500
unset_builtin proc
	Invalid
	adds	r44,0,r1
	Invalid

;; show_var_attributes: 400000000010ACC0
show_var_attributes proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; show_all_var_attributes: 400000000010B5C0
show_all_var_attributes proc
	Invalid
	Invalid
	Invalid

;; show_name_attributes: 400000000010B780
show_name_attributes proc
	Invalid
	Invalid
	Invalid

;; set_var_attribute: 400000000010B8C0
set_var_attribute proc
	Invalid
	Invalid
	adds	r41,0,r1

;; set_or_show_attributes: 400000000010BDC0
set_or_show_attributes proc
	Invalid
	adds	r51,0,r1
	Invalid
