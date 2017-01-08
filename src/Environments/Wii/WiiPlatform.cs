using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core.CLanguage;
using Reko.Core.Serialization;
using Reko.Core.Types;
using Reko.Arch.PowerPC;

namespace Reko.Environments.Wii
{
	public class WiiPlatform : Platform {
		public WiiPlatform(IServiceProvider services, IProcessorArchitecture arch, string platformId) : base(services, arch, "wii") {
		}

		public override string DefaultCallingConvention {
			get {
				return "";
			}
		}

		public override HashSet<RegisterStorage> CreateImplicitArgumentRegisters() {
			//$TODO: find out what registers are always preserved
			return new HashSet<RegisterStorage>();
		}

		public override ProcedureSerializer CreateProcedureSerializer(ISerializedTypeVisitor<DataType> typeLoader, string defaultConvention) {
			return new PowerPcProcedureSerializer(
				(PowerPcArchitecture)Architecture,
				typeLoader,
				defaultConvention);
		}

		public override HashSet<RegisterStorage> CreateTrashedRegisters() {
			//TODO: find out what registers are always trashed
			return new HashSet<RegisterStorage>();
		}

		public override SystemService FindService(int vector, ProcessorState state) {
			//$TODO: implement some services;
			return null;
		}

		public override int GetByteSizeFromCBasicType(CBasicType cb) {
			throw new NotImplementedException();
		}

		public override ProcedureBase GetTrampolineDestination(ImageReader imageReader, IRewriterHost host) {
			return null;
		}

		public override ExternalProcedure LookupProcedureByName(string moduleName, string procName) {
			throw new NotImplementedException();
		}
	}
}
