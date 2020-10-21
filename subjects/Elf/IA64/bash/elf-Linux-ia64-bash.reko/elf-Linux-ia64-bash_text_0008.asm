;;; Segment .text (400000000001C480)

;; expand_prompt_string: 40000000000A58C0
expand_prompt_string proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; cond_expand_word: 40000000000A6540
cond_expand_word proc
	Invalid
	Invalid
	Invalid

;; expand_string_assignment: 40000000000A6700
expand_string_assignment proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; expand_assignment_string_to_string: 40000000000A6940
expand_assignment_string_to_string proc
	Invalid
	Invalid
	Invalid

;; expand_string_unsplit: 40000000000A6BC0
expand_string_unsplit proc
	Invalid
	Invalid
	Invalid

;; expand_string_unsplit_to_string: 40000000000A6D00
expand_string_unsplit_to_string proc
	Invalid
	Invalid
	Invalid

;; expand_word_unsplit: 40000000000A6E00
expand_word_unsplit proc
	Invalid
	addl	r14,23524,r1
	Invalid

;; expand_word_leave_quoted: 40000000000A6F00
expand_word_leave_quoted proc
	Invalid
	addl	r14,23524,r1
	Invalid

;; expand_string_to_string: 40000000000A7840
expand_string_to_string proc
	Invalid
	Invalid
	Invalid

;; expand_word: 40000000000A8FC0
expand_word proc
	Invalid
	Invalid
	Invalid

;; expand_words: 40000000000A90C0
expand_words proc
	Invalid
	addl	r33,31,r0
	nop.i	0x0

;; expand_words_no_vars: 40000000000A9100
expand_words_no_vars proc
	Invalid
	addl	r33,30,r0
	nop.i	0x0

;; expand_words_shellexp: 40000000000A9140
expand_words_shellexp proc
	Invalid
	addl	r33,14,r0
	nop.i	0x0

;; phash_create: 40000000000A9200
phash_create proc
	Invalid
	Invalid
	addl	r32,7596,r1

;; phash_flush: 40000000000A9280
phash_flush proc
	Invalid
	addl	r14,7596,r1
	Invalid

;; phash_remove: 40000000000A9300
phash_remove proc
	Invalid
	addl	r14,5864,r1
	Invalid

;; phash_insert: 40000000000A9480
phash_insert proc
	Invalid
	addl	r14,5864,r1
	Invalid

;; phash_search: 40000000000A9780
phash_search proc
	Invalid
	addl	r14,5864,r1
	Invalid

;; hash_create: 40000000000A9B00
hash_create proc
	Invalid
	adds	r36,0,r1
	Invalid

;; hash_size: 40000000000A9CC0
hash_size proc
	Invalid
	Invalid
	adds	r14,12,r32

;; hash_copy: 40000000000A9D00
hash_copy proc
	Invalid
	Invalid
	Invalid

;; hash_string: 40000000000AA000
hash_string proc
	Invalid
	adds	r8,0,r0
	adds	r32,1,r32

;; hash_bucket: 40000000000AA0C0
hash_bucket proc
	Invalid
	adds	r14,0,r0
	adds	r32,1,r32

;; hash_search: 40000000000AA1C0
hash_search proc
	Invalid
	Invalid
	Invalid

;; hash_remove: 40000000000AA500
hash_remove proc
	Invalid
	adds	r38,12,r33
	Invalid

;; hash_insert: 40000000000AA780
hash_insert proc
	Invalid
	Invalid
	Invalid

;; hash_flush: 40000000000AA980
hash_flush proc
	Invalid
	adds	r39,12,r32
	Invalid

;; hash_dispose: 40000000000AAB80
hash_dispose proc
	Invalid
	Invalid
	Invalid

;; hash_walk: 40000000000AAC00
hash_walk proc
	Invalid
	adds	r14,12,r32
	Invalid

;; time_to_check_mail: 40000000000AB280
time_to_check_mail proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; reset_mail_timer: 40000000000AB400
reset_mail_timer proc
	Invalid
	Invalid
	adds	r34,0,r1

;; reset_mail_files: 40000000000AB480
reset_mail_files proc
	addl	r14,7620,r1
	Invalid
	Invalid

;; free_mail_files: 40000000000AB540
free_mail_files proc
	Invalid
	addl	r38,7620,r1
	Invalid

;; make_default_mailpath: 40000000000AB680
make_default_mailpath proc
	Invalid
	Invalid
	adds	r36,0,r1

;; remember_mail_dates: 40000000000AB7C0
remember_mail_dates proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; init_mail_dates: 40000000000ABB00
init_mail_dates proc
	addl	r14,7612,r1
	Invalid
	nop.i	0x0

;; check_mail: 40000000000ABB40
check_mail proc
	Invalid
	adds	r12,-144,r12
	Invalid

;; initialize_traps: 40000000000AC380
initialize_traps proc
	Invalid
	addl	r14,24316,r1
	addl	r32,19268,r1
