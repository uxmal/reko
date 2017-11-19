using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.NativeInterface.Interfaces
{
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(CustomAnsiStringMarshaler))]
	public delegate string GetStringDelegate();

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NativeProcessorArchitectureProvider
	{
		public GetStringDelegate GetCpuName;
		public GetStringDelegate GetDescription;
	}
}
