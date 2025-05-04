using System;
using System.Runtime.InteropServices;

namespace Reko.Core.NativeInterface
{
    /// <summary>
    /// Windows-specific native methods for loading libraries and getting symbols.
    /// </summary>
	internal class WindowsNativeMethods
	{
		[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
		public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
	}

    /// <summary>
    /// Windows implementation of <see cref="ILibraryLoader"/>.
    /// </summary>
	public class WindowsLibraryLoader : ILibraryLoader
	{
        /// <inheritdoc/>
		public IntPtr GetSymbol(IntPtr handle, string symName)
		{
			return WindowsNativeMethods.GetProcAddress(handle, symName);
		}

        /// <inheritdoc/>
		public IntPtr LoadLibrary(string libPath)
        {
			return WindowsNativeMethods.LoadLibrary(libPath);
		}

        /// <inheritdoc/>
		public int Unload(IntPtr handle)
		{
			return Convert.ToInt32(
				WindowsNativeMethods.FreeLibrary(handle)
			);
		}
	}
}
