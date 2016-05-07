namespace Reko.ImageLoaders.VmsExe
{
    public class ImageSectionDescriptor
    {
        public ushort Size { get; internal set; }
        public ushort NumPages { get; internal set; }
        public uint StartVPage { get; internal set; }
        public uint Flags { get; internal set; }
        public uint RvaFile { get; internal set; }
        public uint GlobalSectionIdent { get; internal set; }
        public string SectionName { get; internal set; }
    }
}