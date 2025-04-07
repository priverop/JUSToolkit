#pragma author priverop
#pragma description Nintendo DS - Jump Ultimate Stars! Koma.bin Format

import type.base;

struct Koma {
    u16 ImageId;
    u16 Unknown2;
    u8 nameIdx;
    u8 nameNum;
    u8 Unknown6;
    u8 Unknown7;
    u8 KShapeGroupId;
    u8 KShapeElementId;
    u8 UnknownA;
    u8 UnknownB;
};

struct Komabin {
    Koma komas[890];
};

Komabin komabin_file @ 0x00;