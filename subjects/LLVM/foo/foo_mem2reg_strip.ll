%struct._IO_FILE = type { i32, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, %struct._IO_marker*, %struct._IO_FILE*, i32, i32, i64, i16, i8, [1 x i8], i8*, i64, i8*, i8*, i8*, i8*, i64, i32, [20 x i8] }
%struct._IO_marker = type { %struct._IO_marker*, %struct._IO_FILE*, i32 }
%struct.node = type { %struct.node*, i32 }
%class.number = type { i32, i32 }
%struct.variant = type { i32, %union.anon }
%union.anon = type { i8* }

@.str = constant [5 x i8] c"zero\00"
@.str.1 = constant [4 x i8] c"one\00"
@.str.2 = constant [4 x i8] c"two\00"
@.str.3 = constant [6 x i8] c"three\00"
@.str.4 = constant [5 x i8] c"four\00"
@.str.5 = constant [5 x i8] c"five\00"
@.str.6 = constant [4 x i8] c"six\00"
@.str.7 = constant [5 x i8] c"many\00"
@.str.8 = constant [4 x i8] c"ett\00"
@.str.9 = constant [4 x i8] c"tio\00"
@.str.10 = constant [7 x i8] c"hundra\00"
@.str.11 = constant [6 x i8] c"tusen\00"
@.str.12 = constant [9 x i8] c"miljoner\00"
@.str.13 = constant [10 x i8] c"miljarder\00"
@.str.14 = constant [5 x i8] c"otal\00"
@.str.15 = constant [3 x i8] c"%d\00"
@.str.16 = constant [3 x i8] c"%s\00"
@.str.17 = constant [6 x i8] c"fn %p\00"
@stderr = external global %struct._IO_FILE*
@.str.18 = constant [20 x i8] c"Not supported yet.\0A\00"

define i8* @_Z14switch_compacti(i32) {
; <label>:1
	switch i32 %0, label %9 [
		i32 0, label %2
		i32 1, label %3
		i32 2, label %4
		i32 3, label %5
		i32 4, label %6
		i32 5, label %7
		i32 6, label %8
	]
; <label>:2
	br label %10
; <label>:3
	br label %10
; <label>:4
	br label %10
; <label>:5
	br label %10
; <label>:6
	br label %10
; <label>:7
	br label %10
; <label>:8
	br label %10
; <label>:9
	br label %10
; <label>:10
	%.0 = phi i8* [ getelementptr ([5 x i8], [5 x i8]* @.str.7, i32 0, i32 0), %9 ], [ getelementptr ([4 x i8], [4 x i8]* @.str.6, i32 0, i32 0), %8 ], [ getelementptr ([5 x i8], [5 x i8]* @.str.5, i32 0, i32 0), %7 ], [ getelementptr ([5 x i8], [5 x i8]* @.str.4, i32 0, i32 0), %6 ], [ getelementptr ([6 x i8], [6 x i8]* @.str.3, i32 0, i32 0), %5 ], [ getelementptr ([4 x i8], [4 x i8]* @.str.2, i32 0, i32 0), %4 ], [ getelementptr ([4 x i8], [4 x i8]* @.str.1, i32 0, i32 0), %3 ], [ getelementptr ([5 x i8], [5 x i8]* @.str, i32 0, i32 0), %2 ]
	ret i8* %.0
}

define i8* @_Z13switch_sparsei(i32) {
; <label>:1
	switch i32 %0, label %8 [
		i32 1, label %2
		i32 10, label %3
		i32 100, label %4
		i32 1000, label %5
		i32 1000000, label %6
		i32 1000000000, label %7
	]
; <label>:2
	br label %9
; <label>:3
	br label %9
; <label>:4
	br label %9
; <label>:5
	br label %9
; <label>:6
	br label %9
; <label>:7
	br label %9
; <label>:8
	br label %9
; <label>:9
	%.0 = phi i8* [ getelementptr ([5 x i8], [5 x i8]* @.str.14, i32 0, i32 0), %8 ], [ getelementptr ([10 x i8], [10 x i8]* @.str.13, i32 0, i32 0), %7 ], [ getelementptr ([9 x i8], [9 x i8]* @.str.12, i32 0, i32 0), %6 ], [ getelementptr ([6 x i8], [6 x i8]* @.str.11, i32 0, i32 0), %5 ], [ getelementptr ([7 x i8], [7 x i8]* @.str.10, i32 0, i32 0), %4 ], [ getelementptr ([4 x i8], [4 x i8]* @.str.9, i32 0, i32 0), %3 ], [ getelementptr ([4 x i8], [4 x i8]* @.str.8, i32 0, i32 0), %2 ]
	ret i8* %.0
}

