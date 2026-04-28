# ALAR container

The _ALAR_ binary format (_AL ARchive_) specifies how to pack binary files
together. There are three versions of the format, but there are no remains of
assets with version 1.

## Format

The binary format changes for each version. However, the header remains the same
across versions:

| Offset | Type    | Description             |
| ------ | ------- | ----------------------- |
| 0x00   | char[4] | Format ID: `ALAR`       |
| 0x04   | byte    | Version                 |
| 0x05   | byte    | Container feature flags |
| 0x06   | short   | Number of files         |

The feature flag is a bit-field:

- 0: filename support
- 1: unknown
- 2: folder and sub-container (ALAR) support
- 3-5: reserved
- 6: path hash version: 0=v1, 1=v2
- 7: reserved

The possible combinations are:

- Version 2: `0x01`, `0x03`
- Version 3: `0x05`, `0x45`

### Version 1

The format structure is:

- [Header](#format)
- Extended header
- Lookup table
- File access table
- File data

#### V1 Extender header

It contains of a 32-bits integer with the first file ID.

#### V1 Lookup table

There is one entry per file with the information to find the index of the file
in the container by path or file ID. The size of each entry depends on the
container features, if it supports file names or not.

- (only if bit0 is set, for filename support) `char[32]` filename
- `uint` file ID

#### V1 File access table

There is one entry per file with the absolute offset to the file data. Each
entry is a 32-bits integer.

#### V1 File data

The file data format is:

- `uint` data size
- `byte[size]` data

### Version 2

The format structure is:

- [Header](#format)
- Extended header
- File info table
- File data

#### V2 Extended header

It has two additional 32-bits integers with the first, and last file ID (without
the type). The last file ID takes into account the frame count from `ALMT` /
`ALOD` files as it were additional files.

#### V2 File info

There are 16 bytes per file:

| Offset | Type | Description               |
| ------ | ---- | ------------------------- |
| 0x00   | uint | File ID                   |
| 0x04   | uint | File data absolute offset |
| 0x08   | uint | File data size            |
| 0x0C   | uint | File info flags           |

The highest byte of the file ID defines the [format type](#file-types).

The file info flags have two parts:

- bits 0-23: frame count
- bit 24: unknown, only set for some `DSIG` formats
- bit 25-29: reserved
- bit 30: if set, the lower part contains the frame count
- bit 31: if set, before the file data there is the file name and hash

#### V2 File data

| Offset | Type     | Description               |
| ------ | -------- | ------------------------- |
| -0x24  | byte[2]  | Padding                   |
| -0x22  | char[32] | Filename (ASCII)          |
| -0x02  | ushort   | [Filename hash](#hash-v1) |
| 0x00   | byte[]   | File data                 |

> [!NOTE]  
> Different to version 1, the file data offset points to the start of the data.
> The path and checksum (if available via file info flags), are 34 bytes before
> the address of the file data offset.

### Version 3

The format structure is:

- [Header](#format)
- Extended header
- File info table
- File data

#### V3 Extended header

| Offset | Type    | Description                  |
| ------ | ------- | ---------------------------- |
| 0x08   | uint    | First file ID (without type) |
| 0x0C   | uint    | Last file ID (without type)  |
| 0x10   | short   | Data offset                  |
| 0x12   | short[] | File info absolute offsets   |

#### V3 File info

| Offset | Type     | Description                             |
| ------ | -------- | --------------------------------------- |
| 0x00   | uint     | File ID                                 |
| 0x04   | uint     | File data absolute offset               |
| 0x08   | uint     | File data size                          |
| 0x0C   | uint     | Flags                                   |
| 0x10   | ushort   | [File path hash](#name-hashes)          |
| 0x12   | char[18] | Null-terminated file path ASCII encoded |

The file ID and info flags have the same meaning as in
[version 2 file info](#v2-file-info).

## Name hashes

The format stores a precomputed hash of the file name, to speed up the file
lookup process. At runtime, the game computes the same hash of the requested
file path, and compare with the stored value. If it matches, it does also
compare the name char by char to prevent false positives due to hash collisions.

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

## File types

- `0x00`: undefined format, specific binary data structure
- `0x40`: `DSTX`
- `0x41`: `ALMT`
- `0x42`: `DSIG`
- `0x43`: `ALOD`
- `0x45`: `NCCL`
- `0x46`: `ALTM`
- `0x47`: `ALAR`
