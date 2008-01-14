/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler.Core;
using System;
using System.Xml.Serialization;

namespace Decompiler.Core.Serialization
{
	public class SerializedService
	{
		public SerializedService()
		{
		}

		[XmlAttribute("name")]
		public string Name;

		[XmlElement("syscallinfo")]
		public SerializedSyscallInfo SyscallInfo;

		[XmlElement("signature")]
		public SerializedSignature Signature;

		[XmlElement("characteristics")]
		public ProcedureCharacteristics Characteristics;

		public SystemService Build(IProcessorArchitecture arch)
		{
			SystemService svc = new SystemService();
			svc.Name = Name;
			svc.SyscallInfo = new SyscallInfo();
			svc.SyscallInfo.Vector = Convert.ToInt32(SyscallInfo.Vector, 16);
			if (SyscallInfo.RegisterValues != null)
			{
				svc.SyscallInfo.RegisterValues = new RegValue[SyscallInfo.RegisterValues.Length];
				for (int i = 0; i < SyscallInfo.RegisterValues.Length; ++i)
				{
					svc.SyscallInfo.RegisterValues[i] = new RegValue();
					svc.SyscallInfo.RegisterValues[i].Register = arch.GetRegister(SyscallInfo.RegisterValues[i].Register);
					svc.SyscallInfo.RegisterValues[i].Value = Convert.ToInt32(SyscallInfo.RegisterValues[i].Value, 16);
				}
			}
			else
			{
				svc.SyscallInfo.RegisterValues = new RegValue[0];
			}
			SignatureSerializer sser = new SignatureSerializer(arch, "stdapi");
			svc.Signature = sser.Deserialize(Signature, new Frame(null));
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
