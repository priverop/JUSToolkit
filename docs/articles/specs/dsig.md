# DSIG images

The _DSIG_ binary format (_nintendo DS Indexed Graphic_) stores a color palette
and encoded pixel information that points to the palette colors (indexed
pixels).

- [ImHex pattern](./resources/dsig.hexpat).

## Format

There are two known versions of the formats, each with different variants. This
games only handles version 2 when the flags value is `0x40`.

| Offset | Type       | Description                                       |
| ------ | ---------- | ------------------------------------------------- |
| 0x00   | char[4]    | Format ID: `DSIG`                                 |
| 0x04   | byte       | Version: 1 or 2                                   |
| 0x05   | byte       | Flags                                             |
| 0x06   | byte       | Number of palettes                                |
| 0x07   | byte       | Metadata length? (v2 is 4, otherwise 0)           |
| 0x08   | ushort     | Image width or block info length / 4 in format 4  |
| 0x0A   | ushort     | Image height or block data length / 4 in format 4 |
| 0x0C   | bgr555[][] | Palettes                                          |
| ...    | uint       | (only v2 with format 4) Metadata?                 |
| ...    | byte[]     | Indexed pixels                                    |

### Flags

- Bits 0-3: bits per pixels (bpp).
  - 0 -> 4bpp.
  - 1 -> 8bpp.
- Bit 4-7: image format.
  - 0 -> not supported.
  - 1 -> tiled.
  - 2 -> texture atlas (lineal).
  - 3 -> unknown.
  - 4 -> compressed sprite.
  - 5 -> unknown.

The game provides an implementation for the following combination of flags, and
gives it a name:

- 0x10 `NCG1`: tiled, 4bpp.
- 0x11 `NG18`: tiled, 8bpp.
- 0x20 `PL4R`: lineal, 4bpp.
- 0x30 `NFC4`: unknown (not implemented?).
- 0x40 `FFL4` (v1 and v2): compressed sprite.
- 0x50 `LCP4`: unknown (lineal 4 bpp?).

The game also supports standard BMP formats with the following names:

- `PAL1`: BMP 1bpp.
- `PAL4`: BMP 4bpp.
- `PAL8`: BMP 8bpp.

And there is one additional unknown implementation with name `NCG2`.

### Palettes

Every palette has 16 colors. Each color is a 16-bits value with BGR555 encoding.
In 8bpp, the palettes are combined into one.

The game finds the first and last non-null color (`0x0000` for black) across the
full block of palettes.

### Compressed sprite

The image format 4 _pixel data_ contains actually a header followed by blocks of
compressed pixel data. It uses Nitro BIOS compression (usually LZSS).

The data starts with a 32-bits integer value with the number of blocks. It
follows 4 bytes per block:

- 16-bits integer (ushort): block offset / 4, relative to this data area.
- 16-bits integer (ushort): block length.

Then, it's each block of data with a BIOS compression, including its header that
defines the compression type and decompressed length. Each block has a padding
to a position multiple of 8, with the padding byte `0x00`.

The game reads the first section with the segment count and segment information
by multiplying the _width_ from the header by 4. This _width_ seems to match
with the number of segments plus one (the 32-bits of the count).

In the version 2 of DSIG files, there is a 32-bits integer before the pixel data
which is unknown.
