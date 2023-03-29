// Declarations for BIOS int 10h, which mostly concern video displays.


[[reko::service(vector=0x10, regs={ax:0x4F05,bh:0x00})]]
[[reko::returns(register,"ah")]] unsigned char supervga_select_video_memory_window(
	[[reko::arg(register,"dx")]] unsigned short window_address_in_video_memory);

[[reko::service(vector=0x10, regs={ax:0x4F05,bh:0x01})]]
[[reko::returns(register,"ah")]] unsigned char supervga_get_video_memory_window(
	[[reko::arg(out,register,"bl")]] unsigned char  window_number,
	[[reko::arg(out,register,"dx")]] unsigned short window_address_in_video_memory);
