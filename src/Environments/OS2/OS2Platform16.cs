using Reko.Core;
using Reko.Core.CLanguage;
using System;
using System.Collections.Generic;
using Reko.Arch.X86;

namespace Reko.Environments.OS2
{
    /// <summary>
    /// <see cref="Platform"/> class representing 16-bit OS/2 versions 1.0, 1.2, and 1.3
    /// </summary>
    /// <remarks>
    /// There will be a lot of similarities with Win16 here.
    /// </remarks>
    public class OS2Platform16 : Platform
    {
        public OS2Platform16(IServiceProvider services, IProcessorArchitecture arch) : base(services, arch, "os2-16")
        {
        }

        public override string DefaultCallingConvention => "pascal";

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException();
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            // Some calling conventions can save registers, like _watcall
            return new HashSet<RegisterStorage>
            {
                Registers.ax,
                Registers.bx,
                Registers.cx,
                Registers.dx,
                Registers.es,
                Registers.Top,
            };
        }

        public override SystemService FindService(int vector, ProcessorState state)
        {
            throw new NotImplementedException();
        }

        public override int GetByteSizeFromCBasicType(CBasicType cb)
        {
            switch (cb)
            {
                case CBasicType.Bool: return 1;
                case CBasicType.Char: return 1;
                case CBasicType.Short: return 2;
                case CBasicType.Int: return 2;
                case CBasicType.Long: return 4;
                case CBasicType.Float: return 4;
                case CBasicType.Double: return 8;
                // Seen in Watcom
                case CBasicType.Int64: return 8;
                // Seen in OpenWatcom as an alias to __int64
                case CBasicType.LongLong: return 8;
                // Used for EBCDIC, Shift-JIS and Unicode
                case CBasicType.WChar_t: return 2;
            }
            throw new NotImplementedException();
        }

        public override CallingConvention GetCallingConvention(string ccName)
        {
            if (ccName == null)
                ccName = "";
            switch (ccName)
            {
                case "":
                // Used by Microsoft C
                case "__cdecl":
                case "cdecl":
                    return new X86CallingConvention(4, 2, 4, true, false);
                // Default for system libraries
                case "pascal":
                    return new X86CallingConvention(4, 2, 4, false, true);
            }
            throw new NotSupportedException(string.Format("Calling convention '{0}' is not supported.", ccName));
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}
