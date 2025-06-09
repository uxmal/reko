#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
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
using Reko.Libraries.Microchip;
using Reko.Libraries.Microchip.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Reko.Arch.MicrochipPIC.Common
{

    /// <summary>
    /// A PIC device configuration register/fuse.
    /// </summary>
    public class PICDevConfigRegister
    {

        private HashSet<DevConfigField> fields;
        private List<DevConfigIllegal> illegals;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dcr">The <see cref="IDeviceFuse"/> interface instance describing the device configuration register.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="dcr"/> is null.</exception>
        public PICDevConfigRegister(IDeviceFuse dcr)
        {
            if (dcr is null)
                throw new ArgumentNullException(nameof(dcr));
            Address = Address.Ptr32((uint)dcr.Addr);
            Name = dcr.Name;
            Descr = dcr.Description;
            BitWidth = dcr.BitWidth;
            Access = dcr.AccessBits;
            Impl = dcr.ImplMask;
            DefaultValue = dcr.DefaultValue;
            fields = new HashSet<DevConfigField>();
            illegals = new List<DevConfigIllegal>();
        }

        /// <summary>
        /// Gets the program memory address of this device configuration register.
        /// </summary>
        public Address Address { get; }

        /// <summary>
        /// Gets the name of this device configuration register.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of this device configuration register.
        /// </summary>
        public string Descr { get; }

        /// <summary>
        /// Gets the total bit-width of this device configuration register.
        /// </summary>
        public int BitWidth { get; }

        /// <summary>
        /// Gets the access bits  of this device configuration register.
        /// </summary>
        public string Access { get; }

        /// <summary>
        /// Gets the implementation mask  of this device configuration register.
        /// </summary>
        public int Impl { get; }

        /// <summary>
        /// Gets the default value of this device configuration register.
        /// </summary>
        public int DefaultValue { get; }


        /// <summary>
        /// Adds a bit-field to this device configuration register.
        /// </summary>
        /// <param name="dcf">The <see cref="DevConfigField"/> instance to add.</param>
        public void AddField(DevConfigField dcf) => fields.Add(dcf);

        /// <summary>
        /// Adds an illegal condition.
        /// </summary>
        /// <param name="dill">The illegal condition descriptor.</param>
        public void AddIllegal(DevConfigIllegal dill) => illegals.Add(dill);

        /// <summary>
        /// Gets a device configuration bit-field given its position in this device configuration register.
        /// </summary>
        /// <param name="bitpos">The bit position (zero-based).</param>
        /// <returns>
        /// A <see cref="DevConfigField"/> instance or null.
        /// </returns>
        public DevConfigField? GetField(int bitpos) => fields.FirstOrDefault(f => f.BitPos == bitpos);

        /// <summary>
        /// Gets a device configuration register's bit-field given its name.
        /// </summary>
        /// <param name="name">The name of this device configuration register's bit-field.</param>
        /// <returns>
        /// A <see cref="DevConfigField"/> instance or null.
        /// </returns>
        public DevConfigField? GetField(string name) => fields.FirstOrDefault(f => f.Name == name);

        /// <summary>
        /// Enumerates the bit-fields contained in this device configuration register.
        /// </summary>
        public IEnumerable<DevConfigField> Fields => fields;

        /// <summary>
        /// Enumerates the illegal conditions for this Device Configuration Register.
        /// </summary>
        public IEnumerable<DevConfigIllegal> Illegals => illegals;

        public override string ToString() => $"{Name}@{Address}";

        public bool CheckIf(string fieldName, string fieldState, uint valueToCheck)
        {
            var dcf = GetField(fieldName);
            if (dcf is null)
                return false;
            uint val = (uint)(((valueToCheck & Impl) >> dcf.BitPos) & dcf.BitMask);
            var sem = dcf.GetSemantic(val);
            return (sem.State.ToUpper() == fieldState.ToUpper());
        }

        public bool CheckIf(string fieldName, string fieldState, int valueToCheck)
            => CheckIf(fieldName, fieldState, (uint)valueToCheck);
    }

    /// <summary>
    /// A PIC device configuration bit-field.
    /// </summary>
    public class DevConfigField
    {

        private List<DevConfigSemantic> semantics;


        public DevConfigField(IDeviceFusesField dcrfield, Address regAddr)
        {
            if (dcrfield is null)
                throw new ArgumentNullException(nameof(dcrfield));
            Name = dcrfield.Name;
            Descr = dcrfield.Description;
            RegAddress = regAddr;
            BitPos = dcrfield.BitPos;
            BitWidth = dcrfield.BitWidth;
            BitMask = dcrfield.BitMask;
            semantics = new List<DevConfigSemantic>();
        }


        /// <summary>
        /// Gets the name of the device configuration bit-field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the description of the device configuration bit-field.
        /// </summary>
        public string Descr { get; }

        /// <summary>
        /// Gets the configuration register address.
        /// </summary>
        public Address RegAddress { get; }

        /// <summary>
        /// Gets the bit-width of the device configuration bit-field.
        /// </summary>
        public int BitWidth { get; }

        /// <summary>
        /// Gets the bit position of the device configuration bit-field.
        /// </summary>
        public int BitPos { get; }

        /// <summary>
        /// Gets the bit mask of the device configuration bit-field.
        /// </summary>
        public int BitMask { get; }


        /// <summary>
        /// Adds a semantic to this device configuration bit field.
        /// </summary>
        /// <param name="dcrsem">The <see cref="DevConfigSemantic"/> semantic instance.</param>
        public void AddSemantic(DevConfigSemantic dcrsem)
        {
            semantics.Add(dcrsem);
        }

        /// <summary>
        /// Enumerates the semantics of this device configuration bit field.
        /// </summary>
        public IEnumerable<DevConfigSemantic> Semantics => semantics;

        /// <summary>
        /// Gets the semantic that matches the given bit field value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The semantic or null.
        /// </returns>
        public DevConfigSemantic GetSemantic(uint value)
        {
            var sem = semantics.FirstOrDefault(s => s.Match(value));
            if (sem is null)
                return DevConfigSemantic.invalid;
            return sem;
        }

        public override string ToString() => $"{Name}@{RegAddress}.b{BitPos}[{BitWidth}]";


    }

    /// <summary>
    /// Semantic associated with a value of a PIC device configuration bit-field.
    /// </summary>
    public class DevConfigSemantic
    {

        // 'When' expression in PIC XML is of the form: "(field mask 0xNN) op 0xHH"
        // with 'mask' being either the AND (&) operator or the OR (|) operator and
        // with 'op' being one of the comparison operators (==, !=, <, <=, etc...).
        // So we isolate (1)the mask operator, (2)the mask value, (3)the comparison operator and (4)the expected result.
        // 
        private const string pattern = @"^\(\s*field\s+([^ ]*)\s+([^ ]*)\s*\)\s+([^ ]*)\s+([^ ]*)$";

        private sealed class InvalidFieldSem : IDeviceFusesSemantic
        {
            public string? Name { get; set; }

            public string? Description { get; set; }

            public string? When { get; set; }

            public bool IsHidden => false;

            public bool IsLangHidden => false;

        }

        /// <summary>
        /// The semantic in case the bit-field value does not correspond to a known value.
        /// </summary>
        public static DevConfigSemantic invalid =
            new DevConfigSemantic(new InvalidFieldSem() { Name = "<invalid>", Description = "Invalid fuse value", When = "?" });

        private DevConfigSemantic()
        {
            State = null!;
            Descr = null!;
            When = null!;
        }

        public DevConfigSemantic(IDeviceFusesSemantic dcrsem)
        {
            State = dcrsem.Name;
            Descr = dcrsem.Description;
            When = dcrsem.When.Trim();
        }


        /// <summary> Gets the name of the device configuration bit-field state. </summary>
        public string State { get; }

        /// <summary> Gets the description of the device configuration bit-field state. </summary>
        public string Descr { get; }

        /// <summary> Gets the 'when' condition corresponding to the device configuration bit-field state. </summary>
        public string When { get; }


        /// <summary>
        /// Indicates whenever the given value corresponds to a valid device configuration bit-field state.
        /// </summary>
        /// <param name="value">The bit-field value.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        public bool Match(uint value)
        {
            var m = Regex.Match(When, pattern);
            if (!m.Success)
                return false;
            if (m.Groups.Count != 5)
                throw new InvalidOperationException($"Undecipherable when-pattern: '{When}'.");
            return EvaluateWhen(value, m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value);
        }

        private static bool EvaluateWhen(uint iValueToTest, string whenOper, string whenMask, string whenCompare, string expectedResult)
        {
            uint mask = (uint)whenMask.ToInt32Ex();
            int expectValue = expectedResult.ToInt32Ex();
            if (whenOper == "&")
                iValueToTest &= mask;
            if (whenOper == "|")
                iValueToTest |= mask;
            if (whenCompare == "==")
                return (iValueToTest == expectValue);
            if (whenCompare == "!=")
                return (iValueToTest != expectValue);
            if (whenCompare == ">")
                return (iValueToTest > expectValue);
            if (whenCompare == ">=")
                return (iValueToTest >= expectValue);
            if (whenCompare == "<=")
                return (iValueToTest <= expectValue);
            if (whenCompare == "<")
                return (iValueToTest < expectValue);
            return false;
        }

        public override string ToString()
            => $"When '{When}' then fuse={State}, meaning '{Descr}'";

    }

    /// <summary>
    /// Represents an illegal condition for a Device Configuration Register's content.
    /// </summary>
    public class DevConfigIllegal
    {

        // 'When' expression in PIC XML is of the form: "(reg mask 0xNN) op 0xHH"
        // with 'mask' being either the AND (&) operator or the OR (|) operator and
        // with 'op' being one of the comparison operators (==, !=, <, <=, etc...).
        // So we isolate (1)the mask operator, (2)the mask value, (3)the comparison operator and (4)the expected result.
        // 
        private const string pattern = @"^\(\s*reg\s+([^ ]*)\s+([^ ]*)\s*\)\s+([^ ]*)\s+([^ ]*)$";


        public DevConfigIllegal(IDeviceFusesIllegal dcrill)
        {
            When = dcrill.When;
            Descr = dcrill.Description;
        }


        /// <summary>
        /// Gets the description of the illegal device configuration register state.
        /// </summary>
        public string Descr { get; }

        /// <summary>
        /// Gets the 'when' condition corresponding to illegal state.
        /// </summary>
        public string When { get; }


        /// <summary>
        /// Indicates whenever the given value corresponds to an invalid device configuration register state.
        /// </summary>
        /// <param name="value">The register value.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        public bool Match(uint value)
        {
            var m = Regex.Match(When, pattern);
            if (!m.Success)
                return false;
            if (m.Groups.Count != 5)
                throw new InvalidOperationException($"Undecipherable when-pattern: '{When}'.");
            return EvaluateWhen(value, m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value);
        }

        private static bool EvaluateWhen(uint iValueToTest, string whenOper, string whenMask, string whenCompare, string expectedResult)
        {
            uint mask = (uint)whenMask.ToInt32Ex();
            int expectValue = expectedResult.ToInt32Ex();
            if (whenOper == "&")
                iValueToTest &= mask;
            if (whenOper == "|")
                iValueToTest |= mask;
            if (whenCompare == "==")
                return (iValueToTest == expectValue);
            if (whenCompare == "!=")
                return (iValueToTest != expectValue);
            if (whenCompare == ">")
                return (iValueToTest > expectValue);
            if (whenCompare == ">=")
                return (iValueToTest >= expectValue);
            if (whenCompare == "<=")
                return (iValueToTest <= expectValue);
            if (whenCompare == "<")
                return (iValueToTest < expectValue);
            return false;
        }

        public override string ToString()
            => $"When '{When}' then '{Descr}'";

    }

}
