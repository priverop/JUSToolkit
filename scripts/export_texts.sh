#/bin/bash

cd "/Users//Dev/JUSToolkit/src/JUS.CLI"
dotnet build
cd "./bin/Debug/net6.0/"

# ./JUS.CLI jus texts export --bin 04-textos/jp/ability_t.bin --output 04-textos/jp/po
./JUS.CLI jus texts export --bin 04-textos/jp/bgm.bin --output 04-textos/jp/po
./JUS.CLI jus texts export --bin 04-textos/jp/clearlst.bin --output 04-textos/jp/po
# ./JUS.CLI jus texts export --bin 04-textos/jp/commwin.bin --output 04-textos/jp/po
./JUS.CLI jus texts export --bin 04-textos/jp/chr_b_t.bin --output 04-textos/jp/po
./JUS.CLI jus texts export --bin 04-textos/jp/chr_s_t.bin --output 04-textos/jp/po
./JUS.CLI jus texts export --bin 04-textos/jp/demo.bin --output 04-textos/jp/po
./JUS.CLI jus texts export --bin 04-textos/jp/infoname.bin --output 04-textos/jp/po
# ./JUS.CLI jus texts export --bin 04-textos/jp/komatxt.bin --output 04-textos/jp/po
./JUS.CLI jus texts export --bin 04-textos/jp/location.bin --output 04-textos/jp/po
./JUS.CLI jus texts export --bin 04-textos/jp/piece.bin --output 04-textos/jp/po
./JUS.CLI jus texts export --bin 04-textos/jp/pname.bin --output 04-textos/jp/po
# ./JUS.CLI jus texts export --bin 04-textos/jp/rulemess.bin --output 04-textos/jp/po
./JUS.CLI jus texts export --bin 04-textos/jp/stage.bin --output 04-textos/jp/po
./JUS.CLI jus texts export --bin 04-textos/jp/title.bin --output 04-textos/jp/po