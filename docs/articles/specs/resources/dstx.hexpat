#pragma author priverop
#pragma description Nintendo DS - Jump Ultimate Stars! Sprite Format

#pragma magic [44 53 54 58] @ 0x00

import type.base;

bitfield DstxSegmentShape {
    size: 4; // see spec for conversion
    horizontal_flip: 1;
    vertical_flip: 1;
};

struct DstxSpriteSegment {
    u16 tileIndex;
    s8 xPos;
    s8 yPos;
    DstxSegmentShape shape;
    u8 paletteIndex;
};

struct DstxSpriteData3 {
    u16 offset;
    u16 segmentsCount @ offset + 0xA;
    DstxSpriteSegment segments[segmentsCount] @ offset + 0xC;
};

struct DstxSpriteData4 {
    u8 width; // in tiles of 48 pixels
    u8 height; // in tiles of 48 pixels
    u16 firstTileIndex; // if 0, use 1, as tile 0 is the transparent tile.
};

bitfield DstxFeatures {
    type: 7;
    bool separateDsig: 1;
};

struct Dstx {
    u32 magic;
    u8 version;
    DstxFeatures features;
    u16 number_elements;
    u16 dsig_offset;

    if (features.type == 3) {
        DstxSpriteData3 spriteData[number_elements];
    } else if (features.type == 4) {
        u16 unknown;
        DstxSpriteData4 spriteData[number_elements];
    }

    if (features.separateDsig) {
        u32 dsigFileId @ dsig_offset;
    } else {
        u8 dsig[] @ dsig_offset;
    }
};

Dstx dtx @ 0x00;
