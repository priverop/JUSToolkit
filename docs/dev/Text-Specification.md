# Text files

In this game the texts are stored in .bin files. Usually we have pointers and
sentences, but each file is different, that's why we have a format for each type
of file.

## Absolute pointers

Absolute pointers means that the text is in the pointer offset plus the position
of the pointer. If the position of the pointer is 0x04 and the value is 0x100,
the text will be in 0x104.

## Utility classes

To help with the process we've developed some classes to read and write easily:

- **JusText.ReadIndirectString:** This method help us to read absolute pointers.
  It reads 4 bytes (absolute pointer), adds up our position, goes to that offset
  and read until a null byte.
- **JusText.WriteStringPointer:** This allows us to write absolute pointers. It
  needs the **JusIndirectText** Class which stores the _StartingOffset_ (where
  the pointer section ends and the text section starts), the strings to write
  and the pointers to these strings.

## Battle folder

Here we have the tutorials. **Relative Pointers**. They all have the same
structure: _StartingOffset_, a lot of random unknown ints and the pointers.
These pointers just store the size of the string starting from 0. For example if
the first two strings are 2bytes and 4bytes long, the pointers will be 02 and
then 06.

We store the unused pointers in the Po file as comments.

| Name          | Format         |
| ------------- | -------------- |
| tutorial0.bin | BattleTutorial |
| tutorial1.bin | BattleTutorial |
| tutorial2.bin | BattleTutorial |
| tutorial3.bin | BattleTutorial |
| tutorial4.bin | BattleTutorial |
| tutorial5.bin | BattleTutorial |

## Bin folder

The Format class has the main properties of the file and a list of Entries.

| Image                                             | Name          | Format                   | Location                                            |
| ------------------------------------------------- | ------------- | ------------------------ | --------------------------------------------------- |
| ![ability_t.bin](../images/formats/ability_t.png) | ability_t.bin | Ability + AbilityEntry   | Koma list, helper (koma size 1) ability description |
| ![bgm.bin](../images/formats/bgm.png)             | bgm.bin       | Bgm + BgmEntry           | Battle stage Background Music (Database menu)       |
| ![chr_b_t.bin](../images/formats/chr_b_t.png)     | chr_b_t.bin   | BtlChr + BtlChrEntry     | Koma abilities info                                 |
| ![chr_s_t.bin](../images/formats/chr_s_t.png)     | chr_s_t.bin   | SuppChr + SuppChrEntry   | Koma abilities info support komas                   |
| ![clearlst.bin](../images/formats/clearlst.png)   | clearlst.bin  | SimpleBin                | Stage goals                                         |
| ![commwin.bin](../images/formats/commwin.png)     | commwin.bin   | Commwin + CommwinEntry   | Common window messages                              |
| ![demo.bin](../images/formats/demo.png)           | demo.bin      | Demo + DemoEntry         | World names (demo player menu, database)            |
| ![infoname.bin](../images/formats/infoname.png)   | infoname.bin  | SimpleBin                | Main menu helper names                              |
| ![komatxt.bin](../images/formats/komatxt.png)     | komatxt.bin   | Komatxt + KomatxtEntry   | Koma names                                          |
| ![location.bin](../images/formats/location.png)   | location.bin  | Location + LocationEntry | Player location                                     |
| ![piece.bin](../images/formats/piece.png)         | piece.bin     | Piece + PieceEntry       | Manga author and info (database menu)               |
| ![pname.bin](../images/formats/pname.png)         | pname.bin     | Pname                    | Player name titles (player settings)                |
| ![rulemess.bin](../images/formats/rulemess.png)   | rulemess.bin  | Rulemess + RulemessEntry | Stage rules                                         |
| ![stage.bin](../images/formats/stage.png)         | stage.bin     | Stage + StageEntry       | Stage names                                         |
| ![title.bin](../images/formats/title.png)         | title.bin     | SimpleBin                | Manga names                                         |

### Formats

The Format class has the main properties of the file and a list of Entries. Each
Entry has different parameters, which are listed here.

Pointer count is the total number of entries. Entry size is the size of the
total entry in the pointer section. The starting offset means that the pointer
section ends and the text section starts.

EntrySize is the size of each header pointer entry. For example:
`A8 00 00 00 A9 00 00 00 B4 00 00 00 BE 00 00 00` the size would be 4 (0x04).

