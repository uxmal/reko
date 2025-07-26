#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Services;

namespace Reko.Services
{
    /// <summary>
    /// A specialization of the <see cref="IEventListener"/> with
    /// decompiler-specific additions.
    /// </summary>
    public interface IDecompilerEventListener : IEventListener
    {
        /// <summary>
        /// This method is called when a procedure is found in the program.
        /// </summary>
        /// <param name="program">Program in which the procedure was found.</param>
        /// <param name="addrProc">Address of the procedure.
        /// </param>
        void OnProcedureFound(Program program, Address addrProc);
    }

    /// <summary>
    /// Dummy implementation of <see cref="IDecompilerEventListener"/>.
    /// </summary>
    public class NullDecompilerEventListener : NullEventListener, IDecompilerEventListener
    {
        /// <summary>
        /// Global instance of the non-mutable null listener.
        /// </summary>
        public static new IDecompilerEventListener Instance { get; } = new NullDecompilerEventListener();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void OnProcedureFound(Program program, Address addrProc) { }
    }
}
