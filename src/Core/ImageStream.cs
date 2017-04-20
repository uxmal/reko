using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core
{
	public class ImageStream : Stream
	{
		private MemoryArea mem;
		private ImageReader rdr;

		private long offset;
		public ImageStream(MemoryArea memArea) {
			this.mem = memArea;
			this.rdr = new ImageReader(memArea.Bytes);
		}

		public override bool CanRead => true;

		public override bool CanSeek => true;

		public override bool CanWrite => true;

		public override long Length {
			get => mem.Length;
		}

		public override long Position {
			get => offset;
			set => offset = value;
		}

		public override void Flush() {}

		public override int Read(byte[] buffer, int offset, int count) {
			return rdr.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin) {
			return rdr.Seek(offset, origin);
		}

		public override void SetLength(long value) {
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count) {
			mem.WriteBytes(buffer, offset, count);
		}
	}
}
