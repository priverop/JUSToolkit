#pragma author priverop
#pragma description Nintendo DS - Jump Ultimate Stars! Sprite Format

#pragma magic [44 53 54 58] @ 0x00

import type.base;

struct Segment {
    u16 tileIndex;
    s8 xPos;
    s8 yPos;
    u8 shape;
    u8 paletteIndex;
};

struct SpriteData3 {
    u16 offset;
    u16 segmentsCount @ offset + 0xA; 
    Segment segments[segmentsCount] @ offset + 0xC;
};

struct SpriteData4 {
    u8 width;
    u8 height;
    u16 tileIndex;
};

struct DTX {
    u32 magic;
    u8 version; 
    u8 dtx_type;
    u16 number_elements;
    type::Hex<u16> dsig_offset;
    if (dtx_type == 03) {
        SpriteData3 spriteData[number_elements];
    }
    if (dtx_type == 04) {
        u16 unknown;
        SpriteData4 spriteData[number_elements];
    }
};

DTX dtx_file @ 0x00;