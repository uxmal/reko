using System;
using System.Runtime.InteropServices;

namespace Reko.Core.NativeInterface
{
    /// <summary>
    /// Posix-specific native methods for loading libraries and getting symbols.
    /// </summary>
	internal class PosixNativeMethods
	{
		public const int RTLD_LAZY = 1;
		public const int RTLD_NOW = 2;
		public const int RTLD_GLOBAL = 4;
		public const int RTLD_NODELETE = 8;
		public const int RTLD_NOLOAD = 16;
		public const int RTLD_DEEPBIND = 32;

		[DllImport("libdl.so")]
		public static extern IntPtr dlopen(string filename, int flags);

		[DllImport("libdl.so")]
		public static extern IntPtr dlsym(IntPtr handle, string symbol);

		[DllImport("libdl.so")]
		public static extern int dlclose(IntPtr handle);
	}

    /// <summary>
    /// Posix implementation of <see cref="ILibraryLoader"/>.
    /// </summary>
    public class PosixLibraryLoader : ILibraryLoader
	{
        /// <inheritdoc/>
		public IntPtr GetSymbol(IntPtr handle, string symName)
		{
			return PosixNativeMethods.dlsym(handle, symName);
		}

        /// <inheritdoc/>
		public IntPtr LoadLibrary(string libPath)
		{
			return PosixNativeMethods.dlopen(libPath, PosixNativeMethods.RTLD_GLOBAL | PosixNativeMethods.RTLD_LAZY);
		}

        /// <inheritdoc/>
		public int Unload(IntPtr handle)
		{
			return PosixNativeMethods.dlclose(handle);
		}
	}
}
