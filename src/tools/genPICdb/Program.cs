#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
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

using Reko.Libraries.Microchip;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Reko.Tools.genPICdb
{
    /// <summary>
    /// A program to generate the PIC definition database from the MPLAB X IDE installation.
    /// The Microchip MPLAB X IDE is freely available at www.microchip.com as a development tool.
    /// </summary>
    class Program
    {
        #region Privates

        private const string keyW32 = "SOFTWARE\\Microchip";
        private const string keyW64 = "SOFTWARE\\Wow6432Node\\Microchip";

        private const string packs = "packs\\Microchip";

        private const string crownkingpath = "mplab_ide\\mplablibs\\modules\\ext";
        private const string crownkingfile = "crownking.edc.jar";

        private const string localdbfile = "picdb.zip";
        private const string defaultdbfile = "defaultpicdb.zip";
        private const string contentPIC16 = @"content/edc/16xxxx";
        private const string contentPIC18 = @"content/edc/18xxxx";

        private bool success = false;

        #endregion

        #region Helpers

        private string MPLABXInstallDir
        {
            get
            {
                if (mplabXinstallDir == null)
                {
                    try
                    {
                        RegistryKey MicrochipKey = Registry.LocalMachine.OpenSubKey(keyW32);
                        if (MicrochipKey?.SubKeyCount <= 0) MicrochipKey = Registry.LocalMachine.OpenSubKey(keyW64);
                        mplabXinstallDir = (string)(MicrochipKey?.OpenSubKey("MPLAB X")?.GetValue("InstallDir", null));
                        mplabXVersion = new DirectoryInfo(mplabXinstallDir).Name;
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning($"Couldn't get path to Microchip MPLAB X IDE installation directory : {ex.StackTrace}");
                    }
                }
                return mplabXinstallDir;
            }
        }
        private string mplabXinstallDir = null;
        private string mplabXVersion = null;

        private string GenPICDBPath
        {
            get
            {
                if (genpicdbpath == null)
                {
                    Assembly CrownkingAssembly;
                    CrownkingAssembly = Assembly.GetAssembly(this.GetType());
                    genpicdbpath = Path.GetDirectoryName(CrownkingAssembly.Location);
                }
                return genpicdbpath;
            }
        }
        private string genpicdbpath = null;

        private string PicLocalDBFilePath => Path.Combine(GenPICDBPath, localdbfile);
        private string PicDefaultDBFilePath => Path.Combine(GenPICDBPath, @"..\..", defaultdbfile);
        private string PicCopyDBFilePath => Path.Combine(GenPICDBPath, @"..\..", localdbfile);

        private string PathToPacks => Path.Combine(MPLABXInstallDir, packs);

        private string CrownkingPath => Path.Combine(MPLABXInstallDir, crownkingpath);

        private static HashSet<string> acceptedArchitectures =
            new HashSet<string>() { "16xxxx", "16Exxx", "18xxxx" };

        // XML elements we are ignoring. This helps decrease the size of the database.
        private static string[] unwantedXNodes =
            new string[] {
                "Import",
                "Power",
                "Programming",
                "Oscillator",
                "Freeze",
                "WatchdogTimer",
                "Breakpoints",
                "MemoryModeList",
                "PinList",
                "LCD",
                "AliasList",
                "Checksum",
                "StimInfo",
            };

        private XDocument Pruning(XDocument xdoc)
        {
            XNamespace edc = "http://crownking/edc";

            XElement xroot = xdoc?.Root;
            if (xroot == null)
                return null;
            if (xroot.Name.Namespace == XNamespace.None)
                return xdoc;
            if (xroot.Name.LocalName != "PIC")
                return null;
            string sArch = xroot.Attribute(edc + "arch")?.Value;
            if (!acceptedArchitectures.Contains(sArch))
                return null;

            // Remove the useless (for us) elements
            foreach (string name in unwantedXNodes)
                xroot.DescendantElements(name).Remove();

            // Remove unwanted sub-elements in various elements that we have no use of.
            xroot.DescendantElements("SFRModeList").Where(p => p.IsEmpty).Remove();

            // Remove the namespaces and prefixes
            foreach (XElement e in xroot.DescendantsAndSelf())
            {
                if (e.Name.Namespace != XNamespace.None)
                    e.Name = XNamespace.None.GetName(e.Name.LocalName);
                if (e.Attributes().Where(a => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None).Any())
                    e.ReplaceAttributes(e.Attributes().Select(a => a.IsNamespaceDeclaration ? null : a.Name.Namespace != XNamespace.None ? new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value) : a));
            }

            // Remove the unwanted root's attribute
            xroot.Attribute("schemaLocation").Remove();
            return xdoc;
        }

        private bool FilterPICName(string s)
        {
            return
                (s.StartsWith("PIC16F")
                ||
                (s.StartsWith("PIC18F")) && !s.Contains("J") && !s.Contains("Q"));
        }

        private void WritePICEntry(XDocument xdoc, string subdir, ZipArchive zout)
        {
            PIC pic = xdoc.ToObject<PIC>();
            var xs = new XmlSerializer(typeof(PIC));
            string picname = pic.Name;
            string picpath = subdir + "/" + picname + ".PIC";
            ZipArchiveEntry picentry = zout.CreateEntry(picpath);
            using (var picw = new StreamWriter(picentry.Open()))
                xs.Serialize(picw, pic);
        }

        #endregion

        /// <summary>
        /// Main entry-point for this application.
        /// </summary>
        /// <param name="args">An array of command-line argument strings.</param>
        /// <returns>
        /// Exit-code for the process - 0 for success, else an error code.
        /// </returns>
        static int Main(string[] args)
        {
            return new Program().Execute(args);
        }

        /// <summary>
        /// Executes the program.
        /// </summary>
        /// <param name="args">The names of the destination directories for copying the 'picdb.zip' file.</param>
        /// <returns>
        /// An error code or 0.
        /// </returns>
        public int Execute(string[] args)
        {
            // Try to regenerate the PIC DB file.
            if (Directory.Exists(PathToPacks))    // Post v4.10 database
                success = (Post410Database() == 0);
            else
            if (Directory.Exists(CrownkingPath))   // Ante v4.10 database
                success = (Ante410Database() == 0);

            if (success)
            {
                Console.WriteLine("Keeping a default copy of PIC DB.");
                File.Copy(PicLocalDBFilePath, PicDefaultDBFilePath, true);
                File.Copy(PicLocalDBFilePath, PicCopyDBFilePath, true);
                return 0;
            }
            if (File.Exists(PicDefaultDBFilePath))
            {
                Console.WriteLine("Copying a default copy of PIC DB.");
                File.Copy(PicDefaultDBFilePath, PicLocalDBFilePath, true);
                File.Copy(PicDefaultDBFilePath, PicCopyDBFilePath, true);
                return 0;
            }
            Console.WriteLine("Unable to find the Microchip MPLAB X IDE installation directory nor a default PIC Database.");
            Console.WriteLine("Please make sure Microchip MPLAB X IDE is installed on this system.");
            return -1;
        }

        /// <summary>
        /// Generate the PIC definition database using old Microchip database (crownking.edc.jar)
        /// </summary>
        /// <returns>
        /// Error code or 0.
        /// </returns>
        private int Ante410Database()
        {
            string crownkingfilepath = Path.Combine(CrownkingPath, crownkingfile);
            if (!File.Exists(crownkingfilepath))
            {
                Trace.TraceError($"File not found: '{crownkingfilepath}'.");
                return -1;
            }

            Console.WriteLine($"Using *old* MPLAB X IDE version {mplabXVersion} installation.");
            int numpics = 0;

            try
            {
                // Create a new local database
                using (var outfile = new FileStream(PicLocalDBFilePath, FileMode.Create, FileAccess.Write))
                {
                    // Those database are compressed (ZIP format)
                    using (ZipArchive crownkingfile = ZipFile.OpenRead(crownkingfilepath),
                                      zoutfile = new ZipArchive(outfile, ZipArchiveMode.Create))
                    {
                        // For each MPLAB X IDE entry, look only for true PIC16 and PIC18
                        foreach (var entry in crownkingfile.Entries)
                        {
                            if (entry.FullName.StartsWith(contentPIC16 + "/PIC16", true, CultureInfo.InvariantCulture) ||
                                entry.FullName.StartsWith(contentPIC18 + "/PIC18", true, CultureInfo.InvariantCulture))
                            {
                                // Caller may want to filter further valid PIC entries
                                if (FilterPICName(entry.Name))
                                {
                                    // Candidate to extract.
                                    XDocument xdoc;
                                    using (var eo = entry.Open())
                                    {
                                        xdoc = XDocument.Load(entry.Open());
                                        xdoc = Pruning(xdoc); // Pruning of the XML tree for unwanted elements
                                    }

                                    if (xdoc != null)
                                    {
                                        WritePICEntry(xdoc, Path.GetDirectoryName(entry.FullName), zoutfile);
                                        numpics++;
                                    }
                                }
                            }
                        }
                    }
                }
                if (numpics > 0)
                {
                    Console.WriteLine($"Created {numpics} PIC entries.");
                    return 0;
                }
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Got exception: {ex.Message}");
                Trace.TraceError($"Update DB : {ex.StackTrace}");
                return -1;
            }
        }

        /// <summary>
        /// Enumerates the "interesting" PICs form the MPLAB X IDE database.
        /// </summary>
        /// <param name="subdir">The sub-directory of interest in MPLAB X IDE installation directory.</param>
        /// 
        private IEnumerable<XDocument> GetValidPIC(string subdir)
        {
            foreach (var dir in Directory.EnumerateDirectories(PathToPacks, subdir, SearchOption.AllDirectories))
            {
                foreach (var edcdir in Directory.EnumerateDirectories(dir, "edc", SearchOption.AllDirectories))
                {
                    foreach (var filename in Directory.EnumerateFiles(edcdir, "PIC*.PIC"))
                    {
                        string picname = Path.GetFileNameWithoutExtension(filename);
                        if (FilterPICName(picname))
                        {
                            var xdoc = XDocument.Load(filename); ;
                            xdoc = Pruning(xdoc); // Pruning of the XML tree for unwanted elements
                            if (xdoc != null) yield return xdoc;
                        }
                    }
                }
            }
            yield break;
        }

        /// <summary>
        /// Generate the PIC definition database using new Microchip database (starting with MPLAB X IDE v4.10)
        /// </summary>
        /// <returns>
        /// Error code or 0.
        /// </returns>
        private int Post410Database()
        {
            int numpics = 0;
            Console.WriteLine($"Using MPLAB X IDE {mplabXVersion} installation.");

            try
            {
                // Create a new local database
                using (var outfile = new FileStream(PicLocalDBFilePath, FileMode.Create, FileAccess.Write))
                {
                    // This database is compressed (ZIP format)
                    using (var zoutfile = new ZipArchive(outfile, ZipArchiveMode.Create))
                    {
                        var xver = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Version", mplabXVersion));
                        ZipArchiveEntry picentry = zoutfile.CreateEntry("_version_.xml");
                        using (var picw = new StreamWriter(picentry.Open()))
                        {
                            xver.Save(picw);
                        }

                            Console.Write("PIC16F");
                        foreach (XDocument xdoc in GetValidPIC("PIC12*_DFP"))
                        {
                            WritePICEntry(xdoc, contentPIC16, zoutfile);
                            numpics++;
                            Console.Write(".");
                        }
                        foreach (XDocument xdoc in GetValidPIC("PIC16*_DFP"))
                        {
                            WritePICEntry(xdoc, contentPIC16, zoutfile);
                            numpics++;
                            Console.Write(".");
                        }
                        Console.WriteLine("done.");
                        Console.Write("PIC18F");
                        foreach (XDocument xdoc in GetValidPIC("PIC18*_DFP"))
                        {
                            WritePICEntry(xdoc, contentPIC18, zoutfile);
                            numpics++;
                            Console.Write(".");
                        }
                        Console.WriteLine("done.");
                    }
                }
                if (numpics > 0)
                {
                    Console.WriteLine($"Created {numpics} PIC entries.");
                    return 0;
                }
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Got exception: {ex.Message}");
                Trace.TraceError($"Update DB : {ex.StackTrace}");
                return -1;
            }
        }

    }

}

