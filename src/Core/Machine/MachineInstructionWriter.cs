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

using Reko.Core.Expressions;
using Reko.Core.NativeInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Reko.Core.Machine
{
    /// <summary>
    /// Used to render machine instructions into text. The abstraction
    /// offers opportunities to perform syntax highlighting etc.
    /// </summary>
    public interface MachineInstructionWriter : INativeInstructionWriter
    {
        /// <summary>
        /// The current platform we're in. May be null, so make sure
        /// you test for that before dereferencing.
        /// </summary>
        IPlatform Platform { get;  }

        /// <summary>
        /// The address of the current instruction being written.
        /// </summary>
        Address Address { get; set; }

        void WriteAddress(string formattedAddress, Address addr);
        void WriteFormat(string fmt, params object[] parms);
    }

    [Flags]
    [NativeInterop]
    public enum MachineInstructionWriterOptions
    {
        None = 0,
        ExplicitOperandSize = 1,
        ResolvePcRelativeAddress = 2,
    }

    /// <summary>
    /// "Dumb" renderer that renders machine instructions as simple text.
    /// </summary>
    public class StringRenderer : MachineInstructionWriter
    {
        private StringBuilder sb;

        public StringRenderer() { sb = new StringBuilder(); }
        public StringRenderer(IPlatform platform) { sb = new StringBuilder(); this.Platform = platform; }

        public IPlatform Platform { get; private set; }
        public Address Address { get; set; }

        /// <summary>
        /// This renderer ignores annotations
        /// </summary>
        /// <param name="annotation"></param>
        public void AddAnnotation(string annotation)
        {
        }

        public void WriteMnemonic(string sMnemonic)
        {
            sb.Append(sMnemonic);
        }

        public void Tab()
        {
            sb.Append('\t');
        }

        public void WriteAddress(string formattedAddress, Address addr)
        {
            sb.Append(formattedAddress);
        }

        public void WriteAddress(string formattedAddres, ulong uAddr)
        {
            WriteAddress(formattedAddres, Address.Ptr64(uAddr));
        }

        public void WriteChar(char c)
        {
            sb.Append(c);
        }

        public void WriteString(string s)
        {
            sb.Append(s);
        }

        public void WriteUInt32(uint u)
        {
            sb.Append(u);
        }

        public void WriteFormat(string fmt, params object[] parms)
        {
            sb.AppendFormat(fmt, parms);
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
