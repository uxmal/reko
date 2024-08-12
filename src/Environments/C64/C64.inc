[[reko::segment("0000", "low_memory", 0x0400, "rwx", "Low memory")]]

// 6510 CPU's data direction I/O port register; 0 = input, 1 = output

// Bit 0 - Direction of I/O on port at bit 0 of address $0001; default = 1
// Bit 1 - Direction of I/O on port at bit 1 of address $0001; default = 1
// Bit 2 - Direction of I/O on port at bit 2 of address $0001; default = 1
// Bit 3 - Direction of I/O on port at bit 3 of address $0001; default = 1
// Bit 4 - Direction of I/O on port at bit 4 of address $0001; default = 0
// Bit 5 - Direction of I/O on port at bit 5 of address $0001; default = 1
// Bit 6 - Direction of I/O on port at bit 6 of address $0001; unused, as bit 6 of address $0001 is undefined
// Bit 7 - Direction of I/O on port at bit 7 of address $0001; unused, as bit 7 of address $0001 is undefined
[[reko::address("0000")]] unsigned char IO_port_direction;

// 6510 CPU's on-chip port register
// Bit 0 - LORAM: Configures [[RAM]] or [[ROM]] at $A000-$BFFF (see [[Bankswitching|bankswitching]])
// Bit 1 - HIRAM: Configures [[RAM]] or [[ROM]] at $E000-$FFFF (see [[Bankswitching|bankswitching]])
// Bit 2 - CHAREN: Configures [[I/O]] or [[ROM]] at $D000-$DFFF (see [[Bankswitching|bankswitching]])
// Bit 3 - Cassette Data Output Line ([[Datasette]])
// Bit 4 - Cassette Switch Sense; 1 = Switch Closed
// Bit 5 - Cassette Motor Control; 0 = On, 1 = Off
// Bit 6 - Undefined
// Bit 7 - Undefined

// Yes, some C64 programs put executable code in the stack page...
// [[reko::segment("0100", "stack_page", 0x0100, "rwx", "Stack page")]]
// [[reko::segment("0200", "os_data",    0x0200, "rw")]]
[[reko::segment("0400", "screen",     0x0400, "rw")]]

[[reko::segment("A000", "basic_ROM",  0x2000, "rx")]]

[[reko::address("A435")]] void OMERR(); 
[[reko::address("A644")]] void SCRTCH();   // BASIC 'new'
[[reko::address("A659")]] void RUNC();   // reset BASIC program ctr
[[reko::address("AF7B")]] void TIME_FAC();   // Copy real-time clock to FAC
[[reko::address("B245")]] void BSERR();   // Bad subscript error
[[reko::address("B248")]] void FCERR();   // Function call error
[[reko::address("B658")]] void LSERR();   // Long string error
[[reko::address("B867")]] void FADD();   // FADD
[[reko::address("BA8C")]] void CONUPK();   // Unpack AY into FAC2
[[reko::address("BB0F")]] void FDIV();   // Divide AY / FAC1
[[reko::address("BBA2")]] void MOVFM();   // Upack AY into FAC1
[[reko::address("BBC7")]] void MOV2F();   //  Pack FAC1 into memory
[[reko::address("BBCA")]] void MOV1F();   // Move FAC1 ?
[[reko::address("BBD4")]] void MOVMF();   // Pack FAC1 into XY
[[reko::address("BC0C")]] void MOVAF();   // Move FAC into ARG
[[reko::address("BC1B")]] void ROUND();   // Round FAC
[[reko::address("BC2B")]] void SIGN();   // Sign of FAC into A

[[reko::segment("D000", "Vic_II", 0x0400, "rw")]]
[[reko::address("D020")]] unsigned char VICII_Border_Color;

[[reko::segment("E000", "kernal_ROM",  0x2000, "rx")]]
[[reko::address("FFE7")]] void CLALL(); // Clear file table
