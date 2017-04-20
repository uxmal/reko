using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core
{
	public static class ImageReaderExtensions
	{
		public static BinaryReader CreateBinaryReader(this ImageReader @rdr)
		{
			return new BinaryReader(new MemoryStream(rdr.Bytes));
		}

		public static BinaryReader CreateBinaryImageReader(this ImageReader @rdr)
		{
			return new BinaryReader(new ImageStream(rdr.Image));
		}
	}
}
