#/bin/bash

NEW_FORMAT=$1
OLD_FORMAT=$2
DIRECTORY="/Users//Dev/JUSToolkit/src/"

if [ $# -ne 2 ]; then
    echo "We need two arguments. Usage: ./copy_text_format new_format old_format"
    exit 1
fi

OLD_FORMAT_LOWER=`echo "${OLD_FORMAT}"  | awk '{$1=tolower(substr($1,0,1))substr($1,2)}1'`
NEW_FORMAT_LOWER=`echo "${NEW_FORMAT}"  | awk '{$1=tolower(substr($1,0,1))substr($1,2)}1'`

cd "${DIRECTORY}/JUS.Tool/Texts/Formats"
cp "${OLD_FORMAT}.cs" "${NEW_FORMAT}.cs"
sed -i "" "s/${OLD_FORMAT}/${NEW_FORMAT}/g" "${NEW_FORMAT}.cs"
sed -i "" "s/${OLD_FORMAT_LOWER}/${NEW_FORMAT_LOWER}/g" "${NEW_FORMAT}.cs"

cp "${OLD_FORMAT}Entry.cs" "${NEW_FORMAT}Entry.cs"
sed -i "" "s/${OLD_FORMAT}/${NEW_FORMAT}/g" "${NEW_FORMAT}Entry.cs"
sed -i "" "s/${OLD_FORMAT_LOWER}/${NEW_FORMAT_LOWER}/g" "${NEW_FORMAT}Entry.cs"

cd "../Converters"
cp "${OLD_FORMAT}2Po.cs" "${NEW_FORMAT}2Po.cs"
sed -i "" "s/${OLD_FORMAT}/${NEW_FORMAT}/g" "${NEW_FORMAT}2Po.cs"
sed -i "" "s/${OLD_FORMAT_LOWER}/${NEW_FORMAT_LOWER}/g" "${NEW_FORMAT}2Po.cs"

cp "Binary2${OLD_FORMAT}.cs" "Binary2${NEW_FORMAT}.cs"
sed -i "" "s/${OLD_FORMAT}/${NEW_FORMAT}/g" "Binary2${NEW_FORMAT}.cs"
sed -i "" "s/${OLD_FORMAT_LOWER}/${NEW_FORMAT_LOWER}/g" "Binary2${NEW_FORMAT}.cs"

cd "${DIRECTORY}/JUS.Tests/Texts/"
cp "${OLD_FORMAT}FormatTest.cs" "${NEW_FORMAT}FormatTest.cs"
sed -i "" "s/${OLD_FORMAT}/${NEW_FORMAT}/g" "${NEW_FORMAT}FormatTest.cs"
sed -i "" "s/${OLD_FORMAT_LOWER}/${NEW_FORMAT_LOWER}/g" "${NEW_FORMAT}FormatTest.cs"