define %struct.node* @_Z9find_nodeP4nodei(%struct.node*, i32) {
; <label>:2
	br label %3
; <label>:3
	%.0 = phi %struct.node* [ %0, %2 ], [ %13, %11 ]
	%4 = icmp ne %struct.node* %.0, null
	br i1 %4, label %5, label %9
; <label>:5
	%6 = getelementptr %struct.node, %struct.node* %.0, i32 0, i32 1
	%7 = load i32, i32* %6
	%8 = icmp ne i32 %7, %1
	br label %9
; <label>:9
	%10 = phi i1 [ false, %3 ], [ %8, %5 ]
	br i1 %10, label %11, label %14
; <label>:11
	%12 = getelementptr %struct.node, %struct.node* %.0, i32 0, i32 0
	%13 = load %struct.node*, %struct.node** %12
	br label %3
; <label>:14
	ret %struct.node* %.0
}

define i64 @_Z20apply_member_pointerM6numberFS_vES_(i64, i64, i64) {
; <label>:3
	%4 = alloca %class.number
	%5 = alloca { i64, i64 }
	%6 = alloca %class.number
	%7 = getelementptr { i64, i64 }, { i64, i64 }* %5, i32 0, i32 0
	store i64 %0, i64* %7
	%8 = getelementptr { i64, i64 }, { i64, i64 }* %5, i32 0, i32 1
	store i64 %1, i64* %8
	%9 = load { i64, i64 }, { i64, i64 }* %5
	%10 = bitcast %class.number* %6 to i64*
	store i64 %2, i64* %10
	%11 = extractvalue { i64, i64 } %9, 1
	%12 = bitcast %class.number* %6 to i8*
	%13 = getelementptr i8, i8* %12, i64 %11
	%14 = bitcast i8* %13 to %class.number*
	%15 = extractvalue { i64, i64 } %9, 0
	%16 = and i64 %15, 1
	%17 = icmp ne i64 %16, 0
	br i1 %17, label %18, label %25
; <label>:18
	%19 = bitcast %class.number* %14 to i8**
	%20 = load i8*, i8** %19
	%21 = sub i64 %15, 1
	%22 = getelementptr i8, i8* %20, i64 %21
	%23 = bitcast i8* %22 to i64 (%class.number*)**
	%24 = load i64 (%class.number*)*, i64 (%class.number*)** %23
	br label %27
; <label>:25
	%26 = inttoptr i64 %15 to i64 (%class.number*)*
	br label %27
; <label>:27
	%28 = phi i64 (%class.number*)* [ %24, %18 ], [ %26, %25 ]
	%29 = call i64 %28(%class.number* %14)
	%30 = bitcast %class.number* %4 to i64*
	store i64 %29, i64* %30
	%31 = bitcast %class.number* %4 to i64*
	%32 = load i64, i64* %31
	ret i64 %32
}

define void @_Z19update_member_fieldM6numberiS_(i64, i64) {
; <label>:2
	%3 = alloca %class.number
	%4 = bitcast %class.number* %3 to i64*
	store i64 %1, i64* %4
	%5 = bitcast %class.number* %3 to i8*
	%6 = getelementptr i8, i8* %5, i64 %0
	%7 = bitcast i8* %6 to i32*
	%8 = load i32, i32* %7
	%9 = add i32 %8, 1
	store i32 %9, i32* %7
	ret void
}

define %struct.variant* @_Z5printRK7variant(%struct.variant*) {
; <label>:1
	%2 = getelementptr %struct.variant, %struct.variant* %0, i32 0, i32 0
	%3 = load i32, i32* %2
	switch i32 %3, label %19 [
		i32 1, label %4
		i32 2, label %9
		i32 3, label %14
	]
; <label>:4
	%5 = getelementptr %struct.variant, %struct.variant* %0, i32 0, i32 1
	%6 = bitcast %union.anon* %5 to i32*
	%7 = load i32, i32* %6
	%8 = call i32 (i8*, ...) @printf(i8* getelementptr ([3 x i8], [3 x i8]* @.str.15, i32 0, i32 0), i32 %7)
	br label %19
; <label>:9
	%10 = getelementptr %struct.variant, %struct.variant* %0, i32 0, i32 1
	%11 = bitcast %union.anon* %10 to i8**
	%12 = load i8*, i8** %11
	%13 = call i32 (i8*, ...) @printf(i8* getelementptr ([3 x i8], [3 x i8]* @.str.16, i32 0, i32 0), i8* %12)
	br label %19
; <label>:14
	%15 = getelementptr %struct.variant, %struct.variant* %0, i32 0, i32 1
	%16 = bitcast %union.anon* %15 to void ()**
	%17 = load void ()*, void ()** %16
	%18 = call i32 (i8*, ...) @printf(i8* getelementptr ([6 x i8], [6 x i8]* @.str.17, i32 0, i32 0), void ()* %17)
	br label %19
; <label>:19
	%20 = load %struct._IO_FILE*, %struct._IO_FILE** @stderr
	%21 = call i32 (%struct._IO_FILE*, i8*, ...) @fprintf(%struct._IO_FILE* %20, i8* getelementptr ([20 x i8], [20 x i8]* @.str.18, i32 0, i32 0))
	ret %struct.variant* %0
}

declare i32 @printf(i8*, ...)

declare i32 @fprintf(%struct._IO_FILE*, i8*, ...)
