;;; Segment .text (400000000001C480)

;; assign_in_env: 400000000006E100
assign_in_env proc
	Invalid
	Invalid
	Invalid

;; reinit_special_variables: 400000000006E7C0
reinit_special_variables proc
	Invalid
	addl	r36,-1356,r1
	Invalid

;; set_pipestatus_array: 400000000006E880
set_pipestatus_array proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; save_pipestatus_array: 400000000006ED80
save_pipestatus_array proc
	Invalid
	addl	r35,-1292,r1
	Invalid

;; restore_pipestatus_array: 400000000006EE80
restore_pipestatus_array proc
	Invalid
	addl	r36,-1292,r1
	Invalid

;; set_pipestatus_from_exit: 400000000006EF80
set_pipestatus_from_exit proc
	Invalid
	Invalid
	addl	r14,5796,r1

;; copy_word: 400000000006EFC0
copy_word proc
	Invalid
	Invalid
	Invalid

;; copy_word_list: 400000000006F1C0
copy_word_list proc
	Invalid
	adds	r36,0,r1
	Invalid

;; copy_redirect: 400000000006F300
copy_redirect proc
	Invalid
	adds	r37,0,r1
	Invalid

;; copy_redirects: 400000000006F580
copy_redirects proc
	Invalid
	Invalid
	Invalid

;; copy_command: 400000000006F6C0
copy_command proc
	Invalid
	Invalid
	Invalid

;; copy_function_def_contents: 4000000000070440
copy_function_def_contents proc
	Invalid
	adds	r14,8,r32
	Invalid

;; copy_function_def: 4000000000070580
copy_function_def proc
	Invalid
	Invalid
	adds	r36,0,r1

;; get_name_for_error: 4000000000070600
get_name_for_error proc
	Invalid
	addl	r14,6512,r1
	Invalid

;; programming_error: 4000000000070A80
programming_error proc
	Invalid
	adds	r16,8,r12
	adds	r17,0,r12

;; report_error: 4000000000070D40
report_error proc
	Invalid
	adds	r16,8,r12
	adds	r17,0,r12

;; file_error: 4000000000070EC0
file_error proc
	Invalid
	adds	r33,0,r32
	Invalid

;; fatal_error: 4000000000070F80
fatal_error proc
	Invalid
	adds	r16,8,r12
	adds	r17,0,r12

;; internal_error: 4000000000071080
internal_error proc
	Invalid
	adds	r16,8,r12
	adds	r17,0,r12

;; internal_warning: 4000000000071180
internal_warning proc
	Invalid
	adds	r16,8,r12
	adds	r17,0,r12

;; sys_error: 40000000000712C0
sys_error proc
	Invalid
	adds	r16,8,r12
	adds	r17,0,r12

;; parser_error: 4000000000071400
parser_error proc
	Invalid
	adds	r16,8,r12
	nop.i	0x0

;; command_error: 4000000000071980
command_error proc
	Invalid
	Invalid
	Invalid

;; command_errstr: 4000000000071A40
command_errstr proc
	Invalid
	Invalid
	Invalid

;; err_badarraysub: 4000000000071AC0
err_badarraysub proc
	Invalid
	addl	r14,6100,r1
	Invalid

;; err_unboundvar: 4000000000071B80
err_unboundvar proc
	Invalid
	Invalid
	Invalid

;; err_readonly: 4000000000071C00
err_readonly proc
	Invalid
	Invalid
	Invalid

;; evalexp: 4000000000076080
evalexp proc
	Invalid
	adds	r16,8,r12
	nop.i	0x0

;; find_flag: 4000000000076340
find_flag proc
	Invalid
	adds	r16,0,r0
	Invalid

;; change_flag: 4000000000076480
change_flag proc
	Invalid
	addl	r14,7352,r1
	Invalid

;; which_set_flags: 4000000000076800
which_set_flags proc
	Invalid
	addl	r32,8916,r1
	Invalid

;; reset_shell_flags: 40000000000769C0
reset_shell_flags proc
	addl	r15,7400,r1
	Invalid
	addl	r14,1,r0

;; initialize_flags: 4000000000076B40
initialize_flags proc
	addl	r14,5836,r1
	addl	r15,97,r0
	adds	r16,0,r0

;; init_job_stats: 400000000007AE80
init_job_stats proc
	addl	r15,-7044,r1
	Invalid
	addl	r14,-20676,r1

;; stop_making_children: 400000000007AF80
stop_making_children proc
	Invalid
	addl	r14,7420,r1
	nop.i	0x0

;; cleanup_the_pipeline: 400000000007AFC0
cleanup_the_pipeline proc
	Invalid
	adds	r12,-256,r12
	Invalid

;; save_pipeline: 400000000007B100
save_pipeline proc
	addl	r14,7428,r1
	Invalid
	addl	r15,7420,r1

;; restore_pipeline: 400000000007B180
restore_pipeline proc
	addl	r14,7428,r1
	Invalid
	addl	r16,7452,r1

;; start_pipeline: 400000000007B240
start_pipeline proc
	Invalid
	addl	r14,7428,r1
	Invalid

;; making_children: 400000000007B380
making_children proc
	addl	r14,7420,r1
	Invalid
	nop.i	0x0

;; delete_job: 400000000007B400
delete_job proc
	Invalid
	addl	r34,-20676,r1
	Invalid

;; nohup_job: 400000000007BE00
nohup_job proc
	addl	r14,-20676,r1
	Invalid
	Invalid

;; append_process: 400000000007BE80
append_process proc
	Invalid
	Invalid
	Invalid

;; terminate_current_pipeline: 400000000007C000
terminate_current_pipeline proc
	Invalid
	addl	r32,7436,r1
	Invalid

;; terminate_stopped_jobs: 400000000007C0C0
terminate_stopped_jobs proc
	Invalid
	addl	r35,-20676,r1
	Invalid

;; hangup_all_jobs: 400000000007C240
hangup_all_jobs proc
	Invalid
	addl	r35,-20676,r1
	Invalid

;; kill_current_pipeline: 400000000007C400
kill_current_pipeline proc
	Invalid
	addl	r14,7420,r1
	nop.i	0x0

;; get_job_by_pid: 400000000007C440
get_job_by_pid proc
	Invalid
	adds	r12,-256,r12
	Invalid
