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
	%2 = alloca i8*
	%3 = alloca i32
	store i32 %0, i32* %3
	%4 = load i32, i32* %3
	switch i32 %4, label %12 [
		i32 0, label %5
		i32 1, label %6
		i32 2, label %7
		i32 3, label %8
		i32 4, label %9
		i32 5, label %10
		i32 6, label %11
	]
; <label>:5
	store i8* getelementptr ([5 x i8], [5 x i8]* @.str, i32 0, i32 0), i8** %2
	br label %13
; <label>:6
	store i8* getelementptr ([4 x i8], [4 x i8]* @.str.1, i32 0, i32 0), i8** %2
	br label %13
; <label>:7
	store i8* getelementptr ([4 x i8], [4 x i8]* @.str.2, i32 0, i32 0), i8** %2
	br label %13
; <label>:8
	store i8* getelementptr ([6 x i8], [6 x i8]* @.str.3, i32 0, i32 0), i8** %2
	br label %13
; <label>:9
	store i8* getelementptr ([5 x i8], [5 x i8]* @.str.4, i32 0, i32 0), i8** %2
	br label %13
; <label>:10
	store i8* getelementptr ([5 x i8], [5 x i8]* @.str.5, i32 0, i32 0), i8** %2
	br label %13
; <label>:11
	store i8* getelementptr ([4 x i8], [4 x i8]* @.str.6, i32 0, i32 0), i8** %2
	br label %13
; <label>:12
	store i8* getelementptr ([5 x i8], [5 x i8]* @.str.7, i32 0, i32 0), i8** %2
	br label %13
; <label>:13
	%14 = load i8*, i8** %2
	ret i8* %14
}

define i8* @_Z13switch_sparsei(i32) {
; <label>:1
	%2 = alloca i8*
	%3 = alloca i32
	store i32 %0, i32* %3
	%4 = load i32, i32* %3
	switch i32 %4, label %11 [
		i32 1, label %5
		i32 10, label %6
		i32 100, label %7
		i32 1000, label %8
		i32 1000000, label %9
		i32 1000000000, label %10
	]
; <label>:5
	store i8* getelementptr ([4 x i8], [4 x i8]* @.str.8, i32 0, i32 0), i8** %2
	br label %12
; <label>:6
	store i8* getelementptr ([4 x i8], [4 x i8]* @.str.9, i32 0, i32 0), i8** %2
	br label %12
; <label>:7
	store i8* getelementptr ([7 x i8], [7 x i8]* @.str.10, i32 0, i32 0), i8** %2
	br label %12
; <label>:8
	store i8* getelementptr ([6 x i8], [6 x i8]* @.str.11, i32 0, i32 0), i8** %2
	br label %12
; <label>:9
	store i8* getelementptr ([9 x i8], [9 x i8]* @.str.12, i32 0, i32 0), i8** %2
	br label %12
; <label>:10
	store i8* getelementptr ([10 x i8], [10 x i8]* @.str.13, i32 0, i32 0), i8** %2
	br label %12
; <label>:11
	store i8* getelementptr ([5 x i8], [5 x i8]* @.str.14, i32 0, i32 0), i8** %2
	br label %12
; <label>:12
	%13 = load i8*, i8** %2
	ret i8* %13
}

define %struct.node* @_Z9find_nodeP4nodei(%struct.node*, i32) {
; <label>:2
	%3 = alloca %struct.node*
	%4 = alloca i32
	store %struct.node* %0, %struct.node** %3
	store i32 %1, i32* %4
	br label %5
; <label>:5
	%6 = load %struct.node*, %struct.node** %3
	%7 = icmp ne %struct.node* %6, null
	br i1 %7, label %8, label %14
; <label>:8
	%9 = load %struct.node*, %struct.node** %3
	%10 = getelementptr %struct.node, %struct.node* %9, i32 0, i32 1
	%11 = load i32, i32* %10
	%12 = load i32, i32* %4
	%13 = icmp ne i32 %11, %12
	br label %14
; <label>:14
	%15 = phi i1 [ false, %5 ], [ %13, %8 ]
	br i1 %15, label %16, label %20
; <label>:16
	%17 = load %struct.node*, %struct.node** %3
	%18 = getelementptr %struct.node, %struct.node* %17, i32 0, i32 0
	%19 = load %struct.node*, %struct.node** %18
	store %struct.node* %19, %struct.node** %3
	br label %5
; <label>:20
	%21 = load %struct.node*, %struct.node** %3
	ret %struct.node* %21
}

