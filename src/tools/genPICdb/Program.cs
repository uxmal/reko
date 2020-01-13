#region License
/* 
 *
 * Copyrighted (c) 2017-2020 Christian Hostelet.
 *
 * The contents of this file are subject to the terms of the Common Development
 * and Distribution License (the License), or the GPL v2, or (at your option)
 * any later version. 
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the License at http://www.netbeans.org/cddl.html
 * or http://www.gnu.org/licenses/gpl-2.0.html.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 *
 * When distributing Covered Code, include this CDDL Header Notice in each file
 * and include the License file at http://www.netbeans.org/cddl.txt.
 * If applicable, add the following below the CDDL Header, with the fields
 * enclosed by brackets [] replaced by your own identifying information:
 * "Portions Copyrighted (c) [year] [name of copyright owner]"
 *
 */
#endregion

namespace Reko.Tools.GenPICdb
{
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
    using System.Diagnostics.CodeAnalysis;

#if NETCOREAPP
    using System.Runtime.InteropServices;
#endif

    using Reko.Libraries.Microchip;

    /// <summary>
    /// A program to generate the PIC definition database from the MPLAB X IDE installation.
    /// <para>
    /// This program is designed to run on Windows (.NET Framework only), Linux (.NET Core only) and OSX (.NET Core only).
    /// </para>
    /// The Microchip MPLAB X IDE is freely available at www.microchip.com as a development tool.
    /// </summary>
    class Program
    {

        private const string defaultDBFilename = @"defaultpicdb.zip";
        private const string edcNamespace = @"http://crownking/edc";

        private bool success = false;

        private static readonly PICPartInfo picPartsInfo = new PICPartInfo();

        private static readonly IMPLABLocations mplabLoc = MPLABLocations.Create();

        private static readonly HashSet<string> acceptedPICArchitectures =
            new HashSet<string>() { "16xxxx", "16Exxx", "18xxxx" };

        // XML elements we are ignoring from the Microchip PIC Device definition.
        // This helps decrease the size of the generated database.
        private static readonly string[] unwantedPICXNodes =
            new string[] {
                "AliasList",
                "Breakpoints",
                "Checksum",
                "Freeze",
                "Import",
                "LCD",
                "MemoryModeList",
                "Oscillator",
                "PinList",
                "Power",
                "Programming",
                "Properties",
                "StimInfo",
                "WatchdogTimer",
            };

