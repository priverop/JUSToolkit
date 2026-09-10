struct Bitmap {
    // yep, it's a standard BMP format
    // bitmap file header
    char formatId[2];
    u32 sectionSize;
    u16 reserved1;
    u16 reserved2;
    u32 bitmapOffset;

    // DIB header 'BITMAPINFOHEADER' type
    u32 dibLength; // 40
    s32 width;
    s32 height;
    u16 colorPlanesCount; // must be 1
    u16 bpp; // supported 1, 4, 8
    u32 compressionMethod; // supported 0: BI_RGB (none) (not read)
    u32 imageSize; // + 2 for some (bug) reason
    s32 horizontalResolution; // pixel per metre
    s32 verticalResolution; // pixel per metre
    u32 paletteColorCount; // actually hard-coded in code: 2 for 1bpp, 16 for 4bpp, 256 for 8bpp
    u32 importantColorCount; // ignored, so 0

    // color table (hard-coded to be at 0x36)
    u32 palette[paletteColorCount]; // RGB32, forth byte (alpha?) is decided by runtime

    // pixels (inversed)
    // for i upto height:
    //   copy width bytes from pixels + width (/2 if 4bpp, /8 if 1bpp) * (height - i - 1)
    u8 pixels[imageSize - 2];
};

struct DsigPalette {
    u16 colors[16];
};

struct Dsig {
    char formatId[4];
    u8 version;
    u8 flags; // for fonts is 0x20: lineal + 4bpp
    u8 paletteCount;
    u8 reserved;
    u16 width;
    u16 height;
    DsigPalette palettes[paletteCount];
    u8 pixels[width * height / 2]; // 4bpp
};

struct GroupInfo {
    u16 codepointStart;
    u16 codepointEnd; // inclusive
    u16 glyphIdxStart;

    u16 count = codepointEnd - codepointStart + 1 [[export]];
};

struct TableInfo {
    char sectionId[2]; // TB
    u8 boxWidth; // in BMP includes border
    u8 boxHeight;
    u16 charsPerRow;
    u16 groupCount;
    GroupInfo info[groupCount];
};

bitfield AlFontFlags {
    bool imageFormat: 2; // 1: BMP (DSFont), 2: DSIG
    bool isAscii: 1; // count end - start + 1 while start <= 0xFF; otherwise SJIS
};

struct AlFont {
    char formatId[4]; // ALFT
    u8 version; // version 0
    AlFontFlags flags;
    TableInfo charInfo;

    // Pad to pos 4
    if ($ & 3 == 2) {
        padding[2];
    }

    if (flags.imageFormat == 2) {
        Dsig dsig;
    } else {
        Bitmap bitmap;
    }
};

AlFont font @0x00;
