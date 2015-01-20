using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        static void TE_WriteMemory(ulong target, int size, double flt)
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

        internal static object TE_GetTargetPath()
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

        internal static void TE_WriteMemory(ulong addr, ulong len, byte[] membuf)
        {
            throw new NotImplementedException();
        }
    }
}
