using Reko.Arch.X86;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.OdbgScript
{
    using rulong = System.UInt64;

    public class Debugger
    {
        private IProcessorEmulator emu;

        public Debugger(IProcessorEmulator emu)
        {
            this.emu = emu;
        }

        public Address InstructionPointer { get { return this.emu.InstructionPointer; } }

        public class fCustomHandlerCallback
        {
        }

        public PROCESS_INFORMATION InitDebugEx(string szFileName, string szCommandLine, string szCurrentFolder, Action EntryCallBack)
        {
            return null;
        }

        public static void SetHardwareBreakPoint(rulong addr, object o, eHWBPType type, byte size, Action callback)
        {
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

        public void RemoveMemoryBPX(rulong membpaddr, rulong membpsize)
        {
            throw new NotImplementedException();
        }

        public void RemoveAllBreakPoints(byte ue)
        {
            throw new NotImplementedException();
        }

        public void DisableBPX(rulong addr)
        {
            throw new NotImplementedException();
        }

        public void SetBPX(rulong addr, byte type, Action SoftwareCallback)
        {
            if (type == Ue.UE_BREAKPOINT)
            {
                emu.SetBreakpoint((uint)addr, SoftwareCallback);
                return;
            }
            throw new NotImplementedException();
        }

        public void DeleteBPX(rulong addr)
        {
            emu.DeleteBreakpoint((uint)addr);
        }

        public bool SetContextData(eContextData p1, rulong p2)
        {
            throw new NotImplementedException();
        }

        public bool SetMemoryBPXEx(rulong addr, rulong size, byte p1, bool p2, Action MemoryCallback)
        {
            throw new NotImplementedException();
        }

        public Var GetJumpDestination(object p, rulong addr)
        {
            throw new NotImplementedException();
        }

        public rulong GetDebuggedFileBaseAddress()
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
