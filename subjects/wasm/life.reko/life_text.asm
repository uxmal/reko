;;; Segment .text (000C0000)
; WASM functions
; (func count_neighbors (; 0 ;) retval??
; 	get_local 0x0
; 	i32.const 0x2710
; 	i32.mul
; 	i32.const 0x10
; 	i32.add
; 	tee_local 0x0
; 	get_local 0x2
; 	i32.const 0x7F
; 	i32.add
; 	i32.const 0x64
; 	i32.rem_s
; 	i32.const 0x64
; 	i32.mul
; 	i32.add
; 	tee_local 0x3
; 	get_local 0x1
; 	i32.const 0x7F
; 	i32.add
; 	i32.const 0x64
; 	i32.rem_s
; 	tee_local 0x4
; 	i32.add
; 	i32.load8_u 0x0,0x0
; 	get_local 0x3
; 	get_local 0x1
; 	i32.const 0x1
; 	i32.add
; 	i32.const 0x64
; 	i32.rem_s
; 	tee_local 0x1
; 	i32.add
; 	i32.load8_u 0x0,0x0
; 	i32.add
; 	get_local 0x0
; 	get_local 0x2
; 	i32.const 0x1
; 	i32.add
; 	i32.const 0x64
; 	i32.rem_s
; 	i32.const 0x64
; 	i32.mul
; 	i32.add
; 	tee_local 0x2
; 	get_local 0x4
; 	i32.add
; 	i32.load8_u 0x0,0x0
; 	i32.add
; 	get_local 0x2
; 	get_local 0x1
; 	i32.add
; 	i32.load8_u 0x0,0x0
; 	i32.add
; )
; 
; (func update_gen (; 1 ;) retval??
; get_local 0x0
; i32.const 0x2710
; i32.mul
; tee_local 0x6
; i32.const 0x10
; i32.add
; set_local 0x1
; i32.const 0x1
; get_local 0x0
; i32.sub
; i32.const 0x2710
; i32.mul
; i32.const 0x10
; i32.add
; set_local 0x2
; i32.const 0x0
; set_local 0x0
; loop
; 	i32.const 0x0
; 	get_local 0x0
; 	i32.const 0x1
; 	i32.add
; 	tee_local 0x4
; 	get_local 0x4
; 	i32.const 0x64
; 	i32.eq
; 	tee_local 0x7
; 	select
; 	set_local 0x5
; 	get_local 0x0
; 	i32.const 0x7F
; 	i32.add
; 	i32.const 0x64
; 	i32.rem_s
; 	set_local 0x3
; 	i32.const 0x0
; 	set_local 0xC
; 	loop
; 		i32.const 0x0
; 		set_local 0xD
; 		block
; 			block
; 				get_local 0x6
; 				i32.const 0x10
; 				i32.add
; 				tee_local 0x9
; 				get_local 0x3
; 				i32.const 0x64
; 				i32.mul
; 				i32.add
; 				tee_local 0xA
; 				i32.const 0x0
; 				get_local 0xC
; 				tee_local 0x0
; 				i32.const 0x1
; 				i32.add
; 				tee_local 0xC
; 				get_local 0x0
; 				i32.const 0x63
; 				i32.eq
; 				select
; 				tee_local 0x8
; 				i32.add
; 				i32.load8_u 0x0,0x0
; 				get_local 0xA
; 				get_local 0x0
; 				i32.const 0x7F
; 				i32.add
; 				i32.const 0x64
; 				i32.rem_s
; 				tee_local 0xB
; 				i32.add
; 				i32.load8_u 0x0,0x0
; 				i32.add
; 				get_local 0x9
; 				get_local 0x5
; 				i32.const 0x64
; 				i32.mul
; 				i32.add
; 				tee_local 0x9
; 				get_local 0xB
; 				i32.add
; 				i32.load8_u 0x0,0x0
; 				i32.add
; 				get_local 0x9
; 				get_local 0x8
; 				i32.add
; 				i32.load8_u 0x0,0x0
; 				i32.add
; 				tee_local 0x9
; 				i32.const 0x3
; 				i32.eq
; 				br_if 0x0
; 				get_local 0x9
; 				i32.const 0x2
; 				i32.ne
; 				br_if 0x1
; 				get_local 0x1
; 				get_local 0x0
; 				i32.add
; 				i32.load8_u 0x0,0x0
; 				set_local 0xD
; 				br 0x1
; 			end
; 			i32.const 0x1
; 			set_local 0xD
; 		end
; 		get_local 0x2
; 		get_local 0x0
; 		i32.add
; 		get_local 0xD
; 		i32.store8 0x0,0x0
; 		get_local 0xC
; 		i32.const 0x64
; 		i32.ne
; 		br_if 0x0
; 	end
; 	get_local 0x1
; 	i32.const 0x64
; 	i32.add
; 	set_local 0x1
; 	get_local 0x2
; 	i32.const 0x64
; 	i32.add
; 	set_local 0x2
; 	get_local 0x4
; 	set_local 0x0
; 	get_local 0x7
; 	i32.eqz
; 	br_if 0x0
; end
; )
; 
; (func main (; 2 ;) retval??
; i32.const 0x2A
; )
