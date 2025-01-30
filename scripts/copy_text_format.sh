#!/usr/bin/env bash

# Script to copy and rename text format files with proper casing
# Usage: ./copy_text_format.sh new_format old_format

set -e  # Exit on error

# Error handling function
error() {
    echo "Error: $1" >&2
    exit 1
}

if [ $# -ne 2 ]; then
    error "We need two arguments. Usage: ./copy_text_format.sh new_format old_format"
fi

NEW_FORMAT=$1
OLD_FORMAT=$2

# Get the script's directory and set relative paths
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BASE_DIR="$(dirname "$SCRIPT_DIR")"
SRC_DIR="$BASE_DIR/src"

# Convert format names to proper casing
OLD_FORMAT_LOWER=$(echo "${OLD_FORMAT}" | awk '{print tolower(substr($1,1,1))substr($1,2)}')
NEW_FORMAT_LOWER=$(echo "${NEW_FORMAT}" | awk '{print tolower(substr($1,1,1))substr($1,2)}')

# Process files in Formats directory
FORMATS_DIR="$SRC_DIR/JUS.Tool/Texts/Formats"
[ -d "$FORMATS_DIR" ] || error "Formats directory not found: $FORMATS_DIR"
cd "$FORMATS_DIR" || error "Failed to change to Formats directory"

# Copy and modify format files
for FILE_PAIR in \
    "${OLD_FORMAT}.cs:${NEW_FORMAT}.cs" \
    "${OLD_FORMAT}Entry.cs:${NEW_FORMAT}Entry.cs"
do
    OLD_FILE="${FILE_PAIR%%:*}"
    NEW_FILE="${FILE_PAIR##*:}"
    [ -f "$OLD_FILE" ] || error "Source file not found: $OLD_FILE"
    cp "$OLD_FILE" "$NEW_FILE" || error "Failed to copy $OLD_FILE to $NEW_FILE"
    sed -i "s/${OLD_FORMAT}/${NEW_FORMAT}/g" "$NEW_FILE"
    sed -i "s/${OLD_FORMAT_LOWER}/${NEW_FORMAT_LOWER}/g" "$NEW_FILE"
done

# Process files in Converters directory
CONVERTERS_DIR="../Converters"
[ -d "$CONVERTERS_DIR" ] || error "Converters directory not found: $CONVERTERS_DIR"
cd "$CONVERTERS_DIR" || error "Failed to change to Converters directory"

# Copy and modify converter files
for FILE_PAIR in \
    "${OLD_FORMAT}2Po.cs:${NEW_FORMAT}2Po.cs" \
    "Binary2${OLD_FORMAT}.cs:Binary2${NEW_FORMAT}.cs"
do
    OLD_FILE="${FILE_PAIR%%:*}"
    NEW_FILE="${FILE_PAIR##*:}"
    [ -f "$OLD_FILE" ] || error "Source file not found: $OLD_FILE"
    cp "$OLD_FILE" "$NEW_FILE" || error "Failed to copy $OLD_FILE to $NEW_FILE"
    sed -i "s/${OLD_FORMAT}/${NEW_FORMAT}/g" "$NEW_FILE"
    sed -i "s/${OLD_FORMAT_LOWER}/${NEW_FORMAT_LOWER}/g" "$NEW_FILE"
done

# Process files in Tests directory
TESTS_DIR="$SRC_DIR/JUS.Tests/Texts"
[ -d "$TESTS_DIR" ] || error "Tests directory not found: $TESTS_DIR"
cd "$TESTS_DIR" || error "Failed to change to Tests directory"

# Copy and modify test files
TEST_OLD_FILE="${OLD_FORMAT}FormatTest.cs"
TEST_NEW_FILE="${NEW_FORMAT}FormatTest.cs"
[ -f "$TEST_OLD_FILE" ] || error "Source test file not found: $TEST_OLD_FILE"
cp "$TEST_OLD_FILE" "$TEST_NEW_FILE" || error "Failed to copy $TEST_OLD_FILE to $TEST_NEW_FILE"
sed -i "s/${OLD_FORMAT}/${NEW_FORMAT}/g" "$TEST_NEW_FILE"
sed -i "s/${OLD_FORMAT_LOWER}/${NEW_FORMAT_LOWER}/g" "$TEST_NEW_FILE"

echo "Successfully created and modified all files for format: $NEW_FORMAT"
