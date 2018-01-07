
// summary:	Implements the Microchip PIC definition loading from the Microchip PIC XML database.
// 
namespace Microchip.Crownking
{
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

    /// <summary>
    /// This factory class provides methods for loading Microchip PIC definition (XML) from the MPLAB X IDE (a.k.a. Crownking) database or a local copy of it.
    /// </summary>
    public class PICCrownking
    {
        #region Privates

        private const string _keyW32 = "SOFTWARE\\Microchip";
        private const string _keyW64 = "SOFTWARE\\Wow6432Node\\Microchip";
        private const string _crownkingpath = "mplab_ide\\mplablibs\\modules\\ext";
        private const string _crownkingfile = "crownking.edc.jar";
        private const string _localdbfile = "picdb.zip";
        private const string _contentPIC16 = "content/edc/16xxxx";
        private const string _contentPIC18 = "content/edc/18xxxx";

        private static PICCrownking currentDB = null;

        private PICCrownkingException _raiseError(DBErrorCode err, string msg)
        {
            LastError = err;
            return new PICCrownkingException(err, msg);
        }

        private void _checkDBExist()
        {
            if (CurrentDBPath == null || !File.Exists(CurrentDBPath))
            {
                throw _raiseError(DBErrorCode.NoDBFile, "No Microchip XML PIC definitions available on this system");
            }
        }

        private string _getIDECrownkingFilePath()
        {
            RegistryKey MicrochipKey = Registry.LocalMachine.OpenSubKey(_keyW32);
            if (MicrochipKey?.SubKeyCount <= 0) MicrochipKey = Registry.LocalMachine.OpenSubKey(_keyW64);
            string ideinstalldir = (string)(MicrochipKey?.OpenSubKey("MPLAB X")?.GetValue("InstallDir", null));
            if (ideinstalldir == null) return null;
            string path = Path.Combine(ideinstalldir, _crownkingpath);
            if (!String.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                path = Path.Combine(path, _crownkingfile);
                return File.Exists(path) ? path : null;
            }
            return null;
        }

        private string _getPICLocalDBFilePath()
        {
            Assembly CrownkingAssembly;
            CrownkingAssembly = Assembly.GetAssembly(this.GetType());
            string sDir = Path.GetDirectoryName(CrownkingAssembly.Location);
//            string sDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(sDir, _localdbfile);
            return path;
        }

        private void _openDB()
        {
            CurrentDBPath = _getPICLocalDBFilePath();

            // No local database, check presence of IDE X database
            if (CurrentDBPath == null || !File.Exists(CurrentDBPath))
            {
                CurrentDBPath = _getIDECrownkingFilePath();
                if (CurrentDBPath == null)
                {
                    Status = DBStatus.NoDB;
                    LastError = DBErrorCode.NoDBFile;
                    return;
                }
                Status = DBStatus.DBObso;
                LastError = DBErrorCode.NoError;
                return;
            }
            // Local database is present. Check if out-of-date versus optional IDE X database
            Status = DBStatus.DBOK;
            LastError = DBErrorCode.NoError;
            string IDEJar = _getIDECrownkingFilePath();
            if (IDEJar != null)
            {
                var timelocal = GetDBDate();
                var timeIDE = GetDBDate(IDEJar);
                if (timelocal.CompareTo(timeIDE) < 0)
                    Status = DBStatus.DBObso;
            }
        }

        // XML elements we are ignoring. This helps decrease the size of the database.
        private static string[] _unwantednodes =
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
            };

        private XDocument _defaultPruning(XDocument xdoc)
        {
            XElement xroot = xdoc?.Root;
            if (xroot == null) return null;
            if (xroot.Name.Namespace == XNamespace.None)
                return xdoc;
            if (xroot.Name.LocalName != "PIC")
                return null;

            // Remove the unwanted elements
            foreach (string name in _unwantednodes)
                xroot.Elements().Where(p => p.Name.LocalName == name).Remove();

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

            // Remove unwanted sub-elements in various elements that we have no use of.
            xroot.Descendants().Where(p => p.Name.LocalName == "AliasList").Remove();
            xroot.Descendants().Where(p => p.Name.LocalName == "StimInfo").Remove();

            return xdoc;
        }

