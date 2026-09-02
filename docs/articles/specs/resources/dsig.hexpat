#pragma author pleonex
#pragma description Nintendo DS - Jump Ultimate Stars! indexed image

#pragma magic [44 53 49 47] @ 0x00

import type.color;

struct DsigPalette {
    // always 16 colors per palette
    // the game may join palettes later for 8bpp, but it reads as separate
    type::RGBA5551 colors[16];
};

struct DsigBlock {
    u16 blockOffset;
    u16 blockLength;

    u8 compressedData[blockLength] @ parent.blockBasePos + (blockOffset * 4);
};

bitfield DsigFlags {
    bpp: 4; // 0: 4bpp, 1: 8bpp
    format: 4; // 0: reserved, 1: tiled, 2: lineal, 3: unk, 4: compressed, 5: unk.
};

struct Dsig {
    char formatId[4];
    u8 version;
    DsigFlags flags;
    u8 paletteCount;

    if (flags.format == 4) {
        u8 metadataLength; // tbc
        u16 blockInfoLength; // divided by 4
        u16 blockDataLength; // divided by 4
    } else {
        u8 reserved;
        u16 width;
        u16 height;
    }

    DsigPalette palettes[paletteCount];

    if (flags.format == 4) {
        if (version != 1) {
            u32 unknown;
        }

        u32 blockBasePos = $;
        u32 blockCount;
        DsigBlock blocks[blockCount];
    } else if (flags.bpp == 0) {
        u8 pixels[width * height / 2];
    } else {
        u8 pixels[width * height];
    }
};

Dsig dig @ 0x00;
