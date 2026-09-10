#pragma author pleonex
#pragma description Nintendo DS - Jump Ultimate Stars! animation format

#pragma magic [41 4C 4D 54] @ 0x00

struct AlmtNodeAnimation {
    u8 command_count;
    u8 unk_count;
    u8 command_idx[command_count];
    u8 unk_idx[unk_count];
    if ($ % 2 != 0) {
        padding[1];
    }

    u16 command_data_offsets[command_count];
    u16 unk_data_offsets[unk_count];
    // command data, then unk_data.
};

struct AlmtNodeAnimationOffset {
    u16 offset; // relative to -0x20 (yes, that's what the games does)
    AlmtNodeAnimation sequence @ offset - 0x20 + parent.base_offset;
};

struct AlmtAnimationSequence {
    u32 base_offset = $;

    u32 id; // same as the ALAR file ID incrementing the first byte.
    u16 unknown2;
    u8 unknown3;
    u8 flags; // game only checks bit 0
    AlmtNodeAnimationOffset nodes[parent.parent.node_count];
};

struct AlmtAnimationOffset {
    u32 offset;
    AlmtAnimationSequence sequence @ offset;
};

struct AlmtCommandNameOffset {
    u16 offset;
    u16 hash @ offset - 2; // same as ALAR v2
    char name[] @ offset; // null-terminated
};

struct AlmtNodeName {
    char name[4];
};

struct Almt {
    char format_id[4];
    u8 version;
    u8 flags; // none, 0
    u8 node_count;
    u8 animation_count;
    u8 command_count;
    u8 reserved[3];

    AlmtNodeName nodes[node_count] @ 0x0C;
    AlmtAnimationOffset animations[animation_count] @ 0x0C + (node_count * 4);
    AlmtCommandNameOffset commands[command_count] @ 0x0C + (node_count * 4) + (animation_count * 4);
};

Almt format @ 0x00;
