using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.NativeInterface
{
	public interface ILibraryLoader
	{
		IntPtr LoadLibrary(string libPath);
		int Unload(IntPtr handle);
		IntPtr GetSymbol(IntPtr handle, string symName);
	}
}
