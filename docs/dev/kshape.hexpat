#pragma author priverop
#pragma description Nintendo DS - Jump Ultimate Stars! KShape Format

import type.base;

struct Sprite {
    u32 uno;
    u32 dos;
    u32 tres;
    u32 cuatro;
    u32 cinco;
    u32 seis;
};

struct KShape {
    u32 group[8];
    u32 numElements[8];
    Sprite spriteInfo[66];
};

KShape kshape_file @ 0x00;