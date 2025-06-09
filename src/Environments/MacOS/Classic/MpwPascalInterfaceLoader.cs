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
using Reko.Core.Expressions;
using Reko.Core.Hll.Pascal;
using Reko.Core.Loading;
using Reko.Core.Serialization;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Environments.MacOS.Classic
{
    /// <summary>
    /// Loads an MPW Pascal interface definition file into a Reko type library.
    /// </summary>
    public class MpwPascalInterfaceLoader : MetadataLoader
    {
        private readonly byte[] bytes;

        public MpwPascalInterfaceLoader(IServiceProvider services, ImageLocation imageUri, byte[] bytes)
            : base(services, imageUri, bytes)
        {
            this.bytes = bytes;
        }

        public override TypeLibrary Load(IPlatform platform, TypeLibrary dstLib)
        {
            var rdr = new StreamReader(new MemoryStream(bytes));
            var lexer = new PascalLexer(rdr, false);    // It seems MPW pascal doesn't allow nesting comments.
            var parser = new PascalParser(lexer);
            var symbolTable = new SymbolTable(platform);
            var declarations = parser.Parse();
            var tldser = new TypeLibraryDeserializer(platform, true, dstLib);
            var constants = EvaluateConstants(declarations);
            var typeImporter = new TypeImporter(platform, tldser, constants, dstLib);
            typeImporter.LoadTypes(declarations);
            LoadServices(declarations, typeImporter, constants, platform, dstLib);
            return dstLib;
        }

        private void LoadServices(
            List<Declaration> declarations, 
            TypeImporter typeImporter,
            IDictionary<string, Expression> constants,
            IPlatform platform,
            TypeLibrary typelib)
        {
            if (!typelib.Modules.TryGetValue("", out var module))
            {
                module = new ModuleDescriptor("");
                typelib.Modules.Add("", module);
            }
            foreach (var decl in declarations.OfType<CallableDeclaration>())
            {
                var ft = (SerializedSignature) decl.Accept(typeImporter);
                if (decl.Body is not InlineMachineCode inline)
                    continue;
                var ici = new InlineCodeInterpreter(constants);
                var syscall =  ici.BuildSystemCallFromMachineCode(decl.Name, ft, inline.Opcodes);
                if (syscall is not null)
                {
                    var svc = syscall.Build(platform, typelib);
                    PostProcessSignature(platform, svc);
                    if (!module.ServicesByVector.TryGetValue(svc.SyscallInfo!.Vector, out List<SystemService>? svcs))
                    {
                        svcs = new List<SystemService>();
                        module.ServicesByVector.Add(svc.SyscallInfo.Vector, svcs);
                    }
                    svcs.Add(svc);
                }
            }
        }

        private void PostProcessSignature(IPlatform platform, SystemService svc)
        {
            var parameters = svc.Signature!.Parameters!;
            for (int i = 0; i < parameters.Length; ++i)
            {
                var p = parameters[i];
                var at = p.DataType.ResolveAs<Reko.Core.Types.ArrayType>();
                if (at is not null && (at.IsUnbounded || at.Length <= 1))
                {
                    // Size 1 arrays are commonly used as boundless arrays in signatures.
                    var dtNew = new Core.Types.Pointer(at.ElementType, platform.PointerType.BitSize);
                    Storage stgNew = CreateStorage(dtNew, p.Storage);
                    p = new Identifier(p.Name, dtNew, stgNew);
                    parameters[i] = p;
                }
                var st = p.DataType.ResolveAs<Core.Types.StructureType>();
                if (st is not null)
                {
                    st.Size = st.GetInferredSize(); 
                }
            }
        }

        private Storage CreateStorage(Core.Types.DataType dt, Storage stg)
        {
            switch (stg)
            {
            case RegisterStorage reg:
                return reg;
            case StackStorage stk:
                return new StackStorage(stk.StackOffset, dt);
            }
            throw new NotImplementedException();
        }

        private Dictionary<string,Expression> EvaluateConstants(IEnumerable<Declaration> decls)
        {
            var evaluated = new Dictionary<string, Expression>
            {
                { "nil", Constant.Word32(0) }
            };

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
            if (declaration is null)
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
            //        if (ssig.ReturnValue is not null)
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
