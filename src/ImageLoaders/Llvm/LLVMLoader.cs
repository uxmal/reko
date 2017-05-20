#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.LLVM
{
    public class LLVMLoader : ImageLoader
    {
        public LLVMLoader(IServiceProvider services, string filename, byte[] imgRaw) : base(services, filename, imgRaw)
        {
        }

        public override Address PreferredBaseAddress
        {
            get
            {
                //$TODO: obvious bug; should look at the exe size first.
                return Address.Ptr64(0x400000);
            }

            set
            {
            }
        }

        public override Program Load(Address addrLoad)
        {
            var rdr = new StreamReader(new MemoryStream(RawImage), Encoding.UTF8);
            var parser = new LLVMParser(new LLVMLexer(rdr));
            var module = parser.ParseModule();
            var builder = new ProgramBuilder(Core.Types.PrimitiveType.Pointer64);   //$BUGBUG: obtain pointer size from LLVM!
            var program = builder.BuildProgram(module);
            return program; 
        }

        public override RelocationResults Relocate(Program program, Address addrLoad)
        {
            return new RelocationResults(new List<ImageSymbol>(), new SortedList<Address, ImageSymbol>());
        }
    }
}
