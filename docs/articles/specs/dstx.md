# DSTX textures

The _DSTX_ binary format (_nintendo DS TeXture_) stores a texture image. It
supports different pixel encodings, and they are used in animations and UI
layers.

- [ImHex pattern](./resources/dstx.hexpat)

## Formats

The format has different variants and versions. They have a common header:

| Offset | Type    | Description              |
| ------ | ------- | ------------------------ |
| 0x00   | char[4] | Format ID: `DSTX`        |
| 0x04   | byte    | Version: 1 or 2          |
| 0x05   | byte    | Flags                    |
| 0x06   | ushort  | Number of elements       |
| 0x08   | ushort  | Image (DSIG) offset      |
| 0x0A   | byte[]  | Texture data (see below) |

The flag byte defines the texture actual format:

- Version 1:
  - 0, 1, 2: unsupported in this game.
  - 3: sprites or textures.
  - 83: image data is in a separate file inside the same ALAR container.
  - 4: tiles of 48x48 pixels (koma).
  - 5, 6: to be investigated.
- Version 2:
  - 3 or 83.

### Format 3: sprites

| Offset | Type                  | Description                      |
| ------ | --------------------- | -------------------------------- |
| 0x0A   | ushort[element_count] | Sprite offsets relative to `0xA` |
| ...    | Sprite[element_count] | Sprite data                      |
| ...    | DSIG                  | Image with palette or file ID    |

If the flag format bit 7 is set (format `0x83`), then at the DSIG image offset
instead of finding the DSIG data there is a 32-bits value with the file ID that
contains the image data (inside the same ALAR container).

#### Image types

DTX03 supports two image modes based on the swizzling:

- **Tiled ("sp")**: standard DS swizzling with 8x8 tiles.
- **Linear ("tx")**: linear image like a texture atlas.

> [!NOTE]  
> The lineal image is like a texture atlas or spritesheet. These images are
> usually manually crafted (probably via custom editors). It would be hard to
> automatically puzzle each segment back with new sizes, so JUSToolkit exports
> the position and size of each segment in a YAML file, and use it when
> importing back.

#### Sprite

| Type                   | Description                      |
| ---------------------- | -------------------------------- |
| ushort                 | Number of segments in the sprite |
| Segment[segment_count] | Segment data                     |

#### Segment

| Type        | Description   |
| ----------- | ------------- |
| ushort      | Tile index    |
| signed byte | X position    |
| signed byte | Y position    |
| byte        | Shape         |
| byte        | Palette index |

#### Tile index

The Tile Index indicates the tile of the image (DSIG) where the segment starts.
The image is divided into a grid of 8x8 pixel tiles, numbered sequentially from
left to right, top to bottom.

Our DSIG images are usually 256 pixels wide, so each row contains 32 tiles (256
/ 8 = 32).

> [!TIP]  
> **Example: Tile Index 65**
>
> - Row: 65 ÷ 32 = 2 (third row, zero-based).
> - Column: 65 % 32 = 1 (second column, zero-based).
>
> ```plain
> Row 0: [0][1][2][3]...[31]
> Row 1: [32][33][34][35]...[63]
> Row 2: [64][65]← HERE [66][67]...[95]
> ```
>
> The tile coordinates are (8, 16) to (15, 23).

#### Segment shape

The **Shape** byte encodes both segment size and flip transformations:

- **Lower 4 bits (shape & 0x0F):** Segment size (width and height).
- **Upper 4 bits (shape >> 4):** Flip transformations.

| Size value | Size  | Description |
| ---------- | ----- | ----------- |
| 0x00       | 8×8   | 1×1 tiles   |
| 0x01       | 16×16 | 2×2 tiles   |
| 0x02       | 32×32 | 4×4 tiles   |
| 0x03       | 64×64 | 8×8 tiles   |
| 0x04       | 16×8  | 2×1 tiles   |
| 0x05       | 32×8  | 4×1 tiles   |
| 0x06       | 32×16 | 4×2 tiles   |
| 0x07       | 64×32 | 8×4 tiles   |
| 0x08       | 8×16  | 1×2 tiles   |
| 0x09       | 8×32  | 1×4 tiles   |
| 0x0A       | 16×32 | 2×4 tiles   |
| 0x0B       | 32×64 | 4×8 tiles   |

| Flip value | Horizontal flip | Vertical flip |
| ---------- | --------------- | ------------- |
| 0x00       | No              | No            |
| 0x01       | Yes             | No            |
| 0x02       | No              | Yes           |
| 0x03       | Yes             | Yes           |

### Format 4: koma

See [the Koma specification](./koma.md) for more details.

| Offset | Type   | Description                                             |
| ------ | ------ | ------------------------------------------------------- |
| 0x0A   | short  | Unknown                                                 |
| 0x0C   | uint[] | Sprite data                                             |
| ...    | byte[] | Uknown area                                             |
| ...    | DSIG   | Image with palette (weight 8, swizzled 48x48 tile size) |

The sprite data is 4 bytes:

1. byte: Width in tiles (48 pixels)
2. byte: Height in tiles (48 pixels)
3. short: Tile index (starting offset of the image). Only if it's 0, use 1. Tile
   0 is transparent tile.

The unknown area is a data pointer the game reads by calculating the offset
after the sprite data info (12 + count \* 4). Then it calculates the size by
substracting the offset to the DSIG data.

> [!NOTE]  
> The sprite IDs from `KShape` do not correspond to the order defined in the
> DSTX. `KShape` points to blocks of 48x48 in the order defined in the image
> DSIG, which may be different as the sprites defined here.
