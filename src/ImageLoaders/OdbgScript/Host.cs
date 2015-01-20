using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Decompiler.ImageLoaders.OdbgScript
{
    class Host
    {
        public static object TS_LOG_COMMAND;
        internal static ulong TE_AllocMemory(ulong size)
        {
            throw new NotImplementedException();
        }

        internal static void TE_FreeMemory(ulong p1, uint p2)
        {
            throw new NotImplementedException();
        }

        public static bool TE_WriteMemory(ulong target, int size, double flt)
        {
            throw new NotImplementedException();
        }

        internal static bool DialogASK(string title, out string returned)
        {
            throw new NotImplementedException();
        }

        internal static int AssembleEx(string p, ulong addr)
        {
            throw new NotImplementedException();
        }

        internal static List<string> getlines_file(string p)
        {
            throw new NotImplementedException();
        }

        internal static string TE_GetTargetDirectory()
        {
            throw new NotImplementedException();
        }

        internal static object TE_GetCurrentThreadHandle()
        {
            throw new NotImplementedException();
        }

        internal static void MsgError(string p)
        {
            throw new NotImplementedException();
        }

        internal static bool TE_GetMemoryInfo(ulong addr, out MEMORY_BASIC_INFORMATION MemInfo)
        {
            throw new NotImplementedException();
        }

        internal static bool TE_ReadMemory(ulong addr, ulong memlen, byte[] membuf)
        {
            throw new NotImplementedException();
        }

        internal static object TE_GetProcessHandle()
        {
            throw new NotImplementedException();
        }

        internal static bool TE_GetModules(List<MODULEENTRY32> Modules)
        {
            throw new NotImplementedException();
        }

        internal static bool TE_FreeMemory(ulong addr)
        {
            throw new NotImplementedException();
        }

        internal static bool TE_FreeMemory(ulong addr, ulong size)
        {
            throw new NotImplementedException();
        }

        internal static var LengthDisassembleEx(ulong addr)
        {
            throw new NotImplementedException();
        }

        internal static string TE_GetTargetPath()
        {
            throw new NotImplementedException();
        }

        internal static var TE_GetProcessId()
        {
            throw new NotImplementedException();
        }

        internal static ulong TE_GetMainThreadHandle()
        {
            throw new NotImplementedException();
        }

        internal static var TE_GetMainThreadId()
        {
            throw new NotImplementedException();
        }

        internal static bool TE_WriteMemory(ulong addr, int p, byte[] membuf)
        {
            throw new NotImplementedException();
        }

        internal static int LengthDisassemble(byte[] membuf, int i)
        {
            throw new NotImplementedException();
        }

        internal static string TE_GetOutputPath()
        {
            throw new NotImplementedException();
        }

        internal static bool TE_WriteMemory(ulong addr, ulong len, byte[] membuf)
        {
            throw new NotImplementedException();
        }

        internal static string DisassembleEx(ulong p)
        {
            throw new NotImplementedException();
        }

        internal static uint TE_GetCurrentThreadId()
        {
            throw new NotImplementedException();
        }

        internal static string Disassemble(byte[] buffer, ulong addr, out int opsize)
        {
            throw new NotImplementedException();
        }

        internal static bool TE_WriteMemory(ulong CSP, int p, ulong dw)
        {
            throw new NotImplementedException();
        }

        internal static bool DialogMSG(string msg, out int input)
        {
            throw new NotImplementedException();
        }

        internal static void TE_Log(string logstr, object p)
        {
            throw new NotImplementedException();
        }

        internal static void TE_Log(string p)
        {
            throw new NotImplementedException();
        }

        internal static bool DialogMSGYN(string msg, out DialogResult input)
        {
            throw new NotImplementedException();
        }

        internal static bool TE_WriteMemory(ulong target, int p, string value)
        {
            throw new NotImplementedException();
        }

        internal static bool TE_WriteMemory(ulong target, ulong maxsize, ulong dw)
        {
            throw new NotImplementedException();
        }


        internal static ulong FindHandle(var var, string sClassName, ulong x, ulong y)
        {
            throw new NotImplementedException();
        }

        internal static int LengthDisassembleBackEx(ulong addr)
        {
            throw new NotImplementedException();
        }

        internal static bool TE_ReadMemory(ulong src, int p, ulong value)
        {
            throw new NotImplementedException();
        }

        internal static bool TE_ReadMemory(ulong src, out ulong value)
        {
            throw new NotImplementedException();
        }

        internal static string DisassembleEx(ulong addr, out int size)
        {
            throw new NotImplementedException();
        }
    }
}
