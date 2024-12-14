#!/bin/bash
set -e

# In the DEBUGDIR I create a directory (DEMODIR) with a directory for each manga with its .png, .dig and .atm.

WORKDIR='//JUSToolkit/src/JUS.CLI'
DEBUGDIR='./bin/Debug/net8.0'
DEMODIR=18-demo
FULL_DEMODIR=$WORKDIR'/bin/Debug/net8.0/'$DEMODIR
TESTDIR='//JUSToolkit/src/JUS.CLI/bin/Debug/net8.0/18-demo/unpacked'

MANGA='dn'

cd $WORKDIR
# dotnet build
cd $DEBUGDIR

rm -rf ${FULL_DEMODIR}/${MANGA}_output

# Singles
# echo 'Singles'
# ./JUS.CLI jus graphics import-dig --input ${FULL_DEMODIR}/${MANGA}_02.png --insertTransparent --dig ${FULL_DEMODIR}/${MANGA}_02.dig --atm ${FULL_DEMODIR}/${MANGA}_02.atm --output ${FULL_DEMODIR}/${MANGA}_output
# ./JUS.CLI jus graphics import-dig --input ${FULL_DEMODIR}/${MANGA}_04.png --insertTransparent --dig ${FULL_DEMODIR}/${MANGA}_04.dig --atm ${FULL_DEMODIR}/${MANGA}_04.atm --output ${FULL_DEMODIR}/${MANGA}_output
# ./JUS.CLI jus graphics import-dig --input ${FULL_DEMODIR}/${MANGA}_06.png --insertTransparent --dig ${FULL_DEMODIR}/${MANGA}_06.dig --atm ${FULL_DEMODIR}/${MANGA}_06.atm --output ${FULL_DEMODIR}/${MANGA}_output
# ./JUS.CLI jus graphics import-dig --input ${FULL_DEMODIR}/${MANGA}_08.png --insertTransparent --dig ${FULL_DEMODIR}/${MANGA}_08.dig --atm ${FULL_DEMODIR}/${MANGA}_08.atm --output ${FULL_DEMODIR}/${MANGA}_output
# ./JUS.CLI jus graphics import-dig --input ${FULL_DEMODIR}/${MANGA}_10.png --insertTransparent --dig ${FULL_DEMODIR}/${MANGA}_10.dig --atm ${FULL_DEMODIR}/${MANGA}_10.atm --output ${FULL_DEMODIR}/${MANGA}_output
# ./JUS.CLI jus graphics import-dig --input ${FULL_DEMODIR}/${MANGA}_11.png --insertTransparent --dig ${FULL_DEMODIR}/${MANGA}_11.dig --atm ${FULL_DEMODIR}/${MANGA}_11.atm --output ${FULL_DEMODIR}/${MANGA}_output
# ./JUS.CLI jus graphics import-dig --input ${FULL_DEMODIR}/${MANGA}_title.png --insertTransparent --dig ${FULL_DEMODIR}/${MANGA}_title.dig --atm ${FULL_DEMODIR}/${MANGA}_title.atm --output ${FULL_DEMODIR}/${MANGA}_output
# Specials
echo 'Specials'
./JUS.CLI jus graphics merge-dig --input ${FULL_DEMODIR}/${MANGA}_03.png ${FULL_DEMODIR}/${MANGA}_m_00.png ${FULL_DEMODIR}/${MANGA}_n_00.png --insertTransparent --dig ${FULL_DEMODIR}/${MANGA}_03.dig --atm ${FULL_DEMODIR}/${MANGA}_03.atm ${FULL_DEMODIR}/${MANGA}_m_00.atm ${FULL_DEMODIR}/${MANGA}_n_00.atm --output ${FULL_DEMODIR}/${MANGA}_output
# ./JUS.CLI jus graphics merge-dig --input ${FULL_DEMODIR}/${MANGA}_05.png ${FULL_DEMODIR}/${MANGA}_m_01.png ${FULL_DEMODIR}/${MANGA}_n_01.png --insertTransparent --dig ${FULL_DEMODIR}/${MANGA}_05.dig --atm ${FULL_DEMODIR}/${MANGA}_05.atm ${FULL_DEMODIR}/${MANGA}_m_01.atm ${FULL_DEMODIR}/${MANGA}_n_01.atm --output ${FULL_DEMODIR}/${MANGA}_output
# ./JUS.CLI jus graphics merge-dig --input ${FULL_DEMODIR}/${MANGA}_07.png ${FULL_DEMODIR}/${MANGA}_m_02.png ${FULL_DEMODIR}/${MANGA}_n_02.png --insertTransparent --dig ${FULL_DEMODIR}/${MANGA}_07.dig --atm ${FULL_DEMODIR}/${MANGA}_07.atm ${FULL_DEMODIR}/${MANGA}_m_02.atm ${FULL_DEMODIR}/${MANGA}_n_02.atm --output ${FULL_DEMODIR}/${MANGA}_output
# ./JUS.CLI jus graphics merge-dig --input ${FULL_DEMODIR}/${MANGA}_09.png ${FULL_DEMODIR}/${MANGA}_m_03.png ${FULL_DEMODIR}/${MANGA}_n_03.png --insertTransparent --dig ${FULL_DEMODIR}/${MANGA}_09.dig --atm ${FULL_DEMODIR}/${MANGA}_09.atm ${FULL_DEMODIR}/${MANGA}_m_03.atm ${FULL_DEMODIR}/${MANGA}_n_03.atm --output ${FULL_DEMODIR}/${MANGA}_output
# Compress into alar3
echo 'Compress into alar3'
./JUS.CLI jus containers import-alar3 --container ${FULL_DEMODIR}/demo.aar --input ${FULL_DEMODIR}/${MANGA}_output --output ${FULL_DEMODIR}/${MANGA}_output
# Copy the generated .aar to the unpacked game directory
cp ${FULL_DEMODIR}/${MANGA}_output/imported.aar $TESTDIR/data/demo/demo.aar
# Copy the generated .aar to the DEMODIR so that you can use it for the next manga
cp ${FULL_DEMODIR}/${MANGA}_output/imported.aar ${FULL_DEMODIR}/demo.aar
# Generate the build
cd $TESTDIR
./ndstool -c build.nds -9 arm9.bin -7 arm7.bin -y9 y9.bin -y7 y7.bin -d data -y overlay -t banner.bin -h header.bin

/Applications/melonDS.app/Contents/MacOS/melonDS $TESTDIR/build.nds