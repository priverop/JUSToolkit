# JUSToolkit
Romhacking tools for Jump Ultimate Stars! (NDS)
[![GPL v3 License](https://img.shields.io/badge/license-GPL%20V3-blue.svg?style=flat)](http://www.gnu.org/copyleft/gpl.html)

## Features

Done ✔️
To test / with issues ⚠️
Not done 🛑

- Extract and reinsert texts (.bin) ✔️
- Pack .aar ALAR3 ⚠️
- Unpack .aar ALAR3 ✔️
- Pack .aar ALAR2 🛑
- Unpack .aar ALAR2 🛑
- Extract ALMT + DIG ✔️
- Import ALMT + DIG ⚠️
- Extract DTX 🛑 Half way there
- Import DTX 🛑

## Tinke
There is a plugin with this tools in [Tinke by PleoNex](https://github.com/pleonex/tinke).

## Stack
- C#
- [YARHL by PleoNex](https://github.com/SceneGate/Yarhl)
- Log4Net

# How To Use

- JUSToolkit.exe `-e <fileToExtract> <dirToSave>`
- JUSToolkit.exe `-i <inputFileName> <dirToSave> <fileToInsert>`
- JUSToolkit.exe `-importdig dir <dirToSave> <dirWithFilesToInsert>`
- JUSToolkit.exe `-exportdig dir <dirToSave> <dirWithFilesToInsert>`
- JUSToolkit.exe `-exportdtx dir <dirToSave> <dirWithFilesToInsert>`

## Batch export dig & almt to png. Folder to folder.
`-exportdig dir ruleselect/out ruleselect/out`

## Batch import files (dig & altm with the same name) from comic/import to comic/import/new
`-importdig dir comic/import/new comic/import`

## Unpack files from demo.aar
`-e alar/demo.aar demo`

## Pack files from insertDemo into demo.aar and save it in root (.)
`-i alar/demo.aar . alar/insertDemo`

# Research
- [FileFormats by Uknown Hacker](FileFormats.md)
- [DTX Research by PleoNex](dtx-research.md)
- DIRECTO ROM Hacking: Triple reto de imágenes: https://www.youtube.com/watch?v=r1Rsx6RRe1U
- DIRECTO Domingos de desensamblador: imágenes de Devil Survivor y JUS y ordenar textos de MetalMax 3: https://www.youtube.com/watch?v=R2h-UEcO_-k
- DIRECTO Predomingos de desensamblador: CLYT de 3DS y el complejo caso de los sprites de JUS: https://www.youtube.com/watch?v=1KT4u_Kvaws 

# Credits
Special thanks to [PleoNex](https://github.com/pleonex) for his help. IntegrationTests of this project are based on [SceneGate Lemon](https://github.com/SceneGate/Lemon).
Thanks to [TraduSquare](https://tradusquare.es) for the inspiration and support.
Thanks to the Jump Ultimate Stars! devs for this amazing game.
