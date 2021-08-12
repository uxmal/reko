using System;

namespace Reko.Tools.specGen
{
	class Program
	{
		static void Main(string[] args) {
            var mode = args[0];
            var configuration = args[1];
            var masterSpecFile = args[2];
            var template = args[3];
            var outputFile = args[4];
            var solutionDir = args[5];


            switch (mode)
            {
            case "nuget":
                ScriptGenerator.UpdateNugetFile(configuration, masterSpecFile, template, outputFile, solutionDir);
                break;
            case "wix":
                ScriptGenerator.UpdateWixFile(configuration, masterSpecFile, template, outputFile, solutionDir);
                break;
            default:
                Console.Error.WriteLine($"Unsupported mode '{mode}'");
                Environment.Exit(1);
                break;
            }
            
		}
	}
}
