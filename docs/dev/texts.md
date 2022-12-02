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
| ability.bin   |                          |                                                             |
| ability_t.bin |                          |                                                             |
| bgm.bin       | Bgm + BgmEntry           | Header (Pointer count) + Relative pointers                  |
| chr_b.bin     |                          |                                                             |
| chr_b_t.bin   | BtlChr + BtlChrEntry     |                                                             |
| chr_s.bin     |                          |                                                             |
| chr_s_t.bin   | SuppChr + SuppChrEntry   |                                                             |
| clear.bin     |                          |                                                             |
| clearlst.bin  |                          |                                                             |
| commwin.bin   |                          |                                                             |
| demo.bin      | Demo + DemoEntry         | Header (Pointer count) + Relative pointers + padding        |
| exadd.bin     |                          |                                                             |
| infoname.bin  |                          |                                                             |
| jpower.bin    |                          |                                                             |
| komatxt.bin   |                          |                                                             |
| location.bin  | Location + LocationEntry | Header (Pointer count) + Relative pointers                  |
| piece.bin     | Piece + PieceEntry       |                                                             |
| pname.bin     | Pname (only strings)     | Header (Pointer count) + Relative pointers                  |
| rulemess.bin  |                          |                                                             |
| secret.bin    |                          |                                                             |
| stage.bin     | Stage + StageEntry       | Header (Entry count) + Relative pointers + unknown pointers |
| state.bin     |                          |                                                             |
| title.bin     |                          |                                                             |

### Without text

| Name       | Description                         |
| ---------- | ----------------------------------- |
| koma.bin   | Used for graphics (dtx koma format) |
| kshape.bin | Used for graphics (dtx koma format) |

## Deckmake folder

| Name         | Format | Description |
| ------------ | ------ | ----------- |
| tutorial.bin |        |             |

## jQuiz folder

Inside the jquiz.aar we have the jquiz.bin.

| Name      | Format | Description |
| --------- | ------ | ----------- |
| jquiz.bin |        |             |

## Bin files without text

- dwc/utility.bin
- opening/PassMark.bin
- pattern.bin