        private bool _defaultFilter(string s)
        {
            return
                s.StartsWith("PIC16C") || s.StartsWith("PIC16F") ||
                s.StartsWith("PIC18F") && !s.Contains("J");
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Specialised default constructor for use only by derived class.
        /// </summary>
        protected PICCrownking()
        {
            _openDB();
        }

        #endregion

        #region API basic functions

        /// <summary>
        /// Gets the last error encountered.
        /// </summary>
        /// <value>
        /// The last error as a value from <see cref="DBErrorCode"/> enumeration.
        /// </value>
        public DBErrorCode LastError { get; private set; }

        /// <summary>
        /// Gets the status the PIC database
        /// </summary>
        /// <value>
        /// The database status as a value from <see cref="DBStatus"/> enumeration.
        /// </value>
        public DBStatus Status { get; private set; }

        /// <summary>
        /// Gets the Microchip Crownking PIC XML database creation date.
        /// </summary>
        /// <param name="path">(Optional) Full pathname of the database file.</param>
        /// <returns>
        /// The database's creation date.
        /// </returns>
        /// <exception cref="PICCrownkingException">If the Microchip file does not exists on the current
        ///                                         system.</exception>
        public DateTime GetDBDate(string path = null)
        {
            if (path != null)
                return File.GetCreationTime(path);
            _checkDBExist();
            return File.GetCreationTime(CurrentDBPath);
        }

        /// <summary>
        /// Gets the full pathname of the current database file.
        /// </summary>
        /// <value>
        /// The full pathname of the current database file.
        /// </value>
        public string CurrentDBPath { get; private set; } = null;

        /// <summary>
        /// Gets access to the Microchip Crownking PIC XML definition database.
        /// </summary>
        /// <remarks>
        /// The local database (if it exists) is used in favor of the Microchip MPLAB X IDE database.
        /// </remarks>
        /// <returns>
        /// The database descriptor or null if no database exists on the current system.
        /// </returns>
        public static PICCrownking GetDB()
        {
            if (currentDB == null)
            {
                currentDB = new PICCrownking();
                if (currentDB.LastError != DBErrorCode.NoError)
                    currentDB = null;
            }
            return currentDB;
        }

        /// <summary>
        /// Gets a PIC XML definition from the database.
        /// </summary>
        /// <param name="sPICName">The name of the PIC being looked for.</param>
        /// <returns>
        /// The XML document as retrieved from the ctive Microchip database. Or null if not found or no database.
        /// </returns>
        /// <remarks>
        /// </remarks>
        public XElement GetPICAsXML(string sPICName)
        {
            XElement xmlpic = null;
            string contentpath = null;
            LastError = DBErrorCode.NoSuchPIC;
            _checkDBExist();

            if (String.IsNullOrEmpty(sPICName)) return null;
            sPICName = sPICName.ToUpperInvariant();
            if (!sPICName.EndsWith(".PIC", true, CultureInfo.InvariantCulture)) sPICName += ".PIC";
            if (sPICName.StartsWith("PIC16", true, CultureInfo.InvariantCulture))
                contentpath = _contentPIC16;
            if (sPICName.StartsWith("PIC18", true, CultureInfo.InvariantCulture))
                contentpath = _contentPIC18;

            if (contentpath != null)
            {
                try
                {
                    using (ZipArchive jarfile = ZipFile.OpenRead(CurrentDBPath))
                    {
                        ZipArchiveEntry entry = jarfile.GetEntry(contentpath + "/" + sPICName);
                        if (entry != null)
                        {
                            using (var eo = entry.Open())
                                xmlpic = _defaultPruning(XDocument.Load(eo))?.Root;
                            LastError = xmlpic == null ? DBErrorCode.WrongDB : DBErrorCode.NoError;
                        }
                    }
                }
                catch
                {
                    xmlpic = null;
                }
            }

            return xmlpic;
        }

        /// <summary>
        /// Enumerates the PIC names contained in the current database.
        /// </summary>
        /// <param name="filter">A filter predicate to select categories of PIC names.</param>
        /// <returns>
        /// An enumerator that allows foreach to be used to process PIC names list in this collection.
        /// </returns>
        /// <remarks>
        /// Only PIC16 and PIC18 are listed whatever database and filter used.
        /// </remarks>
        public IEnumerable<string> EnumPICList(Func<string, bool> filter)
        {
            _checkDBExist();
            using (ZipArchive jarfile = ZipFile.OpenRead(CurrentDBPath))
            {
                foreach (var entry in jarfile.Entries)
                {
                    if (entry.FullName.StartsWith(_contentPIC16 + "/PIC16", true, CultureInfo.InvariantCulture) ||
                        entry.FullName.StartsWith(_contentPIC18 + "/PIC18", true, CultureInfo.InvariantCulture))
                    {
                        if (filter(entry.Name))
                            yield return Path.GetFileNameWithoutExtension(entry.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates all the PIC16 and PIC18 names contained in the current database.
        /// </summary>
        /// <returns>
        /// An enumerator that allows 'foreach' to be used to process PIC names list in this collection.
        /// </returns>
        /// <remarks>
        /// Only PIC16 and PIC18 are listed whatever database content is.
        /// </remarks>
        public IEnumerable<string> EnumPICList() => EnumPICList(filt => true);

        /// <summary>
        /// Forcibly updates the local database for PICs selected via a filter on their names.
        /// </summary>
        /// <remarks>
        /// Only PIC16 and PIC18 are kept whatever filter is provided.
        /// </remarks>
        /// <param name="filter">A filter function to select PICs by name.</param>
        /// <param name="pruning">(Optional) The pruning function to simplify the PIC XML definition.</param>
        public void UpdateDB(Func<string, bool> filter = null, Func<XDocument, XDocument> pruning = null)
        {
            string idecrownkingpath = _getIDECrownkingFilePath();   // The MPLAB X IDE database
            string localpath = _getPICLocalDBFilePath();       // The local database

            if (idecrownkingpath == null)
            {
                if (localpath == null || !File.Exists(localpath))
                {
                    CurrentDBPath = null;              // We have no database at all.
                    Status = DBStatus.NoDB;
                    _raiseError(DBErrorCode.NoDBFile, "No PIC database file. You may wish to install Microchip MPLAB X IDE.");
                }
                // No MPLAB X IDE database on this system. Keep the local one as-is even if not the latest version.
                CurrentDBPath = localpath;
                Status = DBStatus.DBOK;
                LastError = DBErrorCode.NoError;
                return;
            }
            // An MPLAB X IDE database exists. The caller considers we need to (re)generate the local database.
            try
            {
#if false
                XElement xpics = new XElement("PICS");   // For debugging purpose we wish a text-readable (uncompressed) version of the PIC database.
                XDocument xconcat = new XDocument(xpics);
#endif
                // Create a new local database
                using (FileStream outfile = new FileStream(localpath, FileMode.Create, FileAccess.Write))
                {
                    // Those database are compressed
                    using (ZipArchive crownkingfile = ZipFile.OpenRead(idecrownkingpath),
                                      zoutfile = new ZipArchive(outfile, ZipArchiveMode.Create))
                    {
                        // For each MPLAB X IDE entry, look only for true PIC16 and PIC18
                        foreach (var entry in crownkingfile.Entries)
                        {
                            if (entry.FullName.StartsWith(_contentPIC16 + "/PIC16", true, CultureInfo.InvariantCulture) ||
                                entry.FullName.StartsWith(_contentPIC18 + "/PIC18", true, CultureInfo.InvariantCulture))
                            {
                                // Caller may want to filter further valid PIC entries
                                if ((filter ?? _defaultFilter)(entry.Name))
                                {
                                    // Candidate to extract.
                                    XDocument xdoc;
                                    using (var eo = entry.Open())
                                    {
                                        xdoc = XDocument.Load(entry.Open());
                                        xdoc = (pruning ?? _defaultPruning)(xdoc); // Pruning of the XML tree for unwanted elements
                                    }
                                    
                                    if (xdoc != null)
                                    {
#if false
                                        xpics.Add(xdoc.Root);
#endif
                                        ZipArchiveEntry picentry = zoutfile.CreateEntry(entry.FullName);
                                        using (StreamWriter picw = new StreamWriter(picentry.Open()))
                                            xdoc.Save(picw);
                                    }
                                }
                            }
                        }
                    }
                }
#if false
                xconcat.Save(localpath.Replace(".zip", ".xml"), SaveOptions.None);
#endif
                CurrentDBPath = localpath;
                Status = DBStatus.DBOK;
                LastError = DBErrorCode.NoError;
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Update DB : {ex.StackTrace}");
            }
        }

        #endregion

    }

}
