#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Reko.Core.Absyn;
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.Structure
{
    public class Region
    {
        public Block Block;
        public RegionType Type;
        public List<AbsynStatement> Statements = new List<AbsynStatement>();

        public Region(Block block)
        {
            this.Block = block;
        }

        public virtual void Write(StringWriter sb)
        {
            var f = new AbsynCodeFormatter(new TextFormatter(sb));
            foreach (var stm in Statements)
                stm.Accept(f);
        }

        public Expression Expression { get; set; }

        public override string ToString()
        {
            return Block.Name;
        }
    }

    public enum RegionType
    {
        Linear,
        Condition,
        IncSwitch,
        Switch,
        Tail,
    }
}
