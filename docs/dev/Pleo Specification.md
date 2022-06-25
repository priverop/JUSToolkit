# Jump Ultimate Start images

## DSTX

| Offset | Type    | Description        |
| ------ | ------- | ------------------ |
| 0x00   | char[4] | DSTX               |
| 0x04   | byte    | Unknown            |
| 0x05   | byte    | Type, must be 0x04 |
| 0x06   | short   | Number of elements |
| 0x08   | short   | DSIG offset        |
| 0x0C   | uint[]  | Sprite data        |
| ...    | DSIG    | Image with palette |

The sprite data is 4 bytes:

1. byte: Width in tiles (48 pixels)
2. byte: Height in tiles (48 pixels)
3. short: Tile index. Only if it's 0, use 1. Tile 0 is transparent tile.

> **NOTE**:  
> The sprite IDs from `KShape` do not correspond to the order defined in the
> DSTX. `KShape` points to blocks of 48x48 in the order defined in the image
> DSIG, which may be different as the sprites defined here.

## DSIG

| Offset | Type     | Description                                        |
| ------ | -------- | -------------------------------------------------- |
| 0x00   | char[4]  | DSIG                                               |
| 0x04   | byte     | Unknown                                            |
| 0x05   | byte     | nds image format. Different than 0x10, then 8 bpp. |
| 0x06   | short    | number of palettes                                 |
| 0x08   | short    | width                                              |
| 0x0A   | short    | height                                             |
| 0x0C   | bgr555[] | palette                                            |
| ...    | pixels[] | image                                              |

## ALAR

| Offset | Type       | Description                 |
| ------ | ---------- | --------------------------- |
| 0x00   | char[4]    | ALAR                        |
| 0x04   | byte       | Version (3 to follow)       |
| 0x05   | byte       | Minor version?              |
| 0x06   | int        | Number of files             |
| 0x0A   | short      | Reserved?                   |
| 0x0C   | int        | Number of entries           |
| 0x10   | short      | Data offset                 |
| 0x12   | short[]    | File info absolute pointers |
| ..     | FileInfo[] | File info list              |
| ..     | Stream[]   | File data                   |

### File info

| Offset | Type   | Description               |
| ------ | ------ | ------------------------- |
| 0x00   | short  | ID                        |
| 0x02   | short  | Unknown                   |
| 0x04   | int    | Absolute pointer          |
| 0x08   | int    | Size                      |
| 0x0C   | short  | Unknown                   |
| 0x0E   | short  | Unknown                   |
| 0x10   | short  | Unknown                   |
| 0x12   | string | Null-terminated file path |

## KShape

| Offset | Type         | Description                  |
| ------ | ------------ | ---------------------------- |
| 0x00   | int[]        | First group element index    |
| 0x20   | int[]        | Number of elements per group |
| 0x40   | KShapeInfo[] | Info to reconstruct sprites  |

To get the `KShapeInfo` from an image, first get its
[_Koma element_](#koma-element), then:

1. Read the group index at `KShapeGroupIndex * 4`
2. Add to this the `KShapeElementIndex`
3. Multiply by the size of `KShapeInfo`: `0x18` and add `0x40`.

There are 8 groups.

### KShapeInfo

| Offset | Type       | Description                                   |
| ------ | ---------- | --------------------------------------------- |
| 0x00   | byte[0x14] | Segment index - 1 (except 0 for transparency) |
| 0x14   | int        | Unknown                                       |

Each segment is always 48x48 pixels. Start at coordinate (0, 0) and iterate
horizontally until reaching width 240, then increment Y. If the index is 0,
skip, it's a transparent segment (you could use the tile 0). The final image
should be 240x192 pixels.

## Koma

| Offset | Type          | Description |
| ------ | ------------- | ----------- |
| 0x00   | KomaElement[] | Entries     |

### Koma element

| Offset | Type  | Description          |
| ------ | ----- | -------------------- |
| 0x00   | short | Image ID             |
| 0x02   | short | Unknown              |
| 0x04   | byte  | Name table index     |
| 0x05   | byte  | Name number          |
| 0x06   | byte  | Unknown              |
| 0x07   | byte  | Unknown              |
| 0x08   | byte  | KShape group index   |
| 0x09   | byte  | KShape element index |
| 0x0A   | byte  | Unknown              |
| 0x0B   | byte  | Unknown              |

The name is the combination of the prefix from the table and the number:
`$"{table[index]}_{num:D2}"`

### Koma name table

| ID  | Name   |
| --- | ------ |
| 0   | _none_ |
| 1   | es     |
| 2   | jj     |
| 3   | op     |
| 4   | rb     |
| 5   | rk     |
| 6   | bl     |
| 7   | yo     |
| 8   | mr     |
| 9   | mo     |
| 10  | nn     |
| 11  | bb     |
| 12  | hk     |
| 13  | hs     |
| 14  | yh     |
| 15  | bc     |
| 16  | bu     |
| 17  | pj     |
| 18  | hh     |
| 19  | nk     |
| 20  | na     |
| 21  | db     |
| 22  | tl     |
| 23  | ds     |
| 24  | dn     |
| 25  | dg     |
| 26  | to     |
| 27  | tz     |
| 28  | ss     |
| 29  | sd     |
| 30  | dt     |
| 31  | tc     |
| 32  | sk     |
| 33  | nb     |
| 34  | oj     |
| 35  | cb     |
| 36  | kk     |
| 37  | kn     |
| 38  | gt     |
| 39  | ct     |
| 40  | tr     |
| 41  | ig     |
| 42  | is     |
