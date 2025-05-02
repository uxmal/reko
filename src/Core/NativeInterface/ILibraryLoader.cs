using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.NativeInterface
{
    /// <summary>
    /// Interface for loading and unloading dynamic libraries (e.g. <c>.dll</c>,
    /// <c>.so</c>).
    /// </summary>
	public interface ILibraryLoader
	{
        /// <summary>
        /// Loads the library from the give file system location.
        /// </summary>
        /// <param name="libPath">Path to dynamlic library.</param>
        /// <returns>A "handle" to the loaded library if successful; otherwise null.
        /// </returns>
		IntPtr LoadLibrary(string libPath);

        /// <summary>
        /// Unloads the library.
        /// </summary>
        /// <param name="handle">Handle obtained from a call to <see cref="LoadLibrary(string)"/>.</param>
        /// <returns></returns>
		int Unload(IntPtr handle);

        /// <summary>
        /// Perorms a lookup of the symbol <paramref name="symName"/> in the loaded 
        /// dynamic library.
        /// </summary>
        /// <param name="handle">Handle obtained from a call to <see cref="LoadLibrary(string)"/>.</param>
        /// <param name="symName">Name of symbol whose value caller wants.</param>
        /// <returns>A handle (typically a pointer) to the object denoted by <paramref name="symName"/> if 
        /// it could be found; otherwise null.
        /// </returns>
		IntPtr GetSymbol(IntPtr handle, string symName);
    }
}
