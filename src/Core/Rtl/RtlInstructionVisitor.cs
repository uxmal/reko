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

namespace Reko.Core.Rtl
{
    /// <summary>
    /// Interface for visiting <see cref="RtlInstruction" />s.
    /// </summary>
    public interface RtlInstructionVisitor<T>
    {
        /// <summary>
        /// Called when the visitor visits an <see cref="RtlAssignment"/> instruction.
        /// </summary>
        /// <param name="ass"><see cref="RtlAssignment"/> being visited.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitAssignment(RtlAssignment ass);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlBranch"/> instruction.
        /// </summary>
        /// <param name="branch"><see cref="RtlBranch"/> being visited.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitBranch(RtlBranch branch);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlCall"/> instruction.
        /// </summary>
        /// <param name="call"><see cref="RtlCall"/> being visited.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitCall(RtlCall call);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlGoto"/> instruction.
        /// </summary>
        /// <param name="go"><see cref="RtlGoto"/> being visited.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitGoto(RtlGoto go);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlIf"/> instruction.
        /// </summary>
        /// <param name="rtlIf"><see cref="RtlIf"/> being visited.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitIf(RtlIf rtlIf);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlInvalid"/> instruction.
        /// </summary>
        /// <param name="invalid"><see cref="RtlInvalid"/> being visited.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitInvalid(RtlInvalid invalid);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlMicroGoto"/> instruction.
        /// </summary>
        /// <param name="uGoto"><see cref="RtlMicroGoto"/> being visited.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitMicroGoto(RtlMicroGoto uGoto);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlNop"/> instruction.
        /// </summary>
        /// <param name="rtlNop"><see cref="RtlNop"/> being visited.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitNop(RtlNop rtlNop);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlReturn"/> instruction.
        /// </summary>
        /// <param name="ret"><see cref="RtlReturn"/> being visited.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitReturn(RtlReturn ret);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlSideEffect"/> instruction.
        /// </summary>
        /// <param name="side"><see cref="RtlSideEffect"/> being visited.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitSideEffect(RtlSideEffect side);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlSwitch"/> instruction.
        /// </summary>
        /// <param name="sw"><see cref="RtlSwitch"/> being visited.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitSwitch(RtlSwitch sw);
    }

    /// <summary>
    /// Interface for visiting <see cref="RtlInstruction" />s with a 
    /// context <typeparamref name="C"/>.
    /// </summary>
    public interface IRtlInstructionVisitor<T, C>
    {
        /// <summary>
        /// Called when the visitor visits an <see cref="RtlAssignment"/> instruction.
        /// </summary>
        /// <param name="ass"><see cref="RtlAssignment"/> being visited.</param>
        /// <param name="ctx">Contextual information.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitAssignment(RtlAssignment ass, C ctx);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlBranch"/> instruction.
        /// </summary>
        /// <param name="branch"><see cref="RtlBranch"/> being visited.</param>
        /// <param name="ctx">Contextual information.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitBranch(RtlBranch branch, C ctx);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlCall"/> instruction.
        /// </summary>
        /// <param name="call"><see cref="RtlCall"/> being visited.</param>
        /// <param name="ctx">Contextual information.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitCall(RtlCall call, C ctx);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlGoto"/> instruction.
        /// </summary>
        /// <param name="go"><see cref="RtlGoto"/> being visited.</param>
        /// <param name="ctx">Contextual information.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitGoto(RtlGoto go, C ctx);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlIf"/> instruction.
        /// </summary>
        /// <param name="rtlIf"><see cref="RtlIf"/> being visited.</param>
        /// <param name="ctx">Contextual information.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitIf(RtlIf rtlIf, C ctx);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlInvalid"/> instruction.
        /// </summary>
        /// <param name="invalid"><see cref="RtlInvalid"/> being visited.</param>
        /// <param name="ctx">Contextual information.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitInvalid(RtlInvalid invalid, C ctx);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlMicroGoto"/> instruction.
        /// </summary>
        /// <param name="uGoto"><see cref="RtlMicroGoto"/> being visited.</param>
        /// <param name="ctx">Contextual information.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitMicroGoto(RtlMicroGoto uGoto, C ctx);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlNop"/> instruction.
        /// </summary>
        /// <param name="rtlNop"><see cref="RtlNop"/> being visited.</param>
        /// <param name="ctx">Contextual information.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitNop(RtlNop rtlNop, C ctx);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlReturn"/> instruction.
        /// </summary>
        /// <param name="ret"><see cref="RtlReturn"/> being visited.</param>
        /// <param name="ctx">Contextual information.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitReturn(RtlReturn ret, C ctx);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlSideEffect"/> instruction.
        /// </summary>
        /// <param name="side"><see cref="RtlSideEffect"/> being visited.</param>
        /// <param name="ctx">Contextual information.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitSideEffect(RtlSideEffect side, C ctx);

        /// <summary>
        /// Called when the visitor visits an <see cref="RtlSwitch"/> instruction.
        /// </summary>
        /// <param name="sw"><see cref="RtlSwitch"/> being visited.</param>
        /// <param name="ctx">Contextual information.</param>
        /// <returns>A value from the vistor.</returns>
        T VisitSwitch(RtlSwitch sw, C ctx);
    }
}
