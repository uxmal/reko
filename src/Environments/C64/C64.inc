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
[[reko::address("D000")]] unsigned char VICII_m0x;		// Low 8 bits of sprite X position
[[reko::address("D001")]] unsigned char VICII_m0y;		// Sprite Y position
[[reko::address("D002")]] unsigned char VICII_m1x;
[[reko::address("D003")]] unsigned char VICII_m1y;
[[reko::address("D004")]] unsigned char VICII_m2x;
[[reko::address("D005")]] unsigned char VICII_m2y;
[[reko::address("D006")]] unsigned char VICII_m3x;
[[reko::address("D007")]] unsigned char VICII_m3y;
[[reko::address("D008")]] unsigned char VICII_m4x;
[[reko::address("D009")]] unsigned char VICII_m4y;
[[reko::address("D00A")]] unsigned char VICII_m5x;
[[reko::address("D00B")]] unsigned char VICII_m5y;
[[reko::address("D00C")]] unsigned char VICII_m6x;
[[reko::address("D00D")]] unsigned char VICII_m6y;
[[reko::address("D00E")]] unsigned char VICII_m7x;
[[reko::address("D00F")]] unsigned char VICII_m7y;
[[reko::address("D010")]] unsigned char VICII_m_x8;
[[reko::address("D011")]] unsigned char VICII_cr1;		// Control register
[[reko::address("D012")]] unsigned char VICII_raster;		// Control register
[[reko::address("D013")]] unsigned char VICII_lpx;		// Light pen X
[[reko::address("D014")]] unsigned char VICII_lpy;		// Light pen Y

[[reko::address("D020")]] unsigned char VICII_Border_Color;
[[reko::address("D021")]] unsigned char VICII_B0C;		// Background color 0
[[reko::address("D022")]] unsigned char VICII_B1C;		// Background color 1
[[reko::address("D023")]] unsigned char VICII_B2C;		// Background color 2
[[reko::address("D024")]] unsigned char VICII_B3C;		// Background color 3
[[reko::address("D025")]] unsigned char VICII_mm0;		// Sprite multicolor 0
[[reko::address("D026")]] unsigned char VICII_mm1;		// Sprite multicolor 1
[[reko::address("D027")]] unsigned char VICII_m0c;		// Sprite 0 color
[[reko::address("D028")]] unsigned char VICII_m1c;		// Sprite 1 color
[[reko::address("D029")]] unsigned char VICII_m2c;		// Sprite 2 color
[[reko::address("D02A")]] unsigned char VICII_m3c;		// Sprite 3 color
[[reko::address("D02B")]] unsigned char VICII_m4c;		// Sprite 4 color
[[reko::address("D02C")]] unsigned char VICII_m5c;		// Sprite 5 color
[[reko::address("D02D")]] unsigned char VICII_m6c;		// Sprite 6 color
[[reko::address("D02E")]] unsigned char VICII_m7c;		// Sprite 7 color

// References: https://skoolkid.github.io/sk6502/c64rom/index.html
[[reko::segment("E000", "kernal_ROM",  0x2000, "rx")]]
[[reko::address("E544")]] void kernal_clear_screen();	// Clear screen
[[reko::address("FFE7")]] void CLALL(); // Clear file table
