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

using System;
using System.Collections.Generic;
using System.Linq;
using Reko.Core;
using Reko.Core.Types;

namespace Reko.ImageLoaders.MzExe.CodeView
{
    public class TypeBuilder
    {
        private readonly IProcessorArchitecture arch;
        private readonly Dictionary<int, TypeDefinition> dictionary;
        private readonly Dictionary<int, DataType> dataTypesByTypeIndex;

        public TypeBuilder(IProcessorArchitecture arch, Dictionary<int, TypeDefinition> dictionary)
        {
            this.arch = arch;
            this.dictionary = dictionary;
        }

        /// <summary>
        /// Build Reko types from CodeView types.
        /// </summary>
        /// <remarks>
        /// We make two passes through the CodeView types. First we locate 
        /// and build (empty) <see cref="StructureType"/>s from all
        /// CodeView structures. Then 
        /// </remarks>
        public ImageSymbol BuildSymbol(CodeViewLoader.PublicSymbol cvSymbol)
        {
            var imgSym = ImageSymbol.Location(arch, cvSymbol.addr);
            imgSym.Name = cvSymbol.name;
            return imgSym;
        }

        public static List<ImageSymbol> Build(
            IProcessorArchitecture arch,
            Dictionary<int, TypeDefinition> dictionary, 
            List<CodeViewLoader.PublicSymbol> list)
        {
            var builder = new TypeBuilder(arch, dictionary);
            return list.Select(builder.BuildSymbol).ToList();
        }
    }
}