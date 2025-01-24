#!/bin/bash
set -e

clear

JUS_PATH='JUSToolkit/src/JUS.CLI'
BETA_PATH='/beta_textos'
ROM_PATH=$JUS_PATH/bin/Debug/net8.0/jump_en.nds

rm -f $BETA_PATH/new_game.nds
rm -f $BETA_PATH/new_game_font.nds

cd $JUS_PATH
# dotnet build
cd $JUS_PATH/bin/Debug/net8.0

## TUTORIAL
# PO
#./JUS.CLI jus texts batchExport --directory $BETA_PATH/en_rom/battle --output $BETA_PATH/en_rom/battle_po
# Deckmake PO
#./JUS.CLI jus texts export --bin $BETA_PATH/en_rom/deckmake/tutorial.bin --output $BETA_PATH/en_rom/deckmake_po
# Pomerge
#pomerge --from $BETA_PATH/old_weblate/tutorial/**.po --to $BETA_PATH/new_weblate/tutorial/**.po
# Insertar a BIN
#./JUS.CLI jus texts batchImport --directory $BETA_PATH/new_weblate/tutorial --output $BETA_PATH/new_rom/tutorial_bin
# Insertar a la ROM
# ./JUS.CLI jus game import --game $ROM_PATH --input $BETA_PATH/new_rom/tutorial --output $BETA_PATH

## UNICOS
# PO
# ./JUS.CLI jus texts batchExport --directory $BETA_PATH/en_rom/bin --output $BETA_PATH/en_rom/bin_po
# cp -r $BETA_PATH/en_rom/bin_po/ $BETA_PATH/new_weblate/bin/
# Pomerge
# pomerge --from $BETA_PATH/old_weblate/unicos_new/**.po --to $BETA_PATH/new_weblate/bin/**.po
# Borramos la primera linea por el bug de pomerge
# for archivo in "$BETA_PATH/new_weblate/bin"/*.po; do
#   sed -i '' '1d' "$archivo"
# done
# Insertar a BIN
# ./JUS.CLI jus texts batchImport --directory $BETA_PATH/new_weblate/bin --output $BETA_PATH/new_rom/bin_bin
# Insertar a la ROM
# ./JUS.CLI jus game import --game $ROM_PATH --input $BETA_PATH/new_rom/bin_bin --output $BETA_PATH

## JGALAXY
# PO
# ./JUS.CLI jus texts batchExport --directory $BETA_PATH/en_rom/jgalaxy --output $BETA_PATH/en_rom/jgalaxy_po
# cp -r $BETA_PATH/en_rom/jgalaxy_po/ $BETA_PATH/new_weblate/jgalaxy/
# Pomerge
# pomerge --from $BETA_PATH/old_weblate/jgalaxy/**.po --to $BETA_PATH/new_weblate/jgalaxy/**.po
# Borramos la primera linea por el bug de pomerge
# for archivo in "$BETA_PATH/new_weblate/jgalaxy"/*.po; do
#   sed -i '' '1d' "$archivo"
# done
# Insertar a BIN
# ./JUS.CLI jus texts batchImport --directory $BETA_PATH/new_weblate/jgalaxy --output $BETA_PATH/new_rom/jgalaxy_bin
# Insertar a la ROM
# ./JUS.CLI jus game import --game $ROM_PATH --input $BETA_PATH/new_rom/jgalaxy_bin --output $BETA_PATH

## INFODECK (Deck)
# RENAME
# for archivo in "$BETA_PATH/en_rom/infodeck/deck"/*.bin; do
#   nombre_archivo=$(basename "$archivo")
#   nuevo_nombre="bin-deck-$nombre_archivo"
#   mv "$archivo" "$BETA_PATH/en_rom/infodeck/deck/$nuevo_nombre"
# done
# PO
# ./JUS.CLI jus texts batchExport --directory $BETA_PATH/en_rom/infodeck/deck --output $BETA_PATH/en_rom/infodeck_deck_po
# cp -r $BETA_PATH/en_rom/infodeck_deck_po/ $BETA_PATH/new_weblate/infodeck/
# Pomerge
# pomerge --from $BETA_PATH/old_weblate/InfoDeck/**.po --to $BETA_PATH/new_weblate/InfoDeck/**.po
# Borramos la primera linea por el bug de pomerge
# for archivo in "$BETA_PATH/new_weblate/InfoDeck"/*.po; do
#   sed -i '' '1d' "$archivo"
# done
# Insertar a BIN
# ./JUS.CLI jus texts batchImport --directory $BETA_PATH/new_weblate/InfoDeck --output $BETA_PATH/new_rom/InfoDeck_bin
# Insertar a la ROM
# ./JUS.CLI jus game import --game $ROM_PATH --input $BETA_PATH/new_rom/InfoDeck_bin --output $BETA_PATH

