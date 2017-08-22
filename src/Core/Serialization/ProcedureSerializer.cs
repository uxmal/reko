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

using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;

namespace Reko.Core.Serialization
{
	/// <summary>
	/// Helper class that serializes and deserializes procedures with their signatures.
	/// </summary>
	public sealed class ProcedureSerializer
	{
        private IPlatform platform;
        private ArgumentDeserializer argDeser;

        public ProcedureSerializer(
            IPlatform platform, 
            ISerializedTypeVisitor<DataType> typeLoader, 
            string defaultConvention)
		{
            this.platform = platform;
			this.Architecture = platform.Architecture;
            this.TypeLoader = typeLoader;
			this.DefaultConvention = defaultConvention;
		}

        public IProcessorArchitecture Architecture { get; private set; }
        public ISerializedTypeVisitor<DataType> TypeLoader { get; private set; }
        public string DefaultConvention { get; set; }

        public int FpuStackOffset { get; set; }
        public int StackOffset { get; set; }

        public void ApplyConvention(SerializedSignature ssig, FunctionType sig)
        {
            sig.StackDelta = Architecture.PointerType.Size;  //$BUG: far/near pointers?
            if (ssig.StackDelta != 0)
                sig.StackDelta = ssig.StackDelta;
            else
                sig.StackDelta = Architecture.PointerType.Size;   //$BUG: x86 real mode?
            sig.FpuStackDelta = FpuStackOffset;
            sig.ReturnAddressOnStack = Architecture.PointerType.Size;   //$BUG: x86 real mode?
        }

        public Identifier CreateId(string name, DataType type, Storage storage)
		{
			return new Identifier(name, type, storage);
		}

        private bool HasExplicitStorage(Argument_v1 sArg)
        {
            return sArg.Kind != null;
        }

        /// <summary>
        /// Deserializes the signature <paramref name="ss"/>. Any instantiated
        /// registers or stack variables are introduced into the Frame.
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public FunctionType Deserialize(SerializedSignature ss, Frame frame)
        {
            if (ss == null)
                return null;
            var retAddrSize = this.Architecture.PointerType.Size;   //$TODO: deal with near/far calls in x86-realmode
            if (!ss.ParametersValid)
            {
                return new FunctionType
                {
                    StackDelta = ss.StackDelta,
                    ReturnAddressOnStack = retAddrSize,
                };
            }

            this.argDeser = new ArgumentDeserializer(
                this,
                Architecture,
                frame,
                retAddrSize,
                Architecture.WordWidth.Size);

            var parameters = new List<Identifier>();

            Identifier ret = null;
            // If the user has specified the storage on all
            // parameters, and the return value, no heed is taken of any calling 
            // convention.
            if ((ss.Arguments == null || ss.Arguments.All(HasExplicitStorage)) &&
                (ss.ReturnValue == null || ss.ReturnValue.Type == null ||
                 ss.ReturnValue.Type is VoidType_v1 || HasExplicitStorage(ss.ReturnValue)))
            {
                int fpuDelta = FpuStackOffset;
                FpuStackOffset = 0;
                if (ss.ReturnValue != null)
                {
                    ret = DeserializeArgument(ss.ReturnValue, -1, "");
                    fpuDelta += FpuStackOffset;
                }

                FpuStackOffset = 0;
                if (ss.Arguments != null)
                {
                    for (int iArg = 0; iArg < ss.Arguments.Length; ++iArg)
                    {
                        var sArg = ss.Arguments[iArg];
                        var arg = DeserializeArgument(sArg, iArg, ss.Convention);
                        parameters.Add(arg);
                    }
                }
                fpuDelta -= FpuStackOffset;
                FpuStackOffset = fpuDelta;
                var sig = new FunctionType(ret, parameters.ToArray())
                {
                    IsInstanceMetod = ss.IsInstanceMethod,
                };
                ApplyConvention(ss, sig);
                return sig;
            }
            else
            {
                var dtRet = ss.ReturnValue != null
                    ? ss.ReturnValue.Type.Accept(TypeLoader)
                    : VoidType.Instance;
                var dtThis = ss.EnclosingType != null
                    ? new Pointer(ss.EnclosingType.Accept(TypeLoader), Architecture.PointerType.Size)
                    : null;
                var dtParameters = ss.Arguments != null
                    ? ss.Arguments
                        .TakeWhile(p => p.Name != "...")
                        .Select(p => p.Type.Accept(TypeLoader))
                        .ToList()
                    : new List<DataType>();

                // A calling convention governs the storage of a the 
                // parameters

                var cc = platform.GetCallingConvention(ss.Convention);

                var res = cc.Generate(dtRet, dtThis, dtParameters);
                if (res.Return != null)
                {
                    var retReg = res.Return as RegisterStorage;
                    ret = new Identifier(retReg != null ? retReg.Name : "", dtRet, res.Return);
                }
                if (res.ImplicitThis != null)
                {
                    var param = new Identifier("this", dtThis, res.ImplicitThis);
                    parameters.Add(param);
                }
                if (ss.Arguments != null)
                {
                    for (int i = 0; i < ss.Arguments.Length; ++i)
                    {
                        if (ss.Arguments[i].Name == "...")
                        {
                            var unk = new UnknownType();
                            parameters.Add(new Identifier("...", unk, new StackArgumentStorage(0, unk)));
                        }
                        else
                        {
                            var name = GenerateParameterName(ss.Arguments[i].Name, dtParameters[i], res.Parameters[i]);
                            var param = new Identifier(name, dtParameters[i], res.Parameters[i]);
                            parameters.Add(param);
                        }
                    }
                }
                var ft = new FunctionType(ret, parameters.ToArray())
                {
                    IsInstanceMetod = ss.IsInstanceMethod,
                    StackDelta = ss.StackDelta != 0
                        ? ss.StackDelta
                        : res.StackDelta,
                    FpuStackDelta = res.FpuStackDelta,
                    ReturnAddressOnStack = retAddrSize,
                };
                return ft;
            }
        }

