#!/bin/bash

UNPACK_PATH="${1:-.}"

if [ ! -d "$UNPACK_PATH" ]; then
    echo "Error: '$UNPACK_PATH' not found"
    exit 1
fi
echo ""
ERROR_FILE="errors.txt"
> "$ERROR_FILE"

find "$UNPACK_PATH" -type f -name "*.aar" | while read -r aar_file; do
    directory=$(dirname "$aar_file")
    
    echo "Processing: $aar_file"
    echo "Output: $directory/aar"

    mkdir -p $directory/aar
    
    /JUSToolkit/src/JUS.CLI/bin/Debug/net8.0/JUS.CLI jus containers export --container "$aar_file" --output "$directory/aar"
    
    if [ $? -eq 0 ]; then
        echo -e "\033[0;32m✓ Exported\033[0m"
        rm "$aar_file"
    else
        echo -e "\033[0;31m✗ Error exporting $aar_file\033[0m"
        echo "$aar_file" >> "$ERROR_FILE"
    fi
    
    echo "---"
done

echo ""
echo "Process completed"
echo "Errors saved in errors.txt"