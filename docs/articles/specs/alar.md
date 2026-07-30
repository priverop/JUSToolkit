# ALAR container

The _ALAR_ binary format (_AL ARchive_) specifies how to pack binary files
together. There are three versions of the format, but this game only has files
with version 2 and 3.

## Format

The binary format changes for each version. However, the header remains the same
across versions:

| Offset | Type    | Description             |
| ------ | ------- | ----------------------- |
| 0x00   | char[4] | Format ID: `ALAR`       |
| 0x04   | byte    | Version                 |
| 0x05   | byte    | Container feature flags |
| 0x06   | short   | Number of files         |

The feature flag is a bit-field with 8 bits as follows:

- 0: if set, container provides filenames.
- 1: unknown, set when the container has a file with file info flag bit 24 set.
- 2: if set, container supports folders and sub-container (ALAR).
- 3-5: reserved.
- 6: [file path hash](#file-path-hashes) version: 0=v1, 1=v2.
- 7: reserved.

The combinations found in the game assets are:

- Version 2: `0x01`, `0x03`.
- Version 3: `0x05`, `0x45`.

### Version 1

The format structure is:

- [Header](#format).
- Extended header.
- Lookup table.
- File access table.
- File data.

#### V1 Extender header

It contains of a 32-bits integer with the first file ID.

#### V1 Lookup table

There is one entry per file with the information to find the index of the file
in the container by path or file ID. The size of each entry depends on the
container features, if it supports file names or not.

- (only if bit0 is set, for filename support) `char[32]` filename.
- `uint` file ID.

#### V1 File access table

There is one entry per file with the absolute offset to the file data. Each
entry is a 32-bits integer.

#### V1 File data

The file data format is:

- `uint` data size.
- `byte[size]` data.

### Version 2

The format structure is:

- [Header](#format).
- Extended header.
- File info table.
- File data.

#### V2 Extended header

There are four additional bytes:

| Offset | Type | Description                  |
| ------ | ---- | ---------------------------- |
| 0x08   | uint | First file ID (without type) |
| 0x0C   | uint | Last file ID (without type)  |

The last file ID takes into account the frame count from `ALMT` / `ALOD` files
as it were additional files.

#### V2 File info

There are 16 bytes per file:

| Offset | Type | Description               |
| ------ | ---- | ------------------------- |
| 0x00   | uint | File ID                   |
| 0x04   | uint | File data absolute offset |
| 0x08   | uint | File data size            |
| 0x0C   | uint | File info flags           |

The highest byte of the file ID defines the [format type](#file-types). The rest
bytes identify the final asset (like an image).

The file info flags have two parts:

- bits 0-23: _frame count_ if bit 30 is set, otherwise `1`.
  - This seems to be set only for `ALMT` and `ALOD` files.
- bit 24: unknown, only set for some `DSIG` formats.
- bit 25-29: reserved.
- bit 30: if set, the bits 0-23 define the frame count.
- bit 31: if set, before the file data there is the file name and hash.

#### V2 File data

The file data is at an address multiple of 4. There are padding bytes (0x00)
if it's not. The last file does not have padding at the end of the file.

| Offset | Type     | Description               |
| ------ | -------- | ------------------------- |
| -0x24  | byte[2]  | Padding                   |
| -0x22  | char[32] | Filename (ASCII)          |
| -0x02  | ushort   | [Filename hash](#hash-v1) |
| 0x00   | byte[]   | File data                 |

> [!NOTE]  
> Different to version 1, the file data offset points to the start of the data.
> The path and checksum (if available via file info flags), are 36 bytes before
> the address of the file data offset.

### Version 3

The format structure is:

- [Header](#format).
- Extended header.
- File info table.
- File data.

The file hierarchy follows a "depth-first" approach.

#### V3 Extended header

| Offset | Type     | Description                  |
| ------ |----------| ---------------------------- |
| 0x08   | uint     | First file ID (without type) |
| 0x0C   | uint     | Last file ID (without type)  |
| 0x10   | ushort   | Data offset                  |
| 0x12   | ushort[] | File info absolute offsets   |

The block ends with padding (0x00) until an address multiple of 4.

#### V3 File info

The size of the entries is variable (it depends on the file path). They have
padding bytes (0x00) to align the end position to an address multiple of 4.

| Offset | Type   | Description                             |
| ------ |--------| --------------------------------------- |
| 0x00   | uint   | File ID                                 |
| 0x04   | uint   | File data absolute offset               |
| 0x08   | uint   | File data size                          |
| 0x0C   | uint   | Flags                                   |
| 0x10   | ushort | [File path hash](#file-path-hashes)     |
| 0x12   | char[] | Null-terminated file path ASCII encoded |

The file ID and info flags have the same meaning as in
[version 2 file info](#v2-file-info).

The file data offset is an address multiple of 4. There are padding bytes (0x00)
if it's not. The last file does not have padding at the end of the file.

## File path hashes

The format stores a precomputed hash of the file path in the container, to speed
up the file lookup process. At runtime, the game computes the same hash of the
requested path, and compares with the stored value. If it matches, it does also
compare the path char by char to prevent false positives due to hash collisions.

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

- `0x00`: undefined format, specific binary data structure.
- `0x40`: `DSTX`
- `0x41`: `ALMT`
- `0x42`: `DSIG`
- `0x43`: `ALOD`
- `0x45`: `NCCL`
- `0x46`: `ALTM`
- `0x47`: `ALAR`
