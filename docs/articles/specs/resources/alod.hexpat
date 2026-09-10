#pragma author pleonex
#pragma description Nintendo DS - Jump Ultimate Stars! object definition format

#pragma magic [41 4C 4F 44] @ 0x00

struct AlodNodeName {
    u32 offset;
    char name[4] @ offset + parent.base_offset;
};

struct AlodNodeDefinition {
    u32 base_offset = $;
    char name[4];
    char group[4];
    u8 count;
    u8 unk_count;
    u8 reserved[2];
    AlodNodeName parents[count];
};

struct AlodNodeDefinitionOffset {
    u16 offset;
    AlodNodeDefinition def @ offset;
};

struct AlodRootNode {
    u16 offset;
    char name[] @ offset;
};

struct Alod {
    char format_id[4];
    u8 version;
    u8 flags; // none, 0
    u8 node_count;
    u8 root_count;
    u8 reserved[4];
    u32 unknown_offset;

    AlodNodeDefinitionOffset nodes[node_count];
    AlodRootNode roots[root_count];

    if (unknown_offset != 0) {
        u8 unknown_data[] @ unknown_offset;
    }
};

Alod format @ 0x00;