| Format                   | Format Description                                   | Entry Description                                                                                                                          |
| ------------------------ | ---------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| Ability + AbilityEntry   | Starting offset + Absolute pointers                  | Title and Description.                                                                                                                     |
| Bgm + BgmEntry           | Pointer count + Absolute pointers                    | Title, Descriptions (3), Unkown 1 and 2, Icon.                                                                                             |
| BtlChr + BtlChrEntry     | Starting offset + Absolute pointers + padding (0x04) | Char name, abilities and furiganas; passive name, passive furigana, passive descriptions, ability descriptions and interactions; unknowns. |
| Commwin + CommwinEntry   | Pointer count + Absolute pointers                    | ?                                                                                                                                          |
| Demo + DemoEntry         | Pointer count + Absolute pointers + padding (0x04)   | Title, Descriptions (3), Id and Icon.                                                                                                      |
| Komatxt + KomatxtEntry   | Starting offset + Absolute pointers                  | Name and two unknowns (int).                                                                                                               |
| Location + LocationEntry | Pointer count + Absolute pointers                    | Name and two unkown values (short).                                                                                                        |
| Piece + PieceEntry       | Pointer count + Absolute pointers                    | Title, Authors, Info, Page1, Page2, unknown, Id.                                                                                           |
| Pname (only strings)     | Pointer count + Absolute simple pointers             | Just strings.                                                                                                                              |
| Rulemess + RulemessEntry | Starting offset + Absolute pointers                  | 3 Descriptions + int unknown.                                                                                                              |
| SimpleBin                | Starting offset + Absolute pointers                  | Just strings.                                                                                                                              |
| Stage + StageEntry       | Starting offset + Absolute pointers                  | Name and two unkown values (short).                                                                                                        |
| SuppChr + SuppChrEntry   | Starting offset + Absolute pointers                  | Char name, abilities and descriptions.                                                                                                     |

### Without text

| Name        | Description                         |
| ----------- | ----------------------------------- |
| ability.bin |                                     |
| chr_b.bin   |                                     |
| chr_s.bin   |                                     |
| clear.bin   |                                     |
| exadd.bin   |                                     |
| jpower.bin  |                                     |
| koma.bin    | Used for graphics (dtx koma format) |
| kshape.bin  | Used for graphics (dtx koma format) |
| secret.bin  |                                     |
| state.bin   |                                     |

## Deckmake folder

The same format as Battle Tutorials.

| Name         | Format   |
| ------------ | -------- |
| tutorial.bin | Tutorial |

## Deck folder

There is 7 folders. Some of them includes regular files (3 digit numbers) and
some of them includes also p-files (a p and then the 3 digit numbers).

| Name         | Format |
| ------------ | ------ |
| deck-jadv    | Deck   |
| deck-jard    | Deck   |
| deck-jard_P  | PDeck  |
| deck-jarg    | Deck   |
| deck-jarg_P  | PDeck  |
| deck-play    | Deck   |
| deck-priv    | Deck   |
| deck-simpl   | Deck   |
| deck-simpl_P | PDeck  |
| deck-test    | Deck   |

### Deck

| Offset | Type         | Description                      |
| ------ | ------------ | -------------------------------- |
| 0x00   | 64 bytes     | Header                           |
| 0x40   | string       | Name                             |
| ...    | zero padding | Until it reaches 92 bytes length |

> Note: simpl 004.bin is empty.

### PDeck

| Offset | Type         | Description                      |
| ------ | ------------ | -------------------------------- |
| 0x00   | 20 bytes     | Header                           |
| 0x14   | string       | Description                      |
| ...    | zero padding | Until 0x34                       |
| 0x34   | int          | Unknown                          |
| ...    | zero padding | Until it reaches 64 bytes length |

## InfoDeck folder

Deck are the explanation of the komas in the Gallery menu (9 visible lines and 1 invisible = 10 lines per page). Each line is 40 character long.
 
Info are the helper dialogs. Just simple strings.

Starting offset + absolute pointers + just strings.

