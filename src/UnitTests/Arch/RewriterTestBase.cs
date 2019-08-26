#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core.Types;
using Reko.Core.Rtl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch
{
    public abstract class RewriterTestBase : ArchTestBase
    {
        private MemoryArea instructions;

        public void Rewrite(params uint [] words)
        {
            instructions = RewriteCode(words);
        }

        public void Rewrite(string hexbytes)
        {
            instructions = RewriteCode(hexbytes);
        }

        protected virtual MemoryArea RewriteCode(string hexBytes)
        {
            Assert.Fail($"RewriteCode not implemented for {this.GetType().Name}");
            return null;
        }

        protected virtual MemoryArea RewriteCode(uint [] words)
        {
            Assert.Fail();
            return null;
        }

        protected virtual IRewriterHost CreateHost()
        {
            return new RewriterHost(this.Architecture);
        }
    }
}