define i64 @_Z20apply_member_pointerM6numberFS_vES_(i64, i64, i64) {
; <label>:3
	%4 = alloca %class.number
	%5 = alloca { i64, i64 }
	%6 = alloca %class.number
	%7 = alloca { i64, i64 }
	%8 = getelementptr { i64, i64 }, { i64, i64 }* %5, i32 0, i32 0
	store i64 %0, i64* %8
	%9 = getelementptr { i64, i64 }, { i64, i64 }* %5, i32 0, i32 1
	store i64 %1, i64* %9
	%10 = load { i64, i64 }, { i64, i64 }* %5
	%11 = bitcast %class.number* %6 to i64*
	store i64 %2, i64* %11
	store { i64, i64 } %10, { i64, i64 }* %7
	%12 = load { i64, i64 }, { i64, i64 }* %7
	%13 = extractvalue { i64, i64 } %12, 1
	%14 = bitcast %class.number* %6 to i8*
	%15 = getelementptr i8, i8* %14, i64 %13
	%16 = bitcast i8* %15 to %class.number*
	%17 = extractvalue { i64, i64 } %12, 0
	%18 = and i64 %17, 1
	%19 = icmp ne i64 %18, 0
	br i1 %19, label %20, label %27
; <label>:20
	%21 = bitcast %class.number* %16 to i8**
	%22 = load i8*, i8** %21
	%23 = sub i64 %17, 1
	%24 = getelementptr i8, i8* %22, i64 %23
	%25 = bitcast i8* %24 to i64 (%class.number*)**
	%26 = load i64 (%class.number*)*, i64 (%class.number*)** %25
	br label %29
; <label>:27
	%28 = inttoptr i64 %17 to i64 (%class.number*)*
	br label %29
; <label>:29
	%30 = phi i64 (%class.number*)* [ %26, %20 ], [ %28, %27 ]
	%31 = call i64 %30(%class.number* %16)
	%32 = bitcast %class.number* %4 to i64*
	store i64 %31, i64* %32
	%33 = bitcast %class.number* %4 to i64*
	%34 = load i64, i64* %33
	ret i64 %34
}

define void @_Z19update_member_fieldM6numberiS_(i64, i64) {
; <label>:2
	%3 = alloca %class.number
	%4 = alloca i64
	%5 = bitcast %class.number* %3 to i64*
	store i64 %1, i64* %5
	store i64 %0, i64* %4
	%6 = load i64, i64* %4
	%7 = bitcast %class.number* %3 to i8*
	%8 = getelementptr i8, i8* %7, i64 %6
	%9 = bitcast i8* %8 to i32*
	%10 = load i32, i32* %9
	%11 = add i32 %10, 1
	store i32 %11, i32* %9
	ret void
}

define %struct.variant* @_Z5printRK7variant(%struct.variant*) {
; <label>:1
	%2 = alloca %struct.variant*
	store %struct.variant* %0, %struct.variant** %2
	%3 = load %struct.variant*, %struct.variant** %2
	%4 = getelementptr %struct.variant, %struct.variant* %3, i32 0, i32 0
	%5 = load i32, i32* %4
	switch i32 %5, label %24 [
		i32 1, label %6
		i32 2, label %12
		i32 3, label %18
	]
; <label>:6
	%7 = load %struct.variant*, %struct.variant** %2
	%8 = getelementptr %struct.variant, %struct.variant* %7, i32 0, i32 1
	%9 = bitcast %union.anon* %8 to i32*
	%10 = load i32, i32* %9
	%11 = call i32 (i8*, ...) @printf(i8* getelementptr ([3 x i8], [3 x i8]* @.str.15, i32 0, i32 0), i32 %10)
	br label %24
; <label>:12
	%13 = load %struct.variant*, %struct.variant** %2
	%14 = getelementptr %struct.variant, %struct.variant* %13, i32 0, i32 1
	%15 = bitcast %union.anon* %14 to i8**
	%16 = load i8*, i8** %15
	%17 = call i32 (i8*, ...) @printf(i8* getelementptr ([3 x i8], [3 x i8]* @.str.16, i32 0, i32 0), i8* %16)
	br label %24
; <label>:18
	%19 = load %struct.variant*, %struct.variant** %2
	%20 = getelementptr %struct.variant, %struct.variant* %19, i32 0, i32 1
	%21 = bitcast %union.anon* %20 to void ()**
	%22 = load void ()*, void ()** %21
	%23 = call i32 (i8*, ...) @printf(i8* getelementptr ([6 x i8], [6 x i8]* @.str.17, i32 0, i32 0), void ()* %22)
	br label %24
; <label>:24
	%25 = load %struct._IO_FILE*, %struct._IO_FILE** @stderr
	%26 = call i32 (%struct._IO_FILE*, i8*, ...) @fprintf(%struct._IO_FILE* %25, i8* getelementptr ([20 x i8], [20 x i8]* @.str.18, i32 0, i32 0))
	%27 = load %struct.variant*, %struct.variant** %2
	ret %struct.variant* %27
}

declare i32 @printf(i8*, ...)

declare i32 @fprintf(%struct._IO_FILE*, i8*, ...)
