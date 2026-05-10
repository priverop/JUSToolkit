# JUSToolkit: How to use

`./JUS.CLI jus [format_type] [feature] [args]`

To get the arguments of a feature you can use:
`./JUS.CLI jus [format_type] [feature] -h`

**Example:** `./JUS.CLI jus containers export-alar3 --container test.aar --output myDirectory`

---

## Format Types

**Containers:** _ALAR_. We can export every file from a container, or import a 
folder of files into a container. The import only replaces existing files, it 
doesn't add news. It autodetects the format (ALAR2 or ALAR3).

**Graphics:** _DSIG_, _ALMT_, _DTX_, and komas (_DTX_ + _koma.bin_ + _kshape.bin_ + _koma.aar_). 
Export and import graphics. For the _DTX_ type 03 TX, we can export a .yaml file 
with the segments metadata, to modify them in the importing. We can also export 
the base image of the _DTX_ files. For komas, we can export a single one, or all 
of them in batch. We have a `merge-dig` command for importing comics, where a 
single `.dig` file can have multiple `.atm`.

**Texts:** _.bin_. Export text files into .po, or import .po into text files. We 
can export/import a single file, or folder with files/.po. Deck, and jQuiz are 
big .bin split into multiple .po, that's why they have their specific commands.

**Batch:** These commands mix commands and graphics exports. We can export/import 
graphics from .aar directly. Here we also have generic util commands.

**Game:** These commands import graphics, .po files, or the modified font, directly 
to the game using Ekona. Generating a modified ROM.
