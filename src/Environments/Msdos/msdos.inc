// MS-DOS int 21h services

#define SVC(regg) [[reko::service(vector=0x21,regs={regg})]]

#define AREG(regg) [[reko::arg(register,regg)]]
#define RREG(regg) [[reko::returns(register,regg)]]
#define OREG(regg) [[reko::arg(out,register,regg)]]

#define ASEQ(regg) [[reko::arg(seq,regg)]]
#define RSEQ(regg) [[reko::returns(seq,regg)]]
#define OSEQ(regg) [[reko::arg(out,seq,regg)]]

#define RFLAG(regg) [[reko::returns(flag,regg)]]


[[reko::service(vector=0x21,regs={ah:0x30})]]
[[reko::returns(seq,"ah","al")]] unsigned short msdos_get_dos_version(
	[[reko::arg(register,"al")]]		unsigned char bh_choice,
	[[reko::arg(out,register,"bh")]]	unsigned char dos_info,
	[[reko::arg(out,seq,"bl","cx")]]	unsigned int serial_number);

[[reko::service(vector=0x21,regs={ah:0x3D})]]
[[reko::returns(flag,"C")]] bool msdos_open_file(
	[[reko::arg(seq,"ds","dx")]] char *	filename,
	[[reko::arg(register,"al")]]		 unsigned char access_mode,
	[[reko::arg(out,register,"ax")]]	 unsigned short file_handle);

[[reko::service(vector=0x21,regs={ah:0x4A})]]
[[reko::returns(flag,"C")]] bool msdos_resize_memory_block(
	[[reko::arg(register,"es")]]		unsigned short segment_to_resize,
	[[reko::arg(register,"bx")]]		unsigned short new_size_in_paragraphs,
	[[reko::arg(out,register,"ax")]]	unsigned short error_code,
	[[reko::arg(out,register,"bx")]]	unsigned short max_paragraphs_available);

[[reko::service(vector=0x21,regs={ah:0x4C})]]
[[noreturn]] void msdos_terminate(
	[[reko::arg(register,"al")]] unsigned char exit_code);

[[reko::service(vector=0x21,regs={ax:0xFF00,dx:0x0078})]]
[[reko::returns(register,"al")]] unsigned char dos4g_installation_check(
	[[reko::arg(register,"gs")]] unsigned short kernel_segment);

