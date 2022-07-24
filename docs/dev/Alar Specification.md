# ALAR Containers

## ALAR 3

This format is useful to store a lot of files. We have the header where we store all the pointers and then we have the file contents all together.

| Offset | Type    | Description                        |
| ------ | ------- | ---------------------------------- |
| 0x00   | char[4] | ALAR                               |
| 0x04   | byte    | Version (3 to follow)              |
| 0x05   | byte    | Minor version?                     |
| 0x06   | int     | Number of files (includes folders) |
| 0x0A   | short   | Reserved?                          |
| 0x0C   | int     | Number of entries (not folders)    |
| 0x10   | short   | Data offset (pointer section end)  |
| 0x12   | short[] | File info absolute pointers        |
padding?
| ..     | FileInfo[] | File info list              |
| ..     | Stream[]   | File data                   |

### File info

| Offset | Type   | Description                 |
| ------ | ------ | --------------------------- |
| 0x00   | short  | ID                          |
| 0x02   | short  | Unknown                     |
| 0x04   | int    | Absolute pointer            |
| 0x08   | int    | Size                        |
| 0x0C   | short  | Unknown                     |
| 0x0E   | short  | Unknown                     |
| 0x10   | short  | Unknown                     |
| 0x12   | string | Null-terminated file path** |

** char FileName[18] // 14bytes string + 4bytes pad to next entry - null terminated

### Example

koma.aar

| Offset | Size | Hex      | Description             | Content (Hex) | Content (Decimal) |
| ------ | ---- | -------- | ----------------------- | ------------- | ----------------- |
| 0x00   | 4    | 414C4152 | ALAR                    |
| 0x04   | 1    | 03       | Version                 |
| 0x05   | 1    | 05       | Minor Version           |
| 0x06   | 4    | 82030000 | Number of Files         | 382           | 898               |
| 0x0A   | 2    | 000000   | Reserved                |
| 0x0C   | 4    | 81030000 | Number of entries       | 381           | 897               |
| 0x10   | 2    | 6085     | Data Offset             | 8560          |
| 0x12   | 2    | 1807     | File info first pointer | 718           |

First file: koma/bb_00.dtx

| Offset | Size | Hex      | Description                 | Content (Hex) | Content (Decimal) |
| ------ | ---- | -------- | --------------------------- | ------------- | ----------------- |
| 0x718  | 2    | 0000     | ID                          |
| 0x71A  | 2    | 0040     | Unknown                     | 40            |
| 0x71C  | 4    | 60850000 | Absolute Pointer            | 8560          |
| 0x720  | 4    | DC040000 | Size                        | 04DC          | 1244              |
| 0x724  | 2    | 0100     | Unknown                     |
| 0x726  | 2    | 0080     | Unknown                     |
| 0x728  | 2    | 3C96     | Unknown                     |
| 0x72A  | 18   | 1807     | File path: koma/bb_00.dtx** |

** The real size is 14bytes but we have a 00 byte (null terminated)

### List of Alar3 Files

- koma.aar

## ALAR 2

This format is useful to store few files. We have a small header, the info of the files and then the files one after another.

| Offset | Type       | Description     |
| ------ | ---------- | --------------- |
| 0x00   | char[4]    | ALAR            |
| 0x04   | byte       | Version (2)     |
| 0x05   | byte       | Minor version   |
| 0x06   | short      | Number of files |
| 0x08   | byte[8]    | IDs?            |
| 0x10   | FileInfo[] | File info list  |
| ...    | FileData[] | File data       |

### File Info

| Offset | Type | Description      |
| ------ | ---- | ---------------- |
| 0x00   | int  | Unknown? Type?   |
| 0x04   | int  | Absolute pointer |
| 0x08   | int  | Size             |
| 0x0A   | int  | Unknown          |

### File Data

| Offset | Type              | Description                         |
| ------ | ----------------- | ----------------------------------- |
| 0x00   | short             | Unknown, padding?                   |
| 0x02   | string (32 bytes) | Null-terminated file path + padding |
| ...    | short             | Unknown                             |
| ...    | Stream            | File Data                           |

### Example

deck_obj.aar

Header

| Offset | Size | Hex               | Description     | Content (Hex) | Content (Decimal) |
| ------ | ---- | ----------------- | --------------- | ------------- | ----------------- |
| 0x00   | 4    | 414C4152          | ALAR            |
| 0x04   | 1    | 02                | Version         |
| 0x05   | 1    | 01                | Minor Version   |
| 0x06   | 2    | 0200              | Number of Files | 2             | 2                 |
| 0x08   | 8    | 00006800 09006800 | IDs             |

File info #1

| Offset | Size | Hex      | Description      | Content (Hex) | Content (Decimal) |
| ------ | ---- | -------- | ---------------- | ------------- | ----------------- |
| 0x10   | 4    | 00006840 | ID               |
| 0x14   | 4    | 54000000 | Absolute Pointer | 54            |
| 0x18   | 4    | DC010000 | Size             | 1DC           | 476               |
| 0x1C   | 4    | 01000080 | Unknown          |

File info #2

| Offset | Size | Hex      | Description      | Content (Hex) | Content (Decimal) |
| ------ | ---- | -------- | ---------------- | ------------- | ----------------- |
| 0x20   | 4    | 00006840 | ID               |
| 0x24   | 4    | 54020000 | Absolute Pointer | 254           |
| 0x28   | 4    | 10010000 | Size             | 110           | 272               |
| 0x2C   | 4    | 0A0000C0 | Unknown          |

File data #1: deck_obj.dtx

| Offset | Size | Hex                                   | Description      |
| ------ | ---- | ------------------------------------- | ---------------- |
| 0x30   | 2    | 0000                                  | Unkown, padding? |
| 0x32   | 32   | 6465636B 5F6F626A 2E647478 + 20 zeros | deck_obj.dtx     |
| 0x52   | 2    | 7A22                                  | Unknown          |
| 0x54   | 476  | 44535458...                           | Stream           |

File data #2: deck_obj.amt

| Offset | Size | Hex                                   | Description      |
| ------ | ---- | ------------------------------------- | ---------------- |
| 0x230  | 2    | 0000                                  | Unkown, padding? |
| 0x232  | 32   | 6465636B 5F6F626A 2E616D74 + 20 zeros | deck_obj.amt     |
| 0x252  | 2    | 7A22                                  | Unknown          |
| 0x254  | 272  | 44535458...                           | Stream           |

### List of Alar2 Files

- battle/deck_obj.aar
- symbol02.aar