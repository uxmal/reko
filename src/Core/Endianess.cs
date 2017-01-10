using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core {
	/* http://stackoverflow.com/a/2624377 */
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Struct | AttributeTargets.Class, Inherited = true)]
	public class EndianAttribute : Attribute {
		public Endianness Endianness { get; private set; }

		public EndianAttribute(Endianness endianness) {
			this.Endianness = endianness;
		}
	}

	public enum Endianness {
		BigEndian,
		LittleEndian
	}
}
