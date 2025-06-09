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
using System.Linq;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
    /// <summary>
    /// Serialized representation of a system service.
    /// </summary>
	public class SerializedService : ProcedureBase_v1
	{
        /// <summary>
        /// If non-null, specifies the address of a dispatcher function.
        /// </summary>
        [XmlAttribute("address")]
        public string? Address;

        /// <summary>
        /// Describes what registers must be set at what values to call this
        /// system call.
        /// </summary>
		[XmlElement("syscallinfo")]
		public SyscallInfo_v1? SyscallInfo;

        /// <summary>
        /// Build a <see cref="SystemService"/> instance from its
        /// serialized representation.
        /// </summary>
        /// <param name="platform"><see cref="IPlatform"/> in question.</param>
        /// <param name="library">Typelibrary to consult for type references.</param>
        /// <returns>The deserialized <see cref="SystemService"/>.</returns>
		public SystemService Build(IPlatform platform, TypeLibrary library)
		{
			SystemService svc = new SystemService();
			svc.Name = Name;
            if (this.SyscallInfo is not null)
            {
                var syscallinfo = this.SyscallInfo.Build(platform);
                svc.SyscallInfo = syscallinfo;
            }
            else
            {
                svc.SyscallInfo = new SyscallInfo
                {
                    Vector = this.Ordinal
                };
            }
            if (svc.SyscallInfo.RegisterValues is null)
			{
				svc.SyscallInfo.RegisterValues = Array.Empty<RegValue>();
			}
            var loader = new TypeLibraryDeserializer(platform, true, library);
			var sser = new ProcedureSerializer(platform, loader, "stdapi");
            svc.Signature = sser.Deserialize(Signature, platform.Architecture.CreateFrame());
			svc.Characteristics = Characteristics ?? DefaultProcedureCharacteristics.Instance;
			return svc;
		}
    }

    /// <summary>
    /// Serialized representation of a system call info.
    /// </summary>
	public class SyscallInfo_v1
	{
        /// <summary>
        /// Interrupt vector number, expressed as a hexadecimal string.
        /// </summary>
		[XmlElement("vector")]
		public string? Vector;

        /// <summary>
        /// Required register values for this system call.
        /// </summary>
		[XmlElement("regvalue")]
		public SerializedRegValue[]? RegisterValues;

        /// <summary>
        /// Required values for this system call.
        /// </summary>
        [XmlElement("stackvalue")]
        public StackValue_v1[]? StackValues;

        /// <summary>
        /// Build a <see cref="SystemService"/> instance from its
        /// serialized representation.
        /// </summary>
        /// <param name="platform"><see cref="IPlatform"/> in question.</param>
        /// <returns>The deserialized <see cref="SystemService"/>.</returns>
        public SyscallInfo Build(IPlatform platform)
        {
            var syscallinfo = new SyscallInfo();
            syscallinfo.Vector = Convert.ToInt32(this.Vector, 16);
            if (this.RegisterValues is not null)
            {
                syscallinfo.RegisterValues = new RegValue[this.RegisterValues.Length];
                for (int i = 0; i < this.RegisterValues.Length; ++i)
                {
                    var regName = this.RegisterValues[i].Register;
                    if (regName is not null)
                    {
                        syscallinfo.RegisterValues[i] = new RegValue
                        {
                            Register = platform.Architecture.GetRegister(regName),
                            Value = Convert.ToInt32(this.RegisterValues[i].Value, 16),
                        };
                    }
                }
            }
            if (this.StackValues is not null)
            {
                syscallinfo.StackValues = this.StackValues.Select(sv =>
                    new StackValue
                    {
                        Offset = Convert.ToInt32(sv.Offset, 16),
                        Value = Convert.ToInt32(sv.Value, 16)
                    }).ToArray();
            }
            return syscallinfo;
        }
    }

    /// <summary>
    /// Serialized register value.
    /// </summary>
    public class SerializedRegValue
	{
        /// <summary>
        /// Register name.
        /// </summary>
		[XmlAttribute("reg")]
		public string? Register;

        /// <summary>
        /// Register value expressed as a hexadecimal string.
        /// </summary>
		[XmlText]
		public string? Value;

        /// <summary>
        /// Create an uninitialized instance of <see cref="SerializedRegValue"/>.
        /// </summary>
		public SerializedRegValue()
		{
		}

        /// <summary>
        /// Create an initialized instance of <see cref="SerializedRegValue"/>.
        /// </summary>
        /// <param name="reg">Register name.</param>
        /// <param name="val">Register value expressed as a hexadecimal string.
        /// </param>
		public SerializedRegValue(string reg, string val)
		{
			Register = reg;
			Value = val;
		}
	}

    /// <summary>
    /// Serialized stackvalue.
    /// </summary>
    public class StackValue_v1
    {
        /// <summary>
        /// Offset of the stack value, expressed as a hexadecimal string.
        /// </summary>
        [XmlAttribute("Offset")]
        public string? Offset;

        /// <summary>
        /// Value, expressed as a hexadecimal string.
        /// </summary>
        [XmlText]
        public string? Value;
    }
}
