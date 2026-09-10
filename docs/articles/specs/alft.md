# ALFT font

The _ALFT_ binary format (_AL FonT_) stores a bitmap font. The game contains
four different fonts, three are based on DSIG images, and one in BMP. The files
use the extension `.aft`.

## Format

There is only one version (0) for this format.

| Offset | Type      | Description                               |
| ------ | --------- | ----------------------------------------- |
| 0x00   | char[4]   | Format ID: `ALFT`                         |
| 0x04   | byte      | Version (0)                               |
| 0x05   | byte      | Flags                                     |
| 0x06   | TableInfo | Map information between glyphs and images |
| ..     | ...       | Font image (BMP or DSIG)                  |

The flags is a 8-bit field:

- Bits 0-1: image format:
  - 0: none.
  - 1: DSIG.
  - 2: BMP (standard bitmap).
- Bit 2: encoding:
  - 0: Shift-JIS.
  - 1: ASCII.
- Bits 3-7: reserved.

### Table information

| Offset | Type        | Description                                          |
| ------ | ----------- | ---------------------------------------------------- |
| 0x00   | char[2]     | Section ID: `TB`                                     |
| 0x02   | byte        | Glyph box width, in BMP it includes the border size  |
| 0x03   | byte        | Glyph box height, in BMP it includes the border size |
| 0x04   | ushort      | Glyphs per row in the image                          |
| 0x06   | ushort      | Mapping group count                                  |
| 0x08   | GroupInfo[] | Mapping groups                                       |

### Mapping group

| Offset | Type   | Description                             |
| ------ | ------ | --------------------------------------- |
| 0x00   | ushort | Glyph encoded codepoint start           |
| 0x02   | ushort | Glyph encoded codepoint end (inclusive) |
| 0x04   | ushort | Index to the first glyph image          |

### BMP images

The font can contain a
[standard BMP (.bmp) image](https://en.wikipedia.org/wiki/BMP_file_format). For
reference, below is the specific format the game uses for the fonts.

| Offset | Type             | Description                                  |
| ------ | ---------------- | -------------------------------------------- |
| 0x00   | char[2]          | Format ID: `BM`                              |
| 0x02   | uint             | File size                                    |
| 0x06   | ushort           | Reserved                                     |
| 0x08   | ushort           | Reserved                                     |
| 0x0A   | uint             | Bitmap offset                                |
| 0x0E   | BITMAPINFOHEADER | DIB header                                   |
| 0x36   | RGB32[]          | Palette, 4 bytes per color, no alpha channel |
| ...    | byte[]           | Encoded pixels inverted                      |

The supported pixel encodings are 1 bpp, 4 bpp and 8 bpp.

#### DIB header

It uses the type `BITMAPINFOHEADER` with 40 bytes.

| Offset | Type   | Description                                                |
| ------ | ------ | ---------------------------------------------------------- |
| 0x0E   | uint   | Section length (40)                                        |
| 0x12   | int    | Width                                                      |
| 0x16   | int    | Height                                                     |
| 0x1A   | ushort | Color planes count (1)                                     |
| 0x1C   | ushort | Bit depth (bpp): 1, 4, or 8                                |
| 0x1E   | uint   | Compression method: 0 for BI_RGB (none)                    |
| 0x22   | uint   | Image size + 2 (bug?)                                      |
| 0x26   | int    | Horizontal resolution                                      |
| 0x2A   | int    | Vertical resolution                                        |
| 0x2E   | uint   | Palette color count, hard-coded: 2 1bpp, 16 4bpp, 256 8bpp |
| 0x32   | uint   | Important color count, ignored (0)                         |
