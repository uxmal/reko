using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.NativeInterface
{
	public class CustomAnsiStringMarshaler : ICustomMarshaler
	{
		public void CleanUpManagedData(object ManagedObj) {
			// Intentionally left blank, there's nothing to do here
		}

		public void CleanUpNativeData(IntPtr pNativeData) {
			// Intentionally left blank, we don't want to free the original string
		}

		public int GetNativeDataSize()
		{
			// This means the object we're marshaling is not a value of predefined size
			return -1;
		}

		public IntPtr MarshalManagedToNative(object ManagedObj)
		{
			return Marshal.StringToHGlobalAnsi(ManagedObj as string);
		}

		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			return Marshal.PtrToStringAnsi(pNativeData)!;
		}
	}
}
