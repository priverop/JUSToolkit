struct Bitmap {
    char formatId[2];
    u32 sectionSize;
    u8 data[sectionSize-6];
};

struct CharInfo {
    u16 start;
    u16 startWithoutSpace;
    u16 end;
};

struct TableInfo {
    char sectionId[2];
    u8 unknown1;
    u8 unknown2;
    u16 unknown3;
    u16 charCount;
    CharInfo info[charCount];
};

struct AlFont {
    char formatId[4];
    u8 unk04;
    u8 flags; // AND 4 -> count, AND 2
    TableInfo charInfo;
    // Pad to pos 4
    if ($ & 3 == 2) {
        padding[2];
    }
    
    Bitmap bitmap;
};

AlFont font @0x00;