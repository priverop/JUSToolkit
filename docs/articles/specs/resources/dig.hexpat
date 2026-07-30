import type.color;

struct Palette {
   type::RGBA5551 Pixels[parent.colorsPerPalette];
};

struct DSIG {
    u32 magid;
    u8 version;
    u8 imageFormat; // 8bpp if different than 0x10
    u16 numPalettes;
    u16 width;
    u16 height;
    // PixelsStart
    u32 colorsPerPalette = 16;
    
    if (imageFormat != 0x10) {
        colorsPerPalette = 256;
        numPalettes = ((numPalettes - 1) / 16) + 1;
    }

    Palette palettes[numPalettes];
    u8 pixels[width * height / 2];
};

DSIG dig @ 0x0;