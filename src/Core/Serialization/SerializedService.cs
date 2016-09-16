#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System.Xml.Serialization;

namespace Reko.Core.Serialization
{
	public class SerializedService : ProcedureBase_v1
	{
		public SerializedService()
		{
		}

        /// <summary>
        /// Describes what registers must be set at what values to call this system call.
        /// </summary>
		[XmlElement("syscallinfo")]
		public SerializedSyscallInfo SyscallInfo;

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
            }
			if (svc.SyscallInfo.RegisterValues == null)
			{
				svc.SyscallInfo.RegisterValues = new RegValue[0];
			}
            TypeLibraryDeserializer loader = new TypeLibraryDeserializer(platform, true, library);
			var sser = platform.CreateProcedureSerializer(loader, "stdapi");
            svc.Signature = sser.Deserialize(Signature, platform.Architecture.CreateFrame());
			svc.Characteristics = Characteristics != null ? Characteristics : DefaultProcedureCharacteristics.Instance;
			return svc;
		}
	}

	public class SerializedSyscallInfo
	{
		[XmlElement("vector")]
		public string Vector;

		[XmlElement("regvalue")]
		public SerializedRegValue [] RegisterValues;
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
}
