using Texim.TileMaps;

namespace JUS.Tool.Graphics
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
    /// Screen map with format ALTM.
    /// </summary>
    public class Altm : ITileMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Altm"/> class.
        /// </summary>
        public Altm()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Altm"/> class cloning another Altm object.
        /// </summary>
        /// <param name="atm">Altm object to clone.</param>
        public Altm(Altm atm)
        {
            Magic = atm.Magic;
            Unknown = atm.Unknown;
            Unknown2 = atm.Unknown2;
            TileSizeW = atm.TileSizeW;
            TileSizeH = atm.TileSizeH;
            NumTileW = atm.NumTileW;
            NumTileH = atm.NumTileH;
            Unknown3 = atm.Unknown3;
            Maps = atm.Maps;
            Width = atm.Width;
            Height = atm.Height;
            TileSize = atm.TileSize;
            BgMode = atm.BgMode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Altm"/> class cloning another Altm and the ScreenMap properties.
        /// </summary>
        /// <param name="atm">Altm object to clone.</param>
        /// <param name="screenMap">ScreenMap object to clone.</param>
        public Altm(Altm atm, ITileMap screenMap)
            : this(atm)
        {
            Width = screenMap.Width;
            Height = screenMap.Height;
            Maps = screenMap.Maps;
        }

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
        public MapInfo[] Maps { get; set; } = [];

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
