; clang -emit-llvm -S -o foo.ll foo.cpp

; ModuleID = 'foo.cpp'
source_filename = "foo.cpp"
target datalayout = "e-m:e-i64:64-f80:128-n8:16:32:64-S128"
target triple = "x86_64-unknown-linux-gnu"

%struct._IO_FILE = type { i32, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, i8*, %struct._IO_marker*, %struct._IO_FILE*, i32, i32, i64, i16, i8, [1 x i8], i8*, i64, i8*, i8*, i8*, i8*, i64, i32, [20 x i8] }
%struct._IO_marker = type { %struct._IO_marker*, %struct._IO_FILE*, i32 }
%struct.node = type { %struct.node*, i32 }
%class.number = type { i32, i32 }
%struct.variant = type { i32, %union.anon }
%union.anon = type { i8* }

@.str = private unnamed_addr constant [5 x i8] c"zero\00", align 1
@.str.1 = private unnamed_addr constant [4 x i8] c"one\00", align 1
@.str.2 = private unnamed_addr constant [4 x i8] c"two\00", align 1
@.str.3 = private unnamed_addr constant [6 x i8] c"three\00", align 1
@.str.4 = private unnamed_addr constant [5 x i8] c"four\00", align 1
@.str.5 = private unnamed_addr constant [5 x i8] c"five\00", align 1
@.str.6 = private unnamed_addr constant [4 x i8] c"six\00", align 1
@.str.7 = private unnamed_addr constant [5 x i8] c"many\00", align 1
@.str.8 = private unnamed_addr constant [4 x i8] c"ett\00", align 1
@.str.9 = private unnamed_addr constant [4 x i8] c"tio\00", align 1
@.str.10 = private unnamed_addr constant [7 x i8] c"hundra\00", align 1
@.str.11 = private unnamed_addr constant [6 x i8] c"tusen\00", align 1
@.str.12 = private unnamed_addr constant [9 x i8] c"miljoner\00", align 1
@.str.13 = private unnamed_addr constant [10 x i8] c"miljarder\00", align 1
@.str.14 = private unnamed_addr constant [5 x i8] c"otal\00", align 1
@.str.15 = private unnamed_addr constant [3 x i8] c"%d\00", align 1
@.str.16 = private unnamed_addr constant [3 x i8] c"%s\00", align 1
@.str.17 = private unnamed_addr constant [6 x i8] c"fn %p\00", align 1
@stderr = external global %struct._IO_FILE*, align 8
@.str.18 = private unnamed_addr constant [20 x i8] c"Not supported yet.\0A\00", align 1

; Function Attrs: noinline nounwind uwtable
define i8* @_Z14switch_compacti(i32) #0 {
  %2 = alloca i8*, align 8
  %3 = alloca i32, align 4
  store i32 %0, i32* %3, align 4
  %4 = load i32, i32* %3, align 4
  switch i32 %4, label %12 [
    i32 0, label %5
    i32 1, label %6
    i32 2, label %7
    i32 3, label %8
    i32 4, label %9
    i32 5, label %10
    i32 6, label %11
  ]

; <label>:5:                                      ; preds = %1
  store i8* getelementptr inbounds ([5 x i8], [5 x i8]* @.str, i32 0, i32 0), i8** %2, align 8
  br label %13

; <label>:6:                                      ; preds = %1
  store i8* getelementptr inbounds ([4 x i8], [4 x i8]* @.str.1, i32 0, i32 0), i8** %2, align 8
  br label %13

; <label>:7:                                      ; preds = %1
  store i8* getelementptr inbounds ([4 x i8], [4 x i8]* @.str.2, i32 0, i32 0), i8** %2, align 8
  br label %13

; <label>:8:                                      ; preds = %1
  store i8* getelementptr inbounds ([6 x i8], [6 x i8]* @.str.3, i32 0, i32 0), i8** %2, align 8
  br label %13

; <label>:9:                                      ; preds = %1
  store i8* getelementptr inbounds ([5 x i8], [5 x i8]* @.str.4, i32 0, i32 0), i8** %2, align 8
  br label %13

