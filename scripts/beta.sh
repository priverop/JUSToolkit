#!/bin/bash
# Translate this into English:
# Script to generate a demo with the translated data from the Git repo.
# Usage: ./script.sh [debug]
#   - debug: Debug mode (avoid compiling the project, git pull and the rom name doesn't change).
set -eu

clear

# ----------------------------
# DEBUG mode
# ----------------------------
DEBUG=false
if [ "${1:-}" == "debug" ]; then
    DEBUG=true
    echo "DEBUG mode activated"
fi

# ---------
# VARIABLES
# ---------
# Absolute path for the beta root directory. 
# In this directory we will have the Weblate git repository, a folder for the generated .bin and another folder for the Demos (DEMO_PATH).
BETA_PATH='/' # -------------------> Modify this

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BASE_DIR="$(dirname "$SCRIPT_DIR")"

# Absolute Path of the JUSToolKit
JUS_PATH="$BASE_DIR/src/JUS.CLI"
# Absolute path for the beta root directory. 
# In this directory we will have the Weblate git repository, a folder for the generated .bin and another folder for the Demos (DEMO_PATH).
BETA_PATH='/' # -------------------------> MODIFY THIS ONE
# Weblate git directory
GIT_REPO_PATH=$BETA_PATH'/jus-translation-repo'
# Directory where the demo will be saved
DEMO_PATH=$BETA_PATH'/demos'

# Emulator path
EMULATOR_PATH=melonDS.AppImage # MacOS: /Applications/melonDS.app/Contents/MacOS/melonDS
# Path of the English ROM
ROM_PATH=$JUS_PATH/bin/Debug/net10.0/jump_jp.nds

# Don't modify:
TEXT_DIRECTORY=$GIT_REPO_PATH'/es'
IMAGES_DIRECTORY=$GIT_REPO_PATH'/images'
FONTS_DIRECTORY=$GIT_REPO_PATH'/fonts'
if [ "$DEBUG" == false ]; then
    DEMO_NAME=$(date +"%Y%m%d%H%M%S")'_demo.nds'
else
    DEMO_NAME='new_game_font.nds'
fi

# ----------------------------
# Setting up
# ----------------------------

## Deleting temporary demo files:
find $DEMO_PATH -name "new_game*.nds" -exec rm -f {} \;

# Create the directory for the bin files
mkdir -p "$BETA_PATH/new_rom" 
# Backup the rom
cp "$ROM_PATH" "$DEMO_PATH/new_game.nds"

## Updating repository
if [ "$DEBUG" == false ]; then
    cd $GIT_REPO_PATH
    git pull
    # Exit if issues with the git pull:
    if [ $? -ne 0 ]; then
        echo "Error: There was an issue with the git pull."
        exit 1
    fi
fi

# Compile the project
cd $JUS_PATH
if [ "$DEBUG" == false ]; then
    if ! dotnet build; then
        echo "Error: dotnet build failed."
        exit 1
    fi
fi
cd $JUS_PATH/bin/Debug/net10.0

# ----------------------------
# FUNCTIONS
# ----------------------------
import_texts_to_game() {
    local text_format=$1
    local output_bin=$2

    echo "Importing $text_format..."
    ./JUS.CLI jus texts batch-import --directory "$TEXT_DIRECTORY/$text_format" --output "$BETA_PATH/new_rom/$output_bin"
    echo ""
    ./JUS.CLI jus game import --game "$DEMO_PATH/new_game.nds" --input "$BETA_PATH/new_rom/$output_bin" --output "$DEMO_PATH"
}

# ----------------------------
# START the process
# ----------------------------
echo -e '\n\033[1;34m🚀 Starting the importing process...\033[0m'
echo -e '\n\033[1;34m📄 Importing texts\033[0m'

## TEXTS

# TUTORIAL
echo ''
echo '1 - TUTORIAL'
import_texts_to_game "tutorial" "tutorial_bin"

# UNICOS
echo ''
echo '2 - UNICOS'
import_texts_to_game "unicos" "bin_bin"

# JGALAXY
echo ''
echo '3 - JGALAXY'
import_texts_to_game "jgalaxy" "jgalaxy_bin"

# INFODECK (Deck)
echo ''
echo '4 - INFODECK-DECK:'
import_texts_to_game "InfoDeck" "InfoDeck_bin"

