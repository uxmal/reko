#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

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

		public override bool CanRead { get { return true; } }

		public override bool CanSeek { get { return true; } }

        public override bool CanWrite { get { return true; } }

        public override long Length { get { return mem.Length; } }

		public override long Position
        {
			get { return this.offset; }
            set { this.offset = value; }
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
