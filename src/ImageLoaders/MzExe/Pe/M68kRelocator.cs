using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Collections;
using Reko.Core.Memory;

namespace Reko.ImageLoaders.MzExe.Pe
{
    public class M68kRelocator : Relocator
	{
		private readonly IServiceProvider services;

		public M68kRelocator(IServiceProvider services, Program program) : base(program)
		{
			this.services = services;
			this.program = program;
		}

		public override void ApplyRelocation(Address baseOfImage, uint page, EndianImageReader rdr, RelocationDictionary relocations) {
			rdr.ReadUInt16();
			return;
		}
	}
}
