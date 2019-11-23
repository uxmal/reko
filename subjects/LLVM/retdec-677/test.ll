; ModuleID = 'test.c'
source_filename = "test.c"
%struct.A = type { [2 x i32] }

; Function Attrs: noinline nounwind optnone uwtable
define void @test_fn(%struct.A*) #0 {
  %2 = alloca %struct.A*, align 8
  store %struct.A* %0, %struct.A** %2, align 8
  %3 = load %struct.A*, %struct.A** %2, align 8
  %4 = getelementptr inbounds %struct.A, %struct.A* %3, i32 0, i32 0
  %5 = getelementptr inbounds [2 x i32], [2 x i32]* %4, i64 0, i64 0
  store i32 0, i32* %5, align 4
  ret void
}
