// Reko definitions for DPMI

[[reko::service(vector=0x31,regs={ax:0x0000})]]
[[reko::returns(flag,"C")]] bool dpmi_allocate_ldt_descriptors(
	[[reko::arg(register,"cx")]]		unsigned short number_of_selectors,
	[[reko::arg(out,register,"ax")]]	unsigned short base_selector);

[[reko::service(vector=0x31,regs={ax:0x0001})]]
[[reko::returns(flag,"C")]] bool dpmi_free_ldt_descriptor(
	[[reko::arg(register,"bx")]]		unsigned short selector_to_free,
	[[reko::arg(out,register,"ax")]]	unsigned short error_code);

[[reko::service(vector=0x31,regs={ax:0x0003})]]
[[reko::returns(register,"ax")]] unsigned short dpmi_get_next_selector_increment_value();

[[reko::service(vector=0x31,regs={ax:0x0006})]]
[[reko::returns(flag,"C")]] bool DPMI_get_segment_base_address(
	[[reko::arg(register,"bx")]]		unsigned short selector,
	[[reko::arg(out,seq,"cx","dx")]]	unsigned int linear_base_address,
	[[reko::arg(out,register,"ax")]]	unsigned int error_code);

[[reko::service(vector=0x31,regs={ax:0x000B})]]
[[reko::returns(flag,"C")]] bool DPMI_get_descriptor(
	[[reko::arg(register,"bx")]]		unsigned short selector,
	[[reko::arg(seq,"es","di")]]		unsigned char __far * descriptorBuffer, // 8 bytes
	[[reko::arg(out,register,"ax")]]	unsigned int error_code);

[[reko::service(vector=0x31,regs={ax:0x0007})]]
[[reko::returns(flag,"C")]] bool DPMI_set_segment_base_address(
	[[reko::arg(register,"bx")]]		unsigned short selector,
	[[reko::arg(seq,"cx","dx")]]		unsigned int linear_base_address,
	[[reko::arg(out,register,"ax")]]	unsigned int error_code);

[[reko::service(vector=0x31,regs={ax:0x0008})]]
[[reko::returns(flag,"C")]] bool DPMI_set_segment_limit(
	[[reko::arg(register,"bx")]]		unsigned short selector,
	[[reko::arg(seq,"cx","dx")]]		unsigned int segment_limit,
	[[reko::arg(out,register,"ax")]]	unsigned int error_code);

[[reko::service(vector=0x31,regs={ax:0x0009})]]
[[reko::returns(flag,"C")]] bool DPMI_set_descriptor_access_rights(
	[[reko::arg(register,"bx")]]		unsigned short selector,
	[[reko::arg(register,"cl")]]		unsigned char access_rights,
	[[reko::arg(register,"ch")]]		unsigned char extended_access_rights,
	[[reko::arg(out,register,"ax")]]	unsigned int error_code);

[[reko::service(vector=0x31,regs={ax:0x0100})]]
[[reko::returns(flag,"C")]] bool DPMI_allocate_DOS_memory_block(
	[[reko::arg(register,"bx")]]	 unsigned short paragraphs, 
	[[reko::arg(out,register,"ax")]] unsigned short real_mode_segment,
	[[reko::arg(out,register,"dx")]] unsigned short prot_mode_selector);

[[reko::service(vector=0x31,regs={ax:0x0101})]]
[[reko::returns(flag,"C")]] bool DPMI_free_DOS_memory_block(
	[[reko::arg(register,"dx")]]	 unsigned short block_selector, 
	[[reko::arg(out,register,"ax")]] unsigned short error_code);


[[reko::service(vector=0x31,regs={ax:0x0200})]]
[[reko::returns(seq,"cx","dx")]] unsigned int DPMI_get_real_mode_interrupt_vector(
	[[reko::arg(register,"bl")]] unsigned char interrupt_number); 

[[reko::service(vector=0x31,regs={ax:0x0400})]]
void DPMI_get_version(
	[[reko::arg(out,register,"ah")]] unsigned char major,
	[[reko::arg(out,register,"al")]] unsigned char minor,
	[[reko::arg(out,register,"bx")]] unsigned short dpmiHostFlags, 
	[[reko::arg(out,register,"cl")]] unsigned char processorType, // (02h=80286, 03h=80386, 04h=80486)
	[[reko::arg(out,register,"dh")]] unsigned char virtualMasterInterruptControllerBaseInterrupt,
	[[reko::arg(out,register,"dl")]] unsigned char virtualSlaveInterruptControllerBaseInterrupt);


[[reko::service(vector=0x31,regs={ax:0x0501})]]
[[reko::returns(flag,"C")]] bool DPMI_allocate_memory_block(
	[[reko::arg(seq,"bx","cx")]]	 unsigned int byte_count, 
	[[reko::arg(out,seq,"bx","cx")]] unsigned int linear_address,
	[[reko::arg(out,seq,"si","di")]] unsigned int memory_block_handle,
	[[reko::arg(out,register,"ax")]] unsigned short error_code);

[[reko::service(vector=0x31,regs={ax:0x0502})]]
[[reko::returns(flag,"C")]] bool DPMI_free_memory_block(
	[[reko::arg(seq,"si","di")]] unsigned int memory_block_handle,
	[[reko::arg(out,register,"ax")]] unsigned short error_code);
