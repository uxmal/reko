#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using Reko.Core.Lib;
using Reko.Core.Output;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Reko.Core
{
	/// <summary>
	/// Summarizes the effects of calling a procedure, as seen by the caller.
    /// Procedure signatures may be shared by several procedures.
	/// </summary>
	/// <remarks>
	/// Calling a procedure affects a few things: the registers, the stack 
    /// depth, and in the case of the Intel x86 architecture the FPU stack 
    /// depth. These effects are summarized by the signature.
    /// <para>
    /// $TODO: There are CPU-specific items (like x86 FPU stack gunk). Move
    /// these into processor-specific subclasses. Also, some architectures 
    /// -- like the FORTH language -- have multiple stacks.
    /// </para>
	/// </remarks>
    public class ProcedureSignature : FunctionType
    {
        public ProcedureSignature() : base()
        {
        }

        public ProcedureSignature(
            Identifier returnId,
            params Identifier[] formalParameters)
            : base(null, returnId, formalParameters)
        {
        }

        public TypeVariable TypeVariable { get; set; }


    }
}