        private static string getExecutingDirectoryName()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly is null)
                return null;
            var location = new Uri(assembly.GetName().CodeBase);
            return location is null ? null : new FileInfo(location.LocalPath).Directory.FullName;
        }

        private static string _workingDir
        {
            get
            {
                if (workingDir is null)
                    workingDir = getExecutingDirectoryName();
                return workingDir;
            }
        }
        private static string workingDir = null;

        private static string _picLocalDBFilePath { get; set; }
            = Path.Combine(_workingDir, PICConstants.LocalDBFilename);

        private static string _picDefaultDBFilePath { get; set; }
            = Path.Combine(_workingDir, "..", "..", "..", defaultDBFilename);

        private static string _picCopyDBFilePath { get; set; }
            = Path.Combine(_workingDir, "..", "..", "..", PICConstants.LocalDBFilename);


        // Prunes the PIC XML document of unwanted/unnecessary information.
        // Adds missing information (e.g. bitpos for bit fields).
        private static XDocument pruneAndPatch(XDocument xdoc)
        {
            XNamespace edc = edcNamespace;

            var xroot = xdoc?.Root;
            if (xroot is null)
                return null;
            if (xroot.Name.LocalName != "PIC")
                return null;
            if (xroot.Name.Namespace == XNamespace.None)
                return xdoc;
            var sArch = xroot.Attribute(edc + "arch")?.Value;
            if (!acceptedPICArchitectures.Contains(sArch))
                return null;

            // Remove the useless (for our usage) elements
            foreach (var name in unwantedPICXNodes)
                xroot.Descendants().Where(p => p.Name.LocalName == name).Remove();

            // Remove unwanted sub-elements in various elements that we have no use of.
            xroot.Descendants().Where(p => p.Name.LocalName == "SFRModeList").Where(p => p.IsEmpty).Remove();

            // Remove the namespaces and prefixes
            foreach (var e in xroot.DescendantsAndSelf())
            {
                if (e.Name.Namespace != XNamespace.None)
                    e.Name = XNamespace.None.GetName(e.Name.LocalName);
                if (e.Attributes().Where(a => a.IsNamespaceDeclaration || a.Name.Namespace != XNamespace.None).Any())
                    e.ReplaceAttributes(e.Attributes().Select(a => a.IsNamespaceDeclaration ? null : a.Name.Namespace != XNamespace.None ? new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value) : a));
            }

            // Remove the unwanted root's attribute.
            xroot.Attribute("schemaLocation")?.Remove();

            // Add the bit fields' bit-location.
            xdoc = addBitPos(xdoc, "DCR");
            xdoc = addBitPos(xdoc, "SFR");

            // Must be done only after the addition of the bit locations on the bit fields.
            xdoc.Descendants().Where(p => p.Name.LocalName == "AdjustPoint").Remove();

            // Remove useless/confusing attributes in 'JoinedSFR' elements.
            foreach (var xjoined in xroot.Descendants().Where(p => p.Name.LocalName == "JoinedSFRDef"))
            {
                xjoined.Attribute("_modsrc")?.Remove();
                xjoined.Attribute("_refcount")?.Remove();
            }

            // Prune any redundant or meaningless bit field definitions.
            xdoc = removeRedundantFieldDef(xdoc, "DCR");
            xdoc = removeRedundantFieldDef(xdoc, "SFR");

            return xdoc;
        }

        /// <summary>
        /// Adds a bit position attribute to each bitfield definition.
        /// </summary>
        /// <param name="xdoc">The XML document.</param>
        /// <param name="prefix">The prefix to bitfield definition.</param>
        private static XDocument addBitPos(XDocument xdoc, string prefix)
        {
            var mode = prefix + "Mode";
            var fielddef = prefix + "FieldDef";

            foreach (var xelem in xdoc.Root.Descendants().Where(p => p.Name.LocalName == mode))
            {
                var bitpos = 0;
                foreach (var xdesc in xelem.Elements())
                {
                    var name = xdesc.Name.LocalName;
                    if (name.Equals(fielddef))
                    {
                        xdesc.SetAttributeValue("bitpos", bitpos);
                        var nzwidth = xdesc.Attribute("nzwidth").Value.ToInt32Ex();
                        bitpos += nzwidth;
                        continue;
                    }
                    if (name.Equals("AdjustPoint"))
                    {
                        var offset = xdesc.Attribute("offset").Value.ToInt32Ex();
                        bitpos += offset;
                        continue;
                    }
                    throw new NotImplementedException($"Unexpected XElement descendant '{name}' for {mode}.");
                }
            }

            return xdoc;

        }

        /// <summary>
        /// Removes the redundant register field definitions. Those whose width is equal to that of parent register.
        /// </summary>
        /// <param name="xdoc">The xdoc.</param>
        /// <param name="prefix">The prefix.</param>
        private static XDocument removeRedundantFieldDef(XDocument xdoc, string prefix)
        {
            var def = prefix + "Def";
            var fieldmodelist = prefix + "ModeList";
            var fieldmode = prefix + "Mode";
            var fielddef = prefix + "FieldDef";

            var xelem2remove = new List<XElement>();

            foreach (var xdef in xdoc.Root.Descendants().Where(p => p.Name.LocalName == def))
            {
                xdef.Attribute("_modsrc")?.Remove();
                xdef.Attribute("_refcount")?.Remove();

                var defnzwidth = xdef.Attribute("nzwidth").Value.ToUInt32Ex(); // Size of main register

                foreach (var xfielddef in xdef.Descendants().Where(p => p.Name.LocalName == fielddef))
                {
                    var fldnzwidth = xfielddef.Attribute("nzwidth").Value.ToUInt16Ex(); // Size of bit field
                    if (fldnzwidth == defnzwidth)
                    {
                        xelem2remove.Add(xfielddef);
                    }
                }
                xelem2remove.ForEach(xelem => xelem.Remove());
                xelem2remove.Clear();

                foreach (var xmode in xdef.Descendants().Where(p => p.Name.LocalName == fieldmode))
                {
                    if (!xmode.HasElements)
                        xelem2remove.Add(xmode);
                }
                xelem2remove.ForEach(xelem => xelem.Remove());
                xelem2remove.Clear();

                foreach (var xmodelist in xdef.Descendants().Where(p => p.Name.LocalName == fieldmodelist))
                {
                    if (!xmodelist.HasElements)
                        xelem2remove.Add(xmodelist);
                }
                xelem2remove.ForEach(xelem => xelem.Remove());
                xelem2remove.Clear();

            }


            return xdoc;
        }

        // Filters acceptable PIC12, PIC16 and PIC18 by their names.
        private static bool filterPICName(string s)
            => ((s.StartsWith("PIC12", true, CultureInfo.InvariantCulture) ||
                 s.StartsWith("PIC16", true, CultureInfo.InvariantCulture) ||
                 s.StartsWith("PIC18", true, CultureInfo.InvariantCulture))
                && !s.Contains("J")
                && !s.Contains("Q"));

        // Writes a PIC XML document to the PIC compact database. Keeps note of the PIC processor ID for COFF decoding.
        private static void writePICEntry(XDocument xdoc, string subdir, ZipArchive zout)
        {
            var xroot = xdoc?.Root;
            if (xroot is null)
                return;
            var picName = xroot.Attribute("name").Value;
            var procID = xroot.Attribute("procid").Value.ToUInt32Ex();
            var arch = xroot.Attribute("arch").Value;
            var clonedFrom = xroot.Attribute("clonedfrom")?.Value;
            picPartsInfo.Parts.Add(new PICPart(picName, procID, arch, clonedFrom));
            var picpath = subdir + "/" + picName + ".PIC";
            var picentry = zout.CreateEntry(picpath);
            using (var picw = new StreamWriter(picentry.Open()))
                xdoc.Save(picw);
        }

        // Accepts only desired PIC16 and PIC18 from the more general (huge) Microchip MPLAB X database.
        private static bool acceptPICEntry(ZipArchiveEntry picentry, Func<string, bool> filter)
            => ((picentry.FullName.StartsWith(PICConstants.ContentPIC16Path + "/PIC16", true, CultureInfo.InvariantCulture) ||
                 picentry.FullName.StartsWith(PICConstants.ContentPIC18Path + "/PIC18", true, CultureInfo.InvariantCulture))
                && filter(picentry.Name));

        // Writes the list of PIC parts information to the PIC compact database.
        private static void writePartsInfo(ZipArchive zout)
        {
            if (picPartsInfo.Parts.Count() <= 0)
                return;
            picPartsInfo.Version = mplabLoc.Version;
            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            var theDate = DateTime.Now.ToLongDateString();
            var theHour = DateTime.Now.ToShortTimeString();
            xdoc.AddFirst(new XComment($"Generated on {theDate} at {theHour}."));
            xdoc.Add(picPartsInfo.ToXElement());
            var partsentry = zout.CreateEntry(PICConstants.PartsInfoFilename);
            using (var partw = new StreamWriter(partsentry.Open()))
                xdoc.Save(partw);
        }

        /// <summary>
        /// Generate the PIC definition database using older Microchip MPLAB X database (crownking.edc.jar)
        /// </summary>
        /// <returns>
        /// Error code or 0.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private static int before410Database(string outFilename)
        {
            Console.WriteLine($"Using *old* MPLAB X IDE version {mplabLoc.Version} installation.");
            var numpics = 0;

            try
            {
                // Create a new local database
                using (var outfile = new FileStream(outFilename, FileMode.Create, FileAccess.Write))
                {
                    // Those database are compressed (ZIP format)
                    using (ZipArchive crownkingfile = ZipFile.OpenRead(mplabLoc.SourceFolder),
                                      zoutfile = new ZipArchive(outfile, ZipArchiveMode.Create))
                    {
                        // For each MPLAB X IDE entry, look only for true PIC16 and PIC18
                        foreach (var zentry in crownkingfile.Entries)
                        {
                            if (acceptPICEntry(zentry, filterPICName))
                            {
                                // Candidate to extract.
                                XDocument xdoc;
                                using (var eo = zentry.Open())
                                {
                                    xdoc = XDocument.Load(zentry.Open());
                                    xdoc = pruneAndPatch(xdoc); // Pruning of the XML tree for unwanted elements
                                }

                                if (!(xdoc is null))
                                {
                                    writePICEntry(xdoc, Path.GetDirectoryName(zentry.FullName), zoutfile);
                                    numpics++;
                                }
                            }
                        }
                        writePartsInfo(zoutfile);
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
                throw;
            }
        }

        /// <summary>
        /// Generate the PIC definition database using new Microchip Devices Family Packs (starting with MPLAB X IDE v4.10)
        /// </summary>
        /// <returns>
        /// Error code or 0.
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private static int post410Database(string outFilename)
        {
            var numpics = 0;
            Console.WriteLine($"Using MPLAB X IDE {mplabLoc.Version} installation.");

            try
            {
                // Create a new local database
                using (var outfile = new FileStream(outFilename, FileMode.Create, FileAccess.Write))
                {
                    // This database is compressed (ZIP format)
                    using (var zoutfile = new ZipArchive(outfile, ZipArchiveMode.Create))
                    {
                        var xver = new XDocument(
                                        new XDeclaration("1.0", "utf-8", "yes"),
                                        new XElement("Version", mplabLoc.Version)
                                   );
                        var picentry = zoutfile.CreateEntry("_version_.xml");
                        using (var picw = new StreamWriter(picentry.Open()))
                        {
                            xver.Save(picw);
                        }

                        foreach (var (name, folder) in new (string name, string folder)[]
                              { ("PIC12", PICConstants.ContentPIC16Path),
                                ("PIC16", PICConstants.ContentPIC16Path),
                                ("PIC18", PICConstants.ContentPIC18Path)})
                        {
                            Console.WriteLine($"{name}(L)F");
                            foreach (var xdoc in getValidPICInDFP($"{name}*_DFP"))
                            {
                                writePICEntry(xdoc, folder, zoutfile);
                                numpics++;
                                Console.Write(".");
                            }
                            Console.WriteLine("done.");
                        }

                        Console.Write("Checking clones...");
                        var noErrorYet = true;

                        foreach (var p in picPartsInfo.Parts.Where(p => !string.IsNullOrWhiteSpace(p.ClonedFrom)))
                        {
                            if (!picPartsInfo.Parts.Any(cl => cl.Name == p.ClonedFrom))
                            {
                                if (noErrorYet)
                                    Console.WriteLine();
                                noErrorYet = false;
                                Console.WriteLine($"Warning: Missing '{p.ClonedFrom}' for clone '{p.Name}'");
                            }
                        }
                        Console.WriteLine("Done.");
                        writePartsInfo(zoutfile);
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
                throw;
            }
        }

        /// <summary>
        /// Enumerates the "interesting" PICs from the MPLAB X IDE database.
        /// </summary>
        /// <param name="subdir">The sub-directory of interest in MPLAB X IDE installation directory.</param>
        /// 
        private static IEnumerable<XDocument> getValidPICInDFP(string subdir)
        {
            foreach (var dir in Directory.EnumerateDirectories(mplabLoc.SourceFolder, subdir, SearchOption.AllDirectories))
            {
                foreach (var edcdir in Directory.EnumerateDirectories(dir, "edc", SearchOption.AllDirectories))
                {
                    foreach (var filename in Directory.EnumerateFiles(edcdir, "PIC*.PIC"))
                    {
                        var picname = Path.GetFileNameWithoutExtension(filename);
                        if (filterPICName(picname))
                        {
                            var xdoc = XDocument.Load(filename);
                            xdoc = pruneAndPatch(xdoc); // Pruning of the XML tree for unwanted elements
                            if (!(xdoc is null)) yield return xdoc;
                        }
                    }
                }
            }
            yield break;
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
#if NETCOREAPP

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("On Windows, please use the .NET Frame version of this program.");
                return -1;
            }
#endif

            if (args.Count() > 0)
            {
                var filePath = args[0];
                _picLocalDBFilePath = Path.Combine(_workingDir, filePath);
                if (!Directory.Exists(Path.GetDirectoryName(_picLocalDBFilePath)))
                {
                    Console.WriteLine("Target directory does not exist!");
                    return -1;
                }
                _picDefaultDBFilePath = null;
                _picCopyDBFilePath = null;
            }

            if (!mplabLoc.IsValid)
            {
                if (File.Exists(_picDefaultDBFilePath))
                {
                    Console.WriteLine("Copying a default PIC database but may be out-dated. Think of installing Microchip MPLAB X IDE.");
                    File.Copy(_picDefaultDBFilePath, _picLocalDBFilePath, true);
                    File.Copy(_picDefaultDBFilePath, _picCopyDBFilePath, true);
                    return 0;
                }

                Console.WriteLine("Unable to find the Microchip MPLAB X IDE installation directory nor a default PIC database.");
                Console.WriteLine("Please make sure Microchip MPLAB X IDE is installed on this system.");
                return -1;
            }

            if (mplabLoc.UsePacks)
                success = (post410Database(_picLocalDBFilePath) == 0);
            else
                success = (before410Database(_picLocalDBFilePath) == 0);

            if (success && !(_picCopyDBFilePath is null))
            {
                Console.WriteLine("Keeping a default copy of PIC database...");
                File.Copy(_picLocalDBFilePath, _picDefaultDBFilePath, true);
                File.Copy(_picLocalDBFilePath, _picCopyDBFilePath, true);
                return 0;
            }

            Console.WriteLine("Unable to find a properly formatted Microchip PIC Device Definitions nor a default PIC database.");
            return -1;
        }

        /// <summary>
        /// Main entry-point for this application.
        /// </summary>
        /// <param name="args">An array of command-line argument strings.</param>
        /// <returns>
        /// Exit-code for the process - 0 for success, else an error code.
        /// </returns>
        public static int Main(string[] args)
        {
            var res = new Program().Execute(args);
            
            return res;
        }

    }

}

