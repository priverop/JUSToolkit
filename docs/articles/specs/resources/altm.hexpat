#pragma author pleonex
#pragma description Nintendo DS - Jump Ultimate Stars! animation format

#pragma magic [41 4C 4D 54] @ 0x00

bitfield AltmMapInfo {
    tile_index: 10;
    horizontal_flip: 1;
    vertical_flip: 1;
    palette_index: 4;
};

struct Altm {
    char format_id[4];
    u8 version;
    u8 flags;
    u8 unk6; // 1 or 2
    u8 unk7;
    u32 unk8;
    u16 tile_width;
    u16 tile_height;
    u16 width; // in tiles
    u16 height; // in tiles
    u32 unknown2;
    AltmMapInfo map[width * height];
};

Altm format @ 0x00;
