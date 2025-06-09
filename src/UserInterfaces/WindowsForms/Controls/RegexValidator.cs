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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
	public abstract class BaseValidator : Component
	{
		private Control ctrl;
		private bool isValid;
		private string errorMessage;
		private static ErrorProvider errorProvider = new ErrorProvider();
		private static Icon icon = Resources.Error;

		public bool IsValid
		{
			get { return isValid; }
			set { isValid = value; }
		}

		public Control ControlToValidate
		{
			get { return ctrl; }
			set
			{
				if (ctrl is not null)
					ctrl.Validating -= ctrl_Validating;
				ctrl = value;
				if (value is not null)
					ctrl.Validating += ctrl_Validating;
			}
		}

		public string ErrorMessage
		{
			get { return errorMessage; }
			set { errorMessage = value; }
		}

		public void Validate()
		{
			isValid = EvaluateIsValid();
			string errMsg = "";
			if (!isValid)
			{
				errMsg = errorMessage;
			}
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider.Icon = icon;
			errorProvider.SetError(ctrl, errMsg);
		}

		protected abstract bool EvaluateIsValid();

		private void ctrl_Validating(object sender, CancelEventArgs e)
		{
			Validate();
		}
	}

	/// <summary>
	/// Validates a control with a regular expression.
	/// </summary>
	public class RegexValidator : BaseValidator
	{
		private string regex;
		
		public RegexValidator()
		{
		}

		protected override bool EvaluateIsValid()
		{
			if (ControlToValidate is null)
				return true;
			string text = ControlToValidate.Text.Trim();
			if (text.Length == 0)
				return true;
			return System.Text.RegularExpressions.Regex.IsMatch(text, regex);
		}

		public string ValidationExpression
		{
			get { return regex; }
			set { regex = value; 
				Validate();
			}
		}

	}
}
