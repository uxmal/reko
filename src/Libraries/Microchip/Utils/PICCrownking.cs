#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Linq;
using System.Linq;

namespace Reko.Libraries.Microchip
{
    using V1;
    using static PICConstants;

    /// <summary>
    /// This factory class provides methods for loading Microchip PIC definition (XML) from the MPLAB X IDE (a.k.a. Crownking) database or a local copy of it.
    /// </summary>
    public sealed class PICCrownking
    {
        private static PICCrownking currentDB = null;
        private static PartInfo partslist = null;

        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private PICCrownking()
        {
            OpenDB();
        }


        #region Local properties/methods

        private PICCrownkingException RaiseError(DBErrorCode err, string msg)
        {
            LastError = err;
            return new PICCrownkingException(err, msg);
        }

        private void SetError(DBErrorCode dberr, DBStatus stat, string errMsg)
        {
            LastError = dberr;
            Status = stat;
            LastErrMsg = errMsg;
        }

        private void CheckDBExist()
        {
            if (CurrentDBPath == null || !File.Exists(CurrentDBPath))
            {
                SetError(DBErrorCode.NoDBFile,
                         DBStatus.NoDB,
                         nameof(CheckDBExist) + ": " + (CurrentDBPath == null
                                                            ? "Unable to get PIC DB file pathname"
                                                            : $"PIC DB file '{CurrentDBPath}' not found"));
                throw RaiseError(DBErrorCode.NoDBFile, "No Microchip XML PIC definitions available on this system");
            }
        }

        private string GetPICLocalDBFilePath()
        {
            Assembly CrownkingAssembly;
            CrownkingAssembly = Assembly.GetAssembly(GetType());
            string sDir = Path.GetDirectoryName(CrownkingAssembly.Location);
            string path = Path.Combine(sDir, LocalDBFilename);
            if (!File.Exists(path))
            {
                sDir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                path = Path.Combine(sDir, LocalDBFilename);
            }
            return path;
        }

        private void OpenDB()
        {
            SetError(DBErrorCode.NoError, DBStatus.DBOK, string.Empty);
            CurrentDBPath = GetPICLocalDBFilePath();

            // No local database, check presence of IDE X database
            if (CurrentDBPath == null || !File.Exists(CurrentDBPath))
            {
                SetError(DBErrorCode.NoDBFile,
                         DBStatus.NoDB,
                         nameof(OpenDB) + ": " + (CurrentDBPath == null
                                                    ? "Unable to get PIC DB file pathname"
                                                    : $"PIC DB file '{CurrentDBPath}' not found"));
            }
        }

        private bool AcceptPICEntry(ZipArchiveEntry picentry, Func<string, bool> filter)
        {
            return ((picentry.FullName.StartsWith(ContentPIC16Path + "/PIC16", true, CultureInfo.InvariantCulture) ||
                     picentry.FullName.StartsWith(ContentPIC18Path + "/PIC18", true, CultureInfo.InvariantCulture))
                     && filter(picentry.Name));
        }

        private PartInfo PartsInfo
        {
            get
            {
                CheckDBExist();
                if (partslist == null)
                {
                    try
                    {
                        using (ZipArchive picdbzipfile = ZipFile.OpenRead(CurrentDBPath))
                        {
                            var entry = picdbzipfile.GetEntry(PartsinfoFilename);
                            if (entry != null)
                            {
                                using (var eo = entry.Open())
                                {
                                    partslist = XDocument.Load(eo).Root.ToObject<PartInfo>();
                                }
                            }
                        }
                    }
                    catch
                    {
                        partslist = null;
                    }
                }
                return partslist;
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
        /// <remarks>
        /// </remarks>
        public XElement GetPICAsXML(string sPICName)
        {
            XElement xmlpic = null;
            string contentpath = null;
            SetError(DBErrorCode.NoSuchPIC, DBStatus.DBOK, $"PIC '{sPICName}' not found in database");
            CheckDBExist();

            if (String.IsNullOrEmpty(sPICName))
                return null;
            sPICName = sPICName.ToUpperInvariant();
            if (!sPICName.EndsWith(".PIC", true, CultureInfo.InvariantCulture))
                sPICName += ".PIC";
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
                                SetError(DBErrorCode.WrongDB, DBStatus.DBOK, "Invalid PIC database format");
                            else
                                SetError(DBErrorCode.NoError, DBStatus.DBOK, string.Empty);
                        }
                    }
                }
                catch (Exception ex)
                {
                    xmlpic = null;
                    SetError(DBErrorCode.NoSuchPIC, DBStatus.DBOK, $"{nameof(GetPICAsXML)}: {ex.Message}");
                }
            }

            return xmlpic;
        }

        /// <summary>
        /// Gets a PIC XML definition from the database given the processor ID of the PIC.
        /// </summary>
        /// <param name="procID">Identifier for the processor.</param>
        /// <returns>
        /// The XML document as retrieved from the active Microchip database. Or null if not found or no
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
        /// <remarks>
        /// Only PIC16 and PIC18 are listed whatever database and filter used.
        /// </remarks>
        public IEnumerable<string> EnumPICList(Func<string, bool> filter)
        {
            if (PartsInfo == null)
                yield break;
            foreach (var part in PartsInfo.PICNamesList(filter))
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
        public (string Name, int ID)? GetPICInfo(string picName)
            => PartsInfo?.Parts.Where(p => p.Name == picName).Select(p => (p.Name, p.ProcID)).FirstOrDefault();

        /// <summary>
        /// Gets PIC information given its processor ID.
        /// </summary>
        /// <param name="picID">Identifier for the PIC.</param>
        public (string Name, int ID)? GetPICInfo(int picID)
            => PartsInfo?.Parts.Where(p => p.ProcID == picID).Select(p => (p.Name, p.ProcID)).FirstOrDefault();

        #endregion

    }

}
