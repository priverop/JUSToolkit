# DTX Sprites

DTX files contains sprites used in animations and UI layers. 

## Sprite Types 

DTX can be Type/Version 01 or 02. Subversion can be 03, 04 or 83. These are the combinations:

- Type 01 03: Sprites (tiled) or Textures (linear). Most of the images are in this format. Images for menus, overlays...
- Type 02 03: Textures (linear) for battle attacks.
- Type 01 04: Komas. The characters of your deck.
- Type 01 05: Unknown format.
- Type 01 06: Unknown format.
- Type 01 83: Battle attacks. The DSIG is separated in a single file.
- Type 02 83: Battle attacks. The DSIG is separated in a single file.

## Tools and Specifications

Use the [ImHex pattern file](./dtx.hexpat) to automatically parse and visualize DTX files in the [ImHex](https://imhex.werwolv.net/) hex editor. I fully recommend using this to explore the format.

In Ubuntu, copy it to `/usr/share/imhex/pattern` for automatic file recognition.

### Tinke Workflow:

You can use my [Tinke branch](https://github.com/priverop/tinke/tree/feat/jus_dtx) to automatically watch some of the DTX files. If you don't have it, follow these instructions:

1. Open the DTX file and decompress/unpack if necessary (pressing "D").
2. View as Palette: DSIG offset + 0xC (16). This can vary, look for the "E0" byte.
3. View as Tile: Pixels Offset => After the palette and the padding zeros.
4. Settings: Image Pattern is Horizontal (Tiled) or Lineal depending on the DTX. BPP is always 4bpp.

Example: `Commu/commu_pack.aar -> leader00.dtx`

- Palette offset: 0x264
- Tile offset: 0x304 

### DTX 01 03 (1.3)

| Offset | Type                        | Description        |
| ------ | --------------------------- | ------------------ |
| 0x00   | char[4]                     | DSTX               |
| 0x04   | byte                        | Unknown            |
| 0x05   | byte                        | Type, must be 0x03 |
| 0x06   | short                       | Number of elements |
| 0x08   | short                       | DSIG offset        |
| 0x0A   | SpriteOffset[element_count] | Sprite offsets     |
| 0x40   | Sprite[element_count]       | Sprite data        |
| ...    | DSIG                        | Image with palette |

#### Sprite Offset Structure

| Type  | Description                         |
| ----- | ----------------------------------- |
| short | Offsets of the SegmentsInfo section |

Offsets are relative to position 0xA (10). Absolute address = 0xA + offset.

#### Sprite Structure

| Type  | Description            |
| ----- | ---------------------- |
| short | Segment Count          |
| ...   | Segment[segment_count] |

#### Segment Structure

| Type        | Description   |
| ----------- | ------------- |
| short       | Tile index    |
| signed byte | X Position    |
| signed byte | Y Position    |
| byte        | Shape*        |
| byte        | Palette index |

*More info at the bottom of the document.

#### Image Types (DTX1.3)

DTX1.3 supports two image modes based on the swizzling:
- **Tiled ("sp")**: Regular sprites compatible with Texim's sprite system. The base DSIG is tiled.
- **Linear ("tx")**: Textures stored as sprites. The base DSIG is linear.

#### Linear Sprite Workflow

1. Cut segments from the DSIG image using tile index and size (shape*).
2. Paste segments into a new empty image to compose the sprite using the coordinates (starting from the center of the image).

To import them back, we use a .yaml file with all the sprite & segment info in case we want to modify them.

*More info at the bottom of the document.

#### TileIndex

The Tile Index indicates the tile of the image (DSIG) where the segment starts. The image is divided into a grid of 8x8 pixel tiles, numbered sequentially from left to right, top to bottom.

Our DSIG images are usually 256 pixels wide, so each row contains 32 tiles (256 / 8 = 32). 

##### Example: Tile Index 65 

- Row: 65 ÷ 32 = 2 (third row, zero-based).
- Column: 65 % 32 = 1 (second column, zero-based).

```
Row 0: [0][1][2][3]...[31]
Row 1: [32][33][34][35]...[63]
Row 2: [64][65]← HERE [66][67]...[95]
```

The tile coordinates are (8, 16) to (15, 23).

### DTX 02 03 (2.3)

This format is pretty similar to 1.3tx. Let's see the differences:

- After each segment we have coordinates to check the top left pixel and the bottom right pixel. I guess it's a check to see if everything is well built.
- Shape has a bit to check if it's 2bpp or not. 
- The DSIG has a byte (0x50) after the MAGIC and the Version (0x01). We don't know what is it. ImageFormat? Review.
- Palettes are 8bytes (2bpp has 4 colors of 2 bytes), althought the space is still 32 bytes.
- Compression. After the palettes we have a 0x10 byte, that's the LZSS compression header.

Check the [Hexpat](./hexpat/dtx.hexpat) for the specification. 

#### Shape

Shape = 0x42 means 0100 0010. That 01 means that is 2bpp (if not, it will be 00), the next two bits are the flip transformations (false), and the four left 0010 = 2, so 4x4.

Shape = 0x65 means 0110 0101 -> 01 - 2bpp, 10 (2) - vertical flip, 0101 -> 05, then 32x8.

Shape = 0x75 means 0111 0101 -> 2bpp, horizontal and vertical flips, 32x8.

### DTX 01 04 (1.4) - Komas

See [Koma Specification](Koma-Specification.md) for more details.

| Offset | Type    | Description                                             |
| ------ | ------- | ------------------------------------------------------- |
| 0x00   | char[4] | DSTX                                                    |
| 0x04   | byte    | Unknown                                                 |
| 0x05   | byte    | Type, must be 0x04                                      |
| 0x06   | short   | Number of elements                                      |
| 0x08   | short   | DSIG offset                                             |
| 0x0A   | short   | Unknown                                                 |
| 0x0C   | uint[]  | Sprite data                                             |
| ...    | DSIG    | Image with palette (weight 8, swizzled 48x48 tile size) |

The sprite data is 4 bytes:

1. byte: Width in tiles (48 pixels)
2. byte: Height in tiles (48 pixels)
3. short: Tile index (starting offset of the image). Only if it's 0, use 1. Tile 0 is transparent tile.

However, this format uses the Sprite info from the KSHape file, we don't know why they store the sprite info in the .dtx file.

## Shape Property

The **Shape** byte encodes both segment size and flip transformations:
- **Lower 4 bits (shape & 0x0F):** Segment size (width and height).
- **Upper 4 bits (shape >> 4):** Flip transformations.

### Sizes

| Size Value | Size  | Description |
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

### Flips

| Flip Value | Horizontal Flip | Vertical Flip |
| ---------- | --------------- | ------------- |
| 0x00       | No              | No            |
| 0x01       | Yes             | No            |
| 0x02       | No              | Yes           |
| 0x03       | Yes             | Yes           |
