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
        private readonly IPlatform platform;
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
            if (ssig.StackDelta != 0)
                sig.StackDelta = ssig.StackDelta;
            else
                sig.StackDelta = Architecture.PointerType.Size;
            sig.FpuStackDelta = FpuStackOffset;
            if (ssig.ReturnAddressOnStack != 0)
                sig.ReturnAddressOnStack = ssig.ReturnAddressOnStack;
            else
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
            // If there is no explict return address size,
            // use the architecture's default return address size.

            var retAddrSize = ss.ReturnAddressOnStack != 0
                ? ss.ReturnAddressOnStack
                : this.Architecture.ReturnAddressOnStack;
            if (!ss.ParametersValid)
            {
                return new FunctionType
                {
                    StackDelta = ss.StackDelta,
                    ReturnAddressOnStack = retAddrSize,
                };
            }

            var parameters = new List<Identifier>();

            Identifier ret = null;
            if (UseUserSpecifiedStorages(ss))
            {
                this.argDeser = new ArgumentDeserializer(
                    this,
                    Architecture,
                    frame,
                    retAddrSize,
                    Architecture.WordWidth.Size);

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
                var dtRet = ss.ReturnValue != null && ss.ReturnValue.Type != null
                    ? ss.ReturnValue.Type.Accept(TypeLoader)
                    : null;
                var dtThis = ss.EnclosingType != null
                    ? new Pointer(ss.EnclosingType.Accept(TypeLoader), Architecture.PointerType.BitSize)
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
                if (cc == null)
                {
                    // It was impossible to determine a calling convention,
                    // so we don't know how to decode this signature accurately.
                    return new FunctionType
                    {
                        StackDelta = ss.StackDelta,
                        ReturnAddressOnStack = retAddrSize,
                    };
                }
                var res = new CallingConventionEmitter();
                cc.Generate(res, dtRet, dtThis, dtParameters);
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

        /// <summary>
        /// If the user has specified the storage on all parameters, and the
        /// return value, no heed is taken of any calling convention.
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        private bool UseUserSpecifiedStorages(SerializedSignature ss)
        {
            if (ss.Arguments != null && !ss.Arguments.All(HasExplicitStorage))
                return false;
            if (ss.ReturnValue != null && ss.ReturnValue.Type != null &&
                !(ss.ReturnValue.Type is VoidType_v1) && !HasExplicitStorage(ss.ReturnValue))
                return false;
            if (ss.EnclosingType != null)
                return false;
            return true;
        }

        private string GenerateParameterName(string name, DataType dataType, Storage storage)
        {
            if (!string.IsNullOrEmpty(name))
                return name;
            switch (storage)
            {
            case RegisterStorage reg:
                return reg.Name;
            case StackStorage stack:
                return NamingPolicy.Instance.StackArgumentName(dataType, stack.StackOffset, name);
            case SequenceStorage seq:
                return seq.Name;
            default:
                throw new NotImplementedException();
            }
        }

        public SerializedSignature Serialize(FunctionType sig)
        {
            SerializedSignature ssig = new SerializedSignature();
            if (!sig.ParametersValid)
            {
                ssig.ParametersValid = false;
                return ssig;
            }
            var argSer = new ArgumentSerializer(Architecture);
            ssig.ReturnValue = argSer.Serialize(sig.ReturnValue);
            ssig.Arguments = new Argument_v1[sig.Parameters.Length];
            for (int i = 0; i < sig.Parameters.Length; ++i)
            {
                Identifier formal = sig.Parameters[i];
                ssig.Arguments[i] = argSer.Serialize(formal);
            }
            ssig.StackDelta = sig.StackDelta;
            ssig.FpuStackDelta = sig.FpuStackDelta;
            ssig.ReturnAddressOnStack = sig.ReturnAddressOnStack;
            return ssig;
        }

        public Procedure_v1 Serialize(Procedure proc, Address addr)
        {
            var sproc = new Procedure_v1
            {
                Address = addr.ToString(),
                Name = proc.Name,
                Signature = Serialize(proc.Signature),
            };
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
            var kind = arg.Kind;
            if (kind == null)
            {
                kind = new StackVariable_v1();
            }
            return argDeser.Deserialize(arg, kind);
        }
    }
}