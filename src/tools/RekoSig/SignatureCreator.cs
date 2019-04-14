#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Reko.Tools.SignatureGenerator
{
    public class SignatureCreator
    {

        List<SigItem> Signatures;

        public SignatureCreator()
        {
            Signatures = new List<SigItem>();
        }


        // <summary>
        /// Saves the signature data to the specified filename
        /// </summary>
        /// <param name="fileName">The filename to load.</param>
        /// <returns>True if the save was successful</returns>
        public bool SaveSignatures(string fileName)
        {
            // Delete the file if it exists.
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            try
            {
                Stream stream = File.Open(fileName, FileMode.Create);
                BinaryFormatter bformatter = new BinaryFormatter();

                bformatter.Serialize(stream, Signatures);
                stream.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        // <summary>
        /// Adds a method signature to the list and tree.
        /// </summary>
        /// <param name="fileName">The filename to load.</param>
        /// <param name="methodName">Name of the library method</param>
        /// <param name="data">The procedure binary data</param>
        /// <param name="dataLength">Lenght of the procedure binary data</param>
        /// <returns>void</returns>
        public void AddMethodSigniture(string filename, string methodName, byte[] data, int dataLength, List<uint> MissBytes)
        {
            //Create a sig node item
            SigItem newItem = new SigItem();
            newItem.libraryName = filename;
            newItem.methodName = methodName;
            newItem.data = data;
            newItem.dataLength = dataLength;
            newItem.SkipDataItems = MissBytes;

            // Add the node item to the list of signatures
            Signatures.Add(newItem);
        }

    }
}
