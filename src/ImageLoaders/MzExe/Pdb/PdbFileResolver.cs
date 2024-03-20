using Reko.Core.Memory;
using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.ImageLoaders.MzExe.Pdb
{
    /// <summary>
    /// Locates PDF files.
    /// </summary>
    public class PdbFileResolver
    {
        private readonly IFileSystemService fsSvc;
        private readonly IEventListener listener;

        public PdbFileResolver(IFileSystemService fsSvc, IEventListener listener)
        {
            this.fsSvc = fsSvc;
            this.listener = listener;
        }

        public PdbInformation? Load(PdbFileReference pdbref)
        {
            var pdbPath = Path.ChangeExtension(pdbref.Filename, ".pdb");
            if (fsSvc.FileExists(pdbPath))
            {
                var pdbImage = fsSvc.ReadAllBytes(pdbPath);
                var msfReader = MsfReader.Create(pdbImage, listener);
                if (msfReader is null)
                {
                    listener.Error("Unable to read PDB file {0}.", pdbPath);
                    return null;
                }
                var parser = new PdbParser(msfReader);
                var pdbInfo = parser.GetPdbInfo();
                if (pdbInfo is null)
                {
                    listener.Error("Unable to read PDB information stream.");
                    return null;
                }
                if (pdbref.Guid != pdbInfo.Guid)
                {
                    listener.Warn("The PDB file GUID doesn't match. Proceeding may cause " +
                        "incorrect results.");
                }
                //$TODO: read the CodeView type records + public symbols.
            }
            return null;
        }
    }
}
