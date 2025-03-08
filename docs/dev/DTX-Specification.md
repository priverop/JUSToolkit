# DTX

DTX files are sprites used in animations or for layers. 

We have some types:

- Type 03: Sprites and Textures. Images for menus, overlays...
- Type 04: Komas. The characters of your deck.
- Type 05: ??
- Type 06: ??

## DTX 03

| Offset | Type    | Description        |
| ------ | ------- | ------------------ |
| 0x00   | char[4] | DSTX               |
| 0x04   | byte    | Unknown            |
| 0x05   | byte    | Type, must be 0x04 |
| 0x06   | short   | Number of elements |
| 0x08   | short   | DSIG offset        |
| 0x0C   | uint[]  | Sprite data        |
| ...    | DSIG    | Image with palette |

## DTX 04

| Offset | Type    | Description        |
| ------ | ------- | ------------------ |
| 0x00   | char[4] | DSTX               |
| 0x04   | byte    | Unknown            |
| 0x05   | byte    | Type, must be 0x04 |
| 0x06   | short   | Number of elements |
| 0x08   | short   | DSIG offset        |
| 0x0A   | short   | Unknown            |
| 0x0C   | uint[]  | Sprite data        |
| ...    | DSIG    | Image with palette |

The sprite data is 4 bytes:

1. byte: Width in tiles (48 pixels)
2. byte: Height in tiles (48 pixels)
3. short: Tile index. Only if it's 0, use 1. Tile 0 is transparent tile.
