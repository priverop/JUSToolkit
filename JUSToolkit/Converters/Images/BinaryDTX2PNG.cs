using System;
using System.Collections.Generic;
using System.Drawing;
using JUSToolkit.Formats;
using log4net;
using Texim.Media.Image;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

namespace JUSToolkit.Converters.Images
{
    public class BinaryDTX2PNG: IConverter<NodeContainerFormat, List<Bitmap>>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Identify));
        public Node Arm { get; set; }
        public Node Koma { get; set; }
        public Node Komashape { get; set; }
        private readonly byte KOMA_ENTRY_SIZE = 12;
        private readonly int KOMA_NAME_TABLE_OFFSET = 0x9E780;

        public List<Bitmap> Convert(NodeContainerFormat source)
        {

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            List<Bitmap> output = new List<Bitmap>();

            DataReader komaReader = new DataReader(Koma.Stream);

            int komaEntryNumber = (int) (komaReader.Stream.Length / KOMA_ENTRY_SIZE);

            for (int i = 0; i < komaEntryNumber; i++) 
            {
                byte[] entry = komaReader.ReadBytes(KOMA_ENTRY_SIZE);

                // DTX NAME FROM ARM9
                byte letterKomaName = entry[04];
                byte numberKomaName = entry[05];
                byte[] dtxNameBuffer = new byte[2];

                Arm.Stream.RunInPosition(
                () => {
                    dtxNameBuffer[0] = Arm.Stream.ReadByte();
                    dtxNameBuffer[1] = Arm.Stream.ReadByte(); 
                    },
                KOMA_NAME_TABLE_OFFSET);

                string dtxName = System.Text.Encoding.ASCII.GetString(dtxNameBuffer);

                dtxName += numberKomaName;

                log.Debug("dtxName:" + dtxName);

                // DTX SHAPE
                byte indexGroupKshape = entry[08];
                byte indexElementKshape = entry[09];

                DataReader komaShapeReader = new DataReader(Komashape.Stream);

                long komaShapeOffset = 0;

                komaShapeReader.Stream.RunInPosition(
                    () => komaShapeOffset = (komaShapeReader.ReadInt32() + indexElementKshape) * 0x18,
                (indexGroupKshape * 4));

                // DTX File
                Node dtx = Navigator.SearchFile<Node>(source.Root, dtxName);
                // *** quizá haya que concatenar con el directory

                DataReader dtxReader = new DataReader(dtx.Stream);

                int magicid = dtxReader.ReadInt32();
                byte type = dtxReader.ReadByte();
                byte type_alt = dtxReader.ReadByte();
                short totalFramesNumber = dtxReader.ReadInt16();
                short digPointer = dtxReader.ReadInt16();
                short uknown = dtxReader.ReadInt16();
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

                DIG dig = bfDIG.ConvertWith<Binary2DIG, BinaryFormat, DIG>();

                // Iterate KomaShape
                PixelArray extractedDTX = new PixelArray
                {
                    Width = dig.Width,
                    Height = dig.Height,
                };

                Palette palette = dig.Palette;

                komaShapeReader.Stream.Position = komaShapeOffset;
                byte[] raw = new byte[dig.Pixels.GetData().LongLength]; // ***
                int x = 0;
                int y = 0;

                while (komaShapeReader.Stream.Position < dig.Pixels.GetData().LongLength)
                {
                    if (komaShapeReader.ReadByte() > 00)
                    {
                        double position = Math.Ceiling((double)(y / 192)) + x;
                        raw[(int)position] = dig.Pixels.GetData()[(int)position];
                    }
                    x += 48;
                    if (x > 192)
                    {
                        x = 0;
                        y += 48;
                    }
                }

                // Generate new file
                extractedDTX.SetData(raw, PixelEncoding.HorizontalTiles, ColorFormat.Indexed_4bpp);

                var img = extractedDTX.CreateBitmap(palette, 0);

                // Add to container
                output.Add(img);

            }

            return output;
        }
    }
}
