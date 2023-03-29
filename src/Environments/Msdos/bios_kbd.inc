// Declarations for BIOS int 16h, which mostly
// is raw keyboard support.

[[reko::service(vector=0x16,regs={ah:0x10})]]
void keyboard_get_enhanced_keystroke(
	[[reko::arg(out,register,"al")]] unsigned char ascii_char,
	[[reko::arg(out,register,"ah")]] unsigned char bios_scan_code);

[[reko::service(vector=0x16,regs={ah:0x11})]]
[[reko::returns(flag,"Z")]] bool keyboard_check_enhanced_keystroke(
	[[reko::arg(out,register,"al")]] unsigned char ascii_char,
	[[reko::arg(out,register,"ah")]] unsigned char bios_scan_code);
