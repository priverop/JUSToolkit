# Text files

In this game the texts are stored in .bin files. Usually we have pointers and
sentences, but each file is different, that's why we have a format for each type
of file.

## Relative pointers

Relative pointers means that the text is in the pointer offset plus the position of the pointer. If the position of the pointer is 0x04 and the value is 0x100, the text will be in 0x104.

## Utility classes

To help with the process we've developed some classes to read and write easily:

- JusText.ReadIndirectString: This method help us to read relative pointers. It
  reads 4 bytes (relative pointer), adds up our position, goes to that offset
  and read until a null byte.
- JusText.WriteStringPointer: This allows us to write relative pointers. It
  needs the JusIndirectText Class which stores the StartingOffset (where the
  pointer section ends and the text section starts), the strings to write and
  the pointers to these strings.

## Battle folder

Here we have the tutorials. They all have the same structure: StartingOffset, a lot of random unknown ints and the pointers. These pointers just store the size of the string starting from 0. For example if the first two strings are 2bytes and 4bytes long, the pointers will be 02 and then 06.

| Name          | Format | Description |
| ------------- | ------ | ----------- |
| tutorial0.bin |        |             |
| tutorial1.bin |        |             |
| tutorial2.bin |        |             |
| tutorial3.bin |        |             |
| tutorial4.bin |        |             |
| tutorial5.bin |        |             |

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

The Format class has the main properties of the file and a list of Entries. Each Entry has different parameters, which are listed here.

Pointer count is the total number of entries. Entry size is the size of the total entry in the pointer section. The starting offset means that the pointer section ends and the text section starts.

| Format                   | Format Description                                   | Entry Description                                                                                                                          |
| ------------------------ | ---------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------ |
| Ability + AbilityEntry   | Starting offset + Relative pointers                  | Title and Description.                                                                                                                     |
| Bgm + BgmEntry           | Pointer count + Relative pointers                    | Title, Descriptions (3), Unkown 1 and 2, Icon.                                                                                             |
| BtlChr + BtlChrEntry     | Starting offset + Relative pointers + padding (0x04) | Char name, abilities and furiganas; passive name, passive furigana, passive descriptions, ability descriptions and interactions; unknowns. |
| Commwin + CommwinEntry   | Pointer count + Relative pointers                    | ?                                                                                                                                          |
| Demo + DemoEntry         | Pointer count + Relative pointers + padding (0x04)   | Title, Descriptions (3), Id and Icon.                                                                                                      |
| Komatxt + KomatxtEntry   | Starting offset + Relative pointers                  | Name and two unknowns (int).                                                                                                               |
| Location + LocationEntry | Pointer count + Relative pointers                    | Name and two unkown values (short).                                                                                                        |
| Piece + PieceEntry       | Pointer count + Relative pointers                    | Title, Authors, Info, Page1, Page2, unknown, Id.                                                                                           |
| Pname (only strings)     | Pointer count + Relative simple pointers             | Just strings.                                                                                                                              |
| Rulemess + RulemessEntry | Starting offset + Relative pointers                  | 3 Descriptions + int unknown.                                                                                                              |
| SimpleBin                | Starting offset + Relative pointers                  | Just strings.                                                                                                                              |
| Stage + StageEntry       | Starting offset + Relative pointers                  | Name and two unkown values (short).                                                                                                        |
| SuppChr + SuppChrEntry   | Starting offset + Relative pointers                  | Char name, abilities and descriptions.                                                                                                     |

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

| Name         | Format | Description |
| ------------ | ------ | ----------- |
| tutorial.bin |        |             |

## Deck folder

| Name           | Format | Description |
| -------------- | ------ | ----------- |
| deck-jadv.bin  |        |             |
| deck-jard.bin  |        |             |
| deck-jarg.bin  |        |             |
| deck-play.bin  |        |             |
| deck-priv.bin  |        |             |
| deck-simpl.bin |        |             |
| deck-test.bin  |        |             |

## InfoDeck folder

| Name            | Format | Description |
| --------------- | ------ | ----------- |
| bin-deck-bb.bin |        |             |
| bin-deck-bc.bin |        |             |
| bin-deck-bl.bin |        |             |
| bin-deck-bu.bin |        |             |
| bin-deck-cb.bin |        |             |
| bin-deck-ct.bin |        |             |
| bin-deck-db.bin |        |             |
| bin-deck-dg.bin |        |             |
| bin-deck-dn.bin |        |             |
| bin-deck-ds.bin |        |             |
| bin-deck-dt.bin |        |             |
| bin-deck-es.bin |        |             |
| bin-deck-gt.bin |        |             |
| bin-deck-hh.bin |        |             |
| bin-deck-hk.bin |        |             |
| bin-deck-hs.bin |        |             |
| bin-deck-ig.bin |        |             |
| bin-deck-is.bin |        |             |
| bin-deck-jj.bin |        |             |
| bin-deck-kk.bin |        |             |
| bin-deck-kn.bin |        |             |
| bin-deck-mo.bin |        |             |
| bin-deck-mr.bin |        |             |
| bin-deck-na.bin |        |             |
| bin-deck-nb.bin |        |             |
| bin-deck-nk.bin |        |             |
| bin-deck-nn.bin |        |             |
| bin-deck-oj.bin |        |             |
| bin-deck-op.bin |        |             |
| bin-deck-pj.bin |        |             |
| bin-deck-rb.bin |        |             |
| bin-deck-rk.bin |        |             |
| bin-deck-sd.bin |        |             |
| bin-deck-sk.bin |        |             |
| bin-deck-ss.bin |        |             |
| bin-deck-tc.bin |        |             |
| bin-deck-tl.bin |        |             |
| bin-deck-to.bin |        |             |
| bin-deck-tr.bin |        |             |
| bin-deck-tz.bin |        |             |
| bin-deck-yh.bin |        |             |
| bin-deck-yo.bin |        |             |

## jGalaxy folder

| Name                | Format | Description |
| ------------------- | ------ | ----------- |
| jgalaxy-battle.bin  |        |             |
| jgalaxy-jgalaxy.bin |        |             |
| jgalaxy-mission.bin |        |             |

## jQuiz folder

Inside the jquiz.aar we have the jquiz.bin.

| Name      | Format | Description |
| --------- | ------ | ----------- |
| jquiz.bin |        |             |

## Bin files without text

- dwc/utility.bin
- opening/PassMark.bin
- pattern.bin
