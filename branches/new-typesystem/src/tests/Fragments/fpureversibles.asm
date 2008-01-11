;;; Tests FPU opcodes that are reversible, like fsub(r) and fdiv(r).

.i86

	fld	qword ptr [0x300]
	fld qword ptr [0x310]
	fsubp st(1), st(0)
	fstp qword ptr [0x400]
	
	fld qword ptr [0x320]
	fld qword ptr [0x330]
	fdivp st(1), st(0)
	fstp qword ptr [0x338]
	
	fld qword ptr [0x328]
	fld qword ptr [0x320]
	fdivp st(1)
	fstp qword ptr [0x408]
	
	fld qword ptr [0x320]
	fld qword ptr [0x330]
	fsubrp st(1),st(0)
	fstp qword ptr [0x338]
	
	fld qword ptr [0x320]
	fld qword ptr [0x330]
	fdivrp st(0),st(1)
	fstp qword ptr [0x340]
	
	ret

	
	