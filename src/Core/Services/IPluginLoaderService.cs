using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Reko.Core.Services
{
    /// <summary>
    /// Loads fully-qualified types of plugins into the current process.
    /// </summary>
    /// <remarks>
    /// Microsoft in their infinite wisdom completely broke the the way types 
    /// are loaded dynamically with Type.GetType(). Cursing them for a million years
    /// is not enough. But we choose the high path and write this workaround class that
    /// does what Type.GetType used to do.
    /// </remarks>
    public interface IPluginLoaderService
    {
        /// <summary>
        /// Load the .NET type using its fully qualified instance. This method assumes
        /// that the assembly is physically located in the same directory as the
        /// calling assembly.
        /// </summary>
        /// <param name="fullyQualifiedName"></param>
        Type GetType(string fullyQualifiedName);
    }

    public class PluginLoaderService : IPluginLoaderService
    {
        public Type GetType(string fullyQualifiedTypeName)
        {
            var components = fullyQualifiedTypeName.Split(',');
            if (components.Length < 2)
                throw new ApplicationException($"reko.config contains malformed type name {fullyQualifiedTypeName}.");
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            var asmName = Path.Combine(dir, (components[1].Trim() + ".dll"));
            var typeName = components[0].Trim();
            var asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(asmName);
            Type t = asm.GetType(typeName, true)!;
            return t;
        }
    }
}
