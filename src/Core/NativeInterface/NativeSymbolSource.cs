using Reko.Core.Loading;
using Reko.Core.NativeInterface.Interfaces;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Reko.Core.NativeInterface
{
    /// <summary>
    /// Standard implementation of <see cref="ISymbolSource"/> for native code.
    /// </summary>
    public class NativeSymbolSource : ISymbolSource
	{
        private IProcessorArchitecture arch;
		private IntPtr handle;
		private NativeSymbolSourceProvider prv;
		private readonly ILibraryLoader loader;

		private const string SYM_NAME = "gSymProvider";

        /// <summary>
        /// Constructs a native symbol source.
        /// </summary>
        /// <param name="arch">Processor architecture.</param>
        /// <param name="libPath">File system path to the symbol source.</param>
        /// <param name="ldr"><see cref="ILibraryLoader"/> instance used to load native code.</param>
		public NativeSymbolSource(IProcessorArchitecture arch, string libPath, ILibraryLoader ldr)
		{
            this.arch = arch;
			loader = ldr;
			handle = loader.LoadLibrary(libPath);

			IntPtr gSymProvider = loader.GetSymbol(handle, SYM_NAME);
			prv = (NativeSymbolSourceProvider)Marshal.PtrToStructure(gSymProvider, typeof(NativeSymbolSourceProvider))!;
		}

        /// <inheritdoc/>
		public bool CanLoad(string filename, byte[] fileContents)
		{
			return false;
		}

        /// <inheritdoc/>
		public void Dispose()
		{
			prv.Dispose();
		}

        /// <inheritdoc/>
		public List<ImageSymbol> GetAllSymbols()
		{
			//TODO: ask if number of symbols is available and preallocate list
			List<ImageSymbol> symbols = new List<ImageSymbol>();

			IntPtr syms = prv.GetSymbols();
			IntPtr sym;
			for(int off=0; ; off += Marshal.SizeOf(sym)) {
				sym = Marshal.ReadIntPtr(syms, off);
				if (sym == IntPtr.Zero)
					break;

				ulong start = prv.GetSymbolStart(sym);
				ulong end = prv.GetSymbolEnd(sym);

                symbols.Add(ImageSymbol.Create(
                    SymbolType.Unknown,
                    arch,
                    Address.Ptr32((uint)start),
                    name: prv.GetSymbolName(sym),
                    dataType: new UnknownType((int)(end - start))));
			}

			return symbols;
		}
	}
}
