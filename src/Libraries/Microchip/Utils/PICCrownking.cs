#region License
/* 
 * Copyright (c) 2017-2020 Christian Hostelet.
 *
 * The contents of this file are subject to the terms of the Common Development
 * and Distribution License (the License), or the GPL v2, or (at your option)
 * any later version. 
 * You may not use this file except in compliance with the License.
 *
 * You can obtain a copy of the License at http://www.gnu.org/licenses/gpl-2.0.html.
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
 * If applicable, add the following below the header, with the fields
 * enclosed by brackets [] replaced by your own identifying information:
 * "Portions Copyrighted (c) [year] [name of copyright owner]"
 *
 */

#endregion

namespace Reko.Libraries.Microchip
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;

    using static Reko.Libraries.Microchip.PICConstants;

    /// <summary>
    /// This factory class provides methods for loading Microchip PIC16 and PIC18 microcontrollers definitions (XML).
    /// </summary>
    public sealed class PICCrownking
    {
        private static PICCrownking currentDB = null;
        private static PICPartInfo partsinfo = null;

        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private PICCrownking() 
            => openDB();


        #region Local properties/methods

        private PICCrownkingException raiseError(DBErrorCode err, string msg)
        {
            LastError = err;
            return new PICCrownkingException(err, msg);
        }

        private void setError(DBErrorCode dberr, DBStatus stat, string errMsg)
        {
            LastError = dberr;
            Status = stat;
            LastErrMsg = errMsg;
        }

        private void checkDBExist()
        {
            if (CurrentDBPath == null || !File.Exists(CurrentDBPath))
            {
                setError(DBErrorCode.NoDBFile,
                         DBStatus.NoDB,
                         nameof(checkDBExist) + ": " + (CurrentDBPath == null
                                                            ? "Unable to get PIC database file pathname"
                                                            : $"PIC database file '{CurrentDBPath}' not found"));
                throw raiseError(DBErrorCode.NoDBFile, "No Microchip XML PIC definitions available on this system.");
            }
        }

        private string getPICLocalDBFilePath()
        {
            Assembly CrownkingAssembly;
            CrownkingAssembly = Assembly.GetAssembly(GetType());
            var sDir = Path.GetDirectoryName(CrownkingAssembly.Location);
            var path = Path.Combine(sDir, LocalDBFilename);
            if (!File.Exists(path))
            {
                sDir = AppDomain.CurrentDomain.BaseDirectory;
                path = Path.Combine(sDir, LocalDBFilename);
            }
            return path;
        }

        private void openDB()
        {
            setError(DBErrorCode.NoError, DBStatus.DBOK, string.Empty);
            CurrentDBPath = getPICLocalDBFilePath();

            // No local database, check presence of IDE X database
            if (CurrentDBPath == null || !File.Exists(CurrentDBPath))
            {
                setError(DBErrorCode.NoDBFile,
                         DBStatus.NoDB,
                         nameof(openDB) + ": " + (CurrentDBPath == null
                                                    ? "Unable to get PIC database file pathname"
                                                    : $"PIC database file '{CurrentDBPath}' not found"));
            }
        }

        private PICPartInfo partsInfo
        {
            get
            {
                if (partsinfo == null)
                {
                    if (CurrentDBPath == null || !File.Exists(CurrentDBPath))
                        return null;

                    try
                    {
                        using (ZipArchive picdbzipfile = ZipFile.OpenRead(CurrentDBPath))
                        {
                            var entry = picdbzipfile.GetEntry(PartsInfoFilename);
                            if (entry != null)
                            {
                                using (var eo = entry.Open())
                                {
                                    partsinfo = XDocument.Load(eo).Root.ToObject<PICPartInfo>();
                                }
                            }
                        }
                    }
                    catch (PICCrownkingException)
                    {
                        partsinfo = null;
                        throw;
                    }
                    catch (Exception ex)
                    {
                        setError(DBErrorCode.WrongDB, DBStatus.DBOK, $"Got exception retrieving {nameof(partsInfo)} file: {ex.Message}.");
                        partsinfo = null;
                    }
                }
                return partsinfo;
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Gets the last error code encountered.
        /// </summary>
        /// <value>
        /// The last error as a value from <see cref="DBErrorCode"/> enumeration.
        /// </value>
        public static DBErrorCode LastError { get; private set; } = DBErrorCode.NoError;

        /// <summary>
        /// Gets the last error message.
        /// </summary>
        /// <value>
        /// The last error message.
        /// </value>
        public static string LastErrMsg { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the status the PIC database
        /// </summary>
        /// <value>
        /// The database status as a value from <see cref="DBStatus"/> enumeration.
        /// </value>
        public static DBStatus Status { get; private set; } = DBStatus.DBOK;

        /// <summary>
        /// Gets the full pathname of the current database file.
        /// </summary>
        /// <value>
        /// The full pathname of the current database file.
        /// </value>
        public static string CurrentDBPath { get; private set; } = null;

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
                if (LastError != DBErrorCode.NoError)
                {
                    currentDB = null;
                }
            }
            return currentDB;
        }

        /// <summary>
        /// Gets a PIC XML definition from the database given the name of the PIC.
        /// </summary>
        /// <param name="sPICName">The name of the PIC being looked for.</param>
        /// <returns>
        /// The XML document as retrieved from the active Microchip database. Or null if not found or no database.
        /// </returns>
        public XElement GetPICAsXML(string sPICName)
        {
            XElement xmlpic = null;
            string contentpath = null;
            setError(DBErrorCode.NoSuchPIC, DBStatus.DBOK, $"PIC '{sPICName}' not found in database");
            checkDBExist();

            if (string.IsNullOrEmpty(sPICName))
                return null;
            sPICName = sPICName.ToUpperInvariant();
            if (!sPICName.EndsWith(".PIC", true, CultureInfo.InvariantCulture))
                sPICName += ".PIC";
            if (sPICName.StartsWith("PIC12", true, CultureInfo.InvariantCulture))  // FUTURE?  Provisions to accept PIC12 MCUs with PIC16 architecture  
                contentpath = ContentPIC16Path;
            if (sPICName.StartsWith("PIC16", true, CultureInfo.InvariantCulture))
                contentpath = ContentPIC16Path;
            if (sPICName.StartsWith("PIC18", true, CultureInfo.InvariantCulture))
                contentpath = ContentPIC18Path;

            if (contentpath != null)
            {
                try
                {
                    using (ZipArchive picdbzipfile = ZipFile.OpenRead(CurrentDBPath))
                    {
                        ZipArchiveEntry entry = picdbzipfile.GetEntry(contentpath + "/" + sPICName);
                        if (entry != null)
                        {
                            using (var eo = entry.Open())
                                xmlpic = XDocument.Load(eo)?.Root;
                            if (xmlpic == null)
                                setError(DBErrorCode.WrongDB, DBStatus.DBOK, "Invalid PIC database format");
                            else
                                setError(DBErrorCode.NoError, DBStatus.DBOK, string.Empty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    xmlpic = null;
                    setError(DBErrorCode.NoSuchPIC, DBStatus.DBOK, $"{nameof(GetPICAsXML)}: {ex.Message}");
                }
            }

            return xmlpic;
        }

        /// <summary>
        /// Gets a PIC XML definition from the database given the processor ID of the PIC.
        /// </summary>
        /// <param name="procID">Identifier for the PIC processor.</param>
        /// <returns>
        /// The XML document as retrieved from the database. Or null if not found or no
        /// database.
        /// </returns>
        public XElement GetPICAsXML(int procID)
        {
            var inf = GetPICInfo(procID);
            return (inf.HasValue ? GetPICAsXML(inf.Value.Name) : null);
        }

        /// <summary>
        /// Enumerates the PIC names contained in the current database.
        /// </summary>
        /// <param name="filter">A filter predicate to select categories of PIC names.</param>
        /// <returns>
        /// An enumerator that allows <code lang="C#">foreach</code> to be used to process PIC names list in this collection.
        /// </returns>
        public IEnumerable<string> EnumPICList(Func<string, bool> filter)
        {
            if (partsInfo is null)
                yield break;
            foreach (var part in partsInfo.PICNamesList(filter))
                yield return part;

        }

        /// <summary>
        /// Enumerates all the PIC16 and PIC18 names contained in the current database.
        /// </summary>
        /// <returns>
        /// An enumerator that allows <code lang="C#">foreach</code> to be used to process PIC names list in this collection.
        /// </returns>
        /// <remarks>
        /// Only PIC16 and PIC18 are listed whatever database content is.
        /// </remarks>
        public IEnumerable<string> EnumPICList()
            => EnumPICList(filt => true);

        /// <summary>
        /// Gets PIC information given its name.
        /// </summary>
        /// <param name="picName">Name of the PIC.</param>
        public (string Name, uint ID)? GetPICInfo(string picName)
            => partsInfo?.Parts.Where(p => p.Name == picName).Select(p => (p.Name, p.ProcID)).FirstOrDefault();

        /// <summary>
        /// Gets PIC information given its processor ID.
        /// </summary>
        /// <param name="picID">Identifier for the PIC.</param>
        public (string Name, uint ID)? GetPICInfo(int picID)
            => partsInfo?.Parts.Where(p => p.ProcID == picID).Select(p => (p.Name, p.ProcID)).FirstOrDefault();

        #endregion

    }

}
