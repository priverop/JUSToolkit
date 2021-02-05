# JUSToolkit
Romhacking tools for Jump Ultimate Stars! (NDS)
[![GPL v3 License](https://img.shields.io/badge/license-GPL%20V3-blue.svg?style=flat)](http://www.gnu.org/copyleft/gpl.html)

## Features
- Pack and unpack .aar (ALAR2 and ALAR3).
- 

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

## Batch export dig & almt to png. Folder to folder.
`-exportdig dir ruleselect/out ruleselect/out`

## Batch import files (dig & altm with the same name) from comic/import to comic/import/new
`-importdig dir comic/import/new comic/import`

## Unpack files from demo.aar
`-e alar/demo.aar demo`

## Pack files from insertDemo into demo.aar and save it in root (.)
`-i alar/demo.aar . alar/insertDemo`

# External
- DIRECTO ROM Hacking: Triple reto de imágenes: https://www.youtube.com/watch?v=r1Rsx6RRe1U
- DIRECTO Domingos de desensamblador: imágenes de Devil Survivor y JUS y ordenar textos de MetalMax 3: https://www.youtube.com/watch?v=R2h-UEcO_-k
- DIRECTO Predomingos de desensamblador: CLYT de 3DS y el complejo caso de los sprites de JUS: https://www.youtube.com/watch?v=1KT4u_Kvaws 
- [FileFormats by Uknown Hacker](FileFormats.md)
# Credits
Special thanks to (PleoNex)[https://github.com/pleonex] for his help. IntegrationTests of this project are based on [SceneGate Lemon](https://github.com/SceneGate/Lemon).
Thanks to [TraduSquare](https://tradusquare.es) for the inspiration and support.
Thanks to the Jump Ultimate Stars! devs for this amazing game.

___

# WIP WORK

## DTX

Investigación por Pleonex: https://www.youtube.com/watch?v=1KT4u_Kvaws + https://www.youtube.com/watch?v=R2h-UEcO_-k

- DTX tipo 04.

### Flujo

Deck nos da la entrada del koma.bin.

koma.bin -> saca el koma name con el 04 y 05. Lo busca en el ARM9 multiplicando el letters * 4. El number es tal cual.

kshape.bin -> Sacamos la posición inicial de la forma con la fórmula. A partir de ahí leemos byte a byte:

- Si es mayor que 0, pintamos ese tile. Si no, se salta. 
- Se suman 48 a la X. Si es mayor que 192 se resetea a 0 y se suma 48 a la Y.


#### Letters koma name table
0x0209E840
Arm9.bin -> 0x9E780
LKN * 4.

#### Koma.bin

entry ID * 0x0C
Entradas de 12 bytes.

00 - 01 (short) -> image_id (0 - 889)
04 -> letters koma name
05 -> number koma name
08 -> index group kshape
09 -> index element kshape

#### KShape
0x14 bytes cada imagen. 

1 - Cogemos el Index Group Kshape, lo multiplicamos por 4.
2 - Leemos ese número, le sumamos el Index Element KShape.
3 - A la posición 0x40 le sumamos el resultado de (2) multiplicado por 18.

((index group * 4) + index element) + 0x40

#### Deck
entries 16-bits
00 - 16 - ID
03 - 8  - must be 10

### Ejemplo

Koma.bin 0x0 - 0xC: 00 00 01 00 01 00 02 03 00 00 01 30
LKN (0x4) = 01
NKM (0x5) = 00
IGK (0x8) = 00
IEK (0x9) = 00

Arm9.bin 0x9E780 + 01 * 4 = 0x9E784 -> es
es_00

### DTX 
Cada caja tiene tamaño 48x48 (0x30 x 0x30)
06 06 25 00
25 indica dónde empieza ese tile. 25 * 0x20 (32decimal = 8 * 8 / 2bitdepth) = 0x4A0 + 0x44 (posición base).
06 06 ->  ancho y alto. 6 * 8 = 48.

### Implementación

- Bucle kshape.
 - Bucle horizontal tile -> 192.
  - Bucle vertical tile -> 240.
   - Bucle Pixeles horizontales de tile.
    - Bucle Pixeles verticales de tile.

Implementación:

Metemos carpeta con arm9.bin, koma.bin, komashape.bin y todos los .dtx.
Conversor NodeContainerFormat -> NodeContainerFormat
Nuevo NCF.
Guardamos en variables individuales el arm9, koma.bin y komashape.bin.
Iteramos el koma.bin.
Sacamos el nombre del .dtx con el letters koma name y number koma name (mirandolo en el arm9 con la formula)
Sacamos el .dtx del NCF.
Sacamos el komashape con el index group kshape y index element kshape y su formula.
Creamos el nuevo PixelArray con la fórmula.
Sacamos la paleta.
Generamos el nuevo fichero y lo guardamos en el NodeContainerFormat.

### ToDo

- Importar ninoimager al programa
- Leer OAM con ninoimager
- Exportar OAM y ver si funciona
- Pseudocódio de la exportación de Frame
- Implementar extracción en BinaryDTX2PNG

DarkMet hizo algo parecido con los sprites del Megaman.