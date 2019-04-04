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
using Reko.ImageLoaders.Coff;
using RekoSig;
using System;
using System.Collections.Generic;
using System.IO;
using Reko.ImageLoaders.Ar;
using Reko.ImageLoaders.Elf;

namespace makesigs
{

    class SignitureGenerator
    {
        string InputFilePath;
        FileBuffer.FileTypes FileType;
        SignatureCreator signiture;
        FileBuffer fileBuffer;

        internal SignitureGenerator(string filename) 
        {
            InputFilePath = filename;
            FileType = FileBuffer.FileTypes.UNKNOWN;
            fileBuffer = null;
            signiture = new SignatureCreator();
        }


        internal bool GenerateSigniture(string destFile)
        {
            if (ReadFile())
            {
                FileType = fileBuffer.GetFileType();
                if ((FileType == FileBuffer.FileTypes.FILETYPE_LIBRARY) | (FileType == FileBuffer.FileTypes.FILETYPE_OMFLIBRARY))
                {
                    // Input file is a library
                    ProcessFile();
                    signiture.SaveSignatures(destFile);
                    return true;
                }
                else
                {
                    Console.WriteLine("Unable to process the given file type");
                }
            }
            else
            {
                // error message
                Console.WriteLine("Error reading the input file");
            }
            return false;
        }


        internal void AddToSignitureTree(List<SignitureEntry> signitures)
        {
            foreach (SignitureEntry entry in signitures)
            {
                signiture.AddMethodSigniture(Path.GetFileName(InputFilePath), entry.Name, entry.Data, entry.Length, entry.MissBytes);
            }
        }

        private bool ReadFile()
        {
            using (FileStream fs = File.Open(InputFilePath, FileMode.Open))
            {
                
                byte[] dataStream = new byte[fs.Length];
                int read = fs.Read(dataStream, 0, dataStream.Length);
                if (read == dataStream.Length)
                {
                    fileBuffer = new FileBuffer();
                    fileBuffer.SetData(dataStream, 0, (int)fs.Length);
                    return true;
                }
            }
            return false;
        }


        internal void ProcessFile()
        {
           
            // Dispatch according to library type
            switch (FileType)
            {
                case FileBuffer.FileTypes.FILETYPE_LIBRARY:
                {
                    // Extracts contents of UNIX style library
                    ExtractArchive();
                }
                break;                        

                case FileBuffer.FileTypes.FILETYPE_OMFLIBRARY:
                {
                    // Extracts contents of OMF style library
                    ExtractOMF();
                }
                break;                        

                default:
                Console.WriteLine("Objconv program internal inconsistency");        // Should not occur
                break;
            }
        }


        private void ExtractArchive()
        {
            Console.WriteLine("\nDump of Archive library {0}", InputFilePath);
            
            fileBuffer.GetDataSize();

            byte[] tmp = new byte[fileBuffer.GetDataSize() - 8];
            Buffer.BlockCopy(fileBuffer.GetData(), 8, tmp, 0, fileBuffer.GetDataSize() - 8);
            ArLoader ar = new ArLoader(null, InputFilePath, tmp);
    
            for (int index = 0; index < ar.NumberArPackages; index++)
            {
                ArPackage pack = ar.GetPackage(index);

                switch (pack.TypeOfData)
                {
                    case ArFileType.FILETYPE_ELF:
                    {
                        // Use ELF Image loader to get the data
                       
                        //ElfImageLoader elf = new ElfImageLoader(null, pack.PackageName, pack.PackageData);
                       // elf.LoadElfIdentification();
                       // ElfLoader elfLoader = elf.CreateLoader();



                        //List<String> names = elf.GetPublicNames();
                        //foreach (string name in names)
                        //{
                        //    Console.WriteLine(name);
                        //}

                        // List<SignitureEntry> signitures = elf.GetSignitures();
                        // Add to the signiture tree
                        //  AddToSignitureTree(signitures);
                    }
                    break;


                    case ArFileType.FILETYPE_COFF:
                    {
                        CoffLoader cl = new CoffLoader(null, pack.PackageName, pack.PackageData);

                        List<String> names = cl.GetPublicNames();
                        foreach (string name in names)
                        {
                            Console.WriteLine(name);
                        }

                        List<SignitureEntry> signitures = cl.GetSignitures();
                        // Add to the signiture tree
                        AddToSignitureTree(signitures);
                    }
                    break;

                    case ArFileType.FILETYPE_MACHO_LE:
                    {
                        // Use MACHO Image loader to get the data
                        ///TODO
                        ///

                        //List<SignitureEntry> signitures = mac.GetSignitures();
                        // Add to the signiture tree
                        //AddToSignitureTree(signitures);
                    }
                    break;

                    default:
                        Console.WriteLine("\n   Cannot extract symbol names from  Archive package {0}", pack.PackageName);
                    break;
                }
            }  
        }
 


        void ExtractOMF()
        {
            Console.WriteLine("\nDump of OMF library {0}", InputFilePath);

            fileBuffer.GetDataSize();

            byte[] tmp = new byte[fileBuffer.GetDataSize() - 8];
            Buffer.BlockCopy(fileBuffer.GetData(), 8, tmp, 0, fileBuffer.GetDataSize() - 8);
            
        }

    }
}
