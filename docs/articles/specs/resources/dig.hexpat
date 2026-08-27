import type.color;

struct DsigPalette {
    // always 16 colors per palette
    // the game may join palettes later for 8bpp, but it reads as separate
    type::RGBA5551 colors[16];
};

bitfield DsigFlags {
    bpp: 4; // 0: 4bpp, 1: 8bpp
    swizzling: 2; // 0: tbd, 1: tiled, 2: lineal, 3: tbd
    compression: 1;
    reserved7: 1;
};

struct Dsig {
    char formatId[4];
    u8 version;
    DsigFlags flags;
    u8 paletteCount;
    u8 unknown1; // only v2
    u16 width;
    u16 height;
    DsigPalette palettes[paletteCount];

    if (version != 1) {
        u32 unknown2;
    }

    if (flags.bpp == 0) {
        u8 pixels[width * height / 2];
    } else {
        u8 pixels[width * height];
    }
};

Dsig dig @ 0x00;
