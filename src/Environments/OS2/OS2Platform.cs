using Reko.Core;
using Reko.Core.CLanguage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Environments.OS2
{
    /// <summary>
    /// <see cref="Platform"/> class representing 16-bit OS/2 versions 1.0, 1.2, and 1.3
    /// </summary>
    /// <remarks>
    /// There will be a lot of similarities with Win16 here.
    /// </remarks>
    public class OS2Platform : Platform
    {
        public OS2Platform(IServiceProvider services, IProcessorArchitecture arch, string platformId) : base(services, arch, platformId)
        {
        }

        public override string DefaultCallingConvention => "__cdecl";

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException();
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
        {
            throw new NotImplementedException();
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
            }
            throw new NotImplementedException();
        }

        public override CallingConvention GetCallingConvention(string ccName)
        {
            throw new NotImplementedException();
        }

        public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
        {
            throw new NotImplementedException();
        }
    }
}
