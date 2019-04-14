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

using Reko.Core;
using Reko.Core.Services;
using Reko.Tools.SignatureGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Reko.Loading
{
    public class ByteSignatureMatcher
    {
        Dictionary<int, List<SigItem>> SignatureMap;
        IServiceProvider services;
        DecompilerEventListener eventListener;
        IFileSystemService fsSvc;

        public ByteSignatureMatcher(IServiceProvider services)
        {
            SignatureMap = new Dictionary<int, List<SigItem>>();
            this.eventListener = services.RequireService<DecompilerEventListener>();
            fsSvc = services.RequireService<IFileSystemService>();
        }


        // <summary>
        /// Loads the signature file into the signature tree for match processing
        /// </summary>
        /// <param name="fileName">The filename to load.</param>
        /// <returns>True if the signature was loaded correctly</returns>
        public bool LoadByteSignatures(string fileName)
        {
            if (fsSvc.FileExists(fileName))
            {
                try
                {
                    string filename = fileName;

                    //Open the file written above and read values from it.
                    Stream stream = File.Open(fileName, FileMode.Open);
                    BinaryFormatter bformatter = new BinaryFormatter();
                    List<SigItem> Signatures = (List<SigItem>) bformatter.Deserialize(stream);
                    stream.Close();

                    foreach (SigItem item in Signatures)
                    {
                        List<SigItem> lengthList;
                        if (SignatureMap.ContainsKey(item.dataLength) == false)
                        {
                            lengthList = new List<SigItem>();
                            SignatureMap.Add(item.dataLength, lengthList);
                        }
                        else
                        {
                            lengthList = SignatureMap[item.dataLength];
                        }
                        lengthList.Add(item);
                    }

                    // Commented out so that dups are not removed, instead the user will get informed there 
                    // are a number of options that match, so that the user can decide to update manaually.
                    //RemoveDuplicateSigs();
                    return true;
                }
                catch (Exception ex)
                {
                    eventListener.Error(new NullCodeLocation(""), ex, "Error loading signature files");
                    return false;
                }
            }
            return false;
        }


        // <summary>
        /// This check is to see if a byte signature of the specfied length exists. This
        /// done to improve the processing of producer processing
        /// </summary>
        /// <param name="length">L</param>
        /// <returns>True if a signature of the specified length exists</returns>
        bool IsThereSignitureOfLength(int length)
        {
            return SignatureMap.ContainsKey(length);
        }


        // <summary>
        /// This is used to see if a byte match can be found, in order to not to slow the performace a check is carried out 
        /// to see if any signatures exist of that size, if there are some a byte comparision check is carried out whilst also ignoring
        /// bytes whcih are demmed as holding relocatable values
        /// </summary>
        /// <param name="currentName">Current name of the procedure</param>
        /// <param name="data">the bute sequence of the procedure</param>
        /// <param name="dataLength">length of the byte stream</param>
        /// <returns>IF a single match is found, it returns the name by which the procedure is know from within the library</returns>
        public string FindMatchingSignitureStart(ICodeLocation currentName, byte[] data, int dataLength)
        {
            // Check if we have a signiture of this length, this will speed up the processing
            if (IsThereSignitureOfLength(dataLength))
            {
                List<SigItem> lengthList = SignatureMap[dataLength];

                List<SigItem> matches = new List<SigItem>();
                foreach (SigItem item in lengthList)
                {
                    if (item.DoesSignatureMatchBytes(data))
                    {
                        matches.Add(item);
                        //return item.methodName;
                    }
                }

                if(matches.Count == 1)
                {
                    return matches[0].methodName;
                }
                else if(matches.Count > 1)
                {
                    // Output to the log and tell the user they have a number of options
                    //eventListener.
                    string message = "The address function has mutiple options form signature analysis, the options are :- ";

                    foreach(SigItem item in matches)
                    {
                        message += ", " + item.methodName + " form " + item.libraryName + " ";
                    }
                    //$TODO show user a dialog with choices if they click on the diagnostic
                    var dc = services.RequireService<DecompilerEventListener>();
                    dc.Warn(currentName, message);
                }

                return "";
            }
            return "";
        }
    }
}