## DECK
# PO
# ./JUS.CLI jus texts deckExport --directory $BETA_PATH/en_rom/deck/jadv --output $BETA_PATH/en_rom/deck_po
# ./JUS.CLI jus texts deckExport --directory $BETA_PATH/en_rom/deck/jard --output $BETA_PATH/en_rom/deck_po
# ./JUS.CLI jus texts deckExport --directory $BETA_PATH/en_rom/deck/jard_p --pdeck --output $BETA_PATH/en_rom/deck_po
# ./JUS.CLI jus texts deckExport --directory $BETA_PATH/en_rom/deck/jarg --output $BETA_PATH/en_rom/deck_po
# ./JUS.CLI jus texts deckExport --directory $BETA_PATH/en_rom/deck/jarg_p --pdeck --output $BETA_PATH/en_rom/deck_po
# ./JUS.CLI jus texts deckExport --directory $BETA_PATH/en_rom/deck/play --output $BETA_PATH/en_rom/deck_po
# ./JUS.CLI jus texts deckExport --directory $BETA_PATH/en_rom/deck/priv --output $BETA_PATH/en_rom/deck_po
# ./JUS.CLI jus texts deckExport --directory $BETA_PATH/en_rom/deck/smpl --output $BETA_PATH/en_rom/deck_po
# ./JUS.CLI jus texts deckExport --directory $BETA_PATH/en_rom/deck/smpl_p --pdeck --output $BETA_PATH/en_rom/deck_po
# ./JUS.CLI jus texts deckExport --directory $BETA_PATH/en_rom/deck/test --output $BETA_PATH/en_rom/deck_po
# cp -r $BETA_PATH/en_rom/deck_po/ $BETA_PATH/new_weblate/deck/
# Pomerge
# pomerge --from $BETA_PATH/old_weblate/deck/**.po --to $BETA_PATH/new_weblate/deck/**.po
# Borramos la primera linea por el bug de pomerge
# for archivo in "$BETA_PATH/new_weblate/deck"/*.po; do
#   sed -i '' '1d' "$archivo"
# done
# Insertar a BIN
# ./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/decktest/deck-jadv.po --output $BETA_PATH/new_rom/deck_bin/jadv/
# ./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/decktest/deck-jard.po --output $BETA_PATH/new_rom/deck_bin/jard/
# ./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/decktest/deck-jard_p.po --pdeck --output $BETA_PATH/new_rom/deck_bin/jard_p/
# ./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/decktest/deck-jarg.po --output $BETA_PATH/new_rom/deck_bin/jarg/
# ./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/decktest/deck-jarg_p.po --pdeck --output $BETA_PATH/new_rom/deck_bin/jarg_p/
# ./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/decktest/deck-play.po --output $BETA_PATH/new_rom/deck_bin/play/
# ./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/decktest/deck-priv.po --output $BETA_PATH/new_rom/deck_bin/priv/
# ./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/decktest/deck-smpl.po --output $BETA_PATH/new_rom/deck_bin/smpl/
# ./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/decktest/deck-smpl_p.po --pdeck --output $BETA_PATH/new_rom/deck_bin/smpl_p/
# ./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/decktest/deck-test.po --output $BETA_PATH/new_rom/deck_bin/test/
# Importar al juego
# ./JUS.CLI jus game import --game $ROM_PATH --input $BETA_PATH/new_rom/deck_bin/jadv --output $BETA_PATH
# ./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/jard --output $BETA_PATH
# ./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/jarg --output $BETA_PATH
# ./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/play --output $BETA_PATH
# ./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/priv --output $BETA_PATH
# ./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/smpl --output $BETA_PATH
# ./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/test --output $BETA_PATH

## JQuiz
# PO
# ./JUS.CLI jus texts exportJQuiz --bin $BETA_PATH/en_rom/jquiz/jquiz.bin --output $BETA_PATH/en_rom/jquiz_po
# cp -r $BETA_PATH/en_rom/jquiz_po/ $BETA_PATH/new_weblate/jquiz/
# Pomerge I didn't really need to Merge anything because they were the same. The only change was the name of the files 0 -> 00, 1 -> 01, etc.
# Insertar a BIN
# ./JUS.CLI jus texts importjquiz --container $BETA_PATH/new_weblate/jquiz/ --output $BETA_PATH/new_rom/jquiz_bin/
# Importar al juego
# ./JUS.CLI jus game import --game $ROM_PATH --input $BETA_PATH/new_rom/jquiz_bin/ --output $BETA_PATH

## Font
./JUS.CLI jus game importFont --game $BETA_PATH/new_game.nds --font $BETA_PATH/new_rom/font/jskfont_esp.aft --output $BETA_PATH

# Abrimos el juego
/Applications/melonDS.app/Contents/MacOS/melonDS $BETA_PATH/new_game_font.nds