| Name            | Format       |
| --------------- | ------------ |
| bin-deck-bb.bin | InfoDeckDeck |
| bin-deck-bc.bin | InfoDeckDeck |
| bin-deck-bl.bin | InfoDeckDeck |
| bin-deck-bu.bin | InfoDeckDeck |
| bin-deck-cb.bin | InfoDeckDeck |
| bin-deck-ct.bin | InfoDeckDeck |
| bin-deck-db.bin | InfoDeckDeck |
| bin-deck-dg.bin | InfoDeckDeck |
| bin-deck-dn.bin | InfoDeckDeck |
| bin-deck-ds.bin | InfoDeckDeck |
| bin-deck-dt.bin | InfoDeckDeck |
| bin-deck-es.bin | InfoDeckDeck |
| bin-deck-gt.bin | InfoDeckDeck |
| bin-deck-hh.bin | InfoDeckDeck |
| bin-deck-hk.bin | InfoDeckDeck |
| bin-deck-hs.bin | InfoDeckDeck |
| bin-deck-ig.bin | InfoDeckDeck |
| bin-deck-is.bin | InfoDeckDeck |
| bin-deck-jj.bin | InfoDeckDeck |
| bin-deck-kk.bin | InfoDeckDeck |
| bin-deck-kn.bin | InfoDeckDeck |
| bin-deck-mo.bin | InfoDeckDeck |
| bin-deck-mr.bin | InfoDeckDeck |
| bin-deck-na.bin | InfoDeckDeck |
| bin-deck-nb.bin | InfoDeckDeck |
| bin-deck-nk.bin | InfoDeckDeck |
| bin-deck-nn.bin | InfoDeckDeck |
| bin-deck-oj.bin | InfoDeckDeck |
| bin-deck-op.bin | InfoDeckDeck |
| bin-deck-pj.bin | InfoDeckDeck |
| bin-deck-rb.bin | InfoDeckDeck |
| bin-deck-rk.bin | InfoDeckDeck |
| bin-deck-sd.bin | InfoDeckDeck |
| bin-deck-sk.bin | InfoDeckDeck |
| bin-deck-ss.bin | InfoDeckDeck |
| bin-deck-tc.bin | InfoDeckDeck |
| bin-deck-tl.bin | InfoDeckDeck |
| bin-deck-to.bin | InfoDeckDeck |
| bin-deck-tr.bin | InfoDeckDeck |
| bin-deck-tz.bin | InfoDeckDeck |
| bin-deck-yh.bin | InfoDeckDeck |
| bin-deck-yo.bin | InfoDeckDeck |
| bin-info-bb.bin | InfoDeckInfo |
| bin-info-bc.bin | InfoDeckInfo |
| bin-info-bl.bin | InfoDeckInfo |
| bin-info-bu.bin | InfoDeckInfo |
| bin-info-cb.bin | InfoDeckInfo |
| bin-info-ct.bin | InfoDeckInfo |
| bin-info-db.bin | InfoDeckInfo |
| bin-info-dg.bin | InfoDeckInfo |
| bin-info-dn.bin | InfoDeckInfo |
| bin-info-ds.bin | InfoDeckInfo |
| bin-info-dt.bin | InfoDeckInfo |
| bin-info-es.bin | InfoDeckInfo |
| bin-info-gt.bin | InfoDeckInfo |
| bin-info-hh.bin | InfoDeckInfo |
| bin-info-hk.bin | InfoDeckInfo |
| bin-info-hs.bin | InfoDeckInfo |
| bin-info-ig.bin | InfoDeckInfo |
| bin-info-is.bin | InfoDeckInfo |
| bin-info-jj.bin | InfoDeckInfo |
| bin-info-kk.bin | InfoDeckInfo |
| bin-info-kn.bin | InfoDeckInfo |
| bin-info-mo.bin | InfoDeckInfo |
| bin-info-mr.bin | InfoDeckInfo |
| bin-info-na.bin | InfoDeckInfo |
| bin-info-nb.bin | InfoDeckInfo |
| bin-info-nk.bin | InfoDeckInfo |
| bin-info-nn.bin | InfoDeckInfo |
| bin-info-oj.bin | InfoDeckInfo |
| bin-info-op.bin | InfoDeckInfo |
| bin-info-pj.bin | InfoDeckInfo |
| bin-info-rb.bin | InfoDeckInfo |
| bin-info-rk.bin | InfoDeckInfo |
| bin-info-sd.bin | InfoDeckInfo |
| bin-info-sk.bin | InfoDeckInfo |
| bin-info-ss.bin | InfoDeckInfo |
| bin-info-tc.bin | InfoDeckInfo |
| bin-info-tl.bin | InfoDeckInfo |
| bin-info-to.bin | InfoDeckInfo |
| bin-info-tr.bin | InfoDeckInfo |
| bin-info-tz.bin | InfoDeckInfo |
| bin-info-yh.bin | InfoDeckInfo |
| bin-info-yo.bin | InfoDeckInfo |

### Deck

It's a regular file with just indirect pointers and strings.

### Info

| Offset | Type         | Description                      |
| ------ | ------------ | -------------------------------- |
| 0x00   | 64 bytes     | Header                           |
| 0x40   | string       | Name                             |
| ...    | zero padding | Until it reaches 92 bytes length |

## jGalaxy folder

Here we have two formats, even thought they are pretty similar.

