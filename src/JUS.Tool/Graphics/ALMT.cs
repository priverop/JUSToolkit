using Texim.Compressions.Nitro;

namespace JUSToolkit.Formats
{
    public enum NitroBackgroundMode
    {
        Text = 0,
        Affine = 1, // Palette must be 8bpp
        Extended = 2, // Extended mode -> Text | Affine, not bitmap
    }
    /// <summary>
    /// Screen map with format Almt.
    /// </summary>
    public class Almt : IScreenMap
    {
        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public uint Magic {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public uint Unknown {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public uint Unknown2 {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public ushort TileSizeW {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public ushort TileSizeH {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public ushort NumTileW {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public ushort NumTileH {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets TBD.
        /// </summary>
        public uint Unknown3 {
            get;
            set;
        }

        /// <inheritdoc/>
        public MapInfo[] Maps {
            get;
            set;
        }

        /// <inheritdoc/>
        public int Width { get; set; }

        /// <inheritdoc/>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the size of the tile.
        /// </summary>
        public System.Drawing.Size TileSize {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mode of the background (NitroBackgroundMode).
        /// </summary>
        public NitroBackgroundMode BgMode {
            get;
            set;
        }
    }
}
