#pragma once

class ThumbDisassembler : public ComBase, public INativeDisassembler
{
public:
	ThumbDisassembler(const uint8_t * bytes, size_t length, int offset, uint64_t uAddr);
	~ThumbDisassembler();

	STDMETHODIMP QueryInterface(REFIID iid, void ** ppvItf) override;
	STDMETHODIMP_(ULONG) AddRef() override { return  ComBase::AddRef(); }
	STDMETHODIMP_(ULONG) Release() override { return ComBase::Release(); }

	INativeInstruction * STDAPICALLTYPE NextInstruction() override;
private:
	InstrClass InstructionClassFromId(unsigned int armInstrID);

	csh hcapstone;
	const uint8_t * bytes;
	size_t length;
	int offset;
	uint64_t uAddr;
};
