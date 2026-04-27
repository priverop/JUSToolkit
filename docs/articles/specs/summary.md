# Game format specifications

This section list the game assets and their format. You can find more
information about the specification in each sub-page.

## Containers

Most of the assets are packed with the [`ALAR` container format](./alar.md).

Some individual files are furthermore compressed with the `DSCP` format.

## Texts

The game doesn't have a standard format to store text. It's usually included in
data structures, stored in files with `.bin` extension. Typically, we have
pointers and sentences, but each file is different, that's why we have a format
for each type of file. The [text specification](./texts.md) have more details.

- `battle/tutorial*.bin`: tutorials.
- `bin/ability_t.bin`: helper character abilities.
- `bin/bgm.bin`: background music titles.
- `bin/chr_b_t.bin`: battle character abilities.
- `bin/chr_s_t.bin`: support character abilities.
- `bin/clearlst.bin`: stage goals.
- `bin/commwin.bin`: online communication messages.
- `bin/demo.bin`: world names in demo player menu (database).
- `bin/InfoDeck.aar/*`: menu helper explanations.
- `bin/infoname.bin`: menu helper names.
- `bin/komatxt.bin`: koma names.
- `bin/location.bin`: player locations.
- `bin/piece.bin`: manga synopsis, author and info.
- `bin/pname.bin`: player name titles.
- `bin/rulemess.bin` stage rules.
- `bin/stage.bin` stage names.
- `bin/title.bin` manga titles.
- `deck/Deck.aar/*/*`: pre-created deck names.
- `deckmake/tutorial.bin`: deck making tutorial.
- `ending/ending.aar/ending/StaffRoll.bin`: TBD.
- `ending/ending.aar/ending/TitleOrder.bin`: TBD.
- `jgalaxy/jgalaxy.aar/jgalaxy/battle.bin`: JGalaxy battle types.
- `jgalaxy/jgalaxy.aar/jgalaxy/jgalaxy.bin`: JGalaxy world and galaxy names.
- `jgalaxy/jgalaxy.aar/jgalaxy/mission.bin`: JGalaxy mission names.
- `jquiz/jquiz_pack.aar/jquiz/jquiz.bin`: quiz questions

## Fonts

The fonts are in the `font` folder with `ALFT` format.
.
- `font/DSFont.aft`.
- `font/js8font.aft`.
- `font/jskfont_q.aft`.
- `font/jskfont.aft`

## Graphics

The main formats for images are:

- [`DSTX`](./dtx.md): textures
 . - `ALMT`: tile map.
- `DSIG`: indexed image.
 . - `ALTM`: tile map.
 . - `ALOD`: unknown.
- `NCCL`: palette.

Additionally, _komas_ have their own format documented in the
[koma specification](./koma.md).

## Sounds

- `sound/JS2_sound.sdat`: standard Nitro `SDAT` container and sound formats.

## Videos

The video codec is `VXDS` from Mobiclip. This codec was a previous version to
`MODS`. There are no known documentation or tools for this format.

- `opening/opening.vx`

## Data structures

These files have different data structures to support different game features.
.
- `bin/ability.bin`.
- `bin/chr_b.bin`.
- `bin/chr_s.bin`.
- `bin/clear.bin`.
- `bin/exadd.bin`.
- `bin/jpower.bin`.
- `bin/secret.bin`.
- `bin/state.bin`.
- `ChildRom/`: download-play transferable ROM and icon.
- `chr/ChrBin.aar/chr/ai/ai_param.bin`.
- `chr/ChrBin.aar/chr/ai/*` (`AIPM`).
- `chr/ChrBin.aar/chr/ai/move/*` (`AIMV`).
- `chr/ChrBin.aar/chr/col/*`.
- `chr/ChrBin.aar/chr/effect/*`.
- `chr/ChrBin.aar/chr/shot/*`.
- `demo/demo.aar/demo/*.mdf`.
- `dwc/utility.bin`: assets for the online communication.
- `item/item.aar/item/itemprob.ipf`.
- `jgalaxy/jgalaxy.aar/jgalaxy/*.bin` (except the one in text).
- `opening/opening.aar/opening/star.bin`.
- `opening/PassMark.bin`.
- `opening/pattern.bin`.
- `stage/stage.aar/stage/*.bin`.
- `stage/stage.aar/stage/*.cam`.
- `stage/stage.aar/stage/*.mob`.
- `stage/stage.aar/stage/SuddenDeath.bin`.
- `title/title.aar/title/*.bin`.
- `topmenu/topmenu.aar/topmenu/*.bin`.
