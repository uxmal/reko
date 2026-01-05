#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System.Globalization;

namespace Reko.Core.Machine
{
    /// <summary>
    /// Abstraction of a processor instruction operand.
    /// </summary>
	public interface MachineOperand
	{
        /// <summary>
        /// The data type of this MachineOperand.
        /// </summary>
        DataType DataType { get; set; }

        /// <summary>
        /// Renders the operand as a string, according to the specified
        /// <paramref name="options"/>.
        /// </summary>
        /// <param name="options">Options that control the rendering 
        /// of the machine operand.</param>
        string ToString(MachineInstructionRendererOptions options);

        /// <summary>
        /// Renders the operand to a <see cref="MachineInstructionRenderer" />,
        /// according to the specified <paramref name="options" />.
        /// </summary>
        public void Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options);
    }

    /// <summary>
    /// Abstract base class for implementing machine operands.
    /// </summary>
    public abstract class AbstractMachineOperand : MachineOperand
	{
        /// <inheritdoc/>
        public virtual DataType DataType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractMachineOperand"/> class.
        /// </summary>
        /// <param name="width"></param>
		protected AbstractMachineOperand(DataType width)
		{
			this.DataType = width;
		}

        /// <inheritdoc/>
        public sealed override string ToString()
        {
            return ToString(MachineInstructionRendererOptions.Default);
        }

        /// <inheritdoc/>
		public string ToString(MachineInstructionRendererOptions options)
		{
            var sr = new StringRenderer();
            Render(sr, options);
			return sr.ToString();
		}

        /// <inheritdoc/>
        public void Render(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options)
        {
            renderer.BeginOperand();
            DoRender(renderer, options);
            renderer.EndOperand();
        }

        /// <summary>
        /// Derived classes implement this method to render the operand.
        /// </summary>
        /// <param name="renderer">Output sink.</param>
        /// <param name="options">Rendering options.</param>
        protected abstract void DoRender(MachineInstructionRenderer renderer, MachineInstructionRendererOptions options);

        /// <summary>
        /// Converts a signed integer constant to string representatioon.
        /// </summary>
        /// <param name="c">Constant to render into a string.</param>
        /// <param name="forceSign">Should signed integers always be
        /// formatted with a leading sign character? Setting this to true
        /// allows chaining a sequence of numbers into an expression,
        /// like "+5+7-3+9". Setting this to false will format numbers like
        /// "5" and "-3" which is normal for standalone numbers.</param>
        /// <param name="format">Format string; allows injecting platform-
        /// specific characters before/between/after sign and value when
        /// printing an integer value as hex. {0} will be the sign character
        /// (if any) and {1} will be the absolute value.
        /// </param>
        public static string FormatSignedValue(Constant c, bool forceSign = true, string format = "{0}{1}")
        {
            string s = (forceSign ? "+" : "");
            int tmp = c.ToInt32();
            if (tmp < 0)
            {
                s = "-";
                tmp = -tmp;
            }
            return string.Format(format, s, tmp.ToString(FormatString(c.DataType)));
        }

        private static readonly char[] floatSpecials = new char[] { '.', 'e', 'E' };

        /// <summary>
        /// Converts a numeric constant to string representatioon.
        /// </summary>
        /// <param name="c"> Constant to render into a string.</param>
        /// <param name="forceSignForSignedIntegers">sign Should signed
        /// integers always be formatted with a leading sign character?
        /// Setting this to true allows chaining a sequence of numbers into an
        /// expression,like "+5+7-3+9". Setting this to false will format
        /// numbers like "5" and "-3" which is normal for standalone numbers.
        /// </param>
        /// <param name="integerFormat">Format string; allows injecting 
        /// platform-specific characters before/between/after sign and
        /// value when printing an integer value as hex. {0} will be the sign
        /// character (if any) and {1} will be the absolute value.
        /// </param>
        public static string FormatValue(Constant c, bool forceSignForSignedIntegers = true, string integerFormat = "{0}{1}")
        {
            var dt = c.DataType;
            if (dt.Domain == Domain.SignedInt)
            {
                return FormatSignedValue(c, forceSignForSignedIntegers, integerFormat);
            }
            else if (dt.Domain == Domain.Real)
            {
                var str = c.ToReal64().ToString("G", CultureInfo.InvariantCulture);
                if (str.IndexOfAny(floatSpecials) < 0)
                {
                    return str + ".0";
                }
                return str;
            }
            else
                return FormatUnsignedValue(c, integerFormat);
        }

		private static string FormatString(DataType dt)
		{
            if (dt.Size < 8)
                return $"X{dt.Size * 2}";
            else
                return "X8";
		}

        /// <summary>
        /// Converts an unsigned integer constant to string representation.
        /// </summary>
        /// <param name="c">Constant to render.</param>
        /// <param name="format">Format string to use to render.</param>
        /// <returns>The rendered number.</returns>
		public static string FormatUnsignedValue(Constant c, string format = "{0}{1}")
		{
			return string.Format(format, "", c.ToUInt64().ToString(FormatString(c.DataType)));
		}
	}
}
