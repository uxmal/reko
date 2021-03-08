using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.OdbgScript
{
    public static class Ue 
    {
        public const byte UE_BREAKPOINT = 0;
        public const byte UE_SINGLESHOOT = 1;
        public const byte UE_HARDWARE = 2;
        public const byte UE_MEMORY = 3;
        public const byte UE_MEMORY_READ = 4;
        public const byte UE_MEMORY_WRITE = 5;
        public const uint UE_BREAKPOINT_TYPE_INT3 = 0x10000000;
        public const uint UE_BREAKPOINT_TYPE_LONG_INT3 = 0x20000000;
        public const uint UE_BREAKPOINT_TYPE_UD2 = 0x30000000;

        public const byte UE_OPTION_DISABLEALL = 2;
        public static byte UE_OPTION_REMOVEALLDISABLED;
        public static byte UE_OPTION_REMOVEALLENABLED;
        public static eHWBPType UE_HARDWARE_READWRITE;
        public static eHWBPType UE_HARDWARE_WRITE;
        public static eHWBPType UE_HARDWARE_EXECUTE;
        public static byte UE_HARDWARE_SIZE_1;
        public const byte UE_ACCESS_READ = 0;
        public const byte UE_PLUGIN_CALL_REASON_PREDEBUG = 1;
        public const byte UE_PLUGIN_CALL_REASON_POSTDEBUG = 2;
        public const byte UE_PLUGIN_CALL_REASON_EXCEPTION = 3;
    }
}
