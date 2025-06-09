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

using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Helper class that serializes and deserializes procedures with their signatures.
    /// </summary>
    public sealed class ProcedureSerializer
	{
        private readonly IPlatform platform;
        private ArgumentDeserializer? argDeser;

        /// <summary>
        /// Constructs an instance of the <see cref="ProcedureSerializer"/> class.
        /// </summary>
        /// <param name="platform"><see cref="IPlatform"/> instance to use.</param>
        /// <param name="typeLoader"><see cref="ISerializedTypeVisitor{DataType}"/> instance used to load data types.</param>
        /// <param name="defaultConvention">Default calling convention name.</param>
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

        /// <summary>
        /// Default processor architecture to use.
        /// </summary>
        public IProcessorArchitecture Architecture { get; }

        /// <summary>
        /// This object is used to deserialize serialized types.
        /// </summary>
        public ISerializedTypeVisitor<DataType> TypeLoader { get; }

        /// <summary>
        /// The name of the default calling convention on this platform.
        /// </summary>
        public string DefaultConvention { get; set; }

        /// <summary>
        /// Current FPU stack offset.
        /// </summary>
        public int FpuStackOffset { get; set; }

        /// <summary>
        /// True if FPU stack is growing downwards.
        /// </summary>
        public bool FpuStackGrowing { get; set; }

        /// <summary>
        /// True if FPU stack is growing upwards.
        /// </summary>
        public bool FpuStackShrinking => !FpuStackGrowing;

        /// <summary>
        /// Current stack offset.
        /// </summary>
        public int StackOffset { get; set; }

        /// <summary>
        /// True if the procedure signature being deserialized is variadic.
        /// </summary>
        public bool IsVariadic { get; set; }

        /// <summary>
        /// Applies the serialized signature <paramref name="ssig"/> to the 
        /// <see cref="FunctionType"/> <paramref name="sig"/>.
        /// </summary>
        /// <param name="ssig">Serialized signature whose properties are to be 
        /// applied to the function type.</param>
        /// <param name="sig">The function type to receive the changes.
        /// </param>
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
                sig.ReturnAddressOnStack = Architecture.ReturnAddressOnStack;   //$BUG: x86 real mode?
        }

        private bool HasExplicitStorage(Argument_v1 sArg)
        {
            return sArg.Kind is not null;
        }

        /// <summary>
        /// Deserializes the signature <paramref name="ss"/>. Any instantiated
        /// registers or stack variables are introduced into the Frame.
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public FunctionType? Deserialize(SerializedSignature? ss, Frame frame)
        {
            if (ss is null)
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
            var outputs = new List<Identifier>();

            if (UseUserSpecifiedStorages(ss))
            {
                this.argDeser = new ArgumentDeserializer(
                    this,
                    Architecture,
                    frame,
                    retAddrSize,
                    Architecture.WordWidth.Size);

                if (ss.Arguments is not null)
                {
                    FpuStackGrowing = true;
                    for (int iArg = 0; iArg < ss.Arguments.Length; ++iArg)
                    {
                        var sArg = ss.Arguments[iArg];
                        var arg = DeserializeArgument(sArg, ss.Convention);
                        if (arg is not null)
                        {
                            if (sArg.OutParameter)
                                outputs.Add(arg);
                            else
                                parameters.Add(arg);
                        }
                    }
                }
                Identifier? ret = null;
                if (ss.ReturnValue is not null)
                {
                    FpuStackGrowing = false;
                    ret = DeserializeArgument(ss.ReturnValue, "");
                }
                ret ??= new Identifier("", VoidType.Instance, null!);
                outputs.Insert(0, ret);

                FpuStackOffset = -FpuStackOffset;
                var sig = FunctionType.CreateUserDefined(
                    parameters.ToArray(), outputs.ToArray());
                sig.IsVariadic = this.IsVariadic;
                sig.IsInstanceMetod = ss.IsInstanceMethod;
                ApplyConvention(ss, sig);
                return sig;
            }
            else
            {
                var dtRet = ss.ReturnValue is not null && ss.ReturnValue.Type is not null
                    ? ss.ReturnValue.Type.Accept(TypeLoader)
                    : null;
                var dtThis = ss.EnclosingType is not null
                    ? new Pointer(ss.EnclosingType.Accept(TypeLoader), Architecture.PointerType.BitSize)
                    : null;
                var dtParameters = ss.Arguments is not null
                    ? ss.Arguments
                        .TakeWhile(p => p.Name != "...")
                        .Select(DeserializeParameter)
                        .ToList()
                    : new List<DataType>();

                // A calling convention governs the storage of a the 
                // parameters

                var cc = platform.GetCallingConvention(ss.Convention);
                if (cc is null)
                {
                    // It was impossible to determine a calling convention,
                    // so we don't know how to decode this signature accurately.
                    return new FunctionType
                    {
                        StackDelta = ss.StackDelta,
                        ReturnAddressOnStack = retAddrSize,
                    };
                }
                var res = new CallingConventionBuilder();
                cc.Generate(res, retAddrSize, dtRet, dtThis, dtParameters);
                if (res.Return is not null)
                {
                    var ret = new Identifier(
                        res.Return is RegisterStorage retReg ? retReg.Name : "",
                        dtRet ?? VoidType.Instance,
                        res.Return);
                    outputs.Add(ret);
                }
                if (res.ImplicitThis is not null)
                {
                    var param = new Identifier("this", dtThis!, res.ImplicitThis);
                    parameters.Add(param);
                }
                bool isVariadic = false;
                if (ss.Arguments is not null)
                {
                    for (int i = 0; i < ss.Arguments.Length; ++i)
                    {
                        if (ss.Arguments[i].Name == "...")
                        {
                            isVariadic = true;
                        }
                        else
                        {
                            var name = GenerateParameterName(ss.Arguments[i].Name, dtParameters[i], res.Parameters[i]);
                            var param = new Identifier(name, dtParameters[i], res.Parameters[i]);
                            parameters.Add(param);
                        }
                    }
                }
                var ft = FunctionType.CreateUserDefined(
                    parameters.ToArray(), outputs.ToArray());
                ft.IsInstanceMetod = ss.IsInstanceMethod;
                ft.StackDelta = ss.StackDelta != 0
                        ? ss.StackDelta
                        : res.StackDelta;
                ft.FpuStackDelta = res.FpuStackDelta;
                ft.ReturnAddressOnStack = retAddrSize;
                ft.IsVariadic = isVariadic;
                return ft;
            }
        }

        private DataType DeserializeParameter(Argument_v1 parameter, int arg2)
        {
            var dtParam = parameter.Type!.Accept(TypeLoader);
            if (dtParam is TypeReference tref && tref.Referent is FunctionType)
            {
                // A declaration of a parameter as “function returning type”
                // shall be adjusted to “pointer to function returning type”,
                // as in 6.3.2.1.
                return new Pointer(tref.Referent, platform.PointerType.BitSize);
            }
            return dtParam;
        }

        /// <summary>
        /// If the user has specified the storage on all parameters, and the
        /// return value, no heed is taken of any calling convention.
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        private bool UseUserSpecifiedStorages(SerializedSignature ss)
        {
            if (ss.Arguments is not null && !ss.Arguments.All(HasExplicitStorage))
                return false;
            if (ss.ReturnValue is not null && ss.ReturnValue.Type is not null &&
                !(ss.ReturnValue.Type is VoidType_v1) && !HasExplicitStorage(ss.ReturnValue))
                return false;
            if (ss.EnclosingType is not null)
                return false;
            return true;
        }

        private static string GenerateParameterName(string? name, DataType dataType, Storage storage)
        {
            if (!string.IsNullOrEmpty(name))
                return name!;
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

        /// <summary>
        /// Serializes a function signature to a <see cref="SerializedSignature"/>".
        /// </summary>
        /// <param name="sig">Functn signature to serialize.</param>
        /// <returns>Its serialized counterpart.</returns>
        public SerializedSignature Serialize(FunctionType sig)
        {
            SerializedSignature ssig = new SerializedSignature();
            if (!sig.ParametersValid)
            {
                ssig.ParametersValid = false;
                return ssig;
            }
            var parameters = sig.Parameters!;
            var argSer = new ArgumentSerializer(Architecture);
            ssig.ReturnValue = ArgumentSerializer.Serialize(sig.Outputs[0], false);
            ssig.Arguments = new Argument_v1[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                Identifier formal = parameters[i];
                ssig.Arguments[i] = ArgumentSerializer.Serialize(formal, false)!;
            }
            for (int i = 1; i < sig.Outputs.Length; ++i)
            {
                Identifier formal = sig.Outputs[i];
                ssig.Arguments[i] = ArgumentSerializer.Serialize(formal, true)!;
            }
            ssig.StackDelta = sig.StackDelta;
            ssig.FpuStackDelta = sig.FpuStackDelta;
            ssig.ReturnAddressOnStack = sig.ReturnAddressOnStack;
            return ssig;
        }

        /// <summary>
        /// Serlizes information about a procedure.
        /// </summary>
        /// <param name="proc"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
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
        /// <param name="arg">Argument to deserialize</param>
        /// <param name="convention"></param>
        /// <returns></returns>
        public Identifier? DeserializeArgument(Argument_v1 arg, string? convention)
        {
            var kind = arg.Kind;
            if (kind is null)
            {
                kind = new StackVariable_v1();
            }
            return argDeser!.Deserialize(arg, kind);
        }
    }
}