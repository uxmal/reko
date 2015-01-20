using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decompiler.ImageLoaders.OdbgScript
{
    using rulong = System.UInt64;
    class Debugger
    {
        public class fCustomHandlerCallback
        {

        }

        public static void SetHardwareBreakPoint(rulong addr, object o, eHWBPType type, byte size, Action callback)
        {

        }
        internal static rulong GetContextData(eContextData eContextData)
        {
            throw new NotImplementedException();
        }

        internal static void DeleteHardwareBreakPoint(int i)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveMemoryBPX(rulong membpaddr, rulong membpsize)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAllBreakPoints(byte ue)
        {
            throw new NotImplementedException();
        }

        internal static void DisableBPX(rulong addr)
        {
            throw new NotImplementedException();
        }

        internal static void SetBPX(rulong addr, byte p, Action SoftwareCallback)
        {
            throw new NotImplementedException();
        }

        internal static void DeleteBPX(rulong addr)
        {
            throw new NotImplementedException();
        }

        internal static bool SetContextData(eContextData p1, rulong p2)
        {
            throw new NotImplementedException();
        }

        internal static bool SetMemoryBPXEx(rulong addr, rulong size, byte p1, bool p2, Action MemoryCallback)
        {
            throw new NotImplementedException();
        }



        internal static var GetJumpDestination(object p, rulong addr)
        {
            throw new NotImplementedException();
        }

        internal static rulong GetDebuggedFileBaseAddress()
        {
            throw new NotImplementedException();
        }
    }
}
