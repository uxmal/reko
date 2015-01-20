using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Decompiler.ImageLoaders.OdbgScript
{
    public static class Ue 
    {
        public const byte UE_BREAKPOINT = 1;
        public const byte UE_OPTION_DISABLEALL = 2;
        public static byte UE_OPTION_REMOVEALLDISABLED;
        public static byte UE_OPTION_REMOVEALLENABLED;
        public static byte UE_MEMORY_WRITE;
        public static byte UE_MEMORY_READ;
        public static eHWBPType UE_HARDWARE_READWRITE;
        public static eHWBPType UE_HARDWARE_WRITE;
        public static eHWBPType UE_HARDWARE_EXECUTE;
        public static byte UE_HARDWARE_SIZE_1;
        public static object UE_ACCESS_READ;
        public static byte UE_SINGLESHOOT;
    }
}
