#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Zilog.Z8000
{
    public static class Registers
    {
        public static RegisterStorage[] LongRegisters { get; }
        public static RegisterStorage[] WordRegisters { get; }
        public static RegisterStorage[] ByteRegisters { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            var lregs = factory.RangeOfReg(8, r => $"rr{r*2}", PrimitiveType.Word32);
            LongRegisters = lregs
                .SelectMany(r => new[] { r, r })
                .ToArray();

            var wregs = lregs.SelectMany(r => new[]
            {
                new RegisterStorage($"r{r.Number * 2}", r.Number, 16, PrimitiveType.Word16),
                new RegisterStorage($"r{r.Number * 2 + 1}", r.Number, 0, PrimitiveType.Word16),
            }).ToArray();
            WordRegisters = wregs;

            ByteRegisters = wregs
                    .Take(8)
                    .Select((r, i) => 
                        new RegisterStorage($"rh{i}", r.Number, 8, PrimitiveType.Byte))
                 .Concat(wregs
                    .Take(8)
                    .Select((r, i) =>
                        new RegisterStorage($"rl{i}", r.Number, 0, PrimitiveType.Byte)))
            .ToArray();
        }
    }
}
