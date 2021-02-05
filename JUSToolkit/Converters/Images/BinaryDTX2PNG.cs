using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using JUSToolkit.Formats;
using log4net;
using Texim;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Converters.Images
{
    public class BinaryDTX2PNG: IConverter<NodeContainerFormat, NodeContainerFormat>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Identify));
        public Node Arm { get; set; }
        public Node Koma { get; set; }
        public Node Komashape { get; set; }
        public string Directory { get; set; }
        private readonly byte KOMA_ENTRY_SIZE = 12;
        private readonly int KOMA_NAME_TABLE_OFFSET = 0x9E780;

        public NodeContainerFormat Convert(NodeContainerFormat source)
        {

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            NodeContainerFormat output = new NodeContainerFormat();

            DataReader komaReader = new DataReader(Koma.Stream);

            int komaEntryNumber = (int) (komaReader.Stream.Length / KOMA_ENTRY_SIZE);

            for (int i = 0; i < komaEntryNumber; i++) 
            {
                byte[] entry = komaReader.ReadBytes(KOMA_ENTRY_SIZE);

                // DTX NAME FROM ARM9
                byte letterKomaName = entry[04];
                byte numberKomaName = entry[05];

                DataReader armReader = new DataReader(Arm.Stream);
                string dtxName = "";

                armReader.Stream.RunInPosition(
                () => {
                    dtxName = armReader.ReadString();
                    },
                (KOMA_NAME_TABLE_OFFSET + letterKomaName * 4 ));

                dtxName += "_" + numberKomaName;
                if (numberKomaName == 0)
                {
                    dtxName += 0;
                }

                log.Debug("dtxName:" + dtxName);

                // DTX SHAPE
                byte indexGroupKshape = entry[08];
                byte indexElementKshape = entry[09];

                DataReader komaShapeReader = new DataReader(Komashape.Stream);

                long komaShapeOffset = 0;

                komaShapeReader.Stream.RunInPosition(
                    () => komaShapeOffset = ((komaShapeReader.ReadInt32() + indexElementKshape) * 0x18) + 0x40,
                (indexGroupKshape * 4));

                log.Debug("komaShapeOffset:" + komaShapeOffset);

                // DTX File
                Node dtx = Navigator.SearchNode<Node>(source.Root, dtxName + ".dtx");

                if (dtx != null)
                {
                    DataReader dtxReader = new DataReader(dtx.Stream);

                    int magicid = dtxReader.ReadInt32();
                    byte type = dtxReader.ReadByte();
                    byte type_alt = dtxReader.ReadByte();
                    short totalFramesNumber = dtxReader.ReadInt16();
                    short digPointer = dtxReader.ReadInt16();
                    short unknown = dtxReader.ReadInt16();
                    byte[] width = new byte[totalFramesNumber];
                    byte[] height = new byte[totalFramesNumber];
                    short[] frameIndex = new short[totalFramesNumber];
                    for (int j = 0; j < totalFramesNumber; j++)
                    {
                        width[j] = dtxReader.ReadByte();
                        height[j] = dtxReader.ReadByte();
                        frameIndex[j] = dtxReader.ReadInt16();
                    }

                    BinaryFormat bfDIG = new BinaryFormat(dtx.Stream, (long)digPointer, (dtx.Stream.Length - (long)digPointer));

                    DIG dig = (DIG)ConvertFormat.With<Binary2DIG>(bfDIG);

                    // Iterate KomaShape

                    komaShapeReader.Stream.Position = komaShapeOffset;
                    // Fichero Dig tiene 08 de ancho y 872 de alto
                    // 08 * 872 / 2 = 3488 bytes
                    byte[] dtxPixels = new byte[192 * 240 / 2]; // *** REVISAR

                    int x = 0;
                    int y = 0;
                    /*
                    log.Debug("==KOMASHAPE==");

                    // Iterate kshape
                    for (int k = 0; k < 0x14; k++)
                    {
                        byte blockDTX = komaShapeReader.ReadByte();
                        log.Debug(k + " - Byte: " + blockDTX);

                        if (blockDTX > 00)
                        {
                            blockDTX -= 1;
                            // Empieza el primer bloque en el dtx
                            long startIndex = frameIndex[blockDTX] * 0x20 + dig.PixelsStart + 32;
                            log.Debug("startIndex:" + startIndex);

                            int blockSize = width[blockDTX] * 8 * height[blockDTX] * 8;

                            for (int l = 0; l < blockSize; l++)
                            {
                                int position = GetIndex(PixelEncoding.Lineal, x, y, 192, 240, new Size(8, 8));

                                dtxPixels[position] = dig.Pixels.GetData()[startIndex + l];
                                log.Debug(l + " - dtxPixels:" + dtxPixels[l]);

                                x += 1;
                                if (x >= 192)
                                {
                                    x = 0;
                                    y += 1;
                                }
                                log.Debug("x: " + x);
                                log.Debug("y: " + y);
                            }

                        }
                        x += 48;
                        if (x >= 192)
                        {
                            x = 0;
                            y += 48;
                        }
                        log.Debug("x: " + x);
                        log.Debug("y: " + y);

                    }
                    log.Debug("====");
                    */

                    // Generate new image
                    PixelArray extractedDTX = new PixelArray
                    {
                        Width = 192,
                        Height = 240,
                    };
                    Palette palette = dig.Palette;

                    extractedDTX.SetData(dtxPixels, PixelEncoding.Lineal, ColorFormat.Indexed_8bpp);

                    var img = extractedDTX.CreateBitmap(palette, 0);
                    var s = new MemoryStream();
                    img.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                    img.Save("test.png");

                    // Add to container
                    var n = new Node(dtxName, new BinaryFormat(DataStreamFactory.FromStream(s)));
                    output.Root.Add(n);
                }                

            }

            return output;
        }


        // By PleoNeX
        public static int GetIndex(PixelEncoding pxEnc, int x, int y, int width, int height, Size tileSize)
        {
            if (pxEnc == PixelEncoding.Lineal)
                return y * width + x;

            int tileLength = tileSize.Width * tileSize.Height;
            int numTilesX = width / tileSize.Width;
            int numTilesY = height / tileSize.Height;

            // Get lineal index
            Point pixelPos = new Point(x % tileSize.Width, y % tileSize.Height); // Pos. pixel in tile
            Point tilePos = new Point(x / tileSize.Width, y / tileSize.Height); // Pos. tile in image
            int index = 0;

            if (pxEnc == PixelEncoding.HorizontalTiles)
                index = tilePos.Y * numTilesX * tileLength + tilePos.X * tileLength;    // Absolute tile pos.
            else if (pxEnc == PixelEncoding.VerticalTiles)
                index = tilePos.X * numTilesY * tileLength + tilePos.Y * tileLength;    // Absolute tile pos.

            index += pixelPos.Y * tileSize.Width + pixelPos.X;    // Add pos. of pixel inside tile

            return index;
        }
    }
}
