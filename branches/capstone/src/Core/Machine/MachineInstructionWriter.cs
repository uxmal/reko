#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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
using System.Linq;
using System.Text;

namespace Decompiler.Core.Machine
{
    public interface MachineInstructionWriter
    {
        void Opcode(string opcode);
        void Address(string formattedAddress, Address addr);
        void Tab();
        void Write(char c);
        void Write(uint n);
        void Write(string s);
        void Write(string fmt, params object[] parms);
    }

    public class StringRenderer : MachineInstructionWriter
    {
        private StringBuilder sb;

        public StringRenderer() { sb = new StringBuilder(); }

        public void Opcode(string opcode)
        {
            sb.Append(opcode);
        }

        public void Tab()
        {
            sb.Append('\t');
        }

        public void Address(string formattedAddress, Address addr)
        {
            sb.Append(formattedAddress);
        }

        public void Write(char c)
        {
            sb.Append(c);
        }

        public void Write(string s)
        {
            sb.Append(s);
        }

        public void Write(uint u)
        {
            sb.Append(u);
        }

        public void Write(string fmt, params object[] parms)
        {
            sb.AppendFormat(fmt, parms);
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
