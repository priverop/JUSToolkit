#!/bin/bash
set -e
## This is the script I was to import the Demo images into the game.

# In the DEBUGDIR I create a directory (DEMODIR) with a directory for each manga with its .png, .dig and .atm.

WORKDIR='/JUSToolkit/src/JUS.CLI'
DEBUGDIR='./bin/Debug/net6.0'
DEMODIR=/ # This is the home folder where you have your files
TESTDIR='where you have the game unpacked'

MANGA='bb'

cd $WORKDIR
dotnet build
cd $DEBUGDIR

# Singles
echo 'Singles'
./JUS.CLI jus graphics import-dig --input ${DEMODIR}/${MANGA}_input/${MANGA}_02.png --insertTransparent --dig ${DEMODIR}/${MANGA}_input/${MANGA}_02.dig --atm ${DEMODIR}/${MANGA}_input/${MANGA}_02.atm --output ${DEMODIR}/${MANGA}_output
./JUS.CLI jus graphics import-dig --input ${DEMODIR}/${MANGA}_input/${MANGA}_04.png --insertTransparent --dig ${DEMODIR}/${MANGA}_input/${MANGA}_04.dig --atm ${DEMODIR}/${MANGA}_input/${MANGA}_04.atm --output ${DEMODIR}/${MANGA}_output
./JUS.CLI jus graphics import-dig --input ${DEMODIR}/${MANGA}_input/${MANGA}_06.png --insertTransparent --dig ${DEMODIR}/${MANGA}_input/${MANGA}_06.dig --atm ${DEMODIR}/${MANGA}_input/${MANGA}_06.atm --output ${DEMODIR}/${MANGA}_output
./JUS.CLI jus graphics import-dig --input ${DEMODIR}/${MANGA}_input/${MANGA}_08.png --insertTransparent --dig ${DEMODIR}/${MANGA}_input/${MANGA}_08.dig --atm ${DEMODIR}/${MANGA}_input/${MANGA}_08.atm --output ${DEMODIR}/${MANGA}_output
./JUS.CLI jus graphics import-dig --input ${DEMODIR}/${MANGA}_input/${MANGA}_10.png --insertTransparent --dig ${DEMODIR}/${MANGA}_input/${MANGA}_10.dig --atm ${DEMODIR}/${MANGA}_input/${MANGA}_10.atm --output ${DEMODIR}/${MANGA}_output
./JUS.CLI jus graphics import-dig --input ${DEMODIR}/${MANGA}_input/${MANGA}_11.png --insertTransparent --dig ${DEMODIR}/${MANGA}_input/${MANGA}_11.dig --atm ${DEMODIR}/${MANGA}_input/${MANGA}_11.atm --output ${DEMODIR}/${MANGA}_output
./JUS.CLI jus graphics import-dig --input ${DEMODIR}/${MANGA}_input/${MANGA}_title.png --insertTransparent --dig ${DEMODIR}/${MANGA}_input/${MANGA}_title.dig --atm ${DEMODIR}/${MANGA}_input/${MANGA}_title.atm --output ${DEMODIR}/${MANGA}_output
./JUS.CLI jus graphics import-dig --input ${DEMODIR}/${MANGA}_input/${MANGA}_c_01.png --insertTransparent --dig ${DEMODIR}/${MANGA}_input/${MANGA}_c_01.dig --atm ${DEMODIR}/${MANGA}_input/${MANGA}_c_01.atm --output ${DEMODIR}/${MANGA}_output
# Specials
echo 'Specials'
./JUS.CLI jus graphics merge-dig --input ${DEMODIR}/${MANGA}_input/${MANGA}_03.png ${DEMODIR}/${MANGA}_input/${MANGA}_m_00.png ${DEMODIR}/${MANGA}_input/${MANGA}_n_00.png --insertTransparent --dig ${DEMODIR}/${MANGA}_input/${MANGA}_03.dig --atm ${DEMODIR}/${MANGA}_input/${MANGA}_03.atm ${DEMODIR}/${MANGA}_input/${MANGA}_m_00.atm ${DEMODIR}/${MANGA}_input/${MANGA}_n_00.atm --output ${DEMODIR}/${MANGA}_output
./JUS.CLI jus graphics merge-dig --input ${DEMODIR}/${MANGA}_input/${MANGA}_05.png ${DEMODIR}/${MANGA}_input/${MANGA}_m_01.png ${DEMODIR}/${MANGA}_input/${MANGA}_n_01.png --insertTransparent --dig ${DEMODIR}/${MANGA}_input/${MANGA}_05.dig --atm ${DEMODIR}/${MANGA}_input/${MANGA}_05.atm ${DEMODIR}/${MANGA}_input/${MANGA}_m_01.atm ${DEMODIR}/${MANGA}_input/${MANGA}_n_01.atm --output ${DEMODIR}/${MANGA}_output
./JUS.CLI jus graphics merge-dig --input ${DEMODIR}/${MANGA}_input/${MANGA}_07.png ${DEMODIR}/${MANGA}_input/${MANGA}_m_02.png ${DEMODIR}/${MANGA}_input/${MANGA}_n_02.png --insertTransparent --dig ${DEMODIR}/${MANGA}_input/${MANGA}_07.dig --atm ${DEMODIR}/${MANGA}_input/${MANGA}_07.atm ${DEMODIR}/${MANGA}_input/${MANGA}_m_02.atm ${DEMODIR}/${MANGA}_input/${MANGA}_n_02.atm --output ${DEMODIR}/${MANGA}_output
./JUS.CLI jus graphics merge-dig --input ${DEMODIR}/${MANGA}_input/${MANGA}_09.png ${DEMODIR}/${MANGA}_input/${MANGA}_m_03.png ${DEMODIR}/${MANGA}_input/${MANGA}_n_03.png --insertTransparent --dig ${DEMODIR}/${MANGA}_input/${MANGA}_09.dig --atm ${DEMODIR}/${MANGA}_input/${MANGA}_09.atm ${DEMODIR}/${MANGA}_input/${MANGA}_m_03.atm ${DEMODIR}/${MANGA}_input/${MANGA}_n_03.atm --output ${DEMODIR}/${MANGA}_output
# Compress into alar3
echo 'Compress into alar3'
./JUS.CLI jus containers import-alar3 --container ${DEMODIR}/demo.aar --input ${DEMODIR}/${MANGA}_output --output ${DEMODIR}/${MANGA}_output
# Copy the generated .aar to the unpacked game directory
cp ${DEMODIR}/${MANGA}_output/imported_${DEMODIR}/demo.aar $TESTDIR/data/demo/demo.aar
# Copy the generated .aar to the DEMODIR so that you can use it for the next manga
cp ${DEMODIR}/${MANGA}_output/imported_${DEMODIR}/demo.aar ${DEMODIR}/demo.aar
# Generate the build
cd $TESTDIR
./ndstool -c build.nds -9 arm9.bin -7 arm7.bin -y9 y9.bin -y7 y7.bin -d data -y overlay -t banner.bin -h header.bin\