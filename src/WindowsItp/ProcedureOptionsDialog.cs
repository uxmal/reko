using Reko.Core;
using Reko.Core.Configuration;
using Reko.Gui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Reko.WindowsItp
{
    public partial class ProcedureOptionsDialog : Form
    {
        private Dictionary<string, string> values;
        public ProcedureOptionsDialog()
        {
            InitializeComponent();
            var options = new List<PropertyOption>
            {
                new PropertyOption { Name="noText"},
                new PropertyOption { Name="hasText", Text="Has Text", Description = "Has a description", },
                new PropertyOption { Name="Choices", Choices = new []
                    {
                        new ListOption_v1 { Text = "Intel 8086", Value = "8086" },
                        new ListOption_v1 { Value="80286"}
                    }
                },
                new PropertyOption { Name = "Custom", TypeName = typeof(CustomDialog).FullName },
            };

            values = new Dictionary<string, string>
            {
                { "noText", "initial garbage" }
            };
            var adapter = new PropertyOptionsGridAdapter(values, options);

            propertyGrid1.SelectedObject = adapter;
            propertyGrid1.PropertyValueChanged += PropertyGrid1_PropertyValueChanged;
        }

        private void PropertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            textBox1.Text = string.Format("{{ {0} }}",
                string.Join(
                    ",\r\n",
                    values.OrderBy(de => de.Key)
                        .Select(de => $"\"{de.Key}\": \"{de.Value}\"")));
        }

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

            public string GetComponentName()
            {
                return TypeDescriptor.GetComponentName(this, true);
            }

            public EventDescriptor GetDefaultEvent()
            {
                return TypeDescriptor.GetDefaultEvent(this, true);
            }

            public string GetClassName()
            {
                return TypeDescriptor.GetClassName(this, true);
            }

            public EventDescriptorCollection GetEvents(Attribute[] attributes)
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

            public object GetPropertyOwner(PropertyDescriptor pd)
            {
                return values;
            }

            public AttributeCollection GetAttributes()
            {
                return TypeDescriptor.GetAttributes(this, true);
            }

            public object GetEditor(Type editorBaseType)
            {
                return TypeDescriptor.GetEditor(this, editorBaseType, true);
            }

            public PropertyDescriptor GetDefaultProperty()
            {
                return null;
            }

            PropertyDescriptorCollection
                System.ComponentModel.ICustomTypeDescriptor.GetProperties()
            {
                return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
            }

            public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var properties = this.options
                    .Select(option => new DictionaryPropertyDescriptor(values, option))
                    .Cast<PropertyDescriptor>()
                    .ToArray();

                return new PropertyDescriptorCollection(properties);
            }
        }

        public class DictionaryPropertyDescriptor : PropertyDescriptor
        {
            IDictionary values;
            AttributeCollection attrs;

            internal DictionaryPropertyDescriptor(IDictionary d, PropertyOption option)
                : base(option.Text ?? option.Name, null)
            {
                values = d;
                this.Option = option;
                var attrs = new List<Attribute>();
                if (!string.IsNullOrEmpty(option.Description))
                {
                    attrs.Add(new DescriptionAttribute(option.Description));
                }
                if (option.Choices != null && option.Choices.Length > 0)
                {
                    attrs.Add(new TypeConverterAttribute(typeof(PropertyChoiceConverter)));
                }
                if (!string.IsNullOrWhiteSpace(option.TypeName))
                {
                    attrs.Add(new EditorAttribute(typeof(FooEditor), typeof(UITypeEditor)));
                }
                this.attrs = new AttributeCollection(attrs.ToArray());
            }

            public PropertyOption Option { get; }

            public override Type PropertyType
            {
                get { return typeof(string); }
            }

            public static DictionaryPropertyDescriptor GetFromContext(ITypeDescriptorContext context)
            {
                return (DictionaryPropertyDescriptor)context.PropertyDescriptor;
            }

            public override void SetValue(object component, object value)
            {
                values[Option.Name] = value;
            }

            public override object GetValue(object component)
            {
                return values[Option.Name];
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public override Type ComponentType
            {
                get { return null; }
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

        public class PropertyChoiceConverter : StringConverter
        {
            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                var pd = GetPd(context);
                return pd.Option.Choices != null &&
                    pd.Option.Choices.Length > 0;    // show combobox if there are choices
            }

            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                var pd = GetPd(context);
                return new StandardValuesCollection(
                    pd.Option.Choices
                        .Select(c => c.Value).ToList());
            }

            private static DictionaryPropertyDescriptor GetPd(ITypeDescriptorContext context)
            {
                return (DictionaryPropertyDescriptor)context.PropertyDescriptor;
            }
        }


        class FooEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
            {
                var svc = provider.RequireService<IWindowsFormsEditorService>();
                var pd = DictionaryPropertyDescriptor.GetFromContext(context);
                var dlgType = Type.GetType(pd.Option.TypeName);
                if (dlgType == null)
                    return value;
                var form = Activator.CreateInstance(dlgType) as Form;
                if (form == null)
                    return value;
                var valueProperty = dlgType.GetProperty("Value");
                if (valueProperty == null)
                    return value;
                
                valueProperty.SetValue(form, value);
                if (svc.ShowDialog(form) == DialogResult.OK)
                {
                    value = valueProperty.GetValue(form);
                }
                return value;
            }
        }

        class CustomDialog : Form
        {
            private TextBox textBox;
            private Button buttonOK;

            public CustomDialog()
            {
                this.ShowInTaskbar = false;
                this.MinimizeBox = false;
                this.MaximizeBox = false;


                this.textBox = new TextBox();
                this.textBox.Visible = true;
                this.textBox.Location = new Point(3, 3);
                this.textBox.Size = new Size(100, 21);
                this.textBox.BorderStyle = BorderStyle.Fixed3D;
                this.Controls.Add(textBox);

                this.buttonOK = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(3, 28),
                };
                this.Controls.Add(buttonOK);

                this.AcceptButton = buttonOK;
            }

            private void ButtonOK_Click(object sender, EventArgs e)
            {
                throw new NotImplementedException();
            }

            public string Value
            {
                get { return textBox.Text; }
                set { textBox.Text = value; }
            }
        }

    }
}
