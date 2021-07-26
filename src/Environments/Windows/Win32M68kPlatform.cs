using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Rtl;
using Reko.Core.Services;
using System;
using System.Collections.Generic;

namespace Reko.Environments.Windows
{
    public class Win32M68kPlatform : Platform
	{
		public Win32M68kPlatform(IServiceProvider services, IProcessorArchitecture arch) : 
            base(services, arch, "winM68k")
        {
		}

		public override string DefaultCallingConvention
		{
			get { return ""; }
		}

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
		{
			return new HashSet<RegisterStorage>();
		}

		public override CallingConvention GetCallingConvention(string? ccName)
		{
			return new M68kCallingConvention(this.Architecture);
		}

		public override ImageSymbol? FindMainProcedure(Program program, Address addrStart)
		{
			Services.RequireService<DecompilerEventListener>().Warn(new NullCodeLocation(program.Name),
				"Win32 M68k main procedure finder not supported.");
			return null;
		}

		public override SystemService? FindService(int vector, ProcessorState? state, SegmentMap? segmentMap)
		{
			//throw new NotImplementedException("INT services are not supported by " + this.GetType().Name);
			return null;
		}

		public override ProcedureBase? GetTrampolineDestination(Address addrInstr, IEnumerable<RtlInstruction> rtls, IRewriterHost host)
		{
			return null;
		}

		public override int GetBitSizeFromCBasicType(CBasicType cb)
		{
			switch (cb)
			{
            case CBasicType.Bool: return 8;
			case CBasicType.Char: return 8;
			case CBasicType.Short: return 16;
			case CBasicType.Int: return 32;
			case CBasicType.Long: return 32;
			case CBasicType.LongLong: return 64;
			case CBasicType.Float: return 32;
			case CBasicType.Double: return 64;
			case CBasicType.LongDouble: return 64;
			case CBasicType.Int64: return 64;
			default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
			}
		}

		public override ExternalProcedure? LookupProcedureByOrdinal(string moduleName, int ordinal)
		{
			EnsureTypeLibraries(PlatformIdentifier);
			if (!Metadata.Modules.TryGetValue(moduleName.ToUpper(), out ModuleDescriptor mod))
				return null;
			if (mod.ServicesByOrdinal.TryGetValue(ordinal, out SystemService svc))
			{
				return new ExternalProcedure(svc.Name!, svc.Signature!);
			}
			else
				return null;
		}

		public override ExternalProcedure? LookupProcedureByName(string? moduleName, string procName)
		{
            if (moduleName is null)
                return null;
			EnsureTypeLibraries(PlatformIdentifier);
			if (!Metadata.Modules.TryGetValue(moduleName.ToUpper(), out ModuleDescriptor mod))
				return null;
			SystemService svc;
			if (mod.ServicesByName.TryGetValue(moduleName, out svc))
			{
				return new ExternalProcedure(svc.Name!, svc.Signature!);
			}
			else
				throw new NotImplementedException();
		}
	}
}
