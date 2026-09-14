# Ghidra scripts

The `ExportDefinedStrings` is a Java script for Ghidra. It exports into an YAML
file the strings that Ghidra finds in a project.

The script accepts some arguments:

- First: file path to the output YAML document.
- Second: if greater than 0, string length padding (e.g. 8 instead of 4).

To run the script, close your Ghidra instances and run the following command:

```sh
$GHIDRA_INSTALL_DIR/support/analyzeHeadless \
    $PROJECT_FOLDER \
    $PROJECT_NAME \
    -noAnalysis \
    -scriptPath <repo>/scripts/ghidra \
    -process $PROJECT_IMPORTED_PROGRAM \
    -postScript ExportDefinedStrings <repo>/Resources/program_texts.yaml 4
```

- `$GHIDRA_INSTALL_DIR` is the directory to your Ghidra installation. For
  instance: `$HOME/programs/ghidra_10.3.2_PUBLIC`.
- `$PROJECT_FOLDER` is the directory containing your Ghidra project. That is,
  the folder that has a file `$PROJECT_NAME.gpr` and a folder
  `$PROJECT_NAME.rep`.
- `$PROJECT_NAME` is the name of the project to open from `$PROJECT_FOLDER`.
  That is, the name of the file `$PROJECT_NAME.gpr`.
- `$PROJECT_IMPORTED_PROGRAM` is the name of the file you imported in the Ghidra
  project and analyzed. Usually something like `arm9`. Note that if you added
  files to the _program_ (like overlays) they will be analyzed together. But if
  you imported multiple files to the project (like `arm7`), you will need to run
  the script twice. You don't need the original file.
- `-scriptPath` points to the folder containing the Java script.
- `-postScript` is the script to run without _.java_ and their arguments.
- `-noAnalysis` skips the initial analysis as it usually already performed but
  if it's the first time you can omit this argument.

On Windows use `analyzeHeadless.bat` instead of `analyzeHeadless`.
