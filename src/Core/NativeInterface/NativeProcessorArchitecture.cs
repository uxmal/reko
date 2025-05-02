using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Memory;
using Reko.Core.NativeInterface.Interfaces;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Reko.Core.NativeInterface
{
    /// <summary>
    /// Common implementation of <see cref="IProcessorArchitecture"/> for native
    /// architectures.
    /// </summary>
    public class NativeProcessorArchitecture : ProcessorArchitecture
	{
		private IntPtr handle;
		private ILibraryLoader loader;
		private NativeProcessorArchitectureProvider prv;

		private const string SYM_NAME = "gCPUProvider";

        /// <summary>
        /// Creates a new instance of the <see cref="NativeProcessorArchitecture"/>.
        /// </summary>
        /// <param name="services"><see cref="IServiceProvider"/> interface.</param>
        /// <param name="archID">Identifier of this architecture.</param>
        /// <param name="libPath">Path to the native implementation of the architecture.</param>
        /// <param name="ldr"><see cref="ILibraryLoader"/> implementation to load native code.</param>
        /// <param name="options">Processor options.</param>
		public NativeProcessorArchitecture(IServiceProvider services, string archID, string libPath, ILibraryLoader ldr, Dictionary<string, object> options)
            : base(services, archID, options, null!, null!)
		{
			loader = ldr;
			handle = ldr.LoadLibrary(libPath);
			prv = (NativeProcessorArchitectureProvider)Marshal.PtrToStructure(ldr.GetSymbol(handle, SYM_NAME), typeof(NativeProcessorArchitectureProvider))!;
		}

        /// <summary>
        /// The name of the architecture.
        /// </summary>
		public new string Name {
			get {
				return prv.GetCpuName();
			}
		}

        /// <summary>
        /// The description of the architecture.
        /// </summary>
		public new string Description {
			get {
				return prv.GetDescription();
			}
		}

        /// <inheritdoc/>
		public override IEnumerable<MachineInstruction> CreateDisassembler(EndianImageReader imageReader)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
        public override IEqualityComparer<MachineInstruction> CreateInstructionComparer(Normalize norm)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public override IEnumerable<Address> CreatePointerScanner(SegmentMap map, EndianImageReader rdr, IEnumerable<Address> knownAddresses, PointerScannerFlags flags)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public override ProcessorState CreateProcessorState()
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public override IEnumerable<RtlInstructionCluster> CreateRewriter(EndianImageReader rdr, ProcessorState state, IStorageBinder frame, IRewriterHost host)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public override FlagGroupStorage GetFlagGroup(RegisterStorage flagRegister, uint grf)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
        public override FlagGroupStorage GetFlagGroup(string name)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
        public override IEnumerable<FlagGroupStorage> GetSubFlags(FlagGroupStorage flags)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override SortedList<string, int> GetMnemonicNames()
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public override int? GetMnemonicNumber(string name)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public override RegisterStorage[] GetRegisters()
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
        public override FlagGroupStorage[] GetFlags()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string GrfToString(RegisterStorage flagregister, string prefix, uint grf)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public override Address MakeAddressFromConstant(Constant c, bool codeAlign)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public override Address? ReadCodeAddress(int size, EndianImageReader rdr, ProcessorState? state)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public override bool TryGetRegister(string name, out RegisterStorage reg)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public override bool TryParseAddress(string? txtAddr, out Address addr)
		{
			throw new NotImplementedException();
		}
    }
}