; <label>:10:                                     ; preds = %1
  store i8* getelementptr inbounds ([5 x i8], [5 x i8]* @.str.5, i32 0, i32 0), i8** %2, align 8
  br label %13

; <label>:11:                                     ; preds = %1
  store i8* getelementptr inbounds ([4 x i8], [4 x i8]* @.str.6, i32 0, i32 0), i8** %2, align 8
  br label %13

; <label>:12:                                     ; preds = %1
  store i8* getelementptr inbounds ([5 x i8], [5 x i8]* @.str.7, i32 0, i32 0), i8** %2, align 8
  br label %13

; <label>:13:                                     ; preds = %12, %11, %10, %9, %8, %7, %6, %5
  %14 = load i8*, i8** %2, align 8
  ret i8* %14
}

; Function Attrs: noinline nounwind uwtable
define i8* @_Z13switch_sparsei(i32) #0 {
  %2 = alloca i8*, align 8
  %3 = alloca i32, align 4
  store i32 %0, i32* %3, align 4
  %4 = load i32, i32* %3, align 4
  switch i32 %4, label %11 [
    i32 1, label %5
    i32 10, label %6
    i32 100, label %7
    i32 1000, label %8
    i32 1000000, label %9
    i32 1000000000, label %10
  ]

; <label>:5:                                      ; preds = %1
  store i8* getelementptr inbounds ([4 x i8], [4 x i8]* @.str.8, i32 0, i32 0), i8** %2, align 8
  br label %12

; <label>:6:                                      ; preds = %1
  store i8* getelementptr inbounds ([4 x i8], [4 x i8]* @.str.9, i32 0, i32 0), i8** %2, align 8
  br label %12

; <label>:7:                                      ; preds = %1
  store i8* getelementptr inbounds ([7 x i8], [7 x i8]* @.str.10, i32 0, i32 0), i8** %2, align 8
  br label %12

; <label>:8:                                      ; preds = %1
  store i8* getelementptr inbounds ([6 x i8], [6 x i8]* @.str.11, i32 0, i32 0), i8** %2, align 8
  br label %12

; <label>:9:                                      ; preds = %1
  store i8* getelementptr inbounds ([9 x i8], [9 x i8]* @.str.12, i32 0, i32 0), i8** %2, align 8
  br label %12

; <label>:10:                                     ; preds = %1
  store i8* getelementptr inbounds ([10 x i8], [10 x i8]* @.str.13, i32 0, i32 0), i8** %2, align 8
  br label %12

; <label>:11:                                     ; preds = %1
  store i8* getelementptr inbounds ([5 x i8], [5 x i8]* @.str.14, i32 0, i32 0), i8** %2, align 8
  br label %12

; <label>:12:                                     ; preds = %11, %10, %9, %8, %7, %6, %5
  %13 = load i8*, i8** %2, align 8
  ret i8* %13
}

; Function Attrs: noinline nounwind uwtable
define %struct.node* @_Z9find_nodeP4nodei(%struct.node*, i32) #0 {
  %3 = alloca %struct.node*, align 8
  %4 = alloca i32, align 4
  store %struct.node* %0, %struct.node** %3, align 8
  store i32 %1, i32* %4, align 4
  br label %5

; <label>:5:                                      ; preds = %16, %2
  %6 = load %struct.node*, %struct.node** %3, align 8
  %7 = icmp ne %struct.node* %6, null
  br i1 %7, label %8, label %14

; <label>:8:                                      ; preds = %5
  %9 = load %struct.node*, %struct.node** %3, align 8
  %10 = getelementptr inbounds %struct.node, %struct.node* %9, i32 0, i32 1
  %11 = load i32, i32* %10, align 8
  %12 = load i32, i32* %4, align 4
  %13 = icmp ne i32 %11, %12
  br label %14

; <label>:14:                                     ; preds = %8, %5
  %15 = phi i1 [ false, %5 ], [ %13, %8 ]
  br i1 %15, label %16, label %20

