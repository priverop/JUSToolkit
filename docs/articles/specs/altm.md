# ALTM tilemap

The _ALTM_ binary format (_AL Tile Map_) stores information to reconstruct a
tiled image with unique tiles.

- [ImHex pattern](./resources/altm.hexpat)

## Format

The game supports version 0 and 1. This specification is for version 0.

| Offset | Type      | Description                         |
| ------ | --------- | ----------------------------------- |
| 0x00   | char[4]   | Format ID: `ALTM`                   |
| 0x04   | byte      | Version: 0 or 1                     |
| 0x05   | byte      | Flags (3 always?)                   |
| 0x06   | byte      | Unknown (1 for S1 type or 2 for S2) |
| 0x07   | byte      | Unknown 2                           |
| 0x08   | byte[4]   | Unknown 3                           |
| 0x0C   | ushort    | Tile width                          |
| 0x0E   | ushort    | Tile height                         |
| 0x10   | ushort    | Image width in tiles                |
| 0x12   | ushort    | Image height in tiles               |
| 0x14   | byte[4]   | Reserved?                           |
| 0x18   | MapInfo[] | Tile map data                       |

The map information follows the Nitro standard of a 16-bits value:

- Bits 0-9: tile index.
- Bit 10: horizontal flip.
- Bit 11: vertical flip.
- Bits 12-15: palette index.

### About the unknowns

Notes from reverse-engineering formats...

- If _unknown 2_ (byte 7) is not 0, then the width is a 32-bits value at `0x18`,
  and the height is at `0x1C`. Data starts at `0x28`.
