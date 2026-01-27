#pragma author priverop
#pragma description Nintendo DS - Jump Ultimate Stars! Sprite Format

#pragma magic [44 53 54 58] @ 0x00

import type.base;
import std.mem;

struct Segment {
    u16 tileIndex;
    s8 xPos;
    s8 yPos;
    u8 shape;
    u8 paletteIndex;
};

struct SpriteData1_3 {
    u16 offset;
    u16 segmentsCount @ offset + 0xA; 
    Segment segments[segmentsCount] @ offset + 0xC;
};

struct SpriteData2_3 {
    u16 offset;
    u16 segmentsCount @ offset + 0xA; 
    Segment segments[segmentsCount] @ offset + 0xC;
    s16 xTopLeft @ offset + 0x0C + segmentsCount * 0x06;
    s16 yTopLeft @ offset + 0x0E + segmentsCount * 0x06;
    u16 xBottomRight @ offset + 0x10 + segmentsCount * 0x06;
    u16 yBottomRight @ offset + 0x12 + segmentsCount * 0x06;
    u16 unknown @ offset + 0x14 + segmentsCount * 0x06; // padding ?
};

struct SpriteData1_4 {
    u8 width;
    u8 height;
    u16 tileIndex;
};

struct Palette {
    u8 colors[32];
};

struct DSIG1_50 {
    u32 magic;
    u8 version;
    u8 subversion; // Or image format??
    u16 numPalettes;
    u16 width;
    u16 height;
    Palette palettes[numPalettes];
    u8 pixels[while(!std::mem::eof())];
};

struct DTX {
    u32 magic;
    u8 version; 
    u8 subversion;
    u16 number_elements;
    type::Hex<u16> dsig_offset;
    if (version == 01 && subversion == 03) {
        SpriteData1_3 spriteData[number_elements];
    }
    if (version == 02 && subversion == 03) {
        SpriteData2_3 spriteData[number_elements];
        DSIG1_50 dig @ dsig_offset; 
    }
    if (subversion == 04) {
        u16 unknown;
        SpriteData1_4 spriteData[number_elements];
    }
};

DTX dtx_file @ 0x00;