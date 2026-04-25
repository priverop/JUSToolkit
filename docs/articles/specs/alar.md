# ALAR container

The _ALAR_ binary format (_AL ARchive_) specifies how to pack binary files
together. There are three versions of the format, although version 1 is used
anymore (but some blocks of code remains in the games).

## Format

The format structure is:

- Header
- Padding (multiple of 4)
- File info table
- File data

### Header

| Offset | Type    | Description                   |
| ------ | ------- | ----------------------------- |
| 0x00   | char[4] | Format ID: `ALAR`             |
| 0x04   | byte    | Version: `3`                  |
| 0x05   | byte    | Flags: `05` or `45`           |
| 0x06   | short   | Number of files               |
| 0x08   | int     | Reserved: `0`                 |
| 0x0C   | short   | Number of entries (files - 1) |
| 0x10   | short   | Data offset                   |
| 0x12   | short[] | File info absolute offsets    |

The flag is a bitfield:

- 0: always v2 and v3 (has path?)
- 1: maybe v2: has filename?
- 2: always v3: new fileinfo?
- 3-5: reserved
- 6: maybe v3: if set, use checksum version 2 instead of 1.
- 7: reserved

The possible combinations are:

- Version 2:
  - `0x01`: standard format for v3
  - `0x03`
- Version 3:
  - `0x05`: standard format for v3
  - `0x45`: use newer file path hash algorithm

### V2 File info

| Offset | Type | Description               |
| ------ | ---- | ------------------------- |
| 0x00   | uint | Global file ID            |
| 0x04   | uint | File data absolute offset |
| 0x08   | uint | File data size            |
| 0x0C   | uint | File info flags           |

### V2 File data

| Offset | Type     | Description                |
| ------ | -------- | -------------------------- |
| 0x00   | short    | Padding                    |
| 0x02   | char[32] | Null-terminated file path  |
| 0x22   | ushort   | [File path hash](#hash-v1) |
| ...    | Stream   | File data                  |

### V3 File info

| Offset | Type     | Description                             |
| ------ | -------- | --------------------------------------- |
| 0x00   | uint     | Global file ID                          |
| 0x04   | uint     | File data absolute offset               |
| 0x08   | uint     | File data size                          |
| 0x0C   | uint     | Flags                                   |
| 0x10   | ushort   | [File path hash](#name-hashes)          |
| 0x12   | char[18] | Null-terminated file path ASCII encoded |

The highest 8 bits of the file ID indicate the file type.

The file info flags have two parts:

- bits 0-23: format flags. Usually 1 except for `.amt` files.
- bits 24-31: entry flags
  - bit 31: if set, the file info entry contains the checksum and file path.

## Name hashes

The format store a precomputed hash of the file name, to speed up the file
lookup process. At runtime, the game computes the same hash of the requested
file path, and compare with the stored value. This is faster than comparing
paths char by char.

There are two known versions of the hash algorithm. ALAR version 2 uses the
first version. ALAR version 3 has a flag in the header to indicate the hash
version.

> [!NOTE]  
> In both versions, the hash is over the path of the file inside the ALAR
> container **without the file extension**. If the ALAR contains a file
> `data/file.bin`, the game computes the hash over `data/file`. The text
> encoding is ASCII. There is also a limit of 32 bytes. Longer paths are
> truncated.

> [!WARNING]  
> It seems the first version causes some issues (hash collisions?). The game
> disabled file lookup by hash, when the ALAR used version 1. In those cases, it
> performs a full path comparison char by char.

### Hash v1

```csharp
byte[] data = Encoding.ASCII.GetBytes(pathWithoutExtension);

uint hash = 0;
foreach (byte ch in data) {
    uint tmp = (hash << 1) ^ ch;
    hash = (tmp & 0xFFFF) ^ (tmp >> 16);
}
```

### Hash v2

```csharp
byte[] data = Encoding.ASCII.GetBytes(pathWithoutExtension);

uint hash = 0;
foreach (byte ch in data) {
    hash ^= (ushort)(hash << 1) | ch;
}
```
