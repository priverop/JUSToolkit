#!/bin/bash
set -e

clear

JUS_PATH='//JUSToolkit/src/JUS.CLI'
BETA_PATH='//Romhacking/beta_textos'
ROM_PATH=$JUS_PATH/bin/Debug/net8.0/jump_en.nds

# rm -f $BETA_PATH/new_game.nds
# rm -f $BETA_PATH/new_game_font.nds

cd $JUS_PATH
# dotnet build
cd $JUS_PATH/bin/Debug/net8.0

## TUTORIAL
# Insertar a BIN
./JUS.CLI jus texts batchImport --directory $BETA_PATH/new_weblate/tutorial --output $BETA_PATH/new_rom/tutorial_bin
# Insertar a la ROM
./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/tutorial_bin --output $BETA_PATH

## UNICOS
# Insertar a BIN
./JUS.CLI jus texts batchImport --directory $BETA_PATH/new_weblate/unicos --output $BETA_PATH/new_rom/bin_bin
# Insertar a la ROM
./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/bin_bin --output $BETA_PATH

## JGALAXY
# Insertar a BIN
./JUS.CLI jus texts batchImport --directory $BETA_PATH/new_weblate/jgalaxy --output $BETA_PATH/new_rom/jgalaxy_bin
# Insertar a la ROM
./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/jgalaxy_bin --output $BETA_PATH

## INFODECK (Deck)
# Insertar a BIN
./JUS.CLI jus texts batchImport --directory $BETA_PATH/new_weblate/InfoDeck --output $BETA_PATH/new_rom/InfoDeck_bin
# Insertar a la ROM
./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/InfoDeck_bin --output $BETA_PATH

## DECK
# Insertar a BIN
./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/deck/deck-jadv.po --output $BETA_PATH/new_rom/deck_bin/jadv/
./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/deck/deck-jard.po --output $BETA_PATH/new_rom/deck_bin/jard/
./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/deck/deck-jard_p.po --pdeck --output $BETA_PATH/new_rom/deck_bin/jard_p/
./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/deck/deck-jarg.po --output $BETA_PATH/new_rom/deck_bin/jarg/
./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/deck/deck-jarg_p.po --pdeck --output $BETA_PATH/new_rom/deck_bin/jarg_p/
./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/deck/deck-play.po --output $BETA_PATH/new_rom/deck_bin/play/
./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/deck/deck-priv.po --output $BETA_PATH/new_rom/deck_bin/priv/
./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/deck/deck-smpl.po --output $BETA_PATH/new_rom/deck_bin/smpl/
./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/deck/deck-smpl_p.po --pdeck --output $BETA_PATH/new_rom/deck_bin/smpl_p/
./JUS.CLI jus texts deckImport --po $BETA_PATH/new_weblate/deck/deck-test.po --output $BETA_PATH/new_rom/deck_bin/test/
# Importar al juego
./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/jadv --output $BETA_PATH
./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/jard --output $BETA_PATH
./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/jarg --output $BETA_PATH
./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/play --output $BETA_PATH
./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/priv --output $BETA_PATH
./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/smpl --output $BETA_PATH
./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/test --output $BETA_PATH

## JQuiz
# Insertar a BIN
./JUS.CLI jus texts importjquiz --container $BETA_PATH/new_weblate/jquiz/ --output $BETA_PATH/new_rom/jquiz_bin/
# Importar al juego
./JUS.CLI jus game import --game $BETA_PATH/new_game.nds --input $BETA_PATH/new_rom/jquiz_bin/ --output $BETA_PATH

## Font
./JUS.CLI jus game importFont --game $BETA_PATH/new_game.nds --font $BETA_PATH/new_rom/font/jskfont_esp.aft --output $BETA_PATH

# Abrimos el juego
/Applications/melonDS.app/Contents/MacOS/melonDS $BETA_PATH/new_game_font.nds

