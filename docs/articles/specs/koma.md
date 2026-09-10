# Koma sprites

To export all the info from the images, we need to follow some steps detailed
[here](./koma-research.md)

- [koma.bin ImHex pattern](./resources/komabin.hexpat)
- [kshape ImHex pattern](./resources/kshape.hexpat)

## KShape

| Offset | Type         | Description                 |
| ------ | ------------ | --------------------------- |
| 0x00   | int[]        | First group element index   |
| 0x20   | int[]        | Number of shapes per group  |
| 0x40   | KShapeInfo[] | Info to reconstruct sprites |

To get the `KShapeInfo` from an image, first get its
[_Koma element_](#koma-element), then:

1. Read the group index at `KShapeGroupIndex * 4`
2. Add to this the `KShapeElementIndex`
3. Multiply by the size of `KShapeInfo`: `0x18` and add `0x40`.

There are 8 groups. Each group is the number of tiles of the shape of the sprite.
The number of shapes means the posible combinations of the sprite. If it's size 2 (group 1), we will have an horizontal shape and a vertical shape, so group 1 will have 2 shapes.

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
