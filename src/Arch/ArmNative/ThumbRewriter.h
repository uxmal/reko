#pragma once

class ThumbRewriter : public ComBase, public INativeRewriter
{
public:
	ThumbRewriter(
		const uint8_t * rawBytes,
		size_t length,
		uint64_t address,
		INativeRtlEmitter * emitter,
		INativeTypeFactory * ntf,
		INativeRewriterHost * host);

	STDMETHOD(QueryInterface)(REFIID iid, void ** ppvOut) override;
	STDMETHOD_(ULONG, AddRef)() override { return ComBase::AddRef(); }
	STDMETHOD_(ULONG, Release)() override { return ComBase::Release(); }

	STDMETHOD(Next)();
	int32_t STDMETHODCALLTYPE GetCount();
private:
	ULONG cRef;	// COM ref count.

	csh hcapstone;
	INativeRtlEmitter & m;
	INativeTypeFactory & ntf;
	INativeRewriterHost * host;
	cs_insn * instr;
	const uint8_t * rawBytes;
	size_t available;			// Available bytes left past rawBytes
	uint64_t address;
	RtlClass rtlClass;

};