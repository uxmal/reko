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

using System;
using System.Collections.Generic;

namespace Reko.Core.Hll.C
{
    /// <summary>
    /// Extracts the name from a C declarator.
    /// </summary>
    public class NameExtractor : DeclaratorVisitor<string>
    {
        /// <summary>
        /// Extracts the name from a declarator and decl-specs.
        /// </summary>
        /// <param name="declspecs">List of decl-specifications.</param>
        /// <param name="declarator"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static string? GetName(IEnumerable<DeclSpec> declspecs, Declarator? declarator, ParserState state)
        {
            var ndte = new NameExtractor();
            if (declarator is not null)
                return declarator.Accept(ndte);
            else
                return null;
        }

        /// <inheritdoc/>
        public string VisitId(IdDeclarator id)
        {
            return id.Name;
        }

        /// <inheritdoc/>
        public string VisitArray(ArrayDeclarator array)
        {
            return array.Declarator.Accept(this);
        }

        /// <inheritdoc/>
        public string VisitField(FieldDeclarator field)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string VisitPointer(PointerDeclarator pointer)
        {
            return pointer.Pointee!.Accept(this);
        }

        /// <inheritdoc/>
        public string VisitReference(ReferenceDeclarator reference)
        {
            return reference.Referent!.Accept(this);
        }

        /// <inheritdoc/>
        public string VisitFunction(FunctionDeclarator function)
        {
            return function.Declarator.Accept(this);
        }

        /// <inheritdoc/>
        public string VisitCallConvention(CallConventionDeclarator callConvention)
        {
            return callConvention.Declarator.Accept(this);
        }
    }
}
