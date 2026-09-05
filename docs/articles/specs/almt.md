# ALMT animations

The _ALMT_ binary format (_AL MoTion_) stores information to animate
[DSTX](./dstx.md) sprites.

- [ImHex pattern](./resources/almt.hexpat).

## Format

The game only contains files with the version 2 format.

| Offset | Type           | Description                            |
| ------ | -------------- | -------------------------------------- |
| 0x00   | char[4]        | Format ID: `ALMT`                      |
| 0x04   | byte           | Version: 2                             |
| 0x05   | byte           | Flags (0): none supported              |
| 0x06   | byte           | Node count                             |
| 0x07   | byte           | Animation count                        |
| 0x08   | byte           | Commands name's count                  |
| 0x09   | byte[3]        | Reserved (0)                           |
| 0x0C   | char[4][]      | Name of nodes, 4 ASCII chars           |
| ...    | uint[]         | Absolute offsets to the animation data |
| ...    | ushort[]       | Absolute offsets to the command names  |
| ...    | CommandName[]  | Command names                          |
| ...    | AnimationSeq[] | Animation sequences                    |

### Command names

The offset points to the start of the command name. This is a null-terminated
ASCII string.

Two bytes before the name (before the offset), there is a 16-bits unsigned
number with the hash of the name. This is used for fast lookup algorithms. The
hash algorithm is the same as for [ALAR v2](./alar.md#hash-v2).

### Animation sequence

Each sequence has a header, followed by the animation data for each node.

In the case of version 2, the offsets are incremental. Meaning the next offset
is relative to the last offset. The first offset is always relative to this
section.

| Offset | Type     | Description                                             |
| ------ | -------- | ------------------------------------------------------- |
| 0x00   | uint     | Animation ID, same as the file ID in the ALAR container |
| 0x04   | ushort   | Unknown                                                 |
| 0x06   | byte     | Unknown                                                 |
| 0x07   | byte     | Flags: only checks for the first bit, unknown           |
| 0x08   | ushort[] | Relative offsets for node animation data                |

### Animation node

| Offset | Type     | Description                                     |
| ------ | -------- | ----------------------------------------------- |
| 0x00   | byte     | Command count                                   |
| 0x01   | byte     | Unknown count                                   |
| 0x02   | byte[]   | Commands index                                  |
| ...    | byte[]   | Similar for the unknown count (other commands?) |
| ...    | byte[]   | Padding to offset multiple of two               |
| ...    | ushort[] | Command data offsets relative to the node data  |
| ...    | ushort[] | Similar for the unknown count                   |
| ...    | byte[]   | Command data                                    |

The game engine has a specific implementation for each command (look up by
name). The command data depends on the type, its length is not fixed. There are
some commands (unknown part?) that takes a variable length and ends when finding
`0xFFFF`.

## Credits

The format research is independent by reverse engineering the game. We
acknowledge the work of
[keshire in GBATemp](https://gbatemp.net/threads/jump-ultimate-stars-charactermovefile.314168/)
that end up with a similar specification and created a tool for this complex
format.
