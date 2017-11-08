#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Pascal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Environments.MacOS
{
    public class MpwPascalInterfaceLoader : MetadataLoader
    {
        private byte[] bytes;

        public MpwPascalInterfaceLoader(IServiceProvider services, string filename, byte[] bytes)
            : base(services, filename, bytes)
        {
            this.bytes = bytes;
            this.SystemServices = new Dictionary<int, SystemService>();
        }

        public Dictionary<int, SystemService> SystemServices { get; private set; }

        public override TypeLibrary Load(IPlatform platform, TypeLibrary dstLib)
        {
            var rdr = new StreamReader(new MemoryStream(bytes));
            var lexer = new PascalLexer(rdr);
            var parser = new PascalParser(lexer);
            var symbolTable = new SymbolTable(platform);
            var declarations = parser.Parse();
            var tldser = new TypeLibraryDeserializer(platform, true, dstLib);
            foreach (var decl in declarations)
            {
                symbolTable.Add(decl);
            }
            EvaluateConstants(declarations);
            return dstLib;
        }

        private Dictionary<string,Constant> EvaluateConstants(IEnumerable<Declaration> decls)
        {
            var evaluated = new Dictionary<string, Constant>();

            var dict = new SortedDictionary<string, Exp>();
            foreach (var qq in decls.OfType<ConstantDeclaration>())
            {
                dict[qq.Name] = qq.Exp;
            }
            var ceval = new ConstantEvaluator(dict, evaluated);
            foreach (var de in dict)
            {
                var c = de.Value.Accept(ceval);
                evaluated[de.Key] = c;
            }
            return evaluated;
        }

        private void ProcessDeclaration(Declaration decl, IPlatform platform, TypeLibraryDeserializer tldser, SymbolTable symbolTable)
        {
            var declaration = decl as CallableDeclaration;
            if (declaration == null)
                return;

            //int? 
            //var types = symbolTable.AddDeclaration(declaration);
            //var type = types[0];
            //int? vectorOffset = GetVectorOffset(declaration);
            //if (vectorOffset.HasValue)
            //{
            //    var ntde = new NamedDataTypeExtractor(platform, declaration.decl_specs, symbolTable);
            //    foreach (var declarator in declaration.init_declarator_list)
            //    {
            //        var nt = ntde.GetNameAndType(declarator.Declarator);
            //        var ssig = (SerializedSignature)nt.DataType;
            //        if (ssig.ReturnValue != null)
            //        {
            //            ssig.ReturnValue.Kind = ntde.GetArgumentKindFromAttributes(
            //                "returns", declaration.attribute_list);
            //        }
            //        var sser = new ProcedureSerializer(platform, tldser, platform.DefaultCallingConvention);
            //        var sig = sser.Deserialize(ssig, platform.Architecture.CreateFrame());
            //        SystemServices.Add(
            //            vectorOffset.Value,
            //            new SystemService
            //            {
            //                Name = nt.Name,
            //                SyscallInfo = new SyscallInfo
            //                {
            //                    Vector = vectorOffset.Value,
            //                },
            //                Signature = sig,
            //            });
            //    }
            //}

        }
    }
}
