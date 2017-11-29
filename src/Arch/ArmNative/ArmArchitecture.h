#pragma once

class ArmArchitecture : public INativeArchitecture
{
public:
	// Inherited via INativeArchitecture
	virtual STDMETHODIMP QueryInterface(REFIID riid, void ** ppvObject) override;
	virtual STDMETHODIMP_(ULONG) AddRef(void) override;
	virtual STDMETHODIMP_(ULONG) Release(void) override;

	virtual STDMETHODIMP_(void) GetAllRegisters(int * pcRegs, const NativeRegister * * ppRegs) override;
private:
	int cRef;
	static const NativeRegister aRegs[];
};