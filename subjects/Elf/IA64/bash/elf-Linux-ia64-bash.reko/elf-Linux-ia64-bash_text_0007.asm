;;; Segment .text (400000000001C480)

;; char_is_quoted: 400000000008CE00
char_is_quoted proc
	Invalid
	adds	r12,-48,r12
	Invalid

;; unclosed_pair: 400000000008D500
unclosed_pair proc
	Invalid
	adds	r12,-48,r12
	Invalid

;; split_at_delims: 400000000008DCC0
split_at_delims proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; string_list_internal: 400000000008E7C0
string_list_internal proc
	Invalid
	Invalid
	Invalid

;; string_list: 400000000008EC40
string_list proc
	Invalid
	Invalid
	addl	r33,-6732,r1

;; ifs_firstchar: 400000000008EC80
ifs_firstchar proc
	Invalid
	addl	r34,9108,r1
	Invalid

;; string_list_dollar_star: 400000000008EDC0
string_list_dollar_star proc
	Invalid
	adds	r37,0,r12
	Invalid

;; get_word_from_string: 400000000008EF00
get_word_from_string proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; strip_trailing_ifs_whitespace: 400000000008F800
strip_trailing_ifs_whitespace proc
	Invalid
	Invalid
	Invalid

;; list_rest_of_args: 400000000008FAC0
list_rest_of_args proc
	Invalid
	addl	r14,5924,r1
	Invalid

;; number_of_args: 400000000008FC80
number_of_args proc
	addl	r14,5932,r1
	adds	r8,0,r0
	Invalid

;; get_dollar_var_value: 400000000008FD40
get_dollar_var_value proc
	Invalid
	addl	r14,23444,r1
	Invalid

;; string_rest_of_args: 400000000008FF40
string_rest_of_args proc
	Invalid
	Invalid
	adds	r37,0,r1

;; remove_backslashes: 4000000000090040
remove_backslashes proc
	Invalid
	Invalid
	Invalid

;; quote_escapes: 4000000000090180
quote_escapes proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; dequote_escapes: 4000000000090740
dequote_escapes proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; quote_string: 4000000000090C40
quote_string proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; string_list_dollar_at: 4000000000091100
string_list_dollar_at proc
	Invalid
	adds	r40,0,r12
	Invalid

;; dequote_string: 4000000000091400
dequote_string proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; dequote_list: 4000000000091E80
dequote_list proc
	Invalid
	adds	r39,0,r1
	Invalid

;; remove_quoted_escapes: 4000000000092040
remove_quoted_escapes proc
	Invalid
	Invalid
	Invalid

;; remove_quoted_nulls: 4000000000092100
remove_quoted_nulls proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; list_string: 4000000000092540
list_string proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; word_list_remove_quoted_nulls: 4000000000093340
word_list_remove_quoted_nulls proc
	Invalid
	adds	r38,0,r1
	Invalid

;; string_list_pos_params: 4000000000093400
string_list_pos_params proc
	Invalid
	Invalid
	nop.i	0x0

;; copy_fifo_list: 4000000000093880
copy_fifo_list proc
	Invalid
	addl	r14,7576,r1
	Invalid

;; fifos_pending: 40000000000939C0
fifos_pending proc
	Invalid
	adds	r8,0,r0
	Invalid

;; num_fifos: 4000000000093A00
num_fifos proc
	Invalid
	addl	r14,7576,r1
	nop.i	0x0

;; unlink_fifo: 4000000000093A40
unlink_fifo proc
	Invalid
	addl	r34,7564,r1
	Invalid

;; unlink_fifo_list: 4000000000093B00
unlink_fifo_list proc
	Invalid
	addl	r33,7576,r1
	Invalid

;; close_new_fifos: 4000000000093C00
close_new_fifos proc
	Invalid
	Invalid
	Invalid

;; command_substitute: 4000000000093E00
command_substitute proc
	Invalid
	adds	r16,0,r12
	Invalid

;; pat_subst: 4000000000095600
pat_subst proc
	Invalid
	adds	r12,-64,r12
	Invalid

;; string_quote_removal: 4000000000096E00
string_quote_removal proc
	Invalid
	adds	r12,-32,r12
	Invalid

;; expand_arith_string: 4000000000097980
expand_arith_string proc
	Invalid
	Invalid
	addl	r34,-9900,r1

;; do_assignment_no_expand: 40000000000999C0
do_assignment_no_expand proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; do_word_assignment: 4000000000099A40
do_word_assignment proc
	Invalid
	addl	r33,1,r0
	nop.i	0x0

;; do_assignment: 4000000000099A80
do_assignment proc
	Invalid
	adds	r12,-16,r12
	Invalid

;; setifs: 4000000000099B00
setifs proc
	Invalid
	addl	r14,9076,r1
	Invalid

;; getifs: 4000000000099E40
getifs proc
	addl	r14,9092,r1
	Invalid
	nop.i	0x0

;; word_split: 4000000000099E80
word_split proc
	Invalid
	adds	r15,8,r32
	Invalid

;; expand_string: 400000000009A080
expand_string proc
	Invalid
	Invalid
	Invalid