; <label>:16:                                     ; preds = %14
  %17 = load %struct.node*, %struct.node** %3, align 8
  %18 = getelementptr inbounds %struct.node, %struct.node* %17, i32 0, i32 0
  %19 = load %struct.node*, %struct.node** %18, align 8
  store %struct.node* %19, %struct.node** %3, align 8
  br label %5

; <label>:20:                                     ; preds = %14
  %21 = load %struct.node*, %struct.node** %3, align 8
  ret %struct.node* %21
}

; Function Attrs: noinline uwtable
define i64 @_Z20apply_member_pointerM6numberFS_vES_(i64, i64, i64) #1 {
  %4 = alloca %class.number, align 4
  %5 = alloca { i64, i64 }, align 8
  %6 = alloca %class.number, align 4
  %7 = alloca { i64, i64 }, align 8
  %8 = getelementptr inbounds { i64, i64 }, { i64, i64 }* %5, i32 0, i32 0
  store i64 %0, i64* %8, align 8
  %9 = getelementptr inbounds { i64, i64 }, { i64, i64 }* %5, i32 0, i32 1
  store i64 %1, i64* %9, align 8
  %10 = load { i64, i64 }, { i64, i64 }* %5, align 8
  %11 = bitcast %class.number* %6 to i64*
  store i64 %2, i64* %11, align 4
  store { i64, i64 } %10, { i64, i64 }* %7, align 8
  %12 = load { i64, i64 }, { i64, i64 }* %7, align 8
  %13 = extractvalue { i64, i64 } %12, 1
  %14 = bitcast %class.number* %6 to i8*
  %15 = getelementptr inbounds i8, i8* %14, i64 %13
  %16 = bitcast i8* %15 to %class.number*
  %17 = extractvalue { i64, i64 } %12, 0
  %18 = and i64 %17, 1
  %19 = icmp ne i64 %18, 0
  br i1 %19, label %20, label %27

; <label>:20:                                     ; preds = %3
  %21 = bitcast %class.number* %16 to i8**
  %22 = load i8*, i8** %21, align 8
  %23 = sub i64 %17, 1
  %24 = getelementptr i8, i8* %22, i64 %23
  %25 = bitcast i8* %24 to i64 (%class.number*)**
  %26 = load i64 (%class.number*)*, i64 (%class.number*)** %25, align 8
  br label %29

; <label>:27:                                     ; preds = %3
  %28 = inttoptr i64 %17 to i64 (%class.number*)*
  br label %29

; <label>:29:                                     ; preds = %27, %20
  %30 = phi i64 (%class.number*)* [ %26, %20 ], [ %28, %27 ]
  %31 = call i64 %30(%class.number* %16)
  %32 = bitcast %class.number* %4 to i64*
  store i64 %31, i64* %32, align 4
  %33 = bitcast %class.number* %4 to i64*
  %34 = load i64, i64* %33, align 4
  ret i64 %34
}

; Function Attrs: noinline nounwind uwtable
define void @_Z19update_member_fieldM6numberiS_(i64, i64) #0 {
  %3 = alloca %class.number, align 4
  %4 = alloca i64, align 8
  %5 = bitcast %class.number* %3 to i64*
  store i64 %1, i64* %5, align 4
  store i64 %0, i64* %4, align 8
  %6 = load i64, i64* %4, align 8
  %7 = bitcast %class.number* %3 to i8*
  %8 = getelementptr inbounds i8, i8* %7, i64 %6
  %9 = bitcast i8* %8 to i32*
  %10 = load i32, i32* %9, align 4
  %11 = add nsw i32 %10, 1
  store i32 %11, i32* %9, align 4
  ret void
}

; Function Attrs: noinline uwtable
define dereferenceable(16) %struct.variant* @_Z5printRK7variant(%struct.variant* dereferenceable(16)) #1 {
  %2 = alloca %struct.variant*, align 8
  store %struct.variant* %0, %struct.variant** %2, align 8
  %3 = load %struct.variant*, %struct.variant** %2, align 8
  %4 = getelementptr inbounds %struct.variant, %struct.variant* %3, i32 0, i32 0
  %5 = load i32, i32* %4, align 8
  switch i32 %5, label %24 [
    i32 1, label %6
    i32 2, label %12
    i32 3, label %18
  ]

