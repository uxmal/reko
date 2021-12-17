using Reko.Core;
using Reko.Core.Emulation;
using Reko.Core.Expressions;
using System;

namespace Reko.ImageLoaders.OdbgScript
{
    using rulong = System.UInt64;

    public class Debugger
    {
        private IProcessorArchitecture arch;
        private IProcessorEmulator emu;

        public Debugger( IProcessorArchitecture arch, IProcessorEmulator emu)
        {
            this.arch = arch;
            this.emu = emu;
        }

        public Address InstructionPointer { get { return this.emu.InstructionPointer; } }

        public class fCustomHandlerCallback
        {
        }

        public PROCESS_INFORMATION InitDebugEx(string szFileName, string szCommandLine, string szCurrentFolder, Action EntryCallBack)
        {
            return null!;
        }

        public void SetHardwareBreakPoint(Address addr, object? o, eHWBPType type, byte size, Action callback)
        {
            this.emu.SetBreakpoint(addr.ToLinear(), callback);
        }

        public rulong GetContextData(RegisterStorage register)
        {
            return emu.ReadRegister(register);
        }

        public rulong GetContextData(eContextData eContextData)
        {
            switch (eContextData)
            {
            case eContextData.UE_EIP: return emu.InstructionPointer.ToLinear();
            }
            throw new NotImplementedException();
        }

        public void DeleteHardwareBreakPoint(int i)
        {
            throw new NotImplementedException();
        }

        public void RemoveMemoryBPX(Address membpaddr, rulong membpsize)
        {
            throw new NotImplementedException();
        }

        public void RemoveAllBreakPoints(byte ue)
        {
            throw new NotImplementedException();
        }

        public void DisableBPX(Address addr)
        {
            throw new NotImplementedException();
        }

        public void SetBPX(Address addr, byte type, Action SoftwareCallback)
        {
            if (type == Ue.UE_BREAKPOINT)
            {
                emu.SetBreakpoint(addr.ToLinear(), SoftwareCallback);
                return;
            }
            throw new NotImplementedException();
        }

        public void DeleteBPX(Address addr)
        {
            emu.DeleteBreakpoint(addr.ToLinear());
        }

        public bool SetContextData(RegisterStorage reg, rulong value)
        {
            emu.WriteRegister(reg, value);
            return true;
        }

        public bool SetContextData(eContextData reg, rulong value)
        {
            switch (reg)
            {
            case eContextData.UE_EIP:
                var cAddr = Constant.Create(arch.PointerType, value);
                emu.InstructionPointer = arch.MakeAddressFromConstant(cAddr, true);
                return true;
            }
            throw new NotImplementedException();
        }

        public bool SetMemoryBPXEx(Address addr, rulong size, byte p1, bool p2, Action MemoryCallback)
        {
            throw new NotImplementedException();
        }

        public Var GetJumpDestination(object p, Address addr)
        {
            throw new NotImplementedException();
        }

        public Address GetDebuggedFileBaseAddress()
        {
            throw new NotImplementedException();
        }

        public bool SetContextFPUDataEx(object p, FLOATING_SAVE_AREA fltctx)
        {
            throw new NotImplementedException();
        }

        public bool GetContextFPUDataEx(object p, FLOATING_SAVE_AREA fltctx)
        {
            throw new NotImplementedException();
        }

        public void StepInto(Action StepIntoCallback)
        {
            emu.StepInto(StepIntoCallback);
        }

        public void StepOver(Action StepOverCallback)
        {
            emu.StepOver(StepOverCallback);
        }

        internal void DebugLoop()
        {
            throw new NotImplementedException();
        }

        internal void StopDebug()
        {
            emu.Stop();
        }

        public ulong GetRegisterValue(RegisterStorage reg)
        {
            return emu.ReadRegister(reg);
        }

        public void SetRegisterValue(RegisterStorage reg, ulong value)
        {
            emu.WriteRegister(reg, value);
        }
    }
}
