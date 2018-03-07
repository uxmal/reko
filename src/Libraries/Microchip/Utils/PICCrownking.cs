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

namespace Reko.Libraries.Microchip
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Reflection;
    using System.Xml.Linq;
    using System.Linq;
    using System.Text.RegularExpressions;


    /// <summary>
    /// This factory class provides methods for loading Microchip PIC definition (XML) from the MPLAB X IDE (a.k.a. Crownking) database or a local copy of it.
    /// </summary>
    public class PICCrownking
    {
        #region Privates

        private const string localdbfile = "picdb.zip";
        private const string contentPIC16 = @"content/edc/16xxxx";
        private const string contentPIC18 = @"content/edc/18xxxx";

        private static PICCrownking currentDB = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Specialized default constructor for use only by derived class.
        /// </summary>
        protected PICCrownking()
        {
            OpenDB();
        }

        #endregion

        #region Methods

        private PICCrownkingException RaiseError(DBErrorCode err, string msg)
        {
            LastError = err;
            return new PICCrownkingException(err, msg);
        }

        private void CheckDBExist()
        {
            if (CurrentDBPath == null || !File.Exists(CurrentDBPath))
            {
                throw RaiseError(DBErrorCode.NoDBFile, "No Microchip XML PIC definitions available on this system");
            }
        }

        private string GetPICLocalDBFilePath()
        {
            Assembly CrownkingAssembly;
            CrownkingAssembly = Assembly.GetAssembly(this.GetType());
            string sDir = Path.GetDirectoryName(CrownkingAssembly.Location);
            string path = Path.Combine(sDir, localdbfile);
            return path;
        }

        private void OpenDB()
        {
            CurrentDBPath = GetPICLocalDBFilePath();

            // No local database, check presence of IDE X database
            if (CurrentDBPath == null || !File.Exists(CurrentDBPath))
            {
                Status = DBStatus.NoDB;
                LastError = DBErrorCode.NoDBFile;
                return;
            }
            // Local database is present. Check if out-of-date versus optional IDE X database
            Status = DBStatus.DBOK;
            LastError = DBErrorCode.NoError;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Gets the last error encountered.
        /// </summary>
        /// <value>
        /// The last error as a value from <see cref="DBErrorCode"/> enumeration.
        /// </value>
        public static DBErrorCode LastError { get; private set; }

        /// <summary>
        /// Gets the status the PIC database
        /// </summary>
        /// <value>
        /// The database status as a value from <see cref="DBStatus"/> enumeration.
        /// </value>
        public static DBStatus Status { get; private set; }

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
                if (LastError != DBErrorCode.NoError)
                    currentDB = null;
            }
            return currentDB;
        }

        /// <summary>
        /// Gets a PIC XML definition from the database.
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
            LastError = DBErrorCode.NoSuchPIC;
            CheckDBExist();

            if (String.IsNullOrEmpty(sPICName))
                return null;
            sPICName = sPICName.ToUpperInvariant();
            if (!sPICName.EndsWith(".PIC", true, CultureInfo.InvariantCulture)) sPICName += ".PIC";
            if (sPICName.StartsWith("PIC16", true, CultureInfo.InvariantCulture))
                contentpath = contentPIC16;
            if (sPICName.StartsWith("PIC18", true, CultureInfo.InvariantCulture))
                contentpath = contentPIC18;

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
                                xmlpic = XDocument.Load(eo)?.Root;
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
        /// An enumerator that allows <code lang="C#">foreach</code> to be used to process PIC names list in this collection.
        /// </returns>
        /// <remarks>
        /// Only PIC16 and PIC18 are listed whatever database and filter used.
        /// </remarks>
        public IEnumerable<string> EnumPICList(Func<string, bool> filter)
        {
            CheckDBExist();
            var zlist = new List<ZipArchiveEntry>();
            using (ZipArchive jarfile = ZipFile.OpenRead(CurrentDBPath))
            {
                zlist.AddRange(jarfile.Entries.OrderByNatural(e => e.Name));
            }
            
            foreach (var entry in zlist)
            {
                if (entry.FullName.StartsWith(contentPIC16 + "/PIC16", true, CultureInfo.InvariantCulture) ||
                    entry.FullName.StartsWith(contentPIC18 + "/PIC18", true, CultureInfo.InvariantCulture))
                {
                    if (filter(entry.Name))
                        yield return Path.GetFileNameWithoutExtension(entry.Name);
                }
            }
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

        #endregion

    }

}