# INFODECK (INFO)
echo ''
echo '5 - INFODECK-INFO (solo jump-en):'
./JUS.CLI jus texts import --po "$TEXT_DIRECTORY/InfoDeck-Info/bin-info-jump-en.bin.po" --output "$BETA_PATH/new_rom/InfoDeckInfo_bin"
mv "$BETA_PATH/new_rom/InfoDeckInfo_bin/bin-info-jump-en.bin" "$BETA_PATH/new_rom/InfoDeckInfo_bin/bin-info-jump.bin"
./JUS.CLI jus game import --game "$DEMO_PATH/new_game.nds" --input "$BETA_PATH/new_rom/InfoDeckInfo_bin" --output "$DEMO_PATH"

# # DECK texts
echo ''
echo '6 - DECK'
# # Insertar a BIN
./JUS.CLI jus texts import-deck --po $TEXT_DIRECTORY/deck/deck-jadv.po --output $BETA_PATH/new_rom/deck_bin/jadv/
./JUS.CLI jus texts import-deck --po $TEXT_DIRECTORY/deck/deck-jard.po --output $BETA_PATH/new_rom/deck_bin/jard/
./JUS.CLI jus texts import-deck --po $TEXT_DIRECTORY/deck/deck-jard_p.po --pdeck --output $BETA_PATH/new_rom/deck_bin/jard_p/
./JUS.CLI jus texts import-deck --po $TEXT_DIRECTORY/deck/deck-jarg.po --output $BETA_PATH/new_rom/deck_bin/jarg/
./JUS.CLI jus texts import-deck --po $TEXT_DIRECTORY/deck/deck-jarg_p.po --pdeck --output $BETA_PATH/new_rom/deck_bin/jarg_p/
./JUS.CLI jus texts import-deck --po $TEXT_DIRECTORY/deck/deck-play.po --output $BETA_PATH/new_rom/deck_bin/play/
./JUS.CLI jus texts import-deck --po $TEXT_DIRECTORY/deck/deck-priv.po --output $BETA_PATH/new_rom/deck_bin/priv/
./JUS.CLI jus texts import-deck --po $TEXT_DIRECTORY/deck/deck-smpl.po --output $BETA_PATH/new_rom/deck_bin/smpl/
./JUS.CLI jus texts import-deck --po $TEXT_DIRECTORY/deck/deck-smpl_p.po --pdeck --output $BETA_PATH/new_rom/deck_bin/smpl_p/
./JUS.CLI jus texts import-deck --po $TEXT_DIRECTORY/deck/deck-test.po --output $BETA_PATH/new_rom/deck_bin/test/
# # Importar al juego
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/jadv --output $DEMO_PATH
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/jard --output $DEMO_PATH
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/jarg --output $DEMO_PATH
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/play --output $DEMO_PATH
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/priv --output $DEMO_PATH
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/smpl --output $DEMO_PATH
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/deck_bin/test --output $DEMO_PATH

# JQUIZ
echo ''
echo '7 - JQuiz'
# Insertar a BIN
./JUS.CLI jus texts import-jquiz --container $TEXT_DIRECTORY/jquiz/ --output $BETA_PATH/new_rom/jquiz_bin/
# Importar al juego
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $BETA_PATH/new_rom/jquiz_bin/ --output $DEMO_PATH

#--------------------------
## IMAGES
echo -e '\n\033[1;34m🖼  Importing images\033[0m'

# MENUS
echo ''
echo 'MENUS'
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $IMAGES_DIRECTORY/menus --output $DEMO_PATH

# COMICS
echo ''
echo 'COMICS'
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $IMAGES_DIRECTORY/comics --output $DEMO_PATH

# SPRITES
echo ''
echo 'SPRITES'
./JUS.CLI jus game import --game $DEMO_PATH/new_game.nds --input $IMAGES_DIRECTORY/sprites --output $DEMO_PATH

#--------------------------
## Updating Font
echo ''
echo 'Modifying font...'
./JUS.CLI jus game import-font --game $DEMO_PATH/new_game.nds --font $FONTS_DIRECTORY/jskfont_esp.aft --output $DEMO_PATH

# ----------------------------
# Finishing...
# ----------------------------
## Renaming Demo
if [ "$DEBUG" == false ]; then
    mv $DEMO_PATH/new_game_font.nds "$DEMO_PATH/$DEMO_NAME"
fi

echo -e '\n\033[1;32m✅ Finished! File created at \033[0m'$DEMO_PATH/$DEMO_NAME
printf -- "------------------------------\n\n"

# Opening the new game
$EMULATOR_PATH "$DEMO_PATH/$DEMO_NAME"
