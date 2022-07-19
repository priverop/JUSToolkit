# ALAR Containers

## ALAR 3

This format is useful to store a lot of files. We have the header where we store all the pointers and then we have the file contents all together.

| Offset | Type    | Description                       |
| ------ | ------- | --------------------------------- |
| 0x00   | char[4] | ALAR                              |
| 0x04   | byte    | Version (3 to follow)             |
| 0x05   | byte    | Minor version?                    |
| 0x06   | int     | Number of files                   |
| 0x0A   | short   | Reserved?                         |
| 0x0C   | int     | Number of entries                 |
| 0x10   | short   | Data offset (pointer section end) |
| 0x12   | short[] | File info absolute pointers       |
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
| 0x10   | 2    | 6085     | Data Offset             | 8560          |                   |
| 0x12   | 2    | 1807     | File info first pointer | 718           |                   |

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
```
struct ALAR_Type_2
{
    char                ID[4];
    byte                Type;
    byte                Unknown;
    word                NumberFiles;
    ubyte                ID1;
    ubyte                ID2;
    ubyte                ID3;
    ubyte                ID4;
    ubyte                ID5;
    ubyte                ID6;
    ubyte                ID7;
    ubyte                ID8;
    struct ALAR_2_Index        Index[NumberFiles];
                    //Padding
};
```

```
struct ALAR_2_Index
{
    dword            Type;
    dword            Start;
    dword            Size;
    ubyte            Unknown;
    ubyte            Unknown;
    ubyte            Unknown;
    ubyte            Unknown;
};
```