| Name                | Format         | Description                                                        |
| ------------------- | -------------- | ------------------------------------------------------------------ |
| jgalaxy-battle.bin  | JGalaxySimple  | Number of entries + entries                                        |
| jgalaxy-jgalaxy.bin | JGalaxyComplex | Number of entries per block (4) + Pointers of the blocks + entries |
| jgalaxy-mission.bin | JGalaxySimple  | Number of entries + entries                                        |

### JGalaxySimple

This format is pretty simple.

| Offset | Type      | Description        |
| ------ | --------- | ------------------ |
| 0x00   | int       | Number of entries. |
| 0x04   | byte[164] | Entry (164 bytes)  |

So the size of the file is 4 + (164 \* number_of_entries).

Each entry has the text (description), then a lot of 00s and at the end an unknown number of bytes (unknown region).

### JGalaxyComplex

This is also simple but there is a twist that you are not expecting: the blocks
are not entirely ordered. But don't worry it's just one file.

There are four blocks in this file. Each block is a JGalaxySimple Entry. We
share that.

The block 3 is the last block. And the block 4 is before the 3 block: Block 1 -
Block 2 - Block 4 - Block 3.

The block 4 doesn't have text.

| Offset | Type                    | Description                                               |
| ------ | ----------------------- | --------------------------------------------------------- |
| 0x00   | short[4]                | Array of number of entries of each block (4 blocks).      |
| 0x08   | int[4]                  | Array of pointers of the start of the text of each block. |
| 0x18   | byte[size_of_the_block] | String + null bytes + weird stuff                         |

**How to calculate the size_of_the_block?**

Difference between the next block's pointer and the current block's pointers
divide by the number of entries.

#### Example

| Block Number | Pointer        | Number of entries | Size of the entries | Size of the block | Math                                | Notes                                                           |
| ------------ | -------------- | ----------------- | ------------------- | ----------------- | ----------------------------------- | --------------------------------------------------------------- |
| 1            | 24 (0x18)      | 5 (0x5)           | 60bytes             | 300bytes          | 24 + (5 \* 60) = 324                |                                                                 |
| 2            | 324 (0x144)    | 36 (0x24)         | 72bytes             | 2592bytes         | 324 + (36 \* 72) = 2916             |                                                                 |
| 3            | 2916 (0x0B64)  | 142 (0x8E)        | 136bytes            | 4160bytes         | 2916 + (142 \* 136) = 22228         | This pointer 64 0B comes after the next block's pointer (D4 56) |
| 4            | 22228 (0x56D4) | 65 (0x41)         | 64bytes             | 19312bytes        | 22228 + (65 \* 64) = 26388 (0x6714) | Until end of file                                               |

## jQuiz folder

Inside the jquiz.aar we have the jquiz.bin. We have the number of questions
(3006 total questions) and then the entries. Each entry is 40bytes long, here is
the example with the first entry.

| Offset | Type   | Description                     |
| ------ | ------ | ------------------------------- |
| 0x00   | int    | NumQuestions (3006)             |
| 0x04   | byte   | MangaCode                       |
| 0x05   | byte   | Unknown                         |
| 0x06   | short  | Internal number of the question |
| 0x08   | int    | Photo of the question           |
| 0x0C   | int[4] | Question pointers               |
| 0x1C   | int[4] | Answer pointers                 |

Starting Offset: 0x0001D5B4 (120244) where the text starts.

Offset of the empty string: 0x1D5B4 (120244).

### Pointers Entry 1
| Offset | Type       | Pointer             | Text Length* | Pointer Calc (Offset + Pointer) |
| ------ | ---------- | ------------------- | ------------ | ------------------------------- |
| 0x4    | MangaID    |                     | 1            |                                 |
| 0x8    | Photo      | 120236(0x01 D5 AC)  | 0            | 120244 (0x01 D5 B4)             |
| 0xC    | Question 1 | 120233 (0x01 D5 A9) | 30           | 120245 (0x01 D5 B5)             |
| 0x10   | Question 2 | 120260 (0x01 D5 C4) | 14           | 120276 (0x01 D5 D4)             |
| 0x14   | Question 3 | 120224 (0x01 D5 A0) | 0            | 120244 (0x01 D5 B4)             |
| 0x18   | Question 4 | 120220 (0x01 D5 9C) | 0            | 120244 (0x01 D5 B4)             |

*No null byte.

> [!CAUTION]
> This file has two random `81 40 00`, maybe it means something in the game graphics but I don't know yet.

## Bin files without text

- dwc/utility.bin
- opening/PassMark.bin
- pattern.bin
