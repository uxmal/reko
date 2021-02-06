using System;

namespace Reko.Tools.specGen
{
	class Program
	{
		static void Main(string[] args) {
            var mode = args[0];
            var projectDir = args[1];
            var outDir = args[2];
            var targetFramework = args[3];
            var inputFilePath = args[4];

            switch (mode)
            {
            case "nuget":
                new NugetSpecGen(projectDir, outDir, targetFramework, inputFilePath).Generate();
                break;
            case "wix":
                new WixSpecGen(projectDir, outDir, targetFramework, inputFilePath).Generate();
                break;
            default:
                Console.Error.WriteLine($"Unsupported mode '{mode}'");
                Environment.Exit(1);
                break;
            }
            
		}
	}
}
