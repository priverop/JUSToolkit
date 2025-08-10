# Sprites

DTX files are sprites used in animations or for layers. 

We have some types:

- Type 01.
- Type 02.
- Type 03: Some of these sprites are 3D Textures. Most of the images are in this format. Images for menus, overlays...
- Type 04: Komas. The characters of your deck.
- Type 05. 
- Type 06. 
- Type 83: The dsig is separated in a single file.

## Specifications

[Here](./dtx.hexpat) we have the [ImHex](https://github.com/WerWolv/ImHex) Pattern file to see the specification  automatically in the Hex Editor. I fully recommend using this to explore the format.

If you want the editor to get the pattern for you every time you open a .dtx file you can copy it into the pattern folder: `/usr/share/imhex/pattern` in Ubuntu.

It recognizes the DTX type and applies the specific format.

With Tinke, you can view the compressed image opening the .dtx as Palette (adding the Palette Offset) and then as Tile (adding the Pixels Offset). These offsets can be found in the Hex view: after the DSIG offset you add 16 bytes (0xC) and you'll get the Palettes (usually starts with E0). After the palettes, a lot of 00s and then the pixels. Example: Commu/commu_pack.aar -> leader00.dtx -> uncompress/unpack (press "D"). Open as Palette: 264. Open as Tile: Offset 304 (both Hexadecimal), Horizontal and 4bpp usually, play with those. 

### DTX 03

| Offset | Type    | Description        |
| ------ | ------- | ------------------ |
| 0x00   | char[4] | DSTX               |
| 0x04   | byte    | Unknown            |
| 0x05   | byte    | Type, must be 0x04 |
| 0x06   | short   | Number of elements |
| 0x08   | short   | DSIG offset        |
| 0x0A   | uint[]  | Sprite data        |
| ...    | DSIG    | Image with palette |

### DTX 04

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