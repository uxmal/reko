using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core
{
	public static class BinaryReaderExtensions
	{
		public static string ReadNullTerminatedString(this BinaryReader rdr)
		{
			string str = "";
			char ch;
			while ((int)(ch = rdr.ReadChar()) != 0)
				str = str + ch;
			return str;
		}
	}
}
