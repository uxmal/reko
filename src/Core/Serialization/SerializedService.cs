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
using System;
using System.Linq;
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
	public class SerializedService : ProcedureBase_v1
	{
		public SerializedService()
		{
		}

        /// <summary>
        /// If non-null, specifies the address of a dispatcher function.
        /// </summary>
        [XmlAttribute("address")]
        public string Address;

        /// <summary>
        /// Describes what registers must be set at what values to call this
        /// system call.
        /// </summary>
		[XmlElement("syscallinfo")]
		public SyscallInfo_v1 SyscallInfo;

		public SystemService Build(IPlatform platform, TypeLibrary library)
		{
			SystemService svc = new SystemService();
			svc.Name = Name;
			svc.SyscallInfo = new SyscallInfo();
            svc.SyscallInfo.Vector = SyscallInfo != null
                ? Convert.ToInt32(SyscallInfo.Vector, 16)
                : this.Ordinal;
            if (SyscallInfo != null)
            {
                if (SyscallInfo.RegisterValues != null)
                {
                    svc.SyscallInfo.RegisterValues = new RegValue[SyscallInfo.RegisterValues.Length];
                    for (int i = 0; i < SyscallInfo.RegisterValues.Length; ++i)
                    {
                        svc.SyscallInfo.RegisterValues[i] = new RegValue
                        {
                            Register = platform.Architecture.GetRegister(SyscallInfo.RegisterValues[i].Register),
                            Value = Convert.ToInt32(SyscallInfo.RegisterValues[i].Value, 16),
                        };
                    }
                } 
                if (SyscallInfo.StackValues != null)
                {
                    svc.SyscallInfo.StackValues = SyscallInfo.StackValues.Select(sv =>
                        new StackValue
                        {
                            Offset = Convert.ToInt32(sv.Offset, 16),
                            Value = Convert.ToInt32(sv.Value, 16)
                        }).ToArray();
                }
            }
			if (svc.SyscallInfo.RegisterValues == null)
			{
				svc.SyscallInfo.RegisterValues = new RegValue[0];
			}
            TypeLibraryDeserializer loader = new TypeLibraryDeserializer(platform, true, library);
			var sser = new ProcedureSerializer(platform, loader, "stdapi");
            svc.Signature = sser.Deserialize(Signature, platform.Architecture.CreateFrame());
			svc.Characteristics = Characteristics != null ? Characteristics : DefaultProcedureCharacteristics.Instance;
			return svc;
		}
	}

	public class SyscallInfo_v1
	{
		[XmlElement("vector")]
		public string Vector;

		[XmlElement("regvalue")]
		public SerializedRegValue [] RegisterValues;

        [XmlElement("stackvalue")]
        public StackValue_v1[] StackValues;
    }

    [Obsolete("use RegisterValue_v2")]
	public class SerializedRegValue
	{
		[XmlAttribute("reg")]
		public string Register;

		[XmlText]
		public string Value;

		public SerializedRegValue()
		{
		}

		public SerializedRegValue(string reg, string val)
		{
			Register = reg;
			Value = val;
		}
	}

    public class StackValue_v1
    {
        [XmlAttribute("offset")]
        public string Offset;

        [XmlText]
        public string Value;
    }
}
