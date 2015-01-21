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

        public virtual void MsgError(string p)
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_GetMemoryInfo(ulong addr, out MEMORY_BASIC_INFORMATION MemInfo)
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_ReadMemory(ulong addr, ulong memlen, byte[] membuf)
        {
            throw new NotImplementedException();
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

        public virtual var LengthDisassembleEx(ulong addr)
        {
            throw new NotImplementedException();
        }

        public virtual string TE_GetTargetPath()
        {
            throw new NotImplementedException();
        }

        public virtual var TE_GetProcessId()
        {
            throw new NotImplementedException();
        }

        public virtual ulong TE_GetMainThreadHandle()
        {
            throw new NotImplementedException();
        }

        public virtual var TE_GetMainThreadId()
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
            throw new NotImplementedException();
        }

        public virtual bool TE_WriteMemory(ulong addr, ulong len, byte[] membuf)
        {
            throw new NotImplementedException();
        }

        public virtual string DisassembleEx(ulong p)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public virtual void TE_Log(string logstr, object p)
        {
            throw new NotImplementedException();
        }

        public virtual void TE_Log(string p)
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


        public virtual ulong FindHandle(var var, string sClassName, ulong x, ulong y)
        {
            throw new NotImplementedException();
        }

        public virtual int LengthDisassembleBackEx(ulong addr)
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_ReadMemory(ulong src, int p, ulong value)
        {
            throw new NotImplementedException();
        }

        public virtual bool TE_ReadMemory(ulong src, out ulong value)
        {
            throw new NotImplementedException();
        }

        public virtual string DisassembleEx(ulong addr, out int size)
        {
            throw new NotImplementedException();
        }
    }
}
