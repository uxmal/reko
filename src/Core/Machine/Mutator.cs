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

namespace Reko.Core.Machine
{
    /// <summary>
    /// A mutator mutates the internal state of the disassembler specified by
    /// the generic type <typeparamref name="TDasm"/>.
    /// </summary>
    /// <typeparam name="TDasm">Type of the disassembler whose state the 
    /// mutator mutates.</typeparam>
    /// <param name="uInstr">Instruction opcode, up to 32 bits long.</param>
    /// <param name="dasm">Disassembler instance.</param>
    /// <returns>True if the mutator executed without errors, false if the
    /// instruction is invalid in some way.</returns>
    public delegate bool Mutator<TDasm>(uint uInstr, TDasm dasm);

    /// <summary>
    /// A mutator mutates the internal state of the disassembler specified by
    /// the generic type <typeparamref name="TDasm"/>.
    /// </summary>
    /// <typeparam name="TDasm">Type of the disassembler whose state the 
    /// mutator mutates.</typeparam>
    /// <param name="uInstr">Instruction opcode, 33-64 bits long.</param>
    /// <param name="dasm">Disassembler instance.</param>
    /// <returns>True if the mutator executed without errors, false if the
    /// instruction is invalid in some way.</returns>
    public delegate bool WideMutator<TDasm>(ulong uInstr, TDasm dasm);
}
