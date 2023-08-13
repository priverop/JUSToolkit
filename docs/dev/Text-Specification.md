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

| Name          | Format                   | Location                                            |
| ------------- | ------------------------ | --------------------------------------------------- |
| ability_t.bin | Ability + AbilityEntry   | Koma list, helper (koma size 1) ability description |
| bgm.bin       | Bgm + BgmEntry           | Battle stage Background Music (Database menu)       |
| chr_b_t.bin   | BtlChr + BtlChrEntry     | Koma abilities info                                 |
| chr_s_t.bin   | SuppChr + SuppChrEntry   | Koma abilities info support komas                   |
| clearlst.bin  | SimpleBin                | Stage goals                                         |
| commwin.bin   | Commwin + CommwinEntry   | Common window messages                              |
| demo.bin      | Demo + DemoEntry         | World names (demo player menu, database)            |
| infoname.bin  | SimpleBin                | Main menu helper names                              |
| komatxt.bin   | Komatxt + KomatxtEntry   | Koma names                                          |
| location.bin  | Location + LocationEntry | Player location                                     |
| piece.bin     | Piece + PieceEntry       | Manga author and info (database menu)               |
| pname.bin     | Pname                    | Player name titles (player settings)                |
| rulemess.bin  | Rulemess + RulemessEntry | Stage rules                                         |
| stage.bin     | Stage + StageEntry       | Stage names                                         |
| title.bin     | SimpleBin                | Manga names                                         |

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

### PDeck

| Offset | Type         | Description                      |
| ------ | ------------ | -------------------------------- |
| 0x00   | 20 bytes     | Header                           |
| 0x14   | string       | Description                      |
| ...    | zero padding | Until 0x34                       |
| 0x34   | int          | Unknown                          |
| ...    | zero padding | Until it reaches 64 bytes length |

## InfoDeck folder

Starting offset + absolute pointers + just strings

| Name            | Format   |
| --------------- | -------- |
| bin-deck-bb.bin | InfoDeck |
| bin-deck-bc.bin | InfoDeck |
| bin-deck-bl.bin | InfoDeck |
| bin-deck-bu.bin | InfoDeck |
| bin-deck-cb.bin | InfoDeck |
| bin-deck-ct.bin | InfoDeck |
| bin-deck-db.bin | InfoDeck |
| bin-deck-dg.bin | InfoDeck |
| bin-deck-dn.bin | InfoDeck |
| bin-deck-ds.bin | InfoDeck |
| bin-deck-dt.bin | InfoDeck |
| bin-deck-es.bin | InfoDeck |
| bin-deck-gt.bin | InfoDeck |
| bin-deck-hh.bin | InfoDeck |
| bin-deck-hk.bin | InfoDeck |
| bin-deck-hs.bin | InfoDeck |
| bin-deck-ig.bin | InfoDeck |
| bin-deck-is.bin | InfoDeck |
| bin-deck-jj.bin | InfoDeck |
| bin-deck-kk.bin | InfoDeck |
| bin-deck-kn.bin | InfoDeck |
| bin-deck-mo.bin | InfoDeck |
| bin-deck-mr.bin | InfoDeck |
| bin-deck-na.bin | InfoDeck |
| bin-deck-nb.bin | InfoDeck |
| bin-deck-nk.bin | InfoDeck |
| bin-deck-nn.bin | InfoDeck |
| bin-deck-oj.bin | InfoDeck |
| bin-deck-op.bin | InfoDeck |
| bin-deck-pj.bin | InfoDeck |
| bin-deck-rb.bin | InfoDeck |
| bin-deck-rk.bin | InfoDeck |
| bin-deck-sd.bin | InfoDeck |
| bin-deck-sk.bin | InfoDeck |
| bin-deck-ss.bin | InfoDeck |
| bin-deck-tc.bin | InfoDeck |
| bin-deck-tl.bin | InfoDeck |
| bin-deck-to.bin | InfoDeck |
| bin-deck-tr.bin | InfoDeck |
| bin-deck-tz.bin | InfoDeck |
| bin-deck-yh.bin | InfoDeck |
| bin-deck-yo.bin | InfoDeck |
| bin-info-bb.bin | InfoDeck |
| bin-info-bc.bin | InfoDeck |
| bin-info-bl.bin | InfoDeck |
| bin-info-bu.bin | InfoDeck |
| bin-info-cb.bin | InfoDeck |
| bin-info-ct.bin | InfoDeck |
| bin-info-db.bin | InfoDeck |
| bin-info-dg.bin | InfoDeck |
| bin-info-dn.bin | InfoDeck |
| bin-info-ds.bin | InfoDeck |
| bin-info-dt.bin | InfoDeck |
| bin-info-es.bin | InfoDeck |
| bin-info-gt.bin | InfoDeck |
| bin-info-hh.bin | InfoDeck |
| bin-info-hk.bin | InfoDeck |
| bin-info-hs.bin | InfoDeck |
| bin-info-ig.bin | InfoDeck |
| bin-info-is.bin | InfoDeck |
| bin-info-jj.bin | InfoDeck |
| bin-info-kk.bin | InfoDeck |
| bin-info-kn.bin | InfoDeck |
| bin-info-mo.bin | InfoDeck |
| bin-info-mr.bin | InfoDeck |
| bin-info-na.bin | InfoDeck |
| bin-info-nb.bin | InfoDeck |
| bin-info-nk.bin | InfoDeck |
| bin-info-nn.bin | InfoDeck |
| bin-info-oj.bin | InfoDeck |
| bin-info-op.bin | InfoDeck |
| bin-info-pj.bin | InfoDeck |
| bin-info-rb.bin | InfoDeck |
| bin-info-rk.bin | InfoDeck |
| bin-info-sd.bin | InfoDeck |
| bin-info-sk.bin | InfoDeck |
| bin-info-ss.bin | InfoDeck |
| bin-info-tc.bin | InfoDeck |
| bin-info-tl.bin | InfoDeck |
| bin-info-to.bin | InfoDeck |
| bin-info-tr.bin | InfoDeck |
| bin-info-tz.bin | InfoDeck |
| bin-info-yh.bin | InfoDeck |
| bin-info-yo.bin | InfoDeck |

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
| 0x00   | byte      | Number of entries. |
| 0x04   | byte[164] | Entry (164 bytes)  |

