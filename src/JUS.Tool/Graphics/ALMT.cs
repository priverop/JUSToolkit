using Texim.Compressions.Nitro;

namespace JUSToolkit.Formats
{
    /// <summary>
    /// Screen map with format Almt.
    /// </summary>
    public class Almt : IScreenMap
    {
        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public uint Magic
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public uint Unknown
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public uint Unknown2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public ushort TileSizeW
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public ushort TileSizeH
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public ushort NumTileW
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public ushort NumTileH
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public uint Unknown3
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public MapInfo[] Maps
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public int Width { get; init; }

        /// <inheritdoc/>
        public int Height { get; init; }
    }
}
