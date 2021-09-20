using System;

namespace Reko.Tools.specGen
{
	class Program
	{
		static void Main(string[] args) {
            var mode = args[0];
            var configuration = args[1];
            var platform = args[2];
            var masterSpecFile = args[3];
            var template = args[4];
            var outputFile = args[5];
            var solutionDir = args[6];

            // Uncomment for debugging the build
            // Console.Write("specgen ***********************************************************************");
            // Console.WriteLine("  mode: {0}", mode);
            // Console.WriteLine("  configuration: {1}", configuration);
            // Console.WriteLine("  platform: {2}", platform);
            // Console.WriteLine("  masterSpecFile: {3}", masterSpecFile);
            // Console.WriteLine("  template: {4}", template);
            // Console.WriteLine("  outputFile: {5}", outputFile);
            // Console.WriteLine("  solutionDir: {6}", solutionDir);

            switch (mode)
            {
            case "nuget":
                ScriptGenerator.UpdateNugetFile(configuration, platform, masterSpecFile, template, outputFile, solutionDir);
                break;
            case "wix":
                ScriptGenerator.UpdateWixFile(configuration, platform, masterSpecFile, template, outputFile, solutionDir);
                break;
            default:
                Console.Error.WriteLine($"Unsupported mode '{mode}'");
                Environment.Exit(1);
                break;
            }
            
		}
	}
}
