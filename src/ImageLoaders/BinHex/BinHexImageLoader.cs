using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Decompiler.ImageLoaders.BinHex
{
    public class BinHexImageLoader
    {
#if ERROR
        /**
         * Parsing .HQX format.
         * For more details see Appendix A. The BinHex format
         * at the end of RFC 1741.
         *
         * <HR WIDTH=25%>
         * <DL>
         *  <DT><B>Public RCS Heading:</B>
         *    <DD>$Id: JavaBinHex.java,v 1.4 1999/04/07 21:36:07 fox Exp fox $</DD>
         *  </DT>
         * </DL>
         *
         * <DL>
         *  <DT><B>Core Java:</B>
         *   <DD> java.util.*</DD>
         *   <DD> java.io.*</DD>
         *  </DT>
         * </DL>
         *
         * <HR WIDTH=10%>
         * @version $Revision: 1.4 $
         * @author $Author: fox $
         * @since Jdk 1.2.
         *
         */

        private char[] outputFileName = new char[256];
        private BufferedWriter bw = null;

        private boolean decrypt = false;

        private string binHexChars = "!\"#$%&'()*+,-012345689@ABCDEFGHIJKLMNPQRSTUVXYZ[`abcdefhijklmpqr";

        private char[] binHexTable = { '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '0', '1', '2', '3', '4', '5', '6', '8', '9', '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'X', 'Y', 'Z', '[', '`', 'a', 'b', 'c', 'd', 'e', 'f', 'h', 'i', 'j', 'k', 'l', 'm', 'p', 'q', 'r' };

        private byte mapCharToByte[] {
            -1, -1, -1, -1, -1, -1, -1, -1,  -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1,  -1, -1, -1, -1, -1, -1, -1, -1, 
            -1,  0,  1,  2,  3,  4,  5,  6,   7,  8,  9, };
        // Static for decodeBinHexChar
        private int[] currentBitsField = new int[8];
        private int[] previousBitsField = new int[8];
        private boolean previousCompleted = false;
        private int numBits = 0;

        // Static for readBinHexChar
        private int numOfRepeat = 0;
        private int repeatedChar = 0;

        public BinHexImageLoader(string filename)
        {
            TextReader br = null;

            br = new StreamReader(filename, Encoding.ASCII);

            Debug.WriteLine("Starting parsing for file " + filename.toLowerCase());

            int lenghtOfFileName = readBinHexChar(br); // Length of FileName

            Debug.Write("FileName : ");
            for (int i = 0; i < lenghtOfFileName; i++)
            {
                int fileNameChar = readBinHexChar(br);
                outputFileName[i] = (char) fileNameChar;
                Debug.Write((char) fileNameChar);
            }
            Debug.WriteLine("");

            try
            {
                bw = new BufferedWriter(new FileWriter(new String(outputFileName)));
            }
            catch (IOException e)
            {
                Debug.WriteLine("Error while writing file " + outputFileName);
                System.exit(1);
            }

            Debug.Write("Version : ");
            Debug.WriteLine(readBinHexChar(br));

            Debug.Write("Type : ");
            for (int i = 0; i < 4; i++)
                Debug.Write(readBinHexChar(br) + " ");
            Debug.WriteLine("");

            Debug.Write("Creator : ");
            for (int i = 0; i < 4; i++)
                Debug.Write(readBinHexChar(br) + " ");
            Debug.WriteLine("");

            Debug.Write("Flag : ");
            Debug.WriteLine((long) ((readBinHexChar(br) << 8) + readBinHexChar(br)));

            Debug.Write("Length of Data Fork : ");
            long lenghtOfData = (readBinHexChar(br) << 24) + (readBinHexChar(br) << 16) + (readBinHexChar(br) << 8) + (readBinHexChar(br));
            Debug.WriteLine(lenghtOfData);

            Debug.Write("Length of Resource Fork : ");
            long lenghtOfRsrc = (readBinHexChar(br) << 24) + (readBinHexChar(br) << 16) + (readBinHexChar(br) << 8) + (readBinHexChar(br));
            Debug.WriteLine(lenghtOfRsrc);

            Debug.Write("CRC : ");
            Debug.WriteLine((long) ((readBinHexChar(br) << 8) + readBinHexChar(br)));

            Debug.WriteLine("Data Fork : ");
            for (long i = 0; i < lenghtOfData; i++)
            {
                int dataChar = readBinHexChar(br);
                try
                {
                    if (dataChar == 13)
                    {
                        Debug.WriteLine("");
                        bw.newLine();
                    }
                    else
                    {
                        Debug.Write((char) dataChar);
                        bw.write(dataChar);
                    }
                }
                catch (IOException e)
                {
                    Debug.WriteLine(e);
                }
            }

            Debug.Write("CRC : ");
            Debug.WriteLine((long) ((readBinHexChar(br) << 8) + readBinHexChar(br)));

            Debug.WriteLine("Resource Fork : ");
            for (long i = 0; i < lenghtOfData; i++)
            {
                int currentChar = readBinHexChar(br);
                Debug.Write(currentChar + " ");
            }
            Debug.WriteLine("");

            Debug.Write("CRC : ");
            Debug.WriteLine((long) ((readBinHexChar(br) << 8) + readBinHexChar(br)));

            try
            {
                bw.flush();
                bw.close();
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
            }

            Debug.WriteLine("Parsing Done Successfully");
        }

        private int readBinHexChar(TextReader br)
        {
            int readedChar = 0;
            if (numOfRepeat != 0)
            {
                numOfRepeat -= 1;
                return repeatedChar;
            }
            for (; ; )
            {
                try
                {
                    readedChar = br.read();
                }
                catch (IOException e)
                {
                    Debug.WriteLine("Error : Unexpected eof");
                    System.exit(1);
                }

                if (readedChar == 0)
                    return -1;

                if (readedChar == '\n')
                {
                    // Skip
                }
                else
                {
                    // Parsing
                    // Suppress comment
                    if ((readedChar == '(') && (decrypt == false))
                    {
                        try
                        {
                            br.readLine();
                        }
                        catch (IOException e)
                        {
                            Debug.WriteLine("Error : Unexpected eof");
                            System.exit(1);
                        }
                    }
                    else
                    {
                        // Main stream
                        if ((readedChar == ':') && (decrypt == true))
                        {
                            return -1;
                        }
                        if ((readedChar == ':') || (decrypt == true))
                        {
                            int value = decodeBinHexChar((char) readedChar);
                            decrypt = true;
                            if (value == -1)
                            {
                                // another char must be read to complet current byte
                            }
                            else
                            {
                                // But if it's $90 it could be a compressed sequence
                                if (value == 0x90)
                                {
                                    try
                                    {
                                        readedChar = br.read();
                                    }
                                    catch (IOException e)
                                    {
                                        Debug.WriteLine("Error : Unexpected eof");
                                        System.exit(1);
                                    }
                                    value = decodeBinHexChar((char) readedChar);
                                    if (value == -1)
                                    {
                                        // next caracter was not complet
                                        try
                                        {
                                            readedChar = br.read();
                                        }
                                        catch (IOException e)
                                        {
                                            Debug.WriteLine("Error : Unexpected eof");
                                            System.exit(1);
                                        }
                                        // Now value was <> -1 'cause a worth 2 character
                                        // were necessary to build an BinHexChar
                                        value = decodeBinHexChar((char) readedChar);
                                    }
                                    if (value == 0) // it's not an compressed sequence
                                        return 0x90;
                                    else
                                    {
                                        // Repeat n-times the previous character
                                        numOfRepeat = readedChar - 2;
                                        return repeatedChar;
                                    }
                                }
                                else
                                {
                                    // It's a normal value
                                    repeatedChar = value;
                                    return value;
                                }
                            }
                        }
                    }
                }
            }
        }
        private int decodeBinHexChar(char encodedBinHexChar)
        {
            for (int k = 0; k < 64; k++)
            {
                // Search in the value of the caracter in BinHex Table
                if (binHexTable[k] == encodedBinHexChar)
                {
                    for (int l = 5; l >= 0; l--)
                    {
                        int mask = 0x01 << l;
                        currentBitsField[numBits++] = (k & mask) >> l;
                        if (numBits == 8)
                        {
                            for (int i = 0; i < 8; i++)
                                previousBitsField[i] = currentBitsField[i];
                            numBits = 0;
                            previousCompleted = true;
                        }
                    }
                }
            }

            if (previousCompleted == true)
            {
                previousCompleted = false;
                return (previousBitsField[0] << 7 | previousBitsField[1] << 6 | previousBitsField[2] << 5 | previousBitsField[3] << 4 | previousBitsField[4] << 3 | previousBitsField[5] << 2 | previousBitsField[6] << 1 | previousBitsField[7]);
            }
            else
            {
                return -1;
            }
        }

        /**
         * Use to test the JavaBinHex class.
         * @param args Command line.
         * Example : java JavaBinHex myFile.hqx
         *  This execution uncompress the .HQX file
         *
         */
        public static void main(String[] args)
        {
            if (args.length == 2)
            {
                new JavaBinHex(args[0]);
            }
            else
            {
                Debug.WriteLine("Error : Need a filename in parameter");
                Debug.WriteLine("example : java JavaBinHex filename.hqx");
                System.exit(-1);
            }
        }
    }
#endif
    }
}