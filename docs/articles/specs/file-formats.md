# File Formats

This is the format specification from the [English translation](https://web.archive.org/web/20100111220659/http://jumpstars.wikispaces.com/File+Formats#toc10).

## Compression
### DSCP Tag
```
struct DSCP
{
    char                ID[4];
    dword                GBA_Compression_header; //lzss
};
```
## Archive
### .AAR files
#### ALAR Tag (2)
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
#### ALAR Tag (3)
Used by ChrArc.aar

```
struct ALAR_Type_3
{
    char                 ID[4];
    byte                Type;
    byte                Unknown;
    dword                 NumberFiles;
    word                 Unknown;
    dword                 ArrayCount;
    word                 EndFileIndex;
    word                 FileTableIndex[ArrayCount+1];
                    //Padding
    struct ALAR_3_FileHeader     FileIndex[NumberFiles];
};
```
 
```
struct ALAR_3_FileHeader
{
    word             FileID;
    word             Unknown;
    dword             StartOfFile;
    dword             SizeOfFile;
    word             Unknown;
    word             Unknown;
    word             Unknown;
    char             FileName[18]; //Pad to next entry
};
```
## Sprite
### .DIG files (Sometimes encapsulated within a .DTX file)
#### DSIG Tag (1)

```
struct DSIG_Type_1
{
    char                ID[4];
    byte                Type;
    byte                Type_Alt;
    byte                PaletteNumber; //32 byte Color Palette
    byte                Unknown;  //Bit Depth?
    word                Unknown;
    word                Unknown;
    struct  DSIG_Palette        Palette[PaletteNumber];
};
```
#### DSIG Tag (2)
```
struct DSIG_Type_2
{
    char                ID[4];
    byte                Type;
    byte                Type_Alt;
    byte                PaletteNumber; //32 bytes apeice
    byte                Unknown;  //Bit Depth?
    word                Unknown;
    word                Size;
    struct DSIG_Palette        Palettes[PaletteNumber];
 
    dword                Unknown;
    dword                NumberFrames;
    struct DSIG_Offsets        Frames[NumberFrames]; //Each frame starts with a 4 byte lzss header
};
```
 
```
struct DSIG_Offsets
{
    word            Offset;
    word            Size;
};
```
## Tiling Data
### .DTX Files (matched with .DIG files)
#### DSTX Tag (1)
```
struct DSTX_Type_1
{
    char                ID[4];
    ubyte                Type;
    ubyte                Type_Alt;
    word                Frames;
    word                Size;
    word                Index[Frames]; //Relative Offset to next Index.
    struct DSXT_1_TileTable        Frame[Frames];
};
```
 
```
struct DSXT_1_TileTable
{
    word            Count;
    struct DSXT_1_Block    Array[Count];
 
};
```
 
```
struct DSXT_1_Block
{
    byte            Number;
    byte            Unknown;
    byte            Width_Offset;
    byte            Height_Offset;
    byte            Arrangement;
    byte            Unknown; //Pallette Number??
};
```

```
Tile Arrangements
0     1x1
1     2x2
2     4x4
4     2x1
5     4x1
6     4x2
7     8x4
8     1x2
9     1x4
10    2x4
```
#### DSTX Tag (1.4)
I know it exists but haven't looked at it

#### DSTX Tag (2)
This type is the devil
```
struct DSTX_Type_2
{
    char                ID[4];
    ubyte                Type;
    ubyte                Type_Alt;
    word                Frames;
    word                Size;
    word                Index[Frames]; //Relative Offset to next Index.
    struct DSXT_2_TileTable        Frame[Frames];
};
```
 
```
struct DSXT_2_TileTable
{
    word            Count;
    struct DSXT_2_Block    Array[Count]; //Smaller Partitions of the Total Size
    //Total Frame Size
    byte            Width_1;
    byte            Unknown;
 
    byte            Height_1;
    byte            Unknown;
 
    byte            Width_2;
    byte            Unknown;
 
    byte            Height_2;
    byte            Unknown;
};
```
 
```
struct DSXT_2_Block
{
    byte            Position; //Location on Sprite, 1 bit X, 1 bit Y 8X8 blocks.
    byte            Layer;    //Ordering for overlapping Blocks
    byte            X_Offset; //Offset within Total Frame size
    byte            Y_Offset; //Offset within Total Frame size
    byte            Arrangement;  //
    byte            Palette;  //Palette number to use
};
```

```
Best guess as to the frame arrangements
1 = 16X16
2 = 32X32
3 =
4 = 16x8
5 = 32x8
6 = 32x16
7 =
8 =
9 =  8X32
A = 16X32
```
## Move File
### .AMT files
#### ALMT Tag
This is only the header
```
struct ALMT
{
    char                ID[4];
    byte                Unknown;
    byte                Unknown;
    byte                Count_1;
    byte                Count_2;
    dword                Count_3;
    char                Unknown[Count_1*4]; //Some other Identifier
    dword                DataOffsets[Count_2];
    word                StringOffsets[Count_3];
        //Null Terminated strings preceded by an unknown word..
};
```
Unknown Files
.AOD Files
ALOD Tag