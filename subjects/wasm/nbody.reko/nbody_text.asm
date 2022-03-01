;;; Segment .text (000E0000)
; WASM functions
; (func _advance (; 0 ;) retval??
; 	block
; 		get_local 0x0
; 		i32.const 0x0
; 		i32.gt_s
; 		tee_local 0x9
; 		if
; 			i32.const 0x0
; 			set_local 0x3
; 			else
; 			return
; 		end
; 		loop
; 			get_local 0x3
; 			i32.const 0x1
; 			i32.add
; 			tee_local 0x4
; 			get_local 0x0
; 			i32.lt_s
; 			if
; 				get_local 0x1
; 				get_local 0x3
; 				i32.const 0x38
; 				i32.mul
; 				i32.add
; 				f64.load 0x3,0x0
; 				set_local 0x10
; 				get_local 0x1
; 				get_local 0x3
; 				i32.const 0x38
; 				i32.mul
; 				i32.add
; 				f64.load 0x3,0x8
; 				set_local 0x11
; 				get_local 0x1
; 				get_local 0x3
; 				i32.const 0x38
; 				i32.mul
; 				i32.add
; 				f64.load 0x3,0x10
; 				set_local 0x12
; 				get_local 0x1
; 				get_local 0x3
; 				i32.const 0x38
; 				i32.mul
; 				i32.add
; 				i32.const 0x18
; 				i32.add
; 				set_local 0x6
; 				get_local 0x1
; 				get_local 0x3
; 				i32.const 0x38
; 				i32.mul
; 				i32.add
; 				i32.const 0x20
; 				i32.add
; 				set_local 0x7
; 				get_local 0x1
; 				get_local 0x3
; 				i32.const 0x38
; 				i32.mul
; 				i32.add
; 				i32.const 0x28
; 				i32.add
; 				set_local 0x8
; 				get_local 0x1
; 				get_local 0x3
; 				i32.const 0x38
; 				i32.mul
; 				i32.add
; 				f64.load 0x3,0x30
; 				set_local 0xC
; 				get_local 0x4
; 				set_local 0x3
; 				loop
; 					get_local 0x2
; 					get_local 0x10
; 					get_local 0x1
; 					get_local 0x3
; 					i32.const 0x38
; 					i32.mul
; 					i32.add
; 					f64.load 0x3,0x0
; 					f64.sub
; 					tee_local 0xD
; 					get_local 0xD
; 					f64.mul
; 					get_local 0x11
; 					get_local 0x1
; 					get_local 0x3
; 					i32.const 0x38
; 					i32.mul
; 					i32.add
; 					f64.load 0x3,0x8
; 					f64.sub
; 					tee_local 0xE
; 					get_local 0xE
; 					f64.mul
; 					f64.add
; 					get_local 0x12
; 					get_local 0x1
; 					get_local 0x3
; 					i32.const 0x38
; 					i32.mul
; 					i32.add
; 					f64.load 0x3,0x10
; 					f64.sub
; 					tee_local 0xF
; 					get_local 0xF
; 					f64.mul
; 					f64.add
; 					f64.sqrt
; 					tee_local 0xB
; 					get_local 0xB
; 					get_local 0xB
; 					f64.mul
; 					f64.mul
; 					f64.div
; 					set_local 0xA
; 					get_local 0x6
; 					get_local 0x6
; 					f64.load 0x3,0x0
; 					get_local 0xD
; 					get_local 0x1
; 					get_local 0x3
; 					i32.const 0x38
; 					i32.mul
; 					i32.add
; 					f64.load 0x3,0x30
; 					tee_local 0xB
; 					f64.mul
; 					get_local 0xA
; 					f64.mul
; 					f64.sub
; 					f64.store 0x3,0x0
; 					get_local 0x7
; 					get_local 0x7
; 					f64.load 0x3,0x0
; 					get_local 0xA
; 					get_local 0xE
; 					get_local 0xB
; 					f64.mul
; 					f64.mul
; 					f64.sub
; 					f64.store 0x3,0x0
; 					get_local 0x8
; 					get_local 0x8
; 					f64.load 0x3,0x0
; 					get_local 0xA
; 					get_local 0xF
; 					get_local 0xB
; 					f64.mul
; 					f64.mul
; 					f64.sub
; 					f64.store 0x3,0x0
; 					get_local 0x1
; 					get_local 0x3
; 					i32.const 0x38
; 					i32.mul
; 					i32.add
; 					i32.const 0x18
; 					i32.add
; 					tee_local 0x5
; 					get_local 0x5
; 					f64.load 0x3,0x0
; 					get_local 0xA
; 					get_local 0xD
; 					get_local 0xC
; 					f64.mul
; 					f64.mul
; 					f64.add
; 					f64.store 0x3,0x0
; 					get_local 0x1
; 					get_local 0x3
; 					i32.const 0x38
; 					i32.mul
; 					i32.add
; 					i32.const 0x20
; 					i32.add
; 					tee_local 0x5
; 					get_local 0x5
; 					f64.load 0x3,0x0
; 					get_local 0xA
; 					get_local 0xE
; 					get_local 0xC
; 					f64.mul
; 					f64.mul
; 					f64.add
; 					f64.store 0x3,0x0
; 					get_local 0x1
; 					get_local 0x3
; 					i32.const 0x38
; 					i32.mul
; 					i32.add
; 					i32.const 0x28
; 					i32.add
; 					tee_local 0x5
; 					get_local 0x5
; 					f64.load 0x3,0x0
; 					get_local 0xA
; 					get_local 0xF
; 					get_local 0xC
; 					f64.mul
; 					f64.mul
; 					f64.add
; 					f64.store 0x3,0x0
; 					get_local 0x3
; 					i32.const 0x1
; 					i32.add
; 					tee_local 0x3
; 					get_local 0x0
; 					i32.ne
; 					br_if 0x0
; 				end
; 			end
; 			get_local 0x4
; 			get_local 0x0
; 			i32.ne
; 			if
; 				get_local 0x4
; 				set_local 0x3
; 				br 0x1
; 			end
; 		end
; 		get_local 0x9
; 		if
; 			i32.const 0x0
; 			set_local 0x4
; 			else
; 			return
; 		end
; 		loop
; 			get_local 0x1
; 			get_local 0x4
; 			i32.const 0x38
; 			i32.mul
; 			i32.add
; 			tee_local 0x3
; 			get_local 0x3
; 			f64.load 0x3,0x0
; 			get_local 0x1
; 			get_local 0x4
; 			i32.const 0x38
; 			i32.mul
; 			i32.add
; 			f64.load 0x3,0x18
; 			get_local 0x2
; 			f64.mul
; 			f64.add
; 			f64.store 0x3,0x0
; 			get_local 0x1
; 			get_local 0x4
; 			i32.const 0x38
; 			i32.mul
; 			i32.add
; 			i32.const 0x8
; 			i32.add
; 			tee_local 0x3
; 			get_local 0x3
; 			f64.load 0x3,0x0
; 			get_local 0x1
; 			get_local 0x4
; 			i32.const 0x38
; 			i32.mul
; 			i32.add
; 			f64.load 0x3,0x20
; 			get_local 0x2
; 			f64.mul
; 			f64.add
; 			f64.store 0x3,0x0
; 			get_local 0x1
; 			get_local 0x4
; 			i32.const 0x38
; 			i32.mul
; 			i32.add
; 			i32.const 0x10
; 			i32.add
; 			tee_local 0x3
; 			get_local 0x3
; 			f64.load 0x3,0x0
; 			get_local 0x1
; 			get_local 0x4
; 			i32.const 0x38
; 			i32.mul
; 			i32.add
; 			f64.load 0x3,0x28
; 			get_local 0x2
; 			f64.mul
; 			f64.add
; 			f64.store 0x3,0x0
; 			get_local 0x4
; 			i32.const 0x1
; 			i32.add
; 			tee_local 0x4
; 			get_local 0x0
; 			i32.ne
; 			br_if 0x0
; 		end
; 	end
; )
; 
; (func _energy (; 1 ;) retval??
; block 0x7C
; 	get_local 0x0
; 	i32.const 0x0
; 	i32.gt_s
; 	if
; 		i32.const 0x0
; 		set_local 0x2
; 		f64.const 0
; 		set_local 0x4
; 		else
; 		f64.const 0
; 		return
; 	end
; 	loop
; 		get_local 0x4
; 		get_local 0x1
; 		get_local 0x2
; 		i32.const 0x38
; 		i32.mul
; 		i32.add
; 		f64.load 0x3,0x30
; 		tee_local 0x5
; 		f64.const 0.5
; 		f64.mul
; 		get_local 0x1
; 		get_local 0x2
; 		i32.const 0x38
; 		i32.mul
; 		i32.add
; 		f64.load 0x3,0x18
; 		tee_local 0x4
; 		get_local 0x4
; 		f64.mul
; 		get_local 0x1
; 		get_local 0x2
; 		i32.const 0x38
; 		i32.mul
; 		i32.add
; 		f64.load 0x3,0x20
; 		tee_local 0x4
; 		get_local 0x4
; 		f64.mul
; 		f64.add
; 		get_local 0x1
; 		get_local 0x2
; 		i32.const 0x38
; 		i32.mul
; 		i32.add
; 		f64.load 0x3,0x28
; 		tee_local 0x4
; 		get_local 0x4
; 		f64.mul
; 		f64.add
; 		f64.mul
; 		f64.add
; 		set_local 0x4
; 		get_local 0x2
; 		i32.const 0x1
; 		i32.add
; 		tee_local 0x3
; 		get_local 0x0
; 		i32.lt_s
; 		if
; 			get_local 0x1
; 			get_local 0x2
; 			i32.const 0x38
; 			i32.mul
; 			i32.add
; 			f64.load 0x3,0x0
; 			set_local 0x6
; 			get_local 0x1
; 			get_local 0x2
; 			i32.const 0x38
; 			i32.mul
; 			i32.add
; 			f64.load 0x3,0x8
; 			set_local 0x7
; 			get_local 0x1
; 			get_local 0x2
; 			i32.const 0x38
; 			i32.mul
; 			i32.add
; 			f64.load 0x3,0x10
; 			set_local 0x8
; 			get_local 0x3
; 			set_local 0x2
; 			loop
; 				get_local 0x4
; 				get_local 0x5
; 				get_local 0x1
; 				get_local 0x2
; 				i32.const 0x38
; 				i32.mul
; 				i32.add
; 				f64.load 0x3,0x30
; 				f64.mul
; 				get_local 0x6
; 				get_local 0x1
; 				get_local 0x2
; 				i32.const 0x38
; 				i32.mul
; 				i32.add
; 				f64.load 0x3,0x0
; 				f64.sub
; 				tee_local 0x4
; 				get_local 0x4
; 				f64.mul
; 				get_local 0x7
; 				get_local 0x1
; 				get_local 0x2
; 				i32.const 0x38
; 				i32.mul
; 				i32.add
; 				f64.load 0x3,0x8
; 				f64.sub
; 				tee_local 0x4
; 				get_local 0x4
; 				f64.mul
; 				f64.add
; 				get_local 0x8
; 				get_local 0x1
; 				get_local 0x2
; 				i32.const 0x38
; 				i32.mul
; 				i32.add
; 				f64.load 0x3,0x10
; 				f64.sub
; 				tee_local 0x4
; 				get_local 0x4
; 				f64.mul
; 				f64.add
; 				f64.sqrt
; 				f64.div
; 				f64.sub
; 				set_local 0x4
; 				get_local 0x2
; 				i32.const 0x1
; 				i32.add
; 				tee_local 0x2
; 				get_local 0x0
; 				i32.ne
; 				br_if 0x0
; 			end
; 		end
; 		get_local 0x3
; 		get_local 0x0
; 		i32.ne
; 		if
; 			get_local 0x3
; 			set_local 0x2
; 			br 0x1
; 		end
; 	end
; 	get_local 0x4
; end
; )
; 
; (func _offset_momentum (; 2 ;) retval??
; block
; get_local 0x0
; i32.const 0x0
; i32.gt_s
; if
; 	f64.const 0
; 	set_local 0x3
; 	f64.const 0
; 	set_local 0x4
; 	f64.const 0
; 	set_local 0x5
; 	i32.const 0x0
; 	set_local 0x2
; 	loop
; 		get_local 0x5
; 		get_local 0x1
; 		get_local 0x2
; 		i32.const 0x38
; 		i32.mul
; 		i32.add
; 		f64.load 0x3,0x18
; 		get_local 0x1
; 		get_local 0x2
; 		i32.const 0x38
; 		i32.mul
; 		i32.add
; 		f64.load 0x3,0x30
; 		tee_local 0x6
; 		f64.mul
; 		f64.add
; 		set_local 0x5
; 		get_local 0x4
; 		get_local 0x6
; 		get_local 0x1
; 		get_local 0x2
; 		i32.const 0x38
; 		i32.mul
; 		i32.add
; 		f64.load 0x3,0x20
; 		f64.mul
; 		f64.add
; 		set_local 0x4
; 		get_local 0x3
; 		get_local 0x6
; 		get_local 0x1
; 		get_local 0x2
; 		i32.const 0x38
; 		i32.mul
; 		i32.add
; 		f64.load 0x3,0x28
; 		f64.mul
; 		f64.add
; 		set_local 0x3
; 		get_local 0x2
; 		i32.const 0x1
; 		i32.add
; 		tee_local 0x2
; 		get_local 0x0
; 		i32.ne
; 		br_if 0x0
; 	end
; 	else
; 	f64.const 0
; 	set_local 0x3
; 	f64.const 0
; 	set_local 0x4
; 	f64.const 0
; 	set_local 0x5
; end
; get_local 0x1
; get_local 0x5
; f64.neg
; f64.const 39.47841760435743
; f64.div
; f64.store 0x3,0x18
; get_local 0x1
; get_local 0x4
; f64.neg
; f64.const 39.47841760435743
; f64.div
; f64.store 0x3,0x20
; get_local 0x1
; get_local 0x3
; f64.neg
; f64.const 39.47841760435743
; f64.div
; f64.store 0x3,0x28
; end
; )
; 
; (func _start (; 3 ;) retval??
; block 0x7C
; get_global 0x0
; f64.load 0x3,0x30
; tee_local 0x6
; get_global 0x0
; f64.load 0x3,0x20
; f64.mul
; f64.const 0
; f64.add
; get_global 0x0
; f64.load 0x3,0x68
; tee_local 0x7
; get_global 0x0
; f64.load 0x3,0x58
; f64.mul
; f64.add
; get_global 0x0
; f64.load 0x3,0xA0
; tee_local 0x8
; get_global 0x0
; f64.load 0x3,0x90
; f64.mul
; f64.add
; get_global 0x0
; f64.load 0x3,0xD8
; tee_local 0x3
; get_global 0x0
; f64.load 0x3,0xC8
; f64.mul
; f64.add
; get_global 0x0
; f64.load 0x3,0x110
; tee_local 0x4
; get_global 0x0
; f64.load 0x3,0x100
; f64.mul
; f64.add
; set_local 0x5
; get_local 0x6
; get_global 0x0
; f64.load 0x3,0x28
; f64.mul
; f64.const 0
; f64.add
; get_local 0x7
; get_global 0x0
; f64.load 0x3,0x60
; f64.mul
; f64.add
; get_local 0x8
; get_global 0x0
; f64.load 0x3,0x98
; f64.mul
; f64.add
; get_local 0x3
; get_global 0x0
; f64.load 0x3,0xD0
; f64.mul
; f64.add
; get_local 0x4
; get_global 0x0
; f64.load 0x3,0x108
; f64.mul
; f64.add
; set_local 0x2
; get_global 0x0
; get_global 0x0
; f64.load 0x3,0x18
; get_local 0x6
; f64.mul
; f64.const 0
; f64.add
; get_global 0x0
; f64.load 0x3,0x50
; get_local 0x7
; f64.mul
; f64.add
; get_global 0x0
; f64.load 0x3,0x88
; get_local 0x8
; f64.mul
; f64.add
; get_global 0x0
; f64.load 0x3,0xC0
; get_local 0x3
; f64.mul
; f64.add
; get_global 0x0
; f64.load 0x3,0xF8
; get_local 0x4
; f64.mul
; f64.add
; f64.neg
; f64.const 39.47841760435743
; f64.div
; tee_local 0x3
; f64.store 0x3,0x18
; get_global 0x0
; get_local 0x5
; f64.neg
; f64.const 39.47841760435743
; f64.div
; tee_local 0x4
; f64.store 0x3,0x20
; get_global 0x0
; get_local 0x2
; f64.neg
; f64.const 39.47841760435743
; f64.div
; tee_local 0x5
; f64.store 0x3,0x28
; i32.const 0x0
; set_local 0x0
; f64.const 0
; set_local 0x2
; loop
; get_local 0x2
; get_local 0x6
; f64.const 0.5
; f64.mul
; get_local 0x3
; get_local 0x3
; f64.mul
; get_local 0x4
; get_local 0x4
; f64.mul
; f64.add
; get_local 0x5
; get_local 0x5
; f64.mul
; f64.add
; f64.mul
; f64.add
; set_local 0x2
; get_local 0x0
; i32.const 0x1
; i32.add
; tee_local 0x1
; i32.const 0x5
; i32.lt_s
; if
; 	get_global 0x0
; 	get_local 0x0
; 	i32.const 0x38
; 	i32.mul
; 	i32.add
; 	f64.load 0x3,0x0
; 	set_local 0x3
; 	get_global 0x0
; 	get_local 0x0
; 	i32.const 0x38
; 	i32.mul
; 	i32.add
; 	f64.load 0x3,0x8
; 	set_local 0x4
; 	get_global 0x0
; 	get_local 0x0
; 	i32.const 0x38
; 	i32.mul
; 	i32.add
; 	f64.load 0x3,0x10
; 	set_local 0x5
; 	get_local 0x1
; 	set_local 0x0
; 	loop
; 		get_local 0x2
; 		get_local 0x6
; 		get_global 0x0
; 		get_local 0x0
; 		i32.const 0x38
; 		i32.mul
; 		i32.add
; 		f64.load 0x3,0x30
; 		f64.mul
; 		get_local 0x3
; 		get_global 0x0
; 		get_local 0x0
; 		i32.const 0x38
; 		i32.mul
; 		i32.add
; 		f64.load 0x3,0x0
; 		f64.sub
; 		tee_local 0x2
; 		get_local 0x2
; 		f64.mul
; 		get_local 0x4
; 		get_global 0x0
; 		get_local 0x0
; 		i32.const 0x38
; 		i32.mul
; 		i32.add
; 		f64.load 0x3,0x8
; 		f64.sub
; 		tee_local 0x2
; 		get_local 0x2
; 		f64.mul
; 		f64.add
; 		get_local 0x5
; 		get_global 0x0
; 		get_local 0x0
; 		i32.const 0x38
; 		i32.mul
; 		i32.add
; 		f64.load 0x3,0x10
; 		f64.sub
; 		tee_local 0x2
; 		get_local 0x2
; 		f64.mul
; 		f64.add
; 		f64.sqrt
; 		f64.div
; 		f64.sub
; 		set_local 0x2
; 		get_local 0x0
; 		i32.const 0x1
; 		i32.add
; 		tee_local 0x0
; 		i32.const 0x5
; 		i32.ne
; 		br_if 0x0
; 	end
; end
; get_local 0x1
; i32.const 0x5
; i32.ne
; if
; 	get_local 0x1
; 	set_local 0x0
; 	get_global 0x0
; 	get_local 0x1
; 	i32.const 0x38
; 	i32.mul
; 	i32.add
; 	f64.load 0x3,0x30
; 	set_local 0x6
; 	get_global 0x0
; 	get_local 0x1
; 	i32.const 0x38
; 	i32.mul
; 	i32.add
; 	f64.load 0x3,0x18
; 	set_local 0x3
; 	get_global 0x0
; 	get_local 0x1
; 	i32.const 0x38
; 	i32.mul
; 	i32.add
; 	f64.load 0x3,0x20
; 	set_local 0x4
; 	get_global 0x0
; 	get_local 0x1
; 	i32.const 0x38
; 	i32.mul
; 	i32.add
; 	f64.load 0x3,0x28
; 	set_local 0x5
; 	br 0x1
; end
; end
; get_local 0x2
; end
; )
; 
; (func _run (; 4 ;) retval??
; block 0x7C
; get_local 0x0
; i32.const 0x1
; i32.lt_s
; if
; i32.const 0x0
; set_local 0x0
; f64.const 0
; set_local 0x3
; else
; i32.const 0x1
; set_local 0x1
; loop
; i32.const 0x5
; get_global 0x0
; f64.const 0.01
; call 0x0
; get_local 0x1
; i32.const 0x1
; i32.add
; set_local 0x2
; get_local 0x1
; get_local 0x0
; i32.eq
; if
; 	i32.const 0x0
; 	set_local 0x0
; 	f64.const 0
; 	set_local 0x3
; 	else
; 	get_local 0x2
; 	set_local 0x1
; 	br 0x1
; end
; end
; end
; loop
; get_local 0x3
; get_global 0x0
; get_local 0x0
; i32.const 0x38
; i32.mul
; i32.add
; f64.load 0x3,0x30
; tee_local 0x4
; f64.const 0.5
; f64.mul
; get_global 0x0
; get_local 0x0
; i32.const 0x38
; i32.mul
; i32.add
; f64.load 0x3,0x18
; tee_local 0x3
; get_local 0x3
; f64.mul
; get_global 0x0
; get_local 0x0
; i32.const 0x38
; i32.mul
; i32.add
; f64.load 0x3,0x20
; tee_local 0x3
; get_local 0x3
; f64.mul
; f64.add
; get_global 0x0
; get_local 0x0
; i32.const 0x38
; i32.mul
; i32.add
; f64.load 0x3,0x28
; tee_local 0x3
; get_local 0x3
; f64.mul
; f64.add
; f64.mul
; f64.add
; set_local 0x3
; get_local 0x0
; i32.const 0x1
; i32.add
; tee_local 0x1
; i32.const 0x5
; i32.lt_s
; if
; get_global 0x0
; get_local 0x0
; i32.const 0x38
; i32.mul
; i32.add
; f64.load 0x3,0x0
; set_local 0x5
; get_global 0x0
; get_local 0x0
; i32.const 0x38
; i32.mul
; i32.add
; f64.load 0x3,0x8
; set_local 0x6
; get_global 0x0
; get_local 0x0
; i32.const 0x38
; i32.mul
; i32.add
; f64.load 0x3,0x10
; set_local 0x7
; get_local 0x1
; set_local 0x0
; loop
; 	get_local 0x3
; 	get_local 0x4
; 	get_global 0x0
; 	get_local 0x0
; 	i32.const 0x38
; 	i32.mul
; 	i32.add
; 	f64.load 0x3,0x30
; 	f64.mul
; 	get_local 0x5
; 	get_global 0x0
; 	get_local 0x0
; 	i32.const 0x38
; 	i32.mul
; 	i32.add
; 	f64.load 0x3,0x0
; 	f64.sub
; 	tee_local 0x3
; 	get_local 0x3
; 	f64.mul
; 	get_local 0x6
; 	get_global 0x0
; 	get_local 0x0
; 	i32.const 0x38
; 	i32.mul
; 	i32.add
; 	f64.load 0x3,0x8
; 	f64.sub
; 	tee_local 0x3
; 	get_local 0x3
; 	f64.mul
; 	f64.add
; 	get_local 0x7
; 	get_global 0x0
; 	get_local 0x0
; 	i32.const 0x38
; 	i32.mul
; 	i32.add
; 	f64.load 0x3,0x10
; 	f64.sub
; 	tee_local 0x3
; 	get_local 0x3
; 	f64.mul
; 	f64.add
; 	f64.sqrt
; 	f64.div
; 	f64.sub
; 	set_local 0x3
; 	get_local 0x0
; 	i32.const 0x1
; 	i32.add
; 	tee_local 0x0
; 	i32.const 0x5
; 	i32.ne
; 	br_if 0x0
; end
; end
; get_local 0x1
; i32.const 0x5
; i32.ne
; if
; get_local 0x1
; set_local 0x0
; br 0x1
; end
; end
; get_local 0x3
; end
; )
; 
; (func runPostSets (; 5 ;) retval??
; nop
; )
; 
; (func __post_instantiate (; 6 ;) retval??
; block
; get_global 0x0
; i32.const 0x120
; i32.add
; set_global 0x2
; get_global 0x2
; i32.const 0x500000
; i32.add
; set_global 0x3
; call 0x5
; end
; )
