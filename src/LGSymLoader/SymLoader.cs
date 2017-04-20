using Reko.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Core;
using Reko.Core.Configuration;

namespace Reko.Symbols.LGSymLoader
{
	public class LGSymLoader : ISymbolLoadingService
	{
		public ISymbolSource GetSymbolSource(string filename) {
			return new LGSymSource(filename);
		}

		public List<SymbolSourceDefinition> GetSymbolSources() {
			throw new NotImplementedException();
		}
	}
}
