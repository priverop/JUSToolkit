# DSIG images

The _DSIG_ binary format (_nintendo DS Indexed Graphic_) stores a color palette
and encoded pixel information that points to the palette colores (indexed 
pixels).

- [ImHex pattern](./resources/dig.hexpat)

## Format

There are two known versions of the formats, each with different variants. 
This games only handles version 2 when the flags value is `0x40`.

| Offset | Type       | Description           |
| ------ |------------|-----------------------|
| 0x00   | char[4]    | Format ID: `DSIG`     |
| 0x04   | byte       | Version: 1 or 2       |
| 0x05   | byte       | Flags                 |
| 0x06   | byte       | Number of palettes    |
| 0x07   | byte       | Unknown (v2 is not 0) |
| 0x08   | ushort     | Image width           |
| 0x0A   | ushort     | Image height          |
| 0x0C   | bgr555[][] | Palettes              |
| ...    | uint       | (only v2) Unknown     |
| ...    | byte[]     | Indexed pixels        |

### Flags

- Bits 0-3: bits per pixels (bpp)
  - 0 -> 4bpp
  - 1 -> 8bpp
- Bit 4-5: swizzling
  - 0 -> TBD
  - 1 -> tiled
  - 2 -> texture atlas (lineal)
  - 3 -> TBD
- Bit 6: compressed
- Bit 7: reserved

The game supports the following combinations:

- 0x10: tiled, 4bpp
- 0x11: tiled, 8bpp
- 0x20: lineal, 4bpp
- 0x30: TBD
- 0x40 (v1 and v2): compressed, TBD
- 0x50: compressed, tiled

### Palettes

Every palette has 16 colors. Each color is a 16-bits value with BGR555 
encoding. In 8 bpp, the palettes are combined into one.

The game finds the first and last non-null color (`0x0000` for black) across 
the full block of palettes.
