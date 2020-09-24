(module
  (type (;0;) (func (param i32 i32 f64)))
  (type (;1;) (func (param i32 i32) (result f64)))
  (type (;2;) (func (param i32 i32)))
  (type (;3;) (func (result f64)))
  (type (;4;) (func (param i32) (result f64)))
  (type (;5;) (func))
  (import "env" "memoryBase" (global (;0;) i32))
  (import "env" "memory" (memory (;0;) 256))
  (import "env" "table" (table (;0;) 0 anyfunc))
  (import "env" "tableBase" (global (;1;) i32))
  (func (;0;) (type 0) (param i32 i32 f64)
    (local i32 i32 i32 i32 i32 i32 i32 f64 f64 f64 f64 f64 f64 f64 f64 f64)
    block  ;; label = @1
      get_local 0
      i32.const 0
      i32.gt_s
      tee_local 9
      if  ;; label = @2
        i32.const 0
        set_local 3
      else
        return
      end
      loop  ;; label = @2
        get_local 3
        i32.const 1
        i32.add
        tee_local 4
        get_local 0
        i32.lt_s
        if  ;; label = @3
          get_local 1
          get_local 3
          i32.const 56
          i32.mul
          i32.add
          f64.load
          set_local 16
          get_local 1
          get_local 3
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=8
          set_local 17
          get_local 1
          get_local 3
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=16
          set_local 18
          get_local 1
          get_local 3
          i32.const 56
          i32.mul
          i32.add
          i32.const 24
          i32.add
          set_local 6
          get_local 1
          get_local 3
          i32.const 56
          i32.mul
          i32.add
          i32.const 32
          i32.add
          set_local 7
          get_local 1
          get_local 3
          i32.const 56
          i32.mul
          i32.add
          i32.const 40
          i32.add
          set_local 8
          get_local 1
          get_local 3
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=48
          set_local 12
          get_local 4
          set_local 3
          loop  ;; label = @4
            get_local 2
            get_local 16
            get_local 1
            get_local 3
            i32.const 56
            i32.mul
            i32.add
            f64.load
            f64.sub
            tee_local 13
            get_local 13
            f64.mul
            get_local 17
            get_local 1
            get_local 3
            i32.const 56
            i32.mul
            i32.add
            f64.load offset=8
            f64.sub
            tee_local 14
            get_local 14
            f64.mul
            f64.add
            get_local 18
            get_local 1
            get_local 3
            i32.const 56
            i32.mul
            i32.add
            f64.load offset=16
            f64.sub
            tee_local 15
            get_local 15
            f64.mul
            f64.add
            f64.sqrt
            tee_local 11
            get_local 11
            get_local 11
            f64.mul
            f64.mul
            f64.div
            set_local 10
            get_local 6
            get_local 6
            f64.load
            get_local 13
            get_local 1
            get_local 3
            i32.const 56
            i32.mul
            i32.add
            f64.load offset=48
            tee_local 11
            f64.mul
            get_local 10
            f64.mul
            f64.sub
            f64.store
            get_local 7
            get_local 7
            f64.load
            get_local 10
            get_local 14
            get_local 11
            f64.mul
            f64.mul
            f64.sub
            f64.store
            get_local 8
            get_local 8
            f64.load
            get_local 10
            get_local 15
            get_local 11
            f64.mul
            f64.mul
            f64.sub
            f64.store
            get_local 1
            get_local 3
            i32.const 56
            i32.mul
            i32.add
            i32.const 24
            i32.add
            tee_local 5
            get_local 5
            f64.load
            get_local 10
            get_local 13
            get_local 12
            f64.mul
            f64.mul
            f64.add
            f64.store
            get_local 1
            get_local 3
            i32.const 56
            i32.mul
            i32.add
            i32.const 32
            i32.add
            tee_local 5
            get_local 5
            f64.load
            get_local 10
            get_local 14
            get_local 12
            f64.mul
            f64.mul
            f64.add
            f64.store
            get_local 1
            get_local 3
            i32.const 56
            i32.mul
            i32.add
            i32.const 40
            i32.add
            tee_local 5
            get_local 5
            f64.load
            get_local 10
            get_local 15
            get_local 12
            f64.mul
            f64.mul
            f64.add
            f64.store
            get_local 3
            i32.const 1
            i32.add
            tee_local 3
            get_local 0
            i32.ne
            br_if 0 (;@4;)
          end
        end
        get_local 4
        get_local 0
        i32.ne
        if  ;; label = @3
          get_local 4
          set_local 3
          br 1 (;@2;)
        end
      end
      get_local 9
      if  ;; label = @2
        i32.const 0
        set_local 4
      else
        return
      end
      loop  ;; label = @2
        get_local 1
        get_local 4
        i32.const 56
        i32.mul
        i32.add
        tee_local 3
        get_local 3
        f64.load
        get_local 1
        get_local 4
        i32.const 56
        i32.mul
        i32.add
        f64.load offset=24
        get_local 2
        f64.mul
        f64.add
        f64.store
        get_local 1
        get_local 4
        i32.const 56
        i32.mul
        i32.add
        i32.const 8
        i32.add
        tee_local 3
        get_local 3
        f64.load
        get_local 1
        get_local 4
        i32.const 56
        i32.mul
        i32.add
        f64.load offset=32
        get_local 2
        f64.mul
        f64.add
        f64.store
        get_local 1
        get_local 4
        i32.const 56
        i32.mul
        i32.add
        i32.const 16
        i32.add
        tee_local 3
        get_local 3
        f64.load
        get_local 1
        get_local 4
        i32.const 56
        i32.mul
        i32.add
        f64.load offset=40
        get_local 2
        f64.mul
        f64.add
        f64.store
        get_local 4
        i32.const 1
        i32.add
        tee_local 4
        get_local 0
        i32.ne
        br_if 0 (;@2;)
      end
    end)
  (func (;1;) (type 1) (param i32 i32) (result f64)
    (local i32 i32 f64 f64 f64 f64 f64)
    block f64  ;; label = @1
      get_local 0
      i32.const 0
      i32.gt_s
      if  ;; label = @2
        i32.const 0
        set_local 2
        f64.const 0x0p+0 (;=0;)
        set_local 4
      else
        f64.const 0x0p+0 (;=0;)
        return
      end
      loop  ;; label = @2
        get_local 4
        get_local 1
        get_local 2
        i32.const 56
        i32.mul
        i32.add
        f64.load offset=48
        tee_local 5
        f64.const 0x1p-1 (;=0.5;)
        f64.mul
        get_local 1
        get_local 2
        i32.const 56
        i32.mul
        i32.add
        f64.load offset=24
        tee_local 4
        get_local 4
        f64.mul
        get_local 1
        get_local 2
        i32.const 56
        i32.mul
        i32.add
        f64.load offset=32
        tee_local 4
        get_local 4
        f64.mul
        f64.add
        get_local 1
        get_local 2
        i32.const 56
        i32.mul
        i32.add
        f64.load offset=40
        tee_local 4
        get_local 4
        f64.mul
        f64.add
        f64.mul
        f64.add
        set_local 4
        get_local 2
        i32.const 1
        i32.add
        tee_local 3
        get_local 0
        i32.lt_s
        if  ;; label = @3
          get_local 1
          get_local 2
          i32.const 56
          i32.mul
          i32.add
          f64.load
          set_local 6
          get_local 1
          get_local 2
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=8
          set_local 7
          get_local 1
          get_local 2
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=16
          set_local 8
          get_local 3
          set_local 2
          loop  ;; label = @4
            get_local 4
            get_local 5
            get_local 1
            get_local 2
            i32.const 56
            i32.mul
            i32.add
            f64.load offset=48
            f64.mul
            get_local 6
            get_local 1
            get_local 2
            i32.const 56
            i32.mul
            i32.add
            f64.load
            f64.sub
            tee_local 4
            get_local 4
            f64.mul
            get_local 7
            get_local 1
            get_local 2
            i32.const 56
            i32.mul
            i32.add
            f64.load offset=8
            f64.sub
            tee_local 4
            get_local 4
            f64.mul
            f64.add
            get_local 8
            get_local 1
            get_local 2
            i32.const 56
            i32.mul
            i32.add
            f64.load offset=16
            f64.sub
            tee_local 4
            get_local 4
            f64.mul
            f64.add
            f64.sqrt
            f64.div
            f64.sub
            set_local 4
            get_local 2
            i32.const 1
            i32.add
            tee_local 2
            get_local 0
            i32.ne
            br_if 0 (;@4;)
          end
        end
        get_local 3
        get_local 0
        i32.ne
        if  ;; label = @3
          get_local 3
          set_local 2
          br 1 (;@2;)
        end
      end
      get_local 4
    end)
  (func (;2;) (type 2) (param i32 i32)
    (local i32 f64 f64 f64 f64)
    block  ;; label = @1
      get_local 0
      i32.const 0
      i32.gt_s
      if  ;; label = @2
        f64.const 0x0p+0 (;=0;)
        set_local 3
        f64.const 0x0p+0 (;=0;)
        set_local 4
        f64.const 0x0p+0 (;=0;)
        set_local 5
        i32.const 0
        set_local 2
        loop  ;; label = @3
          get_local 5
          get_local 1
          get_local 2
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=24
          get_local 1
          get_local 2
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=48
          tee_local 6
          f64.mul
          f64.add
          set_local 5
          get_local 4
          get_local 6
          get_local 1
          get_local 2
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=32
          f64.mul
          f64.add
          set_local 4
          get_local 3
          get_local 6
          get_local 1
          get_local 2
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=40
          f64.mul
          f64.add
          set_local 3
          get_local 2
          i32.const 1
          i32.add
          tee_local 2
          get_local 0
          i32.ne
          br_if 0 (;@3;)
        end
      else
        f64.const 0x0p+0 (;=0;)
        set_local 3
        f64.const 0x0p+0 (;=0;)
        set_local 4
        f64.const 0x0p+0 (;=0;)
        set_local 5
      end
      get_local 1
      get_local 5
      f64.neg
      f64.const 0x1.3bd3cc9be45dep+5 (;=39.4784;)
      f64.div
      f64.store offset=24
      get_local 1
      get_local 4
      f64.neg
      f64.const 0x1.3bd3cc9be45dep+5 (;=39.4784;)
      f64.div
      f64.store offset=32
      get_local 1
      get_local 3
      f64.neg
      f64.const 0x1.3bd3cc9be45dep+5 (;=39.4784;)
      f64.div
      f64.store offset=40
    end)
  (func (;3;) (type 3) (result f64)
    (local i32 i32 f64 f64 f64 f64 f64 f64 f64)
    block f64  ;; label = @1
      get_global 0
      f64.load offset=48
      tee_local 6
      get_global 0
      f64.load offset=32
      f64.mul
      f64.const 0x0p+0 (;=0;)
      f64.add
      get_global 0
      f64.load offset=104
      tee_local 7
      get_global 0
      f64.load offset=88
      f64.mul
      f64.add
      get_global 0
      f64.load offset=160
      tee_local 8
      get_global 0
      f64.load offset=144
      f64.mul
      f64.add
      get_global 0
      f64.load offset=216
      tee_local 3
      get_global 0
      f64.load offset=200
      f64.mul
      f64.add
      get_global 0
      f64.load offset=272
      tee_local 4
      get_global 0
      f64.load offset=256
      f64.mul
      f64.add
      set_local 5
      get_local 6
      get_global 0
      f64.load offset=40
      f64.mul
      f64.const 0x0p+0 (;=0;)
      f64.add
      get_local 7
      get_global 0
      f64.load offset=96
      f64.mul
      f64.add
      get_local 8
      get_global 0
      f64.load offset=152
      f64.mul
      f64.add
      get_local 3
      get_global 0
      f64.load offset=208
      f64.mul
      f64.add
      get_local 4
      get_global 0
      f64.load offset=264
      f64.mul
      f64.add
      set_local 2
      get_global 0
      get_global 0
      f64.load offset=24
      get_local 6
      f64.mul
      f64.const 0x0p+0 (;=0;)
      f64.add
      get_global 0
      f64.load offset=80
      get_local 7
      f64.mul
      f64.add
      get_global 0
      f64.load offset=136
      get_local 8
      f64.mul
      f64.add
      get_global 0
      f64.load offset=192
      get_local 3
      f64.mul
      f64.add
      get_global 0
      f64.load offset=248
      get_local 4
      f64.mul
      f64.add
      f64.neg
      f64.const 0x1.3bd3cc9be45dep+5 (;=39.4784;)
      f64.div
      tee_local 3
      f64.store offset=24
      get_global 0
      get_local 5
      f64.neg
      f64.const 0x1.3bd3cc9be45dep+5 (;=39.4784;)
      f64.div
      tee_local 4
      f64.store offset=32
      get_global 0
      get_local 2
      f64.neg
      f64.const 0x1.3bd3cc9be45dep+5 (;=39.4784;)
      f64.div
      tee_local 5
      f64.store offset=40
      i32.const 0
      set_local 0
      f64.const 0x0p+0 (;=0;)
      set_local 2
      loop  ;; label = @2
        get_local 2
        get_local 6
        f64.const 0x1p-1 (;=0.5;)
        f64.mul
        get_local 3
        get_local 3
        f64.mul
        get_local 4
        get_local 4
        f64.mul
        f64.add
        get_local 5
        get_local 5
        f64.mul
        f64.add
        f64.mul
        f64.add
        set_local 2
        get_local 0
        i32.const 1
        i32.add
        tee_local 1
        i32.const 5
        i32.lt_s
        if  ;; label = @3
          get_global 0
          get_local 0
          i32.const 56
          i32.mul
          i32.add
          f64.load
          set_local 3
          get_global 0
          get_local 0
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=8
          set_local 4
          get_global 0
          get_local 0
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=16
          set_local 5
          get_local 1
          set_local 0
          loop  ;; label = @4
            get_local 2
            get_local 6
            get_global 0
            get_local 0
            i32.const 56
            i32.mul
            i32.add
            f64.load offset=48
            f64.mul
            get_local 3
            get_global 0
            get_local 0
            i32.const 56
            i32.mul
            i32.add
            f64.load
            f64.sub
            tee_local 2
            get_local 2
            f64.mul
            get_local 4
            get_global 0
            get_local 0
            i32.const 56
            i32.mul
            i32.add
            f64.load offset=8
            f64.sub
            tee_local 2
            get_local 2
            f64.mul
            f64.add
            get_local 5
            get_global 0
            get_local 0
            i32.const 56
            i32.mul
            i32.add
            f64.load offset=16
            f64.sub
            tee_local 2
            get_local 2
            f64.mul
            f64.add
            f64.sqrt
            f64.div
            f64.sub
            set_local 2
            get_local 0
            i32.const 1
            i32.add
            tee_local 0
            i32.const 5
            i32.ne
            br_if 0 (;@4;)
          end
        end
        get_local 1
        i32.const 5
        i32.ne
        if  ;; label = @3
          get_local 1
          set_local 0
          get_global 0
          get_local 1
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=48
          set_local 6
          get_global 0
          get_local 1
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=24
          set_local 3
          get_global 0
          get_local 1
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=32
          set_local 4
          get_global 0
          get_local 1
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=40
          set_local 5
          br 1 (;@2;)
        end
      end
      get_local 2
    end)
  (func (;4;) (type 4) (param i32) (result f64)
    (local i32 i32 f64 f64 f64 f64 f64)
    block f64  ;; label = @1
      get_local 0
      i32.const 1
      i32.lt_s
      if  ;; label = @2
        i32.const 0
        set_local 0
        f64.const 0x0p+0 (;=0;)
        set_local 3
      else
        i32.const 1
        set_local 1
        loop  ;; label = @3
          i32.const 5
          get_global 0
          f64.const 0x1.47ae147ae147bp-7 (;=0.01;)
          call 0
          get_local 1
          i32.const 1
          i32.add
          set_local 2
          get_local 1
          get_local 0
          i32.eq
          if  ;; label = @4
            i32.const 0
            set_local 0
            f64.const 0x0p+0 (;=0;)
            set_local 3
          else
            get_local 2
            set_local 1
            br 1 (;@3;)
          end
        end
      end
      loop  ;; label = @2
        get_local 3
        get_global 0
        get_local 0
        i32.const 56
        i32.mul
        i32.add
        f64.load offset=48
        tee_local 4
        f64.const 0x1p-1 (;=0.5;)
        f64.mul
        get_global 0
        get_local 0
        i32.const 56
        i32.mul
        i32.add
        f64.load offset=24
        tee_local 3
        get_local 3
        f64.mul
        get_global 0
        get_local 0
        i32.const 56
        i32.mul
        i32.add
        f64.load offset=32
        tee_local 3
        get_local 3
        f64.mul
        f64.add
        get_global 0
        get_local 0
        i32.const 56
        i32.mul
        i32.add
        f64.load offset=40
        tee_local 3
        get_local 3
        f64.mul
        f64.add
        f64.mul
        f64.add
        set_local 3
        get_local 0
        i32.const 1
        i32.add
        tee_local 1
        i32.const 5
        i32.lt_s
        if  ;; label = @3
          get_global 0
          get_local 0
          i32.const 56
          i32.mul
          i32.add
          f64.load
          set_local 5
          get_global 0
          get_local 0
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=8
          set_local 6
          get_global 0
          get_local 0
          i32.const 56
          i32.mul
          i32.add
          f64.load offset=16
          set_local 7
          get_local 1
          set_local 0
          loop  ;; label = @4
            get_local 3
            get_local 4
            get_global 0
            get_local 0
            i32.const 56
            i32.mul
            i32.add
            f64.load offset=48
            f64.mul
            get_local 5
            get_global 0
            get_local 0
            i32.const 56
            i32.mul
            i32.add
            f64.load
            f64.sub
            tee_local 3
            get_local 3
            f64.mul
            get_local 6
            get_global 0
            get_local 0
            i32.const 56
            i32.mul
            i32.add
            f64.load offset=8
            f64.sub
            tee_local 3
            get_local 3
            f64.mul
            f64.add
            get_local 7
            get_global 0
            get_local 0
            i32.const 56
            i32.mul
            i32.add
            f64.load offset=16
            f64.sub
            tee_local 3
            get_local 3
            f64.mul
            f64.add
            f64.sqrt
            f64.div
            f64.sub
            set_local 3
            get_local 0
            i32.const 1
            i32.add
            tee_local 0
            i32.const 5
            i32.ne
            br_if 0 (;@4;)
          end
        end
        get_local 1
        i32.const 5
        i32.ne
        if  ;; label = @3
          get_local 1
          set_local 0
          br 1 (;@2;)
        end
      end
      get_local 3
    end)
  (func (;5;) (type 5)
    nop)
  (func (;6;) (type 5)
    block  ;; label = @1
      get_global 0
      i32.const 288
      i32.add
      set_global 2
      get_global 2
      i32.const 5242880
      i32.add
      set_global 3
      call 5
    end)
  (global (;2;) (mut i32) (i32.const 0))
  (global (;3;) (mut i32) (i32.const 0))
  (global (;4;) i32 (i32.const 0))
  (export "_run" (func 4))
  (export "__post_instantiate" (func 6))
  (export "runPostSets" (func 5))
  (export "_energy" (func 1))
  (export "_start" (func 3))
  (export "_offset_momentum" (func 2))
  (export "_advance" (func 0))
  (export "_bodies" (global 4))
  (data (get_global 0) "\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\00\deE\be\c9<\bdC@,\d9<4\a0]\13@|\db\1f\c0\ab\90\f2\bf\f0\eb%l\f9\86\ba\bf\bc\cc\93\9b\06g\e3?\9b\94}\f5\f2~\06@\15\07Z\9a\d7\d2\99\bf\d83\ab\d9\95L\a3?g\ca2\c3\cd\af @\b0\01\de1\cb\7f\10@|F\eb\e1S\d3\d9\bfB\94\87\b8!,\f0\bf\13\8f\1f\bf\e95\fd?\b4#\11_H<\81?7\c6\07\0dI\1d\87?\cf\d9\a7\ce\ea\c9)@~f&\d6\e88.\c0\a0}%\beW\95\cc\bf\ef\1b\91\a9\1cS\f1?\c5\bbT>\7f\cc\eb?|>\f2\fak/\86\bf\b3\1e\f4\9c\d2=\5c?*W\05\a9g\c2.@ \a2\c83X\eb9\c0@\e5\ab\93\f3\f1\c6?J\bcY\16\b6T\ef?\a3\fb\c41\c6\07\e3?\f6evX\88\cb\a1\bf\ac\99\17S\f3\a8`?"))