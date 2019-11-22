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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.LLVM
{
    public class DataLayoutParser
    {
        private string sLayout;

        public DataLayoutParser(string sLayout)
        {
            this.sLayout = sLayout;
        }

        public DataLayout Parse()
        {
            var specs = sLayout.Split('-');
            var layout = new DataLayout();
            foreach (var spec in specs)
            {
                switch (spec[0])
                {
                case 'E': layout.Endianness = EndianServices.Big; break;
                case 'e': layout.Endianness = EndianServices.Big; break;
                case 'm': break; // mangling; 
                case 'n': break; // int sizes.
                case 'p':
                    var (space, ptrSpec) = ParsePointerSpec(spec);
                    layout.PointerLayouts[space] = ptrSpec;
                    break;
                }
            }
            return layout;
        }

        private (int, PointerLayout) ParsePointerSpec(string spec)
        {
            int i = 1;
            TryParseNumber(spec, ref i, out int space);
            Expect(':', spec, ref i);
            ExpectNumber(spec, ref i, out int size);
            if (PeekAndDiscard(':', spec, ref i))
            {
                //$TODO: more parsing needed.
            }
            return (space, new PointerLayout
            {
                BitSize = size
            });
        }

        private bool TryParseNumber(string spec, ref int i, out int n)
        {
            n = 0;
            if (i >= spec.Length)
                return false;
            throw new NotImplementedException();
        }

        private void Expect(char v, string spec, ref int i)
        {
            if (i < spec.Length && spec[i++] == v)
                return;
            throw new FormatException();
        }

        private void ExpectNumber(string spec, ref int i, out int n)
        {
            if (i >= spec.Length)
                throw new FormatException();
            if (!Char.IsDigit(spec[i]))
                throw new FormatException();
            n = 0;
            while (i < spec.Length && Char.IsDigit(spec[i]))
            {
                n = n * 10 + (spec[i] - '0');
                ++i;
            }
        }

        private bool PeekAndDiscard(char v, string spec, ref int i)
        {
            throw new NotImplementedException();
        }
    }
}
