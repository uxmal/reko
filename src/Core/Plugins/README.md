## Reko plugins

To extend Reko with your own plugins, use this directory. First create a .NET 
assembly with one or more classes implementing the `Reko.Core.Plugins.IPlugin`
interface. Copy the resulting `.dll` file into this directory and restart 
Reko. The classes offered by your extension should now become available to
the Reko runtime environment. See the Reko documentation for how to create
a plugin.
