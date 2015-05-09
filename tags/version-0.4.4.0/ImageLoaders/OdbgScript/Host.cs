using Decompiler.Arch.X86;
using Decompiler.Core;
using Decompiler.Core.Services;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Decompiler.ImageLoaders.OdbgScript
{
    public class Host
    {
        public static object TS_LOG_COMMAND;

        private OdbgScriptLoader loader;

        public Host(OdbgScriptLoader loader)
        {
            this.loader = loader;
        }

        public LoadedImage Image { get { return loader.Image; } }

        public virtual ulong TE_AllocMemory(ulong size)
        {
            throw new NotImplementedException();
        }

        public virtual void TE_FreeMemory(ulong p1, uint p2)
        {
            throw new NotImplementedException();
        }

        public static bool TE_WriteMemory(ulong target, int size, double flt)
        {
            throw new NotImplementedException();
        }

        public virtual bool DialogASK(string title, out string returned)
        {
            throw new NotImplementedException();
        }

        public virtual int AssembleEx(string p, ulong addr)
        {
            throw new NotImplementedException();
        }

        public virtual List<string> getlines_file(string p)
        {
            throw new NotImplementedException();
        }

        public virtual string TE_GetTargetDirectory()
        {
            throw new NotImplementedException();
        }

        public virtual object TE_GetCurrentThreadHandle()
        {
            throw new NotImplementedException();
        }

        public virtual void MsgError(string message)
        {
            loader.Services.RequireService<IDiagnosticsService>().Error(message);
        }

        public virtual bool TE_GetMemoryInfo(ulong addr, out MEMORY_BASIC_INFORMATION MemInfo)
        {
            ImageMap map = loader.ImageMap;
            ImageMapSegment segment;
            if (map.TryFindSegment(Address.Ptr32((uint)addr), out segment))
            {
                MemInfo = new MEMORY_BASIC_INFORMATION
                {
                    AllocationBase = segment.Address.ToLinear(),
                    BaseAddress = segment.Address.ToLinear(),
                    RegionSize = segment.Size,
                };
                return true;
            }
            else
            {
                MemInfo = null;
                return false;
            }
        }

        public virtual bool TryReadBytes(ulong addr, ulong memlen, byte[] membuf)
        {
            return Image.TryReadBytes(addr - Image.BaseAddress.ToLinear(), (int)memlen, membuf);
        }

        public virtual object TE_GetProcessHandle()
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_GetModules(List<MODULEENTRY32> Modules)
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_FreeMemory(ulong addr)
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_FreeMemory(ulong addr, ulong size)
        {
            throw new NotImplementedException();
        }

        public virtual Var LengthDisassembleEx(ulong addr)
        {
            throw new NotImplementedException();
        }

        public virtual string TE_GetTargetPath()
        {
            return loader.Filename;
        }

        public virtual ulong TE_GetProcessId()
        {
            throw new NotImplementedException();
        }

        public virtual ulong TE_GetMainThreadHandle()
        {
            throw new NotImplementedException();
        }

        public virtual ulong TE_GetMainThreadId()
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_WriteMemory(ulong addr, int p, byte[] membuf)
        {
            throw new NotImplementedException();
        }

        public virtual int LengthDisassemble(byte[] membuf, int i)
        {
            throw new NotImplementedException();
        }

        public virtual string TE_GetOutputPath()
        {
            return "";
        }

        public virtual bool TE_WriteMemory(ulong addr, ulong len, byte[] membuf)
        {
            throw new NotImplementedException();
        }

        public virtual IntelInstruction DisassembleEx(Address addr)
        {
            var rdr = loader.Architecture.CreateImageReader(loader.Image,  addr);
            var dasm = new X86Disassembler(rdr, PrimitiveType.Word32, PrimitiveType.Word32, false);
            return dasm.DisassembleInstruction();
        }

        public virtual uint TE_GetCurrentThreadId()
        {
            throw new NotImplementedException();
        }

        public virtual string Disassemble(byte[] buffer, ulong addr, out int opsize)
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_WriteMemory(ulong CSP, int p, ulong dw)
        {
            throw new NotImplementedException();
        }

        public virtual bool DialogMSG(string msg, out int input)
        {
            loader.Services.RequireService<IDiagnosticsService>().Warn(msg);
            input = 0;
            return true;
        }

        public virtual void TE_Log(string logstr, object p)
        {
            throw new NotImplementedException();
        }

        public virtual void TE_Log(string message)
        {
            throw new NotImplementedException();
        }

        public virtual bool DialogMSGYN(string msg, out DialogResult input)
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_WriteMemory(ulong target, int p, string value)
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_WriteMemory(ulong target, ulong maxsize, ulong dw)
        {
            throw new NotImplementedException();
        }


        public virtual ulong FindHandle(ulong var, string sClassName, ulong x, ulong y)
        {
            throw new NotImplementedException();
        }

        public virtual int LengthDisassembleBackEx(ulong addr)
        {
            throw new NotImplementedException();
        }

        public virtual void SetOriginalEntryPoint(ulong ep)
        {
            loader.OriginalEntryPoint = Address.Ptr32((uint)ep);
        }
    }
}
