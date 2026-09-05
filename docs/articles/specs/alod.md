# ALOD structure definition

The _ALOD_ binary format (_AL Object Definition_) describes the structure of
sprites and their animations. It defines a tree that later the
[_ALMT_](./almt.md) uses to animate.

- [ImHex pattern](./resources/alod.hexpat)

## Format

| Offset | Type      | Description                        |
| ------ | --------- | ---------------------------------- |
| 0x00   | char[4]   | Format ID: `ALOD`                  |
| 0x04   | byte      | Version: only 2?                   |
| 0x05   | byte      | Flags (0 always?)                  |
| 0x06   | byte      | Node count                         |
| 0x07   | byte      | Root node count                    |
| 0x08   | byte[4]   | Reserved?                          |
| 0x0C   | uint      | Offset to unknown data             |
| 0x10   | ushort[]  | Absolute offset to nodes           |
| ...    | ushort[]  | Absolute offset to root node names |
| ...    | char[]    | Null-terminated root node names    |
| ...    | NodeDef[] | Node definition                    |

### Node definition

| Offset | Type    | Description                                       |
| ------ | ------- | ------------------------------------------------- |
| 0x00   | char[4] | Name                                              |
| 0x04   | char[4] | Group name?                                       |
| 0x08   | byte    | Count                                             |
| 0x09   | byte    | Unknown count                                     |
| 0x0A   | byte[2] | Reserved?                                         |
| 0x0C   | uint[]  | Relative offset to this section for parent names? |
| ...    | char[4] | Parent names?                                     |
