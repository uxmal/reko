using System;

namespace Reko.Tools.nugetSpecGen
{
	class Program
	{
		static void Main(string[] args) {
            var projectDir = args[0];
            var outDir = args[1];
            var targetFramework = args[2];
            var nuspecTemplatePath = args[3];

            new NugetSpecGen(projectDir, outDir, targetFramework, nuspecTemplatePath).Generate();
		}
	}
}
