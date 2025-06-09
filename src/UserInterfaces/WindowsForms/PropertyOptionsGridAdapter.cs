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

#nullable enable

using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Reko.UserInterfaces.WindowsForms
{
    /// <summary>
    /// Generates property descriptors that obey the restrictions specified in the 
    /// supplied PropertyOptions.
    /// </summary>
    public class PropertyOptionsGridAdapter : ICustomTypeDescriptor
    {
        private List<PropertyOption> options;
        private IDictionary values;

        public PropertyOptionsGridAdapter(IDictionary values, List<PropertyOption> options)
        {
            this.options = options;
            this.values = values;
        }

        public string? GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public EventDescriptor? GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string? GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[]? attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public object? GetPropertyOwner(PropertyDescriptor? pd)
        {
            return values;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public object? GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor? GetDefaultProperty()
        {
            return null;
        }

        PropertyDescriptorCollection
            System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(Array.Empty<Attribute>());
        }

        public PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
        {
            var properties = this.options
                .Select(option => new DictionaryPropertyDescriptor(values, option))
                .Cast<PropertyDescriptor>()
                .ToArray();

            return new PropertyDescriptorCollection(properties);
        }


        public class DictionaryPropertyDescriptor : PropertyDescriptor
        {
            IDictionary values;
            AttributeCollection attrs;

            internal DictionaryPropertyDescriptor(IDictionary d, PropertyOption option)
                : base(option.Text ?? option.Name!, null)
            {
                values = d;
                this.Option = option;
                var attrs = new List<Attribute>();
                if (!string.IsNullOrEmpty(option.Description))
                {
                    attrs.Add(new DescriptionAttribute(option.Description));
                }
                if (option.Choices is not null && option.Choices.Length > 0)
                {
                    attrs.Add(new TypeConverterAttribute(typeof(ChoiceConverter)));
                }
                if (!string.IsNullOrWhiteSpace(option.TypeName))
                {
                    attrs.Add(new EditorAttribute(typeof(PropertyOptionEditor), typeof(UITypeEditor)));
                }
                this.attrs = new AttributeCollection(attrs.ToArray());
            }

            public PropertyOption Option { get; }

            public override Type PropertyType
            {
                get { return typeof(string); }
            }

            public static DictionaryPropertyDescriptor? GetFromContext(ITypeDescriptorContext context)
            {
                return (DictionaryPropertyDescriptor?)context.PropertyDescriptor;
            }

            public override void SetValue(object? component, object? value)
            {
                values[Option.Name!] = value;
            }

            public override object? GetValue(object? component)
            {
                return values[Option.Name!];
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public override Type ComponentType
            {
                get { return null!; }
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override void ResetValue(object component)
            {
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            public override AttributeCollection Attributes => attrs;
        }

        public class ChoiceConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
            {
                var pd = GetPd(context);
                return pd.Option.Choices is not null &&
                    pd.Option.Choices.Length > 0;    // show combobox if there are choices
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext? context)
            {
                var pd = GetPd(context);
                if (pd is null)
                    return new StandardValuesCollection(Array.Empty<object>());
                return new StandardValuesCollection(
                    pd.Option.Choices
                        .Select(c => c.Value).ToList());
            }

            private static DictionaryPropertyDescriptor? GetPd(ITypeDescriptorContext? context)
            {
                return (DictionaryPropertyDescriptor?)context?.PropertyDescriptor;
            }
        }

        /// <summary>
        /// Allows a custom editor to pop up a dialog box to collect a value from the user.
        /// </summary>
        /// <remarks>
        /// The custom editor must be a class derived from System.Windows.Forms and have 
        /// a public read/write property called "Value".
        /// </remarks>
        class PropertyOptionEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object? EditValue(ITypeDescriptorContext? context, System.IServiceProvider provider, object? value)
            {
                var svc = provider.RequireService<IWindowsFormsEditorService>();
                var pd = DictionaryPropertyDescriptor.GetFromContext(context);
                var pluginSvc = provider.RequireService<IPluginLoaderService>();

                if (pd.Option.TypeName is null)
                    return value;
                var dlgType = pluginSvc.GetType(pd.Option.TypeName);
                if (dlgType is null)
                    return value;
                if (!(Activator.CreateInstance(dlgType) is Form form))
                    return value;
                var valueProperty = dlgType.GetProperty("Value");
                if (valueProperty is null)
                    return value;

                valueProperty.SetValue(form, value);
                if (svc.ShowDialog(form) == System.Windows.Forms.DialogResult.OK)
                {
                    value = valueProperty.GetValue(form)!;
                }
                form.Dispose();
                return value;
            }
        }
    }
}