using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Core
{
    /// <summary>
    /// Abstraction of an executable image header.
    /// </summary>
    public abstract class ImageHeader
    {
        /// <summary>
        /// The address at which the image would prefer to be loaded.
        /// </summary>
        public virtual Address PreferredBaseAddress { get; set; }
    }
}
