using System;
using System.Runtime.InteropServices;

#pragma warning disable CS1591

namespace Reko.Core.NativeInterface.Interfaces
{
	public delegate int LoadDelegate([MarshalAs(UnmanagedType.LPStr)] string path);
	public delegate void DisposeDelegate();
	public delegate IntPtr GetNameDelegate();
	public delegate IntPtr GetSymbolsDelegate();
	public delegate IntPtr GetSymbolDelegate(uint index);
	public delegate IntPtr PreviousSymbolDelegate();
	public delegate IntPtr NextSymbolDelegate();
	public delegate void DisposeSymbolDelegate(IntPtr sym);
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(CustomAnsiStringMarshaler))]
	public delegate string GetSymbolNameDelegate(IntPtr sym);
	public delegate ulong GetSymbolStartDelegate(IntPtr sym);
	public delegate ulong GetSymbolEndDelegate(IntPtr sym);
	public delegate ulong GetSymbolLengthDelegate(IntPtr sym);

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	public struct NativeSymbolSourceProvider
	{
		public LoadDelegate Load;
		public DisposeDelegate Dispose;
		public GetNameDelegate GetName;
		public GetSymbolsDelegate GetSymbols;
		public GetSymbolDelegate GetSymbol;
		public PreviousSymbolDelegate PreviousSymbol;
		public NextSymbolDelegate NextSymbol;
		public DisposeSymbolDelegate DisposeSymbol;
		public GetSymbolNameDelegate GetSymbolName;
		public GetSymbolStartDelegate GetSymbolStart;
		public GetSymbolEndDelegate GetSymbolEnd;
		public GetSymbolLengthDelegate GetSymbolLength;
	}
}
