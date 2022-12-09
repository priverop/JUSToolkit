# Text files

In this game the texts are stored in .bin files. Usually we have pointers and
sentences, but each file is different, that's why we have a format for each type
of file.

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

Here we have the tutorials.

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

| Name          | Format                   | Description                                                 |
| ------------- | ------------------------ | ----------------------------------------------------------- |
| ability_t.bin |                          |                                                             |
| bgm.bin       | Bgm + BgmEntry           | Header (Pointer count) + Relative pointers                  |
| chr_b_t.bin   | BtlChr + BtlChrEntry     | Header (Entry count) + Relative pointers \*1 + padding      |
| chr_s_t.bin   | SuppChr + SuppChrEntry   | Header (Entry count) + Relative pointers \*2                |
| clearlst.bin  | SimpleBin                | Relative pointers                                           |
| commwin.bin   |                          |                                                             |
| demo.bin      | Demo + DemoEntry         | Header (Pointer count) + Relative pointers + padding        |
| infoname.bin  | SimpleBin                | Relative pointers                                           |
| komatxt.bin   |                          |                                                             |
| location.bin  | Location + LocationEntry | Header (Pointer count) + Relative pointers                  |
| piece.bin     | Piece + PieceEntry       | Header (Pointer count) + Relative pointers \*3              |
| pname.bin     | Pname (only strings)     | Header (Pointer count) + Relative pointers                  |
| rulemess.bin  |                          |                                                             |
| stage.bin     | Stage + StageEntry       | Header (Entry count) + Relative pointers + unknown pointers |
| title.bin     | SimpleBin                | Relative pointers                                           |

- \*1 Char name, abilities and furiganas; passive name, passive furigana,
  passive descriptions, ability descriptions and interactions; unknowns.
- \*2 Char name, abilities and descriptions.
- \*3 Title, Authors, Info, Page1, Page2, unknown, Id.

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