; <label>:6:                                      ; preds = %1
  %7 = load %struct.variant*, %struct.variant** %2, align 8
  %8 = getelementptr inbounds %struct.variant, %struct.variant* %7, i32 0, i32 1
  %9 = bitcast %union.anon* %8 to i32*
  %10 = load i32, i32* %9, align 8
  %11 = call i32 (i8*, ...) @printf(i8* getelementptr inbounds ([3 x i8], [3 x i8]* @.str.15, i32 0, i32 0), i32 %10)
  br label %24

; <label>:12:                                     ; preds = %1
  %13 = load %struct.variant*, %struct.variant** %2, align 8
  %14 = getelementptr inbounds %struct.variant, %struct.variant* %13, i32 0, i32 1
  %15 = bitcast %union.anon* %14 to i8**
  %16 = load i8*, i8** %15, align 8
  %17 = call i32 (i8*, ...) @printf(i8* getelementptr inbounds ([3 x i8], [3 x i8]* @.str.16, i32 0, i32 0), i8* %16)
  br label %24

; <label>:18:                                     ; preds = %1
  %19 = load %struct.variant*, %struct.variant** %2, align 8
  %20 = getelementptr inbounds %struct.variant, %struct.variant* %19, i32 0, i32 1
  %21 = bitcast %union.anon* %20 to void ()**
  %22 = load void ()*, void ()** %21, align 8
  %23 = call i32 (i8*, ...) @printf(i8* getelementptr inbounds ([6 x i8], [6 x i8]* @.str.17, i32 0, i32 0), void ()* %22)
  br label %24

; <label>:24:                                     ; preds = %1, %18, %12, %6
  %25 = load %struct._IO_FILE*, %struct._IO_FILE** @stderr, align 8
  %26 = call i32 (%struct._IO_FILE*, i8*, ...) @fprintf(%struct._IO_FILE* %25, i8* getelementptr inbounds ([20 x i8], [20 x i8]* @.str.18, i32 0, i32 0))
  %27 = load %struct.variant*, %struct.variant** %2, align 8
  ret %struct.variant* %27
}

declare i32 @printf(i8*, ...) #2

declare i32 @fprintf(%struct._IO_FILE*, i8*, ...) #2

attributes #0 = { noinline nounwind uwtable "correctly-rounded-divide-sqrt-fp-math"="false" "disable-tail-calls"="false" "less-precise-fpmad"="false" "no-frame-pointer-elim"="true" "no-frame-pointer-elim-non-leaf" "no-infs-fp-math"="false" "no-jump-tables"="false" "no-nans-fp-math"="false" "no-signed-zeros-fp-math"="false" "no-trapping-math"="false" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+fxsr,+mmx,+sse,+sse2,+x87" "unsafe-fp-math"="false" "use-soft-float"="false" }
attributes #1 = { noinline uwtable "correctly-rounded-divide-sqrt-fp-math"="false" "disable-tail-calls"="false" "less-precise-fpmad"="false" "no-frame-pointer-elim"="true" "no-frame-pointer-elim-non-leaf" "no-infs-fp-math"="false" "no-jump-tables"="false" "no-nans-fp-math"="false" "no-signed-zeros-fp-math"="false" "no-trapping-math"="false" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+fxsr,+mmx,+sse,+sse2,+x87" "unsafe-fp-math"="false" "use-soft-float"="false" }
attributes #2 = { "correctly-rounded-divide-sqrt-fp-math"="false" "disable-tail-calls"="false" "less-precise-fpmad"="false" "no-frame-pointer-elim"="true" "no-frame-pointer-elim-non-leaf" "no-infs-fp-math"="false" "no-nans-fp-math"="false" "no-signed-zeros-fp-math"="false" "no-trapping-math"="false" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+fxsr,+mmx,+sse,+sse2,+x87" "unsafe-fp-math"="false" "use-soft-float"="false" }

!llvm.ident = !{!0}

!0 = !{!"clang version 4.0.0 (tags/RELEASE_400/final)"}
