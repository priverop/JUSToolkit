#!/bin/bash
set -e

clear

# Check for DEBUG mode
DEBUG=false
if [ "$1" == "debug" ]; then
    DEBUG=true
    echo "DEBUG mode activated"
fi

## VARIABLES
# Absolute Path of the Tool
JUS_PATH='//JUSToolkit/src/JUS.CLI'
# Absolute path for the beta root directory. 
# In this directory we will have the Weblate git repository, a folder for the generated .bin and another folder for the Demos (DEMO_PATH).
BETA_PATH='/'
# Weblate git directory
GIT_REPO_PATH=$BETA_PATH'/jus-translation-repo'
# Directory where the demo will be saved
DEMO_PATH=$BETA_PATH'/demos'

# Path of the English ROM
ROM_PATH=$JUS_PATH/bin/Debug/net8.0/jump_en.nds

# Don't modify:
TEXT_DIRECTORY=$GIT_REPO_PATH'/es'
IMAGES_DIRECTORY=$GIT_REPO_PATH'/images'
FONTS_DIRECTORY=$GIT_REPO_PATH'/fonts'
if [ "$DEBUG" == false ]; then
    DEMO_NAME=$(date +"%Y%m%d%H%M%S")'_demo.nds'
else
    DEMO_NAME='new_game_font.nds'
fi

echo '==='
echo $DEMO_NAME
## Deleting temporary demo files:
rm -f $DEMO_PATH/new_game.nds
rm -f $DEMO_PATH/new_game_font.nds

## Updating repository
# cd $GIT_REPO_PATH
# git pull
# Exit if issues with the git pull:
if [ $? -ne 0 ]; then
    echo "Error: There was an issue with the git pull."
    exit 1
fi

# Compile the project
cd $JUS_PATH
if [ "$DEBUG" == false ]; then
    dotnet build
fi
cd $JUS_PATH/bin/Debug/net8.0

# Start the process:

echo 'Creating '$DEMO_NAME

echo 'Importing 7 text formats:'

## TUTORIAL
echo ''
echo '1 - TUTORIAL'
# Insertar a BIN
./JUS.CLI jus texts batchImport --directory $TEXT_DIRECTORY/tutorial --output $BETA_PATH/new_rom/tutorial_bin
# Insertar a la ROM
./JUS.CLI jus game import --game $ROM_PATH --input $BETA_PATH/new_rom/tutorial_bin --output $DEMO_PATH

## UNICOS
echo ''
echo '2 - UNICOS'
# Insertar a BIN
./JUS.CLI jus texts batchImport --directory $TEXT_DIRECTORY/unicos --output $BETA_PATH/new_rom/bin_bin
# Insertar a la ROM
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/bin_bin --output $DEMO_PATH

## JGALAXY
echo ''
echo '3 - JGALAXY'
# Insertar a BIN
./JUS.CLI jus texts batchImport --directory $TEXT_DIRECTORY/jgalaxy --output $BETA_PATH/new_rom/jgalaxy_bin
# Insertar a la ROM
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/jgalaxy_bin --output $DEMO_PATH

## INFODECK (Deck)
echo ''
echo '4 - INFODECK-DECK:'
# Insertar a BIN
./JUS.CLI jus texts batchImport --directory $TEXT_DIRECTORY/InfoDeck --output $BETA_PATH/new_rom/InfoDeck_bin
# Insertar a la ROM
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/InfoDeck_bin --output $DEMO_PATH

## INFODECK (INFO)
echo ''
echo '5 - INFODECK-INFO:'
# Insertar a BIN
./JUS.CLI jus texts batchImport --directory $TEXT_DIRECTORY/InfoDeck-Info --output $BETA_PATH/new_rom/InfoDeckInfo_bin
# Insertar a la ROM
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/InfoDeckInfo_bin --output $DEMO_PATH

## DECK
echo ''
echo '6 - DECK'
# Insertar a BIN
./JUS.CLI jus texts deckImport --po $TEXT_DIRECTORY/deck/deck-jadv.po --output $BETA_PATH/new_rom/deck_bin/jadv/
./JUS.CLI jus texts deckImport --po $TEXT_DIRECTORY/deck/deck-jard.po --output $BETA_PATH/new_rom/deck_bin/jard/
./JUS.CLI jus texts deckImport --po $TEXT_DIRECTORY/deck/deck-jard_p.po --pdeck --output $BETA_PATH/new_rom/deck_bin/jard_p/
./JUS.CLI jus texts deckImport --po $TEXT_DIRECTORY/deck/deck-jarg.po --output $BETA_PATH/new_rom/deck_bin/jarg/
./JUS.CLI jus texts deckImport --po $TEXT_DIRECTORY/deck/deck-jarg_p.po --pdeck --output $BETA_PATH/new_rom/deck_bin/jarg_p/
./JUS.CLI jus texts deckImport --po $TEXT_DIRECTORY/deck/deck-play.po --output $BETA_PATH/new_rom/deck_bin/play/
./JUS.CLI jus texts deckImport --po $TEXT_DIRECTORY/deck/deck-priv.po --output $BETA_PATH/new_rom/deck_bin/priv/
./JUS.CLI jus texts deckImport --po $TEXT_DIRECTORY/deck/deck-smpl.po --output $BETA_PATH/new_rom/deck_bin/smpl/
./JUS.CLI jus texts deckImport --po $TEXT_DIRECTORY/deck/deck-smpl_p.po --pdeck --output $BETA_PATH/new_rom/deck_bin/smpl_p/
./JUS.CLI jus texts deckImport --po $TEXT_DIRECTORY/deck/deck-test.po --output $BETA_PATH/new_rom/deck_bin/test/
# Importar al juego
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/jadv --output $DEMO_PATH
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/jard --output $DEMO_PATH
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/jarg --output $DEMO_PATH
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/play --output $DEMO_PATH
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/priv --output $DEMO_PATH
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/smpl --output $DEMO_PATH
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/test --output $DEMO_PATH

## JQuiz
echo ''
echo '7 - JQuiz'
# Insertar a BIN
./JUS.CLI jus texts importjquiz --container $TEXT_DIRECTORY/jquiz/ --output $BETA_PATH/new_rom/jquiz_bin/
# Importar al juego
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/jquiz_bin/ --output $DEMO_PATH

## Font
echo 'Modifying font...'
./JUS.CLI jus game importFont --game $DEMO_PATH/new_game.nds --font $FONTS_DIRECTORY/jskfont_esp.aft --output $DEMO_PATH

## Renaming Demo
if [ "$DEBUG" == false ]; then
    mv $DEMO_PATH/new_game_font.nds "$DEMO_PATH/$DEMO_NAME"
fi

echo 'Finished!'
echo "Created $DEMO_PATH/$DEMO_NAME"

# Abrimos el juego
# MacOS: /Applications/melonDS.app/Contents/MacOS/melonDS $GIT_REPO_PATH/new_game_font.nds
melonDS.AppImage "$DEMO_PATH/$DEMO_NAME"
