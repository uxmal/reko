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
using RekoSig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Reko.Loading
{
    public class SignatureMatcher
    {
        Dictionary<int, List<SigItem>> SignatureMap;
        IServiceProvider services;
        DecompilerEventListener eventListener;

        public SignatureMatcher(IServiceProvider services)
        {
            SignatureMap = new Dictionary<int, List<SigItem>>();
            this.eventListener = services.RequireService<DecompilerEventListener>();
        }


        // <summary>
        /// Loads the signature file into the signature tree for match processing
        /// </summary>
        /// <param name="fileName">The filename to load.</param>
        /// <returns>True if the signature was loaded correctly</returns>
        public bool LoadSignitures(string fileName)
        {
            if (File.Exists(fileName))
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
        /// This check is to see if a signature of the specfied length exists. This
        /// done to improve the processing of producer processing
        /// </summary>
        /// <param name="length">L</param>
        /// <returns>True if a signature of the specified lenght exists</returns>
        bool IsThereSignitureOfLength(int length)
        {
            return SignatureMap.ContainsKey(length);
        }



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

                    var dc = services.RequireService<DecompilerEventListener>();
                    dc.Warn(currentName, message);
                }

                return "";
            }
            return "";
        }


        private void RemoveDuplicateSigs()
        {
            foreach (List<SigItem> lengthList in SignatureMap.Values)
            {
                List<int> removeList = new List<int>();

                if (lengthList.Count > 1)
                {
                    for (int index = 0; index < lengthList.Count; index++)
                    {
                        for (int index2 = index + 1; index2 < lengthList.Count; index2++)
                        {
                            // compare the 2 sigItems
                            if (lengthList[index].DoesSignatureMatchBytes(lengthList[index2].data))
                            {
                                removeList.Add(index2);
                            }
                        }
                    }

                    if (removeList.Count > 0)
                    {
                        // get the list in order and reverse it, so it will be easier to remove from the list
                        removeList.Sort();
                        removeList.Reverse();

                        foreach (int a in removeList)
                        {
                            lengthList.RemoveAt(a);
                        }
                    }
                }
            }
        }
    }
}

