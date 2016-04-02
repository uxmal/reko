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

using System;
using System.Xml.Serialization;

namespace Reko.Core.Configuration
{
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://schemata.jklnet.org/Reko/Config/v1")]
    public partial class TypeLibraryReference_v1
    {
        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string Arch;

        [XmlAttribute]
        public string Loader;

        [XmlAttribute]
        public string Module;
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://schemata.jklnet.org/Reko/Config/v1")]
    [XmlRoot(ElementName="Reko", Namespace = "http://schemata.jklnet.org/Reko/Config/v1", IsNullable = false)]
    public partial class RekoConfiguration_v1
    {
        private object[] itemsField;

        [XmlArray(ElementName = "Loaders")]
        [XmlArrayItem(ElementName = "Loader")]
        public RekoLoader[] Loaders;

        [XmlArray("RawFiles")]
        [XmlArrayItem("RawFile", typeof(RekoRawFilesRawFile))]
        public RekoRawFilesRawFile[] RawFiles;

        [XmlArray("SignatureFiles")]
        [XmlArrayItem("SignatureFile")]
        public RekoSignatureFilesSignatureFile[] SignatureFiles;

        [XmlArray("Environments")]
        [XmlArrayItem("Environment")]
        public RekoEnvironmentsEnvironment[] Environments;

        [XmlArray("Architectures")]
        [XmlArrayItem("Architecture")]
        public RekoArchitecture[] Architectures;

        [XmlArray("Assemblers")]
        [XmlArrayItem("Assembler")]
        public RekoAssemblersAssembler[] Assemblers;

        [XmlElement("UiPreferences")]
        public RekoUiPreferences UiPreferences;
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://schemata.jklnet.org/Reko/Config/v1")]
    public partial class RekoArchitectures
    {
        private RekoArchitecture[] architectureField;

        [XmlElement("Architecture")]
        public RekoArchitecture[] Architecture
        {
            get
            {
                return this.architectureField;
            }
            set
            {
                this.architectureField = value;
            }
        }
    }

    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "http://schemata.jklnet.org/Reko/Config/v1")]
    public partial class RekoArchitecture
    {

        private string nameField;

        private string descriptionField;

        private string typeField;

        /// <remarks/>
        [XmlAttribute]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [Serializable]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemata.jklnet.org/Reko/Config/v1")]
    public partial class RekoAssemblers
    {

        private RekoAssemblersAssembler[] assemblerField;

        /// <remarks/>
        [XmlElement("Assembler")]
        public RekoAssemblersAssembler[] Assembler
        {
            get
            {
                return this.assemblerField;
            }
            set
            {
                this.assemblerField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [Serializable]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemata.jklnet.org/Reko/Config/v1")]
    public partial class RekoAssemblersAssembler
    {
        /// <remarks/>
        [XmlAttribute]
        public string Name;

        /// <remarks/>
        [XmlAttribute]
        public string Description;

        /// <remarks/>
        [XmlAttribute]
        public string Type;
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [Serializable]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemata.jklnet.org/Reko/Config/v1")]
    public partial class RekoEnvironmentsEnvironment
    {
        [System.Xml.Serialization.XmlArrayItemAttribute("TypeLibrary", typeof(TypeLibraryReference_v1), IsNullable = false)]
        public TypeLibraryReference_v1[] TypeLibraries;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("TypeLibrary", typeof(TypeLibraryReference_v1), IsNullable = false)]
        public TypeLibraryReference_v1[] Characteristics;

        /// <remarks/>
        [XmlAttribute]
        public string Name;

        /// <remarks/>
        [XmlAttribute]
        public string Description;

        /// <remarks/>
        [XmlAttribute]
        public string Type;

        /// <remarks/>
        [XmlAttribute]
        public string MemoryMap;
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [Serializable]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemata.jklnet.org/Reko/Config/v1")]
    public partial class RekoLoader
    {
        [XmlAttribute]
        public string MagicNumber;

        [XmlAttribute]
        public string Type;

        [XmlAttribute]
        public string Offset;

        /// <remarks/>
        [XmlAttribute]
        public string Extension;

        /// <remarks/>
        [XmlAttribute]
        public string Label;

        [XmlAttribute]
        public string Argument;
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [Serializable]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemata.jklnet.org/Reko/Config/v1")]
    public partial class RekoRawFiles
    {

        private RekoRawFilesRawFile[] rawFileField;

        /// <remarks/>
        [XmlElement("RawFile")]
        public RekoRawFilesRawFile[] RawFile
        {
            get
            {
                return this.rawFileField;
            }
            set
            {
                this.rawFileField = value;
            }
        }
    }

    [Serializable]
    public partial class RekoRawFilesRawFile
    {
        [XmlElement("Entry")]
        public RekoRawFilesRawFileEntry[] Entry;

        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string Description;

        [XmlAttribute]
        public string Arch;

        [XmlAttribute]
        public string Env;

        [XmlAttribute]
        public string Base;
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [Serializable]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemata.jklnet.org/Reko/Config/v1")]
    public partial class RekoRawFilesRawFileEntry
    {

        private string addrField;

        private string nameField;

        private string followField;

        /// <remarks/>
        [XmlAttribute]
        public string Addr
        {
            get
            {
                return this.addrField;
            }
            set
            {
                this.addrField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Follow
        {
            get
            {
                return this.followField;
            }
            set
            {
                this.followField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [Serializable]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemata.jklnet.org/Reko/Config/v1")]
    public partial class RekoSignatureFiles
    {

        private RekoSignatureFilesSignatureFile[] signatureFileField;

        /// <remarks/>
        [XmlElement("SignatureFile")]
        public RekoSignatureFilesSignatureFile[] SignatureFile
        {
            get
            {
                return this.signatureFileField;
            }
            set
            {
                this.signatureFileField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [Serializable]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemata.jklnet.org/Reko/Config/v1")]
    public partial class RekoSignatureFilesSignatureFile
    {

        private string filenameField;

        private string typeField;

        /// <remarks/>
        [XmlAttribute]
        public string Filename
        {
            get
            {
                return this.filenameField;
            }
            set
            {
                this.filenameField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    public partial class RekoUiPreferences
    {
        [XmlElement("Style")]
        public RekoUiPreferencesStyle[] Styles;
    }

    [Serializable]
    public partial class RekoUiPreferencesStyle
    {
        /// <remarks/>
        [XmlAttribute]
        public string Name;

        /// <remarks/>
        [XmlAttribute]
        public string Font;

        /// <remarks/>
        [XmlAttribute]
        public string ForeColor;

        [XmlAttribute]
        public string BackColor;

        [XmlAttribute]
        public string Width;

        [XmlAttribute]
        public string Cursor;
    }
}