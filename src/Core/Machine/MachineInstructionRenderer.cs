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

using Reko.Core.NativeInterface;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Reko.Core.Machine
{
    /// <summary>
    /// Used to render machine instructions into text. The abstraction
    /// offers opportunities to perform syntax highlighting etc.
    /// </summary>
    public interface MachineInstructionRenderer : INativeInstructionRenderer
    {
        /// <summary>
        /// Begin the rendering of an instruction at address <paramref name="addr" />.
        /// </summary>
        /// <param name="addr">The address where the instruction begins.</param>
        void BeginInstruction(Address addr);

        /// <summary>
        /// Finish rendering of an instruction.
        /// </summary>
        void EndInstruction();

        /// <summary>
        /// Indicate to the renderer that an operand is about to be rendered.
        /// </summary>
        void BeginOperand();

        /// <summary>
        /// Indicate to the renderer that an operand is finished.
        /// </summary>
        void EndOperand();

        /// <summary>
        /// The address of the current instruction being written.
        /// </summary>
        Address Address { get; }

        /// <summary>
        /// Write an address.
        /// </summary>
        /// <param name="formattedAddress">Address formatted in the 
        /// preferred way of this architecture and syntax.</param>
        /// <param name="addr">Numeric value of the string.</param>
        void WriteAddress(string formattedAddress, Address addr);

        /// <summary>
        /// Writes a format string to the renderer.
        /// </summary>
        /// <param name="fmt">Format string.</param>
        /// <param name="args">Arguments to use in the format string.</param>
        void WriteFormat(string fmt, params object[] args);
    }


    /// <summary>
    /// Flags conrolling the rendering of machine instructions.
    /// </summary>
    [Flags]
    [NativeInterop]
    public enum MachineInstructionRendererFlags
    {
        /// <summary>
        /// No special flags are set.
        /// </summary>
        None = 0,

        /// <summary>
        /// Render the instruction with a specific operand size.
        /// </summary>
        ExplicitOperandSize = 1,

        /// <summary>
        /// Resolve PC-relative addresses to their actual address.
        /// </summary>
        ResolvePcRelativeAddress = 2,
        /// <summary>
        /// Render pseudoinstructions as their 'base' instructions.
        /// </summary>
        RenderInstructionsCanonically = 4,           
    }


    /// <summary>
    /// Describes options to control the rendering of assembly language instructions.
    /// </summary>
    public class MachineInstructionRendererOptions
    {
        /// <summary>
        /// Constructs a new instance of the <see cref="MachineInstructionRendererOptions"/> class.
        /// </summary>
        /// <param name="syntax">Assembler syntax to use.</param>
        /// <param name="flags">Machine instruction renderer flags.</param>
        /// <param name="operandSeparator">String to use for separating operands.</param>
        /// <param name="platform">Current platform.</param>
        public MachineInstructionRendererOptions(
            string? syntax = "",
            MachineInstructionRendererFlags flags = MachineInstructionRendererFlags.None,
            string? operandSeparator = ",",
            IPlatform? platform = null)
        {
            this.Syntax = syntax;
            this.Flags = flags;
            this.OperandSeparator = operandSeparator;
            this.Platform = platform;
            this.SymbolResolver = NullSymbolResolver;
        }

        /// <summary>
        /// Select a particular output syntax by name, if supported.
        /// </summary>
        /// <remarks>
        /// Each processor architecture may have different output syntaxes, including
        /// a default syntax which should be the one used in the processor manufacturer's 
        /// manuals. A null value for this property chooses that default syntax.
        /// </remarks>
        public string? Syntax { get; }

        /// <summary>
        /// The flags that control the rendering of assembly language instructions.
        /// </summary>
        public MachineInstructionRendererFlags Flags { get; }

        /// <summary>
        /// Use this string to specify how assembly language operands shoud be separated.
        /// </summary>
        public string? OperandSeparator { get; }

        /// <summary>
        /// The current operating environment.
        /// </summary>
        public IPlatform? Platform { get; }

        /// <summary>
        /// Delegate used to resolve address to symbols.
        /// </summary>
        public SymbolResolver SymbolResolver { get; set; }

        /// <summary>
        /// A default symbol resolver that does no work.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="symbolName"></param>
        /// <param name="offset"></param>
        /// <returns>Always returns false, since no resolution is ever done.</returns>
        public bool NullSymbolResolver(Address addr, [MaybeNullWhen(false)] out string symbolName, out long offset)
        {
            symbolName = null;
            offset = 0;
            return false;
        }

        /// <summary>
        /// A default instance of the <see cref="MachineInstructionRendererOptions"/>.
        /// </summary>
        public static MachineInstructionRendererOptions Default { get; } = new MachineInstructionRendererOptions(
            syntax: "",
            flags: MachineInstructionRendererFlags.None,
            operandSeparator: ",");
    }

    /// <summary>
    /// Delegate used to resolve a symbol name from an address.
    /// </summary>
    /// <param name="addr">Address.</param>
    /// <param name="symbolName">Symbol name.</param>
    /// <param name="offset">Offset from the symbol name.</param>
    /// <returns>
    /// True if a symbol name could be found; otherwise false.
    /// </returns>
    public delegate bool SymbolResolver(Address addr, [MaybeNullWhen(false)] out string symbolName, out long offset);

    /// <summary>
    /// "Dumb" renderer that renders machine instructions as simple text.
    /// </summary>
    public class StringRenderer : MachineInstructionRenderer
    {
        private readonly StringBuilder sb;
        private Address addrInstr;

        /// <summary>
        /// Constructs a new instance of the <see cref="StringRenderer"/> class.
        /// </summary>
        public StringRenderer() 
        {
            sb = new StringBuilder();
        }

        /// <inheritdoc/>
        public Address Address => addrInstr;

        /// <summary>
        /// This renderer ignores annotations
        /// </summary>
        /// <param name="annotation"></param>
        public void AddAnnotation(string? annotation)
        {
        }

        /// <inheritdoc/>
        public void BeginInstruction(Address addr)
        {
            this.addrInstr = addr;
        }

        /// <inheritdoc/>
        public void EndInstruction()
        {
        }

        /// <inheritdoc/>
        public void BeginOperand()
        {
        }

        /// <inheritdoc/>
        public void EndOperand()
        {
        }

        /// <inheritdoc/>
        public void WriteMnemonic(string sMnemonic)
        {
            sb.Append(sMnemonic);
        }

        /// <inheritdoc/>
        public void Tab()
        {
            sb.Append('\t');
        }

        /// <inheritdoc/>
        public void WriteAddress(string formattedAddress, Address addr)
        {
            sb.Append(formattedAddress);
        }

        /// <inheritdoc/>
        public void WriteAddress(string formattedAddress, ulong uAddr)
        {
            WriteAddress(formattedAddress, Address.Ptr64(uAddr));
        }

        /// <inheritdoc/>
        public void WriteChar(char c)
        {
            sb.Append(c);
        }

        /// <inheritdoc/>
        public void WriteString(string? s)
        {
            sb.Append(s);
        }

        /// <inheritdoc/>
        public void WriteUInt32(uint u)
        {
            sb.Append(u);
        }

        /// <inheritdoc/>
        public void WriteFormat(string fmt, params object[] parms)
        {
            sb.AppendFormat(fmt, parms);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
