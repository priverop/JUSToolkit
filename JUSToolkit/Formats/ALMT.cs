namespace JUSToolkit.Formats
{
    using Texim;
    public class ALMT : Map
    {

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

        public uint Unknown
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
