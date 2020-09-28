using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.CLanguage;
using Reko.Core.Rtl;
using Reko.Core.Services;

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

        public override IPlatformEmulator CreateEmulator(SegmentMap segmentMap, Dictionary<Address, ImportReference> importReferences)
        {
            throw new NotImplementedException();
        }

        public override HashSet<RegisterStorage> CreateTrashedRegisters()
		{
			return new HashSet<RegisterStorage>();
		}

		public override CallingConvention GetCallingConvention(string ccName)
		{
			return new M68kCallingConvention(this.Architecture);
		}

		public override ImageSymbol FindMainProcedure(Program program, Address addrStart)
		{
			Services.RequireService<DecompilerEventListener>().Warn(new NullCodeLocation(program.Name),
				"Win32 M68k main procedure finder not supported.");
			return null;
		}

		public override SystemService FindService(int vector, ProcessorState state)
		{
			//throw new NotImplementedException("INT services are not supported by " + this.GetType().Name);
			return null;
		}

		public override ProcedureBase GetTrampolineDestination(IEnumerable<RtlInstructionCluster> rtls, IRewriterHost host)
		{
			return null;
		}

		public override int GetByteSizeFromCBasicType(CBasicType cb)
		{
			switch (cb)
			{
            case CBasicType.Bool: return 1;
			case CBasicType.Char: return 1;
			case CBasicType.Short: return 2;
			case CBasicType.Int: return 4;
			case CBasicType.Long: return 4;
			case CBasicType.LongLong: return 8;
			case CBasicType.Float: return 4;
			case CBasicType.Double: return 8;
			case CBasicType.LongDouble: return 8;
			case CBasicType.Int64: return 8;
			default: throw new NotImplementedException(string.Format("C basic type {0} not supported.", cb));
			}
		}

		public override ExternalProcedure LookupProcedureByOrdinal(string moduleName, int ordinal)
		{
			EnsureTypeLibraries(PlatformIdentifier);
			ModuleDescriptor mod;
			if (!Metadata.Modules.TryGetValue(moduleName.ToUpper(), out mod))
				return null;
			SystemService svc;
			if (mod.ServicesByOrdinal.TryGetValue(ordinal, out svc))
			{
				return new ExternalProcedure(svc.Name, svc.Signature);
			}
			else
				return null;
		}

		public override ExternalProcedure LookupProcedureByName(string moduleName, string procName)
		{
			EnsureTypeLibraries(PlatformIdentifier);
			ModuleDescriptor mod;
			if (!Metadata.Modules.TryGetValue(moduleName.ToUpper(), out mod))
				return null;
			SystemService svc;
			if (mod.ServicesByName.TryGetValue(moduleName, out svc))
			{
				return new ExternalProcedure(svc.Name, svc.Signature);
			}
			else
				throw new NotImplementedException();
		}
	}
}