        private string GenerateParameterName(string name, DataType dataType, Storage storage)
        {
            if (!string.IsNullOrEmpty(name))
                return name;
            var stack = storage as StackStorage;
            if (stack != null)
                return Frame.FormatStackAccessName(dataType, "Arg", stack.StackOffset, name);
            var seq = storage as SequenceStorage;
            if (seq != null)
                return seq.Name;
            var reg = storage as RegisterStorage;
            if (reg != null)
                return reg.Name;
            throw new NotImplementedException();
        }

        private Identifier DeserializeImplicitThisArgument(SerializedSignature ss)
        {
            var sArg = new Argument_v1
            {
                Type = new PointerType_v1(ss.EnclosingType),
                Name = "this",
            };
            if (ss.Convention == "__thiscall")
            {
                sArg.Kind = new Register_v1("ecx");
            }
            else
                sArg.Kind = new StackVariable_v1();
            var arg = argDeser.Deserialize(sArg);
            return arg;
        }

        public SerializedSignature Serialize(FunctionType sig)
        {
            SerializedSignature ssig = new SerializedSignature();
            if (!sig.ParametersValid)
            {
                ssig.ParametersValid = false;
                return ssig;
            }
            ArgumentSerializer argSer = new ArgumentSerializer(Architecture);
            ssig.ReturnValue = argSer.Serialize(sig.ReturnValue);
            ssig.Arguments = new Argument_v1[sig.Parameters.Length];
            for (int i = 0; i < sig.Parameters.Length; ++i)
            {
                Identifier formal = sig.Parameters[i];
                ssig.Arguments[i] = argSer.Serialize(formal);
            }
            ssig.StackDelta = sig.StackDelta;
            ssig.FpuStackDelta = sig.FpuStackDelta;
            return ssig;
        }

        public Procedure_v1 Serialize(Procedure proc, Address addr)
        {
            Procedure_v1 sproc = new Procedure_v1();
            sproc.Address = addr.ToString();
            sproc.Name = proc.Name;
            if (proc.Signature != null)
                sproc.Signature = Serialize(proc.Signature);
            return sproc;
        }

        /// <summary>
        /// Deserializes an argument.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="idx"></param>
        /// <param name="convention"></param>
        /// <returns></returns>
        public Identifier DeserializeArgument(Argument_v1 arg, int idx, string convention)
        {
            if (arg.Kind != null)
            {
                return argDeser.Deserialize(arg, arg.Kind);
            }
            if (convention == null)
                return argDeser.Deserialize(arg, new StackVariable_v1());
            switch (convention)
            {
            case "":
            case "cdecl":
            case "pascal":
            case "stdapi":
            case "stdcall":
            case "__cdecl":
            case "__stdcall":
            case "__thiscall":
                return argDeser.Deserialize(arg, new StackVariable_v1 { });
            }
            throw new NotSupportedException(string.Format("Unsupported calling convention '{0}'.", convention));
        }
    }
}