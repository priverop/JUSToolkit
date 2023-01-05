#/bin/bash

NEW_FORMAT=$1
OLD_FORMAT="Komatxt"

if [ $# -ne 1 ]; then
    echo "We need an argument."
    exit 1
fi

cd "/Users//Dev/JUSToolkit/src/JUS.Tool/Texts"
cd "Formats"
cp "${OLD_FORMAT}.cs" "${NEW_FORMAT}.cs"
sed -i "" "s/${OLD_FORMAT}/${NEW_FORMAT}/g" "${NEW_FORMAT}.cs"

cp "${OLD_FORMAT}Entry.cs" "${NEW_FORMAT}Entry.cs"
sed -i "" "s/${OLD_FORMAT}/${NEW_FORMAT}/g" "${NEW_FORMAT}Entry.cs"

cd "../Converters"
cp "${OLD_FORMAT}2Po.cs" "${NEW_FORMAT}2Po.cs"
sed -i "" "s/${OLD_FORMAT}/${NEW_FORMAT}/g" "${NEW_FORMAT}2Po.cs"

cp "Binary2${OLD_FORMAT}.cs" "Binary2${NEW_FORMAT}.cs"
sed -i "" "s/${OLD_FORMAT}/${NEW_FORMAT}/g" "Binary2${NEW_FORMAT}.cs"