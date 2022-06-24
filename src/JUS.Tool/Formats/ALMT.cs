namespace JUSToolkit.Formats
{
    using Texim;
    public class ALMT : Map
    {

        public uint Magic
        {
            get;
            set;
        }

        public uint Unknown
        {
            get;
            set;
        }

        public uint Unknown2
        {
            get;
            set;
        }

        public ushort TileSizeW
        {
            get;
            set;
        }

        public ushort TileSizeH
        {
            get;
            set;
        }

        public ushort NumTileW
        {
            get;
            set;
        }

        public ushort NumTileH
        {
            get;
            set;
        }

        public uint Unknown3
        {
            get;
            set;
        }

        public MapInfo[] Info
        {
            get;
            set;
        }

    }
}