So the size of the file is 4 + (164 \* number_of_entries).

### JGalaxyComplex

This is also simple but there is a twist that you are not expecting: the blocks
are not entirely ordered. But don't worry it's just one file.

There are four blocks in this file. Each block is a JGalaxySimple Entry. We
share that.

The block 3 is the last block. And the block 4 is before the 3 block: Block 1 -
Block 2 - Block 4 - Block 3.

| Offset | Type                    | Description                                 |
| ------ | ----------------------- | ------------------------------------------- |
| 0x00   | short[4]                | Number of entries per block (4 blocks).     |
| 0x08   | int[4]                  | Pointer of the start of the text, per block |
| 0x18   | byte[size_of_the_block] | String + null bytes + weird stuff           |

**How to calculate the size_of_the_block?**

We have the size of each block (with the pointers) and the number of entries, so
we can divide them.

#### Example

| Block Number | Pointer        | Number of entries | Size of the entries | Math                        | Notes                                                           |
| ------------ | -------------- | ----------------- | ------------------- | --------------------------- | --------------------------------------------------------------- | ----------------- |
| 1            | 24 (0x18)      | 5 (0x5)           | 60bytes             | 24 + (5 \* 60) = 324        |                                                                 |
| 2            | 324 (0x144)    | 36 (0x24)         | 72bytes             | 324 + (36 \* 72) = 2916     |                                                                 |
| 3            | 2916 (0xB64)   | 142 (0x41)        | 136bytes            | 2916 + (142 \* 136) = 22228 | This pointer 64 0B comes after the next block's pointer (D4 56) |
| 4            | 22228 (0x56D4) | 65 (0x8E)         | 64bytes             | 22228 + (65 \* 64)          | 26388 (0x6714)                                                  | Until end of file |

## jQuiz folder

Inside the jquiz.aar we have the jquiz.bin. We have the number of questions (3006 total questions) and then the entries. Each entry is 40bytes long, here is the example with the first entry. 

| Offset | Type   | Description                     |
| ------ | ------ | ------------------------------- |
| 0x00   | int    | NumQuestions (3006)             |
| 0x04   | byte   | MangaCode                       |
| 0x05   | byte   | Unknown                         |
| 0x06   | short  | Internal number of the question |
| 0x08   | int    | Photo of the question           |
| 0x0C   | int[4] | Question pointers               |
| 0x1C   | int[4] | Answer pointers                 |

## Bin files without text

- dwc/utility.bin
- opening/PassMark.bin
- pattern.bin
