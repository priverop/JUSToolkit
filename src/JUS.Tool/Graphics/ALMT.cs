using Texim.Compressions.Nitro;

namespace JUSToolkit.Graphics
{
    /// <summary>
    /// NDS Background Modes.
    /// </summary>
    public enum NitroBackgroundMode
    {
        /// <summary>
        /// Text bg.
        /// </summary>
        Text = 0,

        /// <summary>
        /// Palette must be 8bpp.
        /// </summary>
        Affine = 1,

        /// <summary>
        /// Extended mode -> Text | Affine, not bitmap.
        /// </summary>
        Extended = 2,
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
