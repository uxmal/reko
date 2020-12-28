;;; Segment .text (400000000001C480)

;; describe_pid: 400000000007C580
describe_pid proc
	Invalid
	adds	r12,-256,r12
	Invalid

;; list_one_job: 400000000007C740
list_one_job proc
	addl	r14,-10260,r1
	Invalid
	adds	r32,0,r35

;; ignore_tty_job_signals: 400000000007C780
ignore_tty_job_signals proc
	Invalid
	Invalid
	adds	r34,0,r1

;; default_tty_job_signals: 400000000007C800
default_tty_job_signals proc
	Invalid
	Invalid
	adds	r34,0,r1

;; get_tty_state: 400000000007C880
get_tty_state proc
	Invalid
	addl	r14,5900,r1
	Invalid

;; set_tty_state: 400000000007CA00
set_tty_state proc
	Invalid
	addl	r14,5900,r1
	Invalid

;; job_exit_status: 400000000007CB80
job_exit_status proc
	Invalid
	Invalid
	Invalid

;; job_exit_signal: 400000000007CC00
job_exit_signal proc
	Invalid
	Invalid
	Invalid

;; kill_pid: 400000000007CC80
kill_pid proc
	Invalid
	adds	r12,-272,r12
	Invalid

;; run_sigchld_trap: 400000000007D280
run_sigchld_trap proc
	Invalid
	addl	r33,24316,r1
	addl	r38,7540,r1

;; reap_dead_jobs: 400000000007EB00
reap_dead_jobs proc
	Invalid
	Invalid
	adds	r34,0,r1

;; notify_and_cleanup: 400000000007EB80
notify_and_cleanup proc
	Invalid
	addl	r14,7464,r1
	Invalid

;; list_all_jobs: 400000000007EC80
list_all_jobs proc
	Invalid
	Invalid
	adds	r36,0,r1

;; list_running_jobs: 400000000007ED00
list_running_jobs proc
	Invalid
	Invalid
	adds	r36,0,r1

;; list_stopped_jobs: 400000000007ED80
list_stopped_jobs proc
	Invalid
	Invalid
	adds	r36,0,r1

;; initialize_job_signals: 400000000007EE00
initialize_job_signals proc
	Invalid
	addl	r14,6516,r1
	Invalid

;; give_terminal_to: 400000000007F080
give_terminal_to proc
	Invalid
	adds	r12,-256,r12
	Invalid

;; initialize_job_control: 400000000007F400
initialize_job_control proc
	Invalid
	adds	r43,0,r1
	Invalid

;; wait_for: 400000000007FE40
wait_for proc
	Invalid
	adds	r12,-256,r12
	Invalid

;; wait_for_job: 4000000000081080
wait_for_job proc
	Invalid
	adds	r12,-256,r12
	Invalid

;; wait_for_single_pid: 4000000000081340
wait_for_single_pid proc
	Invalid
	adds	r12,-256,r12
	Invalid

;; wait_for_background_pids: 4000000000081740
wait_for_background_pids proc
	Invalid
	adds	r12,-512,r12
	Invalid

;; start_job: 4000000000081D00
start_job proc
	Invalid
	adds	r12,-256,r12
	Invalid

;; make_child: 4000000000082740
make_child proc
	Invalid
	adds	r12,-272,r12
	Invalid

;; stop_pipeline: 4000000000083400
stop_pipeline proc
	Invalid
	adds	r12,-512,r12
	Invalid

;; delete_all_jobs: 4000000000084900
delete_all_jobs proc
	Invalid
	adds	r12,-256,r12
	Invalid

;; nohup_all_jobs: 4000000000084C40
nohup_all_jobs proc
	Invalid
	adds	r12,-256,r12
	Invalid

;; count_all_jobs: 4000000000084E40
count_all_jobs proc
	Invalid
	adds	r12,-256,r12
	Invalid

;; freeze_jobs_list: 4000000000085000
freeze_jobs_list proc
	addl	r15,1,r0
	Invalid
	addl	r14,7464,r1

;; unfreeze_jobs_list: 4000000000085040
unfreeze_jobs_list proc
	Invalid
	addl	r14,7464,r1
	nop.i	0x0

;; set_job_control: 4000000000085080
set_job_control proc
	addl	r14,5868,r1
	Invalid
	Invalid

;; without_job_control: 40000000000850C0
without_job_control proc
	Invalid
	addl	r14,7420,r1
	Invalid

;; end_job_control: 4000000000085180
end_job_control proc
	Invalid
	addl	r14,6512,r1
	Invalid

;; restart_job_control: 4000000000085280
restart_job_control proc
	Invalid
	addl	r14,5900,r1
	Invalid

;; set_sigchld_handler: 4000000000085300
set_sigchld_handler proc
	Invalid
	Invalid
	Invalid

;; close_pgrp_pipe: 4000000000085380
close_pgrp_pipe proc
	Invalid
	Invalid
	Invalid

;; save_pgrp_pipe: 4000000000085400
save_pgrp_pipe proc
	addl	r15,5880,r1
	addl	r16,-1,r0
	Invalid

;; restore_pgrp_pipe: 4000000000085480
restore_pgrp_pipe proc
	Invalid
	Invalid
	addl	r14,5880,r1

;; sub_append_string: 40000000000879C0
sub_append_string proc
	Invalid
	Invalid
	nop.i	0x0

;; extract_array_assignment_list: 4000000000087C40
extract_array_assignment_list proc
	Invalid
	Invalid
	Invalid

;; de_backslash: 4000000000087D40
de_backslash proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; skipsubscript: 4000000000088140
skipsubscript proc
	Invalid
	adds	r12,-64,r12
	Invalid

;; extract_process_subst: 400000000008B500
extract_process_subst proc
	Invalid
	adds	r14,0,r33
	nop.i	0x0

;; extract_arithmetic_subst: 400000000008B580
extract_arithmetic_subst proc
	Invalid
	addl	r34,-6788,r1
	addl	r35,-6780,r1

;; extract_command_subst: 400000000008B600
extract_command_subst proc
	Invalid
	Invalid
	Invalid

;; skip_to_delim: 400000000008C140
skip_to_delim proc
	Invalid
	adds	r12,-64,r12
	Invalid
