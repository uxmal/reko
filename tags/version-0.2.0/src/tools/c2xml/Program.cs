using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Decompiler.Tools.C2Xml
{
    class Program
    {
        static int Main(string[] args)
        {
            TextReader input = Console.In;
            TextWriter output = Console.Out;
            if (args.Length > 2)
            {
                Usage();
                return 1;
            }
            if (args.Length >= 1)
            {
                try
                {
                    input = new StreamReader(args[0]);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("c2xml: Unable to open file {0} for reading. {1}", args[0], ex.Message);
                    return 1;
                }
            }
            if (args.Length == 2)
            {
                try
                {
                    output = new StreamWriter(args[1]);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("c2xml: Unable to open file {0} for writing. {1}", args[1], ex.Message);
                    return 1;
                }
            }
            XmlConverter c = new XmlConverter(input, output);
            c.Convert();
            output.Flush();
            return 0;
        }

        static void Usage()
        {
            Console.Error.WriteLine("usage: c2xml [filename]");
            Console.Error.WriteLine("   Reads the filename or standard input and converts the stream of tokens to XML");
        }
    }
}
