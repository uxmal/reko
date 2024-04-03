;;; Segment .plt (000094C0)
000094C0 71 48 00 00 1C C8 49 6A 0E 42 9C E2 4F 40 9C 91 qH....Ij.B..O@..
000094D0 3C C0 9C 91 0E 42 0E 8C 00 C0 9C 52 00 00 00 00 <....B.....R....
000094E0 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ................

;; fn000094F0: 000094F0
;;   Called from:
;;     0000947C (in _init)
fn000094F0 proc
	{ r14 = add(badva,00000030) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009500: 00009500
;;   Called from:
;;     0000AC20 (in prefree)
;;     0000B40C (in alloc_node)
;;     0000B468 (in node_alloc_common)
;;     0000BDA8 (in try_pad_bad_supernodes)
;;     0000BE2C (in try_pad_bad_supernodes)
;;     0000C7FC (in tensor_alloc)
;;     0000C868 (in tensor_dup)
;;     0000CB7C (in nn_os_workers_spawn)
;;     0000CBA0 (in nn_os_workers_spawn)
;;     0000CC28 (in nn_os_workers_spawn)
fn00009500 proc
	{ r14 = add(badva,00000024) }

;; fn00009504: 00009504
;;   Called from:
;;     0000B4B8 (in node_alloc_common)
;;     0000B550 (in node_alloc_common)
;;     00019388 (in supernode_check_ref)
fn00009504 proc
	{ r14 = add(badva,00000024) }

;; fn00009508: 00009508
;;   Called from:
;;     0000B9E0 (in const_depth_extend_8)
;;     0000BAB0 (in const_width_extend_8)
fn00009508 proc
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009510: 00009510
;;   Called from:
;;     0000AA48 (in allocate_and_free)
;;     0000ABB4 (in allocator_teardown)
;;     0000ABD4 (in allocator_teardown)
;;     0000AC90 (in prefree)
;;     0000ADD0 (in hexagon_nn_init)
;;     0000B690 (in node_free_common)
;;     0000B804 (in do_teardown)
;;     0000B80C (in do_teardown)
;;     0000C82C (in tensor_alloc)
;;     0000C89C (in tensor_dup)
;;     0001394C (in const_dtor)
;;     000194AC (in supernode_dtor)
fn00009510 proc
	{ r14 = add(badva,00000018) }

;; fn00009514: 00009514
;;   Called from:
;;     0000BA2C (in const_depth_extend_8)
;;     0000BB08 (in const_width_extend_8)
fn00009514 proc
	{ r14 = add(badva,00000018) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }
00009520 70 48 00 00 0E C6 49 6A 1C C0 8E 91 00 C0 9C 52 pH....Ij.......R

;; fn00009530: 00009530
;;   Called from:
;;     0000ACF8 (in do_execute)
;;     0000AD58 (in do_execute)
;;     0000CD4C (in nn_os_get_perfcount)
;;     0001A150 (in supernode_execute_hvx_slice)
fn00009530 proc
	{ r14 = add(badva,00000000) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009540: 00009540
;;   Called from:
;;     0000AD98 (in hexagon_nn_init)
fn00009540 proc
	{ r14 = add(badva,00000034) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009550: 00009550
;;   Called from:
;;     0000ADB8 (in hexagon_nn_init)
;;     0000C818 (in tensor_alloc)
;;     0000C880 (in tensor_dup)
;;     000193F8 (in supernode_check_ref)
;;     0001D3F8 (in variable_ctor)
fn00009550 proc
	{ r14 = add(badva,00000028) }

;; fn00009554: 00009554
;;   Called from:
;;     00014EC0 (in matmul_check_ref)
fn00009554 proc
	{ r14 = add(badva,00000028) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009560: 00009560
;;   Called from:
;;     0000BA00 (in const_depth_extend_8)
;;     0000BADC (in const_width_extend_8)
;;     0000C7B4 (in fn0000C7B4)
;;     0000C8C8 (in tensor_dup)
;;     0000F4C0 (in expanddims_execute)
;;     0000F644 (in fn0000F644)
;;     0000F674 (in fn0000F650)
;;     0000F86C (in strided_slice_impl)
;;     00010E48 (in pack_execute)
;;     0001193C (in fn000117A4)
;;     00011978 (in fn000117A4)
;;     00014898 (in flatten_execute)
;;     000149EC (in qflatten_execute)
;;     00014A1C (in qflatten_execute)
;;     00014A4C (in qflatten_execute)
;;     0001596C (in fn000157D4)
;;     000159A8 (in fn000157D4)
;;     00016D50 (in nop_execute)
;;     00016F40 (in output_execute)
;;     00017BF8 (in relu_execute)
;;     00018038 (in relu_execute.extracted_region)
;;     0001B0A0 (in concat_execute)
;;     0001CACC (in lrn_8_execute_ref.extracted_region)
;;     0001D0E8 (in nn_variable_read)
;;     0001D174 (in nn_variable_write)
;;     0001D1E4 (in assign_execute)
;;     0001D240 (in assign_execute)
;;     0001D368 (in variable_check)
;;     0001D6D4 (in reshape_execute)
;;     0001D714 (in reshape_execute)
;;     0001D980 (in fn0001D980)
;;     0001D9B0 (in fn0001D98C)
;;     0001DC88 (in slice_impl)
;;     0001DD8C (in fn0001DD8C)
;;     0001DDC4 (in fn0001DD98)
;;     0001DF50 (in split_impl)
;;     0001DFC8 (in split_impl)
;;     0001E680 (in qtanh_execute_hvx)
;;     0001E900 (in qtanh_execute_hvx)
;;     0001ECF0 (in qsigmoid_execute_hvx)
;;     0001EF74 (in qsigmoid_execute_hvx)
;;     0002028C (in transpose_execute)
;;     000214C8 (in prelu_execute)
;;     00021508 (in prelu_execute)
fn00009560 proc
	{ r14 = add(badva,0000001C) }

;; fn00009564: 00009564
;;   Called from:
;;     0000AE48 (in hexagon_nn_getlog)
;;     0000AE78 (in hexagon_nn_getlog)
fn00009564 proc
	{ r14 = add(badva,0000001C) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009570: 00009570
;;   Called from:
;;     0000AE68 (in hexagon_nn_getlog)
;;     0000B2C0 (in hexagon_nn_op_id_to_name)
fn00009570 proc
	{ r14 = add(badva,00000010) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009580: 00009580
;;   Called from:
;;     0000AEA8 (in hexagon_nn_snpprint)
fn00009580 proc
	{ r14 = add(badva,00000004) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009590: 00009590
;;   Called from:
;;     0000B26C (in hexagon_nn_op_name_to_id)
fn00009590 proc
	{ r14 = add(badva,00000038) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }
000095A0 6E 48 00 00 0E D6 49 6A 1C C0 8E 91 00 C0 9C 52 nH....Ij.......R

;; fn000095B0: 000095B0
;;   Called from:
;;     0000B2FC (in hexagon_nn_disable_dcvs)
fn000095B0 proc
	{ r14 = add(badva,00000020) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn000095C0: 000095C0
;;   Called from:
;;     0000B34C (in hexagon_nn_config)
fn000095C0 proc
	{ r14 = add(badva,00000014) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn000095D0: 000095D0
;;   Called from:
;;     0000B398 (in logv)
;;     0000B3D4 (in logv)
;;     0000B850 (in do_snpprint)
;;     0000B8BC (in do_snpprint)
;;     0000B908 (in do_snpprint)
;;     0000B954 (in do_snpprint)
;;     0000B98C (in do_snpprint)
fn000095D0 proc
	{ r14 = add(badva,00000008) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }
000095E0 6D 48 00 00                                     mH..            

;; fn000095E4: 000095E4
;;   Called from:
;;     0000B3B0 (in logv)
fn000095E4 proc
	{ r14 = add(badva,0000003C) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn000095F0: 000095F0
;;   Called from:
;;     0000BA18 (in const_depth_extend_8)
;;     0000BAF4 (in const_width_extend_8)
;;     0000C7CC (in fn0000C7B4)
;;     0000DB4C (in im2col_cn)
;;     0000DB88 (in im2col_cn)
;;     0000DC4C (in im2col_cn)
;;     00013E10 (in conv2d_execute_hvx)
;;     00018BC8 (in supernode_execute_hvx)
;;     00018BF0 (in supernode_execute_hvx)
;;     0001A278 (in supernode_execute_hvx_slice)
;;     0001E668 (in qtanh_execute_hvx)
;;     0001E674 (in qtanh_execute_hvx)
;;     0001ECD8 (in qsigmoid_execute_hvx)
;;     0001ECE4 (in qsigmoid_execute_hvx)
;;     00020BE8 (in execute_finstancenorm)
fn000095F0 proc
	{ r14 = add(badva,00000030) }

;; fn000095F4: 000095F4
;;   Called from:
;;     0000DBB8 (in im2col_cn)
;;     0000DBB8 (in im2col_cn)
;;     0002079C (in execute_qinstancenorm_ref)
fn000095F4 proc
	{ r14 = add(badva,00000030) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009600: 00009600
;;   Called from:
;;     0000E5DC (in deconv_execute_ref)
;;     000121F4 (in biasadd_8p8to32_execute)
;;     00012204 (in biasadd_8p8to32_execute)
;;     0001220C (in biasadd_8p8to32_execute)
;;     00012298 (in biasadd_8p8to32_execute)
;;     000133DC (in fn00013214)
;;     00014810 (in fmaxf.1.0)
;;     00014810 (in fn0001480C)
;;     000155F4 (in fmaxf.1.0)
;;     000155F4 (in fn000155F0)
;;     00016B18 (in qsoftmax_execute_ref)
;;     00017A54 (in fmaxf.1.0)
;;     00017A54 (in fn00017A50)
;;     00018A2C (in fmaxf.1.0)
;;     00018A2C (in fn00018A28)
;;     0001A7B0 (in fmaxf.1.0)
;;     0001C13C (in relu_execute)
;;     0001C2F8 (in reluX_execute)
;;     0001E7C4 (in qtanh_execute_hvx)
;;     0001EE34 (in qsigmoid_execute_hvx)
;;     000212D0 (in prelu_execute)
;;     00021748 (in prelu_execute)
fn00009600 proc
	{ r14 = add(badva,00000024) }

;; fn00009604: 00009604
;;   Called from:
;;     0000BD04 (in try_pad_bad_supernodes)
;;     00012250 (in biasadd_8p8to32_execute)
;;     00016A7C (in qsoftmax_execute_ref)
;;     00017B08 (in relu_execute)
;;     00017DB8 (in reluX_execute)
fn00009604 proc
	{ r14 = add(badva,00000024) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009610: 00009610
;;   Called from:
;;     0000BD10 (in try_pad_bad_supernodes)
;;     0000E580 (in deconv_execute_ref)
;;     0000E594 (in deconv_execute_ref)
;;     0000E5EC (in deconv_execute_ref)
;;     00012100 (in biasadd_8p8to32_execute)
;;     00012110 (in biasadd_8p8to32_execute)
;;     0001225C (in biasadd_8p8to32_execute)
;;     000122A8 (in biasadd_8p8to32_execute)
;;     000122D8 (in biasadd_8p8to32_execute)
;;     00012B58 (in close_execute_q_u8)
;;     00013468 (in concat_execute_slice_ref)
;;     00013678 (in concat_execute_slice_asm)
;;     00013ACC (in conv2d_execute_hvx)
;;     00013ADC (in conv2d_execute_hvx)
;;     00013B0C (in conv2d_execute_hvx)
;;     00013B40 (in conv2d_execute_hvx)
;;     00014348 (in conv2d_execute_ref)
;;     0001435C (in conv2d_execute_ref)
;;     00014390 (in conv2d_execute_ref)
;;     000143C0 (in conv2d_execute_ref)
;;     00014E84 (in matmul_check_ref)
;;     00015080 (in fn00015000)
;;     00015080 (in fn00015000)
;;     00015094 (in fn00015000)
;;     00015094 (in fn00015000)
;;     000150C0 (in fn00015000)
;;     000150C0 (in fn00015000)
;;     000150F8 (in fn00015000)
;;     000150F8 (in fn00015000)
;;     00016AE0 (in qsoftmax_execute_ref)
;;     00016B7C (in qsoftmax_execute_ref)
;;     000175E4 (in quantize_execute)
;;     000175EC (in quantize_execute)
;;     000177EC (in dequantize_execute)
;;     00017B1C (in relu_execute)
;;     00017DCC (in reluX_execute)
;;     000181F8 (in autorequantize_execute)
;;     00018204 (in autorequantize_execute)
;;     0001828C (in autorequantize_execute)
;;     00018570 (in requantize_execute)
;;     0001857C (in requantize_execute)
;;     000185EC (in requantize_execute)
;;     00018614 (in requantize_execute)
;;     00018C48 (in supernode_execute_hvx)
;;     00018C58 (in supernode_execute_hvx)
;;     00018C68 (in supernode_execute_hvx)
;;     00018C74 (in supernode_execute_hvx)
;;     00018C7C (in supernode_execute_hvx)
;;     00018CA4 (in supernode_execute_hvx)
;;     000190DC (in supernode_execute_hvx)
;;     00019364 (in supernode_check_ref)
;;     00019680 (in supernode_execute_ref)
;;     00019690 (in supernode_execute_ref)
;;     000196A4 (in supernode_execute_ref)
;;     000196C4 (in supernode_execute_ref)
;;     00019714 (in supernode_execute_ref)
;;     00019744 (in supernode_execute_ref)
;;     00019EEC (in supernode_execute_hvx_slice)
;;     00019F38 (in supernode_execute_hvx_slice)
;;     0001AB7C (in avgpool_execute)
;;     0001C734 (in lrn_8_execute_ref)
;;     0001E6B8 (in qtanh_execute_hvx)
;;     0001E6C8 (in qtanh_execute_hvx)
;;     0001E778 (in qtanh_execute_hvx)
;;     0001E7D0 (in qtanh_execute_hvx)
;;     0001ED28 (in qsigmoid_execute_hvx)
;;     0001ED38 (in qsigmoid_execute_hvx)
;;     0001EDE8 (in qsigmoid_execute_hvx)
;;     0001EE40 (in qsigmoid_execute_hvx)
;;     00020C2C (in execute_finstancenorm)
;;     00020C4C (in execute_finstancenorm)
;;     00020C70 (in execute_finstancenorm)
;;     000212E4 (in prelu_execute)
fn00009610 proc
	{ r14 = add(badva,00000018) }

;; fn00009614: 00009614
;;   Called from:
;;     0000F8E4 (in resizenear_f_execute)
;;     0000F8F8 (in resizenear_f_execute)
;;     000122C4 (in biasadd_8p8to32_execute)
;;     00012B50 (in close_execute_q_u8)
;;     00012C68 (in close_execute_q_u8)
;;     00012F2C (in check_u8vals)
;;     000134F0 (in concat_execute_slice_ref)
;;     00013524 (in concat_execute_slice_ref)
;;     0001696C (in qsoftmax_execute_ref)
;;     000182B4 (in autorequantize_execute)
;;     00018634 (in requantize_execute)
;;     0001E3D4 (in qtanh_execute_ref)
;;     0001EA44 (in qsigmoid_execute_ref)
fn00009614 proc
	{ r14 = add(badva,00000018) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009620: 00009620
;;   Called from:
;;     000190E4 (in supernode_execute_hvx)
fn00009620 proc
	{ r14 = add(badva,0000000C) }

;; fn00009624: 00009624
;;   Called from:
;;     0000BD28 (in try_pad_bad_supernodes)
;;     0000E5D8 (in deconv_execute_ref)
;;     0000E5FC (in deconv_execute_ref)
;;     00012270 (in biasadd_8p8to32_execute)
;;     000122B8 (in biasadd_8p8to32_execute)
;;     0001241C (in biasadd_8p8to32_execute)
;;     00013B24 (in conv2d_execute_hvx)
;;     00013B50 (in conv2d_execute_hvx)
;;     000143AC (in conv2d_execute_ref)
;;     000143D0 (in conv2d_execute_ref)
;;     00014E9C (in matmul_check_ref)
;;     000150E0 (in fn00015000)
;;     000150E0 (in fn00015000)
;;     0001510C (in fn00015000)
;;     0001510C (in fn00015000)
;;     00017654 (in quantize_execute)
;;     00017B34 (in relu_execute)
;;     00017DE0 (in reluX_execute)
;;     00017DF0 (in reluX_execute)
;;     000189F0 (in autorequantize_execute.extracted_region)
;;     00018CBC (in supernode_execute_hvx)
;;     0001937C (in supernode_check_ref)
;;     000196F8 (in supernode_execute_ref)
;;     00019728 (in supernode_execute_ref)
;;     00019754 (in supernode_execute_ref)
;;     00019958 (in supernode_execute_ref)
;;     00019F04 (in supernode_execute_hvx_slice)
;;     00019F48 (in supernode_execute_hvx_slice)
;;     0001E7F8 (in qtanh_execute_hvx)
;;     0001EE68 (in qsigmoid_execute_hvx)
;;     000212FC (in prelu_execute)
fn00009624 proc
	{ r14 = add(badva,0000000C) }

l00009628:
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009630: 00009630
;;   Called from:
;;     0000BEA0 (in try_pad_bad_supernodes)
fn00009630 proc
	{ r14 = add(badva,00000000) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }
00009640 6C 48 00 00                                     lH..            

;; fn00009644: 00009644
;;   Called from:
;;     0000C924 (in do_perfinfo_reset)
fn00009644 proc
	{ r14 = add(badva,00000034) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009650: 00009650
;;   Called from:
;;     0000CB28 (in nn_os_work_for_vector)
;;     0000CB34 (in nn_os_work_for_scalar)
;;     0000CDC8 (in nn_os_vector_workers_acquire)
fn00009650 proc
	{ r14 = add(badva,00000028) }

;; fn00009658: 00009658
;;   Called from:
;;     0000CE5C (in nn_os_vector_workers_release)
fn00009658 proc
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009660: 00009660
;;   Called from:
;;     0000CB4C (in nn_os_vector_release)
;;     0000CE80 (in nn_os_vector_workers_release)
;;     0000CE88 (in worker_release)
fn00009660 proc
	{ r14 = add(badva,0000001C) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009670: 00009670
;;   Called from:
;;     0000CB9C (in nn_os_workers_spawn)
;;     0000CBBC (in nn_os_workers_spawn)
fn00009670 proc
	{ r14 = add(badva,00000010) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009680: 00009680
;;   Called from:
;;     0000CC24 (in nn_os_workers_spawn)
fn00009680 proc
	{ r14 = add(badva,00000004) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009690: 00009690
;;   Called from:
;;     0000CC58 (in nn_os_workers_spawn)
fn00009690 proc
	{ r14 = add(badva,00000038) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn000096A0: 000096A0
;;   Called from:
;;     0000CDCC (in nn_os_vector_workers_acquire)
;;     0000CE18 (in worker_acquire)
;;     0000CE6C (in nn_os_vector_workers_release)
;;     0000CEB0 (in worker_release)
;;     00011900 (in fn000117A4)
;;     000132C8 (in fn00013214)
;;     00015930 (in fn000157D4)
;;     00019120 (in supernode_execute_hvx)
;;     00019128 (in supernode_execute_hvx)
fn000096A0 proc
	{ r14 = add(badva,0000002C) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn000096B0: 000096B0
;;   Called from:
;;     0000CCA8 (in qurt_worker)
;;     0000CCC4 (in qurt_worker)
fn000096B0 proc
	{ r14 = add(badva,00000020) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

l000096C0:
	{ r14 = add(badva,00000014) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn000096D0: 000096D0
;;   Called from:
;;     0000CCE8 (in nn_os_hvx_power_on)
fn000096D0 proc
	{ r14 = add(badva,00000008) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

l000096E0:
	{ r14 = add(badva,0000003C) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn000096F0: 000096F0
;;   Called from:
;;     0000CD54 (in nn_os_get_perfcount)
fn000096F0 proc
	{ r14 = add(badva,00000030) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

l00009700:
	{ r14 = add(badva,00000024) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009710: 00009710
;;   Called from:
;;     0000CD60 (in nn_os_get_perfcount)
;;     0000CD64 (in nn_os_get_perfcount)
fn00009710 proc
	{ r14 = add(badva,00000018) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

l00009720:
	{ r14 = add(badva,0000000C) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

l00009730:
	{ r14 = add(badva,00000000) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009740: 00009740
;;   Called from:
;;     00011834 (in fn000117A4)
;;     0001183C (in fn000117A4)
;;     000132A0 (in fn00013214)
;;     000132A8 (in fn00013214)
;;     00015864 (in fn000157D4)
;;     0001586C (in fn000157D4)
;;     00018BDC (in supernode_execute_hvx)
;;     00018C04 (in supernode_execute_hvx)
fn00009740 proc
	{ r14 = add(badva,00000034) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009750: 00009750
;;   Called from:
;;     0000D7C0 (in im2col_co)
;;     0000D818 (in im2col_co)
;;     0000D818 (in im2col_co)
;;     0000D8C0 (in im2col_co)
;;     0000DCF4 (in im2col_slice_v0_co)
;;     0000E060 (in im2col_slice_co)
;;     0000E090 (in im2col_slice_co)
;;     0000E990 (in deconv_execute_ref)
;;     0000E9D8 (in deconv_execute_ref)
;;     0000F008 (in deconv_f_execute_ref)
;;     0000F050 (in deconv_f_execute_ref)
;;     0000F78C (in strided_slice_impl)
;;     0001A1DC (in supernode_execute_hvx_slice)
;;     0001A1E8 (in supernode_execute_hvx_slice)
;;     0001DEB8 (in split_impl)
fn00009750 proc
	{ r14 = add(badva,00000028) }

;; fn00009754: 00009754
;;   Called from:
;;     0001A23C (in supernode_execute_hvx_slice)
fn00009754 proc
	{ r14 = add(badva,00000028) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009760: 00009760
;;   Called from:
;;     0000E4E4 (in deconv_execute_ref)
;;     0000E548 (in deconv_execute_ref)
;;     0000E63C (in deconv_execute_ref)
;;     0000E6B4 (in deconv_execute_ref)
;;     0000EC74 (in deconv_f_execute_ref)
;;     0000ECD0 (in deconv_f_execute_ref)
;;     0000ED20 (in deconv_f_execute_ref)
;;     0000ED84 (in deconv_f_execute_ref)
;;     000117C8 (in fn000117A4)
;;     00011804 (in fn000117A4)
;;     00011A38 (in avgpool_execute_slice_ref)
;;     00011A84 (in avgpool_execute_slice_ref)
;;     00011E34 (in avgpool_execute_slice_asm)
;;     00011E7C (in avgpool_execute_slice_asm)
;;     00013A3C (in conv2d_execute_hvx)
;;     00013A6C (in conv2d_execute_hvx)
;;     00013A8C (in conv2d_execute_hvx)
;;     000142B0 (in conv2d_execute_ref)
;;     00014308 (in conv2d_execute_ref)
;;     000157F8 (in fn000157D4)
;;     00015834 (in fn000157D4)
;;     00015A64 (in maxpool_execute_slice_ref)
;;     00015AAC (in maxpool_execute_slice_ref)
;;     00015E18 (in maxpool_execute_slice_asm)
;;     00015E60 (in maxpool_execute_slice_asm)
;;     00018B24 (in supernode_execute_hvx)
;;     00018B88 (in supernode_execute_hvx)
;;     0001957C (in supernode_execute_ref)
;;     000195E4 (in supernode_execute_ref)
;;     00019BDC (in supernode_execute_ref)
;;     00019DEC (in supernode_execute_hvx_slice)
;;     00019E40 (in supernode_execute_hvx_slice)
;;     0001A84C (in avgpool_execute)
;;     0001A8A0 (in avgpool_execute)
;;     0001B368 (in conv2d_f_execute_ref)
;;     0001B3B0 (in conv2d_f_execute_ref)
;;     0001BBE8 (in maxpool_execute)
;;     0001BC3C (in maxpool_execute)
;;     0001D634 (in reshape_execute)
;;     0001DECC (in split_impl)
fn00009760 proc
	{ r14 = add(badva,0000001C) }

;; fn00009764: 00009764
;;   Called from:
;;     00011C40 (in avgpool_execute_slice_ref)
;;     00011FD0 (in avgpool_execute_slice_asm)
;;     000190F4 (in supernode_execute_hvx)
fn00009764 proc
	{ r14 = add(badva,0000001C) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009770: 00009770
;;   Called from:
;;     0000E980 (in deconv_execute_ref)
;;     0000E9C8 (in deconv_execute_ref)
;;     0000EFF8 (in deconv_f_execute_ref)
;;     0000F040 (in deconv_f_execute_ref)
fn00009770 proc
	{ r14 = add(badva,00000010) }

;; fn00009774: 00009774
;;   Called from:
;;     0001DF74 (in split_impl)
;;     0001DF78 (in split_impl)
fn00009774 proc
	{ r14 = add(badva,00000010) }

l00009778:
	{ r28 = memw(r14) }
	{ jumpr	r28 }
00009780 69 48 00 00                                     iH..            

;; fn00009784: 00009784
;;   Called from:
;;     0000F2E0 (in logsoftmax_execute)
;;     00016AB8 (in qsoftmax_execute_ref)
;;     00016B10 (in qsoftmax_execute_ref)
;;     0001C458 (in softmax_execute)
;;     0001CED8 (in lrn_f_execute)
fn00009784 proc
	{ r14 = add(badva,00000004) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009790: 00009790
;;   Called from:
;;     0001CEC8 (in lrn_f_execute)
fn00009790 proc
	{ r14 = add(badva,00000038) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn000097A0: 000097A0
;;   Called from:
;;     00012644 (in check_execute)
fn000097A0 proc
	{ r14 = add(badva,0000002C) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn000097B0: 000097B0
;;   Called from:
;;     000133C4 (in fn00013214)
;;     00016B20 (in qsoftmax_execute_ref)
;;     000175CC (in quantize_execute)
;;     000181D8 (in autorequantize_execute)
;;     00018550 (in requantize_execute)
;;     0001C300 (in reluX_execute)
;;     00021758 (in prelu_execute)
fn000097B0 proc
	{ r14 = add(badva,00000020) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn000097C0: 000097C0
;;   Called from:
;;     0001353C (in concat_execute_slice_ref)
;;     0001E6DC (in qtanh_execute_hvx)
;;     0001ED4C (in qsigmoid_execute_hvx)
fn000097C0 proc
	{ r14 = add(badva,00000014) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }
000097D0 68 48 00 00                                     hH..            

;; fn000097D4: 000097D4
;;   Called from:
;;     00017610 (in quantize_execute)
;;     0001822C (in autorequantize_execute)
;;     000185A0 (in requantize_execute)
fn000097D4 proc
	{ r14 = add(badva,00000008) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn000097E0: 000097E0
;;   Called from:
;;     00018C88 (in supernode_execute_hvx)
fn000097E0 proc
	{ r14 = add(badva,0000003C) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn000097F0: 000097F0
;;   Called from:
;;     0001E0AC (in tanh_execute)
fn000097F0 proc
	{ r14 = add(badva,00000030) }

;; fn000097F4: 000097F4
;;   Called from:
;;     0001E254 (in sigmoid_execute)
;;     0001E46C (in qtanh_execute_ref)
;;     0001EAE4 (in qsigmoid_execute_ref)
fn000097F4 proc
	{ r14 = add(badva,00000030) }

l000097F8:
	{ r28 = memw(r14) }
	{ jumpr	r28 }

;; fn00009800: 00009800
;;   Called from:
;;     00020C6C (in execute_finstancenorm)
fn00009800 proc
	{ r14 = add(badva,00000024) }
	{ r28 = memw(r14) }
	{ jumpr	r28 }
