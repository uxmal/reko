#region License
/* 
 * Copyright (C) 1999-2026 Pavel Tomin.
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
using Reko.Core.Expressions;
using Reko.Core.Types;

namespace Reko.Scanning;

/// <summary>
/// Results of a successfull varargs analysis.
/// </summary>
/// <param name="Signature">Function signature.</param>
/// <param name="FormatStringAddress">The address of the format string</param>
/// <param name="FormatString">The format string itself.</param>
public record VarargsResult(
        FunctionType Signature,
        Address FormatStringAddress,
        StringConstant FormatString);
