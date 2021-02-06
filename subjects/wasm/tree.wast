(module
  (type $FUNCSIG$vi (func (param i32)))
  (import "env" "_Z9frobulateP3foo" (func $_Z9frobulateP3foo (param i32)))
  (import "env" "g_ptr" (global $g_ptr i32))
  (table 0 anyfunc)
  (memory $0 1)
  (export "memory" (memory $0))
  (export "_Z6locateiP3foo" (func $_Z6locateiP3foo))
  (export "_Z11irreducibleP3foo" (func $_Z11irreducibleP3foo))
  (export "_Z4evalP3foo" (func $_Z4evalP3foo))
  (export "main" (func $main))
  (func $_Z6locateiP3foo (param $0 i32) (param $1 i32) (result i32)
    (local $2 i32)
    (block $label$0
      (block $label$1
        (br_if $label$1
          (i32.eqz
            (get_local $1)
          )
        )
        (loop $label$2
          (block $label$3
            (br_if $label$3
              (i32.gt_s
                (tee_local $2
                  (i32.load offset=8
                    (get_local $1)
                  )
                )
                (get_local $0)
              )
            )
            (br_if $label$0
              (i32.ge_s
                (get_local $2)
                (get_local $0)
              )
            )
            (set_local $1
              (i32.add
                (get_local $1)
                (i32.const 4)
              )
            )
          )
          (br_if $label$2
            (tee_local $1
              (i32.load
                (get_local $1)
              )
            )
          )
        )
      )
      (return
        (i32.const 0)
      )
    )
    (get_local $1)
  )
  (func $_Z11irreducibleP3foo (param $0 i32) (result i32)
    (local $1 i32)
    (block $label$0
      (block $label$1
        (br_if $label$1
          (i32.lt_s
            (tee_local $0
              (i32.load offset=8
                (get_local $0)
              )
            )
            (i32.const 1)
          )
        )
        (set_local $0
          (i32.shl
            (get_local $0)
            (i32.const 1)
          )
        )
        (set_local $1
          (i32.const 1)
        )
        (br $label$0)
      )
      (set_local $1
        (i32.const 0)
      )
    )
    (loop $label$2 i32
      (block $label$3
        (block $label$4
          (block $label$5
            (block $label$6
              (br_table $label$5 $label$6 $label$4 $label$4
                (get_local $1)
              )
            )
            (set_local $0
              (i32.shl
                (get_local $0)
                (i32.const 1)
              )
            )
            (set_local $1
              (i32.const 0)
            )
            (br $label$2)
          )
          (br_if $label$3
            (i32.and
              (tee_local $0
                (i32.xor
                  (get_local $0)
                  (i32.const 16843009)
                )
              )
              (i32.const 1)
            )
          )
          (set_local $1
            (i32.const 2)
          )
          (br $label$2)
        )
        (return
          (get_local $0)
        )
      )
      (set_local $1
        (i32.const 1)
      )
      (br $label$2)
    )
  )
  (func $_Z4evalP3foo (param $0 i32) (result i32)
    (local $1 i32)
    (local $2 i32)
    (set_local $2
      (i32.shr_s
        (tee_local $1
          (i32.load offset=8
            (get_local $0)
          )
        )
        (i32.const 1)
      )
    )
    (block $label$0
      (block $label$1
        (block $label$2
          (block $label$3
            (block $label$4
              (block $label$5
                (br_if $label$5
                  (i32.and
                    (get_local $1)
                    (i32.const 1)
                  )
                )
                (block $label$6
                  (br_if $label$6
                    (i32.gt_u
                      (get_local $2)
                      (i32.const 5)
                    )
                  )
                  (block $label$7
                    (br_table $label$7 $label$4 $label$3 $label$2 $label$1 $label$0 $label$7
                      (get_local $2)
                    )
                  )
                  (return
                    (i32.add
                      (call $_Z4evalP3foo
                        (i32.load
                          (get_local $0)
                        )
                      )
                      (call $_Z4evalP3foo
                        (i32.load offset=4
                          (get_local $0)
                        )
                      )
                    )
                  )
                )
                (set_local $2
                  (i32.const 0)
                )
              )
              (return
                (get_local $2)
              )
            )
            (return
              (i32.mul
                (call $_Z4evalP3foo
                  (i32.load
                    (get_local $0)
                  )
                )
                (call $_Z4evalP3foo
                  (i32.load offset=4
                    (get_local $0)
                  )
                )
              )
            )
          )
          (return
            (i32.sub
              (call $_Z4evalP3foo
                (i32.load
                  (get_local $0)
                )
              )
              (call $_Z4evalP3foo
                (i32.load offset=4
                  (get_local $0)
                )
              )
            )
          )
        )
        (return
          (i32.div_s
            (call $_Z4evalP3foo
              (i32.load
                (get_local $0)
              )
            )
            (call $_Z4evalP3foo
              (i32.load offset=4
                (get_local $0)
              )
            )
          )
        )
      )
      (return
        (i32.sub
          (i32.const 0)
          (call $_Z4evalP3foo
            (i32.load
              (get_local $0)
            )
          )
        )
      )
    )
    (i32.div_s
      (call $_Z4evalP3foo
        (i32.load
          (get_local $0)
        )
      )
      (call $_Z4evalP3foo
        (i32.load offset=4
          (get_local $0)
        )
      )
    )
  )
  (func $main (result i32)
    (local $0 i32)
    (local $1 i32)
    (set_local $0
      (get_global $g_ptr)
    )
    (call $_Z9frobulateP3foo
      (get_global $g_ptr)
    )
    (block $label$0
      (loop $label$1
        (block $label$2
          (br_if $label$2
            (i32.ge_s
              (tee_local $1
                (i32.load offset=8
                  (get_local $0)
                )
              )
              (i32.const 33)
            )
          )
          (br_if $label$0
            (i32.eq
              (get_local $1)
              (i32.const 32)
            )
          )
          (set_local $0
            (i32.add
              (get_local $0)
              (i32.const 4)
            )
          )
        )
        (br_if $label$1
          (tee_local $0
            (i32.load
              (get_local $0)
            )
          )
        )
      )
      (set_local $0
        (i32.const 0)
      )
    )
    (i32.store offset=28
      (i32.const 0)
      (get_local $0)
    )
    (i32.const 0)
  )
)
