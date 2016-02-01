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
using Reko.Core.Configuration;
using Reko.Core.Services;
using System;
using System.IO;
using System.Xml.Serialization;

[XmlRoot(ElementName ="memory", Namespace="http://schemata.jklnet.org/Reko/v4")]
public class MemoryMap_v1
{
    [XmlElement("segment")]
    public MemorySegment_v1[] Segments;

    public static MemoryMap_v1 LoadMemoryMapFromFile(IServiceProvider svc, string mmap)
    {
        var cfgSvc = svc.RequireService<IConfigurationService>();
        var fsSvc = svc.RequireService<IFileSystemService>();
        var diagSvc = svc.RequireService<IDiagnosticsService>();
        try
        {
            var filePath = cfgSvc.GetInstallationRelativePath(mmap);
            XmlSerializer ser = new XmlSerializer(typeof(MemoryMap_v1));
            using (var stm = fsSvc.CreateFileStream(filePath, FileMode.Open))
            {
                return (MemoryMap_v1)ser.Deserialize(stm);
            }
        }
        catch (Exception ex)
        {
            diagSvc.Error(ex, string.Format("Unable to open memory map file '{0}.", mmap));
            return null;
        }
    }
}

public partial class MemorySegment_v1
{
    [XmlAttribute("name")]
    public string Name;

    [XmlAttribute("addr")]
    public string Address;

    [XmlAttribute("size")]
    public string Size;

    [XmlAttribute("attr")]
    public string Attributes;

    [XmlElement("description")]
    public string Description;
}
