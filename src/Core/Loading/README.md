### Reko.Core.Loading

The `Reko.Core.Loading` namespace contains interfaces and classes related to binary image loading. Binary files are loaded using classes derived from the abstract base class `ImageLoader`. These image loaders are responsible for interpreting the contents of the file and returning a class that implements the `ILoadedImage` interface. Reko handles the following `ILoadedImage` implementations:

- `Reko.Core.Program`: the image file consists of an executable image or a dynamically loadable library.
- `Reko.Core.Loading.IArchive`: the image file has a container format, which may contain one or more internal files. Examples of such container formats are floppy disk or optical ISO images, or archive formats like `ar` or `tar`.
- `Reko.Core.Project`: the file is actually a Reko project file.
- `Reko.Core.Loading.Blob`: the image file is loaded, but more information is needed from the user to indicate what address it should be loaded at, what processor architecture the contents should be interpreted as, etc.

The class `ImageLocation` abstracts the notion of the location of a file within an archive. Reko can be provided the location of a binary file through a custom URL scheme 'archive:'. The following examples should clarify how this scheme is used.

To load a simple binary located in a file, either use an absolute URL:
```
/home/bob/libbob.so
```
or a relative URL:
```
libbob.so
```
To load a file located inside of an archive:
```
archive:///home/bob/archive.tar#dir1/dir2/libbob.so
```
