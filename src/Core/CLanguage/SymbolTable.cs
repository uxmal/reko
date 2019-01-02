#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using Reko.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core.CLanguage
{
    /// <summary>
    /// Symbol table for the C parser.
    /// </summary>
    public class SymbolTable 
    {
        private IPlatform platform;

        public SymbolTable(IPlatform platform) : this(platform, new Dictionary<string, SerializedType>())
        {
        }

        public SymbolTable(IPlatform platform, Dictionary<string, SerializedType> namedTypes)
        {
            this.platform = platform;

            this.Types = new List<SerializedType>();
            this.StructsSeen = new Dictionary<string, StructType_v1>();
            this.UnionsSeen = new Dictionary<string, UnionType_v1>();
            this.EnumsSeen = new Dictionary<string, SerializedEnumType>();
            this.Constants = new Dictionary<string, int>();
            this.Procedures = new List<ProcedureBase_v1>();
            this.Variables = new List<GlobalDataItem_v2>();
            this.NamedTypes = namedTypes;
            this.Sizer = new TypeSizer(this.NamedTypes);
        }

        public List<SerializedType> Types { get; private set; }
        public Dictionary<string, StructType_v1> StructsSeen { get; private set; }
        public Dictionary<string, UnionType_v1> UnionsSeen { get; private set; }
        public Dictionary<string, SerializedEnumType> EnumsSeen { get; private set; }
        public Dictionary<string, int> Constants { get; private set; }
        public Dictionary<string, SerializedType> NamedTypes { get; private set; }
        public List<ProcedureBase_v1> Procedures { get; private set; }
        public List<GlobalDataItem_v2> Variables { get; private set; }

        public TypeSizer Sizer { get; private set; }

        /// <summary>
        /// Given a C declaration, adds it to the symbol table 
        /// as a function or a type declaration.
        /// </summary>
        /// <param name="decl"></param>
        public List<SerializedType> AddDeclaration(Decl decl)
        {
            var types = new List<SerializedType>();
            var fndec = decl as FunctionDecl;
            if (fndec != null)
            {
                return types;
            }

            IEnumerable<DeclSpec> declspecs = decl.decl_specs;
            var isTypedef = false;
            var scspec = decl.decl_specs[0] as StorageClassSpec;
            if (scspec != null && scspec.Type == CTokenType.Typedef)
            {
                declspecs = decl.decl_specs.Skip(1);
                isTypedef = true;
            }

            var ntde = new NamedDataTypeExtractor(platform, declspecs, this);
            foreach (var declarator in decl.init_declarator_list)
            {
                var nt = ntde.GetNameAndType(declarator.Declarator);
                var serType = nt.DataType;

                var sSig = nt.DataType as SerializedSignature;
                if (sSig != null)
                {
                    if (sSig.ReturnValue != null)
                    {
                        sSig.ReturnValue.Kind = ntde.GetArgumentKindFromAttributes(
                            "returns", decl.attribute_list);
                    }
                    Procedures.Add(new Procedure_v1
                    {
                        Name = nt.Name,
                        Signature = sSig,
                    });
                    types.Add(sSig);
                }
                else if (!isTypedef)
                {
                    var variable = new GlobalDataItem_v2
                    {
                        Name = nt.Name,
                        DataType = serType,
                    };
                    Variables.Add(variable);
                    types.Add(serType);
                }
                if (isTypedef)
                {
                    //$REVIEW: should make sure that if the typedef already exists, 
                    // then the types match but a real compiler would have validated that.
                    var typedef = new SerializedTypedef
                    {
                        Name = nt.Name,
                        DataType = serType
                    };
                    Types.Add(typedef);
                    //$REVIEW: do we really need to check for consistence?
                    NamedTypes[typedef.Name] = serType;
                    types.Add(serType);
                }
            }
            return types;
        }
    }
}
